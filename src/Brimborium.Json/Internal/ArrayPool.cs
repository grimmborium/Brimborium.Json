#nullable disable

using System;
using System.Collections.Generic;

namespace Brimborium.Json.Internal {
    internal sealed class BufferPool : ArrayPool<byte> {
        public static readonly BufferPool Default = new BufferPool(65535);

        public BufferPool(int bufferLength)
            : base(bufferLength) {
        }
    }

    internal class ArrayPool<T> {
        readonly int _BufferLength;
        readonly List<T[]> _Buffers;

        public ArrayPool(int bufferLength) {
            this._BufferLength = bufferLength;
            this._Buffers = new List<T[]>();
        }

        public T[] Rent() {
            lock (this._Buffers) {
                if (_Buffers.Count == 0) {
                    return new T[_BufferLength];
                } else {
                    var idx = this._Buffers.Count - 1;
                    var result = this._Buffers[idx];
                    this._Buffers.RemoveAt(idx);
                    return result;
                }
            }
        }

        public void Return(T[] array) {
            if (array.Length == _BufferLength) {

                lock (this._Buffers) {
                    this._Buffers.Add(array);
                }
            }
        }

        public RentBuffer<T> UsingRent() {
            return new RentBuffer<T>(this.Rent(), this.Return);
        }
    }

    internal class RentBuffer<T> : IDisposable {
        private T[] _Instance;
        private System.Action<T[]> _GiveBack;

        internal RentBuffer(T[] rent, System.Action<T[]> giveBack) {
            this._Instance = rent;
            this._GiveBack = giveBack;
        }

        public void Dispose() {
            var instance = System.Threading.Interlocked.Exchange(ref this._Instance, null);
            if (instance != null) { 
                this._GiveBack(instance);
            }
        }
    }
}
