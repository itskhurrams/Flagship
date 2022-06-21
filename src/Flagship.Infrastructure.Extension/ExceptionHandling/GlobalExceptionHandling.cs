using Flagship.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flagship.Infrastructure.Extension.ExceptionHandling {
    public class GlobalExceptionHandling {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandling> _logger;
        public GlobalExceptionHandling(RequestDelegate next, ILogger<GlobalExceptionHandling> logger) {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext) {
            try {
                await _next(httpContext);
            }
            catch (AccessViolationException avEx) {
                string message = $"A new violation exception has been thrown: {avEx}";
                _logger.LogError(message);
                await HandleExceptionAsync(httpContext, avEx);
            }
            catch (Exception ex) {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception) {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorResponse = new ExceptionInformation(response.StatusCode, response.Body.ToString(), response.ToString());
            switch (exception) {
                case DbException ex:
                    if (ex.Message.Contains("connection")) {
                        errorResponse = new ExceptionInformation((int)HttpStatusCode.ServiceUnavailable, ex.Message, ex.Data.ToString());
                        break;
                    }
                    errorResponse = new ExceptionInformation((int)HttpStatusCode.BadRequest, ex.Message, ex.Data.ToString());
                    break;
                case ApplicationException ex:
                    if (ex.Message.Contains("Invalid token")) {
                        errorResponse = new ExceptionInformation((int)HttpStatusCode.Forbidden, ex.Message, ex.Data.ToString());
                        break;
                    }
                    errorResponse = new ExceptionInformation((int)HttpStatusCode.BadRequest, ex.Message, ex.Data.ToString());
                    break;
                case AccessViolationException ex:
                    if (ex.Message.Contains("Access Violation")) {
                        errorResponse = new ExceptionInformation((int)HttpStatusCode.Forbidden, ex.Message, ex.Data.ToString());
                        break;
                    }

                    errorResponse = new ExceptionInformation((int)HttpStatusCode.BadRequest, ex.Message, ex.Data.ToString());
                    break;
                case KeyNotFoundException ex:
                    errorResponse = new ExceptionInformation((int)HttpStatusCode.NotFound, ex.Message, ex.Data.ToString());
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "Internal Server errors. Check Logs!";
                    errorResponse = new ExceptionInformation((int)HttpStatusCode.InternalServerError, "Internal Server errors. Check Logs!", "Either Server is not working or stopped.");

                    break;
            }
            _logger.LogError(exception.Message);
            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}
