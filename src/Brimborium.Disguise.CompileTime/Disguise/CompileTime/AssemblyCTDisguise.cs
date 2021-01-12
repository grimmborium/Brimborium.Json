using Microsoft.CodeAnalysis;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Disguise.CompileTime {
    public sealed class AssemblyCTDisguise : AssemblyDisguise {
        public static AssemblyIdentity GetAssemblyIdentity(Project project) => new AssemblyIdentity(project.AssemblyName);

        public readonly Project Project;
        private readonly AssemblyIdentity _Identity;

        public AssemblyCTDisguise(Project project, ContextDisguise? contextDisguise)
            : base(contextDisguise) {
            this.Project = project;
            this._Identity = GetAssemblyIdentity(project);
            //
            this.PostInit();
        }

        public override string Name => this._Identity.Name;

        public override AssemblyIdentity Identity => this._Identity;

        protected override void ContextDisguiseUpdated() {
            if (this.ContextDisguise is object) { 
                this.ContextDisguise.Assemblies[this.Identity] = this;
            }
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
            // var typeInfo = new TypeCTDisguise(type, this.ContextDisguise);
            //this.ContextDisguise.Assemblies

        }
    }
}
