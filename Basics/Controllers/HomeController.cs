﻿using System;
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

        public IActionResult Authenticate()
        {
            var grandmaClaims=new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Bob"),
                new Claim(ClaimTypes.Email,"Bob@gmail.com"),
                new Claim("Grandma.Says","You are a good boi")
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
    }
}