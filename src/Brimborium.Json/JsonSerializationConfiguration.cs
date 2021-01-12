using Brimborium.Json.Internal;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Brimborium.Json {
    public class JsonSerializationConfigurationBuilder {
        public Func<string, string> PropertyNameMutator;
        public List<IJsonFormatterResolver> Resolvers;
        public List<IJsonFormatter> Formatters;

        public JsonSerializationConfigurationBuilder() {
            this.PropertyNameMutator = StringMutator.Original;

        }
        public JsonSerializationConfigurationBuilder WithPropertyNameMutator(string propertyNameMutator) {
            this.PropertyNameMutator =
                propertyNameMutator switch {
                    "CamelCase" => StringMutator.CamelCase,
                    "SnakeCase" => StringMutator.SnakeCase,
                    _ => StringMutator.Original
                };
            return this;
        }

        public JsonSerializationConfiguration Build() {
            var result = new JsonSerializationConfiguration(
                this.PropertyNameMutator,
                this.Resolvers.ToArray(),
                this.Formatters.ToArray()
                );
            return result;
        }
    }

    public class JsonSerializationConfiguration {
        private static int _Count;

        public readonly int Instance;

        public Func<string, string> PropertyNameMutator { get; internal set; }

        public IJsonFormatterResolver[] Resolvers { get; internal set; }

        public IJsonFormatter[] Formatters { get; internal set; }

        public JsonSerializationConfiguration(
                Func<string, string> propertyNameMutator,
                IJsonFormatterResolver[] resolvers,
                IJsonFormatter[] formatters
            ) {
            this.Instance = System.Threading.Interlocked.Increment(ref _Count);
            this.PropertyNameMutator = propertyNameMutator;
            this.Resolvers = resolvers;
            this.Formatters = formatters;
        }
    }

    public class JsonSerializationConfigurationSpecWriter<TJsonWriter>
        where TJsonWriter : JsonWriter {
        public JsonSerializationConfigurationSpecWriter() : base() { }
    }

    public class JsonSerializationConfigurationSpecReader<TJsonReader>
        where TJsonReader : JsonReader {
        public JsonSerializationConfigurationSpecReader() : base() { }

        /*
         * 
         public interface IJsonFormatterSpecWriter<T, TJsonWriter>
            : IJsonFormatter
            where TJsonWriter : JsonWriter{
            void SerializeSpec(TJsonWriter writer, T value, IJsonFormatterResolver formatterResolver);
        }
        public interface IJsonFormatterSpecReader<T, TJsonReader>
            : IJsonFormatter
            where TJsonReader : JsonReader {
            T DeserializeSpec(TJsonReader reader, IJsonFormatterResolver formatterResolver);
        }* 

             public interface IJsonFormatterSpecReader<T, TJsonReader>
            : IJsonFormatter
            where TJsonReader : JsonReader {
            T DeserializeSpec(TJsonReader reader, IJsonFormatterResolver formatterResolver);
        }

         */
    }
}