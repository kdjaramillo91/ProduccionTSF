using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DXPANACEASOFT.Models
{
    public partial class ImportResult
    {
        public DocumentoImportado[] Importados { get; set; }
        public DocumentoFallido[] Fallidos { get; set; }

        public class DocumentoImportado 
        {
            public string Filas { get; set; }
            public int NumDocumento { get; set; }
            public DateTime FechaProceso { get; set; }
            public string Descripcion { get; set; }
        }

        public class DocumentoFallido
        {
            public string Filas { get; set; }
            public string Descripcion { get; set; }
        }
    }
}