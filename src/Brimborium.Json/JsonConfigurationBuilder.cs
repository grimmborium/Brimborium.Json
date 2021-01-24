using System.Collections.Generic;

namespace Brimborium.Json {
    public class JsonConfigurationBuilder {
        public readonly List<JsonSerializerFactory> JsonSerializerFactory;
        
        public JsonConfigurationBuilder() {
            this.JsonSerializerFactory = new List<JsonSerializerFactory>();
        }

        public JsonConfiguration Build() {
            var result = new JsonConfiguration();
#warning TODO add defaults
            result.JsonSerializerFactory.AddRange(this.JsonSerializerFactory);
            return result;
        }
    }
}