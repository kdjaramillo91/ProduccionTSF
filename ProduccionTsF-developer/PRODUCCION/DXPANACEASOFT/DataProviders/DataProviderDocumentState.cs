using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DocumentStateP.DocumentStateModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderDocumentState
    {
        private static DBContext _db = null;

        public static IEnumerable DocumentStates(int id_company)
        {
            _db = new DBContext();
            var model = _db.DocumentState.Where(t => t.isActive).ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company && p.isActive).OrderBy(p => p.id).ToList();
            }

            return model;


        }
        public static IEnumerable DocumentStatesByDocumentType( string code)
        {
            _db = new DBContext();
            return _db.DocumentState
                        .Where(w => w.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code.Equals(code)))
                        .Distinct()
                        .Select(s => new DocumentStateModelP
                        {
                            idDocumentStateModelP = s.id,
                            codeDocumentStateModelP = s.code,
                            descDocumentStateModelP = s.name,
                            isActive = s.isActive
                        }).ToArray();
        }
        public static IQueryable<DocumentStateModelP> QueryDocumentStatesByDocumentType(DBContext db, string code)
        {
            
            return db.DocumentState
                        .Where(w => w.tbsysDocumentTypeDocumentState.Any(a => a.DocumentType.code.Equals(code)))
                        .Distinct()
                        .Select(s => new DocumentStateModelP
                        {
                            idDocumentStateModelP = s.id,
                            codeDocumentStateModelP = s.code,
                            descDocumentStateModelP = s.name,
                            isActive = s.isActive
                        });
        }
        public static DocumentStateModelP GetDocumentStateByCode(string code)
        {
            _db = new DBContext();
            return _db.DocumentState
                        .Where(w => w.code == code)
                        .Select(s => new DocumentStateModelP
                        {
                            idDocumentStateModelP = s.id,
                            codeDocumentStateModelP = s.code,
                            descDocumentStateModelP = s.name,
                            isActive = s.isActive
                        }).FirstOrDefault();
        }

        public static IEnumerable DocumentStateFilter(int? id_company)
        {
            _db = new DBContext();
            var model = _db.DocumentState.ToList();

            if (id_company != 0)
            {
                model = model.Where(p => p.id_company == id_company).OrderBy(p => p.id).ToList();
            }

            return model;
        }

        public static DocumentState DocumentStateById(int? id)
        {
            _db = new DBContext();
            return _db.DocumentState.FirstOrDefault(t => t.id == id);
        }

        public static int GetIdDocumentStateDefault()
        {
            _db = new DBContext();
            var documentState = _db.DocumentState.FirstOrDefault(t => t.code == "01");
            return documentState?.id ?? 0;
        }

        public static DocumentState DocumentStateByCodeCompany(int id_documentState, int id_company)
        {
            _db = new DBContext();


            return _db.DocumentState.FirstOrDefault(s => s.isActive && s.id == id_documentState && s.id_company == id_company);
        }
        public static IEnumerable DocumentStateByDocumentTypeCode(string code)
        {
            _db = new DBContext();

            var documentType = _db.DocumentType.FirstOrDefault(t => t.code.Equals(code));
            if (documentType != null)
            {
                return documentType.DocumentState.Where(s => s.isActive).OrderBy(s => s.id).ToList();
            }

            return _db.DocumentState.Where(s => s.isActive).OrderBy(s => s.id).ToList();
        }

        public static IEnumerable DocumentStatesByDocumentType(int? id_documentType, int? id_company)
        {
            _db = new DBContext();
            DocumentType documentType = _db.DocumentType.FirstOrDefault(t => t.id == id_documentType && t.id_company == id_company);

            if (documentType == null)
            {
                return _db.DocumentState.Where(s => s.id_company == id_company && s.isActive)?.ToList();
            }

            return documentType?.tbsysDocumentTypeDocumentState.Select(s =>
               s.DocumentState
            ).ToList() ?? new List<DocumentState>();

        }
        public static IEnumerable DocumentStatesByCompany(int? id_company)
        {
            _db = new DBContext();

            return _db.DocumentState.Where(s => s.id_company == id_company && s.isActive)?.ToList();

        }

        public static IEnumerable AllDocumentStatesByCompany(int? id_company)
        {
            _db = new DBContext();

            return _db.DocumentState.Where(s => s.id_company == id_company).Select(s => new { id = s.id, name = s.name }).ToList();

        }

        public static IEnumerable InventoryMoveDocumentStatesByCompany(int? id_company)
        {
            _db = new DBContext();

            return _db.DocumentState.Where(t => (t.code.Equals("01") || t.code.Equals("03")) &&
                                               t.id_company == id_company && t.isActive)?.ToList();

        }

        public static IEnumerable PurchaseRequestDocumentStates()
        {
            _db = new DBContext();
            var query = _db.DocumentState.OrderBy(s => s.id); //.Where(t => t.isPurchaseRequest);
            return query.ToList();
        }

        public static IEnumerable PurchaseOrderDocumentStates()
        {
            _db = new DBContext();
            var query = _db.DocumentState.OrderBy(s => s.id); //.Where(t => t.isPurchaseOrder);
            return query.ToList();
        }

        public static IEnumerable BusinessOportunityStatesByCompanyAndCurrent(int? id_company, int? id_current)
        {
            _db = new DBContext();

            return _db.BusinessOportunityState.Where(s => (s.id_company == id_company && s.isActive) ||
                                                 s.id == (id_current == null ? 0 : id_current))?.ToList();

        }

        public static IEnumerable BusinessOportunityStatesByCompany(int? id_company)
        {
            _db = new DBContext();

            return _db.BusinessOportunityState.Where(s => (s.id_company == id_company && s.isActive))?.ToList();

        }

        public static DocumentState DocumentStateByCodeByCompany(int id_company, string CodeDocumentState)
        {
            _db = new DBContext();
            var model = _db.DocumentState.FirstOrDefault
                (
                    t =>
                    t.isActive &&
                    t.id_company == id_company &&
                    t.code == CodeDocumentState
                );


            return model;
        }

        public static IEnumerable GetDocumentStateModelP()
        {
            _db = new DBContext();

            return _db.DocumentState
                        .Select(s => new DocumentStateModelP
                        {
                            idDocumentStateModelP = s.id,
                            codeDocumentStateModelP = s.code,
                            descDocumentStateModelP = s.description,
                            isActive = s.isActive
                        }).ToList();
        }
    }
}