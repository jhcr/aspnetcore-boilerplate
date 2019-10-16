using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Common.AspNetCore.Logging
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private CorrelationIdOptions _options = new CorrelationIdOptions() { Header = "X-Correlation-ID" };

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public CorrelationIdMiddleware(RequestDelegate next, Action<CorrelationIdOptions> setupAction = null)
        {
            _next = next;

            setupAction?.Invoke(_options);

            if (string.IsNullOrWhiteSpace(_options.Header))
            {
                throw new ArgumentNullException(nameof(_options.Header));
            }
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = Guid.NewGuid().ToString();

            if (context.Request.Headers.TryGetValue(_options.Header, out var cids))
            {
                correlationId = cids.FirstOrDefault();
            }
           
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(_options.Header))
                {
                    context.Response.Headers.Add(_options.Header, correlationId);
                }
                return Task.CompletedTask;
            });

            context.Items["CorrelationId"] = correlationId;

            await _next(context);

        }
    }

    public class CorrelationIdOptions : IOptions<CorrelationIdOptions>
    {
        /// <summary>
        /// Lookup name in header
        /// </summary>
        public string Header { get; set; }

        public CorrelationIdOptions Value => this;
    }
}
