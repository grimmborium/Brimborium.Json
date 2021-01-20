using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleWepApi {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddControllers().AddMvcOptions(options => {
                //options.InputFormatters
            });
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SampleWepApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SampleWepApi v1"));
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            //app.Map(new PathString("/speedtest"), (builder) => {

            //});

            app.UseAuthorization();
            // app.UseMvc();
            app.UseEndpoints(endpoints => {
                endpoints.MapPost("/speedtest", async context => {
                    var sb = new StringBuilder();
                    if (context.Request != null) {
                        try {
                            //var contentLength = context.Request.ContentLength;
                            ////context.Request.BodyReader.CopyToAsync()
                            //System.IO.Pipelines.ReadResult readResult;
                            //ReadOnlySequence<byte> buffer = readResult.Buffer;
                            //SequencePosition position = buffer.Start;
                            //SequencePosition consumed = position;
                            //if (context.Request.BodyReader.TryRead(out readResult)) {
                            //} else { 
                            //    readResult = await context.Request.BodyReader.ReadAsync(context.RequestAborted);
                            //}
                            //while (readResult.IsCompleted) {
                            //    //readResult.Buffer.FirstSpan
                            //    buffer = readResult.Buffer;
                            //    position = buffer.Start;
                            //    ReadOnlyMemory<byte> memory;
                            //    while (buffer.TryGet(ref position, out memory)) { 
                            //    }
                            //    context.Request.BodyReader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);
                            //    if (context.Request.BodyReader.TryRead(out readResult)) {
                            //    } else {
                            //        readResult = await context.Request.BodyReader.ReadAsync(context.RequestAborted);
                            //    }
                            //}
                            System.DateTime start = System.DateTime.UtcNow;
                            System.DateTime last = start;
                            sb.AppendLine($"Start:{start}");
                            int l = 0;
                            var bodyReader = context.Request?.BodyReader;
                            if (bodyReader != null) {
                                //System.IO.Pipelines.ReadResult readResult;
                                while (true) {
                                    System.IO.Pipelines.ReadResult readResult;
                                    //if (bodyReader.TryRead(out readResult)) {
                                    //    sb.AppendLine("Fast");
                                    //} else {
                                    //    sb.AppendLine("await");
                                    //    readResult = await bodyReader.ReadAsync(context.RequestAborted);
                                    //}
                                    readResult = await bodyReader.ReadAsync(context.RequestAborted);
                                    var buffer = readResult.Buffer;
                                    var position = buffer.Start;
                                    var consumed = position;

                                    try {
                                        if (readResult.IsCanceled) {
                                            throw new OperationCanceledException();
                                        }

                                        while (buffer.TryGet(ref position, out var memory)) {
                                            //await writeAsync(destination, memory, cancellationToken).ConfigureAwait(false);
                                            consumed = position;
                                            System.DateTime now = System.DateTime.UtcNow;
                                            sb.AppendLine($"dt:{(now.Subtract(start)).TotalMilliseconds} dif:{now.Subtract(last).TotalMilliseconds} position:{position.GetInteger()}; len:{memory.Length}");
                                            last = now;
                                            l += memory.Length;
                                        }

                                        // The while loop completed succesfully, so we've consumed the entire buffer.
                                        consumed = buffer.End;

                                        if (readResult.IsCompleted) {
                                            break;
                                        }
                                    } finally {
                                        // Advance even if WriteAsync throws so the PipeReader is not left in the
                                        // currently reading state
                                        bodyReader.AdvanceTo(consumed);
                                    }
                                }
                                {
                                    System.DateTime now = System.DateTime.UtcNow;
                                    sb.AppendLine($"dt:{(now.Subtract(start)).TotalMilliseconds} Done len:{l}");
                                }
                                // 60 kb / ms

                                //context.Response.BodyWriter.WriteAsync(sb.ToString() ).ConfigureAwait(false);
                            }
                            await context.Response.WriteAsync(sb.ToString()).ConfigureAwait(false);
                        } catch (System.Exception e) {
                            sb.AppendLine(e.ToString());
                            await context.Response.WriteAsync(sb.ToString()).ConfigureAwait(false);
                        }
                    }
                });
                endpoints.MapControllers();
            });
        }
    }
}
