using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DXPANACEASOFT.Models;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using Utilitarios.CorreoElectronico;
using Utilitarios.Encriptacion;
using Utilitarios.Logs;
using Utilitarios.ProdException;

namespace DXPANACEASOFT.Services
{
    public class ServicePrintCrystalReport
    {

        public static void LogInfo(string mensaje, DateTime fechaHora)
        {
            string app = "Produccion";
            string origen = "ServicePrintCrystalReport";
            string rutaLog = ConfigurationManager.AppSettings.Get("rutalog");
            MetodosEscrituraLogs.EscribeMensajeLog($"{mensaje} - {fechaHora}", rutaLog, origen, app);
        }

        public static Stream PrintCRParameters(DBContext dbContext, string reportCode, List<ParamCR> reportParameters, Conexion conexionInfo, bool toSend = false)
        {
            var pathReportEntity = dbContext
                .tbsysPathReportProduction
                .FirstOrDefault(fod => fod.code == reportCode);

            if (pathReportEntity == null)
            {
                throw new ProdHandlerException($"Reporte no configurado: {reportCode}.");
            }

            if (pathReportEntity.isCustomized)
            {
                string dapperDBContext = ConfigurationManager.ConnectionStrings["DapperDBContext"].ConnectionString;
                var sqlConnection = new SqlConnection(dapperDBContext);

                if (sqlConnection == null)
                {
                    throw new ProdHandlerException("La cadena de conexión no está correctamente configurada.");
                }

                var parametros = reportParameters
                    .Select(e => new BibliotecaReporte.Model.Parametro()
                    {
                        Nombre = e.Nombre,
                        Valor = e.Valor,
                    })
                    .ToArray();

                if (reportCode == "RPFM")
                {
                    return ConsultaReporte.Consulta.ObtenerCrystalReportStream(sqlConnection, reportCode.Trim(), parametros);
                }
                else
                {
                    return ConsultaReporte.Consulta.ObtenerCrystalReportStream(sqlConnection, reportCode.Trim(), parametros);
                }
            }
            else
            {
                return PrintCRParametersActual(dbContext, reportCode, reportParameters, conexionInfo, toSend);
            }
        }
        public static Stream PrintCRParametersActual(
            DBContext dbContext, string reportCode, List<ParamCR> reportParameters, Conexion conexionInfo, bool toSend = false)
        {
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            int tag = 0;
            // Ubicamos al reporte. Si no existe, retornamos un Stream nulo...
            var pathReportEntity = dbContext
                .tbsysPathReportProduction
                .FirstOrDefault(fod => fod.code == reportCode);

            if (pathReportEntity == null)
            {
                return null;
            }
            tag = 1;
            LogInfo(tag.ToString(), DateTime.Now);
            // Preparamos la ruta completa del reporte
            var reportPath = Path.Combine(pathReportEntity.path, pathReportEntity.nameReport);

            if (String.IsNullOrEmpty(reportPath))
            {
                return null;
            }

            // Procesamos el reporte...
            using (ReportDocument reportDocument = new ReportDocument())
            {

                tag = 2;
                LogInfo(tag.ToString(), DateTime.Now);
                // Cargamos el reporte y le asignamos los parámetros enviados...
                reportDocument.Load(reportPath);

                tag = 3;
                LogInfo(tag.ToString(), DateTime.Now);
                if (reportParameters != null)
                {
                    tag = 4;
                    LogInfo(tag.ToString(), DateTime.Now);
                    foreach (var reportParameter in reportParameters)
                    {
                        reportDocument.SetParameterValue(
                            reportParameter.Nombre, reportParameter.Valor);
                    }
                }

                tag = 5;
                LogInfo(tag.ToString(), DateTime.Now);
                // Preparamos la información de la conexión a la base de datos
                var connectionInfo = new ConnectionInfo
                {
                    ServerName = conexionInfo.SrvName,
                    DatabaseName = conexionInfo.DbName,
                    UserID = conexionInfo.UsrName,
                    Password = conexionInfo.PswName,
                    IntegratedSecurity = false,
                };

                tag = 6;
                LogInfo(tag.ToString(), DateTime.Now);
                // Asignamos la conexión de datos a todas las tablas del reporte...
                foreach (Table reportTable in reportDocument.Database.Tables)
                {
                    var tableLogoninfo = reportTable.LogOnInfo;
                    tableLogoninfo.ConnectionInfo = connectionInfo;
                    reportTable.ApplyLogOnInfo(tableLogoninfo);
                }

                tag = 7;
                LogInfo(tag.ToString(), DateTime.Now);
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

                tag = 8;
                LogInfo(tag.ToString(), DateTime.Now);
                if (toSend)
                {
                    //Generamos un File temp con el resultado del reporte en PDF para enviarlo por correo
                    tag = 9;
                    LogInfo(tag.ToString(), DateTime.Now);
                    var fileDescriptor = Path.Combine(FileUploadHelper.GetUploadedFilesDirectory().FullName, pathReportEntity.nameReport + ".pdf");

                    tag = 10;
                    LogInfo(tag.ToString(), DateTime.Now);
                    reportDocument.ExportToDisk(ExportFormatType.PortableDocFormat, fileDescriptor);

                    tag = 11;
                    LogInfo(tag.ToString(), DateTime.Now);
                    FileStream fs = new FileStream(fileDescriptor, FileMode.Open, FileAccess.Read);

                    tag = 12;
                    LogInfo(tag.ToString(), DateTime.Now);
                    Attachment attachment = new Attachment(fs, pathReportEntity.nameReport + ".pdf", MediaTypeNames.Application.Octet);

                    tag = 13;
                    LogInfo(tag.ToString(), DateTime.Now);
                    int idAux = (int)reportParameters[0].Valor;

                    tag = 14;
                    LogInfo(tag.ToString(), DateTime.Now);
                    ProductionLot productionLotAux = dbContext.ProductionLot.FirstOrDefault(fod => fod.id == idAux);

                    tag = 15;
                    LogInfo(tag.ToString(), DateTime.Now);
                    Provider providerAux = dbContext.Provider.FirstOrDefault(fod => fod.id == productionLotAux.id_provider);

                    tag = 16;
                    LogInfo(tag.ToString(), DateTime.Now);
                    Person buyerAux = dbContext.Person.FirstOrDefault(fod => fod.id == productionLotAux.id_buyer);
                    string correoHasta = "";
                    //correoHasta = providerAux.ProviderGeneralDataEP.emailEP ?? "";

                    tag = 17;
                    LogInfo(tag.ToString(), DateTime.Now);
                    correoHasta = providerAux.Person.email ?? "";

                    tag = 18;
                    LogInfo(tag.ToString(), DateTime.Now);
                    var correoHastaAux = buyerAux?.Employee?.Person?.email ?? "";

                    tag = 19;
                    LogInfo(tag.ToString(), DateTime.Now);
                    if (!string.IsNullOrEmpty(correoHastaAux))
                    {
                        if (!string.IsNullOrEmpty(correoHasta))
                        {
                            correoHasta += ";" + correoHastaAux;
                        }
                        else
                        {
                            correoHasta = correoHastaAux;
                        }
                    }
                    tag = 20;
                    LogInfo(tag.ToString(), DateTime.Now);
                    correoHastaAux = ConfigurationManager.AppSettings["correoFijoComprobanteUnicoPago"];
                    if (!string.IsNullOrEmpty(correoHastaAux))
                    {
                        if (!string.IsNullOrEmpty(correoHasta))
                        {
                            correoHasta += ";" + correoHastaAux;
                        }
                        else
                        {
                            correoHasta = correoHastaAux;
                        }
                    }
                    tag = 21;
                    LogInfo(tag.ToString(), DateTime.Now);
                    if (!string.IsNullOrEmpty(correoHasta))
                    {

                        string correoBCC = "";
                        correoBCC = providerAux.Person.bCC ?? "";
                        var correoBCCAux = buyerAux?.Employee?.Person?.bCC ?? "";
                        if (!string.IsNullOrEmpty(correoBCCAux))
                        {
                            if (!string.IsNullOrEmpty(correoBCC))
                            {
                                correoBCC += ";" + correoBCCAux;
                            }
                            else
                            {
                                correoBCC = correoBCCAux;
                            }
                        }
                        //                        Liquidación Compra No. 29800 11 - jun - 2019 CAMARONERA LA VICTORIA LANGOVIC
                        //TEXTO:


                        //                    Proveedor: CAMARONERA LA VICTORIA LANGOVIC
                        //Camaronera: CAMARONERA LA VICTORIA LANGOVIC
                        //Piscina: 4
                        //Número de Lote: 1915441 - 0
                        //Cantidad Remitida: 4.666
                        //Fecha de Recepción: 03 / 06 / 2019
                        //Comercial: BRAVO AVILES CIRIA JANETH
                        //Proceso de: Camarón Entero
                        //Tipo de Proceso: ENTERO
                        tag = 22;
                        LogInfo(tag.ToString(), DateTime.Now);
                        string asunto = "Liquidación Compra No. " + (productionLotAux.sequentialLiquidation?.ToString() ?? "") + " " + (productionLotAux.liquidationPaymentDate?.ToString("dd-MMM-yyyy") ?? "") + " " + (providerAux?.Person.fullname_businessName ?? "");

                        tag = 23;
                        LogInfo(tag.ToString(), DateTime.Now);
                        var productionUnitProviderPoolAux = dbContext.ProductionUnitProviderPool.FirstOrDefault(t => t.id == productionLotAux.id_productionUnitProviderPool);

                        tag = 24;
                        LogInfo(tag.ToString(), DateTime.Now);
                        var productionUnitProviderAux = dbContext.ProductionUnitProvider.FirstOrDefault(t => t.id == productionLotAux.id_productionUnitProvider);

                        tag = 25;
                        LogInfo(tag.ToString(), DateTime.Now);
                        var detail = productionLotAux.ProductionLotDetail.FirstOrDefault();
                        //var id_buyer = detail?.ProductionLotDetailPurchaseDetail.FirstOrDefault().PurchaseOrderDetail.PurchaseOrder.id_buyer;
                        tag = 26;
                        LogInfo(tag.ToString(), DateTime.Now);
                        string cuerpoMensaje = "<b>Proveedor:</b> " + (providerAux?.Person.fullname_businessName ?? "") + "<br>" +
                                               "<b>Camaronera:</b> " + (productionUnitProviderAux?.name ?? "") + "<br>" +
                                               "<b>Piscina:</b> " + (productionUnitProviderPoolAux?.name ?? "") + "<br>" +
                                               "<b>Número de Lote:</b> " + (productionLotAux.internalNumberConcatenated) + "<br>" +
                                               "<b>Cantidad Remitida:</b> " + (productionLotAux.totalQuantityRecived.ToString("#,##0.00") ?? "") + "<br>" +
                                               "<b>Fecha de Recepción:</b> " + productionLotAux.receptionDate.ToString("dd/MM/yyyy") + "<br>" +
                                               "<b>Comercial:</b> " + (buyerAux?.Employee?.Person.fullname_businessName ?? "") + "<br>" +
                                               "<b>Proceso de:</b> " + (detail?.Item.name ?? "") + "<br>" +
                                               "<b>Tipo de Proceso:</b> " + productionLotAux.ProcessType.name;

                        try
                        {
                            tag = 26;
                            LogInfo(tag.ToString(), DateTime.Now);
                            string passwordSMTP = clsEncriptacion1.LeadnjirSimple.Desencriptar(ConfigurationManager.AppSettings["contrasenaCorreoDefault"]);
                            tag = 27;
                            LogInfo($"{tag.ToString()}-{passwordSMTP}", DateTime.Now);
                            var respuestaCorreo = clsCorreoElectronico.EnviarCorreoElectronico(
                                                                    correoHasta, ConfigurationManager.AppSettings["correoDefaultDesde"],
                                                                    asunto, ConfigurationManager.AppSettings["smtpHost"],
                                                                    Int32.Parse(ConfigurationManager.AppSettings["puertoHost"]),
                                                                    passwordSMTP,
                                                                    cuerpoMensaje, ';', attachment,
                                                                    rutalog: ruta
                                                                    );

                            tag = 28;
                            LogInfo($"{tag.ToString()}-{respuestaCorreo}", DateTime.Now);
                        }
                        catch (Exception ex)
                        {
                            tag = 28;
                            LogInfo($"{tag.ToString()}-{ex.Message}", DateTime.Now);
                            
                            MetodosEscrituraLogs.EscribeExcepcionLog(ex, ruta, "ProductionLotReception", "PLRE");
                        }
                    }
                }
                tag = 29;
                LogInfo(tag.ToString(), DateTime.Now);
                // Generamos un Stream con el resultado del reporte en PDF
                return reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);
            }
        }

        public static Stream PrintExcelParameters(
            DBContext dbContext, string reportCode, List<ParamCR> reportParameters, Conexion conexionInfo)
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

        public static Stream PrintCR(
            DBContext dbContext, string reportCode, DataSet dataSet, bool hasSubreport, bool toSend = false, User userActive = null)
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

            // Preparamos los datos recibidos
            var reportePrincipalDataTable = new DataTable();
            var subReporteLogoDataTable = new DataTable();

            var subReporteData1 = new DataTable();
            bool hasSubReportData = false;

            if ((dataSet != null) && (dataSet.Tables.Count > 0))
            {
                reportePrincipalDataTable = dataSet.Tables[0];

                if (hasSubreport && (dataSet.Tables.Count > 1))
                {
                    subReporteLogoDataTable = dataSet.Tables[1];
                }

                if (hasSubreport && (dataSet.Tables.Count > 2))
                {
                    subReporteData1 = dataSet.Tables[2];
                    hasSubReportData = true;
                }
            }

            //if (toSend)
            //{
            //    // Generamos un File temp con el resultado del reporte en PDF para enviarlo por correo
            //    var fileDescriptor = Path.Combine(FileUploadHelper.GetUploadedFilesDirectory().FullName, "prueba.pdf");
            //    //reportDocument.ExportToDisk(ExportFormatType.PortableDocFormat, fileDescriptor);
            //    FileStream fs = new FileStream(fileDescriptor, FileMode.Open, FileAccess.Read);
            //    Attachment attachment = new Attachment(fs, "prueba.pdf", MediaTypeNames.Application.Octet);
            //    List<string> listEmail = new List<string>();
            //    listEmail.Add("wleonardb@gmail.com");
            //    //ServiceSendMail.SendEmail("Prueba", "Prueba", listEmail, attachment);
            //    var respuestaCorreo = clsCorreoElectronico.EnviarCorreoElectronico(
            //           "wleonardb@gmail.com", "victor_ti93@hotmail.com", "Prueba", "smtp.live.com", 587,
            //           clsEncriptacion1.LeadnjirSimple.Desencriptar("x9sKt+M8+GUr0mhbqgD+ZQ=="), "Prueba", ';', attachment);
            //}

            // Procesamos el reporte...
            using (ReportDocument reportDocument = new ReportDocument())
            {
                // Cargamos el reporte y le asignamos los datos recibidos...
                reportDocument.Load(reportPath);

                reportDocument.SetDataSource(reportePrincipalDataTable);
                if (hasSubreport)
                {
                    reportDocument.Subreports[0].SetDataSource(subReporteLogoDataTable);

                    if (hasSubReportData)
                    {
                        reportDocument.Subreports[1].SetDataSource(subReporteData1);

                    }
                }

                if (toSend)
                {
                    // Generamos un File temp con el resultado del reporte en PDF para enviarlo por correo
                    //var fileDescriptor = Path.Combine(FileUploadHelper.GetUploadedFilesDirectory().FullName, pathReportEntity.nameReport + ".pdf");
                    //reportDocument.ExportToDisk(ExportFormatType.PortableDocFormat, fileDescriptor);
                    //FileStream fs = new FileStream(fileDescriptor, FileMode.Open, FileAccess.Read);
                    //Attachment attachment = new Attachment(fs, pathReportEntity.nameReport + ".pdf", MediaTypeNames.Application.Octet);
                    //var correoHasta = userActive?.Employee?.Person?.email;
                    //if (!string.IsNullOrEmpty(correoHasta)) {
                    //    var respuestaCorreo = clsCorreoElectronico.EnviarCorreoElectronico(
                    //    correoHasta, ConfigurationManager.AppSettings["correoDefaultDesde"], pathReportEntity.nameReport, ConfigurationManager.AppSettings["smtpHost"], Int32.Parse(ConfigurationManager.AppSettings["puertoHost"]),
                    //    clsEncriptacion1.LeadnjirSimple.Desencriptar(ConfigurationManager.AppSettings["contrasenaCorreoDefault"]), pathReportEntity.nameReport, ';', attachment);
                    //}
                }


                // Generamos un Stream con el resultado del reporte en PDF
                return reportDocument.ExportToStream(ExportFormatType.PortableDocFormat);
            }
        }

        public static Stream PrintCRExcel(
                                            DBContext dbContext,
                                            string reportCode,
                                            DataSet dataSet,
                                            bool hasSubreport,
                                            string[] subReportNames,
                                            bool toSend = false,
                                            User userActive = null)
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

            // Preparamos los datos recibidos
            var reportePrincipalDataTable = new DataTable();
            var subReporteLogoDataTable = new DataTable();

            var subReporteData1 = new DataTable();
            var subReporteData2 = new DataTable();
            bool hasSubReportData1 = false;
            bool hasSubReportData2 = false;

            if ((dataSet != null) && (dataSet.Tables.Count > 0))
            {
                reportePrincipalDataTable = dataSet.Tables[0];

                if (hasSubreport && (dataSet.Tables.Count > 1))
                {
                    subReporteLogoDataTable = dataSet.Tables[1];
                }

                if (hasSubreport && (dataSet.Tables.Count > 2))
                {
                    subReporteData1 = dataSet.Tables[2];
                    hasSubReportData1 = true;
                }
                if (hasSubreport && (dataSet.Tables.Count > 3))
                {
                    subReporteData2 = dataSet.Tables[3];
                    hasSubReportData2 = true;
                }
            }

            // Procesamos el reporte...
            using (ReportDocument reportDocument = new ReportDocument())
            {
                // Cargamos el reporte y le asignamos los datos recibidos...
                reportDocument.Load(reportPath);

                reportDocument.SetDataSource(reportePrincipalDataTable);
                if (hasSubreport)
                {
                    reportDocument.Subreports[0].SetDataSource(subReporteLogoDataTable);

                    if (hasSubReportData1)
                    {
                        reportDocument.Subreports[subReportNames[1]].SetDataSource(subReporteData1);

                    }
                    if (hasSubReportData2)
                    {
                        reportDocument.Subreports[subReportNames[2]].SetDataSource(subReporteData2);
                    }
                }

                // Generamos un Stream con el resultado del reporte en PDF
                return reportDocument.ExportToStream(ExportFormatType.Excel);
            }
        }
    }
}