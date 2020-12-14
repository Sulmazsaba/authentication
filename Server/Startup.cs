using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", config =>
                {

                    // show token in url
                    config.Events=new JwtBearerEvents()
                    {
                        OnMessageReceived = context=>
                        {
                            if (context.Request.Query.ContainsKey("access_token"))
                                context.Token = context.Request.Query["access_token"];

                            return Task.CompletedTask;
                        }
                    };

                    var bytes = Encoding.UTF8.GetBytes(Constants.Secret);
                    var key = new SymmetricSecurityKey(bytes);

                    // show token in header
                    config.TokenValidationParameters=new TokenValidationParameters()
                    {
                        ValidAudience = Constants.Audiance,
                        ValidIssuer = Constants.Issuer,
                        IssuerSigningKey = key
                    };
                });
            services.AddControllersWithViews();
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
            });
        }
    }
}
