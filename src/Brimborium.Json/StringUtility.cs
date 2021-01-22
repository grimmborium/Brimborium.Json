using System;

namespace Brimborium.Json {
    public class StringUtility {
        public static readonly System.Text.UTF8Encoding Utf8NoBOM = new System.Text.UTF8Encoding(false, false);
        public static readonly System.Text.UnicodeEncoding UnicodeEncodingNoBom = new System.Text.UnicodeEncoding(false, false);
        public static readonly System.Text.UnicodeEncoding UnicodeEncodingBom = new System.Text.UnicodeEncoding(false, true);

        public static char[] FromUtf8(byte[] buffer) {
            return Utf8NoBOM.GetChars(buffer);
        }
        public static int ConvertFromUtf8(Span<byte> src, Span<char> dst) {
#if NETCOREAPP3_1 || NET5_0
            return Utf8NoBOM.GetChars(src, dst);
#else
            unsafe { 
                return Utf8NoBOM.GetChars(
                    (byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref src.GetPinnableReference()), src.Length,
                    (char*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref dst.GetPinnableReference()), dst.Length);
            }
            
#endif
        }

        public static Memory<char> FromUtf8(Span<byte> buffer) {
#if NETCOREAPP3_1 || NET5_0
            var count = Utf8NoBOM.GetCharCount(buffer);
            var result = new Memory<char>(new char[count]);
            Utf8NoBOM.GetChars(buffer, result.Span!);
            return result;
#else
            return new Memory<char>(Utf8NoBOM.GetChars(buffer.ToArray()));
#endif
        }

        //public static char[] ToUtf16(byte[] buffer, BufferPool? bufferPool) {
        //    if (bufferPool is null) {
        //        return Utf8NoBOM.GetChars(buffer);
        //    } else {
        //        var count = Utf8NoBOM.GetCharCount(buffer);
        //        var result = bufferPool.RendUtf8(count);
        //        result.
        //    }
        //}

        public static byte[] ToUtf8(char[] buffer) {
            return Utf8NoBOM.GetBytes(buffer);
        }

        public static Memory<byte> ToUtf8(ReadOnlySpan<char> buffer) {
#if NETCOREAPP3_1 || NET5_0
            var count = Utf8NoBOM.GetByteCount(buffer);
            var result = new Memory<byte>(new byte[count]);
            Utf8NoBOM.GetBytes(buffer, result.Span!);
            return result;
#else
            return Utf8NoBOM.GetBytes(buffer.ToArray());
#endif
        }

        public static byte[] ToUtf16(char[] buffer) {
            return UnicodeEncodingNoBom.GetBytes(buffer);
        }

        public static Memory<byte> ToUtf16(ReadOnlySpan<char> buffer) {
#if NETCOREAPP3_1 || NET5_0
            var count = UnicodeEncodingNoBom.GetByteCount(buffer);
            var result = new Memory<byte>(new byte[count]);
            UnicodeEncodingNoBom.GetBytes(buffer, result.Span!);
            return result;
#else
            return UnicodeEncodingNoBom.GetBytes(buffer.ToArray());
#endif
        }

        public static int ToUtf16(Span<char> buffer, Span<byte> target) {
#if NETCOREAPP3_1 || NET5_0
            var count = UnicodeEncodingNoBom.GetMaxByteCount(buffer.Length);
            if (count < target.Length) {
                UnicodeEncodingBom.GetBytes(buffer, target);
                return buffer.Length;
            }
            count = UnicodeEncodingNoBom.GetByteCount(buffer);
            if (count < target.Length) {
                UnicodeEncodingBom.GetBytes(buffer, target);
                return buffer.Length;
            }
            return 0;
#else
            throw new NotSupportedException();
            //var arrayBuffer=buffer.ToArray();
            //var count = UnicodeEncodingNoBom.GetMaxByteCount(buffer.Length);
            //if (count < target.Length) {
            //    UnicodeEncodingBom.GetBytes(buffer, target);
            //    return buffer.Length;
            //}
            //count = UnicodeEncodingNoBom.GetByteCount(buffer);
            //if (count < target.Length) {
            //    UnicodeEncodingBom.GetBytes(buffer, target);
            //    return buffer.Length;
            //}
            //return 0;
            
#endif
        }

        //public static byte[]? ToUtf8(char[] buffer, BufferPool? bufferPool) {
        //    throw new NotImplementedException();
        //}
    }
}