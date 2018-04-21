using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using native_api_proxy_module.Utilities;

namespace native_api_proxy_module.Middlewares
{
    public class HeaderPreRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderPreRequestMiddleware(RequestDelegate next)
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

                string cookie = _cookieHelper.GetCookie(_cookieHelper.GetCookieName());

                // Get JWT from request header.
                string hdrJWT = _JWTHelper.getBearerHeaderValue(headers);

                // If header doesn't have a JWT but the cookie does, add the cookie's JWT to the header.
                if (String.IsNullOrEmpty(hdrJWT) && !String.IsNullOrEmpty(cookie))
                {
                    string decryptedJWTToken = _cryptoHelper.decrypt(cookie);
                    _JWTHelper.setClaimsFromCookie(decryptedJWTToken);
                    string bearerToken = string.Format("Bearer {0}", cookie);
                    headers.SetCommaSeparatedValues("Authorization", bearerToken);
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