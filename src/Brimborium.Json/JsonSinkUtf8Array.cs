using Brimborium.Json.Internal;

namespace Brimborium.Json {
    public class JsonSinkUtf8Array : JsonSinkUtf8 {
        private AppendArray<BoundedByteArray> boundedByteArrays;

        public JsonSinkUtf8Array(JsonConfiguration configuration)
            : base(configuration) {
            this.boundedByteArrays = new AppendArray<BoundedByteArray>();
        }

        protected override void WriteDown(int nextRequestedCount) {
            if (this.Buffer.ReadLength > 0) {
                this.boundedByteArrays.Add(ref this.Buffer);
                this.Buffer = (nextRequestedCount < 0)
                    ? BoundedByteArray.Empty()
                    : BoundedByteArray.Rent(this.Buffer.Buffer.Length);
            }
        }

        public override void Flush() {
            this.WriteDown(-1);
            if (boundedByteArrays.Count == 0) {
                // OK
            } else {
                int length = 0;
                for (int idx = 0; idx < boundedByteArrays.Count; idx++) {
                    length += boundedByteArrays.Items[idx].ReadLength;
                }
                this.Buffer = BoundedByteArray.Rent(length);
                for (int idx = 0; idx < boundedByteArrays.Count; idx++) {
                    var srcSpan = boundedByteArrays.Items[idx].GetReadSpan();
                    srcSpan.CopyTo(this.Buffer.GetFeedSpan());
                    this.Buffer.FeedOffset += srcSpan.Length;
                    boundedByteArrays.Items[idx].Return();
                }
            }
        }
    }
}
