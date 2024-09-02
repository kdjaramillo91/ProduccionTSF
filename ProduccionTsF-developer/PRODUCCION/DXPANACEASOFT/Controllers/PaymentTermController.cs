using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;
namespace DXPANACEASOFT.Controllers
{
    
    public class PaymentTermController : DefaultController
    {
        public ActionResult Index()
        {
            
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult PaymentTermResults(PaymentTerm PaymentTerm
                                                  )
        {

            var model = db.PaymentTerm.ToList();

            #region  FILTERS



            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_PaymentTermResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult PaymentTermPartial()
        {
            var model = (TempData["model"] as List<PaymentTerm>);

            model = db.PaymentTerm.Where(g => g.id_company == ActiveUser.id_company).ToList();

            model = model ?? new List<PaymentTerm>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_PaymentTermPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit PaymentTerm
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditPaymentTerm(int id, int[] orderDetails)
        {
            PaymentTerm PaymentTerm = db.PaymentTerm.Where(o => o.id == id).FirstOrDefault();

            if (PaymentTerm == null)
            {

                PaymentTerm = new PaymentTerm
                {

                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now

                };
            }
            TempData["PaymentTerm"] = PaymentTerm;
            TempData.Keep("PaymentTerm");

            return PartialView("_FormEditPaymentTerm", PaymentTerm);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_PaymentTerm)
        {
            TempData.Keep("PaymentTerm");
            int index = db.PaymentTerm.Where(g => g.id_company == ActiveUser.id_company).OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_PaymentTerm);
            var result = new
            {
                maximunPages = db.PaymentTerm.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            PaymentTerm PaymentTerm = db.PaymentTerm.Where(g => g.id_company == ActiveUser.id_company).OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (PaymentTerm != null)
            {
                TempData["PaymentTerm"] = PaymentTerm;
                TempData.Keep("PaymentTerm");
                return PartialView("_PaymentTermMainFormPartial", PaymentTerm);
            }

            TempData.Keep("PaymentTerm");

            return PartialView("_PaymentTermMainFormPartial", new PaymentTerm());
        }
        #endregion
        #region Validacion
        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id_PaymentTerm, string codio)
        {
            TempData.Keep("PaymentTerm");


            bool rept = false;

            var cantre = db.PaymentTerm.Where(x => x.id != id_PaymentTerm && x.code == codio && x.id_company == ActiveUser.id_company).ToList().Count();
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
        public ActionResult PaymentTermPartialAddNew(PaymentTerm item)
        {
            PaymentTerm conPaymentTerm = (TempData["PaymentTerm"] as PaymentTerm);


            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region PaymentTerm

                    #endregion


                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;

                    item.id_company = ActiveUser.id_company;

                    dbemp.PaymentTerm.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    TempData["PaymentTerm"] = item;
                    TempData.Keep("PaymentTerm");
                    ViewData["EditMessage"] = SuccessMessage("Forma de Pago: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("PaymentTerm");
                    item = (TempData["PaymentTerm"] as PaymentTerm);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                    trans.Rollback();
                }
            }



            //return PartialView("_PaymentTermMainFormPartial", item);

            return PartialView("_PaymentTermPartial", dbemp.PaymentTerm.Where(g => g.id_company == ActiveUser.id_company).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult PaymentTermPartialUpdate(PaymentTerm item)
        {
            PaymentTerm modelItem = db.PaymentTerm.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                PaymentTerm conPaymentTerm = (TempData["PaymentTerm"] as PaymentTerm);





                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region PaymentTerm
                        modelItem.id_PeriodType = item.id_PeriodType;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.descriptionEnglish = item.descriptionEnglish;
                        modelItem.code = item.code;
                        modelItem.isActive = item.isActive;
                        modelItem.firstPaymentDays = item.firstPaymentDays;
                        modelItem.formulaReady = item.formulaReady;
                        modelItem.PercentAnticipation = item.PercentAnticipation;
                        modelItem.name = item.name;
                        modelItem.PercentBalance = item.PercentBalance;
                        modelItem.numberPeriods = item.numberPeriods;
                        modelItem.description = item.description;
                        modelItem.id_company = ActiveUser.id_company;


                        #endregion




                        db.PaymentTerm.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["PaymentTerm"] = modelItem;
                        TempData.Keep("PaymentTerm");
                        ViewData["EditMessage"] = SuccessMessage("Forma de Pago: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("PaymentTerm");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);

                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("PaymentTerm");


            return PartialView("_PaymentTermPartial", db.PaymentTerm.Where(g => g.id_company == ActiveUser.id_company).ToList());
        }
        #endregion
        #region PaymentTerm Gridview

        [ValidateInput(false)]
        public ActionResult PaymentTermPartial(int? id)
        {
            if (id != null)
            {
                ViewData["PaymentTermToCopy"] = db.PaymentTerm.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.PaymentTerm.ToList();
            return PartialView("_PaymentTermPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PaymentTermPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.PaymentTerm.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.PaymentTerm.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Forma de Pago : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.PaymentTerm.Where(g => g.id_company == ActiveUser.id_company).ToList();
            return PartialView("_PaymentTermPartial", model.ToList());
        }

        public ActionResult DeleteSelectedPaymentTerm(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var PaymentTerms = db.PaymentTerm.Where(i => ids.Contains(i.id));
                        foreach (var vPaymentTerm in PaymentTerms)
                        {


                            vPaymentTerm.id_userUpdate = ActiveUser.id;
                            vPaymentTerm.dateUpdate = DateTime.Now;

                            db.PaymentTerm.Attach(vPaymentTerm);
                            db.Entry(vPaymentTerm).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Forma de Pago desactivadas exitosamente");
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

            var model = db.PaymentTerm.Where(g => g.id_company == ActiveUser.id_company).ToList();
            return PartialView("_PaymentTermPartial", model.ToList());
        }

        #endregion
    }
}
