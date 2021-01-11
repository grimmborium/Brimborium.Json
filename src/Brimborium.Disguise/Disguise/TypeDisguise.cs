using System;
using System.Collections.Generic;

namespace Brimborium.Disguise {
    public abstract class TypeDisguise
        : BaseDisguise {
        protected TypeDisguise(ContextDisguise? contextDisguise)
            : base(contextDisguise) {
        }
        public abstract string Name { get; }
        public abstract string Namespace { get; }
        public abstract AssemblyDisguise Assembly { get; }
        public virtual TypeIdentity Identity => new TypeIdentity(this.Name, this.Namespace);
    }

    public readonly struct TypeIdentity {
        public TypeIdentity(string name, string @namespace) {
            Name = name;
            Namespace = @namespace;
        }
        public string Name { get; }
        public string Namespace { get; }
    }

    public sealed class TypeIdentityEqualityComparer
        : IEqualityComparer<TypeIdentity> {
        public static readonly TypeIdentityEqualityComparer Instance = new TypeIdentityEqualityComparer();

        public bool Equals(TypeIdentity x, TypeIdentity y) {
            if (ReferenceEquals(x.Name, y.Name)
                && ReferenceEquals(x.Namespace, y.Namespace)
                ) return true;
            return StringComparer.Ordinal.Equals(x.Name, y.Name)
                && StringComparer.Ordinal.Equals(x.Namespace, y.Namespace);
        }

        public int GetHashCode(TypeIdentity obj) {
            unchecked { 
                return StringComparer.Ordinal.GetHashCode(obj.Name)
                    ^ StringComparer.Ordinal.GetHashCode(obj.Namespace)
                    ;
            }
        }
    }
}
