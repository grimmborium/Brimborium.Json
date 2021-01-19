using System;

namespace Brimborium.Json {
    public struct BoundedCharArray {
        public char[] Buffer;
        public int Offset;
        public int Length;
        public bool ReturnBuffer;
        public int Free => Length - Offset;

        public BoundedCharArray(
            char[] buffer,
            int offset,
            int length,
            bool returnBuffer) {
            Buffer = buffer;
            Offset = offset;
            Length = length;
            ReturnBuffer = returnBuffer;
        }

        public static BoundedCharArray Rent(int minimumLength) {
            var buffer = System.Buffers.ArrayPool<char>.Shared.Rent(minimumLength);
            return new BoundedCharArray(buffer, 0, buffer.Length, true);
        }

        public void Return() {
            if (ReturnBuffer) {
                System.Buffers.ArrayPool<char>.Shared.Return(this.Buffer);
            }
            this.Buffer = Array.Empty<char>();
            this.Offset = 0;
            this.Length = 0;
        }


        public Span<char> GetSpan() => new Span<char>(Buffer, Offset, Length);
    }
}
