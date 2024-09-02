using System.Xml.Serialization;

namespace DXPANACEASOFT.Models.FE.Xmls.NotaCredito
{
    public class TotalImpuesto
    {
        ///<codigo>0</codigo>
        [XmlElement(ElementName = "codigo", Order = 1, IsNullable = false)]
        public int Codigo { get; set; }

        ///<codigoPorcentaje>0</codigoPorcentaje>
        [XmlElement(ElementName = "codigoPorcentaje", Order = 2, IsNullable = false)]
        public int CodigoPorcentaje { get; set; }

        ///<baseImponible>0</baseImponible>
        [XmlElement(ElementName = "baseImponible", Order = 3, IsNullable = false)]
        public decimal BaseImponible { get; set; }

        ///<valor>0</valor>
        [XmlElement(ElementName = "valor", Order = 4, IsNullable = false)]
        public decimal Valor { get; set; }

        public TotalImpuesto()
        {
            Codigo = 0;
            CodigoPorcentaje = 0;
            BaseImponible = 0.0M;
            Valor = 0.0M;
        }

        public TotalImpuesto(int codigo, int codigoPorcentaje, decimal baseImponible, decimal valor)
        {
            Codigo = codigo;
            CodigoPorcentaje = codigoPorcentaje;
            BaseImponible = baseImponible;
            Valor = valor;
        }
    }
}
