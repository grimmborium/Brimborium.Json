using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Json.Tool {
    class Program {
        static async Task<int> Main(string[] args) {
            var rootCommand = new RootCommand();
            // rootCommand.Add(new Option<int>("--an-int"));
            // rootCommand.Add(new Option<string>("--a-string"));
            // int anInt, string aString

            rootCommand.Add(new Option<string[]>(new string[] { "--solution", "-s" }, "Solution"));
            rootCommand.Add(new Option<string[]>(new string[] { "--project", "-p" }, "Project"));

            //rootCommand.Handler = System.CommandLine.Invocation.CommandHandler.Create<int, string>(DoSomething);
            rootCommand.Handler = CommandHandler.Create(async (string solution, string[] arrProject, IConsole console, CancellationToken cancellationToken) => {
                try {
                    int result = await Generate(solution, arrProject ?? Array.Empty<string>(), console, cancellationToken);
                    return result;
                } catch (System.Exception error) {
                    console.Error.WriteLine(error.ToString());
                    return 1;
                }
            });

            return await rootCommand.InvokeAsync(args);
        }

        public static async Task<int> Generate(string solution, string[] arrProject, IConsole console, CancellationToken cancellationToken) {
            /* do something */
            console.Out.WriteLine("Generate");
            console.Out.WriteLine($"Solution: {solution}");
            foreach (var project in arrProject) {
                console.Out.WriteLine($"Project: {project}");
            }
            await Task.Delay(1000, cancellationToken);
            console.Out.WriteLine("-fini-");
            return 0;
        }
        /*
         * RudolfKurka.StructPacker
         * Testura
         * MappingGenerator
         * github\dotnet\roslyn-sdk\samples\CSharp\RefOutModifier\RefOutModifier.Implementation\ApplicableActionFinder.cs
         * https://github.com/ironcev/awesome-roslyn
         * https://docs.microsoft.com/en-us/archive/msdn-magazine/2018/january/csharp-all-about-span-exploring-a-new-net-mainstay
         https://github.com/trampster/JsonSrcGen
         */
        // https://carlos.mendible.com/2017/01/29/net-core-roslyn-and-code-generation/
        // https://github.com/cmendible/dotnetcore.samples/tree/main/roslyn.codegeneration
        // https://itnext.io/no-need-to-wait-for-net-5-to-start-using-code-generation-with-roslyn-f2317b438a6c

    }
}
