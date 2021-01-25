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


        public JsonSourceUtf16 JsonSource;
        public int InitialCharBufferSize;
        //
        public int LineNo;
        public int LineGlobalOffset;
        public int GlobalOffset;
        public int GlobalOffsetTokenStart;
        public int NumberSign;
        public ulong uNumber;
        public long sNumber;
        public ParserState State;
        public bool NeedMoreContent;
        public bool Faulted;
        public bool FinalContent => this.JsonSource.FinalContent;

        public JsonParserUtf16(JsonSourceUtf16 jsonSource, int initialCharBufferSize) {
            JsonSource = jsonSource;
            InitialCharBufferSize = (initialCharBufferSize > (4 * 1024)) ? initialCharBufferSize : (64 * 1024);
            //
            LineNo = 1;
            LineGlobalOffset = 0;
            GlobalOffset = 0;
            GlobalOffsetTokenStart = 0;
            NumberSign = 0;
            uNumber = 0;
            sNumber = 0;
            State = 0;
            NeedMoreContent = false;
            Faulted = false;
        }

        public JsonToken RentFromTokenCache()
            => this.JsonSource.RentFromTokenCache();
        public void ReturnToTokenCache(JsonToken jsonToken)
            => this.JsonSource.ReturnToTokenCache(jsonToken);

        public void Parse(int countWanted = 1) {
            if (this.Faulted) { return; }
            if (countWanted < 1) { countWanted = 1; }
        }

        public void Finalize() {
        }
    }
}
