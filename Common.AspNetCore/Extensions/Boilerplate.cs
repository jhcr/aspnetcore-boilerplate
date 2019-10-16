using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using Common.AspNetCore.ErrorHandling;
using Common.AspNetCore.ApiClient;

namespace Common.AspNetCore.Extensions
{
    public static class Boilerplate
    {
        /// <summary>
        /// Boilerplate for DI with pre-defined features:
        /// * Swagger
        /// * Serilog
        /// * Correlation Id
        /// * MVC with global error handler
        /// * Api client handler for logging non-success status code
        /// * CORS
        /// </summary>
        /// <param name="builder">Web hosing builder</param>
        /// <param name="mvcSetupAction">change MVC options, add more filters</param>
        /// <param name="serilogLocalSetupAction">Change local debug log options</param>
        /// <returns></returns>
        public static IWebHostBuilder UseBoilerplate(this IWebHostBuilder builder, BoilerPlateOptions options = null)
        {
            return builder.ConfigureServices((context, services) =>
            {
                services.AddHttpContextToSerilog();

                services.AddCustomSwagger(options?.SwaggerGenSetup);

                services
                .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddMvcOptions(c =>
                {
                    c.Filters.Add<ValidateModelFilter>();
                    options?.MvcSetup?.Invoke(c);
                });

                services.AddCors();

                services.AddSingleton<NonSuccessStatusCodeHandler>();
            })
             .UseCustomSerilog(options?.SerilogLocalSetup);
        }


        public static IApplicationBuilder UseBoilerplate(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCorrelationIdToHttpContext(c => c.Header = "X-Correlation-ID");

            app.UseHttpContextToSerilog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseMvc();

            app.UseCors(c => c.AllowAnyOrigin());

            app.UseCustomSwagger(env);

            return app;
        }
    }

    public class BoilerPlateOptions
    {
        public Action<MvcOptions> MvcSetup { get; set; }
        public Action<Serilog.LoggerConfiguration> SerilogLocalSetup { get; set; }
        public Action<SwaggerGenOptions> SwaggerGenSetup { get; set; }
    }
}
