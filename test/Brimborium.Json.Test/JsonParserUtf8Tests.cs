
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.Json {
    public class JsonParserUtf8Tests {
        [Fact]
        public async Task JsonParserUtf8_001_EmptyObject() {
            string json = "{}";
            await InvokeParse(json, async (jsonSource) => {
                if (jsonSource.EnsureTokens()) { await jsonSource.EnsureTokensAsync(); }
                Assert.Equal(JsonTokenKind.ObjectStart, jsonSource.CurrentToken.Kind);
                jsonSource.Advance();
                if (jsonSource.EnsureTokens()) { await jsonSource.EnsureTokensAsync(); }
                Assert.Equal(JsonTokenKind.ObjectEnd, jsonSource.CurrentToken.Kind);
                jsonSource.Advance();
            });
            await InvokeParse(json, async (jsonSource) => {
                if (jsonSource.EnsureTokens(2)) { await jsonSource.EnsureTokensAsync(); }
                Assert.Equal(JsonTokenKind.ObjectStart, jsonSource.GetToken(0).Kind);
                Assert.Equal(JsonTokenKind.ObjectEnd, jsonSource.GetToken(1).Kind);
                jsonSource.Advance(2);
            });
        }

        [Fact]
        public async Task JsonParserUtf8_002_EmptyArray() {
            string json = "[]";
            await InvokeParse(json, async (jsonSource) => {
                if (jsonSource.EnsureTokens()) { await jsonSource.EnsureTokensAsync(); }
                Assert.Equal(JsonTokenKind.ArrayStart, jsonSource.CurrentToken.Kind);
                jsonSource.Advance();
                if (jsonSource.EnsureTokens()) { await jsonSource.EnsureTokensAsync(); }
                Assert.Equal(JsonTokenKind.ArrayEnd, jsonSource.CurrentToken.Kind);
                jsonSource.Advance();
            });
            await InvokeParse(json, async (jsonSource) => {
                if (jsonSource.EnsureTokens(2)) { await jsonSource.EnsureTokensAsync(); }
                Assert.Equal(JsonTokenKind.ArrayStart, jsonSource.GetToken(0).Kind);
                Assert.Equal(JsonTokenKind.ArrayEnd, jsonSource.GetToken(1).Kind);
                jsonSource.Advance(2);
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

        [Fact]
        public async Task JsonParserUtf8_011_BooleanManyArray() {
            string json = "[true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,]";
            for (int loop = 0; loop < 100000; loop++) {
                //string json = "[true,true,true,]";
                await InvokeParse(json, async (jsonSource) => {
                    if (jsonSource.EnsureTokens()) { await jsonSource.EnsureTokensAsync(); }
                    Assert.Equal(JsonTokenKind.ArrayStart, jsonSource.CurrentToken.Kind);
                    jsonSource.Advance();


                    while (true) {
                        if (jsonSource.EnsureTokens(2)) { await jsonSource.EnsureTokensAsync(); }
                        if (jsonSource.GetToken(0).Kind == JsonTokenKind.ArrayEnd) { break; }
                        Assert.Equal(JsonTokenKind.True, jsonSource.GetToken(0).Kind);
                        Assert.Equal(JsonTokenKind.ValueSep, jsonSource.GetToken(1).Kind);
                        jsonSource.Advance(2);
                    }

                    if (jsonSource.EnsureTokens()) { await jsonSource.EnsureTokensAsync(); }
                    Assert.Equal(JsonTokenKind.ArrayEnd, jsonSource.CurrentToken.Kind);
                    jsonSource.Advance();

                    Assert.Equal(JsonTokenKind.EOF, jsonSource.CurrentToken.Kind);
                });
            }
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

        private JsonTokenKind[] GetTokenKinds(JsonSource jsonSource) {
            var result = new JsonTokenKind[jsonSource.CountToken];
            for (int idx = jsonSource.ReadIndexToken; idx < jsonSource.FeedIndexToken; idx++) {
                result[idx] = jsonSource.Tokens[idx].Kind;
            }
            return result;
        }
        //private JsonTokenKind[] GetTokenKinds(JsonReaderContext context) {
        //    for (int idx = 0; (idx < context.CountToken); idx++) {
        //    }
        //    Assert.Equal(context.CountToken);
        //}

        //private static async Task InvokeParse(string json, Action<JsonReaderContext> action) {
        //    JsonReaderContext context = JsonReaderContextPool.Instance.Rent();
        //    var parser = new JsonParserUtf8(context);
        //    JsonText jsonText = new JsonText(json, false);
        //    var utf8 = jsonText.GetUtf8();
        //    BoundedByteArray src = new BoundedByteArray(utf8, 0, utf8.Length, false);
        //    context = new JsonReaderContext();
        //    parser.Parse(src, true);
        //    action(context);
        //    JsonReaderContextPool.Instance.Return(context);
        //}

        private static async Task InvokeParse(string json, Action<JsonSourceUtf8> action) {
            JsonText jsonText = new JsonText(json, false);
            var utf8 = jsonText.GetUtf8();
            MemoryStream ms = new MemoryStream(utf8);
            JsonConfiguration configuration = new JsonConfiguration();
            BoundedByteArray src = new BoundedByteArray(utf8, 0, utf8.Length, false);
            using (var source = new JsonSourceUtf8SyncStream(ms, configuration)) {
                await source.ReadFromSourceAsync();
                action(source);
            }
        }
    }
}
