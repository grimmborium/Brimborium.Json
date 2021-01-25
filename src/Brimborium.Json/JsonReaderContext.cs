#pragma warning disable IDE0041 // Use 'is null' check
using System.Collections.Generic;
using System.Threading;

namespace Brimborium.Json {
    public class JsonReaderContext {

        public JsonToken[] Tokens;
        public JsonToken[] TokenCache;
        public int TokenCacheCount;
        public BoundedByteArray BoundedByteArray;
        public BoundedCharArray BoundedCharArray;

        public JsonReaderContext() {
            this.TokenCache = new JsonToken[128];
            this.Tokens = new JsonToken[128];
            for (int idx = 0; idx < this.Tokens.Length; idx++) {
                this.Tokens[idx] = JsonToken.TokenFault;
            }

            this.BoundedByteArray = BoundedByteArray.Empty();
            this.BoundedCharArray = BoundedCharArray.Empty();
        }

        public JsonReaderContext Reset(int tokenCacheCount, BoundedByteArray boundedByteArray, BoundedCharArray boundedCharArray) {
            for (int idx = 0; idx < this.Tokens.Length; idx++) {
                this.Tokens[idx] = JsonToken.TokenFault;
            }
            this.TokenCacheCount = tokenCacheCount;
            this.BoundedByteArray = boundedByteArray;
            this.BoundedCharArray = boundedCharArray;
            return this;
        }
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

                int slot = Thread.CurrentThread.ManagedThreadId;
                for (int idx = 0; idx < this._Contexts.Length; idx++) {
                    if (this._Contexts[(idx + slot) % 8] is null) {
                        if (ReferenceEquals(
                            System.Threading.Interlocked.CompareExchange(ref this._Contexts[(idx + slot) % 8], null, context),
                            null)) {
                        }
                    }
                }
            }
        }
    }
}

