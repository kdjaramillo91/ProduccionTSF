using DevExpress.Utils.Extensions;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DocumentP.DocumentModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderDocument
	{
		private static DBContext _db = null;

		public static IEnumerable Documents()
		{
			_db = new DBContext();
			return _db.Document.ToList();
		}

		public static Document Document(int? id_document)
		{
			_db = new DBContext();
			return _db.Document.FirstOrDefault(fod => fod.id == id_document);
		}
		public static List<PurchaseRequestDetail> PurchesRequestDetailByDocument(int id)
		{
			_db = new DBContext();
			return _db.PurchaseRequestDetail.Where(t => t.id_purchaseRequest == id).ToList();
		}
		public static List<PurchaseOrderDetail> PurchesOrderDetailByDocument(int id)
		{
			_db = new DBContext();
			return _db.PurchaseOrderDetail.Where(t => t.id_purchaseOrder == id).ToList();
		}

		public static IEnumerable DocumentsByCompanyAndCurrent(int? id_company, int? id_current)
		{
			_db = new DBContext();

			return _db.Document.Where(s => (s.EmissionPoint.BranchOffice.Division.id_company == id_company) ||
												 s.id == (id_current == null ? 0 : id_current))?.ToList();

		}

		public static IEnumerable DocumentsByCompanyAndReasignacion(int? id_company, int? id_current)
		{
			_db = new DBContext();

			var document = _db.Document.Where(s => (s.EmissionPoint.BranchOffice.Division.id_company == id_company
											&& s.Invoice.InvoiceExterior.dismissalreason == "Reasignación de Proforma"
											&& s.DocumentState.code == "05") ||
												 s.id == (id_current == null ? 0 : id_current))?.ToList();

			return document;

		}
        public static IEnumerable DocumentsByCompanyAndInventory(int? id_company, int? id_current)
        {
            _db = new DBContext();

            var lstInvoicesInventory = _db.InventoryMove
                                        .Where(w => w.id_Invoice != null && w.Document.DocumentState.code != "05")
                                        .Select(s => s.id_Invoice).Distinct().ToList();

            var document = _db.Document.Where(s => (s.EmissionPoint.BranchOffice.Division.id_company == id_company
											&& s.DocumentType.code == "07"
                                            && s.DocumentState.code != "05"
											&& !lstInvoicesInventory.Contains(s.id)) ||
                                                 s.id == (id_current == null ? 0 : id_current))?.ToList();

            return document;

        }

        public static IEnumerable DocumentsByCompanyAndInventoryInvoiceProvider(int? id_company,int? personCustomerId, int? id_current)
        {
            _db = new DBContext();

            var lstInvoicesInventory = _db.InventoryMove
												.Where(w => w.id_Invoice != null && w.Document.DocumentState.code != "05")
												.Select(s => s.id_Invoice).Distinct().ToList();

			//var document = _db.Document.Where(s => (s.EmissionPoint.BranchOffice.Division.id_company == id_company
			//                                && s.DocumentType.code == "07"
			//                                && s.DocumentState.code != "05"
			//                                && !lstInvoicesInventory.Contains(s.id)) ||
			//                                     s.id == (id_current == null ? 0 : id_current))?.ToList();

			var result = _db.Invoice
											.Include("Document")
											.Where(r => (r.id_buyer == personCustomerId
														&& r.Document.EmissionPoint.BranchOffice.Division.id_company == id_company
														&& r.Document.DocumentType.code == "07"
														&& r.Document.DocumentState.code != "05"
														&& !lstInvoicesInventory.Contains(r.id))
														||
														r.id == (id_current == null ? 0 : id_current)).ToList();
			var invoiceDocumentList = result.Select(r => new Document
										{
											id = r.Document.id,
											number = r.Document.number
										})
										.ToList();
											//.SelectMany(r=> r)
											//.ToArray();
			//return document;
			return invoiceDocumentList;

        }


        public static IEnumerable DocumentsByCompany(int? id_company)
		{
			_db = new DBContext();

			return _db.Document.Where(s => (s.EmissionPoint.BranchOffice.Division.id_company == id_company))?.ToList();

		}

		public static IEnumerable DocumentsByCompany(int? id_company, string codeBusinessOportunityDocumentType)
		{
			_db = new DBContext();
			//15: Oportunidad de Venta y 16: Oportunidad de Compra
			var documents = _db.Document.Where(w => (w.EmissionPoint.BranchOffice.Division.id_company == id_company && ((codeBusinessOportunityDocumentType == "15" && w.SalesQuotation != null && w.DocumentState.code == "06") ||
																													   (codeBusinessOportunityDocumentType == "16" && w.PurchaseOrder != null && w.DocumentState.code == "06")))).ToList();
			var model = documents.Select(s => new { s.id, s.number }).ToList();
			return model;
		}
		public static List<int> GetIdsDocuments()
		{
			_db = new DBContext();
			return _db.Document.Select(s => s.id).ToList();
		}

		public static IQueryable<DocumentModelP> QueryDocumentModelP(DBContext db, int? id_documenType)
		{
			return db.Document
				.Where(w => w.id_documentType == id_documenType)
				.Select(s => new DocumentModelP
				{
					idDocumentModelP = s.id,
					numberModelP = s.number,
					sequentialModelP = s.sequential,
					emissionDateModelP = s.emissionDate,
					authorizationDateModelP = s.authorizationDate,
					authorizationNumberModelP = s.authorizationNumber,
					accessKeyModelP = s.accessKey,
					descriptionModelP = s.description,
					referenceModelP = s.reference,
					idEmissionPointModelP = s.id_emissionPoint,
					idDocumentTypeModelP = s.id_documentType,
					idDocumentStateModelP = s.id_documentState,
					isOpenModelP = s.isOpen
				});
			;
		}
		public static List<DocumentModelP> GetDocumentModelP()
		{
			_db = new DBContext();
			return _db.Document
						.Select(s => new DocumentModelP
						{
							idDocumentModelP = s.id,
							numberModelP = s.number,
							sequentialModelP = s.sequential,
							emissionDateModelP = s.emissionDate,
							authorizationDateModelP = s.authorizationDate,
							authorizationNumberModelP = s.authorizationNumber,
							accessKeyModelP = s.accessKey,
							descriptionModelP = s.description,
							referenceModelP = s.reference,
							idEmissionPointModelP = s.id_emissionPoint,
							idDocumentTypeModelP = s.id_documentType,
							idDocumentStateModelP = s.id_documentState,
							isOpenModelP = s.isOpen
						}).ToList();
			;
		}
		public static List<DocumentModelP> GetDocumentModelP(int[] arrExclude)
		{
			_db = new DBContext();
			return _db.Document
						.Where(w => !arrExclude.Contains(w.id_documentType))
						.Select(s => new DocumentModelP
						{
							idDocumentModelP = s.id,
							numberModelP = s.number,
							sequentialModelP = s.sequential,
							emissionDateModelP = s.emissionDate,
							authorizationDateModelP = s.authorizationDate,
							authorizationNumberModelP = s.authorizationNumber,
							accessKeyModelP = s.accessKey,
							descriptionModelP = s.description,
							referenceModelP = s.reference,
							idEmissionPointModelP = s.id_emissionPoint,
							idDocumentTypeModelP = s.id_documentType,
							idDocumentStateModelP = s.id_documentState,
							isOpenModelP = s.isOpen
						}).ToList();
			;
		}
		public static DocumentModelP GetOneDocumentModelP(int? idDocument)
		{
			_db = new DBContext();
			return _db.Document
				.Where(w => w.id == idDocument)
				.Select(s => new DocumentModelP
				{
					idDocumentModelP = s.id,
					numberModelP = s.number,
					sequentialModelP = s.sequential,
					emissionDateModelP = s.emissionDate,
					authorizationDateModelP = s.authorizationDate,
					authorizationNumberModelP = s.authorizationNumber,
					accessKeyModelP = s.accessKey,
					descriptionModelP = s.description,
					referenceModelP = s.reference,
					idEmissionPointModelP = s.id_emissionPoint,
					idDocumentTypeModelP = s.id_documentType,
					idDocumentStateModelP = s.id_documentState,
					isOpenModelP = s.isOpen
				}).FirstOrDefault();
			;
		}

		public static IEnumerable GetDocumentLog(int idDocument)
		{
			_db = new DBContext();
			return _db.DocumentLog
					.Where(w => w.id == idDocument)
					.Select(s => new DocumentLogModelP
					{
						idDocumentModelP = s.id_Document,
						idActionOnDocumentModelP = s.id_ActionOnDocument,
						descriptionModelP = s.description,
						idUserModelP = s.id_user,
						dtDateModelP = s.dateUser
					}).ToList();
		}
		public static IEnumerable GetDocumentTrackState(int idDocument)
		{
			_db = new DBContext();
			return _db.DocumentActionTrack
					.Where(w => w.idDocument == idDocument)
					.Select(s => new DocumentTrackStateModelP
					{
						idDocumentModelP = s.idDocument,
						idActionOnDocumentModelP = s.idActionOnDocument,
						descriptionModelP = s.description,
						idUserModelP = s.idUser,
						dtDateModelP = s.dateUser
					}).ToList();
		}
		public static ActionOnDocumentModelP GetActionOnDocument(string code)
		{
			_db = new DBContext();
			return _db.tbsysActionOnDocument
						.Where(w => w.code == code)
						.Select(s => new ActionOnDocumentModelP
						{
							idActionOnDocumentModelP = s.id,
							codeActionOnDocumentModelP = s.code,
							nameActionOnDocumentModelP = s.name,
							descriptionActionOnDocumentModelP = s.description,
							isActiveActionOnDocumentModelP = s.isActive
						}).FirstOrDefault();
		}

		public static IEnumerable GetActionOnDocuments()
		{
			_db = new DBContext();
			return _db.tbsysActionOnDocument
						.Select(s => new ActionOnDocumentModelP
						{
							idActionOnDocumentModelP = s.id,
							codeActionOnDocumentModelP = s.code,
							nameActionOnDocumentModelP = s.name,
							descriptionActionOnDocumentModelP = s.description,
							isActiveActionOnDocumentModelP = s.isActive
						}).ToList();
		}

		public static Document GetDocumentSourcePenOApro(int? id_documentOrigin, string codDocumentType)
		{
			_db = new DBContext();
			return _db.Document.FirstOrDefault(fod => fod.id == (_db.DocumentSource.FirstOrDefault(fod2 => fod2.id_documentOrigin == id_documentOrigin &&
																	(fod2.Document.DocumentState.code.Equals("01") || fod2.Document.DocumentState.code.Equals("03")) &&
																	fod2.Document.DocumentType.code.Equals(codDocumentType)).id_document));
			//_db.Document.FirstOrDefault(fod => fod.id == id_document);
		}
	}
}