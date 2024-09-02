using System.Collections.Generic;
using System.Xml.Serialization;
using DXPANACEASOFT.Models.FE.Xmls.Common;

namespace DXPANACEASOFT.Models.FE.Xmls.GuiaRemision
{
    public class Detalle
    {
        ///<codigoInterno>Ax</codigoInterno>
        [XmlElement(ElementName = "codigoInterno", Order = 1, IsNullable = false)]
        public string CodigoInterno { get; set; }

        ///<codigoAdicional>Ax</codigoAdicional>
        [XmlElement(ElementName = "codigoAdicional", Order = 2, IsNullable = false)]
        public string CodigoAdicional { get; set; }
        
        ///<descripcion>Ax</descripcion>
        [XmlElement(ElementName = "descripcion", Order = 3, IsNullable = false)]
        public string Descripcion { get; set; }

        ///<cantidad>0</cantidad>
        [XmlElement(ElementName = "cantidad", Order = 4, IsNullable = false)]
        public decimal Cantidad { get; set; }

        [XmlArray(ElementName = "detallesAdicionales", Order = 5, IsNullable = true)]
        [XmlArrayItem(ElementName = "detAdicional", IsNullable = true)]
        public List<DetalleAdicional> DetallesAdicionales { get; set; }

        public Detalle()
        {
            CodigoInterno = string.Empty;
            CodigoAdicional = string.Empty;
            Descripcion = string.Empty;
            Cantidad = 0.0M;
            DetallesAdicionales = new List<DetalleAdicional>();
        }

        public Detalle(string codigoInterno, string codigoAdicional, string descripcion, decimal cantidad, List<DetalleAdicional> detallesAdicionales)
        {
            CodigoInterno = codigoInterno;
            CodigoAdicional = codigoAdicional;
            Descripcion = descripcion;
            Cantidad = cantidad;
            DetallesAdicionales = detallesAdicionales;
        }
    }
}