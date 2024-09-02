using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class CategoryCustomerTypeController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult CategoryCustomerTypeResults(CategoryCustomerType CategoryCustomerType
                                                  )
        {
            
            var model = db.CategoryCustomerType.ToList();

            #region  FILTERS
          


            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_CategoryCustomerTypeResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryCustomerTypePartial()
        {
            var model = (TempData["model"] as List<CategoryCustomerType>);

            model = db.CategoryCustomerType.ToList();

            model = model ?? new List<CategoryCustomerType>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_CategoryCustomerTypePartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit CategoryCustomerType
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditCategoryCustomerType(int id, int[] orderDetails)
        {
            CategoryCustomerType CategoryCustomerType = db.CategoryCustomerType.Where(o => o.id == id).FirstOrDefault();

            if (CategoryCustomerType == null)
            {

                CategoryCustomerType = new CategoryCustomerType
                {                   
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now
         
                };
            }
            TempData["CategoryCustomerType"] = CategoryCustomerType;
            TempData.Keep("CategoryCustomerType");

            return PartialView("_FormEditCategoryCustomerType", CategoryCustomerType);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_CategoryCustomerType)
        {
            TempData.Keep("CategoryCustomerType");
            int index = db.CategoryCustomerType.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_CategoryCustomerType);
            var result = new
            {
                maximunPages = db.CategoryCustomerType.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

 
     
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            CategoryCustomerType CategoryCustomerType = db.CategoryCustomerType.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (CategoryCustomerType != null)
            {
                TempData["CategoryCustomerType"] = CategoryCustomerType;
                TempData.Keep("CategoryCustomerType");
                return PartialView("_CategoryCustomerTypeMainFormPartial", CategoryCustomerType);
            }

            TempData.Keep("CategoryCustomerType");

            return PartialView("_CategoryCustomerTypeMainFormPartial", new CategoryCustomerType());
        }
        #endregion
        #region Validacion
        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id,int id_Category, int id_CustomerType)
        {
            TempData.Keep("CategoryCustomerType");


            bool rept = false;

            var cantre = db.CategoryCustomerType.Where(x => x.id != id && x.id_Category == id_Category && x.id_CustomerType == id_CustomerType).ToList().Count();
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
        public ActionResult CategoryCustomerTypePartialAddNew(CategoryCustomerType item)
        {
            CategoryCustomerType conCategoryCustomerType = (TempData["CategoryCustomerType"] as CategoryCustomerType);


            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region CategoryCustomerType

                    #endregion


                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;



                    dbemp.CategoryCustomerType.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    TempData["CategoryCustomerType"] = item;
                    TempData.Keep("CategoryCustomerType");
                    ViewData["EditMessage"] = SuccessMessage("Categoría: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("CategoryCustomerType");
                    item = (TempData["CategoryCustomerType"] as CategoryCustomerType);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                    trans.Rollback();
                }
            }

            var modelo = db.CategoryCustomerType.ToList();


            //return PartialView("_ShippingLineShippingAgencyMainFormPartial", item);

            return PartialView("_CategoryCustomerTypePartial", modelo);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryCustomerTypePartialUpdate(CategoryCustomerType item)
        {
            CategoryCustomerType modelItem = db.CategoryCustomerType.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                CategoryCustomerType conCategoryCustomerType = (TempData["CategoryCustomerType"] as CategoryCustomerType);


        


                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region CategoryCustomerType
                        modelItem.id_CustomerType = item.id_CustomerType;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.id_Category = item.id_Category;




                        #endregion




                        db.CategoryCustomerType.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["CategoryCustomerType"] = modelItem;
                        TempData.Keep("CategoryCustomerType");
                        ViewData["EditMessage"] = SuccessMessage("Categoría: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("CategoryCustomerType");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);

                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("CategoryCustomerType");


            return PartialView("_CategoryCustomerTypePartial", db.CategoryCustomerType.ToList());
        }
        #endregion
        #region CategoryCustomerType Gridview

        [ValidateInput(false)]
        public ActionResult CategoryCustomerTypePartial(int? id)
        {
            if (id != null)
            {
                ViewData["CategoryCustomerTypeToCopy"] = db.CategoryCustomerType.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.CategoryCustomerType.ToList();
            return PartialView("_CategoryCustomerTypePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CategoryCustomerTypePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.CategoryCustomerType.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                    
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.CategoryCustomerType.Attach(item);
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

            var model = db.CategoryCustomerType.ToList();
            return PartialView("_CategoryCustomerTypePartial", model.ToList());
        }

        public ActionResult DeleteSelectedCategoryCustomerType(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var CategoryCustomerTypes = db.CategoryCustomerType.Where(i => ids.Contains(i.id));
                        foreach (var vCategoryCustomerType in CategoryCustomerTypes)
                        {


                            vCategoryCustomerType.id_userUpdate = ActiveUser.id;
                            vCategoryCustomerType.dateUpdate = DateTime.Now;

                            db.CategoryCustomerType.Attach(vCategoryCustomerType);
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

            var model = db.CategoryCustomerType.ToList();
            return PartialView("_CategoryCustomerTypePartial", model.ToList());
        }

        #endregion
    }
}
