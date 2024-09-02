
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models.PersonP.PersonModel
{
    public class PersonBasicModelP
    {
        public int idPersonP { get; set; }
        public int? idPersonTypeP { get; set; }
        public int? idIdentificationTypeP { get; set; }
        public string identificationNumberP { get; set; }
        public string addressP { get; set; }
        public string emailP { get; set; }
        public string fullNamePersonP { get; set; }
        public bool isActive { get; set; }
    }
}