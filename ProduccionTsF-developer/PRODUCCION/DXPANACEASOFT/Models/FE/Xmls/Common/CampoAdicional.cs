using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.Common
{
    ///<campoAdicional nombre="Ax">Ax</campoAdicional>
    public class CampoAdicional
    {
        [XmlAttribute(AttributeName = "nombre")]
        public string Nombre { get; set; }

        [XmlText]
        public string Valor { get; set; }

        public CampoAdicional()
        {
            Nombre = string.Empty;
            Valor = string.Empty;
        }

        public CampoAdicional(string nombre, string valor)
        {
            Nombre = nombre;
            Valor = valor;
        }
    }
}
