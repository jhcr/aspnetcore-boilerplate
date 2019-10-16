using System;
using Xunit;
using NSubstitute;
using Microsoft.Extensions.Logging;
using Common.AspNetCore.Logging;
using Microsoft.AspNetCore.Http;

namespace Common.AspNetCore.Test.Logging
{
    public class CorrelationMiddlewareTest
    {
        readonly ILogger<CorrelationIdMiddleware> _loggerMock;
        readonly RequestDelegate _nextMock;
        readonly CorrelationIdMiddleware _middleware;
        readonly IHttpContextAccessor _accessorMock;

        public CorrelationMiddlewareTest()
        {

            _loggerMock = Substitute.For<ILogger<CorrelationIdMiddleware>>();

            _nextMock = Substitute.For<RequestDelegate>();

            _accessorMock = Substitute.For<IHttpContextAccessor>();

            _middleware = new CorrelationIdMiddleware(
                next: _nextMock, 
                setupAction: c => 
                {
                    c.Header = "testKey";
                });
        }

        [Fact]
        public async void Can_Response_Return_Same_Correlation_Id_When_Request_Header_Have_Correlation_Id()
        {
            var contextMock = new DefaultHttpContext();
            contextMock.Request.Headers.Add("testKey", "testValue");

            await _middleware.Invoke(contextMock);

            Assert.True(contextMock.Items.ContainsKey("CorrelationId"));
            Assert.Equal("testValue", contextMock.Items["CorrelationId"]?.ToString());

            _nextMock.Received(1);
        }

        [Fact]
        public async void Can_Response_Return_New_Correlation_Id_When_Request_Header_Not_Have_Correlation_Id()
        {
            var contextMock = new DefaultHttpContext();

            await _middleware.Invoke(contextMock);

            Assert.True(contextMock.Items.ContainsKey("CorrelationId"));
            Assert.True(Guid.TryParse(contextMock.Items["CorrelationId"]?.ToString(),out var guid));

            _nextMock.Received(1);
        }
        
        [Fact]
        public void Can_Throw_ArgumentNullException_When_Header_Option_Missing()
        {
            Func<CorrelationIdMiddleware> init = () => new CorrelationIdMiddleware(
                next: _nextMock,
                setupAction: c =>
                {
                    c.Header = "";
                });

            Assert.Throws<ArgumentNullException>(init);
        }

        [Fact]
        public async void Can_Default_Header_Used_When_Not_Overriden()
        {
            var middlewareFake = new CorrelationIdMiddleware(
                next: _nextMock,
                setupAction: null);

            var contextMock = new DefaultHttpContext();

            await middlewareFake.Invoke(contextMock);

            Assert.True(contextMock.Items.ContainsKey("CorrelationId"));
            Assert.True(Guid.TryParse(contextMock.Items["CorrelationId"]?.ToString(), out var guid));

            _nextMock.Received(1);
        }
    }
}
