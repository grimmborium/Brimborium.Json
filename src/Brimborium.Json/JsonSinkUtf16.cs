using System;

namespace Brimborium.Json {
    public class JsonSinkUtf16 : JsonSink {
        protected internal BoundedCharArray Buffer;

        public JsonSinkUtf16(JsonConfiguration configuration)
            :base(configuration) {
            this.Buffer = BoundedCharArray.Rent(64 * 1024);
        }

        public override void Write(JsonText jsonText) {
            var src = jsonText.GetSpanUtf16();
            var dst = this.GetSpan(src.Length, true);
            src.CopyTo(dst);
        }

        public virtual ref BoundedCharArray GetBuffer(int count) {
            if (count > Buffer.Free) {
                WriteDown(count);
                if (count > Buffer.Free) {
                    this.Buffer.Return();
                    this.Buffer = BoundedCharArray.Rent(count);
                }
            }
            return ref Buffer;
        }

        public virtual Span<char> GetSpan(int count, bool advance) {
            if (count > Buffer.Free) {
                WriteDown(count);
                if (this.Buffer.Length < count) {
                    this.Buffer.Return();
                    this.Buffer = BoundedCharArray.Rent(count);
                }
            }
            if (count <= Buffer.Free) {
                if (advance) {
                    var result = Buffer.GetSpan();
                    Buffer.Offset += count;
                    return result;
                } else {
                    return Buffer.GetSpan();
                }
            } else {
                throw new InvalidOperationException();
            }
        }

        public virtual void Advance(int count) {
            this.Buffer.Offset += count;
        }

        protected virtual void WriteDown(int nextRequestedCount) {
            // after
            this.Buffer.Offset = 0;
            this.Buffer.Length = this.Buffer.Buffer.Length;
        }

        protected override void Disposing(bool disposing) {
            this.Buffer.Return();
        }
    }
}
