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
        public virtual TypeIdentity Identity => new TypeIdentity(
            $"{Namespace}.{Name}",
            this.Name, 
            null,
            this.Namespace,
            this.Assembly.Identity);
        public virtual bool IsArray => false;
        public virtual bool IsClass => false;
        public virtual bool IsValueType => false;
        //public virtual bool IsClass => false;
    }

    public readonly struct TypeIdentity {
        public TypeIdentity(
            string displayString, 
            string name,
            string? containingType,
            string? containingNamespace,
            AssemblyIdentity? assemblyIdentity
            ) {
            DisplayString = displayString;
            Name = name;
            ContainingType = containingType;
            ContainingNamespace = containingNamespace;
            AssemblyIdentity = assemblyIdentity;
        }

        public string DisplayString { get; }
        public string Name { get; }
        public string? ContainingType { get; }
        public string ContainingNamespace { get; }
        public AssemblyIdentity? AssemblyIdentity { get; }
    }

    public sealed class TypeIdentityEqualityComparer
        : IEqualityComparer<TypeIdentity> {
        public static readonly TypeIdentityEqualityComparer Instance = new TypeIdentityEqualityComparer();

        public bool Equals(TypeIdentity x, TypeIdentity y) {
            if (ReferenceEquals(x.Name, y.Name)
                && ReferenceEquals(x.ContainingNamespace, y.ContainingNamespace)
                ) return true;
            return StringComparer.Ordinal.Equals(x.Name, y.Name)
                && StringComparer.Ordinal.Equals(x.ContainingNamespace, y.ContainingNamespace);
        }

        public int GetHashCode(TypeIdentity obj) {
            unchecked { 
                return StringComparer.Ordinal.GetHashCode(obj.Name)
                    ^ StringComparer.Ordinal.GetHashCode(obj.ContainingNamespace)
                    ;
            }
        }
    }
}
