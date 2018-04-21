using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace native_api_proxy_module.Utilities
{
    public interface IJWTHelper
    {
        string createJWTToken(List<Claim> claims);
        string updateJWTToken();
        string updateJWTToken(List<Claim> newClaims);
        string getBearerHeaderValue(IHeaderDictionary headers);
        void setClaimsFromCookie(string jwt);
        Claim getClaim(string claimName);
    }

    public class JWTHelper : IJWTHelper
    {
        private IConfiguration _config;
        private string _issuer = "localhost:5000";
        private string _audience = "localhost:5000";
        private int _expiration = 20;
        private string _signingSymmetricKey = "secretKey";
        private List<Claim> _claimsFromCookie = new List<Claim>();

        public JWTHelper(IServiceProvider serviceProvider)
        {
            _config = serviceProvider.GetInstance<IConfiguration>(); 

            if (_config != null)
            {
                _issuer = _config["Jwt:Issuer"];
                _audience = _config["Jwt:Audience"];
                _expiration = int.Parse(_config["Jwt:ExpiryMinutes"]);
                _signingSymmetricKey = _config["Jwt:Key"];
            }
        }

        public void setClaimsFromCookie(string jwt)
        {
             var handler = new JwtSecurityTokenHandler();
             var jsonToken = handler.ReadToken(jwt) as JwtSecurityToken;
             this._claimsFromCookie = jsonToken.Claims.ToList();
        }

        /// <summary>
        /// Gets the bearer token's value (a JWT) from the Http request's authorization header.
        /// </summary>
        /// <param name="headers">The Http request's header collection</param>
        /// <returns>The value of the bearer token if it exists. Null otherwise.</returns>
        public string getBearerHeaderValue(IHeaderDictionary headers)
        {
            if (headers == null || headers.Count == 0)
            {
                return null;
            }

           string hdr = (from h in headers
                          where h.Key.ToLower() == "authorization" && h.Value.ToString().ToLower().Contains("bearer")
                          select h.Value.ToString()).FirstOrDefault();

            var hdrVal = String.IsNullOrEmpty(hdr) ? String.Empty : hdr.Replace(" ", "").Substring(6);
            return hdrVal;
        }

        public string createJWTToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingSymmetricKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                claims.ToArray(),
                expires: DateTime.Now.AddMinutes(_expiration),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Claim getClaim(string claimName)
        {
            Claim _claim = null;
            
            _claim = _claimsFromCookie.Where(c => c.Type.ToLower() == claimName.ToLower()).FirstOrDefault();

            return _claim;
        }

        public string updateJWTToken()
        {
            return updateJWTToken(null);
        }
        public string updateJWTToken(List<Claim> newClaims = null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingSymmetricKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>();

            if ((_claimsFromCookie != null) && (_claimsFromCookie.Count > 0))
            {
                claims.AddRange(_claimsFromCookie);
            }

            if (newClaims != null)
            {
                List<string> newClaimNames = newClaims.Select(c => c.Type.ToLower()).ToList();
                claims.RemoveAll(c => newClaimNames.Contains(c.Type.ToLower()));
                claims.AddRange(newClaims);
            }

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                claims.ToArray(),
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}