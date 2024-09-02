using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace EntidadesAuxiliares.CrystalReport
{
    /// <summary>
    /// Esta clase contiene la definición 
    /// de datos necesarios para una consulta
    /// a un SP
    /// </summary>
    public class ParamCR
    {
        /// <summary>
        /// Nombre del Parámetro
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Valor del Parámetro
        /// </summary>
        public Object Valor { get; set; }
    }
}
