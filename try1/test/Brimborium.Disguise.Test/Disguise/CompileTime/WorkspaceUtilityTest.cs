using Brimborium.Disguise.Tool;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
namespace Brimborium.Disguise.CompileTime {
    public class WorkspaceUtilityTest {
        [Fact]
        public async Task WorkspaceUtility_OpenSolution() {
            BuildUtility.InitLocator(null);
            ContextDisguise contextDisguise = new ContextDisguise();
            var sut = WorkspaceCTUtility.Create(null);
            var solution = await sut.OpenSolutionAsync(
                SolutionFolderPath.GetSolutionItemPath(@"sample\SampleApp1.sln"),
                contextDisguise,
                default);
            Assert.NotNull(solution);
            Assert.True(contextDisguise.Assemblies.Count > 2);

            // var findSpecificationDeclarations = new FindSpecificationDeclarations();

            var findTypeDeclarations = new FindTypeDeclarations();
            var cus = await sut.ProcessProjectsAsync(
                (pu) => true,
                (c) => true,
                contextDisguise,
                (ctxt, codeUtility) => {
                    foreach (var typeDeclaration in findTypeDeclarations.Execute(codeUtility.SyntaxTree)) {
                        var semanticModel = codeUtility.SemanticModel;
                        var namedTypeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration);
                        if (namedTypeSymbol is object) {
                            //if (namedTypeSymbol.TypeKind == TypeKind.Class) { }
                            if ((namedTypeSymbol.TypeKind == TypeKind.Class)
                                || (namedTypeSymbol.TypeKind == TypeKind.Struct)
                                || (namedTypeSymbol.TypeKind == TypeKind.Interface)
                            ) {
                                TypeIdentity typeIdentity = TypeCTDisguise.GetTypeIdentity(namedTypeSymbol, codeUtility.ProjectUtility.AssemblyIdentity);
                                var found = (contextDisguise.Types.TryGetValue(typeIdentity, out var typeFound)) ;
                                var typeCTDisguise = new TypeCTDisguise(namedTypeSymbol, codeUtility.ProjectUtility.Assembly, contextDisguise);
                                var baseType = typeCTDisguise.NamedTypeSymbol.BaseType;
                            }
                        }
                    }

                },
                default);
            /*
            
            var w = new FindTypeDeclarations();
            var lstCuClsDecl = cus.SelectMany(
                codeUtility
                    => w.GetResult(codeUtility.SyntaxTree)
                    .Select(classDeclaration => ((codeUtility, classDeclaration)))
                );
            foreach (var cc in lstCuClsDecl) {
                var x = cc.classDeclaration.Identifier.ToString();
                System.Diagnostics.Debug.Assert(x != null);

                var semanticModel = cc.codeUtility.SemanticModel;
                var namedTypeSymbol = semanticModel.GetDeclaredSymbol(cc.classDeclaration);

                var name = namedTypeSymbol.Name;
                var containingNamespace = namedTypeSymbol.ContainingNamespace.Name;


                //var ti = semanticModel.GetTypeInfo(cc.classDeclaration);
                //var si = semanticModel.GetSymbolInfo(cc.classDeclaration);
                //Assert.Equal(SymbolKind.NamedType, si.Symbol.Kind);
            }

            */

            //solution.Workspace.CurrentSolution.GetProjectDependencyGraph()
            //var project = await sut.OpenProjectAsync(SolutionFolderPath.GetSolutionItemPath(@"sample\SampleLibrary1\SampleLibrary1.csproj"), default);
            //Assert.NotNull(project);
            //var assembly = new AssemblyCTDisguise(project, contextDisguise);
            //var projectCompilation = await project.GetCompilationAsync(default);
            //Assert.NotNull(projectCompilation);
            //projectCompilation.GlobalNamespace
            //var syntaxTrees = projectCompilation.SyntaxTrees;
            //var semanticModel = projectCompilation.GetSemanticModel(syntaxTrees, false);

        }
        public class FindSpecificationDeclarations : CSharpSyntaxWalker {
            public List<TypeDeclarationSyntax> TypeDeclarationSyntax;
            public FindSpecificationDeclarations() : base(SyntaxWalkerDepth.Node) {
                this.TypeDeclarationSyntax = new List<TypeDeclarationSyntax>();
            }
            public TypeDeclarationSyntax[] Execute(SyntaxTree syntaxTree) {
                this.Visit(syntaxTree.GetRoot());
                return this.DestroyingRead();
            }

            public TypeDeclarationSyntax[] DestroyingRead() {
                if (this.TypeDeclarationSyntax.Count == 0) {
                    return System.Array.Empty<TypeDeclarationSyntax>();
                } else {
                    var result = this.TypeDeclarationSyntax.ToArray();
                    this.TypeDeclarationSyntax.Clear();
                    return result;
                }
            }

            public override void VisitClassDeclaration(ClassDeclarationSyntax node) {
                this.TypeDeclarationSyntax.Add(node);
                //if (node.BaseList is object) {
                //    foreach (var baseTypeSyntax in node.BaseList.Types) {
                //        var baseTypeString = baseTypeSyntax.ToString();
                //        if (baseTypeString.Contains("IJsonSpecification", StringComparison.Ordinal)) {
                //            this.ClassDeclarations.Add(node);
                //            break;
                //        }
                //    }
                //}
                ////
                base.VisitClassDeclaration(node);
            }
            public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) {
                base.VisitInterfaceDeclaration(node);
            }
            public override void VisitRecordDeclaration(RecordDeclarationSyntax node) {
                base.VisitRecordDeclaration(node);
            }
            public override void VisitStructDeclaration(StructDeclarationSyntax node) {
                base.VisitStructDeclaration(node);
            }
        }
        public class FindTypeDeclarations : CSharpSyntaxWalker {
            public List<TypeDeclarationSyntax> TypeDeclarations;
            public FindTypeDeclarations() : base(SyntaxWalkerDepth.Node) {
                this.TypeDeclarations = new List<TypeDeclarationSyntax>();
            }
            public TypeDeclarationSyntax[] Execute(SyntaxTree syntaxTree) {
                this.Visit(syntaxTree.GetRoot());
                return this.DestroyingRead();
            }

            public TypeDeclarationSyntax[] DestroyingRead() {
                if (this.TypeDeclarations.Count == 0) {
                    return System.Array.Empty<TypeDeclarationSyntax>();
                } else {
                    var result = this.TypeDeclarations.ToArray();
                    this.TypeDeclarations.Clear();
                    return result;
                }
            }

            public override void VisitClassDeclaration(ClassDeclarationSyntax node) {
                this.TypeDeclarations.Add(node);
                base.VisitClassDeclaration(node);
            }
            public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) {
                this.TypeDeclarations.Add(node);
                base.VisitInterfaceDeclaration(node);
            }
            public override void VisitRecordDeclaration(RecordDeclarationSyntax node) {
                this.TypeDeclarations.Add(node);
                base.VisitRecordDeclaration(node);
            }
            public override void VisitStructDeclaration(StructDeclarationSyntax node) {
                this.TypeDeclarations.Add(node);
                base.VisitStructDeclaration(node);
            }
        }
        [Fact]
        public async Task WorkspaceUtility_OpenProject() {
            BuildUtility.InitLocator(null);
            ContextDisguise contextDisguise = new ContextDisguise();
            var sut = WorkspaceCTUtility.Create(null);
            var project = await sut.OpenProjectAsync(
                SolutionFolderPath.GetSolutionItemPath(@"sample\SampleLibrary1\SampleLibrary1.csproj"),
                contextDisguise,
                default);
            Assert.NotNull(project);
            Assert.True(contextDisguise.Assemblies.Count == 1);
            //var assembly = new AssemblyCTDisguise(project, contextDisguise);
            //var projectCompilation = await project.GetCompilationAsync(default);
            //Assert.NotNull(projectCompilation);
            //projectCompilation.GlobalNamespace
            //var syntaxTrees = projectCompilation.SyntaxTrees;
            //var semanticModel = projectCompilation.GetSemanticModel(syntaxTrees, false);

        }
    }
}