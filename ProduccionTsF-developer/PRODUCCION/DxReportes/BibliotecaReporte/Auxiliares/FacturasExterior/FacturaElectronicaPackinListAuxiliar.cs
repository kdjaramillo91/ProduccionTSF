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
    internal class FacturaElectronicaPackinListAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_FacturaElectronicaPackinList";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static FacturaElectronicaPackinListDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptPackinglist = new FacturaElectronicaPackinListDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<FacturaElectronicaPackinListModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptPackinglist.CabeceraPackingList.NewCabeceraPackingListRow();
                    cabeceraRow.ID = detailResult.ID;
                    cabeceraRow.NombreCia = detailResult.NombreCia;
                    cabeceraRow.TelefONoCia = detailResult.TelefONoCia;
                    cabeceraRow.Email = detailResult.Email;
                    cabeceraRow.PlantCode = detailResult.PlantCode;
                    cabeceraRow.FDA = detailResult.FDA;
                    cabeceraRow.Web = detailResult.Web;
                    cabeceraRow.DirSucural = detailResult.DirSucural;
                    cabeceraRow.RucCia = detailResult.RucCia;
                    cabeceraRow.Factura = detailResult.Factura;
                    cabeceraRow.FechaEmisiON = detailResult.FechaEmisiON;
                    cabeceraRow.TerminONegociacion = detailResult.TerminONegociacion;
                    cabeceraRow.OrdenPedido = detailResult.OrdenPedido;
                    cabeceraRow.RazONSocialSoldTo = detailResult.RazONSocialSoldTo;
                    cabeceraRow.USCISoldTo = detailResult.USCISoldTo;
                    cabeceraRow.AddressSoldTo1 = detailResult.AddressSoldTo1;
                    cabeceraRow.AddressSoldTo2 = detailResult.AddressSoldTo2;
                    cabeceraRow.CountrySoldTo = detailResult.CountrySoldTo;
                    cabeceraRow.CitySoldTo = detailResult.CitySoldTo;
                    cabeceraRow.TelefONo1SoldTo = detailResult.TelefONo1SoldTo;
                    cabeceraRow.TelefONo2SoldTo = detailResult.TelefONo2SoldTo;
                    cabeceraRow.RazONSocialShipTo = detailResult.RazONSocialShipTo;
                    cabeceraRow.USCIShipTo = detailResult.USCIShipTo;
                    cabeceraRow.AddressShipTo1 = detailResult.AddressShipTo1;
                    cabeceraRow.AddressShipTo2 = detailResult.AddressShipTo2;
                    cabeceraRow.CountryShipTo = detailResult.CountryShipTo;
                    cabeceraRow.CityShipTo = detailResult.CityShipTo;
                    cabeceraRow.TelefONo1ShipTo = detailResult.TelefONo1ShipTo;
                    cabeceraRow.TelefONo2ShipTo = detailResult.TelefONo2ShipTo;
                    cabeceraRow.AddressShipTo1 = detailResult.AddressShipTo1;
                    cabeceraRow.CountryOfOrigin = detailResult.CountryOfOrigin;
                    cabeceraRow.PuertoDestino = detailResult.PuertoDestino;
                    cabeceraRow.PuertoDeEmbaruqe = detailResult.PuertoDeEmbaruqe;
                    cabeceraRow.Shipper = detailResult.Shipper;
                    cabeceraRow.Buque = detailResult.Buque;
                    cabeceraRow.VIAJE = detailResult.VIAJE;
                    cabeceraRow.Naviera = detailResult.Naviera;
                    cabeceraRow.BL = detailResult.BL;
                    cabeceraRow.PurchASeORDER = detailResult.PurchASeORDER;
                    cabeceraRow.FechaEmbarque = detailResult.FechaEmbarque;
                    cabeceraRow.CartONes = detailResult.CartONes;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.Precio = detailResult.Precio;
                    cabeceraRow.Size = detailResult.Size;
                    cabeceraRow.Certificacion = detailResult.Certificacion;
                    cabeceraRow.PesoKG = detailResult.PesoKG;
                    cabeceraRow.PesoLB = detailResult.PesoLB;
                    cabeceraRow.PesoGlaseoKG = detailResult.PesoGlaseoKG;
                    cabeceraRow.PesoGlaseoLB = detailResult.PesoGlaseoLB;
                    cabeceraRow.GlaseoLIBRA = detailResult.GlaseoLIBRA;
                    cabeceraRow.GlaseoKG = detailResult.GlaseoKG;
                    cabeceraRow.Marca = detailResult.Marca;
                    cabeceraRow.DescripciON2 = detailResult.DescripciON2;
                    cabeceraRow.DescripciON = detailResult.DescripciON;
                    cabeceraRow.Total = detailResult.Total;
                    cabeceraRow.NumeroContenedores = detailResult.NumeroContenedores;
                    cabeceraRow.Contenedores = detailResult.Contenedores;
                    cabeceraRow.FechaProforma = detailResult.FechaProforma;
                    cabeceraRow.Dae = detailResult.Dae;
                    cabeceraRow.FechaDae = detailResult.FechaDae;
                    cabeceraRow.FechaCarga = detailResult.FechaCarga;
                    cabeceraRow.NetoKilos = detailResult.NetoKilos;
                    cabeceraRow.NetoLibras = detailResult.NetoLibras;
                    cabeceraRow.BrutoKilos = detailResult.BrutoKilos;
                    cabeceraRow.BrutoLibras = detailResult.BrutoLibras;
                    cabeceraRow.GlaseoKilos = detailResult.GlaseoKilos;
                    cabeceraRow.GlaseoLibras = detailResult.GlaseoLibras;
                    cabeceraRow.FechaETD = detailResult.FechaETD;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Casepack = detailResult.Casepack;
                    
                    
                    rptPackinglist.CabeceraPackingList.AddCabeceraPackingListRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptPackinglist;
        }

        private static Stream SetDataReport(FacturaElectronicaPackinListDataSet rptpackinglistDataSet)
        {
            using (var report = new RptFacturaElectronicaPackinList())
            {
                report.SetDataSource(rptpackinglistDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}