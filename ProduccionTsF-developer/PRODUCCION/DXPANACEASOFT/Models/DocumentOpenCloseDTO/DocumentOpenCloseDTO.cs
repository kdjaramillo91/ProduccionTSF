
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DXPANACEASOFT.Models.DocumentOpenCloseDTO
{
    public class DocumentOpenCloseForm
    {
        [Key]
        public int id_doc { get; set; }
        
        public string numberDoc { get; set; }

		public string codeDocumentType { get; set; }

		public string stateDoc { get; set; }

		public DateTime emissionDate { get; set; }

        public string nameProvider { get; set; }

        public string nameProductionUnitProvider { get; set; }

        [Required]
        public bool isOpen { get; set; }

        [Required]
        public string commentOnAction { get; set; }

        public DateTime? DateUpdate { get; set; }



    }
}