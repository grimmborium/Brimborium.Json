using Brimborium.Json.Internal;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Brimborium.Json {
    /// <summary>
    /// This enum defines the various ways the <see cref="Utf8JsonReader"/> can deal with comments.
    /// </summary>
    public enum JsonCommentHandling {
        /// <summary>
        /// By default, do no allow comments within the JSON input.
        /// Comments are treated as invalid JSON if found and a
        /// <see cref="JsonException"/> is thrown.
        /// </summary>
        Disallow = 0,
        /// <summary>
        /// Allow comments within the JSON input and ignore them.
        /// The <see cref="Utf8JsonReader"/> will behave as if no comments were present.
        /// </summary>
        Skip = 1,
        /// <summary>
        /// Allow comments within the JSON input and treat them as valid tokens.
        /// While reading, the caller will be able to access the comment values.
        /// </summary>
        Allow = 2,
    }
    public enum JsonIgnoreCondition {
        /// <summary>
        /// Property is never ignored during serialization or deserialization.
        /// </summary>
        Never = 0,
        /// <summary>
        /// Property is always ignored during serialization and deserialization.
        /// </summary>
        Always = 1,
        /// <summary>
        /// If the value is the default, the property is ignored during serialization.
        /// This is applied to both reference and value-type properties and fields.
        /// </summary>
        WhenWritingDefault = 2,
        /// <summary>
        /// If the value is <see langword="null"/>, the property is ignored during serialization.
        /// This is applied only to reference-type properties and fields.
        /// </summary>
        WhenWritingNull = 3,
    }
    /// <summary>
    /// Determines how <see cref="JsonSerializer"/> handles numbers when serializing and deserializing.
    /// </summary>
    [Flags]
    public enum JsonNumberHandling {
        /// <summary>
        /// Numbers will only be read from <see cref="JsonTokenType.Number"/> tokens and will only be written as JSON numbers (without quotes).
        /// </summary>
        Strict = 0x0,
        /// <summary>
        /// Numbers can be read from <see cref="JsonTokenType.String"/> tokens.
        /// Does not prevent numbers from being read from <see cref="JsonTokenType.Number"/> token.
        /// Strings that have escaped characters will be unescaped before reading.
        /// Leading or trailing trivia within the string token, including whitespace, is not allowed.
        /// </summary>
        AllowReadingFromString = 0x1,
        /// <summary>
        /// Numbers will be written as JSON strings (with quotes), not as JSON numbers.
        /// </summary>
        WriteAsString = 0x2,
        /// <summary>
        /// The "NaN", "Infinity", and "-Infinity" <see cref="JsonTokenType.String"/> tokens can be read as
        /// floating-point constants, and the <see cref="float"/> and <see cref="double"/> values for these
        /// constants (such as <see cref="float.PositiveInfinity"/> and <see cref="double.NaN"/>)
        /// will be written as their corresponding JSON string representations.
        /// Strings that have escaped characters will be unescaped before reading.
        /// Leading or trailing trivia within the string token, including whitespace, is not allowed.
        /// </summary>
        AllowNamedFloatingPointLiterals = 0x4
    }

    //public class ReferenceHandler {     }

    public class JsonSerializationConfigurationBuilder {
        public Func<string, string> PropertyNameMutator { get; set; }
        // public bool AllowTrailingCommas { get; set; }
        // public JsonCommentHandling CommentHandling { get; set; }
        public JsonNumberHandling NumberHandling { get; set; }
        public bool PropertyNameCaseInsensitive { get; set; }
        // public ReferenceHandler ReferenceHandler { get; set; }
        public List<IJsonFormatterResolver> Resolvers;
        public List<IJsonFormatter> Formatters;

        public JsonSerializationConfigurationBuilder() {
            this.PropertyNameMutator = StringMutator.Original;
            this.Resolvers = new List<IJsonFormatterResolver>();
            this.Formatters = new List<IJsonFormatter>();
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
            return JsonSerializationConfiguration.Build(this);
        }
    }

    public class JsonSerializationConfiguration {

        internal static JsonSerializationConfiguration Build(JsonSerializationConfigurationBuilder builder) {
            var result = new JsonSerializationConfiguration(
                builder.PropertyNameMutator,
                builder.NumberHandling,
                builder.PropertyNameCaseInsensitive
                );
            result.Resolvers.AddRange(builder.Resolvers);
            result.Formatters.AddRange(builder.Formatters);
            return result;
        }

        private static int _Count;

        public readonly int Instance;
        public readonly Func<string, string> PropertyNameMutator;
        // private readonly bool AllowTrailingCommas;
        // private readonly JsonCommentHandling CommentHandling;
        public readonly JsonNumberHandling NumberHandling;
        public readonly bool PropertyNameCaseInsensitive;

        internal readonly List<IJsonFormatterResolver> Resolvers;
        internal readonly List<IJsonFormatter> Formatters;

        protected JsonSerializationConfiguration(
                Func<string, string> propertyNameMutator,
                // bool allowTrailingCommas,
                // JsonCommentHandling commentHandling,
                JsonNumberHandling numberHandling,
                bool propertyNameCaseInsensitive
            ) {
            this.Instance = System.Threading.Interlocked.Increment(ref _Count);
            this.PropertyNameMutator = propertyNameMutator;
            this.NumberHandling = numberHandling;
            this.PropertyNameCaseInsensitive = propertyNameCaseInsensitive;
            this.Formatters = new List<IJsonFormatter>();
            this.Resolvers = new List<IJsonFormatterResolver>();
        }

        private JsonSerializationConfigurationForReader<JsonReaderUtf8>? _cfgJsonReaderUtf8;
        private JsonSerializationConfigurationForReader<JsonReaderCharArray>? _cfgJsonReaderCharArray;

        public virtual Type? GetReaderType() => null;
        public virtual Type? GetWriterType() => null;

        public virtual JsonSerializationConfiguration ForReader<TJsonReader>()
            where TJsonReader : JsonReader {
            if (typeof(TJsonReader).Equals(typeof(JsonReaderUtf8))) {
                return (_cfgJsonReaderUtf8 ??= (new JsonSerializationConfigurationForReader<JsonReaderUtf8>(this).Init()));
            }
            if (typeof(TJsonReader).Equals(typeof(JsonReaderCharArray))) {
                return (_cfgJsonReaderCharArray ??= (new JsonSerializationConfigurationForReader<JsonReaderCharArray>(this).Init()));
            }
            throw new NotSupportedException();
        }

        private JsonSerializationConfigurationForWriter<JsonWriterUtf8>? _cfgJsonWriterUtf8;
        private JsonSerializationConfigurationForWriter<JsonWriterCharArray>? _cfgJsonWriterCharArray;
        private JsonSerializationConfigurationForWriter<JsonWriterStringBuilder>? _cfgJsonWriterStringBuilder;

        public virtual JsonSerializationConfiguration ForWriter<TJsonWriter>()
            where TJsonWriter : JsonWriter {
            if (typeof(TJsonWriter).Equals(typeof(JsonWriterUtf8))) {
                return (_cfgJsonWriterUtf8 ??= (new JsonSerializationConfigurationForWriter<JsonWriterUtf8>(this).Init()));
            }
            if (typeof(TJsonWriter).Equals(typeof(JsonWriterCharArray))) {
                return (_cfgJsonWriterCharArray ??= (new JsonSerializationConfigurationForWriter<JsonWriterCharArray>(this).Init()));
            }
            if (typeof(TJsonWriter).Equals(typeof(JsonWriterStringBuilder))) {
                return (_cfgJsonWriterStringBuilder ??= (new JsonSerializationConfigurationForWriter<JsonWriterStringBuilder>(this).Init()));
            }
            throw new NotSupportedException();
        }

        public virtual IJsonSerializer2<T> GetSerializer<T>() {
            throw new NotImplementedException();
        }
        public virtual IJsonDeserializer2<T> GetDeserializer<T>() {
            throw new NotImplementedException();
        }

    }

    public class JsonSerializationConfigurationForReader<TForJsonReader>
        : JsonSerializationConfiguration
        where TForJsonReader : JsonReader {
        private readonly JsonSerializationConfiguration _Configuration;

        public JsonSerializationConfigurationForReader(
                JsonSerializationConfiguration configuration
            ) : base(
                configuration.PropertyNameMutator,
                configuration.NumberHandling,
                configuration.PropertyNameCaseInsensitive
                ) {
            this._Configuration = configuration;
        }

        public override Type? GetReaderType() => typeof(TForJsonReader);

        public override JsonSerializationConfiguration ForReader<TJsonReader>() => (
            (typeof(TForJsonReader).Equals(typeof(TJsonReader))) 
                ? this 
                : this._Configuration.ForReader<TJsonReader>()
            );

        public override JsonSerializationConfiguration ForWriter<TJsonWriter>() => this._Configuration.ForWriter<TJsonWriter>();

        public JsonSerializationConfigurationForReader<TForJsonReader> Init() {
            foreach (var resolver in this._Configuration.Resolvers) {
                if (resolver is IJsonDeserializerResolverCommon || resolver is IJsonDeserializerResolver<TForJsonReader>) {
                    var boundResolver = resolver.BindForReader(this);
                    this.Resolvers.Add(boundResolver);
                }
            }
            foreach (var formatter in this._Configuration.Formatters) {
                var boundFormatter = formatter.BindForReader(this);
#warning here Formatters
                this.Formatters.Add(boundFormatter);
            }
            return this;
        }

        public override IJsonDeserializer2<T> GetDeserializer<T>() {
            return base.GetDeserializer<T>();
        }
    }
    public class JsonSerializationConfigurationForWriter<TForJsonWriter>
        : JsonSerializationConfiguration
        where TForJsonWriter : JsonWriter {
        private readonly JsonSerializationConfiguration _Configuration;

        public JsonSerializationConfigurationForWriter(
                JsonSerializationConfiguration configuration
            ) : base(
                configuration.PropertyNameMutator,
                configuration.NumberHandling,
                configuration.PropertyNameCaseInsensitive
                ) {
            this._Configuration = configuration;
        }

        public override JsonSerializationConfiguration ForReader<TJsonReader>() => this._Configuration.ForReader<TJsonReader>();

        public override JsonSerializationConfiguration ForWriter<TJsonWriter>() => (
            (typeof(TForJsonWriter).Equals(typeof(TJsonWriter)))
                ? this
                : this._Configuration.ForWriter<TJsonWriter>()
            );

        public override Type? GetWriterType() => typeof(TForJsonWriter);

        public JsonSerializationConfigurationForWriter<TForJsonWriter> Init() {
            foreach (var resolver in this._Configuration.Resolvers) {
                if (resolver is IJsonSerializerResolverCommon || resolver is IJsonSerializerResolver<TForJsonWriter>) {
                    var boundResolver = resolver.BindForWriter(this);
                    this.Resolvers.Add(boundResolver);
                }
            }
            foreach (var formatter in this._Configuration.Formatters) {
                var boundFormatter = formatter.BindForReader(this);
#warning here Formatters
                this.Formatters.Add(boundFormatter);
            }

            return this;
        }

        public override IJsonSerializer2<T> GetSerializer<T>() {
            return base.GetSerializer<T>();
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