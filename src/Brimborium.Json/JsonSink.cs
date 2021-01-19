using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.Json {
    public class JsonSink : IDisposable {
        private int _IsDisposed;
        
        public virtual void Write(JsonText jsonText) { }
        public virtual void Flush() { }
        public virtual Task FlushAsync() { return Task.CompletedTask; }

        protected bool IsDisposed => this._IsDisposed != 0;

        public readonly JsonConfiguration Configuration;

        public JsonSink(JsonConfiguration configuration) {
            this.Configuration = configuration;
        }

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

}
