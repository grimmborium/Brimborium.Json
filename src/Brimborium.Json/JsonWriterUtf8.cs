#nullable enable

using System;
using System.Text;
using Brimborium.Json.Internal;

using System.Runtime.CompilerServices;
//
namespace Brimborium.Json {
    public sealed class JsonWriterUtf8 : JsonWriter {
        private static readonly byte[] emptyBytes = new byte[0];

        public static byte[] GetEncodedPropertyName(string propertyName) {
            var writer = new JsonWriterUtf8(Array.Empty<byte>());
            writer.WritePropertyName(propertyName);
            return writer.GetBufferAndReset();
        }

        public static byte[] GetEncodedPropertyNameWithPrefixValueSeparator(string propertyName) {
            var writer = new JsonWriterUtf8(Array.Empty<byte>());
            writer.WriteValueSeparator();
            writer.WritePropertyName(propertyName);
            return writer.GetBufferAndReset();
        }

        public static byte[] GetEncodedPropertyNameWithBeginObject(string propertyName) {
            var writer = new JsonWriterUtf8(Array.Empty<byte>());
            writer.WriteBeginObject();
            writer.WritePropertyName(propertyName);
            return writer.GetBufferAndReset();
        }

        public static byte[] GetEncodedPropertyNameWithoutQuotation(string propertyName) {
            var writer = new JsonWriterUtf8(Array.Empty<byte>());
            writer.WriteString(propertyName); // "propname"
            return writer.GetBufferAndReset();
        }

        internal byte[] GetBufferAndReset() {
            byte[] result;
            if (this.ownBuffer && this.buffer.Length == this.offset) {
                result = this.buffer;
            } else {
                result = this.ToUtf8ByteArray();
            }
            this.buffer = Array.Empty<byte>();
            this.offset = 0;
            return result;

        }

        internal bool ownBuffer;

        // write direct from UnsafeMemory
        internal byte[] buffer;
        internal int offset;

        public override int CurrentOffset {
            get {
                return this.offset;
            }
        }

        public override void AdvanceOffset(int offset) {
            this.offset += offset;
        }

        public JsonWriterUtf8() {
            this.buffer = Array.Empty<byte>();
            this.offset = 0;
            this.ownBuffer = false;
        }

        public JsonWriterUtf8(byte[]? initialBuffer) {
            this.offset = 0;
            if (initialBuffer is null) {
                this.ownBuffer = true;
                this.buffer = Array.Empty<byte>();
            } else if (ReferenceEquals(initialBuffer, Array.Empty<byte>())) {
                this.ownBuffer = true;
                this.buffer = initialBuffer;
            } else {
                this.ownBuffer = false;
                this.buffer = initialBuffer;
            }
        }

        public override ArraySegment<byte> GetBuffer() {
            if (this.buffer == null) {
                return new ArraySegment<byte>(emptyBytes, 0, 0);
            }
            return new ArraySegment<byte>(this.buffer, 0, this.offset);
        }

        public override byte[] ToUtf8ByteArray() {
            if (this.buffer == null) {
                return emptyBytes;
            }
            return ByteArrayUtil.FastCloneWithResize(this.buffer, this.offset);
        }

        public override string ToString() {
            if (this.buffer == null) {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(this.buffer, 0, this.offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EnsureCapacity(int length) {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> WriteRange(int length) {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, length);
            return new Span<byte>(this.buffer, this.offset, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EnsureCapacity(int offset, int length) {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, offset, length);
        }

#if false
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetBuffer(int length) {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, length);
            return new Span<byte>(this.buffer, this.offset, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetBuffer(int offset, int length) {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, offset, length);
            return new Span<byte>(this.buffer, offset, length);
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteRaw(byte rawValue) {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 1);
            this.buffer[this.offset++] = rawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteRaw(byte[] rawValue) {
#if NETSTANDARD
            UnsafeMemory.WriteRaw(this, rawValue);
#else
            BinaryUtil.EnsureCapacity(ref buffer, offset, rawValue.Length);
            Buffer.BlockCopy(rawValue, 0, buffer, offset, rawValue.Length);
            offset += rawValue.Length;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteRawUnsafe(byte rawValue) {
            this.buffer[this.offset++] = rawValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteBeginArray() {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 1);
            this.buffer[this.offset++] = (byte)'[';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteEndArray() {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 1);
            this.buffer[this.offset++] = (byte)']';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteBeginObject() {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 1);
            this.buffer[this.offset++] = (byte)'{';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteEndObject() {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 1);
            this.buffer[this.offset++] = (byte)'}';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteValueSeparator() {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 1);
            this.buffer[this.offset++] = (byte)',';
        }

        /// <summary>:</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteNameSeparator() {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 1);
            this.buffer[this.offset++] = (byte)':';
        }

        /// <summary>WriteString + WriteNameSeparator</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WritePropertyName(string propertyName) {
            this.WriteString(propertyName);
            this.WriteNameSeparator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteQuotation() {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 1);
            this.buffer[this.offset++] = (byte)'\"';
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteNull() {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 4);
            this.buffer[this.offset + 0] = (byte)'n';
            this.buffer[this.offset + 1] = (byte)'u';
            this.buffer[this.offset + 2] = (byte)'l';
            this.buffer[this.offset + 3] = (byte)'l';
            this.offset += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteBoolean(bool value) {
            if (value) {
                ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 4);
                this.buffer[this.offset + 0] = (byte)'t';
                this.buffer[this.offset + 1] = (byte)'r';
                this.buffer[this.offset + 2] = (byte)'u';
                this.buffer[this.offset + 3] = (byte)'e';
                this.offset += 4;
            } else {
                ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 5);
                this.buffer[this.offset + 0] = (byte)'f';
                this.buffer[this.offset + 1] = (byte)'a';
                this.buffer[this.offset + 2] = (byte)'l';
                this.buffer[this.offset + 3] = (byte)'s';
                this.buffer[this.offset + 4] = (byte)'e';
                this.offset += 5;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteTrue() {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 4);
            this.buffer[this.offset + 0] = (byte)'t';
            this.buffer[this.offset + 1] = (byte)'r';
            this.buffer[this.offset + 2] = (byte)'u';
            this.buffer[this.offset + 3] = (byte)'e';
            this.offset += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteFalse() {
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, 5);
            this.buffer[this.offset + 0] = (byte)'f';
            this.buffer[this.offset + 1] = (byte)'a';
            this.buffer[this.offset + 2] = (byte)'l';
            this.buffer[this.offset + 3] = (byte)'s';
            this.buffer[this.offset + 4] = (byte)'e';
            this.offset += 5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteSingle(Single value) {
            this.offset += Brimborium.Json.Internal.DoubleConversion.DoubleToStringConverter.GetBytes(ref this.buffer, this.offset, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteDouble(double value) {
            this.offset += Brimborium.Json.Internal.DoubleConversion.DoubleToStringConverter.GetBytes(ref this.buffer, this.offset, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteByte(byte value) {
            this.WriteUInt64((ulong)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteUInt16(ushort value) {
            this.WriteUInt64((ulong)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteUInt32(uint value) {
            this.WriteUInt64((ulong)value);
        }

        public override void WriteUInt64(ulong value) {
            this.offset += NumberConverter.WriteUInt64(ref this.buffer, this.offset, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteSByte(sbyte value) {
            this.WriteInt64((long)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteInt16(short value) {
            this.WriteInt64((long)value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteInt32(int value) {
            this.WriteInt64((long)value);
        }

        public override void WriteInt64(long value) {
            this.offset += NumberConverter.WriteInt64(ref this.buffer, this.offset, value);
        }

        public override void WriteString(string? value) {
            if (value == null) {
                this.WriteNull();
                return;
            }

            var max = Utils.GetUtf8ByteCountForStringToEncode(value) + 2;
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, max);

            // nonescaped-ensure
            var startoffset = this.offset;
            this.buffer[this.offset++] = (byte)'\"';

            var from = 0;
            // for JIT Optimization, for-loop i < str.Length
            for (int i = 0; i < value.Length; i++) {
                byte escapeChar = default(byte);
                switch (value[i]) {
                    case '"':
                        escapeChar = (byte)'"';
                        break;
                    case '\\':
                        escapeChar = (byte)'\\';
                        break;
                    case '\b':
                        escapeChar = (byte)'b';
                        break;
                    case '\f':
                        escapeChar = (byte)'f';
                        break;
                    case '\n':
                        escapeChar = (byte)'n';
                        break;
                    case '\r':
                        escapeChar = (byte)'r';
                        break;
                    case '\t':
                        escapeChar = (byte)'t';
                        break;
                    // use switch jumptable
                    case (char)0:
                    case (char)1:
                    case (char)2:
                    case (char)3:
                    case (char)4:
                    case (char)5:
                    case (char)6:
                    case (char)7:
                    case (char)11:
                    case (char)14:
                    case (char)15:
                    case (char)16:
                    case (char)17:
                    case (char)18:
                    case (char)19:
                    case (char)20:
                    case (char)21:
                    case (char)22:
                    case (char)23:
                    case (char)24:
                    case (char)25:
                    case (char)26:
                    case (char)27:
                    case (char)28:
                    case (char)29:
                    case (char)30:
                    case (char)31:
                    case (char)32:
                    case (char)33:
                    case (char)35:
                    case (char)36:
                    case (char)37:
                    case (char)38:
                    case (char)39:
                    case (char)40:
                    case (char)41:
                    case (char)42:
                    case (char)43:
                    case (char)44:
                    case (char)45:
                    case (char)46:
                    case (char)47:
                    case (char)48:
                    case (char)49:
                    case (char)50:
                    case (char)51:
                    case (char)52:
                    case (char)53:
                    case (char)54:
                    case (char)55:
                    case (char)56:
                    case (char)57:
                    case (char)58:
                    case (char)59:
                    case (char)60:
                    case (char)61:
                    case (char)62:
                    case (char)63:
                    case (char)64:
                    case (char)65:
                    case (char)66:
                    case (char)67:
                    case (char)68:
                    case (char)69:
                    case (char)70:
                    case (char)71:
                    case (char)72:
                    case (char)73:
                    case (char)74:
                    case (char)75:
                    case (char)76:
                    case (char)77:
                    case (char)78:
                    case (char)79:
                    case (char)80:
                    case (char)81:
                    case (char)82:
                    case (char)83:
                    case (char)84:
                    case (char)85:
                    case (char)86:
                    case (char)87:
                    case (char)88:
                    case (char)89:
                    case (char)90:
                    case (char)91:
                    default:
                        continue;
                }

                max += 2;
                ByteArrayUtil.EnsureCapacity(ref this.buffer, startoffset, max); // check +escape capacity

                this.offset += StringEncoding.UTF8NoBOM.GetBytes(value, from, i - from, this.buffer, this.offset);
                from = i + 1;
                this.buffer[this.offset++] = (byte)'\\';
                this.buffer[this.offset++] = escapeChar;
            }

            if (from != value.Length) {
                this.offset += StringEncoding.UTF8NoBOM.GetBytes(value, from, value.Length - from, this.buffer, this.offset);
            }

            this.buffer[this.offset++] = (byte)'\"';
        }

        public override void WriteStringWithoutQuotation(string value) {
            if (value == null) {
                this.WriteNull();
                return;
            }

            var max = Utils.GetUtf8ByteCountForStringToEncode(value);
            ByteArrayUtil.EnsureCapacity(ref this.buffer, this.offset, max);

            // nonescaped-ensure
            var startoffset = this.offset;

            var from = 0;
            // for JIT Optimization, for-loop i < str.Length
            for (int i = 0; i < value.Length; i++) {
                byte escapeChar = default(byte);
                switch (value[i]) {
                    case '"':
                        escapeChar = (byte)'"';
                        break;
                    case '\\':
                        escapeChar = (byte)'\\';
                        break;
                    case '\b':
                        escapeChar = (byte)'b';
                        break;
                    case '\f':
                        escapeChar = (byte)'f';
                        break;
                    case '\n':
                        escapeChar = (byte)'n';
                        break;
                    case '\r':
                        escapeChar = (byte)'r';
                        break;
                    case '\t':
                        escapeChar = (byte)'t';
                        break;
                    // use switch jumptable
                    case (char)0:
                    case (char)1:
                    case (char)2:
                    case (char)3:
                    case (char)4:
                    case (char)5:
                    case (char)6:
                    case (char)7:
                    case (char)11:
                    case (char)14:
                    case (char)15:
                    case (char)16:
                    case (char)17:
                    case (char)18:
                    case (char)19:
                    case (char)20:
                    case (char)21:
                    case (char)22:
                    case (char)23:
                    case (char)24:
                    case (char)25:
                    case (char)26:
                    case (char)27:
                    case (char)28:
                    case (char)29:
                    case (char)30:
                    case (char)31:
                    case (char)32:
                    case (char)33:
                    case (char)35:
                    case (char)36:
                    case (char)37:
                    case (char)38:
                    case (char)39:
                    case (char)40:
                    case (char)41:
                    case (char)42:
                    case (char)43:
                    case (char)44:
                    case (char)45:
                    case (char)46:
                    case (char)47:
                    case (char)48:
                    case (char)49:
                    case (char)50:
                    case (char)51:
                    case (char)52:
                    case (char)53:
                    case (char)54:
                    case (char)55:
                    case (char)56:
                    case (char)57:
                    case (char)58:
                    case (char)59:
                    case (char)60:
                    case (char)61:
                    case (char)62:
                    case (char)63:
                    case (char)64:
                    case (char)65:
                    case (char)66:
                    case (char)67:
                    case (char)68:
                    case (char)69:
                    case (char)70:
                    case (char)71:
                    case (char)72:
                    case (char)73:
                    case (char)74:
                    case (char)75:
                    case (char)76:
                    case (char)77:
                    case (char)78:
                    case (char)79:
                    case (char)80:
                    case (char)81:
                    case (char)82:
                    case (char)83:
                    case (char)84:
                    case (char)85:
                    case (char)86:
                    case (char)87:
                    case (char)88:
                    case (char)89:
                    case (char)90:
                    case (char)91:
                    default:
                        continue;
                }

                max += 1;
                ByteArrayUtil.EnsureCapacity(ref this.buffer, startoffset, max); // check +escape capacity

                this.offset += StringEncoding.UTF8NoBOM.GetBytes(value, from, i - from, this.buffer, this.offset);
                from = i + 1;
                this.buffer[this.offset++] = (byte)'\\';
                this.buffer[this.offset++] = escapeChar;
            }

            if (from != value.Length) {
                this.offset += StringEncoding.UTF8NoBOM.GetBytes(value, from, value.Length - from, this.buffer, this.offset);
            }
        }

        public override void WriteStartProperty(JsonSerializationInfo jsonSerializationInfo, int key) {
#warning here WriteStartProperty
            //jsonSerializationInfo.Properties
            throw new NotImplementedException();
        }

        public class Utils {

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int GetUtf8ByteCountForStringToEncode(string value) {
                var max = StringEncoding.UTF8NoBOM.GetByteCount(value);

                for (int i = 0; i < value.Length; i++) {
                    switch (value[i]) {
                        case '"':
                            break;
                        case '\\':
                            break;
                        case '\b':
                            break;
                        case '\f':
                            break;
                        case '\n':
                            break;
                        case '\r':
                            break;
                        case '\t':
                            break;
                        // use switch jumptable
                        case (char)0:
                        case (char)1:
                        case (char)2:
                        case (char)3:
                        case (char)4:
                        case (char)5:
                        case (char)6:
                        case (char)7:
                        case (char)11:
                        case (char)14:
                        case (char)15:
                        case (char)16:
                        case (char)17:
                        case (char)18:
                        case (char)19:
                        case (char)20:
                        case (char)21:
                        case (char)22:
                        case (char)23:
                        case (char)24:
                        case (char)25:
                        case (char)26:
                        case (char)27:
                        case (char)28:
                        case (char)29:
                        case (char)30:
                        case (char)31:
                        case (char)32:
                        case (char)33:
                        case (char)35:
                        case (char)36:
                        case (char)37:
                        case (char)38:
                        case (char)39:
                        case (char)40:
                        case (char)41:
                        case (char)42:
                        case (char)43:
                        case (char)44:
                        case (char)45:
                        case (char)46:
                        case (char)47:
                        case (char)48:
                        case (char)49:
                        case (char)50:
                        case (char)51:
                        case (char)52:
                        case (char)53:
                        case (char)54:
                        case (char)55:
                        case (char)56:
                        case (char)57:
                        case (char)58:
                        case (char)59:
                        case (char)60:
                        case (char)61:
                        case (char)62:
                        case (char)63:
                        case (char)64:
                        case (char)65:
                        case (char)66:
                        case (char)67:
                        case (char)68:
                        case (char)69:
                        case (char)70:
                        case (char)71:
                        case (char)72:
                        case (char)73:
                        case (char)74:
                        case (char)75:
                        case (char)76:
                        case (char)77:
                        case (char)78:
                        case (char)79:
                        case (char)80:
                        case (char)81:
                        case (char)82:
                        case (char)83:
                        case (char)84:
                        case (char)85:
                        case (char)86:
                        case (char)87:
                        case (char)88:
                        case (char)89:
                        case (char)90:
                        case (char)91:
                        default:
                            continue;
                    }
                    max++;
                }

                return max;
            }

        }
    }
}