using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Common.Core.Logging;

namespace Common.AspNetCore.ApiClient
{
    /// <summary>
    /// Handler for non 200 to 300 series of http status codes
    /// Ensure request response log, exception throwing
    /// </summary>
    public class NonSuccessStatusCodeHandler : DelegatingHandler
    {
        private readonly ILogger<NonSuccessStatusCodeHandler> _logger;

        public NonSuccessStatusCodeHandler(ILogger<NonSuccessStatusCodeHandler> logger)
        {
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(new EventId(LoggingEvents.ApiClient), await response.GetLogMessage());
            }

            return response;
        }
    }
}
