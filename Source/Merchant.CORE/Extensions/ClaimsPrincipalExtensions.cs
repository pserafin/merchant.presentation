using System;
using System.Linq;
using System.Security.Claims;
using Merchant.CORE.Enums;

namespace Merchant.CORE.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public const string NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        public const string IdClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        public const string RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        public static string GetName(this ClaimsPrincipal principal) =>
            principal.Claims.FirstOrDefault(x => x.Type == NameClaimType)?.Value;

        public static int? GetId(this ClaimsPrincipal principal)
        {
            var id = principal.Claims.FirstOrDefault(x => x.Type == IdClaimType)?.Value;

            return string.IsNullOrEmpty(id) 
                ? (int?)null 
                : Convert.ToInt32(id);
        }

        public static bool IsCustomerOnly(this ClaimsPrincipal principal)
        {
            var roles = principal.Claims.Where(x => x.Type == RoleClaimType);
            return roles.Count() == 1 && roles.Any(x => x.Value == Role.Customer.ToString());
        }

        public static bool IsCustomer(this ClaimsPrincipal principal) =>
            principal.Claims.Any(x => x.Type == RoleClaimType && x.Value == Role.Customer.ToString());

        public static bool IsAdmin(this ClaimsPrincipal principal) =>
            principal.Claims.Any(x => x.Type == RoleClaimType && x.Value == Role.Administrator.ToString());
    }
}
