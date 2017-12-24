using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.IO;

namespace Shay.Core.Web
{
    public static class HttpContext
    {
        private static IHttpContextAccessor _accessor;

        public static Microsoft.AspNetCore.Http.HttpContext Current => _accessor.HttpContext;

        /// <summary>
        /// 客户端IP
        /// </summary>
        public static string RemoteIpAddress
        {
            get
            {
                //获取代理IP
                if (Current.Request.Headers.TryGetValue("HTTP_X_FORWARDED_FOR", out StringValues addr) && !string.IsNullOrWhiteSpace(addr))
                    return addr;
                //获取真实IP
                if (Current.Request.Headers.TryGetValue("X_REAL_IP", out addr) && !string.IsNullOrWhiteSpace(addr))
                    return addr;
                //获取客户端IP
                if (Current.Request.Headers.TryGetValue("REMOTE_ADDR", out addr) && !string.IsNullOrWhiteSpace(addr))
                    return addr;
                return Current.Connection.RemoteIpAddress.ToString();
            }
        }
        /// <summary>
        /// 本地IP
        /// </summary>
        public static string LocalIpAddress
        {
            get
            {
                return Current.Connection.LocalIpAddress.ToString();
            }
        }
        /// <summary>
        /// 请求类型
        /// </summary>
        public static string RequestType => Current.Request.Method;

        /// <summary>
        /// 表单
        /// </summary>
        public static IFormCollection Form => Current.Request.Form;
        /// <summary>
        /// 请求体
        /// </summary>
        public static Stream Body => Current.Request.Body;

        /// <summary>
        /// 用户代理
        /// </summary>
        public static string UserAgent => Current.Request.Headers["User-Agent"];

        /// <summary>
        /// 内容类型
        /// </summary>
        public static string ContentType => Current.Request.ContentType;

        /// <summary>
        /// 参数
        /// </summary>
        public static string QueryString => Current.Request.QueryString.ToString();

        internal static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }

    public static class StaticHttpContextExtensions
    {
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            HttpContext.Configure(httpContextAccessor);
            return app;
        }
    }
}
