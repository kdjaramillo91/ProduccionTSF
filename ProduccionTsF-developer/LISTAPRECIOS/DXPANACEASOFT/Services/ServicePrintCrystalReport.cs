using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DXPANACEASOFT.Models;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DXPANACEASOFT.Services
{
	public class ServicePrintCrystalReport
	{
		public static Stream PrintCRParameters(
			DBContextProd dbContext, string reportCode, List<ParamCR> reportParameters, Conexion conexionInfo)
		{
			// Ubicamos al reporte. Si no existe, retornamos un Stream nulo...
			var pathReportEntity = dbContext
				.tbsysPathReportProduction
				.FirstOrDefault(fod => fod.code == reportCode);

			if (pathReportEntity == null)
			{
				return null;
			}

			// Preparamos la ruta completa del reporte
			var reportPath = Path.Combine(pathReportEntity.path, pathReportEntity.nameReport);

			if (String.IsNullOrEmpty(reportPath))
			{
				return null;
			}

			// Procesamos el reporte...
			using (ReportDocument reportDocument = new ReportDocument())
			{
				// Cargamos el reporte y le asignamos los parámetros enviados...
				reportDocument.Load(reportPath);

				if(reportParameters != null)
				{ 
					foreach (var reportParameter in reportParameters)
					{
						reportDocument.SetParameterValue(
							reportParameter.Nombre, reportParameter.Valor);
					}
				}

				// Preparamos la información de la conexión a la base de datos
				var connectionInfo = new ConnectionInfo
				{
					ServerName = conexionInfo.SrvName,
					DatabaseName = conexionInfo.DbName,
					UserID = conexionInfo.UsrName,
					Password = conexionInfo.PswName,
					IntegratedSecurity = false,
				};

				// Asignamos la conexión de datos a todas las tablas del reporte...
				foreach (Table reportTable in reportDocument.Database.Tables)
				{
					var tableLogoninfo = reportTable.LogOnInfo;
					tableLogoninfo.ConnectionInfo = connectionInfo;
					reportTable.ApplyLogOnInfo(tableLogoninfo);
				}

				// Recorremos las secciones, en busca de los subreportes, para asignarles la conexión de datos...
				foreach (Section reportSection in reportDocument.ReportDefinition.Sections)
				{
					foreach (ReportObject reportObject in reportSection.ReportObjects)
					{
						if (reportObject.Kind == ReportObjectKind.SubreportObject)
						{
							var subreportObject = (SubreportObject)reportObject;
							var subreportDocument = subreportObject.OpenSubreport(subreportObject.SubreportName);

							foreach (Table reportTable in subreportDocument.Database.Tables)
							{
								var tableLogoninfo = reportTable.LogOnInfo;
								tableLogoninfo.ConnectionInfo = connectionInfo;
								reportTable.ApplyLogOnInfo(tableLogoninfo);
							}
						}
					}
				}


				// Generamos un Stream con el resultado del reporte en PDF
				return reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);
			}
		}

		public static Stream PrintExcelParameters(
			DBContextProd dbContext, string reportCode, List<ParamCR> reportParameters, Conexion conexionInfo)
		{
			// Ubicamos al reporte. Si no existe, retornamos un Stream nulo...
			var pathReportEntity = dbContext
				.tbsysPathReportProduction
				.FirstOrDefault(fod => fod.code == reportCode);

			if (pathReportEntity == null)
			{
				return null;
			}

			// Preparamos la ruta completa del reporte
			var reportPath = Path.Combine(pathReportEntity.path, pathReportEntity.nameReport);

			if (String.IsNullOrEmpty(reportPath))
			{
				return null;
			}

			// Procesamos el reporte...
			using (ReportDocument reportDocument = new ReportDocument())
			{
				// Cargamos el reporte y le asignamos los parámetros enviados...
				reportDocument.Load(reportPath);

				foreach (var reportParameter in reportParameters)
				{
					reportDocument.SetParameterValue(
						reportParameter.Nombre, reportParameter.Valor);
				}

				// Preparamos la información de la conexión a la base de datos
				var connectionInfo = new ConnectionInfo
				{
					ServerName = conexionInfo.SrvName,
					DatabaseName = conexionInfo.DbName,
					UserID = conexionInfo.UsrName,
					Password = conexionInfo.PswName,
					IntegratedSecurity = false,
				};

				// Asignamos la conexión de datos a todas las tablas del reporte...
				foreach (Table reportTable in reportDocument.Database.Tables)
				{
					var tableLogoninfo = reportTable.LogOnInfo;
					tableLogoninfo.ConnectionInfo = connectionInfo;
					reportTable.ApplyLogOnInfo(tableLogoninfo);
				}

				// Recorremos las secciones, en busca de los subreportes, para asignarles la conexión de datos...
				foreach (Section reportSection in reportDocument.ReportDefinition.Sections)
				{
					foreach (ReportObject reportObject in reportSection.ReportObjects)
					{
						if (reportObject.Kind == ReportObjectKind.SubreportObject)
						{
							var subreportObject = (SubreportObject)reportObject;
							var subreportDocument = subreportObject.OpenSubreport(subreportObject.SubreportName);

							foreach (Table reportTable in subreportDocument.Database.Tables)
							{
								var tableLogoninfo = reportTable.LogOnInfo;
								tableLogoninfo.ConnectionInfo = connectionInfo;
								reportTable.ApplyLogOnInfo(tableLogoninfo);
							}
						}
					}
				}


				// Generamos un Stream con el resultado del reporte en Excel
				return reportDocument.ExportToStream(ExportFormatType.Excel);
			}
		}
	}
}