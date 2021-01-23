using System.Text;

namespace Brimborium.Json {
    public sealed class JsonSinkString : JsonSinkUtf16 {
        public readonly StringBuilder Target;

        public JsonSinkString(StringBuilder? buffer, JsonConfiguration configuration)
            : base(configuration) {
            this.Target = buffer ?? new StringBuilder();
        }

        protected override void WriteDown(int nextRequestedCount) {
                var readLength = Buffer.ReadLength;
            if (readLength > 0) {
                this.Target.Append(Buffer.Buffer, Buffer.ReadOffset, readLength);
                Buffer.AdjustAfterReading(readLength);
            }
        }

        public override string ToString() {
            return this.Target.ToString();
        }
    }
}
