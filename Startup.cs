using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using native_api_proxy_module.Middlewares;
using native_api_proxy_module.Utilities;

namespace native_api_proxy_module
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["Jwt:Issuer"],
                            ValidAudience = Configuration["Jwt:Issuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                        };

                        options.Events = new JwtBearerEvents
                        {
                            // Decrypt the JWT so it can be processed by Authorize logic.
                            OnMessageReceived = context =>
                            {
                                string jwt = String.Empty;
                                string hdrJWT = String.Empty;
                                ICryptoHelper _cryptoHelper = context.HttpContext.GetInstanceFromContext<ICryptoHelper>();
                                IJWTHelper _JWTHelper = context.HttpContext.GetInstanceFromContext<IJWTHelper>();
                                
                                try
                                {
                                    hdrJWT = _JWTHelper.getBearerHeaderValue(context.Request.Headers);
                                    jwt = _cryptoHelper.decrypt(hdrJWT);
                                    context.Token = jwt;
                                }
                                catch (Exception ex)
                                {
                                    string msg = $"ERROR: Missing or invalid JWT. EXCEPTION: {ex.Message} JWT: {hdrJWT} DECRYPTED: {jwt}";
                                    Console.WriteLine("USER AUTHENTICATION", "Startup.cs", "ConfigureServices.OnMessageReceived", msg, ex);
                                }
                                return Task.FromResult(0);
                            },

                            // On valid token received, this fires. Useful for debugging purposes.
                            OnTokenValidated = context =>
                            {
                                string plainTextJWT = context.SecurityToken.ToString();
                                return Task.FromResult(0);
                            },

                            // On authentication failures, redirect to the public login page
                            OnChallenge = context =>
                            {
                                ICryptoHelper _cryptoHelper = context.HttpContext.GetInstanceFromContext<ICryptoHelper>();
                                IJWTHelper _JWTHelper = context.HttpContext.GetInstanceFromContext<IJWTHelper>();

                                // If no path was requested, get the default landing page. Otherwise
                                // use the requested path ensuring the correct context root is included.
                                PathString currenturi = context.Request.Path == null
                                    ? new PathString(Configuration["AppSettings:homePageUrl"])
                                    : new PathString(Configuration["AppSettings:appRootPath"] + context.Request.Path);

                                // Pass the originally requested page as a querystring parm preserving its querystring if present
                                string q = context.HttpContext.Request.QueryString.HasValue
                                    ? WebUtility.UrlEncode(context.HttpContext.Request.QueryString.ToString())
                                    : String.Empty;

                                string redirectUrl = Configuration["AppSettings:loginUrl"] + QueryString.Create("ReturnUrl", currenturi) + q;
                                
                                // Save the originally requested page in a cookie
                                context.Response.Cookies.Append("requestedurl", currenturi);
                                context.Response.Redirect(redirectUrl);
                                context.HandleResponse();

                                string hdrJWT = string.Empty;
                                string jwtDetail = string.Empty;
                                try
                                {
                                    hdrJWT = _JWTHelper.getBearerHeaderValue(context.Request.Headers);
                                    if (!String.IsNullOrEmpty(hdrJWT))
                                    {
                                        string format = "yyyy-MM-dd HH:mm:ss:fffffff";
                                        string currentUTC = DateTime.UtcNow.ToString(format);
                                        string detailMsg = context.AuthenticateFailure.Message;
                                        string jwtEXP = _JWTHelper.getClaim(JwtRegisteredClaimNames.Exp.ToString()).Value;
                                        string jwtExpAsDate = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(Convert.ToDouble(jwtEXP)).ToString(format);
                                        jwtDetail = $"JWT Expires: {jwtExpAsDate} JWT EXP: {jwtEXP} Current Time: {currentUTC} Auth Error: {detailMsg} ";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string setBreakPointHere = ex.Message;
                                }

                                string msg = String.IsNullOrEmpty(context.ErrorDescription) ? String.Empty : $"EXCEPTION: {context.Error} - {context.ErrorDescription}";
                                string errorMsg = $"ERROR: Missing or invalid JWT. {msg} JWT: {hdrJWT} DETAIL: {jwtDetail}";
                                Console.WriteLine("USER AUTHENTICATION", "Startup.cs", "ConfigureServices.OnChallenge", errorMsg);
                                return Task.FromResult(0);
                            }
                        };
                    });
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICryptoHelper, CryptoHelper>();
            services.AddScoped<ICookieHelper, CookieHelper>();
            services.AddScoped<IJWTHelper, JWTHelper>();
            services.AddScoped<IProxyHelper, ProxyHelper>();

            // Convert all generated URLs to lowercase.
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            // Default all controller actions to require authentication unless adorned with [AllowAnonymous].
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Public/Error");
            }

            //Credit : https://devblog.dymel.pl/2017/04/25/aspnetcore-reverse-proxy-client-ip/
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All,
                RequireHeaderSymmetry = false,
                ForwardLimit = null
                //KnownProxies = { IPAddress.Parse("162.158.202.131"), IPAddress.Parse("10.7.0.2") },
            });

            app.UseMiddleware<HeaderPreRequestMiddleware>();

            app.UseAuthentication();

            app.UseMiddleware<ProxyMiddleware>();

            app.UseMiddleware<HeaderPostRequestMiddleware>();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "apiRoute",
                    template: "api/{*appRoutes}",
                    defaults: new { Controller = "Api" });

                routes.MapRoute(
                    name: "protectedRoute",
                    template: "protected/{*appRoutes}",
                    defaults: new { Controller = "Protected", action = "Index" });

                routes.MapRoute(
                    name: "publicRoute",
                    template: "public/{*appRoutes}",
                    defaults: new { Controller = "Public", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Public}/{action=Index}/{id?}");
            });
        }
    }
}
