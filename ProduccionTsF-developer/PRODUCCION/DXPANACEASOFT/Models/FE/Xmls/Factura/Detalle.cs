using System.Collections.Generic;
using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.Factura
{
    public class Detalle
    {
        ///<codigoPrincipal>Ax</codigoPrincipal>
        [XmlElement(ElementName = "codigoPrincipal", Order = 1, IsNullable = true)]
        public string CodigoPrincipal { get; set; }

        ///<codigoAuxiliar>Ax</codigoAuxiliar>
        [XmlElement(ElementName = "codigoAuxiliar", Order = 2, IsNullable = true)]
        public string CodigoAuxiliar { get; set; }

        ///<descripcion >Ax</descripcion>
        [XmlElement(ElementName = "descripcion", Order = 3, IsNullable = false)]
        public string Descripcion { get; set; }

        ///<descripcion >Ax</descripcion>
        [XmlElement(ElementName = "unidadMedida", Order = 4, IsNullable = false)]
        public string UnidadMedida { get; set; }

        
        ///<cantidad >0</cantidad>
        [XmlElement(ElementName = "cantidad", Order = 5, IsNullable = false)]
        public decimal Cantidad { get; set; }

        ///<precioUnitario >0</precioUnitario>
        [XmlElement(ElementName = "precioUnitario", Order = 6, IsNullable = false)]
        public decimal PrecioUnitario { get; set; }

        ///<descuento >0</descuento>
        [XmlElement(ElementName = "descuento", Order = 7, IsNullable = false)]
        public decimal Descuento { get; set; }

        ///<precioTotalSinImpuesto >0</precioTotalSinImpuesto>
        [XmlElement(ElementName = "precioTotalSinImpuesto", Order = 8, IsNullable = false)]
        public decimal PrecioTotalSinImpueto { get; set; }

        [XmlArray(ElementName = "detallesAdicionales", Order = 9, IsNullable = true)]
        [XmlArrayItem(ElementName = "detAdicional", IsNullable = true)]
        public List<Common.DetalleAdicional> DetallesAdicionales { get; set; }

        [XmlArray(ElementName = "impuestos", Order = 10, IsNullable = false)]
        [XmlArrayItem(ElementName = "impuesto", IsNullable = false)]
        public List<Impuesto> Impuestos { get; set; }

        public Detalle()
        {
            CodigoPrincipal = string.Empty;
            CodigoAuxiliar = string.Empty;
            Descripcion = string.Empty;
            Cantidad = 0.0M;
            PrecioUnitario = 0.0M;
            Descuento = 0.0M;
            PrecioTotalSinImpueto = 0.0M;
            DetallesAdicionales = new List<Common.DetalleAdicional>();
            Impuestos = new List<Impuesto>();
        }

        public Detalle(string codigoPrincipal, string codigoAuxiliar, string descripcion,
            decimal cantidad, decimal precioUnitario, decimal descuento, decimal precioTotalSinImpuesto, List<Impuesto> impuestos, List<Common.DetalleAdicional> detallesAdicionales = null)
        {
            CodigoPrincipal = codigoPrincipal;
            CodigoAuxiliar = codigoAuxiliar;
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
