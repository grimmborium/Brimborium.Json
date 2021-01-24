#pragma warning disable IDE0041 // Use 'is null' check
using System.Collections.Generic;
using System.Threading;

namespace Brimborium.Json {
    public class JsonReaderContext {
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

        public int ReadIndexToken;
        public int FeedIndexToken;
        public int CountToken => FeedIndexToken - ReadIndexToken;

        public JsonToken[] Tokens;
        public JsonToken[] TokenCache;
        public int TokenCacheCount;
        public BoundedByteArray BoundedByteArray;
        public BoundedCharArray BoundedCharArray;
        public bool FinalContent;
        public readonly Stack<JsonTokenKind> Stack;

        public JsonReaderContext() {
            this.TokenCache = new JsonToken[128];
            this.Tokens = new JsonToken[128];
            for (int idx = 0; idx < this.Tokens.Length; idx++) {
                this.Tokens[idx] = TokenFault;
            }

            this.BoundedByteArray = BoundedByteArray.Empty();
            this.BoundedCharArray = BoundedCharArray.Empty();
            this.ReadIndexToken = 0;
            this.FeedIndexToken = 0;
        }

        public void Reset() {
            this.ReadIndexToken = 0;
            this.FeedIndexToken = 0;
        }

        public JsonToken CurrentToken
            => (ReadIndexToken < FeedIndexToken)
            ? Tokens[ReadIndexToken]
            : (FinalContent)
                ? TokenEOF
                : TokenReadAwait;


        public JsonToken GetToken(int offset)
            => ((ReadIndexToken + offset) < FeedIndexToken)
            ? Tokens[ReadIndexToken + offset]
            : (FinalContent)
                ? TokenEOF
                : TokenReadAwait;

        //public bool EnsureTokens(int count = 1) {
        //    return true;
        //}

        //public async ValueTask EnsureTokensAsync(int count = 1) {
        //    await Task.CompletedTask;
        //}

        public bool Advance(int count = 1) {
#warning TODO
            while ((count--) > 0) {
                if (this.ReadIndexToken < this.FeedIndexToken) {
                    var currentToken= Tokens[ReadIndexToken];
                    switch (currentToken.Kind) {                        
                        case JsonTokenKind.ObjectStart:
                            this.Stack.Push(JsonTokenKind.ObjectStart);
                            break;
                        case JsonTokenKind.ObjectEnd:
                            this.Stack.Pop();
                            break;
                        case JsonTokenKind.ArrayStart:
                            this.Stack.Push(JsonTokenKind.ArrayStart);
                            break;
                        case JsonTokenKind.ArrayEnd:
                            this.Stack.Pop();
                            break;
                        //case JsonTokenKind.ValueSep:
                        //    break;
                        //case JsonTokenKind.PairSep:
                        //    break;
                        case JsonTokenKind.StringSimpleUtf8:
                            this.ReturnToTokenCache(currentToken);
#warning handle protected   currentToken.OffsetUtf8
                            break;
                        case JsonTokenKind.StringComplex:
                            this.ReturnToTokenCache(currentToken);
#warning handle protected   currentToken.OffsetUtf8
                            break;
                        //case JsonTokenKind.True:
                        //    break;
                        //case JsonTokenKind.False:
                        //    break;
                        //case JsonTokenKind.Null:
                        //    break;
                        case JsonTokenKind.Number:
                            this.ReturnToTokenCache(currentToken);
                            break;
                        case JsonTokenKind.Value:
                            this.TokenCache[TokenCacheCount++] = currentToken;
                            break;
                        default:
                            break;
                    }
                } else {
                }
            }
            if (this.ReadIndexToken < this.FeedIndexToken) {
                this.ReadIndexToken = 0;
                this.FeedIndexToken = 0;
                return true;
            } else {
                return false;
            }
            //var next = this.ReadIndexToken + count;
            //if (next < FeedIndexToken) {
            //    this.ReadIndexToken = next;
            //return true;
            //} else if (next == FeedIndexToken) {
            //    this.ReadIndexToken = next;
            //return false;
            //} else {
            //    throw new ArgumentException($"{count} leads to {next} >= {FeedIndexToken}");
            //}
        }

        public JsonToken RentFromTokenCache() {
            if (this.TokenCacheCount > 0) {
                return this.TokenCache[--this.TokenCacheCount] ?? new JsonToken();
            }
            return new JsonToken();
        }

        public void ReturnToTokenCache(JsonToken jsonToken) {
            if (TokenCacheCount < this.TokenCache.Length) {
                this.TokenCache[this.TokenCacheCount++] = jsonToken;
            }
        }

        /*
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

        public object GetToken(int v) {
            throw new NotImplementedException();
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
        */
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

