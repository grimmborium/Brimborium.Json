using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;

namespace Brimborium.Disguise.CompileTime {
    public partial class WorkspaceCTUtility {
        public class ProjectCTUtility {
            public readonly Project Project;
            public readonly AssemblyIdentity AssemblyIdentity;
            public readonly List<CodeUtility> SemanticModels;

            public AssemblyCTDisguise Assembly { get; internal set; }
            public Compilation? Compilation { get; set; }

            public ProjectCTUtility(Project project) {
                this.Project = project;
                this.AssemblyIdentity = AssemblyCTDisguise.GetAssemblyIdentity(project);
                this.SemanticModels = new List<CodeUtility>();
            }

            public async Task<List<CodeUtility>> ProcessProjectAsync(
                   Func<Compilation, bool>? predicateCompilation,
                   ContextDisguise contextDisguise,
                   Action<ContextDisguise, CodeUtility> process,
                   CancellationToken cancellationToken
                ) {
                var compilation = await this.GetCompilationAsync(cancellationToken);
                if (compilation is object) {
                    if (predicateCompilation is null || predicateCompilation(compilation)) {
                        var result = await this.GetSemanticModelAsync(cancellationToken);
                        foreach (var codeUtility in result){
                            process(contextDisguise, codeUtility);
                        }
                        return result;
                    }
                }
                return new List<CodeUtility>();
            }

            public async Task<Compilation?> GetCompilationAsync(CancellationToken cancellationToken) {
                var compilation = this.Compilation ??= await this.Project.GetCompilationAsync(cancellationToken);
                //if (compilation is object) {
                //    InspectNamespace(compilation.GlobalNamespace);
                //}
                return compilation;
            }

            public async Task<List<CodeUtility>> GetSemanticModelAsync(
                //Func<SyntaxTree, bool>? predicate,
                CancellationToken cancellationToken
                ) {
                var result = new List<CodeUtility>();
                foreach (var doc in this.Project.Documents) {
                    var syntaxTree = await doc.GetSyntaxTreeAsync();
                    if (syntaxTree is object) {
                        var semanticModel = await doc.GetSemanticModelAsync(cancellationToken);
                        if (semanticModel is object) {
                            result.Add(new CodeUtility(this, syntaxTree, semanticModel));
                        }
                    }
                }
                //
                this.SemanticModels.Clear();
                this.SemanticModels.AddRange(result);
                return result;
            }
        }
    }
}
