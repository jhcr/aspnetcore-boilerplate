using System;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Common.AspNetCore.Extensions
{
    public static class SerilogExtension
    {
        /// <summary>
        /// Use serilog with pre-defined options for Production, Staging, QA, Development and Local:
        /// * Enable Elasticsearch log at "Error" level for Production, Staging, QA, Development
        /// * Enable File log at "Information" level for QA, Development and Local
        /// * Enable Console log at "Debug" level for Local
        /// </summary>
        /// <param name="builder">Web host builder</param>
        /// <param name="overrideLocalAction">Change options for Local</param>
        /// <returns></returns>
        public static IWebHostBuilder UseCustomSerilog(this IWebHostBuilder builder, Action<LoggerConfiguration> overrideLocalAction = null)
        {
            return builder.UseSerilog((context, loggerConfiguration) =>
             {
                 //Enable Elasticsearch log for non-local at "Error" level
                 if (context.HostingEnvironment.IsProduction()
                 || context.HostingEnvironment.IsStaging()
                 || context.HostingEnvironment.IsEnvironment("QA")
                 || context.HostingEnvironment.IsDevelopment())
                 {
                     loggerConfiguration
                     .MinimumLevel.Error()
                     .Enrich.WithProperty("Environment", context?.HostingEnvironment?.EnvironmentName)
                     .Enrich.WithProperty("Application", context?.HostingEnvironment?.ApplicationName)
                     .Enrich.FromLogContext()
                     .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                     {
                         MinimumLogEventLevel = LogEventLevel.Error, // only send error to elasticsearch even non-prod
                         ConnectionTimeout = new TimeSpan(0, 0, 30),
                         IndexFormat = "wm-log-index-{0:yyyy.MM}"
                     });
                 }

                 // Enable File log for QA, Development and Local at "Information" level
                 if (context.HostingEnvironment.IsEnvironment("QA")
                 || context.HostingEnvironment.IsDevelopment()
                 || context.HostingEnvironment.IsEnvironment("Local"))
                 {
                     loggerConfiguration
                     .MinimumLevel.Information()
                     .WriteTo.File($"c:\\Logs\\{context?.HostingEnvironment?.ApplicationName??"wm_common"}.log",
                         outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}",
                         restrictedToMinimumLevel: LogEventLevel.Information,
                         rollingInterval: RollingInterval.Day,
                         fileSizeLimitBytes: 10240000,
                         rollOnFileSizeLimit: true,
                         retainedFileCountLimit: 14,
                         buffered: false, // buffered seems only working with Json formater...
                         shared: false);
                 }

                 // Enable Console log for Local at "Debug" level
                 if (context.HostingEnvironment.IsEnvironment("Local"))
                 {
                     loggerConfiguration
                     .MinimumLevel.Debug()
                     .WriteTo.Console(
                         restrictedToMinimumLevel: LogEventLevel.Debug,
                         outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}");
                 }

                 // Able to override configuration for Local 
                 // When debug locally, people may want to see different information than the default debug
                 if (context.HostingEnvironment.IsEnvironment("Local"))
                 {
                     overrideLocalAction?.Invoke(loggerConfiguration);
                 }
                 
             });
        }
    }
}
