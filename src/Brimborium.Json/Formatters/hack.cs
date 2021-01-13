using System;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Json.Formatters {

    public sealed class DecimalFormatterUtf816
        : IJsonSerializerUtf816<decimal>
        , IJsonDeserializerUtf816<decimal> {
        //    public static readonly IJsonFormatter<decimal> Default = new DecimalFormatter();

        //    readonly bool serializeAsString;

        //    public DecimalFormatter()
        //        : this(false) {
        //    }

        //    public DecimalFormatter(bool serializeAsString) {
        //        this.serializeAsString = serializeAsString;
        //    }

        //    public void Serialize(JsonWriter writer, decimal value, IJsonFormatterResolver formatterResolver) {
        //        //if (writer is JsonWriterUtf8 jsonWriterUtf8) {
        //        //    ((IJsonSerializer<decimal, JsonWriterUtf8>)this).SerializeSpec(jsonWriterUtf8, value, formatterResolver);
        //        //} else { 
        //        throw new NotSupportedException();
        //        //}
        //    }

        //    public decimal Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver) {
        //        //if (reader is JsonReaderUtf8 jsonReaderUtf8) {
        //        //    return ((IJsonDeserializer<decimal, JsonReaderUtf8>)this).DeserializeSpec(jsonReaderUtf8, formatterResolver);
        //        //} else {
        //        throw new NotSupportedException();
        //        //}
        //    }

        //    void IJsonSerializer<decimal, JsonWriterUtf8>.Serialize(JsonWriterUtf8 writer, decimal value, JsonSerializationConfiguration configuration) {
        //        if (serializeAsString) {
        //            writer.WriteString(value.ToString(CultureInfo.InvariantCulture));
        //        } else {
        //            // write as number format.
        //            writer.WriteRaw(StringEncoding.UTF8NoBOM.GetBytes(value.ToString(CultureInfo.InvariantCulture)));
        //        }
        //    }

        //    decimal IJsonDeserializer<decimal, JsonReaderUtf8>.Deserialize(JsonReaderUtf8 reader, JsonSerializationConfiguration configuration) {
        //        var token = reader.GetCurrentJsonToken();
        //        if (token == JsonToken.Number) {
        //            var number = reader.ReadNumberSegment();
        //            return decimal.Parse(StringEncoding.UTF8NoBOM.GetString(number.Array, number.Offset, number.Count), NumberStyles.Float, CultureInfo.InvariantCulture);
        //        } else if (token == JsonToken.String && configuration.PropertyNameCaseInsensitive) {
        //            return decimal.Parse(reader.ReadString(), NumberStyles.Float, CultureInfo.InvariantCulture);
        //        } else {
        //            throw new InvalidOperationException("Invalid Json Token for DecimalFormatter:" + token);
        //        }
        //    }
        //}

        decimal IJsonDeserializer<decimal, JsonReaderUtf8>.Deserialize(JsonReaderUtf8 reader, JsonSerializationConfiguration configuration) {
            throw new NotImplementedException();
        }
        decimal IJsonDeserializer<decimal, JsonReaderUtf16>.Deserialize(JsonReaderUtf16 reader, JsonSerializationConfiguration configuration) {
            throw new NotImplementedException();
        }

        void IJsonSerializer<decimal, JsonWriterUtf8>.Serialize(JsonWriterUtf8 writer, decimal value, JsonSerializationConfiguration configuration) {
            throw new NotImplementedException();
        }

        void IJsonSerializer<decimal, JsonWriterUtf16>.Serialize(JsonWriterUtf16 writer, decimal value, JsonSerializationConfiguration configuration) {
            throw new NotImplementedException();
        }
    }
}
