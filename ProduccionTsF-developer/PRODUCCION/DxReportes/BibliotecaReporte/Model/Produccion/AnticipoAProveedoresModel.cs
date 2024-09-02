using System;
namespace BibliotecaReporte.Model.Produccion
{
   internal class AnticipoAProveedoresModel
    {
        public int IdAnticipo { get; set; }
        public string Proveedor { get; set; }
        public string Ruc { get; set; }
        public string Telefono { get; set; }
        public string Lote { get; set; }
        public string N { get; set; }
        public DateTime Fecha { get; set; }
        public string Comprador { get; set; }
        public string Sitio { get; set; }
        public int Libras_recibidas { get; set; }
        public decimal Libras_remitidas { get; set; }
        public decimal Libras_despachadas { get; set; }
        public decimal Gramage_promedio { get; set; }
        public DateTime Fecha_recepcion { get; set; }
        public DateTime Fecha_procesamiento { get; set; }
        public string inp_camar { get; set; }
        public string Acu_camar { get; set; }
        public string Tra_camar { get; set; }
        public string Inp_ampar { get; set; }
        public string Acu_ampar { get; set; }
        public string Tra_ampar { get; set; }
        public byte[] Logo { get; set; }
        public string Lista { get; set; }
        public decimal Porcentajeanticipo { get; set; }
        public decimal Valor_anticipo { get; set; }
        public string Proceso { get; set; }
        public string Clase { get; set; }
        public string Talla { get; set; }
        public decimal Libras { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }
        public string Observacion { get; set; }
        public string NombreUsuario { get; set; }
        public string Piscina { get; set; }
        public decimal PrecioPromedio { get; set; }
        public decimal ValorAproximado { get; set; }
        public string ProcesoLote { get; set; }
        public string ProcesoPlanta { get; set; }

    }
}
