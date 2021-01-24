
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.Json {
    public class JsonParserUtf8Tests {
        //        [Fact]
        //        public async Task JsonParserUtf8_001_EmptyObject() {
        //            string json = "{}";
        //                InvokeParse(json, (context, iteration) => {
        //                    if (iteration == 0) {
        //                        Assert.Equal(2, context.CountToken);
        //                        var token0 = context.ReadCurrentToken();
        //                        var token1 = context.ReadCurrentToken();
        //                        Assert.Equal(0, context.CountToken);

        //                        Assert.Equal(JsonTokenKind.ObjectStart, token0.Kind);
        //                        Assert.Equal(JsonTokenKind.ObjectEnd, token1.Kind);

        //                        return true;
        //                    }
        //                    if (iteration == 1) {
        //#warning                        Assert.Equal(0, context.CountToken);
        //                    }
        //                    return false;
        //                });
        //            Assert.Equal(
        //                1,
        //                InvokeParse(json, (context, iteration) => {
        //                    Assert.Equal(new JsonTokenKind[] { JsonTokenKind.ObjectStart, JsonTokenKind.ObjectEnd }, GetTokenKinds(context));
        //                    return false;
        //                }));
        //        }

        [Fact]
        public async Task JsonParserUtf8_002_EmptyArray() {
            string json = "[]";
            await InvokeParse(json, async (context) => {
                if (context.EnsureTokens()) { await context.EnsureTokensAsync(); }
                Assert.Equal(JsonTokenKind.ArrayStart, context.CurrentToken.Kind);
                context.MoveNext();
                if (context.EnsureTokens()) { await context.EnsureTokensAsync(); }
                Assert.Equal(JsonTokenKind.ArrayEnd, context.CurrentToken.Kind);
            });
            await InvokeParse(json, async (context) => {
                if (context.EnsureTokens(2)) { await context.EnsureTokensAsync(); }
                Assert.Equal(JsonTokenKind.ArrayStart, context.GetToken(0).Kind);
                Assert.Equal(JsonTokenKind.ArrayEnd, context.GetToken(1).Kind);
                context.MoveNext(2);
            });
        }

        //        [Fact]
        //        public void JsonParserUtf8_003_true() {
        //            string json = "true";
        //            Assert.Equal(
        //                1,
        //                InvokeParse(json, (context, iteration) => {
        //                    Assert.Equal(new JsonTokenKind[] { JsonTokenKind.True }, GetTokenKinds(context));
        //                    return false;
        //                }));
        //        }

        //        [Fact]
        //        public void JsonParserUtf8_004_false() {
        //            string json = "false";
        //            Assert.Equal(
        //                1,
        //                InvokeParse(json, (context, iteration) => {
        //                    Assert.Equal(new JsonTokenKind[] { JsonTokenKind.False }, GetTokenKinds(context));
        //                    return false;
        //                }));
        //        }

        //        [Fact]
        //        public void JsonParserUtf8_004_null() {
        //            string json = "null";
        //            Assert.Equal(
        //                1,
        //                InvokeParse(json, (context, iteration) => {
        //                    Assert.Equal(new JsonTokenKind[] { JsonTokenKind.Null }, GetTokenKinds(context));
        //                    return false;
        //                }));
        //        }

        //        [Fact]
        //        public void JsonParserUtf8_005_Number() {
        //            string json = "1";
        //            Assert.Equal(
        //               1,
        //               InvokeParse(json, (context, iteration) => {
        //                   Assert.Equal(new JsonTokenKind[] { JsonTokenKind.Number }, GetTokenKinds(context));
        //                   return false;
        //               }));
        //        }

        //        [Fact]
        //        public void JsonParserUtf8_010_Boolean1Array() {
        //            string json = "[true,]";
        //            Assert.Equal(
        //                1,
        //                InvokeParse(json, (context, iteration) => {
        //                    Assert.Equal(new JsonTokenKind[] { JsonTokenKind.ArrayStart, JsonTokenKind.True, JsonTokenKind.ValueSep, JsonTokenKind.ArrayEnd }, GetTokenKinds(context));
        //                    return false;
        //                }));
        //        }

        public async Task JsonParserUtf8_011_BooleanManyArray() {
            //string json = "[true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,]";
            string json = "[true,true,true,]";
            await InvokeParse(json, async (context) => {
                if (context.EnsureTokens()) { await context.EnsureTokensAsync(); }
                Assert.Equal(JsonTokenKind.ArrayStart, context.CurrentToken.Kind);
                context.MoveNext();


                while (true) {
                    if (context.EnsureTokens(2)) { await context.EnsureTokensAsync(); }
                    if (context.GetToken(0).Kind == JsonTokenKind.ArrayEnd) { break; }
                    Assert.Equal(JsonTokenKind.True, context.GetToken(0).Kind);
                    Assert.Equal(JsonTokenKind.ValueSep, context.GetToken(1).Kind);
                    context.MoveNext(2);
                }

                if (context.EnsureTokens()) { await context.EnsureTokensAsync(); }
                Assert.Equal(JsonTokenKind.ArrayEnd, context.CurrentToken.Kind);
                context.MoveNext();

                Assert.Equal(JsonTokenKind.EOF, context.CurrentToken.Kind);
            });
        }

        //        [Fact]
        //        public void JsonParserUtf8_011_BooleanManyArray() {
        //            //string json = "[true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,]";
        //            string json = "[true,true,true]";
        //            var act = new List<JsonTokenKind>();
        //            var loops=InvokeParse(json, (context, iteration) => {
        //                act.AddRange(GetTokenKinds(context));
        //                context.FeedIndexToken = 0;
        //                context.ReadIndexToken = 0;
        //                //Assert.Equal(new JsonTokenKind[] { JsonTokenKind.ArrayStart, JsonTokenKind.True, JsonTokenKind.ValueSep, JsonTokenKind.ArrayEnd }, act);
        //                return (act.Last() != JsonTokenKind.ArrayEnd) && iteration < 10000;
        //            });
        //            Assert.True(loops<500,$"loops {loops}");
        //            Assert.Equal(JsonTokenKind.ArrayStart, act.First());
        //            Assert.Equal(JsonTokenKind.ArrayEnd, act.Last());
        //            for (var idx = 1; idx < act.Count - 1; idx += 2) {
        //                Assert.Equal(JsonTokenKind.True, act[idx]);
        //                Assert.Equal(JsonTokenKind.ValueSep, act[idx + 1]);
        //            }
        //        }

        //        [Fact]
        //        public void JsonParserUtf8_Speed() {
        //            string json = "[true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,]";
        //            for (int loop = 0; loop < 1000; loop++) {
        //                var act = new List<JsonTokenKind>();
        //                InvokeParse(json, (context, iteration) => {
        //                    act.AddRange(GetTokenKinds(context));
        //                    context.FeedIndexToken = 0;
        //                    context.ReadIndexToken = 0;
        //                    return (act.Last() != JsonTokenKind.ArrayEnd) && iteration < 1000;
        //                });
        //            }
        //        }
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

        private static async Task InvokeParse(string json, Func<JsonReaderContext, Task> action) {
            JsonReaderContext context = JsonReaderContextPool.Instance.Rent();
            var parser = new JsonParserUtf8(context);
            JsonText jsonText = new JsonText(json, false);
            var utf8 = jsonText.GetUtf8();
            BoundedByteArray src = new BoundedByteArray(utf8, 0, utf8.Length, false);
            context = new JsonReaderContext();
            parser.Parse(src, true);
            await action(context);
            JsonReaderContextPool.Instance.Return(context);
        }
    }
}
