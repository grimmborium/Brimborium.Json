using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Brimborium.Json.Specification {
    public class SpecificationContext {
        public readonly Dictionary<string, AssemblySpecification> Assemblies;
        public SpecificationContext() {
            this.Assemblies = new Dictionary<string, AssemblySpecification>();
        }

        public bool TryAddProjectSpecification(
            Assembly assembly,
            out AssemblySpecification result
            ) {
            var search = new AssemblySpecification();
            search.SetAssembly(assembly);
            if (this.Assemblies.TryGetValue(search.AssemblyName!, out var found)) {
                result = found;
                return true;
            } else {
                result = search;
                return false;
            }            
        }
    }
}