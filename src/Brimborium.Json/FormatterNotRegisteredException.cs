#nullable enable

using System;

namespace Brimborium.Json {
    public class FormatterNotRegisteredException : Exception {
        public FormatterNotRegisteredException(string message) : base(message) {
        }
    }
    public class FormatterDuplicatedRegisteredException : Exception {
        public FormatterDuplicatedRegisteredException(string message) : base(message) {
        }
    }
}