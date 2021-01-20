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
            builder.AddSerializationType<RPerson>()
                .WithName("RPerson")
                .AddProperty(p => p.FirstName, p => p.WithName("fn") /*.WithConverter()*/)
                .IgnoreProperty(p => p.LastName)
                .BuildType();
            builder.AddSerializationType<CPerson>()
                .WithName("CPerson")
                .AddProperty(p => p.FirstName, p => p.WithName("fn") /*.WithConverter()*/)
                .IgnoreProperty(p => p.LastName)
                .BuildType();

            builder.AddSerializationType<PocoA>()
                .WithName("A")
                .AddProperty(p => p.A)
                .BuildType();
            builder.AddSerializationType<PocoB>()
                .WithName("BE")
                .WithClassHierachie()
                .AddProperty(p => p.B);
            return builder.BuildSpecification();
        }
    }

#if false
    public class JsonFormatterResolverRPerson
        : IJsonFormatterResolver
        , IJsonFormatterResolverStatic {
        private JsonFormatterRPerson? _JsonFormatterPerson;

        public IJsonFormatter<T> GetFormatter<T>(JsonSerializationConfiguration configuration) {
            if (typeof(T).Equals(typeof(JsonFormatterRPerson))) {
                this._JsonFormatterPerson ??= new JsonFormatterRPerson(configuration);
            }
            throw new System.NotImplementedException();
        }

        public IEnumerable<IJsonFormatter> GetFormatters(JsonSerializationConfiguration configuration) {
            return new IJsonFormatter[]{
                    this._JsonFormatterPerson ??= new JsonFormatterRPerson(configuration)
                };
        }
    }

    public class JsonFormatterRPerson : IJsonFormatterCommon<RPerson> {
        private readonly JsonSerializationConfiguration _Configuration;

        public JsonFormatterRPerson(JsonSerializationConfiguration configuration) {
            this._Configuration = configuration;
        }

        public RPerson Deserialize(JsonReader reader, JsonSerializationConfiguration configuration) {
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
                var result = new RPerson() {
                    FirstName = buffer.FirstName.Value,
                    LastName = buffer.LastName.Value
                };
                //if (buffer.FirstName.IsSet) {
                //    result.FirstName = buffer.FirstName.Value;
                //}
                //if (buffer.LastName.IsSet) {
                //    result.LastName = buffer.LastName.Value;
                //}

                //result.FirstName = buffer.FirstName;
                //result.LastName = buffer.LastName;
                return result;
            }
        }

        public void Serialize(JsonWriter writer, RPerson value, JsonSerializationConfiguration configuration) {

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
        : IJsonFormatter<PocoA>
        , IJsonFormatterCommon<PocoA>
        , IJsonFormatterResolver
        , IJsonFormatterResolverStatic {
        private JsonSerializationConfiguration? configuration;

        public JsonFormatterPocoA() {
        }

        public JsonFormatterPocoA(JsonSerializationConfiguration configuration) {
            this.configuration = configuration;
        }

        //public IJsonFormatter<T> GetFormatter<T>(JsonSerializationConfiguration configuration) {
        //    return new JsonFormatterPocoA(configuration);
        //}

        //IJsonFormatter<T> GetFormatter<T>(JsonSerializationConfiguration configuration);
        //IJsonFormatter<T> GetFormatter<T>(JsonSerializationConfiguration configuration);

        public IJsonFormatter<T> GetFormatter<T>(JsonSerializationConfiguration configuration) {
            if (typeof(PocoA).Equals(typeof(JsonFormatterRPerson))) {
                return (IJsonFormatter<T>)(IJsonFormatter)(new JsonFormatterPocoA(configuration));
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

#endif
}

