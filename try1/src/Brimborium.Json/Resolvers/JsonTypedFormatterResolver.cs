
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Brimborium.Json.Resolvers {
    public class JsonTypedFormatterResolver : IJsonFormatterResolver {
        private readonly Dictionary<Type, IJsonFormatter> _TypeToFormatter;

        public JsonTypedFormatterResolver(List<IJsonFormatter> lstFormatters) {
            var dict = new Dictionary<Type, IJsonFormatter>(lstFormatters.Count);
            foreach (var formatter in lstFormatters) {
                if (formatter is null) {
                    continue;
                }
                var formatterType = formatter.GetType();
                var formatterTypeArgument = GetIJsonFormatterArgument(formatterType);
                if (formatterTypeArgument is null) {
                } else {
                    if (dict.TryGetValue(formatterTypeArgument, out var oldFormatter)) {
                        throw new FormatterDuplicatedRegisteredException($"old: {oldFormatter?.GetType().FullName}; new: {formatter.GetType().FullName};");
                    } else {
                        dict.Add(formatterTypeArgument, formatter);
                    }
                }
            }
            this._TypeToFormatter = dict;
        }

        public IJsonFormatter<T> GetFormatter<T>(JsonSerializationConfiguration configuration) {
            if (this._TypeToFormatter.TryGetValue(typeof(T), out var formatter)) {
                return (IJsonFormatter<T>)formatter;
            } else { 
                throw new FormatterNotRegisteredException($"{typeof(T).FullName} no formatter is registered.");
            }
        }

        public static Type? GetIJsonFormatterArgument(Type formatterType) {
            foreach (var @interface in formatterType.GetTypeInfo().ImplementedInterfaces) {
                var ti = @interface.GetTypeInfo();
                if (ti.IsConstructedGenericType && typeof(IJsonFormatter<>).Equals(ti.GetGenericTypeDefinition())) {
                    return ti.GetGenericArguments()[0];
                }
            }
            return null;
        }
    }
}
