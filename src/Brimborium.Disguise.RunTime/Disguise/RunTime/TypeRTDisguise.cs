using System;
using System.Collections.Generic;
using System.Reflection;

namespace Brimborium.Disguise.RunTime {
    public class TypeRTDisguise : TypeDisguise {
        public static AssemblyIdentity GetTypeIdentity(Type type) => new AssemblyIdentity(type.GetTypeInfo().FullName);

        public readonly TypeInfo TypeInfo;
        private AssemblyDisguise? _Assembly;

        public TypeRTDisguise(Type type, ContextDisguise? contextDisguise)
            : this(type.GetTypeInfo(), contextDisguise) {
        }

        public TypeRTDisguise(TypeInfo typeInfo, ContextDisguise? contextDisguise)
            : base(contextDisguise) {
            this.TypeInfo = typeInfo;
            // this.TypeInfo.GetMembers
            // this.TypeInfo.IsGenericType
        }

        public override string Name => this.TypeInfo.Name;

        public override string Namespace => this.TypeInfo.Namespace;

        public override AssemblyDisguise Assembly {
            get {
                var assembly = this.TypeInfo.Assembly;
                if (this._Assembly is null) {
                    if (this.ContextDisguise is object) {
                        return this._Assembly = this.ContextDisguise.TryGetOrAddAssembly(
                            AssemblyRTDisguise.GetAssemblyIdentity(assembly),
                            this,
                            (that) => new AssemblyRTDisguise(that.TypeInfo.Assembly, that.ContextDisguise)
                            );
                    } else {
                        return this._Assembly = new AssemblyRTDisguise(assembly, this.ContextDisguise);
                    }
                } else { 
                    return this._Assembly;
                }
            }
        }

        public override bool IsArray => this.TypeInfo.IsArray;
        public override bool IsClass => this.TypeInfo.IsClass;
        public override bool IsValueType => this.TypeInfo.IsValueType;
    }
}