using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace DXPANACEASOFT.Models.GenericProcess
{
    internal static class DrainingTestOperation
    {

        internal static Boolean ValidateCanAnnul(this DrainingTest o, DBContext db, out string errorMessage)
        {

            Boolean returnValidateCanAnnul = false;
            string validCodeStateQualityControl = "01";
            string validCodeStateProductionLotReception = "01";

            try
            {

                int? idValidCodeStateQualityControl = db.DocumentState
                                                                .FirstOrDefault(r => r.code == validCodeStateQualityControl)?.id;
                int? idValidCodeStateProductionLotReception = db.ProductionLotState
                                                                        .FirstOrDefault(r => r.code == validCodeStateProductionLotReception)?.id;



                if (idValidCodeStateQualityControl == null || idValidCodeStateProductionLotReception == null)
				{
					errorMessage = $"";
					return returnValidateCanAnnul;
				}
					


                int? idReceptionDetail = db.ReceptionDetailDrainingTest
                                                        .FirstOrDefault(r => r.idDrainingTest == o.id && r.isActive)?
                                                                    .ResultProdLotReceptionDetail?
                                                                    .idProductionLotReceptionDetail;

              
                if (idReceptionDetail != null )
                {

                    // Validar estado de Recepción
                    
                    int? idProductionLot = db.ProductionLotDetail
                                                .FirstOrDefault(r => r.id == idReceptionDetail)?.id_productionLot;


                    if(idProductionLot  != null)
                    {
                        int? idStatusProductionLot = db.ProductionLot
                                                            .FirstOrDefault(r => r.id == idProductionLot)?.id_ProductionLotState;

                        if (idStatusProductionLot != null && idStatusProductionLot != idValidCodeStateProductionLotReception)
						{
							errorMessage = $"Ya que el lote se encuentra en estado diferente a PENDIENTE DE RECEPCIÓN.";
							return returnValidateCanAnnul;
						}
                            

						var existAdvancedProvider = db.AdvanceProvider
							.FirstOrDefault(e => e.id_Lot == idProductionLot && e.Document.DocumentState.code != "05");

						if (existAdvancedProvider != null)
						{
							errorMessage = $"Ya que existe un anticipo de proveedor relacionado con el lote correspondiente." + 
								$"Número: {existAdvancedProvider.Document.number}. Estado: {existAdvancedProvider.Document.DocumentState.name}";
							return returnValidateCanAnnul;
						}
							

					}


     //               // Validar estado de QualityControl
     //               int? idQualityControl = db.ProductionLotDetailQualityControl
     //                                               .FirstOrDefault(r => r.id_productionLotDetail == idReceptionDetail)?.id_qualityControl;

     //               if(idQualityControl != null)
     //               {

     //                   int? idStatusQualityControl = db.Document
     //                                                       .FirstOrDefault(r => r.id == idQualityControl)?.id_documentState;

     //                   if (idStatusQualityControl != null && idStatusQualityControl != idValidCodeStateQualityControl)
					//	{
					//		errorMessage = $"Ya que el análisis de calidad correspondiente al Lote esta en estado diferente a PENDIENTE.";
					//		return returnValidateCanAnnul;
					//	}
					//}
                }

				errorMessage = ""; 
				returnValidateCanAnnul = true;
            }
            catch
            {
				errorMessage = ""; 
				returnValidateCanAnnul = false;

            }

			return returnValidateCanAnnul;
        }

        internal static DrainingTest ColateralAnnul( this DrainingTest o, DBContext db)
        {

            try
            {

                ReceptionDetailDrainingTest oReceptionDetailDrainingTest = db.ReceptionDetailDrainingTest
                                                                        .FirstOrDefault(r => r.idDrainingTest == o.id && r.isActive);

                if (oReceptionDetailDrainingTest == null) throw new Exception("No existe Recepción para esta prueba de escurrido.");


                ResultProdLotReceptionDetail oReceptionDetail = oReceptionDetailDrainingTest.ResultProdLotReceptionDetail;

                if (oReceptionDetail == null) throw new Exception("No existe Recepción para esta prueba de escurrido.");

                oReceptionDetailDrainingTest.isActive = false;

                db.ReceptionDetailDrainingTest.Attach(oReceptionDetailDrainingTest);
                db.Entry(oReceptionDetailDrainingTest).State = EntityState.Modified;


                int idReceptionDetail = oReceptionDetail.idProductionLotReceptionDetail;

                ProductionLotDetail oProductionLotDetail = db.ProductionLotDetail
                                                                    .FirstOrDefault(r => r.id == idReceptionDetail);

                if(oProductionLotDetail !=  null)
                {
                    oProductionLotDetail.quantitydrained = 0;

                    db.ProductionLotDetail.Attach(oProductionLotDetail);
                    db.Entry(oProductionLotDetail).State = EntityState.Modified;
                }

                ProductionLotDetailQualityControl oProductionLotDetailQualityControl =
                                                                db.ProductionLotDetailQualityControl
                                                                        .FirstOrDefault(r => r.id_productionLotDetail == oProductionLotDetail.id);

                if (oProductionLotDetailQualityControl != null)
                {

                    QualityControl oQualityControl = db.QualityControl
                                                            .FirstOrDefault(r => r.id == oProductionLotDetailQualityControl.id_qualityControl);

                    if(oQualityControl != null)
                    {
                        oQualityControl.QuantityPoundsReceived = 0;

                        db.QualityControl.Attach(oQualityControl);
                        db.Entry(oQualityControl).State = EntityState.Modified;
                    }
                }


            }
            catch (Exception e)
            {
                throw e;

            }


            return o;
        }


    }
}