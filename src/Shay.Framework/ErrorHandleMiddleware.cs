using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Shay.Core;
using Shay.Core.Exceptions;
using Shay.Core.Logging;
using Shay.Core.Serialize;
using System;
using System.Threading.Tasks;

namespace Shay.Framework
{
    public class ErrorHandleMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var statusCode = context.Response.StatusCode;
                if (ex is BusiException)
                {
                    statusCode = StatusCodes.Status200OK;
                    var busiEx = ex as BusiException;
                    await HandleExceptionAsync(context, busiEx.Code, busiEx.Message);
                    return;
                }
                LogManager.Logger(typeof(ErrorHandleMiddleware)).Error(ex.Message, ex);
                await HandleExceptionAsync(context, statusCode, "服务器异常,请稍候重试!");
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, int statusCode, string msg)
        {
            var result = new DResult(msg, statusCode);
            var resultData = JsonHelper.ToJson(result);
            context.Response.ContentType = "application/json;charset=utf-8";
            return context.Response.WriteAsync(resultData);
        }
    }
    public static class ErrorHandleExtensions
    {
        /// <summary> 使用全局异常处理中间件 </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseErrorHanleMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandleMiddleware>();
        }
    }
}
