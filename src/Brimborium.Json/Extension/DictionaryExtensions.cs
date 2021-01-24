using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic {
    public static class DictionaryExtensions {
#if NET5_0 || NETCOREAPP3_1
        // no
#else
        public static bool TryAdd<TKey, TValue>(
            this Dictionary<TKey, TValue> that,
            TKey key, 
            TValue value) {
            if (that.ContainsKey(key)) {
                return false;
            } else {
                that.Add(key, value);
                return true;
            }
        }
#endif
    }
}
