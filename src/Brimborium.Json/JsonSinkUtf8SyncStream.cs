using System.IO;
using System.Threading.Tasks;

namespace Brimborium.Json {
    public class JsonSinkUtf8SyncStream : JsonSinkUtf8 {
        private readonly Stream _Stream;

        public JsonSinkUtf8SyncStream(Stream stream, JsonConfiguration configuration)
            : base(configuration) {
            this._Stream = stream;
        }

        protected override void WriteDown(int nextRequestedCount) {
            _Stream.Write(this.Buffer.Buffer, 0, this.Buffer.Offset);
            // base.WriteDown(nextRequestedCount);
            // after
            this.Buffer.Offset = 0;
            this.Buffer.Length = this.Buffer.Buffer.Length;
        }

        public override void Flush() {
            if (this.Buffer.Offset > 0) {
                WriteDown(0);
            }
            this._Stream.Flush();
        }

        public override Task FlushAsync() {
            if (this.Buffer.Offset > 0) {
                WriteDown(0);
            }
            this._Stream.Flush();
            return Task.CompletedTask;
        }

    }
}
