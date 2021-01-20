using System;
using System.Collections.Generic;

namespace Brimborium.Json {
    public class JsonSinkUtf8 : JsonSink {
        protected internal BoundedByteArray Buffer;

        protected JsonSinkUtf8(JsonConfiguration configuration)
            : base(configuration) {
            this.Buffer = BoundedByteArray.Rent(64 * 1024);
        }

        public override void Write(JsonText jsonText) {
            var src = jsonText.GetSpanUtf8();
            var dst = this.GetFreeSpan(src.Length, true);
            src.CopyTo(dst);
        }

        public virtual BoundedByteArray TransGetBuffer() {
            var buffer = this.Buffer;
            this.Buffer = new BoundedByteArray(Array.Empty<byte>(), 0, 0, false);
            return buffer;
        }

        public virtual ref BoundedByteArray GetBuffer(int count) {
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
                    var result = Buffer.GetFreeSpan();
                    Buffer.Offset += count;
                    return result;
                } else {
                    return Buffer.GetFreeSpan();
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
