using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace Brimborium.Json.Resolvers {
    public class BuiltinResolvers {
        public static IEnumerable<IJsonFormatterResolver> GetResolvers() {
            return Array.Empty<IJsonFormatterResolver>();
        }

        public static IEnumerable<IJsonFormatter> GetFormatters() {
            return Array.Empty<IJsonFormatter>();
        }
    }

    //public sealed class Int32Formatter2 
    //    : IJsonDeserializer<Int32, Utf8JsonReader>
    //    , IJsonSerializer<Int32, Utf8JsonWriter> 
    //    , IJsonFormatter2<Int32>
    //    ,        IObjectPropertyNameFormatter<Int32> {
    //    //public static readonly Int32Formatter Default = new Int32Formatter();

    //    public void Serialize(JsonWriter writer, Int32 value, IJsonFormatterResolver formatterResolver) {
    //        writer.WriteInt32(value);
    //    }

    //    public Int32 Deserialize(JsonReader reader, IJsonFormatterResolver formatterResolver) {
    //        return reader.ReadInt32();
    //    }

    //    public void SerializeToPropertyName(JsonWriter writer, Int32 value, IJsonFormatterResolver formatterResolver) {
    //        writer.WriteQuotation();
    //        writer.WriteInt32(value);
    //        writer.WriteQuotation();
    //    }

    //    public Int32 DeserializeFromPropertyName(JsonReader reader, IJsonFormatterResolver formatterResolver) {
    //        var key = reader.ReadStringSegmentRaw();
    //        int _;
    //        return NumberConverter.ReadInt32(key.Array, key.Offset, out _);
    //    }
    //}
}
