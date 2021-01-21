using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Brimborium.Json {
    public class JsonConfiguration {
        public JsonConfiguration() {
        }

        public JsonConfiguration GetForUtf8() {
            return this;
        }

        public JsonConfiguration GetForUtf16() {
            return this;
        }


        public JsonSerializerInfo<T> PreCalcJsonSerializerInfo<T>() {
            var result = new JsonSerializerInfo<T>(null);
            // may be an alternative
            TryGetSerializerInfo<T>(null, ref result);
            return result;
        }

        public bool TryGetSerializerInfo<T>(
            Type? currentType,
            ref JsonSerializerInfo<T> jsonSerializerInfo
            ) {
            throw new NotImplementedException();
            //return false;
        }

        public bool TryGetSerializer<T>(
            Type currentType,
            [MaybeNullWhen(false)] out JsonSerializer jsonSerializer
            ) {
            var result = new JsonSerializerInfo<T>(currentType);
            if (TryGetSerializerInfo(currentType, ref result)) {
                if (result.DynamicJsonSerializer is object) {
                    jsonSerializer = result.DynamicJsonSerializer;
                    return true;
                }
                if (result.StaticJsonSerializer is object) {
                    jsonSerializer = result.StaticJsonSerializer;
                    return true;
                }
            }
            {
                jsonSerializer = null;
                return false;
            }
        }

        public void Serialize<T>(T value, JsonSink jsonSink, JsonWriterContext jsonContext, ref JsonSerializerInfo<T> jsonSerializerInfo) {
            throw new NotImplementedException();
        }

        public ValueTask<T> DeserializeAsync<T>(JsonSource jsonSource, JsonReaderContext jsonContext, ref JsonSerializerInfo<T> jsonSerializerInfo) {
            throw new NotImplementedException();
        }
    }
}