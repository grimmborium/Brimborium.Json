#nullable enable

using Brimborium.Json.Internal;
using Brimborium.Json.Resolvers;

using System;

namespace Brimborium.Json {
    /// <summary>
    /// EntryPoint of Brimborium.Json.
    /// </summary>
    public static partial class JsonSerializer {
        static IJsonFormatterResolver? defaultResolver;

        /// <summary>
        /// FormatterResolver that used resolver less overloads. If does not set it, used StandardResolver.Default.
        /// </summary>
        [Obsolete]
        public static IJsonFormatterResolver DefaultResolver {
            get {
                throw new NotSupportedException();
            }
            set {
                defaultResolver = value; ;
            }
        }

        /// <summary>
        /// Is resolver decided?
        /// </summary>
        public static bool IsResolverInitialized {
            get {
                return defaultResolver != null;
            }
        }


        /// <summary>
        /// Set default resolver of Utf8Json APIs.
        /// </summary>
        /// <param name="resolver"></param>
        public static void SetDefaultResolver(IJsonFormatterResolver resolver) {
            defaultResolver = resolver;
        }

        private static JsonSerializationConfiguration? _DefaultConfiguration;

        /// <summary>
        /// Is DefaultConfiguration decided?
        /// </summary>
        public static bool IsConfigurationInitialized => _DefaultConfiguration != null;

        public static JsonSerializationConfiguration GetDefaultConfiguration() {
            if (_DefaultConfiguration is object) {
                return _DefaultConfiguration;
            } else {
                lock (typeof(JsonSerializer)) {
                    var builder = new JsonSerializationConfigurationBuilder();
#warning TODO builder.Resolvers
                    var defaultConfiguration = builder.Build();
                    System.Threading.Volatile.Write(ref _DefaultConfiguration, defaultConfiguration);
                    return defaultConfiguration;
                }
            }
        }
        public static void SetDefaultConfiguration(JsonSerializationConfiguration defaultConfiguration) {
            System.Threading.Volatile.Write(ref _DefaultConfiguration, defaultConfiguration);
        }
    }
}
