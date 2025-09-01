using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using System.Text;

namespace TouchpointMedical.Extensions
{
    public sealed class RawBodyBufferingOptions
    {
        public string[] PathPrefixes { get; set; } = [];

        public string[] Methods { get; set; } = ["POST"];

        public string[] ContentTypes { get; set; } = ["application/json"];

        // 64 KB in-memory before temp file
        public int BufferThresholdBytes { get; set; } = 64 * 1024;

        // 10 MB max (null = unlimited)
        public long? BufferLimitBytes { get; set; } = 10 * 1024 * 1024;

        // set true if you want Items["RawBody"]
        public bool ShouldStashInHttpContextItems { get; set; } = false;

        public string ItemsKey { get; set; } = "RawBody";
    }

    public static class ApplictionBuilderExtensions
    {
        public static IApplicationBuilder UseRawBodyBuffering(
            this IApplicationBuilder app,
            Action<RawBodyBufferingOptions>? configure = null)
        {
            var options = new RawBodyBufferingOptions();

            configure?.Invoke(options);

                return app.Use(async (context, next) =>
                {
                    // Filter by path
                    if (!options.PathPrefixes.Any(
                        s => context.Request.Path.StartsWithSegments(s, StringComparison.OrdinalIgnoreCase)))
                    {
                        await next();

                        return;
                    }

                    // Filter by method
                    if (Array.IndexOf(options.Methods, context.Request.Method.ToUpperInvariant()) < 0)
                    {
                        await next();

                        return;
                    }

                    // Filter by content type (lenient contains check)
                    var ct = context.Request.ContentType ?? string.Empty;
                    var matchesCt = options.ContentTypes.Length == 0
                        || Array.Exists(options.ContentTypes,
                            t => ct.Contains(t, StringComparison.OrdinalIgnoreCase));

                    if (!matchesCt)
                    {
                        await next();

                        return;
                    }

                    // Enable buffering with thresholds/limits
                    if (options.BufferLimitBytes.HasValue)
                    {
                        context.Request.EnableBuffering(
                            bufferThreshold: options.BufferThresholdBytes,
                            bufferLimit: (int)options.BufferLimitBytes.Value);
                    }
                    else
                    {
                        context.Request.EnableBuffering(options.BufferThresholdBytes);
                    }

                    if (options.ShouldStashInHttpContextItems)
                    {
                        context.Request.Body.Position = 0;
                        using var reader = new StreamReader(
                            context.Request.Body, 
                            Encoding.UTF8, 
                            detectEncodingFromByteOrderMarks: false, 
                            leaveOpen: true);

                        var raw = await reader.ReadToEndAsync();
                        context.Items[options.ItemsKey] = raw;
                        context.Request.Body.Position = 0;
                    }

                    await next();
                });
        }

    }

}
