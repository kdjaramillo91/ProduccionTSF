using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
    public class InventoryReasonController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult InventoryReasonResults(InventoryReason InventoryReason
                                                  )
        {
            var model = db.InventoryReason.ToList();

            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_InventoryReasonResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }

        #endregion FILTERS RESULTS

        #region HEADER

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryReasonPartial()
        {
            var model = (TempData["model"] as List<InventoryReason>);

            model = db.InventoryReason.Where(g => g.id_company == ActiveUser.id_company).ToList();

            model = model ?? new List<InventoryReason>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_InventoryReasonPartial", model.OrderByDescending(r => r.id).ToList());
        }

        #endregion HEADER

        #region Edit InventoryReason

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditInventoryReason(int id, int[] orderDetails)
        {
            InventoryReason InventoryReason = db.InventoryReason.Where(o => o.id == id).FirstOrDefault();

            if (InventoryReason == null)
            {
                InventoryReason = new InventoryReason
                {
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now
                };
            }
            TempData["InventoryReason"] = InventoryReason;
            TempData.Keep("InventoryReason");

            return PartialView("_FormEditInventoryReason", InventoryReason);
        }

        #endregion Edit InventoryReason

        #region PAGINATION

        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_InventoryReason)
        {
            TempData.Keep("InventoryReason");
            int index = db.InventoryReason.Where(g => g.id_company == ActiveUser.id_company).OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_InventoryReason);
            var result = new
            {
                maximunPages = db.InventoryReason.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            InventoryReason InventoryReason = db.InventoryReason.Where(g => g.id_company == ActiveUser.id_company).OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (InventoryReason != null)
            {
                TempData["InventoryReason"] = InventoryReason;
                TempData.Keep("InventoryReason");
                return PartialView("_InventoryReasonMainFormPartial", InventoryReason);
            }

            TempData.Keep("InventoryReason");

            return PartialView("_InventoryReasonMainFormPartial", new InventoryReason());
        }

        #endregion PAGINATION

        #region Validacion

        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id_InventoryReason, string codio)
        {
            TempData.Keep("InventoryReason");

            bool rept = false;

            var cantre = db.InventoryReason.Where(x => x.id != id_InventoryReason && x.code == codio && x.id_company == ActiveUser.id_company).ToList().Count();
            if (cantre > 0)
            {
                rept = true;
            }

            var result = new
            {
                rept = rept
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion Validacion

        #region Save and Update

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryReasonPartialAddNew(InventoryReason item)
        {
            InventoryReason conInventoryReason = (TempData["InventoryReason"] as InventoryReason);

            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    // Temporal, no viaja el Motivo costo en Egreso
                    item.MotivoCosto = SetValueMotivoCosto(item);

                    item.sequential = 1;
                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;

                    item.id_company = ActiveUser.id_company;

                    dbemp.InventoryReason.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    TempData["InventoryReason"] = item;
                    TempData.Keep("InventoryReason");
                    ViewData["EditMessage"] = SuccessMessage("Motivos Invetrario: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("InventoryReason");
                    item = (TempData["InventoryReason"] as InventoryReason);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                    trans.Rollback();
                }
            }

            //return PartialView("_InventoryReasonMainFormPartial", item);

            return PartialView("_InventoryReasonPartial", dbemp.InventoryReason.Where(g => g.id_company == ActiveUser.id_company)
                                                                                .OrderByDescending(r => r.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryReasonPartialUpdate(InventoryReason item)
        {
            InventoryReason modelItem = db.InventoryReason.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {
                InventoryReason conInventoryReason = (TempData["InventoryReason"] as InventoryReason);

                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region InventoryReason

                        modelItem.idNatureMove = item.idNatureMove;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.id_documentType = item.id_documentType;
                        modelItem.code = item.code;
                        modelItem.op = item.op;
                        modelItem.isActive = item.isActive;
                        modelItem.isAuthomatic = item.isAuthomatic;
                        modelItem.isForTransfer = item.isForTransfer;
                        modelItem.isSystem = item.isSystem;
                        modelItem.name = item.name;
                        modelItem.requiereSystemLotNubmber = item.requiereSystemLotNubmber;
                        modelItem.requiereUserLotNubmber = item.requiereUserLotNubmber;
                        modelItem.description = item.description;
                        modelItem.id_company = ActiveUser.id_company;
                        modelItem.valorization = item.valorization;
                        modelItem.applyinCost = item.applyinCost;
                        modelItem.typeOfCalculation = item.typeOfCalculation;
                        modelItem.id_inventoryReasonRelated = item.id_inventoryReasonRelated;
                        modelItem.ordenAut = item.ordenAut;
                        modelItem.CategoriaCosto = item.CategoriaCosto;
                        modelItem.MotivoCosto = SetValueMotivoCosto(item);
                        modelItem.IdMotivoEgreso = item.IdMotivoEgreso;

                        #endregion InventoryReason

                        db.InventoryReason.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["InventoryReason"] = modelItem;
                        TempData.Keep("InventoryReason");
                        ViewData["EditMessage"] = SuccessMessage("Motivos Invetrario: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("InventoryReason");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);

                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("InventoryReason");

            return PartialView("_InventoryReasonPartial", db.InventoryReason.Where(g => g.id_company == ActiveUser.id_company)
                                                                                        .OrderByDescending(r => r.id).ToList());
        }

        #endregion Save and Update

        #region InventoryReason Gridview

        [ValidateInput(false)]
        public ActionResult InventoryReasonPartial(int? id)
        {
            if (id != null)
            {
                ViewData["InventoryReasonToCopy"] = db.InventoryReason.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.InventoryReason.ToList();
            return PartialView("_InventoryReasonPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryReasonPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.InventoryReason.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }
                        db.InventoryReason.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Motivos Invetrario : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }
            }

            var model = db.InventoryReason.Where(g => g.id_company == ActiveUser.id_company).OrderByDescending(r => r.id).ToList();
            return PartialView("_InventoryReasonPartial", model.ToList());
        }

        public ActionResult DeleteSelectedInventoryReason(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var InventoryReasons = db.InventoryReason.Where(i => ids.Contains(i.id));
                        foreach (var vInventoryReason in InventoryReasons)
                        {
                            vInventoryReason.id_userUpdate = ActiveUser.id;
                            vInventoryReason.dateUpdate = DateTime.Now;

                            db.InventoryReason.Attach(vInventoryReason);
                            db.Entry(vInventoryReason).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Motivos Invetrario desactivadas exitosamente");
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.InventoryReason.Where(g => g.id_company == ActiveUser.id_company).OrderByDescending(r => r.id).ToList();
            return PartialView("_InventoryReasonPartial", model.ToList());
        }

        #endregion InventoryReason Gridview

        public string SetValueMotivoCosto(InventoryReason item)
        {
            var nombreNaturaleza = db.AdvanceParametersDetail.FirstOrDefault(fod => fod.id == item.idNatureMove).valueName;

            return nombreNaturaleza == "EGRESO" ? "Ninguno" : item.MotivoCosto;
        }
    }
}