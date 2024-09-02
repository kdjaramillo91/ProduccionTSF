using DXPANACEASOFT.Models.Dto;
using System.Linq;

namespace DXPANACEASOFT.Models.Helpers
{
    public static class EntityObjectPermissionsHelper
    {
        public static EntityObjectPermissionsDto ToDto(this EntityObjectPermissions input)
        {
            EntityObjectPermissionsDto model = null;

            if (input == null) return model;

            model = new EntityObjectPermissionsDto
            {
                 listEntityPermissions = input.listEntityPermissions.Select(r=> r.ToDto()).ToList()                                     
            };
            return model;

        }

        public static EntityPermissionsDto ToDto(this EntityPermissions input)
        {
            EntityPermissionsDto model = null;
            if (input == null) return model;

            model = new EntityPermissionsDto
            {
                codeEntity = input.codeEntity,
                id_entity = input.id_entity,
                nameEntity = input.nameEntity,
                listValue = input.listValue.Select(r=> r.ToDto()).ToList()                 
            };
            return model;

        }

        public static EntityValuePermissionsDto ToDto(this EntityValuePermissions input)
        {
            EntityValuePermissionsDto model = null;
            if(input == null) return model;

            model = new EntityValuePermissionsDto
            {
                 id_entityValue = input.id_entityValue,
                 listPermissions = input.listPermissions.Select(r=> r.ToDto()).ToList()
            };
            return model;
        }

        public static PermissionDto ToDto(this Permission input)
        {
            PermissionDto model = null;
            if (input == null) return model;
            model = new PermissionDto
            {
                 id  = input.id,
                name = input.name,
                description = input.description,
                id_company = input.id_company,
                isActive = input.isActive,
                id_userCreate = input.id_userCreate,
                dateCreate = input.dateCreate,
                id_userUpdate = input.id_userUpdate,
                dateUpdate = input.dateUpdate,
            };
            return model;
        }

        public static EntityObjectPermissions ToModel(this EntityObjectPermissionsDto input)
        {
            EntityObjectPermissions model = null;

            if (input == null) return model;
            model = new EntityObjectPermissions
            {
                listEntityPermissions = input.listEntityPermissions.Select(r => r.ToModel()).ToList()
            };
            return model;

        }

        public static EntityPermissions ToModel(this EntityPermissionsDto input)
        {
            EntityPermissions model = null;
            if (input == null) return model;

            model = new EntityPermissions
            {
                codeEntity = input.codeEntity,
                id_entity = input.id_entity,
                nameEntity = input.nameEntity,
                listValue = input.listValue.Select(r => r.ToModel()).ToList()
            };
            return model;
        }

        public static EntityValuePermissions ToModel(this EntityValuePermissionsDto input)
        {
            EntityValuePermissions model = null;
            if (input == null) return model;

            model = new EntityValuePermissions
            {
                id_entityValue = input.id_entityValue,
                listPermissions = input.listPermissions.Select(r => r.ToModel()).ToList()
            };
            return model;


        }

        public static Permission ToModel(this PermissionDto input)
        {
            Permission model = null;
            if (input == null) return model;
            model = new Permission
            {
                id = input.id,
                name = input.name,
                description = input.description,
                id_company = input.id_company,
                isActive = input.isActive,
                id_userCreate = input.id_userCreate,
                dateCreate = input.dateCreate,
                id_userUpdate = input.id_userUpdate,
                dateUpdate = input.dateUpdate,
            };
            return model;
        }

    }
        
}