using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DocumentTypeP.DocumentTypeModels;
using DXPANACEASOFT.Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderDocumentType
	{
		private static DBContext db = null;

		public static IEnumerable DocumentTypes()
		{
			db = new DBContext();
			return db.DocumentType.Where(t => t.isActive).OrderBy(t => t.id).ToList();
		}
		public static IEnumerable DocumentTypeOpportunities(int? id_company)
		{
			db = new DBContext();
			return db.DocumentType.Where(t => t.isActive &&
											  t.id_company == id_company &&
											  (t.code.Equals("15") || t.code.Equals("16"))).OrderBy(t => t.id).ToList();
		}
		public static IEnumerable DocumentTypeOpeningClose()
		{
			db = new DBContext();
			return db.DocumentType.Where(t => t.isActive
											&& (t.code.Equals("02") || t.code.Equals("08"))).ToList();
		}
		public static DocumentType DocumentTypeById(int? id)
		{
			db = new DBContext();
			return db.DocumentType.FirstOrDefault(t => t.id == id && t.isActive);
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

			if (documentType != null)
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

		public static IEnumerable DocumentTypeForIntegrationProcess(int? id_company)
		{
			db = new DBContext();



			var documentTypeCodeList = db.tbsysIntegrationDocumentConfig
														  .Where(t => t.isActive)
														  .ToList()
														  .Select(r => new { r.code, r.codeDocumentType, r.description, r.isGroupValue })
														  .ToList();

			string[] documentTypeCodeArray = documentTypeCodeList
													.Select(r => r.codeDocumentType)
													.ToArray();

			var documentTypeList = db.DocumentType
											.Where(t => documentTypeCodeArray.Contains(t.code))
											.ToList();

			Dictionary<string, bool> isRequeredByInt = ServiceIntegrationProcess.GetRequeridDate(db);


			var resultList = from _documentType in documentTypeList
							 join _documentTypeIntegration in documentTypeCodeList on
							 _documentType.code equals _documentTypeIntegration.codeDocumentType
							 join _rqInt in isRequeredByInt on
							 _documentTypeIntegration.code equals _rqInt.Key
							 select new
							 {
								 _documentType.id,
								 _documentType.code,
								 _documentTypeIntegration.description,
								 isRequiredDate = _rqInt.Value,
								 isGroup = _documentTypeIntegration.isGroupValue
							 };





			/**
			 
		   var preDocument2 = from _document in documentList
							   join _lote in lotesCompraValidados on
							   _document.id equals _lote.id
							   select _document;
		 
		 */


			return resultList;
		}

		public static DocumentTypeModelP GetOneDocumentType(int idDt)
		{
			db = new DBContext();
			return db.DocumentType.Where(w => w.id == idDt)
						.Select(s => new DocumentTypeModelP
						{
							idDocumentTypeModelP = s.id,
							codeModelP = s.code,
							nameModelP = s.name
						}).FirstOrDefault();

		}
		public static DocumentTypeModelP GetOneDocumentTypeByCode(string code_Dt)
		{
			db = new DBContext();
			return db.DocumentType.Where(w => w.code == code_Dt)
						.Select(s => new DocumentTypeModelP
						{
							idDocumentTypeModelP = s.id,
							codeModelP = s.code,
							nameModelP = s.name
						}).FirstOrDefault();

		}
		public static IEnumerable GetDocumentTypes()
		{
			db = new DBContext();
			return db.DocumentType
						.Select(s => new DocumentTypeModelP
						{
							idDocumentTypeModelP = s.id,
							codeModelP = s.code,
							nameModelP = s.name
						}).ToList();
		}

		public static IEnumerable DocumentTypesByCompany(int? id_company)
		{
			db = new DBContext();

			return db.DocumentType.Where(t =>
										 t.id_company == id_company &&
										 t.isActive).OrderBy(t => t.id).ToList();
		}

        public static IEnumerable DocumentTypesByCompanyAndCurrent(int? id_company, int? id_current)
        {
            db = new DBContext();
            var documentTypeList = db.DocumentType.Where(g => (g.isActive && g.id_company == id_company) ||
                                                 g.id == (id_current == null ? 0 : id_current)).ToList();
            return documentTypeList;
        }
    }
}