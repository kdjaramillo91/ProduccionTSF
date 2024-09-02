using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.Retencion
{
    public class Impuesto
    {
        ///<codigo>0</codigo>
        [XmlElement(ElementName = "codigo", Order = 1, IsNullable = false)]
        public int Codigo { get; set; }

        ///<codigoRetencion>Ax</codigoRetencion>
        [XmlElement(ElementName = "codigoRetencion", Order = 2, IsNullable = false)]
        public string CodigoRetencion { get; set; }

        ///<baseImponible>0</baseImponible>
        [XmlElement(ElementName = "baseImponible", Order = 3, IsNullable = false)]
        public decimal BaseImponible { get; set; }

        ///<porcentajeRetener>0</porcentajeRetener>
        [XmlElement(ElementName = "porcentajeRetener", Order = 4, IsNullable = false)]
        public int PorcentajeRetener { get; set; }

        ///<valorRetenido>0</valorRetenido>
        [XmlElement(ElementName = "valorRetenido", Order = 5, IsNullable = false)]
        public decimal ValorRetenido { get; set; }

        ///<codDocSustento>00</codDocSustento>
        [XmlElement(ElementName = "codDocSustento", Order = 6, IsNullable = false)]
        public string CodigoDocumentoSustento { get; set; }

        ///<numDocSustento>000000000000000</numDocSustento>
        [XmlElement(ElementName = "numDocSustento", Order = 7, IsNullable = false)]
        public string NumeroDocumentoSustento { get; set; }

        ///<fechaEmisionDocSustento>01/01/1000</fechaEmisionDocSustento>
        [XmlElement(ElementName = "fechaEmisionDocSustento", Order = 8, IsNullable = false)]
        public string FechaEmisionDocumentoSustento { get; set; }

        public Impuesto()
        {
            Codigo = 0;
            CodigoRetencion = string.Empty;
            BaseImponible = 0.0M;
            PorcentajeRetener = 0;
            ValorRetenido = 0.0M;
            CodigoDocumentoSustento = string.Empty;
            NumeroDocumentoSustento = string.Empty;
            FechaEmisionDocumentoSustento = string.Empty;
        }

        public Impuesto(int codigo, string codigoRetencion, decimal baseImponible, int porcentajeRetener,
            decimal valorRetenido, string codigoDocumentoSustento, string numeroDocumentoSustento,
            string fechaEmisionDocumentoSustento)
        {
            Codigo = codigo;
            CodigoRetencion = codigoRetencion;
            BaseImponible = baseImponible;
            PorcentajeRetener = porcentajeRetener;
            ValorRetenido = valorRetenido;
            CodigoDocumentoSustento = codigoDocumentoSustento;
            NumeroDocumentoSustento = numeroDocumentoSustento;
            FechaEmisionDocumentoSustento = fechaEmisionDocumentoSustento;
        }
    }
}