using Brimborium.Json.Specification;

namespace SampleLibrary2 {
    public class ProjectJsonSpecification : IJsonSpecification {
        public ProjectJsonSpecification() {
        }

        public JsonSpecification GetJsonSpecification(IJsonSpecificationBuilder builder) {
            builder.AddSerializationType<Person>()
                .WithName("Person")
                .AddProperty(p => p.FirstName, p => p.WithName("fn") /*.WithConverter()*/)
                .IgnoreProperty(p => p.LastName)
                .Validate()
                .Build();
            builder.AddSerializationType<PocoA>()
                .WithName("A")
                .AddProperty(p => p.A)
                .Build();
            builder.AddSerializationType<PocoB>()
                .WithName("BE")
                .WithClassHierachie()
                .AddProperty(p => p.B);
            return builder.Build();
        }
    }
}
