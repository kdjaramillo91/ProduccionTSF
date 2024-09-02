using System.Collections.Generic;
using System.Xml.Serialization;
using DXPANACEASOFT.Models.FE.Xmls.Common;

namespace DXPANACEASOFT.Models.FE.Xmls.Retencion
{
    [XmlRoot("comprobanteRetencion")]
    public class Retencion
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        /// <infoTributaria></infoTributaria>
        [XmlElement(ElementName = "infoTributaria", Order = 1, IsNullable = false)]
        public InformacionTributaria InfoTributaria { get; set; }

        ///<infoTributaria></infoTributaria>
        [XmlElement(ElementName = "infoCompRetencion", Order = 2, IsNullable = false)]
        public InformacionRetencion InfoRetencion { get; set; }

        ///<impuestos></impuestos>
        [XmlArray(ElementName = "impuestos", Order = 3, IsNullable = false)]
        [XmlArrayItem(ElementName = "impuesto", IsNullable = false)]
        public List<Impuesto> Impuestos { get; set; }

        ///<infoAdicional></infoAdicional>
        [XmlArray(ElementName = "infoAdicional", Order = 4, IsNullable = true)]
        [XmlArrayItem(ElementName = "campoAdicional", IsNullable = true)]
        public List<CampoAdicional> InfoAdicional { get; set; }

        public Retencion()
        {
            Id = "comprobante";
            Version = "1.0.0";
            InfoTributaria = new InformacionTributaria();
            InfoRetencion = new InformacionRetencion();
            Impuestos = new List<Impuesto>();
            InfoAdicional = new List<CampoAdicional>();
        }

        public Retencion(InformacionTributaria infoTributaria, InformacionRetencion infoRetencion,
            List<Impuesto> impuestos, List<CampoAdicional> infoAdicional)
        {
            Id = "comprobante";
            Version = "1.0.0";
            InfoTributaria = infoTributaria;
            InfoRetencion = infoRetencion;
            Impuestos = impuestos;
            InfoAdicional = infoAdicional;
        }
    }
}
