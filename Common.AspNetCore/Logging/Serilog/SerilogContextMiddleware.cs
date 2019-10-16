using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Threading.Tasks;

namespace Common.AspNetCore.Logging.Serilog
{
    public class HttpContextToSerilogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor _accessor;

        public HttpContextToSerilogMiddleware(RequestDelegate next, IHttpContextAccessor accessor)
        {
            _next = next;
            _accessor = accessor;
        }

        public async Task Invoke(HttpContext context)
        {
            using (LogContext.Push(new SerilogHttpContextEnricher(_accessor)))
            {
                await _next(context);
            }
        }
    }


}
