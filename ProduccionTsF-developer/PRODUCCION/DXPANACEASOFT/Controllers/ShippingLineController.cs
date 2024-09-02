using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;
namespace DXPANACEASOFT.Controllers
{
    public class ShippingLineController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult ShippingLineResults(ShippingLine ShippingLine,
                                                  string scode
                                                  )
        {
            var model = db.ShippingLine.ToList();

            #region  FILTERS
            if (!string.IsNullOrEmpty(scode))
            {
                model = model.Where(o => o.code == scode).ToList();
            }


            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_ShippingLineResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult ShippingLinePartial()
        {
            var model = (TempData["model"] as List<ShippingLine>);

            model = db.ShippingLine.ToList();

            model = model ?? new List<ShippingLine>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_ShippingLinePartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit ShippingLine
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditShippingLine(int id, int[] orderDetails)
        {
            ShippingLine ShippingLine = db.ShippingLine.Where(o => o.id == id).FirstOrDefault();

            if (ShippingLine == null)
            {

                ShippingLine = new ShippingLine
                {

                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now,
                    isActive = true,
                };
            }
            TempData["ShippingLine"] = ShippingLine;
            TempData.Keep("ShippingLine");

            return PartialView("_FormEditShippingLine", ShippingLine);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_ShippingLine)
        {
            TempData.Keep("ShippingLine");
            int index = db.ShippingLine.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_ShippingLine);
            var result = new
            {
                maximunPages = db.ShippingLine.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id_ShippingLine, string codio)
        {
            TempData.Keep("ShippingLine");


            bool rept = false;

            var cantre = db.ShippingLine.Where(x => x.id != id_ShippingLine && x.code == codio).ToList().Count();
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
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            ShippingLine ShippingLine = db.ShippingLine.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (ShippingLine != null)
            {
                TempData["ShippingLine"] = ShippingLine;
                TempData.Keep("ShippingLine");
                return PartialView("_ShippingLineMainFormPartial", ShippingLine);
            }

            TempData.Keep("ShippingLine");

            return PartialView("_ShippingLineMainFormPartial", new ShippingLine());
        }
        #endregion
        #region Validacion
        public Boolean validate(ShippingLine item)
        {
            Boolean wsreturn = true;
            try
            {
        
                if (String.IsNullOrEmpty(item.name))
                {
                    TempData.Keep("ShippingLine");
                    ViewData["EditMessage"] = ErrorMessage("Ingrse el Nombre del Linea");

                    wsreturn = false;
                }

                if (String.IsNullOrEmpty(item.code))
                {
                    TempData.Keep("ShippingLine");
                    ViewData["EditMessage"] = ErrorMessage("Ingrse el Codigo del Linea");

                    wsreturn = false;
                }
                else
                {


                    if (item.code.ToString().ToString().Contains(" ") && item.code.ToString().ToString().Length > 0)
                    {
                        TempData.Keep("ShippingLine");
                        ViewData["EditMessage"] = ErrorMessage("El Codigo de la Linea no puede tener espacio");

                        wsreturn = false;
                    }
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
        public ActionResult ShippingLinePartialAddNew(ShippingLine item)
        {
            ShippingLine conShippingLine = (TempData["ShippingLine"] as ShippingLine);

            if (!validate(item))
            {
 

                return PartialView("_ShippingLinePartial", db.ShippingLine.ToList());
            }
            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region ShippingLine

                    #endregion


                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;



                    dbemp.ShippingLine.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    TempData["ShippingLine"] = item;
                    TempData.Keep("ShippingLine");
                    ViewData["EditMessage"] = SuccessMessage("Linea: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("ShippingLine");
                    item = (TempData["ShippingLine"] as ShippingLine);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                    trans.Rollback();
                }
            }



            //return PartialView("_ShippingLineMainFormPartial", item);

            return PartialView("_ShippingLinePartial", dbemp.ShippingLine.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ShippingLinePartialUpdate(ShippingLine item)
        {
            ShippingLine modelItem = db.ShippingLine.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                ShippingLine conShippingLine = (TempData["ShippingLine"] as ShippingLine);


                if (!validate(item))
                {

          
                    return PartialView("_ShippingLinePartial", db.ShippingLine.ToList());
                }



                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region ShippingLine
                        modelItem.code = item.code;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.name = item.name;
                        modelItem.isActive = item.isActive;



                        #endregion




                        db.ShippingLine.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["ShippingLine"] = modelItem;
                        TempData.Keep("ShippingLine");
                        ViewData["EditMessage"] = SuccessMessage("Linea: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("ShippingLine");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);

                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("ShippingLine");


            return PartialView("_ShippingLinePartial", db.ShippingLine.ToList());
        }
        #endregion
        #region ShippingLine Gridview

        [ValidateInput(false)]
        public ActionResult ShippingLinePartial(int? id)
        {
            if (id != null)
            {
                ViewData["ShippingLineToCopy"] = db.ShippingLine.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.ShippingLine.ToList();
            return PartialView("_ShippingLinePartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ShippingLinePartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.ShippingLine.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.ShippingLine.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Linea : " + (item?.name ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.ShippingLine.ToList();
            return PartialView("_ShippingLinePartial", model.ToList());
        }

        public ActionResult DeleteSelectedShippingLine(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var ShippingLines = db.ShippingLine.Where(i => ids.Contains(i.id));
                        foreach (var vShippingLine in ShippingLines)
                        {
                            vShippingLine.isActive = false;

                            vShippingLine.id_userUpdate = ActiveUser.id;
                            vShippingLine.dateUpdate = DateTime.Now;

                            db.ShippingLine.Attach(vShippingLine);
                            db.Entry(vShippingLine).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Linea desactivadas exitosamente");
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

            var model = db.ShippingLine.ToList();
            return PartialView("_ShippingLinePartial", model.ToList());
        }

        #endregion
    }
}
