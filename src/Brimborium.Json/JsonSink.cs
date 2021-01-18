using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.Json {
    public class JsonSink : IDisposable {
        protected int _IsDisposed;

        public virtual void Write(JsonText jsonText) { }
        public virtual void Flush() { }
        public virtual Task FlushAsync() { return Task.CompletedTask; }

        protected virtual void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref _IsDisposed, 1)) {
                if (disposing) {
                } else {
                }
            }
        }

        //~JsonSink() {
        //    Dispose(disposing: false);
        //}

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class JsonSinkUtf8 : JsonSink {
        protected BoundedByteArray _Buffer;

        public JsonSinkUtf8() {
            this._Buffer = BoundedByteArray.Rent(64 * 1024);
        }

        public override void Write(JsonText jsonText) {
            var src = jsonText.GetSpanUtf8();
            var dst = this.GetSpan(src.Length, true);
            src.CopyTo(dst);
        }

        public virtual Span<byte> GetSpan(int count, bool advance) {
            if (count > (_Buffer.Length - _Buffer.Offset)) {
                WriteDown(count);
                if (this._Buffer.Length < count) {
                    this._Buffer.Return();
                    this._Buffer = BoundedByteArray.Rent(count);
                }
            }
            if (count <= (_Buffer.Length - _Buffer.Offset)) {
                if (advance) {
                    var result = _Buffer.GetSpan();
                    _Buffer.Offset += count;
                    return result;
                } else {
                    return _Buffer.GetSpan();
                }
            } else {
                throw new InvalidOperationException();
            }
        }

        public virtual void Advance(int count) {
            this._Buffer.Offset += count;
        }

        protected virtual void WriteDown(int nextRequestedCount) {
            // after
            this._Buffer.Offset = 0;
            this._Buffer.Length = this._Buffer.Buffer.Length;
        }

        protected override void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref _IsDisposed, 1)) {
                if (disposing) {
                    this._Buffer.Return();
                } else {
                }
            }
        }
    }

    public class JsonSinkUtf16 : JsonSink {
        public override void Write(JsonText jsonText) {
            base.Write(jsonText);
        }

        public virtual Span<char> GetSpan(int count, bool advance) {
            return new Span<char>(new char[count]);
        }

        public virtual void Advance(int count) { }
    }

    public class JsonSinkUtf8Stream : JsonSinkUtf8 {
        private readonly Stream _Stream;

        public JsonSinkUtf8Stream(Stream stream) {
            this._Stream = stream;
        }

        protected override void WriteDown(int nextRequestedCount) {
            _Stream.Write(this._Buffer.Buffer, 0, this._Buffer.Offset);
            // base.WriteDown(nextRequestedCount);
            // after
            this._Buffer.Offset = 0;
            this._Buffer.Length = this._Buffer.Buffer.Length;
        }
        public override void Flush() {
            if (this._Buffer.Offset > 0) {
                WriteDown(0);
            }
            this._Stream.Flush();
        }

        public override Task FlushAsync() {
            return base.FlushAsync();
        }

    }

    public class JsonSinkUtf16Stream : JsonSinkUtf16 {
        private static UnicodeEncoding UnicodeEncodingNoBom = new UnicodeEncoding(false, false);
        private static UnicodeEncoding UnicodeEncodingBom = new UnicodeEncoding(false, true);
        private readonly Stream _Stream;
        private IMemoryOwner<byte> _MemoryOwner;
        private Memory<byte> _Memory;
        private int _Position;
        private UnicodeEncoding _UnicodeEncoding;
        // copy CreateTranscodingStream https://source.dot.net/#System.Private.CoreLib/TranscodingStream.cs,ed28ec6ee60823f2
        public JsonSinkUtf16Stream(Stream stream) {
            this._UnicodeEncoding = UnicodeEncodingBom;
            this._Stream = stream;
            this._MemoryOwner = MemoryPool<byte>.Shared.Rent(64 * 1024);
            this._Memory = this._MemoryOwner.Memory;
            this._Position = 0;
        }

        public void Write(Span<char> buffer) {
#if NETCOREAPP3_1 || NET5_0
            
            //if (buffer.Length > this._Memory.Length) {
                
            //    this._Stream.Write(this._MemoryOwner.Memory.Span.Slice(0, this._Position));
            //    this._Memory = this._MemoryOwner.Memory;
            //    this._Position = 0;
            //    if (buffer.Length > this._Memory.Length) {
            //        this._Stream.Write(this._UnicodeEncoding.GetBytes(buffer));
            //    }
            //}


            //StringUtility.ToUtf16(buffer).CopyTo(this._Memory.Span);
            //this._Memory = this._Memory.Slice(buffer.Length);
            //this._Position += buffer.Length;
            throw new NotSupportedException();
#else
            throw new NotSupportedException();
#endif
        }

        public override void Flush() {
#if NETCOREAPP3_1 || NET5_0
            if (this._Position > 0) {
                this._Stream.Write(this._MemoryOwner.Memory.Span.Slice(0, this._Position));
                this._Position = 0;
            }
#else
            throw new NotSupportedException();
#endif
        }

        ~JsonSinkUtf16Stream() {
            this.Dispose(disposing: false);
        }
    }

    public class JsonSinkString : JsonSink {
        public readonly StringBuilder Buffer;

        public JsonSinkString(StringBuilder? buffer) {
            this.Buffer = buffer ?? new StringBuilder();
        }

        public override string ToString() {
            return this.Buffer.ToString();
        }
    }
    public class ChunckedBufferUtf8StreamWriter : IDisposable {
        private readonly Stream _Stream;
        private IMemoryOwner<byte> _MemoryOwner;
        private Memory<byte> _Memory;
        private int _IsDisposed;
        private int _Position;

        // copy CreateTranscodingStream https://source.dot.net/#System.Private.CoreLib/TranscodingStream.cs,ed28ec6ee60823f2
        public ChunckedBufferUtf8StreamWriter(Stream stream) {
            this._Stream = stream;
            this._MemoryOwner = MemoryPool<byte>.Shared.Rent(64 * 1024);
            this._Memory = this._MemoryOwner.Memory;
            this._Position = 0;
        }

        public void Write(ReadOnlySpan<byte> buffer) {
#if NETCOREAPP3_1 || NET5_0
            if (buffer.Length > this._Memory.Length) {
                this._Stream.Write(this._MemoryOwner.Memory.Span.Slice(0, this._Position));
                this._Memory = this._MemoryOwner.Memory;
                this._Position = 0;
                if (buffer.Length > this._Memory.Length) {
                    this._Stream.Write(buffer);
                }
            }
            buffer.CopyTo(this._Memory.Span);
            this._Memory = this._Memory.Slice(buffer.Length);
            this._Position += buffer.Length;
#else
            throw new NotSupportedException();
#endif
        }

        public void Flush() {
#if NETCOREAPP3_1 || NET5_0
            if (this._Position > 0) {
                this._Stream.Write(this._MemoryOwner.Memory.Span.Slice(0, this._Position));
                this._Position = 0;
            }
#else
            throw new NotSupportedException();
#endif
        }
        protected virtual void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref _IsDisposed, 1)) {
                if (disposing) {
                    using (var m = this._MemoryOwner) {
                        this._MemoryOwner = null!;
                    }
                } else {
                    try {
                        using (var m = this._MemoryOwner) {
                            this._MemoryOwner = null!;
                        }
                    } catch { }
                }

            }
        }

        ~ChunckedBufferUtf8StreamWriter() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
