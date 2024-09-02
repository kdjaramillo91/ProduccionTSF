using System.Collections.Generic;
using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.Factura
{
    public class InformacionFactura
    {
        ///<fechaEmision>01/01/1000</fechaEmision>
        [XmlElement(ElementName = "fechaEmision", Order = 1, IsNullable = false)]
        public string FechaEmision { get; set; }

        ///<dirEstablecimiento>Ax</dirEstablecimiento>
        [XmlElement(ElementName = "dirEstablecimiento", Order = 2, IsNullable = true)]
        public string DireccionEstablecimiento { get; set; }

        ///<contribuyenteEspecial>123</contribuyenteEspecial>
        [XmlElement(ElementName = "contribuyenteEspecial", Order = 3, IsNullable = true)]
        public string ContribuyenteEspecial { get; set; }

        ///<obligadoContabilidad>SI</obligadoContabilidad>
        [XmlElement(ElementName = "obligadoContabilidad", Order = 4, IsNullable = true)]
        public string ObligadoContabilidad { get; set; }

       /// Grupo de campos factura del exterior
        
        ///<comercioExterior>EXPORTADOR</comercioExterior>
        [XmlElement(ElementName = "comercioExterior", Order = 5, IsNullable = false)]
        public string ComercioExterior { get; set; }

        ///<incoTermFactura>FOB</incoTermFactura>
        [XmlElement(ElementName = "incoTermFactura", Order = 6, IsNullable = false)]
        public string IncoTermFactura { get; set; }

        ///<lugarIncoTerm>GUAYAQUIL</lugarIncoTerm>
        [XmlElement(ElementName = "lugarIncoTerm", Order = 7, IsNullable = false)]
        public string LugarIncoTerm { get; set; }

        ///<paisOrigen>593</paisOrigen>   
        [XmlElement(ElementName = "paisOrigen", Order = 8, IsNullable = false)]
        public string PaisOrigen { get; set; }

        ///<puertoEmbarque>INARPI, GUAYAQUIL</puertoEmbarque>  
        [XmlElement(ElementName = "puertoEmbarque", Order = 9, IsNullable = false)]
        public string PuertoEmbarque { get; set; }

        ///<puertoDestino>LE HAVRE, FRANCIA</puertoDestino>
        [XmlElement(ElementName = "puertoDestino", Order = 10, IsNullable = false)]
        public string PuertoDestino { get; set; }

        ///<paisDestino>211</paisDestino>
        [XmlElement(ElementName = "paisDestino", Order = 11, IsNullable = false)]
        public string PaisDestino { get; set; }

        ///<paisAdquisicion>593</paisAdquisicion>
        [XmlElement(ElementName = "paisAdquisicion", Order = 12, IsNullable = false)]
        public string PaisAdquisicion { get; set; }


        ///<tipoIdentificacionComprador>04</tipoIdentificacionComprador>
        [XmlElement(ElementName = "tipoIdentificacionComprador", Order = 13, IsNullable = false)]
        public string TipoIdentificacionComprador { get; set; }

        ///<guiaRemision>000-000-000000000</guiaRemision>
        [XmlElement(ElementName = "guiaRemision", Order = 14, IsNullable = true)]
        public string GuiaRemision { get; set; }

        ///<razonSocialComprador>Ax</razonSocialComprador>
        [XmlElement(ElementName = "razonSocialComprador", Order = 15, IsNullable = false)]
        public string RazonSocialComprador { get; set; }

        ///<identificacionComprador>Ax</identificacionComprador>
        [XmlElement(ElementName = "identificacionComprador", Order = 16, IsNullable = false)]
        public string IdentificacionComprador { get; set; }

        ///<dirComprador>salinas y santiago</dirComprador>
        [XmlElement(ElementName = "direccionComprador", Order = 17, IsNullable = true)]
        public string DireccionComprador { get; set; }

        ///<totalSinImpuestos>0</totalSinImpuestos>
        [XmlElement(ElementName = "totalSinImpuestos", Order = 18, IsNullable = false)]
        public decimal TotalSinImpuestos { get; set; }


        [XmlElement(ElementName = "incoTermTotalSinImpuestos", Order = 19, IsNullable = false)]
        public string IncoTermTotalSinImpuestos { get; set; }

        ///<totalDescuento>3.14</totalDescuento>
        [XmlElement(ElementName = "totalDescuento", Order = 20, IsNullable = false)]
        public decimal TotalDescuento { get; set; }

        [XmlArray(ElementName = "totalConImpuestos", Order = 21, IsNullable = false)]
        [XmlArrayItem(ElementName = "totalImpuesto", IsNullable = false)]
        public List<TotalImpuesto> totalConImpuestos { get; set; }

        
        ///<propina>0</propina>
        [XmlElement(ElementName = "propina", Order = 22, IsNullable = false)]
        public decimal Propina { get; set; }


        //Factura del exterior

        ///<fleteInternacional>0.00</fleteInternacional>
        [XmlElement(ElementName = "fleteInternacional", Order = 23, IsNullable = false)]
        public decimal FleteInternacional { get; set; }

        ///<seguroInternacional>0.00</seguroInternacional>
        [XmlElement(ElementName = "seguroInternacional", Order = 24, IsNullable = false)]
        public decimal SeguroInternacional { get; set; }

        ///<gastosAduaneros>0.00</gastosAduaneros>
        [XmlElement(ElementName = "gastosAduaneros", Order = 25, IsNullable = false)]
        public decimal GastosAduaneros { get; set; }

        ///<gastosTransporteOtros>0.00</gastosTransporteOtros> 
        [XmlElement(ElementName = "gastosTransporteOtros", Order = 26, IsNullable = false)]
        public decimal GastosTransporteOtros { get; set; }                                                                                                                                                        
        

        ///<importeTotal>0</importeTotal>
        [XmlElement(ElementName = "importeTotal", Order = 27, IsNullable = false)]
        public decimal ImporteTotal { get; set; }

        ///<moneda>Ax</moneda>
        [XmlElement(ElementName = "moneda", Order = 28, IsNullable = true)]
        public string Moneda { get; set; }

        

        [XmlArray(ElementName = "pagos", Order = 29, IsNullable = false)]
        [XmlArrayItem(ElementName = "pago", IsNullable = false)]
        public List<Pago> pagos { get; set; }

        public InformacionFactura()
        {
            FechaEmision = string.Empty;
            DireccionEstablecimiento = string.Empty;
            ContribuyenteEspecial = string.Empty;
            ObligadoContabilidad = string.Empty;
            TipoIdentificacionComprador = string.Empty;
            GuiaRemision = string.Empty;
            RazonSocialComprador = string.Empty;
            IdentificacionComprador = string.Empty;
            DireccionComprador = string.Empty;
            TotalSinImpuestos = 0.0M;
            TotalDescuento = 0.0M;
            totalConImpuestos = new List<TotalImpuesto>();
            pagos = new List<Pago>();
            Propina = 0.0M;
            ImporteTotal = 0.0M;
            Moneda = string.Empty;
        }

        public InformacionFactura(string fechaEmision, string direccionEstablecimiento, string contribuyenteEspecial,
            string obligadoContabilidad, string tipoIdentificacionComprador, string guiaRemision, string razonSocialComprador,
            string identificacionComprador, string direccionComprador, decimal totalSinImpuesto, decimal totalDescuento,
            List<TotalImpuesto> totalConImpuestos, List<Pago> pagos, decimal propina, decimal importeTotal, string moneda = "DOLAR")
        {
            FechaEmision = fechaEmision;
            DireccionEstablecimiento = direccionEstablecimiento;
            ContribuyenteEspecial = contribuyenteEspecial;
            ObligadoContabilidad = obligadoContabilidad;
            TipoIdentificacionComprador = tipoIdentificacionComprador;
            GuiaRemision = guiaRemision;
            RazonSocialComprador = razonSocialComprador;
            IdentificacionComprador = identificacionComprador;
            DireccionComprador = direccionComprador;
            TotalSinImpuestos = totalSinImpuesto;
            TotalDescuento = totalDescuento;
            this.totalConImpuestos = totalConImpuestos;
            this.pagos = pagos;
            Propina = propina;
            ImporteTotal = importeTotal;
            Moneda = moneda;
        }
    }
}
