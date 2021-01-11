using Microsoft.CodeAnalysis;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Disguise.CompileTime {
    public class AssemblyCTDisguise : AssemblyDisguise {
        public readonly Project Project;

        public AssemblyCTDisguise(Project project, ContextDisguise? contextDisguise)
            : base(contextDisguise) {
            this.Project = project;
        }

        public override string Name => this.Project.AssemblyName;

        public override AssemblyIdentity Identity => new AssemblyIdentity(this.Name);

        public Compilation? Compilation { get; set; }

        public async Task<Compilation?> GetCompilationAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            var compilation=this.Compilation ??= await this.Project.GetCompilationAsync(cancellationToken);
            if (compilation is object) {
                InspectNamespace(compilation.GlobalNamespace);
                
            }
            return this.Compilation;
        }

        private void InspectNamespace(INamespaceSymbol? namespaceSymbol) {
            if (namespaceSymbol is null) {
                return;
            } 
            if (namespaceSymbol.IsGlobalNamespace) {
            } else { 
            }
            //foreach (var namespaceMember in namespaceSymbol.GetNamespaceMembers()) {
            //    InspectNamespace(namespaceMember);
            //}
            foreach (var member in namespaceSymbol.GetMembers()) {
                if (member is null) {
                } else if (member is INamespaceSymbol memberNamespaceSymbol) {
                    InspectNamespace(memberNamespaceSymbol);
                } else if (member is ITypeSymbol memberTypeSymbol) {
                    InspectType(memberTypeSymbol);
                }
            }
        }

        private void InspectType(ITypeSymbol type) {
            // type.GetTypeMembers()
            //TypeCTDisguise.
            var typeInfo = new TypeCTDisguise(type, this.ContextDisguise);
            this.ContextDisguise.Assemblies
            
        }
    }
}
