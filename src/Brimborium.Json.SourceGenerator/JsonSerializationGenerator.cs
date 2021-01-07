using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Brimborium.Json.SourceGenerator
{
    [Generator]
    public class JsonSerializationGenerator : ISourceGenerator {

        /*
         https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/
         */
        public JsonSerializationGenerator() {
        }

        public void Initialize(GeneratorInitializationContext context) {
            throw new NotImplementedException();
        }

        public void Execute(GeneratorExecutionContext context) {
            throw new NotImplementedException();
        }

    }
}
