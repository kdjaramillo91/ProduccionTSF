using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderPurchaseRequest
	{
		private static DBContext db = null;

		public static PurchaseRequest PurchaseRequest(int? id)
		{
			db = new DBContext();
			return db.PurchaseRequest.FirstOrDefault(t => t.id == id);
		}

		public static IEnumerable GetPurchaseRequestDetailsForOrderWithCurrent(int? id_purchaseRequestDetail)
		{
			var db = new DBContext();
			//var ListPurchaseResquest = (from e in db.PurchaseOrderDetailByGrammagePurchaseRequest
			//                            where e.PurchaseOrderDetailByGrammage.PurchaseOrder.Document.DocumentState.code != "05" && //"05" Anulada
			//                                  e.PurchaseOrderDetailByGrammage.isActive
			//                            select e.id_purchaseRequestDetail).ToList();

			var model = db.PurchaseRequestDetail.Where(d => d.id == id_purchaseRequestDetail || (d.PurchaseRequest.Document.DocumentState.code == "06" && d.quantityOutstandingPurchase > 0))//"06" AUTORIZADA
						  .OrderByDescending(d => d.id_purchaseRequest);

			//if (ListPurchaseResquest != null && ListPurchaseResquest.Count > 0)
			//{
			//    model = model.Where(X => X.id == id_purchaseRequestDetail || (!ListPurchaseResquest.Contains(X.id))).OrderByDescending(d => d.id_purchaseRequest);

			//}

			return model.Select(s => new
			{
				s.id,
				s.PurchaseRequest.Document.number,
				item = s.Item.name,
				grammageFrom = s.Grammage.code,
				grammageTo = s.Grammage1.code
			}).ToList();
		}
	}
}