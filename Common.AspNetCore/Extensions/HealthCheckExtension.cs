using System;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Common.AspNetCore.Extensions
{
    public static class HealthCheckExtension
    {
        /// <summary>
        /// Use health check middleware with pre-defined options:
        /// * ResponseWriter: Include each dependency's status and elapsed time in response
        /// * Endpoint: /HealthCheck
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="setupAction">change pre-defined options</param>
        /// <param name="path">change pre-defined healthcheck endpoint path</param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomHealthChecks(this IApplicationBuilder app, Action<HealthCheckOptions> setupAction = null, string path = null)
        {
            var options = new HealthCheckOptions
            {
                ResponseWriter = async (c, r) =>
                {
                    c.Response.ContentType = MediaTypeNames.Application.Json;
                    var result = JsonConvert.SerializeObject(
                       new
                       {
                           status = r.Status.ToString(),
                           checks = r.Entries.Select(e =>
                           new
                           {
                               description = e.Key,
                               status = e.Value.Status.ToString(),
                               responseTime = e.Value.Duration.TotalMilliseconds
                           }),
                           totalResponseTime = r.TotalDuration.TotalMilliseconds
                       });
                    await c.Response.WriteAsync(result);
                }
            };

            setupAction?.Invoke(options);

            return app.UseHealthChecks(path ?? "/healthcheck", options);
        }
    }
}
