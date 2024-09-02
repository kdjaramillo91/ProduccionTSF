using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.Factura
{
    public class TotalImpuesto
    {
        ///<codigo>0</codigo>
        [XmlElement(ElementName = "codigo", Order = 1, IsNullable = false)]
        public int Codigo { get; set; }

        ///<codigoPorcentaje>0</codigoPorcentaje>
        [XmlElement(ElementName = "codigoPorcentaje", Order = 2, IsNullable = false)]
        public int CodigoPorcentaje { get; set; }

        ///<descuentoAdicional>0</descuentoAdicional>
        [XmlElement(ElementName = "descuentoAdicional", Order = 3, IsNullable = false)]
        public decimal DescuentoAdicional { get; set; }

        ///<baseImponible>0</baseImponible>
        [XmlElement(ElementName = "baseImponible", Order = 4, IsNullable = false)]
        public decimal BaseImponible { get; set; }

        ///<tarifa>0</tarifa>
        [XmlElement(ElementName = "tarifa", Order = 5, IsNullable = false)]
        public decimal Tarifa { get; set; }

        ///<valor>0</valor>
        [XmlElement(ElementName = "valor", Order = 6, IsNullable = false)]
        public decimal Valor { get; set; }

        public TotalImpuesto()
        {
            Codigo = 0;
            CodigoPorcentaje = 0;
            DescuentoAdicional = 0.0M;
            BaseImponible = 0.0M;
            Tarifa = 0.0M;
            Valor = 0.0M;
        }

        public TotalImpuesto(int codigo, int codigoPorcentaje, decimal descuentoAdicional, decimal baseImponible,
            decimal tarifa, decimal valor)
        {
            Codigo = codigo;
            CodigoPorcentaje = codigoPorcentaje;
            DescuentoAdicional = descuentoAdicional;
            BaseImponible = baseImponible;
            Tarifa = tarifa;
            Valor = valor;
        }
    }
}
