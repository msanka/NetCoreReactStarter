using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using native_api_proxy_module.Models;

namespace native_api_proxy_module.Controllers
{
    [Authorize]
    public class ProtectedController : Controller
    {
        private IConfiguration _configuration;

        public ProtectedController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index(string appRoutes = null)
        {
            IActionResult result = View();

            if (appRoutes == null)
            {
                result = Redirect(_configuration["AppSettings:homePageUrl"]);
            }

            return result;
        }
    }
}
