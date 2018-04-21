using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace native_api_proxy_module.Utilities
{
    public interface IProxyHelper
    {
        bool isProxyRequest();
        Task processProxyRequestStreamAsync();
    }

    public class ProxyHelper : IProxyHelper
    {
        const string configPattern = @"\$Config.[A-z0-9]+\$";
        const string jwtPattern = @"\$Jwt.[A-z0-9]+\$";

        private IServiceProvider _serviceProvider;
        private IConfiguration _config;
        private IJWTHelper _JWTHelper;
        private string _proxyPath = "/api/proxy";
        private string _apiUrlQueryStringKey = "url";
        private int _bufferLengthInKB = 4;
        //runtime variables
        private string _realApiUrl= null;
        private bool _isAuthenticatedSession = false;

        public ProxyHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _config = serviceProvider.GetInstance<IConfiguration>();
            _JWTHelper = serviceProvider.GetInstance<IJWTHelper>();

            if (_config != null)
            {
                _proxyPath = _config["APIProxySettings:proxyPath"];
                _apiUrlQueryStringKey = _config["APIProxySettings:apiUrlQueryStringKey"];
                int.TryParse(_config["APIProxySettings:bufferLengthInKB"], out _bufferLengthInKB);
            }
        }

        public bool isProxyRequest()
        {
            bool _isProxyRequest = false;

            IHttpContextAccessor currentContextAccessor = _serviceProvider.GetInstance<IHttpContextAccessor>();
            if ((currentContextAccessor != null) && (currentContextAccessor.HttpContext != null))
            {
                HttpContext context = currentContextAccessor.HttpContext;
                if (context.Request.Path.Value.Equals(_proxyPath, StringComparison.OrdinalIgnoreCase))
                {
                    if (context.Request.Query.ContainsKey(_apiUrlQueryStringKey))
                    {
                        _realApiUrl = context.Request.Query[_apiUrlQueryStringKey][0];
                    }
                    _isProxyRequest = true;
                }
                _isAuthenticatedSession =context.User.Identity.IsAuthenticated;
            }

            return _isProxyRequest;
        }

        private bool hasTokens(string stringWithTokens)
        {
            bool _hasTokens = false;

            MatchCollection matchedConfigTokens = Regex.Matches(stringWithTokens, configPattern, RegexOptions.None);
            MatchCollection matchedJwtTokens = Regex.Matches(stringWithTokens, jwtPattern, RegexOptions.None);

            if (matchedConfigTokens.Count > 0 || matchedJwtTokens.Count > 0)
                _hasTokens = true;

            return _hasTokens;
        }

        private string resolveTokens(string stringWithTokens)
        {
            string resolvedString = stringWithTokens;

            #region Resolve From Config
            MatchCollection matchedConfigTokens = Regex.Matches(resolvedString, configPattern, RegexOptions.None);

            foreach (Match match in matchedConfigTokens)
            {
                string token = match.Value.Replace("$Config.","").Replace("$","");
                string tokenValue = _config["APIProxySettings:clientSettings:" + token];
                Console.WriteLine("token : {0} value : {1}", match.Value, tokenValue);
                resolvedString = resolvedString.Replace(match.Value, tokenValue);
            }
         
            #endregion

            #region Resolve From JWT
            MatchCollection matchedJwtTokens = Regex.Matches(resolvedString, jwtPattern, RegexOptions.None);

            if (!_isAuthenticatedSession)
            {
                throw new Exception("Request needs an authenticated session for token replacement");
            }
            else
            {
                foreach (Match match in matchedJwtTokens)
                {
                    string token = match.Value.Replace("$Jwt.","").Replace("$","");

                    Claim requestedClaim = _JWTHelper.getClaim(token);
                    if (requestedClaim != null)
                    {
                        string tokenValue =  requestedClaim.Value;
                        Console.WriteLine("token : {0} value : {1}", match.Value, tokenValue);
                        resolvedString = resolvedString.Replace(match.Value, tokenValue);
                    }
                    else
                    {
                        throw new Exception(string.Format("Request needs an authenticated session with valid claim name for token replacement. Claim name {0}", token));
                    }
                }
            }

            #endregion

            return resolvedString;
        }

        private HttpContext getResolvedContext(HttpContext current)
        {
            HttpContext _context = current;
            
            #region Resolve Tokens in Headers
            List<string> _headersWithTokens = new List<string>();
            _context.Request.Headers.ToList().ForEach(header =>
            {
                if(hasTokens(header.Key) || hasTokens(header.Value))
                {
                    _headersWithTokens.Add(header.Key);
                }
            });

            List<KeyValuePair<string,string>> newHeaders = new List<KeyValuePair<string,string>>();
            _headersWithTokens.ForEach(key => 
            {
                string value = _context.Request.Headers[key];
                string newKey = hasTokens(key) ? resolveTokens(key) : key;
                string newValue = hasTokens(value) ? resolveTokens(value) : value;
                newHeaders.Add(new KeyValuePair<string, string>(newKey,newValue));
                _context.Request.Headers.Remove(key);
            });

            newHeaders.ForEach(newHeader => { _context.Request.Headers.Add(newHeader.Key,newHeader.Value); });

            #endregion

            #region Resolve Tokens in Body
            
            #endregion

            return _context;
        }

        public async Task processProxyRequestStreamAsync()
        {
            HttpContext context = null;

            IHttpContextAccessor currentContextAccessor = _serviceProvider.GetInstance<IHttpContextAccessor>();
            if ((currentContextAccessor != null) && (currentContextAccessor.HttpContext != null))
            {
                //Resolve Tokens in Headers, Body
                 context = getResolvedContext(currentContextAccessor.HttpContext);
            }

            string resolvedRealApiUrl = resolveTokens(_realApiUrl);

            var httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };

            var webRequest = new HttpClient(httpClientHandler);

            var buffer = new byte[_bufferLengthInKB * 1024];
            var localResponse = context.Response;
            try
            {
                using (var remoteStream = await webRequest.GetStreamAsync(resolvedRealApiUrl))
                {
                    var bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                    localResponse.Clear();
                    
                    while (bytesRead > 0) // && localResponse.IsClientConnected)
                    {
                        await localResponse.Body.WriteAsync(buffer, 0, bytesRead);
                        bytesRead = remoteStream.Read(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                // Do some logging here
                Console.Write(ex.Message);
            }
        }
    }
}