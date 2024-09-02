using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;

namespace DXPANACEASOFT.Controllers
{
    public class PortController : DefaultController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult PortResults(Port Port
                                                  )
        {

            var model = db.Port.ToList();

            #region  FILTERS



            #endregion
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_PortResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region  HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult PortPartial()
        {
            var model = (TempData["model"] as List<Port>);

            model = db.Port.ToList();

            model = model ?? new List<Port>();
            TempData["model"] = model;
            TempData.Keep("model");
            return PartialView("_PortPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion
        #region Edit Port
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditPort(int id, int[] orderDetails)
        {
            Port Port = db.Port.Where(o => o.id == id).FirstOrDefault();

            if (Port == null)
            {

                Port = new Port
                {

                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now

                };
            }
            TempData["Port"] = Port;
            TempData.Keep("Port");

            return PartialView("_FormEditPort", Port);
        }
        #endregion
        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_Port)
        {
            TempData.Keep("Port");
            int index = db.Port.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_Port);
            var result = new
            {
                maximunPages = db.Port.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            Port Port = db.Port.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (Port != null)
            {
                TempData["Port"] = Port;
                TempData.Keep("Port");
                return PartialView("_PortMainFormPartial", Port);
            }

            TempData.Keep("Port");

            return PartialView("_PortMainFormPartial", new Port());
        }
        #endregion
        #region Validacion
        [HttpPost, ValidateInput(false)]
        public JsonResult ReptCodigo(int id_Port, string codio)
        {
            TempData.Keep("Port");


            bool rept = false;

            var cantre = db.Port.Where(x => x.id != id_Port && x.code == codio).ToList().Count();
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
        public ActionResult PortPartialAddNew(Port item)
        {
            Port conPort = (TempData["Port"] as Port);


            DBContext dbemp = new DBContext();

            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region Port

                    #endregion


                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;

                

                    dbemp.Port.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    TempData["Port"] = item;
                    TempData.Keep("Port");
                    ViewData["EditMessage"] = SuccessMessage("Puerto: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("Port");
                    item = (TempData["Port"] as Port);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);

                    trans.Rollback();
                }
            }



            //return PartialView("_PortMainFormPartial", item);

            return PartialView("_PortPartial", dbemp.Port.ToList());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult PortPartialUpdate(Port item)
        {
            Port modelItem = db.Port.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                Port conPort = (TempData["Port"] as Port);





                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region Port
               
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.id_city = item.id_city;
                        modelItem.code = item.code;
                        modelItem.isActive = item.isActive;
						modelItem.id_portType = item.id_portType;

						modelItem.nombre = item.nombre;
                        modelItem.transitDays = item.transitDays;


                        #endregion




                        db.Port.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["Port"] = modelItem;
                        TempData.Keep("Port");
                        ViewData["EditMessage"] = SuccessMessage("Puerto: " + modelItem.id + " guardada exitosamente");
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("Port");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);

                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("Port");


            return PartialView("_PortPartial", db.Port.ToList());
        }
        #endregion
        #region Port Gridview

        [ValidateInput(false)]
        public ActionResult PortPartial(int? id)
        {
            if (id != null)
            {
                ViewData["PortToCopy"] = db.Port.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.Port.ToList();
            return PartialView("_PortPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PortPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Port.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.Port.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Puerto : " + (item?.id.ToString() ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.Port.ToList();
            return PartialView("_PortPartial", model.ToList());
        }

        public ActionResult DeleteSelectedPort(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Ports = db.Port.Where(i => ids.Contains(i.id));
                        foreach (var vPort in Ports)
                        {


                            vPort.id_userUpdate = ActiveUser.id;
                            vPort.dateUpdate = DateTime.Now;

                            db.Port.Attach(vPort);
                            db.Entry(vPort).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Puerto desactivadas exitosamente");
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

            var model = db.Port.ToList();
            return PartialView("_PortPartial", model.ToList());
        }

        #endregion
    }
}
