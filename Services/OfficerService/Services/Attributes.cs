// Attributes/RoleAuthorizeAttribute.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace OfficerService.Services
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class Attributes : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly int[] _allowedRoleIds;

        public Attributes(params int[] allowedRoleIds)
        {
            _allowedRoleIds = allowedRoleIds;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if user is authenticated
            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Get role from claims
            var roleClaim = user.FindFirst("RoleId");
            if (roleClaim == null || !int.TryParse(roleClaim.Value, out int userRoleId))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Check if user's role is in allowed roles
            if (!_allowedRoleIds.Contains(userRoleId))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }

    // Specific role attributes for convenience
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminOnlyAttribute : Attributes
    {
        public AdminOnlyAttribute() : base(1) { }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class OfficerOnlyAttribute : Attributes
    {
        public OfficerOnlyAttribute() : base(19, 20, 21, 22, 23, 24, 25) { }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class StateOfficerOnlyAttribute : Attributes
    {
        public StateOfficerOnlyAttribute() : base(19) { }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApplicantOnlyAttribute : Attributes
    {
        public ApplicantOnlyAttribute() : base(28) { }
    }
}

