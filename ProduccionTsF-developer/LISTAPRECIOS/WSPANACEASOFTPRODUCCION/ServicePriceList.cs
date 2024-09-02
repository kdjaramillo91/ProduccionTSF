using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DAPANACEAPROD;
using System.Data;
using System.Data.Entity;


namespace WSPANACEASOFTPRODUCCION
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class ServicePriceList : IServicePriceList
    {
        DBContextProduction db;
        public string GetPriceListInformation(int idList)
        {
            db = new DBContextProduction();
            string result = "";
            var _rP = db.RegisterInfReplicationSource.FirstOrDefault(fod => fod.idRegisterSource == idList);
            if (_rP != null)
            {
                var lstPurchases = db.PurchaseOrder.Where(w => w.id_priceList == _rP.idRegisterDestination
                                        && !w.Document.DocumentState.code.Equals("05"))
                                        .Select(s => s.Document.sequential)
                                        .ToList();
                if (lstPurchases != null && lstPurchases.Count > 0)
                {
                    result = String.Join(", ", lstPurchases.ToArray());
                }

            }
            return result;
        }
    }
}
