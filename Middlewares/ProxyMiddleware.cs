using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using native_api_proxy_module.Utilities;
using System.Net.Http;

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
                }
            }
        }
    }
}