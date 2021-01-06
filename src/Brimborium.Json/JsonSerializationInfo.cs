using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Brimborium.Json.Internal;

namespace Brimborium.Json {
    public class JsonSerializationInfoBuilder {
        private readonly List<JsonPropertySerializationData> _Properties;

        public JsonSerializationInfoBuilder() {
            this._Properties = new List<JsonPropertySerializationData>();
        }
        public List<JsonPropertySerializationData> Properties => this._Properties;

        public JsonSerializationInfoBuilder Add(JsonPropertySerializationData info) {
            this._Properties.Add(info);
            return this;
        }

        public JsonSerializationInfoBuilder Add(string name, int order, bool isReadable, bool isWritable) {
            this._Properties.Add(new JsonPropertySerializationData(
                name,
                order,
                isReadable,
                isWritable
            ));
            return this;
        }

        public JsonSerializationInfo Build() {
            return new JsonSerializationInfo(this);
        }
    }

    public class JsonPropertySerializationData {
        public JsonPropertySerializationData(
                string name,
                int order,
                bool isReadable,
                bool isWritable
            ) {
            this.Name = name;
            this.Order = order;
            this.IsReadable = isReadable;
            this.IsWritable = isWritable;
        }

        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsReadable { get; set; }
        public bool IsWritable { get; set; }
    }


    public class JsonSerializationInfo {
        private AutomataDictionary                   _Dictionary;
        public JsonSerializationInfo(JsonSerializationInfoBuilder builder) {
             this._Dictionary = new AutomataDictionary();
            this.Properties = builder.Properties.Select(
                p => new JsonPropertySerializationInfo(
                    p.Name,
                    p.Order,
                    p.IsReadable,
                    p.IsWritable
                    )).ToArray();
                    foreach(var property in this.Properties){

            this._Dictionary.Add(JsonWriter.GetEncodedPropertyNameWithoutQuotation(property.Name), property.Order);
                    }
        }

        public JsonPropertySerializationInfo[] Properties { get; }

        public bool TryGetParameterValue(ArraySegment<byte> keyString,[MaybeNullWhen(false)] out int key) {
            throw new NotImplementedException();
        }
    }
    public class JsonPropertySerializationInfo {
        public JsonPropertySerializationInfo(
                string name,
                int order,
                bool isReadable,
                bool isWritable
            ) {
            this.Name = name;
            this.Order = order;
            this.IsReadable = isReadable;
            this.IsWritable = isWritable;
        }

        public string Name { get; }
        public int Order { get; }
        public bool IsReadable { get; }
        public bool IsWritable { get; }
    }

}