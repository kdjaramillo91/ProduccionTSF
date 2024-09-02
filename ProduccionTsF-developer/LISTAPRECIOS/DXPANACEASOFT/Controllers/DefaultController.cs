using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;
using EntidadesAuxiliares.General;
using System.Configuration;
using Utilitarios.General;

namespace DXPANACEASOFT.Controllers
{
    public class DefaultController : Controller
    {
        public DBContext db { get; } = new DBContext();

        public User ActiveUser
        {
            get
            {
                ApplicationUser appUser = (ApplicationUser)((LogedUser)User).User;
                return db.User.FirstOrDefault(u => u.id == appUser.id);
            }
        }

        #region ACTIVE STRUCTURE

        protected Company ActiveCompany
        {
            get
            {   
                return db.Company.FirstOrDefault(c => c.id == ActiveUser.id_company); ;
            }
        }

        protected Division ActiveDivision
        {
            get
            {
                return db.Division.FirstOrDefault(d => d.id_company == ActiveCompany.id);
            }
        }

        protected BranchOffice ActiveSucursal
        {
            get
            {
                return db.BranchOffice.FirstOrDefault(o => o.id_division == ActiveDivision.id);
            }
        }

        protected EmissionPoint ActiveEmissionPoint
        {
            get
            {
                return db.EmissionPoint.FirstOrDefault(e => e.id_branchOffice == ActiveSucursal.id);
            }
        }

        #endregion

        #region COMMUN TOOLBOX FUNCTIONS 

        protected string FormatDate(string date, string [] langs)
        {
            string formatedDate = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                formatedDate = DateTime.Parse(date, new CultureInfo(langs.First())).ToString("dd/MM/yyyy");
                return formatedDate;
            }
            catch (Exception)
            {
                formatedDate = DateTime.Now.ToString("dd/MM/yyyy");
                return formatedDate;
            }
        }

        protected int GetDocumentSequential(int id_documentType)
        {
            DocumentType documentType = db.DocumentType.FirstOrDefault(d => d.id == id_documentType && d.id_company == ActiveCompany.id);
            return documentType?.currentNumber ?? 0;
        }

        protected  string GetDocumentNumber(int id_documentType)
        {
            string number = GetDocumentSequential(id_documentType).ToString().PadLeft(9, '0');
            string documentNumber = string.Empty;
            documentNumber = $"{ActiveEmissionPoint.BranchOffice.code.ToString().PadLeft(3, '0')}-{ActiveEmissionPoint.code.ToString().PadLeft(3, '0')}-{number}";
            return documentNumber;
        }

        protected string SuccessMessage(string text = "Registro guardado exitosamente")
        {
            string message = @"<div id=""successMessage"" class=""alert alert-success alert-dismissible fade in"" style=""margin-top: 10px; text-align: center; padding: 10px 15px;"">"
                           + @"<button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""close"" title=""close"" style=""top: 0px; right: 0px;""><span aria-hidden=""true"">&times;</span></button>"
                           + text
                           + @"</div>";
            return message;
        }

        protected string ErrorMessage(string text = "Ah ocurrido un error")
        {
            string message = @"<div id=""errorMessage"" class=""alert alert-danger alert-dismissible fade in"" style=""margin-top: 10px; text-align: center; padding: 10px 15px;"">"
                           + @"<button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""close"" title=""close"" style=""top: 0px; right: 0px;""><span aria-hidden=""true"">&times;</span></button>"
                           + text
                           + @"</div>";
            return message;
        }

        protected string WarningMessage(string text = "Advertencia!, puede ocurrir un error")
        {
            string message = @"<div id=""warningMessage"" class=""alert alert-warning alert-dismissible fade in"" style=""margin-top: 10px; text-align: center; padding: 10px 15px;"">"
                           + @"<button type=""button"" class=""close"" data-dismiss=""alert"" aria-label=""close"" title=""close"" style=""top: 0px; right: 0px;""><span aria-hidden=""true"">&times;</span></button>"
                           + text
                           + @"</div>";
            return message;
        }

        protected void UpdateArrayTempDataKeep(string[] arrayTempDataKeep)
        {
            //ViewData["arrayTempDataKeep"] = arrayTempDataKeep;
            TempData["arrayTempDataKeep"] = arrayTempDataKeep;
            TempData.Keep("arrayTempDataKeep");
            if (arrayTempDataKeep != null && arrayTempDataKeep.Length > 0)
            {
                foreach (var str in arrayTempDataKeep)
                {
                    TempData.Keep(str);
                }

            }
        }

        

#endregion

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            try
            {
                User user = ActiveUser;

                if (user != null)
                {
                    ViewData["id_user"] = user.id;
                    ViewData["username"] = user.username;
                    ViewData["id_company"] = ActiveCompany.id;
                    ViewData["company"] = ActiveCompany.trademark;
                    ViewData["id_division"] = ActiveDivision.id;
                    ViewData["division"] = ActiveDivision.name;
                    ViewData["id_sucursal"] = ActiveSucursal.id;
                    ViewData["sucursal"] = ActiveSucursal.name;
                    ViewData["id_emissionPoint"] = ActiveEmissionPoint.id;
                    ViewData["emissionPoint"] = ActiveEmissionPoint.name;
                    ViewData["id_menu"] = 0;
                    ViewData["permissions"] = null;

                    UserMenu userMenu = null;

                    if (TempData["menu"] != null)
                    {
                        userMenu = (TempData["menu"] as UserMenu);
                        List<int> permissions = userMenu?.Permission.Where(x => x.isActive).Select(x => x.id).ToList();

                        ViewData["id_menu"] = userMenu?.Menu.id ?? 0;
                        ViewData["permissions"] = permissions;
                    }

                    string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                    string action = filterContext.ActionDescriptor.ActionName;

                    Menu menu = db.Menu.FirstOrDefault(m => m.TController.name.Equals(controller) && m.TAction.name.Equals(action));

                    if (menu != null)
                    {
                        userMenu = db.UserMenu.FirstOrDefault(m => m.id_user == user.id && m.Menu.id == menu.id);

                        if (TempData["menu"] == null || userMenu != null)
                            TempData["menu"] = userMenu;

                        userMenu = (TempData["menu"] as UserMenu);
                        List<int> permissions = userMenu?.Permission.Where(x => x.isActive).Select(x => x.id).ToList();

                        ViewData["id_menu"] = userMenu?.Menu.id ?? 0;
                        ViewData["permissions"] = permissions;
                    }
                        

                }
            }
            catch (Exception)
            {
                ViewData["id_user"] = 0;
                ViewData["username"] = "";
                ViewData["id_company"] = 0;
                ViewData["company"] = "";
                ViewData["id_division"] = 0;
                ViewData["division"] = "";
                ViewData["id_sucursal"] = 0;
                ViewData["sucursal"] = "";
                ViewData["id_emissionPoint"] = 0;
                ViewData["emissionPoint"] = "";
                ViewData["id_menu"] = 0;
                ViewData["permissions"] = null;
            }

            TempData.Keep("menu");
        }

        #region Auxiliar

        public static decimal TruncateDecimal(decimal decimalParam)
        {
            var decimalTruncate = decimal.Truncate(decimalParam);
            if ((decimalParam - decimalTruncate) > 0)
            {
                decimalTruncate++;
            };
            return decimalTruncate;
        }

        public static decimal GetFactorConversion(MetricUnit metricOrigin, MetricUnit metricDestiny, string messageError, DBContext db)
        {
            decimal factorConversionAux;
            try
            {
                factorConversionAux = (metricOrigin.id != metricDestiny.id) ? db.MetricUnitConversion.FirstOrDefault(fod => fod.id_metricOrigin == metricOrigin.id &&
                                                                                                                            fod.id_metricDestiny == metricDestiny.id)?.factor ?? 0 : 1;
                if (factorConversionAux == 0)
                {
                    throw new Exception(messageError);

                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
            
            return factorConversionAux;
        }

        #endregion

        public static Conexion GetObjectConnection(string strConex)
        {
            string strCon = ConfigurationManager.ConnectionStrings[strConex].ConnectionString;
            Conexion _conex = null;

            if (!string.IsNullOrWhiteSpace(strCon))
            {
                string[] lstConex = strCon.Split(';');
                _conex = new Conexion();
                if (lstConex.Length > 0)
                {
                    foreach (string _strC in lstConex)
                    {
                        if (_strC.Contains("data source"))
                        {
                            _conex.SrvName = _strC.Substring(12);
                        }
                        if (_strC.Contains("initial catalog"))
                        {
                            _conex.DbName = _strC.Substring(16);
                        }
                        if (_strC.Contains("user id"))
                        {
                            _conex.UsrName = _strC.Substring(8);
                        }
                        if (_strC.Contains("password"))
                        {
                            _conex.PswName = _strC.Substring(9);
                        }
                    }
                }
            }
            return _conex;
        }

        public static ReportParanNameModel GetTmpDataName(int strLength)
        {
            ReportParanNameModel _rep = new ReportParanNameModel();

            _rep.nameQS = GeneralStr.GetRandomStr(strLength);
            _rep.isEncripted = true;
            _rep.messError = "";

            return _rep;
        }
    }
}