using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class EntityObjectPermissions
    {
        public EntityObjectPermissions()
        {
            listEntityPermissions = new List<EntityPermissions>();
            listObjectPermissions = new List<ObjectPermissions>();
        }
        public List<EntityPermissions> listEntityPermissions { get; set; }
        public List<ObjectPermissions> listObjectPermissions { get; set; }
        

    }
    public class EntityPermissions
    {
        
        public int id_entity { get; set; }

        public string codeEntity { get; set; }

        public string nameEntity { get; set; }

        public List<EntityValuePermissions> listValue { get; set; }

    }

    public class EntityValuePermissions
    {

        public int id_entityValue { get; set; }

        public List<Permission> listPermissions { get; set; }

    }

    public class ObjectPermissions
    {

        public int id_object { get; set; }

        public string codeObject { get; set; }

        public string nameObject { get; set; }

    }
}