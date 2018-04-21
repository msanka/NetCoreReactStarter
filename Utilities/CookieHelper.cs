using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace native_api_proxy_module.Utilities
{
    public interface ICookieHelper
    {
        void WriteCookie(string cookieName, string value, int expiryMinutes, string path);
        void WriteCookie(string cookieName, string value);
        void UpdateCookie(string value);
        void DeleteCookie(string cookieName);
        string GetCookieName();
        string GetCookie(string cookieName);

        bool isCookieDeleted(string cookieName);
    }

    public class CookieHelper : ICookieHelper
    {
        private IServiceProvider _serviceProvider;
        private IConfiguration _config;
        private int _expiryMinutes = 20;
        private string _cookieName = "jwt_cookie";
        private int _cookieExpirationOffset = 1;//Number of minutes to decrement the expiration time of a cookie by
        private ChunkingCookieManager _chunkingCookieManager;

        public CookieHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _config = _serviceProvider.GetInstance<IConfiguration>(); 

            if (_config != null)
            {
                _cookieName = _config["Cookie:Name"];
                _expiryMinutes = int.Parse(_config["Cookie:ExpiryMinutes"]);//TimeOut
            }

            _chunkingCookieManager = new ChunkingCookieManager();
            _chunkingCookieManager.ChunkSize = 1000;
        }

        public bool isCookieDeleted(string cookieName)
        {
            string cookie = GetCookie(cookieName);

            return string.IsNullOrWhiteSpace(cookie);
        }

        public string GetCookieName()
        { 
            return _cookieName;
        }

        public void UpdateCookie(string value)
        {
            WriteCookie(_cookieName, value, 0, "/");
        }

        public void WriteCookie(string cookieName, string value)
        {
            WriteCookie(cookieName, value, 0, "/");
        }

        /// <summary>
        /// Stores a value across mulitple cookies using chunkedCookieManager.
        /// </summary>        
        /// <param name="cookieName">The cookie key to write. If null, default cookieName in app.config will be used</param>
        /// <param name="value">The value to store</param>
        /// <param name="expiryMinutes">Optional. Number of minutes the cookie should exist for</param>
        /// <param name="path">Optional. Path for the cookie</param>
        public void WriteCookie(string cookieName, string value, int expiryMinutes = 0, string path = "/")
        {
            try
            {
                expiryMinutes = expiryMinutes == 0 ? _expiryMinutes : expiryMinutes;
                string key = String.IsNullOrEmpty((cookieName ?? String.Empty).Trim()) ? _cookieName : cookieName.Trim();

                CookieOptions options = new CookieOptions()
                {
                    Expires = DateTime.UtcNow.AddMinutes(expiryMinutes - _cookieExpirationOffset),
                    Path = path,       //make available to all pages
                    HttpOnly = true,
                    Secure = true
                };

                if (String.Equals("Development", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), StringComparison.OrdinalIgnoreCase))
                {
                    options.Secure = false;    //allow cookie to be served/received over HTTP (vs HTTPs)
                }

                IHttpContextAccessor currentContextAccessor = _serviceProvider.GetInstance<IHttpContextAccessor>();
                if ((currentContextAccessor != null) && (currentContextAccessor.HttpContext != null))
                {
                    _chunkingCookieManager.DeleteCookie(currentContextAccessor.HttpContext, key, options);
                    _chunkingCookieManager.AppendResponseCookie(currentContextAccessor.HttpContext, key, value, options);
                }
            }
            catch (Exception ex)
            {
                string setBreakPointHere = ex.Message;
            }
        }

        /// <summary>
        /// Delete all matching chunked cookies using chunkingCookieManager.
        /// </summary>        
        /// <param name="cookieName">The cookie to delete. If null, default cookieName in app.config will be used</param>        
        public void DeleteCookie(string cookieName)
        {
            try
            {
                string key = String.IsNullOrEmpty(cookieName) ? _cookieName : cookieName;

                CookieOptions options = new CookieOptions()
                {
                    Expires = DateTime.UtcNow.AddDays(-1),
                    Path = "/",       //make available to all pages
                    HttpOnly = true,
                    Secure = true
                };

                if (String.Equals("Development", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), StringComparison.OrdinalIgnoreCase))
                {
                    options.Secure = false;    //allow cookie to be served/received over HTTP (vs HTTPs)
                }

                IHttpContextAccessor currentContextAccessor = _serviceProvider.GetInstance<IHttpContextAccessor>();
                if ((currentContextAccessor != null) && (currentContextAccessor.HttpContext != null))
                {
                    _chunkingCookieManager.DeleteCookie(currentContextAccessor.HttpContext, key, options);
                }
            }
            catch (Exception ex)
            {
                string setBreakPointHere = ex.Message;
            }
        }


        /// <summary>
        /// Gets the reassembled cookie using chunkingCookieManager.
        /// </summary>
        /// <param name="cookieName">The cookie to get. If null, default cookieName in app.config will be used</param>
        /// <returns>The value of the cookie or an empty string</returns>
        public string GetCookie(string cookieName)
        {
            try
            {
                string cookie = string.Empty;
                string key = String.IsNullOrEmpty(cookieName) ? _cookieName : cookieName;

                IHttpContextAccessor currentContextAccessor = _serviceProvider.GetInstance<IHttpContextAccessor>();
                if ((currentContextAccessor != null) && (currentContextAccessor.HttpContext != null))
                {
                    cookie = _chunkingCookieManager.GetRequestCookie(currentContextAccessor.HttpContext, key);
                }
                
                return cookie;
            }
            catch (Exception ex)
            {
                string setBreakPointHere = ex.Message;
                return String.Empty;
            }
        }
    }
}