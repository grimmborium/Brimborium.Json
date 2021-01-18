using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.Json {
    public class JsonSource { }
    public class JsonSourceUtf8 : JsonSource {        
        public Span<byte> Rent(int capa) {
            return new Span<byte>(new byte[capa]);
        }
        public void Written(int count) { }
    }
    public class JsonSourceUtf16 : JsonSource {
    }
    public class JsonSourceUtf8Stream : JsonSource { }
    public class JsonSourceUtf16Stream : JsonSource { }

    public class JsonSourceString : JsonSourceUtf16 {
        public readonly string Buffer;

        public JsonSourceString(string buffer) {
            this.Buffer = buffer;
        }

    }
}
