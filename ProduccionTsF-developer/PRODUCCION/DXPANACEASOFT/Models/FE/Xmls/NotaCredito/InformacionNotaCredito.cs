using System.Collections.Generic;
using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.NotaCredito
{
    public class InformacionNotaCredito
    {
        ///<fechaEmision>01/01/1000</fechaEmision>
        [XmlElement(ElementName = "fechaEmision", Order = 1, IsNullable = false)]
        public string FechaEmision { get; set; }

        ///<dirEstablecimiento>Ax</dirEstablecimiento>
        [XmlElement(ElementName = "dirEstablecimiento", Order = 2, IsNullable = true)]
        public string DireccionEstablecimiento { get; set; }

        ///<tipoIdentificacionComprador>04</tipoIdentificacionComprador>
        [XmlElement(ElementName = "tipoIdentificacionComprador", Order = 3, IsNullable = false)]
        public string TipoIdentificacionComprador { get; set; }

        ///<razonSocialComprador>Ax</razonSocialComprador>
        [XmlElement(ElementName = "razonSocialComprador", Order = 4, IsNullable = false)]
        public string RazonSocialComprador { get; set; }

        ///<identificacionComprador>Ax</identificacionComprador>
        [XmlElement(ElementName = "identificacionComprador", Order = 5, IsNullable = false)]
        public string IdentificacionComprador { get; set; }
        
        ///<contribuyenteEspecial>123</contribuyenteEspecial>
        [XmlElement(ElementName = "contribuyenteEspecial", Order = 6, IsNullable = true)]
        public string ContribuyenteEspecial { get; set; }

        ///<obligadoContabilidad>SI</obligadoContabilidad>
        [XmlElement(ElementName = "obligadoContabilidad", Order = 7, IsNullable = true)]
        public string ObligadoContabilidad { get; set; }

        ///<rise>String</rise>
        [XmlElement(ElementName = "rise", Order = 8, IsNullable = false)]
        public string Rise { get; set; }

        ///<codDocModificado>00</codDocModificado>
        [XmlElement(ElementName = "codDocModificado", Order = 9, IsNullable = false)]
        public string CodigoDocumentoModificado { get; set; }

        ///<numDocModificado>000-000-000000000</numDocModificado>
        [XmlElement(ElementName = "numDocModificado", Order = 10, IsNullable = false)]
        public string NumeroDocumentoModificado { get; set; }

        ///<fechaEmisionDocSustento>01/01/1000</fechaEmisionDocSustento>
        [XmlElement(ElementName = "fechaEmisionDocSustento", Order = 11, IsNullable = false)]
        public string FechaEmisionDocumentoSustento { get; set; }

        ///<totalSinImpuestos>0</totalSinImpuestos>
        [XmlElement(ElementName = "totalSinImpuestos", Order = 12, IsNullable = false)]
        public decimal TotalSinImpuestos { get; set; }

        ///<valorModificacion>0</valorModificacion>
        [XmlElement(ElementName = "valorModificacion", Order = 13, IsNullable = false)]
        public decimal ValorModificacion { get; set; }

        ///<moneda>Ax</moneda>
        [XmlElement(ElementName = "moneda", Order = 14, IsNullable = true)]
        public string Moneda { get; set; }

        [XmlArray(ElementName = "totalConImpuestos", Order = 15, IsNullable = false)]
        [XmlArrayItem(ElementName = "totalImpuesto", IsNullable = false)]
        public List<TotalImpuesto> TotalConImpuestos { get; set; }

        ///<motivo>Ax</motivo>
        [XmlElement(ElementName = "motivo", Order = 16, IsNullable = true)]
        public string Motivo { get; set; }

        public InformacionNotaCredito()
        {
            FechaEmision = string.Empty;
            DireccionEstablecimiento = string.Empty;
            TipoIdentificacionComprador = string.Empty;
            RazonSocialComprador = string.Empty;
            IdentificacionComprador = string.Empty;
            ContribuyenteEspecial = string.Empty;
            ObligadoContabilidad = string.Empty;
            Rise = string.Empty;
            CodigoDocumentoModificado = string.Empty;
            NumeroDocumentoModificado = string.Empty;
            FechaEmisionDocumentoSustento = string.Empty;
            TotalSinImpuestos = 0.0M;
            ValorModificacion = 0.0M;
            Moneda = string.Empty;
            TotalConImpuestos = new List<TotalImpuesto>();            
            Motivo = string.Empty;
        }

        public InformacionNotaCredito(string fechaEmision, string direccionEstablecimiento, string contribuyenteEspecial,
            string obligadoContabilidad, string tipoIdentificacionComprador, string guiaRemision, string razonSocialComprador,
            string identificacionComprador, string direccionComprador, decimal totalSinImpuesto, decimal totalDescuento,
            List<TotalImpuesto> totalConImpuestos, decimal propina, decimal importeTotal, string moneda = "DOLAR")
        {
            FechaEmision = string.Empty;
            DireccionEstablecimiento = string.Empty;
            TipoIdentificacionComprador = string.Empty;
            RazonSocialComprador = string.Empty;
            IdentificacionComprador = string.Empty;
            ContribuyenteEspecial = string.Empty;
            ObligadoContabilidad = string.Empty;
            Rise = string.Empty;
            CodigoDocumentoModificado = string.Empty;
            NumeroDocumentoModificado = string.Empty;
            FechaEmisionDocumentoSustento = string.Empty;
            TotalSinImpuestos = 0.0M;
            ValorModificacion = 0.0M;
            Moneda = string.Empty;
            TotalConImpuestos = new List<TotalImpuesto>();
            Motivo = string.Empty;
        }
    }
}
