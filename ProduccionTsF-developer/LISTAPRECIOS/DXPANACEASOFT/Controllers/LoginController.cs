using DXPANACEASOFT.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace DXPANACEASOFT.Controllers
{
    public class LoginController : DefaultController
    {
        // GET: Login
        public ActionResult Index(string ReturnUrl = "~/")
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public JsonResult ValidateLogin(string username, string password, bool rememberMe = false)
        {
            bool valid = false;
            string message = "";

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
            password = Convert.ToBase64String(hash);

            User user = db.User.FirstOrDefault(u => u.username.Equals(username) && u.password.Equals(password));
            
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.username, rememberMe);

                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                
                ApplicationUser applicationUser = new ApplicationUser
                {
                    id = user.id,
                    username = user.username,
                    id_company = user.id_company
                };

                string data = jsSerializer.Serialize(applicationUser);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.username, DateTime.Now, DateTime.Now.AddMinutes(30), rememberMe, data);

                string encryptToken = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptToken);
                Response.Cookies.Add(cookie);
                //var info = FilePathResult();
                //if (Url.Action IsLocalUrl(loginInfo.url))
                //{
                //    result.Data = loginInfo.url;
                //}
                //else
                //{
                //    result.Data = "/";
                //}
                valid = true;
                message = "Bienvenido " + user.username;
            }
            else
            {
                valid = false;
                message = "Credenciales Incorrectas";
            }

            var result = new
            {
                valid,
                message
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: Logout
        [Authorize]
        public void Logout()
        {
            FormsAuthentication.SignOut();
            //Response.Redirect("/");
        }
    }
}