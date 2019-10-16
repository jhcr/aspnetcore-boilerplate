using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Common.AspNetCore.Logging.Serilog;

namespace Common.AspNetCore.Extensions
{
    public static class HttpContextToSerilogMiddlewareExtension
    {
        /// <summary>
        /// Register HttpContextToSerilog middleware's dependecies to DI
        /// </summary>
        /// <param name="services"></param>
        public static void AddHttpContextToSerilog(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<SerilogHttpContextEnricher>();
        }
        /// <summary>
        /// Use HttpContextToSerilog middleware
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpContextToSerilog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HttpContextToSerilogMiddleware>();
        }

    }
}
