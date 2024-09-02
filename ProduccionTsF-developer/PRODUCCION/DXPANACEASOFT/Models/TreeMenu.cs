using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public class TreeMenu
    {
        public int id { get; set; }

        public int? id_parent { get; set; }

        public int position { get; set; }

        public string title { get; set; }

        public string controller { get; set; }

        public string action { get; set; }

        public List<TreeMenu> children { get; set; }

        public TreeMenu()
        {
            children = new List<TreeMenu>();
        }
    }
}