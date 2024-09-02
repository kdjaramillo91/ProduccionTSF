using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DocumentP.DocumentModels;
using DXPANACEASOFT.Models.RequestInventoryMoveDTO;
using DXPANACEASOFT.Models.RequestInventoryMoveModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderRequestInventoryMove
	{
		private static DBContext _db = null;

		public static IEnumerable GetNatureMove()
		{
			_db = new DBContext();
			return _db.AdvanceParametersDetail
						.Where(w => w.AdvanceParameters.code.Equals("NMMGI"))
						.Select(s => new NatureMoveRIMFilter
						{
							idNatureF = s.id,
							descNatureF = s.description
						}).ToList();
		}
		public static IEnumerable GetWarehouse()
		{
			_db = new DBContext();
			return _db.Warehouse
						.Where(w => w.isActive)
						.Select(s => new WarehouseRIMFilter
						{
							idWarehouseF = s.id,
							descWarehouseF = s.name
						}).ToList();
		}
		public static IEnumerable GetPersonRequest()
		{
			_db = new DBContext();
			return _db.Person
						.Where(w => w.Rol.Any(a => a.name.Equals("Empleado")))
						.Select(s => new PersonRequestRIMFilter
						{
							idPersonRequestF = s.id,
							descPersonRequestF = s.fullname_businessName
						}).ToList();
		}

		public static List<int> GetIdsRequestInventoryMove()
		{
			_db = new DBContext();
			return _db.RequestInventoryMove.Select(s => s.id).ToList();
		}
		public static IQueryable<RequestInventoryMoveModelP> QueryRequestInventoryMoveModelP(DBContext db)
		{
			return db.RequestInventoryMove
				.Select(s => new RequestInventoryMoveModelP
				{
					idRIMModelP = s.id,
					idPersonRModelP = s.id_PersonRequest,
					idWarehouseModelP = s.id_Warehouse,
					idProviderModelP = s.id_Provider,
					idCustomerModelP = s.id_Customer,
					idNatureMoveModelP = s.id_NatureMove
				});
		}

		public static RequestInventoryMoveModelP GetRequestInventoryMove(int idRim)
		{
			RequestInventoryMoveModelP rqInMov = new RequestInventoryMoveModelP();

			_db = new DBContext();
			rqInMov = _db.RequestInventoryMove
						.Where(w => w.id == idRim)
						.Select(s => new RequestInventoryMoveModelP
						{
							idRIMModelP = s.id,
							idPersonRModelP = s.id_PersonRequest,
							idWarehouseModelP = s.id_Warehouse,
							idProviderModelP = s.id_Provider,
							idCustomerModelP = s.id_Customer,
							idNatureMoveModelP = s.id_NatureMove
						})
						.FirstOrDefault();

			rqInMov.documentModelP = _db.Document
						.Where(w => w.id == idRim)
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

			rqInMov.lstRequestInvDetail = _db.RequestInventoryMoveDetail
				.Where(w => w.id_RequestInventoryMove == idRim)
				.Select(s => new RequestInventoryMoveDetailModelP
				{
					id = s.id,
					id_item = s.id_item,
					quantityRequest = s.quantityRequest,
					quantityUpdate = (s.quantityUpdate == null) ? s.quantityRequest : s.quantityUpdate,
					id_warehouseLocation = s.idWarehouseLocation,
					isActive = s.isActive
				}).ToList();

			return rqInMov;


		}

		public static int GetIndexRequestInventoryMove(DBContext db, int id)
		{
			return QueryRequestInventoryMoveModelP(db)
				.OrderByDescending(o => o.idRIMModelP)
				.ToList()
				.FindIndex(f => f.idRIMModelP == id);
		}

		public static int GetCountRequestInventoryMove()
		{
			_db = new DBContext();
			return _db.RequestInventoryMove.Count();
		}

		public static int GetLastIdRequestInventoryMove(DBContext db, int page)
		{
			return QueryRequestInventoryMoveModelP(db)
				.OrderByDescending(p => p.idRIMModelP)
				.Take(page)
				.ToList()
				.Last()
				.idRIMModelP;
		}
	}
}