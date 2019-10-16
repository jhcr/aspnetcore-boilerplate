using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace Common.AspNetCore.Logging.Serilog
{
    public class SerilogHttpContextEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SerilogHttpContextEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

        }
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var ctx = _httpContextAccessor.HttpContext;

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("CorrelationId",
               ctx.Items["CorrelationId"]?.ToString()));

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Host",
               ctx.Request.Host.ToString()));

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("QueryString",
              ctx.Request.QueryString.ToString()));

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Method",
              ctx.Request.Method));
        }
    }
}
