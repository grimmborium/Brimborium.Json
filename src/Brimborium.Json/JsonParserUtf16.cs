using System;

namespace Brimborium.Json {
    public struct JsonParserUtf16 {
        public enum ParserState {
            Start = 0,
            T,
            TR,
            TRU,
            F,
            FA,
            FAL,
            FALS,
            N,
            NU,
            NUL,
            NULL,
            Slash,
            QuoteStart,
            QuoteContentComplex,
            QuoteBackSlash,
            NumberSignPlus,
            NumberSignMinus,
            NumberInt,
            NumberIEEE,
            EOF // must be the last
        }
        public const int InitialCharBufferSize = 64 * 1024;
        public int LineNo;
        public int LineOffset;
        public int NumberSign;
        public ulong uNumber;
        public long sNumber;
        public int IdxToken;
        public int Offset;
        public ParserState State;
        public int OffsetTokenStart;
        public bool FinalContent;
        public JsonReaderContext Context;

        public JsonParserUtf16(JsonReaderContext jsonReaderContext) {
            Context = jsonReaderContext;
            //
            LineNo = 1;
            LineOffset = 0;
            NumberSign = 0;
            uNumber = 0;
            sNumber = 0;
            IdxToken = 0;
            Offset = -1;
            State = 0;
            OffsetTokenStart = 0;
            FinalContent = false;
        }

        public void Parse(BoundedByteArray src, bool finalContent) {
            Context.BoundedByteArray = src;
            Context.FinalContent = finalContent;
            Parse();
        }

        public void Parse() {
        }

        public void Finalize(BoundedByteArray src) {
        }
    }
}
