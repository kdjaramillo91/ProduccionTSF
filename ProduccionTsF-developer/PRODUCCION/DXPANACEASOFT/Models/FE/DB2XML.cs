using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
//using DevExpress.XtraPrinting.Native;
///Common
using XmlInformacionTributaria = DXPANACEASOFT.Models.FE.Xmls.Common.InformacionTributaria;
using XmlCampoAdicional = DXPANACEASOFT.Models.FE.Xmls.Common.CampoAdicional;
///Factura
using XmlInformacionFactura = DXPANACEASOFT.Models.FE.Xmls.Factura.InformacionFactura;
using XmlTotalImpuestoFactura = DXPANACEASOFT.Models.FE.Xmls.Factura.TotalImpuesto;
using XmlPagoFactura = DXPANACEASOFT.Models.FE.Xmls.Factura.Pago;
using XmlFactura = DXPANACEASOFT.Models.FE.Xmls.Factura.Factura;
using XmlDetalleFactura = DXPANACEASOFT.Models.FE.Xmls.Factura.Detalle;
using XmlImpuestoFactura = DXPANACEASOFT.Models.FE.Xmls.Factura.Impuesto;
using XmlDetalleAdicional = DXPANACEASOFT.Models.FE.Xmls.Common.DetalleAdicional;
///GuiaRemision
using XmlGuiaRemision = DXPANACEASOFT.Models.FE.Xmls.GuiaRemision.GuiaRemision;
using XmlGuiaRemision2 = DXPANACEASOFT.Models.FE.Xmls.GuiaRemision.GuiaRemision2;
using XmlInfoGuiaRemision = DXPANACEASOFT.Models.FE.Xmls.GuiaRemision.InformacionGuiaRemision;
using XmlDetalleGuiaRemision = DXPANACEASOFT.Models.FE.Xmls.GuiaRemision.Detalle;
using XmlDestinatario = DXPANACEASOFT.Models.FE.Xmls.GuiaRemision.Destinatario;
using XmlDestinatario2 = DXPANACEASOFT.Models.FE.Xmls.GuiaRemision.Destinatario2;
///Retencion
using XmlRetencion = DXPANACEASOFT.Models.FE.Xmls.Retencion.Retencion;
using XmlInfoRetencion = DXPANACEASOFT.Models.FE.Xmls.Retencion.InformacionRetencion;
using XmlImpuestoRetencion = DXPANACEASOFT.Models.FE.Xmls.Retencion.Impuesto;
///NotaDeDebito
using XmlNotaDebito = DXPANACEASOFT.Models.FE.Xmls.NotaDebito.NotaDebito;
using XmlInformacionNotaDebito = DXPANACEASOFT.Models.FE.Xmls.NotaDebito.InformacionNotaDebito;
using XmlMotivo = DXPANACEASOFT.Models.FE.Xmls.NotaDebito.Motivo;
using XmlImpuestoNotaDebito = DXPANACEASOFT.Models.FE.Xmls.NotaDebito.Impuesto;
//NotaCredito
using XmlNotaCredito = DXPANACEASOFT.Models.FE.Xmls.NotaCredito.NotaCredito;
using XmlInformacionNotaCredito = DXPANACEASOFT.Models.FE.Xmls.NotaCredito.InformacionNotaCredito;
using XmlTotalImpuestoNotaCredito = DXPANACEASOFT.Models.FE.Xmls.NotaCredito.TotalImpuesto;
using XmlDetalleNotaCredito = DXPANACEASOFT.Models.FE.Xmls.NotaCredito.Detalle;
using XmlImpuestoNotaCredito = DXPANACEASOFT.Models.FE.Xmls.NotaCredito.Impuesto;
//using thiscode.Tools.DynamicSelectExtensions;
using DXPANACEASOFT.Auxiliares;

namespace DXPANACEASOFT.Models.FE
{
    public class DB2XML
    {
        private static DBContext db = new DBContext();

        public static XmlFactura Factura2Xml(Invoice factura)
        {
            InvoiceExterior facturaExterior = db.InvoiceExterior.FirstOrDefault(f => f.id == factura.id);
            factura.calculateTotales();
            factura.calculateTotalesInvoiceExterior();

            string[] configDec = new string[2];
            string[] configDec6 = new string[2];
            if (factura == null)
                return null;

            int ambiente = factura.Document.EmissionPoint.BranchOffice.Division.Company.CompanyElectronicFacturation.EnvironmentType.codeSRI;
            int tipoEmision = factura.Document.EmissionPoint.BranchOffice.Division.Company.CompanyElectronicFacturation.EmissionType.codeSRI;
            string razonSocial = ProcesarTexto(factura.Document.EmissionPoint.BranchOffice.Division.Company.trademark);
            string nombreComercial = ProcesarTexto(factura.Document.EmissionPoint.BranchOffice.Division.Company.businessName);
            string ruc = ProcesarCodigo(factura.Document.EmissionPoint.BranchOffice.Division.Company.ruc);
            string codigoDocumento = ProcesarCodigo(factura.Document.DocumentType.codeSRI);
            string establecimiento = ProcesarCodigo(factura.Document.EmissionPoint.BranchOffice.code);
            int puntoEmision = factura.Document.EmissionPoint.code;
            int secuencial = factura.Document.sequential;
            string direccionMatriz = ProcesarTexto(factura.Document.EmissionPoint.BranchOffice.Division.Company.address);
            string claveAcceso = factura.Document.accessKey;

            XmlInformacionTributaria informacionTributaria = new XmlInformacionTributaria
                                                             (
                                                                ambiente,
                                                                tipoEmision,
                                                                razonSocial,
                                                                nombreComercial,
                                                                ruc,
                                                                codigoDocumento,
                                                                establecimiento,
                                                                puntoEmision,
                                                                secuencial,
                                                                direccionMatriz,
                                                                claveAcceso
                                                            );


            decimal valueImporteTotal = 0;
            if (factura.InvoiceExterior != null)
            {

                valueImporteTotal = factura.valuetotalCIFTruncate;
            }
            else
            {

                valueImporteTotal = factura.totalValueTruncate;
            }

            string codeContribuyenteEspecial = factura.Document.EmissionPoint.BranchOffice.Division.Company.CompanyElectronicFacturation.resolutionNumber;
            codeContribuyenteEspecial = (codeContribuyenteEspecial == "0") ? "000" : codeContribuyenteEspecial;
            var xmlcl = db.Setting.FirstOrDefault(fod => fod.code == "XMLCL");
            var direccionComprador = "";
            var identificacionComprador = "";
            var razonSocialComprador = "";
            var tipoIdentificacionComprador = "";
            if (xmlcl != null && xmlcl.value == "SI")
            {
                direccionComprador = facturaExterior.Person1.address.Trim().Replace(System.Environment.NewLine, "");
                identificacionComprador = facturaExterior.Person1.identification_number;
                razonSocialComprador = facturaExterior.Person1.fullname_businessName;
                tipoIdentificacionComprador = facturaExterior.Person1.IdentificationType.codeSRI;
            }
            else
            {
                direccionComprador = factura.Person.address.Trim().Replace(System.Environment.NewLine, "");
                identificacionComprador = factura.Person.identification_number;
                razonSocialComprador = factura.Person.fullname_businessName;
                tipoIdentificacionComprador = factura.Person.IdentificationType.codeSRI;
            }

            XmlInformacionFactura informacionFactura = new XmlInformacionFactura
            {
                ContribuyenteEspecial = ProcesarTexto(codeContribuyenteEspecial),
                DireccionComprador = ProcesarTexto(direccionComprador), //Comprobante.Persona.Cliente.direccion,
                DireccionEstablecimiento = ProcesarTexto(factura.Document.EmissionPoint.BranchOffice.address),
                FechaEmision = factura.Document.emissionDate.ToString("dd/MM/yyyy"),
                GuiaRemision = ProcesarCodigo(factura.InvoiceExterior.numberRemissionGuide ?? "000-000-000000000"), 
                IdentificacionComprador = ProcesarCodigo(identificacionComprador),
                ImporteTotal = valueImporteTotal,
                Moneda = "DOLAR", // TODO:
                ObligadoContabilidad = (factura.Document.EmissionPoint.BranchOffice.Division.Company.CompanyElectronicFacturation.requireAccounting) ? "SI" : "NO",
                Propina = factura.tip.ToAdvanceDecimal(configDec, out configDec),
                RazonSocialComprador = ProcesarTexto(razonSocialComprador),
                TipoIdentificacionComprador = ProcesarTexto(tipoIdentificacionComprador),
                TotalDescuento = factura.totalDiscountTruncate,
                TotalSinImpuestos = factura.totalValueTruncate 
            };

            // Factura del Exterior
            if (factura.InvoiceExterior != null)
            {
                informacionFactura.ComercioExterior = "EXPORTADOR";
                informacionFactura.IncoTermFactura = ProcesarTexto(factura.InvoiceExterior.TermsNegotiation.code.ToUpper());
                informacionFactura.LugarIncoTerm = "GUAYAQUIL";
                informacionFactura.PaisOrigen = "593";
                Port puertoEmbarque = db.Port.FirstOrDefault(r => r.id == factura.InvoiceExterior.id_portShipment);
                Port puertoDestino = db.Port.FirstOrDefault(r => r.id == factura.InvoiceExterior.id_portDestination);
                informacionFactura.PuertoEmbarque = ProcesarTexto(puertoEmbarque.nombre + ", " + puertoEmbarque.City.name);
                informacionFactura.PuertoDestino = ProcesarTexto(puertoDestino.nombre + ", " + puertoDestino.City.name);
                informacionFactura.PaisDestino = ProcesarTexto(puertoDestino.City?.Country?.code2 ?? "0");
                informacionFactura.PaisAdquisicion = "593";
                informacionFactura.IncoTermTotalSinImpuestos = factura.InvoiceExterior.TermsNegotiation.code.ToUpper();
                informacionFactura.FleteInternacional = factura.InvoiceExterior.valueInternationalFreight.ToAdvanceDecimal(configDec, out configDec);
                informacionFactura.SeguroInternacional = factura.InvoiceExterior.valueInternationalInsurance.ToAdvanceDecimal(configDec, out configDec);
                informacionFactura.GastosAduaneros = factura.InvoiceExterior.valueCustomsExpenditures.ToAdvanceDecimal(configDec, out configDec);
                informacionFactura.GastosTransporteOtros = factura.InvoiceExterior.valueTransportationExpenses.ToAdvanceDecimal(configDec, out configDec);
            }

            informacionFactura.totalConImpuestos = new List<XmlTotalImpuestoFactura>();

            if (factura.InvoiceExterior != null)
            {
                XmlTotalImpuestoFactura xmlTotalImpuesto = new XmlTotalImpuestoFactura
                {
                    Codigo = 2,
                    CodigoPorcentaje = 0,
                    DescuentoAdicional = 0,
                    BaseImponible = factura.totalValueTruncate,
                    Tarifa = 0,
                    Valor = 0

                };

                informacionFactura.totalConImpuestos.Add(xmlTotalImpuesto);

                informacionFactura.pagos = new List<XmlPagoFactura>();

                XmlPagoFactura xmlPagoFactura = new XmlPagoFactura
                {
                    FormaPago = 20,
                    Total = factura.totalValueTruncate,
                    Plazo = 30,
                    UnidadTiempo = "dias"
                };
                informacionFactura.pagos.Add(xmlPagoFactura);

            }
            else
            {

                foreach (var impuesto in factura.InvoiceTotalTaxes)
                {
                    XmlTotalImpuestoFactura xmlTotalImpuesto = new XmlTotalImpuestoFactura
                    {
                        Codigo = int.Parse(impuesto.TaxType.code),
                        CodigoPorcentaje = int.Parse(impuesto.Rate1.code),
                        DescuentoAdicional = impuesto.aditionalDiscount.ToAdvanceDecimal(configDec, out configDec),
                        BaseImponible = impuesto.taxableBase.ToAdvanceDecimal(configDec, out configDec),
                        Tarifa = impuesto.rate.ToAdvanceDecimal(configDec, out configDec),
                        Valor = impuesto.value.ToAdvanceDecimal(configDec, out configDec),

                    };
                    informacionFactura.totalConImpuestos.Add(xmlTotalImpuesto);
                }
            }


            List<XmlDetalleFactura> detalles = new List<XmlDetalleFactura>();

            foreach (var detalle in factura.InvoiceDetail.Where(r => r.isActive).ToList())
            {
                int? idMetricUnitDetailName = detalle.id_metricUnitInvoiceDetail;
                string nameUnidadMedida = db.MetricUnit.FirstOrDefault(r => r.id == idMetricUnitDetailName)?.name ?? "";
                var codigoAuxiliar = string.IsNullOrEmpty(detalle.Item.auxCode) 
                    ? detalle.Item.masterCode : detalle.Item.auxCode;

                var xmlDetalle = new XmlDetalleFactura
                {
                    CodigoPrincipal = ProcesarCodigo(detalle.Item.masterCode),
                    CodigoAuxiliar = ProcesarCodigo(codigoAuxiliar),
                    Descripcion = ProcesarTexto(detalle.Item.name),

                    Cantidad = ((decimal)detalle.id_amountInvoice).ToAdvanceDecimal(null, out configDec6, "FXGEE"),
                    PrecioUnitario = detalle.unitPrice.ToAdvanceDecimal(configDec6, out configDec6, "FXGEE"),
                    Descuento = detalle.discount.ToAdvanceDecimal(configDec, out configDec), 
                    PrecioTotalSinImpueto = detalle.totalPriceWithoutTax.ToAdvanceDecimal(configDec, out configDec), 
                    UnidadMedida = ProcesarTexto(nameUnidadMedida),
                    Impuestos = new List<XmlImpuestoFactura>(),
                    DetallesAdicionales = new List<XmlDetalleAdicional>()
                };

                List<XmlDetalleAdicional> detallesAdicionales = null;

                detallesAdicionales = new List<XmlDetalleAdicional>();
                XmlDetalleAdicional xmlDetalleAdicional = null;
                xmlDetalleAdicional = new XmlDetalleAdicional
                {
                    Nombre = "CARTONES",
                    Valor = detalle.numBoxes.ToString()
                };
                detallesAdicionales.Add(xmlDetalleAdicional);

                // Obtener Code Unidad de Medida
                int? idMetricUnitDetail = detalle.id_metricUnitInvoiceDetail;
                string codeUnidadMedida = db.MetricUnit.FirstOrDefault(r => r.id == idMetricUnitDetail)?.code ?? "";
                xmlDetalleAdicional = new XmlDetalleAdicional
                {
                    Nombre = "UNIDAD",
                    Valor = ProcesarCodigo(codeUnidadMedida.ToUpper())
                };
                detallesAdicionales.Add(xmlDetalleAdicional);

                xmlDetalle.DetallesAdicionales = detallesAdicionales;

                List<XmlImpuestoFactura> impuestos = new List<XmlImpuestoFactura>();

                XmlImpuestoFactura xmlImpuesto = new XmlImpuestoFactura
                {
                    Codigo = 2,
                    CodigoPorPorcentaje = 0,
                    Tarifa = 0,
                    BaseImponible = decimal.Round(detalle.totalPriceWithoutTax, 2, MidpointRounding.AwayFromZero),
                    Valor = 0
                };
                impuestos.Add(xmlImpuesto);

                xmlDetalle.Impuestos = impuestos;

                detalles.Add(xmlDetalle);
            }

            List<XmlCampoAdicional> camposAdicionales = new List<XmlCampoAdicional>();
            XmlCampoAdicional xmlCampoAdicional = null;

            var nac001 = db.Setting.FirstOrDefault(fod => fod.code == "NAC001");
            {
                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "Direccion",
                    Valor = ProcesarTexto(factura.Person.address),
                };
                camposAdicionales.Add(xmlCampoAdicional);

                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "Email",
                    Valor = ProcesarTexto(factura.Person.email),
                };
                camposAdicionales.Add(xmlCampoAdicional);

                // obtener puerto de descarga
                string puertoDescarga = db.Port.FirstOrDefault(r => r.id == factura.InvoiceExterior.id_portDischarge)?.nombre ?? "";
                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "PuertoDescarga",
                    Valor = ProcesarTexto(puertoDescarga),
                };
                camposAdicionales.Add(xmlCampoAdicional);

                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "FechaEmbarque",
                    Valor = string.Format("{0:MM/dd/yyyy}", factura.InvoiceExterior.dateShipment),
                };
                camposAdicionales.Add(xmlCampoAdicional);

                // Obtener Nombre naviera
                string nombreNaviera = db.ShippingAgency.FirstOrDefault(r => r.id == factura.InvoiceExterior.id_shippingAgency)?.name ?? "";
                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "Naviera",
                    Valor = ProcesarTexto(nombreNaviera),
                };
                camposAdicionales.Add(xmlCampoAdicional);

                // Obtener pais destino
                string nombrePaisDestino = "";
                Port puertoDestino = db.Port.FirstOrDefault(r => r.id == factura.InvoiceExterior.id_portDestination);
                if (puertoDestino != null)
                {
                    City ciudadDestino = db.City.FirstOrDefault(r => r.id == puertoDestino.id_city);
                    if (ciudadDestino != null)
                    {
                        nombrePaisDestino = db.Country.FirstOrDefault(r => r.id == ciudadDestino.id_country)?.name ?? "";
                    }
                }

                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "PaisDestino",
                    Valor = ProcesarTexto(nombrePaisDestino),
                };
                camposAdicionales.Add(xmlCampoAdicional);

                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "Buque",
                    Valor = ProcesarTexto(factura.InvoiceExterior.shipName),
                };
                camposAdicionales.Add(xmlCampoAdicional);

                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "DAE",
                    Valor = ProcesarTexto(factura.InvoiceExterior.daeNumber + "-" +
                              factura.InvoiceExterior.daeNumber2 + "-" +
                              factura.InvoiceExterior.daeNumber3 + "-" +
                              factura.InvoiceExterior.daeNumber4
                            )
                };
                camposAdicionales.Add(xmlCampoAdicional);

                int? totalBoxes = factura.InvoiceDetail.Where(r => r.isActive).Sum(s => s.numBoxes);
                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "TotalCM",
                    Valor = totalBoxes.ToString(),
                };
                camposAdicionales.Add(xmlCampoAdicional);

                List<InvoiceExteriorWeight> pesosFactura = null;
                List<InvoiceExteriorWeight> pesosFactura2 = null;
                string stringPesos = null;
                string stringPesos2 = null;

                // Obtener Peso Neto;                    
                pesosFactura = factura.InvoiceExteriorWeight.Where(r => r.WeightType.code == "NET").ToList();
                stringPesos = pesosFactura.Select(s => new { Valor = s.peso.ToString() + " " + s.MetricUnit.code }).Select(r => r.Valor.ToString()).Aggregate((i, j) => i + "  " + j);
                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "PesoNeto",
                    Valor = stringPesos,
                };
                camposAdicionales.Add(xmlCampoAdicional);

                // Obtener Peso Bruto;                    
                pesosFactura2 = factura.InvoiceExteriorWeight.Where(r => r.WeightType.code == "BRT").ToList();
                stringPesos2 = pesosFactura2.Select(s => new { Valor = s.peso.ToString() + " " + s.MetricUnit.code }).Select(r => r.Valor.ToString()).Aggregate((i, j) => i + "  " + j);
                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "PesoBruto",
                    Valor = stringPesos2,
                };
                camposAdicionales.Add(xmlCampoAdicional);

                string partidaContenedores = (factura.InvoiceExterior.tariffHeadingDescription ?? "") + " Nr.Conten" + (factura.InvoiceExterior.CapacityContainer?.name ?? "") + ": " + factura.InvoiceExterior.numeroContenedores.ToString();
                partidaContenedores = partidaContenedores.Replace(System.Environment.NewLine, " - ");
                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "Partida",
                    Valor = ProcesarTexto(partidaContenedores),
                };
                camposAdicionales.Add(xmlCampoAdicional);


                xmlCampoAdicional = new XmlCampoAdicional
                {
                    Nombre = "-",
                    Valor = "EXPORTADORES HABITUALES DE BIENES",
                };
                camposAdicionales.Add(xmlCampoAdicional);

                if (nac001 != null && nac001.value == "SI" && factura.Document.emissionDate >= new DateTime(2020, 10, 1))
                {
                    xmlCampoAdicional = new XmlCampoAdicional
                    {
                        Nombre = "Agente de Retencion",
                        Valor = ProcesarTexto(nac001.description),
                    };
                    camposAdicionales.Add(xmlCampoAdicional);
                }
            }

            XmlFactura xmlFactura = new XmlFactura(informacionTributaria, informacionFactura, detalles, camposAdicionales);
            return xmlFactura;
        }



        public static XmlGuiaRemision GuiaRemision2Xml(RemissionGuide guiaRemision)
        {
            if (guiaRemision == null)
                return null;

            var id_company = guiaRemision.Document.EmissionPoint.id_company;
            string emissionDate = guiaRemision.Document.emissionDate.ToString("dd/MM/yyyy");

            Company vrCompa = db.Company.Where(h => h.id == id_company).FirstOrDefault();

            int ambiente = vrCompa.CompanyElectronicFacturation.EnvironmentType.codeSRI;
            int tipoEmision = vrCompa.CompanyElectronicFacturation.EmissionType.codeSRI;
            string razonSocial = ProcesarTexto(vrCompa.businessName);
            string nombreComercial = ProcesarTexto(vrCompa.trademark);
            string ruc = ProcesarCodigo(vrCompa.ruc);
            string codigoDocumento = ProcesarTexto(guiaRemision.Document.DocumentType.codeSRI);
            string establecimiento = ProcesarTexto(guiaRemision.Document.EmissionPoint.BranchOffice.code.PadLeft(3, '0'));
            int puntoEmision = guiaRemision.Document.EmissionPoint.code;
            int secuencial = guiaRemision.Document.sequential;
            string direccionMatriz = ProcesarTexto(vrCompa.address);
            string claveAcceso = guiaRemision.Document.accessKey;

            XmlInformacionTributaria informacionTributaria = new XmlInformacionTributaria(
                ambiente,
                tipoEmision,
                razonSocial,
                nombreComercial,
                ruc,
                codigoDocumento,
                establecimiento,
                puntoEmision,
                secuencial,
                direccionMatriz,
                claveAcceso
            );

            string carRegistration = "";
            if (guiaRemision.RemissionGuideTransportation.DriverVeicleProviderTransport != null)
            {


                carRegistration = guiaRemision.RemissionGuideTransportation.DriverVeicleProviderTransport.VeicleProviderTransport.Vehicle.carRegistration;
            }
            else
            {

                carRegistration = guiaRemision.RemissionGuideTransportation.carRegistration;
            }

            var razonSocialTransportista = (guiaRemision.RemissionGuideTransportation.Person != null)
                ? guiaRemision.RemissionGuideTransportation.Person.fullname_businessName
                : guiaRemision.Document.EmissionPoint.BranchOffice.Division.Company.businessName;

            var rucTransportista = (guiaRemision.RemissionGuideTransportation.Person != null)
                ? guiaRemision.RemissionGuideTransportation.Person.identification_number
                : guiaRemision.Document.EmissionPoint.BranchOffice.Division.Company.ruc;

            if (String.IsNullOrEmpty(rucTransportista))
            {
                throw new Exception("Ruc del transportista es obligatorio");
            }

            var tipoIdentificacion = (guiaRemision.RemissionGuideTransportation.Person != null)
                ? guiaRemision.RemissionGuideTransportation.Person.IdentificationType.codeSRI
                : "04";

            XmlInfoGuiaRemision infoGuiaRemision = new XmlInfoGuiaRemision
            {
                ContribuyenteEspecial = ProcesarTexto(vrCompa.CompanyElectronicFacturation.resolutionNumber),
                DireccionEstablecimiento = ProcesarTexto(guiaRemision.Document.EmissionPoint.BranchOffice.address),
                DireccionPartida = ProcesarTexto(guiaRemision.startAdress),
                FechaFinTransporte = emissionDate,
                FechaInicioTransporte = emissionDate,
                ObligadoContabilidad = (vrCompa.CompanyElectronicFacturation.requireAccounting) ? "SI" : "NO",
                Placa = ProcesarCodigo(carRegistration),
                RazonSocialTransportista = ProcesarTexto(razonSocialTransportista),
                Rise = ProcesarTexto(vrCompa.CompanyElectronicFacturation.rise),
                RucTransportista = ProcesarCodigo(rucTransportista),
                TipoIdentificacionTransportista = ProcesarTexto(tipoIdentificacion),
            };

            List<XmlDestinatario> destinatarios = new List<XmlDestinatario>();
            foreach (RemissionGuideDetail remissionGuideDetail in guiaRemision.RemissionGuideDetail)
            {
                foreach (var purchaseOrderDetail in remissionGuideDetail.RemissionGuideDetailPurchaseOrderDetail)
                {
                    PurchaseOrder purchaseOrder = purchaseOrderDetail.PurchaseOrderDetail.PurchaseOrder;

                    XmlDestinatario destinatario = destinatarios.FirstOrDefault(d => d.IdentificacionDestinatario.Equals(purchaseOrder.Person.identification_number));

                    #region"Si el destinatario no existe"
                    if (destinatario == null)
                    {
                        XmlDestinatario xmlDestinatario = new XmlDestinatario
                        {
                            CodigoDocumentoSustento = "00",
                            CodigoEstablecimientoDestino = "000",
                            DireccionDestinatario = ProcesarTexto(purchaseOrder.Person.address),
                            DocumentoAduaneroUnico = string.IsNullOrEmpty(guiaRemision.uniqueCustomDocument) ? "000" : guiaRemision.uniqueCustomDocument,
                            FechaEmisionDocumentoSustento = purchaseOrder.Document.emissionDate.ToString("dd/MM/yyyy"),
                            IdentificacionDestinatario = ProcesarCodigo(purchaseOrder.Person.identification_number),
                            MotivoTraslado = ProcesarTexto(guiaRemision.RemissionGuideReason.name),
                            NumeroAutorizacionDocumentoSustento = ProcesarCodigo(purchaseOrder.Document.authorizationNumber),
                            NumeroDocumentoSustento = ProcesarCodigo(purchaseOrder.Document.number),
                            RazonSocialDestinatario = ProcesarTexto(purchaseOrder.Person.fullname_businessName),
                            Ruta = ProcesarTexto(guiaRemision.route)
                        };

                        var codigoInterno = (!string.IsNullOrEmpty(remissionGuideDetail.Item.auxCode))
                            ? remissionGuideDetail.Item.auxCode
                            : remissionGuideDetail.Item.masterCode;
                        XmlDetalleGuiaRemision xmlDetalle = new XmlDetalleGuiaRemision
                        {
                            CodigoAdicional = ProcesarCodigo(remissionGuideDetail.Item.masterCode),
                            CodigoInterno = ProcesarCodigo(codigoInterno),
                            Descripcion = ProcesarTexto(remissionGuideDetail.Item.name),
                            Cantidad = decimal.Round(remissionGuideDetail.quantityOrdered, 2, MidpointRounding.AwayFromZero)
                        };

                        xmlDestinatario.Detalles.Add(xmlDetalle);


                        destinatarios.Add(xmlDestinatario);
                    }
                    #endregion
                    #region"Si el destinatario existe"
                    else
                    {
                        XmlDetalleGuiaRemision detail = destinatario.Detalles.FirstOrDefault(
                                                        d => d.CodigoAdicional.Equals(remissionGuideDetail.Item.masterCode));

                        if (detail == null)
                        {
                            var codigoInterno = (!string.IsNullOrEmpty(remissionGuideDetail.Item.auxCode)) ? remissionGuideDetail.Item.auxCode : remissionGuideDetail.Item.masterCode;
                            XmlDetalleGuiaRemision xmlDetalle = new XmlDetalleGuiaRemision
                            {
                                CodigoAdicional = ProcesarCodigo(remissionGuideDetail.Item.masterCode),
                                CodigoInterno = ProcesarCodigo(codigoInterno),
                                Descripcion = ProcesarTexto(remissionGuideDetail.Item.name),
                                Cantidad = decimal.Round(remissionGuideDetail.quantityOrdered, 2, MidpointRounding.AwayFromZero)
                            };

                            destinatario.Detalles.Add(xmlDetalle);
                        }
                    }
                    #endregion

                }
            }


            if (destinatarios.Count <= 0/* IsEmpty()*/)
            {
                XmlDestinatario xmlDestinatario = new XmlDestinatario
                {
                    CodigoDocumentoSustento = "00",
                    CodigoEstablecimientoDestino = "000",
                    DireccionDestinatario = ProcesarTexto(guiaRemision.Person.address),
                    DocumentoAduaneroUnico = ProcesarCodigo(string.IsNullOrEmpty(guiaRemision.uniqueCustomDocument) ? "000" : guiaRemision.uniqueCustomDocument),
                    FechaEmisionDocumentoSustento = guiaRemision.Document.emissionDate.ToString("dd/MM/yyyy"),
                    IdentificacionDestinatario = ProcesarCodigo(guiaRemision.Person.identification_number),
                    MotivoTraslado = ProcesarTexto(guiaRemision.RemissionGuideReason.name),
                    NumeroAutorizacionDocumentoSustento = guiaRemision.Document.accessKey,
                    NumeroDocumentoSustento = ProcesarCodigo(guiaRemision.Document.number),
                    RazonSocialDestinatario = ProcesarTexto(guiaRemision.Person.fullname_businessName),
                    Ruta = guiaRemision.route

                };

                foreach (RemissionGuideDetail detalle in guiaRemision.RemissionGuideDetail)
                {
                    var codigoInterno = (!string.IsNullOrEmpty(detalle.Item.auxCode)) ? detalle.Item.auxCode : detalle.Item.masterCode;
                    XmlDetalleGuiaRemision xmlDetalle = new XmlDetalleGuiaRemision
                    {
                        CodigoAdicional = ProcesarTexto(detalle.Item.masterCode),
                        CodigoInterno = ProcesarCodigo(codigoInterno),
                        Descripcion = ProcesarTexto(detalle.Item.name),
                        Cantidad = decimal.Round(detalle.quantityOrdered, 2, MidpointRounding.AwayFromZero)
                    };
                    xmlDestinatario.Detalles.Add(xmlDetalle);
                }

                destinatarios.Add(xmlDestinatario);
            }

            List<XmlCampoAdicional> camposAdicionales = null;

            XmlGuiaRemision xmlGuiaRemision = new XmlGuiaRemision(informacionTributaria, infoGuiaRemision, destinatarios, camposAdicionales);

            return xmlGuiaRemision;
        }


        public static XmlGuiaRemision GuiaRemision2Xml(int id_remissionGuide)
        {
            RemissionGuide guiaRemision = db.RemissionGuide.Where(g => g.id == id_remissionGuide).FirstOrDefault();

            if (guiaRemision == null)
                return null;
            var id_company = guiaRemision.Document.EmissionPoint.id_company;
            string emissionDate = guiaRemision.Document.emissionDate.ToString("dd/MM/yyyy");

            Company vrCompa = db.Company.Where(h => h.id == id_company).FirstOrDefault();

            int ambiente = vrCompa.CompanyElectronicFacturation.EnvironmentType.codeSRI;
            int tipoEmision = vrCompa.CompanyElectronicFacturation.EmissionType.codeSRI;
            string razonSocial = ProcesarTexto(vrCompa.businessName);
            string nombreComercial = ProcesarTexto(vrCompa.trademark);
            string ruc = ProcesarCodigo(vrCompa.ruc);
            string codigoDocumento = ProcesarTexto(guiaRemision.Document.DocumentType.codeSRI);
            string establecimiento = ProcesarTexto(guiaRemision.Document.EmissionPoint.BranchOffice.code.PadLeft(3, '0'));
            int puntoEmision = guiaRemision.Document.EmissionPoint.code;
            int secuencial = guiaRemision.Document.sequential;
            string direccionMatriz = ProcesarTexto(vrCompa.address);
            string claveAcceso = guiaRemision.Document.accessKey;

            XmlInformacionTributaria informacionTributaria = new XmlInformacionTributaria(
                ambiente,
                tipoEmision,
                razonSocial,
                nombreComercial,
                ruc,
                codigoDocumento,
                establecimiento,
                puntoEmision,
                secuencial,
                direccionMatriz,
                claveAcceso
            );

            string carRegistration = "";
            if (guiaRemision.RemissionGuideTransportation.DriverVeicleProviderTransport != null)
            {


                carRegistration = guiaRemision.RemissionGuideTransportation.DriverVeicleProviderTransport.VeicleProviderTransport.Vehicle.carRegistration;
            }
            else
            {

                carRegistration = guiaRemision.RemissionGuideTransportation.carRegistration;
            }

            var razonSocialTransportista = (guiaRemision.RemissionGuideTransportation.Person != null) 
                ? guiaRemision.RemissionGuideTransportation.Person.fullname_businessName 
                : guiaRemision.Document.EmissionPoint.BranchOffice.Division.Company.businessName;

            var rucTransportista = (guiaRemision.RemissionGuideTransportation.Person != null)
                ? guiaRemision.RemissionGuideTransportation.Person.identification_number
                : guiaRemision.Document.EmissionPoint.BranchOffice.Division.Company.ruc;

            if (String.IsNullOrEmpty(rucTransportista))
            {
                throw new Exception("Ruc del transportista es obligatorio");
            }

            var tipoIdentificacion = (guiaRemision.RemissionGuideTransportation.Person != null)
                ? guiaRemision.RemissionGuideTransportation.Person.IdentificationType.codeSRI
                : "04";

            XmlInfoGuiaRemision infoGuiaRemision = new XmlInfoGuiaRemision
            {
                ContribuyenteEspecial = ProcesarTexto(vrCompa.CompanyElectronicFacturation.resolutionNumber),
                DireccionEstablecimiento = ProcesarTexto(guiaRemision.Document.EmissionPoint.BranchOffice.address),
                DireccionPartida = ProcesarTexto(guiaRemision.startAdress),
                FechaFinTransporte = emissionDate,
                FechaInicioTransporte = emissionDate,
                ObligadoContabilidad = (vrCompa.CompanyElectronicFacturation.requireAccounting) ? "SI" : "NO",
                Placa = ProcesarCodigo(carRegistration),
                RazonSocialTransportista = ProcesarTexto(razonSocialTransportista),
                Rise = ProcesarTexto(vrCompa.CompanyElectronicFacturation.rise),
                RucTransportista = ProcesarCodigo(rucTransportista),
                TipoIdentificacionTransportista = ProcesarTexto(tipoIdentificacion),
            };

            List<XmlDestinatario> destinatarios = new List<XmlDestinatario>();
            foreach (RemissionGuideDetail remissionGuideDetail in guiaRemision.RemissionGuideDetail)
            {
                foreach (var purchaseOrderDetail in remissionGuideDetail.RemissionGuideDetailPurchaseOrderDetail)
                {
                    PurchaseOrder purchaseOrder = purchaseOrderDetail.PurchaseOrderDetail.PurchaseOrder;

                    XmlDestinatario destinatario = destinatarios.FirstOrDefault(d => d.IdentificacionDestinatario.Equals(purchaseOrder.Person.identification_number));

                    #region"Si el destinatario no existe"
                    if (destinatario == null)
                    {
                        XmlDestinatario xmlDestinatario = new XmlDestinatario
                        {
                            CodigoDocumentoSustento = "00",
                            CodigoEstablecimientoDestino = "000",
                            DireccionDestinatario = ProcesarTexto(purchaseOrder.Person.address),
                            DocumentoAduaneroUnico = string.IsNullOrEmpty(guiaRemision.uniqueCustomDocument) ? "000" : guiaRemision.uniqueCustomDocument,
                            FechaEmisionDocumentoSustento = purchaseOrder.Document.emissionDate.ToString("dd/MM/yyyy"),
                            IdentificacionDestinatario = ProcesarCodigo(purchaseOrder.Person.identification_number),
                            MotivoTraslado = ProcesarTexto(guiaRemision.RemissionGuideReason.name),
                            NumeroAutorizacionDocumentoSustento = ProcesarCodigo(purchaseOrder.Document.authorizationNumber),
                            NumeroDocumentoSustento = ProcesarCodigo(purchaseOrder.Document.number),
                            RazonSocialDestinatario = ProcesarTexto(purchaseOrder.Person.fullname_businessName),
                            Ruta = ProcesarTexto(guiaRemision.route)
                        };

                        var codigoInterno = (!string.IsNullOrEmpty(remissionGuideDetail.Item.auxCode))
                            ? remissionGuideDetail.Item.auxCode
                            : remissionGuideDetail.Item.masterCode;
                        XmlDetalleGuiaRemision xmlDetalle = new XmlDetalleGuiaRemision
                        {
                            CodigoAdicional = ProcesarCodigo(remissionGuideDetail.Item.masterCode),
                            CodigoInterno = ProcesarCodigo(codigoInterno),
                            Descripcion = ProcesarTexto(remissionGuideDetail.Item.name),
                            Cantidad = decimal.Round(remissionGuideDetail.quantityOrdered, 2, MidpointRounding.AwayFromZero)
                        };

                        xmlDestinatario.Detalles.Add(xmlDetalle);


                        destinatarios.Add(xmlDestinatario);
                    }
                    #endregion
                    #region"Si el destinatario existe"
                    else
                    {
                        XmlDetalleGuiaRemision detail = destinatario.Detalles.FirstOrDefault(
                                                        d => d.CodigoAdicional.Equals(remissionGuideDetail.Item.masterCode));

                        if (detail == null)
                        {
                            var codigoInterno = (!string.IsNullOrEmpty(remissionGuideDetail.Item.auxCode)) ? remissionGuideDetail.Item.auxCode : remissionGuideDetail.Item.masterCode;
                            XmlDetalleGuiaRemision xmlDetalle = new XmlDetalleGuiaRemision
                            {
                                CodigoAdicional = ProcesarCodigo(remissionGuideDetail.Item.masterCode),
                                CodigoInterno = ProcesarCodigo(codigoInterno),
                                Descripcion = ProcesarTexto(remissionGuideDetail.Item.name),
                                Cantidad = decimal.Round(remissionGuideDetail.quantityOrdered, 2, MidpointRounding.AwayFromZero)
                            };

                            destinatario.Detalles.Add(xmlDetalle);
                        }
                    }
                    #endregion

                }
            }


            if (destinatarios.Count <= 0/* IsEmpty()*/)
            {
                XmlDestinatario xmlDestinatario = new XmlDestinatario
                {
                    CodigoDocumentoSustento = "00",
                    CodigoEstablecimientoDestino = "000",
                    DireccionDestinatario = ProcesarTexto(guiaRemision.Person.address),
                    DocumentoAduaneroUnico = ProcesarCodigo(string.IsNullOrEmpty(guiaRemision.uniqueCustomDocument) ? "000" : guiaRemision.uniqueCustomDocument),
                    FechaEmisionDocumentoSustento = guiaRemision.Document.emissionDate.ToString("dd/MM/yyyy"),
                    IdentificacionDestinatario = ProcesarCodigo(guiaRemision.Person.identification_number),
                    MotivoTraslado = ProcesarTexto(guiaRemision.RemissionGuideReason.name),
                    NumeroAutorizacionDocumentoSustento = guiaRemision.Document.accessKey,
                    NumeroDocumentoSustento = ProcesarCodigo(guiaRemision.Document.number),
                    RazonSocialDestinatario = ProcesarTexto(guiaRemision.Person.fullname_businessName),
                    Ruta = guiaRemision.route

                };

                foreach (RemissionGuideDetail detalle in guiaRemision.RemissionGuideDetail)
                {
                    var codigoInterno = (!string.IsNullOrEmpty(detalle.Item.auxCode)) ? detalle.Item.auxCode : detalle.Item.masterCode;
                    XmlDetalleGuiaRemision xmlDetalle = new XmlDetalleGuiaRemision
                    {
                        CodigoAdicional = ProcesarTexto(detalle.Item.masterCode),
                        CodigoInterno = ProcesarCodigo(codigoInterno),
                        Descripcion = ProcesarTexto(detalle.Item.name),
                        Cantidad = decimal.Round(detalle.quantityOrdered, 2, MidpointRounding.AwayFromZero)
                    };
                    xmlDestinatario.Detalles.Add(xmlDetalle);
                }

                destinatarios.Add(xmlDestinatario);
            }

            List<XmlCampoAdicional> camposAdicionales = null;

            XmlGuiaRemision xmlGuiaRemision = new XmlGuiaRemision(informacionTributaria, infoGuiaRemision, destinatarios, camposAdicionales);

            return xmlGuiaRemision;
        }

        public static XmlGuiaRemision2 GuiaRemision3Xml(int id_remissionGuide, string driverNameThird = "", string carRegistrationThird = "")
        {
            RemissionGuide guiaRemision = db.RemissionGuide.Where(g => g.id == id_remissionGuide).FirstOrDefault();

            if (guiaRemision == null)
                return null;
            var id_company = guiaRemision.Document.EmissionPoint.id_company;
            string emissionDate = guiaRemision.Document.emissionDate.ToString("dd/MM/yyyy");


            Company vrCompa = db.Company.Where(h => h.id == id_company).FirstOrDefault();


            int ambiente = vrCompa.CompanyElectronicFacturation.EnvironmentType.codeSRI;
            int tipoEmision = vrCompa.CompanyElectronicFacturation.EmissionType.codeSRI;
            string razonSocial = ProcesarTexto(vrCompa.businessName);
            string nombreComercial = ProcesarTexto(vrCompa.trademark);
            string ruc = ProcesarCodigo(vrCompa.ruc);
            string codigoDocumento = ProcesarTexto(guiaRemision.Document.DocumentType.codeSRI);
            string establecimiento = ProcesarTexto(guiaRemision.Document.EmissionPoint.BranchOffice.code.PadLeft(3, '0'));
            int puntoEmision = guiaRemision.Document.EmissionPoint.code;
            int secuencial = guiaRemision.Document.sequential;
            int _id_prg = (int)guiaRemision.id_providerRemisionGuide;


            string direccionMatriz = ProcesarTexto(vrCompa.address);
            string claveAcceso = guiaRemision.Document.accessKey;

            #region"Informacion Tributaria"
            XmlInformacionTributaria informacionTributaria = new XmlInformacionTributaria(
                ambiente,
                tipoEmision,
                razonSocial,
                nombreComercial,
                ruc,
                codigoDocumento,
                establecimiento,
                puntoEmision,
                secuencial,
                direccionMatriz,
                claveAcceso
            );

            string carRegistration = "";

            if (guiaRemision.RemissionGuideTransportation.isOwn)
            {
                carRegistration = carRegistrationThird;
            }
            else
            {
                if (guiaRemision.RemissionGuideTransportation.DriverVeicleProviderTransport != null)
                {
                    carRegistration = guiaRemision.RemissionGuideTransportation.DriverVeicleProviderTransport.VeicleProviderTransport.Vehicle.carRegistration;
                }
                else
                {
                    carRegistration = guiaRemision.RemissionGuideTransportation.carRegistration;
                }
            }

            #endregion

            #region"Info Guia Remision"
            var razonSocialTransportista = (guiaRemision.RemissionGuideTransportation.Person != null)
                ? guiaRemision.RemissionGuideTransportation.Person.fullname_businessName
                : guiaRemision.Document.EmissionPoint.BranchOffice.Division.Company.businessName;

            var rucTransportista = (guiaRemision.RemissionGuideTransportation.Person != null) 
                ? guiaRemision.RemissionGuideTransportation.Person.identification_number 
                : guiaRemision.Document.EmissionPoint.BranchOffice.Division.Company.ruc;

            if (String.IsNullOrEmpty(rucTransportista))
            {
                throw new Exception("Ruc de transportista es obligatorio.");
            }

            var tipoIdentTransportista = (guiaRemision.RemissionGuideTransportation.Person != null)
                ? guiaRemision.RemissionGuideTransportation.Person.IdentificationType.codeSRI : "04";

            XmlInfoGuiaRemision infoGuiaRemision = new XmlInfoGuiaRemision
            {
                ContribuyenteEspecial = ProcesarTexto(vrCompa.CompanyElectronicFacturation.resolutionNumber),
                DireccionEstablecimiento = ProcesarTexto(guiaRemision.Document.EmissionPoint.BranchOffice.address),
                DireccionPartida = ProcesarTexto(guiaRemision.startAdress),
                FechaFinTransporte = emissionDate,
                FechaInicioTransporte = emissionDate,
                ObligadoContabilidad = (vrCompa.CompanyElectronicFacturation.requireAccounting) ? "SI" : "NO",
                Placa = ProcesarTexto(carRegistration),
                RazonSocialTransportista = ProcesarTexto(razonSocialTransportista),
                Rise = vrCompa.CompanyElectronicFacturation.rise,
                RucTransportista = ProcesarCodigo(rucTransportista),
                TipoIdentificacionTransportista = ProcesarTexto(tipoIdentTransportista),
            };
            #endregion

            #region"Destinatarios"
            List<XmlDestinatario2> destinatarios = new List<XmlDestinatario2>();

            ProductionUnitProvider pup = guiaRemision.ProductionUnitProvider ?? db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == guiaRemision.id_productionUnitProvider);

            if (pup != null)
            {
                XmlDestinatario2 destinatario = new XmlDestinatario2
                {
                    IdentificacionDestinatario = ProcesarCodigo(pup.Provider.Person.identification_number),
                    RazonSocialDestinatario = ProcesarTexto(pup.Provider.Person.fullname_businessName),
                    DireccionDestinatario = ProcesarTexto(pup.address),
                    MotivoTraslado = ProcesarTexto(guiaRemision.RemissionGuideReason.name),
                };
                if (!guiaRemision.RemissionGuideTransportation.isOwn)
                {
                    int grgdm = guiaRemision.RemissionGuideDispatchMaterial.Where(W => W.isActive && W.sourceExitQuantity > 0).ToList().Count();
                    if (grgdm > 0)
                    {
                        foreach (RemissionGuideDispatchMaterial rgdm in guiaRemision.RemissionGuideDispatchMaterial)
                        {
                            if (rgdm.sourceExitQuantity > 0)
                            {
                                var codigoInterno = (!string.IsNullOrEmpty(rgdm.Item.auxCode)) ? rgdm.Item.auxCode : rgdm.Item.masterCode;
                                XmlDetalleGuiaRemision xmlDetalle = new XmlDetalleGuiaRemision
                                {
                                    CodigoAdicional = ProcesarCodigo(rgdm.Item.masterCode),
                                    CodigoInterno = ProcesarCodigo(codigoInterno),
                                    Descripcion = ProcesarTexto(rgdm.Item.name),
                                    Cantidad = decimal.Round(rgdm.sourceExitQuantity, 2, MidpointRounding.AwayFromZero)
                                };
                                destinatario.Detalles.Add(xmlDetalle);
                            }

                        }
                        destinatarios.Add(destinatario);
                    }
                    else
                    {
                        foreach (RemissionGuideDetail rgd in guiaRemision.RemissionGuideDetail)
                        {
                            var codigoInterno = (!string.IsNullOrEmpty(rgd.Item.auxCode)) ? rgd.Item.auxCode : rgd.Item.masterCode;
                            XmlDetalleGuiaRemision xmlDetalle = new XmlDetalleGuiaRemision
                            {
                                CodigoAdicional = ProcesarCodigo(rgd.Item.masterCode),
                                CodigoInterno = ProcesarCodigo(codigoInterno),
                                Descripcion = ProcesarTexto(rgd.Item.name),
                                Cantidad = decimal.Round(rgd.quantityProgrammed, 2, MidpointRounding.AwayFromZero)
                            };
                            destinatario.Detalles.Add(xmlDetalle);

                        }
                        destinatarios.Add(destinatario);
                    }

                }
                else
                {
                    foreach (RemissionGuideDetail rgd in guiaRemision.RemissionGuideDetail)
                    {
                        if (rgd.quantityProgrammed > 0)
                        {
                            var codigoInterno = (!string.IsNullOrEmpty(rgd.Item.auxCode)) ? rgd.Item.auxCode : rgd.Item.masterCode;
                            XmlDetalleGuiaRemision xmlDetalle = new XmlDetalleGuiaRemision
                            {
                                CodigoAdicional = ProcesarCodigo(rgd.Item.masterCode),
                                CodigoInterno = ProcesarCodigo(codigoInterno),
                                Descripcion = ProcesarTexto(rgd.Item.name),
                                Cantidad = decimal.Round(rgd.quantityProgrammed, 2, MidpointRounding.AwayFromZero)
                            };

                            destinatario.Detalles.Add(xmlDetalle);
                        }

                    }
                    destinatarios.Add(destinatario);
                }

            }
            #endregion

            #region"Campos Adicionales"
            List<XmlCampoAdicional> camposAdicionales = new List<XmlCampoAdicional>();
            var lst = db.SettingDetail.Where(w => w.Setting.code == "DXPCARGPXP").ToList();
            if (lst != null && lst.Count > 0)
            {
                foreach (var det in lst)
                {
                    if (det.value == "CA1")
                    {
                        XmlCampoAdicional xmlCampoAdicional = new XmlCampoAdicional
                        {
                            Nombre = det.valueAux.ToString(),
                            Valor = guiaRemision
                                    .RemissionGuideDetail
                                    .Select(s => s.quantityProgrammed)
                                    .DefaultIfEmpty(0).Sum().ToString()
                        };
                        camposAdicionales.Add(xmlCampoAdicional);
                    }
                    if (det.value == "CA2")
                    {

                        XmlCampoAdicional xmlCampoAdicional = new XmlCampoAdicional
                        {
                            Nombre = det.valueAux.ToString(),
                            Valor = (db.Person.FirstOrDefault(fod => fod.id == _id_prg).email ?? (db.Setting.FirstOrDefault(fod => fod.code == "CAL1")?.value ?? ""))
                        };
                        camposAdicionales.Add(xmlCampoAdicional);
                    }
                    if (det.value == "CA3")
                    {
                        XmlCampoAdicional xmlCampoAdicional = new XmlCampoAdicional
                        {
                            Nombre = det.valueAux.ToString(),
                            Valor = db.Setting.FirstOrDefault(fod => fod.code == "CAL2")?.value ?? ""
                        };
                        camposAdicionales.Add(xmlCampoAdicional);
                    }
                    if (det.value == "CA4")
                    {
                        XmlCampoAdicional xmlCampoAdicional = new XmlCampoAdicional
                        {
                            Nombre = det.valueAux.ToString(),
                            Valor = guiaRemision.despachureDate.ToString("dd/MM/yyyy")
                        };
                        camposAdicionales.Add(xmlCampoAdicional);
                    }
                    if (det.value == "CA5")
                    {
                        XmlCampoAdicional xmlCampoAdicional = new XmlCampoAdicional
                        {
                            Nombre = det.valueAux.ToString(),
                            Valor = db.Setting.FirstOrDefault(fod => fod.code == "TPRG")?.value ?? ""
                        };
                        camposAdicionales.Add(xmlCampoAdicional);
                    }
                }
            }
            #endregion

            XmlGuiaRemision2 xmlGuiaRemision = new XmlGuiaRemision2(informacionTributaria, infoGuiaRemision, destinatarios, camposAdicionales);

            return xmlGuiaRemision;

        }

        public static XmlGuiaRemision2 GuiaRemision4Xml(int id_remissionGuideRiver)
        {
            RemissionGuideRiver guiaRemision = db.RemissionGuideRiver.Where(g => g.id == id_remissionGuideRiver).FirstOrDefault();

            if (guiaRemision == null)
                return null;
            var id_company = guiaRemision.Document.EmissionPoint.id_company;
            string emissionDate = guiaRemision.Document.emissionDate.ToString("dd/MM/yyyy");

            Company vrCompa = db.Company.Where(h => h.id == id_company).FirstOrDefault();


            int ambiente = vrCompa.CompanyElectronicFacturation.EnvironmentType.codeSRI;
            int tipoEmision = vrCompa.CompanyElectronicFacturation.EmissionType.codeSRI;
            string razonSocial = ProcesarTexto(vrCompa.businessName);
            string nombreComercial = ProcesarTexto(vrCompa.trademark);
            string ruc = ProcesarCodigo(vrCompa.ruc);
            string codigoDocumento = ProcesarCodigo(guiaRemision.Document.DocumentType.codeSRI);
            string establecimiento = ProcesarCodigo(guiaRemision.Document.EmissionPoint.BranchOffice.code.PadLeft(3, '0'));
            int puntoEmision = guiaRemision.Document.EmissionPoint.code;
            int secuencial = guiaRemision.Document.sequential;
            string direccionMatriz = ProcesarTexto(vrCompa.address);
            string claveAcceso = ProcesarCodigo(guiaRemision.Document.accessKey);

            #region"Informacion Tributaria"
            XmlInformacionTributaria informacionTributaria = new XmlInformacionTributaria(
                ambiente,
                tipoEmision,
                razonSocial,
                nombreComercial,
                ruc,
                codigoDocumento,
                establecimiento,
                puntoEmision,
                secuencial,
                direccionMatriz,
                claveAcceso
            );

            string carRegistration = "";
            if (guiaRemision.RemissionGuideRiverTransportation.DriverVeicleProviderTransport != null)
            {
                carRegistration = guiaRemision.RemissionGuideRiverTransportation.DriverVeicleProviderTransport.VeicleProviderTransport.Vehicle.carRegistration;
            }
            else
            {
                carRegistration = guiaRemision.RemissionGuideRiverTransportation.Vehicle.carRegistration;
            }
            #endregion

            #region"Info Guia Remision"
            var razonSocialTransportista = (guiaRemision.RemissionGuideRiverTransportation.Person != null) 
                ? guiaRemision.RemissionGuideRiverTransportation.Person.fullname_businessName 
                : guiaRemision.Document.EmissionPoint.BranchOffice.Division.Company.businessName;

            var rucTransportista = (guiaRemision.RemissionGuideRiverTransportation.Person != null)
                ? guiaRemision.RemissionGuideRiverTransportation.Person.identification_number
                : guiaRemision.Document.EmissionPoint.BranchOffice.Division.Company.ruc;

            if (String.IsNullOrEmpty(rucTransportista))
            {
                throw new Exception("RUC del transportista es obligatorio.");
            }

            var tipoIdentificacionTransportista = (guiaRemision.RemissionGuideRiverTransportation.Person != null)
                ? guiaRemision.RemissionGuideRiverTransportation.Person.IdentificationType.codeSRI : "04";

            XmlInfoGuiaRemision infoGuiaRemision = new XmlInfoGuiaRemision
            {
                ContribuyenteEspecial = ProcesarTexto(vrCompa.CompanyElectronicFacturation.resolutionNumber),
                DireccionEstablecimiento = ProcesarTexto(guiaRemision.Document.EmissionPoint.BranchOffice.address),
                DireccionPartida = ProcesarTexto(guiaRemision.startAdress),
                FechaFinTransporte = emissionDate,
                FechaInicioTransporte = emissionDate,
                ObligadoContabilidad = (vrCompa.CompanyElectronicFacturation.requireAccounting) ? "SI" : "NO",
                Placa = ProcesarTexto(carRegistration),
                RazonSocialTransportista = ProcesarTexto(razonSocialTransportista),
                Rise = ProcesarCodigo(vrCompa.CompanyElectronicFacturation.rise),
                RucTransportista = ProcesarTexto(rucTransportista),
                TipoIdentificacionTransportista = ProcesarCodigo(tipoIdentificacionTransportista)
            };
            #endregion

            #region"Destinatarios"
            List<XmlDestinatario2> destinatarios = new List<XmlDestinatario2>();

            ProductionUnitProvider pup = guiaRemision.ProductionUnitProvider ?? db.ProductionUnitProvider.FirstOrDefault(fod => fod.id == guiaRemision.id_productionUnitProvider);

            if (pup != null)
            {
                XmlDestinatario2 destinatario = new XmlDestinatario2
                {
                    IdentificacionDestinatario = ProcesarCodigo(pup.Provider.Person.identification_number),
                    RazonSocialDestinatario = ProcesarTexto(pup.Provider.Person.fullname_businessName),
                    DireccionDestinatario = ProcesarTexto(pup.address),
                    MotivoTraslado = ProcesarTexto(guiaRemision.RemissionGuideReason.name),
                };
                foreach (RemissionGuideRiverDispatchMaterial rgdm in guiaRemision.RemissionGuideRiverDispatchMaterial)
                {
                    var codigoInterno = !string.IsNullOrEmpty(rgdm.Item.auxCode) ? rgdm.Item.auxCode : rgdm.Item.masterCode;
                    XmlDetalleGuiaRemision xmlDetalle = new XmlDetalleGuiaRemision
                    {
                        CodigoAdicional = ProcesarCodigo(rgdm.Item.masterCode),
                        CodigoInterno = ProcesarCodigo(codigoInterno),
                        Descripcion = ProcesarTexto(rgdm.Item.name),
                        Cantidad = decimal.Round(rgdm.sourceExitQuantity, 2, MidpointRounding.AwayFromZero)
                    };

                    destinatario.Detalles.Add(xmlDetalle);
                }
                destinatarios.Add(destinatario);
            }
            #endregion

            #region"Campos Adicionales"
            List<XmlCampoAdicional> camposAdicionales = new List<XmlCampoAdicional>();
            var lst = db.SettingDetail.Where(w => w.Setting.code == "DXPCARGPXP").ToList();
            if (lst != null && lst.Count > 0)
            {
                foreach (var det in lst)
                {
                    if (det.value == "CA1")
                    {
                        XmlCampoAdicional xmlCampoAdicional = new XmlCampoAdicional
                        {
                            Nombre = det.valueAux.ToString(),
                            Valor = guiaRemision
                                    .RemissionGuideRiverDetail
                                    .Select(s => s.quantityProgrammed)
                                    .DefaultIfEmpty(0).Sum().ToString()
                        };
                        camposAdicionales.Add(xmlCampoAdicional);
                    }
                    if (det.value == "CA2")
                    {
                        XmlCampoAdicional xmlCampoAdicional = new XmlCampoAdicional
                        {
                            Nombre = det.valueAux.ToString(),
                            Valor = db.Setting.FirstOrDefault(fod => fod.code == "CAL1")?.value ?? ""
                        };
                        camposAdicionales.Add(xmlCampoAdicional);
                    }
                    if (det.value == "CA3")
                    {
                        XmlCampoAdicional xmlCampoAdicional = new XmlCampoAdicional
                        {
                            Nombre = det.valueAux.ToString(),
                            Valor = db.Setting.FirstOrDefault(fod => fod.code == "CAL2")?.value ?? ""
                        };
                        camposAdicionales.Add(xmlCampoAdicional);
                    }
                    if (det.value == "CA4")
                    {
                        XmlCampoAdicional xmlCampoAdicional = new XmlCampoAdicional
                        {
                            Nombre = det.valueAux.ToString(),
                            Valor = guiaRemision.despachureDate.ToString("dd/MM/yyyy")
                        };
                        camposAdicionales.Add(xmlCampoAdicional);
                    }
                    if (det.value == "CA5")
                    {
                        XmlCampoAdicional xmlCampoAdicional = new XmlCampoAdicional
                        {
                            Nombre = det.valueAux.ToString(),
                            Valor = db.Setting.FirstOrDefault(fod => fod.code == "TPRG")?.value ?? ""
                        };
                        camposAdicionales.Add(xmlCampoAdicional);
                    }
                }
            }
            #endregion


            XmlGuiaRemision2 xmlGuiaRemision = new XmlGuiaRemision2(informacionTributaria, infoGuiaRemision, destinatarios, camposAdicionales);

            return xmlGuiaRemision;
        }

        public static XmlRetencion Retencion2Xml(int id_retencion)
        {
            return null;
        }

        public static XmlNotaCredito NotaCredito2Xml(int id_notaCredito)
        {
            return null;
        }

        public static XmlNotaDebito NotaDebito2Xml(int id_notaDebito)
        {
            return null;
        }

        public static XmlDocument SerializeToXml<T>(T source, XmlSerializerNamespaces ns)
        {
            var document = new XmlDocument();
            var navigator = document.CreateNavigator();

            using (var writer = navigator.AppendChild())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, source, ns);
            }
            return document;
        }

        #region Procesamiento de textos para el SRI
        public static string ProcesarTexto(string texto)
        {
            return EliminarCaracteresEspeciales(texto, false);
        }

        public static string ProcesarCodigo(string texto)
        {
            return EliminarCaracteresEspeciales(texto, true);
        }

        private static string EliminarCaracteresEspeciales(string texto, bool esNumDocumento)
        {
            if (string.IsNullOrEmpty(texto)) return texto;

            var textoAuxiliar = texto;
            if (esNumDocumento)
            {
                textoAuxiliar = textoAuxiliar.Replace(" ", ""); // Eliminamos los espacios en blanco en el texto
            }

            textoAuxiliar = textoAuxiliar.Replace("\n", ""); // Eliminamos los saltos de línea
            textoAuxiliar = textoAuxiliar.Trim('.', ',', '_', '-'); // Eliminamos caracteres especiales
            textoAuxiliar = textoAuxiliar.Trim(); // Eliminamos espacios en blanco al final

            return textoAuxiliar;
        } 
        #endregion
    }
}