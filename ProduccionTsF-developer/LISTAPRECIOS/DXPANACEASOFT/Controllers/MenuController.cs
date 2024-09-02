using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Controllers
{
    public class MenuController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult MenuTreeListPartial()
        {
            var model = db.Menu.Where(p => p.isActive).OrderBy(m => m.id_parent).ToList().OrderBy(m => m.position);
            return PartialView("_MenuTreeListPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MenuTreeListPartialAddNew(DXPANACEASOFT.Models.Menu item)
        {   
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        Menu parent = db.Menu.FirstOrDefault(p => p.id == item.id_parent);

                        if(parent != null)
                        {
                            if(parent.Menu1.Count > 0)
                            {
                                Menu lastMenu = parent.Menu1.OrderBy(p => p.position).Last();
                                item.position = lastMenu.position + 1;
                            }
                            else
                            {
                                item.position = 1;
                            }
                        }
                        else
                        {
                            List<Menu> roots = db.Menu.Where(p => p.id_parent == null).OrderBy(p => p.position).ToList();
                            if(roots.Count > 0)
                            {
                                item.position = roots.OrderBy(p => p.position).Last().position + 1;
                            }
                            else
                            {
                                item.position = 1;
                            }
                        }

                        item.id_company = ActiveCompany.id;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;
                        item.code = "";

                        List<Menu> menues = db.Menu.Where(m => m.id_parent == item.id_parent &&  m.position >= item.position).ToList();
                        if(menues?.Count > 0)
                        {
                            foreach (var m in menues)
                            {
                                m.position++;
                                db.Menu.Attach(m);
                                db.Entry(m).State = EntityState.Modified;
                            }
                        }

                        db.Menu.Add(item);
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            var model = db.Menu.Where(p => p.isActive).OrderBy(m => m.id_parent).ToList().OrderBy(m => m.position);
            return PartialView("_MenuTreeListPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MenuTreeListPartialUpdate(DXPANACEASOFT.Models.Menu item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.Menu.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                            item.code = "";
                            if(item.position > modelItem.position)
                            {
                                List<Menu> menues = db.Menu.Where(m => m.id_parent == item.id_parent && (m.position >= item.position && m.position <= item.position)).ToList();
                                if (menues?.Count > 0)
                                {
                                    foreach (var m in menues)
                                    {
                                        m.position--;
                                        db.Menu.Attach(m);
                                        db.Entry(m).State = EntityState.Modified;
                                    }
                                }
                            }
                            else
                            {
                                List<Menu> menues = db.Menu.Where(m => m.id_parent == item.id_parent && m.position >= item.position).ToList();
                                if (menues?.Count > 0)
                                {
                                    foreach (var m in menues)
                                    {
                                        m.position++;
                                        db.Menu.Attach(m);
                                        db.Entry(m).State = EntityState.Modified;
                                    }
                                }
                            }
                            

                            this.UpdateModel(modelItem);
                            db.SaveChanges();
                            trans.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
                
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            var model = db.Menu.Where(p => p.isActive).OrderBy(m => m.id_parent).ToList().OrderBy(m => m.position);
            return PartialView("_MenuTreeListPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MenuTreeListPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.Menu.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;
                        }
                            
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }

            var model = db.Menu.Where(p => p.isActive).OrderBy(m => m.id_parent).ToList().OrderBy(m => m.position);
            return PartialView("_MenuTreeListPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MenuTreeListPartialMove(System.Int32 id, System.Int32? id_parent)
        {
            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    var item = db.Menu.FirstOrDefault(it => it.id == id);

                    if (item != null)
                    {
                        item.id_parent = id_parent;

                        List<Menu> menues = db.Menu.Where(m => m.id_parent == item.id_parent).ToList();
                        if (menues?.Count > 0)
                        {
                            item.position = menues.OrderBy(m => m.position).ToList().Last().position + 1;
                        }
                        else
                        {
                            item.position = 1;
                        }

                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;
                    }
                    
                    db.Menu.Attach(item);
                    db.Entry(item).State = EntityState.Modified;

                    db.SaveChanges();
                    trans.Commit();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }

            var model = db.Menu.Where(p => p.isActive).OrderBy(m => m.id_parent).ToList().OrderBy(m => m.position);
            return PartialView("_MenuTreeListPartial", model.ToList());
        }

        #region AUXILIAR FUNCTIONS

        [HttpPost, ValidateInput(false)]
        public JsonResult GetActionsByController(int id_controller)
        {
            var result = db.TAction.Where(a => a.id_controller == id_controller).Select(a => new
            {
                a.id,
                a.name
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult SearchMenu(string query)
        {
            User user = db.User.FirstOrDefault(u => u.id == ActiveUser.id);

            var suggestions = user.UserMenu.Where(m =>
                                                    //m.Menu.title.Contains(query) &&
                                                    (m.Menu.code.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                    m.Menu.title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) &&
                                                    m.Menu.id_controller != null &&
                                                    m.Menu.id_action != null)
                                   .Select(m => new
                                   {
                                       value = m.Menu.code + " - " + m.Menu.title,
                                       data = new { controller = m.Menu.TController.name, action = m.Menu.TAction.name }
                                   }).ToList();

            var result = new
            {
                query = query,
                suggestions = suggestions
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}