#if false
using System;
using System.Buffers;
using System.IO;
using System.Text;

namespace Brimborium.Json {
    public class JsonSinkEncodingStream : JsonSinkUtf16 {
        private static UnicodeEncoding UnicodeEncodingNoBom = new UnicodeEncoding(false, false);
        private static UnicodeEncoding UnicodeEncodingBom = new UnicodeEncoding(false, true);
        private readonly Stream _Stream;
        private IMemoryOwner<byte> _MemoryOwner;
        private Memory<byte> _Memory;
        private int _Position;
        private UnicodeEncoding _UnicodeEncoding;
        // copy CreateTranscodingStream https://source.dot.net/#System.Private.CoreLib/TranscodingStream.cs,ed28ec6ee60823f2
        public JsonSinkEncodingStream(Stream stream) {
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

        ~JsonSinkEncodingStream() {
            this.Dispose(disposing: false);
        }
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
#endif