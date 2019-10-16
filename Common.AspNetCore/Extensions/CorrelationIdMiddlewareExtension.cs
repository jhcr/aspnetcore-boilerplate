using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Common.AspNetCore.Logging;

namespace Common.AspNetCore.Extensions
{
    public static class CorrelationIdMiddlewareExtension
    {
        /// <summary>
        /// Use correlationId middleware
        /// </summary>
        /// <param name="app"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCorrelationIdToHttpContext(this IApplicationBuilder app, Action<CorrelationIdOptions> setupAction = null)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>(setupAction);
        }
    }
}
