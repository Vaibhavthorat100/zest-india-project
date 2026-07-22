using System.Net;
using System.Text.Json;
using StudentManagementAPI.Common.Exceptions;
using StudentManagementAPI.DTOs;

namespace StudentManagementAPI.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception captured. TraceId: {TraceId}, Endpoint: {Path}", 
                    context.TraceIdentifier, context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected server error occurred. Please try again later.";
            List<string>? errors = null;

            switch (exception)
            {
                case NotFoundException notFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundEx.Message;
                    break;
                case ConflictException conflictEx:
                    statusCode = HttpStatusCode.Conflict;
                    message = conflictEx.Message;
                    break;
                case BadRequestException badRequestEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = badRequestEx.Message;
                    break;
                case UnauthorizedException unauthEx:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = unauthEx.Message;
                    break;
                case InvalidOperationException invalidOpEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = invalidOpEx.Message;
                    break;
                default:
                    errors = new List<string> { exception.Message };
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.FailureResponse(message, errors);
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonResult = JsonSerializer.Serialize(response, jsonOptions);

            return context.Response.WriteAsync(jsonResult);
        }
    }
}
