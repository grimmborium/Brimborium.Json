using System;

namespace Brimborium.Json {
    public class JsonConfiguration {
        public void Serialize<T>(T value, JsonSink jsonSink) {
            throw new NotImplementedException();
        }
        public T Deserialize<T>(JsonSource jsonSource) {
            throw new NotImplementedException();
        }
    }
}