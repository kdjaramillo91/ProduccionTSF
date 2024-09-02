using System.Collections.Generic;
using System.Xml.Serialization;
using DXPANACEASOFT.Models.FE.Xmls.Common;

namespace DXPANACEASOFT.Models.FE.Xmls.NotaCredito
{
    [XmlRoot("notaCredito")]
    public class NotaCredito
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        ///<infoTributaria></infoTributaria>
        [XmlElement(ElementName = "infoTributaria", Order = 1, IsNullable = false)]
        public InformacionTributaria InfoTributaria { get; set; }

        ///<infoNotaCredito></infoNotaCredito>
        [XmlElement(ElementName = "infoNotaCredito", Order = 2, IsNullable = false)]
        public InformacionNotaCredito InfoNotaCredito { get; set; }

        ///<detalles></detalles>
        [XmlArray(ElementName = "detalles", Order = 3, IsNullable = false)]
        [XmlArrayItem(ElementName = "detalle", IsNullable = false)]
        public List<Detalle> Detalles { get; set; }

        /// <infoTributaria></infoTributaria>
        [XmlArray(ElementName = "infoAdicional", Order = 4, IsNullable = true)]
        [XmlArrayItem(ElementName = "campoAdicional", IsNullable = true)]
        public List<CampoAdicional> InfoAdicional { get; set; }

        public NotaCredito()
        {
            Id = "comprobante";
            Version = "1.0.0";
            InfoTributaria = new InformacionTributaria();
            InfoNotaCredito = new InformacionNotaCredito();
            Detalles = new List<Detalle>();
            InfoAdicional = new List<CampoAdicional>();
        }

        public NotaCredito(InformacionTributaria infoTributaria, InformacionNotaCredito infoNotaCredito,
            List<Detalle> detalles, List<CampoAdicional> infoAdicional)
        {
            Id = "comprobante";
            Version = "1.0.0";
            InfoTributaria = infoTributaria;
            InfoNotaCredito = infoNotaCredito;
            Detalles = detalles;
            InfoAdicional = infoAdicional;
        }
    }
}
