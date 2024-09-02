using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.NotaDebito
{
    public class Motivo
    {
        ///<razon>Ax</razon>
        [XmlElement(ElementName = "razon", Order = 1, IsNullable = false)]
        public string Razon { get; set; }

        ///<valor>0</valor>
        [XmlElement(ElementName = "valor", Order = 2, IsNullable = false)]
        public decimal Valor { get; set; }

        public Motivo()
        {
            Razon = string.Empty;
            Valor = 0.0M;
        }

        public Motivo(string razon, decimal valor)
        {
            Razon = razon;
            Valor = valor;
        }
    }
}