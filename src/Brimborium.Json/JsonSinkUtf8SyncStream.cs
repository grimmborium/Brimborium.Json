using System.IO;

namespace Brimborium.Json {
    public class JsonSinkUtf8SyncStream : JsonSinkUtf8 {
        private readonly Stream _Stream;

        public JsonSinkUtf8SyncStream(Stream stream, JsonConfiguration configuration)
            : base(configuration) {
            this._Stream = stream;
        }

        protected override void WriteDown(int nextRequestedCount) {
            if (this.Buffer.ReadLength > 0) {
                _Stream.Write(this.Buffer.Buffer, this.Buffer.ReadOffset, this.Buffer.ReadLength);

                this.Buffer.ReadOffset = 0;
                this.Buffer.FeedOffset = 0;
            }
        }

        public override void Flush() {
            WriteDown(0);
            this._Stream.Flush();
        }
    }
}
