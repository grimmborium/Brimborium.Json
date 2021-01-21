using System;
using System.IO;
using System.Threading.Tasks;

namespace Brimborium.Json {
    // public enum GetTokenResult { Ok, NeedMore, End, Fault }

    public class JsonSource : IDisposable {
        private int _IsDisposed;
        public readonly JsonConfiguration Configuration;

        public JsonToken JsonToken;

        public JsonSource(JsonConfiguration configuration) {
            this.Configuration = configuration;
        }

        public virtual bool TryGetNextToken() {
            throw new NotImplementedException();
        }

        public virtual ValueTask GetNextTokenAsync() {
            throw new NotImplementedException();
        }

        /*
        public virtual object Parse(Type? type) {
            throw new NotImplementedException();
        }
        public virtual ValueTask<object> ParseAsync(Type? type) {
            throw new NotImplementedException();
        }
        */

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

        public JsonSourceUtf8SyncStream(System.IO.Stream stream, JsonConfiguration configuration)
            : base(configuration) {
            this._Stream = stream;
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
