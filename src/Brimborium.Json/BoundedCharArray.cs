using System;

namespace Brimborium.Json {
    public struct BoundedCharArray {
        private static char[]? _EmptyArray;
        public static char[] EmptyArray => (_EmptyArray ??= new char[0]);

        public char[] Buffer;
        public int Offset;
        public int Length;
        public bool ReturnBuffer;

        public int Free => Length - Offset;
        public char Current => Buffer[Offset];

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

        public BoundedCharArray(char[] buffer) {
            Buffer = buffer;
            Offset = 0;
            Length = Buffer.Length;
            ReturnBuffer = false;
        }

        public static BoundedCharArray Rent(int minimumLength) {
            var buffer = System.Buffers.ArrayPool<char>.Shared.Rent(minimumLength);
            return new BoundedCharArray(buffer, 0, buffer.Length, true);
        }

        public static BoundedCharArray Empty() {
            return new BoundedCharArray(EmptyArray, 0, 0, false);
        }

        public void Return() {
            if (ReturnBuffer) {
                System.Buffers.ArrayPool<char>.Shared.Return(this.Buffer);
                ReturnBuffer = false;
            }
            this.Buffer = Array.Empty<char>();
            this.Offset = 0;
            this.Length = 0;
        }

        public Span<char> GetSpan(int offsetUtf8, int lengthUtf8) => new Span<char>(Buffer, offsetUtf8, lengthUtf8);

        public Span<char> GetFreeSpan() => new Span<char>(Buffer, Offset, Free);

        public Span<char> GetUsedSpan() => new Span<char>(Buffer, 0, Offset);
    }
}
