using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.AdvanceParametersDetailP.AdvanceParametersDetailModels;
using DXPANACEASOFT.Models.WarehouseP.WarehouseModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderRecordSecurity
	{
		private static DBContext _db = null;

		public static tbsysUserRecordSecurity RecordSecurityById(int? id_RecordSecurity)
		{
			_db = new DBContext();
			var model = _db.tbsysUserRecordSecurity
						.Where(t => t.id == id_RecordSecurity)?.FirstOrDefault() ?? new tbsysUserRecordSecurity();

			return model;
		}

		public static tbsysObjSecurityRecord ObjRecordSecurityById(int? id_ObjRecordSecurity)
		{
			_db = new DBContext();
			var model = _db.tbsysObjSecurityRecord
						.Where(t => t.id == id_ObjRecordSecurity)?.FirstOrDefault() ?? new tbsysObjSecurityRecord();

			return model;
		}

		public static IEnumerable AllObjRecordSecurity()
		{
			_db = new DBContext();
			var model = _db.tbsysObjSecurityRecord
						.Select(e => new
						{
							id = e.id,
							name = e.obj
						});

			return model.ToList();
		}
	}
}