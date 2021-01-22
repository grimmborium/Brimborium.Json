using System;

namespace Brimborium.Json {
    public sealed class JsonToken {
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
