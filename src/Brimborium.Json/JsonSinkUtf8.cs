using System;

namespace Brimborium.Json {
    public class JsonSinkUtf8 : JsonSink {
        public const int DefaultInitialLength = 64 * 1024;
        protected internal BoundedByteArray Buffer;

        protected JsonSinkUtf8(JsonConfiguration configuration)
            : base(configuration.GetForUtf8()) {
            this.Buffer = BoundedByteArray.Rent(64 * 1024);
        }

        public override void Write(JsonText jsonText) {
            var src = jsonText.GetSpanUtf8();
            var dst = this.GetFeedSpan(src.Length, true);
            src.CopyTo(dst);
        }

        public virtual BoundedByteArray DisposeAndGetBuffer() {
            var buffer = this.Buffer;
            this.Buffer = BoundedByteArray.Empty();
            this.Dispose();
            return buffer;
        }

        public virtual ref BoundedByteArray GetFeedBuffer(int count) {
            if (count > Buffer.FeedLength) {
                WriteDown(count);
                if (this.Buffer.FeedLength < count) {
                    this.Buffer.AdjustBeforeFeeding(count, DefaultInitialLength);
                }
            }
            return ref Buffer;
        }

        public virtual Span<byte> GetFeedSpan(int count, bool advance) {
            if (count > Buffer.FeedLength) {
                WriteDown(count);
                if (this.Buffer.FeedLength < count) {
                    this.Buffer.AdjustBeforeFeeding(count, DefaultInitialLength);
                }
            }
            if (count <= Buffer.FeedLength) {
                var result = Buffer.GetFeedSpan();
                if (advance) {
                    Buffer.AdjustAfterFeeding(count);
                }
                return result;
            } else {
                throw new InvalidOperationException();
            }
        }

        public virtual void Advance(int count) {
            this.Buffer.FeedOffset += count;
        }

        protected override void Disposing(bool disposing) {
            base.Disposing(disposing);
            this.Buffer.Return();
        }
    }
}
