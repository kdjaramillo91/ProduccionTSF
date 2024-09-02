using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace EntidadesAuxiliares.SQL
{
    /// <summary>
    /// Esta clase contiene la definición 
    /// de datos necesarios para una consulta
    /// a un SP
    /// </summary>
    public class ParamSQL
    {
        /// <summary>
        /// Nombre del Parámetro
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Valor del Parámetro
        /// </summary>
        public Object Valor { get; set; }
        /// <summary>
        /// Tipo de Dato
        /// </summary>
        public DbType TipoDato { get; set; }
    }
}
