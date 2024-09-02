using System;
namespace BibliotecaReporte.Model.Configuracion
{
    internal class IntegrationProcessModel
    {

        public string Code_lote { get; set; }
        public string Tipo_Integracion { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha_Contabilizacion { get; set; }
        public string Observacion { get; set; }

        public string Documento { get; set; }
        public DateTime Fecha_Emision { get; set; }
        public string GlossData1 { get; set; }

        public decimal Valor_Documento { get; set; }
        public decimal Total_Lote_Integracion { get; set; }
        public int Numero_Documentos { get; set; }
        public byte[] Logo { get; set; }
        public byte[] Logo2 { get; set; }

    }
}
