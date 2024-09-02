namespace DXPANACEASOFT.Models.Dto
{

    public class ActiveUserDto
    {
        public int id { get; set; }
        public string username { get; set; }
        public int id_company { get; set; }
        public int id_emissionPoint { get; set; }

        public EntityObjectPermissionsDto permisos { get; set; }
    }
}