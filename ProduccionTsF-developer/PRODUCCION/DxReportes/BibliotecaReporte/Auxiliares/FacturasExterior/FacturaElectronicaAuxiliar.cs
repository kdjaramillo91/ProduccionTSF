using BibliotecaReporte.Dataset;
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
    internal class FacturaElectronicaAuxiliar
    {
        private const string m_StoredProcedureName = "spPar_FacturaElectronicaAuxiliar";

        internal static Stream GetReport(SqlConnection sql, Parametro[] parametros)
        {
            return SetDataReport(PrepareData(sql, parametros));
        }

        private static DataSet[] PrepareData(SqlConnection sqlConnection, Parametro[] parametros)
        {
            var rptFacturaElectronica = new FacturaElectronicaDataSet();
            var rptCompaniaInfo = new CompaniaInfoDataSet();

            using (SqlConnection db = sqlConnection)
            {
                if (db.State == ConnectionState.Closed) { db.Open(); }

                var queryParameters = ReportAuxiliar.ToDynamicParameters(parametros);
                var detailsResult = db.Query<FacturaElectronicaModel>(m_StoredProcedureName, queryParameters, commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var detailResult in detailsResult)
                {
                    var cabeceraRow = rptFacturaElectronica.CabeceraFacturaElectronica.NewCabeceraFacturaElectronicaRow();
                    cabeceraRow.ID = detailResult.ID;
                    cabeceraRow.Id_saleOrder = detailResult.Id_saleOrder;
                    cabeceraRow.Id_buyer = detailResult.Id_buyer;
                    cabeceraRow.Id_remissionGuide = detailResult.Id_remissionGuide;
                    cabeceraRow.Subtotal12 = detailResult.Subtotal12;
                    cabeceraRow.SubTotal0 = detailResult.SubTotal0;
                    cabeceraRow.SubTotalNoObjectIVA = detailResult.SubTotalNoObjectIVA;
                    cabeceraRow.SubTotalExentIVA = detailResult.SubTotalExentIVA;
                    cabeceraRow.SubTotal = detailResult.SubTotal;
                    cabeceraRow.Descuento = detailResult.Descuento;
                    cabeceraRow.ValueICE = detailResult.ValueICE;
                    cabeceraRow.ValueIRBPNR = detailResult.ValueIRBPNR;
                    cabeceraRow.IVA = detailResult.IVA;
                    cabeceraRow.Tip = detailResult.Tip;
                    cabeceraRow.TotalValue = detailResult.TotalValue;
                    cabeceraRow.SubTotalSinImpuesto = detailResult.SubTotalSinImpuesto;
                    cabeceraRow.Id_InvoiceType = detailResult.Id_InvoiceType;
                    cabeceraRow.Id_InvoiceMode = detailResult.Id_InvoiceMode;
                    cabeceraRow.InvoiceDetail_id = detailResult.InvoiceDetail_id;
                    cabeceraRow.Id_invoice = detailResult.Id_invoice;
                    cabeceraRow.Id_item = detailResult.Id_item;
                    cabeceraRow.Description = detailResult.Description;
                    cabeceraRow.Amount = detailResult.Amount;
                    cabeceraRow.Precio = detailResult.Precio;
                    cabeceraRow.Dscto = detailResult.Dscto;
                    cabeceraRow.TotalPriceWithoutTax = detailResult.TotalPriceWithoutTax;
                    cabeceraRow.Iva = detailResult.Iva;
                    cabeceraRow.Iva0 = detailResult.Iva0;
                    cabeceraRow.IvaNoObject = detailResult.IvaNoObject;
                    cabeceraRow.IvaExented = detailResult.IvaExented;
                    cabeceraRow.InvoiceDetail_valueICE = detailResult.InvoiceDetail_valueICE;
                    cabeceraRow.InvoiceDetail_valueIRBPNR = detailResult.InvoiceDetail_valueIRBPNR;
                    cabeceraRow.PrecioTotal = detailResult.PrecioTotal;
                    cabeceraRow.DescriptionAuxCode = detailResult.DescriptionAuxCode;
                    cabeceraRow.MasterCode = detailResult.MasterCode;
                    cabeceraRow.Cartones = detailResult.Cartones;
                    cabeceraRow.Id_metricUnit = detailResult.Id_metricUnit;
                    cabeceraRow.Id_metricUnitInvoiceDetail = detailResult.Id_metricUnitInvoiceDetail;
                    cabeceraRow.Cantidad = detailResult.Cantidad;
                    cabeceraRow.DateUpdate = detailResult.DateUpdate;
                    cabeceraRow.CodePresentation = detailResult.CodePresentation;
                    cabeceraRow.PresentationMinimum = detailResult.PresentationMinimum;
                    cabeceraRow.PresentationMaximum = detailResult.PresentationMaximum;
                    cabeceraRow.InvoiceDetail_id = detailResult.InvoiceDetail_id;
                    cabeceraRow.Id_productionOrder = detailResult.Id_productionOrder;
                    cabeceraRow.Id_releaseProduction = detailResult.Id_releaseProduction;
                    cabeceraRow.Id_termsNegotiation = detailResult.Id_termsNegotiation;
                    cabeceraRow.Id_PaymentMethod = detailResult.Id_PaymentMethod;
                    cabeceraRow.Id_PaymentTerm = detailResult.Id_PaymentTerm;
                    cabeceraRow.Id_portShipment = detailResult.Id_portShipment;
                    cabeceraRow.Id_portDestination = detailResult.Id_portDestination;
                    cabeceraRow.Id_portDischarge = detailResult.Id_portDischarge;
                    cabeceraRow.FechaEmbarque = detailResult.FechaEmbarque;
                    cabeceraRow.Id_shippingAgency = detailResult.Id_shippingAgency;
                    cabeceraRow.DAE = detailResult.DAE;
                    cabeceraRow.BL = detailResult.BL;
                    cabeceraRow.GuiaRemision = detailResult.GuiaRemision;
                    cabeceraRow.Buque2 = detailResult.Buque2;
                    cabeceraRow.Buque = detailResult.Buque;
                    cabeceraRow.VIAJE = detailResult.VIAJE;
                    cabeceraRow.TotalCM = detailResult.TotalCM;
                    cabeceraRow.Id_capacityContainer = detailResult.Id_capacityContainer;
                    cabeceraRow.Id_tariffHeading = detailResult.Id_tariffHeading;
                    cabeceraRow.Direccion = detailResult.Direccion;
                    cabeceraRow.Email = detailResult.Email;
                    cabeceraRow.PtoEmision = detailResult.PtoEmision;
                    cabeceraRow.Establecimiento = detailResult.Establecimiento;
                    cabeceraRow.Secuencial = detailResult.Secuencial;
                    cabeceraRow.ValorTotalFob = detailResult.ValorTotalFob;
                    cabeceraRow.FleteInternacional = detailResult.FleteInternacional;
                    cabeceraRow.SeguroInternacional = detailResult.SeguroInternacional;
                    cabeceraRow.GastosAduaneros = detailResult.GastosAduaneros;
                    cabeceraRow.GastosTrasnporte = detailResult.GastosTrasnporte;
                    cabeceraRow.Id_metricUnitInvoice = detailResult.Id_metricUnitInvoice;
                    cabeceraRow.InvoiceExterior_id_userCreate = detailResult.InvoiceExterior_id_userCreate;
                    cabeceraRow.InvoiceExterior_dateCreate = detailResult.InvoiceExterior_dateCreate;
                    cabeceraRow.InvoiceExterior_id_userUpdate = detailResult.InvoiceExterior_id_userUpdate;
                    cabeceraRow.Id_consignee = detailResult.Id_consignee;
                    cabeceraRow.Id_notifier = detailResult.Id_notifier;
                    cabeceraRow.PurchaseOrder = detailResult.PurchaseOrder;
                    cabeceraRow.Id_ShippingLine = detailResult.Id_ShippingLine;
                    cabeceraRow.Unidad = detailResult.Unidad;
                    cabeceraRow.RazonSocial = detailResult.RazonSocial;
                    cabeceraRow.RucCedula = detailResult.RucCedula;
                    cabeceraRow.CodigoAuxiliar = detailResult.CodigoAuxiliar;
                    cabeceraRow.CodigoPrincipal = detailResult.CodigoPrincipal;
                    cabeceraRow.Descripcion = detailResult.Descripcion;
                    cabeceraRow.PuertoDescarga = detailResult.PuertoDescarga;
                    cabeceraRow.TipoPuertoDescarga_ = detailResult.TipoPuertoDescarga;
                    cabeceraRow.PuertoDeEmbaruqe = detailResult.PuertoDeEmbaruqe;
                    cabeceraRow.TipoPuertoEmbarque = detailResult.TipoPuertoEmbarque;
                    cabeceraRow.PuertoDestino = detailResult.PuertoDestino;
                    cabeceraRow.TipoPuertoDestino = detailResult.TipoPuertoDestino;
                    cabeceraRow.EmissionPoint_name = detailResult.EmissionPoint_name;
                    cabeceraRow.BusinessName = detailResult.BusinessName;
                    cabeceraRow.DirMatiz = detailResult.DirMatiz;
                    cabeceraRow.Factura = detailResult.Factura;
                    cabeceraRow.FEchaEmision = detailResult.FEchaEmision;
                    cabeceraRow.NumeroAutorizacion = detailResult.NumeroAutorizacion;
                    cabeceraRow.FechaHoraAutorizacion = detailResult.FechaHoraAutorizacion;
                    cabeceraRow.Pesonetokilos = detailResult.Pesonetokilos;
                    cabeceraRow.Pesonetolibras = detailResult.Pesonetolibras;
                    cabeceraRow.Pesobrutokilos = detailResult.Pesobrutokilos;
                    cabeceraRow.Pesobrutolibras = detailResult.Pesobrutolibras;
                    cabeceraRow.Pesoglaskilos = detailResult.Pesoglaskilos;
                    cabeceraRow.Pesoglaslibras = detailResult.Pesoglaslibras;
                    cabeceraRow.FormaDePago = detailResult.FormaDePago;
                    cabeceraRow.Plazo = detailResult.Plazo;
                    cabeceraRow.Tiempo = detailResult.Tiempo;
                    cabeceraRow.PaisDestino = detailResult.PaisDestino;
                    cabeceraRow.Naviera = detailResult.Naviera;
                    cabeceraRow.Linea_Naviera = detailResult.Linea_Naviera;
                    cabeceraRow.Partida = detailResult.Partida;
                    cabeceraRow.Partida2 = detailResult.Partida2;
                    cabeceraRow.DirSucural = detailResult.DirSucural;
                    cabeceraRow.Division_Address = detailResult.Division_Address;
                    cabeceraRow.RUCcia = detailResult.RUCcia;
                    cabeceraRow.NombreCia = detailResult.NombreCia;
                    cabeceraRow.Archivo = detailResult.Archivo;
                    cabeceraRow.ClaveAcceso = detailResult.ClaveAcceso;
                    cabeceraRow.TerminoNegocia = detailResult.TerminoNegocia;
                    cabeceraRow.ContribuyenteEspecialNo = detailResult.ContribuyenteEspecialNo;
                    cabeceraRow.Ambiente = detailResult.Ambiente;
                    cabeceraRow.Emision = detailResult.Emision;
                    cabeceraRow.ObligadoLlevarContabilidad = detailResult.ObligadoLlevarContabilidad;
                    cabeceraRow.CodigoEstado = detailResult.CodigoEstado;
                    cabeceraRow.TerminoNegociacion = detailResult.TerminoNegociacion;
                    cabeceraRow.NumeroContenedores = detailResult.NumeroContenedores;
                    cabeceraRow.Proforma = detailResult.Proforma;
                    cabeceraRow.Orden = detailResult.Orden;
                    cabeceraRow._RUCEXTERIOR = detailResult.RUCEXTERIOR;
                    cabeceraRow.NoConten = detailResult.NoConten;
                    cabeceraRow.Valor = detailResult.Valor;
                    cabeceraRow.RucCedula = detailResult.RucCedula;

                    rptFacturaElectronica.CabeceraFacturaElectronica.AddCabeceraFacturaElectronicaRow(cabeceraRow);
                }

                //Información de Logo
                rptCompaniaInfo = CompaniaInfoAuxiliar.PrepareLogo(db);

                if (db.State == ConnectionState.Open) { db.Close(); }
            }

            var dataSet = new DataSet[]
            {
                rptFacturaElectronica,
                rptCompaniaInfo,
            };

            return dataSet;
        }

        private static Stream SetDataReport(DataSet[] dataSet)
        {
            using (var report = new RptFacturaElectronica())
            {
                report.SetDataSource(dataSet[0]);

                // Subreporte
                report.OpenSubreport("RptLogo.rpt").SetDataSource(dataSet[1]);
                var streamReport = report.ExportToStream(ExportFormatType.PortableDocFormat);
                report.Close();

                return streamReport;
            }
        }
    }
}