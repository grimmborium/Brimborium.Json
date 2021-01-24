#nullable enable


using System;
using System.Threading.Tasks;

namespace Brimborium.Json {
    public class JsonSerializerBooleanFactory : JsonSerializerFactory {
        private static JsonSerializerBoolean? jsonSerializerBoolean;
        public JsonSerializerBooleanFactory() {
        }
        public override JsonSerializer[] CreateJsonSerializerUtf8(JsonConfiguration configuration) {
            return new JsonSerializer[] { jsonSerializerBoolean ??= new JsonSerializerBoolean() };
        }
        public override JsonSerializer[] CreateJsonSerializerUtf16(JsonConfiguration configuration) {
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

        public override async ValueTask<bool> DeserializeAsync(Type? currentType, JsonSource jsonSource) {
            //JsonToken token;
            //if (!jsonSource.TryReadToken(out token)) { token = await jsonSource.ReadCurrentTokenAsync(); }
            //if (token.Kind == JsonTokenKind.True) { return true; }
            //if (token.Kind == JsonTokenKind.False) { return false; }
            //throw new System.Exception($"Unexpected Token {token.Kind}");

            if (jsonSource.EnsureTokens()) { await jsonSource.EnsureTokensAsync(); }
            switch (jsonSource.CurrentToken.Kind) {
                case JsonTokenKind.True: jsonSource.Advance(); return true;
                case JsonTokenKind.False: jsonSource.Advance(); return false;
                default:
                    throw new System.Exception($"Unexpected Token {jsonSource.CurrentToken}");
            }

        }

        //public override async ValueTask<bool> DeserializeAsync(JsonSource jsonSource, JsonSerializerInfo<bool> jsonSerializerInfo) {
        //    if (jsonSource.IsFeedNeeded()) { await jsonSource.FeedAsync(); }
        //    switch(jsonSource.CurrentToken.Kind){
        //        case JsonTokenKind.True: jsonSource.MoveNext(); return true;
        //        case JsonTokenKind.False: jsonSource.MoveNext(); return false;
        //        default:
        //            throw new System.Exception($"Unexpected Token {jsonSource.CurrentToken}");
        //    }
        //}
    }
}
