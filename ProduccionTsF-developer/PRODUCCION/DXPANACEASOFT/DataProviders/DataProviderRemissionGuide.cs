using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DispatchMaterialP.DispatchMaterialModel;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderRemissionGuide
	{
		private static DBContext _db = null;

		public static RemissionGuide RemissionGuide(int? id_remissionGuide)
		{
			_db = new DBContext();
			return _db.RemissionGuide.FirstOrDefault(r => r.id == id_remissionGuide);
		}
		public static string RemissionGuideDetailPurchaseDetail(int? id_remissionGuide)
		{
			_db = new DBContext();
			var remissionGuide = _db.RemissionGuide
									.FirstOrDefault(fod => fod.id == id_remissionGuide);

			var po = _db.RemissionGuideDetailPurchaseOrderDetail
						.FirstOrDefault(fod => fod.RemissionGuideDetail.RemissionGuide.id == id_remissionGuide)?.PurchaseOrderDetail?.PurchaseOrder ?? new PurchaseOrder();


			return po?.Document?.number ?? "";
		}
		public static RemissionGuideDetail RemissionGuideDetail(int? id_remissionGuideDet)
		{
			_db = new DBContext();
			return _db.RemissionGuideDetail.FirstOrDefault(r => r.id == id_remissionGuideDet);
		}

		public static RemissionGuideDispatchMaterial RemissionGuideDispatchMaterial(int? id_remissionGuideDisp)
		{
			_db = new DBContext();
			return _db.RemissionGuideDispatchMaterial.FirstOrDefault(r => r.id == id_remissionGuideDisp);
		}

		public static RemissionGuideControlVehicle RemissionGuideControlVehicle(int? id_remissionGuide)
		{
			_db = new DBContext();
			return _db.RemissionGuideControlVehicle.Where(x => x.id_remissionGuide == id_remissionGuide).FirstOrDefault(); //FirstOrDefault(r => r.id == id_remissionGuideDisp);
		}

		public static IQueryable<DispatchMaterialSequentialModelP> QueryAllDispatchMaterial(DBContext db)
		{
			return db.DispatchMaterialSequential
						.Select(s => new DispatchMaterialSequentialModelP
						{
							idRemGuideDispatchMaterialSequentialModelP = s.id_RemissionGuide,
							idWareDispatchMaterialSequentialModelP = s.id_Warehouse,
							sequentialDispatchMaterialSequentialModelP = s.sequential.ToString()
						});
		}
	}
}