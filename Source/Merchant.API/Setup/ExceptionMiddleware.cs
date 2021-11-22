using Merchant.CORE.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Merchant.API.Setup
{
    public class ExceptionMiddleware 
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception is UIException)
            {
                return PrepareErrorResponse(context, HttpStatusCode.InternalServerError, exception.Message);
            }
            if (exception is UIValidationException)
            {
                return PrepareErrorResponse(context, HttpStatusCode.NotAcceptable, exception.Message);
            }

            return PrepareErrorResponse(context, HttpStatusCode.InternalServerError, exception.Message);
        }

        private async Task PrepareErrorResponse(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                statusCode = context.Response.StatusCode,
                message = message
            }));
        }
    }
}
