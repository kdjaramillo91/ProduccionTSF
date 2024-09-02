using DocumentFormat.OpenXml.Spreadsheet;
using System;

namespace DXPANACEASOFT.Models
{
    public class JobScheduler
    { 
        public int id { get; set; }
        public DateTime dateInit { get; set; }
        public DateTime dateEnd { get; set; }
        public Nullable<System.TimeSpan> timeHourExecute { get; set; }
        public int id_documentState { get; set; }
        public string serverHost { get; set; }
        public string databaseHost { get; set; }
        public string userdb { get; set; }
        public string passwordb { get; set; }
        public string storeProcedure { get; set; }
        public int id_statusProcess { get; set; }
        public string dataResult { get; set; }
    }
}