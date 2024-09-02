using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace EntidadesAuxiliares.General
{
    /// <summary>
    /// Esta clase contiene la definición 
    /// de datos necesarios para una consulta
    /// a un SP
    /// </summary>
    public class Conexion
    {
        /// <summary>
        /// Servidor
        /// </summary>
        public string SrvName { get; set; }
        /// <summary>
        /// Base de Datos
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// Nombre de Usuario
        /// </summary>
        public string UsrName { get; set; }
        /// <summary>
        /// Contraseña
        /// </summary>
        public string PswName { get; set; }
    }
}
