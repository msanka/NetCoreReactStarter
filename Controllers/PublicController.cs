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
    [AllowAnonymous]
    public class PublicController : Controller
    {
        private IConfiguration _configuration;

        public PublicController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index(string appRoutes = null)
        {
            IActionResult result = View();

            if (appRoutes == null)
            {
                //Response.Redirect Vs Redirect action : https://stackoverflow.com/questions/21043315/i-cant-resolve-unhandeld-exception-cannot-set-status-after-http-headers-alread
                result = Redirect(_configuration["AppSettings:loginUrl"]);
            }

            return result;
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
