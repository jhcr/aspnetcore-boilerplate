using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Common.AspNetCore.ApiClient
{
    /// <summary>
    /// HttpClient handler reading CorrelationId from HttpContext and passing to downstream services in request header
    /// Default header name is "X-Correlation-ID"
    /// </summary>
    public class CorrelationIdDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor;
        private CorrelationIdOptions _options = new CorrelationIdOptions() { Header = "X-Correlation-ID" };

        public CorrelationIdDelegatingHandler(
            IHttpContextAccessor accessor,
            Action<CorrelationIdOptions> setupAction = null)
        {
            _accessor = accessor;

            setupAction?.Invoke(_options);

            if (string.IsNullOrWhiteSpace(_options?.Header))
            {
                throw new ArgumentNullException(nameof(_options.Header));
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(_options.Header)
                && _accessor.HttpContext.Items.TryGetValue("CorrelationId", out var correlationId))
            {
                request.Headers.Add(_options.Header, (string)correlationId);
            }

            return base.SendAsync(request, cancellationToken);
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
}