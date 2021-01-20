#if false
using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Brimborium.Benchmark {
    [SimpleJob]
    public class BenchmarkOverheadAsync {
        //[Params(1, 2, 512, 1024, 2048, 4096)]
        [Params (22)]
        public int N;

        [Benchmark (Baseline = true)]
        public void Sync () {
            var hugo = new Hugo (0);
            for (int i = 0; i < N; i++) {
                hugo.Sync ();
            }
        }

        [Benchmark]
        public async Task WithAsync () {
            var hugo = new Hugo (0);
            for (int i = 0; i < N; i++) {
                await hugo.WithAsync ();
            }
        }

        [Benchmark]
        public async Task WithAsync2 () {
            var hugo = new Hugo (0);
            for (int i = 0; i < N; i++) {
                var t = hugo.WithAsync ();
                // if (t.IsCompletedSuccessfully){
                //     s += t.GetAwaiter().GetResult();
                // } else {
                //     s += await t;
                // }
                if (t.IsCompleted) {
                    //
                } else {
                    await t;
                }
            }
        }

        /*


        |    Method |    N |           Mean |        Error |       StdDev | Ratio | RatioSD |
        |---------- |----- |---------------:|-------------:|-------------:|------:|--------:|
        |      Sync |    1 |       888.5 ns |     17.35 ns |     26.50 ns |  1.00 |    0.00 |
        | WithAsync |    1 |       971.0 ns |     19.46 ns |     30.86 ns |  1.10 |    0.04 |
        |           |      |                |              |              |       |         |
        |      Sync |    2 |     1,127.0 ns |     22.39 ns |     40.94 ns |  1.00 |    0.00 |
        | WithAsync |    2 |     1,271.6 ns |     25.19 ns |     30.94 ns |  1.15 |    0.05 |
        |           |      |                |              |              |       |         |
        |      Sync |  512 |   104,020.1 ns |  2,033.88 ns |  2,176.23 ns |  1.00 |    0.00 |
        | WithAsync |  512 |   123,016.4 ns |  2,204.86 ns |  2,062.43 ns |  1.18 |    0.03 |
        |           |      |                |              |              |       |         |
        |      Sync | 1024 |   213,810.7 ns |  4,128.30 ns |  6,051.21 ns |  1.00 |    0.00 |
        | WithAsync | 1024 |   262,442.5 ns |  5,234.57 ns | 14,504.97 ns |  1.28 |    0.08 |
        |           |      |                |              |              |       |         |
        |      Sync | 2048 |   418,438.8 ns |  8,162.55 ns |  7,235.89 ns |  1.00 |    0.00 |
        | WithAsync | 2048 |   502,674.2 ns |  8,287.25 ns |  7,751.90 ns |  1.20 |    0.03 |
        |           |      |                |              |              |       |         |
        |      Sync | 4096 |   838,276.9 ns | 16,155.27 ns | 19,840.13 ns |  1.00 |    0.00 |
        | WithAsync | 4096 | 1,028,825.4 ns | 19,956.22 ns | 21,352.93 ns |  1.23 |    0.04 |
        */
/*
|     Method |  N |     Mean |    Error |   StdDev |   Median | Ratio | RatioSD |
|----------- |--- |---------:|---------:|---------:|---------:|------:|--------:|
|       Sync | 22 | 29.72 ms | 0.440 ms | 0.390 ms | 29.70 ms |  1.00 |    0.00 |
|  WithAsync | 22 | 31.23 ms | 0.622 ms | 1.199 ms | 30.90 ms |  1.04 |    0.04 |
| WithAsync2 | 22 | 35.07 ms | 1.551 ms | 4.573 ms | 33.66 ms |  1.21 |    0.10 |
*/
        internal class Hugo {
            private byte[] a;
            private byte[] b;
            private int loop;
            private string fileName;
            public Hugo (int l, [System.Runtime.CompilerServices.CallerFilePath] string fn = null) {
                a = new byte[64 * 1024 * 1024];
                b = new byte[64 * 1024 * 1024];
                loop = l;
                fileName = fn;
                fileName = @"\\localhost\G\temp\SDHO.7z";
            }

            public async Task WithAsync () {
                if ((loop++ % 11) == 0) {
                    (await System.IO.File.ReadAllBytesAsync (fileName)).AsSpan ().CopyTo (a.AsSpan ());
                } else {
                    a.AsSpan ((loop % 11) * 1024, 1024).CopyTo (b.AsSpan ((loop % 11) * 1024));
                }
            }

            public void Sync () {
                if ((loop++ % 11) == 0) {
                    //(System.IO.File.ReadAllBytesAsync (fileName).GetAwaiter().GetResult()).AsSpan ().CopyTo (a.AsSpan ());
                    System.IO.File.ReadAllBytes (fileName).AsSpan ().CopyTo (a.AsSpan ());
                } else {
                    a.AsSpan ((loop % 11) * 1024, 1024).CopyTo (b.AsSpan ((loop % 11) * 1024));
                }
            }
        }
    }
}
#endif