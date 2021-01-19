#nullable enable

//
namespace Brimborium.Json {
    public static class JsonConstText {
        public static readonly JsonText True = new JsonText("true", false);
        public static readonly JsonText False = new JsonText("false", false);
        public static readonly JsonText ObjectOpen = new JsonText("{", false);
        public static readonly JsonText ObjectClose = new JsonText("}", false);
        public static readonly JsonText String1 = new JsonText("\"", false);
        public static readonly JsonText String2 = new JsonText("'", false);
        public static readonly JsonText StringQuote1 = new JsonText("\\\"", false);
        public static readonly JsonText StringQuote2 = new JsonText("\\'", false);
        public static readonly JsonText KeyValueSep = new JsonText(":", false);
        public static readonly JsonText ArrayOpen = new JsonText("[", false);
        public static readonly JsonText ArrayClose = new JsonText("]", false);
        public static readonly JsonText ArraySep = new JsonText(",", false);
    }
}