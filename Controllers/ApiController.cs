using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using native_api_proxy_module.Models;
using native_api_proxy_module.Utilities;

namespace native_api_proxy_module.Controllers
{
    [Authorize]
    public class ApiController : Controller
    {
        private IConfiguration _configuration;
        private ICookieHelper _cookieHelper;
        private IJWTHelper _JWTHelper;
        private ICryptoHelper _cryptoHelper;

        public ApiController(IConfiguration configuration, ICookieHelper cookieHelper, IJWTHelper JWTHelper, ICryptoHelper cryptoHelper)
        {
            _configuration = configuration;
            _cookieHelper = cookieHelper;
            _JWTHelper = JWTHelper;
            _cryptoHelper = cryptoHelper;
        }

        [AllowAnonymous]
        [HttpPost("api/security/authenticate")]
        public IActionResult AuthenticateUser([FromBody]LoginViewModel login)
        {
            IActionResult response = BadRequest(new { message = "Authentication failed." } );
            var user = Authenticate(login);

            if (user != null)
            {
                var claims = new[] 
                {
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                    new Claim(JwtRegisteredClaimNames.GivenName, user.Name),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var tokenString = _JWTHelper.createJWTToken(claims.ToList());
                var encryptedToken = _cryptoHelper.encrypt(tokenString);

                string _cookieName = _cookieHelper.GetCookieName();
                _cookieHelper.WriteCookie(_cookieName, encryptedToken);

                response = Ok(new { token = encryptedToken, redirectUrl = _configuration["AppSettings:homePageUrl"] });
            }

            return response;
        }

        [AllowAnonymous]
        [HttpGet("api/security/logout")]
        public IActionResult Logout()
        {
            IActionResult response = Unauthorized();
           
            string _cookieName = _cookieHelper.GetCookieName();
            _cookieHelper.DeleteCookie(_cookieName);

            response = Ok(new { redirectUrl = _configuration["AppSettings:loginUrl"] });
           
            return response;
        }

        private UserModel Authenticate(LoginViewModel login)
        {
            UserModel user = null;

            if (login != null)
            {
                if (login.UserName == "nat" && login.Password == "secret")
                {
                    user = new UserModel { Id = "123", Name = "Natarajan Sivasubramanian", Email = "nat.siva@domain.com"};
                }
            }

            return user;
        }
        
    }
}
