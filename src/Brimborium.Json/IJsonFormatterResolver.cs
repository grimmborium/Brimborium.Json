#nullable disable

using System;
using System.Reflection;

namespace Brimborium.Json {
    public interface IJsonFormatterResolver {
        IJsonFormatter<T> GetFormatter<T>();

        IJsonFormatterResolver? BindForReader(JsonSerializationConfiguration configuration) => this;

        IJsonFormatterResolver? BindForWriter(JsonSerializationConfiguration configuration) => this;
    }

    public interface IJsonSerializerResolverCommon {
        IJsonSerializer<T> GetSerializerCommon<T>(JsonSerializationConfiguration configuration);
    }

    public interface IJsonDeserializerResolverCommon {
        IJsonDeserializer<T> GetDeserializerCommon<T>(JsonSerializationConfiguration configuration);
    }

    public interface IJsonDeserializerResolver<TJsonReader>
            where TJsonReader : JsonReader {
        IJsonDeserializer<T, TJsonReader> GetJsonDeserializer<T>(JsonSerializationConfiguration configuration)            ;
    }

    public interface IJsonSerializerResolver<TJsonWriter>
            where TJsonWriter : JsonWriter {
        IJsonSerializer<T, TJsonWriter> GetJsonSerializer<T>(JsonSerializationConfiguration configuration);
    }

    public static class JsonFormatterResolverExtensions {
        public static IJsonSerializer2<T> GetSerializerWithVerify<T>(this JsonSerializationConfiguration configuration) {
            IJsonSerializer2<T> jsonSerializer;
            try {
                jsonSerializer = configuration.GetSerializer<T>();
            } catch (TypeInitializationException ex) {
                Exception inner = ex;
                while (inner.InnerException != null) {
                    inner = inner.InnerException;
                }

                throw inner;
            }

            if (jsonSerializer == null) {
                throw new FormatterNotRegisteredException(typeof(T).FullName + " Serializer is not registered.");
            }

            return jsonSerializer;
        }
        public static IJsonDeserializer2<T> GetDeserializerWithVerify<T>(this JsonSerializationConfiguration configuration) {
            IJsonDeserializer2<T> jsonDeserializer;
            try {
                jsonDeserializer = configuration.GetDeserializer<T>();
            } catch (TypeInitializationException ex) {
                Exception inner = ex;
                while (inner.InnerException != null) {
                    inner = inner.InnerException;
                }

                throw inner;
            }

            if (jsonDeserializer == null) {
                throw new FormatterNotRegisteredException(typeof(T).FullName + " Deserializer is not registered.");
            }

            return jsonDeserializer;
        }

        public static IJsonFormatter<T> GetFormatterWithVerify<T>(this IJsonFormatterResolver resolver) {
            IJsonFormatter<T> formatter;
            try {
                formatter = resolver.GetFormatter<T>();
            } catch (TypeInitializationException ex) {
                Exception inner = ex;
                while (inner.InnerException != null) {
                    inner = inner.InnerException;
                }

                throw inner;
            }

            if (formatter == null) {
                throw new FormatterNotRegisteredException(typeof(T).FullName + " is not registered in this resolver. resolver:" + resolver.GetType().Name);
            }

            return formatter;
        }

        public static object GetFormatterDynamic(this IJsonFormatterResolver resolver, Type type) {
            var methodInfo = typeof(IJsonFormatterResolver).GetRuntimeMethod("GetFormatter", Type.EmptyTypes);

            var formatter = methodInfo.MakeGenericMethod(type).Invoke(resolver, null);
            return formatter;
        }
    }

    public class FormatterNotRegisteredException : Exception {
        public FormatterNotRegisteredException(string message) : base(message) {
        }
    }
}