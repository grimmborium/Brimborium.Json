
using System;
using System.Net.Http.Headers;

using Xunit;

namespace Brimborium.Json {
    public class JsonParserUtf8Tests {
        [Fact]
        public void JsonParserUtf8_001_EmptyObject() {
            string json = "{}";
            var context = InvokeParse(json);
            Assert.Equal(2, context.CountToken);
            Assert.Equal(JsonTokenKind.ObjectStart, context.Tokens[0].Kind);
            Assert.Equal(JsonTokenKind.ObjectEnd, context.Tokens[1].Kind);
            Assert.Equal(new JsonTokenKind[] { JsonTokenKind.ObjectStart, JsonTokenKind.ObjectEnd }, GetTokenKinds(context));
        }

        [Fact]
        public void JsonParserUtf8_002_EmptyArray() {
            string json = "[]";
            var context = InvokeParse(json);
            Assert.Equal(new JsonTokenKind[] { JsonTokenKind.ArrayStart, JsonTokenKind.ArrayEnd }, GetTokenKinds(context));
        }

        [Fact]
        public void JsonParserUtf8_003_true() {
            string json = "true";
            var context = InvokeParse(json);
            Assert.Equal(new JsonTokenKind[] { JsonTokenKind.True }, GetTokenKinds(context));
        }

        [Fact]
        public void JsonParserUtf8_004_false() {
            string json = "false";
            var context = InvokeParse(json);
            Assert.Equal(new JsonTokenKind[] { JsonTokenKind.False }, GetTokenKinds(context));
        }

        [Fact]
        public void JsonParserUtf8_004_null() {
            string json = "null";
            var context = InvokeParse(json);
            Assert.Equal(new JsonTokenKind[] { JsonTokenKind.Null }, GetTokenKinds(context));
        }

        [Fact]
        public void JsonParserUtf8_005_Number() {
            string json = "1";
            var context = InvokeParse(json);
            Assert.Equal(new JsonTokenKind[] { JsonTokenKind.Number }, GetTokenKinds(context));
        }

        [Fact]
        public void JsonParserUtf8_010_Boolean1Array() {
            string json = "[true,]";
            var context = InvokeParse(json);
            Assert.Equal(new JsonTokenKind[] { JsonTokenKind.ArrayStart, JsonTokenKind.True, JsonTokenKind.ValueSep, JsonTokenKind.ArrayEnd }, GetTokenKinds(context));
        }

        private JsonTokenKind[] GetTokenKinds(JsonReaderContext context) {
            var result = new JsonTokenKind[context.CountToken];
            for (int idx = 0; (idx < context.CountToken); idx++) {
                result[idx] = context.Tokens[idx].Kind;
            }
            return result;
        }
        /*
         private JsonTokenKind[] GetTokenKinds(JsonReaderContext context) {
            for (int idx=0; (idx<context.CountToken) ; idx++ ) {
            }
            Assert.Equal(context.CountToken);
        }
         */

        private static JsonReaderContext InvokeParse(string json) {
            var parser = new JsonParserUtf8();
            JsonText jsonText = new JsonText(json, false);
            BoundedByteArray src = new BoundedByteArray(jsonText.GetUtf8());
            JsonReaderContext context = new JsonReaderContext();
            parser.Parse(src, context);
            return context;
        }
    }
}
