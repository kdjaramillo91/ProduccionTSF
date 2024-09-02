using DXPANACEASOFT.Models;
using System.Linq;

namespace DXPANACEASOFT.Services
{
	public static class ServicePriceList
	{
		public static IQueryable<PriceList> GetPriceList(string pathProgram, string pathLog)
		{
			IQueryable<PriceList> pricesList;

			using (var db = new DBContext())
			{
				pricesList = db.PriceList;
			}

			return ProcesPricesList(pricesList, pathProgram, pathLog);
		}

		public static IQueryable<PriceList> GetPriceList(IQueryable<PriceList> pricesList, string pathProgram, string pathLog)
		{
			return ProcesPricesList(pricesList, pathProgram, pathLog);
		}

		private static IQueryable<PriceList> ProcesPricesList(IQueryable<PriceList> pricesList, string pathProgram, string pathLog)
		{
			//string priceListProcess = "";
			//using (var db = new DBContext())
			//{
			//    var stateClose = db.DocumentState.FirstOrDefault(s => s.code.Equals("04"));
			//    var stateAnnul = db.DocumentState.FirstOrDefault(s => s.code.Equals("05"));
			//    foreach (var priceList in pricesList)
			//    {
			//        var endDate = priceList.endDate.AddDays(1);

			//        if (endDate < DateTime.Now && 
			//            priceList.Document.id_documentState != stateClose.id &&
			//            priceList.Document.id_documentState != stateAnnul.id)
			//        {
			//            priceList.Document.DocumentState = stateClose;
			//            priceList.Document.id_documentState = stateClose.id;

			//            var pListDb = db.PriceList.FirstOrDefault(p => p.id == priceList.id);
			//            pListDb.Document.DocumentState = stateClose;
			//            pListDb.Document.id_documentState = stateClose.id;
			//            db.PriceList.Attach(pListDb);
			//            db.Entry(pListDb).State = EntityState.Modified;
			//            db.SaveChanges();
			//            priceListProcess += priceList.id.ToString() + ",";
			//        }
			//    }

			//    if (priceListProcess != "" && priceListProcess.Length > 1)
			//    {
			//        #region Replicate Person to PriceList
			//        try
			//        {
			//            #region 
			//            ProcessStartInfo startInfo = new ProcessStartInfo();
			//            startInfo.FileName = pathProgram;
			//            startInfo.Arguments = "0 RLPTP ReplicateInformationMassive " + priceListProcess;
			//            Process.Start(startInfo);
			//            #endregion
			//        }                    
			//        catch (Exception ex)
			//        {
			//            MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, pathLog, "PersonUpdateReplication", "PROD");
			//        }
			//        #endregion
			//    }
			//}
			return pricesList;
		}
	}
}