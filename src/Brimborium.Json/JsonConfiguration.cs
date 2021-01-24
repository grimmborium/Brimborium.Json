using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Brimborium.Json {

    public class JsonConfiguration {
        public enum JsonConfigurationMode {
            Common, Utf8, Utf16
        }

        private readonly JsonConfigurationMode _Mode;
        private JsonConfiguration? _Parent;
        private JsonConfiguration? _GetForUtf8;
        private JsonConfiguration? _GetForUtf16;

        public readonly List<JsonSerializerFactory> JsonSerializerFactory;
        private Dictionary<Type, JsonSerializer> Type2JsonSerializer;
        public readonly List<JsonSerializerDynamicFactory> JsonSerializerDynamicFactory;

        public JsonConfiguration() {
            this._Mode = JsonConfigurationMode.Common;
            this._Parent = null;
            this.JsonSerializerFactory = new List<JsonSerializerFactory>();
            this.Type2JsonSerializer = new Dictionary<Type, JsonSerializer>();
            this.JsonSerializerDynamicFactory = new List<JsonSerializerDynamicFactory>();
        }

        private JsonConfiguration(JsonConfigurationMode mode, JsonConfiguration parent) {
            this._Mode = mode;
            this._Parent = parent;
            this.JsonSerializerFactory = new List<JsonSerializerFactory>();
            this.Type2JsonSerializer = new Dictionary<Type, JsonSerializer>();
            this.JsonSerializerDynamicFactory = new List<JsonSerializerDynamicFactory>();
        }

        public JsonConfiguration GetForUtf8() {
            if (this._Mode == JsonConfigurationMode.Utf8) {
                return this;
            } else if (this._Mode == JsonConfigurationMode.Common) {
                if (_GetForUtf8 is null) {
                    lock (this) {
                        if (_GetForUtf8 is null) {
                            var configuration = new JsonConfiguration(JsonConfigurationMode.Utf8, this);
                            configuration.JsonSerializerFactory.AddRange(this.JsonSerializerFactory);
                            configuration.Build();
                            System.Threading.Interlocked.CompareExchange(ref this._GetForUtf8, configuration, null);
                        }
                    }
                }
                return this._GetForUtf8;
            } else {
                return this._Parent!.GetForUtf8();
            }
        }

        public JsonConfiguration GetForUtf16() {
            if (this._Mode == JsonConfigurationMode.Utf16) {
                return this;
            } else if (this._Mode == JsonConfigurationMode.Common) {
                if (_GetForUtf16 is null) {
                    lock (this) {
                        if (_GetForUtf16 is null) {
                            var configuration = new JsonConfiguration(JsonConfigurationMode.Utf16, this);
                            configuration.JsonSerializerFactory.AddRange(this.JsonSerializerFactory);
                            configuration.Build();
                            System.Threading.Interlocked.CompareExchange(ref this._GetForUtf16, configuration, null);
                        }
                    }
                }
                return this._GetForUtf16;
            } else {
                return this._Parent!.GetForUtf16();
            }
        }

        private void Build() {
            if (this._Mode == JsonConfigurationMode.Common) {
                return;
            }
            foreach (var factory in this.JsonSerializerFactory) {
                var lstJsonSerializer = this._Mode switch {
                    JsonConfigurationMode.Utf8 => factory.CreateJsonSerializerUtf8(this),
                    JsonConfigurationMode.Utf16 => factory.CreateJsonSerializerUtf16(this),
                    _ => throw new Exception()
                };

                foreach (var jsonSerializer in lstJsonSerializer) {
                    var elementType = jsonSerializer.GetElementType();
                    if (elementType is object) {
                        if (this.Type2JsonSerializer.TryAdd(elementType, jsonSerializer)) {
                            // OK
                        } else {
#warning TODO
                            throw new Exception();
                        }
                    }
                    //this.JsonSerializerDynamicFactory
                }

                var lstDynamicFactory = this._Mode switch {
                    JsonConfigurationMode.Utf8 => factory.CreateDynamicFactoryUtf8(this),
                    JsonConfigurationMode.Utf16 => factory.CreateDynamicFactoryUtf16(this),
                    _ => throw new Exception()
                };
                JsonSerializerDynamicFactory.AddRange(lstDynamicFactory);

            }
        }

        public JsonSerializerInfo<T> PreCalcJsonSerializerInfo<T>() {
            var result = new JsonSerializerInfo<T>(null);
            // may be an alternative
            TryGetSerializerInfo<T>(null, result);
            return result;
        }

        public bool TryGetSerializerInfo<T>(
            Type? currentType,
            JsonSerializerInfo<T> jsonSerializerInfo
            ) {
            throw new NotImplementedException();
            //return false;
        }

        public bool TryGetSerializer<T>(
            Type currentType,
            [MaybeNullWhen(false)] out JsonSerializer jsonSerializer
            ) {
            var result = new JsonSerializerInfo<T>(currentType);
            if (TryGetSerializerInfo(currentType, result)) {
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

        public void Serialize<T>(T value, JsonSink jsonSink, JsonSerializerInfo<T> jsonSerializerInfo) {
            //jsonSink.Context
            throw new NotImplementedException();
        }

        public async ValueTask<T> DeserializeAsync<T>(JsonSource jsonSource, JsonSerializerInfo<T> jsonSerializerInfo) {
            if (jsonSerializerInfo.DynamicJsonSerializer is JsonSerializer<T> dynamicJsonSerializer) {
                return await dynamicJsonSerializer.DeserializeAsync(jsonSource, jsonSerializerInfo);
            }
            if (jsonSerializerInfo.StaticJsonSerializer is JsonSerializer<T> staticJsonSerializer) {
                return await staticJsonSerializer.DeserializeAsync(jsonSource, jsonSerializerInfo);
            }
            throw new NotImplementedException();
        }
    }
}