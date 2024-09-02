using System.Collections.Generic;
using System.Linq;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderControlState
	{
		private static DBContext db = null;

		public static List<tbsysDocumentDocumentStateControlsState> ControlStateByDocumentByState(string codedocumentType, string codeDocumentState)
		{
			db = new DBContext();

			var idDocumentType = db.DocumentType.FirstOrDefault(e => e.code == codedocumentType)?.id ?? 0;
			var idDocumentState = db.DocumentState.FirstOrDefault(e => e.code == codeDocumentState)?.id ?? 0;

			return db.tbsysDocumentDocumentStateControlsState
				.Where(r => r.id_documentType == idDocumentType && r.id_documentState == idDocumentState)
				.ToList();
		}
		public static tbsysDocumentDocumentStateControlsState ControlStateByDocumentByStateByControl(
			string codedocumentType, string codeDocumentState, string controlName)
		{
			db = new DBContext();

			var idDocumentType = db.DocumentType.FirstOrDefault(e => e.code == codedocumentType)?.id ?? 0;
			var idDocumentState = db.DocumentState.FirstOrDefault(e => e.code == codeDocumentState)?.id ?? 0;
			var model = db.tbsysDocumentDocumentStateControlsState
				.FirstOrDefault(r => r.id_documentType == idDocumentType && r.id_documentState == idDocumentState && r.controlName == controlName);

			return model ?? new tbsysDocumentDocumentStateControlsState();
		}
	}
}