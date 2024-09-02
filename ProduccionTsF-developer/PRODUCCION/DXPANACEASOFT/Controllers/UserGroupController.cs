using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.XtraPrinting.Native;
using DXPANACEASOFT.Models;

namespace DXPANACEASOFT.Controllers
{
    public class UserGroupController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region USERGROUP EDITFORM

        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditUserGroup(int id)
        {
            UserGroup group = db.UserGroup.FirstOrDefault(g => g.id == id);
            group = group ?? new UserGroup
            {
                isActive = true
            };

            List<UserGroupMenu> menuList = group?.UserGroupMenu?.ToList() ?? new List<UserGroupMenu>();

            List<AssignedMenu> menues = db.Menu.Select(m => new AssignedMenu
            {
                id = m.id,
                id_parent = m.id_parent,
                title = m.title,
                position = m.position,
                isActive = m.isActive,
                id_controller = m.id_controller,
                id_action = m.id_action,
                isAssigned = false

            }).OrderBy(m => m.id_parent).ToList().OrderBy(m => m.position).ToList();

            foreach (var menu in menues)
            {
                menu.isAssigned = menuList.Select(x => x.id_menu).Contains(menu.id);
                menu.Permissions = menuList.FirstOrDefault(x => x.id_menu == menu.id)?.Permission?.ToList() ?? db.Permission.ToList();
            }

            TempData["menues"] = menues;
            TempData.Keep("menues");

            return PartialView("_UserGroupFormEditPartial", group);
        }

        #endregion

        #region USERGROUP HEADER

        [HttpPost, ValidateInput(false)]
        public ActionResult UserGroupsPartial()
        {
            var model = db.UserGroup;
            return PartialView("_UserGroupsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UserGroupsPartialAddNew(DXPANACEASOFT.Models.UserGroup item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        List<AssignedMenu> menues = (TempData["menues"] as List<AssignedMenu>);

                        if(menues != null)
                        {
                            item.UserGroupMenu = new List<UserGroupMenu>();
                            foreach (var detail in menues)
                            {
                                if (detail.isAssigned)
                                {
                                    Menu menu = db.Menu.FirstOrDefault(m => m.id == detail.id);

                                    UserGroupMenu userGroupMenu = new UserGroupMenu
                                    {
                                        Menu = menu
                                    };

                                    foreach (var p in detail.Permissions)
                                    {
                                        Permission permission = db.Permission.FirstOrDefault(x => x.id == p.id);
                                        userGroupMenu.Permission.Add(permission);
                                    }

                                    item.UserGroupMenu.Add(userGroupMenu);
                                }
                            }
                        }

                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.UserGroup.Add(item);
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
            return PartialView("Index");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UserGroupsPartialUpdate(DXPANACEASOFT.Models.UserGroup item)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.UserGroup.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.name = item.name;
                            modelItem.description = item.description;
                            modelItem.isActive = item.isActive;

                            for(int i = modelItem.UserGroupMenu.Count - 1; i >= 0; i--)
                            {
                                db.UserGroupMenu.Remove(modelItem.UserGroupMenu.ElementAt(i));
                            }

                            List<AssignedMenu> menues = (TempData["menues"] as List<AssignedMenu>);

                            if (menues != null)
                            {
                                item.UserGroupMenu = new List<UserGroupMenu>();
                                foreach (var detail in menues)
                                {
                                    if (detail.isAssigned)
                                    {
                                        Menu menu = db.Menu.FirstOrDefault(m => m.id == detail.id);
                                        UserGroupMenu userGroupMenu = new UserGroupMenu
                                        {
                                            Menu = menu
                                        };

                                        foreach (var permission in detail.Permissions)
                                        {
                                            Permission p = db.Permission.FirstOrDefault(x => x.id == permission.id);
                                            if(p != null)
                                            {
                                                userGroupMenu.Permission.Add(p);
                                            }
                                            
                                        }

                                        modelItem.UserGroupMenu.Add(userGroupMenu);
                                    }
                                }
                            }

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.UserGroup.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

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
            return PartialView("Index");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UserGroupsPartialDelete(System.Int32 id)
        {
            var model = db.UserGroup;
            if (id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.id == id);
                    if (item != null)
                        model.Remove(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_UserGroupsPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public void DeleteSelectedUserGroups(int [] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelItem = db.UserGroup.Where(i => ids.Contains(i.id));
                        foreach (var item in modelItem)
                        {
                            item.isActive = false;

                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                            db.UserGroup.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                        }

                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                    }
                }
            }
        }

        #endregion

        #region MENU TREELIST

        [HttpPost, ValidateInput(false)]
        public ActionResult UserGroupMenuTreeViewPartial()
        {
            List<AssignedMenu> menues = (TempData["menues"] as List<AssignedMenu>);
            TempData.Keep("menues");
            return PartialView("_UserGroupMenuTreeViewPartial", menues);
        }

        [HttpPost, ValidateInput(false)]
        public void UpdateMenuPermissions(AssignedMenu item, string [] permissions)
        {
            List<AssignedMenu> menues = (TempData["menues"] as List<AssignedMenu>);

            if(menues != null)
            {
                AssignedMenu menu = menues.FirstOrDefault(m => m.id == item.id);

                int[] ids = new int[] {};
                if(permissions.Length > 0 && !string.IsNullOrEmpty(permissions[0]))
                {
                    ids = Array.ConvertAll(permissions, int.Parse);
                    //ids = permissions.ConvertAll(Int32.Parse).ToArray();
                }
                
                if (menu != null)
                {
                    menu.Permissions = new List<Permission>();
                    List<Permission> items = db.Permission.Where(p => ids.Contains(p.id)).ToList();
                    foreach(var i in items)
                    {
                        menu.Permissions.Add(i);
                    }
                }
            }

            TempData["menues"] = menues;
            TempData.Keep("menues");
        }

        #endregion

        #region AUXILIAR FUNCTIONS

        [HttpPost]
        public void UpdateMenuSelection(int [] ids)
        {
            List<AssignedMenu> menues = (TempData["menues"] as List<AssignedMenu>);
            
            if(menues != null)
            {
                foreach (AssignedMenu m in menues)
                {
                    m.isAssigned = false;
                }
            }

            if(menues != null && ids != null)
            {
                foreach (var id in ids)
                {
                    AssignedMenu menu = menues.FirstOrDefault(m => m.id == id);
                    if(menu != null)
                    {
                        menu.isAssigned = true;
                    }

                    int? id_parent = menu?.id_parent ?? null;
                    while(id_parent != null)
                    {
                        AssignedMenu parent = menues.FirstOrDefault(m => m.id == id_parent);
                        if(parent != null)
                        {
                            parent.isAssigned = true;
                            id_parent = parent.id_parent;
                        }
                    }
                }
            }

            TempData["menues"] = menues;
            TempData.Keep("menues");
        }

        #endregion
    }
}