
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

using Xunit;

namespace Brimborium.Json {
    public class JsonParserUtf8Tests {
        [Fact]
        public void JsonParserUtf8_001_EmptyObject() {
            string json = "{}";
            Assert.Equal(2,
                InvokeParse(json, (context, iteration) => {
                    if (iteration == 0) {
                        Assert.Equal(2, context.CountToken);
                        var token0 = context.ReadCurrentToken();
                        var token1 = context.ReadCurrentToken();
                        Assert.Equal(0, context.CountToken);

                        Assert.Equal(JsonTokenKind.ObjectStart, token0.Kind);
                        Assert.Equal(JsonTokenKind.ObjectEnd, token1.Kind);

                        return true;
                    }
                    if (iteration == 1) {
                        Assert.Equal(0, context.CountToken);
                    }
                    return false;
                }));
            Assert.Equal(
                1,
                InvokeParse(json, (context, iteration) => {
                    Assert.Equal(new JsonTokenKind[] { JsonTokenKind.ObjectStart, JsonTokenKind.ObjectEnd }, GetTokenKinds(context));
                    return false;
                }));
        }

        [Fact]
        public void JsonParserUtf8_002_EmptyArray() {
            string json = "[]";
            Assert.Equal(
                1,
                InvokeParse(json, (context, iteration) => {
                    Assert.Equal(new JsonTokenKind[] { JsonTokenKind.ArrayStart, JsonTokenKind.ArrayEnd }, GetTokenKinds(context));
                    return false;
                }));
        }

        [Fact]
        public void JsonParserUtf8_003_true() {
            string json = "true";
            Assert.Equal(
                1,
                InvokeParse(json, (context, iteration) => {
                    Assert.Equal(new JsonTokenKind[] { JsonTokenKind.True }, GetTokenKinds(context));
                    return false;
                }));
        }

        [Fact]
        public void JsonParserUtf8_004_false() {
            string json = "false";
            Assert.Equal(
                1,
                InvokeParse(json, (context, iteration) => {
                    Assert.Equal(new JsonTokenKind[] { JsonTokenKind.False }, GetTokenKinds(context));
                    return false;
                }));
        }

        [Fact]
        public void JsonParserUtf8_004_null() {
            string json = "null";
            Assert.Equal(
                1,
                InvokeParse(json, (context, iteration) => {
                    Assert.Equal(new JsonTokenKind[] { JsonTokenKind.Null }, GetTokenKinds(context));
                    return false;
                }));
        }

        [Fact]
        public void JsonParserUtf8_005_Number() {
            string json = "1";
            Assert.Equal(
               1,
               InvokeParse(json, (context, iteration) => {
                   Assert.Equal(new JsonTokenKind[] { JsonTokenKind.Number }, GetTokenKinds(context));
                   return false;
               }));
        }

        [Fact]
        public void JsonParserUtf8_010_Boolean1Array() {
            string json = "[true,]";
            Assert.Equal(
                1,
                InvokeParse(json, (context, iteration) => {
                    Assert.Equal(new JsonTokenKind[] { JsonTokenKind.ArrayStart, JsonTokenKind.True, JsonTokenKind.ValueSep, JsonTokenKind.ArrayEnd }, GetTokenKinds(context));
                    return false;
                }));
        }

        [Fact]
        public void JsonParserUtf8_011_BooleanManyArray() {
            string json = "[true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,]";
            var act = new List<JsonTokenKind>();
            Assert.Equal(
                1,
                InvokeParse(json, (context, iteration) => {
                    act.AddRange(GetTokenKinds(context));

                    //Assert.Equal(new JsonTokenKind[] { JsonTokenKind.ArrayStart, JsonTokenKind.True, JsonTokenKind.ValueSep, JsonTokenKind.ArrayEnd }, act);
                    return (act.Last() == JsonTokenKind.ArrayEnd) || iteration == 1000;
                }));
            Assert.Equal(JsonTokenKind.ArrayStart, act.First());
            Assert.Equal(JsonTokenKind.ArrayEnd, act.Last());
            for (var idx = 1; idx < act.Count-1; idx += 2) {
                Assert.Equal(JsonTokenKind.True, act[idx]);
                Assert.Equal(JsonTokenKind.ValueSep, act[idx + 1]);
            }
        }
        /*
        */

        private JsonTokenKind[] GetTokenKinds(JsonReaderContext context) {
            var result = new JsonTokenKind[context.CountToken];
            for (int idx = context.ReadIndexToken; idx < context.FeedIndexToken; idx++) {
                result[idx] = context.Tokens[idx].Kind;
            }
            return result;
        }
        //private JsonTokenKind[] GetTokenKinds(JsonReaderContext context) {
        //    for (int idx = 0; (idx < context.CountToken); idx++) {
        //    }
        //    Assert.Equal(context.CountToken);
        //}

        private static int InvokeParse(string json, Func<JsonReaderContext, int, bool> action) {
            var parser = new JsonParserUtf8();
            JsonText jsonText = new JsonText(json, false);
            var utf8 = jsonText.GetUtf8();
            JsonReaderContext context = JsonReaderContextPool.Instance.Rent();
            BoundedByteArray src = new BoundedByteArray(utf8, 0, utf8.Length, false);
            context = new JsonReaderContext();
            int iteration = 0;
            parser.Parse(src, context, true);
            while (action(context, iteration)) {
                iteration++;
                parser.Parse(context);
            }
            JsonReaderContextPool.Instance.Return(context);
            return iteration;
        }
    }
}
