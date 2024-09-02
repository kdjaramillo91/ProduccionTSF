using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using DevExpress.XtraPrinting.Native;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;

namespace DXPANACEASOFT.Controllers
{
    public class UserController : DefaultController
    {
        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region USER EDITFORM

        [HttpPost, ValidateInput(false)]
        public ActionResult EditFromUserPartial(int id)
        {
            User user = db.User.FirstOrDefault(u => u.id == id);
            user = user ?? new User
            {
                isActive = true
            };

            #region USER MENU

            List<UserMenu> menuList = user?.UserMenu?.ToList() ?? new List<UserMenu>();

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

            #endregion

            #region USER EMISSION POINT

            user.EmissionPoint = user.EmissionPoint ?? new List<EmissionPoint>();

            #endregion

            #region USER PERMISSION OBJECTS

            user.ObjectPermissionUser = user.ObjectPermissionUser ?? new List<ObjectPermissionUser>();

            #endregion

            return PartialView("_EditFormUserPartial", user);
        }

        #endregion

        #region USER HEADER

        [ValidateInput(false)]
        public ActionResult UsersPartial()
        {
            var model = db.User;
            return PartialView("_UsersPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UsersPartialAddNew(User item, List<ObjectPermissionUser> aaaa)
        {
            User userAux = new User
            {
                isActive = true
            };
            var passwordEncryptMd5Base64 = EncryptMd5Base64(item.password);
            var emissionsPoints = (item != null && item.EmissionPoint != null && item.EmissionPoint.Count() > 0 && item.EmissionPoint.FirstOrDefault(fod => fod.name == null) == null) ? item.EmissionPoint.ToList() : new List<EmissionPoint>();
            userAux.username = item.username;
            userAux.password = passwordEncryptMd5Base64;
            userAux.id_employee = item.id_employee;
            userAux.Employee = db.Employee.FirstOrDefault(u => u.id == item.id_employee);
            userAux.id_group = item.id_group;
            userAux.UserGroup = db.UserGroup.FirstOrDefault(u => u.id == item.id_group);
            userAux.isActive = item.isActive;


            userAux.EmissionPoint = emissionsPoints;

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        User tempUser = TempData["user"] as User;
                        //MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                        //byte [] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(item.password));
                        //item.password = Convert.ToBase64String(hash);
                        item.password = passwordEncryptMd5Base64;

                        List<AssignedMenu> menues = (TempData["menues"] as List<AssignedMenu>);

                        if (menues != null)
                        {
                            item.UserMenu = new List<UserMenu>();
                            foreach (var detail in menues)
                            {
                                if (detail.isAssigned)
                                {
                                    Menu menu = db.Menu.FirstOrDefault(m => m.id == detail.id);

                                    UserMenu userMenu = new UserMenu
                                    {
                                        Menu = menu
                                    };

                                    foreach (var permission in detail.Permissions)
                                    {
                                        Permission p = db.Permission.FirstOrDefault(x => x.id == permission.id);
                                        if (p != null)
                                        {
                                            userMenu.Permission.Add(p);
                                        }

                                    }

                                    item.UserMenu.Add(userMenu);
                                }
                            }
                        }

                        #region Permiso de objetos

                        if (tempUser?.ObjectPermissionUser != null)
                        {
                            item.ObjectPermissionUser = new List<ObjectPermissionUser>();

                            var objectPermissionUsers = tempUser.ObjectPermissionUser.ToList();

                            foreach (var objectPermissionUser in objectPermissionUsers)
                            {
                                var tempObjectUser = new ObjectPermissionUser
                                {
                                    id_objectPemission = objectPermissionUser.id_objectPemission,
                                    id_user = item.id,
                                    id_controller = objectPermissionUser.id_controller,
                                    isActive = true,
                                    id_userCreate = this.ActiveUserId,
                                    dateCreate = DateTime.Now,
                                };
                                item.ObjectPermissionUser.Add(tempObjectUser);
                            }
                        }

                        #endregion

                        #region Punto de Emisión (EmissionPoint)

                        List<EmissionPoint> emissionPoints = new List<EmissionPoint>();
                        foreach (var emissionPoint in item.EmissionPoint)
                        {
                            EmissionPoint p = db.EmissionPoint.FirstOrDefault(e => e.id == emissionPoint.id);
                            if (p != null)
                            {
                                emissionPoints.Add(p);
                            }
                        }
                        item.EmissionPoint = emissionPoints;

                        #endregion

                        item.id_company = this.ActiveCompanyId;
                        item.id_userCreate = ActiveUser.id;
                        item.dateCreate = DateTime.Now;
                        item.id_userUpdate = ActiveUser.id;
                        item.dateUpdate = DateTime.Now;

                        db.User.Add(item);
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                        return PartialView("_EditFormUserPartial", userAux);
                    }
                }
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
                return PartialView("_EditFormUserPartial", userAux);//item);

            }
            return PartialView("Index");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UsersPartialUpdate(User item)
        {
            User userAux = db.User.FirstOrDefault(u => u.id == item.id);
            userAux = userAux ?? new User
            {
                isActive = true
            };

            var passwordEncryptMd5Base64 = userAux.password;
            if (userAux.password != item.password)
            {
                passwordEncryptMd5Base64 = EncryptMd5Base64(item.password);
            }
            var emissionsPoints = (item != null && item.EmissionPoint != null && item.EmissionPoint.Count() > 0 && item.EmissionPoint.FirstOrDefault(fod => fod.name == null) == null) ? item.EmissionPoint.ToList() : new List<EmissionPoint>();
            userAux.username = item.username;
            userAux.password = passwordEncryptMd5Base64;
            userAux.id_employee = item.id_employee;
            userAux.Employee = db.Employee.FirstOrDefault(u => u.id == item.id_employee);
            userAux.id_group = item.id_group;
            userAux.UserGroup = db.UserGroup.FirstOrDefault(u => u.id == item.id_group);
            userAux.isActive = item.isActive;

            userAux.EmissionPoint = emissionsPoints;

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        User tempUser = TempData["user"] as User;
                        var modelItem = db.User.FirstOrDefault(it => it.id == item.id);
                        if (modelItem != null)
                        {
                            modelItem.username = item.username;
                            modelItem.password = passwordEncryptMd5Base64;
                            modelItem.id_group = item.id_group;
                            modelItem.id_employee = item.id_employee;
                            modelItem.isActive = item.isActive;

                            #region USER MENU

                            for (int i = modelItem.UserMenu.Count - 1; i >= 0; i--)
                            {
                                db.UserMenu.Remove(modelItem.UserMenu.ElementAt(i));
                            }


                            List<AssignedMenu> menues = (TempData["menues"] as List<AssignedMenu>);

                            if (menues != null)
                            {
                                item.UserMenu = new List<UserMenu>();
                                foreach (var detail in menues)
                                {
                                    if (detail.isAssigned)
                                    {
                                        Menu menu = db.Menu.FirstOrDefault(m => m.id == detail.id);
                                        UserMenu userMenu = new UserMenu
                                        {
                                            Menu = menu
                                        };

                                        foreach (var permission in detail.Permissions)
                                        {
                                            Permission p = db.Permission.FirstOrDefault(x => x.id == permission.id);
                                            if (p != null)
                                            {
                                                userMenu.Permission.Add(p);
                                            }

                                        }

                                        modelItem.UserMenu.Add(userMenu);
                                    }
                                }
                            }

                            #endregion

                            #region USER EMISSION POINTS

                            for (int i = modelItem.EmissionPoint.Count - 1; i >= 0; i--)
                            {
                                EmissionPoint p = modelItem.EmissionPoint.ElementAt(i);

                                EmissionPoint emissionPoint = item.EmissionPoint.FirstOrDefault(e => e.id == p.id);
                                if (emissionPoint == null)
                                {
                                    modelItem.EmissionPoint.Remove(p);
                                }
                            }

                            foreach (var emissionPoint in item.EmissionPoint)
                            {
                                EmissionPoint p = modelItem.EmissionPoint.FirstOrDefault(e => e.id == emissionPoint.id);

                                if (p == null)
                                {
                                    EmissionPoint x = db.EmissionPoint.FirstOrDefault(t => t.id == emissionPoint.id);
                                    if (x != null)
                                    {
                                        modelItem.EmissionPoint.Add(x);
                                    }
                                }
                            }

                            #endregion

                            #region PERMISOS DE OBJETO

                            //if (tempUser?.ObjectPermissionUser != null)
                            //{
                            //    var objectPermissionUsers = db.ObjectPermissionUser.Where(e => e.id_user == modelItem.id);
                            //    foreach (var objectPermissionUser in objectPermissionUsers)
                            //    {
                            //        modelItem.ObjectPermissionUser.Remove(objectPermissionUser);
                            //        db.SaveChanges();
                            //    }

                            //    var userObjectPermissionUsers = tempUser?.ObjectPermissionUser.ToList();

                            //    foreach (var userObjectPermissionUser in userObjectPermissionUsers)
                            //    {
                            //        var tempObjectUser = new ObjectPermissionUser
                            //        {
                            //            id = db.ObjectPermissionUser.Count() > 0 ? db.ObjectPermissionUser.Max(m => m.id) + 1 : 1,
                            //            id = userObjectPermissionUser.id,
                            //            id_objectPemission = userObjectPermissionUser.id_objectPemission,
                            //            id_user = modelItem.id,
                            //            id_controller = userObjectPermissionUser.id_controller,
                            //            isActive = userObjectPermissionUser.isActive,
                            //            id_userCreate = userObjectPermissionUser.id_userCreate,
                            //            id_userUpdate = this.ActiveUserId,
                            //            dateCreate = userObjectPermissionUser.dateCreate,
                            //            dateUpdate = DateTime.Now,
                            //        };
                            //        modelItem.ObjectPermissionUser.Add(tempObjectUser);
                            //    }
                            //}

                            if(tempUser?.ObjectPermissionUser != null)
                            {
                                var objectUsers = db.ObjectPermissionUser.Where(e => e.id_user == modelItem.id);
                                foreach (var objectUser in objectUsers)
                                {
                                    db.ObjectPermissionUser.Remove(objectUser);
                                    db.Entry(objectUser).State = EntityState.Deleted;
                                }

                                var objectPermissionUsers = tempUser.ObjectPermissionUser.ToList();

                                foreach (var objectPermissionUser in objectPermissionUsers)
                                {
                                    var tempObjectUser = new ObjectPermissionUser()
                                    {
                                        id_user = modelItem.id,
                                        id_controller = objectPermissionUser.id_controller,
                                        id_objectPemission = objectPermissionUser.id_objectPemission,
                                        isActive = objectPermissionUser.isActive,
                                        id_userCreate = objectPermissionUser.id_userCreate,
                                        id_userUpdate = objectPermissionUser.id_userUpdate,
                                        dateCreate = objectPermissionUser.dateCreate,
                                        dateUpdate = objectPermissionUser.dateUpdate,                                        
                                    };

                                    db.ObjectPermissionUser.Add(tempObjectUser);
                                    db.Entry(tempObjectUser).State = EntityState.Added;
                                }
                            }

                            #endregion

                            modelItem.id_userUpdate = ActiveUser.id;
                            modelItem.dateUpdate = DateTime.Now;

                            db.User.Attach(modelItem);
                            db.Entry(modelItem).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        ViewData["EditError"] = !string.IsNullOrEmpty(e.InnerException?.Message) ? e.InnerException?.Message : e.Message;
                        trans.Rollback();
                        return PartialView("_EditFormUserPartial", userAux);
                    }
                }
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";



                return PartialView("_EditFormUserPartial", userAux);

            }
            return PartialView("Index");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UsersPartialDelete(System.Int32 id)
        {
            var model = db.User;
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
            return PartialView("_UsersPartial", model.ToList());
        }

        #endregion

        #region USER MENU TREE LIST

        [HttpPost, ValidateInput(false)]
        public ActionResult UserMenuTreeViewPartial()
        {
            List<AssignedMenu> menues = (TempData["menues"] as List<AssignedMenu>);
            TempData.Keep("menues");

            return PartialView("_UserMenuTreeViewPartial", menues);
        }

        [HttpPost, ValidateInput(false)]
        public void UpdateMenuPermissions(AssignedMenu item, string[] permissions)
        {
            List<AssignedMenu> menues = (TempData["menues"] as List<AssignedMenu>);

            if (menues != null)
            {
                AssignedMenu menu = menues.FirstOrDefault(m => m.id == item.id);

                int[] ids = new int[] { };
                if (permissions.Length > 0 && !string.IsNullOrEmpty(permissions[0]))
                {
                    ids = Array.ConvertAll(permissions, int.Parse);
                    //ids = permissions.ConvertAll(Int32.Parse).ToArray<int>();
                }

                if (menu != null)
                {
                    menu.Permissions = new List<Permission>();
                    List<Permission> items = db.Permission.Where(p => ids.Contains(p.id)).ToList();
                    foreach (var i in items)
                    {
                        menu.Permissions.Add(i);
                    }
                }
            }

            TempData["menues"] = menues;
            TempData.Keep("menues");
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CallbackPanelPartial()
        {
            return PartialView("_CallbackPanelPartial");
        }

        #endregion

        #region USER EMISSION POINTS

        //[ValidateInput(false)]
        //public ActionResult UserEmissionPointsPartial()
        //{
        //    var model = db.UserCompanyInformation;
        //    return PartialView("_UserEmissionPointsPartial", model.ToList());
        //}

        //[HttpPost, ValidateInput(false)]
        //public ActionResult UserEmissionPointsPartialAddNew(DXPANACEASOFT.Models.UserCompanyInformation item)
        //{
        //    var model = db.UserCompanyInformation;
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            model.Add(item);
        //            db.SaveChanges();
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    else
        //        ViewData["EditError"] = "Please, correct all errors.";
        //    return PartialView("_UserEmissionPointsPartial", model.ToList());
        //}

        //[HttpPost, ValidateInput(false)]
        //public ActionResult UserEmissionPointsPartialUpdate(DXPANACEASOFT.Models.UserCompanyInformation item)
        //{
        //    var model = db.UserCompanyInformation;
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var modelItem = model.FirstOrDefault(it => it.id_emissionPoint == item.id_emissionPoint);
        //            if (modelItem != null)
        //            {
        //                this.UpdateModel(modelItem);
        //                db.SaveChanges();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    else
        //        ViewData["EditError"] = "Please, correct all errors.";
        //    return PartialView("_UserEmissionPointsPartial", model.ToList());
        //}

        //[HttpPost, ValidateInput(false)]
        //public ActionResult UserEmissionPointsPartialDelete(System.Int32 id_emissionPoint)
        //{
        //    var model = db.UserCompanyInformation;
        //    if (id_emissionPoint >= 0)
        //    {
        //        try
        //        {
        //            var item = model.FirstOrDefault(it => it.id_emissionPoint == id_emissionPoint);
        //            if (item != null)
        //                model.Remove(item);
        //            db.SaveChanges();
        //        }
        //        catch (Exception e)
        //        {
        //            ViewData["EditError"] = e.Message;
        //        }
        //    }
        //    return PartialView("_UserEmissionPointsPartial", model.ToList());
        //}

        #endregion

        #region AUXILIAR FUNCTIONS

        private string EncryptMd5Base64(string passwordAux)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(passwordAux));
            passwordAux = Convert.ToBase64String(hash);
            return passwordAux;
        }

        [HttpPost, ValidateInput(false)]
        public void UpdateMenuSelection(int[] ids)
        {
            List<AssignedMenu> menues = (TempData["menues"] as List<AssignedMenu>);

            if (menues != null)
            {
                foreach (AssignedMenu m in menues)
                {
                    m.isAssigned = false;
                }
            }

            if (menues != null && ids != null)
            {
                foreach (var id in ids)
                {
                    AssignedMenu menu = menues.FirstOrDefault(m => m.id == id);
                    if (menu != null)
                    {
                        menu.isAssigned = true;
                    }

                    int? id_parent = menu?.id_parent ?? null;
                    while (id_parent != null)
                    {
                        AssignedMenu parent = menues.FirstOrDefault(m => m.id == id_parent);
                        if (parent != null)
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

        [HttpPost, ValidateInput(false)]
        public void UpdateMenuByGroup(int id_group)
        {
            UserGroup group = db.UserGroup.FirstOrDefault(g => g.id == id_group);
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
        }

        #endregion

        #region PERMISO DE OBJETOS

        [ValidateInput(false)]
        public ActionResult UserObjectPermissionPartial(int? id_user)
        {
            ObjectPermissionUser tempObjectPermissionUser = (TempData["objectPermissionUser"] as ObjectPermissionUser);
            if (tempObjectPermissionUser != null)
                TempData.Remove("objectPermissionUser");

            User user = GetUserCurrent(id_user.Value);

            var objectUser = user.ObjectPermissionUser.ToList();

            var model = objectUser ?? new List<ObjectPermissionUser>();

            return PartialView("_UserObjectEditGridViewPartial", model.OrderByDescending(e => e.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UserObjectPartialAddNew(int id_user, ObjectPermissionUser objectPermissionUser)
        {
            User user = GetUserCurrent(id_user);

            var objectUser = user.ObjectPermissionUser.ToList();
            user.ObjectPermissionUser = objectUser ?? new List<ObjectPermissionUser>();

            if (objectUser.Any(e => e.id_objectPemission == objectPermissionUser.id_objectPemission && e.id_controller == objectPermissionUser.id_controller))
            {
                throw new Exception("Permiso de objeto ya existente");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (objectPermissionUser != null)
                    {
                        var tmpobjectPermissionUser = new ObjectPermissionUser()
                        {
                            id = user.ObjectPermissionUser.Count() > 0 ? user.ObjectPermissionUser.Max(m => m.id) + 1 : 1,
                            id_objectPemission = objectPermissionUser.id_objectPemission,
                            id_user = objectPermissionUser.id_user,
                            id_controller = objectPermissionUser.id_controller,
                            isActive = objectPermissionUser.isActive,

                            // Auditoria 
                            id_userCreate = this.ActiveUserId,
                            id_userUpdate = 0,
                            dateCreate = DateTime.Now,
                            dateUpdate = DateTime.Now,
                        };
                        user.ObjectPermissionUser.Add(tmpobjectPermissionUser);
                        db.SaveChanges();

                        TempData["user"] = user;
                    }
                }
                catch (Exception ex)
                {
                    ViewData["EditError"] = ex.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Por favor, corrija todos los errores.";
            }

            TempData.Keep("user");

            var model = user.ObjectPermissionUser;

            return PartialView("_UserObjectEditGridViewPartial", model.OrderByDescending(e => e.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UserObjectPartialUpdate(int id_user, ObjectPermissionUser objectPermissionUser)
        {
            User user = GetUserCurrent(id_user);

            var objectUser = user.ObjectPermissionUser.ToList();
            var model = objectUser ?? new List<ObjectPermissionUser>();

            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = user.ObjectPermissionUser.FirstOrDefault(e => e.id == objectPermissionUser.id);
                    if (modelItem != null)
                    {
                        modelItem.id_userUpdate = this.ActiveUserId;
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.isActive = objectPermissionUser.isActive;
                        this.UpdateModel(modelItem);
                        db.SaveChanges();
                        TempData["user"] = user;
                    }
                }
                catch (Exception ex)
                {
                    ViewData["EditError"] = ex.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Por favor, corregir todos los errores.";
            }

            TempData.Keep("user");
            var modelList = user.ObjectPermissionUser;

            return PartialView("_UserObjectEditGridViewPartial", modelList.OrderByDescending(ob => ob.id).ToList());
        }

        [HttpPost, ValidateInput(false)]

        public ActionResult UserObjectPartialDelete(int id_user, int id)
        {
            User user = GetUserCurrent(id_user);

            try
            {
                if(user != null)
                {
                    var objectUser = user?.ObjectPermissionUser.FirstOrDefault(e => e.id == id);
                    if(objectUser != null)
                    {
                        user?.ObjectPermissionUser.Remove(objectUser);
                    }
                    TempData["user"] = user;
                }
            }
            catch (Exception ex)
            {
                ViewData["EditError"] = ex.InnerException != null ? ex.InnerException?.Message : ex.Message;
            }

            TempData.Keep("item");

            var model = user?.ObjectPermissionUser.ToList() ?? new List<ObjectPermissionUser>();

            return PartialView("_UserObjectEditGridViewPartial", model.OrderByDescending(ob => ob.id).ToList());
        }

        private User GetUserCurrent(int id_user)
        {
            User user = (TempData["user"] as User);
            user = user ?? db.User.FirstOrDefault(e => e.id == id_user);
            user = user ?? new User();

            TempData["user"] = user;
            TempData.Keep("user");

            return user;
        }

        public ObjectPermissionUser GetObjectPermisssionUser(int id_person, int id_objectPermissionUser)
        {
            User user = GetUserCurrent(id_person);

            ObjectPermissionUser objectPermissionUser = (TempData["objectPermissionUser"] as ObjectPermissionUser);
            objectPermissionUser = objectPermissionUser ?? user.ObjectPermissionUser.FirstOrDefault(e => e.id == id_objectPermissionUser);
            objectPermissionUser = objectPermissionUser ?? new ObjectPermissionUser();

            TempData["objectPermissionUser"] = objectPermissionUser;
            TempData.Keep("objectPermissionUser");

            return objectPermissionUser;
        }

        #endregion

    }
}