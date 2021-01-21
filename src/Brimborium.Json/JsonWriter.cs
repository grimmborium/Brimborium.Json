#pragma warning disable IDE0041 // Use 'is null' check

#nullable enable

using System;
using System.Threading.Tasks;
//
namespace Brimborium.Json {
    // JSON RFC: https://www.ietf.org/rfc/rfc4627.txt
    public struct JsonWriter {
        public readonly JsonSink JsonSink;

        public JsonWriter(JsonSink jsonSink) {
            this.JsonSink = jsonSink;
        }

        public void Serialize<T>(T value) {
            if (typeof(T).IsValueType) {
                // valuetypes will be boxed - avoid
            } else if (ReferenceEquals(value, null)) {
                this.JsonSink.Write(JsonConstText.Null);
                return;
            }
            {
                var configuration = this.JsonSink.Configuration;
                var jsonSerializerInfo = configuration.PreCalcJsonSerializerInfo<T>();
                if (configuration.TryGetSerializerInfo<T>(value?.GetType(), ref jsonSerializerInfo)) {
                    var jsonContext = new JsonWriterContext();
                    this.JsonSink.Configuration.Serialize<T>(value, this.JsonSink, jsonContext, ref jsonSerializerInfo);
                    return;
                } else {
                    throw new FormatterNotRegisteredException(typeof(T).FullName ?? typeof(T).Name);
                }
                /*
                if (this.JsonSink.Configuration.TryGetSerializer<T>(value!.GetType(), out var jsonSerializer)) {
                    JsonContext jsonContext = new JsonContext();
                    this.JsonSink.Configuration.Serialize<T>(value, this.JsonSink, jsonContext, jsonSerializer);
                    return;
                } else {
                    throw new FormatterNotRegisteredException(typeof(T).FullName);
                }
                */
            }
        }

        public void Flush() => this.JsonSink.Flush();

        public Task FlushAsync() => this.JsonSink.FlushAsync();

        //    public virtual int CurrentOffset {
        //        get {
        //            throw new InvalidProgramException();
        //        }
        //    }

        //    protected JsonWriter() {
        //    }

        //    public abstract void EnsureCapacity(int offset, int appendLength);
        //    public abstract ArraySegment<byte> GetBuffer();
        //    public abstract byte[] ToUtf8ByteArray();
        //    public abstract void AdvanceOffset(int offset);
        //    public abstract void EnsureCapacity(int appendLength);
        //    public abstract void WriteBeginArray();
        //    public abstract void WriteBeginObject();
        //    public abstract void WriteBoolean(bool value);
        //    public abstract void WriteByte(byte value);
        //    public abstract void WriteDouble(double value);
        //    public abstract void WriteEndArray();
        //    public abstract void WriteEndObject();
        //    public abstract void WriteFalse();
        //    public abstract void WriteInt16(short value);
        //    public abstract void WriteInt32(int value);
        //    public abstract void WriteInt64(long value);
        //    public abstract void WriteNameSeparator();
        //    public abstract void WriteNull();
        //    public abstract void WritePropertyName(string propertyName);
        //    public abstract void WriteStartProperty(JsonSerializationInfo jsonSerializationInfo, int key);
        //    public abstract void WriteQuotation();
        //    public abstract void WriteRaw(byte rawValue);
        //    public abstract void WriteRaw(byte[] rawValue);
        //    public abstract void WriteRawUnsafe(byte rawValue);
        //    public abstract void WriteSByte(sbyte value);
        //    public abstract void WriteSingle(Single value);
        //    public abstract void WriteString(string? value);
        //    public abstract void WriteStringWithoutQuotation(string value);
        //    public abstract void WriteTrue();
        //    public abstract void WriteUInt16(ushort value);
        //    public abstract void WriteUInt32(uint value);
        //    public abstract void WriteUInt64(ulong value);
        //    public abstract void WriteValueSeparator();
        //}
    }
    public class JsonWriterContext { }
}