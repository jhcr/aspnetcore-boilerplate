using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Common.Core.Logging;

namespace Common.AspNetCore.Security
{
    /// <summary>
    /// Middleware to validate ApiKey in header or query string
    /// </summary>
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ApiKeyMiddleware> _logger;

        private APiKeyOptions _options = new APiKeyOptions();

        public ApiKeyMiddleware(RequestDelegate next, Action<APiKeyOptions> setupAction, ILogger<ApiKeyMiddleware> logger)
        {
            if (setupAction == null)
                throw new ArgumentNullException("options", "options cannot be null");

            setupAction(_options);

            if (!(_options?.ValidApiKeys?.Count > 0))
                throw new ArgumentNullException("ValidApiKeys", "ValidApiKeys cannot be empty");
            if (string.IsNullOrWhiteSpace(_options?.NameInHeader) 
                && string.IsNullOrWhiteSpace(_options?.NameInQuery))
                throw new ArgumentNullException("NameInHeader or NameInQuery", "Either need have value");

            _logger = logger;

            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (ShouldSkip(context))
            {
                await _next.Invoke(context);
                return;
            }

            var apiKey = string.Empty;

            if (!string.IsNullOrWhiteSpace(_options.NameInHeader)
                && context.Request.Headers.ContainsKey(_options.NameInHeader))
            {
                apiKey = context.Request.Headers[_options.NameInHeader].FirstOrDefault();
            }
            else if (!string.IsNullOrWhiteSpace(_options.NameInQuery)
                && context.Request.Query.ContainsKey(_options.NameInQuery))
            {
                apiKey = context.Request.Query[_options.NameInQuery].FirstOrDefault();
            }

            if (ValidateApiKey(apiKey))
            {
                await _next.Invoke(context);
                return;
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Unauthorized",
                    Status = (int)HttpStatusCode.Unauthorized,
                    Detail = "Api key is not valid"
                };
                problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

                _logger.LogError(LoggingEvents.Authentication, problemDetails.Detail);

                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails));
            }
        }

        private bool ShouldSkip(HttpContext context)
        {
            if (_options.PathSegmentsToInclude != null && _options.PathSegmentsToInclude.Count() > 0
                && !_options.PathSegmentsToInclude.Any(o => context.Request.Path.StartsWithSegments(new PathString(o))))
                return true;
            else if (_options.PathSegmentsToExclude != null 
                && _options.PathSegmentsToExclude.Any(o => context.Request.Path.StartsWithSegments(new PathString(o))))
                return true;
            else
                return false;
        }

        private bool ValidateApiKey(string apiKey)
        {
            if (!string.IsNullOrWhiteSpace(apiKey) && _options.ValidApiKeys.Contains(apiKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class APiKeyOptions : IOptions<APiKeyOptions>
    {
        /// <summary>
        /// Expected api key value
        /// </summary>
        public ICollection<string> ValidApiKeys { get; set; }
        /// <summary>
        /// Header name if api key should be passed by http request header
        /// </summary>
        public string NameInHeader { get; set; }
        /// <summary>
        /// Query parameter name if api key should be passed by query string
        /// </summary>
        public string NameInQuery { get; set; }
        /// <summary>
        /// Specify which APIs should enable this validation by their starting path segment
        /// </summary>
        public IEnumerable<string> PathSegmentsToInclude { get; set; }
        /// <summary>
        /// Specify which APIs should disable this validation by their starting path segment
        /// </summary>
        public IEnumerable<string> PathSegmentsToExclude { get; set; }

        public APiKeyOptions Value => this;
    }
}