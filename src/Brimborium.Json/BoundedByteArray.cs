using System;
using System.Runtime.CompilerServices;

namespace Brimborium.Json {
    public struct BoundedByteArray {
        private static byte[]? _EmptyArray;
        public static byte[] EmptyArray => (_EmptyArray ??= new byte[0]);

        public byte[] Buffer;
        public int Offset;
        public int Length;
        public bool ReturnBuffer;

        public int Free => Length - Offset;

        public byte Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Buffer[Offset];
        }

        public BoundedByteArray(
            byte[] buffer,
            int offset,
            int length,
            bool returnBuffer) {
            Buffer = buffer;
            Offset = offset;
            Length = length;
            ReturnBuffer = returnBuffer;
        }

        public static BoundedByteArray Rent(int minimumLength) {
            var buffer = System.Buffers.ArrayPool<byte>.Shared.Rent(minimumLength);
            return new BoundedByteArray(buffer, 0, buffer.Length, true);
        }

        public static BoundedByteArray Empty() {
            return new BoundedByteArray(EmptyArray, 0, 0, false);
        }

        public void Return() {
            if (ReturnBuffer) {
                System.Buffers.ArrayPool<byte>.Shared.Return(this.Buffer);
                this.ReturnBuffer = false;
            }
            this.Buffer = Array.Empty<byte>();
            this.Offset = 0;
            this.Length = 0;
        }

        public Span<byte> GetSpan(int offsetUtf8, int lengthUtf8) => new Span<byte>(Buffer, offsetUtf8, lengthUtf8);

        public Span<byte> GetLeftSpan() => new Span<byte>(Buffer, 0, Offset);

        public Span<byte> GetRightSpan() => new Span<byte>(Buffer, Offset, Free);

        public void EnsureCapacity(int minimumLength) {
            if (this.Buffer.Length < minimumLength) {
                var oldBuffer = this.Buffer;
                var nextBuffer = System.Buffers.ArrayPool<byte>.Shared.Rent(minimumLength);
                oldBuffer.AsSpan().CopyTo(nextBuffer.AsSpan());
                this.Buffer = nextBuffer;
                if (ReturnBuffer) {
                    System.Buffers.ArrayPool<byte>.Shared.Return(oldBuffer);
                } else {
                    ReturnBuffer = true;
                }
            }
        }
    }
}
