# AspNetCore Boilerplate Library

### Features can be enabled by the library:
* Swagger
* Serilog (Elastic search and Kibana)
* Correlation Id
* MVC with global error handler
* Api client handler for logging non-success status code
* CORS

### Configuration to use:

Startup.cs:
```
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.UseBoilerplate(env);
    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient<IDownStreamApiClient, DownStreamApiClient>()
                .AddHttpMessageHandler<CorrelationIdDelegatingHandler>()
                .AddHttpMessageHandler<NonSuccessStatusCodeHandler>();
    }
``` 
 Program.cs:
 ```
    public class Program
    { 
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseBoilerplate()
            .UseStartup<Startup>();
    }
 ```   
 Configurations for MVC, serilog and swagger can also be overrided by passing BoilerPlateOptions to UseBoilerplate method.

### Configuration for log:
* Elasticsearch log is enabled at "Error" level for Production, Staging, QA, Development
* File log is enabled at "Information" level for QA, Development and Local
* Console log is enabled at "Debug" level for Local
* Local log configuration can be changed by BoilerPlateOptions.SerilogLocalSetup
