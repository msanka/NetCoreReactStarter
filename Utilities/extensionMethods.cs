using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace native_api_proxy_module.Utilities
{
    public static class ExtensionMethods
    {
        public static T GetInstance<T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }

        public static T GetInstanceFromContext<T>(this HttpContext context)
        {
            return (T)context.RequestServices.GetService(typeof(T));
        }
    }
}