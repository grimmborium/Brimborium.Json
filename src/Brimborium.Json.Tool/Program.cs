using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Json.Tool
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand();
            rootCommand.Add(new Option<int>("--an-int"));
            rootCommand.Add(new Option<string>("--a-string"));

            //rootCommand.Handler = System.CommandLine.Invocation.CommandHandler.Create<int, string>(DoSomething);
            rootCommand.Handler = CommandHandler.Create(async(int anInt, string aString, IConsole console, CancellationToken cancellationToken) => {
                console.Out.WriteLine("Hello World");
                await Task.Delay(1000, cancellationToken);
                console.Out.WriteLine("-fini-");
            });

            return await rootCommand.InvokeAsync(args);
        }

        public static int DoSomething(int anInt, string aString) {
            /* do something */
            return 0;
        }
        // https://carlos.mendible.com/2017/01/29/net-core-roslyn-and-code-generation/
        // https://github.com/cmendible/dotnetcore.samples/tree/main/roslyn.codegeneration
        // https://itnext.io/no-need-to-wait-for-net-5-to-start-using-code-generation-with-roslyn-f2317b438a6c

    }
}
