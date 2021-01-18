using System;
using System.ComponentModel;
using System.Text;

namespace Brimborium.Json.Internal
{
    public static class StringEncoding
    {
        public static readonly Encoding UTF8NoBOM = new UTF8Encoding(false);
        public static void GetBytes(char[] source, byte[] dest) {
#if NET5_0 || NETCOREAPP3_1
            StringEncoding.UTF8NoBOM.GetBytes(source.AsSpan(), dest.AsSpan());
#elif NET472 || NETSTANDARD2_0
            unsafe { 
                fixed(char * s = &source[0])
                    fixed(byte * d = &dest[0])
                        StringEncoding.UTF8NoBOM.GetBytes(s, source.Length, d, dest.Length);
            }
#else
#endif
        }
    }
}