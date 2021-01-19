using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.Json {
    public class JsonSource {
        public readonly JsonConfiguration Configuration;
        public JsonSource(JsonConfiguration configuration) {
            this.Configuration = configuration;
        }
        public virtual object Parse(Type? type) {
            throw new NotImplementedException();
        }
        public virtual ValueTask<object> ParseAsync(Type? type) {
            throw new NotImplementedException();
        }

    }
    public class JsonSourceUtf8 : JsonSource {
        public JsonSourceUtf8(JsonConfiguration configuration)
            : base(configuration) {

        }
     
        public Span<byte> Rent(int capa) {
            return new Span<byte>(new byte[capa]);
        }
        public void Written(int count) { }
    }
    public class JsonSourceAsyncUtf8 : JsonSource {
        public JsonSourceAsyncUtf8(JsonConfiguration configuration)
            : base(configuration) {
        }

        
        public Span<byte> Rent(int capa) {
            return new Span<byte>(new byte[capa]);
        }
        public void Written(int count) { }
    }
    public class JsonSourceUtf8Pipe : JsonSource {
        private readonly PipeReader bodyReader;

        public JsonSourceUtf8Pipe(PipeReader bodyReader, JsonConfiguration configuration)
            : base(configuration) {
            this.bodyReader = bodyReader;
        }
    }
    public class JsonSourceUtf16 : JsonSource {
        public JsonSourceUtf16(JsonConfiguration configuration)
            : base(configuration) {

        }
    }
    public class JsonSourceUtf8Stream : JsonSource {
        public JsonSourceUtf8Stream(JsonConfiguration configuration)
            : base(configuration) {

        }
    }
    public class JsonSourceUtf16Stream : JsonSource {
        public JsonSourceUtf16Stream(JsonConfiguration configuration)
            : base(configuration) {

        }
    }

    public class JsonSourceString : JsonSourceUtf16 {
        public readonly string Buffer;

        public JsonSourceString(string buffer, JsonConfiguration configuration)
            : base(configuration) {
            this.Buffer = buffer;
        }

    }
}
