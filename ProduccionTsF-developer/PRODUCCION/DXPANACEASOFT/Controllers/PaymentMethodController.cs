using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class PaymentMethodController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult PaymentMethodResults(PaymentMethod PaymentMethod
                                                  )
        {

            var model = db.PaymentMethod.ToList();

            #region  FILTERS



            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_PaymentMethodResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult PaymentMethodPartial()
        {
            var model = (TempData["model"] as List<PaymentMethod>);

            model = db.PaymentMethod.Where(g => g.id_company == ActiveUser.id_company).ToList();

            model = model ?? new List<PaymentMethod>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_PaymentMethodPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit PaymentMethod
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditPaymentMethod(int id, int[] orderDetails)
        {
            PaymentMethod PaymentMethod = db.PaymentMethod.Where(o => o.id == id).FirstOrDefault();

            if (PaymentMethod == null)
            {

                PaymentMethod = new PaymentMethod
                {

                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now

                };
            }
            TempData["PaymentMethod"] = PaymentMethod;
            TempData.Keep("PaymentMethod");

            return PartialView("_FormEditPaymentMethod", PaymentMethod);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_PaymentMethod)
        {
            TempData.Keep("PaymentMethod");
            int index = db.PaymentMethod.Where(g => g.id_company == ActiveUser.id_company).OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_PaymentMethod);
            var result = new
            {
                maximunPages = db.PaymentMethod.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            PaymentMethod PaymentMethod = db.PaymentMethod.Where(g => g.id_company == ActiveUser.id_company).OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (PaymentMethod != null)
            {
                TempData["PaymentMethod"] = PaymentMethod;
                TempData.Keep("PaymentMethod");
                return PartialView("_PaymentMethodMainFormPartial", PaymentMethod);
            }

            TempData.Keep("PaymentMethod");

            return PartialView("_PaymentMethodMainFormPartial", new PaymentMethod());
        }
        #endregion
        #region Validacion
        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id_PaymentMethod, string codio)
        {
            TempData.Keep("PaymentMethod");


            bool rept = false;

            var cantre = db.PaymentMethod.Where(x => x.id != id_PaymentMethod && x.code == codio && x.id_company == ActiveUser.id_company).ToList().Count();
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
        public ActionResult PaymentMethodPartialAddNew(PaymentMethod item)
        {
            PaymentMethod conPaymentMethod = (TempData["PaymentMethod"] as PaymentMethod);


            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region PaymentMethod

                    #endregion


                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;

                    item.id_company = ActiveUser.id_company;

                    dbemp.PaymentMethod.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    TempData["PaymentMethod"] = item;
                    TempData.Keep("PaymentMethod");
                    ViewData["EditMessage"] = SuccessMessage("Metodo de Pago: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("PaymentMethod");
                    item = (TempData["PaymentMethod"] as PaymentMethod);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                    trans.Rollback();
                }
            }



            //return PartialView("_PaymentMethodMainFormPartial", item);

            return PartialView("_PaymentMethodPartial", dbemp.PaymentMethod.Where(g => g.id_company == ActiveUser.id_company).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult PaymentMethodPartialUpdate(PaymentMethod item)
        {
            PaymentMethod modelItem = db.PaymentMethod.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                PaymentMethod conPaymentMethod = (TempData["PaymentMethod"] as PaymentMethod);





                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region PaymentMethod
                        modelItem.codeProgram = item.codeProgram;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.codeSRI = item.codeSRI;
                        modelItem.code = item.code;
                        modelItem.isActive = item.isActive;
                        modelItem.descriptionEnglish = item.descriptionEnglish;
                      
                        modelItem.name = item.name;
                    
                        modelItem.description = item.description;
                        modelItem.id_company = ActiveUser.id_company;


                        #endregion




                        db.PaymentMethod.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["PaymentMethod"] = modelItem;
                        TempData.Keep("PaymentMethod");
                        ViewData["EditMessage"] = SuccessMessage("Metodo de Pago: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("PaymentMethod");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);

                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("PaymentMethod");


            return PartialView("_PaymentMethodPartial", db.PaymentMethod.Where(g => g.id_company == ActiveUser.id_company).ToList());
        }
        #endregion
        #region PaymentMethod Gridview

        [ValidateInput(false)]
        public ActionResult PaymentMethodPartial(int? id)
        {
            if (id != null)
            {
                ViewData["PaymentMethodToCopy"] = db.PaymentMethod.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.PaymentMethod.ToList();
            return PartialView("_PaymentMethodPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PaymentMethodPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.PaymentMethod.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.PaymentMethod.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Metodo de Pago : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.PaymentMethod.Where(g => g.id_company == ActiveUser.id_company).ToList();
            return PartialView("_PaymentMethodPartial", model.ToList());
        }

        public ActionResult DeleteSelectedPaymentMethod(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var PaymentMethods = db.PaymentMethod.Where(i => ids.Contains(i.id));
                        foreach (var vPaymentMethod in PaymentMethods)
                        {


                            vPaymentMethod.id_userUpdate = ActiveUser.id;
                            vPaymentMethod.dateUpdate = DateTime.Now;

                            db.PaymentMethod.Attach(vPaymentMethod);
                            db.Entry(vPaymentMethod).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Metodo de Pago desactivadas exitosamente");
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

            var model = db.PaymentMethod.Where(g => g.id_company == ActiveUser.id_company).ToList();
            return PartialView("_PaymentMethodPartial", model.ToList());
        }

        #endregion
    }
}
