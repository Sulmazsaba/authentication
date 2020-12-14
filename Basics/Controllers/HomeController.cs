 using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        [Authorize(policy: "claim.DOB")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }

        [Authorize(Roles="Admin")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }

        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var grandmaClaims=new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Bob"),
                new Claim(ClaimTypes.Email,"Bob@gmail.com"),
                new Claim("Grandma.Says","You are a good boi"),
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim(ClaimTypes.DateOfBirth,DateTime.Now.ToString()),
            };

            var licenseClaims=new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Bob Practor"),
                new Claim("License Type","A2")

            };

            var grandmaIdentity = new ClaimsIdentity(grandmaClaims,"grandam Identity");
            var licenseIdentity=new ClaimsIdentity(licenseClaims,"License Identity");

            HttpContext.SignInAsync(new ClaimsPrincipal(new[]{grandmaIdentity,licenseIdentity}));
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DoStuff([FromServices] IAuthorizationService authorizationService)
        {
            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("claim.DOB").Build();

           var result= await authorizationService.AuthorizeAsync(HttpContext.User, customPolicy);
           if(result.Succeeded)
           {
               return View("Index");
           }

           return View("Index");
        }
    }
}
