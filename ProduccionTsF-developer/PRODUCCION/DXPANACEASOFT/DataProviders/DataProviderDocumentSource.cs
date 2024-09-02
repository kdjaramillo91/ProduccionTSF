using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DocumentSourceP.DocumentsourceModels;
using System.Collections;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderDocumentSource
	{
		private static DBContext _db = null;

		public static IQueryable<DocumentSourceModelP> QueryDocumentsSource(DBContext db, int idDocument)
		{
			return db.DocumentSource
						.Where(w => w.id_document == idDocument)
						.Select(s => new DocumentSourceModelP
						{
							id_ds = s.id,
							id_Document_P = s.id_document,
							id_Document_Origin_P = s.id_documentOrigin
						});
		}
		public static IQueryable<DocumentSourceModelP> QueryDocumentsSource(DBContext db)
		{
			return db.DocumentSource
						.Select(s => new DocumentSourceModelP
						{
							id_ds = s.id,
							id_Document_P = s.id_document,
							id_Document_Origin_P = s.id_documentOrigin
						});
		}
		public static IEnumerable DocumentsSourceOrigin(int idDocumentOrigin)
		{
			_db = new DBContext();
			return _db.DocumentSource
						.Where(w => w.id_documentOrigin == idDocumentOrigin)
						.Select(s => new DocumentSourceModelP
						{
							id_ds = s.id,
							id_Document_P = s.id_document,
							id_Document_Origin_P = s.id_documentOrigin
						}).ToList();
		}
	}
}