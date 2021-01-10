using System;

namespace Brimborium.Disguise {
    public abstract class TypeInfoDisguise: BaseDisguise {
        protected TypeInfoDisguise(ContextDisguise? contextDisguise)
            : base(contextDisguise) {
        }
        public abstract string Name { get; }
        public abstract string Namespace { get; }
        public abstract AssemblyDisguise Assembly { get; }
    }
}
