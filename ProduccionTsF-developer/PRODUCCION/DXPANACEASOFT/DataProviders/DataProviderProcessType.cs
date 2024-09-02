using DXPANACEASOFT.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public class DataProviderProcessType
	{
		public static ProcessType ProcessTypeById(int? id)
		{
			var db = new DBContext();
			return db.ProcessType.FirstOrDefault(t => t.id == id);
		}

		public static IEnumerable ProcessTypeByCompany(int? id_company)
		{
			var db = new DBContext();
			var model = db.ProcessType.Where(t => t.isActive).ToList();

			if (id_company != 0)
			{
				model = model.Where(p => p.id_company == id_company && p.isActive).ToList();
			}

			return model;
		}

		public static IEnumerable ProcessTypeByCompanyAndProcessType(int? id_company, int? id_processTypeBegin)
		{
			var db = new DBContext();
			IEnumerable<int> ids = new int[] { };
			var model = db.ProcessType.Where(w => w.isActive);

			if (id_processTypeBegin != null && id_processTypeBegin != 0)
			{
				ids = db.ConfProcessTypeQualityControl
					.Where(w => w.id_processTypeBegin == id_processTypeBegin)
					.Select(s => (int)s.id_processTypeResult);
			}

			model = model.Where(w => ids.Contains(w.id));

			return model.ToList();
		}

		public static IEnumerable AllTypeProcess()
        {
			var db = new DBContext();
			return db.ProcessType.Where(w=>w.id<=4).ToList();
		}

		public static IEnumerable ProcessTypeSizeByProcess(int idProcessType)
		{
			var db = new DBContext();
			List<ProcessType> lstProcessType = new List<ProcessType>();

			var pt = db.ProcessType.FirstOrDefault(fod => fod.id == idProcessType);

			if (pt != null)
			{
				lstProcessType.Add(pt);
				if (pt.code.Equals("ENC"))
				{
					lstProcessType.Remove(pt);
					lstProcessType.Add(db.ProcessType.FirstOrDefault(fod => fod.code.Equals("COL")));
				}

				if (pt.code.Equals("ENT"))
				{
					lstProcessType.Add(db.ProcessType.FirstOrDefault(fod => fod.code.Equals("COL")));
				}
			}

			return lstProcessType;
		}

		
	}
}