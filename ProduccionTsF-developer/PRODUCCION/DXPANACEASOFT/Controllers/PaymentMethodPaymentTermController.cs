using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;
namespace DXPANACEASOFT.Controllers
{/// <summary>
/// 
/// </summary>
    public class PaymentMethodPaymentTermController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult PaymentMethodPaymentTermResults(PaymentMethodPaymentTerm PaymentMethodPaymentTerm
                                                  )
        {

            var model = db.PaymentMethodPaymentTerm.ToList();

            #region  FILTERS



            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_PaymentMethodPaymentTermResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult PaymentMethodPaymentTermPartial()
        {
            var model = (TempData["model"] as List<PaymentMethodPaymentTerm>);

            model = db.PaymentMethodPaymentTerm.ToList();

            model = model ?? new List<PaymentMethodPaymentTerm>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_PaymentMethodPaymentTermPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit PaymentMethodPaymentTerm
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditPaymentMethodPaymentTerm(int id, int[] orderDetails)
        {
            PaymentMethodPaymentTerm PaymentMethodPaymentTerm = db.PaymentMethodPaymentTerm.Where(o => o.id == id).FirstOrDefault();

            if (PaymentMethodPaymentTerm == null)
            {

                PaymentMethodPaymentTerm = new PaymentMethodPaymentTerm
                {
                  
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now

                };
            }
            TempData["PaymentMethodPaymentTerm"] = PaymentMethodPaymentTerm;
            TempData.Keep("PaymentMethodPaymentTerm");

            return PartialView("_FormEditPaymentMethodPaymentTerm", PaymentMethodPaymentTerm);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_PaymentMethodPaymentTerm)
        {
            TempData.Keep("PaymentMethodPaymentTerm");
            int index = db.PaymentMethodPaymentTerm.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_PaymentMethodPaymentTerm);
            var result = new
            {
                maximunPages = db.PaymentMethodPaymentTerm.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            PaymentMethodPaymentTerm PaymentMethodPaymentTerm = db.PaymentMethodPaymentTerm.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (PaymentMethodPaymentTerm != null)
            {
                TempData["PaymentMethodPaymentTerm"] = PaymentMethodPaymentTerm;
                TempData.Keep("PaymentMethodPaymentTerm");
                return PartialView("_PaymentMethodPaymentTermMainFormPartial", PaymentMethodPaymentTerm);
            }

            TempData.Keep("PaymentMethodPaymentTerm");

            return PartialView("_PaymentMethodPaymentTermMainFormPartial", new PaymentMethodPaymentTerm());
        }
        #endregion
        #region Validacion
        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id, int id_paymentMethod, int id_paymentTerm)
        {
            TempData.Keep("PaymentMethodPaymentTerm");


            bool rept = false;

            var cantre = db.PaymentMethodPaymentTerm.Where(x => x.id != id && x.id_paymentMethod == id_paymentMethod && x.id_paymentTerm == id_paymentTerm).ToList().Count();
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
        public ActionResult PaymentMethodPaymentTermPartialAddNew(PaymentMethodPaymentTerm item)
        {
            PaymentMethodPaymentTerm conPaymentMethodPaymentTerm = (TempData["PaymentMethodPaymentTerm"] as PaymentMethodPaymentTerm);


            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region PaymentMethodPaymentTerm

                    #endregion


                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;



                    dbemp.PaymentMethodPaymentTerm.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    TempData["PaymentMethodPaymentTerm"] = item;
                    TempData.Keep("PaymentMethodPaymentTerm");
                    ViewData["EditMessage"] = SuccessMessage("Linea: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("PaymentMethodPaymentTerm");
                    item = (TempData["PaymentMethodPaymentTerm"] as PaymentMethodPaymentTerm);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                    trans.Rollback();
                }
            }



            //return PartialView("_PaymentMethodPaymentTermMainFormPartial", item);

            return PartialView("_PaymentMethodPaymentTermPartial", dbemp.PaymentMethodPaymentTerm.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult PaymentMethodPaymentTermPartialUpdate(PaymentMethodPaymentTerm item)
        {
            PaymentMethodPaymentTerm modelItem = db.PaymentMethodPaymentTerm.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                PaymentMethodPaymentTerm conPaymentMethodPaymentTerm = (TempData["PaymentMethodPaymentTerm"] as PaymentMethodPaymentTerm);





                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region PaymentMethodPaymentTerm
                        modelItem.id_paymentMethod = item.id_paymentMethod;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.id_paymentTerm = item.id_paymentTerm;
                        modelItem.isActive = item.isActive;




                        #endregion




                        db.PaymentMethodPaymentTerm.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["PaymentMethodPaymentTerm"] = modelItem;
                        TempData.Keep("PaymentMethodPaymentTerm");
                        ViewData["EditMessage"] = SuccessMessage("Linea: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("PaymentMethodPaymentTerm");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);

                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("PaymentMethodPaymentTerm");


            return PartialView("_PaymentMethodPaymentTermPartial", db.PaymentMethodPaymentTerm.ToList());
        }
        #endregion
        #region PaymentMethodPaymentTerm Gridview

        [ValidateInput(false)]
        public ActionResult PaymentMethodPaymentTermPartial(int? id)
        {
            if (id != null)
            {
                ViewData["PaymentMethodPaymentTermToCopy"] = db.PaymentMethodPaymentTerm.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.PaymentMethodPaymentTerm.ToList();
            return PartialView("_PaymentMethodPaymentTermPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PaymentMethodPaymentTermPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.PaymentMethodPaymentTerm.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.PaymentMethodPaymentTerm.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Linea : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.PaymentMethodPaymentTerm.ToList();
            return PartialView("_PaymentMethodPaymentTermPartial", model.ToList());
        }

        public ActionResult DeleteSelectedPaymentMethodPaymentTerm(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var PaymentMethodPaymentTerms = db.PaymentMethodPaymentTerm.Where(i => ids.Contains(i.id));
                        foreach (var vPaymentMethodPaymentTerm in PaymentMethodPaymentTerms)
                        {


                            vPaymentMethodPaymentTerm.id_userUpdate = ActiveUser.id;
                            vPaymentMethodPaymentTerm.dateUpdate = DateTime.Now;

                            db.PaymentMethodPaymentTerm.Attach(vPaymentMethodPaymentTerm);
                            db.Entry(vPaymentMethodPaymentTerm).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Linea desactivadas exitosamente");
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

            var model = db.PaymentMethodPaymentTerm.ToList();
            return PartialView("_PaymentMethodPaymentTermPartial", model.ToList());
        }

        #endregion
    }
}
