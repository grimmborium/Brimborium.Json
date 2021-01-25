using System;

namespace Brimborium.Json {
    public sealed class JsonToken {
        public static JsonToken TokenFault = new JsonToken(JsonTokenKind.Fault);
        public static JsonToken TokenEOF = new JsonToken(JsonTokenKind.EOF);
        public static JsonToken TokenReadAwait = new JsonToken(JsonTokenKind.ReadAwait);
        public static JsonToken TokenObjectStart = new JsonToken(JsonTokenKind.ObjectStart);
        public static JsonToken TokenObjectEnd = new JsonToken(JsonTokenKind.ObjectEnd);
        public static JsonToken TokenArrayStart = new JsonToken(JsonTokenKind.ArrayStart);
        public static JsonToken TokenArrayEnd = new JsonToken(JsonTokenKind.ArrayEnd);
        public static JsonToken TokenValueSep = new JsonToken(JsonTokenKind.ValueSep);
        public static JsonToken TokenPairSep = new JsonToken(JsonTokenKind.PairSep);
        public static JsonToken TokenTrue = new JsonToken(JsonTokenKind.True);
        public static JsonToken TokenFalse = new JsonToken(JsonTokenKind.False);
        public static JsonToken TokenNull = new JsonToken(JsonTokenKind.Null);
        public static JsonToken TokenValue = new JsonToken(JsonTokenKind.Value);

        public JsonTokenKind Kind;
        public int OffsetUtf8;
        public int LengthUtf8;
        public int OffsetUtf16;
        public int LengthUtf16;

        public bool IsValidUtf8 => LengthUtf8 > 0;

        public bool IsValidUtf16 => LengthUtf16 > 0;

        public JsonToken() {
        }

        public JsonToken(JsonTokenKind kind) {
            Kind = kind;
        }

        public Span<byte> GetSpanUtf8(JsonReaderContext context) {
            return context.BoundedByteArray.GetSpan(OffsetUtf8, LengthUtf8);
        }

        public Span<char> GetSpanUtf16(JsonReaderContext context) {
            return context.BoundedCharArray.GetSpan(OffsetUtf16, LengthUtf16);
        }

        public bool IsEqual(JsonText jsonText, JsonReaderContext context) {
            if (IsValidUtf8) {
                return this.GetSpanUtf8(context).SequenceEqual(jsonText.GetSpanUtf8());
            }
            if (IsValidUtf16) {
                return this.GetSpanUtf16(context).SequenceEqual(jsonText.GetSpanUtf16());
            }
            return false;
        }

        public void SetKind(JsonTokenKind kind) {
            this.Kind = kind;
            this.LengthUtf8 = 0;
            this.LengthUtf16 = 0;
        }

        public void SetKindUtf8(JsonTokenKind kind, int offsetStart, int offsetEnd) {
            this.Kind = kind;
            this.OffsetUtf8 = offsetStart;
            this.LengthUtf8 = offsetEnd - offsetStart;
            this.LengthUtf16 = 0;
        }

        public override string ToString() {
            return $"JsonToken: {this.Kind}";
        }
    }

    public enum JsonTokenKind : int {
        Fault,
        EOF,
        ReadAwait,
        ObjectStart,
        ObjectEnd,
        ArrayStart,
        ArrayEnd,
        ValueSep,
        PairSep,
        StringSimpleUtf8,
        StringComplex,
        True,
        False,
        Null,
        Number,
        Value
    };
}
