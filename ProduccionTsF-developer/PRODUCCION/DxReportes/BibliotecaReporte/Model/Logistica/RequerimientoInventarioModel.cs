using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaReporte.Model.Logistica
{
    internal class RequerimientoInventarioModel
    {
        public int IdRequerimiento { get; set; }
        public int NumeroRequerimiento { get; set; }
        public DateTime FechaEmision { get; set; }
        public string NaturalezaMovimiento { get; set; }
        public string Estado { get; set; }
        public string NombreBodega { get; set; }
        public string PersonaRequiere { get; set; }
        public string NumeroGuia { get; set; }
        public DateTime FechaGuia { get; set; }
        public string NumeroRequisicion { get; set; }
        public string CodigoProducto { get; set; }
        public string NombreProducto { get; set; }
        public string Medida { get; set; }
        public string Ubicacion { get; set; }
        public decimal CantidadRequerida { get; set; }
        public decimal CantidadEntregada { get; set; }
    }
}
