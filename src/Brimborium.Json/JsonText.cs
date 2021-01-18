using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Json {
    public class JsonText {
        public byte[]? Utf8;
        public char[]? Utf16;
        public JsonText(byte[] buffer, bool lazy) {
            Utf8 = buffer;
            if (!lazy) {
                Utf16 = StringUtility.FromUtf8(buffer);
            }
        }
        public JsonText(char[] buffer, bool lazy) {
            Utf16 = buffer;
            if (!lazy) {
                Utf8 = StringUtility.ToUtf8(buffer);
            }
        }

        public Span<byte> GetSpanUtf8() {
            if (Utf8 is null) {
                Utf8 = StringUtility.ToUtf8(Utf16!);
                return Utf8.AsSpan();
            } else {
                return Utf8.AsSpan();
            }
        }

        public Span<char> GetSpanUtf16() {
            if (Utf16 is null) {
                Utf16 = StringUtility.FromUtf8(Utf8!);
                return Utf16.AsSpan();
            } else {
                return Utf16.AsSpan();
            }
        }
    }
    //public class MemoryPool2 {
    //    public MemoryPool<byte> Utf8Pool { get; }
    //    public MemoryPool<char> Utf16Pool { get; }

    //    public MemoryPool2() {
    //        this.Utf8Pool = MemoryPool<byte>.Shared
    //            .Create(128 * 1024, System.Environment.ProcessorCount);
    //        this.Utf16Pool = MemoryPool<char>.Create(128 * 1024, System.Environment.ProcessorCount);
    //    }


    //    public byte[] RendUtf8(int minimumLength) {
    //        return Utf8Pool.Rent(minimumLength);
    //    }

    //    public char[] RendUtf16(int minimumLength) {
    //        return Utf16Pool.Rent(minimumLength);
    //    }

    //    public void ReturnUtf8(byte[] buffer) {
    //        Utf8Pool.Return(buffer, false);
    //    }

    //    public void ReturnUtf16(char[] buffer) {
    //        Utf16Pool.Return(buffer, false);
    //    }
    //}
    //public class ArrayPool2 {
    //    public ArrayPool<byte> Utf8Pool { get; }
    //    public ArrayPool<char> Utf16Pool { get; }

    //    public BufferPool() {
    //        this.Utf8Pool = ArrayPool<byte>.Create(128 * 1024, System.Environment.ProcessorCount);
    //        this.Utf16Pool = ArrayPool<char>.Create(128 * 1024, System.Environment.ProcessorCount);
    //    }


    //    public byte[] RendUtf8(int minimumLength) {
    //        return Utf8Pool.Rent(minimumLength);
    //    }

    //    public char[] RendUtf16(int minimumLength) {
    //        return Utf16Pool.Rent(minimumLength);
    //    }

    //    public void ReturnUtf8(byte[] buffer) {
    //        Utf8Pool.Return(buffer, false);
    //    }

    //    public void ReturnUtf16(char[] buffer) {
    //        Utf16Pool.Return(buffer, false);
    //    }
    //}
}
