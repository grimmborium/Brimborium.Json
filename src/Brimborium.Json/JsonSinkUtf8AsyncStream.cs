using Brimborium.Json.Internal;

using System;
using System.IO;
using System.Threading.Tasks;

namespace Brimborium.Json {
    public class JsonSinkUtf8AsyncStream : JsonSinkUtf8 {
        private readonly Stream _Stream;
        private AppendArray<BoundedByteArray> _BufferWriteAsync;
        private Task _Task;

        public JsonSinkUtf8AsyncStream(Stream stream, JsonConfiguration configuration)
            : base(configuration) {
            this._Stream = stream;
            this._BufferWriteAsync = new AppendArray<BoundedByteArray>(16);
            this._Task = Task.CompletedTask;
        }

        protected override void WriteDown(int nextRequestedCount) {
            if (this.Buffer.ReadLength > 0) {
                int indexWriteTo;
                lock (this._BufferWriteAsync) {
                    indexWriteTo = this._BufferWriteAsync.Add(ref this.Buffer);
                    this.Buffer = ((nextRequestedCount < 0) ? BoundedByteArray.Empty() : BoundedByteArray.Rent(16 * 1024));
                    this._Task = this.WriteDownChain(this._Task, indexWriteTo);
                }
            }
        }

        private Task WriteDownChain(Task task, int indexWriteTo) {
            var tsc = new TaskCompletionSource<int>();
            task.ContinueWith((t) => { WriteDownAsync(indexWriteTo, tsc); });
            return tsc.Task;
        }

        private async void WriteDownAsync(int indexWriteTo, TaskCompletionSource<int> tsc) {
            try {
                await _Stream.WriteAsync(
                    _BufferWriteAsync.Items[indexWriteTo].Buffer,
                    _BufferWriteAsync.Items[indexWriteTo].ReadOffset,
                    _BufferWriteAsync.Items[indexWriteTo].ReadLength);

                this._BufferWriteAsync.Items[indexWriteTo].Return();
                tsc.TrySetResult(indexWriteTo);
            } catch (Exception error) {
                tsc.TrySetException(error);
            }
        }

        public override void Flush() {
            this.FlushAsync().GetAwaiter().GetResult();
        }

        public override async Task FlushAsync() {
            if (this.Buffer.FeedOffset > 0) {
                this.WriteDown(-1);
            }
            await this._Task;
        }
    }
}
