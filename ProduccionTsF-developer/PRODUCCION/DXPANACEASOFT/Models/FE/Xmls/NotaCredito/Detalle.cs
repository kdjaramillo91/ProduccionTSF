using System.Collections.Generic;
using System.Xml.Serialization;
using DXPANACEASOFT.Models.FE.Xmls.Common;

namespace DXPANACEASOFT.Models.FE.Xmls.NotaCredito
{
    public class Detalle
    {
        ///<codigoInterno>Ax</codigoInterno>
        [XmlElement(ElementName = "codigoInterno", Order = 1, IsNullable = true)]
        public string CodigoInterno { get; set; }

        ///<codigoAdicional>Ax</codigoAdicional>
        [XmlElement(ElementName = "codigoAdicional", Order = 2, IsNullable = true)]
        public string CodigoAdicional { get; set; }

        ///<descripcion >Ax</descripcion>
        [XmlElement(ElementName = "descripcion", Order = 3, IsNullable = false)]
        public string Descripcion { get; set; }

        ///<cantidad >0</cantidad>
        [XmlElement(ElementName = "cantidad", Order = 4, IsNullable = false)]
        public decimal Cantidad { get; set; }

        ///<precioUnitario >0</precioUnitario>
        [XmlElement(ElementName = "precioUnitario", Order = 5, IsNullable = false)]
        public decimal PrecioUnitario { get; set; }

        ///<descuento >0</descuento>
        [XmlElement(ElementName = "descuento", Order = 6, IsNullable = false)]
        public decimal Descuento { get; set; }

        ///<precioTotalSinImpuesto >0</precioTotalSinImpuesto>
        [XmlElement(ElementName = "precioTotalSinImpuesto", Order = 7, IsNullable = false)]
        public decimal PrecioTotalSinImpueto { get; set; }

        [XmlArray(ElementName = "detallesAdicionales", Order = 8, IsNullable = true)]
        [XmlArrayItem(ElementName = "detAdicional", IsNullable = true)]
        public List<DetalleAdicional> DetallesAdicionales { get; set; }

        [XmlArray(ElementName = "impuestos", Order = 9, IsNullable = false)]
        [XmlArrayItem(ElementName = "impuesto", IsNullable = false)]
        public List<Impuesto> Impuestos { get; set; }

        public Detalle()
        {
            CodigoInterno = string.Empty;
            CodigoAdicional = string.Empty;
            Descripcion = string.Empty;
            Cantidad = 0.0M;
            PrecioUnitario = 0.0M;
            Descuento = 0.0M;
            PrecioTotalSinImpueto = 0.0M;
            DetallesAdicionales = new List<DetalleAdicional>();
            Impuestos = new List<Impuesto>();
        }

        public Detalle(string codigoPrincipal, string codigoAuxiliar, string descripcion,
            decimal cantidad, decimal precioUnitario, decimal descuento, decimal precioTotalSinImpuesto, List<Impuesto> impuestos, List<DetalleAdicional> detallesAdicionales = null)
        {
            CodigoInterno = codigoPrincipal;
            CodigoAdicional = codigoAuxiliar;
            Descripcion = descripcion;
            Cantidad = cantidad;
            PrecioUnitario = precioUnitario;
            Descuento = descuento;
            PrecioTotalSinImpueto = precioTotalSinImpuesto;
            DetallesAdicionales = detallesAdicionales;
            Impuestos = impuestos;
        }
    }
}
