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
        /*
                    
            json
            element

            value
            object
            array
            string
            number
            "true"
            "false"
            "null"

            object
                '{' ws '}'
                '{' members '}'

            members
                member
                member ',' members

            member
                ws string ws ':' element

            array
                '[' ws ']'
                '[' elements ']'

            elements
                element
                element ',' elements

            element
                ws value ws

            string
            '"' characters '"'

            characters
                ""
                character characters

            character
                '0020' . '10FFFF' - '"' - '\'
            '\' escape

            escape
                '"'
                '\'
                '/'
                'b'
                'f'
                'n'
                'r'
                't'
                'u' hex hex hex hex

            hex
                digit
                'A' . 'F'
                'a' . 'f'

            number
                integer fraction exponent

            integer
                digit
                onenine digits
                '-' digit
                '-' onenine digits

            digits
                digit
                digit digits

            digit
                '0'
                onenine

            onenine
                '1' . '9'

            fraction
                ""
                '.' digits

            exponent
                ""
                'E' sign digits
                'e' sign digits

            sign
                ""
                '+'
                '-'

            ws
                ""
                '0020' ws
                '000A' ws
                '000D' ws
                '0009' ws

        */
        /*
        { "": value, }
        [ value, ]
        +-0-9.0-9E+-0-9
        true
        false
        null
        "([^"] | \\u[0-9]{4} | \r | \n | \t | \")*"
        */
        public const int InitialCharBufferSize = 64 * 1024;
        public int lineNo;
        public int lineOffset;
        public int numberSign;
        public ulong uNumber;
        public long sNumber;
        public int idxToken;
        public int offset;
        public ParserState State;
        public int offsetTokenStart;
        public bool FinalContent;

        public JsonParserUtf8(bool finalContent) {
            lineNo = 1;
            lineOffset = 0;
            numberSign = 0;
            uNumber = 0;
            sNumber = 0;
            idxToken = 0;
            offset = -1;
            State = 0;
            offsetTokenStart = 0;
            FinalContent = finalContent;
        }
        public void Parse(BoundedByteArray src, JsonReaderContext context, bool finalContent) {
            context.BoundedByteArray = src;
            context.FinalContent = finalContent;
            Parse(context);
        }
        public void Parse(JsonReaderContext context) {
            var usedSpan = context.BoundedByteArray.GetReadSpan();
            int length = usedSpan.Length;
            bool finalContent = context.FinalContent;

            int tokensLength = context.Tokens.Length;
            //
            byte current;

            ParserState state = this.State;
            //
            // now goto hell
            //
            while (++offset < length) {
                switch (state) {
                    case ParserState.Start: {
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
                                    --offset;
                                    while (++offset < length) {
                                        switch (usedSpan[offset]) {
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
                                                this.lineNo++;
                                                this.lineOffset = offset + 1;
                                                continue;
                                            case 11:
                                            case 12:
                                                continue;
                                            case 13: /* \r */
                                                this.lineNo++;
                                                if ((offset + 1) < length) {
                                                    if (usedSpan[offset + 1] == 10 /* \n */) {
                                                        ++offset;
                                                    }
                                                }
                                                this.lineOffset = offset + 1;
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
                                                --offset;
                                                goto lblSpaceNext;
                                        }
                                    }
                                    lblSpaceNext:
                                    offsetTokenStart = offset;
                                    continue;

                                case 33: /* ! */
                                    offsetTokenStart = offset;
                                    break;
                                case 34: /* " */
                                    state = ParserState.QuoteStart;
                                    offsetTokenStart = offset;
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
                                    state = ParserState.NumberSignPlus;
                                    this.numberSign = 1;
                                    offsetTokenStart = offset;
                                    continue;

                                case 44: /* , */
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.ValueSep);
                                    context.BoundedCharArray.GlobalProtected = -1;
                                    if (idxToken >= tokensLength) { goto lblTokensLength; }
                                    if (finalContent && (length - offset) < 128) {
                                        continue;
                                    } else {
                                        goto lblDone;
                                    }

                                case 45: /* - */
                                    state = ParserState.NumberSignMinus;
                                    this.numberSign = -1;
                                    offsetTokenStart = offset;
                                    continue;
                                case 46: /* . */
                                    state = ParserState.NumberIEEE;
                                    this.numberSign = 1;
                                    offsetTokenStart = offset;
                                    continue;
                                case 47: /* / */
                                    state = ParserState.Slash;
                                    offsetTokenStart = offset;
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
                                    state = ParserState.NumberInt;
                                    this.numberSign = 1;
                                    offsetTokenStart = offset;
                                    this.uNumber = (ulong)(current -/* '0' */48);
                                    continue;

                                case 58: /* : */
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.PairSep);
                                    if (idxToken >= tokensLength) { goto lblTokensLength; }
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
                                    state = ParserState.F;
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
                                    state = ParserState.N;
                                    offsetTokenStart = offset;
                                    continue;
                                case 79: /* O */
                                case 80: /* P */
                                case 81: /* Q */
                                case 82: /* R */
                                case 83: /* S */
                                    break;
                                case 84: /* T */
                                    state = ParserState.T;
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
                                    if (idxToken >= tokensLength) { goto lblTokensLength; }
                                    offsetTokenStart = offset + 1;
                                    continue;

                                case 92: /* \ */
                                    break;

                                case 93: /* ] */
                                    context.BoundedCharArray.GlobalProtected = -1;
                                    //
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.ArrayEnd);
                                    if (idxToken >= tokensLength) { goto lblTokensLength; }
                                    offsetTokenStart = offset + 1;
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
                                    state = ParserState.F;
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
                                    state = ParserState.N;
                                    offsetTokenStart = offset;
                                    continue;

                                case 111: /* o */
                                case 112: /* p */
                                case 113: /* q */
                                case 114: /* r */
                                case 115: /* s */
                                    break;

                                case 116: /* t */
                                    state = ParserState.T;
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
                                    if (idxToken >= tokensLength) { goto lblTokensLength; }
                                    offsetTokenStart = offset + 1;
                                    continue;

                                case 124: /* | */
                                    break;
                                case 125: /* } */
                                    context.BoundedCharArray.GlobalProtected = -1;
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.ObjectEnd);
                                    if (idxToken >= tokensLength) { goto lblTokensLength; }
                                    offsetTokenStart = offset + 1;
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
                        switch (current = usedSpan[offset]) {
                            case (byte)'R':
                            case (byte)'r':
                                state = ParserState.TR;
                                if (++offset < length) {
                                    goto lblTR;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }

                    case ParserState.TR:
                        lblTR:
                        switch (current = usedSpan[offset]) {
                            case (byte)'U':
                            case (byte)'u':
                                state = ParserState.TRU;
                                if (++offset < length) {
                                    goto lblTRU;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }

                    case ParserState.TRU:
                        lblTRU:
                        switch (current = usedSpan[offset]) {
                            case (byte)'E':
                            case (byte)'e':

                                var offset1 = offset + 1;
                                if (((offset1) < length)
                                    ? GetEndOfValueNextValue(usedSpan[offset1])
                                    : finalContent
                                    ) {
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.True);
                                    if (idxToken >= tokensLength) { goto lblTokensLength; }
                                    state = ParserState.Start;
                                    offsetTokenStart = offset1;
                                    continue;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }

                    case ParserState.F:
                        switch (current = usedSpan[offset]) {
                            case (byte)'A':
                            case (byte)'a':
                                state = ParserState.TRU;
                                if (++offset < length) {
                                    goto lblFA;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }

                    case ParserState.FA:
                        lblFA:
                        switch (current = usedSpan[offset]) {
                            case (byte)'L':
                            case (byte)'l':
                                state = ParserState.FAL;
                                if (++offset < length) {
                                    goto lblFAL;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }

                    case ParserState.FAL:
                        lblFAL:
                        switch (current = usedSpan[offset]) {
                            case (byte)'S':
                            case (byte)'s':
                                state = ParserState.FALS;
                                if (++offset < length) {
                                    goto lblFALS;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }

                    case ParserState.FALS:
                        lblFALS:
                        switch (current = usedSpan[offset]) {
                            case (byte)'E':
                            case (byte)'e':
                                if ((++offset < length)
                                    ? GetEndOfValueNextValue(usedSpan[offset])
                                    : finalContent
                                    ) {
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.False);
                                    if (idxToken >= tokensLength) { goto lblTokensLength; }
                                    state = ParserState.Start;
                                    offsetTokenStart = offset;
                                    continue;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }

                    case ParserState.N:
                        switch (current = usedSpan[offset]) {
                            case (byte)'U':
                            case (byte)'u':
                                state = ParserState.NU;
                                if (++offset < length) {
                                    goto lblNU;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }

                    case ParserState.NU:
                        lblNU:
                        switch (current = usedSpan[offset]) {
                            case (byte)'L':
                            case (byte)'l':
                                state = ParserState.NUL;
                                if (++offset < length) {
                                    goto lblNUL;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }

                    case ParserState.NUL:
                        lblNUL:
                        switch (current = usedSpan[offset]) {
                            case (byte)'L':
                            case (byte)'l':
                                if ((++offset < length)
                                    ? GetEndOfValueNextValue(usedSpan[offset])
                                    : finalContent
                                    ) {
                                    context.Tokens[idxToken++].SetKind(JsonTokenKind.Null);
                                    if (idxToken >= tokensLength) { goto lblTokensLength; }
                                    state = ParserState.Start;
                                    continue;
                                } else {
                                    --offset;
                                    goto lblNeedMoreContent;
                                }
                            default:
                                goto lblUnexpectedChar;
                        }

                    case ParserState.QuoteStart:
                        context.BoundedCharArray.GlobalProtected = offset + context.BoundedCharArray.GlobalShift;
                        --offset;
                        while (++offset < length) {
                            switch (current = usedSpan[offset]) {
                                case 10: { /* \n */
                                        this.lineNo++;
                                        this.lineOffset = offset + 1;
                                    }
                                    continue;

                                case 13: { /* \r */
                                        this.lineNo++;
                                        if ((offset + 1) < length) {
                                            current = usedSpan[offset + 1];
                                            if (current == 10) {
                                                ++offset;
                                            }
                                        }
                                        this.lineOffset = offset + 1;
                                    }
                                    continue;

                                case (byte)'"': {
                                        context.Tokens[idxToken++].SetKindUtf8(JsonTokenKind.StringSimpleUtf8, offsetTokenStart + 1, offset - 1);
                                        if (idxToken >= tokensLength) { goto lblTokensLength; }
                                        state = ParserState.Start;
                                        offsetTokenStart = offset + 1;
                                    }
                                    goto lblQuoteStartNext;


                                case (byte)'\\': {
                                        --offset;
                                        state = ParserState.QuoteContentComplex;
                                        //state = ParserState.QuoteBackSlash;
                                        var len = (offset - offsetTokenStart);
                                        if (len > 0) {
                                            context.BoundedCharArray.AdjustBeforeFeeding(
                                            len,
                                            InitialCharBufferSize
                                            );
                                            var countChars = StringUtility.ConvertFromUtf8(
                                                usedSpan.Slice(offsetTokenStart, len),
                                                context.BoundedCharArray.GetFeedSpan());
                                            context.BoundedCharArray.AdjustAfterFeeding(countChars);
                                        }
                                    }
                                    goto lblQuoteStartNext;

                                default:
                                    if (current < 127) {
                                        // short loop
                                        // add key magic here 
                                        continue;
                                    } else {
                                        --offset;
                                        state = ParserState.QuoteContentComplex;
                                        var len = (offset - offsetTokenStart);
                                        if (len > 0) {
                                            context.BoundedCharArray.AdjustBeforeFeeding(len, InitialCharBufferSize);
                                            var countChars = StringUtility.ConvertFromUtf8(
                                                usedSpan.Slice(offsetTokenStart, len),
                                                context.BoundedCharArray.GetFeedSpan());
                                            context.BoundedCharArray.AdjustAfterFeeding(countChars);
                                        }
                                        goto lblQuoteStartNext;
                                    }
                            }
                        }
                        lblQuoteStartNext:
                        break;

                    case ParserState.QuoteContentComplex:
                        --offset;
                        while (++offset < length) {
                            switch (current = usedSpan[offset]) {
                                case 10: /* \n */
                                    this.lineNo++;
                                    this.lineOffset = offset + 1;
                                    continue;
                                case 13: /* \r */
                                    this.lineNo++;
                                    if ((offset + 1) < length) {
                                        current = usedSpan[offset + 1];
                                        if (current == 10) {
                                            ++offset;
                                        }
                                    }
                                    this.lineOffset = offset + 1;
                                    continue;

                                case (byte)'"':
                                    if ((++offset < length)
                                       ? GetEndOfValueNextValue(usedSpan[offset])
                                       : finalContent
                                       ) {
                                        context.Tokens[idxToken++].SetKind(JsonTokenKind.StringSimpleUtf8);
                                        if (idxToken >= tokensLength) { goto lblTokensLength; }
                                        state = ParserState.Start;
                                        goto lblQuoteContentComplexNext;
                                    } else {
                                        --offset;
                                        goto lblNeedMoreContent;
                                    }

                                case (byte)'\\':
                                    state = ParserState.QuoteBackSlash;
                                    goto lblQuoteContentComplexNext;

                                default:
                                    ++offset;
                                    break;
                            }
                        }
                        lblQuoteContentComplexNext:
                        break;

                    case ParserState.QuoteBackSlash:
                        continue;

                    case ParserState.Slash:
                        switch (current = usedSpan[offset]) {
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
            lblDone:
            this.State = state;
            //this.State = (int)state;
            //this.idxToken = idxToken;
            //this.offset = offset;
            //this.offsetTokenStart = offsetTokenStart;
            context.ReadIndexToken = 0;
            context.FeedIndexToken = idxToken;
            return;

            lblNeedMoreContent:
            throw new NotImplementedException($"TODO {offset}");

            lblUnexpectedChar:
            throw new ArgumentException($"Unexpected Char {current} L:{this.lineNo}; C:{offset - this.lineOffset + 1};");

            lblTokensLength:
            throw new InvalidOperationException("TokensLength too big.");
        }

        public void Finalize(BoundedByteArray src, JsonReaderContext context) {
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
