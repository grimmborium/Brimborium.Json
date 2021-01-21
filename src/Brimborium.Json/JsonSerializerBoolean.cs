#nullable enable


using System.Threading.Tasks;

namespace Brimborium.Json {
    public class JsonSerializerBoolean : JsonSerializerFactory {
        public JsonSerializerBoolean() {
        }
        public override JsonSerializer[] CreateUtf8(JsonConfiguration configuration) {
            return new JsonSerializer[] { new JsonSerializerBooleanUtf8() };
        }
        public override JsonSerializer[] CreateUtf16(JsonConfiguration configuration) {
            return new JsonSerializer[] { new JsonSerializerBooleanUtf16() };
        }
    }
    public class JsonSerializerBooleanUtf8 : JsonSerializerUtf8<bool> {
        public JsonSerializerBooleanUtf8() {
        }

        public override async ValueTask<bool> DeserializeAsync(JsonSource jsonSource, JsonReaderContext jsonContext) {
            var jsonSourceUtf8 = (JsonSourceUtf8)jsonSource;
            if (!jsonSourceUtf8.TryGetNextToken()) { await jsonSourceUtf8.GetNextTokenAsync(); }
            if (jsonSource.JsonToken.Kind == JsonTokenKind.Value) {
                if (jsonSource.JsonToken.IsValidUtf8) {
                    jsonSource.JsonToken.GetSpanUtf8();
                    jsonSource.JsonToken.GetSpanUtf16();
                    //JsonConstText.True
                }
                if (jsonSource.JsonToken.IsEqual(JsonConstText.True)) {
                    return true;
                } else if (jsonSource.JsonToken.IsEqual(JsonConstText.False)) {
                    return false;
                }
            }
            throw new System.Exception("");
            //jsonSource.JsonToken.GetSpanUtf8();
            //jsonSource.JsonToken.GetSpanUtf16();
            //return base.DeserializeAsync(jsonSource, jsonContext);
        }

        public override ValueTask<bool> DeserializeAsync(JsonSource jsonSource, JsonReaderContext jsonContext, ref JsonSerializerInfo<bool> jsonSerializerInfo) {
            var jsonSourceUtf8 = (JsonSourceUtf8)jsonSource;
            return base.DeserializeAsync(jsonSource, jsonContext, ref jsonSerializerInfo);
        }

        public override void Serialize(bool value, JsonSink jsonSink, JsonWriterContext jsonContext) {
            var jsonSinkUtf8 = (JsonSinkUtf8)jsonSink;
            base.Serialize(value, jsonSink, jsonContext);
        }
    }
    public class JsonSerializerBooleanUtf16 : JsonSerializerUtf16<bool> {
        public JsonSerializerBooleanUtf16() {
        }

        public override ValueTask<bool> DeserializeAsync(JsonSource jsonSource, JsonReaderContext jsonContext) {
            var jsonSourceUtf16 = (JsonSourceUtf16)jsonSource;
            return base.DeserializeAsync(jsonSource, jsonContext);
        }

        public override ValueTask<bool> DeserializeAsync(JsonSource jsonSource, JsonReaderContext jsonContext, ref JsonSerializerInfo<bool> jsonSerializerInfo) {
            var jsonSourceUtf16 = (JsonSourceUtf16)jsonSource;
            return base.DeserializeAsync(jsonSource, jsonContext, ref jsonSerializerInfo);
        }

        public override void Serialize(bool value, JsonSink jsonSink, JsonWriterContext jsonContext) {
            var jsonSinkUtf16 = (JsonSinkUtf16)jsonSink;
            base.Serialize(value, jsonSink, jsonContext);
        }
    }
}
