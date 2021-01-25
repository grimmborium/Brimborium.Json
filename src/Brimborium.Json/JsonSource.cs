using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Json {
    // public enum GetTokenResult { Ok, NeedMore, End, Fault }

    public class JsonSource : IDisposable {
        private int _IsDisposed;
        public JsonConfiguration Configuration;
        private JsonReaderContext _Context;

        public JsonToken[] Tokens;
        public JsonToken[] TokenCache;
        public int TokenCacheCount;
        public BoundedByteArray BoundedByteArray;
        public BoundedCharArray BoundedCharArray;
        public bool FinalContent;
        public readonly Stack<JsonTokenKind> Stack;
        public int ReadIndexToken;
        public int FeedIndexToken;
        public int CountToken => FeedIndexToken - ReadIndexToken;

        public JsonSource(JsonConfiguration configuration) {
            this.Configuration = configuration;
            this._Context = JsonReaderContextPool.Instance.Rent();
            this.BoundedByteArray = this._Context.BoundedByteArray;
            this.BoundedCharArray = this._Context.BoundedCharArray;
            this.FinalContent = false;
            this.Stack = new Stack<JsonTokenKind>();
            this.ReadIndexToken = 0;
            this.FeedIndexToken = 0;
        }

        public JsonToken CurrentToken
            => (ReadIndexToken < FeedIndexToken)
            ? Tokens[ReadIndexToken]
            : (FinalContent)
                ? JsonToken.TokenEOF
                : JsonToken.TokenReadAwait;

        public JsonToken GetToken(int offset)
            => ((ReadIndexToken + offset) < FeedIndexToken)
            ? Tokens[ReadIndexToken + offset]
            : (FinalContent)
                ? JsonToken.TokenEOF
                : JsonToken.TokenReadAwait;

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
                    var currentToken = Tokens[ReadIndexToken];
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns>need more content</returns>
        public virtual bool Parse(int count) {
            return false;
        }

        /// <summary>
        /// Try to parse without reading from the source (async).
        /// </summary>
        /// <param name="count">the number of token</param>
        /// <returns>true if EnsureTokensAsync is needed</returns>
        public virtual bool EnsureTokens(int count = 1) {
            if ((this.ReadIndexToken + count) < this.FeedIndexToken) {
                return false;
            }
            Parse(count + ReadIndexToken - this.FeedIndexToken );
            if ((this.ReadIndexToken + count) < this.FeedIndexToken) {
                return false;
            }
            return true;
        }

        public virtual async ValueTask EnsureTokensAsync(int count = 1) {
            if (ReadIndexToken == FeedIndexToken) {
                ReadIndexToken = 0;
                FeedIndexToken = 0;
            } else if ((this.ReadIndexToken + count) < this.FeedIndexToken) {
                return;
            }
            var orgReadIndexToken = ReadIndexToken;
            while (true) {
                await this.ReadFromSourceAsync();
                var needMoreContent = this.Parse(count + orgReadIndexToken - FeedIndexToken);
                if (needMoreContent) {
                    continue;
                } else if ((this.ReadIndexToken + count) < this.FeedIndexToken) {
                    return;
                }            
            }
        }

        public JsonToken Token1
            => this.GetToken(1);
        public JsonToken Token2
            => this.GetToken(2);
        public JsonToken Token3
            => this.GetToken(3);


        public virtual ValueTask ReadFromSourceAsync() {
            throw new NotImplementedException("ReadFromSourceAsync");
        }

        protected bool IsDisposed => this._IsDisposed != 0;

        protected void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref _IsDisposed, 1)) {
                this.Disposing(disposing);
            } else {
                if (disposing) {
                    try {
                        this.Disposing(disposing);
                    } catch { }
                }
            }
        }

        protected virtual void Disposing(bool disposing) {
            JsonReaderContextPool.Instance.Return(this._Context, this.TokenCacheCount);
            this.Configuration = null!;
            this._Context = null!;
        }

        //~JsonSink() {
        //    Dispose(disposing: false);
        //}

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class JsonSourceUtf8 : JsonSource {
        public JsonParserUtf8 Parser;
        public JsonSourceUtf8(JsonConfiguration configuration)
            : base(configuration.GetForUtf8()) {
            this.Parser = new JsonParserUtf8(this, 0);
        }

        protected override void Disposing(bool disposing) {
            base.Disposing(disposing);
        }
    }

    public class JsonSourceUtf16 : JsonSource {
        public JsonParserUtf16 Parser;
        public JsonSourceUtf16(JsonConfiguration configuration)
            : base(configuration.GetForUtf16()) {
            this.Parser = new JsonParserUtf16(this, 0);
        }
    }

    public class JsonSourceAsyncUtf8 : JsonSource {
        public JsonSourceAsyncUtf8(JsonConfiguration configuration)
            : base(configuration.GetForUtf8()) {
        }
    }

    public class JsonSourceUtf8AsyncStream : JsonSourceUtf8 {
        private readonly Stream _Stream;

        public JsonSourceUtf8AsyncStream(System.IO.Stream stream, JsonConfiguration configuration)
            : base(configuration) {
            this._Stream = stream;
        }
    }

    public class JsonSourceUtf8SyncStream : JsonSourceUtf8 {
        private readonly Stream _Stream;
        public const int DefaultInitialLength = 64 * 1024;

        public JsonSourceUtf8SyncStream(System.IO.Stream stream, JsonConfiguration configuration)
            : base(configuration) {
            this._Stream = stream;
        }

        public override async ValueTask ReadFromSourceAsync() {
            if (this.FinalContent) {
                return;
            }
            this.BoundedByteArray.AdjustBeforeFeeding(4096, DefaultInitialLength);
            var read = await this._Stream.ReadAsync(
                this.BoundedByteArray.Buffer,
                this.BoundedByteArray.FeedOffset,
                this.BoundedByteArray.FeedLength);
            this.BoundedByteArray.AdjustAfterFeeding(read);
            this.FinalContent = (read == 0);            
        }

        public override bool Parse(int count) {
            if (count <= 0) {
                return true;
            }
            if (this.Parser.Faulted) {
                return false;
            }
            this.Parser.Parse(count);
            return this.Parser.NeedMoreContent>0;
        }
    }

    public class JsonSourceUtf16Stream : JsonSourceUtf16 {
        private readonly Stream _Stream;

        public JsonSourceUtf16Stream(System.IO.Stream stream, JsonConfiguration configuration)
            : base(configuration) {
            this._Stream = stream;
        }
    }

    public class JsonSourceString : JsonSourceUtf16 {
        public readonly string Buffer;

        public JsonSourceString(string buffer, JsonConfiguration configuration)
            : base(configuration) {
            this.Buffer = buffer;
        }

    }
}
