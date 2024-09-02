using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderDocumentType
    {
        private static DBContext db = null;

        public static IEnumerable DocumentTypes()
        {
            db = new DBContext();
            return db.DocumentType.Where(t=>t.isActive).OrderBy(t => t.id).ToList();
        }
        public static IEnumerable DocumentTypeOpportunities(int? id_company)
        {
            db = new DBContext();
            return db.DocumentType.Where(t => t.isActive && 
                                              t.id_company == id_company &&
                                              (t.code.Equals("15") || t.code.Equals("16"))).OrderBy(t => t.id).ToList();
        }
        public static DocumentType DocumentTypeById(int? id)
        {
            db = new DBContext();
            return db.DocumentType.FirstOrDefault(t => t.id == id&& t.isActive);
        }

        public static DocumentType DocumentTypeByCode(string code)
        {
            db = new DBContext();
            return db.DocumentType.FirstOrDefault(t => t.code.Equals(code) && t.isActive);
        }

        public static IEnumerable DocumentTypeState(int? id)
        {
            db = new DBContext();
            DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.id == id && t.isActive);

            if(documentType != null)
            {
                return documentType.DocumentState.Where(s => s.isActive).OrderBy(s => s.id).ToList();
            }

            return new List<DocumentState>(); 
        }

        public static IEnumerable InventoryMoveDocumentTypesByCompany(int id_company)
        {
            db = new DBContext();
            return db.DocumentType.Where(t => (t.code.Equals("03") || t.code.Equals("04") || t.code.Equals("05") || t.code.Equals("06") ||
                                               t.code.Equals("23") || t.code.Equals("24") || t.code.Equals("25") || t.code.Equals("26") ||
                                               t.code.Equals("27") || t.code.Equals("28") || t.code.Equals("32") || t.code.Equals("34")) &&
                                         t.id_company == id_company && 
                                         t.isActive).OrderBy(t => t.id).ToList();
        }

        public static IEnumerable ElectronicDocumentTypes(int id_company)
        {
            db = new DBContext();
            return db.DocumentType.Where(t => t.id_company == id_company && t.isElectronic && t.isActive).OrderBy(t => t.id).ToList();
        }

        public static IEnumerable AllElectronicDocumentTypes()
        {
            db = new DBContext();
            return db.DocumentType.Where(t => t.isElectronic).OrderBy(t => t.id).ToList();
        }

        public static IEnumerable DocumentTypesOfPriceListByCompany(int? id_company)
        {
            db = new DBContext();
            return db.DocumentType.Where(t => (t.code.Equals("18") || t.code.Equals("19") || t.code.Equals("20") || t.code.Equals("21")) &&
                                         t.id_company == id_company &&
                                         t.isActive).OrderBy(t => t.id).ToList();
        }
    }
}