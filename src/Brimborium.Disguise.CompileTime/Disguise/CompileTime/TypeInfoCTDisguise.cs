using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace Brimborium.Disguise.CompileTime {
    public class TypeInfoCTDisguise : TypeInfoDisguise {
        private readonly TypeInfo _TypeInfo;

        public TypeInfoCTDisguise(TypeInfo typeInfo, ContextDisguise? contextDisguise) 
            :base(contextDisguise) {
            this._TypeInfo = typeInfo;
        }

        public override string Name => throw new System.NotImplementedException();

        public override string Namespace => throw new System.NotImplementedException();

        public override AssemblyDisguise Assembly => throw new System.NotImplementedException();
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