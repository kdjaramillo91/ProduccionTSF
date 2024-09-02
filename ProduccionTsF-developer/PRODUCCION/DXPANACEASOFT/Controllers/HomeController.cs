using DevExpress.DashboardWeb.Mvc;
using DXPANACEASOFT.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

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
			// Recuperar los elementos del menú asignados al usuario
			var assignedMenues = db.UserMenu
				.Include(u => u.Menu)
				.Where(um => um.id_user == this.ActiveUserId)
				.Select(m => m.Menu)
				.OrderBy(m => m.position)
				.ToList();
			var menuRoots = assignedMenues
				.Where(m => m.id_parent == null && m.isActive)
				.ToList();

			// Recuperar los controladores y las acciones correspondientes a los elementos del menú anterior
			var assignedControllersId = assignedMenues
				.Select(m => m.id_controller)
				.Distinct();
			var assignedControllers = db.TController
				.Where(c => assignedControllersId.Contains(c.id))
				.ToList();

			var assignedActionsId = assignedMenues
				.Select(m => m.id_action)
				.Distinct();
			var assignedActions = db.TAction
				.Where(a => assignedActionsId.Contains(a.id))
				.ToList();

			// Construir el árbol del menú
			var treeMenu = new List<TreeMenu>();

			foreach (var menuRoot in menuRoots)
			{
				if (menuRoot.isActive)
				{
					var root = this.BuildTreeMenu(menuRoot, assignedMenues, assignedControllers, assignedActions);
					treeMenu.Add(root);
				}
			}

			return Json(treeMenu, JsonRequestBehavior.AllowGet);
		}

		public ActionResult DocumentDialog(string url)
		{
			//string[] route = url.Split(new char[] { '/' });
			return PartialView(url);
		}

		#region AUXILIAR FUNCTIONS 

		private TreeMenu BuildTreeMenu(Menu menu, List<Menu> assignedMenues,
			List<TController> assignedControllers, List<TAction> assignedActions)
		{
			var treeMenu = new TreeMenu()
			{
				id = menu.id,
				id_parent = menu.id_parent,
				position = menu.position,
				title = menu.title,
				controller = assignedControllers.FirstOrDefault(c => c.id == menu.id_controller)?.name ?? "",
				action = assignedActions.FirstOrDefault(a => a.id == menu.id_action)?.name ?? "",
			};

			foreach (var childMenu in assignedMenues.Where(m => m.id_parent == menu.id))
			{
				if (childMenu.isActive)
				{
					var childTreeMenu = this.BuildTreeMenu(
						childMenu, assignedMenues,
						assignedControllers, assignedActions);

					treeMenu.children.Add(childTreeMenu);
				}
			}

			return treeMenu;
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