#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Brimborium.Json {
    public class JsonSerializerInfo<T> {
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
        private static JsonSerializer[]? _EmptyArrayJsonSerializer;
        private static JsonSerializerDynamicFactory[]? _EmptyArrayJsonSerializerDynamicFactory;

        public JsonSerializerFactory() {
        }

        public virtual JsonSerializer[] CreateJsonSerializerUtf8(JsonConfiguration configuration)
            => (_EmptyArrayJsonSerializer ??= new JsonSerializer[0]);

        public virtual JsonSerializer[] CreateJsonSerializerUtf16(JsonConfiguration configuration)
            => (_EmptyArrayJsonSerializer ??= new JsonSerializer[0]);

        public virtual JsonSerializerDynamicFactory[] CreateDynamicFactoryUtf8(JsonConfiguration configuration)
            => (_EmptyArrayJsonSerializerDynamicFactory ??= new JsonSerializerDynamicFactory[0]);

        public virtual JsonSerializerDynamicFactory[] CreateDynamicFactoryUtf16(JsonConfiguration configuration)
            => (_EmptyArrayJsonSerializerDynamicFactory ??= new JsonSerializerDynamicFactory[0]);
    }

    public class JsonSerializerDynamicFactory {
        private static JsonSerializer[]? _EmptyArrayJsonSerializer;
        public JsonSerializerDynamicFactory() {

        }

        public virtual JsonSerializer? CreateJsonSerializerUtf8(Type type, JsonConfiguration configuration) => default;

        public virtual JsonSerializer? CreateJsonSerializerUtf16(Type type, JsonConfiguration configuration) => default;

    }

    public class JsonSerializer {
        public JsonSerializer() {
        }

        public virtual Type GetElementType() => null!;
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

        public override Type GetElementType() => typeof(T);

        public virtual void Serialize(T value, JsonSink jsonSink) {
        }

        public virtual ValueTask<T> DeserializeAsync(Type? currentType, JsonSource jsonSource) {
            throw new NotImplementedException();
        }

        //public virtual ValueTask<T> DeserializeAsync(JsonSource jsonSource, JsonSerializerInfo<T> jsonSerializerInfo) {
        //    throw new NotImplementedException();
        //}
    }

    public class JsonSerializerUtf8<T> : JsonSerializer<T> {
    }
    public class JsonSerializerUtf16<T> : JsonSerializer<T> {
    }
    public class JsonSerializerCommon<T> : JsonSerializer<T> {
    }
}
