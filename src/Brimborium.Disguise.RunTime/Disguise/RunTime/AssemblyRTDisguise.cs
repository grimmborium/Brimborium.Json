using System.Reflection;

namespace Brimborium.Disguise.RunTime {
    public class AssemblyRTDisguise : AssemblyDisguise {
        public static AssemblyIdentity GetAssemblyIdentity(Assembly assembly) => new AssemblyIdentity(assembly.GetName().Name);

        public readonly Assembly Assembly;

        public AssemblyRTDisguise(Assembly assembly, ContextDisguise? contextDisguise)
            : base(contextDisguise) {
            this.Assembly = assembly;
        }
        public override string Name => this.Assembly.GetName().Name;

        public override AssemblyIdentity Identity => GetAssemblyIdentity(this.Assembly);
    }
}