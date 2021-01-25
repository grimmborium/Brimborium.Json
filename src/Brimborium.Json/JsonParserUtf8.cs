using System;
using System.Runtime.Serialization;

namespace Brimborium.Json {
    public struct JsonParserUtf8 {
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

        
        public JsonSourceUtf8 JsonSource;
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
        public int NeedMoreContent;
        public bool Faulted;
        public bool FinalContent => this.JsonSource.FinalContent;

        public JsonParserUtf8(JsonSourceUtf8 jsonSource, int initialCharBufferSize) {
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
            NeedMoreContent = 0;
            Faulted = false;
        }

        public JsonToken RentFromTokenCache()
            => this.JsonSource.RentFromTokenCache();
        public void ReturnToTokenCache(JsonToken jsonToken)
            => this.JsonSource.ReturnToTokenCache(jsonToken);

        public void Parse(int countWanted=1) {
            if (this.Faulted) { return; }
            ref var BoundedByteArray = ref this.JsonSource.BoundedByteArray;
            if (this.NeedMoreContent > 0) {
                if (BoundedByteArray.GlobalReadOffset == this.NeedMoreContent) {
                    return;
                }
            }
            if (countWanted < 1) { countWanted = 1; }

            ref var ReadIndexToken = ref this.JsonSource.ReadIndexToken;
            ref var FeedIndexToken = ref this.JsonSource.FeedIndexToken;
            ref var Tokens = ref this.JsonSource.Tokens;
            ref var BoundedCharArray = ref this.JsonSource.BoundedCharArray;


            var protectRange = BoundedByteArray.GetProtectRange();
            
            var sourceSpan = BoundedByteArray.GetSpan(protectRange.lowerOffset, protectRange.lowerLength);
            int length = protectRange.lowerLength; // == sourceSpan.Length;
            
            int FeedIndexTokenLimit = this.JsonSource.Tokens.Length;
            byte current;
            //
            int OffsetTokenStart = this.GlobalOffsetTokenStart - protectRange.lowerGlobalOffset; 
            int Offset = this.GlobalOffset - protectRange.lowerGlobalOffset;
            //
            if (ReadIndexToken == FeedIndexToken) {
                ReadIndexToken = 0;
                FeedIndexToken = 0;
            }
            if (FeedIndexToken < Tokens.Length) {
                //
                // now goto hell
                //
                while (Offset < length) {
                    switch (State) {
                        case ParserState.Start: {
                                switch (current = sourceSpan[Offset]) {
                                    case 1:
                                    case 2:
                                    case 3:
                                    case 4:
                                    case 5:
                                    case 6:
                                    case 7:
                                    case 8:
                                    case 9: /* \t */
                                    case 10: /* \n */
                                    case 11:
                                    case 12:
                                    case 13: /* \r */
                                    case 14:
                                    case 15:
                                    case 16:
                                    case 17:
                                    case 18:
                                    case 19:
                                    case 20:
                                    case 21:
                                    case 22:
                                    case 23:
                                    case 24:
                                    case 25:
                                    case 26:
                                    case 27:
                                    case 28:
                                    case 29:
                                    case 30:
                                    case 31:
                                    case 32: /* SPACE */
                                        while (Offset < length) {
                                            switch (sourceSpan[Offset]) {
                                                case 1:
                                                case 2:
                                                case 3:
                                                case 4:
                                                case 5:
                                                case 6:
                                                case 7:
                                                case 8:
                                                case 9: /* \t */
                                                    ++Offset;
                                                    continue;
                                                case 10: /* \n */
                                                    ++Offset;
                                                    this.LineNo++;
                                                    this.LineGlobalOffset = Offset + BoundedByteArray.GlobalReadOffset;
                                                    continue;
                                                case 11:
                                                case 12:
                                                    ++Offset;
                                                    continue;
                                                case 13: /* \r */
                                                    this.LineNo++;
                                                    if ((++Offset) < length) {
                                                        if (sourceSpan[Offset + 1] == 10 /* \n */) {
                                                            ++Offset;
                                                        }
                                                    }
                                                    this.LineGlobalOffset = Offset + BoundedByteArray.GlobalReadOffset;
                                                    continue;
                                                case 14:
                                                case 15:
                                                case 16:
                                                case 17:
                                                case 18:
                                                case 19:
                                                case 20:
                                                case 21:
                                                case 22:
                                                case 23:
                                                case 24:
                                                case 25:
                                                case 26:
                                                case 27:
                                                case 28:
                                                case 29:
                                                case 30:
                                                case 31:
                                                case 32: /* SPACE */
                                                    // ignore
                                                    ++Offset;
                                                    continue;

                                                default:
                                                    goto lblSpaceNext;
                                            }
                                        }
                                        lblSpaceNext:
                                        OffsetTokenStart = Offset;
                                        continue;

                                    //case 33: /* ! */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    case 34: /* " */
                                        State = ParserState.QuoteStart;
                                        OffsetTokenStart = Offset;
                                        ++Offset;
                                        continue;

                                    //case 35: /* # */
                                    //case 36: /* $ */
                                    //case 37: /* % */
                                    //case 38: /* & */
                                    //case 39: /* ' */
                                    //case 40: /* ( */
                                    //case 41: /* ) */
                                    //case 42: /* * */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    case 43: /* + */
                                        State = ParserState.NumberSignPlus;
                                        this.NumberSign = 1;
                                        OffsetTokenStart = Offset;
                                        ++Offset;
                                        continue;

                                    case 44: /* , */
#warning BoundedCharArray.GlobalProtected = -1;
                                        BoundedCharArray.GlobalProtected = -1;
                                        Tokens[FeedIndexToken++] = JsonToken.TokenValueSep;
                                        if (FeedIndexToken >= FeedIndexTokenLimit) { goto lblTokensLength; }

                                        // State = ParserState.Start;
                                        ++Offset;
                                        OffsetTokenStart = Offset;
                                        goto lblDone;

                                    case 45: /* - */
                                        State = ParserState.NumberSignMinus;
                                        this.NumberSign = -1;
                                        OffsetTokenStart = Offset;
                                        ++Offset;
                                        continue;

                                    case 46: /* . */
                                        State = ParserState.NumberIEEE;
                                        this.NumberSign = 1;
                                        OffsetTokenStart = Offset;
                                        ++Offset;
                                        continue;

                                    case 47: /* / */
                                        State = ParserState.Slash;
                                        OffsetTokenStart = Offset;
                                        ++Offset;
                                        continue;

                                    case 48: /* 0 */
                                    case 49: /* 1 */
                                    case 50: /* 2 */
                                    case 51: /* 3 */
                                    case 52: /* 4 */
                                    case 53: /* 5 */
                                    case 54: /* 6 */
                                    case 55: /* 7 */
                                    case 56: /* 8 */
                                    case 57: /* 9 */
                                        State = ParserState.NumberInt;
                                        this.NumberSign = 1;
                                        OffsetTokenStart = Offset;
                                        this.uNumber = (ulong)(current -/* '0' */48);
                                        ++Offset;
                                        continue;

                                    case 58: /* : */
                                        Tokens[FeedIndexToken++] = JsonToken.TokenPairSep;
                                        if (FeedIndexToken >= FeedIndexTokenLimit) { goto lblTokensLength; }
                                        ++Offset;
                                        // State = ParserState.Start;
                                        OffsetTokenStart = Offset;
                                        continue;

                                    //case 59: /* ; */
                                    //case 60: /* < */
                                    //case 61: /* = */
                                    //case 62: /* > */
                                    //case 63: /* ? */
                                    //case 64: /* @ */
                                    //case 65: /* A */
                                    //case 66: /* B */
                                    //case 67: /* C */
                                    //case 68: /* D */
                                    //case 69: /* E */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    case 70: /* F */
                                        State = ParserState.F;
                                        OffsetTokenStart = Offset;
                                        ++Offset;
                                        continue;

                                    //case 71: /* G */
                                    //case 72: /* H */
                                    //case 73: /* I */
                                    //case 74: /* J */
                                    //case 75: /* K */
                                    //case 76: /* L */
                                    //case 77: /* M */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    case 78: /* N */
                                        State = ParserState.N;
                                        OffsetTokenStart = Offset;
                                        ++Offset;
                                        continue;

                                    //case 79: /* O */
                                    //case 80: /* P */
                                    //case 81: /* Q */
                                    //case 82: /* R */
                                    //case 83: /* S */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    case 84: /* T */
                                        State = ParserState.T;
                                        OffsetTokenStart = Offset;
                                        ++Offset;
                                        continue;

                                    //case 85: /* U */
                                    //case 86: /* V */
                                    //case 87: /* W */
                                    //case 88: /* X */
                                    //case 89: /* Y */
                                    //case 90: /* Z */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    case 91: /* [ */
                                        Tokens[FeedIndexToken++] = JsonToken.TokenArrayStart;
                                        if (FeedIndexToken >= FeedIndexTokenLimit) { goto lblTokensLength; }
                                        ++Offset;
                                        OffsetTokenStart = Offset;
                                        continue;

                                    //case 92: /* \ */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    case 93: /* ] */
                                        BoundedCharArray.GlobalProtected = -1;
                                        //
                                        Tokens[FeedIndexToken++] = JsonToken.TokenArrayEnd;
                                        if (FeedIndexToken >= FeedIndexTokenLimit) { goto lblTokensLength; }
                                        ++Offset;
                                        OffsetTokenStart = Offset;
                                        goto lblDone;

                                    //case 94: /* ^ */
                                    //case 95: /* _ */
                                    //case 96: /* ` */
                                    //case 97: /* a */
                                    //case 98: /* b */
                                    //case 99: /* c */
                                    //case 100: /* d */
                                    //case 101: /* e */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    case 102: /* f */
                                        State = ParserState.F;
                                        OffsetTokenStart = Offset;
                                        ++Offset;
                                        continue;

                                    //case 103: /* g */
                                    //case 104: /* h */
                                    //case 105: /* i */
                                    //case 106: /* j */
                                    //case 107: /* k */
                                    //case 108: /* l */
                                    //case 109: /* m */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    case 110: /* n */
                                        State = ParserState.N;
                                        OffsetTokenStart = Offset;
                                        ++Offset;
                                        continue;

                                    //case 111: /* o */
                                    //case 112: /* p */
                                    //case 113: /* q */
                                    //case 114: /* r */
                                    //case 115: /* s */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    case 116: /* t */
                                        State = ParserState.T;
                                        OffsetTokenStart = Offset;
                                        ++Offset;
                                        continue;

                                    //case 117: /* u */
                                    //case 118: /* v */
                                    //case 119: /* w */
                                    //case 120: /* x */
                                    //case 121: /* y */
                                    //case 122: /* z */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    case 123: /* { */
                                        Tokens[FeedIndexToken++] = JsonToken.TokenObjectStart;
                                        if (FeedIndexToken >= FeedIndexTokenLimit) { goto lblTokensLength; }
                                        ++Offset;
                                        OffsetTokenStart = Offset;
                                        continue;

                                    // case 124: /* | */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    case 125: /* } */
                                        BoundedCharArray.GlobalProtected = -1;
                                        Tokens[FeedIndexToken++] = JsonToken.TokenObjectEnd;
                                        if (FeedIndexToken >= FeedIndexTokenLimit) { goto lblTokensLength; }
                                        OffsetTokenStart = Offset + 1;
                                        ++Offset;
                                        goto lblDone;

                                    //case 126: /* ~ */
                                    //case 127: /*   */
                                    //    OffsetTokenStart = Offset;
                                    //    goto lblUnexpectedChar;

                                    default:
                                        OffsetTokenStart = Offset;
                                        goto lblUnexpectedChar;
                                }
                                //
                                // goto lblUnexpectedChar;
                            }

                        case ParserState.T:
                            switch (current = sourceSpan[Offset]) {
                                case (byte)'R':
                                case (byte)'r':
                                    State = ParserState.TR;
                                    if (++Offset < length) {
                                        goto lblTR;
                                    } else {
                                        --Offset;
                                        goto lblNeedMoreContent;
                                    }
                                default:
                                    goto lblUnexpectedChar;
                            }

                        case ParserState.TR:
                            lblTR:
                            switch (current = sourceSpan[Offset]) {
                                case (byte)'U':
                                case (byte)'u':
                                    State = ParserState.TRU;
                                    if (++Offset < length) {
                                        goto lblTRU;
                                    } else {
                                        --Offset;
                                        goto lblNeedMoreContent;
                                    }
                                default:
                                    goto lblUnexpectedChar;
                            }

                        case ParserState.TRU:
                            lblTRU:
                            switch (current = sourceSpan[Offset]) {
                                case (byte)'E':
                                case (byte)'e':
                                    ++Offset;
                                    if ((Offset < length)
                                        ? GetEndOfValueNextValue(sourceSpan[Offset])
                                        : FinalContent
                                        ) {
                                        Tokens[FeedIndexToken++] = JsonToken.TokenTrue;
                                        if (FeedIndexToken >= FeedIndexTokenLimit) { goto lblTokensLength; }
                                        State = ParserState.Start;
                                        OffsetTokenStart = Offset;
                                        continue;
                                    } else {
                                        --Offset;
                                        goto lblNeedMoreContent;
                                    }
                                default:
                                    goto lblUnexpectedChar;
                            }

                        case ParserState.F:
                            switch (current = sourceSpan[Offset]) {
                                case (byte)'A':
                                case (byte)'a':
                                    State = ParserState.TRU;
                                    if (++Offset < length) {
                                        goto lblFA;
                                    } else {
                                        --Offset;
                                        goto lblNeedMoreContent;
                                    }
                                default:
                                    goto lblUnexpectedChar;
                            }

                        case ParserState.FA:
                            lblFA:
                            switch (current = sourceSpan[Offset]) {
                                case (byte)'L':
                                case (byte)'l':
                                    State = ParserState.FAL;
                                    if (++Offset < length) {
                                        goto lblFAL;
                                    } else {
                                        --Offset;
                                        goto lblNeedMoreContent;
                                    }
                                default:
                                    goto lblUnexpectedChar;
                            }

                        case ParserState.FAL:
                            lblFAL:
                            switch (current = sourceSpan[Offset]) {
                                case (byte)'S':
                                case (byte)'s':
                                    State = ParserState.FALS;
                                    if (++Offset < length) {
                                        goto lblFALS;
                                    } else {
                                        --Offset;
                                        goto lblNeedMoreContent;
                                    }
                                default:
                                    goto lblUnexpectedChar;
                            }

                        case ParserState.FALS:
                            lblFALS:
                            switch (current = sourceSpan[Offset]) {
                                case (byte)'E':
                                case (byte)'e':
                                    if ((++Offset < length)
                                        ? GetEndOfValueNextValue(sourceSpan[Offset])
                                        : FinalContent
                                        ) {
                                        Tokens[FeedIndexToken++] = JsonToken.TokenFalse;
                                        if (FeedIndexToken >= FeedIndexTokenLimit) { goto lblTokensLength; }
                                        State = ParserState.Start;
                                        OffsetTokenStart = Offset;
                                        continue;
                                    } else {
                                        --Offset;
                                        goto lblNeedMoreContent;
                                    }
                                default:
                                    goto lblUnexpectedChar;
                            }

                        case ParserState.N:
                            switch (current = sourceSpan[Offset]) {
                                case (byte)'U':
                                case (byte)'u':
                                    State = ParserState.NU;
                                    if (++Offset < length) {
                                        goto lblNU;
                                    } else {
                                        --Offset;
                                        goto lblNeedMoreContent;
                                    }
                                default:
                                    goto lblUnexpectedChar;
                            }

                        case ParserState.NU:
                            lblNU:
                            switch (current = sourceSpan[Offset]) {
                                case (byte)'L':
                                case (byte)'l':
                                    State = ParserState.NUL;
                                    if (++Offset < length) {
                                        goto lblNUL;
                                    } else {
                                        --Offset;
                                        goto lblNeedMoreContent;
                                    }
                                default:
                                    goto lblUnexpectedChar;
                            }

                        case ParserState.NUL:
                            lblNUL:
                            switch (current = sourceSpan[Offset]) {
                                case (byte)'L':
                                case (byte)'l':
                                    if ((++Offset < length)
                                        ? GetEndOfValueNextValue(sourceSpan[Offset])
                                        : FinalContent
                                        ) {
                                        Tokens[FeedIndexToken++] = JsonToken.TokenNull;
                                        if (FeedIndexToken >= FeedIndexTokenLimit) { goto lblTokensLength; }
                                        State = ParserState.Start;
                                        continue;
                                    } else {
                                        --Offset;
                                        goto lblNeedMoreContent;
                                    }
                                default:
                                    goto lblUnexpectedChar;
                            }

                        case ParserState.QuoteStart:
                            BoundedCharArray.GlobalProtected = Offset + BoundedCharArray.GlobalShift;

                            while (Offset < length) {
                                switch (current = sourceSpan[Offset]) {
                                    case 10: { /* \n */
                                            ++Offset;
                                            this.LineNo++;
                                            this.LineGlobalOffset = Offset + BoundedByteArray.GlobalReadOffset;
                                        }
                                        continue;

                                    case 13: { /* \r */
                                            ++Offset;
                                            this.LineNo++;
                                            if (Offset < length) {
                                                current = sourceSpan[Offset];
                                                if (current == 10) {
                                                    ++Offset;
                                                }
                                            }
                                            this.LineGlobalOffset = Offset + BoundedByteArray.GlobalReadOffset;
                                        }
                                        continue;

                                    case (byte)'"': {
                                            var jsonToken = this.JsonSource.RentFromTokenCache();
                                            jsonToken.SetKindUtf8(JsonTokenKind.StringSimpleUtf8, OffsetTokenStart + 1, Offset - 1);
                                            Tokens[FeedIndexToken++] = jsonToken;
                                            if (FeedIndexToken >= FeedIndexTokenLimit) { goto lblTokensLength; }
                                            State = ParserState.Start;
                                            ++Offset;
                                            OffsetTokenStart = Offset;
                                        }
                                        goto lblQuoteStartNext;


                                    case (byte)'\\': {
                                            --Offset;
                                            State = ParserState.QuoteContentComplex;
                                            //state = ParserState.QuoteBackSlash;
                                            var len = (Offset - OffsetTokenStart);
                                            if (len > 0) {
                                                BoundedCharArray.AdjustBeforeFeeding(
                                                len,
                                                InitialCharBufferSize
                                                );
                                                var countChars = StringUtility.ConvertFromUtf8(
                                                    sourceSpan.Slice(OffsetTokenStart, len),
                                                    BoundedCharArray.GetFeedSpan());
                                                BoundedCharArray.AdjustAfterFeeding(countChars);
                                            }
                                        }
                                        goto lblQuoteStartNext;

                                    default:
                                        if (current < 127) {
                                            // short loop
                                            // add key magic here 
                                            ++Offset;
                                            continue;
                                        } else {
                                            --Offset;
                                            State = ParserState.QuoteContentComplex;
                                            var len = (Offset - OffsetTokenStart);
                                            if (len > 0) {
                                                BoundedCharArray.AdjustBeforeFeeding(len, InitialCharBufferSize);
                                                var countChars = StringUtility.ConvertFromUtf8(
                                                    sourceSpan.Slice(OffsetTokenStart, len),
                                                    BoundedCharArray.GetFeedSpan());
                                                BoundedCharArray.AdjustAfterFeeding(countChars);
                                            }
                                            goto lblQuoteStartNext;
                                        }
                                }
                            }
                            lblQuoteStartNext:
                            break;

                        case ParserState.QuoteContentComplex:
                            while (Offset < length) {
                                switch (current = sourceSpan[Offset]) {
                                    case 10: /* \n */
                                        ++Offset;
                                        this.LineNo++;
                                        this.LineGlobalOffset = Offset + BoundedByteArray.GlobalReadOffset;
                                        continue;
                                    case 13: /* \r */
                                        ++Offset;
                                        this.LineNo++;
                                        if ((Offset) < length) {
                                            current = sourceSpan[Offset];
                                            if (current == 10) {
                                                ++Offset;
                                            }
                                        }
                                        this.LineGlobalOffset = Offset + BoundedByteArray.GlobalReadOffset;
                                        continue;

                                    case (byte)'"':
                                        ++Offset;
                                        if ((Offset < length)
                                            ? GetEndOfValueNextValue(sourceSpan[Offset])
                                            : FinalContent
                                            ) {
#warning TODO
                                            var jsonToken = RentFromTokenCache();
                                            jsonToken.SetKind(JsonTokenKind.StringSimpleUtf8);
                                            Tokens[FeedIndexToken++] = jsonToken;
                                            if (FeedIndexToken >= FeedIndexTokenLimit) { goto lblTokensLength; }
                                            State = ParserState.Start;
                                            goto lblQuoteContentComplexNext;
                                        } else {
                                            --Offset;
                                            goto lblNeedMoreContent;
                                        }

                                    case (byte)'\\':
                                        State = ParserState.QuoteBackSlash;
                                        ++Offset;
                                        goto lblQuoteContentComplexNext;

                                    default:
                                        ++Offset;
                                        break;
                                }
                            }
                            lblQuoteContentComplexNext:
                            break;

                        case ParserState.QuoteBackSlash:
#warning TODO
                            throw new NotImplementedException("TODO QuoteBackSlash");
                        // continue;

                        case ParserState.Slash:
#warning TODO
                            switch (current = sourceSpan[Offset]) {
                                case (byte)'/':
                                    break;
                                case (byte)'*':
                                    break;
                            }
                            throw new NotImplementedException("TODO Slash");
                        // break;

                        case ParserState.NumberSignMinus:
                            throw new NotImplementedException("TODO NumberSignMinus");
                        case ParserState.NumberSignPlus:
                            throw new NotImplementedException("TODO NumberSignPlus");
                        // break;

                        case ParserState.NumberInt:
                            throw new NotImplementedException("TODO NumberInt");
                        // break;

                        default:
                            throw new NotImplementedException("TODO default");
                            // break;
                    }
                }
                //
            }
            //

            lblDone:
            NeedMoreContent = 0;
            this.GlobalOffset = Offset + protectRange.lowerGlobalOffset;
            this.GlobalOffsetTokenStart = OffsetTokenStart + protectRange.lowerGlobalOffset;
            JsonSource.BoundedByteArray.AdjustAfterReading(Offset);
            return;

            lblNeedMoreContent:
            NeedMoreContent = Offset + protectRange.lowerGlobalOffset;
            if (BoundedByteArray.GlobalProtected < 0) { 
                BoundedByteArray.GlobalProtected = OffsetTokenStart + protectRange.lowerGlobalOffset;
            }
            this.GlobalOffset = Offset + protectRange.lowerGlobalOffset;
            this.GlobalOffsetTokenStart = OffsetTokenStart + protectRange.lowerGlobalOffset;
            JsonSource.BoundedByteArray.AdjustAfterReading(Offset);
            return;

            lblUnexpectedChar:
            Faulted = true;
            throw new ArgumentException($"Unexpected Char {current} L:{this.LineNo}; C:{(Offset - (this.LineGlobalOffset - BoundedByteArray.GlobalReadOffset))};");
            

            lblTokensLength:
            Faulted = true;
            throw new InvalidOperationException("TokensLength too big.");
        }

        public bool GetEndOfValueNextValue(byte current) {
            switch (current) {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9: /* \t */
                case 10: /* \n */
                case 11:
                case 12:
                case 13: /* \r */
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32: /* SPACE */
                    return true;
                case 33: /* ! */
                case 34: /* " */
                case 35: /* # */
                case 36: /* $ */
                case 37: /* % */
                case 38: /* & */
                case 39: /* ' */
                case 40: /* ( */
                case 41: /* ) */
                case 42: /* * */
                case 43: /* + */
                    return false;
                case 44: /* , */
                    return true;
                case 45: /* - */
                case 46: /* . */
                    return false;
                case 47: /* / */
                    return true;
                case 48: /* 0 */
                case 49: /* 1 */
                case 50: /* 2 */
                case 51: /* 3 */
                case 52: /* 4 */
                case 53: /* 5 */
                case 54: /* 6 */
                case 55: /* 7 */
                case 56: /* 8 */
                case 57: /* 9 */
                case 58: /* : */
                case 59: /* ; */
                case 60: /* < */
                case 61: /* = */
                case 62: /* > */
                case 63: /* ? */
                case 64: /* @ */
                case 65: /* A */
                case 66: /* B */
                case 67: /* C */
                case 68: /* D */
                case 69: /* E */
                case 70: /* F */
                case 71: /* G */
                case 72: /* H */
                case 73: /* I */
                case 74: /* J */
                case 75: /* K */
                case 76: /* L */
                case 77: /* M */
                case 78: /* N */
                case 79: /* O */
                case 80: /* P */
                case 81: /* Q */
                case 82: /* R */
                case 83: /* S */
                case 84: /* T */
                case 85: /* U */
                case 86: /* V */
                case 87: /* W */
                case 88: /* X */
                case 89: /* Y */
                case 90: /* Z */
                case 91: /* [ */
                case 92: /* \ */
                    return false;
                case 93: /* ] */
                    return true;
                case 94: /* ^ */
                case 95: /* _ */
                case 96: /* ` */
                case 97: /* a */
                case 98: /* b */
                case 99: /* c */
                case 100: /* d */
                case 101: /* e */
                case 102: /* f */
                case 103: /* g */
                case 104: /* h */
                case 105: /* i */
                case 106: /* j */
                case 107: /* k */
                case 108: /* l */
                case 109: /* m */
                case 110: /* n */
                case 111: /* o */
                case 112: /* p */
                case 113: /* q */
                case 114: /* r */
                case 115: /* s */
                case 116: /* t */
                case 117: /* u */
                case 118: /* v */
                case 119: /* w */
                case 120: /* x */
                case 121: /* y */
                case 122: /* z */
                case 123: /* { */
                case 124: /* | */
                case 125: /* } */
                case 126: /* ~ */
                case 127: /*   */
                    return false;
                default:
                    return true;
            }
        }
    }
}
