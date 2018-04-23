using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using native_api_proxy_module.Utilities;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

namespace native_api_proxy_module.Middlewares
{
    //Credit : http://overengineer.net/creating-a-simple-proxy-server-middleware-in-asp-net-core
    public class ProxyMiddleware
    {
        private readonly RequestDelegate _next;

        public ProxyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            IProxyHelper _proxyHelper = context.GetInstanceFromContext<IProxyHelper>();

            if (!_proxyHelper.isProxyRequest())
            {
                await _next(context);
            }
            else
            {
                try
                {
                    await _proxyHelper.processProxyRequestStreamAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await HandleExceptionAsync(context, ex);
                }
            }
        }

        //Courtesy : https://stackoverflow.com/questions/38630076/asp-net-core-web-api-exception-handling
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if      (exception is FileNotFoundException)     code = HttpStatusCode.NotFound;
            else if (exception is UnauthorizedAccessException) code = HttpStatusCode.Unauthorized;
            
            var result = JsonConvert.SerializeObject(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}