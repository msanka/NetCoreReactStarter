using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using native_api_proxy_module.Utilities;

namespace native_api_proxy_module.Middlewares
{
    public class HeaderPostRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderPostRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                IHeaderDictionary headers = context.Request.Headers;
                ICookieHelper _cookieHelper = context.GetInstanceFromContext<ICookieHelper>();
                IJWTHelper _JWTHelper = context.GetInstanceFromContext<IJWTHelper>();
                ICryptoHelper _cryptoHelper = context.GetInstanceFromContext<ICryptoHelper>();
                
                string cookieName = _cookieHelper.GetCookieName(); 
                if (!_cookieHelper.isCookieDeleted(cookieName))
                {
                    string updatedToken = _JWTHelper.updateJWTToken();
                    string encryptedToken = _cryptoHelper.encrypt(updatedToken);
                    _cookieHelper.UpdateCookie(encryptedToken);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await _next.Invoke(context);
            }
        }
    }
}