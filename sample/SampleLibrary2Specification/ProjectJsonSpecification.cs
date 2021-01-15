#nullable enable

using Brimborium.Json.Specification;
using Brimborium.Json;
using System.Collections.Generic;
using System;

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

    public class JsonFormatterResolverPerson
        : IJsonFormatterResolver
        , IJsonFormatterResolverStatic {
        private JsonFormatterPerson? _JsonFormatterPerson;

        public IJsonFormatter<T> GetFormatter<T>(JsonSerializationConfiguration configuration) {
            if (typeof(T).Equals(typeof(JsonFormatterPerson))) {
                this._JsonFormatterPerson ??= new JsonFormatterPerson(configuration)
            }
            throw new System.NotImplementedException();
        }

        public IEnumerable<IJsonFormatter> GetFormatters(JsonSerializationConfiguration configuration) {
            return new IJsonFormatter[]{
                    this._JsonFormatterPerson ??= new JsonFormatterPerson(configuration)
                };
        }
    }

    public class JsonFormatterPerson : IJsonFormatterCommon<Person> {
        private readonly JsonSerializationConfiguration _Configuration;

        public JsonFormatterPerson(JsonSerializationConfiguration configuration) {
            this._Configuration = configuration;
        }

        public Person Deserialize(JsonReader reader, JsonSerializationConfiguration configuration) {
            var buffer = new DeserializePerson();
            reader.ReadIsBeginObjectWithVerify();
            int count = 0;
            while (reader.ReadIsEndObjectWithSkipValueSeparator(ref count)) {
                var propertyName = reader.ReadPropertyName();
                switch (propertyName) {
                    case "FirstName":
                        buffer.FirstName.Value = reader.ReadString();
                        buffer.FirstName.IsSet = true;
                        break;
                    case "LastName":
                        buffer.LastName.Value = reader.ReadString();
                        buffer.LastName.IsSet = true;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            reader.ReadIsEndObjectWithVerify();
            if (true) {
                var result = new Person() {
                        buffer.FirstName,
                        buffer.LastName
                    );
                //result.FirstName = buffer.FirstName;
                //result.LastName = buffer.LastName;
                return result;
            }
        }

        public void Serialize(JsonWriter writer, Person value, JsonSerializationConfiguration configuration) {

            throw new System.NotImplementedException();
        }
    }
    public struct DeserializePerson {
        public JsonDeserializeProperty<string> FirstName;
        public JsonDeserializeProperty<string> LastName;
    }
    public struct JsonDeserializeProperty<T> {
        public T Value;
        public bool IsSet;
    }


    public class JsonFormatterPocoA
        : IJsonFormatterCommon<PocoA>
        , IJsonFormatterResolverStatic {
        public JsonFormatterPocoA() {
        }

        //public IJsonFormatter<T> GetFormatter<T>(JsonSerializationConfiguration configuration) {
        //    return new JsonFormatterPocoA(configuration)
        //}

        public IJsonFormatter<PocoA> GetFormatter<T>(JsonSerializationConfiguration configuration) {
            if (typeof(PocoA).Equals(typeof(JsonFormatterPerson))) {
                return new JsonFormatterPocoA(configuration);
                } else {
                throw new System.NotImplementedException();
            }
        }

        public IEnumerable<IJsonFormatter> GetFormatters(JsonSerializationConfiguration configuration) {
            return new IJsonFormatter[] { this };
        }

        public PocoA Deserialize(JsonReader reader, JsonSerializationConfiguration configuration) {
            throw new NotImplementedException();
        }

        public void Serialize(JsonWriter writer, PocoA value, JsonSerializationConfiguration configuration) {
            throw new NotImplementedException();
        }
    }


}

