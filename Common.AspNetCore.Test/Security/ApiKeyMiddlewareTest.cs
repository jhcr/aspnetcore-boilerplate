using System;
using Xunit;
using NSubstitute;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Common.AspNetCore.Security;
using Common.Core.Logging;

namespace Common.AspNetCore.Test.Security
{
    public class ApiKeyMiddlewareTest
    {
        readonly ILogger<ApiKeyMiddleware> _loggerMock;
        readonly RequestDelegate _nextMock;
        readonly ApiKeyMiddleware _middleware;

        public ApiKeyMiddlewareTest()
        {

            _loggerMock = Substitute.For<ILogger<ApiKeyMiddleware>>();

            _nextMock = Substitute.For<RequestDelegate>();

            _middleware = new ApiKeyMiddleware(next: async (innerHttpContext) =>
            {
                await _nextMock(innerHttpContext);
            }
            ,setupAction: c => {
                c.ValidApiKeys = new List<string> { "test", "test2" };
                c.NameInHeader = "X-API-KEY";
                c.NameInQuery = "apikey";
            }
            ,logger: _loggerMock);
        }

        [Fact]
        public async void Can_Pass_When_Header_ApiKey_Correct()
        {
            var contextFake = new DefaultHttpContext();
            contextFake.Request.Headers.Add("X-API-KEY", "test");

            await _middleware.Invoke(contextFake);

            _nextMock.Received(1);
            _loggerMock.Received(0).Log<Object>(Arg.Any<LogLevel>(), Arg.Is<EventId>(x => x.Id == LoggingEvents.Authentication), Arg.Any<Object>(), Arg.Any<Exception>(), Arg.Any<Func<Object, Exception, string>>());
        }

        [Fact]
        public async void Can_Pass_When_QueryString_ApiKey_Correct()
        {
            var contextFake = new DefaultHttpContext();
            contextFake.Request.QueryString = new QueryString("?apikey=test");

            await _middleware.Invoke(contextFake);

            _nextMock.Received(1);
            _loggerMock.Received(0).Log<Object>(Arg.Any<LogLevel>(), Arg.Is<EventId>(x => x.Id == LoggingEvents.Authentication), Arg.Any<Object>(), Arg.Any<Exception>(), Arg.Any<Func<Object, Exception, string>>());
        }

        [Fact]
        public async void Can_Log_Error_When_Header_ApiKey_Wrong()
        {
            var contextFake = new DefaultHttpContext();
            contextFake.Request.Headers.Add("X-API-KEY", "WrongValue");

            await _middleware.Invoke(contextFake);

            _nextMock.Received(0);
            _loggerMock.Received(1).Log<Object>(Arg.Any<LogLevel>(), Arg.Is<EventId>(x => x.Id == LoggingEvents.Authentication), Arg.Any<Object>(), Arg.Any<Exception>(), Arg.Any<Func<Object, Exception, string>>());
        }

        [Fact]
        public async void Can_Log_Error_When_QueryString_ApiKey_Wrong()
        {
            var contextFake = new DefaultHttpContext();
            contextFake.Request.QueryString = new QueryString("?apikey=WrongValue");

            await _middleware.Invoke(contextFake);

            _nextMock.Received(0);
            _loggerMock.Received(1).Log<Object>(Arg.Any<LogLevel>(), Arg.Is<EventId>(x => x.Id == LoggingEvents.Authentication), Arg.Any<Object>(), Arg.Any<Exception>(), Arg.Any<Func<Object, Exception, string>>());
        }

        [Fact]
        public async void Can_Log_Error_When_Header_ApiKey_Name_Wrong()
        {
            var contextFake = new DefaultHttpContext();
            contextFake.Request.Headers.Add("WrongName", "test");

            await _middleware.Invoke(contextFake);

            _nextMock.Received(0);
            _loggerMock.Received(1).Log<Object>(Arg.Any<LogLevel>(), Arg.Is<EventId>(x => x.Id == LoggingEvents.Authentication), Arg.Any<Object>(), Arg.Any<Exception>(), Arg.Any<Func<Object, Exception, string>>());
        }

        [Fact]
        public async void Can_Log_Error_When_QueryString_ApiKey_Name_Wrong()
        {
            var contextFake = new DefaultHttpContext();
            contextFake.Request.QueryString = new QueryString("?wrongName=test");

            await _middleware.Invoke(contextFake);

            _nextMock.Received(0);
            _loggerMock.Received(1).Log<Object>(Arg.Any<LogLevel>(), Arg.Is<EventId>(x => x.Id == LoggingEvents.Authentication), Arg.Any<Object>(), Arg.Any<Exception>(), Arg.Any<Func<Object, Exception, string>>());
        }

        [Fact]
        public async void Can_Log_Error_When_No_ApiKey_Given()
        {
            await _middleware.Invoke(new DefaultHttpContext());

            _nextMock.Received(0);
            _loggerMock.Received(1).Log<Object>(Arg.Any<LogLevel>(), Arg.Is<EventId>(x=>x.Id == LoggingEvents.Authentication), Arg.Any<Object>(), Arg.Any<Exception>(),Arg.Any<Func<Object, Exception, string>>());
        }

        [Fact]
        public void Can_Throw_ArgumentNullException_When_Name_Option_Missing()
        {
            Func<ApiKeyMiddleware> init = () => new ApiKeyMiddleware(next: async (innerHttpContext) =>
           {
               await _nextMock(innerHttpContext);
           }
           , setupAction: c =>
           {
               c.ValidApiKeys = new List<string> { "test", "test2" };
           }
           , logger: _loggerMock);

            Assert.Throws<ArgumentNullException>(init);
        }

        [Fact]
        public async void Can_Skip_When_Path_Not_Included()
        {
            var contextFake = new DefaultHttpContext();
            contextFake.Request.Headers.Add("X-API-KEY", "WrongValue");

            contextFake.Request.Path = new PathString("/api3");

            var middlewareFake = new ApiKeyMiddleware(next: async (innerHttpContext) =>
            {
                await _nextMock(innerHttpContext);
            }
           , setupAction: c =>
           {
               c.ValidApiKeys = new List<string> { "test", "test2" };
               c.NameInHeader = "X-API-KEY";
               c.PathSegmentsToInclude = new List<string> { "/api1" };
               c.PathSegmentsToExclude = new List<string> { "/api2" };
           }
           , logger: _loggerMock);

            await middlewareFake.Invoke(contextFake);

            _nextMock.Received(1);
            _loggerMock.Received(0).Log<Object>(Arg.Any<LogLevel>(), Arg.Is<EventId>(x => x.Id == LoggingEvents.Authentication), Arg.Any<Object>(), Arg.Any<Exception>(), Arg.Any<Func<Object, Exception, string>>());

        }

        [Fact]
        public async void Can_Skip_When_Path_Excluded()
        {
            var contextFake = new DefaultHttpContext();
            contextFake.Request.Headers.Add("X-API-KEY", "WrongValue");

            contextFake.Request.Path = new PathString("/api");

            var middlewareFake = new ApiKeyMiddleware(next: async (innerHttpContext) =>
            {
                await _nextMock(innerHttpContext);
            }
           , setupAction: c =>
           {
               c.ValidApiKeys = new List<string> { "test", "test2" };
               c.NameInHeader = "X-API-KEY";
               c.PathSegmentsToExclude = new List<string> { "/api" };
           }
           , logger: _loggerMock);

            await middlewareFake.Invoke(contextFake);

            _nextMock.Received(1);
            _loggerMock.Received(0).Log<Object>(Arg.Any<LogLevel>(), Arg.Is<EventId>(x => x.Id == LoggingEvents.Authentication), Arg.Any<Object>(), Arg.Any<Exception>(), Arg.Any<Func<Object, Exception, string>>());

        }
    }
}
