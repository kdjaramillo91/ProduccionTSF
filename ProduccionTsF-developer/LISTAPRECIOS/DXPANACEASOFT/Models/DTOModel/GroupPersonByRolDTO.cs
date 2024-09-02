using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.DTOModel
{
    public class GroupPersonByRolDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string rol { get; set; }
        public int? id_rol { get; set; }
        public string company { get; set; }
        public int? id_company { get; set; }
        public bool isActive { get; set; }

        public List<GroupPersonByRolDetailDTO> ListGroupPersonByRolDetailDTO { get; set; }
    }

    public class GroupPersonByRolDetailDTO
    {
        public int id { get; set; }
        public int id_groupPersonByRol { get; set; }
        public int id_person { get; set; }
    }
}