using System;
using System.IO;
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

        public bool HasToken() => this.Context.HasToken();

        public JsonToken GetCurrentToken() => this.Context.GetCurrentToken();

        public JsonToken ReadCurrentToken() => this.Context.ReadCurrentToken();

        public bool TryPeekToken(out JsonToken jsonToken) => this.Context.TryPeekToken(out jsonToken);

        public bool TryReadToken(out JsonToken jsonToken) => this.Context.TryReadToken(out jsonToken);

        public ValueTask<JsonToken> ReadCurrentTokenAsync() {
            throw new NotImplementedException();
        }


        public virtual Task ReadParseAsync() {
            throw new NotImplementedException();
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
        public JsonSourceUtf8(JsonConfiguration configuration)
            : base(configuration.GetForUtf8()) {
        }


        //    public Span<byte> Rent(int capa) {
        //        return new Span<byte>(new byte[capa]);
        //    }
        //    public void Written(int count) { }
    }

    public class JsonSourceUtf16 : JsonSource {
        public JsonSourceUtf16(JsonConfiguration configuration)
            : base(configuration.GetForUtf16()) {
        }
    }

    public class JsonSourceAsyncUtf8 : JsonSource {
        public JsonSourceAsyncUtf8(JsonConfiguration configuration)
            : base(configuration.GetForUtf8()) {
        }

        public Span<byte> Rent(int capa) {
            return new Span<byte>(new byte[capa]);
        }
        public void Written(int count) { }
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

        public override async Task ReadParseAsync() {
            this.Context.BoundedByteArray.AdjustBeforeFeeding(4096, DefaultInitialLength);
            var read = await this._Stream.ReadAsync(
                this.Context.BoundedByteArray.Buffer,
                this.Context.BoundedByteArray.FeedOffset,
                this.Context.BoundedByteArray.FeedLength);
            this.Context.BoundedByteArray.AdjustAfterFeeding(read);
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
