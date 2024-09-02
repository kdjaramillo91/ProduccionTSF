using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.PriceListDet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class PriceListDetailController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }
        
        #region PriceListDetail

        public ActionResult PopupControlPriceListDetailPartial()
        {
            try
            {
                List<PriceListDetItemSize> lstIPo = (TempData["PriceListDetail"] as List<PriceListDetItemSize>);

                lstIPo = lstIPo ?? new List<PriceListDetItemSize>();

                TempData["PriceListDetail"] = lstIPo;
                TempData.Keep();

                return PartialView("_PopupControlPriceListItemSizeDetail", lstIPo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //[ActionName("PopupComentarioPospuesto")]
        [HttpPost, ValidateInput(false)]
        public ActionResult PopupControlPriceListDetail(int id_priceList, int? idProvider, int? idOc)
        {
            //List<PriceListItemSizeDetail> plisDet = null;
            //List<PriceListItemSizeDetail> plisDetBase = null;
            List<PriceListDetItemSize> plDetPrice = new List<PriceListDetItemSize>();

            //string sProccessTypeCode = "";

            try
            {
                 plDetPrice = db.PriceListItemSizeDetail
                                                .Where(w => w.Id_PriceList == id_priceList && w.ClassShrimp.code == "D0")
                                                .Select(s => new PriceListDetItemSize
                                                {
                                                    Id_Itemsize = s.Id_Itemsize,
                                                    sItemSize = s.ItemSize != null ? s.ItemSize.name : "",
                                                    Order = s.ItemSize.orderSize,
                                                    //PriceA = s.price - s.commission,
                                                    //PriceB = ((s.price - valB - s.commission) >= 0) ? s.price - valB - s.commission : 0
                                                    Price = s.price - s.commission,
                                                    id_Class = s.id_class,
                                                    codeClass = s.Class.code,
                                                    nameClass = s.Class.description,
                                                    id_ProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.Id_Itemsize)
                                                                    ? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.Id_Itemsize)
                                                                    .id_ProcessType.Value : 0,
                                                    sProcessType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == s.Id_Itemsize)
                                                                    ? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == s.Id_Itemsize)
                                                                    .ProcessType.name : "",
                                                    Libras = 0,
                                                    Total = 0,
                                                }).ToList();

                if(idOc.HasValue && idOc != 0)
                {
                    decimal LibrasCola = 0;
                    decimal LibrasEntero = 0;
                    decimal Libras = 0;
                    var setting = db.Setting.Where(X => X.code == "PRENTC").FirstOrDefault()?.value;
                    var idGramage = db.PurchaseOrderDetail.FirstOrDefault(a => a.id_purchaseOrder == idOc)?.id_Grammage ?? 0;
                    var cantidadLibras = db.PurchaseOrderDetail.Where(a => a.id_purchaseOrder == idOc).Sum(e => e.quantityApproved);
                    ItemProcessType itemProcessType = db.PurchaseOrderDetail.FirstOrDefault(a => a.id_purchaseOrder == idOc)?.Item.ItemProcessType.FirstOrDefault();
                    ProcessType processtype = db.ProcessType.Where(x => x.id == itemProcessType.Id_ProcessType).FirstOrDefault();


                    for (int i = 0; i < plDetPrice.Count; i++)
                    {
                        var pl = plDetPrice[i];

                        if (setting != null && processtype.code == "ENT")
                        {
                            LibrasEntero = cantidadLibras * (decimal.Parse(setting) / 100);
                            LibrasCola = (cantidadLibras - LibrasEntero) * (decimal.Parse(setting) / 100);
                        }
                        if (setting != null && processtype.code == "COL")
                        {
                            LibrasEntero = cantidadLibras;
                        }
                        if (setting != null && processtype.code == "ENC")
                        {
                            LibrasCola = cantidadLibras * (decimal.Parse("66") / 100);
                        }
                        
                        var plDetTallas = db.FanSeriesGrammage
                            .FirstOrDefault(w => w.id_grammage == idGramage
                                && (w.percentage.HasValue && w.percentage > 0)
                                && w.id_itemsize == pl.Id_Itemsize && w.id_class == pl.id_Class);

                        if(plDetTallas != null)
                        {
                            if (processtype.code == "ENT" && plDetTallas.ProcessType1.code == "ENT")
                            {
                                Libras = LibrasEntero;
                            }
                            if (processtype.code == "ENT" && plDetTallas.ProcessType1.code == "COL")
                            {
                                Libras = LibrasCola;
                            }
                            if (processtype.code == "COL")
                            {
                                Libras = cantidadLibras;
                            }
                            if (processtype.code == "ENC")
                            {
                                Libras = LibrasCola;
                            }
                        }
                        
                        var cantidadDetalle = plDetTallas != null
                            ? plDetTallas.percentage.Value * Libras : 0m;

                        plDetPrice[i].Libras = cantidadDetalle;
                        plDetPrice[i].Total = pl.Price * cantidadDetalle;
                    }                  

                    

                }

                //int idClassShrimpB = db.ClassShrimp
                //                        .FirstOrDefault(fod => fod.code.Trim().Equals("B"))?
                //                        .id ?? 0;

                //decimal valB = db.PriceListClassShrimp
                //                  .FirstOrDefault(fod => fod.id_priceList == id_priceList
                //                  && fod.id_classShrimp == idClassShrimpB)?.value ?? 0;

                ////Busco proveedor en algún grupo
                //int idGroupPerson = db.GroupPersonByRolDetail
                //                        .FirstOrDefault(fod => fod.id_person == idProvider)?
                //                        .id_groupPersonByRol ?? 0;

                //List<ItemSizePriceClass> lstPreciosClases = null;
                //List<Class> lstClass = db.Class.ToList();

                //int idBrok = lstClass.FirstOrDefault(fod => fod.code.Trim().Equals("BROK")).id;
                //int idVetl = lstClass.FirstOrDefault(fod => fod.code.Trim().Equals("VETL")).id;

                //if (idGroupPerson > 0)
                //{
                //    lstPreciosClases = db.ItemSizePriceClass
                //                            .Where(w => w.idGroupPersonByRol == idGroupPerson)
                //                            .ToList();
                //}

                //if (lstPreciosClases == null || lstPreciosClases.Count == 0)
                //{
                //    lstPreciosClases = db.ItemSizePriceClass
                //                            .Where(w => w.idGroupPersonByRol == null)
                //                            .ToList();
                //}

                //var pl = db.PriceList
                //                .FirstOrDefault(fod => fod.id == id_priceList);

                //plisDet = db.PriceListItemSizeDetail
                //            .Where(fod => fod.Id_PriceList == id_priceList)
                //            .ToList();

                //var lstIPo = db.ItemSizeProcessPLOrder.OrderBy(o => o.Order ).ToList();

                //foreach (var iLs in plisDet)
                //{
                //    sProccessTypeCode = lstIPo.FirstOrDefault(fod => fod.id_ItemSize == iLs.Id_Itemsize)?.ProcessType?.code ?? "";

                //    PriceListDetItemSize plTmp = new PriceListDetItemSize();
                //    plTmp.id = iLs.Id_Itemsize;
                //    plTmp.id_ProcessType = lstIPo.FirstOrDefault(fod => fod.id_ItemSize == iLs.Id_Itemsize)?.id_ProcessType ?? 0;
                //    plTmp.sProcessType = lstIPo.FirstOrDefault(fod => fod.id_ItemSize == iLs.Id_Itemsize)?.ProcessType?.name ?? "";
                //    plTmp.Id_Itemsize = iLs.Id_Itemsize;
                //    plTmp.sItemSize = iLs.ItemSize?.name ?? "";
                //    plTmp.Order = lstIPo.FirstOrDefault(fod => fod.id_ItemSize == iLs.Id_Itemsize)?.Order ?? 0;
                //    plTmp.PriceA = (plisDet
                //                    .FirstOrDefault(fod => fod.Id_Itemsize == iLs.Id_Itemsize)?
                //                    .price ?? 0) - (plisDet.FirstOrDefault(fod => fod.Id_Itemsize == iLs.Id_Itemsize)?.commission ?? 0);
                //    plTmp.PriceB = ((plisDet
                //                    .FirstOrDefault(fod => fod.Id_Itemsize == iLs.Id_Itemsize)?
                //                    .price ?? 0) - valB) - (plisDet.FirstOrDefault(fod => fod.Id_Itemsize == iLs.Id_Itemsize)?.commission ?? 0);//aqui va precioB

                //    if (sProccessTypeCode == "ENT") { plTmp.PriceB = 0; }

                //    plDetPrice.Add(plTmp);
                //}

                ////Actualizo Valores de Broken y Venta Local
                //foreach (var det in plDetPrice)
                //{
                //    var PrecioTalla = lstPreciosClases
                //                        .FirstOrDefault(fod => fod.idItemSize == det.Id_Itemsize);

                //    if (PrecioTalla != null)
                //    {

                //        //Actualizo Precio A y Precio B
                //        det.PriceA = lstPreciosClases
                //                        .FirstOrDefault(fod => fod.idItemSize == det.Id_Itemsize && fod.idClass == idBrok)?.price ?? 0;

                //        det.PriceB = lstPreciosClases
                //                        .FirstOrDefault(fod => fod.idItemSize == det.Id_Itemsize && fod.idClass == idVetl)?.price ?? 0;

                //    }
                //}

                TempData["PriceListDetail"] = plDetPrice;

                TempData.Keep();
                return PartialView("_FormEditPopupControlPriceListDetailPartial", plDetPrice.OrderBy(o => o.Order).ToList());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [ValidateInput(false)]
        public ActionResult FormEditDetailDetailPriceListItemSizePartial()
        {
            List<PriceListDetItemSize> lstIPo = (TempData["PriceListDetail"] as List<PriceListDetItemSize>);

            lstIPo = lstIPo ?? new List<PriceListDetItemSize>();

            TempData["PriceListDetail"] = lstIPo;
            TempData.Keep();

            return PartialView("_FormEditDetailPriceListPartial", lstIPo);
        }


        #endregion

    }
}