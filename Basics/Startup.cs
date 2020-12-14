using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Basics.AuthorizationRequirements;
using Basics.Controllers;
using Basics.Transformers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basics
{
    public class Startup
    {
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("cookieAuth")
                .AddCookie("cookieAuth",
                    config =>
                    {
                        config.Cookie.Name = "Grandmas.Cookie";
                        config.LoginPath = "/Home/Index";

                    });

            services.AddAuthorization(config =>
            {
                //var defaultAuthBuilder=new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthBuilder
                //    .RequireAuthenticatedUser()
                //    .RequireClaim(ClaimTypes.DateOfBirth)
                //    .Build();

                //config.DefaultPolicy = defaultAuthPolicy;

                config.AddPolicy("claim.DOB", policyBuilder =>
                {
                    policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
                });

                config.AddPolicy("Admin", policyBuilder =>
                {
                    policyBuilder.RequireClaim(ClaimTypes.Role, "Admin");
                });

            });

            services.AddScoped<IAuthorizationHandler,CustomAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler,CookieJarAuthorizationHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

            services.AddControllersWithViews(config =>
            {
                var defaultAuthBuilder=new AuthorizationPolicyBuilder();
                var policy = defaultAuthBuilder.RequireAuthenticatedUser().Build();
                //globl authorization filter
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddRazorPages()
                .AddRazorPagesOptions(config =>
                {
                    config.Conventions.AuthorizePage("/Razor/Secured");
                    config.Conventions.AuthorizePage("/Razor/Policy", "Admin");
                    //config.Conventions.AuthorizeFolder("/RazorSecured");
                    //config.Conventions.AllowAnonymousToPage("/RazorSecured/Anon");
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
