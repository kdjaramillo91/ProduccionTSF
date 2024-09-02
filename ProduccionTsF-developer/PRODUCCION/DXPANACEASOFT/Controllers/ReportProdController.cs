using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity;
using DXPANACEASOFT.Services;
using System.Diagnostics;
using System.Configuration;
using Utilitarios.Logs;
using OfficeOpenXml;
using DXPANACEASOFT.Models.ProductionLotP.ProductionLotModel;
using OfficeOpenXml.Style;
using System.IO.Packaging;
using DXPANACEASOFT.Models.InventoryMoveDTO.InventoryMovePlantTransfer;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Presentation;
using static DXPANACEASOFT.Models.ProductionLotCloseDTO.Extensions;


namespace DXPANACEASOFT.Controllers
{
	public class ReportProdController : DefaultController
	{
		[HttpGet]
		[Authorize]
		public ActionResult Index()
		{
			string repProd = Convert.ToString(Request.QueryString["trepd"]);

			ReportProdModel model = (TempData[repProd] as ReportProdModel);

			model = model ?? new ReportProdModel();

			Stream str = ServicePrintCrystalReport.PrintCRParameters(db, model.codeReport, model.paramCRList, model.conex);

			TempData.Remove(repProd);

			return File(str, "application/pdf");
		}

		[HttpGet]
		[Authorize]
		public ActionResult Report()
		{
			string repProd = Convert.ToString(Request.QueryString["trepd"]);

			ReportProdFromSystemModel model = (TempData[repProd] as ReportProdFromSystemModel);

			model = model ?? new ReportProdFromSystemModel();

			Stream str = ServicePrintCrystalReport.PrintCR(db, model.codeReport, model.dsSc, model.hasSubReport, model.toSend, ActiveUser);

			TempData.Remove(repProd);

			return File(str, "application/pdf");
		}

		[HttpPost]
		[Authorize]
		public void PrintDirect(string _strq)
		{
			try
			{
				#region VERIFY PATH REPORTS

				string _pathReportTotal = "";
				string _pathFileTotal = "";
				string _NamePrinter = "";
				string _NameFormat = "";
				string _pathReports = ConfigurationManager.AppSettings["pathPrintDirect"];
				string _pathPrintDirect = ConfigurationManager.AppSettings["pathProgramPD"];

				string NameDateFolderReport = DateTime.Now.ToString("yyyyMMdd");
				string NameTimeFolderReport = DateTime.Now.ToString("HHmmss");
				string _strIdUser = ActiveUser.id.ToString();

				string _NamePrinterGuide = ConfigurationManager.AppSettings["PLGuidePrinter"];
				string _NameFormatGuide = ConfigurationManager.AppSettings["PLGuideFormat"];

				string _NamePrinterViatic = ConfigurationManager.AppSettings["PLViaticPrinter"];
				string _NameFormatReporte = ConfigurationManager.AppSettings["PLReporteFormat"];

				string _NamePrinterWarehouse = ConfigurationManager.AppSettings["PLWarehousePrinter"];


				_pathReportTotal = _pathReports + "\\" + NameDateFolderReport;
				_pathFileTotal = _pathReports + "\\" + NameDateFolderReport + "\\" + NameTimeFolderReport + _strIdUser + ".pdf";

				// Crea Directorio.
				if (!Directory.Exists(_pathReportTotal))
				{
					DirectoryInfo di = Directory.CreateDirectory(_pathReportTotal);
				}

				#endregion

				#region GENERATE REPORTS

				ReportProdModel model = (TempData[_strq] as ReportProdModel);

				model = model ?? new ReportProdModel();

				tbsysPathReportProduction _tbsprp = db.tbsysPathReportProduction.FirstOrDefault(fod => fod.code == model.codeReport);

				Stream str = ServicePrintCrystalReport.PrintCRParameters(db, model.codeReport, model.paramCRList, model.conex);

				TempData.Remove(_strq);

				//File(str, "application/pdf", _pathFileTotal);

				using (System.IO.FileStream output = new System.IO.FileStream(_pathFileTotal, FileMode.Create))
				{
					str.CopyTo(output);
				}

				#endregion

				#region SET PRINT INFORMATION 


				if (model.codeReport == "LGRVD1" || model.codeReport == "RGRVD1")
				{
					_NamePrinter = _NamePrinterGuide;
				}
				else if (model.codeReport == "D1GRVC" || model.codeReport == "D1GRVS" || model.codeReport == "F1GRVS" || model.codeReport == "F1GRVC")
				{
					_NamePrinter = _NamePrinterViatic;
				}
				else if (model.codeReport == "D1GRDM")
				{
					_NamePrinter = _NamePrinterWarehouse;
				}


				_NameFormat = ((model.codeReport == "LGRVD1" || model.codeReport == "RGRVD1") ? _NameFormatGuide : _NameFormatReporte);
				#endregion

				#region PRINT REPORT

				ProcessStartInfo startInfo = new ProcessStartInfo();
				startInfo.FileName = _pathPrintDirect;
				startInfo.Arguments = _NamePrinter + " " + _NameFormat + " " + NameDateFolderReport + " " + NameTimeFolderReport + _strIdUser;
				Process.Start(startInfo);

				#endregion
			}
			catch (Exception ex)
			{
				MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, "PrintDirect", "IMPRESION DIRECTA", "PROD");
			}

		}

		[HttpGet]
		[Authorize]
		public ActionResult ToExcel()
		{
			string repProd = Convert.ToString(Request.QueryString["trepd"]);
			ReportProdModel model = (TempData[repProd] as ReportProdModel);
			model = model ?? new ReportProdModel();
			//Stream str = ServicePrintCrystalReport.PrintExcelParameters(db, model.codeReport, model.paramCRList, model.conex);
			Stream str = ServicePrintCrystalReport.PrintCRParameters(db, model.codeReport, model.paramCRList, model.conex);
			str.Seek(0, SeekOrigin.Begin);
			TempData.Remove(repProd);
			return File(str, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ((model.nameReport != "" && model.nameReport != null) ? model.nameReport + ".xls": "Reporte.xls"));

		}

		[HttpGet]
		[Authorize]
		public ActionResult ToExcelDataTable()
		{
			string repProd = Convert.ToString(Request.QueryString["trepd"]);
			ReportProdFromSystemModel model = (TempData[repProd] as ReportProdFromSystemModel);
			model = model ?? new ReportProdFromSystemModel();
			Stream str = ServicePrintCrystalReport.PrintCRExcel(db, model.codeReport, model.dsSc, model.hasSubReport, model.subReportNames, model.toSend, ActiveUser);
			str.Seek(0, SeekOrigin.Begin);
			TempData.Remove(repProd);
			return File(str, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ((model.nameReport != "" && model.nameReport != null) ? model.nameReport + ".xls" : "Reporte.xls"));

		}

		#region EXCEL PERSONALIZADOS
		[HttpGet]
		public ActionResult DownloadExcelResumeProcessDetails()
        {
			var fileName = "Reporte de Procesos Internos.xlsx";
			Stream stream = new MemoryStream();
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var modelExcel = TempData["modelProcessInternalDetailed"] as List<ResultProductLotProcessDetailed>;
			var initialDate = TempData["par_initialDate"];
			var endDate = TempData["par_endDate"];
			var imagenEncriptadaHex = db.Company.Select(x => x.logo).FirstOrDefault();
			string imagePath = Server.MapPath("~/Content/image/logo.png");
			System.IO.File.WriteAllBytes(imagePath, imagenEncriptadaHex);

			

			var distinctRows = modelExcel.Select(x => new { x.PRODLnumberNumerodeLote, 
				x.PRODUNnameNombreUnidadLoteProduccion,
				x.PRODPRnameProcesoLoteProduccion,
				PRODLreceptionDateFechaRecepcion = x.PRODLreceptionDateFechaRecepcion?.ToString("dd/MM/yyyy"),
				x.Estado,
                Rendimiento = x.MERMA,
                Merma = x.MERMA > 99 ? 0 : (x.MERMA - 100) * (-1),
                x.RESPONSABLE
            }).Distinct().OrderBy(o => o.PRODLnumberNumerodeLote).ToArray();



			var spanishColumnNames = new Dictionary<string, string>
			{
				{"PRODLnumberNumerodeLote","No. de Liquidación"},
				{"PRODUNnameNombreUnidadLoteProduccion","Unidad de Producción"},
				{"PRODPRnameProcesoLoteProduccion","Proceso"},
				{"PRODLreceptionDateFechaRecepcion","Fecha"},
				{"Estado","Estado"},
				{"Rendimiento","Rendimiento"},
				{"Merma","Merma"},
				{"RESPONSABLE","Responsable"}
			};

			var detail1ColumnNames = new Dictionary<string, string>
			{
				{ "INTERNALLOT","Sec. Interna"},
				{ "LOTORInumberLoteOrigen","Lote Recibido"},
				{ "ITEMmasterCodeCodigoProducto","Código"},
				{ "ITEMnameNombreItem","Nombre Producto"},
				{ "ITY2nameliqTipoProducto","Tipo Producto"},
				{ "ITEMSZnameTalla","Talla"},
				{ "PRODLDquantityRecivedCantidadRecibidaDetalle","Cantidad"},
				{ "Libras","Libras"},
				{ "Kilos","Kilos"}
			};

			using (var package = new ExcelPackage())
			{
				var worksheet = package.Workbook.Worksheets.Add("Reporte Procesos Internos");

				worksheet.Cells.Style.Font.Name = "Tahoma";
				// Carga la imagen en una celda específica
				var picture = worksheet.Drawings.AddPicture("LogoCia", new FileInfo(imagePath));
				picture.SetPosition(0, 0, 0, 0); // Define el rango de celdas (fila inicial, columna inicial, fila final, columna final)
				picture.SetSize(100, 75); // Establece el tamaño de la imagen (ancho, alto)
				// Establecer el nombre del reporte
				// Unir celdas para crear un encabezado
				worksheet.View.TabSelected = true; // Marcar la hoja como seleccionada
				worksheet.Cells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
				worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

				worksheet.Cells["C3:F3"].Merge = true; // Unir desde A1 hasta B1
				worksheet.Cells["C3:F3"].Style.Font.Bold = true; // Establecer negrita
				worksheet.Cells["C3"].Value = "REPORTE DE PROCESOS INTERNOS"; // Contenido en A1
                worksheet.Cells["C3:F3"].Style.Font.Size = 10;
				worksheet.Cells["C3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
				worksheet.Cells["C3:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				//// Establecer el nombre de la compañía en A1

				worksheet.Cells["G3"].Style.Font.Bold = true;
				worksheet.Cells["G3"].Value = "Página:";
				worksheet.Cells["H3"].Value = "Pagina 1 de 1 ";
				
				worksheet.Cells["G4"].Style.Font.Bold = true;
                worksheet.Cells["G4"].Value = "Fecha:";
                worksheet.Cells["H4"].Value = DateTime.Now.ToString("dd/MM/yyyy");

                worksheet.Cells["G6"].Style.Font.Bold = true;
                worksheet.Cells["G6"].Value = "Hora";
                worksheet.Cells["H6"].Value = DateTime.Now.ToString("HH:mm:ss");

				worksheet.Cells["C7"].Style.Font.Bold = true;
				worksheet.Cells["C7"].Value = "Fecha Desde:";
				worksheet.Cells["D7"].Value = initialDate.ToString();

				worksheet.Cells["E7"].Style.Font.Bold = true;
				worksheet.Cells["E7"].Value = "Fecha Hasta";
				worksheet.Cells["F7"].Value = endDate.ToString();

				worksheet.Cells["G3:H7"].Style.Font.Size = 8;
				worksheet.Cells["C7:F7"].Style.Font.Size = 8;
				int rowIndex = 12; // Empieza en la fila 10

				if (distinctRows.Count() > 0)
				{
					foreach (var headers in distinctRows)
					{
						var columnIndex = 1; // Empieza en la columna 1 (A)

						foreach (var property in headers.GetType().GetProperties())
						{
							if (property.Name != "PRODUNnameNombreUnidadLoteProduccion" && property.Name != "RESPONSABLE"&& property.Name != "PRODPRnameProcesoLoteProduccion")
                            {
								worksheet.Cells[rowIndex, columnIndex].Value = property.GetValue(headers);
								worksheet.Cells[rowIndex, columnIndex].Style.Font.Size = 8;
								columnIndex++; // Avanza a la siguiente columna
                            }
                            else
                            {
								worksheet.Cells[rowIndex, columnIndex, rowIndex, columnIndex + 1].Merge = true;
								worksheet.Cells[rowIndex, columnIndex].Value = property.GetValue(headers);
								worksheet.Cells[rowIndex, columnIndex, rowIndex, columnIndex + 1].Style.Font.Size = 8;
								columnIndex = columnIndex + 2;
							}
								
								
						}
						
						columnIndex = 1;
						foreach (var kvp in spanishColumnNames)
						{
							if (kvp.Value != "Unidad de Producción" && kvp.Value != "Responsable" && kvp.Value != "Proceso")
                            {
								worksheet.Cells[rowIndex - 1, columnIndex].Style.Font.Bold = true;
								worksheet.Cells[rowIndex - 1, columnIndex].Value = kvp.Value;
								worksheet.Cells[rowIndex - 1, columnIndex].Style.Font.Size = 8;
								columnIndex++;
								
							}
                            else
                            {
								worksheet.Cells[rowIndex - 1, columnIndex, rowIndex - 1, columnIndex + 1].Merge = true;
								worksheet.Cells[rowIndex - 1, columnIndex, rowIndex - 1, columnIndex + 1].Style.Font.Bold = true;
								worksheet.Cells[rowIndex - 1, columnIndex].Value = kvp.Value;
								worksheet.Cells[rowIndex - 1, columnIndex, rowIndex - 1, columnIndex + 1].Style.Font.Size = 8;
								columnIndex = columnIndex + 2;
							}
								
							
						}

						var detail1 = modelExcel.Where(x => x.PRODLnumberNumerodeLote == headers.PRODLnumberNumerodeLote && x.ITEMmasterCodeCodigoProducto != "")
							.Select( d1 => new {
								d1.INTERNALLOT,
								d1.LOTORInumberLoteOrigen,
								d1.ITEMmasterCodeCodigoProducto,
								d1.ITEMnameNombreItem,
								d1.ITY2nameliqTipoProducto,
								d1.ITEMSZnameTalla,
								d1.PRODLDquantityRecivedCantidadRecibidaDetalle,
								Libras = d1.PRESEID == 4 ? d1.PMINIMUM * d1.PMAXIMUM * d1.PRODLDquantityRecivedCantidadRecibidaDetalle
														: (d1.PMINIMUM * d1.PMAXIMUM * d1.PRODLDquantityRecivedCantidadRecibidaDetalle) * 2.2046m,
								Kilos = d1.PRESEID == 1 ? d1.PMINIMUM * d1.PMAXIMUM * d1.PRODLDquantityRecivedCantidadRecibidaDetalle
														: (d1.PMINIMUM * d1.PMAXIMUM * d1.PRODLDquantityRecivedCantidadRecibidaDetalle) / 2.2046m,
								d1.MPROLIQ

							}).ToList();

						var document = modelExcel.Where(x => x.PRODLnumberNumerodeLote == headers.PRODLnumberNumerodeLote)
										.Select(d => d.MPROLIQ).Distinct().ToList();

						var cab = 2;
                        foreach (var doc in document)
                        {
							if (!string.IsNullOrEmpty(doc) && (doc == "LIQUIDACION" || doc == "MATERIA PRIMA"))
							{
								worksheet.Cells[rowIndex + cab-1, 1].Value = doc;
								worksheet.Cells[rowIndex + cab-1, 1].Style.Font.Size = 8;
								worksheet.Cells[rowIndex + cab-1, 1].Style.Font.Bold = true;
								var deta = detail1.Where(x => x.MPROLIQ == doc).ToList();
								columnIndex = 1;
								foreach (var kvp in detail1ColumnNames)
								{
									if (kvp.Value == "Nombre Producto")
									{
										worksheet.Cells[rowIndex + cab, columnIndex,rowIndex + cab, columnIndex + 2].Style.Font.Bold = true;
										worksheet.Cells[rowIndex + cab, columnIndex,rowIndex + cab, columnIndex + 2].Merge = true;
										worksheet.Cells[rowIndex + cab, columnIndex].Value = kvp.Value;
										worksheet.Cells[rowIndex + cab, columnIndex,rowIndex + cab, columnIndex + 2].Style.Font.Size = 7;
										columnIndex = columnIndex + 3;

                                    }
                                    else
                                    {
										worksheet.Cells[rowIndex + cab, columnIndex].Style.Font.Bold = true;
										worksheet.Cells[rowIndex + cab, columnIndex].Value = kvp.Value == "Lote Recibido" && doc == "LIQUIDACION"
																					? "Lote Generado" : kvp.Value;
										worksheet.Cells[rowIndex + cab, columnIndex].Style.Font.Size = 7;
										columnIndex++;
									}
									
								}

								columnIndex = 1;
								var det = 1;
								
								foreach (var detail in deta)
                                {
									foreach (var property in detail.GetType().GetProperties())
									{
										if (property.Name != "MPROLIQ")
										{
                                            if (property.Name == "ITEMnameNombreItem")
                                            {
												worksheet.Cells[rowIndex + cab + det, columnIndex, rowIndex + cab + det, columnIndex + 2].Merge = true;
												worksheet.Cells[rowIndex + cab + det, columnIndex].Value = property.GetValue(detail);
												worksheet.Cells[rowIndex + cab + det, columnIndex, rowIndex + cab + det, columnIndex + 2].Style.Font.Size = 7;
												columnIndex = columnIndex + 3;
											}
                                            else
                                            {
												var value = property.GetValue(detail);

												if (value is decimal decimalValue)
												{
													// Es un valor decimal, formatea con dos decimales
													worksheet.Cells[rowIndex + cab + det, columnIndex].Value = decimalValue;
													worksheet.Cells[rowIndex + cab + det, columnIndex].Style.Numberformat.Format = "#,##0.00";
													worksheet.Cells[rowIndex + cab + det, columnIndex].Style.Font.Size = 7;
												}
												else
												{
													// No es un valor decimal, agrégalo tal cual
													worksheet.Cells[rowIndex + cab + det, columnIndex].Value = value;
													worksheet.Cells[rowIndex + cab + det, columnIndex].Style.Font.Size = 7;
												}
												columnIndex++;
											}
											
											
										}
									}
									det++;
									columnIndex = 1;
								}
								var index = rowIndex + cab + det ;
								worksheet.Cells["I" + (index )].Value = "Total Libras Remitidas";
								worksheet.Cells["I" + (index + 1)].Value = doc == "LIQUIDACION" ? "Total Merma" : "Total Desperdicio";
								worksheet.Cells["I" + (index + 2)].Value = "Total Libras Netas";
								worksheet.Cells["J" + (index )].Value = deta.Sum(x => x.Libras);
								worksheet.Cells["J" + (index + 1)].Value = 0;
								worksheet.Cells["J" + (index + 2)].Value = deta.Sum(x => x.Libras);
								worksheet.Cells["k" + (index )].Value = deta.Sum(x => x.Kilos);
								worksheet.Cells["k" + (index + 1)].Value = 0;
								worksheet.Cells["k" + (index + 2)].Value = deta.Sum(x => x.Kilos);
								worksheet.Cells["I" + (index ) + ":K" + (index + 2)].Style.Font.Size = 7;
								worksheet.Cells["I" + (index ) + ":K" + (index + 2)].Style.Font.Bold = true;
								worksheet.Cells["I" + (index ) + ":K" + (index + 2)].Style.Numberformat.Format = "#,##0.00";
								cab = deta.Count() + 6;
							}

						}

						rowIndex += detail1.Count()+ 18;

						/*DETALLE FINAL POR PROCESO */
						worksheet.Cells["A" + (rowIndex - 7)].Value = "Total por Proceso";
						worksheet.Cells["B" + (rowIndex - 7)].Value = distinctRows.Select(b => b.PRODPRnameProcesoLoteProduccion).FirstOrDefault();
						worksheet.Cells["C" + (rowIndex - 7)].Value = "Lbs Remitidas:";
						worksheet.Cells["D" + (rowIndex - 7)].Value = detail1.Where(d => d.MPROLIQ == "MATERIA PRIMA").Sum(s => s.Libras);
						worksheet.Cells["D" + (rowIndex - 7)].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["E" + (rowIndex - 7)].Value = "Lbs Desperdicio:";
						worksheet.Cells["F" + (rowIndex - 7)].Value = detail1.Where(d => d.MPROLIQ == "DESPERDICIO").Sum(s => s.Libras);
						worksheet.Cells["F" + (rowIndex - 7)].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["G" + (rowIndex - 7)].Value = "Lbs Netas:";
						worksheet.Cells["H" + (rowIndex - 7)].Value = detail1.Where(d => d.MPROLIQ == "MATERIA PRIMA").Sum(s => s.Libras);
						worksheet.Cells["H" + (rowIndex - 7)].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["I" + (rowIndex - 7)].Value = "Lbs Procesadas:";
						worksheet.Cells["J" + (rowIndex - 7)].Value = detail1.Where(d => d.MPROLIQ == "LIQUIDACION").Sum(s => s.Libras);
						worksheet.Cells["J" + (rowIndex - 7)].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["K" + (rowIndex - 7)].Value = detail1.Where(d => d.MPROLIQ == "MATERIA PRIMA").Sum(s => s.Libras) > 0 ?
							(detail1.Where(d => d.MPROLIQ == "LIQUIDACION").Sum(s => s.Libras) / detail1.Where(d => d.MPROLIQ == "MATERIA PRIMA").Sum(s => s.Libras)) * 100 : 0;
						worksheet.Cells["K" + (rowIndex - 7)].Style.Numberformat.Format = "#,##0.00";


						worksheet.Cells["A" + (rowIndex - 6)].Value = "Total Unidad de Producción y Proceso";
						worksheet.Cells["A" + (rowIndex - 5)].Value = distinctRows.Select(b => b.PRODUNnameNombreUnidadLoteProduccion).FirstOrDefault();
						worksheet.Cells["A" + (rowIndex - 5) + ":B" + (rowIndex - 5)].Merge = true;
						worksheet.Cells["C" + (rowIndex - 5)].Value = "Lbs Remitidas:";
						worksheet.Cells["D" + (rowIndex - 5)].Value = detail1.Where(d => d.MPROLIQ == "MATERIA PRIMA").Sum(s => s.Libras);
						worksheet.Cells["D" + (rowIndex - 5)].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["E" + (rowIndex - 5)].Value = "Lbs Desperdicio:";
						worksheet.Cells["F" + (rowIndex - 5)].Value = detail1.Where(d => d.MPROLIQ == "DESPERDICIO").Sum(s => s.Libras);
						worksheet.Cells["F" + (rowIndex - 5)].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["G" + (rowIndex - 5)].Value = "Lbs Netas:";
						worksheet.Cells["H" + (rowIndex - 5)].Value = detail1.Where(d => d.MPROLIQ == "MATERIA PRIMA").Sum(s => s.Libras);
						worksheet.Cells["H" + (rowIndex - 5)].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["I" + (rowIndex - 5)].Value = "Lbs Procesadas:";
						worksheet.Cells["J" + (rowIndex - 5)].Value = detail1.Where(d => d.MPROLIQ == "LIQUIDACION").Sum(s => s.Libras);
						worksheet.Cells["J" + (rowIndex - 5)].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells["K" + (rowIndex - 5)].Value = detail1.Where(d => d.MPROLIQ == "MATERIA PRIMA").Sum(s => s.Libras) > 0 ?
							(detail1.Where(d => d.MPROLIQ == "LIQUIDACION").Sum(s => s.Libras) / detail1.Where(d => d.MPROLIQ == "MATERIA PRIMA").Sum(s => s.Libras)) * 100 : 0;
						worksheet.Cells["K" + (rowIndex - 5)].Style.Numberformat.Format = "#,##0.00";


						worksheet.Cells["A" + (rowIndex - 7) + ":K" + (rowIndex - 5)].Style.Font.Size = 7;
						worksheet.Cells["A" + (rowIndex - 7) + ":K" + (rowIndex - 5)].Style.Font.Bold = true;
						
					}

					var detailGeneral = modelExcel.Where(x => x.ITEMmasterCodeCodigoProducto != "")
							.Select(d1 => new {
								Libras = d1.PRESEID == 4 ? d1.PMINIMUM * d1.PMAXIMUM * d1.PRODLDquantityRecivedCantidadRecibidaDetalle
													   : (d1.PMINIMUM * d1.PMAXIMUM * d1.PRODLDquantityRecivedCantidadRecibidaDetalle) * 2.2046m,
								Kilos = d1.PRESEID == 1 ? d1.PMINIMUM * d1.PMAXIMUM * d1.PRODLDquantityRecivedCantidadRecibidaDetalle
													   : (d1.PMINIMUM * d1.PMAXIMUM * d1.PRODLDquantityRecivedCantidadRecibidaDetalle) / 2.2046m,
								d1.MPROLIQ,
								d1.suma,
								d1.sumamerma,
								d1.sumadesperdicio,
								d1.totalliq,
								d1.mermatotalGeneral

							}).ToList();

					worksheet.Cells["A" + (rowIndex + 1)].Value = "Total General";
					
					worksheet.Cells["A" + (rowIndex + 2)].Value = "Total Recibido";
					worksheet.Cells["B" + (rowIndex + 2)].Value = detailGeneral.Where( d => d.MPROLIQ == "LIQUIDACION" || d.MPROLIQ ==  "MATERIA PRIMA").Sum(s => s.Libras);
					worksheet.Cells["B" + (rowIndex + 2)].Style.Numberformat.Format = "#,##0.00";
					
					worksheet.Cells["A" + (rowIndex + 3)].Value = "Total Desperdicio";
					worksheet.Cells["B" + (rowIndex + 3)].Value = detailGeneral.Sum(s => s.sumadesperdicio);
					worksheet.Cells["B" + (rowIndex + 3)].Style.Numberformat.Format = "#,##0.00";

					worksheet.Cells["A" + (rowIndex + 4)].Value = "Total Libras Netas";
					worksheet.Cells["B" + (rowIndex + 4)].Value = detailGeneral.Sum(s => s.suma) - detailGeneral.Sum(s => s.sumadesperdicio);
					worksheet.Cells["B" + (rowIndex + 4)].Style.Numberformat.Format = "#,##0.00";

					worksheet.Cells["A" + (rowIndex + 5)].Value = "Total Procesado";
					worksheet.Cells["B" + (rowIndex + 5)].Value = detailGeneral.Select(m => m.totalliq).FirstOrDefault();
					worksheet.Cells["B" + (rowIndex + 5)].Style.Numberformat.Format = "#,##0.00";

					worksheet.Cells["A" + (rowIndex + 6)].Value = "Total Merma";
					worksheet.Cells["B" + (rowIndex + 6)].Value = detailGeneral.Select(m => m.mermatotalGeneral).FirstOrDefault();
					worksheet.Cells["B" + (rowIndex + 6)].Style.Numberformat.Format = "#,##0.00";

					worksheet.Cells["A" + (rowIndex + 7)].Value = "% Rendimiento";
					worksheet.Cells["B" + (rowIndex + 7)].Value = detailGeneral.Sum(s => s.suma) > 0 ?
						detailGeneral.Sum(s => s.suma) / (detailGeneral.Sum(s => s.suma) - detailGeneral.Sum(s => s.sumadesperdicio)) * 100 : 0;
					worksheet.Cells["B" + (rowIndex + 7)].Style.Numberformat.Format = "#,##0.00";

					worksheet.Cells["A" + (rowIndex +1) + ":B" + (rowIndex + 8)].Style.Font.Size = 7;
					worksheet.Cells["A" + (rowIndex +1) + ":B" + (rowIndex + 8)].Style.Font.Bold = true;
				}



                worksheet.Cells.AutoFitColumns();
                stream = new MemoryStream(package.GetAsByteArray());
				stream.Position = 0;
			}
			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}

        [HttpGet]
        public ActionResult DownloadExcelSizeBuy()
        {
			var fileName = "Compras Por Tallas.xlsx";

			Stream stream = new MemoryStream();
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var modelExcel = TempData["modelProcessSizeBuy"] as List<ResultProductSizeBuy>;
			var parameter = TempData["repMod"] as ReportProdModel;

			var imagenEncriptadaHex = db.Company.Select(x => x.logo).FirstOrDefault();
			string imagePath = Server.MapPath("~/Content/image/logo.png");
			System.IO.File.WriteAllBytes(imagePath, imagenEncriptadaHex);

			using (var package = new ExcelPackage())
            {
				var worksheet = package.Workbook.Worksheets.Add("Compras Por Tallas");
				worksheet.Cells.Style.Font.Name = "Tahoma";

				var picture = worksheet.Drawings.AddPicture("LogoCia", new FileInfo(imagePath));
				picture.SetPosition(0, 0, 0, 0); 
				picture.SetSize(100, 75);

				worksheet.View.TabSelected = true; // Marcar la hoja como seleccionada

				worksheet.Cells["D3:F3"].Merge = true; // Unir desde A1 hasta B1
				worksheet.Cells["D3:F3"].Style.Font.Bold = true; // Establecer negrita
				worksheet.Cells["D3"].Value = "COMPRAS POR TALLAS"; // Contenido en A1
				worksheet.Cells["D3:F3"].Style.Font.Size = 10;
				worksheet.Cells["D3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
				worksheet.Cells["D3:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

				worksheet.Cells["A6"].Style.Font.Bold = true;
				worksheet.Cells["A6"].Value = "Fecha Desde:";
				worksheet.Cells["B6"].Value = parameter.paramCRList[1].Valor.ToString() != "" ? parameter.paramCRList[1].Valor.ToString() : "Todos" ;

				worksheet.Cells["E6"].Style.Font.Bold = true;
				worksheet.Cells["E6"].Value = "Hasta:";
				worksheet.Cells["F6"].Value = parameter.paramCRList[2].Valor.ToString() != "" ? parameter.paramCRList[2].Valor.ToString() : "Todos"; 

				worksheet.Cells["A7"].Style.Font.Bold = true;
				worksheet.Cells["A7"].Value = "Liquidaciones:";
				worksheet.Cells["B7"].Value = modelExcel[0].idestado == 9 ? "PRELIMINAR" :
												modelExcel[0].idestado == 10 ? "DEFINITVA" : "PRELIMINAR Y DEFINITIVA";

				worksheet.Cells["E7"].Style.Font.Bold = true;
				worksheet.Cells["E7"].Value = "Proveedor:";
				worksheet.Cells["F7"].Value = parameter.paramCRList[0].Valor.ToString() != "" ? modelExcel[0].Proveedores : "Todos";


				worksheet.Cells["A9"].Style.Font.Bold = true;
				worksheet.Cells["A9"].Value = "Fecha Emisión:";
				worksheet.Cells["B9"].Value = DateTime.Now.ToString("dd/MM/yyyy");

				worksheet.Cells["C9"].Style.Font.Bold = true;
				worksheet.Cells["C9"].Value = "Hora Emisión:";
				worksheet.Cells["D9"].Value = DateTime.Now.ToString("HH:mm:ss");

				worksheet.Cells["E10"].Style.Font.Bold = true;
				worksheet.Cells["E10"].Value = "Comisionista:";
				worksheet.Cells["F10"].Value =modelExcel[0].Comisionista;

				worksheet.Cells["A12:G12"].Style.Font.Bold = true;
				worksheet.Cells["A6:G12"].Style.Font.Size = 8;
				worksheet.Cells["A12"].Value = "Nombre Producto";
				worksheet.Cells["B12"].Value = "Rendimiento";
				worksheet.Cells["C12"].Value = "Valor $";
				worksheet.Cells["D12"].Value = "Prec. Prom $";
				worksheet.Cells["E12"].Value = "Valor $";
				worksheet.Cells["F12"].Value = "Prec. Ref $";
				worksheet.Cells["G12"].Value = "Margen $";


				var tipo = modelExcel.Select(x => new { x.AgruparTipoProducto, x.TipoProducto }).Distinct().ToList();
				var rowIndex = 13;
                foreach (var types in tipo)
                {
					
					worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
					worksheet.Cells[rowIndex, 1].Style.Font.Size = 8;
					worksheet.Cells[rowIndex, 1].Value = types.TipoProducto;

					var categories = modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto)
									.Select(c => new { c.AgruparCategoriaProducto, c.CategoriaProducto }).Distinct().ToList();


                    foreach (var category in categories)
                    {
						worksheet.Cells[rowIndex + 1, 1].Style.Font.Bold = true;
						worksheet.Cells[rowIndex + 1, 1].Style.Font.Size = 8;
						worksheet.Cells[rowIndex + 1, 1].Value = category.CategoriaProducto;


						var deta = modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto && d.AgruparCategoriaProducto == category.AgruparCategoriaProducto)
						.GroupBy(t => t.AgruparTallas) // Aquí usamos el ID de las tallas para agrupar
						.Select(de => new
						{
							Talla = de.Select(a => a.Talla).FirstOrDefault(),
							Rendimiento = de.Sum(item => item.Rendimiento),
							Valor = de.Sum(item => item.Valor),
							Promedio = de.Average(item => item.Valor / item.Rendimiento),
							PrecioRendimiento = de.Sum(item => item.PrecioRendimiento),
							PrecioRef = de.Average(item => item.PrecioRef),
							Margen = de.Average(item => item.Valor / item.Rendimiento) - de.Average(item => item.PrecioRef)
						})
						.ToList();

						var rowIndexDet = 2;

						foreach (var detail in deta)
                        {
							var columnIndex = 1;
							foreach (var property in detail.GetType().GetProperties())
                            {
								var value = property.GetValue(detail);

								if (value is decimal decimalValue)
								{
									// Es un valor decimal, formatea con dos decimales
									worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Value = decimalValue;
									worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Style.Numberformat.Format = "#,##0.00";
									worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Style.Font.Size = 7;
								}
								else
								{
									// No es un valor decimal, agrégalo tal cual
									worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Value = value;
									worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Style.Font.Size = 7;
								}
								columnIndex++;
								
							}
							rowIndexDet++;
						}
						rowIndex = rowIndex + rowIndexDet;

						worksheet.Cells[rowIndex, 2].Value = deta.Sum(x=> x.Rendimiento);
						worksheet.Cells[rowIndex, 2].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells[rowIndex, 2].Style.Font.Size = 7;

						worksheet.Cells[rowIndex, 3].Value = deta.Sum(x => x.Valor);
						worksheet.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells[rowIndex, 3].Style.Font.Size = 7;

						worksheet.Cells[rowIndex, 4].Value = deta.Sum(x => x.Valor) /deta.Sum(x => x.Rendimiento);
						worksheet.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells[rowIndex, 4].Style.Font.Size = 7;

						worksheet.Cells[rowIndex, 5].Value = deta.Sum(x => x.PrecioRendimiento);
						worksheet.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells[rowIndex, 5].Style.Font.Size = 7;

						worksheet.Cells[rowIndex, 6].Value = deta.Sum(x => x.PrecioRendimiento) / deta.Sum(x => x.Rendimiento);
						worksheet.Cells[rowIndex, 6].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells[rowIndex, 6].Style.Font.Size = 7;

						worksheet.Cells[rowIndex, 7].Value = (deta.Sum(x => x.Valor) / deta.Sum(x => x.Rendimiento)) -
												(deta.Sum(x => x.PrecioRendimiento) / deta.Sum(x => x.Rendimiento));
						worksheet.Cells[rowIndex, 7].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells[rowIndex, 7].Style.Font.Size = 7;

					}

					
					worksheet.Cells[rowIndex + 1, 2].Value = modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto).Sum(s => s.Rendimiento);
					worksheet.Cells[rowIndex + 1, 2].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[rowIndex + 1, 2].Style.Font.Size = 7;
					worksheet.Cells[rowIndex + 1, 2].Style.Font.Bold = true;

					worksheet.Cells[rowIndex + 1, 3].Value = modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto).Sum(s => s.Valor);
					worksheet.Cells[rowIndex + 1, 3].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[rowIndex + 1, 3].Style.Font.Size = 7;
					worksheet.Cells[rowIndex + 1, 3].Style.Font.Bold = true;

					worksheet.Cells[rowIndex + 1, 4].Value = modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto).Sum(s => s.Valor)/
														modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto).Sum(s => s.Rendimiento);
					worksheet.Cells[rowIndex + 1, 4].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[rowIndex + 1, 4].Style.Font.Size = 7;
					worksheet.Cells[rowIndex + 1, 4].Style.Font.Bold = true;

					worksheet.Cells[rowIndex + 1, 5].Value = modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto).Sum(s => s.PrecioRendimiento);
					worksheet.Cells[rowIndex + 1, 5].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[rowIndex + 1, 5].Style.Font.Size = 7;
					worksheet.Cells[rowIndex + 1, 5].Style.Font.Bold = true;

					worksheet.Cells[rowIndex + 1, 6].Value = modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto).Sum(s => s.PrecioRendimiento)/
															modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto).Sum(s => s.Rendimiento);
					worksheet.Cells[rowIndex + 1, 6].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[rowIndex + 1, 6].Style.Font.Size = 7;
					worksheet.Cells[rowIndex + 1, 6].Style.Font.Bold = true;

					worksheet.Cells[rowIndex + 1, 7].Value = (modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto).Sum(s => s.Valor) /
														modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto).Sum(s => s.Rendimiento)) -
														modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto).Sum(s => s.PrecioRendimiento) /
															modelExcel.Where(d => d.AgruparTipoProducto == types.AgruparTipoProducto).Sum(s => s.Rendimiento);
					worksheet.Cells[rowIndex + 1, 7].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[rowIndex + 1, 7].Style.Font.Size = 7;
					worksheet.Cells[rowIndex + 1, 7].Style.Font.Bold = true;

					rowIndex = rowIndex + 2;

				}

				worksheet.Cells[rowIndex, 2].Value = modelExcel.Sum(s => s.Rendimiento);
				worksheet.Cells[rowIndex, 2].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 2].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 2].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 3].Value = modelExcel.Sum(s => s.Valor);
				worksheet.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 3].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 4].Value = modelExcel.Sum(s => s.Valor) / modelExcel.Sum(s => s.Rendimiento);
				worksheet.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 4].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 5].Value = modelExcel.Sum(s => s.Valor);
				worksheet.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 5].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 6].Value = modelExcel.Sum(s => s.PrecioRendimiento) / modelExcel.Sum(s => s.Rendimiento);
				worksheet.Cells[rowIndex, 6].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 6].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 7].Value = (modelExcel.Sum(s => s.Valor) / modelExcel.Sum(s => s.Rendimiento)) - modelExcel.Sum(s => s.PrecioRendimiento) / modelExcel.Sum(s => s.Rendimiento);
				worksheet.Cells[rowIndex, 7].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 7].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;

				worksheet.Cells.AutoFitColumns();
				stream = new MemoryStream(package.GetAsByteArray());
				stream.Position = 0;
			}
			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}

		[HttpGet]
		public ActionResult DownloadExcelSupplierLiquidationResumen()
		{
			var fileName = "Resumen De Liquidaciones por Proveedor.xlsx";

			Stream stream = new MemoryStream();
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var modelExcel = TempData["modelLiquidationSupplier"] as List<ResultSupplierLiquidationResumen>;
			var modelExcelResumen = TempData["modelLiquidationSupplieRresume"] as List<ResultSupplierLiquidationResumen>;
			var parameter = TempData["repModLS"] as ReportProdModel;

			var imagenEncriptadaHex = db.Company.Select(x => x.logo).FirstOrDefault();
			string imagePath = Server.MapPath("~/Content/image/logo.png");
			System.IO.File.WriteAllBytes(imagePath, imagenEncriptadaHex);

			using (var package = new ExcelPackage())
			{
				var worksheet = package.Workbook.Worksheets.Add("Resumen De Liquidaciones por Proveedor");
				worksheet.Cells.Style.Font.Name = "Tahoma";

				var picture = worksheet.Drawings.AddPicture("LogoCia", new FileInfo(imagePath));
				picture.SetPosition(0, 0, 0, 0);
				picture.SetSize(100, 75);

				worksheet.View.TabSelected = true; // Marcar la hoja como seleccionada

				worksheet.Cells["D3:H3"].Merge = true; // Unir desde A1 hasta B1
				worksheet.Cells["D3:H3"].Style.Font.Bold = true; // Establecer negrita
				worksheet.Cells["D3"].Value = "RESUMEN DE LIQUIDACIONES POR PROVEEDOR"; // Contenido en A1
				worksheet.Cells["D3:H3"].Style.Font.Size = 10;
				worksheet.Cells["D3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
				worksheet.Cells["D3:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

				worksheet.Cells["A5"].Style.Font.Bold = true;
				worksheet.Cells["A5"].Value = "Fecha Desde:";
				worksheet.Cells["B5"].Value = parameter.paramCRList[1].Valor.ToString() != "" ? parameter.paramCRList[1].Valor.ToString() : "Todos";

				worksheet.Cells["E5"].Style.Font.Bold = true;
				worksheet.Cells["E5"].Value = "Hasta:";
				worksheet.Cells["F5"].Value = parameter.paramCRList[1].Valor.ToString() != "" ? parameter.paramCRList[1].Valor.ToString() : "Todos";

				worksheet.Cells["A7"].Style.Font.Bold = true;
				worksheet.Cells["A7"].Value = "Fecha Emisión:";
				worksheet.Cells["B7"].Value = DateTime.Now.ToString("dd/MM/yyyy");

				worksheet.Cells["A8:Q9"].Style.Font.Bold = true;
				worksheet.Cells["A5:Q9"].Style.Font.Size = 8;
				worksheet.Cells["A8:Q9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
				worksheet.Cells["A8:Q9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Cells["D8:F8"].Merge = true;
				worksheet.Cells["D8"].Value = "Libras";
				worksheet.Cells["G8"].Value = "Kilos";
				worksheet.Cells["H8"].Value = "Libras";
				worksheet.Cells["I8:J8"].Merge = true;
				worksheet.Cells["I8"].Value = "% Rendimiento";
				worksheet.Cells["K8:M8"].Merge = true;
				worksheet.Cells["K8"].Value = "Valores Pagados en Dolares";
				worksheet.Cells["N8:P8"].Merge = true;
				worksheet.Cells["N8"].Value = "Precio Promedio";

				worksheet.Cells["A9"].Value = "Num.Liq";
				worksheet.Cells["A9:B9"].Merge = true;
				worksheet.Cells["C9"].Value = "Proveedor";
				worksheet.Cells["D9"].Value = "Despachadas";
				worksheet.Cells["E9"].Value = "Recibidas";
				worksheet.Cells["F9"].Value = "Procesadas";
				worksheet.Cells["G9"].Value = "Cabeza";
				worksheet.Cells["H9"].Value = "Cola";
				worksheet.Cells["I9"].Value = "Cabeza";
				worksheet.Cells["J9"].Value = "Cola";
				worksheet.Cells["K9"].Value = "Cabeza";
				worksheet.Cells["L9"].Value = "Cola";
				worksheet.Cells["M9"].Value = "Total";
				worksheet.Cells["N9"].Value = "Cabeza";
				worksheet.Cells["O9"].Value = "Cola";
				worksheet.Cells["P9"].Value = "Total";


				var rowIndex = 10;
				var rowIndexDet = 1;
				foreach (var det in modelExcel)
				{
						var columnIndex = 1;
						foreach (var property in det.GetType().GetProperties())
						{
							var value = property.GetValue(det);

							if (value is decimal decimalValue)
							{
								// Es un valor decimal, formatea con dos decimales
								worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Value = decimalValue;
								worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Style.Numberformat.Format = "#,##0.00";
								worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Style.Font.Size = 7;
							}
							else
							{
								// No es un valor decimal, agrégalo tal cual
								worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Value = value;
								worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Style.Font.Size = 7;
							}
							columnIndex++;

						}
						rowIndexDet++;

				}

				rowIndex = rowIndex + rowIndexDet + 1 ;

				worksheet.Cells[rowIndex, 3].Value = "Total";
				worksheet.Cells[rowIndex, 3].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 4].Value = modelExcel.Sum(s => s.LibrasDespachadas);
				worksheet.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 4].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 5].Value = modelExcel.Sum(s => s.LibrasRemitidas);
				worksheet.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 5].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 6].Value = modelExcel.Sum(s => s.LibrasProcesadas);
				worksheet.Cells[rowIndex, 6].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 6].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 7].Value = modelExcel.Sum(s => s.KilosCabeza);
				worksheet.Cells[rowIndex, 7].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 7].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 8].Value = modelExcel.Sum(s => s.LibrasCola);
				worksheet.Cells[rowIndex, 8].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 8].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 9].Value = modelExcel.Sum(s => s.RendimientoCabeza);
				worksheet.Cells[rowIndex, 9].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 9].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 10].Value = modelExcel.Sum(s => s.RendimientoCola);
				worksheet.Cells[rowIndex, 10].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 10].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 11].Value = modelExcel.Sum(s => s.ValorCabeza);
				worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 11].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 11].Style.Font.Bold = true;


				worksheet.Cells[rowIndex, 12].Value = modelExcel.Sum(s => s.ValorCola);
				worksheet.Cells[rowIndex, 12].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 12].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 12].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 13].Value = modelExcel.Sum(s => s.dolaresTotal);
				worksheet.Cells[rowIndex, 13].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 13].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 13].Style.Font.Bold = true;


				worksheet.Cells[rowIndex + 2, 1].Value = "RESUMEN";
				worksheet.Cells[rowIndex + 2, 1].Style.Font.Size = 9;
				worksheet.Cells[rowIndex + 2, 1].Style.Font.Bold = true;


				rowIndex = rowIndex + 3;

				worksheet.Cells["A" + rowIndex + ":Q" + (rowIndex + 1)].Style.Font.Bold = true;
				worksheet.Cells["A" + rowIndex + ":Q" + (rowIndex + 1)].Style.Font.Size = 8;
				worksheet.Cells["A" + rowIndex + ":Q" + (rowIndex + 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
				worksheet.Cells["A" + rowIndex + ":Q" + (rowIndex + 1)].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Cells["C" + rowIndex + ":E" + rowIndex].Merge = true;
				worksheet.Cells["C" + rowIndex].Value = "Libras";
				worksheet.Cells["F" + rowIndex].Value = "Kilos";
				worksheet.Cells["G" + rowIndex].Value = "Libras";
				worksheet.Cells["H" + rowIndex + ":I" + rowIndex].Merge = true;
				worksheet.Cells["H" + rowIndex].Value = "% Rendimiento";
				worksheet.Cells["J" + rowIndex + ":L" + rowIndex].Merge = true;
				worksheet.Cells["J" + rowIndex].Value = "Valores Pagados en Dolares";
				worksheet.Cells["M" + rowIndex + ":O" + rowIndex].Merge = true;
				worksheet.Cells["M" + rowIndex].Value = "Precio Promedio";

				worksheet.Cells["A" + (rowIndex + 1)].Value = "Num.Liq";
				worksheet.Cells["B" + (rowIndex + 1)].Value = "Proveedor";
				worksheet.Cells["C" + (rowIndex + 1)].Value = "Despachadas";
				worksheet.Cells["D" + (rowIndex + 1)].Value = "Recibidas";
				worksheet.Cells["E" + (rowIndex + 1)].Value = "Procesadas";
				worksheet.Cells["F" + (rowIndex + 1)].Value = "Cabeza";
				worksheet.Cells["G" + (rowIndex + 1)].Value = "Cola";
				worksheet.Cells["H" + (rowIndex + 1)].Value = "Cabeza";
				worksheet.Cells["I" + (rowIndex + 1)].Value = "Cola";
				worksheet.Cells["J" + (rowIndex + 1)].Value = "Cabeza";
				worksheet.Cells["K" + (rowIndex + 1)].Value = "Cola";
				worksheet.Cells["L" + (rowIndex + 1)].Value = "Total";
				worksheet.Cells["M" + (rowIndex + 1)].Value = "Cabeza";
				worksheet.Cells["N" + (rowIndex + 1)].Value = "Cola";
				worksheet.Cells["O" + (rowIndex + 1)].Value = "Total";

				rowIndex = rowIndex + 2;
				rowIndexDet = 1;
				foreach (var det in modelExcelResumen)
				{
					var columnIndex = 1;
					foreach (var property in det.GetType().GetProperties())
					{
						var value = property.GetValue(det);

                        if (property.Name != "etiquetaComprador")
                        {
							if (value is decimal decimalValue)
							{
								// Es un valor decimal, formatea con dos decimales
								worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Value = decimalValue;
								worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Style.Numberformat.Format = "#,##0.00";
								worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Style.Font.Size = 7;
							}
							else
							{
								// No es un valor decimal, agrégalo tal cual
								worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Value = value;
								worksheet.Cells[rowIndex + rowIndexDet, columnIndex].Style.Font.Size = 7;
							}
							columnIndex++;
						}

					}
					rowIndexDet++;

				}

				rowIndex = rowIndex + rowIndexDet + 1;

				worksheet.Cells[rowIndex, 2].Value = "Total";
				worksheet.Cells[rowIndex, 2].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 2].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 3].Value = modelExcelResumen.Sum(s => s.LibrasDespachadas);
				worksheet.Cells[rowIndex, 3].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 3].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 4].Value = modelExcelResumen.Sum(s => s.LibrasRemitidas);
				worksheet.Cells[rowIndex, 4].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 4].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 5].Value = modelExcelResumen.Sum(s => s.LibrasProcesadas);
				worksheet.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 5].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 6].Value = modelExcelResumen.Sum(s => s.KilosCabeza);
				worksheet.Cells[rowIndex, 6].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 6].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 7].Value = modelExcelResumen.Sum(s => s.LibrasCola);
				worksheet.Cells[rowIndex, 7].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 7].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 8].Value = modelExcelResumen.Sum(s => s.RendimientoCabeza);
				worksheet.Cells[rowIndex, 8].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 8].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 9].Value = modelExcelResumen.Sum(s => s.RendimientoCola);
				worksheet.Cells[rowIndex, 9].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 9].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 10].Value = modelExcelResumen.Sum(s => s.ValorCabeza);
				worksheet.Cells[rowIndex, 10].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 10].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;


				worksheet.Cells[rowIndex, 11].Value = modelExcelResumen.Sum(s => s.ValorCola);
				worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 11].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 11].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 12].Value = modelExcelResumen.Sum(s => s.dolaresTotal);
				worksheet.Cells[rowIndex, 12].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 12].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 12].Style.Font.Bold = true;



				worksheet.Cells.AutoFitColumns();
				stream = new MemoryStream(package.GetAsByteArray());
				stream.Position = 0;
			}
			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}

		[HttpGet]
		public ActionResult DownloadExcelResumeProcessDetailsInternal()
        {

			var fileName = "Reporte de Procesos Internos Resumido.xlsx";
			Stream stream = new MemoryStream();
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var modelExcel = TempData["modelProcessInternalDetailed"] as List<ResultProductLotProcessDetailed>;
			var initialDate = TempData["par_initialDate"];
			var endDate = TempData["par_endDate"];
			var imagenEncriptadaHex = db.Company.Select(x => x.logo).FirstOrDefault();
			string imagePath = Server.MapPath("~/Content/image/logo.png");
			System.IO.File.WriteAllBytes(imagePath, imagenEncriptadaHex);


			using (var package = new ExcelPackage())
			{
				var worksheet = package.Workbook.Worksheets.Add("Reporte Procesos Internos Resumido");

				worksheet.Cells.Style.Font.Name = "Tahoma";

				var picture = worksheet.Drawings.AddPicture("LogoCia", new FileInfo(imagePath));
				picture.SetPosition(0, 0, 0, 0);
				picture.SetSize(100, 75); 

				worksheet.View.TabSelected = true;

				worksheet.Cells["D3:H3"].Merge = true; // Unir desde A1 hasta B1
				worksheet.Cells["D3:H3"].Style.Font.Bold = true; // Establecer negrita
				worksheet.Cells["D3"].Value = "RESUMEN DEPROCESOS INTERNOS RESUMIDO"; // Contenido en A1
				worksheet.Cells["D3:H3"].Style.Font.Size = 10;
				worksheet.Cells["D3:H3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
				worksheet.Cells["D3:H3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

				
				worksheet.Cells["J3:K4"].Style.Font.Size = 8;
				worksheet.Cells["D5"].Style.Font.Bold = true;
				worksheet.Cells["D5"].Value = "Fecha Desde:";
				worksheet.Cells["E5"].Value = initialDate.ToString();

				worksheet.Cells["H5"].Style.Font.Bold = true;
				worksheet.Cells["H5"].Value = "Fecha Hasta:";
				worksheet.Cells["I5"].Value = endDate.ToString();

				worksheet.Cells["J3"].Style.Font.Bold = true;
				worksheet.Cells["J3"].Value = "Pagina:";
				worksheet.Cells["K3"].Value = "Page 1 of 1";

				worksheet.Cells["J4"].Style.Font.Bold = true;
				worksheet.Cells["J4"].Value = "Fecha:";
				worksheet.Cells["K4"].Value = DateTime.Now.ToString("dd/MM/yyyy");

				worksheet.Cells["J5"].Style.Font.Bold = true;
				worksheet.Cells["J5"].Value = "Hora:";
				worksheet.Cells["K5"].Value = DateTime.Now.ToString("HH:mm:ss");


				worksheet.Cells["A5:K7"].Style.Font.Size = 8;
				worksheet.Cells["A7:K7"].Style.Font.Bold = true;
				worksheet.Cells["A7:B7"].Merge = true;
				worksheet.Cells["A7"].Value = "UNIDAD DE PRODUCCION";
				worksheet.Cells["C7:D7"].Merge = true;
				worksheet.Cells["C7"].Value = "PROCESO";
				worksheet.Cells["E7"].Value = "LBS RECIBIDAS";
				worksheet.Cells["F7"].Value = "LBS DESPERDICIO";
				worksheet.Cells["G7"].Value = "LBS LIQUIDADAS";
				worksheet.Cells["H7"].Value = "LBS MERMA";
				worksheet.Cells["I7"].Value = "DIFERENCIA EN PESO";
				worksheet.Cells["J7"].Value = "RENDIMIENTO";
				worksheet.Cells["K7"].Value = "MERMA";

				

				var dataDetail = modelExcel
						.GroupBy(t => new { t.Estado, t.PRODUNnameNombreUnidadLoteProduccion, t.PRODPRnameProcesoLoteProduccion})
						.Select(de => new
						{
							Estado = de.Select(a => a.Estado).FirstOrDefault(),
							PRODUNnameNombreUnidadLoteProduccion = de.Select(a => a.PRODUNnameNombreUnidadLoteProduccion).FirstOrDefault(),
							Proceso = de.Select(a => a.PRODPRnameProcesoLoteProduccion).FirstOrDefault(),
							suma = de.Sum(item => item.suma),
							sumadesperdicio = de.Sum(item => item.sumadesperdicio),
							sumaliquidacion = de.Sum(item => item.sumaliquidacion),
							sumamerma = de.Sum(item => item.sumamerma),
							Diff = de.Sum(item => item.sumaliquidacion - item.suma),
							Rendimiento = de.Sum(item => item.suma - item.sumadesperdicio) == 0 ? 0 :
							(de.Sum(item => item.sumaliquidacion) / de.Sum(item => item.suma - item.sumadesperdicio)) * 100,
							Merma = de.Sum(item => item.suma - item.sumadesperdicio) == 0 ? 0 :
							(((de.Sum(item => item.sumaliquidacion) / de.Sum(item => item.suma - item.sumadesperdicio)) * 100) - 100) *(-1),
						})
						.ToList();



				var distinctStatus = dataDetail.Select(s => s.Estado).Distinct().ToList();
				var rowIndex = 8;

                foreach (var det in distinctStatus)
                {

                    worksheet.Cells["A" + rowIndex].Value = det;
                    worksheet.Cells["A" + rowIndex].Style.Font.Size = 8;

                    var loteProd = dataDetail.Where(l => l.Estado == det).Select(s=> new { s.Estado, s.PRODUNnameNombreUnidadLoteProduccion })
						.Distinct().ToList();


                    foreach (var lot in loteProd)
                    {
                        rowIndex = rowIndex + 1;
                        worksheet.Cells["A" + rowIndex].Value = lot.PRODUNnameNombreUnidadLoteProduccion;
                        worksheet.Cells["A" + rowIndex].Style.Font.Size = 8;

                        var process = dataDetail.Where(p => p.Estado == det
                                    && p.PRODUNnameNombreUnidadLoteProduccion == lot.PRODUNnameNombreUnidadLoteProduccion)
							.Select( s=> new { 
							s.Proceso,
							s.suma,
							s.sumadesperdicio,
							s.sumaliquidacion,
							s.sumamerma,
							s.Diff,
							s.Rendimiento,
							s.Merma
							}).ToList();

                        rowIndex = rowIndex + 1;
                        foreach (var pro in process)
                        {
                            var columnIndex = 4;
                            foreach (var property in pro.GetType().GetProperties())
                            {
                                var value = property.GetValue(pro);

                                if (value is decimal decimalValue)
                                {
                                    // Es un valor decimal, formatea con dos decimales
                                    worksheet.Cells[rowIndex, columnIndex].Value = decimalValue;
                                    worksheet.Cells[rowIndex, columnIndex].Style.Font.Size = 8;

                                    if (property.Name == "Rendimiento" || property.Name == "Merma" )
                                    {
										worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#,##0.00\\%";
									}
                                    else
                                    {
										worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#,##0.00";
									}

                                }
                                else
                                {
                                    // No es un valor decimal, agrégalo tal cual
                                    worksheet.Cells[rowIndex, columnIndex].Value = value;
                                    worksheet.Cells[rowIndex, columnIndex].Style.Font.Size = 8;
                                }
                                columnIndex++;

                            }
							rowIndex++;

						}


                        worksheet.Cells[rowIndex, 1].Value = "TOTAL DE ESTADO " + det;
                        worksheet.Cells[rowIndex, 1].Style.Font.Size = 8;
                        worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;

                        worksheet.Cells[rowIndex, 5].Value = process.Sum(s => s.suma);
                        worksheet.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[rowIndex, 5].Style.Font.Size = 7;
                        worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;

                        worksheet.Cells[rowIndex, 6].Value = process.Sum(s => s.sumadesperdicio);
                        worksheet.Cells[rowIndex, 6].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[rowIndex, 6].Style.Font.Size = 7;
                        worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;

                        worksheet.Cells[rowIndex, 7].Value = process.Sum(s => s.sumaliquidacion);
                        worksheet.Cells[rowIndex, 7].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[rowIndex, 7].Style.Font.Size = 7;
                        worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;

                        worksheet.Cells[rowIndex, 8].Value = process.Sum(s => s.sumamerma);
                        worksheet.Cells[rowIndex, 8].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[rowIndex, 8].Style.Font.Size = 7;
                        worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;

                        worksheet.Cells[rowIndex, 9].Value = process.Sum(item => item.sumaliquidacion - item.suma);
                        worksheet.Cells[rowIndex, 9].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[rowIndex, 9].Style.Font.Size = 7;
                        worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;

                        worksheet.Cells[rowIndex, 10].Value = (process.Sum(s => s.sumaliquidacion) / (process.Sum(x => x.sumadesperdicio - x.suma))) * 100 * (-1);
                        worksheet.Cells[rowIndex, 10].Style.Numberformat.Format = "#,##0.00\\%";
                        worksheet.Cells[rowIndex, 10].Style.Font.Size = 7;
                        worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;

                        worksheet.Cells[rowIndex, 11].Value = ((((process.Sum(s => s.sumaliquidacion)) / (process.Sum(x => x.suma - x.sumadesperdicio))) * 100) - 100) * (-1);
                        worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0.00\\%";
                        worksheet.Cells[rowIndex, 11].Style.Font.Size = 7;
                        worksheet.Cells[rowIndex, 11].Style.Font.Bold = true;
                        //E

                        rowIndex = rowIndex + 1;

                    }

				}

				worksheet.Cells[rowIndex, 1].Value = "TOTAL GENERAL";
				worksheet.Cells[rowIndex, 1].Style.Font.Size = 8;
				worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 5].Value = dataDetail.Sum(s => s.suma);
				worksheet.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 5].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 6].Value = dataDetail.Sum(s => s.sumadesperdicio);
				worksheet.Cells[rowIndex, 6].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 6].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 7].Value = dataDetail.Sum(s => s.sumaliquidacion);
				worksheet.Cells[rowIndex, 7].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 7].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 7].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 8].Value = dataDetail.Sum(s => s.sumamerma);
				worksheet.Cells[rowIndex, 8].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 8].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 9].Value = dataDetail.Sum(item => item.sumaliquidacion - item.suma);
				worksheet.Cells[rowIndex, 9].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells[rowIndex, 9].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 10].Value = (dataDetail.Sum(s => s.sumaliquidacion) / (dataDetail.Sum(x => x.sumadesperdicio - x.suma))) * 100 * (-1);
				worksheet.Cells[rowIndex, 10].Style.Numberformat.Format = "#,##0.00\\%";
				worksheet.Cells[rowIndex, 10].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;

				worksheet.Cells[rowIndex, 11].Value = ((((dataDetail.Sum(s => s.sumaliquidacion)) / (dataDetail.Sum(x => x.suma - x.sumadesperdicio))) * 100) - 100) * (-1);
				worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0.00\\%";
				worksheet.Cells[rowIndex, 11].Style.Font.Size = 7;
				worksheet.Cells[rowIndex, 11].Style.Font.Bold = true;



				worksheet.Cells.AutoFitColumns();
				stream = new MemoryStream(package.GetAsByteArray());
				stream.Position = 0;
			}

			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}

		[HttpGet]
		public ActionResult DownloadExcelSalesQuotationExterior()
        {
			var fileName = "Proforma.xlsx";
			Stream stream = new MemoryStream();
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var modelExcel = TempData["modelSalesQuotationExterior"] as List<ResultSalesQuotationExterior>;
			var imagenEncriptadaHex = db.Company.Select(x => x.logo).FirstOrDefault();
			string imagePath = Server.MapPath("~/Content/image/logo.png");
			System.IO.File.WriteAllBytes(imagePath, imagenEncriptadaHex);

			using (var package = new ExcelPackage())
            {
				var worksheet = package.Workbook.Worksheets.Add("Proforma");

				worksheet.Cells.Style.Font.Name = "Tahoma";

				var picture = worksheet.Drawings.AddPicture("LogoCia", new FileInfo(imagePath));
				picture.SetPosition(0, 0, 0, 0);
				picture.SetSize(100, 75);

				worksheet.View.TabSelected = true; // Marcar la hoja como seleccionada
				worksheet.Cells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
				worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
				worksheet.Cells.Style.Font.Size = 7;

				worksheet.Cells["C1:J1"].Merge = true; // Unir desde A1 hasta B1
				worksheet.Cells["C1:J1"].Style.Font.Bold = true; // Establecer negrita
				worksheet.Cells["C1"].Value = modelExcel[0].NombreCia; // Contenido en A1
				worksheet.Cells["C1:J1"].Style.Font.Size = 11;
				worksheet.Cells["C1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
				worksheet.Cells["C1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


				worksheet.Cells["D2:F2"].Merge = true; // Unir desde A1 hasta B1
				worksheet.Cells["D2:G2"].Style.Font.Bold = true; // Establecer negrita
				worksheet.Cells["D2"].Value = modelExcel[0].DireccionCia + " - "; // Contenido en A1
				worksheet.Cells["G2"].Value = modelExcel[0].TelefonoCia ; // Contenido en A1
				worksheet.Cells["E3:F3"].Merge = true;
				worksheet.Cells["E3:F3"].Style.Font.Bold = true;
				worksheet.Cells["E3"].Value = "GUAYAQUIL - ECUADOR";

				worksheet.Cells["A5:K5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

				worksheet.Cells["A6"].Style.Font.Size = 12;
				worksheet.Cells["A6"].Style.Font.Bold = true;
				worksheet.Cells["A6"].Value = "PROFORMA";
				worksheet.Cells["A6:B6"].Merge = true;
				worksheet.Cells["C6:F6"].Merge = true;
				worksheet.Cells["C6"].Value = modelExcel[0].OrdenPedido;
				worksheet.Cells["G6"].Value = "Date:";
				worksheet.Cells["H6"].Value = modelExcel[0].FechaEmisión.ToString("dd/MM/yyyy");

				worksheet.Cells["A7:K7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

				worksheet.Cells["A9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells["A9"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
				worksheet.Cells["A9"].Style.Font.Color.SetColor(System.Drawing.Color.White);
				worksheet.Cells["A9"].Style.Font.Bold = true;
				worksheet.Cells["A9"].Value = "Buyer:";

				worksheet.Cells["A10"].Value = "Names:";
				worksheet.Cells["A10:A15"].Style.Font.Bold = true;
				worksheet.Cells["B10"].Value = modelExcel[0].RazonSocialSoldTo;
				worksheet.Cells["B11"].Value = modelExcel[0].USCISoldTo;
				worksheet.Cells["B10:F10"].Merge = true;
				worksheet.Cells["B11:F11"].Merge = true;
				worksheet.Cells["B12:F12"].Merge = true;
				worksheet.Cells["B13:F13"].Merge = true;
				worksheet.Cells["B14:F14"].Merge = true;
				worksheet.Cells["B15:F15"].Merge = true;

				worksheet.Cells["A12"].Value = "Address 1:";
				worksheet.Cells["B12"].Value = modelExcel[0].AddressSoldTo1;

				worksheet.Cells["A13"].Value = "City:";
				worksheet.Cells["B13"].Value = modelExcel[0].CitySoldTo;

				worksheet.Cells["A14"].Value = "Country :";
				worksheet.Cells["B14"].Value = modelExcel[0].CountrySoldTo;

				worksheet.Cells["A15"].Value = "Phone 1:";
				worksheet.Cells["B15"].Value = modelExcel[0].Telefono2SoldTo;




				worksheet.Cells["G9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells["G9"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
				worksheet.Cells["G9"].Style.Font.Color.SetColor(System.Drawing.Color.White);
				worksheet.Cells["G9"].Style.Font.Bold = true;
				worksheet.Cells["G9"].Value = "Notify:";

				worksheet.Cells["G10"].Value = "Names:";
				worksheet.Cells["G10:G15"].Style.Font.Bold = true;
				worksheet.Cells["H10"].Value = modelExcel[0].Notificador;
				worksheet.Cells["H10:K10"].Merge = true;
				worksheet.Cells["H11:K11"].Merge = true;
				worksheet.Cells["H12:K12"].Merge = true;
				worksheet.Cells["H13:K13"].Merge = true;
				worksheet.Cells["H14:K14"].Merge = true;
				worksheet.Cells["H15:K15"].Merge = true;

				worksheet.Cells["G12"].Value = "Address 1:";
				worksheet.Cells["H12"].Value = modelExcel[0].direccionNotif;

				worksheet.Cells["G13"].Value = "City:";
				worksheet.Cells["H13"].Value = modelExcel[0].Citynotif;

				worksheet.Cells["G14"].Value = "Country:";
				worksheet.Cells["H14"].Value = modelExcel[0].Countrynotif;

				worksheet.Cells["G15"].Value = "Phone 1:";
				worksheet.Cells["H15"].Value = modelExcel[0].Telefono1notif;

				worksheet.Cells["A16:K16"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

				worksheet.Cells["A18"].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells["A18"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
				worksheet.Cells["A18"].Style.Font.Color.SetColor(System.Drawing.Color.White);
				worksheet.Cells["A18"].Style.Font.Bold = true;
				worksheet.Cells["A18"].Value = "Consignee:";

				
				worksheet.Cells["A19:A23"].Style.Font.Bold = true;
				worksheet.Cells["B19:F19"].Merge = true;
				worksheet.Cells["B20:F20"].Merge = true;
				worksheet.Cells["B21:F21"].Merge = true;
				worksheet.Cells["B22:F22"].Merge = true;
				worksheet.Cells["B23:F23"].Merge = true;


				worksheet.Cells["A19"].Value = "Names:";
				worksheet.Cells["B19"].Value = modelExcel[0].RazonSocialShipTo;
				worksheet.Cells["A20"].Value = "Address 1:";
				worksheet.Cells["B20"].Value = modelExcel[0].AddressShipTo1;
				worksheet.Cells["A21"].Value = "City:";
				worksheet.Cells["B21"].Value = modelExcel[0].CityShipTo;
				worksheet.Cells["A22"].Value = "Country:";
				worksheet.Cells["B22"].Value = modelExcel[0].CountryShipTo;
				worksheet.Cells["A23"].Value = "Phone 1:";
				worksheet.Cells["B23"].Value = modelExcel[0].Telefono1ShipTo;


				worksheet.Cells["G19:G23"].Style.Font.Bold = true;
				worksheet.Cells["H19:K19"].Merge = true;
				worksheet.Cells["H20:K20"].Merge = true;
				worksheet.Cells["H21:K21"].Merge = true;
				worksheet.Cells["H23:K23"].Merge = true;


				worksheet.Cells["G19"].Value = "Vessel:";
				worksheet.Cells["H19"].Value = modelExcel[0].vessel;
				worksheet.Cells["G20"].Value = "Shipment Date:";
				worksheet.Cells["H20"].Value = modelExcel[0].ShipmentDate;
				worksheet.Cells["G21:G22"].Value = "Port of departure:";
				worksheet.Cells["G21:G22"].Merge = true;
				worksheet.Cells["G23"].Value = "Port of discharge:";
				worksheet.Cells["H23"].Value = modelExcel[0].PuertoDestino;

				worksheet.Cells["A24:K24"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

				worksheet.Cells["A26"].Value = "Payment:";
				worksheet.Cells["A26"].Style.Font.Bold = true;
				worksheet.Cells["B26"].Value = modelExcel[0].METODOPAGO + "/" + modelExcel[0].PlazoPago ;
				worksheet.Cells["B26:K26"].Merge = true;

				worksheet.Cells["A27"].Value = "Description:";
				worksheet.Cells["A27"].Style.Font.Bold = true;
				worksheet.Cells["B27"].Value = modelExcel[0].Descripción;
				worksheet.Cells["B27:K27"].Merge = true;

				var rowIndex = 30;

				var detaild = modelExcel.Select(d => new { 
				d.descripcion,
				d.size,
				d.Cartones,
				d.Cantidad,
				d.Precio,
				Total = d.Cantidad * d.Precio
				}).ToList();

				worksheet.Cells["A29:E29"].Merge = true;
				worksheet.Cells["H29:I29"].Merge = true;
				worksheet.Cells["A29:K29"].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells["A29:K29"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
				worksheet.Cells["A29:K29"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
				worksheet.Cells["A29:K29"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
				worksheet.Cells["A29:K29"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
				worksheet.Cells["A29:K29"].Style.Font.Bold = true;

				worksheet.Cells["A29"].Value = "Product Description";
				worksheet.Cells["F29"].Value = "Size";
				worksheet.Cells["G29"].Value = "Cases";
				worksheet.Cells["H29"].Value = "Net Weight";
				worksheet.Cells["J29"].Value = "$ USD CFR";
				worksheet.Cells["K29"].Value = "Total";


				foreach (var pro in detaild)
				{
					var columnIndex = 1;
					foreach (var property in pro.GetType().GetProperties())
					{
						var value = property.GetValue(pro);

						if (value is decimal decimalValue)
						{
							worksheet.Cells[rowIndex, columnIndex].Value = decimalValue;
							worksheet.Cells[rowIndex, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[rowIndex, columnIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, columnIndex].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, columnIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, columnIndex].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#,##0.00";
							// Es un valor decimal, formatea con dos decimales
							if (property.Name == "Cantidad")
							{
								worksheet.Cells[rowIndex, columnIndex, rowIndex, columnIndex + 1].Merge = true;
								worksheet.Cells[rowIndex, columnIndex, rowIndex, columnIndex + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
								worksheet.Cells[rowIndex, columnIndex, rowIndex, columnIndex + 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[rowIndex, columnIndex, rowIndex, columnIndex + 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[rowIndex, columnIndex, rowIndex, columnIndex + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[rowIndex, columnIndex, rowIndex, columnIndex + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#,##0.00";
								columnIndex = columnIndex + 1;

							}


						}
						else
						{
							worksheet.Cells[rowIndex, columnIndex].Value = value;
							worksheet.Cells[rowIndex, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[rowIndex, columnIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, columnIndex].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, columnIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, columnIndex].Style.Border.Right.Style = ExcelBorderStyle.Thin;

							if (property.Name == "descripcion")
                            {
								worksheet.Cells[rowIndex, columnIndex , rowIndex, columnIndex + 4].Merge = true;
								worksheet.Cells[rowIndex, columnIndex , rowIndex, columnIndex + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
								worksheet.Cells[rowIndex, columnIndex , rowIndex, columnIndex + 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[rowIndex, columnIndex , rowIndex, columnIndex + 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[rowIndex, columnIndex , rowIndex, columnIndex + 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[rowIndex, columnIndex , rowIndex, columnIndex + 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
								columnIndex = columnIndex + 4;

							}
                            else
                            {
								worksheet.Cells[rowIndex, columnIndex ].Merge = true;
							}
							// No es un valor decimal, agrégalo tal cual
							

						}
						columnIndex++;

					}
					rowIndex++;

				}

				worksheet.Cells["A" + rowIndex +":F" + rowIndex].Merge = true;
				worksheet.Cells["A" + rowIndex +":K" + rowIndex].Style.Font.Bold = true;
				worksheet.Cells["A" + rowIndex +":K" + rowIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells["A" + rowIndex +":K" + rowIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
				worksheet.Cells["A" + rowIndex +":K" + rowIndex].Style.Border.Left.Style = ExcelBorderStyle.Thin;
				worksheet.Cells["A" + rowIndex +":K" + rowIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
				worksheet.Cells["A" + rowIndex +":K" + rowIndex].Style.Border.Right.Style = ExcelBorderStyle.Thin;

				worksheet.Cells["G" + rowIndex ].Value = detaild.Sum(d => d.Cartones);
				worksheet.Cells["G" + rowIndex].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells["I" + rowIndex ].Value = detaild.Sum(d => d.Cantidad);
				worksheet.Cells["I" + rowIndex].Style.Numberformat.Format = "#,##0.00";
				worksheet.Cells["K" + rowIndex ].Value = detaild.Sum(d => d.Total);
				worksheet.Cells["K" + rowIndex].Style.Numberformat.Format = "#,##0.00";

				rowIndex++;

                worksheet.Cells["G" + rowIndex].Value = "TOTAL";
                worksheet.Cells["G" + rowIndex].Style.Font.Bold = true;
				worksheet.Cells["H" + rowIndex].Value = modelExcel[0].TerminoNegociación;
                worksheet.Cells["I" + rowIndex + ":J" + rowIndex].Merge = true;
                worksheet.Cells["I" + rowIndex].Value = modelExcel[0].PuertoDestino;
				worksheet.Cells["K" + rowIndex].Style.Font.Bold = true;
				worksheet.Cells["K" + rowIndex].Value = detaild.Sum(d => d.Total);
				worksheet.Cells["K" + rowIndex].Style.Numberformat.Format = "#,##0.00";

				rowIndex++;

				worksheet.Cells["B" + rowIndex].Value = "Total Ammount:";
				worksheet.Cells["B" + rowIndex + ":C" + rowIndex].Merge = true;
				worksheet.Cells["B" + rowIndex + ":K" + rowIndex].Style.Font.Bold = true;
				worksheet.Cells["D" + rowIndex + ":K" + rowIndex].Merge = true;
				worksheet.Cells["D" + rowIndex].Value = modelExcel[0].Total;

				rowIndex++;
				worksheet.Cells["A" + rowIndex + ":K" + rowIndex].Merge = true;
				worksheet.Cells["A" + rowIndex + ":K" + rowIndex].Style.Font.Bold = true;
				worksheet.Cells["A" + rowIndex].Value= "COUNTRY OF ORIGIN OF PRODUCT: ECUADOR";

				rowIndex++;
				worksheet.Cells["A" + rowIndex + ":K" + (rowIndex + 1)].Style.Font.Bold = true;
				worksheet.Cells["A" + rowIndex + ":K" + (rowIndex + 1)].Merge = true;
				worksheet.Cells["A" + rowIndex].Value = modelExcel[0].leyenda;


				rowIndex = rowIndex + 2;
				worksheet.Cells["A" + rowIndex + ":C" + rowIndex].Style.Font.Bold = true;
				worksheet.Cells["A" + rowIndex + ":C" + rowIndex].Merge = true;
				worksheet.Cells["A" + rowIndex].Value = "PAYMENT INSTRUCCIONS";

				rowIndex++;

				string[] etiquetasYValores = modelExcel[0].BankTransferInfo.Split(new string[] { "\r\n" }, StringSplitOptions.None);


				for (int i = 0; i < etiquetasYValores.Length; i ++)
				{
					string etiqueta = etiquetasYValores[i].Trim();
                    worksheet.Cells["A" + rowIndex + ":H" + rowIndex].Merge = true;
                    worksheet.Cells["A" + rowIndex].Value = etiqueta ;

					rowIndex++;
				}


				worksheet.Cells.AutoFitColumns();
				stream = new MemoryStream(package.GetAsByteArray());
				stream.Position = 0;
			}

			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}

		[HttpGet]
		public ActionResult DownLoadExcelInvoiceComercial()
        {
			var fileName = "Factura Comercial.xlsx";
			Stream stream = new MemoryStream();
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var modelExcel = TempData["modelInvoiceComercial"] as List<ResultInvoiceComercial>;
			var imagenEncriptadaHex = db.Company.Select(x => x.logo).FirstOrDefault();
			string imagePath = Server.MapPath("~/Content/image/logo.png");
			System.IO.File.WriteAllBytes(imagePath, imagenEncriptadaHex);

			var companyName = db.Company.Where(c => c.isActive).Select(x => new { x.id, x.trademark, x.address, x.phoneNumber }).FirstOrDefault();


			using (var package = new ExcelPackage())
			{
				var worksheet = package.Workbook.Worksheets.Add("Factura Comercial");

				worksheet.Cells.Style.Font.Name = "Tahoma";

                var picture = worksheet.Drawings.AddPicture("LogoCia", new FileInfo(imagePath));
                picture.SetPosition(0, 0, 0, 0);
                picture.SetSize(100, 75);

                worksheet.View.TabSelected = true; // Marcar la hoja como seleccionada
                worksheet.Cells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                worksheet.Cells.Style.Font.Size = 7;

                worksheet.Cells["C1:J1"].Merge = true; // Unir desde A1 hasta B1
                worksheet.Cells["C1:J1"].Style.Font.Bold = true; // Establecer negrita
                worksheet.Cells["C1"].Value = companyName.trademark; // Contenido en A1
                worksheet.Cells["C1:J1"].Style.Font.Size = 11;
                worksheet.Cells["C1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
                worksheet.Cells["C1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                worksheet.Cells["D2:F2"].Merge = true; // Unir desde A1 hasta B1
                worksheet.Cells["D2:G2"].Style.Font.Bold = true; // Establecer negrita
                worksheet.Cells["D2"].Value = companyName.address + " - "; // Contenido en A1
                worksheet.Cells["G2"].Value = companyName.phoneNumber; // Contenido en A1
                worksheet.Cells["E3:F3"].Merge = true;
                worksheet.Cells["E3:F3"].Style.Font.Bold = true;
                worksheet.Cells["E3"].Value = "GUAYAQUIL - ECUADOR";

                worksheet.Cells["A5:K5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                worksheet.Cells["A7"].Style.Font.Bold = true;
                worksheet.Cells["A7"].Value = "Date";
                worksheet.Cells["C6"].Value = modelExcel[0].emissionDateformat.ToString("dd/MM/yyyy");
                worksheet.Cells["C6:I6"].Merge = true;
                worksheet.Cells["C6:I6"].Style.Font.Bold = true;
                worksheet.Cells["C6"].Value = "Commercial Invoice # " + modelExcel[0].documento + " / " + "Sale Confirmation # " + modelExcel[0].InvComm_orden_pedido;


                worksheet.Cells["A7:K7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A9"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                worksheet.Cells["A9"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells["A9"].Style.Font.Bold = true;
                worksheet.Cells["A9"].Value = "Buyer:";
                worksheet.Cells["B9"].Value = modelExcel[0].fullname_businessName;


                worksheet.Cells["A10:A15"].Style.Font.Bold = true;
                worksheet.Cells["B10"].Value = modelExcel[0].USCI;
                worksheet.Cells["B10:F10"].Merge = true;
                worksheet.Cells["B11:F11"].Merge = true;
                worksheet.Cells["B12:F12"].Merge = true;
                worksheet.Cells["B13:F13"].Merge = true;
                worksheet.Cells["B14:F14"].Merge = true;
                worksheet.Cells["B15:F15"].Merge = true;

                worksheet.Cells["A11"].Value = "Address:";
                worksheet.Cells["B11"].Value = modelExcel[0].direccion;


                worksheet.Cells["A13"].Value = "Phone:";
                worksheet.Cells["B13"].Value = modelExcel[0].telefono;




                worksheet.Cells["G9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["G9"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                worksheet.Cells["G9"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells["G9"].Style.Font.Bold = true;
                worksheet.Cells["G9"].Value = "Notify:";
                worksheet.Cells["H9"].Value = modelExcel[0].notificador;

                worksheet.Cells["G10:G15"].Style.Font.Bold = true;
                worksheet.Cells["H10:K10"].Merge = true;
                worksheet.Cells["H11:K11"].Merge = true;
                worksheet.Cells["H12:K12"].Merge = true;
                worksheet.Cells["H13:K13"].Merge = true;
                worksheet.Cells["H14:K14"].Merge = true;
                worksheet.Cells["H15:K15"].Merge = true;

                worksheet.Cells["G11"].Value = "Address:";
                worksheet.Cells["H11"].Value = modelExcel[0].direccionnotif;


                worksheet.Cells["G13"].Value = "Phone:";
                worksheet.Cells["H13"].Value = modelExcel[0].TelefonoNotif;

                worksheet.Cells["A14:K14"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A16"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A16"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                worksheet.Cells["A16"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                worksheet.Cells["A16"].Style.Font.Bold = true;
                worksheet.Cells["A16"].Value = "Consignee:";
                worksheet.Cells["B16"].Value = modelExcel[0].consignador;

                worksheet.Cells["A17:A21"].Style.Font.Bold = true;
                worksheet.Cells["B17:F17"].Merge = true;
                worksheet.Cells["B18:F18"].Merge = true;
                worksheet.Cells["B19:F19"].Merge = true;
                worksheet.Cells["B20:F20"].Merge = true;
                worksheet.Cells["B21:F21"].Merge = true;


                worksheet.Cells["B17"].Value = modelExcel[0].USCIconsig;
                worksheet.Cells["A18"].Value = "Address:";
                worksheet.Cells["B18"].Value = modelExcel[0].direccconsigna;
                worksheet.Cells["A19"].Value = "Phone:";
                worksheet.Cells["B19"].Value = modelExcel[0].Telefonoconsig;
                worksheet.Cells["A20"].Value = "Packer/MFR:";
                worksheet.Cells["B20"].Value = companyName.trademark;
                worksheet.Cells["B21"].Value = companyName.address + " - Guayaquil, Ecuador";
                worksheet.Cells["B22"].Value = "FDA. REGISTRATION NO. OF PACKER: 15722396588";


                worksheet.Cells["G19:G23"].Style.Font.Bold = true;
                worksheet.Cells["H19:K19"].Merge = true;
                worksheet.Cells["H20:K20"].Merge = true;
                worksheet.Cells["H21:K21"].Merge = true;
                worksheet.Cells["H23:K23"].Merge = true;


                worksheet.Cells["G16"].Value = "Vessel:";
                worksheet.Cells["H16"].Value = modelExcel[0].BuquemasViaje;
                worksheet.Cells["G17"].Value = "Shipment Date:";
                worksheet.Cells["H17"].Value = modelExcel[0].InvComm_fecha_embarque;
                worksheet.Cells["G18"].Value = "Port of departure:";
                worksheet.Cells["H18"].Value = modelExcel[0].Portofdeparture;
                worksheet.Cells["G19"].Value = "Final Destination:";
                worksheet.Cells["H19"].Value = modelExcel[0].Portofdestination;
                worksheet.Cells["G20"].Value = "Shipping Line:";
                worksheet.Cells["H20"].Value = modelExcel[0].Shipping_Line;
                worksheet.Cells["G21"].Value = "B/L No.:";
                worksheet.Cells["H21"].Value = modelExcel[0].InvComm_numero_bl;
                worksheet.Cells["G21"].Value = "Container No.:";
                worksheet.Cells["H21"].Value = modelExcel[0].numcontenedor;


                worksheet.Cells["A23:K23"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells["A24"].Value = "Payment:";
                worksheet.Cells["A24"].Style.Font.Bold = true;
                worksheet.Cells["B24"].Value = modelExcel[0].descripformapago + "-" + modelExcel[0].plazo;
                worksheet.Cells["B24:K24"].Merge = true;

                worksheet.Cells["A25"].Value = "Description:";
                worksheet.Cells["A25"].Style.Font.Bold = true;
                worksheet.Cells["B25"].Value = modelExcel[0].Good;
                worksheet.Cells["B25:K25"].Merge = true;

                var rowIndex = 29;

                var detaild = modelExcel.Select(d => new
                {
                    d.Item_Origen,
                    d.Talla_Origen,
                    d.InvCommDet_cantidad_cajas,
                    d.InvCommDet_cantidad,
                    d.InvCommDet_precio_unitario,
                    Total = d.InvCommDet_valor_total
                }).ToList();

                worksheet.Cells["A27:E28"].Merge = true;
                worksheet.Cells["G27:I27"].Merge = true;
                worksheet.Cells["A27:K28"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A27:K28"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A27:K28"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A27:K28"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A27:K28"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells["A27:K28"].Style.Font.Bold = true;

                worksheet.Cells["A27"].Value = "Product Description";
                worksheet.Cells["F27"].Value = "Size";
                worksheet.Cells["G27"].Value = "Quantity";
                worksheet.Cells["G28"].Value = "CN TS";
                worksheet.Cells["H28"].Value = "WEIGHT";
                worksheet.Cells["J28"].Value = "PROCE FOB";
                worksheet.Cells["K28"].Value = "TOTAL USD";


                foreach (var pro in detaild)
                {
                    var columnIndex = 1;
                    foreach (var property in pro.GetType().GetProperties())
                    {
                        var value = property.GetValue(pro);

                        if (value is decimal decimalValue)
                        {
                            worksheet.Cells[rowIndex, columnIndex].Value = decimalValue;
                            worksheet.Cells[rowIndex, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[rowIndex, columnIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[rowIndex, columnIndex].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[rowIndex, columnIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[rowIndex, columnIndex].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#,##0.00";
                            // Es un valor decimal, formatea con dos decimales
                            if (property.Name == "InvCommDet_cantidad")
                            {
                                worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 1)].Merge = true;
                                worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 1)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 1)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 1)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#,##0.00";
                                columnIndex = columnIndex + 1;

                            }


                        }
                        else
                        {
                            worksheet.Cells[rowIndex, columnIndex].Value = value;
                            worksheet.Cells[rowIndex, columnIndex].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[rowIndex, columnIndex].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[rowIndex, columnIndex].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[rowIndex, columnIndex].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            worksheet.Cells[rowIndex, columnIndex].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            if (property.Name == "Item_Origen")
                            {
                                worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 4)].Merge = true;
                                worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 4)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 4)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 4)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 4)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 4)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                columnIndex = columnIndex + 4;

                            }
                            else
                            {
                                worksheet.Cells[rowIndex, columnIndex].Merge = true;
                            }
                            // No es un valor decimal, agrégalo tal cual


                        }
                        columnIndex++;

                    }
                    rowIndex++;

                }

                worksheet.Cells["D" + rowIndex].Value = "TOTAL";
                worksheet.Cells["G" + rowIndex].Value = detaild.Sum(d => d.InvCommDet_cantidad_cajas);
                worksheet.Cells["G" + rowIndex].Style.Numberformat.Format = "#,##0.00";
                //worksheet.Cells["I" + rowIndex + ":J" + rowIndex].Merge = true;
                worksheet.Cells["I" + rowIndex].Value = detaild.Sum(d => d.InvCommDet_cantidad);
                worksheet.Cells["I" + rowIndex].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells["K" + rowIndex].Value = detaild.Sum(d => d.Total);
                worksheet.Cells["K" + rowIndex].Style.Numberformat.Format = "#,##0.00";

                rowIndex++;

                worksheet.Cells["H" + rowIndex].Value = "Total :";
                worksheet.Cells["H" + rowIndex].Style.Font.Bold = true;
                worksheet.Cells["K" + rowIndex].Style.Font.Bold = true;
                worksheet.Cells["K" + rowIndex].Value = detaild.Sum(d => d.Total);
                worksheet.Cells["K" + rowIndex].Style.Numberformat.Format = "#,##0.00";

                rowIndex++;
                worksheet.Cells["A" + rowIndex + ":K" + rowIndex].Merge = true;
                worksheet.Cells["A" + rowIndex + ":K" + rowIndex].Style.Font.Bold = true;
                worksheet.Cells["A" + rowIndex].Value = "Total Ammount: " + modelExcel[0].letras;

                rowIndex++;
                worksheet.Cells["A" + rowIndex + ":K" + rowIndex].Merge = true;
                worksheet.Cells["A" + rowIndex + ":K" + rowIndex].Style.Font.Bold = true;
                worksheet.Cells["A" + rowIndex].Value = "COUNTRY OF ORIGIN OF PRODUCT: ECUADOR";

                rowIndex++;
                worksheet.Cells["A" + rowIndex + ":C" + rowIndex].Style.Font.Bold = true;
                worksheet.Cells["A" + rowIndex + ":C" + rowIndex].Merge = true;
                worksheet.Cells["A" + rowIndex].Value = "PAYMENT INSTRUCCIONS";

                rowIndex++;

                string[] etiquetasYValores = modelExcel[0].BankTransferInfo.Split(new string[] { "\r\n" }, StringSplitOptions.None);


                for (int i = 0; i < etiquetasYValores.Length; i++)
                {
                    string etiqueta = etiquetasYValores[i].Trim();
                    worksheet.Cells["A" + rowIndex + ":H" + rowIndex].Merge = true;
                    worksheet.Cells["A" + rowIndex].Value = etiqueta;

                    rowIndex++;
                }


                worksheet.Cells.AutoFitColumns();
				stream = new MemoryStream(package.GetAsByteArray());
				stream.Position = 0;
			}

			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

		}

		[HttpGet]
		public ActionResult DownLoadExcelRemisionGuieT()
        {
			var fileName = "Guias de Remision.xlsx";
			Stream stream = new MemoryStream();
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var modelExcel = TempData["modelRemisionGuieT"] as List<ResultRemisionGuieT>;
			var imagenEncriptadaHex = db.Company.Select(x => x.logo).FirstOrDefault();
			string imagePath = Server.MapPath("~/Content/image/logo.png");
			System.IO.File.WriteAllBytes(imagePath, imagenEncriptadaHex);

			var companyName = db.Company.Where(c => c.isActive).Select(x => new { x.id, x.businessName,x.ruc, x.address, x.phoneNumber }).FirstOrDefault();
			using (var package = new ExcelPackage())
			{
				var worksheet = package.Workbook.Worksheets.Add("Guias de Remision");

				worksheet.Cells.Style.Font.Name = "Tahoma";

				var picture = worksheet.Drawings.AddPicture("LogoCia", new FileInfo(imagePath));
				picture.SetPosition(0, 0, 0, 0);
				picture.SetSize(100, 75);

				worksheet.View.TabSelected = true;
				worksheet.Cells.Style.Font.Size = 7;

				worksheet.Cells["E3:L3"].Merge = true; // Unir desde A1 hasta B1
				worksheet.Cells["E3:L3"].Style.Font.Bold = true; // Establecer negrita
				worksheet.Cells["E3:L3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells["E3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
				worksheet.Cells["E3:L3"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
				worksheet.Cells["E3"].Value = "GUIAS DE REMISION"; // Contenido en A1
				worksheet.Cells["E3:L3"].Style.Font.Size = 10;
				worksheet.Cells["E3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
				worksheet.Cells["E3:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


				worksheet.Cells["E4"].Value = modelExcel.Count() == 0 ? companyName.businessName : modelExcel[0]?.NombreCompania;

				worksheet.Cells["E5"].Style.Font.Bold = true;
				worksheet.Cells["E5"].Value = "#RUC:";
				worksheet.Cells["F5"].Value = modelExcel.Count() == 0 ? companyName.ruc : modelExcel[0]?.RucCompania;

				worksheet.Cells["E6"].Style.Font.Bold = true;
				worksheet.Cells["E6"].Value = "Telf.:";
				worksheet.Cells["F6"].Value = modelExcel.Count() == 0 ? companyName.phoneNumber : modelExcel[0]?.TelefonoCompania;

				worksheet.Cells["K4"].Style.Font.Bold = true;
				worksheet.Cells["K4"].Value = "Fecha:";
				worksheet.Cells["L4"].Value = DateTime.Now.ToString("dd/MM/yyyy");

				worksheet.Cells["K5"].Style.Font.Bold = true;
				worksheet.Cells["K5"].Value = "Hora:";
				worksheet.Cells["L5"].Value = DateTime.Now.ToString("HH:mm:ss");

				worksheet.Cells["K6"].Style.Font.Bold = true;
				worksheet.Cells["K6"].Value = "Pagina:";
				worksheet.Cells["L6"].Value = "Pagina 1 of 1";


				worksheet.Cells["A7:M7"].Style.Font.Size = 8;
				worksheet.Cells["A7:M7"].Style.Font.Bold = true;
				worksheet.Cells["A7"].Value = "#";
				worksheet.Cells["B7"].Value = "No. de Guia";
				worksheet.Cells["C7"].Value = "Planta Proceso";
				worksheet.Cells["D7"].Value	= "Fecha de Emision";
				worksheet.Cells["E7:F7"].Merge = true;
				worksheet.Cells["E7"].Value = "Proveedor Camaronera";
				worksheet.Cells["G7"].Value = "Fecha de Despacho";
				worksheet.Cells["H7"].Value = "Libras Programadas";
				worksheet.Cells["I7"].Value = "Libras Remitidas";
				worksheet.Cells["J7:K7"].Merge = true;
				worksheet.Cells["J7"].Value = "Comprador";
				worksheet.Cells["L7"].Value = "Tipo";
				worksheet.Cells["M7"].Value = "Estado";



				


				if(modelExcel.Count() > 0)
                {
					var distinctStatus = modelExcel.Select(s => s.PlantaProceso).Distinct().ToList();
					var rowIndex = 8;
					var count = 1;
					foreach (var det in distinctStatus)
					{
						worksheet.Cells["A" + rowIndex + ":B" + rowIndex].Merge = true;
						worksheet.Cells["A" + rowIndex].Value = "Planta Proceso";
						worksheet.Cells["A" + rowIndex].Style.Font.Bold = true;
						worksheet.Cells["C" + rowIndex].Value = det;


						var process = modelExcel.Where(l => l.PlantaProceso == det)
							.Select((s, index) => new {
								Count = index + 1,
								s.Secuencial,
								s.PlantaProceso,
								FechaEmision = s.FechaEmision.ToString("dd/MM/yyyy"),
								s.ProveedorCompleto,
								s.FechaDespacho,
								s.LibrasProgramadas,
								s.LibrasRemitidas,
								s.Comprador,
								s.TipoGuia,
								s.Estado
							}).ToList();



						rowIndex = rowIndex + 1;

						foreach (var pro in process)
						{
							var columnIndex = 1;
							foreach (var property in pro.GetType().GetProperties())
							{
								var value = property.GetValue(pro);

								if (value is decimal decimalValue)
								{
									// Es un valor decimal, formatea con dos decimales
									worksheet.Cells[rowIndex, columnIndex].Value = decimalValue;
									worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#,##0.00";
								}
								else
								{
									worksheet.Cells[rowIndex, columnIndex].Value = value;
									if (property.Name == "ProveedorCompleto" || property.Name == "Comprador")
									{
										worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 1)].Merge = true;
										columnIndex++;

									}

								}
								columnIndex++;

							}
							rowIndex++;
						}


						worksheet.Cells[rowIndex, 8].Value = process.Sum(s => s.LibrasProgramadas);
						worksheet.Cells[rowIndex, 8].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells[rowIndex, 8].Style.Font.Size = 7;
						worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;

						worksheet.Cells[rowIndex, 9].Value = process.Sum(s => s.LibrasRemitidas);
						worksheet.Cells[rowIndex, 9].Style.Numberformat.Format = "#,##0.00";
						worksheet.Cells[rowIndex, 9].Style.Font.Size = 7;
						worksheet.Cells[rowIndex, 9].Style.Font.Bold = true;


						//E

						rowIndex = rowIndex + 1;


					}

				}

				worksheet.Cells.AutoFitColumns();
				stream = new MemoryStream(package.GetAsByteArray());
				stream.Position = 0;
			}

			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
		}
				
		[HttpGet]
		public ActionResult DownLoadExcelRemisionGuideTerrestreViatic()
        {
			var fileName = "Pagos Viaticos Transportistas Terrestres.xlsx";
			Stream stream = new MemoryStream();
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var modelExcel = TempData["modelRemisionGuieTransportViatic"] as List<ResultRemisionGuideTransportViatic>;
			var imagenEncriptadaHex = db.Company.Select(x => x.logo).FirstOrDefault();
			string imagePath = Server.MapPath("~/Content/image/logo.png");
			System.IO.File.WriteAllBytes(imagePath, imagenEncriptadaHex);

			var companyName = db.Company.Where(c => c.isActive).Select(x => new { x.id, x.businessName, x.ruc, x.address, x.phoneNumber }).FirstOrDefault();

			using (var package = new ExcelPackage())
			{
				var worksheet = package.Workbook.Worksheets.Add("Pagos Viaticos Transportistas Terrestres");

				worksheet.Cells.Style.Font.Name = "Tahoma";

				var picture = worksheet.Drawings.AddPicture("LogoCia", new FileInfo(imagePath));
				picture.SetPosition(0, 0, 0, 0);
				picture.SetSize(100, 75);

				worksheet.View.TabSelected = true;
				worksheet.Cells.Style.Font.Size = 7;

				worksheet.Cells["E3:L3"].Merge = true; // Unir desde A1 hasta B1
				worksheet.Cells["E3:L3"].Style.Font.Bold = true; // Establecer negrita
				worksheet.Cells["E3:L3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells["E3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
				worksheet.Cells["E3:L3"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
				worksheet.Cells["E3"].Value = "PAGOS DE VIATICOS A TRANSPORTISTAS TERRESTRES"; // Contenido en A1
				worksheet.Cells["E3:L3"].Style.Font.Size = 10;
				worksheet.Cells["E3:L3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
				worksheet.Cells["E3:L3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


				worksheet.Cells["E4"].Value = modelExcel.Count() == 0 ? companyName.businessName : modelExcel?[0].descripcion;
				worksheet.Cells["E4"].Style.Font.Bold = true;

				worksheet.Cells["E5"].Style.Font.Bold = true;
				worksheet.Cells["E5"].Value = "#RUC:";
				worksheet.Cells["F5"].Value = modelExcel.Count() == 0 ? companyName.ruc : modelExcel?[0].ruc;

				worksheet.Cells["E6"].Style.Font.Bold = true;
				worksheet.Cells["E6"].Value = "Telf.:";
				worksheet.Cells["F6"].Value = modelExcel.Count() == 0 ? companyName.phoneNumber : modelExcel?[0].telefono;

				worksheet.Cells["K4"].Style.Font.Bold = true;
				worksheet.Cells["K4"].Value = "Fecha:";
				worksheet.Cells["L4"].Value = DateTime.Now.ToString("dd/MM/yyyy");

				worksheet.Cells["K5"].Style.Font.Bold = true;
				worksheet.Cells["K5"].Value = "Hora:";
				worksheet.Cells["L5"].Value = DateTime.Now.ToString("HH:mm:ss");

				worksheet.Cells["K6"].Style.Font.Bold = true;
				worksheet.Cells["K6"].Value = "Pagina:";
				worksheet.Cells["L6"].Value = "Pagina 1 of 1";


				worksheet.Cells["A7:M7"].Style.Font.Size = 8;
				worksheet.Cells["A7:M7"].Style.Font.Bold = true;
				worksheet.Cells["A7"].Value = "Guia";
				worksheet.Cells["B7"].Value = "No. ";
				worksheet.Cells["C7"].Value = "Fecha";
				worksheet.Cells["D7"].Value = "Placa";
				worksheet.Cells["E7:F7"].Merge = true;
				worksheet.Cells["E7"].Value = "Conductor";
				worksheet.Cells["G7"].Value = "Ruc Coop.";
				worksheet.Cells["H7:I7"].Merge = true;
				worksheet.Cells["H7"].Value = "Coop.";
				worksheet.Cells["J7:K7"].Merge = true;
				worksheet.Cells["J7"].Value = "Usuario Pago";
				worksheet.Cells["L7"].Value = "Valor";
		
				var rowIndex = 8;


                if (modelExcel.Count() > 0)
                {
					var process = modelExcel
						.Select((s, index) => new {
							s.documento,
							s.NumeroAnticipo,
							FechaEmision = s.emissionDate.ToString("dd/MM/yyyy"),
							s.placa,
							s.Chofer,
							s.CipersonaChofer,
							s.Transportista,
							s.UsuarioPago,
							s.viatico
						}).ToList();

					foreach (var pro in process)
					{
						var columnIndex = 1;
						foreach (var property in pro.GetType().GetProperties())
						{
							var value = property.GetValue(pro);

							if (value is decimal decimalValue)
							{
								// Es un valor decimal, formatea con dos decimales
								worksheet.Cells[rowIndex, columnIndex].Value = decimalValue;
								worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#,##0.00";
							}
							else
							{

								if (property.Name != "Chofer" && property.Name != "Transportista" && property.Name != "UsuarioPago")
								{
									worksheet.Cells[rowIndex, columnIndex].Value = value;
								}
								else
								{
									worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 1)].Merge = true;
									worksheet.Cells[rowIndex, columnIndex].Value = value;
									columnIndex++;
								}

							}
							columnIndex++;

						}
						rowIndex++;
					}

					worksheet.Cells[rowIndex, 10].Value = "Total";
					worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;

					worksheet.Cells[rowIndex, 12].Value = process.Sum(s => s.viatico);
					worksheet.Cells[rowIndex, 12].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[rowIndex, 12].Style.Font.Bold = true;

				}
				

				worksheet.Cells.AutoFitColumns();
				stream = new MemoryStream(package.GetAsByteArray());
				stream.Position = 0;
			}

			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

		}

		[HttpGet]
		public ActionResult DownLoadExcelRemisionGuideViaticTerrestre()
		{
			var fileName = "Guia Remision Viaticos Terrestres.xlsx";
			Stream stream = new MemoryStream();
			ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
			var modelExcel = TempData["modelRemisionGuieViaticTerrestre"] as List<ResultRemisionGuideViaticTerrestre>;
			var imagenEncriptadaHex = db.Company.Select(x => x.logo).FirstOrDefault();
			string imagePath = Server.MapPath("~/Content/image/logo.png");
			System.IO.File.WriteAllBytes(imagePath, imagenEncriptadaHex);

			var companyName = db.Company.Where(c => c.isActive).Select(x => new { x.id, x.businessName, x.ruc, x.address, x.phoneNumber }).FirstOrDefault();

			using (var package = new ExcelPackage())
			{
				var worksheet = package.Workbook.Worksheets.Add("Guia Remision Viaticos Terrestres");

				worksheet.Cells.Style.Font.Name = "Tahoma";

				var picture = worksheet.Drawings.AddPicture("LogoCia", new FileInfo(imagePath));
				picture.SetPosition(0, 0, 0, 0);
				picture.SetSize(100, 75);

				worksheet.View.TabSelected = true;
				worksheet.Cells.Style.Font.Size = 7;

				worksheet.Cells["D3:J3"].Merge = true; // Unir desde A1 hasta B1
				worksheet.Cells["D3:J3"].Style.Font.Bold = true; // Establecer negrita
				worksheet.Cells["D3:J3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells["D3:J3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
				worksheet.Cells["D3:J3"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
				worksheet.Cells["D3"].Value = "GUIA REMISION VIATICO TERRESTRE"; // Contenido en A1
				worksheet.Cells["D3:J3"].Style.Font.Size = 10;
				worksheet.Cells["D3:J3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
				worksheet.Cells["D3:J3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


				worksheet.Cells["D4"].Value = modelExcel.Count() == 0 ? companyName.businessName : modelExcel?[0].descripcion;
				worksheet.Cells["D4"].Style.Font.Bold = true;

				worksheet.Cells["D5"].Style.Font.Bold = true;
				worksheet.Cells["D5"].Value = "#RUC:";
				worksheet.Cells["E5"].Value = modelExcel.Count() == 0 ? companyName.ruc : modelExcel?[0].ruc;

				worksheet.Cells["D6"].Style.Font.Bold = true;
				worksheet.Cells["D6"].Value = "Telf.:";
				worksheet.Cells["E6"].Value = modelExcel.Count() == 0 ? companyName.phoneNumber : modelExcel?[0].telefono;

				worksheet.Cells["I4"].Style.Font.Bold = true;
				worksheet.Cells["I4"].Value = "Fecha:";
				worksheet.Cells["J4"].Value = DateTime.Now.ToString("dd/MM/yyyy");

				worksheet.Cells["I5"].Style.Font.Bold = true;
				worksheet.Cells["I5"].Value = "Hora:";
				worksheet.Cells["J5"].Value = DateTime.Now.ToString("HH:mm:ss");

				worksheet.Cells["I6"].Style.Font.Bold = true;
				worksheet.Cells["I6"].Value = "Pagina:";
				worksheet.Cells["J6"].Value = "Pagina 1 of 1";


				worksheet.Cells["A7:J7"].Style.Font.Size = 8;
				worksheet.Cells["A7:J7"].Style.Font.Bold = true;
				worksheet.Cells["A7"].Value = "Guia";
				worksheet.Cells["B7"].Value = "No. ";
				worksheet.Cells["C7"].Value = "Fecha";
				worksheet.Cells["D7"].Value = "Cedula";
				worksheet.Cells["E7:F7"].Merge = true;
				worksheet.Cells["E7"].Value = "Nombre";
				worksheet.Cells["G7"].Value = "Ruc";
				worksheet.Cells["H7:I7"].Merge = true;
				worksheet.Cells["H7"].Value = "Usuario Pago";
				worksheet.Cells["J7"].Value = "Valor";

				var rowIndex = 8;

				if(modelExcel.Count() > 0)
                {
					var process = modelExcel
						.Select((s, index) => new {
							s.documento,
							s.NumeroViaticoPersonal,
							FechaEmision = s.emissionDate.ToString("dd/MM/yyyy"),
							s.cipersona,
							s.persona,
							s.rol,
							s.UsuarioPago,
							s.viatico
						}).ToList();

					foreach (var pro in process)
					{
						var columnIndex = 1;
						foreach (var property in pro.GetType().GetProperties())
						{
							var value = property.GetValue(pro);

							if (value is decimal decimalValue)
							{
								// Es un valor decimal, formatea con dos decimales
								worksheet.Cells[rowIndex, columnIndex].Value = decimalValue;
								worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#,##0.00";
							}
							else
							{

								if (property.Name != "persona" && property.Name != "UsuarioPago")
								{
									worksheet.Cells[rowIndex, columnIndex].Value = value;
								}
								else
								{
									worksheet.Cells[rowIndex, columnIndex, rowIndex, (columnIndex + 1)].Merge = true;
									worksheet.Cells[rowIndex, columnIndex].Value = value;
									columnIndex++;
								}

							}
							columnIndex++;

						}
						rowIndex++;
					}

					worksheet.Cells[rowIndex, 8].Value = "Total";
					worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;

					worksheet.Cells[rowIndex, 10].Value = process.Sum(s => s.viatico);
					worksheet.Cells[rowIndex, 10].Style.Numberformat.Format = "#,##0.00";
					worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;
				}

				

				worksheet.Cells.AutoFitColumns();
				stream = new MemoryStream(package.GetAsByteArray());
				stream.Position = 0;
			}

			return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

		}

        [HttpGet]
		public ActionResult DownloadExcelMovementCost()
		{
            var fileName = "Movimientos con Costos.xlsx";
            Stream stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var modelExcel = TempData["modelMovementCost"] as List<ResultMovementCost>;
            var parameter = TempData["repMod"] as ReportProdModel;
            var imagenEncriptadaHex = db.Company.Select(x => x.logo).FirstOrDefault();
            string imagePath = Server.MapPath("~/Content/image/logo.png");
            System.IO.File.WriteAllBytes(imagePath, imagenEncriptadaHex);

            using (var package = new ExcelPackage())
			{
                var worksheet = package.Workbook.Worksheets.Add("Movimientos Con Costos");

                worksheet.Cells.Style.Font.Name = "Tahoma";

                var picture = worksheet.Drawings.AddPicture("LogoCia", new FileInfo(imagePath));
                picture.SetPosition(0, 0, 0, 0);
                picture.SetSize(100, 75);

                worksheet.View.TabSelected = true;
                worksheet.Cells.Style.Font.Size = 7;

                worksheet.Cells["D3:J3"].Merge = true; // Unir desde A1 hasta B1
                worksheet.Cells["D3:J3"].Style.Font.Bold = true; // Establecer negrita
                worksheet.Cells["D3:J3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["D3:J3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Orange);
                worksheet.Cells["D3:J3"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                worksheet.Cells["D3"].Value = "MOVIMIENTOS DE PROCESOS INTERNOS"; // Contenido en A1
                worksheet.Cells["D3:J3"].Style.Font.Size = 10;
                worksheet.Cells["D3:J3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
                worksheet.Cells["D3:J3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["E5"].Style.Font.Bold = true;
                worksheet.Cells["E5"].Value = "Desde:";
                worksheet.Cells["F5"].Value = (parameter.paramCRList[1].Valor ?? "Todos").ToString();

                worksheet.Cells["H5"].Style.Font.Bold = true;
                worksheet.Cells["H5"].Value = "Hasta:";
                worksheet.Cells["I5"].Value = (parameter.paramCRList[2].Valor ?? "Todos").ToString();


				var wareHouses= modelExcel.Select(x => x.nameBodega).Distinct();
                
				
				var rowIndexCab1 = 7;
				var rowIndexCab2 = 8;
                var columnIndex = 1;
                var rowIndex = 9;
                var r = 3;
				var n = "n";
                var rowIndex2 = 10;
                var row = 9;
                var column = 3;
                foreach (var wareHouse in wareHouses)
				{
                    
                    var data = modelExcel.Where(x => x.nameBodega == wareHouse);

                    
                    worksheet.Cells[rowIndex, columnIndex].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, columnIndex].Value = wareHouse;
                    rowIndex++;
					columnIndex++;
                    var group = data.Select(d => d.grupo).Distinct();
                    foreach (var ws in group)
					{
                        foreach (var property in ws.GetType().GetProperties())
						{
                            worksheet.Cells[rowIndex, columnIndex].Value = ws;
                        }
                        rowIndex++;
                    }

                    columnIndex++;
					var cab = data.Select(d => d.periodo).Distinct().OrderBy(x => x).ToList();


                    foreach (var ws in cab)
                    {
                        worksheet.Cells[rowIndexCab1, columnIndex].Style.Font.Bold = true;
                        worksheet.Cells[rowIndexCab1, columnIndex, rowIndexCab1, (columnIndex + 1)].Merge = true;
                        worksheet.Cells[rowIndexCab1, columnIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
                        worksheet.Cells[rowIndexCab1, columnIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[rowIndexCab1, columnIndex].Value = ws;

                        worksheet.Cells[rowIndexCab2, columnIndex].Style.Font.Bold = true;
                        worksheet.Cells[rowIndexCab2, columnIndex].Value = "Suma de Cantidad Lbs";

                       
                        worksheet.Cells[rowIndexCab2, (columnIndex + 1)].Style.Font.Bold = true;
                        worksheet.Cells[rowIndexCab2, (columnIndex + 1)].Value = "Suma de Costo Total";


						var values = data.Where(v => v.nameBodega == wareHouse && v.periodo == ws)
							.Select(d => new { d.periodoValores, d.valores, d.grupo }).OrderBy(d => d.grupo).ToList();
                        
                        worksheet.Cells[row, column].Value = values.Where(a => a.periodoValores == "Suma de Cantidad Lbs").Sum(x => x.valores);
                        worksheet.Cells[row, column].Style.Numberformat.Format = "#,##0.00";
                        worksheet.Cells[row, (column + 1)].Value = values.Where(a => a.periodoValores == "Suma de Costo Total").Sum(x => x.valores);
                        worksheet.Cells[row, (column + 1)].Style.Numberformat.Format = "#,##0.00";

						if (n == "n") { rowIndex2 = 10; }

                        foreach (var detail in values.Where(x => x.periodoValores == "Suma de Cantidad Lbs").Select(d => d.valores))
						{
							
                            // Es un valor decimal, formatea con dos decimales
                            worksheet.Cells[rowIndex2, r].Value = detail;
							worksheet.Cells[rowIndex2, r].Style.Numberformat.Format = "#,##0.00";
                            rowIndex2++;

                        }
                        if (n == "n") { rowIndex2 = 10; } else { rowIndex2 = row + 1; }
                        foreach (var detail in values.Where(x => x.periodoValores == "Suma de Costo Total").Select(d => d.valores))
                        {

                            // Es un valor decimal, formatea con dos decimales
                            worksheet.Cells[rowIndex2, (r + 1)].Value = detail;
                            worksheet.Cells[rowIndex2, (r + 1)].Style.Numberformat.Format = "#,##0.00";
                            rowIndex2++;
							
                        }

                        columnIndex +=2;
						column+=2;
                        rowIndex2++;
                        r+= 2;

                    }

					columnIndex = 1;
					row = rowIndex;
					column = 3;
                    rowIndex2 = rowIndex + 1;
					r = 3;
					n = "s";

                }

                worksheet.Cells.AutoFitColumns();
                stream = new MemoryStream(package.GetAsByteArray());
                stream.Position = 0;
            }

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

      [HttpGet]
	  public ActionResult DownloadExcelPlantTransfer(bool pdf)
    {
        var fileName = "Transferencia a tuneles.xlsx";
        Stream stream = new MemoryStream();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var modelExcel = TempData["modelMovePlantTransferDTO"] as List<ResultInventoryMovePlantTransferDTO>;
		var emissionDate = TempData["paramStartDate"];

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Transferencia a tuneles");
            worksheet.Cells.Style.Font.Name = "Tahoma";
            
            worksheet.View.TabSelected = true;
            worksheet.Cells.Style.Font.Size = 7;

            var userName = db.User.Where(u => u.id == ActiveUserId).Select(x => x.username);

            worksheet.Cells["B2:H2"].Merge = true; // Unir desde A1 hasta B1
            worksheet.Cells["B2:H2"].Style.Font.Bold = true; // Establecer negrita
            worksheet.Cells["B2"].Value = "CABECERA"; // Contenido en A1
            worksheet.Cells["B2:B2"].Style.Font.Size = 10;
            worksheet.Cells["B2:B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
            worksheet.Cells["B2:B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            var cells = worksheet.Cells["B2:H2"]; // Reemplaza "A1:B2" con tu rango de celdas específico
            var border = cells.Style.Border;

            border.Top.Style = ExcelBorderStyle.Thin;
            border.Bottom.Style = ExcelBorderStyle.Thin;
            border.Left.Style = ExcelBorderStyle.Thin;
            border.Right.Style = ExcelBorderStyle.Thin;

            //Cabecera
            worksheet.Cells["B3"].Style.Font.Bold = true; // Establecer negrita
            worksheet.Cells["B3"].Value = "Fecha:"; // Establecer negrita
            worksheet.Cells["C3"].Value = Convert.ToDateTime(emissionDate).ToShortDateString(); // Establecer negrita

            worksheet.Cells["D3"].Style.Font.Bold = true; // Establecer negrita
            worksheet.Cells["D3"].Value = "Fecha y hora impresion:"; // Establecer negrita
            worksheet.Cells["E3"].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");// Establecer negrita

            worksheet.Cells["F3:G3"].Style.Font.Bold = true; // Establecer negrita
            worksheet.Cells["F3"].Value = "Usuario:"; // Establecer negrita
            worksheet.Cells["G3"].Value = userName; // Establecer negrita

            worksheet.Cells["B4"].Style.Font.Bold = true; // Establecer negrita
            worksheet.Cells["B4"].Value = "Tunel:"; // Establecer negrita
            worksheet.Cells["C4"].Value = modelExcel.Select(m => m.warehouseLocation).FirstOrDefault(); // Establecer negrita
            worksheet.Cells["D4:H4"].Merge = true;


            cells = worksheet.Cells["B3:H4"];
            border = cells.Style.Border;

            border.Top.Style = ExcelBorderStyle.Thin;
            border.Bottom.Style = ExcelBorderStyle.Thin;
            border.Left.Style = ExcelBorderStyle.Thin;
            border.Right.Style = ExcelBorderStyle.Thin;


            worksheet.Cells["B5:H5"].Merge = true; // Unir desde A1 hasta B1
            worksheet.Cells["B5:H6"].Style.Font.Bold = true; // Establecer negrita
            worksheet.Cells["B5"].Value = "DETALLE"; // Contenido en A1
            worksheet.Cells["B5:B5"].Style.Font.Size = 10;
            worksheet.Cells["B5:B5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
            worksheet.Cells["B5:B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //Detalle

            cells = worksheet.Cells["B5:H6"];
            border = cells.Style.Border;

            border.Top.Style = ExcelBorderStyle.Thin;
            border.Bottom.Style = ExcelBorderStyle.Thin;
            border.Left.Style = ExcelBorderStyle.Thin;
            border.Right.Style = ExcelBorderStyle.Thin;

            worksheet.Cells["B6"].Value = "LOTE"; // Contenido en A1
            worksheet.Cells["C6"].Value = "Fecha y Hora de Ingreso a Túneles"; // Contenido en A1
            worksheet.Cells["D6"].Value = "COCHE"; // Contenido en A1
            worksheet.Cells["E6"].Value = "CANTIDAD"; // Contenido en A1
            worksheet.Cells["F6"].Value = "UM"; // Contenido en A1
            worksheet.Cells["G6"].Value = "COD. PRODUCTO"; // Contenido en A1
            worksheet.Cells["H6"].Value = "DES. PRODUCTO"; // Contenido en A1


            if (modelExcel.Count() > 0)
            {
                var distinctLot = modelExcel.Select(l => l.id_Lot).Distinct().ToList();

                var rowIndex = 6;

                foreach (var det in distinctLot)
                {
                    var lot = modelExcel.Where(l => l.id_Lot == det)
                        .Select(d => 
                        new
                        {
                            d.numberLot,
							d.dateTimeEntry,
                            d.car,
                            d.quantity,
                            d.metricUnit,
                            d.masterCode,
                            d.nameItem
                        }).ToList().OrderByDescending(w => w.car);

                    rowIndex = rowIndex + 1;

                    foreach (var lotDet in lot)
                    {
                        var columnIndex = 2;
                        foreach (var property in lotDet.GetType().GetProperties())
                        {
                            var value = property.GetValue(lotDet);

                            if (value is decimal decimalValue)
                            {
                                // Es un valor decimal, formatea con dos decimales
                                worksheet.Cells[rowIndex, columnIndex].Value = decimalValue;
                                worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "#,##0.00";
                            }
                            else
							if ( value is DateTime dateTimeValue)
								{
                                    worksheet.Cells[rowIndex, columnIndex].Value = dateTimeValue;
                                    worksheet.Cells[rowIndex, columnIndex].Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                                }
							else
                            {
                                worksheet.Cells[rowIndex, columnIndex].Value = value;
                            }
                            columnIndex++;
                        }
                        rowIndex++;

                        
                    }

                    worksheet.Cells[rowIndex, 2, rowIndex, 4].Merge = true;
                    worksheet.Cells[rowIndex, 2, rowIndex, 4].Value = "TOTAL LOTE";
                    worksheet.Cells[rowIndex, 2, rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; // Centra horizontalmente
                    worksheet.Cells[rowIndex, 2, rowIndex, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[rowIndex, 5].Value = lot.Sum(s => s.quantity);
                    worksheet.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
                    worksheet.Cells[rowIndex, 2, rowIndex, 5].Style.Font.Bold = true; // Establecer negrita


                }

                rowIndex++;

                worksheet.Cells[rowIndex, 2, rowIndex, 4].Merge = true;
                worksheet.Cells[rowIndex, 2, rowIndex, 4].Value = "GRAN LOTE";
                worksheet.Cells[rowIndex, 2, rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; // Centra horizontalmente
                worksheet.Cells[rowIndex, 2, rowIndex, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[rowIndex, 5].Value = modelExcel.Sum(s => s.quantity);
                worksheet.Cells[rowIndex, 5].Style.Numberformat.Format = "#,##0.00";
                worksheet.Cells[rowIndex, 2, rowIndex, 5].Style.Font.Bold = true; // Establecer negrita

                cells = worksheet.Cells[7, 2, rowIndex,8];
                border = cells.Style.Border;

                border.Top.Style = ExcelBorderStyle.Thin;
                border.Bottom.Style = ExcelBorderStyle.Thin;
                border.Left.Style = ExcelBorderStyle.Thin;
                border.Right.Style = ExcelBorderStyle.Thin;

                rowIndex++;

                worksheet.Cells[rowIndex, 2, rowIndex, 8].Merge = true; // Unir desde A1 hasta B1
                worksheet.Cells[rowIndex, 2, rowIndex, 8].Style.Font.Bold = true; // Establecer negrita
                worksheet.Cells[rowIndex, 2, rowIndex, 2].Style.Font.Size = 10;
                worksheet.Cells[rowIndex, 2].Value = "RESUMEN POR LOTE";
                worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Centra horizontalmente
                worksheet.Cells[rowIndex, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

				var rowIndexDetail = rowIndex;

                cells = worksheet.Cells[rowIndex, 2, rowIndex, 8];
                border = cells.Style.Border;
                border.Top.Style = ExcelBorderStyle.Thin;
                border.Bottom.Style = ExcelBorderStyle.Thin;
                border.Left.Style = ExcelBorderStyle.Thin;
                border.Right.Style = ExcelBorderStyle.Thin;

				rowIndex++;

                foreach (var det in distinctLot)
				{

						var numberLot = modelExcel.Where(l => l.id_Lot == det).Select(l => l.numberLot);

                    var lot = modelExcel.Where(l => l.id_Lot == det)
						.Select(d =>
						new
						{
							d.masterCode,
							d.nameItem,
                            d.quantity
                        }).ToList().OrderByDescending(o => o.nameItem);

                   
                        worksheet.Cells[rowIndex, 2, (rowIndex+1), 8].Style.Font.Bold = true; // Establecer negrita
                        worksheet.Cells[rowIndex, 2].Value = "LOTE";
                        worksheet.Cells[rowIndex, 3, rowIndex, 8].Merge = true;
                        worksheet.Cells[rowIndex, 3].Value = numberLot;

                        rowIndex++;

                        worksheet.Cells[rowIndex, 2].Value = "COD. PRODUCTO";
                        worksheet.Cells[rowIndex, 3, rowIndex, 7].Merge = true;
                        worksheet.Cells[rowIndex, 3].Value = "DESCRIPCIÓN DEL PRODUCTO";
                        worksheet.Cells[rowIndex, 8].Value = "CANTIDAD";

                        rowIndex++;

                        foreach (var lotDet in lot)
                        {
                            var columnIndex = 2;
                            foreach (var property in lotDet.GetType().GetProperties())
                            {
                                var value = property.GetValue(lotDet);

                                if (value is decimal decimalValue)
                                {
                                    // Es un valor decimal, formatea con dos decimales
                                    worksheet.Cells[rowIndex, 8].Value = decimalValue;
                                    worksheet.Cells[rowIndex, 8].Style.Numberformat.Format = "#,##0.00";
                                }
                                else
                                {
									if(property.Name == "nameItem")
									{
                                        worksheet.Cells[rowIndex, columnIndex, rowIndex, 7].Merge = true;
                                    }
                                    worksheet.Cells[rowIndex, columnIndex].Value = value;
                                    
                                }
                                columnIndex++;
                            }
                            rowIndex++;
                        }


                        worksheet.Cells[rowIndex, 2, rowIndex, 7].Merge = true;
                        worksheet.Cells[rowIndex, 2,rowIndex, 8].Style.Font.Bold = true; // Establecer negrita
                        worksheet.Cells[rowIndex, 2].Value = "Total Lote";
                        worksheet.Cells[rowIndex, 8].Value = lot.Sum(s => s.quantity);
                        worksheet.Cells[rowIndex, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; // Centra horizontalmente
                        worksheet.Cells[rowIndex, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[rowIndex, 8].Style.Numberformat.Format = "#,##0.00";

                        cells = worksheet.Cells[rowIndexDetail, 2, rowIndex, 8];
                        border = cells.Style.Border;
                        border.Top.Style = ExcelBorderStyle.Thin;
                        border.Bottom.Style = ExcelBorderStyle.Thin;
                        border.Left.Style = ExcelBorderStyle.Thin;
                        border.Right.Style = ExcelBorderStyle.Thin;
                        rowIndex++;
                    }


            }

            worksheet.Cells.AutoFitColumns();
            stream = new MemoryStream(package.GetAsByteArray());
            stream.Position = 0;

        }
        if (pdf)
        {
            string tempFile = Path.GetTempFileName();
            MemoryStream memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream);
			byte[] bytes = memoryStream.ToArray();

            System.IO.File.WriteAllBytes(tempFile, bytes); // Escribe los datos del Excel en el archivo temporal
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
			Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Open(tempFile);
            Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];
            ws.PageSetup.Orientation = Microsoft.Office.Interop.Excel.XlPageOrientation.xlLandscape;
			ws.PageSetup.FitToPagesWide = 1;
			ws.PageSetup.FitToPagesTall = false;

            string pdfPath = Path.ChangeExtension(tempFile, ".pdf");
			wb.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, pdfPath);
			wb.Close();
			app.Quit();

			// Finalmente, lee el archivo PDF en un FileStream y devuélvelo
			byte[] pdfBytes = System.IO.File.ReadAllBytes(pdfPath);
			return File(pdfBytes, "application/pdf", "Transferencia a tuneles.pdf");

        }
        else
        {
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        
    }


        #endregion
    }
}