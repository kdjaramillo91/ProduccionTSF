using DevExpress.DashboardWeb;
using DevExpress.DashboardWeb.Mvc;
using System.Web.Mvc;
using System.Web.Routing;

namespace DXPANACEASOFT
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");
			routes.MapDashboardRoute();

			routes.MapRoute(
				name: "Default", // Route name
				url: "{controller}/{action}/{id}", // URL with parameters
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

			DashboardFileStorage dashboardFileStorage = new DashboardFileStorage("~/App_Data");
			DashboardConfigurator.Default.SetDashboardStorage(dashboardFileStorage);
		}
	}
}