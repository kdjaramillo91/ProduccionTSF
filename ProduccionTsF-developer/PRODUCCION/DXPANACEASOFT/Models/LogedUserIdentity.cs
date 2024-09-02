using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class LogedUserIdentity : IIdentity
    {
        public IIdentity Identity { get; set; }

        public ApplicationUser User { get; set; }

        public LogedUserIdentity(ApplicationUser user)
        {
            // TODO: 
            Identity = new GenericIdentity(user.username, "Administrador"/*user.Group*/);
            User = user;
        }

        public string AuthenticationType => Identity.AuthenticationType;

        public bool IsAuthenticated => Identity.IsAuthenticated;

        public string Name => Identity.Name;
    }
}