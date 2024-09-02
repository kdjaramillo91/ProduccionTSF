using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.NotaCredito
{
    public class Impuesto
    {
        ///<codigo>0</codigo>
        [XmlElement(ElementName = "codigo", Order = 1, IsNullable = false)]
        public int Codigo { get; set; }

        ///<codigoPorcentaje>0</codigoPorcentaje>
        [XmlElement(ElementName = "codigoPorcentaje", Order = 2, IsNullable = false)]
        public int CodigoPorPorcentaje { get; set; }

        ///<tarifa>0</tarifa>
        [XmlElement(ElementName = "tarifa", Order = 3, IsNullable = false)]
        public decimal Tarifa { get; set; }

        ///<baseImponible>0</baseImponible>
        [XmlElement(ElementName = "baseImponible", Order = 4, IsNullable = false)]
        public decimal BaseImponible { get; set; }

        ///<valor>0</valor>
        [XmlElement(ElementName = "valor", Order = 5, IsNullable = false)]
        public decimal Valor { get; set; }

        public Impuesto()
        {
            Codigo = 0;
            CodigoPorPorcentaje = 0;
            Tarifa = 0.0M;
            BaseImponible = 0.0M;
            Valor = 0.0M;
        }

        public Impuesto(int codigo, int codigoPorcentaje, decimal tarifa, decimal baseImponible, decimal valor)
        {
            Codigo = codigo;
            CodigoPorPorcentaje = codigoPorcentaje;
            Tarifa = tarifa;
            BaseImponible = baseImponible;
            Valor = valor;
        }
    }
}
