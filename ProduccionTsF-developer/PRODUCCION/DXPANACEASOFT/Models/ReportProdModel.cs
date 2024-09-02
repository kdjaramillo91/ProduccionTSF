using System;
using System.Data;
using System.Collections.Generic;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;

namespace DXPANACEASOFT.Models
{
	public class ReportProdModel
	{
		public string codeReport { get; set; }

		public List<ParamCR> paramCRList { get; set; }

		public Conexion conex { get; set; }

		public string nameFile { get; set; }

		public string nameReport { get; set; }

		public string nameDepartment { get; set; }

		public string nameUser { get; set; }
	}
	public class ReportProdFromSystemModel
	{
		public string codeReport { get; set; }
		public DataTable dtSc { get; set; }
		public DataSet dsSc { get; set; }
		public bool hasSubReport { get; set; }
		public string nameFile { get; set; }
		public string nameReport { get; set; }
		public string nameDepartment { get; set; }
		public string nameUser { get; set; }
		public bool toSend { get; set; }
		public string[] subReportNames { get; set; }
	}
}