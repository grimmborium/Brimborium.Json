using System;
using System.Collections.Generic;
using System.Reflection;

namespace Brimborium.Json.Specification {
    public class JsonSpecification : AssemblySpecification {
        public readonly List<AssemblySpecification> Assemblies;
        public readonly List<TypeSpecification> Types;
        public JsonSpecification() {
            this.Types = new List<TypeSpecification>();
            this.Assemblies = new List<AssemblySpecification>();
        }
        public AssemblySpecification AddAssembly(Assembly assembly) {
            var result = new AssemblySpecification();
            result.SetAssembly(assembly);
            this.Assemblies.Add(result);
            return result;
        }
        public override void SetAssembly(Assembly assembly) {
            base.SetAssembly(assembly);
            this.AddAssembly(assembly);
        }
    }

    public class AssemblySpecification {
        private readonly List<TypeSpecification> _Types;
        public AssemblySpecification() {
            this._Types = new List<TypeSpecification>();
        }

        public Assembly? CrlAssembly { get; set; }

        public string? AssemblyName { get; set; }

        public virtual void SetAssembly(Assembly assembly) {
            this.CrlAssembly = assembly;
                var name = assembly.GetName();
                this.AssemblyName = name.Name;
        }

        public TypeSpecification AddType<TType>(Action<TypeSpecification>? configure) 
            => this.AddType(typeof(TType), configure);
        

        public TypeSpecification AddType(Type type, Action<TypeSpecification>? configure) {
            var result = new TypeSpecification();
            result.SetType(type);
            this._Types.Add(result);
            return result;
        }
    }

    public class TypeSpecification {
        public Type? CrlType { get; set; }
        // public Type? BaseType { get; set; }
        public string? Namespace { get; set; }
        public string? Name { get; set; }
        public string? FullName { get; set; }

        public readonly List<PropertySpecification> Properties;
        public TypeSpecification() {
            this.Properties = new List<PropertySpecification>();
        }

        public void SetType(Type? type) {
            this.CrlType = type;
            if (type is null) {
                this.FullName = null;
            } else {
                this.Namespace = type.Namespace;
                this.Name = type.Name;
                this.FullName = type.FullName;
                //type.BaseType
            }
        }

        public void AddAllProperties() {
            var crlType = this.CrlType;
            if (crlType is object) {

                var members = crlType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
                foreach (var member in members) {
                    if (member.MemberType == MemberTypes.Field) {
                        var propertySpecification = new PropertySpecification();
                        propertySpecification.SetField(member);
                        this.Properties.Add(propertySpecification);
                    } else if (member.MemberType == MemberTypes.Property) {
                        var propertySpecification = new PropertySpecification();
                        propertySpecification.SetProperty(member);
                        this.Properties.Add(propertySpecification);
                    } else {
                    }
                }
            }
        }
    }
    public class PropertySpecification {
        public PropertySpecification() {
        }

        public string? Name { get; set; }

        public void SetField(MemberInfo member) {
            this.Name = member.Name;
        }

        public void SetProperty(MemberInfo member) {
            this.Name = member.Name;
        }
    }
}