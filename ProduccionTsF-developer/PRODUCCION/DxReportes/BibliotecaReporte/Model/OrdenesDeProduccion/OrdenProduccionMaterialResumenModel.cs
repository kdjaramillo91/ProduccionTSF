using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaReporte.Model.OrdenesDeProduccion
{
    internal class OrdenProduccionMaterialResumenModel
    {
        public string MasterCode { get; set; }
        public string Name { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadFormulada { get; set; }
        public string MU { get; set; }

    }
}
