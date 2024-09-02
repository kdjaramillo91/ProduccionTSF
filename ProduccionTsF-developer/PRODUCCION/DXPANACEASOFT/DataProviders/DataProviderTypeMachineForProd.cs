using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Data.Entity;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
    public class DataProviderTypeMachineForProd
	{
        private static DBContext _db = null;
		public static tbsysTypeMachineForProd TypeMachineForProdById(int? id)
		{
			_db = new DBContext();
			var query = _db.tbsysTypeMachineForProd.FirstOrDefault(t => t.id == id);
			return query;
		}

	}
}