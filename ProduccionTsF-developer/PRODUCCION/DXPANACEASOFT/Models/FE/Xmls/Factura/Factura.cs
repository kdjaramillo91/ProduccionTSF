using System.Collections.Generic;
using System.Xml.Serialization;
using DXPANACEASOFT.Models.FE.Xmls.Common;

namespace DXPANACEASOFT.Models.FE.Xmls.Factura
{
    [XmlRoot("factura")]    
    public class Factura
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        /// <infoTributaria></infoTributaria>
        [XmlElement(ElementName = "infoTributaria", Order = 1, IsNullable = false)]
        public InformacionTributaria InfoTributaria { get; set; }

        /// <infoFactura></infoFactura>
        [XmlElement(ElementName = "infoFactura", Order = 2, IsNullable = false)]
        public InformacionFactura InfoFactura { get; set; }

        ///<detalles></detalles>
        [XmlArray(ElementName = "detalles", Order = 3, IsNullable = false)]
        [XmlArrayItem(ElementName = "detalle", IsNullable = false)]
        public List<Detalle> Detalles { get; set; }

        ///<infoAdicional></infoAdicional>
        [XmlArray(ElementName = "infoAdicional", Order = 4, IsNullable = true)]
        [XmlArrayItem(ElementName = "campoAdicional", IsNullable = true)]
        public List<CampoAdicional> CamposAdicionales { set; get; }

        public Factura()
        {
            Id = "comprobante";
            Version = "1.1.0";
            InfoTributaria = new InformacionTributaria();
            InfoFactura = new InformacionFactura();
            Detalles = new List<Detalle>();
            CamposAdicionales = new List<CampoAdicional>(); 
        }

        public Factura(InformacionTributaria informacionTributaria, InformacionFactura informacionFactura, List<Detalle> detalles, List<CampoAdicional> camposAdicionales)
        {
            Id = "comprobante";
            Version = "1.1.0";
            InfoTributaria = informacionTributaria;
            InfoFactura = informacionFactura;
            Detalles = detalles;
            CamposAdicionales = camposAdicionales;
        }
    }
}
