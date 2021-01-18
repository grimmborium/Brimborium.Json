using System;
using System.Collections.Generic;

namespace Brimborium.Json {

    public delegate void JsonSerializeActionCommon<T>(JsonWriter writer, T value, JsonSerializationConfiguration configuration);
    public delegate void JsonSerializeActionUtf8<T>(JsonWriterUtf8 writer, T value, JsonSerializationConfiguration configuration);
    public delegate void JsonSerializeActionUtf16<T>(JsonWriterUtf16 writer, T value, JsonSerializationConfiguration configuration);

    public delegate T JsonDeserializeFuncCommon<T>(JsonReader reader, JsonSerializationConfiguration configuration);
    public delegate T JsonDeserializeFuncUtf8<T>(JsonReaderUtf8 reader, JsonSerializationConfiguration configuration);
    public delegate T JsonDeserializeFuncUtf16<T>(JsonReaderUtf16 reader, JsonSerializationConfiguration configuration);

    public interface IJsonFormatter {
    }
    public interface IJsonFormatterWithInitialization : IJsonFormatter {
        IJsonFormatter BindConfiguration(JsonSerializationConfiguration configuration);
    }

    public interface IJsonFormatter<T>
      : IJsonFormatter { }

    public interface IJsonFormatterCommon<T>
        : IJsonFormatter<T> {
        void Serialize(JsonWriter writer, T value, JsonSerializationConfiguration configuration);
        T Deserialize(JsonReader reader, JsonSerializationConfiguration configuration);
    }

    public interface IJsonFormatterUtf816<T>
        : IJsonFormatter<T> {
        void Serialize(JsonWriterUtf8 writer, T value, JsonSerializationConfiguration configuration);
        void Serialize(JsonWriterUtf16 writer, T value, JsonSerializationConfiguration configuration);

        T Deserialize(JsonReaderUtf8 reader, JsonSerializationConfiguration configuration);
        T Deserialize(JsonReaderUtf16 reader, JsonSerializationConfiguration configuration);
    }


    public interface IObjectPropertyNameFormatter<T> {
        void SerializeToPropertyName(JsonWriter writer, T value, IJsonFormatterResolver formatterResolver);
        T DeserializeFromPropertyName(JsonReader reader, IJsonFormatterResolver formatterResolver);
    }

    public interface IJsonFormatterResolver {
        IJsonFormatter<T> GetFormatter<T>(JsonSerializationConfiguration configuration);
    }

    public interface IJsonFormatterResolverStatic : IJsonFormatterResolver {
        IEnumerable<IJsonFormatter> GetFormatters(JsonSerializationConfiguration configuration);
    }
    public interface IJsonFormatterResolverWithInitialization : IJsonFormatterResolver {
        IJsonFormatterResolver BindConfiguration(JsonSerializationConfiguration configuration);
    }
}