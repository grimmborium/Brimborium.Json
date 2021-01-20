#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Brimborium.Json {
    public struct JsonSerializerInfo<T> {
        public Type StaticType;
        public Type? DynamicType;
        public JsonSerializer<T>? JsonSerializer;
        public JsonSerializerInfo(Type? dynamicType) {
            StaticType = typeof(T);
            DynamicType = dynamicType;
            JsonSerializer = null;
        }
    }

    public class JsonSerializer {
    }
    public class JsonSerializer<T>: JsonSerializer {
        public JsonSerializer() {
        }
        public virtual void Serialize(T value, JsonSink jsonSink) {
        }
        public virtual T Deserialize(JsonSource jsonSource) {
            throw new NotImplementedException();
        }
    }
}
