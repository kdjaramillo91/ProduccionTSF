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


namespace DXPANACEASOFT.Controllers
{
    public class ReportProdController : DefaultController
    {
        public DBContextProd dbProd { get; } = new DBContextProd();

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            string repProd = Convert.ToString(Request.QueryString["trepd"]);

            ReportProdModel model = (TempData[repProd] as ReportProdModel);
      
            model = model ?? new ReportProdModel();
            
            Stream str = ServicePrintCrystalReport.PrintCRParameters(dbProd, model.codeReport, model.paramCRList, model.conex);

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

                tbsysPathReportProduction _tbsprp = dbProd.tbsysPathReportProduction.FirstOrDefault(fod => fod.code == model.codeReport);

                Stream str = ServicePrintCrystalReport.PrintCRParameters(dbProd, model.codeReport, model.paramCRList, model.conex);

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
            Stream str = ServicePrintCrystalReport.PrintExcelParameters(dbProd, model.codeReport, model.paramCRList, model.conex);
            str.Seek(0, SeekOrigin.Begin);
            TempData.Remove(repProd);
            return File(str, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ((model.nameReport != "" && model.nameReport != null) ? model.nameReport + ".xls": "Reporte.xls"));

        }

        #region AUXILIARS METHODS
        #endregion
    }
}