using System;

namespace Brimborium.Json {
    public class JsonSinkUtf8 : JsonSink {
        protected internal BoundedByteArray Buffer;

        protected JsonSinkUtf8(JsonConfiguration configuration)
            : base(configuration.GetForUtf8()) {
            this.Buffer = BoundedByteArray.Rent(64 * 1024);
        }

        public override void Write(JsonText jsonText) {
            var src = jsonText.GetSpanUtf8();
            var dst = this.GetFreeSpan(src.Length, true);
            src.CopyTo(dst);
        }

        public virtual BoundedByteArray DisposeAndGetBuffer() {
            var buffer = this.Buffer;
            this.Buffer = BoundedByteArray.Empty();
            this.Dispose();
            return buffer;
        }

        public virtual ref BoundedByteArray GetFreeBuffer(int count) {
            if (count > Buffer.Free) {
                WriteDown(count);
                if (count > Buffer.Free) {
                    this.Buffer.Return();
                    this.Buffer = BoundedByteArray.Rent(count);
                }
            }
            return ref Buffer;
        }

        public virtual Span<byte> GetFreeSpan(int count, bool advance) {
            if (count > Buffer.Free) {
                WriteDown(count);
                if (this.Buffer.Length < count) {
                    this.Buffer.Return();
                    this.Buffer = BoundedByteArray.Rent(count);
                }
            }
            if (count <= Buffer.Free) {
                if (advance) {
                    var result = Buffer.GetRightSpan();
                    Buffer.Offset += count;
                    return result;
                } else {
                    return Buffer.GetRightSpan();
                }
            } else {
                throw new InvalidOperationException();
            }
        }

        public virtual void Advance(int count) {
            this.Buffer.Offset += count;
        }

        //protected virtual void WriteDown(int nextRequestedCount) {
        //    // after
        //    this.Buffer.Offset = 0;
        //    this.Buffer.Length = this.Buffer.Buffer.Length;
        //}

        protected override void Disposing(bool disposing) {
            this.Buffer.Return();
        }
    }
}
