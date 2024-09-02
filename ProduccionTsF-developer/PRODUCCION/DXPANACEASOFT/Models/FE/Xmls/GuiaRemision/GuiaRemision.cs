using System.Collections.Generic;
using System.Xml.Serialization;
using DXPANACEASOFT.Models.FE.Xmls.Common;

namespace DXPANACEASOFT.Models.FE.Xmls.GuiaRemision
{
    [XmlRoot("guiaRemision")]
    public class GuiaRemision
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }

        /// <infoTributaria></infoTributaria>
        [XmlElement(ElementName = "infoTributaria", Order = 1, IsNullable = false)]
        public InformacionTributaria InfoTributaria { get; set; }

        [XmlElement(ElementName = "infoGuiaRemision", Order = 2, IsNullable = false)]
        public InformacionGuiaRemision InfoGuiaRemision { get; set; }

        [XmlArray(ElementName = "destinatarios", Order = 3, IsNullable = false)]
        [XmlArrayItem(ElementName = "destinatario", IsNullable = false)]
        public List<Destinatario> Destinatarios { get; set; }

        /// <infoTributaria></infoTributaria>
        [XmlArray(ElementName = "infoAdicional", Order = 4, IsNullable = true)]
        [XmlArrayItem(ElementName = "campoAdicional", IsNullable = true)]
        public List<CampoAdicional> CamposAdicionales { set; get; }

        public GuiaRemision()
        {
            Id = "comprobante";
            Version = "1.0.0";
            InfoTributaria = new InformacionTributaria();
            InfoGuiaRemision = new InformacionGuiaRemision();
            Destinatarios = new List<Destinatario>();
            CamposAdicionales = new List<CampoAdicional>();
        }

        public GuiaRemision(InformacionTributaria infoTributaria, InformacionGuiaRemision infoGuiaRemision,
            List<Destinatario> destinatarios, List<CampoAdicional> camposAdicionales)
        {
            Id = "comprobante";
            Version = "1.0.0";
            InfoTributaria = infoTributaria;
            InfoGuiaRemision = infoGuiaRemision;
            Destinatarios = destinatarios;
            CamposAdicionales = camposAdicionales;
        }
    }
}
