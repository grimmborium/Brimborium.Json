using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;

namespace Brimborium.Json.SourceGenerator {
    public class JsonSourceGenerator : ISourceGenerator {
        public void Initialize(GeneratorInitializationContext context) {
        }

        public void Execute(GeneratorExecutionContext context) {
            if (context.Compilation is CSharpCompilation compilation){
                var cu = SyntaxFactory.CompilationUnit()
                .WithMembers
                (
                    SyntaxFactory.SingletonList<MemberDeclarationSyntax>
                    (
                        SyntaxFactory.NamespaceDeclaration
                        (
                            SyntaxFactory.IdentifierName("NSX"))
                        .WithMembers
                        (
                            SyntaxFactory.SingletonList<MemberDeclarationSyntax>
                            (
                                SyntaxFactory.ClassDeclaration("Foo")))))
                .NormalizeWhitespace();
                Microsoft.CodeAnalysis.Text.SourceText sourceText = cu.GetText(System.Text.Encoding.UTF8);
                context.AddSource("Magic.generated.cs", sourceText);
            }
        }

    }
}
