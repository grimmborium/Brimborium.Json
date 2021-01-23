using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace Brimborium.Json {
    public struct BoundedByteArray {
        private static byte[]? _EmptyArray;
        public static byte[] EmptyArray => (_EmptyArray ??= new byte[0]);

        // ReadOffset <= FeedOffset <=Buffer.Length
        public byte[] Buffer;
        public int GlobalProtected;
        public int GlobalShift;
        public int ReadOffset;
        public int FeedOffset;
        public bool ReturnBuffer;

        public int ReadLength => FeedOffset - ReadOffset;
        public int FeedLength => Buffer.Length - FeedOffset;

        public BoundedByteArray(
                byte[] buffer,
                int readOffset,
                int feedOffset,
                bool returnBuffer
            ) {
            Buffer = buffer;
            GlobalProtected = -1;
            GlobalShift = 0;
            ReadOffset = readOffset;
            FeedOffset = feedOffset;
            ReturnBuffer = returnBuffer;
        }

        public static BoundedByteArray Rent(int minimumLength)
            => new BoundedByteArray(System.Buffers.ArrayPool<byte>.Shared.Rent(minimumLength), 0, 0, true);

        public static BoundedByteArray Empty()
            => new BoundedByteArray(EmptyArray, 0, 0, false);

        public void Return() {
            if (ReturnBuffer) {
                System.Buffers.ArrayPool<byte>.Shared.Return(this.Buffer);
                this.ReturnBuffer = false;
            }
            this.Buffer = EmptyArray;
            this.ReadOffset = 0;
            this.FeedOffset = 0;
        }

        public Span<byte> GetSpan(int offsetUtf8, int lengthUtf8)
            => new Span<byte>(Buffer, offsetUtf8, lengthUtf8);

        public Span<byte> GetReadSpan()
          => ((FeedOffset - ReadOffset) < 0)
          ? EmptyArray.AsSpan()
          : new Span<byte>(Buffer, ReadOffset, FeedOffset - ReadOffset);

        public Span<byte> GetFeedSpan()
            => new Span<byte>(Buffer, FeedOffset, this.Buffer.Length - FeedOffset);

        public (int lowerOffset, int lowerLength,
                int nextReadOffset, int nextFeedOffset) GetProtectRange() {
            int lowerOffset = ReadOffset;
            if (this.GlobalProtected <= 0) {
                int localProtected = (this.GlobalProtected - this.GlobalShift);
                if (localProtected < lowerOffset) {
                    lowerOffset = localProtected;
                }
            }
            int nextReadOffset = ReadOffset - lowerOffset;
            int nextFeedOffset = FeedOffset - lowerOffset;

            return (
                lowerOffset: lowerOffset,
                lowerLength: nextFeedOffset,
                nextReadOffset: nextReadOffset,
                nextFeedOffset: nextFeedOffset);
        }


        public void EnsureCapacity(int additionalLength, int initialLength) {
            if (this.FeedLength < additionalLength) {
                var range = this.GetProtectRange();

                int requestLength = System.Math.Max(
                    range.nextFeedOffset + additionalLength,
                    initialLength);
                var oldBuffer = this.Buffer;
                var nextBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(requestLength);
                if (range.lowerLength > 0) {
                    this.Buffer.AsSpan(range.lowerOffset, range.lowerLength)
                        .CopyTo(nextBuffer.AsSpan(0, range.lowerLength));
                }
                this.GlobalShift += range.lowerOffset;
                this.Buffer = nextBuffer;
                this.ReadOffset = range.nextReadOffset;
                this.FeedOffset = range.nextFeedOffset;
                if (ReturnBuffer) {
                    System.Buffers.ArrayPool<byte>.Shared.Return(oldBuffer);
                } else {
                    this.ReturnBuffer = true;
                }
            }
        }

        public void AdjustBeforeFeeding(int requested, int initialLength) {
            if (this.ReadOffset >= this.FeedOffset && this.FeedOffset > 0) {
                // so all content is read
                this.GlobalShift += this.ReadOffset;
                this.ReadOffset = 0;
                this.FeedOffset = 0;
            }
            if (this.FeedLength < requested) {
                if ((this.GlobalProtected < 0)
                    && (ReadLength < ReadOffset)
                    && (ReadLength < (Buffer.Length / 2))
                    ) {
                    var readSpan = this.GetReadSpan();
                    readSpan.CopyTo(this.Buffer.AsSpan(0, readSpan.Length));
                    this.GlobalShift += this.ReadOffset;
                    this.ReadOffset = 0;
                } else {
                    EnsureCapacity(requested, initialLength);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AdjustAfterFeeding(int feededCount) {
            this.FeedOffset += feededCount;
        }

        public void AdjustAfterReading(int readLength) {
            var next = this.ReadOffset + readLength;
            if (this.GlobalProtected < 0) {
                if (next < this.FeedOffset) {
                    this.ReadOffset = next;
                } else if (next == this.FeedOffset) {
                    this.GlobalShift += next;
                    this.ReadOffset = 0;
                    this.FeedOffset = 0;
                } else {
                    throw new ArgumentException($"Buffer overrun. ReadLength:{readLength}; ReadOffset:{ReadOffset}; FeedOffset:{FeedOffset};");
                }
            } else {
                if (next > this.FeedOffset) {
                    throw new ArgumentException($"Buffer overrun. ReadLength:{readLength}; ReadOffset:{ReadOffset}; FeedOffset:{FeedOffset};");
                }
            }
        }
    }
}
