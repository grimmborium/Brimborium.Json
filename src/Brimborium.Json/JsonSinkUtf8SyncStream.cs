using System.IO;

namespace Brimborium.Json {
    public class JsonSinkUtf8SyncStream : JsonSinkUtf8 {
        private readonly Stream _Stream;

        public JsonSinkUtf8SyncStream(Stream stream, JsonConfiguration configuration)
            : base(configuration) {
            this._Stream = stream;
        }

        protected override void WriteDown(int nextRequestedCount) {
            if (this.Buffer.Offset > 0) {
                _Stream.Write(this.Buffer.Buffer, 0, this.Buffer.Offset);

                this.Buffer.Offset = 0;
                this.Buffer.Length = this.Buffer.Buffer.Length;
            }
        }

        public override void Flush() {
            WriteDown(0);
            this._Stream.Flush();
        }
    }
}
