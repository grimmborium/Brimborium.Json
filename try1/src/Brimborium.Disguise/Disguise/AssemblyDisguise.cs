using System;
using System.Collections.Generic;

namespace Brimborium.Disguise {
    public abstract class AssemblyDisguise : BaseDisguise {
        protected AssemblyDisguise(ContextDisguise? contextDisguise)
            : base(contextDisguise) {
        }

        public abstract string Name { get; }

        public virtual AssemblyIdentity Identity => new AssemblyIdentity(this.Name);
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
