using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTSW.Constant
{
    public class UserConstant
    {
        public static class Role
        {
            public const string Admin = "Admin";
            public const string Developer = "Developer";
            public const string Tester = "Tester";
            public const string Manager = "Manager";
        }

        public static class RoleClaim
        {
            public const string Admin = "AdminRole";
            public const string Developer = "DeveloperRole";
            public const string Tester = "TesterRole";
            public const string Manager = "ManagerRole";
        }

        public static class UserClaim
        {
            public const string Admin = "AdminUser";
            public const string Developer = "DeveloperUser";
            public const string Tester = "TesterUser";
            public const string Manager = "ManagerUser";
        }

        public static Dictionary<string, string> Roles
        {
            get
            {
                var roles = new Dictionary<string, string>();
                roles.Add(UserConstant.Role.Admin, UserConstant.RoleClaim.Admin);
                roles.Add(UserConstant.Role.Developer, UserConstant.RoleClaim.Developer);
                roles.Add(UserConstant.Role.Manager, UserConstant.RoleClaim.Manager);
                roles.Add(UserConstant.Role.Tester, UserConstant.RoleClaim.Tester);

                return roles;
            }
        }
    }
}
