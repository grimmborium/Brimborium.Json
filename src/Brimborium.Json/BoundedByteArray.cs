using System;
using System.Runtime.CompilerServices;

namespace Brimborium.Json {
    public struct BoundedByteArray {
        private static byte[]? _EmptyArray;
        public static byte[] EmptyArray => (_EmptyArray ??= new byte[0]);

        // ReadOffset <= FeedOffset <=Buffer.Length
        public byte[] Buffer;
        public int ReadOffset;
        public int FeedOffset;
        public bool ReturnBuffer;

        public int ReadLength => FeedOffset - ReadOffset;
        public int FeedLength => Buffer.Length - FeedOffset;

        public BoundedByteArray(
            byte[] buffer,
            int readOffset,
            int feedOffset,
            bool returnBuffer) {
            Buffer = buffer;
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

        public Span<byte> GetSpan(int offsetUtf8, int lengthUtf8) => new Span<byte>(Buffer, offsetUtf8, lengthUtf8);

        public Span<byte> GetReadSpan()
            => ((FeedOffset - ReadOffset) < 0)
            ? EmptyArray.AsSpan()
            : new Span<byte>(Buffer, ReadOffset, FeedOffset - ReadOffset);
        public Span<byte> GetFeedSpan()
            => new Span<byte>(Buffer, FeedOffset, this.Buffer.Length - FeedOffset);

        public void EnsureCapacity(int minimumLength) {
            if (this.Buffer.Length < minimumLength) {
                var oldBuffer = this.Buffer;
                var nextBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(minimumLength);
                var readSpan = this.GetReadSpan();
                if (readSpan.Length > 0) {
                    readSpan.CopyTo(nextBuffer.AsSpan());
                }
                this.Buffer = nextBuffer;
                this.ReadOffset = 0;
                this.FeedOffset = readSpan.Length;
                if (ReturnBuffer) {
                    System.Buffers.ArrayPool<byte>.Shared.Return(oldBuffer);
                } else {
                    ReturnBuffer = true;
                }
            }
        }

        public void AdjustBeforeFeeding(int requested) {
            if (this.ReadOffset >= this.FeedOffset) {
                // so all content is read
                this.ReadOffset = 0;
                this.FeedOffset = 0;
            }
            if (requested < this.FeedLength) {
                if ((ReadLength < 256) && (ReadOffset>512) && (ReadOffset > (Buffer.Length / 2)) ) {
                    var readSpan = this.GetReadSpan();
                    this.Buffer.AsSpan(0, ReadOffset).CopyTo(readSpan);
                } else {
                    EnsureCapacity(requested + ReadLength);
                }
            }
        }

        public void AdjustAfterFeeding(int feededCount) {
            this.FeedOffset += feededCount;
        }
    }
}
