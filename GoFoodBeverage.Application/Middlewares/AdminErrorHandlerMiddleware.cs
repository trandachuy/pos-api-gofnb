using System;
using System.Net;
using System.Web;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using GoFoodBeverage.Common.Exceptions.ErrorModel;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Middlewares
{
    public class AdminErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<AdminErrorHandlingMiddleware> _logger;

        public AdminErrorHandlingMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnvironment,
            ILogger<AdminErrorHandlingMiddleware> logger)
        {
            _next = next;
            _hostingEnvironment = hostingEnvironment;
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
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex, _hostingEnvironment, false);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, IWebHostEnvironment hostingEnvironment, bool htmlEncode)
        {
            var httpStatusCode = HttpStatusCode.InternalServerError; // 500 if unexpected
            string message = string.Empty;
            List<ErrorItemModel> errors = new();
            var keys = exception.Data.Keys;
            foreach (var key in keys)
            {
                errors.Add(new ErrorItemModel()
                {
                    Type = key.ToString(),
                    Message = exception.Data[key].ToString()
                });
            }
            
            string stackTrace = string.Empty;
            //if (hostingEnvironment.IsDevelopment())
            //{
                stackTrace = exception.StackTrace;
            //}

            if (exception is HttpStatusCodeException statusCodeException)
            {
                httpStatusCode = statusCodeException.HttpStatusCode;
                message = statusCodeException.Message;
                errors = statusCodeException.Errors;
            }

            if (message.IsBlank())
            {
                message = exception?.Message;
            }

            if (errors != null && htmlEncode)
            {
                errors = HtmlEncode(errors);
            }

            var result = JsonConvert.SerializeObject(
                new ErrorModel
                {
                    Error = "An error occurred.",
                    ErrorTime = DateTime.UtcNow,
                    Message = htmlEncode ? HttpUtility.HtmlEncode(message) : message,
                    StackTrace = stackTrace,
                    Errors = errors,
                },
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpStatusCode;
            return context.Response.WriteAsync(result);
        }

        private static List<ErrorItemModel> HtmlEncode(List<ErrorItemModel> errors)
        {
            return errors.Select(err =>
                new ErrorItemModel
                {
                    Type = HttpUtility.HtmlEncode(err.Type),
                    Message = HttpUtility.HtmlEncode(err.Message)
                }).ToList();
        }
    }
}
