using System;

namespace Brimborium.Json.Specification {
    public static class JsonSpecificationExtensions {
        public static JsonSpecification CreateProjectSpecification<TOwner>(
                this TOwner jsonSpecification
            )
            where TOwner : IJsonSpecification {
            var assembly = jsonSpecification.GetType().Assembly;
            var result = new JsonSpecification();
            result.SetAssembly(assembly);
            return result;
            
        }
    }
}
