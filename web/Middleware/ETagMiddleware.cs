using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Security.Cryptography;

namespace Atlas_Web.Middleware
{
    //https://gist.github.com/madskristensen/36357b1df9ddbfd123162cd4201124c4
    public class ETagMiddleware
    {
        private readonly RequestDelegate _next;

        public ETagMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var response = context.Response;
            var originalStream = response.Body;

            using var ms = new MemoryStream();
            response.Body = ms;

            await _next(context);

            if (IsEtagSupported(response))
            {
                string checksum = CalculateChecksum(ms);

                response.Headers[HeaderNames.ETag] = checksum;

                if (
                    context.Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var etag)
                    && checksum == etag
                )
                {
                    response.StatusCode = StatusCodes.Status304NotModified;
                    response.Headers[HeaderNames.ContentLength] = "0";
                    response.ContentType = null;
                    return;
                }
            }

            ms.Position = 0;
            await ms.CopyToAsync(originalStream).ConfigureAwait(false);
        }

        private static bool IsEtagSupported(HttpResponse response)
        {
            if (response.StatusCode != StatusCodes.Status200OK)
            {
                return false;
            }

            // The 200kb length limit is not based in science. Feel free to change
            if (response.Body.Length > 200 * 1024)
            {
                return false;
            }

            if (response.Headers.ContainsKey(HeaderNames.ETag))
            {
                return false;
            }

            return true;
        }

        private static string CalculateChecksum(MemoryStream ms)
        {
            string checksum = "";

            using (var algo = SHA1.Create())
            {
                ms.Position = 0;
                byte[] bytes = algo.ComputeHash(ms);
                checksum = $"\"{WebEncoders.Base64UrlEncode(bytes)}\"";
            }

            return checksum;
        }
    }

    public static class ApplicationBuilderExtensions
    {
        public static void UseETagger(this IApplicationBuilder app)
        {
            app.UseMiddleware<ETagMiddleware>();
        }
    }
}
