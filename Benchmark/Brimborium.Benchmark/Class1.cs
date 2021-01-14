using BenchmarkDotNet.Attributes;

using Brimborium.Json;
using Brimborium.Json.Internal;

using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Benchmark {

    [SimpleJob]
    public class Class1 {
        private Utf816Array utf816Array;
        private byte[] byteD = null!;
        private JsonWriterUtf8 writer = null!;

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

            utf816Array = new Utf816Array("".PadLeft(N, '-'));
            writer = new JsonWriterUtf8(byteD);
        }

        [Benchmark]
        public void B_Array_Copy() {
            Array.Copy(utf816Array.Buffer8, 0, byteD, 0, N);
        }

        //[Benchmark]
        //public void B_Utf8Array8S() {
        //    utf8Array8S.CopyTo(byteD, 0);
        //}

        [Benchmark]
        public void B_Span() {
            utf816Array.AsSpan8().CopyTo(byteD.AsSpan());
        }

        //[Benchmark]
        //public void B_WriteRaw() {
        //    writer.Reset();
        //    writer.WriteRaw(byteD);
        //}
    }
}
