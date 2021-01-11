using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis;

using System.Threading;

namespace Brimborium.Disguise.CompileTime {
    public class TypeCTDisguise : TypeDisguise {
        public static TypeIdentity GetTypeIdentity(ITypeSymbol typeSymbol) => new TypeIdentity(typeSymbol.Name);
        //private readonly TypeInfo _TypeInfo;
        public readonly ITypeSymbol TypeSymbol;

        public TypeCTDisguise(ITypeSymbol typeSymbol, ContextDisguise? contextDisguise)
            : base(contextDisguise) {
            this.TypeSymbol = typeSymbol;
        }

        public override string Name => this.TypeSymbol.Name;

        public override string Namespace => (this.TypeSymbol.ContainingNamespace is null)
            ? string.Empty
            : this.TypeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        public override AssemblyDisguise Assembly => (this.TypeSymbol.ContainingAssembly is null)
            ? string.Empty
            : this.TypeSymbol.ContainingAssembly.Name;

    }
}

#if false

    //
    // Summary:
    //     Context passed to a source generator when Microsoft.CodeAnalysis.ISourceGenerator.Execute(Microsoft.CodeAnalysis.GeneratorExecutionContext)
    //     is called
    public readonly struct GeneratorExecutionContext {
        //
        // Summary:
        //     Get the current Microsoft.CodeAnalysis.GeneratorExecutionContext.Compilation
        //     at the time of execution.
        //
        // Remarks:
        //     This compilation contains only the user supplied code; other generated code is
        //     not available. As user code can depend on the results of generation, it is possible
        //     that this compilation will contain errors.
        public Compilation Compilation {
            get;
        }

        //
        // Summary:
        //     Get the Microsoft.CodeAnalysis.GeneratorExecutionContext.ParseOptions that will
        //     be used to parse any added sources.
        public ParseOptions ParseOptions {
            get;
        }

        //
        // Summary:
        //     Allows access to options provided by an analyzer config
        public AnalyzerConfigOptionsProvider AnalyzerConfigOptions {
            get;
        }

        //
        // Summary:
        //     If the generator registered an Microsoft.CodeAnalysis.ISyntaxReceiver during
        //     initialization, this will be the instance created for this generation pass.
        public ISyntaxReceiver? SyntaxReceiver {
            [System.Runtime.CompilerServices.NullableContext(2)]
            get;
        }

        //
        // Summary:
        //     A Microsoft.CodeAnalysis.GeneratorExecutionContext.CancellationToken that can
        //     be checked to see if the generation should be cancelled.
        public CancellationToken CancellationToken {
            get;
        }

#endif