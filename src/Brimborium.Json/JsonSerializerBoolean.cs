#nullable enable


using System.Threading.Tasks;

namespace Brimborium.Json {
    public class JsonSerializerBooleanFactory : JsonSerializerFactory {
        private static JsonSerializerBoolean? jsonSerializerBoolean;
        public JsonSerializerBooleanFactory() {
        }
        public override JsonSerializer[] CreateUtf8(JsonConfiguration configuration) {
            return new JsonSerializer[] { jsonSerializerBoolean ??= new JsonSerializerBoolean() };
        }
        public override JsonSerializer[] CreateUtf16(JsonConfiguration configuration) {
            return new JsonSerializer[] { jsonSerializerBoolean ??= new JsonSerializerBoolean() };
        }
    }
    public class JsonSerializerBoolean : JsonSerializerCommon<bool> {
        public JsonSerializerBoolean() {
        }

        public override void Serialize(bool value, JsonSink jsonSink) {
            if (value) {
                jsonSink.Write(JsonConstText.True);
            } else {
                jsonSink.Write(JsonConstText.False);
            }
        }

        public override async ValueTask<bool> DeserializeAsync(JsonSource jsonSource) {
            JsonToken token;
            if (!jsonSource.TryReadToken(out token)) { token = await jsonSource.ReadCurrentTokenAsync(); }
            if (token.Kind == JsonTokenKind.True) { return true; }
            if (token.Kind == JsonTokenKind.False) { return false; }
            throw new System.Exception($"Unexpected Token {token.Kind}");
        }

        public override ValueTask<bool> DeserializeAsync(JsonSource jsonSource, ref JsonSerializerInfo<bool> jsonSerializerInfo) {
            return this.DeserializeAsync(jsonSource);
        }
    }
}
