using DXPANACEASOFT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderSetting
	{
		private static DBContext db = null;

		public static SettingDetail GetSettingDetailById(int id_Setting)
		{
			db = new DBContext();
			return db.SettingDetail.FirstOrDefault(i => i.id == id_Setting);
		}

		public static string ValueSetting(string code)
		{
			db = new DBContext();
			return db.Setting.FirstOrDefault(i => i.code.Equals(code))?.value;
		}
		public static string ValueSettingDetailByCodes(string codeSet, string codeSetDetail)
		{
			db = new DBContext();
			return db.SettingDetail
						.FirstOrDefault(fod => fod.Setting.code == codeSet &&
										fod.value == codeSetDetail)?.valueAux;
		}

		public static List<SettingDetail> GetSettingDetailBySetting(string code)
		{

			db = new DBContext();

			return db.SettingDetail.Where(r => r.Setting.code == code).ToList();
		}

		public static Setting SettingById(int? id_Setting)
		{
			db = new DBContext(); ;
			return db.Setting.FirstOrDefault(v => v.id == id_Setting);
		}

        public static Setting SettingByCode(string code_Setting)
        {
            db = new DBContext(); ;
            return db.Setting.FirstOrDefault(v => v.code == code_Setting);
        }

        public static IEnumerable getDataType(int? id_current)
		{
			db = new DBContext();
			var model = db.SettingDataType.Where(g => g.isActive || id_current == g.id).Select(p => new
			{
				id = p.id,
				name = p.name,
				code = p.code
			});

			return model.ToList();
		}

		public static IEnumerable getModulos(int? id_current)
		{
			db = new DBContext();
			var model = db.Module.Where(g => g.isActive || id_current == g.id).Select(p =>
			new {
				id = p.id,
				name = p.name,
				code = p.code
			});

			return model.ToList();
		}
	}
}