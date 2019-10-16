using Microsoft.AspNetCore.Builder;
using System;
using Common.AspNetCore.Security;

namespace Common.AspNetCore.Extensions
{
    public static class ApiKeyMiddlewareExtension
    {
        /// <summary>
        /// Use ApiKey middleware
        /// <remarks>
        /// Sample:
        /// app.UseCustomApiKey(c =>
        /// {
        ///     c.NameInHeader = "X-API-KEY";
        ///     c.ValidApiKeys = new List<string>(configuration["HostedApi:ValidApiKeys"]?.Split(','));
        ///     c.PathSegmentsToExclude = new List<string> { "/swagger", "/healthcheck" };
        /// });
        /// </remarks>
        /// </summary>
        public static IApplicationBuilder UseCustomApiKey(this IApplicationBuilder app, Action<APiKeyOptions> setupAction = null)
        {
            return app.UseMiddleware<ApiKeyMiddleware>(setupAction);
        }
    }
}
