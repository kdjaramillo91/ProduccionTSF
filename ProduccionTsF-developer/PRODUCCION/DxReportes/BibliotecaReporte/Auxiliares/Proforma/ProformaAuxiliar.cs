using BibliotecaReporte.Dataset.Proforma;
using BibliotecaReporte.Model;
using BibliotecaReporte.Model.ProformaModel;
using BibliotecaReporte.Reportes.Proforma;
using CrystalDecisions.Shared;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace BibliotecaReporte.Auxiliares.ProformasAuxiliar
{
    internal class ProformaAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_Proforma";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static ProformaDataSet PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptProforma = new ProformaDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<ProformaModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptProforma.CabeceraProforma.NewCabeceraProformaRow();
                    cabeceraRow.Id = detailResult.Id;
                    cabeceraRow.NombreCia = detailResult.NombreCia;
                    cabeceraRow.TelefonoCia = detailResult.TelefonoCia;
                    cabeceraRow.Email = detailResult.Email;
                    cabeceraRow.PlantCode = detailResult.PlantCode;
                    cabeceraRow.FDA = detailResult.FDA;
                    cabeceraRow.Web = detailResult.Web;
                    cabeceraRow.DireccionCia = detailResult.DireccionCia;
                    cabeceraRow.RucCia = detailResult.RucCia;
                    cabeceraRow.Factura = detailResult.Factura;
                    cabeceraRow.FechaEmision = detailResult.FechaEmision;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.Referencia = detailResult.Referencia;
                    cabeceraRow.TerminoNegociacion = detailResult.TerminoNegociacion;
                    cabeceraRow.OrdenPedido = detailResult.OrdenPedido;
                    cabeceraRow.RazonSocialSoldTo = detailResult.RazonSocialSoldTo;
                    cabeceraRow.USCISoldTo = detailResult.USCISoldTo;
                    cabeceraRow.AddressSoldTo1 = detailResult.AddressSoldTo1;
                    cabeceraRow.AddressSoldTo2 = detailResult.AddressSoldTo2;
                    cabeceraRow.CountrySoldTo = detailResult.CountrySoldTo;
                    cabeceraRow.Leyenda = detailResult.Leyenda;
                    cabeceraRow.CitySoldTo = detailResult.CitySoldTo;
                    cabeceraRow.Telefono1SoldTo = detailResult.Telefono1SoldTo;
                    cabeceraRow.Telefono2SoldTo = detailResult.Telefono2SoldTo;
                    cabeceraRow.EmailSoldTo = detailResult.EmailSoldTo;
                    cabeceraRow.RazonSocialShipTo = detailResult.RazonSocialShipTo;
                    cabeceraRow.USCIShipTo = detailResult.USCIShipTo;
                    cabeceraRow.packingdetail = detailResult.packingdetail;
                    cabeceraRow.AddressShipTo1 = detailResult.AddressShipTo1;
                    cabeceraRow.AddressShipTo2 = detailResult.AddressShipTo2;
                    cabeceraRow.CountryShipTo = detailResult.CountryShipTo;
                    cabeceraRow.Telefono1ShipTo = detailResult.Telefono1ShipTo;
                    cabeceraRow.Telefono2ShipTo = detailResult.Telefono2ShipTo;
                    cabeceraRow.EmailShipTo = detailResult.EmailShipTo;
                    cabeceraRow.PuertoDestino = detailResult.PuertoDestino;
                    cabeceraRow.PortDischarge = detailResult.PortDischarge;
                    cabeceraRow.Shipper = detailResult.Shipper;
                    cabeceraRow.PuertoDeEmbaruqe = detailResult.PuertoDeEmbaruqe;
                    cabeceraRow.FechaEmbarque = detailResult.FechaEmbarque;
                    cabeceraRow.PurchaseOrder = detailResult.PurchaseOrder;
                    cabeceraRow.Cartones = detailResult.Cartones;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.Precio = detailResult.Precio;
                    cabeceraRow.GastoDistribuido = detailResult.GastoDistribuido;
                    cabeceraRow.Size = detailResult.Size;
                    cabeceraRow.size2 = detailResult.size2;
                    cabeceraRow.Casepack = detailResult.Casepack;
                    cabeceraRow.Marca = detailResult.Marca;
                    cabeceraRow.descripcion = detailResult.descripcion;
                    cabeceraRow.Total = detailResult.Total;
                    cabeceraRow.Estado = detailResult.Estado;
                    cabeceraRow.PrecioFob = detailResult.PrecioFob;
                    cabeceraRow.ValuetotalCIF = detailResult.ValuetotalCIF;
                    cabeceraRow.Vendedor = detailResult.Vendedor;
                    cabeceraRow.Contacto = detailResult.Contacto;
                    cabeceraRow.PlazoPago = detailResult.PlazoPago;
                    cabeceraRow.CodigoBanco = detailResult.CodigoBanco;
                    cabeceraRow.NombreBanco = detailResult.NombreBanco;
                    cabeceraRow.PaisBanco = detailResult.PaisBanco;
                    cabeceraRow.DireccionBanco = detailResult.DireccionBanco;
                    cabeceraRow.MonedaBanco = detailResult.MonedaBanco;
                    cabeceraRow.EnrutamientoBanco = detailResult.EnrutamientoBanco;
                    cabeceraRow.CuentaBanco = detailResult.CuentaBanco;
                    cabeceraRow.NombreCompaniaBanco = detailResult.NombreCompaniaBanco;
                    cabeceraRow.CodigoBancoIntermediario = detailResult.CodigoBancoIntermediario;
                    cabeceraRow.CuentaBancoIntermediario = detailResult.CuentaBancoIntermediario;
                    cabeceraRow.MonedaBancoIntermediario = detailResult.MonedaBancoIntermediario;
                    cabeceraRow.PaisBancoIntermediario = detailResult.PaisBancoIntermediario;
                    cabeceraRow.Product = detailResult.Product;
                    cabeceraRow.ColourGrade = detailResult.ColourGrade;
                    cabeceraRow.PackingDetails = detailResult.PackingDetails;
                    cabeceraRow.ContainerDetails = detailResult.ContainerDetails;
                    cabeceraRow.ShipmentDate = detailResult.ShipmentDate;
                    cabeceraRow.NetoKilos = detailResult.NetoKilos;
                    cabeceraRow.NetoLibras = detailResult.NetoLibras;
                    cabeceraRow.BrutoKilos = detailResult.BrutoKilos;
                    cabeceraRow.BrutoLibras = detailResult.BrutoLibras;
                    cabeceraRow.GlaseoKilos = detailResult.GlaseoKilos;
                    cabeceraRow.GlaseoLibras = detailResult.GlaseoLibras;
                    cabeceraRow.FechaActualizacion = detailResult.FechaActualizacion;
                    cabeceraRow.FechaActual = detailResult.FechaActual;
                    cabeceraRow.Usuario = detailResult.Usuario;
                    cabeceraRow.CodigoPlanta = detailResult.CodigoPlanta;
                    cabeceraRow.NumeroContenedores = detailResult.NumeroContenedores;
                    cabeceraRow.ValorAbonado = detailResult.ValorAbonado;
                    cabeceraRow.transport = detailResult.transport;
                    cabeceraRow.MU = detailResult.MU;
                    cabeceraRow.METODOPAGO = detailResult.METODOPAGO;
                    cabeceraRow.BankTransferInfo = detailResult.BankTransferInfo;
                    cabeceraRow.Notificador = detailResult.Notificador;
                    cabeceraRow.direccionNotif = detailResult.direccionNotif;
                    cabeceraRow.cellPhoneNumberPerson = detailResult.cellPhoneNumberPerson;
                    cabeceraRow.EspecialCondition = detailResult.EspecialCondition;
                    cabeceraRow.USCINotif = detailResult.USCINotif;
                    cabeceraRow.Logo = detailResult.Logo;
                    cabeceraRow.Citynotif = detailResult.Citynotif;
                    cabeceraRow.Countrynotif = detailResult.Countrynotif;
                    cabeceraRow.Telefono1notif = detailResult.Telefono1notif;
                    cabeceraRow.Vessel = detailResult.Vessel;

                    rptProforma.CabeceraProforma.AddCabeceraProformaRow(cabeceraRow);
                }

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            return rptProforma;
        }

        private static Stream SetDataReport(ProformaDataSet rptPilotoDataSet)
        {
            using (var report = new RptProforma())
            {
                report.SetDataSource(rptPilotoDataSet);

                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}