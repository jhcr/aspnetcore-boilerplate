# AspNetCore Boilerplate Library

Following features can be enabled to boilerplate aspnet core applications:
* Swagger
* Serilog (Elastic search and Kibana)
* Correlation Id
* MVC with global error handler
* Api client handler for logging non-success status code
* CORS

By following configuration:

Startup.cs:

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
 
 Program.cs:
 
    public class Program
    { 
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseBoilerplate()
            .UseStartup<Startup>();
    }
    
 Configurations for MVC, serilog and swagger can also be overrided by passing BoilerPlateOptions to UseBoilerplate method.
    
    