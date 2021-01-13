using System;

namespace Brimborium.Json {
    public delegate void JsonSerializeAction<T>(JsonWriter writer, T value, IJsonFormatterResolver resolver);

    public delegate T JsonDeserializeFunc<T>(JsonReader reader, IJsonFormatterResolver resolver);


    public delegate void JsonSerializeAction2<T>(JsonWriter writer, T value, JsonSerializationConfiguration configuration);

    public delegate T JsonDeserializeFunc2<T>(JsonReader reader, JsonSerializationConfiguration configuration);

    public interface IJsonFormatter {
        IJsonFormatter? BindForReader(JsonSerializationConfiguration configuration) => this;

        IJsonFormatter? BindForWriter(JsonSerializationConfiguration configuration) => this;
    }

    public interface IJsonSerializer<T> : IJsonFormatter {
        void Serialize(JsonWriter writer, T value, IJsonFormatterResolver formatterResolver);
    }

    public interface IJsonDeserializer<T> : IJsonFormatter {
        T Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver);
    }

    public interface IJsonFormatter<T>
        : IJsonFormatter
        , IJsonSerializer<T>
        , IJsonDeserializer<T> {
    }

    public interface IJsonFormatterTyped<T>
        : IJsonFormatter { }

    public interface IJsonFormatterCommon<T>
        : IJsonFormatterTyped<T> {
        void Serialize(JsonWriter writer, T value, JsonSerializationConfiguration configuration);
        T Deserialize(JsonReader reader, JsonSerializationConfiguration configuration);
    }

    public interface IJsonSerializerUtf816<T>
       : IJsonFormatterTyped<T> {
        void Serialize(JsonWriterUtf8 writer, T value, JsonSerializationConfiguration configuration);
        void Serialize(JsonWriterUtf16 writer, T value, JsonSerializationConfiguration configuration);
    }

    public interface IJsonDeserializerUtf816<T>
        : IJsonFormatterTyped<T> {
        T Deserialize(JsonReaderUtf8 reader, JsonSerializationConfiguration configuration);
        T Deserialize(JsonReaderUtf16 reader, JsonSerializationConfiguration configuration);
    }

    public interface IJsonFormatterUtf816<T>
        : IJsonSerializerUtf816<T>
        , IJsonDeserializerUtf816<T> {
    }


    public interface IObjectPropertyNameFormatter<T> {
        void SerializeToPropertyName(JsonWriter writer, T value, IJsonFormatterResolver formatterResolver);
        T DeserializeFromPropertyName(JsonReader reader, IJsonFormatterResolver formatterResolver);
    }

    //public interface IObjectPropertyNameFormatterRW<T, TJsonWriter, TJsonReader>
    //    where TJsonWriter : JsonWriter
    //    where TJsonReader : JsonReader {
    //    void SerializeToPropertyNameRW(TJsonWriter writer, T value, IJsonFormatterResolver formatterResolver);
    //    T DeserializeFromPropertyNameRW(TJsonReader reader, IJsonFormatterResolver formatterResolver);
    //}


    public static class JsonFormatterExtensions {
        public static string ToJsonString<T>(this IJsonFormatter<T> formatter, T value, IJsonFormatterResolver formatterResolver) {
            var writer = new JsonWriterUtf8();
            formatter.Serialize(writer, value, formatterResolver);
            return writer.ToString();
        }
    }
}