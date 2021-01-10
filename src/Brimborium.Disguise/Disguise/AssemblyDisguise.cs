namespace Brimborium.Disguise {
    public abstract class AssemblyDisguise : BaseDisguise {
        protected AssemblyDisguise(ContextDisguise? contextDisguise)
            : base(contextDisguise) {
        }

        public abstract string Name { get; }

        public abstract AssemblyIdentity Identity { get; }
    }
}
