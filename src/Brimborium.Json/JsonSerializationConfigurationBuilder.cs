using Brimborium.Json.Internal;
using Brimborium.Json.Resolvers;

using System;
using System.Collections.Generic;

namespace Brimborium.Json {
    public class JsonSerializationConfigurationBuilder {
        private JsonSerializationConfigurationState _State;
        public bool AddDefaultResolversAndFormatters { get { return this._State.AddDefaultResolversAndFormatters; } set { this._State.AddDefaultResolversAndFormatters = value; } }
        public Func<string, string> PropertyNameMutator { get { return this._State.PropertyNameMutator; } set { this._State.PropertyNameMutator = value; } }
        // public bool AllowTrailingCommas { get; set; }
        // public JsonCommentHandling CommentHandling { get; set; }
        public JsonNumberHandling NumberHandling { get { return this._State.NumberHandling; } set { this._State.NumberHandling = value; } }
        public bool PropertyNameCaseInsensitive { get { return this._State.PropertyNameCaseInsensitive; } set { this._State.PropertyNameCaseInsensitive = value; } }
        // public ReferenceHandler ReferenceHandler { get; set; }
        public List<IJsonFormatterResolver> Resolvers => this._State.Resolvers;
        public List<IJsonFormatter> Formatters => this._State.Formatters;

        public JsonSerializationConfigurationBuilder() {
            this._State = new JsonSerializationConfigurationState(
                addDefaultResolversAndFormatters: true,
                propertyNameMutator: StringMutator.Original,
                numberHandling: JsonNumberHandling.Strict,
                propertyNameCaseInsensitive: false
                );
        }

        public JsonSerializationConfigurationBuilder WithPropertyNameMutator(string propertyNameMutator) {
            this.PropertyNameMutator =
                propertyNameMutator switch {
                    "CamelCase" => StringMutator.CamelCase,
                    "SnakeCase" => StringMutator.SnakeCase,
                    _ => StringMutator.Original
                };
            return this;
        }

        public JsonSerializationConfiguration Build() {
            return JsonSerializationConfigurationRoot.Build(this._State);
        }
    }

    public struct JsonSerializationConfigurationState {
        public bool AddDefaultResolversAndFormatters { get; set; }
        public Func<string, string> PropertyNameMutator { get; set; }
        // public bool AllowTrailingCommas { get; set; }
        // public JsonCommentHandling CommentHandling { get; set; }
        public JsonNumberHandling NumberHandling { get; set; }
        public bool PropertyNameCaseInsensitive { get; set; }
        // public ReferenceHandler ReferenceHandler { get; set; }
        public readonly List<IJsonFormatterResolver> Resolvers;
        public readonly List<IJsonFormatter> Formatters;

        public JsonSerializationConfigurationState(
                bool addDefaultResolversAndFormatters,
                Func<string, string> propertyNameMutator,
                // bool allowTrailingCommas,
                // JsonCommentHandling commentHandling,
                JsonNumberHandling numberHandling,
                bool propertyNameCaseInsensitive
            ) {
            AddDefaultResolversAndFormatters = addDefaultResolversAndFormatters;
            PropertyNameMutator = propertyNameMutator;
            NumberHandling = numberHandling;
            PropertyNameCaseInsensitive = propertyNameCaseInsensitive;
            this.Resolvers = new List<IJsonFormatterResolver>();
            this.Formatters = new List<IJsonFormatter>();
        }
    }
}