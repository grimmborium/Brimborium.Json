using System;

namespace Brimborium.Json {
    public class JsonSinkUtf16 : JsonSink {
        public const int DefaultInitialLength = 64 * 1024;
        protected internal BoundedCharArray Buffer;

        public JsonSinkUtf16(JsonConfiguration configuration)
            :base(configuration.GetForUtf16()) {
            this.Buffer = BoundedCharArray.Rent(DefaultInitialLength);
        }

        public override void Write(JsonText jsonText) {
            var src = jsonText.GetSpanUtf16();
            var dst = this.GetFeedSpan(src.Length, true);
            src.CopyTo(dst);
        }

        public virtual BoundedCharArray DisposeAndGetBuffer() {
            var buffer = this.Buffer;
            this.Buffer = BoundedCharArray.Empty();
            this.Dispose();
            return buffer;
        }

        public virtual ref BoundedCharArray GetFeedBuffer(int count) {
            if (count > Buffer.FeedLength) {
                WriteDown(count);
                if (this.Buffer.FeedLength < count) {
                    this.Buffer.AdjustBeforeFeeding(count, DefaultInitialLength);
                }
            }
            return ref Buffer;
        }

        public virtual Span<char> GetFeedSpan(int count, bool advance) {
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
