using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Validation;

namespace DXPANACEASOFT.Controllers
{
	[Authorize]
	public class WarehouseUserPermitController : DefaultController
	{

        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult WarehouseUserPermitPartial()
        {
            UserEntity warehouseUserPermit = (TempData["warehouseUserPermit"] as UserEntity);
            if (warehouseUserPermit != null) TempData.Remove("warehouseUserPermit");

            var model = db.UserEntity.OrderBy(p => p.id);

            ViewData["ShowCodeUser"] = true;
            ViewData["ShowCodeWarehouse"] = true;

            return PartialView("_WarehouseUserPermitPartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult WarehouseUserPermitPartialAddNew(DXPANACEASOFT.Models.UserEntity warehouseUserPermit)
        {
            var model = db.UserEntity;

            UserEntity tempWarehouseUserPermit = (TempData["warehouseUserPermit"] as UserEntity);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    // Detalles del Registro
                    if (tempWarehouseUserPermit?.UserEntityDetail != null)
                    {
                        warehouseUserPermit.UserEntityDetail = new List<UserEntityDetail>();

                        var warehouseUserPermitDetails = tempWarehouseUserPermit.UserEntityDetail.ToList();

                        foreach (var warehouseUserPermitDetail in warehouseUserPermitDetails)
                        {
                            var tempwarehouseUserPermitDetail = new UserEntityDetail
                            {
                                id_userEntity = warehouseUserPermitDetail.id_userEntity,
                                id_entityValue = warehouseUserPermitDetail.id_entityValue
                            };

                            foreach(var aaa in warehouseUserPermitDetail.UserEntityDetailPermission)
                            {
                                var tempwarehousePermition = new UserEntityDetailPermission
                                {
                                    id_userEntityDetail = aaa.id_userEntityDetail,
                                    id_permission = aaa.id_permission
                                };

                                tempwarehouseUserPermitDetail.UserEntityDetailPermission.Add(tempwarehousePermition);
                            }

                            warehouseUserPermit.UserEntityDetail.Add(tempwarehouseUserPermitDetail);
                        }
                    }

                   
                    // CAMPOS DE AUDITORIA 
                    warehouseUserPermit.id_userCreate = ActiveUser.id;
                    warehouseUserPermit.dateCreate = DateTime.Now;
                    warehouseUserPermit.id_userUpdate = ActiveUser.id;
                    warehouseUserPermit.dateUpdate = DateTime.Now;

                    // CAMPO DE ENTIDAD
                    warehouseUserPermit.id_entity = 1;

                    model.Add(warehouseUserPermit);
                    db.SaveChanges();
                    trans.Commit();

                    TempData.Keep("warehouseUserPermit");
                    TempData.Keep("id_warehouseUserPermitDetail");

                    ViewData["ShowCodeUser"] = true;

                    ViewData["EditMessage"] = SuccessMessage("Permisos a Bodega otorgados exitosamente");
                }
                catch (DbEntityValidationException e)
                {
                    TempData.Keep("warehouseUserPermit");
                    TempData.Keep("id_warehouseUserPermitDetail");

                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }
            return PartialView("_WarehouseUserPermitPartial", model.OrderBy(p => p.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult WarehouseUserPermitPartialUpdate(DXPANACEASOFT.Models.UserEntity warehouseUserPermit)
        {
            var modelUserEntity = db.UserEntity.FirstOrDefault(it => it.id == warehouseUserPermit.id);

            UserEntity tempWarehouseUserPermit = (TempData["warehouseUserPermit"] as UserEntity);
            var modelUser = db.UserEntity.FirstOrDefault(e => e.id == tempWarehouseUserPermit.id);

             if (ModelState.IsValid)
             {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (modelUserEntity != null)
                        {
                            if (modelUserEntity?.UserEntityDetail != null)
                            {
                                warehouseUserPermit.UserEntityDetail = new List<UserEntityDetail>();
                                var dbwarehouseDetails = modelUserEntity.UserEntityDetail.ToList();
                                var uptdbwarehouseDetails = tempWarehouseUserPermit.UserEntityDetail.ToList();
                                var uptdbwarehouseDetailss = modelUser.UserEntityDetail.ToList();

                                foreach (var warehouseDetail in uptdbwarehouseDetails)
                                {
                                    var oriWarehouseDetail = dbwarehouseDetails.FirstOrDefault(r => r.id == warehouseDetail.id);
                                    if (oriWarehouseDetail != null)
                                    {
                                        oriWarehouseDetail.id = warehouseDetail.id;
                                        oriWarehouseDetail.id_userEntity = warehouseDetail.id_userEntity;
                                        oriWarehouseDetail.id_entityValue = warehouseDetail.id_entityValue;

                                        var lista = warehouseDetail.UserEntityDetailPermission.ToList();

                                        foreach (var aaa in warehouseDetail.UserEntityDetailPermission)
                                        {
                                            var listaAux = lista.FirstOrDefault(X => X.id == aaa.id);
                                            if(listaAux != null)
                                            {
                                                listaAux.id_userEntityDetail = aaa.id_userEntityDetail;
                                                listaAux.id_permission = aaa.id_permission;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var tempwarehouseUserPermitDetail = new UserEntityDetail
                                        {
                                            id_userEntity = warehouseDetail.id_userEntity,
                                            id_entityValue = warehouseDetail.id_entityValue
                                        };

                                        var lista = warehouseDetail.UserEntityDetailPermission.ToList();

                                        foreach (var aaa in lista)
                                        {
                                            var tempwarehousePermition = new UserEntityDetailPermission
                                            {
                                                id_userEntityDetail = aaa.id_userEntityDetail,
                                                id_permission = aaa.id_permission
                                            };

                                            tempwarehouseUserPermitDetail.UserEntityDetailPermission.Add(tempwarehousePermition);
                                        }

                                        modelUserEntity.UserEntityDetail.Add(tempwarehouseUserPermitDetail);
                                    }
                                }
                            }

                            // CAMPOS DE AUDITORIA 
                            modelUserEntity.id_userCreate = ActiveUser.id;
                            modelUserEntity.dateCreate = DateTime.Now;
                            modelUserEntity.id_userUpdate = ActiveUser.id;
                            modelUserEntity.dateUpdate = DateTime.Now;

                            // CAMPO DE ENTIDAD
                            modelUserEntity.id_entity = 1;

                            db.UserEntity.Attach(modelUserEntity);
                            db.Entry(modelUserEntity).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["ShowCodeUser"] = true;

                            ViewData["EditMessage"] = SuccessMessage("Permisos a Bodega Actualizado exitosamente");
                        }
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("warehouseUserPermit");
                        TempData.Keep("id_warehouseUserPermitDetail");

                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
             }
             else
             {
                ViewData["EditMessage"] = ErrorMessage();
             }

            if (modelUserEntity == null)
            {
                return WarehouseUserPermitPartialAddNew(warehouseUserPermit);
            }

            return PartialView("_WarehouseUserPermitPartial", db.UserEntity.OrderBy(m => m.id).ToList());
        }

        /////*  Detalle*/

        [ValidateInput(false)]
        public ActionResult WarehouseUserPermitDetail(int id_warehouseUserPermit, int id_warehouseUserPermitDetail)
        {
            UserEntity userEntity = ObtainWarehouseUserPermit(id_warehouseUserPermit);

            var model = userEntity.UserEntityDetail?.ToList() ?? new List<UserEntityDetail>();

            TempData["id_warehouseUserPermitDetail"] = id_warehouseUserPermitDetail;
            TempData.Keep("id_warehouseUserPermitDetail");

            TempData["warehouseUserPermit"] = TempData["warehouseUserPermit"] ?? userEntity;
            TempData.Keep("warehouseUserPermit");

            ViewData["ShowCodeWarehouse"] = true;

            return PartialView("_WarehouseUserPermitDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult WarehouseUserPermitDetailChangePartial(int id_warehouseUserPermitDetail, int id_warehouseUserPermit)
        {
            UserEntity userEntity = ObtainWarehouseUserPermit(id_warehouseUserPermit);
            var model = userEntity.UserEntityDetail?.ToList() ?? new List<UserEntityDetail>();

            TempData["warehouseUserPermit"] = userEntity;
            TempData.Keep("warehouseUserPermit");
            TempData["id_warehouseUserPermit"] = id_warehouseUserPermitDetail;
            TempData.Keep("id_warehouseUserPermit");

            ViewData["ShowCodeWarehouse"] = true;

            return PartialView("_WarehouseUserPermitDetailPartial", model);
        }

        private UserEntity ObtainWarehouseUserPermit(int? id_warehouseUserPermit)
        {

            UserEntity userEntity = (TempData["warehouseUserPermit"] as UserEntity);

            userEntity = userEntity ?? db.UserEntity.FirstOrDefault(i => i.id == id_warehouseUserPermit);
            userEntity = userEntity ?? new UserEntity();

            return userEntity;

        }

        /////* ADD */
        [HttpPost, ValidateInput(false)]
        public ActionResult WarehouseUserPermitDetailAddNew(int id_warehouseUserPermit, UserEntityDetail userEntityDetail)
        {

            UserEntity userEntity = ObtainWarehouseUserPermit(id_warehouseUserPermit);


            if (ModelState.IsValid)
            {
                try
                {
                    userEntityDetail.id = userEntity.UserEntityDetail.Count() > 0 ? userEntity.UserEntityDetail.Max(pld => pld.id) + 1 : 1;
                    var lengtDetail = userEntity.UserEntityDetail.Count() + 1;

                    userEntity.UserEntityDetail.Add(userEntityDetail);

                    int[] permisos = new int[8] { 12, 11, 10, 2, 4, 5, 6, 1 };
                    int i = 0;
                    int j = lengtDetail;

                    while (j <= lengtDetail)
                    {
                        while (i < 8)
                        {
                            var tempwarehouseUserPermitDetailPermision = new UserEntityDetailPermission
                            {
                                id_userEntityDetail = userEntityDetail.id,
                                id_permission = permisos[i]
                            };

                            userEntityDetail.UserEntityDetailPermission.Add(tempwarehouseUserPermitDetailPermision);

                            i++;
                        }
                        j++;

                    }
                   
                    TempData["warehouseUserPermit"] = userEntity;
                    TempData["id_warehouseUserPermitDetail"] = userEntityDetail;
                    TempData.Keep("warehouseUserPermit");
                    TempData.Keep("id_warehouseUserPermitDetail");
                    TempData.Keep("warehouseUserPermission");

                    ViewData["ShowCodeWarehouse"] = true;
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Por favor, corrija todos los errores.";
            }

            var model = userEntity.UserEntityDetail;
            if (model != null)
            {
                model = model.ToList();
            }
            else
            {
                model = new List<UserEntityDetail>();
            }

            return PartialView("_WarehouseUserPermitDetailPartial", model);
        }

        //Delete

        [HttpPost, ValidateInput(false)]
        public ActionResult WarehouseUserPermitDetailDelete(int id_warehouseUserPermit, int id)
        {
            UserEntity usuarioEntidad = ObtainWarehouseUserPermit(id_warehouseUserPermit);

            List<UserEntityDetail> userEntityDetails = DetailUpdateDelete(usuarioEntidad, id);
            //TempData.Keep("warehouseUserPermit");
            //TempData.Keep("id_warehouseUserPermitDetail");

            ViewData["ShowCodeWarehouse"] = true;

            return PartialView("_WarehouseUserPermitDetailPartial", userEntityDetails);
           
        }

        private List<UserEntityDetail> DetailUpdateDelete(UserEntity userEntityDetail, int id_userEntity)
        {
            var modelUserEntity = db.UserEntityDetailPermission;
            var modelUser = db.UserEntityDetail;
            var user = db.UserEntityDetail.FirstOrDefault(e => e.id == id_userEntity);

            if (ModelState.IsValid && userEntityDetail != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelWarehouseDetailDetail = userEntityDetail.UserEntityDetail.FirstOrDefault(i => i.id == id_userEntity);
                        if (modelWarehouseDetailDetail != null)
                        {
                            var item = modelUserEntity.Where(it => it.id_userEntityDetail == user.id).ToList();

                            for (var i = 0; i < item.Count(); i++)
                            {
                                modelUserEntity.Remove(item[i]);
                            }

                            var item2 = modelUser.Where(e => e.id == id_userEntity).FirstOrDefault();

                            modelUser.Remove(item2);

                            db.SaveChanges();
                            trans.Commit();

                        }
                        userEntityDetail = modelUser.FirstOrDefault(e => e.id_userEntity == userEntityDetail.id).UserEntity;
                        TempData["warehouseUserPermit"] = userEntityDetail;
                        //TempData["id_warehouseUserPermitDetail"] = modelUser;
                        //TempData.Keep("warehouseUserPermit");
                        //TempData.Keep("id_warehouseUserPermitDetail");

                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                    }
                }
                
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";

            List<UserEntityDetail> model = modelUser?.Where(x => x.id_userEntity == user.id_userEntity).ToList() ?? new List<UserEntityDetail>();
            model = (model.Count() != 0) ? model : new List<UserEntityDetail>();

            return model;

        }


        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedDetail(int? id_bodega, int? id_user)
        {
            UserEntity codeWarehouse = (TempData["warehouseUserPermit"] as UserEntity);
            codeWarehouse = codeWarehouse ?? new UserEntity();
            var userEntity = db.UserEntity.Where(e => e.id_user == id_user).Select(e => e.id).ToList();
            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            for (int i = 0; i < userEntity.Count(); i++)
            {
                int index = userEntity[i];
                var detail = db.UserEntityDetail.Where(e => e.id_userEntity == index && e.id_entityValue == id_bodega).ToList();
                if(detail != null && detail.Count() > 0)
                {
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "Usuario con permiso a Bodega ya existe."
                    };
                }
            }

            
            var codeWarehouseAux = codeWarehouse.UserEntityDetail.FirstOrDefault(fod => fod.id_entityValue == id_bodega);
            if (codeWarehouseAux != null)
            {
                result = new
                {
                    itsRepeated = 1,
                    Error = "No se puede repetir la Bodega en Detalles."
                };
            }

            TempData["warehouseUserPermit"] = codeWarehouse;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedUser(int? id_user)
        {
            UserEntity codeWarehouse = (TempData["warehouseUserPermit"] as UserEntity);
            codeWarehouse = codeWarehouse ?? new UserEntity();
            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            var userEntity = db.UserEntity.Where(e => e.id_user == id_user).Select(e => e.id).ToList();

            for (int i = 0; i < userEntity.Count(); i++)
            {
                int index = userEntity[i];
                result = new
                {
                    itsRepeated = 1,
                    Error = "Usuario ya se encuentra registrado."
                };
            }

            TempData["warehouseUserPermit"] = codeWarehouse;

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CodeUserChangeData(int? codeUser)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var db = new DBContext();
            var codeAccount = db.User.FirstOrDefault(t => t.id == codeUser);

            if (codeAccount == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                codeUsuario = codeAccount.username
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult NameWarehouseChangeData(int? idBodega)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //codeAccount
            var dbCI = new DBContext();
            var codeAccount = dbCI.Warehouse.FirstOrDefault(e => e.id == idBodega);

            if (codeAccount == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                nameWarehouse = codeAccount.code
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UserEmployeeChangeData(int id_usuario)
        {

            var codEmploye = db.User.FirstOrDefault(e => e.id == id_usuario);

            var personEmploye = db.Person.FirstOrDefault(q => q.id == codEmploye.id_employee);

            var objeto = new
            {
                id = id_usuario,
                fullname_businessName = personEmploye?.fullname_businessName,
            };

            var result = new[]
            {
                objeto
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
