using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;
namespace DXPANACEASOFT.Controllers
{
    public class TermsNegotiationController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult TermsNegotiationResults(TermsNegotiation TermsNegotiation )
        {

            var model = db.TermsNegotiation.ToList();

            #region  FILTERS



            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_TermsNegotiationResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult TermsNegotiationPartial()
        {
            var model = (TempData["model"] as List<TermsNegotiation>);

            model = db.TermsNegotiation.ToList();

            model = model ?? new List<TermsNegotiation>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_TermsNegotiationPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit TermsNegotiation
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditTermsNegotiation(int id, int[] orderDetails)
        {
            TermsNegotiation TermsNegotiation = db.TermsNegotiation.Where(o => o.id == id).FirstOrDefault();

            if (TermsNegotiation == null)
            {

                TermsNegotiation = new TermsNegotiation
                {

                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now

                };
            }
            TempData["TermsNegotiation"] = TermsNegotiation;
            TempData.Keep("TermsNegotiation");

            return PartialView("_FormEditTermsNegotiation", TermsNegotiation);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_TermsNegotiation)
        {
            TempData.Keep("TermsNegotiation");
            int index = db.TermsNegotiation.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_TermsNegotiation);
            var result = new
            {
                maximunPages = db.TermsNegotiation.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            TermsNegotiation TermsNegotiation = db.TermsNegotiation.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (TermsNegotiation != null)
            {
                TempData["TermsNegotiation"] = TermsNegotiation;
                TempData.Keep("TermsNegotiation");
                return PartialView("_TermsNegotiationMainFormPartial", TermsNegotiation);
            }

            TempData.Keep("TermsNegotiation");

            return PartialView("_TermsNegotiationMainFormPartial", new TermsNegotiation());
        }
        #endregion
        #region Validacion
        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id_TermsNegotiation, string codio)
        {
            TempData.Keep("TermsNegotiation");


            bool rept = false;

            var cantre = db.TermsNegotiation.Where(x => x.id != id_TermsNegotiation && x.code == codio ).ToList().Count();
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
        #endregion
        #region Save and Update
        [HttpPost, ValidateInput(false)]
        public ActionResult TermsNegotiationPartialAddNew(TermsNegotiation item)
        {
            TermsNegotiation conTermsNegotiation = (TempData["TermsNegotiation"] as TermsNegotiation);


            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region TermsNegotiation

                    #endregion


                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;

                  

                    dbemp.TermsNegotiation.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    TempData["TermsNegotiation"] = item;
                    TempData.Keep("TermsNegotiation");
                    ViewData["EditMessage"] = SuccessMessage("Terminos de Negociacion: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("TermsNegotiation");
                    item = (TempData["TermsNegotiation"] as TermsNegotiation);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                    trans.Rollback();
                }
            }



            //return PartialView("_TermsNegotiationMainFormPartial", item);

            return PartialView("_TermsNegotiationPartial", dbemp.TermsNegotiation.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult TermsNegotiationPartialUpdate(TermsNegotiation item)
        {
            TermsNegotiation modelItem = db.TermsNegotiation.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                TermsNegotiation conTermsNegotiation = (TempData["TermsNegotiation"] as TermsNegotiation);





                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region TermsNegotiation
                     
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.code = item.code;
                        modelItem.isActive = item.isActive;
                        modelItem.name = item.name;
                        #endregion
                        db.TermsNegotiation.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["TermsNegotiation"] = modelItem;
                        TempData.Keep("TermsNegotiation");
                        ViewData["EditMessage"] = SuccessMessage("Terminos de Negociacion: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("TermsNegotiation");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("TermsNegotiation");


            return PartialView("_TermsNegotiationPartial", db.TermsNegotiation.ToList());
        }
        #endregion
        #region TermsNegotiation Gridview

        [ValidateInput(false)]
        public ActionResult TermsNegotiationPartial(int? id)
        {
            if (id != null)
            {
                ViewData["TermsNegotiationToCopy"] = db.TermsNegotiation.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.TermsNegotiation.ToList();
            return PartialView("_TermsNegotiationPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TermsNegotiationPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.TermsNegotiation.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }
                        db.TermsNegotiation.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Terminos de Negociacion : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.TermsNegotiation.ToList();
            return PartialView("_TermsNegotiationPartial", model.ToList());
        }

        public ActionResult DeleteSelectedTermsNegotiation(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var TermsNegotiations = db.TermsNegotiation.Where(i => ids.Contains(i.id));
                        foreach (var vTermsNegotiation in TermsNegotiations)
                        {


                            vTermsNegotiation.id_userUpdate = ActiveUser.id;
                            vTermsNegotiation.dateUpdate = DateTime.Now;
                            db.TermsNegotiation.Attach(vTermsNegotiation);
                            db.Entry(vTermsNegotiation).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Terminos de Negociacion desactivadas exitosamente");
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

            var model = db.TermsNegotiation.ToList();
            return PartialView("_TermsNegotiationPartial", model.ToList());
        }

        #endregion
    }
}
