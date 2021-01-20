using System;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Json {
    public class JsonConfiguration {
        public void Serialize<T>(T value, JsonSink jsonSink) {
            throw new NotImplementedException();
        }
        public T Deserialize<T>(JsonSource jsonSource) {
            throw new NotImplementedException();
        }
        public JsonSerializerInfo PreCalcJsonSerializerInfo(Type type) {
            var result = new JsonSerializerInfo(type);
            return result;
        }
        public bool TryGetSerializerInfo(
            Type? currentType,
            ref JsonSerializerInfo jsonSerializerInfo
            ) {
            return false;
        }

        public bool TryGetSerializer(
            Type? currentType,
            [MaybeNullWhen(false)] out JsonSerializer jsonSerializer
            ) {
            var result = new JsonSerializerInfo(currentType);
            if (TryGetSerializerInfo(currentType, ref result)) {
                if (result.JsonSerializer is object) {
                    jsonSerializer = result.JsonSerializer;
                    return true;
                }
            }
            {
                jsonSerializer = null;
                return false;
            }
        }
    }
}