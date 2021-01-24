using System;

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

        public JsonParserUtf8(JsonReaderContext jsonReaderContext) {
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
            var usedSpan = Context.BoundedByteArray.GetReadSpan();
            int length = usedSpan.Length;
            bool finalContent = Context.FinalContent;
            int tokensLength = Context.Tokens.Length;
            byte current;
            if (Context.ReadIndexToken == Context.FeedIndexToken) {
                Context.ReadIndexToken = 0;
                Context.FeedIndexToken = 0;
            }
            if (Context.FeedIndexToken < Context.Tokens.Length) {
                //
                // now goto hell
                //
                while (++Offset < length) {
                    switch (State) {
                        case ParserState.Start: {
                                switch (current = usedSpan[Offset]) {
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
                                        --Offset;
                                        while (++Offset < length) {
                                            switch (usedSpan[Offset]) {
                                                case 1:
                                                case 2:
                                                case 3:
                                                case 4:
                                                case 5:
                                                case 6:
                                                case 7:
                                                case 8:
                                                case 9: /* \t */
                                                    continue;
                                                case 10: /* \n */
                                                    this.LineNo++;
                                                    this.LineOffset = Offset + 1;
                                                    continue;
                                                case 11:
                                                case 12:
                                                    continue;
                                                case 13: /* \r */
                                                    this.LineNo++;
                                                    if ((Offset + 1) < length) {
                                                        if (usedSpan[Offset + 1] == 10 /* \n */) {
                                                            ++Offset;
                                                        }
                                                    }
                                                    this.LineOffset = Offset + 1;
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
                                                    continue;

                                                default:
                                                    --Offset;
                                                    goto lblSpaceNext;
                                            }
                                        }
                                        lblSpaceNext:
                                        OffsetTokenStart = Offset;
                                        continue;

                                    case 33: /* ! */
                                        OffsetTokenStart = Offset;
                                        break;
                                    case 34: /* " */
                                        State = ParserState.QuoteStart;
                                        OffsetTokenStart = Offset;
                                        break;
                                    case 35: /* # */
                                    case 36: /* $ */
                                    case 37: /* % */
                                    case 38: /* & */
                                    case 39: /* ' */
                                    case 40: /* ( */
                                    case 41: /* ) */
                                    case 42: /* * */
                                        break;
                                    case 43: /* + */
                                        State = ParserState.NumberSignPlus;
                                        this.NumberSign = 1;
                                        OffsetTokenStart = Offset;
                                        continue;

                                    case 44: /* , */
                                        Context.Tokens[IdxToken++] = JsonReaderContext.TokenValueSep;
                                        Context.BoundedCharArray.GlobalProtected = -1;
                                        if (IdxToken >= tokensLength) { goto lblTokensLength; }
                                        if (finalContent && (length - Offset) < 128) {
                                            continue;
                                        } else {
                                            goto lblDone;
                                        }

                                    case 45: /* - */
                                        State = ParserState.NumberSignMinus;
                                        this.NumberSign = -1;
                                        OffsetTokenStart = Offset;
                                        continue;
                                    case 46: /* . */
                                        State = ParserState.NumberIEEE;
                                        this.NumberSign = 1;
                                        OffsetTokenStart = Offset;
                                        continue;
                                    case 47: /* / */
                                        State = ParserState.Slash;
                                        OffsetTokenStart = Offset;
                                        break;

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
                                        continue;

                                    case 58: /* : */
                                        Context.Tokens[IdxToken++] = JsonReaderContext.TokenPairSep;
                                        if (IdxToken >= tokensLength) { goto lblTokensLength; }
                                        continue;

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
                                        break;
                                    case 70: /* F */
                                        State = ParserState.F;
                                        OffsetTokenStart = Offset;
                                        continue;
                                    case 71: /* G */
                                    case 72: /* H */
                                    case 73: /* I */
                                    case 74: /* J */
                                    case 75: /* K */
                                    case 76: /* L */
                                    case 77: /* M */
                                        break;
                                    case 78: /* N */
                                        State = ParserState.N;
                                        OffsetTokenStart = Offset;
                                        continue;
                                    case 79: /* O */
                                    case 80: /* P */
                                    case 81: /* Q */
                                    case 82: /* R */
                                    case 83: /* S */
                                        break;
                                    case 84: /* T */
                                        State = ParserState.T;
                                        OffsetTokenStart = Offset;
                                        continue;
                                    case 85: /* U */
                                    case 86: /* V */
                                    case 87: /* W */
                                    case 88: /* X */
                                    case 89: /* Y */
                                    case 90: /* Z */
                                        break;

                                    case 91: /* [ */
                                        Context.Tokens[IdxToken++] = JsonReaderContext.TokenArrayStart;
                                        if (IdxToken >= tokensLength) { goto lblTokensLength; }
                                        OffsetTokenStart = Offset + 1;
                                        continue;

                                    case 92: /* \ */
                                        break;

                                    case 93: /* ] */
                                        Context.BoundedCharArray.GlobalProtected = -1;
                                        //
                                        Context.Tokens[IdxToken++] = JsonReaderContext.TokenArrayEnd;
                                        if (IdxToken >= tokensLength) { goto lblTokensLength; }
                                        OffsetTokenStart = Offset + 1;
                                        goto lblDone;

                                    case 94: /* ^ */
                                    case 95: /* _ */
                                    case 96: /* ` */
                                    case 97: /* a */
                                    case 98: /* b */
                                    case 99: /* c */
                                    case 100: /* d */
                                    case 101: /* e */
                                        break;

                                    case 102: /* f */
                                        State = ParserState.F;
                                        OffsetTokenStart = Offset;
                                        continue;

                                    case 103: /* g */
                                    case 104: /* h */
                                    case 105: /* i */
                                    case 106: /* j */
                                    case 107: /* k */
                                    case 108: /* l */
                                    case 109: /* m */
                                        break;

                                    case 110: /* n */
                                        State = ParserState.N;
                                        OffsetTokenStart = Offset;
                                        continue;

                                    case 111: /* o */
                                    case 112: /* p */
                                    case 113: /* q */
                                    case 114: /* r */
                                    case 115: /* s */
                                        break;

                                    case 116: /* t */
                                        State = ParserState.T;
                                        OffsetTokenStart = Offset;
                                        continue;

                                    case 117: /* u */
                                    case 118: /* v */
                                    case 119: /* w */
                                    case 120: /* x */
                                    case 121: /* y */
                                    case 122: /* z */
                                        break;

                                    case 123: /* { */
                                        Context.Tokens[IdxToken++] = JsonReaderContext.TokenObjectStart;
                                        if (IdxToken >= tokensLength) { goto lblTokensLength; }
                                        OffsetTokenStart = Offset + 1;
                                        continue;

                                    case 124: /* | */
                                        break;
                                    case 125: /* } */
                                        Context.BoundedCharArray.GlobalProtected = -1;
                                        Context.Tokens[IdxToken++] = JsonReaderContext.TokenObjectEnd;
                                        if (IdxToken >= tokensLength) { goto lblTokensLength; }
                                        OffsetTokenStart = Offset + 1;
                                        goto lblDone;

                                    case 126: /* ~ */
                                        break;
                                    case 127: /*   */
                                        break;
                                    default:
                                        break;
                                }
                                //
                                goto lblUnexpectedChar;
                            }

                        case ParserState.T:
                            switch (current = usedSpan[Offset]) {
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
                            switch (current = usedSpan[Offset]) {
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
                            switch (current = usedSpan[Offset]) {
                                case (byte)'E':
                                case (byte)'e':

                                    var offset1 = Offset + 1;
                                    if (((offset1) < length)
                                        ? GetEndOfValueNextValue(usedSpan[offset1])
                                        : finalContent
                                        ) {
                                        Context.Tokens[IdxToken++] = JsonReaderContext.TokenTrue;
                                        if (IdxToken >= tokensLength) { goto lblTokensLength; }
                                        State = ParserState.Start;
                                        OffsetTokenStart = offset1;
                                        continue;
                                    } else {
                                        --Offset;
                                        goto lblNeedMoreContent;
                                    }
                                default:
                                    goto lblUnexpectedChar;
                            }

                        case ParserState.F:
                            switch (current = usedSpan[Offset]) {
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
                            switch (current = usedSpan[Offset]) {
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
                            switch (current = usedSpan[Offset]) {
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
                            switch (current = usedSpan[Offset]) {
                                case (byte)'E':
                                case (byte)'e':
                                    if ((++Offset < length)
                                        ? GetEndOfValueNextValue(usedSpan[Offset])
                                        : finalContent
                                        ) {
                                        Context.Tokens[IdxToken++] = JsonReaderContext.TokenFalse;
                                        if (IdxToken >= tokensLength) { goto lblTokensLength; }
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
                            switch (current = usedSpan[Offset]) {
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
                            switch (current = usedSpan[Offset]) {
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
                            switch (current = usedSpan[Offset]) {
                                case (byte)'L':
                                case (byte)'l':
                                    if ((++Offset < length)
                                        ? GetEndOfValueNextValue(usedSpan[Offset])
                                        : finalContent
                                        ) {
                                        Context.Tokens[IdxToken++] = JsonReaderContext.TokenNull;
                                        if (IdxToken >= tokensLength) { goto lblTokensLength; }
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
                            Context.BoundedCharArray.GlobalProtected = Offset + Context.BoundedCharArray.GlobalShift;
                            --Offset;
                            while (++Offset < length) {
                                switch (current = usedSpan[Offset]) {
                                    case 10: { /* \n */
                                            this.LineNo++;
                                            this.LineOffset = Offset + 1;
                                        }
                                        continue;

                                    case 13: { /* \r */
                                            this.LineNo++;
                                            if ((Offset + 1) < length) {
                                                current = usedSpan[Offset + 1];
                                                if (current == 10) {
                                                    ++Offset;
                                                }
                                            }
                                            this.LineOffset = Offset + 1;
                                        }
                                        continue;

                                    case (byte)'"': {
                                            var jsonToken = this.Context.RentFromTokenCache();
                                            jsonToken.SetKindUtf8(JsonTokenKind.StringSimpleUtf8, OffsetTokenStart + 1, Offset - 1);
                                            Context.Tokens[IdxToken++] = jsonToken;
                                            if (IdxToken >= tokensLength) { goto lblTokensLength; }
                                            State = ParserState.Start;
                                            OffsetTokenStart = Offset + 1;
                                        }
                                        goto lblQuoteStartNext;


                                    case (byte)'\\': {
                                            --Offset;
                                            State = ParserState.QuoteContentComplex;
                                            //state = ParserState.QuoteBackSlash;
                                            var len = (Offset - OffsetTokenStart);
                                            if (len > 0) {
                                                Context.BoundedCharArray.AdjustBeforeFeeding(
                                                len,
                                                InitialCharBufferSize
                                                );
                                                var countChars = StringUtility.ConvertFromUtf8(
                                                    usedSpan.Slice(OffsetTokenStart, len),
                                                    Context.BoundedCharArray.GetFeedSpan());
                                                Context.BoundedCharArray.AdjustAfterFeeding(countChars);
                                            }
                                        }
                                        goto lblQuoteStartNext;

                                    default:
                                        if (current < 127) {
                                            // short loop
                                            // add key magic here 
                                            continue;
                                        } else {
                                            --Offset;
                                            State = ParserState.QuoteContentComplex;
                                            var len = (Offset - OffsetTokenStart);
                                            if (len > 0) {
                                                Context.BoundedCharArray.AdjustBeforeFeeding(len, InitialCharBufferSize);
                                                var countChars = StringUtility.ConvertFromUtf8(
                                                    usedSpan.Slice(OffsetTokenStart, len),
                                                    Context.BoundedCharArray.GetFeedSpan());
                                                Context.BoundedCharArray.AdjustAfterFeeding(countChars);
                                            }
                                            goto lblQuoteStartNext;
                                        }
                                }
                            }
                            lblQuoteStartNext:
                            break;

                        case ParserState.QuoteContentComplex:
                            --Offset;
                            while (++Offset < length) {
                                switch (current = usedSpan[Offset]) {
                                    case 10: /* \n */
                                        this.LineNo++;
                                        this.LineOffset = Offset + 1;
                                        continue;
                                    case 13: /* \r */
                                        this.LineNo++;
                                        if ((Offset + 1) < length) {
                                            current = usedSpan[Offset + 1];
                                            if (current == 10) {
                                                ++Offset;
                                            }
                                        }
                                        this.LineOffset = Offset + 1;
                                        continue;

                                    case (byte)'"':
                                        if ((++Offset < length)
                                           ? GetEndOfValueNextValue(usedSpan[Offset])
                                           : finalContent
                                           ) {
                                            Context.Tokens[IdxToken++].SetKind(JsonTokenKind.StringSimpleUtf8);
                                            if (IdxToken >= tokensLength) { goto lblTokensLength; }
                                            State = ParserState.Start;
                                            goto lblQuoteContentComplexNext;
                                        } else {
                                            --Offset;
                                            goto lblNeedMoreContent;
                                        }

                                    case (byte)'\\':
                                        State = ParserState.QuoteBackSlash;
                                        goto lblQuoteContentComplexNext;

                                    default:
                                        ++Offset;
                                        break;
                                }
                            }
                            lblQuoteContentComplexNext:
                            break;

                        case ParserState.QuoteBackSlash:
                            continue;

                        case ParserState.Slash:
                            switch (current = usedSpan[Offset]) {
                                case (byte)'/':
                                    break;
                                case (byte)'*':
                                    break;
                            }
                            break;

                        case ParserState.NumberSignMinus:
                        case ParserState.NumberSignPlus:
                            break;
                        case ParserState.NumberInt:
                            break;
                        default:
                            break;
                    }
                }
                //
            }
            //
            lblDone:

            return;

            lblNeedMoreContent:
            throw new NotImplementedException($"TODO {Offset}");

            lblUnexpectedChar:
            throw new ArgumentException($"Unexpected Char {current} L:{this.LineNo}; C:{Offset - this.LineOffset + 1};");

            lblTokensLength:
            throw new InvalidOperationException("TokensLength too big.");
        }

        public void Finalize(BoundedByteArray src) {
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
