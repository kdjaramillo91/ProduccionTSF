using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class StateOfContryController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult StateOfContryResults(StateOfContry StateOfContry,
                                                  string scode,
                                                  int? id_country)
        { var model = db.StateOfContry.ToList();

            #region  FILTERS
            if (!string.IsNullOrEmpty(scode))
            {
                model = model.Where(o => o.code == scode).ToList();
            }

            if (id_country != null && id_country > 0)
            {
                model = model.Where(o => o.id_country == id_country).ToList();
            }
            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_StateOfContryResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult StateOfContryPartial()
        {
            var model = (TempData["model"] as List<StateOfContry>);
            model = model ?? new List<StateOfContry>();
            TempData.Keep("model");
            return PartialView("_StateOfContryPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit StateOfContry
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditStateOfContry(int id, int[] orderDetails)
        {
            StateOfContry StateOfContry = db.StateOfContry.Where(o => o.id == id).FirstOrDefault();

            if (StateOfContry == null)
            {

                StateOfContry = new StateOfContry
                {  
                   
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now,
                    isActive = true,
                };
            }
            TempData["StateOfContry"] = StateOfContry;
            TempData.Keep("StateOfContry");

            return PartialView("_FormEditStateOfContry", StateOfContry);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_StateOfContry)
        {
            TempData.Keep("StateOfContry");
            int index = db.StateOfContry.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_StateOfContry);
            var result = new
            {
                maximunPages = db.StateOfContry.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            StateOfContry StateOfContry = db.StateOfContry.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (StateOfContry != null)
            {
                TempData["StateOfContry"] = StateOfContry;
                TempData.Keep("StateOfContry");
                return PartialView("_StateOfContryMainFormPartial", StateOfContry);
            }

            TempData.Keep("StateOfContry");

            return PartialView("_StateOfContryMainFormPartial", new StateOfContry());
        }
        #endregion
        #region Validacion
        public Boolean validate(StateOfContry item)
        {
            Boolean wsreturn = true;
            try
            {
              ///  StateOfContry conStateOfContry = (TempData["StateOfContry"] as StateOfContry);
                if (String.IsNullOrEmpty(item.name) )
                {
                    TempData.Keep("StateOfContry");
                    ViewData["EditMessage"] = ErrorMessage("Ingrse el Nombre del Estado");
                    wsreturn = false;
                }

                if (String.IsNullOrEmpty(item.code))
                {
                    TempData.Keep("StateOfContry");
                    ViewData["EditMessage"] = ErrorMessage("Ingrse el Codigo del Estado");
                    wsreturn = false;
                }
                else {


                    if (item.code.ToString().ToString().Contains(" ") && item.code.ToString().ToString().Length>0)
                    {
                        TempData.Keep("StateOfContry");
                        ViewData["EditMessage"] = ErrorMessage("El Codigo del Estado no puede tener espacio");
                        wsreturn = false;
                    }
                }


                var cantre = db.StateOfContry.Where(x => x.id != item.id && x.code == item.code ).ToList().Count();
                if (cantre > 0)
                {
                    TempData.Keep("StateOfContry");
                    ViewData["EditMessage"] = ErrorMessage("Y existe un Estado con el mismo Codigo");
                    wsreturn = false;
                }

           }
            catch (Exception)
            {


            }

            return wsreturn;

        }
        #endregion
        #region Save and Update
        [HttpPost, ValidateInput(false)]
        public ActionResult StateOfContryPartialAddNew(bool approve, StateOfContry item)
        {
            StateOfContry conStateOfContry = (TempData["StateOfContry"] as StateOfContry);
           
            if (!validate(item))
            {
                return PartialView("_StateOfContryMainFormPartial", item);
            }
            DBContext dbemp = new DBContext();
      
            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region StateOfContry
                    item.Country = null;
                    #endregion

               
                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;

                    if (approve)
                    { item.isActive = true; }

                    dbemp.StateOfContry.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    item.Country = dbemp.Country.Where(x => x.id == item.id_country).FirstOrDefault();
                    TempData["StateOfContry"] = item;
                    TempData.Keep("StateOfContry");
                    ViewData["EditMessage"] = SuccessMessage("Estado: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("StateOfContry");
                    item = (TempData["StateOfContry"] as StateOfContry);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            return PartialView("_StateOfContryMainFormPartial", item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult StateOfContryPartialUpdate(bool approve, StateOfContry item)
        {
            StateOfContry modelItem = db.StateOfContry.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                StateOfContry conStateOfContry = (TempData["StateOfContry"] as StateOfContry);

                //bool bauto = false;
                if (!validate(item))
                {
                    return PartialView("_StateOfContryMainFormPartial", item);
                }

              

                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region StateOfContry
                        modelItem.code = item.code;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.name = item.name;
                        modelItem.isActive = item.isActive;
                        modelItem.id_country = item.id_country;
                       

                        modelItem.Country = db.Country.Where(x => x.id == modelItem.id_country).FirstOrDefault();
                        #endregion


                        if (approve)
                        {
                            modelItem.isActive = true;
                        }

                        db.StateOfContry.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["StateOfContry"] = modelItem;
                        TempData.Keep("StateOfContry");
                        ViewData["EditMessage"] = SuccessMessage("Estado: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("StateOfContry");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("StateOfContry");
            return PartialView("_StateOfContryMainFormPartial", modelItem);
        }
        #endregion
        #region StateOfContry Gridview

        [ValidateInput(false)]
        public ActionResult StateOfContryPartial(int? id)
        {
            if (id != null)
            {
                ViewData["StateOfContryToCopy"] = db.StateOfContry.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.StateOfContry.ToList();
            return PartialView("_StateOfContryPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult StateOfContryPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.StateOfContry.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.StateOfContry.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Estado : " + (item?.name ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.StateOfContry.ToList();
            return PartialView("_StateOfContryPartial", model.ToList());
        }

        public ActionResult DeleteSelectedStateOfContry(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var StateOfContrys = db.StateOfContry.Where(i => ids.Contains(i.id));
                        foreach (var vStateOfContry in StateOfContrys)
                        {
                            vStateOfContry.isActive = false;

                            vStateOfContry.id_userUpdate = ActiveUser.id;
                            vStateOfContry.dateUpdate = DateTime.Now;

                            db.StateOfContry.Attach(vStateOfContry);
                            db.Entry(vStateOfContry).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Estado desactivadas exitosamente");
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

            var model = db.StateOfContry.ToList();
            return PartialView("_StateOfContryPartial", model.ToList());
        }

        #endregion
    }
}
