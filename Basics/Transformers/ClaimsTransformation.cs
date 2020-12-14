using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Basics.Transformers
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var hasFriendClaims = principal.Claims.Any(i => i.Type == "friend");

            if (!hasFriendClaims)
            {
                ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("friend","Good"));
            }

            return Task.FromResult(principal);
        }
    }
}
