using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Disguise {
    public class ContextDisguise {
        public readonly Dictionary<AssemblyIdentity, AssemblyDisguise> Assembly;

        protected ContextDisguise() {
            this.Assembly = new Dictionary<AssemblyIdentity, AssemblyDisguise>(AssemblyIdentityEqualityComparer.Instance);
        }

        public bool TryGetAssembly(
            AssemblyIdentity identity,
            [MaybeNullWhen(false)] out AssemblyDisguise assembly
            ) {
            return (this.Assembly.TryGetValue(identity, out assembly));
        }

        public AssemblyDisguise TryGetOrAddAssembly<A>(
            AssemblyIdentity identity,
            A a,
            Func<A, AssemblyDisguise> createAssembly
            ) {
            if (this.Assembly.TryGetValue(identity, out var assemblyFound)) {
                return assemblyFound;
            } else {
                var assemblyNew = createAssembly(a);
                if (this.Assembly.TryGetValue(identity, out assemblyFound)) {
                    return assemblyFound;
                } else { 
                    this.Assembly.Add(identity, assemblyNew);
                    return assemblyNew;
                }
            }
        }
    }

    public readonly struct AssemblyIdentity {
        public AssemblyIdentity(string name) {
            Name = name;
        }
        public string Name { get; }
    }

    public sealed class AssemblyIdentityEqualityComparer
        : IEqualityComparer<AssemblyIdentity> {
        public static readonly AssemblyIdentityEqualityComparer Instance = new AssemblyIdentityEqualityComparer();

        public bool Equals(AssemblyIdentity x, AssemblyIdentity y) {
            if (ReferenceEquals(x.Name, y.Name)) return true;
            return StringComparer.Ordinal.Equals(x.Name, y.Name);
        }

        public int GetHashCode(AssemblyIdentity obj) {
            return StringComparer.Ordinal.GetHashCode(obj.Name);
        }
    }
}
