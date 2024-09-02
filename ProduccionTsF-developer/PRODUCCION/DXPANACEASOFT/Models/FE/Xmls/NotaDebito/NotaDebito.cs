using System.Collections.Generic;
using System.Xml.Serialization;
using DXPANACEASOFT.Models.FE.Xmls.Common;

namespace DXPANACEASOFT.Models.FE.Xmls.NotaDebito
{
    [XmlRoot("notaDebito")]
    public class NotaDebito
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        ///<infoTributaria></infoTributaria>
        [XmlElement(ElementName = "infoTributaria", Order = 1, IsNullable = false)]
        public InformacionTributaria InfoTributaria { get; set; }

        ///<infoNotaDebito></infoNotaDebito>
        [XmlElement(ElementName = "infoNotaDebito", Order = 2, IsNullable = false)]
        public InformacionNotaDebito InfoNotaDebito { get; set; }

        ///<motivos></motivos>
        [XmlArray(ElementName = "motivos", Order = 3, IsNullable = false)]
        [XmlArrayItem(ElementName = "motivo", IsNullable = false)]
        public List<Motivo> Motivos { get; set; }

        ///<infoAdicional></infoAdicional>
        [XmlArray(ElementName = "infoAdicional", Order = 4, IsNullable = true)]
        [XmlArrayItem(ElementName = "campoAdicional", IsNullable = true)]
        public List<CampoAdicional> InfoAdicional { get; set; }

        public NotaDebito()
        {
            Id = "comprobante";
            Version = "1.0.0";
            InfoTributaria = new InformacionTributaria();
            InfoNotaDebito = new InformacionNotaDebito();
            Motivos = new List<Motivo>();
            InfoAdicional = new List<CampoAdicional>();
        }

        public NotaDebito(InformacionTributaria infoTributaria, InformacionNotaDebito infoNotaDebito, List<Motivo> motivos, List<CampoAdicional> infoAdicional)
        {
            Id = "comprobante";
            Version = "1.0.0";
            InfoTributaria = infoTributaria;
            InfoNotaDebito = infoNotaDebito;
            Motivos = motivos;
            InfoAdicional = infoAdicional;
        }
    }
}
