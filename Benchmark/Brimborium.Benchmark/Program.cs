namespace Brimborium.Benchmark {
    /*
     * 
     dotnet build -c Release
     dotnet run -c Release --filter *
     dotnet run -c Release --filter * --warmupCount 1 --iterationCount 1
     */
    public class Program {
        public static void Main(string[] args)
            => BenchmarkDotNet.Running.BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
    /*


    |        Method |  N |      Mean |     Error |    StdDev |
    |-------------- |--- |----------:|----------:|----------:|
    |  B_Array_Copy |  4 | 10.419 ns | 0.2384 ns | 0.2341 ns |
    | B_Utf8Array8S |  4 |  5.587 ns | 0.0586 ns | 0.0489 ns |
    |    B_WriteRaw |  4 | 19.220 ns | 0.2203 ns | 0.1840 ns |
    |  B_Array_Copy |  7 | 10.686 ns | 0.1181 ns | 0.0986 ns |
    | B_Utf8Array8S |  7 |  5.602 ns | 0.0718 ns | 0.0637 ns |
    |    B_WriteRaw |  7 | 19.782 ns | 0.2686 ns | 0.2243 ns |
    |  B_Array_Copy |  8 | 10.195 ns | 0.1191 ns | 0.1114 ns |
    | B_Utf8Array8S |  8 |  5.576 ns | 0.0598 ns | 0.0530 ns |
    |    B_WriteRaw |  8 | 19.267 ns | 0.3246 ns | 0.2877 ns |
    |  B_Array_Copy | 15 | 11.305 ns | 0.2581 ns | 0.2761 ns |
    | B_Utf8Array8S | 15 |  6.343 ns | 0.0867 ns | 0.0769 ns |
    |    B_WriteRaw | 15 | 22.706 ns | 0.5020 ns | 0.7962 ns |
    |  B_Array_Copy | 22 | 10.208 ns | 0.1875 ns | 0.1662 ns |
    | B_Utf8Array8S | 22 |  7.011 ns | 0.0783 ns | 0.0694 ns |
    |    B_WriteRaw | 22 | 22.178 ns | 0.3348 ns | 0.2796 ns |
    |  B_Array_Copy | 32 |  9.918 ns | 0.1419 ns | 0.1258 ns |
    | B_Utf8Array8S | 32 |  7.588 ns | 0.0884 ns | 0.0784 ns |
    |    B_WriteRaw | 32 | 25.442 ns | 0.4300 ns | 0.4022 ns |
    |  B_Array_Copy | 45 | 12.742 ns | 0.0835 ns | 0.0740 ns |
    | B_Utf8Array8S | 45 | 10.087 ns | 0.0959 ns | 0.0748 ns |
    |    B_WriteRaw | 45 | 25.211 ns | 0.2220 ns | 0.1968 ns |
    |  B_Array_Copy | 82 | 14.417 ns | 0.1686 ns | 0.1577 ns |
    | B_Utf8Array8S | 82 | 14.439 ns | 0.1176 ns | 0.1043 ns |
    |    B_WriteRaw | 82 | 26.620 ns | 0.2273 ns | 0.2126 ns |


|        Method |    N |       Mean | Error |    StdDev |
|-------------- |----- |-----------:|------:|----------:|
|  B_Array_Copy |   15 |  11.077 ns |    NA | 0.0077 ns |
| B_Utf8Array8S |   15 |   6.207 ns |    NA | 0.0685 ns |
|        B_Span |   15 |   5.574 ns |    NA | 0.0261 ns |
|    B_WriteRaw |   15 | 202.375 ns |    NA | 0.4543 ns |
|  B_Array_Copy |  512 |  30.697 ns |    NA | 0.2020 ns |
| B_Utf8Array8S |  512 |  60.819 ns |    NA | 0.0003 ns |
|        B_Span |  512 |  20.384 ns |    NA | 0.2788 ns |
|    B_WriteRaw |  512 | 211.920 ns |    NA | 1.8963 ns |
|  B_Array_Copy | 2048 |  48.237 ns |    NA | 1.0669 ns |
| B_Utf8Array8S | 2048 |  49.182 ns |    NA | 0.2567 ns |
|        B_Span | 2048 |  49.788 ns |    NA | 0.5715 ns |
|    B_WriteRaw | 2048 | 195.267 ns |    NA | 1.1541 ns |


|       Method |    N |     Mean | Error |   StdDev |
|------------- |----- |---------:|------:|---------:|
| B_Array_Copy |  512 | 31.26 ns |    NA | 0.144 ns |
|       B_Span |  512 | 21.08 ns |    NA | 0.431 ns |
| B_Array_Copy |  680 | 30.01 ns |    NA | 0.216 ns |
|       B_Span |  680 | 17.14 ns |    NA | 0.201 ns |
| B_Array_Copy | 1024 | 34.30 ns |    NA | 0.835 ns |
|       B_Span | 1024 | 24.15 ns |    NA | 0.145 ns |
    // * Hints * 
     */
}
