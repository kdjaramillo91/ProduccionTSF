using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderPhase
    {
        private static DBContext db = null;

        public static IEnumerable Phases(int? id_company)
        {
            db = new DBContext();
            var model = db.Phase.Where(t => t.isActive).ToList();

            if (id_company != 0 && id_company != null)
            {
                model = model.Where(p => p.id_company == id_company ).ToList();
            }

            return model;
        }
       
        public static Phase PhaseById(int? id_phase)
        {
            db = new DBContext(); ;
            return db.Phase.FirstOrDefault(v => v.id == id_phase);
        }

        public static IEnumerable PhasesByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();

            return db.Phase.Where(s => (s.id_company == id_company && s.isActive) ||
                                                 s.id == (id_current == null ? 0 : id_current))?.ToList();

        }
        public static IEnumerable BusinessOportunityDocumentTypePhasesByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();

            var model = db.BusinessOportunityDocumentTypePhase.Where(s => (s.id_company == id_company && s.isActive) ||
                                                 s.id == (id_current == null ? 0 : id_current))?.Select(s=> new { id = s.id, name = s.Phase.name, advance = s.advance }).ToList();
            return model.OrderBy(ob => ob.advance).ToList();

        }
    }
}