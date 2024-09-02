using DXPANACEASOFT.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class CatalogueController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region Catalogue GridView

        [ValidateInput(false)]
        public ActionResult CataloguesPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.tbsysCatalogueDetail.FirstOrDefault(b => b.id == keyToCopy);
            }
           
            var model = db.tbsysCatalogueDetail;
            return PartialView("_CataloguesPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CataloguesPartialAddNew(tbsysCatalogueDetail item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.tbsysCatalogueDetail.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Catálogo : " + item.name + " guardado exitosamente");
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

            var model = db.tbsysCatalogueDetail;
            return PartialView("_CataloguesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CataloguesPartialUpdate(tbsysCatalogueDetail item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.tbsysCatalogueDetail.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.id_Catalogue = item.id_Catalogue;
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.fldvarchar1 = item.fldvarchar1;
                            modelItem.flddatetime1 = item.flddatetime1;
                            modelItem.flddatetime2 = item.flddatetime2;
                            modelItem.fldFullText = item.fldFullText;
                            modelItem.isActive = item.isActive;

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.tbsysCatalogueDetail.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            ViewData["EditMessage"] =
                                SuccessMessage("Catálogo: " + item.name + " guardado exitosamente");
                        }
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

            var model = db.tbsysCatalogueDetail;
            return PartialView("_CataloguesPartial", model.ToList());

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ItemTrademarkModelsPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.tbsysCatalogueDetail.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.tbsysCatalogueDetail.Attach(item);
                            db.Entry(item).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] =
                                SuccessMessage("Catálogo: " + (item?.name ?? "") + " desactivado exitosamente");
                        }
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.tbsysCatalogueDetail;
            return PartialView("_CataloguesPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedItemTrademarkModels(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.tbsysCatalogueDetail.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.tbsysCatalogueDetail.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Catálogos desactivados exitosamente");
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

            var model = db.tbsysCatalogueDetail;
            return PartialView("_CataloguesPartial", model.ToList());
        }

        #endregion

        public JsonResult ValidateCodeCatalogue(int id_catalogue, string code)
        {
            tbsysCatalogueDetail catalogueDetail = db.tbsysCatalogueDetail.FirstOrDefault(b =>  b.code == code
                                                                            && b.id != id_catalogue);

            if (catalogueDetail == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Modelo " }, JsonRequestBehavior.AllowGet);
        }
    }
}

