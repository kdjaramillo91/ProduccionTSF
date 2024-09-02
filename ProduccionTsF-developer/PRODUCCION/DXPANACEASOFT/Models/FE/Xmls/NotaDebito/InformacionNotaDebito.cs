using System.Collections.Generic;
using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.NotaDebito
{
    public class InformacionNotaDebito
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

        ///<totalSinImpuestos>3.14</totalSinImpuestos>
        [XmlElement(ElementName = "totalSinImpuestos", Order = 12, IsNullable = false)]
        public decimal TotalSinImpuestos { get; set; }

        ///<impuestos></impuestos>
        [XmlArray(ElementName = "impuestos", Order = 13, IsNullable = false)]
        [XmlArrayItem(ElementName = "impuesto", IsNullable = false)]
        public List<Impuesto> Impuestos { get; set; }

        ///<valorTotal>0</valorTotal>
        [XmlElement(ElementName = "valorTotal", Order = 14, IsNullable = false)]
        public decimal ValorTotal { get; set; }

        public InformacionNotaDebito()
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
            Impuestos = new List<Impuesto>();
            ValorTotal = 0.0M;
        }

        public InformacionNotaDebito(string fechaEmision, string direccionEstablecimiento, string tipoIdentificacionComprador,
            string razonSocialComprador, string identificacionComprador, string contribuyenteEspecial, string obligadoContabilidad,
            string rise, string codigoDocumentoModificado, string numeroDocumentoModificado, string fechaEmisionDocumentoSustento,
            decimal totalSinImpuestos, List<Impuesto> impuestos, decimal valorTotal)
        {
            FechaEmision = fechaEmision;
            DireccionEstablecimiento = direccionEstablecimiento;
            TipoIdentificacionComprador = tipoIdentificacionComprador;
            RazonSocialComprador = razonSocialComprador;
            IdentificacionComprador = identificacionComprador;
            ContribuyenteEspecial = contribuyenteEspecial;
            ObligadoContabilidad = obligadoContabilidad;
            Rise = rise;
            CodigoDocumentoModificado = codigoDocumentoModificado;
            NumeroDocumentoModificado = numeroDocumentoModificado;
            FechaEmisionDocumentoSustento = fechaEmisionDocumentoSustento;
            TotalSinImpuestos = totalSinImpuestos;
            Impuestos = impuestos;
            ValorTotal = valorTotal;
        }
    }
}