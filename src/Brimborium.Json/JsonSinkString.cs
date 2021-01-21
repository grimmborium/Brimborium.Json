using System.Text;

namespace Brimborium.Json {
    public sealed class JsonSinkString : JsonSinkUtf16 {
        public readonly StringBuilder Target;

        public JsonSinkString(StringBuilder? buffer, JsonConfiguration configuration)
            : base(configuration) {
            this.Target = buffer ?? new StringBuilder();
        }

        protected override void WriteDown(int nextRequestedCount) {
            if (Buffer.Offset > 0) {
                Target.Append(Buffer.Buffer, 0, Buffer.Offset);

                this.Buffer.Offset = 0;
                this.Buffer.Length = this.Buffer.Buffer.Length;
            }
        }

        public override string ToString() {
            return this.Target.ToString();
        }
    }
}
