using System.Collections.Generic;

namespace DXPANACEASOFT.Models.Dto
{
    public class EntityObjectPermissionsDto
    {

        public EntityObjectPermissionsDto()
        {
            listEntityPermissions = new List<EntityPermissionsDto>();
        }

        public List<EntityPermissionsDto> listEntityPermissions { get; set; }
    }


    public class EntityPermissionsDto
    {

        public int id_entity { get; set; }

        public string codeEntity { get; set; }

        public string nameEntity { get; set; }

        public List<EntityValuePermissionsDto> listValue { get; set; }

    }

    public class EntityValuePermissionsDto
    {

        public int id_entityValue { get; set; }

        public List<PermissionDto> listPermissions { get; set; }

    }

    public partial class PermissionDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int id_company { get; set; }
        public bool isActive { get; set; }
        public int id_userCreate { get; set; }
        public System.DateTime dateCreate { get; set; }
        public int id_userUpdate { get; set; }
        public System.DateTime dateUpdate { get; set; }         
    }

}