using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class Country_IdentificationTypeController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult Country_IdentificationTypeResults(Country_IdentificationType CategoryCustomerType
                                                  )
        {
            
            var model = db.Country_IdentificationType.ToList();

            #region  FILTERS
          


            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_Country_IdentificationTypeResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult Country_IdentificationTypePartial()
        {
            var model = (TempData["model"] as List<Country_IdentificationType>);

            model = db.Country_IdentificationType.ToList();

            model = model ?? new List<Country_IdentificationType>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_Country_IdentificationTypePartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit CategoryCustomerType
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditCountry_IdentificationType(int id, int[] orderDetails)
        {
            Country_IdentificationType CategoryCustomerType = db.Country_IdentificationType.Where(o => o.id == id).FirstOrDefault();

            if (CategoryCustomerType == null)
            {

                CategoryCustomerType = new Country_IdentificationType
                {                   
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now
         
                };
            }
            TempData["Country_IdentificationType"] = CategoryCustomerType;
            TempData.Keep("Country_IdentificationType");

            return PartialView("_FormEditCountry_IdentificationType", CategoryCustomerType);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_CategoryCustomerType)
        {
            TempData.Keep("Country_IdentificationType");
            int index = db.Country_IdentificationType.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_CategoryCustomerType);
            var result = new
            {
                maximunPages = db.Country_IdentificationType.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

 
     
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            Country_IdentificationType CategoryCustomerType = db.Country_IdentificationType.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (CategoryCustomerType != null)
            {
                TempData["Country_IdentificationType"] = CategoryCustomerType;
                TempData.Keep("Country_IdentificationType");
                return PartialView("_Country_IdentificationTypeMainFormPartial", CategoryCustomerType);
            }

            TempData.Keep("Country_IdentificationType");

            return PartialView("_Country_IdentificationTypeMainFormPartial", new Country_IdentificationType());
        }
        #endregion
        #region Validacion
        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id,int id_country, int id_identificationType)
        {
            TempData.Keep("Country_IdentificationType");


            bool rept = false;

            var cantre = db.Country_IdentificationType.Where(x => x.id != id && x.id_country == id_country && x.id_identificationType == id_identificationType).ToList().Count();
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
        public ActionResult Country_IdentificationTypePartialAddNew(Country_IdentificationType item)
        {
            Country_IdentificationType conCategoryCustomerType = (TempData["Country_IdentificationType"] as Country_IdentificationType);


            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region Country_IdentificationType

                    #endregion


                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;



                    dbemp.Country_IdentificationType.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    TempData["Country_IdentificationType"] = item;
                    TempData.Keep("Country_IdentificationType");
                    ViewData["EditMessage"] = SuccessMessage("Categoría: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("Country_IdentificationType");
                    item = (TempData["Country_IdentificationType"] as Country_IdentificationType);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                    trans.Rollback();
                }
            }

            var modelo = db.Country_IdentificationType.ToList();


            //return PartialView("_ShippingLineShippingAgencyMainFormPartial", item);

            return PartialView("_Country_IdentificationTypePartial", modelo);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Country_IdentificationTypePartialUpdate(Country_IdentificationType item)
        {
            Country_IdentificationType modelItem = db.Country_IdentificationType.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                Country_IdentificationType conCategoryCustomerType = (TempData["Country_IdentificationType"] as Country_IdentificationType);


        


                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region Country_IdentificationType
                        modelItem.id_identificationType = item.id_identificationType;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.id_country = item.id_country;




                        #endregion




                        db.Country_IdentificationType.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["Country_IdentificationType"] = modelItem;
                        TempData.Keep("Country_IdentificationType");
                        ViewData["EditMessage"] = SuccessMessage("Categoría: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("Country_IdentificationType");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);

                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("Country_IdentificationType");


            return PartialView("_Country_IdentificationTypePartial", db.Country_IdentificationType.ToList());
        }
        #endregion
        #region Country_IdentificationType Gridview

        [ValidateInput(false)]
        public ActionResult Country_IdentificationTypePartial(int? id)
        {
            if (id != null)
            {
                ViewData["Country_IdentificationTypeToCopy"] = db.Country_IdentificationType.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.Country_IdentificationType.ToList();
            return PartialView("_Country_IdentificationTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Country_IdentificationTypePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Country_IdentificationType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                    
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.Country_IdentificationType.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Categoría : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.Country_IdentificationType.ToList();
            return PartialView("_Country_IdentificationTypePartial", model.ToList());
        }

        public ActionResult DeleteSelectedCountry_IdentificationType(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var CategoryCustomerTypes = db.Country_IdentificationType.Where(i => ids.Contains(i.id));
                        foreach (var vCategoryCustomerType in CategoryCustomerTypes)
                        {


                            vCategoryCustomerType.id_userUpdate = ActiveUser.id;
                            vCategoryCustomerType.dateUpdate = DateTime.Now;

                            db.Country_IdentificationType.Attach(vCategoryCustomerType);
                            db.Entry(vCategoryCustomerType).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Categorías desactivadas exitosamente");
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

            var model = db.Country_IdentificationType.ToList();
            return PartialView("_Country_IdentificationTypePartial", model.ToList());
        }

        #endregion
    }
}
