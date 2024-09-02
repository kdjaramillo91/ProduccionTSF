using DXPANACEASOFT.Dapper;
using System.Linq;

namespace DXPANACEASOFT.Models
{
    public partial class TransCtlDocumentTypeConfig
    {
        public int Id { get; set; }  //DocumentType Id
        public string DtName { get; set; }
        public int Peso { get; set; }  // -- Manejo de peso/costo de documento/transaccion en el conjunto de las demas transacciones
        public int EstTimePerform { get; set; }  // -- Estadisticas de tiempo ejecucion tipo de documento
        public string Controller { get; set; }
        public string Method { get; set; }
        public string CodeStateOK { get; set; }
        public string CodeStateError { get; set; }



        private static string SELECT_ONE_BY_ID =        " SELECT  id, DtName, Peso, " +
                                                        " EstTimePerform, Controller, Method, CodeStateOK, CodeStateError " +
                                                        " FROM TransCtlDocumentTypeConfig " +
                                                        " WHERE Id = @id";

        public static TransCtlDocumentTypeConfig GetOneById(int id)
        {
            return DapperConnection.Execute<TransCtlDocumentTypeConfig>(SELECT_ONE_BY_ID, new
            {
                id = id
            })?.FirstOrDefault();
        }
    }
}