using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Services
{
    public class ServicePurchaseRemission
    {

        #region OPTMIZCION RECEPCION
        public static void UpdateQuantityRecivedOP( DBContext db, 
                                                    int? id_purchaseOrderDetail, 
                                                    int? id_remissionGuideDetail, 
                                                    decimal quantity,
                                                    RemissionGuideDetail[] remissionGuideDetails,
                                                    PurchaseOrderDetail[] purchaseOrderDetails)
        {
            string result = "";
            //Lot lot = new Lot();
            try
            {
                if (id_remissionGuideDetail != null)
                {
                    var remissionGuideDetail = remissionGuideDetails.FirstOrDefault(fod => fod.id == id_remissionGuideDetail);
                    remissionGuideDetail.quantityReceived += quantity;
                    db.RemissionGuideDetail.Attach(remissionGuideDetail);
                    db.Entry(remissionGuideDetail).State = EntityState.Modified;
                };

                if (id_purchaseOrderDetail != null)
                {
                    var purchaseOrderDetail = purchaseOrderDetails.FirstOrDefault(fod => fod.id == id_purchaseOrderDetail);
                    purchaseOrderDetail.quantityReceived += quantity;
                    db.PurchaseOrderDetail.Attach(purchaseOrderDetail);
                    db.Entry(purchaseOrderDetail).State = EntityState.Modified;
                }
            }
            catch (Exception e)
            {
                result = e.Message;
                throw;
            }

            //return lot;
        }


        #endregion
        public static void UpdateQuantityRecived(DBContext db, int? id_purchaseOrderDetail, int? id_remissionGuideDetail, decimal quantity)
        {
            string result = "";
            //Lot lot = new Lot();
            try
            {
                if (id_remissionGuideDetail != null)
                {
                    var remissionGuideDetail = db.RemissionGuideDetail.FirstOrDefault(fod=> fod.id == id_remissionGuideDetail);
                    remissionGuideDetail.quantityReceived += quantity;
                    db.RemissionGuideDetail.Attach(remissionGuideDetail);
                    db.Entry(remissionGuideDetail).State = EntityState.Modified;
                };

                if (id_purchaseOrderDetail != null)
                {
                    var purchaseOrderDetail = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == id_purchaseOrderDetail);
                    purchaseOrderDetail.quantityReceived += quantity;
                    db.PurchaseOrderDetail.Attach(purchaseOrderDetail);
                    db.Entry(purchaseOrderDetail).State = EntityState.Modified;
                }
            }
            catch (Exception e)
            {
                result = e.Message;
                throw;
            }

            //return lot;
        }

        public static void UpdateQuantityRecivedPurchaseOrderDetailPurchaseRequest(DBContext db, int id_purchaseOrderDetail, int? id_purchaseRequestDetail, decimal quantity)
        {
            string result = "";
            //Lot lot = new Lot();
            try
            {
                if (id_purchaseRequestDetail != null)
                {
                    var purchaseRequestDetail = db.PurchaseRequestDetail.FirstOrDefault(fod => fod.id == id_purchaseRequestDetail);
                    purchaseRequestDetail.quantityOutstandingPurchase -= quantity;
                    db.PurchaseRequestDetail.Attach(purchaseRequestDetail);
                    db.Entry(purchaseRequestDetail).State = EntityState.Modified;
                }
            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }

            //return lot;
        }

        public static void UpdateQuantityDispatchedPurchaseOrderDetailRemissionGuide(DBContext db, int id_purchaseOrderDetail, int id_remissionGuideDetail, decimal quantity)
        {
            string result = "";

            try
            {
                var purchaseOrderDetail = db.PurchaseOrderDetail.FirstOrDefault(fod => fod.id == id_purchaseOrderDetail);
                purchaseOrderDetail.quantityDispatched += quantity;
                db.PurchaseOrderDetail.Attach(purchaseOrderDetail);
                db.Entry(purchaseOrderDetail).State = EntityState.Modified;
              
            }
            catch (Exception e)
            {
                result = e.Message;
                throw e;
            }
        }

        #region Auxiliar

        protected static int GetDocumentSequential(int id_documentType, DBContext db, Company activeCompany)
        {
            DocumentType documentType = db.DocumentType.FirstOrDefault(d => d.id == id_documentType && d.id_company == activeCompany.id);
            return documentType?.currentNumber ?? 0;
        }

        protected static string GetDocumentNumber(int id_documentType, DBContext db, Company activeCompany, EmissionPoint activeEmissionPoint)
        {
            string number = GetDocumentSequential(id_documentType, db, activeCompany).ToString().PadLeft(9, '0');
            string documentNumber = string.Empty;
            documentNumber = $"{activeEmissionPoint.BranchOffice.code.ToString().PadLeft(3, '0')}-{activeEmissionPoint.code.ToString().PadLeft(3, '0')}-{number}";
            return documentNumber;
        }

        #endregion

    }
}