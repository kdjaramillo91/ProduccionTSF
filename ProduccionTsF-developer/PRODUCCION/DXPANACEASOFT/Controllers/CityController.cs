using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class CityController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult CityResults(City City,
                                                  string scode,
                                                  int? id_country,
                                                  int? id_StateOfContry)
        {
            var model = db.City.ToList();

            #region  FILTERS
            if (!string.IsNullOrEmpty(scode))
            {
                model = model.Where(o => o.code == scode).ToList();
            }

            if (id_country != null && id_country > 0)
            {
                model = model.Where(o => o.id_country == id_country).ToList();
            }

            if (id_StateOfContry != null && id_StateOfContry > 0)
            {
                model = model.Where(o => o.id_stateOfContry == id_StateOfContry).ToList();
            }
         
            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_CityResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult StateOfContryall(int? id_country, int? id_countryCurrent)
        {
    
            if (id_country == null || id_country < 0)
            {
                if (Request.Params["id_country"] != null && Request.Params["id_country"] != "") id_country = int.Parse(Request.Params["id_country"]);
                else id_country = -1;
            }
            var StateOfContryAux = db.StateOfContry.Where(t => t.isActive && t.id_country == id_country).ToList();

            var StateOfContryCurrentAux = db.StateOfContry.FirstOrDefault(fod => fod.id == id_countryCurrent);
            if (StateOfContryCurrentAux != null && !StateOfContryAux.Contains(StateOfContryCurrentAux)) StateOfContryAux.Add(StateOfContryCurrentAux);


            TempData["StateOfContryall"] = StateOfContryAux.Select(s => new {
                s.id,
                name = s.name
            }).OrderBy(t => t.id).ToList();

            TempData.Keep("StateOfContryall");
            City City = (TempData["City"] as City);
            TempData.Keep("City");
            return PartialView("comboboxcascading/_cmbsateof", City);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CityPartial()
        {
            var model = (TempData["model"] as List<City>);
            model = model ?? new List<City>();
            TempData.Keep("model");
            return PartialView("_CityPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit City
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditCity(int id, int[] orderDetails)
        {
            City City = db.City.Where(o => o.id == id).FirstOrDefault();

            if (City == null)
            {

                City = new City
                {

                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now,
                    isActive = true,
                };
            }
            else
            {

                var StateOfContryAux = db.StateOfContry.Where(t => t.isActive && t.id_country == City.id_country).ToList();

                var StateOfContryCurrentAux = db.StateOfContry.FirstOrDefault(fod => fod.id == City.id_country);
                if (StateOfContryCurrentAux != null && !StateOfContryAux.Contains(StateOfContryCurrentAux)) StateOfContryAux.Add(StateOfContryCurrentAux);


                TempData["StateOfContryall"] = StateOfContryAux.Select(s => new {
                    s.id,
                    name = s.name
                }).OrderBy(t => t.id).ToList();

                TempData.Keep("StateOfContryall");
            }
            TempData["City"] = City;
            TempData.Keep("City");

            return PartialView("_FormEditCity", City);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_City)
        {
            TempData.Keep("City");
            int index = db.City.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_City);
            var result = new
            {
                maximunPages = db.City.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            City City = db.City.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (City != null)
            {
                TempData["City"] = City;
                TempData.Keep("City");
                return PartialView("_CityMainFormPartial", City);
            }

            TempData.Keep("City");

            return PartialView("_CityMainFormPartial", new City());
        }
        #endregion
        #region Validacion
        public Boolean validate(City item)
        {
            Boolean wsreturn = true;
            try
            {
                ///  City conCity = (TempData["City"] as City);
                if (String.IsNullOrEmpty(item.name))
                {
                    TempData.Keep("City");
                    ViewData["EditMessage"] = ErrorMessage("Ingrse el Nombre del Ciudad");
                    wsreturn = false;
                }

                if (String.IsNullOrEmpty(item.code))
                {
                    TempData.Keep("City");
                    ViewData["EditMessage"] = ErrorMessage("Ingrse el Codigo del Ciudad");
                    wsreturn = false;
                }
                else
                {


                    if (item.code.ToString().ToString().Contains(" ") && item.code.ToString().ToString().Length > 0)
                    {
                        TempData.Keep("City");
                        ViewData["EditMessage"] = ErrorMessage("El Codigo del Ciudad no puede tener espacio");
                        wsreturn = false;
                    }
                }


                var cantre = db.City.Where(x => x.id != item.id && x.code == item.code).ToList().Count();
                if (cantre > 0)
                {
                    TempData.Keep("City");
                    ViewData["EditMessage"] = ErrorMessage("Y existe un Ciudad con el mismo Codigo");
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
        public ActionResult CityPartialAddNew(bool approve, City item)
        {
            City conCity = (TempData["City"] as City);

            if (!validate(item))
            {
                return PartialView("_CityMainFormPartial", item);
            }
            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region City
                    item.Country = null;
                    #endregion


                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;

                    if (approve)
                    { item.isActive = true; }

                    dbemp.City.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    item.Country = dbemp.Country.Where(x => x.id == item.id_country).FirstOrDefault();
                    TempData["City"] = item;
                    TempData.Keep("City");
                    ViewData["EditMessage"] = SuccessMessage("Ciudad: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("City");
                    item = (TempData["City"] as City);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            return PartialView("_CityMainFormPartial", item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CityPartialUpdate(bool approve, City item)
        {
            City modelItem = db.City.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                City conCity = (TempData["City"] as City);

                //bool bauto = false;
                if (!validate(item))
                {
                    return PartialView("_CityMainFormPartial", item);
                }



                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region City
                        modelItem.code = item.code;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.name = item.name;
                        modelItem.isActive = item.isActive;
                        modelItem.id_country = item.id_country;
                        modelItem.id_stateOfContry = item.id_stateOfContry;
                        modelItem.isCapital = item.isCapital;

                        modelItem.Country = db.Country.Where(x => x.id == modelItem.id_country).FirstOrDefault();
                        modelItem.StateOfContry = db.StateOfContry.Where(x => x.id == modelItem.id_stateOfContry).FirstOrDefault();
                        #endregion


                        if (approve)
                        {
                            modelItem.isActive = true;
                        }

                        db.City.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["City"] = modelItem;
                        TempData.Keep("City");
                        ViewData["EditMessage"] = SuccessMessage("Ciudad: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("City");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("City");
            return PartialView("_CityMainFormPartial", modelItem);
        }
        #endregion
        #region City Gridview

        [ValidateInput(false)]
        public ActionResult CityPartial(int? id)
        {
            if (id != null)
            {
                ViewData["CityToCopy"] = db.City.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.City.ToList();
            return PartialView("_CityPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CityPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.City.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.City.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Ciudad : " + (item?.name ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.City.ToList();
            return PartialView("_CityPartial", model.ToList());
        }

        public ActionResult DeleteSelectedCity(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Citys = db.City.Where(i => ids.Contains(i.id));
                        foreach (var vCity in Citys)
                        {
                            vCity.isActive = false;

                            vCity.id_userUpdate = ActiveUser.id;
                            vCity.dateUpdate = DateTime.Now;

                            db.City.Attach(vCity);
                            db.Entry(vCity).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Ciudad desactivadas exitosamente");
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

            var model = db.City.ToList();
            return PartialView("_CityPartial", model.ToList());
        }

        #endregion
    }
}
