using BibliotecaReporte.Dataset;
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
    internal class ProformaTSAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_ProformaTS";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }
        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptProformaTS = new ProformaTSDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<ProformaTSModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptProformaTS.CabeceraProformaTS.NewCabeceraProformaTSRow();
                    cabeceraRow.Id = detailResult.Id;
                    cabeceraRow.NombreCia = detailResult.NombreCia;
                    cabeceraRow.TelefonoCia = detailResult.TelefonoCia;
                    cabeceraRow.Email = detailResult.Email;
                    cabeceraRow.PlantCode = detailResult.PlantCode;
                    cabeceraRow.FDA = detailResult.FDA;
                    cabeceraRow.Web = detailResult.Web;
                    cabeceraRow.DirSucural = detailResult.DirSucural;
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
                    cabeceraRow.CitySoldTo = detailResult.CitySoldTo;
                    cabeceraRow.Telefono1SoldTo = detailResult.Telefono1SoldTo;
                    cabeceraRow.Telefono2SoldTo = detailResult.Telefono2SoldTo;
                    cabeceraRow.EmailSoldTo = detailResult.EmailSoldTo;
                    cabeceraRow.RazonSocialShipTo = detailResult.RazonSocialShipTo;
                    cabeceraRow.USCIShipTo = detailResult.USCIShipTo;
                    cabeceraRow.packingdetail = detailResult.packingdetail;
                    cabeceraRow.AddressShipTo1 = detailResult.AddressShipTo1;
                    cabeceraRow.CountryShipTo = detailResult.CountryShipTo;
                    cabeceraRow.CityShipTo = detailResult.CityShipTo;
                    cabeceraRow.Telefono1ShipTo = detailResult.Telefono1ShipTo;
                    cabeceraRow.Telefono2ShipTo = detailResult.Telefono2ShipTo;
                    cabeceraRow.EmailShipTo = detailResult.EmailShipTo;
                    cabeceraRow.PuertoDestino = detailResult.PuertoDestino;
                    cabeceraRow.Shipper = detailResult.Shipper;
                    cabeceraRow.PuertoDeEmbaruqe = detailResult.PuertoDeEmbaruqe;
                    cabeceraRow.FechaEmbarque = detailResult.FechaEmbarque;
                    cabeceraRow.PurchaseOrder = detailResult.PurchaseOrder;
                    cabeceraRow.Cartones = detailResult.Cartones;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.Precio = detailResult.Precio;
                    cabeceraRow.GastoDistribuido = detailResult.GastoDistribuido;
                    cabeceraRow.Size = detailResult.Size;
                    cabeceraRow.Casepack = detailResult.Casepack;
                    cabeceraRow.Marca = detailResult.Marca;
                    cabeceraRow.descripcion = detailResult.descripcion;
                    cabeceraRow.Total = detailResult.Total;
                    cabeceraRow.Estado = detailResult.Estado;
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
                    cabeceraRow.FDA = detailResult.FDA;
                    cabeceraRow.NumeroContenedores = detailResult.NumeroContenedores;
                    cabeceraRow.ValorAbonado = detailResult.ValorAbonado;
                    cabeceraRow.contacto_2 = detailResult.contacto_2;
                    cabeceraRow.descripcionCliente = detailResult.descripcionCliente;
                    cabeceraRow.METODOPAGO = detailResult.METODOPAGO;
                    cabeceraRow.transport = detailResult.transport;
                    cabeceraRow.MU = detailResult.MU;                    
                    cabeceraRow.FDA2 = detailResult.FDA2;                    
                    cabeceraRow.contacto_2 = detailResult.contacto_2;                    
                    cabeceraRow.PaisEmbarque = detailResult.PaisEmbarque;                    
                    cabeceraRow.PortLoading = detailResult.PortLoading;                    
                    cabeceraRow.FechaEmbarqueIngles = detailResult.FechaEmbarqueIngles;                    
                    cabeceraRow.FechaEmbarqueEsp = detailResult.FechaEmbarqueEsp;                    
                    cabeceraRow.FechaETA = detailResult.FechaETA;                    
                    cabeceraRow.CantidadOrigen = detailResult.CantidadOrigen;
                    cabeceraRow.CasePackDimension = detailResult.CasePackDimension;
                    cabeceraRow.NombreProducto = detailResult.NombreProducto;
                    cabeceraRow.CantidadContenedores = detailResult.CantidadContenedores;
                    cabeceraRow.TamañoContenedor = detailResult.TamañoContenedor;
                    cabeceraRow.NProforma = detailResult.NProforma;
                    cabeceraRow.IdentificationTypeName = detailResult.IdentificationTypeName;
                    cabeceraRow.CityName= detailResult.CityName;
                    cabeceraRow.StateofCountryName= detailResult.StateofCountryName;
                    cabeceraRow.Categoria2= detailResult.Categoria2;
                    cabeceraRow.ValorAnticipo= detailResult.ValorAnticipo;
                    cabeceraRow.ValorTotalTermPago= detailResult.ValorTotalTermPago;

                    
                  

                    rptProformaTS.CabeceraProformaTS.AddCabeceraProformaTSRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
          {
                rptProformaTS,
                rptCompaniaInfo,
          };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] rptPilotoDataSet)
        {
            using (var report = new RptProformaTS())
            {
                report.SetDataSource(rptPilotoDataSet[0]);
                report.OpenSubreport("RptLogo.rpt").SetDataSource(rptPilotoDataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}