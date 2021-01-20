using System;

namespace Brimborium.Json {
    public struct BoundedByteArray{
        public byte[] Buffer;
        public int Offset;
        public int Length;
        public bool ReturnBuffer;
        public int Free => Length - Offset;

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
        
        public void Return() {
            if (ReturnBuffer) {
                System.Buffers.ArrayPool<byte>.Shared.Return(this.Buffer);
                this.ReturnBuffer = false;
            }
            this.Buffer = Array.Empty<byte>();
            this.Offset = 0;
            this.Length = 0;
        }

        public Span<byte> GetFreeSpan() => new Span<byte>(Buffer, Offset, Free);
        public Span<byte> GetUsedSpan() => new Span<byte>(Buffer, 0, Offset);
    }
}
