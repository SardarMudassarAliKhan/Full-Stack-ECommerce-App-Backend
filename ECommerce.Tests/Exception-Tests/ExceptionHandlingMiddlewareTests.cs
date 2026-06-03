using ECommerce.API.Middleware;
using ECommerce.Application.Common;
using ECommerce.Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;
using Xunit;

namespace ECommerce.Tests.Exception_Tests
{
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
        private readonly Mock<IHostEnvironment> _envMock;

        public ExceptionHandlingMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            _envMock = new Mock<IHostEnvironment>();
            _envMock.Setup(e => e.EnvironmentName).Returns("Production");
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnNotFound_WhenNotFoundExceptionThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var exception = new NotFoundException("Resource not found");
            RequestDelegate next = (ctx) => throw exception;

            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object, _envMock.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            context.Response.ContentType.Should().Contain("application/json");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            errorResponse.Should().NotBeNull();
            errorResponse!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            errorResponse.Message.Should().Be("Resource not found");
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnBadRequest_WhenBadRequestExceptionThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var exception = new BadRequestException("Invalid input");
            RequestDelegate next = (ctx) => throw exception;

            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object, _envMock.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            errorResponse.Should().NotBeNull();
            errorResponse!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            errorResponse.Message.Should().Be("Invalid input");
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnInternalServerError_WhenGenericExceptionThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var exception = new Exception("Something went wrong");
            RequestDelegate next = (ctx) => throw exception;

            var middleware = new ExceptionHandlingMiddleware(next, _loggerMock.Object, _envMock.Object);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            errorResponse.Should().NotBeNull();
            errorResponse!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            errorResponse.Message.Should().Be("An internal server error has occurred.");
            errorResponse.Details.Should().BeNull(); // Production env
        }
    }
}
