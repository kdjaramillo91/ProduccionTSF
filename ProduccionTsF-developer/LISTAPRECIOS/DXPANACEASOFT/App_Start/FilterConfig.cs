using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Controllers;

namespace DXPANACEASOFT {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}