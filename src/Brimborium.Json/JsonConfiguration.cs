#pragma warning disable IDE0041 // Use 'is null' check

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
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
            if (this.Type2JsonSerializer.TryGetValue(typeof(T), out var staticJsonSerializer)) {
                if (staticJsonSerializer is JsonSerializer<T> staticJsonSerializerT) {
                    result.StaticJsonSerializer = staticJsonSerializerT;
                }
            }
            return result;
        }

        public bool TryGetSerializerInfo<T>(
            Type? currentType,
            JsonSerializerInfo<T> jsonSerializerInfo
            ) {
            if (currentType is null) {
                if (jsonSerializerInfo.StaticJsonSerializer is null) {
                    if (this.Type2JsonSerializer.TryGetValue(jsonSerializerInfo.StaticType ?? typeof(T), out var jsonSerializer)) {
                        if (jsonSerializer is JsonSerializer<T> staticJsonSerializer) {
                            jsonSerializerInfo.StaticJsonSerializer = staticJsonSerializer;
                            return true;
                        } else {
                            return false;
                        }
                    }
                } else {
                    return true;
                }
            } else {
                if ((jsonSerializerInfo.DynamicType is object)
                    && jsonSerializerInfo.DynamicType.Equals(currentType)) {
                    return true;
                }
                if ((jsonSerializerInfo.DynamicType is null)
                    || !jsonSerializerInfo.DynamicJsonSerializer.Equals(currentType)) {
                    jsonSerializerInfo.DynamicType = currentType;
                    if (this.Type2JsonSerializer.TryGetValue(currentType, out var jsonSerializer)) {
                        if (jsonSerializer is JsonSerializer<T> dynamicJsonSerializer) {
                            jsonSerializerInfo.DynamicJsonSerializer = dynamicJsonSerializer;
                            return true;
                        } else {
                            jsonSerializerInfo.DynamicJsonSerializer = null;
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        public bool TryGetSerializer<T>(
            Type currentType,
            JsonSerializerInfo<T>? jsonSerializerInfo,
            [MaybeNullWhen(false)] out JsonSerializer<T> jsonSerializer
            ) {
            if (jsonSerializerInfo is null) {
                jsonSerializerInfo = PreCalcJsonSerializerInfo<T>();
            }
            if (TryGetSerializerInfo(currentType, jsonSerializerInfo)) {
                if (jsonSerializerInfo.DynamicJsonSerializer is JsonSerializer<T>) {
                    jsonSerializer = jsonSerializerInfo.DynamicJsonSerializer;
                    return true;
                }
                if (jsonSerializerInfo.StaticJsonSerializer is JsonSerializer<T>) {
                    jsonSerializer = jsonSerializerInfo.StaticJsonSerializer;
                    return true;
                }
            }
            {
                jsonSerializer = null;
                return false;
            }
        }

        public void Serialize<T>(T value, JsonSink jsonSink, JsonSerializerInfo<T> jsonSerializerInfo) {
            if (typeof(T).IsValueType) {
                // not null
            } else if (ReferenceEquals(value, null)) {
                jsonSink.Write(JsonConstText.Null);
                return;
            }
            if (this.TryGetSerializer<T>(value.GetType(), null, out var jsonSerializer)) {
                jsonSerializer.Serialize(value, jsonSink);
            } else {
                throw new FormatterNotRegisteredException($"Seríalizer for {typeof(T).FullName ?? typeof(T).Name} not found.");
            }
        }

        public async ValueTask<T> DeserializeAsync<T>(Type? currentType, JsonSource jsonSource, JsonSerializerInfo<T> jsonSerializerInfo) {
            if (this.TryGetSerializer<T>(currentType, jsonSerializerInfo, out var jsonSerializer)) {
                return await jsonSerializer.DeserializeAsync(currentType, jsonSource);
            } else {
                throw new FormatterNotRegisteredException($"Seríalizer for {typeof(T).FullName ?? typeof(T).Name} not found.");
            }
            //  if (jsonSerializerInfo.DynamicJsonSerializer is JsonSerializer<T> dynamicJsonSerializer) {
            //      return await dynamicJsonSerializer.DeserializeAsync(jsonSource, jsonSerializerInfo);
            //  }
            //  if (jsonSerializerInfo.StaticJsonSerializer is JsonSerializer<T> staticJsonSerializer) {
            //      return await staticJsonSerializer.DeserializeAsync(jsonSource, jsonSerializerInfo);
            //  }
            //  throw new FormatterNotRegisteredException($"Seríalizer for {typeof(T).FullName ?? typeof(T).Name} not found.");
        }
    }
}