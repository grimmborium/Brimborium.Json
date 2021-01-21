using System;

namespace Brimborium.Json {
    public struct JsonParserUtf8 {
        enum State {
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
            QuoteStart,
            NumberSignPlus,
            NumberSignMinus,
            NumberInt,
            NumberIEEE,
            EOF // must be the last
        }
        public void Parse(BoundedByteArray src, JsonReaderContext context) {
            context.SourceByteArray = src;

            var usedSpan = src.GetUsedSpan();
            int length = usedSpan.Length;
            int tokensLength = context.Tokens.Length;
            //
            int lineNo = context.SaveStateUtf8.lineNo;
            int lineOffset = context.SaveStateUtf8.lineOffset;
            int numberSign = context.SaveStateUtf8.numberSign;
            ulong uNumber = context.SaveStateUtf8.uNumber;
            long sNumber = context.SaveStateUtf8.sNumber;
            int idxToken = 0;
            int offset = -1;
            int offsetTokenStart = 0;
            //
            byte current;
            var iState = context.SaveStateUtf8.State;
            if (((int)State.Start <= iState) && (iState <= (int)State.EOF)) {
                // OK
            } else {
                throw new InvalidOperationException("Invalid Parser state");
            }
            //
            // goto hell
            //
            State state = (State)iState;
            while (++(offset) < length) {
                switch (state) {
                    case State.Start: {
                            switch (current = usedSpan[offset]) {
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
                                    lineNo++;
                                    lineOffset = offset + 1;
                                    continue;
                                case 11:
                                case 12:
                                    continue;
                                case 13: /* \r */
                                    lineNo++;
                                    if ((offset + 1) < length) {
                                        current = usedSpan[offset + 1];
                                        if (current == 10) {
                                            ++offset;
                                        }
                                    }
                                    lineOffset = offset + 1;
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
                                case 33: /* ! */
                                    break;
                                case 34: /* " */
                                    state = State.QuoteStart;
                                    offsetTokenStart = offset;
                                    break;
                                case 35: /* # */
                                    break;
                                case 36: /* $ */
                                    break;
                                case 37: /* % */
                                    break;
                                case 38: /* & */
                                    break;
                                case 39: /* ' */
                                    break;
                                case 40: /* ( */
                                    break;
                                case 41: /* ) */
                                    break;
                                case 42: /* * */
                                    break;
                                case 43: /* + */
                                    state = State.NumberSignPlus;
                                    numberSign = 1;
                                    offsetTokenStart = offset;
                                    continue;

                                case 44: /* , */
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.ValueSep);
                                    if (idxToken >= tokensLength) goto lblTokensLength;
                                    goto lblDone;

                                case 45: /* - */
                                    state = State.NumberSignMinus;
                                    numberSign = -1;
                                    offsetTokenStart = offset;
                                    continue;
                                case 46: /* . */
                                    state = State.NumberIEEE;
                                    numberSign = 1;
                                    offsetTokenStart = offset;
                                    continue;
                                case 47: /* / */
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
                                    state = State.NumberInt;
                                    numberSign = 1;
                                    offsetTokenStart = offset;
                                    uNumber = (ulong)(current -/* '0' */48);
                                    continue;

                                case 58: /* : */
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.PairSep);
                                    if (idxToken >= tokensLength) goto lblTokensLength;
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
                                    state = State.F;
                                    offsetTokenStart = offset;
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
                                    state = State.N;
                                    offsetTokenStart = offset;
                                    continue;
                                case 79: /* O */
                                case 80: /* P */
                                case 81: /* Q */
                                case 82: /* R */
                                case 83: /* S */
                                    break;
                                case 84: /* T */
                                    state = State.T;
                                    offsetTokenStart = offset;
                                    continue;
                                case 85: /* U */
                                case 86: /* V */
                                case 87: /* W */
                                case 88: /* X */
                                case 89: /* Y */
                                case 90: /* Z */
                                    break;

                                case 91: /* [ */
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.ArrayStart);
                                    if (idxToken >= tokensLength) goto lblTokensLength;
                                    continue;

                                case 92: /* \ */
                                    break;

                                case 93: /* ] */
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.ArrayEnd);
                                    if (idxToken >= tokensLength) goto lblTokensLength;
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
                                    state = State.F;
                                    offsetTokenStart = offset;
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
                                    state = State.N;
                                    offsetTokenStart = offset;
                                    continue;
                                case 111: /* o */
                                case 112: /* p */
                                case 113: /* q */
                                case 114: /* r */
                                case 115: /* s */
                                    break;
                                case 116: /* t */
                                    state = State.T;
                                    offsetTokenStart = offset;
                                    continue;
                                case 117: /* u */
                                case 118: /* v */
                                case 119: /* w */
                                case 120: /* x */
                                case 121: /* y */
                                case 122: /* z */
                                    break;
                                case 123: /* { */
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.ObjectStart);
                                    if (idxToken >= tokensLength) goto lblTokensLength;
                                    continue;
                                case 124: /* | */
                                    break;
                                case 125: /* } */
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.ObjectEnd);
                                    if (idxToken >= tokensLength) goto lblTokensLength;
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
                            //throw new ArgumentException($"Unexpected Char {current} L:{lineNo}; C:{offset - lineOffset + 1};");
                        }

                    case State.T:
                        switch (current = usedSpan[offset]) {
                            case (byte)'R':
                            case (byte)'r':
                                state = State.TR;
                                if (++offset < length) {
                                    goto lblTR;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }

                    case State.TR:
                        lblTR:
                        switch (current = usedSpan[offset]) {
                            case (byte)'U':
                            case (byte)'u':
                                state = State.TRU;
                                if (++offset < length) {
                                    goto lblTRU;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }

                    case State.TRU:
                        lblTRU:
                        switch (current = usedSpan[offset]) {
                            case (byte)'E':
                            case (byte)'e':
                                if (++offset < length  || finalContent) {
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.True);
                                    if (idxToken >= tokensLength) goto lblTokensLength;
                                    continue;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }
                    case State.F:
                        switch (current = usedSpan[offset]) {
                            case (byte)'A':
                            case (byte)'a':
                                state = State.TRU;
                                if (++offset < length) {
                                    goto lblFA;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }
                    case State.FA:
                        lblFA:
                        switch (current = usedSpan[offset]) {
                            case (byte)'L':
                            case (byte)'l':
                                state = State.FAL;
                                if (++offset < length) {
                                    goto lblFAL;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }
                    case State.FAL:
                        lblFAL:
                        switch (current = usedSpan[offset]) {
                            case (byte)'S':
                            case (byte)'s':
                                state = State.FALS;
                                if (++offset < length) {
                                    goto lblFALS;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }
                    case State.FALS:
                        lblFALS:
                        switch (current = usedSpan[offset]) {
                            case (byte)'E':
                            case (byte)'e':
                                if (++offset < length) {
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.False);
                                    if (idxToken >= tokensLength) goto lblTokensLength;
                                    continue;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }
                    case State.N:
                        switch (current = usedSpan[offset]) {
                            case (byte)'U':
                            case (byte)'u':
                                state = State.NU;
                                if (++offset < length) {
                                    goto lblNU;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }
                    case State.NU:
                        lblNU:
                        switch (current = usedSpan[offset]) {
                            case (byte)'L':
                            case (byte)'l':
                                state = State.NUL;
                                if (++offset < length) {
                                    goto lblNUL;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }
                    case State.NUL:
                        lblNUL:
                        switch (current = usedSpan[offset]) {
                            case (byte)'L':
                            case (byte)'l':
                                if (++offset < length) {
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.Null);
                                    if (idxToken >= tokensLength) goto lblTokensLength;
                                    continue;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }
                    case State.QuoteStart:
                        break;
                    case State.NumberSignMinus:
                    case State.NumberSignPlus:
                        break;
                    case State.NumberInt:
                        break;
                    default:
                        break;
                }
            }
            //
            lblDone:
            context.SaveStateUtf8.State = (int)state;
            context.SaveStateUtf8.lineNo = lineNo;
            context.SaveStateUtf8.lineOffset = lineOffset;
            context.SaveStateUtf8.numberSign = numberSign;
            context.SaveStateUtf8.uNumber = uNumber;
            context.SaveStateUtf8.sNumber = sNumber;
            context.SaveStateUtf8.idxToken = idxToken;
            context.SaveStateUtf8.offset = offset;
            context.SaveStateUtf8.offsetTokenStart = offsetTokenStart;
            context.IndexToken = 0;
            context.CountToken = idxToken;
            return;

            lblNeedMoreContent:
            throw new NotImplementedException("TODO");

            lblUnexpectedChar:
            throw new ArgumentException($"Unexpected Char {current} L:{lineNo}; C:{offset - lineOffset + 1};");

            lblTokensLength:
            throw new InvalidOperationException("TokensLength too big.");
        }
        public void Finalize(BoundedByteArray src, JsonReaderContext context) {
        }
    }
    public struct JsonParserUtf16 {
        public void Parse(BoundedCharArray src, JsonReaderContext context) {
        }
    }

    public struct JsonReaderContextStateUtf8 {
        public int lineNo;
        public int lineOffset;
        public int numberSign;
        public ulong uNumber;
        public long sNumber;
        public int idxToken;
        public int offset;
        public int State;
        public int offsetTokenStart;

        public static JsonReaderContextStateUtf8 Start() {
            var r = new JsonReaderContextStateUtf8();
            r.lineNo = 1;
            r.lineOffset = 0;
            r.numberSign = 0;
            r.uNumber = 0;
            r.sNumber = 0;
            r.idxToken = 0;
            r.offset = -1;
            r.State = 0;
            r.offsetTokenStart = 0;
            return r;
        }

    }
}
