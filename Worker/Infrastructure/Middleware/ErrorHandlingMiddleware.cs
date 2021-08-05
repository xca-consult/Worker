using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Worker.Infrastructure.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                await HandleException(context, ex);
            }
        }

        private static Task HandleException(HttpContext context, Exception ex)
        {
            var code = GetHttpStatusCode(ex);

            ProblemDetails details = new ProblemDetails 
            {
                Detail = ex.Message, 
                Title = "An error occurred", 
                Type = ex.GetType().Name, 
                Status = code, 
                Instance = context.Request.Path.Value 
            };

            string result = JsonConvert.SerializeObject(details);
            context.Response.StatusCode = code;
            context.Response.ContentType = "application/json";

            return context.Response.WriteAsync(result);
        }

        private static int GetHttpStatusCode(Exception ex)
        {
            HttpStatusCode code = ex switch
            {
                ArgumentException _ => HttpStatusCode.BadRequest,
                NotFoundException _ => HttpStatusCode.NotFound,
                InvalidOperationException _ => HttpStatusCode.Conflict,
                _ => HttpStatusCode.InternalServerError
            };

            return (int)code;
        }
    }
  }
