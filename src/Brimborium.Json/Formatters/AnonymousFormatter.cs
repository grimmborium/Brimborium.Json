#nullable enable

using System;

namespace Brimborium.Json.Formatters {
    public sealed class AnonymousFormatterCommon<T> : IJsonFormatterCommon<T> {
        private readonly JsonSerializeActionCommon<T>? _Serialize;
        private readonly JsonDeserializeFuncCommon<T>? _Deserialize;

        public AnonymousFormatterCommon(JsonSerializeActionCommon<T>? serialize, JsonDeserializeFuncCommon<T>? deserialize) {
            this._Serialize = serialize;
            this._Deserialize = deserialize;
        }

        public T Deserialize(JsonReader reader, JsonSerializationConfiguration configuration) {
            if (this._Deserialize == null) {
                throw new InvalidOperationException(this.GetType().Name + " does not support Deserialize.");
            } else {
                return this._Deserialize(reader, configuration);
            }
        }

        public void Serialize(JsonWriter writer, T value, JsonSerializationConfiguration configuration) {
            if (this._Serialize == null) {
                throw new InvalidOperationException(this.GetType().Name + " does not support Serialize.");
            } else {
                this._Serialize(writer, value, configuration);
            }
        }
    }

    public sealed class AnonymousFormatterUtf816<T> : IJsonFormatterUtf816<T> {
        private readonly JsonSerializeActionUtf8<T>? _SerializeUtf8;
        private readonly JsonSerializeActionUtf16<T>? _SerializeUtf16;
        private readonly JsonDeserializeFuncUtf8<T>? _DeserializeUtf8;
        private readonly JsonDeserializeFuncUtf16<T>? _DeserializeUtf16;

        public AnonymousFormatterUtf816(
                JsonSerializeActionUtf8<T>? serializeUtf8,
                JsonSerializeActionUtf16<T>? serializeUtf16,
                JsonDeserializeFuncUtf8<T>? deserializeUtf8,
                JsonDeserializeFuncUtf16<T>? deserializeUtf16
            ) {
            this._SerializeUtf8 = serializeUtf8;
            this._SerializeUtf16 = serializeUtf16;
            this._DeserializeUtf8 = deserializeUtf8;
            this._DeserializeUtf16 = deserializeUtf16;
        }

        public T Deserialize(JsonReaderUtf8 reader, JsonSerializationConfiguration configuration) {
            if (this._DeserializeUtf8 == null) {
                throw new InvalidOperationException(this.GetType().Name + " does not support Deserialize.");
            } else {
                return this._DeserializeUtf8(reader, configuration);
            }
        }

        public T Deserialize(JsonReaderUtf16 reader, JsonSerializationConfiguration configuration) {
            if (this._DeserializeUtf16 == null) {
                throw new InvalidOperationException(this.GetType().Name + " does not support Deserialize.");
            } else {
                return this._DeserializeUtf16(reader, configuration);
            }
        }

        public void Serialize(JsonWriterUtf8 writer, T value, JsonSerializationConfiguration configuration) {
            if (this._SerializeUtf8 == null) {
                throw new InvalidOperationException(this.GetType().Name + " does not support Serialize.");
            } else {
                this._SerializeUtf8(writer, value, configuration);
            }
        }

        public void Serialize(JsonWriterUtf16 writer, T value, JsonSerializationConfiguration configuration) {
            if (this._SerializeUtf16 == null) {
                throw new InvalidOperationException(this.GetType().Name + " does not support Serialize.");
            } else {
                this._SerializeUtf16(writer, value, configuration);
            }
        }
    }
}
