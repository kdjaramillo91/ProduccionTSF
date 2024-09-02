using BibliotecaReporte.Dataset.FacturasExterior;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.FacturasExterior;
using BibliotecaReporte.Reportes.FacturasExterior;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
namespace BibliotecaReporte.Auxiliares.FacturasExterior
{
   internal class FacturaElectronicaPropiaExcelAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_FacturaElectronicaPropia";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static FacturaElectronicaPropiaExcelDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptFacturaElectroniaPropiaExcel = new FacturaElectronicaPropiaExcelDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<FacturaElectronicaPropiaExcelModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptFacturaElectroniaPropiaExcel.CabeceraFacturaElectronicaPropiaExcel.NewCabeceraFacturaElectronicaPropiaExcelRow();
                    cabeceraRow.NombreCia = detailResult.NombreCia;
                    cabeceraRow.TelefONoCia = detailResult.TelefONoCia;
                    cabeceraRow.Email = detailResult.Email;
                    cabeceraRow.Web = detailResult.Web;
                    cabeceraRow.DirSucural = detailResult.DirSucural;
                    cabeceraRow.RucCia = detailResult.RucCia;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Factura = detailResult.Factura;
                    cabeceraRow.FechaEmisiON = detailResult.FechaEmisiON;
                    cabeceraRow.Observacion = detailResult.Observacion;
                    cabeceraRow.TerminONegociacion = detailResult.TerminONegociacion;
                    cabeceraRow.OrdenPedido = detailResult.OrdenPedido;
                    cabeceraRow.RazONSocialSoldTo = detailResult.RazONSocialSoldTo;
                    cabeceraRow.USCISoldto = detailResult.USCISoldto;
                    cabeceraRow.AddressSoldTo1 = detailResult.AddressSoldTo1;
                    cabeceraRow.RazONSocialShipTo = detailResult.RazONSocialShipTo;
                    cabeceraRow.USCIShipTo = detailResult.USCIShipTo;
                    cabeceraRow.AddressShipTo1 = detailResult.AddressShipTo1;
                    cabeceraRow.Contacto = detailResult.Contacto;
                    cabeceraRow.PuertoDestino = detailResult.PuertoDestino;
                    cabeceraRow.PuertoDeEmbaruqe = detailResult.PuertoDeEmbaruqe;
                    cabeceraRow.FechaEmbarque = detailResult.FechaEmbarque;
                    cabeceraRow.Buque = detailResult.Buque;
                    cabeceraRow.VIAJE = detailResult.VIAJE;
                    cabeceraRow.Naviera = detailResult.Naviera;
                    cabeceraRow.CartONes = detailResult.CartONes;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.Precio = detailResult.Precio;
                    cabeceraRow.PrecioProforma = detailResult.PrecioProforma;
                    cabeceraRow.Size = detailResult.Size;
                    cabeceraRow.Marca = detailResult.Marca;
                    cabeceraRow.DescripciON = detailResult.DescripciON;
                    cabeceraRow.Total = detailResult.Total;
                    cabeceraRow.CodigoBanco = detailResult.CodigoBanco;
                    cabeceraRow.NombreBanco = detailResult.NombreBanco;
                    cabeceraRow.PaisBanco = detailResult.PaisBanco;
                    cabeceraRow.DireccionBanco = detailResult.DireccionBanco;
                    cabeceraRow.MonedaBanco = detailResult.MonedaBanco;
                    cabeceraRow.CuentaBanco = detailResult.CuentaBanco;
                    cabeceraRow.NombreCompaniaBanco = detailResult.NombreCompaniaBanco;
                    cabeceraRow.CodigoBancoIntermediario = detailResult.CodigoBancoIntermediario;
                    cabeceraRow.NombreBancoIntermediario = detailResult.NombreBancoIntermediario;
                    cabeceraRow.CuentaBancoIntermediario = detailResult.CuentaBancoIntermediario;
                    cabeceraRow.MonedaBancoIntermediario = detailResult.MonedaBancoIntermediario;
                    cabeceraRow.PaisBancoIntermediario = detailResult.PaisBancoIntermediario;
                    cabeceraRow.NumeroContenedores = detailResult.NumeroContenedores;
                    cabeceraRow.FechaProforma = detailResult.FechaProforma;
                    cabeceraRow.Proforma = detailResult.Proforma;
                    cabeceraRow.Dae = detailResult.Dae;
                    cabeceraRow.PlazoPago = detailResult.PlazoPago;
                    cabeceraRow.FechaDae = detailResult.FechaDae;
                    cabeceraRow.FechaCarga = detailResult.FechaCarga;
                    cabeceraRow.NetoKilos = detailResult.NetoKilos;
                    cabeceraRow.NetoLibras = detailResult.NetoLibras;
                    cabeceraRow.BrutoKilos = detailResult.BrutoKilos;
                    cabeceraRow.BrutoLibras = detailResult.BrutoLibras;
                    cabeceraRow.Logo2 = detailResult.Logo2;
                    cabeceraRow.Contenedores = detailResult.Contenedores;
                    cabeceraRow.Casepack = detailResult.Casepack;
                    cabeceraRow.EnrutamientoBanco = detailResult.EnrutamientoBanco;
                    cabeceraRow.CitySoldTo = detailResult.CitySoldTo;
                    cabeceraRow.EnrutamientoBanco = detailResult.EnrutamientoBanco;
                    cabeceraRow.CitySoldTo = detailResult.CitySoldTo;
                    cabeceraRow.CountrySoldTo = detailResult.CountrySoldTo;
                    cabeceraRow.Telefono1SoldTo = detailResult.Telefono1SoldTo;
                    cabeceraRow.CityShipTo = detailResult.CityShipTo;
                    cabeceraRow.CountryShipTo = detailResult.CountryShipTo;
                    cabeceraRow.Telefono1ShipTo = detailResult.Telefono1ShipTo;
                    rptFacturaElectroniaPropiaExcel.CabeceraFacturaElectronicaPropiaExcel.AddCabeceraFacturaElectronicaPropiaExcelRow(cabeceraRow);
                }
                if (db.State == ConnectionState.Open) { db.Close(); }
            }
            return rptFacturaElectroniaPropiaExcel;
        }
       private static Stream SetDataReport(FacturaElectronicaPropiaExcelDataSet rptfacturaElectronicaPropiaExcelDataSet)
        {
            using (var report = new RptFacturaElectronicaPropiaExcel())
            {
                report.SetDataSource(rptfacturaElectronicaPropiaExcelDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.Excel);
                report.Close();

                return streamReport;
            }
        }

    }
}
