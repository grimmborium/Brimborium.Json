using BenchmarkDotNet.Attributes;

using Brimborium.Json;

using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Benchmark {
    [SimpleJob]
    public class BenchmarkCopy {
        private JsonText jsonText = null!;
        private byte[] byteD = null!;

        private char[] charS = null!;
        private char[] charD = null!;

        //[Params(4, 7, 8, 15, 22, 32, 45, 82)]
        [Params(512, 1024, 2048, 4096)]
        public int N;

        [GlobalSetup]
        public void Setup() {
            byteD = new byte[10240];

            charS = new char[N];
            charD = new char[10240];

            jsonText = new JsonText("".PadLeft(N, '-').ToCharArray(), false);
            //writer = new JsonWriterUtf8(byteD);
        }

        [Benchmark]
        public void B_Array_Copy() {
            Array.Copy(jsonText.Utf8!, 0, byteD, 0, N);
        }

        //[Benchmark]
        //public void B_Utf8Array8S() {
        //    utf8Array8S.CopyTo(byteD, 0);
        //}

        [Benchmark]
        public void B_Span() {
            jsonText.GetSpanUtf8().CopyTo(byteD.AsSpan());
        }

        //[Benchmark]
        //public void B_WriteRaw() {
        //    writer.Reset();
        //    writer.WriteRaw(byteD);
        //}
    }
}
