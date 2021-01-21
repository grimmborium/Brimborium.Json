using System;
using System.Reflection.Emit;
using System.Security.AccessControl;

namespace Brimborium.Json {
    public struct JsonToken {
        public JsonTokenKind Kind;

        public bool IsValidUtf8;

        public bool IsValidUtf16;

        public BoundedByteArray BoundedByteArray;

        public BoundedCharArray BoundedCharArray;

        public JsonToken(int minimumLength) {
            Kind = JsonTokenKind.Fault;
            BoundedByteArray = BoundedByteArray.Rent(minimumLength);
            BoundedCharArray = BoundedCharArray.Rent(minimumLength * 4);
            IsValidUtf8 = true;
            IsValidUtf16 = true;
        }

        public Span<byte> GetSpanUtf8() {
            return this.BoundedByteArray.GetUsedSpan();
        }
        public Span<char> GetSpanUtf16() {
            return this.BoundedCharArray.GetUsedSpan();
        }

        public bool IsEqual(JsonText jsonText) {
            if (IsValidUtf8) {
                return this.GetSpanUtf8().SequenceEqual(jsonText.GetSpanUtf8());
            }
            if (IsValidUtf16) {
                return this.GetSpanUtf16().SequenceEqual(jsonText.GetSpanUtf16());
            }
            return false;
        }
    }

    public enum JsonTokenKind {
        Fault,
        ObjectStart,
        ObjectEnd,
        ArrayStart,
        ArrayEnd,
        ValueSep,
        PairSep,
        String,
        Number,
        Value
    };
}
