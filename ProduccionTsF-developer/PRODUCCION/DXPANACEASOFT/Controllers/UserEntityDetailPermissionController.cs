using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class UserEntityDetailPermissionController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult UserEntityDetailPermissionResults(UserEntityDetailPermission UserEntityDetailPermission
                                                  )
        {
            
            var model = db.UserEntityDetailPermission.ToList();

            #region  FILTERS
          
            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_UserEntityDetailPermissionResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion

        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult UserEntityDetailPermissionPartial()
        {
            var model = (TempData["model"] as List<UserEntityDetail>);

            model = db.UserEntityDetail.ToList();

            var warehouse = db.Warehouse.ToList();
            var userR = db.UserEntityDetail.ToList();

            model = model ?? new List<UserEntityDetail>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_UserEntityDetailPermissionPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion

        #region Edit UserEntityDetailPermission
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditUserEntityDetailPermission(int id, int[] orderDetails)
        {
            UserEntityDetailPermission UserEntityDetailPermission = db.UserEntityDetailPermission.Where(o => o.id == id).FirstOrDefault();

            if (UserEntityDetailPermission == null)
            {

                UserEntityDetailPermission = new UserEntityDetailPermission
                {                   
                    //id = ActiveUser.id,
                    //dateUpdate = DateTime.Now
         
                };
            }
            TempData["UserEntityDetailPermission"] = UserEntityDetailPermission;
            TempData.Keep("UserEntityDetailPermission");

            return PartialView("_FormEditUserEntityDetailPermission", UserEntityDetailPermission);
        }
        #endregion

        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_CategoryCustomerType)
        {
            TempData.Keep("UserEntityDetailPermission");
            int index = db.UserEntityDetailPermission.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_CategoryCustomerType);
            var result = new
            {
                maximunPages = db.UserEntityDetailPermission.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

 
     
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            UserEntityDetailPermission UserEntityDetailPermission = db.UserEntityDetailPermission.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (UserEntityDetailPermission != null)
            {
                TempData["UserEntityDetailPermission"] = UserEntityDetailPermission;
                TempData.Keep("UserEntityDetailPermission");
                return PartialView("_UserEntityDetailPermissionMainFormPartial", UserEntityDetailPermission);
            }

            TempData.Keep("UserEntityDetailPermission");

            return PartialView("_UserEntityDetailPermissionMainFormPartial", new UserEntityDetailPermission());
        }
        #endregion

        #region Validacion
        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id,int id_Category, int id_CustomerType)
        {
            TempData.Keep("UserEntityDetail");


            bool rept = false;

            var cantre = db.UserEntityDetailPermission.Where(x => x.id != id && x.id_permission == id_Category && x.id_userEntityDetail == id_CustomerType).ToList().Count();
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
        public ActionResult UserEntityDetailPermissionPartialAddNew(string id_user, string entity)
        {
            UserEntityDetail conCategoryCustomerType = (TempData["UserEntityDetail"] as UserEntityDetail);

            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region UserEntityDetail

                    #endregion


                    //item.dateCreate = DateTime.Now;
                    //item.dateUpdate = DateTime.Now;

                    if (!string.IsNullOrWhiteSpace(id_user) && !string.IsNullOrWhiteSpace(entity))
                    {
                        var id_userOk = db.User.Where(i => i.username == id_user).FirstOrDefault().id;
                        var id_entity = db.Warehouse.Where(i => i.name == entity).FirstOrDefault().id;
                        UserEntity id_userEntity = db.UserEntity.Where(e => e.id_user == id_userOk).FirstOrDefault();

                        if (id_userEntity == null)
                        {
                            dbemp.UserEntity.Add(new UserEntity()
                            {
                                id_user = id_userOk,
                                id_entity = 1,
                                id_userCreate = 1,
                                dateCreate = DateTime.Now,
                                id_userUpdate = 1,
                                dateUpdate = DateTime.Now,
                            });
                            dbemp.SaveChanges();
                        }
                        id_userEntity = db.UserEntity.Where(e => e.id_user == id_userOk).FirstOrDefault();

                        var validador = db.UserEntityDetail.Where(e => e.id_userEntity == id_userEntity.id && e.id_entityValue == id_entity).ToList();
                        if (validador.Count > 0)
                        {
                            ViewData["EditMessage"] = ErrorMessage("Relación de usuario-bodega ya existente");
                        }
                        else
                        {
                            dbemp.UserEntityDetail.Add(new UserEntityDetail()
                            {
                                id_entityValue = id_entity,
                                id_userEntity = id_userEntity.id
                            });
                            dbemp.SaveChanges();
                            trans.Commit();

                            //TempData["UserEntityDetail"] = item;
                            //TempData.Keep("UserEntityDetail");
                            SuccessMessage();
                            ViewData["EditMessage"] = SuccessMessage("Permiso a la bodega: " + entity + " guardado exitosamente");
                        }
                    }
                }
                catch (Exception e)
                {
                    //TempData.Keep("UserEntityDetail");
                    //item = (TempData["UserEntityDetail"] as UserEntityDetail);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                    trans.Rollback();
                }
            }

            var modelo = db.UserEntityDetail.ToList();


            //return PartialView("_ShippingLineShippingAgencyMainFormPartial", item);

            return PartialView("_UserEntityDetailPermissionPartial", modelo.OrderByDescending(r => r.id).ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UserEntityDetailPermissionPartialUpdate(UserEntityDetailPermission item)
        {
            UserEntityDetailPermission modelItem = db.UserEntityDetailPermission.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                UserEntityDetailPermission conCategoryCustomerType = (TempData["UserEntityDetailPermission"] as UserEntityDetailPermission);

                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region UserEntityDetailPermission
                        //modelItem.id_CustomerType = item.id_CustomerType;
                        //modelItem.id_userUpdate = ActiveUser.id;
                        //modelItem.dateUpdate = DateTime.Now;
                        //modelItem.id_Category = item.id_Category;




                        #endregion


                        db.UserEntityDetailPermission.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["UserEntityDetailPermission"] = modelItem;
                        TempData.Keep("UserEntityDetailPermission");
                        ViewData["EditMessage"] = SuccessMessage("Permiso: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                         TempData.Keep("UserEntityDetailPermission");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);

                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("UserEntityDetailPermission");


            return PartialView("_UserEntityDetailPermissionPartial", db.UserEntityDetailPermission.ToList());
        }
        #endregion

        #region UserEntityDetailPermission Gridview

        [ValidateInput(false)]
        public ActionResult UserEntityDetailPermissionPartial(int? id)
        {
            if (id != null)
            {
                ViewData["UserEntityDetailPermissionToCopy"] = db.UserEntityDetailPermission.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.UserEntityDetailPermission.ToList();
            return PartialView("_UserEntityDetailPermissionPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UserEntityDetailPermissionPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.UserEntityDetailPermission.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                    
                            //item.id_userUpdate = ActiveUser.id;
                            //item.dateUpdate = DateTime.Now;

                        }
                        db.UserEntityDetailPermission.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Permiso : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.UserEntityDetailPermission.ToList();
            return PartialView("_UserEntityDetailPermissionPartial", model.ToList());
        }

        public ActionResult DeleteSelectedUserEntityDetailPermission(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var CategoryCustomerTypes = db.UserEntityDetailPermission.Where(i => ids.Contains(i.id));
                        foreach (var vCategoryCustomerType in CategoryCustomerTypes)
                        {


                            //vCategoryCustomerType.id_userUpdate = ActiveUser.id;
                            //vCategoryCustomerType.dateUpdate = DateTime.Now;

                            db.UserEntityDetailPermission.Attach(vCategoryCustomerType);
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

            var model = db.UserEntityDetailPermission.ToList();
            return PartialView("_UserEntityDetailPermissionPartial", model.ToList());
        }

        #endregion
    }
}
