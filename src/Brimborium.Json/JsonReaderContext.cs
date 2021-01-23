#pragma warning disable IDE0041 // Use 'is null' check
using System.Threading;

namespace Brimborium.Json {
    public class JsonReaderContext {
        public static JsonToken TokenEOF = new JsonToken(JsonTokenKind.EOF);
        public static JsonToken TokenFault = new JsonToken(JsonTokenKind.Fault);
        public static JsonToken TokenReadAwait = new JsonToken(JsonTokenKind.ReadAwait);
        public int ReadIndexToken;
        public int FeedIndexToken;
        public int CountToken => FeedIndexToken - ReadIndexToken;

        public JsonToken[] Tokens;
        public BoundedByteArray BoundedByteArray;
        public BoundedCharArray BoundedCharArray;
        public bool FinalContent;


        public JsonReaderContext() {
            this.Tokens = new JsonToken[128];
            for (int idx = 0; idx < this.Tokens.Length; idx++) {
                this.Tokens[idx] = new JsonToken();
            }

            this.BoundedByteArray = BoundedByteArray.Empty();
            this.BoundedCharArray = BoundedCharArray.Empty();
            //this.SaveStateUtf8 = JsonReaderContextStateUtf8.Start();
            this.ReadIndexToken = 0;
            this.FeedIndexToken = 0;
        }

        public void Reset() {
            this.SaveStateUtf8 = JsonReaderContextStateUtf8.Start();
            r.lineNo = 1;
            r.lineOffset = 0;
            r.numberSign = 0;
            r.uNumber = 0;
            r.sNumber = 0;
            r.idxToken = 0;
            r.offset = -1;
            r.State = 0;
            r.offsetTokenStart = 0;

            this.ReadIndexToken = 0;
            this.FeedIndexToken = 0;
        }

        public bool HasToken() => (ReadIndexToken < FeedIndexToken);

        public JsonToken GetCurrentToken()
            => (ReadIndexToken < FeedIndexToken)
            ? Tokens[ReadIndexToken]
            : (FinalContent)
                ? TokenEOF
                : TokenReadAwait;

        public JsonToken ReadCurrentToken()
            => (ReadIndexToken < FeedIndexToken)
            ? Tokens[ReadIndexToken++]
            : (FinalContent)
                ? TokenEOF
                : TokenReadAwait;

        public bool TryPeekToken(out JsonToken jsonToken) {
            if (ReadIndexToken < FeedIndexToken) {
                jsonToken = Tokens[ReadIndexToken];
                return true;
            } else {
                if (FinalContent) {
                    jsonToken = TokenEOF;
                } else {
                    jsonToken = TokenReadAwait;
                }
                return false;
            }
        }
        public bool TryReadToken(out JsonToken jsonToken) {
            if (ReadIndexToken < FeedIndexToken) {
                jsonToken = Tokens[ReadIndexToken++];
                return true;
            } else {
                if (FinalContent) {
                    jsonToken = TokenEOF;
                } else {
                    jsonToken = TokenReadAwait;
                }
                return false;
            }
        }

        //  public JsonReaderContextStateUtf8 SaveStateUtf8;
    }

    public class JsonReaderContextPool {
        private static JsonReaderContextPool? _Instance;
        public static JsonReaderContextPool Instance => (_Instance ??= new JsonReaderContextPool());

        public JsonReaderContext?[] _Contexts;

        public JsonReaderContextPool() {
            this._Contexts = new JsonReaderContext[8];
        }
        public JsonReaderContext Rent() {
            int slot = Thread.CurrentThread.ManagedThreadId;
            for (int idx = 0; idx < this._Contexts.Length; idx++) {
                if (this._Contexts[(idx + slot) % 8] is object) {
                    var result = System.Threading.Interlocked.Exchange(ref this._Contexts[(idx + slot) % 8], null);
                    if (result is object) {
                        return result;
                    }
                }
            }

            return new JsonReaderContext();
        }

        public void Return(JsonReaderContext context) {
            if (context is null) {
                // nothing to do
            } else {
                context.Reset();

                int slot = Thread.CurrentThread.ManagedThreadId;
                for (int idx = 0; idx < this._Contexts.Length; idx++) {
                    if (this._Contexts[(idx + slot) % 8] is null) {
                        if (ReferenceEquals(
                            System.Threading.Interlocked.CompareExchange(ref this._Contexts[(idx + slot) % 8], null, context),
                            null)) {
                            return;
                        }
                    }
                }
            }
        }
    }
}

