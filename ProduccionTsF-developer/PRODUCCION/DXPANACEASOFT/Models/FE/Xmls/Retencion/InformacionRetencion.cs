using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.Retencion
{
    public class InformacionRetencion
    {
        ///<fechaEmision>01/01/1000</fechaEmision>
        [XmlElement(ElementName = "fechaEmision", Order = 1, IsNullable = false)]
        public string FechaEmision { get; set; }

        ///<dirEstablecimiento>Ax</dirEstablecimiento>
        [XmlElement(ElementName = "dirEstablecimiento", Order = 2, IsNullable = false)]
        public string DireccionEstablecimiento { get; set; }

        ///<contribuyenteEspecial>123</contribuyenteEspecial>
        [XmlElement(ElementName = "contribuyenteEspecial", Order = 3, IsNullable = false)]
        public string ContribuyenteEspecial { get; set; }

        ///<obligadoContabilidad>SI</obligadoContabilidad>
        [XmlElement(ElementName = "obligadoContabilidad", Order = 4, IsNullable = false)]
        public string ObligadoContabilidad { get; set; }

        ///<tipoIdentificacionSujetoRetenido>04</tipoIdentificacionSujetoRetenido>
        [XmlElement(ElementName = "tipoIdentificacionSujetoRetenido", Order = 5, IsNullable = false)]
        public string TipoIdentificacionSujetoRetenido { get; set; }

        ///<razonSocialSujetoRetenido>Ax</razonSocialSujetoRetenido>
        [XmlElement(ElementName = "razonSocialSujetoRetenido", Order = 6, IsNullable = false)]
        public string RazonSocialSujetoRetenido { get; set; }

        ///<identificacionSujetoRetenido>Ax</identificacionSujetoRetenido>
        [XmlElement(ElementName = "identificacionSujetoRetenido", Order = 7, IsNullable = false)]
        public string IdentificacionSujetoRetenido { get; set; }

        ///<periodoFiscal>01/1000</periodoFiscal>
        [XmlElement(ElementName = "periodoFiscal", Order = 8, IsNullable = false)]
        public string PeriodoFiscal { get; set; }

        public InformacionRetencion()
        {
            FechaEmision = string.Empty;
            DireccionEstablecimiento = string.Empty;
            ContribuyenteEspecial = string.Empty;
            ObligadoContabilidad = string.Empty;
            TipoIdentificacionSujetoRetenido = string.Empty;
            RazonSocialSujetoRetenido = string.Empty;
            IdentificacionSujetoRetenido = string.Empty;
            PeriodoFiscal = string.Empty;
        }

        public InformacionRetencion(string fechaEmision, string direccionEstablecimiento, string contribuyenteEspecial,
            string obligadoContabilidad, string tipoIdentificacionSujetoRetenido, string razonSocialSujetoRetenido, 
            string identificacionSujetoRetenido, string periodoFiscal)
        {
            FechaEmision = fechaEmision;
            DireccionEstablecimiento = direccionEstablecimiento;
            ContribuyenteEspecial = contribuyenteEspecial;
            ObligadoContabilidad = obligadoContabilidad;
            TipoIdentificacionSujetoRetenido = tipoIdentificacionSujetoRetenido;
            RazonSocialSujetoRetenido = razonSocialSujetoRetenido;
            IdentificacionSujetoRetenido = identificacionSujetoRetenido;
            PeriodoFiscal = periodoFiscal;
        }
    }
}