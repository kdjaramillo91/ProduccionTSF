using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.Common
{
    public class DetalleAdicional
    {
        [XmlAttribute(AttributeName = "nombre")]
        public string Nombre { get; set; }

        [XmlAttribute(AttributeName = "valor")]
        public string Valor { get; set; }

        public DetalleAdicional()
        {
            Nombre = string.Empty;
            Valor = string.Empty;
        }

        public DetalleAdicional(string nombre, string valor)
        {
            Nombre = nombre;
            Valor = valor;
        }
    }
}
