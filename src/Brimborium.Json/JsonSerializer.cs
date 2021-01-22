#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Brimborium.Json {
    public struct JsonSerializerInfo<T> {
        public readonly Type StaticType;

        public JsonSerializer<T>? StaticJsonSerializer;


        public Type? DynamicType;

        public JsonSerializer<T>? DynamicJsonSerializer;

        public JsonSerializerInfo(Type? dynamicType) {
            StaticType = typeof(T);
            DynamicType = dynamicType;
            StaticJsonSerializer = null;
            DynamicJsonSerializer = null;
        }
    }


    public class JsonSerializerFactory {
        public JsonSerializerFactory() {
        }

        public virtual JsonSerializer[] CreateUtf8(JsonConfiguration configuration) { return new JsonSerializer[0]; }

        public virtual JsonSerializer[] CreateUtf16(JsonConfiguration configuration) { return new JsonSerializer[0]; }
    }

    public class JsonSerializer {
        public JsonSerializer() {
        }
    }

    public struct DeserializeResult<T> {
        public bool Success;
        public T Value;

        public DeserializeResult(bool success) {
            this = default;
            Success = success;
        }

        public DeserializeResult(bool success, T value) {
            this.Success = success;
            this.Value = value;
        }
    }
    public class JsonSerializer<T> : JsonSerializer {
        public JsonSerializer() {
        }

        public virtual void Serialize(T value, JsonSink jsonSink) {
        }

        public virtual ValueTask<T> DeserializeAsync(JsonSource jsonSource) {
            throw new NotImplementedException();
        }

        public virtual ValueTask<T> DeserializeAsync(JsonSource jsonSource, ref JsonSerializerInfo<T> jsonSerializerInfo) {
            throw new NotImplementedException();
        }
    }

    public class JsonSerializerUtf8<T> : JsonSerializer<T> {
    }
    public class JsonSerializerUtf16<T> : JsonSerializer<T> {
    }
    public class JsonSerializerCommon<T> : JsonSerializer<T> {
    }
}
