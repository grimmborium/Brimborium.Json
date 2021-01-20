using System.Collections.Generic;
using System.Linq;

namespace Brimborium.Json {
    public class JsonSinkUtf8Array : JsonSinkUtf8 {
        private List<BoundedByteArray> boundedByteArrays;
        public JsonSinkUtf8Array(JsonConfiguration configuration)
            : base(configuration) {
            this.boundedByteArrays = new List<BoundedByteArray>();
        }
        protected override void WriteDown(int nextRequestedCount) {
            this.boundedByteArrays.Add(this.Buffer);
            this.Buffer = BoundedByteArray.Rent(this.Buffer.Buffer.Length);
            // base.WriteDown(nextRequestedCount);
            //
            //this.Buffer.Offset = 0;
            //this.Buffer.Length = this.Buffer.Buffer.Length;

        }
        public override void Flush() {
            if (boundedByteArrays.Count == 0) {
                // OK
            } else {
                this.boundedByteArrays.Add(this.Buffer);
                var length = boundedByteArrays.Sum(bba => bba.Offset);
                this.Buffer = BoundedByteArray.Rent(length);
                foreach(var bba in this.boundedByteArrays){
                    bba.GetUsedSpan().CopyTo(this.Buffer.GetFreeSpan());
                    this.Buffer.Offset += bba.Offset;
                }
            }
        }
    }
}
