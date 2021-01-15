using System;

namespace Brimborium.Json.Internal {
    public struct Utf816Array {
        public readonly byte[] Buffer8;
        public readonly char[] Buffer16;

        public Utf816Array(string text) {
            var size = StringEncoding.UTF8NoBOM.GetByteCount(text);
            Buffer16 = text.ToCharArray();
            Buffer8 = new byte[size];
            StringEncoding.GetBytes(Buffer16, Buffer8);
        }

        public Span<byte> AsSpan8() {
            return new Span<byte>(Buffer8);
        }

        public Memory<byte> AsMemory8() {
            return new Memory<byte>(Buffer8);
        }
        public Span<char> AsSpan16() {
            return new Span<char>(Buffer16);
        }

        public Memory<char> AsMemory16() {
            return new Memory<char>(Buffer16);
        }

        public bool CopyTo(byte[] dest, int offset) {
            var free = dest.Length - offset;
            if (free < this.Buffer8.Length) {
                return false;
            } else {
                this.Buffer8.AsSpan().CopyTo(dest.AsSpan(offset, this.Buffer8.Length));
                return true;
            }
        }

        public bool CopyTo(char[] dest, int offset) {
            var free = dest.Length - offset;
            if (free < this.Buffer16.Length) {
                return false;
            } else {
                this.Buffer16.AsSpan().CopyTo(dest.AsSpan(offset, this.Buffer16.Length));
                return true;
            }
        }
    }
}
