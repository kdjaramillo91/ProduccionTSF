using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.AdvanceParametersDetailP.AdvanceParametersDetailModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.DataProviders
{
	public static class DataProviderAdvanceParametersDetail
	{
		private static DBContext _db = null;

		public static IQueryable<AdvanceParametersDetailModelP> QueryAdvanceParametersModelPById(DBContext db, int? _idAdvanceParameters)
		{
			return db.AdvanceParametersDetail
						.Where(w => w.id_AdvanceParameters == _idAdvanceParameters)
						.Select(s => new AdvanceParametersDetailModelP
						{
							idAdvanceDetailModelP = s.id,
							codeAdvanceDetailModelP = s.valueCode,
							idAdvanceModelP = s.id_AdvanceParameters,
							nameAdvanceDetailModelP = s.valueName
						});
		}
		public static List<AdvanceParametersModelP> GetListAdvanceParametersModelPByCode(string _code)
		{
			_db = new DBContext();
			return _db.AdvanceParameters
						.Where(w => w.code.Equals(_code))
						.Select(s => new AdvanceParametersModelP
						{
							idAdvanceModelP = s.id,
							codeAdvanceModelP = s.code,
							hasDetailModelP = s.hasDetail,
							valueIntegerModelP = s.valueInteger
						}).ToList();
		}
		public static AdvanceParametersModelP GetAdvanceParameterModelPByCode(string _code)
		{
			_db = new DBContext();
			return _db.AdvanceParameters
						.Where(w => w.code.Equals(_code))
						.Select(s => new AdvanceParametersModelP
						{
							idAdvanceModelP = s.id,
							codeAdvanceModelP = s.code,
							hasDetailModelP = s.hasDetail,
							valueIntegerModelP = s.valueInteger
						}).FirstOrDefault();
		}

		public static IEnumerable GetAdvanceParameterDetailByCode(string code)
		{
			_db = new DBContext();
			return _db.AdvanceParametersDetail
						.Where(w => w.AdvanceParameters.code.Equals(code))
						.Select(s => new AdvanceParametersDetailModelP
						{
							idAdvanceDetailModelP = s.id,
							codeAdvanceDetailModelP = s.valueCode,
							nameAdvanceDetailModelP = s.valueName,
							idAdvanceModelP = s.id_AdvanceParameters
						}).ToList();
		}

		public static AdvanceParametersDetail[] GetAdvanceParameterDetailByCode(string codeAdvanceParameters, string[] codes)
		{
			_db = new DBContext();
			return _db.AdvanceParametersDetail
				.Where(w => w.AdvanceParameters.code.Equals(codeAdvanceParameters))
				.Where(e => codes.Contains(e.valueCode.Trim()))
				.ToArray();
		}
	}
}