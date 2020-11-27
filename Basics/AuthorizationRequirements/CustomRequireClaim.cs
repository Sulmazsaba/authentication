using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Basics.AuthorizationRequirements
{
    public class CustomRequireClaim :IAuthorizationRequirement
    {
        public string Claim { get; }
        public CustomRequireClaim(string claim)
        {
            Claim = claim;
        }
    }

    public class CustomAuthorizationHandler : AuthorizationHandler<CustomRequireClaim>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequireClaim requirement)
        {
          var hasClaim=context.User.Claims.Any(i => i.Type == requirement.Claim);
          if (hasClaim)
          {
              context.Succeed(requirement);
          }
          return Task.CompletedTask;
        }
    }


    public static class RequireCustomBuilderExtensions{

        public static AuthorizationPolicyBuilder RequireCustomClaim(
            this AuthorizationPolicyBuilder builder
            ,string claim)
        {
            builder.AddRequirements(new CustomRequireClaim(claim));

            return builder;
        }

    }
}
