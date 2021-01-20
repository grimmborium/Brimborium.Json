using Brimborium.Json.Specification;

using System;
using System.Linq;

namespace SampleApp1 {
    class Program {
        static void Main(string[] args) {
            var projectJsonSpecification = new SampleLibrary2.ProjectJsonSpecification();
            //var spec2 = projectJsonSpecification.GetJsonSpecification(new JsonSpecificationBuilder());
            //System.Diagnostics.Debug.Assert("SampleLibrary2Specification" == spec2.Assemblies.First().AssemblyName);
            Console.WriteLine();

            Console.WriteLine("- fini -");
        }
    }
}
