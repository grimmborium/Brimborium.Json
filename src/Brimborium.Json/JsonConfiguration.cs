﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.Json {
    public class JsonConfiguration {
        public void Serialize<T>(T value, JsonSink jsonSink) {
            throw new NotImplementedException();
        }
        public T Deserialize<T>(JsonSource jsonSource) {
            throw new NotImplementedException();
        }
        public JsonSerializerInfo<T> PreCalcJsonSerializerInfo<T>() {
            var result = new JsonSerializerInfo<T>(null);
            return result;
        }
        public bool TryGetSerializerInfo<T>(
            Type? currentType,
            ref JsonSerializerInfo<T> jsonSerializerInfo
            ) {
            return false;
        }

        public bool TryGetSerializer<T>(
            Type currentType,
            [MaybeNullWhen(false)] out JsonSerializer jsonSerializer
            ) {
            var result = new JsonSerializerInfo<T>(currentType);
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