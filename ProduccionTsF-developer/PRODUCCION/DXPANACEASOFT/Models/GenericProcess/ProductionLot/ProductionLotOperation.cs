using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models.ModelExtension;


namespace DXPANACEASOFT.Models.GenericProcess
{
    public  static class ProductionLotOperationExtension
    {


        public static ProductionLotDetail ValidateDrainingTest(this ProductionLotDetail thisObj, DBContext db)
        {

            try
            {

                string settLE = db.Setting.FirstOrDefault(r => r.code == "ULERMP")?.value ?? "N";

                if (settLE == "N") return thisObj;

                int idDrainingTest = thisObj.ResultProdLotReceptionDetail?.idDrainingTest ?? 0;
                string codeStateDrainingTest = db.Document.FirstOrDefault(r => r.id == idDrainingTest)?.DocumentState?.code ?? "0";

                if (idDrainingTest != 0 && !codeStateDrainingTest.Equals("05")) return thisObj;
                
                List<EntityParameters> parameterMaxLb = (List<EntityParameters>)db.AdvanceParameters.FindParameters("PRESC");

                if (parameterMaxLb == null) throw new Exception("No existe configuración de Parámetros Prueba de Escurrido");

                EntityParameters entParameterMaxLb = parameterMaxLb.FirstOrDefault(r => r.code == "ESENT");
                if(entParameterMaxLb == null) throw new Exception("No existe valor parámetro valor Mínimo prueba de escurrido");

                string codeMetricUnit = entParameterMaxLb.valueString;
                int valueParameterMaxLb = (int)entParameterMaxLb.valueInteger;

                if(thisObj.quantityRecived < valueParameterMaxLb)
                {
                    thisObj.quantitydrained = thisObj.quantityRecived;
                }
                else
                {
                    thisObj.quantitydrained = 0;
                }


            }
            catch(Exception e)
            {
                throw e;
            }
            

            return thisObj;
        }
        
        public static void  ValidateNA()
        {


        }
        public static ProductionLotDetail ExistingDrainingTestForProductionLotReception(this ProductionLotDetail o )
        {

            DBContext db = new DBContext();
            try
            {
               
                ProductionLotDetail _productionLotDetail = db.ProductionLotDetail.FirstOrDefault(r => r.id == o.id);
                if (_productionLotDetail == null)
                {

                    o.quantityRecivedEditing = false;

                    return o;
                }
                    


                int idDrainingTest = _productionLotDetail
                                                    .ResultProdLotReceptionDetail?
                                                    .ReceptionDetailDrainingTest?
                                                    .FirstOrDefault(r => r.isActive)?.idDrainingTest ?? 0;

                string codeStateDrainingTest = db.Document.FirstOrDefault(r => r.id == idDrainingTest)?.DocumentState?.code ?? "0";

                if (idDrainingTest != 0 && codeStateDrainingTest.Equals("03"))
                {
                    o.quantityRecivedEditing = true;
                    return o;
                }

                o.quantityRecivedEditing = false;

            }
            catch //(Exception e)
            {
                o.quantityRecivedEditing = true;
            }


            return o;
        }


    }
}