using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;

namespace Common.AspNetCore.Extensions
{
    public static class SwaggerExtension
    {
        /// <summary>
        /// Use swagger on non-production enviroments
        /// </summary>
        /// <param name="app">Applicaiton builder</param>
        /// <param name="env">Hosting enviroment</param>
        /// <param name="swaggerSetupAction">Override swagger options</param>
        /// <param name="swaggerUISetupAction">Override swagger UI options</param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, IHostingEnvironment env, Action<SwaggerOptions> swaggerSetupAction = null, Action<SwaggerUIOptions> swaggerUISetupAction = null)
        {
            if (!env.IsProduction())
            {
                app.UseSwagger(c =>
                {
                    swaggerSetupAction?.Invoke(c);
                });

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", env.ApplicationName);

                    swaggerUISetupAction?.Invoke(c);
                });
            }

            return app;
        }

        /// <summary>
        /// Register swagger Gen for DI with pre-defined options:
        /// * Include XML comments from codes
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="setupAction">Extend Swagger Gen options</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, Action<SwaggerGenOptions> setupAction = null)
        {
            return services
            .AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "APIs", Version = "v1" });

                /* Security control for callers, don't need now
                c.AddSecurityDefinition("API Key Authentication", new ApiKeyScheme()
                {
                    In = "header",
                    Name = "X-API-KEY",
                    Type = "apiKey",
                    Description = "Copy \"1cd789b9-12e7-421a-9c82-2e64ad39190c\" as API key value for non-production"
                });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "API Key Authentication",new string[]{}}
                });
                */

                // Swagger reads XML comments from below files.
                foreach (var xmlFile in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

                setupAction?.Invoke(c);

            });
        }

    }
}
