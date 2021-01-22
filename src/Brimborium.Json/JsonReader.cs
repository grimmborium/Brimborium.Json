using System;
using System.Data;
using System.Threading.Tasks;

namespace Brimborium.Json {
    // JSON RFC: https://www.ietf.org/rfc/rfc4627.txt

    public struct JsonReader {
        public readonly JsonSource JsonSource;

        public JsonReader(JsonSource jsonSource) {
            this.JsonSource = jsonSource;
        }

        public T Deserialize<T>() {
            throw new NotImplementedException();
        }

        public async ValueTask<T> DeserializeAsync<T>() {
            var configuration = this.JsonSource.Configuration;
            var jsonSerializerInfo = configuration.PreCalcJsonSerializerInfo<T>();
            if (configuration.TryGetSerializerInfo<T>(null, ref jsonSerializerInfo)) {
                var jsonReaderContext = JsonReaderContextPool.Instance.Rent();
                var result = await this.JsonSource.Configuration.DeserializeAsync<T>(this.JsonSource, jsonReaderContext, ref jsonSerializerInfo);
                JsonReaderContextPool.Instance.Return(jsonReaderContext);
                return result;
            } else {
                throw new FormatterNotRegisteredException(typeof(T).FullName ?? typeof(T).Name);
            }
            //if (typeof(T).IsValueType) {
            //    // valuetypes will be boxed - avoid
            //} else if (ReferenceEquals(value, null)) {
            //    this.JsonSource.Write(JsonConstText.Null);
            //}
            /*
                if (this.JsonSource.Configuration.TryGetSerializer<T>(null, out var jsonSerializer)) {
                JsonContext jsonContext = new JsonContext();
                    await this.JsonSource.Configuration.DeserializeAsync<T>(this.JsonSource, jsonContext, jsonSerializer);
                }
            */
        }

        //public object Parse(Type? type) {
        //    return this.JsonSource.Parse(type);
        //}
        //public ValueTask<object> ParseAsync(Type? type) {
        //    return this.JsonSource.ParseAsync(type);
        //}


        //        public abstract ArraySegment<byte> ReadPropertyNameSegmentRaw();
        //        public abstract ArraySegment<byte> ReadStringSegmentRaw();
        //        public abstract ArraySegment<byte> ReadStringSegmentUnsafe();
        //        public abstract bool TryGetParameterValue(JsonSerializationInfo jsonSerializationInfo, out int key);
        //        public abstract Double ReadDouble();
        //        public abstract JsonToken GetCurrentJsonToken();
        //        public abstract Single ReadSingle();
        //        public abstract bool ReadBoolean();
        //        public abstract bool ReadIsBeginArray();
        //        public abstract bool ReadIsBeginObject();
        //        public abstract bool ReadIsEndArray();
        //        public abstract bool ReadIsEndArrayWithSkipValueSeparator(ref int count);
        //        public abstract bool ReadIsEndObject();
        //        public abstract bool ReadIsEndObjectWithSkipValueSeparator(ref int count);
        //        public abstract bool ReadIsInArray(ref int count);
        //        public abstract bool ReadIsInObject(ref int count);
        //        public abstract bool ReadIsNameSeparator();
        //        public abstract bool ReadIsNull();
        //        public abstract bool ReadIsValueSeparator();
        //        public abstract byte ReadByte();
        //        public abstract byte[] GetBufferUnsafe();
        //        public abstract int GetCurrentOffsetUnsafe();
        //        public abstract int ReadInt32();
        //        public abstract long ReadInt64();
        //        public abstract sbyte ReadSByte();
        //        public abstract short ReadInt16();
        //        public abstract string ReadPropertyName();
        //        public abstract string ReadString();
        //        public abstract uint ReadUInt32();
        //        public abstract ulong ReadUInt64();
        //        public abstract ushort ReadUInt16();
        //        public abstract void AdvanceOffset(int offset);
        //        public abstract void ReadIsBeginArrayWithVerify();
        //        public abstract void ReadIsBeginObjectWithVerify();
        //        public abstract void ReadIsEndArrayWithVerify();
        //        public abstract void ReadIsEndObjectWithVerify();
        //        public abstract void ReadIsNameSeparatorWithVerify();
        //        public abstract void ReadIsValueSeparatorWithVerify();
        //        public abstract void ReadNext();
        //        public abstract void ReadNextBlock();
        //        public abstract void SkipWhiteSpace();
    }
}
