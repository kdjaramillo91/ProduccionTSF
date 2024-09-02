using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderCatalogStatePay
    {
        private static DBContext db = null;

        public static IEnumerable DocumentStatesPay()
        {
            db = new DBContext();

            var model = db.tbsysCatalogState.Where(t => t.isActive == true).ToList();

            //if (id_company != 0)
            //{
            //    model = model.Where(p => p.id_company == id_company && p.isActive).OrderBy(p => p.id).ToList();
            //}

            return model;
        }

        public static IEnumerable DocumentStatePayFilter(int id_company)
        {
            db = new DBContext();
            var model = db.DocumentState.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).OrderBy(p => p.id).ToList();
            }

            return model;
        }

        public static DocumentState DocumentStatePayById(int? id)
        {
            db = new DBContext();
            return db.DocumentState.FirstOrDefault(t => t.id == id);
        }

        public static IEnumerable DocumentStatePayByDocumentTypeCode(string code)
        {
            db = new DBContext();

            var documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals(code));
            if (documentType != null)
            {   
                return documentType.DocumentState.Where(s => s.isActive).OrderBy(s => s.id).ToList();
            }

            return db.DocumentState.Where(s => s.isActive).OrderBy(s => s.id).ToList();
        }

        public static IEnumerable DocumentStatesPayByDocumentType(int? id_documentType, int? id_company)
        {
            db = new DBContext();
            DocumentType documentType = db.DocumentType.FirstOrDefault(t =>t.id == id_documentType && t.id_company == id_company);

            if(documentType == null)
            {
                return db.DocumentState.Where(s => s.id_company == id_company && s.isActive)?.ToList();
            }

            return documentType?.DocumentState.Where(s => s.isActive)?.ToList() ?? new List<DocumentState>();
        }
        public static IEnumerable DocumentStatesPayByCompany(int? id_company)
        {
            db = new DBContext();

            return db.DocumentState.Where(s => s.id_company == id_company && s.isActive)?.ToList();

        }

        public static IEnumerable AllDocumentStatesPayByCompany(int? id_company)
        {
            db = new DBContext();
            
            return db.DocumentState.Where(s => s.id_company == id_company).Select(s => new { id = s.id, name = s.name }).ToList();

        }

        public static IEnumerable InventoryMoveDocumentStatesPayByCompany(int? id_company)
        {
            db = new DBContext();
            
            return db.DocumentState.Where(t => (t.code.Equals("01") || t.code.Equals("03")) && 
                                               t.id_company == id_company && t.isActive)?.ToList();
            
        }

        public static IEnumerable PurchaseRequestDocumentStatesPay()
        {
            db = new DBContext();
            var query = db.DocumentState.OrderBy(s => s.id); //.Where(t => t.isPurchaseRequest);
            return query.ToList();
        }

        public static IEnumerable PurchaseOrderDocumentStatesPay()
        {
            db = new DBContext();
            var query = db.DocumentState.OrderBy(s => s.id); //.Where(t => t.isPurchaseOrder);
            return query.ToList();
        }

        public static IEnumerable BusinessOportunityStatesPayByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();

            return db.BusinessOportunityState.Where(s => (s.id_company == id_company && s.isActive) ||
                                                 s.id == (id_current == null ? 0 : id_current))?.ToList();

        }

        public static IEnumerable BusinessOportunityStatesPayByCompany(int? id_company)
        {
            db = new DBContext();

            return db.BusinessOportunityState.Where(s => (s.id_company == id_company && s.isActive))?.ToList();

        }
    }
}