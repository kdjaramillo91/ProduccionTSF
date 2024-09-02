using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using Excel = Microsoft.Office.Interop.Excel;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class TypeMachineForProdController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region TypeMachineForProd GRIDVIEW

        [HttpPost, ValidateInput(false)]
        public ActionResult TypeMachineForProdPartial(int? keyToCopy)
        {
            if (keyToCopy != null)
            {
                ViewData["rowToCopy"] = db.tbsysTypeMachineForProd.FirstOrDefault(b => b.id == keyToCopy);
            }
            var model = db.tbsysTypeMachineForProd;
            return PartialView("_TypeMachineForProdPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TypeMachineForProdPartialAddNew(tbsysTypeMachineForProd item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.tbsysTypeMachineForProd.Add(item);
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Tipo de Máquina: " + item.name + " guardado exitosamente");
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

            var model = db.tbsysTypeMachineForProd;
            return PartialView("_TypeMachineForProdPartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult TypeMachineForProdPartialUpdate(tbsysTypeMachineForProd item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.tbsysTypeMachineForProd.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {

                            modelItem.code = item.code;
                            modelItem.name = item.name;
							modelItem.id_Rol = item.id_Rol;
                            modelItem.isActive = item.isActive;

                            db.tbsysTypeMachineForProd.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();

                            ViewData["EditMessage"] = SuccessMessage("Tipo de Máquina: " + item.name + " guardado exitosamente");
                        }
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

            var model = db.tbsysTypeMachineForProd;
            return PartialView("_TypeMachineForProdPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TypeMachineForProdPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.tbsysTypeMachineForProd.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                                item.isActive = false;

                                db.tbsysTypeMachineForProd.Attach(item);
                                db.Entry(item).State = EntityState.Modified;

                                db.SaveChanges();
                                trans.Commit();


                            ViewData["EditMessage"] = SuccessMessage("Tipo Máquina: " + (item?.name ?? "") + " desactivado exitosamente");
                        }
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.tbsysTypeMachineForProd;
            return PartialView("_TypeMachineForProdPartial", model.ToList());
        }

        [HttpPost]
        public ActionResult DeleteSelectedTypeMachineForProd(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.tbsysTypeMachineForProd.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            db.tbsysTypeMachineForProd.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Tipos de Máquina desactivados exitosamente");
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

            var model = db.tbsysTypeMachineForProd;
            return PartialView("_TypeMachineForProdPartial", model.ToList());
        }
        #endregion

        #region AUXILIAR FUNCTIONS
        [HttpPost]
        public JsonResult ValidateCodeTypeMachineForProd(int id_tbsysTypeMachineForProd, string code)
        {
            tbsysTypeMachineForProd tbsysTypeMachineForProd = db.tbsysTypeMachineForProd.FirstOrDefault(b => b.code == code
                                                                            && b.id != id_tbsysTypeMachineForProd);

            if (tbsysTypeMachineForProd == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otro Tipo de Máquina." }, JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}



