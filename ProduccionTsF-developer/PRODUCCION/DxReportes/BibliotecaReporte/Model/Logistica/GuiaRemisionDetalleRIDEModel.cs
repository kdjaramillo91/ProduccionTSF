using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaReporte.Model.Logistica
{
    internal class GuiaRemisionDetalleRIDEModel
    {
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public string Codigo { get; set; }
        public string Auxiliar { get; set; }
    }
}
