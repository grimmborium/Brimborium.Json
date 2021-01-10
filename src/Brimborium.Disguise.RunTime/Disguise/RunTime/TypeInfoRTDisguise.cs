using System;
using System.Collections.Generic;
using System.Reflection;

namespace Brimborium.Disguise.RunTime {
    public class TypeInfoRTDisguise : TypeInfoDisguise {
        public readonly TypeInfo TypeInfo;
        private AssemblyDisguise? _Assembly;

        public TypeInfoRTDisguise(Type type, ContextDisguise? contextDisguise)
            : this(type.GetTypeInfo(), contextDisguise) {
        }

        public TypeInfoRTDisguise(TypeInfo typeInfo, ContextDisguise? contextDisguise)
            : base(contextDisguise) {
            this.TypeInfo = typeInfo;
            // this.TypeInfo.GetMembers
            // this.TypeInfo.Assembly
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
    }
}