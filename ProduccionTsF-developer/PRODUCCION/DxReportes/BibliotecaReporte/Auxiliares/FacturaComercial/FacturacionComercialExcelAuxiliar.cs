using BibliotecaReporte.Dataset.FacturasComercial;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.facturasComercial;
using BibliotecaReporte.Reportes.FacturaComercial;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.FacturaComercial
{
   internal class FacturacionComercialExcelAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_FacturacionComercialExcel";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static FacturacionComercialExcelDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptFacturaComercialExcel = new FacturacionComercialExcelDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<FacturacionComercialExcelModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptFacturaComercialExcel.CabeceraFacturacionComercialExcel.NewCabeceraFacturacionComercialExcelRow();
                    cabeceraRow.BuquemasViaje = detailResult.BuquemasViaje;
                    cabeceraRow.Shipping_Line = detailResult.Shipping_Line;
                    cabeceraRow.Documento = detailResult.Documento;
                    cabeceraRow.InvComm_orden_pedido = detailResult.InvComm_orden_pedido;
                    cabeceraRow.InvComm_fecha_embarque = detailResult.InvComm_fecha_embarque;
                    cabeceraRow.InvComm_numero_bl = detailResult.InvComm_numero_bl;
                    cabeceraRow.InvCommDet_cantidad = detailResult.InvCommDet_cantidad;
                    cabeceraRow.InvCommDet_Cantidad_Cajas = detailResult.InvCommDet_Cantidad_Cajas;
                    cabeceraRow.InvCommDet_Valor_Total = detailResult.InvCommDet_Valor_Total;
                    cabeceraRow.TermsNegotiation_code = detailResult.TermsNegotiation_code;
                    cabeceraRow.Document = detailResult.Document;
                    cabeceraRow.EmissionDateformat = detailResult.EmissionDateformat;
                    cabeceraRow.Fullname_businessName = detailResult.Fullname_businessName;
                    cabeceraRow.Good = detailResult.Good;
                    cabeceraRow.Portofdeparture = detailResult.Portofdeparture;
                    cabeceraRow.Portofdestination = detailResult.Portofdestination;
                    cabeceraRow.Numcontendor = detailResult.Numcontendor;
                    cabeceraRow.Direccion = detailResult.Direccion;
                    cabeceraRow.Descripformapago = detailResult.Descripformapago;
                    cabeceraRow.Letras = detailResult.Letras;
                    cabeceraRow.Plazo = detailResult.Plazo;
                    cabeceraRow.BankTransferInfo = detailResult.BankTransferInfo;
                    cabeceraRow.Descuento = detailResult.Descuento;
                    cabeceraRow.Item_Origen = detailResult.Item_Origen;
                    cabeceraRow.Talla_Origen = detailResult.Talla_Origen;
                    cabeceraRow.Notifier = detailResult.Notifier;
                    cabeceraRow.Direccionnotif = detailResult.Direccionnotif;
                    cabeceraRow.TelefonoNotif = detailResult.TelefonoNotif;
                    cabeceraRow.Telefonoconsig = detailResult.Telefonoconsig;
                    cabeceraRow.InvCommDet_precio_unitario = detailResult.InvCommDet_precio_unitario;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Trademark_Company = detailResult.Trademark_Company;                    
                    rptFacturaComercialExcel.CabeceraFacturacionComercialExcel.AddCabeceraFacturacionComercialExcelRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptFacturaComercialExcel;
        }

        private static Stream SetDataReport(FacturacionComercialExcelDataSet rptFacturacomercialExcelDataSet)
        {
            using (var report = new RptFacturacionComercialExcel())
            {
                report.SetDataSource(rptFacturacomercialExcelDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.Excel);
                report.Close();

                return streamReport;
            }
        }

    }
}
