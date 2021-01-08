using System;

namespace Brimborium.Json.Specification {
    public static class JsonSpecificationExtensions {
        public static ProjectSpecification CreateProjectSpecification<TOwner>(
                this TOwner jsonSpecification,
                SpecificationContext ctxt
            )
            where TOwner : IJsonSpecification {
            
            var assembly = jsonSpecification.GetType().Assembly;
            if (ctxt.TryAddProjectSpecification(assembly, out var result)) {

                return result;
            }
            return result;
            
        }
    }
}
