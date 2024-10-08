﻿using System.Security.Claims;

namespace FurniflexBE.Helpers
{
    public static class IdentityHelper
    {
        public static int? GetUserId(ClaimsIdentity identity)
        {
            
            var userIdClaim = identity.FindFirst("userid");
            if (userIdClaim != null)
            {
                return int.TryParse(userIdClaim.Value, out var userId) ? userId : (int?)null;
            }
       
            return null;
        }

        public static int? GetRoleId(ClaimsIdentity identity)
        {

            var roleIdClaim = identity.FindFirst("role");
            if (roleIdClaim != null)
            {
                return int.TryParse(roleIdClaim.Value, out var roleId) ? roleId : (int?)null;
            }

            return null;
        }

        public static string GetRoleName(ClaimsIdentity identity)
        {
      
            var roleNameClaim = identity.FindFirst("roleName");
            if (roleNameClaim != null)
            {
                System.Diagnostics.Debug.WriteLine("Role Name Claim: " + roleNameClaim.Value);
                return roleNameClaim.Value;
            }

            return "";
        }


        // Additional helper functions can be added here as needed
    }
}
