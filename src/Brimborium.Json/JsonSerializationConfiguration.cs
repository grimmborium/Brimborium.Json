using Brimborium.Json.Internal;
using Brimborium.Json.Resolvers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public abstract class JsonSerializationConfiguration {
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
                JsonSerializationConfigurationState state
            ) {
            this.Instance = System.Threading.Interlocked.Increment(ref _Count);
            this.PropertyNameMutator = state.PropertyNameMutator;
            this.NumberHandling = state.NumberHandling;
            this.PropertyNameCaseInsensitive = state.PropertyNameCaseInsensitive;
            this.Formatters = new List<IJsonFormatter>();
            this.Formatters.AddRange(state.Formatters);
            this.Resolvers = new List<IJsonFormatterResolver>();
            this.Resolvers.AddRange(state.Resolvers);
        }

        protected JsonSerializationConfiguration(JsonSerializationConfiguration configuration) {
            this.Instance = System.Threading.Interlocked.Increment(ref _Count);
            this.PropertyNameMutator = configuration.PropertyNameMutator;
            this.NumberHandling = configuration.NumberHandling;
            this.PropertyNameCaseInsensitive = configuration.PropertyNameCaseInsensitive;
            this.Formatters = new List<IJsonFormatter>();
            this.Formatters.AddRange(configuration.Formatters);
            this.Resolvers = new List<IJsonFormatterResolver>();
            this.Resolvers.AddRange(configuration.Resolvers);
        }

        //public virtual Type? GetReaderType() => null;
        //public virtual Type? GetWriterType() => null;

        public abstract JsonDeserializerConfigurationUtf8 ForDeserializationUtf8();

        public abstract JsonDeserializerConfigurationUtf16 ForDeserializationUtf16();

        public abstract JsonSerializerConfigurationUtf8 ForSerializationUtf8();

        public abstract JsonSerializerConfigurationUtf16 ForSerializationUtf16();

        public virtual void Serialize<T>(JsonWriter writer, [AllowNull] T value) {
            switch (writer) {
                case JsonWriterUtf8 jsonWriterUtf8:
                    this.ForSerializationUtf8().Serialize<T>(writer, value);
                    break;
                case JsonWriterUtf16 jsonWriterUtf16:
                    this.ForSerializationUtf16().Serialize<T>(writer, value);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public virtual void Serialize<T>(JsonWriterUtf8 writer, [AllowNull] T value)
            => this.ForSerializationUtf8().Serialize<T>(writer, value);

        public virtual void Serialize<T>(JsonWriterUtf16 writer, [AllowNull] T value)
            => this.ForSerializationUtf16().Serialize<T>(writer, value);

        public virtual T Deserialize<T>(JsonReader reader)
            => reader switch {
                JsonReaderUtf8 jsonReaderUtf8 => this.ForDeserializationUtf8().Deserialize<T>(jsonReaderUtf8),
                JsonReaderUtf16 jsonReaderUtf16 => this.ForDeserializationUtf16().Deserialize<T>(jsonReaderUtf16),
                _ => throw new NotSupportedException()
            };

        public virtual T Deserialize<T>(JsonReaderUtf8 reader)
            => this.ForDeserializationUtf8().Deserialize<T>(reader);

        public virtual T Deserialize<T>(JsonReaderUtf16 reader)
            => this.ForDeserializationUtf16().Deserialize<T>(reader);
    }

    public sealed class JsonSerializationConfigurationRoot : JsonSerializationConfiguration {
        internal static JsonSerializationConfiguration Build(JsonSerializationConfigurationState state) {
            var result = new JsonSerializationConfigurationRoot(state);
            return result;
        }

        public JsonSerializationConfigurationRoot(
            JsonSerializationConfigurationState state
            ) : base(state) {
        }

        private JsonDeserializerConfigurationUtf8? _CfgJsonReaderUtf8;
        private JsonDeserializerConfigurationUtf16? _CfgJsonReaderUtf16;
        private JsonSerializerConfigurationUtf8? _CfgJsonWriterUtf8;
        private JsonSerializerConfigurationUtf16? _CfgJsonWriterUtf16;

        public override JsonDeserializerConfigurationUtf8 ForDeserializationUtf8() {
            if (_CfgJsonReaderUtf8 is null) {
                lock (this) {
                    if (_CfgJsonReaderUtf8 is null) {
                        var cfg = new JsonDeserializerConfigurationUtf8(this);
                        cfg.Init();
                        _CfgJsonReaderUtf8 = cfg;
                        System.Threading.Interlocked.MemoryBarrier();
                    }
                }
            }
            return _CfgJsonReaderUtf8;
        }

        public override JsonDeserializerConfigurationUtf16 ForDeserializationUtf16() {
            if (_CfgJsonReaderUtf16 is null) {
                lock (this) {
                    if (_CfgJsonReaderUtf16 is null) {
                        var cfg = new JsonDeserializerConfigurationUtf16(this);
                        cfg.Init();
                        _CfgJsonReaderUtf16 = cfg;
                        System.Threading.Interlocked.MemoryBarrier();
                    }
                }
            }
            return _CfgJsonReaderUtf16;
        }

        public override JsonSerializerConfigurationUtf8 ForSerializationUtf8() {
            if (_CfgJsonWriterUtf8 is null) {
                lock (this) {
                    if (_CfgJsonWriterUtf8 is null) {
                        var cfg = new JsonSerializerConfigurationUtf8(this);
                        cfg.Init();
                        _CfgJsonWriterUtf8 = cfg;
                        System.Threading.Interlocked.MemoryBarrier();
                    }
                }
            }
            return _CfgJsonWriterUtf8;
        }

        public override JsonSerializerConfigurationUtf16 ForSerializationUtf16() {
            if (_CfgJsonWriterUtf16 is null) {
                lock (this) {
                    if (_CfgJsonWriterUtf16 is null) {
                        var cfg = new JsonSerializerConfigurationUtf16(this);
                        cfg.Init();
                        _CfgJsonWriterUtf16 = cfg;
                        System.Threading.Interlocked.MemoryBarrier();
                    }
                }
            }
            return _CfgJsonWriterUtf16;
        }

        public override void Serialize<T>(JsonWriter writer, [AllowNull] T value) {
            switch (writer) {
                case JsonWriterUtf8 jsonWriterUtf8:
                    this.ForSerializationUtf8().Serialize<T>(jsonWriterUtf8, value);
                    break;
                case JsonWriterUtf16 jsonWriterUtf16:
                    this.ForSerializationUtf16().Serialize<T>(jsonWriterUtf16, value);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public override void Serialize<T>(JsonWriterUtf8 writer, [AllowNull] T value)
            => this.ForSerializationUtf8().Serialize<T>(writer, value);

        public override void Serialize<T>(JsonWriterUtf16 writer, [AllowNull] T value)
            => this.ForSerializationUtf16().Serialize<T>(writer, value);

        public override T Deserialize<T>(JsonReader reader)
            => reader switch {
                JsonReaderUtf8 jsonReaderUtf8 => this.ForDeserializationUtf8().Deserialize<T>(jsonReaderUtf8),
                JsonReaderUtf16 jsonReaderUtf16 => this.ForDeserializationUtf16().Deserialize<T>(jsonReaderUtf16),
                _ => throw new NotSupportedException()
            };

        public override T Deserialize<T>(JsonReaderUtf8 reader)
            => this.ForDeserializationUtf8().Deserialize<T>(reader);

        public override T Deserialize<T>(JsonReaderUtf16 reader)
            => this.ForDeserializationUtf16().Deserialize<T>(reader);
    }

    public abstract class JsonDeserializerConfiguration<TForJsonReader>
        : JsonSerializationConfiguration
        where TForJsonReader : JsonReader {
        protected readonly JsonSerializationConfigurationRoot _ConfigurationRoot;

        protected JsonDeserializerConfiguration(
                JsonSerializationConfigurationRoot configurationRoot
            ) : base(configurationRoot) {
            this._ConfigurationRoot = configurationRoot;
        }

        public void Init() {
            foreach (var resolver in this._ConfigurationRoot.Resolvers) {
                if (resolver is IJsonFormatterResolverWithInitialization withInitialization) {
                    var boundResolver = withInitialization.BindForReader(this);
                    if (boundResolver is object) {
                        this.Resolvers.Add(boundResolver);
                        continue;
                    }
                }
                this.Resolvers.Add(resolver);
            }
            foreach (var formatter in this._ConfigurationRoot.Formatters) {
                if (formatter is IJsonFormatterWithInitialization withInitialization) {
                    var boundFormatter = withInitialization.BindForReader(this);
                    if (boundFormatter is object) {
                        this.Formatters.Add(boundFormatter);
                        continue;
                    }
                }
                this.Formatters.Add(formatter);
#warning here Formatters
            }
        }

        public override JsonDeserializerConfigurationUtf8 ForDeserializationUtf8() => this._ConfigurationRoot.ForDeserializationUtf8();
        public override JsonDeserializerConfigurationUtf16 ForDeserializationUtf16() => this._ConfigurationRoot.ForDeserializationUtf16();
        public override JsonSerializerConfigurationUtf8 ForSerializationUtf8() => this._ConfigurationRoot.ForSerializationUtf8();
        public override JsonSerializerConfigurationUtf16 ForSerializationUtf16() => this._ConfigurationRoot.ForSerializationUtf16();

    }

    public sealed class JsonDeserializerConfigurationUtf8 : JsonDeserializerConfiguration<JsonReaderUtf8> {
        internal JsonDeserializerConfigurationUtf8(
                JsonSerializationConfigurationRoot configurationRoot
            ) : base(configurationRoot) {
        }

        public override T Deserialize<T>(JsonReader reader)
            => reader switch {
                JsonReaderUtf8 jsonReaderUtf8 => this.Deserialize<T>(jsonReaderUtf8),
                JsonReaderUtf16 jsonReaderUtf16 => this.ForDeserializationUtf16().Deserialize<T>(jsonReaderUtf16),
                _ => throw new NotSupportedException()
            };

        public override T Deserialize<T>(JsonReaderUtf8 reader) {
            return base.Deserialize<T>(reader);
        }
    }

    public sealed class JsonDeserializerConfigurationUtf16 : JsonDeserializerConfiguration<JsonReaderUtf16> {
        internal JsonDeserializerConfigurationUtf16(
                JsonSerializationConfigurationRoot configurationRoot
            ) : base(configurationRoot) {
        }

        public override T Deserialize<T>(JsonReader reader)
           => reader switch {
               JsonReaderUtf16 jsonReaderUtf16 => this.Deserialize<T>(jsonReaderUtf16),
               JsonReaderUtf8 jsonReaderUtf8 => this.ForDeserializationUtf8().Deserialize<T>(jsonReaderUtf8),
               _ => throw new NotSupportedException()
           };

        public override T Deserialize<T>(JsonReaderUtf16 reader) {
            return base.Deserialize<T>(reader);
        }
    }

    public abstract class JsonSerializerConfiguration<TForJsonWriter>
        : JsonSerializationConfiguration
        where TForJsonWriter : JsonWriter {
        private readonly JsonSerializationConfigurationRoot _ConfigurationRoot;

        protected JsonSerializerConfiguration(
                JsonSerializationConfigurationRoot configurationRoot
            ) : base(configurationRoot) {
            this._ConfigurationRoot = configurationRoot;
        }

        public void Init() {
            foreach (var resolver in this._ConfigurationRoot.Resolvers) {
                var boundResolver = resolver.BindForWriter(this);
                if (boundResolver is object) {
                    this.Resolvers.Add(boundResolver);
                }
            }
            foreach (var formatter in this._ConfigurationRoot.Formatters) {
                var boundFormatter = formatter.BindForWriter(this);
#warning here Formatters
                if (boundFormatter is object) {
                    this.Formatters.Add(boundFormatter);
                }
            }
        }

        public override JsonDeserializerConfigurationUtf8 ForDeserializationUtf8() => this._ConfigurationRoot.ForDeserializationUtf8();
        public override JsonDeserializerConfigurationUtf16 ForDeserializationUtf16() => this._ConfigurationRoot.ForDeserializationUtf16();
        public override JsonSerializerConfigurationUtf8 ForSerializationUtf8() => this._ConfigurationRoot.ForSerializationUtf8();
        public override JsonSerializerConfigurationUtf16 ForSerializationUtf16() => this._ConfigurationRoot.ForSerializationUtf16();
    }

    public sealed class JsonSerializerConfigurationUtf8 : JsonSerializerConfiguration<JsonWriterUtf8> {
        internal JsonSerializerConfigurationUtf8(
                JsonSerializationConfigurationRoot configurationRoot
            ) : base(configurationRoot) {
        }

        public override void Serialize<T>(JsonWriter writer, [AllowNull] T value) {
            switch (writer) {
                case JsonWriterUtf8 jsonWriterUtf8:
                    this.Serialize<T>(jsonWriterUtf8, value);
                    break;
                case JsonWriterUtf16 jsonWriterUtf16:
                    this.ForSerializationUtf16().Serialize<T>(jsonWriterUtf16, value);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public override void Serialize<T>(JsonWriterUtf8 writer, [AllowNull] T value) {
            base.Serialize<T>(writer, value);
        }
    }

    public sealed class JsonSerializerConfigurationUtf16 : JsonSerializerConfiguration<JsonWriterUtf16> {
        internal JsonSerializerConfigurationUtf16(
                JsonSerializationConfigurationRoot configurationRoot
            ) : base(configurationRoot) {
        }

        public override void Serialize<T>(JsonWriter writer, [AllowNull] T value) {
            switch (writer) {
                case JsonWriterUtf16 jsonWriterUtf16:
                    this.Serialize<T>(jsonWriterUtf16, value);
                    break;
                case JsonWriterUtf8 jsonWriterUtf8:
                    this.ForSerializationUtf8().Serialize<T>(jsonWriterUtf8, value);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public override void Serialize<T>(JsonWriterUtf16 writer, [AllowNull] T value) {
            base.Serialize<T>(writer, value);
        }
    }



    /*
    public class JsonSerializationConfigurationSpecWriter<TJsonWriter>
        where TJsonWriter : JsonWriter {
        public JsonSerializationConfigurationSpecWriter() : base() { }
    }

    public class JsonSerializationConfigurationSpecReader<TJsonReader>
        where TJsonReader : JsonReader {
        public JsonSerializationConfigurationSpecReader() : base() { }
    }
    */
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