using System;

using Brimborium.Json.Specification;

namespace SampleLibrary2 {
    public class ProjectJsonSpecification : IJsonSpecification {
        public ProjectJsonSpecification() {
        }

        public JsonSpecification GetSpecification() {
            var result = this.CreateProjectSpecification();
            result.AddType<Person>(t => {
                t.AddAllProperties();
            });
            return result;
        }
    }
}
