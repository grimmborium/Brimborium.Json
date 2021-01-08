using System;

using Brimborium.Json.Specification;

namespace SampleLibrary2 {
    public class JsonSpecification : IJsonSpecification {
        public JsonSpecification() {
        }

        public ProjectSpecification GetSpecification(SpecificationContext ctxt) {
            return this.CreateProjectSpecification(ctxt)
                .AddType<Person>(ctxt);
        }
    }
}
