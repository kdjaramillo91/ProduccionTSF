using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.DashboardWeb.Mvc;
using DXPANACEASOFT.Models;
using Newtonsoft.Json;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class HomeController : DefaultController
    {
        // GET: Home
        public ActionResult Index()
        {
            User user = db.User.FirstOrDefault(u => u.id == ActiveUser.id);
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult SideBarMenu()
        {
            User user = db.User.FirstOrDefault(u => u.id == ActiveUser.id);
            List<Menu> assignedMenues = user?.UserMenu.Select(m => m.Menu).OrderBy(m => m.position).ToList() ?? new List<Menu>();

            List<Menu> roots = assignedMenues.Where(m => m.id_parent == null && m.isActive).ToList();
            List<TreeMenu> treeMenu = new List<TreeMenu>();

            foreach (var menu in roots)
            {
                TreeMenu root = new TreeMenu();
                treeMenu.Add(root);
                FillChildren(root, menu, assignedMenues);
            }

            return Json(treeMenu, JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult DocumentDialog(string url)
        {
            //string[] route = url.Split(new char[] { '/' });
            return PartialView(url);
        }

        #region AUXILIAR FUNCTIONS 

        private void FillChildren(TreeMenu root, Menu menu, List<Menu> assignedMenues)
        {
            root.id = menu.id;
            root.id_parent = menu.id_parent;
            root.position = menu.position;
            root.title = menu.title;
            root.controller = menu.TController?.name ?? "";
            root.action = menu.TAction?.name ?? "";

            foreach (var subMenu in menu.Menu1)
            {
                Menu m = assignedMenues.FirstOrDefault(x => x.id == subMenu.id);

                if (subMenu.isActive && m != null)
                {
                    TreeMenu subTree = new TreeMenu();
                    root.children.Add(subTree);
                    FillChildren(subTree, subMenu, assignedMenues);
                }
            }
        }

        #endregion

        [ValidateInput(false)]
        public ActionResult DashboardViewerPartial()
        {
            return PartialView("_DashboardViewerPartial", DashboardViewerSettings.Model);
        }

        public FileStreamResult DashboardViewerPartialExport()
        {
            return DevExpress.DashboardWeb.Mvc.DashboardViewerExtension.Export("DashboardViewer", DashboardViewerSettings.Model);
        }
    }

    class DashboardViewerSettings
    {
        public static DashboardSourceModel Model => DashboardSourceModel();

        private static DashboardSourceModel DashboardSourceModel()
        {
            DashboardSourceModel model = new DashboardSourceModel();
            model.DashboardSource = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/dashboard2.xml");
            return model;
        }
    }

}