using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Json {
    // public enum GetTokenResult { Ok, NeedMore, End, Fault }

    public class JsonSource : IDisposable {
        private int _IsDisposed;
        public JsonConfiguration Configuration;
        public JsonReaderContext Context;

        public JsonSource(JsonConfiguration configuration) {
            this.Configuration = configuration;
            this.Context = JsonReaderContextPool.Instance.Rent();
        }

        public virtual bool EnsureTokens(int count = 1) {
            if ((this.Context.ReadIndexToken + count) < this.Context.FeedIndexToken) {
                return true;
            }
#warning TODO add missing logic
            return false;
        }

        public virtual async ValueTask EnsureTokensAsync(int count = 1) {
            while (true) {
                if ((this.Context.ReadIndexToken + count) < this.Context.FeedIndexToken) {
                    return;
                }
                await this.ReadFromSourceAsync();
#warning TODO add missing logic
            }
        }

        public JsonToken CurrentToken
            => this.Context.CurrentToken;
        public JsonToken Token1
            => this.Context.GetToken(1);
        public JsonToken Token2
            => this.Context.GetToken(2);
        public JsonToken Token3
            => this.Context.GetToken(3);


        public virtual bool Advance(int count = 1) {
            return this.Context.Advance(count);
        }

        public virtual async ValueTask ReadFromSourceAsync() {
            await Task.CompletedTask;
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
            JsonReaderContextPool.Instance.Return(this.Context);
            this.Configuration = null!;
            this.Context = null!;
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
            this.Parser = new JsonParserUtf8(this.Context);
        }

        protected override void Disposing(bool disposing) {
            base.Disposing(disposing);
            JsonReaderContextPool.Instance.Return(this.Context);
        }
    }

    public class JsonSourceUtf16 : JsonSource {
        public JsonParserUtf16 Parser;
        public JsonSourceUtf16(JsonConfiguration configuration)
            : base(configuration.GetForUtf16()) {
            this.Parser = new JsonParserUtf16(this.Context);
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
            this.Context.BoundedByteArray.AdjustBeforeFeeding(4096, DefaultInitialLength);
            var read = await this._Stream.ReadAsync(
                this.Context.BoundedByteArray.Buffer,
                this.Context.BoundedByteArray.FeedOffset,
                this.Context.BoundedByteArray.FeedLength);
            this.Context.BoundedByteArray.AdjustAfterFeeding(read);
            this.Context.FinalContent = (read == 0);
            this.Parser.Parse(this.Context.BoundedByteArray, read == 0);
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
