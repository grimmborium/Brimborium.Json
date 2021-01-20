#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Brimborium.Json {
    public struct JsonSerializerInfo {
        public Type StaticType;
        public Type? DynamicType;
        public JsonSerializer? JsonSerializer;
        public JsonSerializerInfo(Type type) {
            StaticType = type;
            DynamicType = null;
            JsonSerializer = null;
        }
    }

    public class JsonSerializer {
        public JsonSerializer() {
        }
    }
}
