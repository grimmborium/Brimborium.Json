using System;

namespace Brimborium.Json.Internal {
    public sealed class AppendArray<T>
        where T : struct {
        public T[] Items;

        public int Count;

        public AppendArray(int size = 0) {
            this.Items = new T[System.Math.Max(size, 16)];
            this.Count = 0;
        }

        /// <summary>
        /// Caller has to lock this.
        /// </summary>
        /// <param name="value"></param>
        public int Add(ref T value) {
            var count = System.Threading.Interlocked.Increment(ref Count);
            var indexWriteTo = count - 1;
            if (count >= this.Items.Length) {
                EnsureCapacity(0, 16);
            }
            this.Items[indexWriteTo] = value;
            return indexWriteTo;
        }

        /// <summary>
        /// Caller has to lock this.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="increse"></param>
        public void EnsureCapacity(int size, int increse) {
            var oldSize = this.Items.Length;
            var newSize = System.Math.Max(size, oldSize + System.Math.Max(increse, 16));
            var nextItems = new T[newSize];
            this.Items.AsSpan().CopyTo(nextItems.AsSpan());
            System.Threading.Interlocked.Exchange(ref this.Items, nextItems);
        }
    }
}
