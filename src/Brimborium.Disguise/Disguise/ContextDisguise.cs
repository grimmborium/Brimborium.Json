using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Disguise {
    public class ContextDisguise {
        public readonly Dictionary<AssemblyIdentity, AssemblyDisguise> Assemblies;
        public readonly Dictionary<TypeIdentity, TypeDisguise> Types;

        public ContextDisguise() {
            this.Assemblies = new Dictionary<AssemblyIdentity, AssemblyDisguise>(AssemblyIdentityEqualityComparer.Instance);
            this.Types = new Dictionary<TypeIdentity, TypeDisguise>(TypeIdentityEqualityComparer.Instance);
        }

        public bool TryGetAssembly(
            AssemblyIdentity identity,
            [MaybeNullWhen(false)] out AssemblyDisguise assembly
            ) {
            return (this.Assemblies.TryGetValue(identity, out assembly));
        }

        public AssemblyDisguise TryGetOrAddAssembly<A>(
            AssemblyIdentity identity,
            A a,
            Func<A, AssemblyDisguise> createAssembly
            ) {
            if (this.Assemblies.TryGetValue(identity, out var assemblyFound)) {
                return assemblyFound;
            } else {
                var assemblyNew = createAssembly(a);
                if (this.Assemblies.TryGetValue(identity, out assemblyFound)) {
                    return assemblyFound;
                } else { 
                    this.Assemblies.Add(identity, assemblyNew);
                    return assemblyNew;
                }
            }
        }
    }

    
}
