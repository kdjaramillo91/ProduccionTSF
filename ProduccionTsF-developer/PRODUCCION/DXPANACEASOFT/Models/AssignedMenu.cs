using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class AssignedMenu : Menu
    {
        public bool isAssigned { get; set; }

        public List<Permission> Permissions { get; set; }
    }
}