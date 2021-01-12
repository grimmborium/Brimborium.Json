using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis;

using System.Threading;

namespace Brimborium.Disguise.CompileTime {
    public sealed class TypeCTDisguise : TypeDisguise {
        // public static TypeIdentity GetTypeIdentity(ITypeSymbol typeSymbol) => new TypeIdentity(typeSymbol.Name, "");
        // public static TypeIdentity GetTypeIdentity(ITypeSymbol typeSymbol) => new TypeIdentity(typeSymbol.Name, typeSymbol.ContainingNamespace, typeSymbol.ContainingAssembly?.ContainingAssembly?.Identity?.Name);
        // INamedTypeSymbol
        public static TypeIdentity GetTypeIdentity(ITypeSymbol typeSymbol) => new TypeIdentity(
                typeSymbol.ToDisplayString(),
                typeSymbol.Name,
                typeSymbol.ContainingType?.ToDisplayString(),
                typeSymbol.ContainingNamespace?.ToDisplayString(),
                null
                //typeSymbol.ContainingAssembly?.ContainingAssembly?.Identity?.Name
            );
        public static TypeIdentity GetTypeIdentity(ITypeSymbol typeSymbol, AssemblyIdentity assemblyIdentity) => new TypeIdentity(
                typeSymbol.ToDisplayString(),
                typeSymbol.Name,
                typeSymbol.ContainingType?.ToDisplayString(),
                typeSymbol.ContainingNamespace?.ToDisplayString(),
                assemblyIdentity
            //typeSymbol.ContainingAssembly?.ContainingAssembly?.Identity?.Name
            );

        //private readonly TypeInfo _TypeInfo;
        public readonly INamedTypeSymbol NamedTypeSymbol;
        private readonly AssemblyDisguise _Assembly;
        private readonly TypeIdentity _Identity;

        public TypeCTDisguise(INamedTypeSymbol typeSymbol, AssemblyDisguise assembly, ContextDisguise? contextDisguise)
            : base(contextDisguise) {
            this.NamedTypeSymbol = typeSymbol;
            this._Assembly = assembly;
            this._Identity = GetTypeIdentity(typeSymbol, assembly.Identity);
            //
            this.PostInit();
        }

        protected override void ContextDisguiseUpdated() {
            if (this.ContextDisguise is object) {
                this.ContextDisguise.Types[this.Identity] = this;
            }
        }
        public override TypeIdentity Identity => this._Identity;
        public override string Name => this.NamedTypeSymbol.Name;

        public override string Namespace => (this.NamedTypeSymbol.ContainingNamespace is null)
            ? string.Empty
            : this.NamedTypeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        public override AssemblyDisguise Assembly => this._Assembly;
        //(this.TypeSymbol.ContainingAssembly is null)
        //? string.Empty
        //: this.TypeSymbol.ContainingAssembly.Name;

        public override bool IsArray => base.IsArray;
        public override bool IsClass => this.NamedTypeSymbol.IsGenericType;
        public override bool IsValueType => base.IsValueType;
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