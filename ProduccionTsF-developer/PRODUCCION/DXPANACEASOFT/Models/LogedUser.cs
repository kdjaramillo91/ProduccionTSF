using System.Security.Principal;

namespace DXPANACEASOFT.Models
{
    public class LogedUser : IPrincipal
    {
        private readonly LogedUserIdentity identity;

        public ApplicationUser User => identity.User;

        public LogedUser(ApplicationUser user)
        {
            identity = new LogedUserIdentity(user);
        }

        public IIdentity Identity => identity;

        public bool IsInRole(string role)
        {
            return identity.AuthenticationType.Equals(role);
        }
    }
}