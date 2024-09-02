using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderBusinessOportunityDocumentTypePhase
    {
        private static DBContext db = null;

        public static IEnumerable BusinessOportunityDocumentTypePhases(int? id_company)
        {

            db = new DBContext();
            var model = db.BusinessOportunityDocumentTypePhase.Where(t => t.isActive && t.id_company == id_company).Select(p => new { p.id, name = p.Phase.name + "(" + p.DocumentType.name + ")"} ).ToList();
            
            return model;
        }

       
        //public static Phase PhaseById(int? id_phase)
        //{
        //    db = new DBContext(); ;
        //    return db.Phase.FirstOrDefault(v => v.id == id_phase);
        //}

    }
}