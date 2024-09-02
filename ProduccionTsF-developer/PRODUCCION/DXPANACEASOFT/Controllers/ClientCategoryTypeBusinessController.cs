using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class ClientCategoryTypeBusinessController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult ClientCategoryTypeBusinessResults(ClientCategoryTypeBusiness ClientCategoryTypeBusiness
                                                  )
        {
            
            var model = db.ClientCategoryTypeBusiness.ToList();

            #region  FILTERS
          


            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_ClientCategoryTypeBusinessResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult ClientCategoryTypeBusinessPartial()
        {
            var model = (TempData["model"] as List<ClientCategoryTypeBusiness>);

            model = db.ClientCategoryTypeBusiness.ToList();

            model = model ?? new List<ClientCategoryTypeBusiness>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_ClientCategoryTypeBusinessPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit ClientCategoryTypeBusiness
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditClientCategoryTypeBusiness(int id, int[] orderDetails)
        {
            ClientCategoryTypeBusiness ClientCategoryTypeBusiness = db.ClientCategoryTypeBusiness.Where(o => o.id == id).FirstOrDefault();

            if (ClientCategoryTypeBusiness == null)
            {

                ClientCategoryTypeBusiness = new ClientCategoryTypeBusiness
                {                   
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now
         
                };
            }
            TempData["ClientCategoryTypeBusiness"] = ClientCategoryTypeBusiness;
            TempData.Keep("ClientCategoryTypeBusiness");

            return PartialView("_FormEditClientCategoryTypeBusiness", ClientCategoryTypeBusiness);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_ClientCategoryTypeBusiness)
        {
            TempData.Keep("ClientCategoryTypeBusiness");
            int index = db.ClientCategoryTypeBusiness.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_ClientCategoryTypeBusiness);
            var result = new
            {
                maximunPages = db.ClientCategoryTypeBusiness.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

 
     
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            ClientCategoryTypeBusiness ClientCategoryTypeBusiness = db.ClientCategoryTypeBusiness.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (ClientCategoryTypeBusiness != null)
            {
                TempData["ClientCategoryTypeBusiness"] = ClientCategoryTypeBusiness;
                TempData.Keep("ClientCategoryTypeBusiness");
                return PartialView("_ClientCategoryTypeBusinessMainFormPartial", ClientCategoryTypeBusiness);
            }

            TempData.Keep("ClientCategoryTypeBusiness");

            return PartialView("_ClientCategoryTypeBusinessMainFormPartial", new ClientCategoryTypeBusiness());
        }
        #endregion
        #region Validacion
        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id,int id_Category, int id_CustomerType, int id_businessLine)
        {
            TempData.Keep("ClientCategoryTypeBusiness");


            bool rept = false;

            var cantre = db.ClientCategoryTypeBusiness.Where(x => x.id != id && x.id_clientCategory == id_Category && x.id_customerType == id_CustomerType && x.id_businessLine == id_businessLine).ToList().Count();
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
        public ActionResult ClientCategoryTypeBusinessPartialAddNew(ClientCategoryTypeBusiness item)
        {
            ClientCategoryTypeBusiness conClientCategoryTypeBusiness = (TempData["ClientCategoryTypeBusiness"] as ClientCategoryTypeBusiness);


            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region ClientCategoryTypeBusiness

                    #endregion


                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;



                    dbemp.ClientCategoryTypeBusiness.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    TempData["ClientCategoryTypeBusiness"] = item;
                    TempData.Keep("ClientCategoryTypeBusiness");
                    ViewData["EditMessage"] = SuccessMessage("Categoría: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("ClientCategoryTypeBusiness");
                    item = (TempData["ClientCategoryTypeBusiness"] as ClientCategoryTypeBusiness);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                    trans.Rollback();
                }
            }



            //return PartialView("_ShippingLineShippingAgencyMainFormPartial", item);

            return PartialView("_ClientCategoryTypeBusinessPartial", dbemp.ClientCategoryTypeBusiness.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ClientCategoryTypeBusinessPartialUpdate(ClientCategoryTypeBusiness item)
        {
            ClientCategoryTypeBusiness modelItem = db.ClientCategoryTypeBusiness.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                ClientCategoryTypeBusiness conClientCategoryTypeBusiness = (TempData["ClientCategoryTypeBusiness"] as ClientCategoryTypeBusiness);


        


                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region ClientCategoryTypeBusiness
                        modelItem.id_customerType = item.id_customerType;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.id_clientCategory = item.id_clientCategory;
                        modelItem.id_businessLine = item.id_businessLine;




                        #endregion




                        db.ClientCategoryTypeBusiness.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["ClientCategoryTypeBusiness"] = modelItem;
                        TempData.Keep("ClientCategoryTypeBusiness");
                        ViewData["EditMessage"] = SuccessMessage("Categoría: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("ClientCategoryTypeBusiness");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);

                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("ClientCategoryTypeBusiness");


            return PartialView("_ClientCategoryTypeBusinessPartial", db.ClientCategoryTypeBusiness.ToList());
        }
        #endregion
        #region ClientCategoryTypeBusiness Gridview

        [ValidateInput(false)]
        public ActionResult ClientCategoryTypeBusinessPartial(int? id)
        {
            if (id != null)
            {
                ViewData["ClientCategoryTypeBusinessToCopy"] = db.ClientCategoryTypeBusiness.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.ClientCategoryTypeBusiness.ToList();
            return PartialView("_ClientCategoryTypeBusinessPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ClientCategoryTypeBusinessPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ClientCategoryTypeBusiness.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                    
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.ClientCategoryTypeBusiness.Attach(item);
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

            var model = db.ClientCategoryTypeBusiness.ToList();
            return PartialView("_ClientCategoryTypeBusinessPartial", model.ToList());
        }

        public ActionResult DeleteSelectedClientCategoryTypeBusiness(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var CategoryCustomerTypes = db.ClientCategoryTypeBusiness.Where(i => ids.Contains(i.id));
                        foreach (var vCategoryCustomerType in CategoryCustomerTypes)
                        {


                            vCategoryCustomerType.id_userUpdate = ActiveUser.id;
                            vCategoryCustomerType.dateUpdate = DateTime.Now;

                            db.ClientCategoryTypeBusiness.Attach(vCategoryCustomerType);
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

            var model = db.ClientCategoryTypeBusiness.ToList();
            return PartialView("_ClientCategoryTypeBusinessPartial", model.ToList());
        }

        #endregion
    }
}
