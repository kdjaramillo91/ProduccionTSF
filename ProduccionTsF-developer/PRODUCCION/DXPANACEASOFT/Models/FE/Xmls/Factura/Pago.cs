using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.Factura
{
    public class Pago
    {

        /* <formaPago>20</formaPago> */
        [XmlElement(ElementName = "formaPago", Order = 1, IsNullable = false)]
        public int FormaPago { get; set; }

        /* <total>148288.00</total> */
        [XmlElement(ElementName = "total", Order = 2, IsNullable = false)]
        public decimal Total { get; set; }

        /* <plazo>30</plazo> */
        [XmlElement(ElementName = "plazo", Order = 3, IsNullable = false)]
        public int Plazo { get; set; }

        /* <unidadTiempo>dias</unidadTiempo>  */
        [XmlElement(ElementName = "unidadTiempo", Order = 4, IsNullable = false)]
        public string UnidadTiempo { get; set; }


        public Pago()
        {
            FormaPago = 0;
            Total = 0;
            Plazo = 0;
            UnidadTiempo = "";
        }


        public Pago(int formaPago,  decimal total, int plazo, string unidadTiempo)
        {
            FormaPago = formaPago;
            Total = total;
            Plazo = plazo;
            UnidadTiempo = unidadTiempo;
        }

    }
}