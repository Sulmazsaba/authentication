using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.Extensions.Options;

namespace Basics.CustomPolicyProvider
{
    public class SecurityLevelAttribute : AuthorizeAttribute
    {
        public SecurityLevelAttribute(int level)
        {
            Policy = $"{DynamicPolicies.SecurityLevel}.{level}";
        }
    }
    public static class DynamicPolicies
    {
        public static IEnumerable<string> Get()
        {
            yield return SecurityLevel;
            yield return Rank;
        }
        public  const string SecurityLevel= "SecurityLevel";
        public const string Rank = "Rank";
    }

    public static class DynamicAuthorizationPolicyFactory
    {
        public static AuthorizationPolicy Create(string policyName)
        {
            var parts = policyName.Split(".");
            var type = parts.First();
            var value = parts.Last();

            switch (type)
            {
                case DynamicPolicies.Rank:
                    return new AuthorizationPolicyBuilder()
                        .RequireClaim("Rank",value)
                        .Build();
                case DynamicPolicies.SecurityLevel:
                    return new AuthorizationPolicyBuilder()
                            .AddRequirements(
                                new SecurityLevelRequirement(Convert.ToInt32(value)))
                            .Build();
                default:
                    return null;
            }
        }

    } 
    public class SecurityLevelRequirement: IAuthorizationRequirement
        {
            public int Value { get; }

            public SecurityLevelRequirement(int value)
            {
                this.Value = value;
            }
        }

    public class SecurityLevelHandler : AuthorizationHandler<SecurityLevelRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            SecurityLevelRequirement requirement)
        {
            var claimValue =Convert.ToInt32(context.User.Claims
                .FirstOrDefault(i => i.Type == DynamicPolicies.SecurityLevel).Value??"0");

                
            if(requirement.Value <= claimValue)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
    public class CustomAuthorizationPolicyProvider: DefaultAuthorizationPolicyProvider
    {
        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        {

        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            foreach (var customPolicy in DynamicPolicies.Get())
            {
                if (policyName.StartsWith(customPolicy))
                {
                    //var policy=new AuthorizationPolicyBuilder().Build();
                    var policy = DynamicAuthorizationPolicyFactory.Create(policyName);
                    return Task.FromResult(policy);
                }
            }
            return base.GetPolicyAsync(policyName);
        }
    }

    
}
