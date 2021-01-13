using Brimborium.Json.Internal;
using Brimborium.Json.Resolvers;

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

            this.Resolvers.AddRange(BuiltinResolvers.GetResolvers());
            this.Formatters.AddRange(BuiltinResolvers.GetFormatters());
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

        private JsonDeserializerConfiguration<JsonReaderUtf8>? _cfgJsonReaderUtf8;
        private JsonDeserializerConfiguration<JsonReaderUtf16>? _cfgJsonReaderUtf16;

        public virtual Type? GetReaderType() => null;
        public virtual Type? GetWriterType() => null;

        public virtual JsonDeserializerConfiguration<JsonReaderUtf8> ForDeserializationUtf8() {
            if (_cfgJsonReaderUtf8 is null) {
                lock (this) {
                    if (_cfgJsonReaderUtf8 is null) {
                        var cfg = new JsonDeserializerConfiguration<JsonReaderUtf8>(this);
                        cfg.Init();
                        _cfgJsonReaderUtf8 = cfg;
                    }
                }
            }
            return _cfgJsonReaderUtf8;
        }

        public virtual JsonDeserializerConfiguration<JsonReaderUtf16> ForDeserializationUtf16(){
            if (_cfgJsonReaderUtf16 is null) {
                lock (this) {
                    if (_cfgJsonReaderUtf16 is null) {
                        var cfg = new JsonDeserializerConfiguration<JsonReaderUtf16>(this);
                        cfg.Init();
                        _cfgJsonReaderUtf16 = cfg;
                    }
                }
            }
            return _cfgJsonReaderUtf16;
        }


        private JsonSerializerConfiguration<JsonWriterUtf8>? _cfgJsonWriterUtf8;
        private JsonSerializerConfiguration<JsonWriterUtf16>? _cfgJsonWriterUtf16;

        /*
        public virtual JsonSerializationConfiguration<TJsonWriter> ForSerialization<TJsonWriter>([AllowNull] TJsonWriter writer = default)
            where TJsonWriter : JsonWriter {
            if (typeof(TJsonWriter).Equals(typeof(JsonWriterUtf8))) {
                return (JsonSerializationConfiguration<TJsonWriter>)(_cfgJsonWriterUtf8 ??= (new JsonSerializerConfiguration<JsonWriterUtf8>(this).Init()));
            }
            if (typeof(TJsonWriter).Equals(typeof(JsonWriterUtf16))) {
                return (JsonSerializationConfiguration<TJsonWriter>)(_cfgJsonWriterUtf16 ??= (new JsonSerializerConfiguration<JsonWriterUtf16>(this).Init()));
            }
            throw new NotSupportedException();
        }
        */

        public virtual JsonSerializationConfiguration<JsonWriterUtf8> ForSerializationUtf8() {
            if (_cfgJsonWriterUtf8 is null) {
                lock (this) {
                    if (_cfgJsonWriterUtf8 is null) {
                        var cfg = new JsonDeserializerConfiguration<JsonWriterUtf8>(this);
                        cfg.Init();
                        _cfgJsonWriterUtf8 = cfg;
                    }
                }
            }
            return _cfgJsonWriterUtf8;
        }

        public virtual JsonSerializationConfiguration<JsonWriterUtf16> ForSerializationUtf16() {
            if (_cfgJsonWriterUtf16 is null) {
                lock (this) {
                    if (_cfgJsonWriterUtf16 is null) {
                        var cfg = new JsonDeserializerConfiguration<JsonWriterUtf16>(this);
                        cfg.Init();
                        _cfgJsonWriterUtf16 = cfg;
                    }
                }
            }
            return _cfgJsonWriterUtf16;
        }

        public virtual IJsonSerializer<T, TJsonWriter> GetSerializer<T, TJsonWriter>()
            where TJsonWriter : JsonWriter {
            throw new NotImplementedException();
        }

        public virtual IJsonDeserializer<T, TJsonReader> GetDeserializer<T, TJsonReader>()
            where TJsonReader : JsonReader {
            throw new NotImplementedException();
        }

    }

    public class JsonDeserializerConfiguration<TForJsonReader>
        : JsonSerializationConfiguration
        where TForJsonReader : JsonReader {
        private readonly JsonSerializationConfiguration _Configuration;

        public JsonDeserializerConfiguration(
                JsonSerializationConfiguration configuration
            ) : base(
                configuration.PropertyNameMutator,
                configuration.NumberHandling,
                configuration.PropertyNameCaseInsensitive
                ) {
            this._Configuration = configuration;
        }

        public override Type? GetReaderType() => typeof(TForJsonReader);

        public override JsonSerializationConfiguration ForDeserialization<TJsonReader>([AllowNull] TJsonReader jsonReader = default)
            => (
            (typeof(TForJsonReader).Equals(typeof(TJsonReader)))
                ? this
                : this._Configuration.ForDeserialization<TJsonReader>()
            );

        public override JsonSerializationConfiguration ForSerialization<TJsonWriter>([AllowNull] TJsonWriter writer = default)
            => this._Configuration.ForSerialization<TJsonWriter>();

        public JsonDeserializerConfiguration<TForJsonReader> Init() {
            foreach (var resolver in this._Configuration.Resolvers) {
                if (resolver is IJsonDeserializerResolverCommon || resolver is IJsonDeserializerResolver<TForJsonReader>) {
                    var boundResolver = resolver.BindForReader(this);
                    if (boundResolver is object) {
                        this.Resolvers.Add(boundResolver);
                    }
                }
            }
            foreach (var formatter in this._Configuration.Formatters) {
                var boundFormatter = formatter.BindForReader(this);
#warning here Formatters
                if (boundFormatter is object) {
                    this.Formatters.Add(boundFormatter);
                }
            }
            return this;
        }

        public override IJsonDeserializer<T, TJsonReader> GetDeserializer<T, TJsonReader>() {
            if (typeof(TForJsonReader).Equals(typeof(TJsonReader))) {
            }
            return base.GetDeserializer<T, TJsonReader>();
        }
    }
    public class JsonSerializerConfiguration<TForJsonWriter>
        : JsonSerializationConfiguration
        where TForJsonWriter : JsonWriter {
        private readonly JsonSerializationConfiguration _Configuration;

        public JsonSerializerConfiguration(
                JsonSerializationConfiguration configuration
            ) : base(
                configuration.PropertyNameMutator,
                configuration.NumberHandling,
                configuration.PropertyNameCaseInsensitive
                ) {
            this._Configuration = configuration;
        }

        public override JsonSerializationConfiguration ForDeserialization<TJsonReader>([AllowNull] TJsonReader reader = default)
            => this._Configuration.ForDeserialization<TJsonReader>(reader);

        public override JsonSerializationConfiguration ForSerialization<TJsonWriter>([AllowNull] TJsonWriter writer = default)
            => (
            (typeof(TForJsonWriter).Equals(typeof(TJsonWriter)))
                ? this
                : this._Configuration.ForSerialization<TJsonWriter>(writer)
            );

        public override Type? GetWriterType() => typeof(TForJsonWriter);

        public JsonSerializerConfiguration<TForJsonWriter> Init() {
            foreach (var resolver in this._Configuration.Resolvers) {
                if (resolver is IJsonSerializerResolverCommon || resolver is IJsonSerializerResolver<TForJsonWriter>) {
                    var boundResolver = resolver.BindForWriter(this);
                    if (boundResolver is object) {
                        this.Resolvers.Add(boundResolver);
                    }
                }
            }
            foreach (var formatter in this._Configuration.Formatters) {
                var boundFormatter = formatter.BindForReader(this);
#warning here Formatters
                if (boundFormatter is object) {
                    this.Formatters.Add(boundFormatter);
                }
            }

            return this;
        }

        public override IJsonSerializer<T, TJsonWriter> GetSerializer<T, TJsonWriter>() {
            return base.GetSerializer<T, TJsonWriter>(writer);
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