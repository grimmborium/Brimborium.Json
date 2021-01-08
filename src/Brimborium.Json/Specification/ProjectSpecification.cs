using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Brimborium.Json.Specification {
    public class ProjectSpecification {
        private readonly List<TypeSpecification> _Types;
        public ProjectSpecification() {
            this._Types = new List<TypeSpecification>();  
        }

        public Assembly? CrlAssembly { get; set; }
        public string? AssemblyName { get; set; }

        public void SetAssembly(Assembly? assembly) {
            this.CrlAssembly = assembly;
            if (assembly is null) {
                this.AssemblyName = null;
            } else {
                var name = assembly.GetName();
                this.AssemblyName = name.Name;
            }
        }

        public ProjectSpecification AddType<TType>(Action<TypeSpecification>? configure=null) {
            var typeSpecification = new TypeSpecification();
            typeSpecification.SetType(typeof(TType));
            this._Types.Add(typeSpecification);
            return this;
        }
    }
    public class TypeSpecification {
        public Type? CrlType { get; set; }
        // public Type? BaseType { get; set; }
        public string? Namespace { get; set; }
        public string? Name { get; set; }
        public string? FullName { get; set; }

        public TypeSpecification() {
        }
        public void SetType(Type? type) {
            this.CrlType = type;
            if (type is null) {
                this.FullName = null;
            } else {
                this.Namespace = type.Namespace;
                this.Name = type.Name;
                this.FullName = type.FullName;
                type.BaseType
            }
        }
    }
    public class PropertySpecification {
    }
}