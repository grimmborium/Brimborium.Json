namespace Brimborium.Disguise.RunTime {
    public sealed class AssemblyRTDisguise : AssemblyDisguise {
        public static AssemblyIdentity GetAssemblyIdentity(System.Reflection.Assembly assembly)
            => new AssemblyIdentity(assembly.GetName().Name);

        private readonly AssemblyIdentity _Identity;

        public readonly System.Reflection.Assembly Assembly;

        public AssemblyRTDisguise(System.Reflection.Assembly assembly, ContextDisguise? contextDisguise)
            : base(contextDisguise) {
            this.Assembly = assembly;
            this._Identity = GetAssemblyIdentity(this.Assembly);
        }

        public override string Name => this._Identity.Name;

        public override AssemblyIdentity Identity => this._Identity;
    }
}