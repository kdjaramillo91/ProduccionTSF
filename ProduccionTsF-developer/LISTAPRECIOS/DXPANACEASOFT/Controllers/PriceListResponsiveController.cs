using DevExpress.DashboardWeb.Native;
using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Extensions.Querying;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DTOModel;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Utilitarios.Logs;

namespace DXPANACEASOFT.Controllers
{
    public class PriceListResponsiveController : DefaultController
    {
        private bool readOnly;
        private DocumentState crateState;
        private DocumentState reversedState;
        private DocumentState aprovedState;
        private SettingPriceList settingPriceList;
        public string ruta = ConfigurationManager.AppSettings["rutaLog"];
        public string app = "ListaPrecio";


        // GET: PriceListResponsive
        public ActionResult Index()
        {
            BuildViewData();
            return View();
        }

        private PriceListDTO GetPriceListDTO()
        {
            if (!(Session["PriceListDTO"] is PriceListDTO priceListDTO))
                priceListDTO = new PriceListDTO();
            return priceListDTO;
        }

        private void SetPriceListDTO(PriceListDTO priceListDTO)
        {
            Session["PriceListDTO"] = priceListDTO;
        }

        private void Estados(string From = "")
        {
            try
            {
                ViewData["Estados"] = db.tbsysDocumentTypeDocumentState
                    .Where(d => d.DocumentType.code.Equals("18") ||
                                d.DocumentType.code.Equals("19") ||
                                d.DocumentType.code.Equals("20") ||
                                d.DocumentType.code.Equals("21"))
                    .Select(s => new SelectListItem
                    {
                        Text = s.DocumentState.name,
                        Value = s.id_DocumenteState.ToString()
                    }).Distinct().ToList();
                MetodosEscrituraLogs.EscribeMensajeLog("OK", ruta, "SearchResult", app);
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "Estados", app);
            }
        }

        public ActionResult ComboBoxEstados(string From, bool enabled = true, bool IsOwner = true)
        {

            using (DBContext db = new DBContext())
            {
                try
                {
                    Estados();

                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    MetodosEscrituraLogs.EscribeMensajeLog("OK", ruta, "ComboBoxEstados", app);
                }
                catch (Exception ex)
                {
                    MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "ComboBoxEstados", app);
                }

                return PartialView("_ComboBoxEstadosIndex");
            }
        }

        private void Proveedores(string From = "")
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    ViewData["Proveedores"] = db.Person.Where(p => (p.Rol.Any(a => a.name == "Proveedor"))).Select(s => new SelectListItem
                    {
                        Text = s.fullname_businessName,
                        Value = s.id.ToString()
                    }).ToList();
                }
                else
                {
                    ViewData["Proveedores"] = db.Person.Where(p => (p.Rol.Any(a => a.name == "Proveedor"))).Select(s => new SelectListItem
                    {
                        Text = s.fullname_businessName,
                        Value = s.id.ToString()
                    }).ToList();
                }
            }
        }

        public ActionResult ComboBoxProveedores(string From, bool enabled = true, bool IsOwner = true)
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    Proveedores(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxProveedoresEdit");
                }
                else
                {
                    Proveedores(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxProveedoresIndex");
                }
            }
        }

        private void Grupos(string From = "")
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    ViewData["Grupos"] = db.GroupPersonByRol.Where(g => g.isActive).Select(s => new SelectListItem
                    {
                        Text = s.name,
                        Value = s.id.ToString()
                    }).ToList();
                }
                else
                {
                    ViewData["Grupos"] = db.GroupPersonByRol.Select(s => new SelectListItem
                    {
                        Text = s.name,
                        Value = s.id.ToString()
                    }).ToList();
                }
            }
        }

        public ActionResult ComboBoxGrupos(string From, bool enabled = true, bool IsOwner = true)
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    Grupos(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxGruposEdit");
                }
                else
                {
                    Grupos(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxGruposIndex");
                }
            }
        }

        private void TipoLista(string From = "")
        {
            ViewData["TipoLista"] = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "1",
                    Text = "Referencial"
                },
                new SelectListItem
                {
                    Value = "2",
                    Text = "LP Proveedores"
                }
            };
        }

        public ActionResult ComboBoxTipoLista(string From, bool enabled = true, bool IsOwner = true)
        {
            TipoLista(From);
            using (DBContext db = new DBContext())
            {
                ViewBag.enabled = enabled;
                ViewBag.IsOwner = IsOwner;
                return PartialView("_ComboBoxTipoListaIndex");
            }
        }

        private void TipoListaCamaron(string From = "")
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    ViewData["TipoListaCamaron"] = db.ProcessType
                        .Where(w => w.isActive)
                        .Select(s => new SelectListItem
                        {
                            Text = s.name,
                            Value = s.id.ToString()
                        }).ToList();
                }
                else
                {
                    ViewData["TipoListaCamaron"] = db.ProcessType
                        .Where(w => w.isActive)
                        .Select(s => new SelectListItem
                        {
                            Text = s.name,
                            Value = s.id.ToString()
                        }).ToList();
                }
            }
        }

        public ActionResult ComboBoxTipoListaCamaron(string From, bool enabled = true, bool IsOwner = true)
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    TipoListaCamaron(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxTipoListaCamaronEdit");
                }
                else
                {
                    TipoListaCamaron(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxTipoListaCamaronIndex");
                }
            }
        }

        private void Compradores(string From = "")
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    User user = DataProviderUser.UserById(ActiveUser.id);
                    int id_userGroup = user.id_group;

                    var seeAllStates = false;
                    var settingPriceList = db.SettingPriceList.FirstOrDefault(s => s.id_userGroupApproval == id_userGroup);
                    if (settingPriceList != null)
                        seeAllStates = settingPriceList.seeAllStates;

                    if (seeAllStates)
                    {
                        ViewData["Compradores"] = db.User
                        //.Where(e => e.Department.name.Equals("Comercial"))
                        .Select(s => new SelectListItem
                        {
                            Text = s.Employee.Person.fullname_businessName,
                            Value = s.id.ToString(),
                        }).ToList();
                    }
                    else
                    {
                        ViewData["Compradores"] = db.User.Where(e => e.id == ActiveUser.id)
                        //.Where(e => e.Department.name.Equals("Comercial"))
                        .Select(s => new SelectListItem
                        {
                            Text = s.Employee.Person.fullname_businessName,
                            Value = s.id.ToString(),
                        }).ToList();
                    }

                    var priceListDTO = GetPriceListDTO();
                    if (priceListDTO?.id != 0)
                    {
                        var comprador = db.User.FirstOrDefault(p => p.id == priceListDTO.id_comprador);
                        if (comprador != null)
                        {
                            var value = (ViewData["Compradores"] as List<SelectListItem>).FirstOrDefault(c => c.Value.Equals(comprador.id.ToString()));
                            if (value != null)
                                (ViewData["Compradores"] as List<SelectListItem>).Remove(value);

                            (ViewData["Compradores"] as List<SelectListItem>).Insert(0, new SelectListItem
                            {
                                Text = comprador.Employee.Person.fullname_businessName,
                                Value = comprador.id.ToString()
                            });
                        }
                    }
                }
                else
                {
                    ViewData["Compradores"] = db.User
                    //.Where(e => e.Department.name.Equals("Comercial"))
                    .Select(s => new SelectListItem
                    {
                        Text = s.Employee.Person.fullname_businessName,
                        Value = s.id.ToString()
                    }).ToList();
                }
            }
        }

        public ActionResult ComboBoxCompradores(string From, bool enabled = true, bool IsOwner = true)
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    Compradores(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxCompradoresEdit");
                }
                else
                {
                    Compradores(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxCompradoresIndex");
                }
            }
        }

        private void PeriodoCalendario(string From = "", PriceList priceList = null)
        {
            using (DBContext db = new DBContext())
            {
                var dayNow = DateTime.Now;

                if (From.Equals("Edit"))
                {
                    ViewData["PeriodoCalendario"] = db.CalendarPriceList.Where(p => p.isActive &&
                                                                                    p.startDate.Year == dayNow.Year &&
                                                                                    (p.startDate.Month == dayNow.Month ||
                                                                                     p.endDate.Month == dayNow.Month ||
                                                                                     p.endDate.Month == dayNow.Month + 1)).AsEnumerable()
                    .Select(g => new SelectListItem
                    {
                        Text = g.CalendarPriceListType.name + " " + g.startDate.ToString("dd/MM/yyyy") + " - " + g.endDate.ToString("dd/MM/yyyy"),
                        Value = g.id.ToString()
                    }).ToList();

                    if (priceList?.CalendarPriceList?.CalendarPriceListType != null)
                    {
                        if (((List<SelectListItem>)ViewData["PeriodoCalendario"]).Any(s => s.Value.Equals(priceList.id_calendarPriceList.ToString())) == false)
                        {
                            ((List<SelectListItem>)ViewData["PeriodoCalendario"]).Add(new SelectListItem
                            {
                                Text = priceList.CalendarPriceList.CalendarPriceListType.name + " " + priceList.CalendarPriceList.startDate.ToString("dd/MM/yyyy") + " - " + priceList.CalendarPriceList.endDate.ToString("dd/MM/yyyy"),
                                Value = priceList.id_calendarPriceList.ToString()
                            });
                        }
                    }
                }
                else
                {
                    ViewData["PeriodoCalendario"] = db.CalendarPriceList.AsEnumerable().Select(g => new SelectListItem
                    {
                        Text = g.CalendarPriceListType.name + " " + g.startDate.ToString("dd/MM/yyyy") + " - " + g.endDate.ToString("dd/MM/yyyy"),
                        Value = g.id.ToString()
                    }).ToList();
                }
            }
        }

        public ActionResult ComboBoxPeriodoCalendario(string From, bool enabled = true, bool IsOwner = true)
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    PeriodoCalendario(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxPeriodoCalendarioEdit");
                }
                else
                {
                    PeriodoCalendario(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxPeriodoCalendarioIndex");
                }
            }
        }

        private void ListasPrecio(string From = "", PriceList priceList = null)
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    FindSettingPriceList();

                    var maxSettingRol = db.SettingPriceList.FirstOrDefault(p => p.weight == db.SettingPriceList.Max(r => r.weight));
                    if (priceList != null && priceList.id != 0)
                    {
                        ViewData["ListasPrecio"] = db.PriceList.Where(l =>
                        !l.isQuotation &&
                        l.Document.id_documentState == maxSettingRol.id_aprovedState //&&
                                                                                     // (l.startDate >= priceList.startDate && l.endDate <= priceList.endDate))
                       )
                        .Select(l => new SelectListItem
                        {
                            Text = l.name,
                            Value = l.id.ToString()
                        }).ToList();
                    }
                    else
                    {
                        ViewData["ListasPrecio"] = db.PriceList.Where(l =>
                        !l.isQuotation &&
                        l.Document.id_documentState == maxSettingRol.id_aprovedState &&
                        (l.startDate >= priceList.startDate && l.endDate <= priceList.endDate))
                        .Select(l => new SelectListItem
                        {
                            Text = l.name,
                            Value = l.id.ToString()
                        }).ToList();
                    }

                }
                else
                {
                    ViewData["ListasPrecio"] = db.PriceList.Where(l => !l.isQuotation).Select(l => new SelectListItem
                    {
                        Text = l.name,
                        Value = l.id.ToString()
                    }).ToList();
                }
            }
        }

        public ActionResult ComboBoxListasPrecio(string startDate, string endDate, string From, bool enabled = true, bool IsOwner = true)
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    PriceList priceList = null;
                    if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                    {
                        priceList = new PriceList();
                        priceList.startDate = DateTime.Parse(startDate);
                        priceList.endDate = DateTime.Parse(endDate);
                    }
                    ListasPrecio(From, priceList);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxListasPrecioEdit");
                }
                else
                {
                    ListasPrecio(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxListasPrecioIndex");
                }
            }
        }

        private void Certificaciones(string From = "")
        {
            var aPriceListDTO = GetPriceListDTO();
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    ViewData["Certificaciones"] = db.Certification.Where(w => w.id == aPriceListDTO.id_certification || w.isActive).Select(s => new SelectListItem
                    {
                        Text = s.name,
                        Value = s.id.ToString()
                    }).ToList();
                }
                else
                {
                    ViewData["Certificaciones"] = db.Certification.Select(s => new SelectListItem
                    {
                        Text = s.name,
                        Value = s.id.ToString()
                    }).ToList();
                }
            }
        }

        public ActionResult ComboBoxCertificaciones(string From, bool enabled = true, bool IsOwner = true)
        {
            using (DBContext db = new DBContext())
            {
                if (From.Equals("Edit"))
                {
                    Certificaciones(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxCertificacionesEdit");
                }
                else
                {
                    Certificaciones(From);
                    ViewBag.enabled = enabled;
                    ViewBag.IsOwner = IsOwner;
                    return PartialView("_ComboBoxCertificacionesIndex");
                }
            }
        }

        [HttpPost]
        public JsonResult ProvidersByGroup(int id_grupo, int id_priceList)
        {
            var result = new ApiResult();
            try
            {
                if (id_priceList == 0)
                {
                    var groupPerson = db.GroupPersonByRolDetail.Where(g => g.id_groupPersonByRol == id_grupo).ToList();
                    if (groupPerson == null)
                        result.Data = "";
                    else
                        result.Data = string.Join("</br>", groupPerson.Select(c => c.Person.fullname_businessName));
                }
                else
                {
                    var priceListPersonPersonRol = db.PriceListPersonPersonRol.Where(p => p.id_PriceList == id_priceList).ToList();
                    if (priceListPersonPersonRol == null)
                        result.Data = "";
                    else
                        result.Data = string.Join("</br>", priceListPersonPersonRol.Select(p => p.Person.fullname_businessName));
                }
            }
            catch (Exception e)
            {
                result.Code = e.HResult;
                result.Message = e.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void FindSettingPriceList()
        {
            try
            {
                using (var db = new DBContext())
                {
                    User user = DataProviderUser.UserById(ActiveUser.id);
                    int id_userGroup = user.id_group;

                    settingPriceList = db.SettingPriceList.FirstOrDefault(s => s.id_userGroupApproval == id_userGroup);
                    if (settingPriceList == null)
                        throw new Exception("No existe una configuración en el Setting de Lista de Precio para el usuario registrado");

                    readOnly = settingPriceList.readOnly;
                    crateState = db.DocumentState.FirstOrDefault(d => d.id == settingPriceList.id_crateState);
                    aprovedState = db.DocumentState.FirstOrDefault(d => d.id == settingPriceList.id_aprovedState);
                    reversedState = db.DocumentState.FirstOrDefault(d => d.id == settingPriceList.id_reversedState);

                    if (crateState == null)
                        throw new Exception("No encontro el estado de creación del usuario registrado en el Setting de Lista de Precio");
                    if (aprovedState == null)
                        throw new Exception("No encontro el estado de aprovación del usuario registrado en el Setting de Lista de Precio");
                    if (reversedState == null)
                        throw new Exception("No encontro el estado de reversión del usuario registrado en el Setting de Lista de Precio");
                }
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "FindSettingPriceList", app);
            }
        }

        private void BuildViewData(string From = "", PriceList priceList = null, bool enabled = true)
        {
            try
            {
                FindSettingPriceList();
                Estados(From);
                Proveedores(From);
                PeriodoCalendario(From, priceList);
                ListasPrecio(From, priceList);
                TipoLista(From);
                TipoListaCamaron(From);
                Compradores(From);
                Grupos(From);
                Certificaciones(From);

                ViewBag.readOnly = readOnly;
                MetodosEscrituraLogs.EscribeMensajeLog(From, ruta, "BuildViewData", app);
                if (!From.Equals("Edit"))
                    return;

                var stateClose = db.DocumentState.FirstOrDefault(s => s.code.Equals("04"));


                var priceListDTO = GetPriceListDTO();
                var code_processtype = db.ProcessType.FirstOrDefault(s => s.id == priceListDTO.id_processtype)?.code;
                //ViewBag.CanReplicateDetailsCOL = code_processtype == "COL" && enabled;
                //ViewBag.CanReplicateDetailsENT = code_processtype == "ENT" && enabled;
                ViewBag.code_processtype = code_processtype;
                MetodosEscrituraLogs.EscribeMensajeLog(ViewBag.code_processtype, ruta, "BuildViewData", app);
                ViewBag.enabled = enabled;
                ViewBag.CanSave = enabled;
                MetodosEscrituraLogs.EscribeMensajeLog("Guardar: " + ViewBag.CanSave, ruta, "BuildViewData", app);
                ViewBag.CanDuplicate = priceList?.id != 0;
                MetodosEscrituraLogs.EscribeMensajeLog("Duplicar: " + ViewBag.CanDuplicate, ruta, "BuildViewData", app);
                ViewBag.CanEdit = !enabled && (priceListDTO.id_state == crateState.id || priceListDTO.id_state == reversedState.id);
                MetodosEscrituraLogs.EscribeMensajeLog("Editar: " + ViewBag.CanEdit, ruta, "BuildViewData", app);
                ViewBag.IsOwner = priceList.id == 0 || priceList.Document.id_userCreate == ActiveUser.id;
                ViewBag.CanAproved = (priceListDTO.id_state == crateState.id || priceListDTO.id_state == reversedState.id);
                MetodosEscrituraLogs.EscribeMensajeLog("Aprobar: " + ViewBag.CanAproved, ruta, "BuildViewData", app);
                ViewBag.CanReverse = (priceListDTO.id_state == aprovedState.id && db.PriceList.FirstOrDefault(c => c.isQuotation && c.id_priceListBase == priceListDTO.id) == null);
                MetodosEscrituraLogs.EscribeMensajeLog("Reversar: " + ViewBag.CanReverse, ruta, "BuildViewData", app);
                ViewBag.CanClose = (priceListDTO.id_state != stateClose.id) || (priceListDTO.startDate < DateTime.Now && priceListDTO.endDate > DateTime.Now);
                MetodosEscrituraLogs.EscribeMensajeLog("Cerrar: " + ViewBag.CanClose, ruta, "BuildViewData", app);
                ViewBag.CanAnnul = (priceListDTO.id_state == crateState.id || priceListDTO.id_state == reversedState.id);
                MetodosEscrituraLogs.EscribeMensajeLog("Anular: " + ViewBag.CanAnnul, ruta, "BuildViewData", app);
                ViewBag.canRefreshReplicate = priceList?.id != 0;

                int nviewall = 0;
                string nvalViewall = db.Setting.FirstOrDefault(r => r.code == "VWPP")?.value ?? "0";
                int.TryParse(nvalViewall, out nviewall);
                ViewBag.Nviewall = nviewall;
                MetodosEscrituraLogs.EscribeMensajeLog("OK", ruta, "BuildViewData", app);
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "BuildViewData", app);
            }


        }

        private List<PriceListDTO> GetPriceListsDTO(PriceListConsultDTO consulta)
        {

            FindSettingPriceList();
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string _pathProgramReplication = ConfigurationManager.AppSettings["pathProgramReplication"];
            var seeAllStates = false;
            if (settingPriceList != null)
                seeAllStates = settingPriceList.seeAllStates;

            using (var db = new DBContext())
            {
                var stateClose = db.DocumentState.FirstOrDefault(s => s.code.Equals("04"));
                var pricesListConsult = db.PriceList.Where(PriceListQueryExtensions.GetRequestByFilter(ActiveUser, consulta, seeAllStates));
                var query = ServicePriceList.GetPriceList(pricesListConsult, _pathProgramReplication, ruta)
                    .OrderByDescending(o => o.id)
                    .Select(lp => new PriceListDTO
                    {
                        id = lp.id,
                        name = lp.name,
                        id_state = lp.Document.id_documentState,
                        state = lp.Document.DocumentState.name,
                        basePriceList = lp.isQuotation ? lp.PriceList2.name : "",
                        certification = lp.isQuotation ? (lp.Certification != null ? lp.Certification.name : "") : "",
                        isUsed = !lp.isQuotation
                                 ? db.PriceList.FirstOrDefault(c => c.isQuotation && c.id_priceListBase == lp.id) != null
                                 : false,
                        canClose = lp.Document.id_documentState != stateClose.id &&
                                   lp.startDate < DateTime.Now && lp.endDate > DateTime.Now,
                        canAnnul = lp.Document.id_documentState == crateState.id || lp.Document.id_documentState == reversedState.id,
                        canRefreshReplicate = true

                    }).ToList();

                ViewBag.fechaInicio = consulta.fechaInicio;
                ViewBag.fechaFin = consulta.fechaFin;
                ViewBag.id_estado = consulta.id_estado;
                ViewBag.id_tipoLista = consulta.id_tipoLista;
                ViewBag.id_proveedor = consulta.id_proveedor;
                ViewBag.id_grupo = consulta.id_grupo;
                ViewBag.id_tipoListaCamaron = consulta.id_tipoListaCamaron;
                ViewBag.id_responsable = consulta.id_responsable;
                ViewBag.id_certification = consulta.id_certification;

                return query;
            }
        }

        [HttpPost]
        public ActionResult SearchResult(PriceListConsultDTO consulta)
        {
            try
            {
                FindSettingPriceList();
                ViewBag.readOnly = readOnly;
                ViewBag.id_crateState = crateState.id;
                ViewBag.id_aprovedState = aprovedState.id;
                ViewBag.id_reversedState = reversedState.id;

                MetodosEscrituraLogs.EscribeMensajeLog("OK", ruta, "SearchResult", app);
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "SearchResult", app);
            }
            return PartialView("ConsultResult", GetPriceListsDTO(consulta));
        }

        [ValidateInput(false)]
        public ActionResult GridViewPriceList(PriceListConsultDTO consulta)
        {
            return PartialView("_GridViewPriceList", GetPriceListsDTO(consulta));
        }

        private List<PriceListDTO> GetPriceListsDTOPendingAproval()
        {
            FindSettingPriceList();
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string _pathProgramReplication = ConfigurationManager.AppSettings["pathProgramReplication"];
            var id_reversedState = 0;
            var id_aprovedState = 0;
            var id_crateState = 0;
            if (settingPriceList != null)
            {
                id_crateState = settingPriceList.id_crateState;
                id_reversedState = settingPriceList.id_reversedState;
                id_aprovedState = settingPriceList.id_aprovedState;
            }


            using (var db = new DBContext())
            {
                var stateClose = db.DocumentState.FirstOrDefault(s => s.code.Equals("04"));
                var pricesListConsult = db.PriceList.Where(PriceListQueryExtensions.GetRequestPendingAproval(ActiveUser, id_crateState, id_reversedState, id_aprovedState));
                var query = ServicePriceList.GetPriceList(pricesListConsult, _pathProgramReplication, ruta).Select(lp => new PriceListDTO
                {
                    id = lp.id,
                    name = lp.name,
                    id_state = lp.Document.id_documentState,
                    state = lp.Document.DocumentState.name,
                    basePriceList = lp.isQuotation ? lp.PriceList2.name : "",
                    certification = lp.isQuotation ? (lp.Certification != null ? lp.Certification.name : "") : "",
                    isUsed = !lp.isQuotation
                              ? db.PriceList.FirstOrDefault(c => c.isQuotation && c.id_priceListBase == lp.id) != null
                              : false,
                    canClose = lp.Document.id_documentState != stateClose.id &&
                                lp.startDate < DateTime.Now && lp.endDate > DateTime.Now,
                    canAnnul = lp.Document.id_documentState == crateState.id || lp.Document.id_documentState == reversedState.id,

                }).ToList();

                return query;
            }
        }

        [ValidateInput(false)]
        public ActionResult GridViewPriceListPendingAproval()
        {
            return PartialView("_GridViewPriceListPendingAproval", GetPriceListsDTOPendingAproval());
        }

        [HttpPost]
        public ActionResult PendingApprove()
        {
            FindSettingPriceList();
            ViewBag.id_crateState = crateState.id;
            ViewBag.id_aprovedState = aprovedState.id;
            ViewBag.id_reversedState = reversedState.id;

            return View(GetPriceListsDTOPendingAproval());
        }

        [HttpPost]
        public ActionResult AddWhithBase(int id)
        {
            PriceList priceList;

            var pricelistBase = db.PriceList.FirstOrDefault(pl => pl.id == id);
            if (pricelistBase != null && pricelistBase.isQuotation)
                pricelistBase = pricelistBase.PriceList2;

            var apruvedPriceList = db.PriceList.Where(l =>
                !l.isQuotation &&
                db.SettingPriceList.FirstOrDefault(s => s.id_aprovedState == l.Document.id_documentState) != null).ToList();

            var maxSettingRol = db.SettingPriceList.FirstOrDefault(p => p.weight == db.SettingPriceList.Max(r => r.weight));

            if (pricelistBase == null ||
                !apruvedPriceList.Contains(pricelistBase) ||
                pricelistBase.Document.id_documentState != maxSettingRol.id_aprovedState)
            {
                priceList = BuildFirstPriceList();
            }
            else
            {
                FindSettingPriceList();
                var seq = db.Setting.FirstOrDefault(fod => fod.code.Equals("SLP05"));
                int seqInteger = 0;
                if (seq != null) seqInteger = Convert.ToInt32(seq.value);

                priceList = new PriceList
                {
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    endDate = DateTime.Now,
                    Document = new Document
                    {
                        id_documentState = crateState.id,
                        DocumentState = crateState
                    },
                    name = (seqInteger).ToString() + ". "
                };

                priceList.name += "COT-";

                priceList.CalendarPriceList = pricelistBase.CalendarPriceList;
                priceList.id_calendarPriceList = pricelistBase.id_calendarPriceList;
                priceList.startDate = pricelistBase.startDate;
                priceList.endDate = pricelistBase.endDate;

                priceList.isQuotation = true;
                priceList.id_priceListBase = pricelistBase.id;
                priceList.PriceList2 = pricelistBase;
                priceList.id_processtype = pricelistBase.id_processtype;
                priceList.ProcessType = pricelistBase.ProcessType;

                priceList.name += pricelistBase.ProcessType.name + "-";
                priceList.name += priceList.CalendarPriceList.CalendarPriceListType.name + " [";
                priceList.name += priceList.startDate.ToString("dd/MM/yyyy") + "-";
                priceList.name += priceList.endDate.ToString("dd/MM/yyyy") + "]";
            }

            int nviewall = 0;
            string nvalViewall = db.Setting.FirstOrDefault(r => r.code == "VWPP")?.value ?? "0";
            int.TryParse(nvalViewall, out nviewall);
            ViewBag.nViewallHidden = nviewall;

            CreatePriceListDTO(priceList);

            BuildViewData("Edit", priceList);

            var priceListDTO = GetPriceListDTO();
            return PartialView("Edit", priceListDTO);
        }

        [HttpPost]
        public ActionResult Edit(int id, bool enabled = true, string from = "")
        {
            var pricelist = BuildFirstPriceList(id);

            if (enabled)
                pricelist.Document.DocumentState = crateState;

            CreatePriceListDTO(pricelist);

            BuildViewData("Edit", pricelist, enabled);
            ViewBag.from = from;

            int nviewall = 0;
            string nvalViewall = db.Setting.FirstOrDefault(r => r.code == "VWPP")?.value ?? "0";
            int.TryParse(nvalViewall, out nviewall);
            ViewBag.nViewallHidden = nviewall;


            var priceListDTO = GetPriceListDTO();
            return PartialView(priceListDTO);
        }

        [HttpPost]
        public ActionResult Duplicate(int id, string from = "")
        {
            var pricelist = BuildFirstPriceList(id);

            pricelist.Document.DocumentState = crateState;

            CreatePriceListDTO(pricelist);

            BuildViewData("Edit", pricelist, true);
            ViewBag.from = from;
            ViewBag.IsOwner = true;
            ViewBag.CanDuplicate = false;

            int nviewall = 0;
            string nvalViewall = db.Setting.FirstOrDefault(r => r.code == "VWPP")?.value ?? "0";
            int.TryParse(nvalViewall, out nviewall);
            ViewBag.nViewallHidden = nviewall;

            var priceListDTO = GetPriceListDTO();
            var seq = db.Setting.FirstOrDefault(fod => fod.code.Equals("SLP05"));
            int seqInteger = 0;



            if (seq != null) seqInteger = Convert.ToInt32(seq.value);
            priceListDTO.id = 0;
            priceListDTO.name = (seqInteger).ToString() + ". " + priceListDTO.name.Split('.')[1];
            SetPriceListDTO(priceListDTO);
            return PartialView("Edit", priceListDTO);
        }

        private PriceList BuildFirstPriceList(int id = 0)
        {
            FindSettingPriceList();

            var pricelist = db.PriceList.FirstOrDefault(pl => pl.id == id);

            if (pricelist == null)
            {
                var seq = db.Setting.FirstOrDefault(fod => fod.code.Equals("SLP05"));
                int seqInteger = 0;
                if (seq != null) seqInteger = Convert.ToInt32(seq.value);
                pricelist = new PriceList
                {
                    startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    endDate = DateTime.Now,
                    Document = new Document
                    {
                        id_documentState = crateState.id,
                        DocumentState = crateState
                    },
                    name = (seqInteger).ToString() + ". "
                };

                pricelist.name += "REF-";

                var dayNow = DateTime.Now;
                var periodoCalendario = db.CalendarPriceList.FirstOrDefault(p => p.startDate.Year == dayNow.Year &&
                                                                                 p.startDate.Month == dayNow.Month);
                if (periodoCalendario != null)
                {
                    pricelist.CalendarPriceList = periodoCalendario;
                    pricelist.id_calendarPriceList = periodoCalendario.id;
                    pricelist.startDate = periodoCalendario.startDate;
                    pricelist.endDate = periodoCalendario.endDate;

                    pricelist.name += pricelist.CalendarPriceList.name.Split(' ')[0] + " [";
                    pricelist.name += pricelist.startDate.ToString("dd/MM/yyyy") + "-";
                    pricelist.name += pricelist.endDate.ToString("dd/MM/yyyy") + "]";
                }
                else
                {
                    pricelist.name += "CALENDARIO";
                }
            };

            return pricelist;
        }

        private void CreatePriceListDTO(PriceList pricelist)
        {
            var priceListDTO = new PriceListDTO
            {
                id = pricelist.id,
                name = pricelist.name,
                id_state = pricelist.Document.id_documentState,
                state = pricelist.Document.DocumentState.name,
                startDate = pricelist.startDate,
                endDate = pricelist.endDate,
                id_processtype = pricelist.id_processtype,
                code_processtype = pricelist.ProcessType?.code,
                id_periodoCalendario = pricelist.id_calendarPriceList != 0 ? pricelist.id_calendarPriceList : (int?)null,
                id_comprador = pricelist.id_userResponsable,
                isQuotation = pricelist.isQuotation,
                id_certification = pricelist.id_certification,
                certification = pricelist.Certification != null ? pricelist.Certification.name : "",
                id_priceListBase = pricelist.id_priceListBase,
                state_code = pricelist.Document.DocumentState.code
            };

            ViewBag.IsApprovalGerencial = priceListDTO.state_code == "15";

            if (priceListDTO.state_code == "15")
            {
                priceListDTO.userAproval = db.User.Where(s => s.id == pricelist.Document.id_userUpdate).FirstOrDefault().Employee.Person.fullname_businessName;
            }


            SetPriceListDTO(priceListDTO);
            BuildPriceListDtoDetails(pricelist.id_processtype, pricelist.id_priceListBase);
            PriceListDtoPenality();

            if (pricelist.id != 0)
            {
                priceListDTO.id_priceListBase = pricelist.id_priceListBase;
            }

            if (!priceListDTO.isQuotation)
                return;

            if (pricelist.id == 0)
                priceListDTO.id_priceListBase = pricelist.id_priceListBase;

            priceListDTO.paraProveedor = !pricelist.byGroup ?? (pricelist.isQuotation ? true : (bool?)null);
            priceListDTO.paraGrupo = pricelist.byGroup ?? (pricelist.isQuotation ? false : (bool?)null);

            if (priceListDTO.paraGrupo != null && priceListDTO.paraGrupo.Value)
            {
                priceListDTO.id_grupo = pricelist.id_groupPersonByRol;
            }
            else
            {
                priceListDTO.id_proveedor = pricelist.PriceListPersonPersonRol.FirstOrDefault()?.id_Person;
            }

            SetPriceListDTO(priceListDTO);
        }

        [HttpPost]
        public JsonResult InfoListaBase(int? id_priceListBase)
        {
            var result = new ApiResult();

            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var priceListBase = db.PriceList.FirstOrDefault(p => p.id == id_priceListBase);
                        if (priceListBase != null)
                        {
                            result.Data = priceListBase.id_processtype.ToString();
                        }
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                    }
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void BuildPriceListDtoDetails(int? id_processtype = null, int? id_priceListBase = null)
        {
            //Construir los detalles a partir de la informacion de la base de datos
            using (var db = new DBContext())
            {
                var aOrderClassShrimp = db.ClassShrimp.Where(fod => fod.isActive).ToList().OrderBy(ob => ob.order).ToArray();
                ViewBag.OrderClassShrimp = aOrderClassShrimp.Select(s => new SelectListItem { Text = s.description, Value = s.code/*, Value = s.id.ToString()*/ }).ToList();
                //ViewBag.OrderClassShrimp = aOrderClassShrimp.Select(s => new { s.order, s.name, s.code, s.description });
                var priceListDTO = GetPriceListDTO();
                if (priceListDTO.id != 0 && (priceListDTO.ListPriceListDetailDTO?.Count ?? 0) == 0)
                {

                    var priceListItemSize = db.PriceListItemSizeDetail
                                                    .Where(d => d.Id_PriceList == priceListDTO.id)
                                                    .GroupBy(gb => new
                                                    {
                                                        gb.Id_Itemsize,
                                                        gb.id_class
                                                    })
                                                    .Select(r => new
                                                    {
                                                        Id_Itemsize = r.Key.Id_Itemsize,
                                                        id_class = r.Key.id_class,

                                                    }).ToList();

                    int[] _itemsizesIds = priceListItemSize
                                            .GroupBy(r=> r.Id_Itemsize)
                                            .Select(r => r.Key)
                                            .ToArray();
                    int[] _classIds = priceListItemSize
                                            .GroupBy(r => r.id_class)
                                            .Select(r => r.Key)
                                            .ToArray();

                    var _itemSizesList = db.ItemSize
                                            .Where(r => _itemsizesIds.Contains(r.id))
                                            .ToList();
                    var _classList = db.Class
                                          .Where(r => _classIds.Contains(r.id))
                                          .ToList();
                    var _itemSizeProcessList = db.ItemSizeProcessPLOrder
                                                        .Where(r => _itemsizesIds.Contains(r.id_ItemSize))
                                                        .GroupBy(r=> new { r.id_ItemSize, r.id_ProcessType } )
                                                        .Select(r=>  new 
                                                        {
                                                            id_ItemSize = r.Key.id_ItemSize,
                                                            id_ProcessType = r.Key.id_ProcessType
                                                        })
                                                        .ToList();

                    int[] _processTypeIds = _itemSizeProcessList
                                                    .Where(r=> r.id_ProcessType.HasValue)
                                                    .GroupBy(r=> r.id_ProcessType.Value)
                                                    .Select(r => r.Key)
                                                    .ToArray();

                    var _processTypeList = db.ProcessType
                                                .Where(r => _processTypeIds.Contains(r.id))
                                                .ToList();


                    priceListDTO.ListPriceListDetailDTO = (from priceList in priceListItemSize
                                                           join itemSize in _itemSizesList
                                                           on priceList.Id_Itemsize equals itemSize.id
                                                           into _tmpitemSize
                                                           from dfItemSize in _tmpitemSize.DefaultIfEmpty()
                                                           join classS in _classList
                                                           on priceList.id_class equals classS.id
                                                           into _tmpclassS
                                                           from dfClassS in _tmpclassS.DefaultIfEmpty()
                                                           join itemsizeProcess in _itemSizeProcessList
                                                           on priceList.Id_Itemsize equals itemsizeProcess.id_ItemSize
                                                           into _tmpitemsizeProcess
                                                           from dfItemsizeProcess in _tmpitemsizeProcess.DefaultIfEmpty()
                                                           join processType in _processTypeList
                                                           on dfItemsizeProcess.id_ProcessType equals processType.id
                                                           into _tmpprocessType
                                                           from dfProcessType in _tmpprocessType.DefaultIfEmpty()
                                                           select new PriceListDetailDTO
                                                           {
                                                               idItemSize = priceList.Id_Itemsize,
                                                               name = (dfItemSize!= null) ? dfItemSize.name : string.Empty,
                                                               order = (dfItemSize != null) ? dfItemSize.orderSize : 0,
                                                               codeType = (dfProcessType != null)? dfProcessType.code : string.Empty,
                                                               type = (dfProcessType != null) ? dfProcessType.name : string.Empty,
                                                               price = 0,
                                                               commission = 0,
                                                               pricePurchase = 0,//i.price - i.commission,
                                                               basePrice = 0,
                                                               distint = 0,
                                                               id_Class = priceList.id_class,
                                                               codeClass = (dfClassS != null)? dfClassS.code  : string.Empty,
                                                               nameClass = (dfClassS != null) ? dfClassS.description : string.Empty,
                                                           })
                                                           .ToList()?? new List<PriceListDetailDTO>();


                    //priceListDTO.ListPriceListDetailDTO = db.PriceListItemSizeDetail
                    //    .Where(d => d.Id_PriceList == priceListDTO.id)
                    //    .GroupBy(gb => new
                    //    {
                    //        gb.Id_Itemsize,
                    //        gb.id_class
                    //    })
                    //    .Select(i => new PriceListDetailDTO
                    //    {
                    //        idItemSize = i.Key.Id_Itemsize,
                    //        name = db.ItemSize.FirstOrDefault(t => t.id == i.Key.Id_Itemsize).name,
                    //        order = db.ItemSize.FirstOrDefault(t => t.id == i.Key.Id_Itemsize).orderSize,
                    //        codeType = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == i.Key.Id_Itemsize)
                    //                ? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == i.Key.Id_Itemsize)
                    //                .ProcessType.code : "",
                    //        type = db.ItemSizeProcessPLOrder.Any(t => t.id_ItemSize == i.Key.Id_Itemsize)
                    //                ? db.ItemSizeProcessPLOrder.FirstOrDefault(t => t.id_ItemSize == i.Key.Id_Itemsize)
                    //                .ProcessType.name : "",
                    //        price = 0,
                    //        commission = 0,
                    //        pricePurchase = 0,//i.price - i.commission,
                    //        basePrice = 0,
                    //        distint = 0,
                    //        id_Class = i.Key.id_class,
                    //        codeClass = db.Class.FirstOrDefault(t => t.id == i.Key.id_class).code,
                    //        nameClass = db.Class.FirstOrDefault(t => t.id == i.Key.id_class).description
                    //    }).ToList() ?? new List<PriceListDetailDTO>();


                    int count = 0;
                    decimal priceLast = 0;
                    PriceListClassShrimpDTO aPriceListClassShrimpDTO = null;
                    foreach (var aListPriceListDetailDTO in priceListDTO.ListPriceListDetailDTO)
                    {
                        if (aListPriceListDetailDTO.codeType == "COL")
                        {
                            if (aListPriceListDetailDTO.codeClass == "PRIM")
                            {
                                aListPriceListDetailDTO.nameClass = "1." + aListPriceListDetailDTO.nameClass;
                            }
                            else
                            {
                                if (aListPriceListDetailDTO.codeClass == "BROK")
                                {
                                    aListPriceListDetailDTO.nameClass = "2." + aListPriceListDetailDTO.nameClass;
                                }
                                else
                                {
                                    if (aListPriceListDetailDTO.codeClass == "VETL")
                                    {
                                        aListPriceListDetailDTO.nameClass = "3." + aListPriceListDetailDTO.nameClass;
                                    }
                                }
                            }
                        }

                        count = 0;
                        foreach (var aClassShrimp in aOrderClassShrimp.ToArray())
                        {
                            var aPriceListItemSizeDetail = db.PriceListItemSizeDetail.FirstOrDefault(fod => fod.Id_Itemsize == aListPriceListDetailDTO.idItemSize &&
                                                                                                            fod.id_class == aListPriceListDetailDTO.id_Class &&
                                                                                                            fod.id_classShrimp == aClassShrimp.id &&
                                                                                                            fod.Id_PriceList == priceListDTO.id);
                            PriceListItemSizeDetail basePriceList = null;
                            if (aPriceListItemSizeDetail != null)
                            {
                                basePriceList = db.PriceListItemSizeDetail.FirstOrDefault(b => b.Id_PriceList == priceListDTO.id_priceListBase &&
                                                                                                      b.Id_Itemsize == aListPriceListDetailDTO.idItemSize &&
                                                                                                      b.id_class == aListPriceListDetailDTO.id_Class &&
                                                                                                      b.id_classShrimp == aClassShrimp.id);
                                var aPrice_RF = aPriceListItemSizeDetail.price_RF != 0 ? aPriceListItemSizeDetail.price_RF : (basePriceList?.price ?? 0);
                                aPriceListClassShrimpDTO = (new PriceListClassShrimpDTO
                                {
                                    id_classShrimp = aClassShrimp.id,
                                    classShrimp = aClassShrimp.code,
                                    price = aPriceListItemSizeDetail.price,
                                    commission = aPriceListItemSizeDetail.commission,
                                    price_RF = aPrice_RF,
                                    price_PC = aPriceListItemSizeDetail.price_PC,
                                    difference_F_RF = aPriceListItemSizeDetail.price - aPrice_RF,
                                    difference = priceLast - aPriceListItemSizeDetail.price
                                });
                            }
                            else
                            {
                                if (priceListDTO.id_priceListBase != id_priceListBase)
                                {
                                    if (id_priceListBase == null)
                                    {
                                        basePriceList = db.PriceListItemSizeDetail.FirstOrDefault(b => b.Id_PriceList == priceListDTO.id_priceListBase &&
                                                                                                      b.Id_Itemsize == aListPriceListDetailDTO.idItemSize &&
                                                                                                      b.id_class == aListPriceListDetailDTO.id_Class &&
                                                                                                      b.id_classShrimp == aClassShrimp.id);
                                        aPriceListClassShrimpDTO = (new PriceListClassShrimpDTO
                                        {
                                            id_classShrimp = aClassShrimp.id,
                                            classShrimp = aClassShrimp.code,
                                            price = basePriceList?.price ?? 0,
                                            commission = 0,
                                            price_RF = basePriceList?.price ?? 0,
                                            price_PC = basePriceList?.price ?? 0,
                                            difference_F_RF = 0,
                                            difference = priceLast - (basePriceList?.price ?? 0)
                                        });
                                    }
                                    else
                                    {
                                        basePriceList = db.PriceListItemSizeDetail.FirstOrDefault(b => b.Id_PriceList == id_priceListBase &&
                                                                                                     b.Id_Itemsize == aListPriceListDetailDTO.idItemSize &&
                                                                                                     b.id_class == aListPriceListDetailDTO.id_Class &&
                                                                                                     b.id_classShrimp == aClassShrimp.id);
                                        aPriceListClassShrimpDTO = (new PriceListClassShrimpDTO
                                        {
                                            id_classShrimp = aClassShrimp.id,
                                            classShrimp = aClassShrimp.code,
                                            price = basePriceList?.price ?? 0,
                                            commission = 0,
                                            price_RF = basePriceList?.price ?? 0,
                                            price_PC = basePriceList?.price ?? 0,
                                            difference_F_RF = 0,
                                            difference = priceLast - (basePriceList?.price ?? 0)
                                        });
                                    }
                                }
                                else
                                {
                                    basePriceList = db.PriceListItemSizeDetail.FirstOrDefault(b => b.Id_PriceList == priceListDTO.id_priceListBase &&
                                                                                                      b.Id_Itemsize == aListPriceListDetailDTO.idItemSize &&
                                                                                                      b.id_class == aListPriceListDetailDTO.id_Class &&
                                                                                                      b.id_classShrimp == aClassShrimp.id);
                                    aPriceListClassShrimpDTO = (new PriceListClassShrimpDTO
                                    {
                                        id_classShrimp = aClassShrimp.id,
                                        classShrimp = aClassShrimp.code,
                                        price = basePriceList?.price ?? 0,
                                        commission = 0,
                                        price_RF = basePriceList?.price ?? 0,
                                        price_PC = basePriceList?.price ?? 0,
                                        difference_F_RF = 0,
                                        difference = priceLast - (basePriceList?.price ?? 0)
                                    });
                                }
                            }

                            switch (count)
                            {
                                case 0:
                                    aListPriceListDetailDTO.D0 = aPriceListClassShrimpDTO;
                                    break;
                                case 1:
                                    aListPriceListDetailDTO.D1 = aPriceListClassShrimpDTO;
                                    break;
                                case 2:
                                    aListPriceListDetailDTO.D2 = aPriceListClassShrimpDTO;
                                    break;
                                case 3:
                                    aListPriceListDetailDTO.D3 = aPriceListClassShrimpDTO;
                                    break;
                                case 4:
                                    aListPriceListDetailDTO.D4 = aPriceListClassShrimpDTO;
                                    break;
                                case 5:
                                    aListPriceListDetailDTO.D5 = aPriceListClassShrimpDTO;
                                    break;
                                case 6:
                                    aListPriceListDetailDTO.D6 = aPriceListClassShrimpDTO;
                                    break;
                                case 7:
                                    aListPriceListDetailDTO.D7 = aPriceListClassShrimpDTO;
                                    break;
                                case 8:
                                    aListPriceListDetailDTO.D8 = aPriceListClassShrimpDTO;
                                    break;
                                default:
                                    break;
                            }
                            priceLast = aPriceListClassShrimpDTO.price;
                            count++;
                        }
                    }
                }

                //Contruir el detalle como deberia quedar en caso que sea nuevo o cambie
                if (priceListDTO.id_processtype != id_processtype || priceListDTO.ListPriceListDetailDTO == null || priceListDTO.id_priceListBase != id_priceListBase)
                    priceListDTO.ListPriceListDetailDTO = new List<PriceListDetailDTO>();

                var itemSizes = new List<PriceListDetailDTO>();
                if (id_processtype != null)
                {
                    var procesType = db.ProcessType.FirstOrDefault(p => p.id == id_processtype);
                    if (procesType.code.Equals("ENT"))
                    {
                        itemSizes = db.ItemSizeProcessPLOrder
                            .Where(d => d.ProcessType.code.Equals("ENT") || d.ProcessType.code.Equals("COL") && (bool)d.isActive)
                            .Select(i => new PriceListDetailDTO
                            {
                                idItemSize = i.id_ItemSize,
                                name = i.ItemSize.name,
                                order = i.ItemSize.orderSize,
                                codeType = i.ProcessType.code,
                                type = i.ProcessType.name,
                                price = (decimal)i.priceDefault,
                                basePrice = (decimal)i.priceDefault,
                                distint = 0,
                                commission = 0,
                                pricePurchase = 0,
                                id_Class = i.id_class,
                                codeClass = db.Class.FirstOrDefault(t => t.id == i.id_class).code,
                                nameClass = db.Class.FirstOrDefault(t => t.id == i.id_class).description
                                //ListPriceListClassShrimpDTO = null//new List<PriceListClassShrimpDTO>()
                            }).ToList();
                    }
                    else if (procesType.code.Equals("COL"))
                    {
                        itemSizes = db.ItemSizeProcessPLOrder
                            .Where(d => d.ProcessType.code.Equals("COL") && (bool)d.isActive)
                            .Select(i => new PriceListDetailDTO
                            {
                                idItemSize = i.id_ItemSize,
                                name = i.ItemSize.name,
                                order = i.ItemSize.orderSize,
                                codeType = i.ProcessType.code,
                                type = i.ProcessType.name,
                                price = (decimal)i.priceDefault,
                                basePrice = (decimal)i.priceDefault,
                                distint = 0,
                                commission = 0,
                                pricePurchase = 0,
                                id_Class = i.id_class,
                                codeClass = db.Class.FirstOrDefault(t => t.id == i.id_class).code,
                                nameClass = db.Class.FirstOrDefault(t => t.id == i.id_class).description
                                //ListPriceListClassShrimpDTO = new List<PriceListClassShrimpDTO>()
                            }).ToList();
                    }
                    else if (procesType.code.Equals("ENC"))
                    {
                        itemSizes = db.ItemSizeProcessPLOrder
                            .Where(d => d.ProcessType.code.Equals("COL") && (bool)d.isActive)
                            .Select(i => new PriceListDetailDTO
                            {
                                idItemSize = i.id_ItemSize,
                                name = i.ItemSize.name,
                                order = i.ItemSize.orderSize,
                                codeType = i.ProcessType.code,
                                type = i.ProcessType.name,
                                price = (decimal)i.priceDefault,
                                basePrice = (decimal)i.priceDefault,
                                distint = 0,
                                commission = 0,
                                pricePurchase = 0,
                                id_Class = i.id_class,
                                codeClass = db.Class.FirstOrDefault(t => t.id == i.id_class).code,
                                nameClass = db.Class.FirstOrDefault(t => t.id == i.id_class).description
                                //ListPriceListClassShrimpDTO = null//new List<PriceListClassShrimpDTO>()
                            }).ToList();
                    }
                }

                int count2 = 0;
                decimal priceLast2 = 0;
                PriceListClassShrimpDTO aPriceListClassShrimpDTO2 = null;
                //Adicionar las nuevas tallas agregadas a la base despues de creada la lista
                foreach (var itemSize in itemSizes)
                {
                    var aListPriceListDetailDTO = priceListDTO.ListPriceListDetailDTO.FirstOrDefault(fod => fod.idItemSize == itemSize.idItemSize && fod.id_Class == itemSize.id_Class);
                    if (aListPriceListDetailDTO == null)
                    {
                        count2 = 0;
                        foreach (var aClassShrimp in aOrderClassShrimp)
                        {
                            PriceListItemSizeDetail basePriceList = null;
                            if (priceListDTO.id_priceListBase != id_priceListBase)
                            {
                                if (id_priceListBase == null)
                                {
                                    basePriceList = db.PriceListItemSizeDetail.FirstOrDefault(b => b.Id_PriceList == priceListDTO.id_priceListBase &&
                                                                                                  b.Id_Itemsize == itemSize.idItemSize &&
                                                                                                  b.id_class == itemSize.id_Class &&
                                                                                                  b.id_classShrimp == aClassShrimp.id);
                                    aPriceListClassShrimpDTO2 = (new PriceListClassShrimpDTO
                                    {
                                        id_classShrimp = aClassShrimp.id,
                                        classShrimp = aClassShrimp.code,
                                        price = basePriceList?.price ?? 0,
                                        commission = 0,
                                        price_RF = basePriceList?.price ?? 0,
                                        price_PC = basePriceList?.price ?? 0,
                                        difference_F_RF = 0,
                                        difference = priceLast2 - (basePriceList?.price ?? 0)
                                    });
                                    if (id_priceListBase == null)
                                    {
                                        if (aPriceListClassShrimpDTO2.price == 0 && aClassShrimp.code == "D0")
                                        {
                                            aPriceListClassShrimpDTO2.price = itemSize.price;
                                            aPriceListClassShrimpDTO2.difference = priceLast2 - itemSize.price;
                                        }
                                    }
                                }
                                else
                                {
                                    basePriceList = db.PriceListItemSizeDetail.FirstOrDefault(b => b.Id_PriceList == id_priceListBase &&
                                                                                                 b.Id_Itemsize == itemSize.idItemSize &&
                                                                                                 b.id_class == itemSize.id_Class &&
                                                                                                 b.id_classShrimp == aClassShrimp.id);
                                    aPriceListClassShrimpDTO2 = (new PriceListClassShrimpDTO
                                    {
                                        id_classShrimp = aClassShrimp.id,
                                        classShrimp = aClassShrimp.code,
                                        price = basePriceList?.price ?? 0,
                                        commission = 0,
                                        price_RF = basePriceList?.price ?? 0,
                                        price_PC = basePriceList?.price ?? 0,
                                        difference_F_RF = 0,
                                        difference = priceLast2 - (basePriceList?.price ?? 0)
                                    });
                                }
                            }
                            else
                            {
                                basePriceList = db.PriceListItemSizeDetail.FirstOrDefault(b => b.Id_PriceList == priceListDTO.id_priceListBase &&
                                                                                                  b.Id_Itemsize == itemSize.idItemSize &&
                                                                                                  b.id_class == itemSize.id_Class &&
                                                                                                  b.id_classShrimp == aClassShrimp.id);
                                aPriceListClassShrimpDTO2 = (new PriceListClassShrimpDTO
                                {
                                    id_classShrimp = aClassShrimp.id,
                                    classShrimp = aClassShrimp.code,
                                    price = basePriceList?.price ?? 0,
                                    commission = 0,
                                    price_RF = basePriceList?.price ?? 0,
                                    price_PC = basePriceList?.price ?? 0,
                                    difference_F_RF = 0,
                                    difference = priceLast2 - (basePriceList?.price ?? 0)
                                });
                                if (id_priceListBase == null)
                                {
                                    if (aPriceListClassShrimpDTO2.price == 0 && aClassShrimp.code == "D0")
                                    {
                                        aPriceListClassShrimpDTO2.price = itemSize.price;
                                        aPriceListClassShrimpDTO2.difference = priceLast2 - itemSize.price;
                                    }
                                }
                            }
                            switch (count2)
                            {
                                case 0:
                                    itemSize.D0 = aPriceListClassShrimpDTO2;
                                    break;
                                case 1:
                                    itemSize.D1 = aPriceListClassShrimpDTO2;
                                    break;
                                case 2:
                                    itemSize.D2 = aPriceListClassShrimpDTO2;
                                    break;
                                case 3:
                                    itemSize.D3 = aPriceListClassShrimpDTO2;
                                    break;
                                case 4:
                                    itemSize.D4 = aPriceListClassShrimpDTO2;
                                    break;
                                case 5:
                                    itemSize.D5 = aPriceListClassShrimpDTO2;
                                    break;
                                case 6:
                                    itemSize.D6 = aPriceListClassShrimpDTO2;
                                    break;
                                case 7:
                                    itemSize.D7 = aPriceListClassShrimpDTO2;
                                    break;
                                case 8:
                                    itemSize.D8 = aPriceListClassShrimpDTO2;
                                    break;
                                default:
                                    break;
                            }
                            priceLast2 = aPriceListClassShrimpDTO2.price;
                            count2++;
                        }
                        if (itemSize.codeType == "COL")
                        {
                            if (itemSize.codeClass == "PRIM")
                            {
                                itemSize.nameClass = "1." + itemSize.nameClass;
                            }
                            else
                            {
                                if (itemSize.codeClass == "BROK")
                                {
                                    itemSize.nameClass = "2." + itemSize.nameClass;
                                }
                                else
                                {
                                    if (itemSize.codeClass == "VETL")
                                    {
                                        itemSize.nameClass = "3." + itemSize.nameClass;
                                    }
                                }
                            }
                        }
                        priceListDTO.ListPriceListDetailDTO.Add(itemSize);
                    }
                }

                //            if (priceListDTO.id_priceListBase != id_priceListBase)
                //{
                //	if (id_priceListBase == null)
                //	{
                //		foreach (var detail in priceListDTO.ListPriceListDetailDTO)
                //			detail.basePrice = 0;
                //	}
                //	else
                //	{
                //		var basePriceList = db.PriceListItemSizeDetail.Where(b => b.Id_PriceList == id_priceListBase);
                //		if (priceListDTO.id == 0)
                //		{
                //			foreach (var detail in priceListDTO.ListPriceListDetailDTO)
                //			{
                //				detail.price = basePriceList.FirstOrDefault(b => b.Id_Itemsize == detail.idItemSize)?.price ?? 0;
                //				detail.basePrice = basePriceList.FirstOrDefault(b => b.Id_Itemsize == detail.idItemSize)?.price ?? 0;
                //				detail.distint = 0;
                //				detail.pricePurchase = detail.price - detail.commission;
                //			}
                //		}
                //		else
                //		{
                //			foreach (var detail in priceListDTO.ListPriceListDetailDTO)
                //			{
                //				detail.price = detail.price;
                //				detail.basePrice = basePriceList.FirstOrDefault(b => b.Id_Itemsize == detail.idItemSize)?.price ?? 0;
                //				detail.distint = detail.price - detail.basePrice;
                //				detail.pricePurchase = detail.price - detail.commission;
                //			}
                //		}
                //	}
                //}
            }
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewPriceListDetailsENT(int? id_processtype = null, bool isQuotation = false,
                                                    int? id_priceListBase = null, bool enabled = true,
                                                    bool IsOwner = true, string code_processtype = "", bool isClassB = false)
        {
            return GridViewPriceListDetails(id_processtype, isQuotation, id_priceListBase, enabled,
                                                    IsOwner, code_processtype, isClassB);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewPriceListDetailsCOL(int? id_processtype = null, bool isQuotation = false,
                                                    int? id_priceListBase = null, bool enabled = true,
                                                    bool IsOwner = true, string code_processtype = "", bool isClassB = false)
        {
            return GridViewPriceListDetails(id_processtype, isQuotation, id_priceListBase, enabled,
                                                    IsOwner, code_processtype, isClassB);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewPriceListDetailsCOLB(int? id_processtype = null, bool isQuotation = false,
                                                    int? id_priceListBase = null, bool enabled = true,
                                                    bool IsOwner = true, string code_processtype = "", bool isClassB = true)
        {
            return GridViewPriceListDetails(id_processtype, isQuotation, id_priceListBase, enabled,
                                                    IsOwner, code_processtype, isClassB);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewPriceListDetails(int? id_processtype = null, bool isQuotation = false,
                                                    int? id_priceListBase = null, bool enabled = true,
                                                    bool IsOwner = true, string code_processtype = "", bool isClassB = false)
        {
            var priceListDTO = GetPriceListDTO();

            //el modelo que esta en la vista no es el mismo que se necesita
            if (priceListDTO.id_processtype != id_processtype ||
                priceListDTO.id_priceListBase != id_priceListBase)
            {
                BuildPriceListDtoDetails(id_processtype, id_priceListBase);
            }

            priceListDTO.id_priceListBase = id_priceListBase;
            priceListDTO.id_processtype = id_processtype;
            priceListDTO.isQuotation = isQuotation;

            var aOrderClassShrimp = db.ClassShrimp.Where(fod => fod.isActive).OrderBy(ob => ob.order).ToArray();
            ViewBag.OrderClassShrimp = aOrderClassShrimp.Select(s => new SelectListItem { Text = s.description, Value = s.code/*, Value = s.id.ToString()*/ }).ToList();

            //ViewBag.CanReplicateDetailsCOL = code_processtype == "COL" && enabled;
            //ViewBag.CanReplicateDetailsENT = code_processtype == "ENT" && enabled;

            ViewBag.code_processtype = code_processtype;
            ViewBag.id_processtype = priceListDTO.id_processtype;
            ViewBag.isQuotation = priceListDTO.isQuotation;
            ViewBag.id_priceListBase = priceListDTO.id_priceListBase;
            ViewBag.enabled = enabled;
            ViewBag.IsOwner = IsOwner;

            int nviewall = 0;
            string nvalViewall = db.Setting.FirstOrDefault(r => r.code == "VWPP")?.value ?? "0";
            int.TryParse(nvalViewall, out nviewall);
            ViewBag.viewall = nviewall;

            var aSelectListItemLCCBLP = db.SettingDetail.Where(fod => fod.Setting.code.Equals("LCCBLP")).Select(s => new SelectListItem { Text = s.value, Value = s.id.ToString() }).ToList();
            if (isClassB)
            {
                //var apriceListDTO = priceListDTO.ListPriceListDetailDTO.Where(w => w.codeType != "ENT" && aSelectListItemLCCBLP.FirstOrDefault(fod => fod.Text == w.codeClass) != null).OrderBy(ob => ob.order).ToList();
                return PartialView("_GridViewPriceListDetails" + code_processtype + "B", priceListDTO.ListPriceListDetailDTO.Where(w => w.codeType != "ENT" && aSelectListItemLCCBLP.FirstOrDefault(fod => fod.Text == w.codeClass) != null).OrderBy(ob => ob.order)/*.ThenBy(ob => ob.order)*/);

            }
            else
            {
                //var apriceListDTO = priceListDTO.ListPriceListDetailDTO.Where(w => code_processtype == "ENT" ? w.codeType == code_processtype : (w.codeType != "ENT" && aSelectListItemLCCBLP.FirstOrDefault(fod => fod.Text == w.codeClass) == null)).OrderBy(ob => ob.order).ToList();
                return PartialView("_GridViewPriceListDetails" + code_processtype, priceListDTO.ListPriceListDetailDTO.Where(w => code_processtype == "ENT" ? w.codeType == code_processtype : (w.codeType != "ENT" && aSelectListItemLCCBLP.FirstOrDefault(fod => fod.Text == w.codeClass) == null)).OrderBy(ob => ob.order)/*.ThenBy(ob => ob.order)*/);
            }

        }

        public void PriceListDtoPenality()
        {
            using (var db = new DBContext())
            {
                var priceListDTO = GetPriceListDTO();

                priceListDTO.ListPriceListPenaltyDTO = new List<PriceListPenaltyDTO>();
                if (priceListDTO.id != 0)
                {
                    priceListDTO.ListPriceListPenaltyDTO = db.PriceListClassShrimp.Where(d => d.id_priceList == priceListDTO.id)
                    .Select(i => new PriceListPenaltyDTO
                    {
                        id_classShrimp = i.id_classShrimp,
                        classShrimp = i.ClassShrimp.name,
                        value = i.value,
                        order = i.ClassShrimp.order,
                    }).ToList();
                }

                var listClassShrimp = new List<PriceListPenaltyDTO>();
                listClassShrimp = db.ClassShrimp.Where(p => p.isActive).Select(i => new PriceListPenaltyDTO
                {
                    id_classShrimp = i.id,
                    classShrimp = i.name,
                    value = 0,
                    order = i.order,
                }).ToList();

                foreach (var classShrimp in listClassShrimp)
                {
                    bool isIn = false;
                    foreach (var detailClassShrimp in priceListDTO.ListPriceListPenaltyDTO)
                    {
                        if (classShrimp.id_classShrimp == detailClassShrimp.id_classShrimp)
                        {
                            isIn = true;
                            break;
                        }
                    }
                    if (!isIn)
                        priceListDTO.ListPriceListPenaltyDTO.Add(classShrimp);
                }
            }
        }

        private void BuildPriceListDtoPenality(int? id_provider = null, int? id_group = null)
        {
            using (var db = new DBContext())
            {
                var priceListDTO = GetPriceListDTO();

                priceListDTO.ListPriceListPenaltyDTO = new List<PriceListPenaltyDTO>();
                priceListDTO.id_proveedor = id_provider;
                priceListDTO.id_grupo = id_group;

                var listClassShrimp = new List<PriceListPenaltyDTO>();
                listClassShrimp = db.ClassShrimp.Where(p => p.isActive).Select(i => new PriceListPenaltyDTO
                {
                    id_classShrimp = i.id,
                    classShrimp = i.name,
                    value = 0,
                    order = i.order,
                }).ToList();

                foreach (var classShrimp in listClassShrimp)
                {
                    bool isIn = false;
                    foreach (var detailClassShrimp in priceListDTO.ListPriceListPenaltyDTO)
                    {
                        if (classShrimp.id_classShrimp == detailClassShrimp.id_classShrimp)
                        {
                            isIn = true;
                            break;
                        }
                    }
                    if (!isIn)
                        priceListDTO.ListPriceListPenaltyDTO.Add(classShrimp);
                }

                List<PenalityClassShrimpDetails> penalityDetails = null;
                if (id_group != null)
                    penalityDetails = db.PenalityClassShrimpDetails.Where(p => p.PenalityClassShrimp.id_groupPersonByRol == id_group).ToList();
                else if (id_provider != null)
                    penalityDetails = db.PenalityClassShrimpDetails.Where(p => p.PenalityClassShrimp.id_provider == id_provider).ToList();

                if (penalityDetails != null)
                {
                    foreach (var detail in priceListDTO.ListPriceListPenaltyDTO)
                        detail.value = penalityDetails.FirstOrDefault(b => b.id_classShrimp == detail.id_classShrimp)?.value ?? 0;
                }
                else
                {
                    foreach (var detail in priceListDTO.ListPriceListPenaltyDTO)
                        detail.value = 0;
                }
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GridViewPriceListPenalty(int? id_provider = null, int? id_group = null,
                                                     bool enabled = true, bool IsOwner = true,
                                                     bool isQuotation = false)
        {
            var priceListDTO = GetPriceListDTO();

            if (priceListDTO.id_proveedor != id_provider || priceListDTO.id_grupo != id_group)
            {
                BuildPriceListDtoPenality(id_provider, id_group);
            }

            ViewBag.id_group = id_group;
            ViewBag.id_provider = id_provider;
            ViewBag.enabled = enabled;
            ViewBag.IsOwner = IsOwner;
            ViewBag.isQuotation = isQuotation;

            return PartialView("_GridViewPriceListPenalty", priceListDTO.ListPriceListPenaltyDTO.OrderBy(ob => ob.order));
        }

        [HttpPost]
        public JsonResult Save(string jsonPriceList)
        {
            var result = new ApiResult();
            bool isSaved = false;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string _pathProgramReplication = ConfigurationManager.AppSettings["pathProgramReplication"];
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        JToken tokenPriceList = JsonConvert.DeserializeObject<JToken>(jsonPriceList);
                        var newObject = false;
                        var id = tokenPriceList.Value<int>("id");

                        FindSettingPriceList();

                        var priceList = db.PriceList.FirstOrDefault(p => p.id == id);
                        if (priceList == null)
                        {
                            newObject = true;

                            var documentType = db.DocumentType.FirstOrDefault(t => t.name.Equals("GENERAL - COMPRA"));

                            var id_emissionPoint = ActiveUser.EmissionPoint.Count > 0
                                ? ActiveUser.EmissionPoint.First().id
                                : 0;
                            if (id_emissionPoint == 0)
                                throw new Exception("Su usuario no tiene asociado ningún punto de emisión.");

                            priceList = new PriceList
                            {
                                isForPurchase = true,
                                isForSold = false,
                                id_company = 1,
                                Document = new Document
                                {
                                    number = GetDocumentSequential(documentType?.id ?? 0).ToString(),
                                    sequential = GetDocumentSequential(documentType?.id ?? 0),
                                    emissionDate = DateTime.Now,
                                    authorizationDate = DateTime.Now,
                                    id_emissionPoint = id_emissionPoint,
                                    id_documentType = documentType.id,
                                    id_userCreate = ActiveUser.id,
                                    dateCreate = DateTime.Now,
                                }
                            };

                            // PLU: Reasignar el número de secuencia para prevenir duplicados
                            var _setTmp = db.Setting
                                .FirstOrDefault(fod => fod.code.Equals("SLP05"));

                            if (_setTmp == null)
                            {
                                throw new ApplicationException("No se encontró el registro de secuencial de lista de precios.");
                            }

                            priceList.name = _setTmp.value + "." +
                                tokenPriceList.Value<string>("name").Split('.')[1];

                            _setTmp.value = (Convert.ToInt32(_setTmp.value) + 1).ToString();

                            db.Setting.Attach(_setTmp);
                            db.Entry(_setTmp).State = EntityState.Modified;
                        }
                        else
                        {
                            // Si ya existe la lista de precios, le dejamos su nombre
                            priceList.name = tokenPriceList.Value<string>("name");
                        }

                        priceList.Document.id_documentState = settingPriceList.id_crateState;
                        priceList.Document.id_userUpdate = ActiveUser.id;
                        priceList.Document.dateUpdate = DateTime.Now;

                        if (db.DocumentStateChange.FirstOrDefault(s => s.id_documentStateOld == settingPriceList.id_crateState &&
                                                                      s.id_documentStateNew == settingPriceList.id_crateState &&
                                                                      s.id_user == ActiveUser.id &&
                                                                      s.id_userGroup == ActiveUser.id_group) == null)
                        {
                            priceList.Document.DocumentStateChange.Add(new DocumentStateChange
                            {
                                id_documentStateOld = settingPriceList.id_crateState,
                                id_documentStateNew = settingPriceList.id_crateState,
                                id_user = ActiveUser.id,
                                id_userGroup = ActiveUser.id_group,
                                changeTime = DateTime.Now
                            });
                        }

                        if (newObject)
                        {
                            var stateAnnul = db.DocumentState.FirstOrDefault(s => s.code.Equals("05"));
                            var listaExistente = db.PriceList.Where(pl => pl.Document.id_documentState != stateAnnul.id).AsEnumerable()
                                                .FirstOrDefault(p => p.name.Split('.')[1] == priceList.name.Split('.')[1]);
                            if (listaExistente != null)
                            {
                                throw new Exception("Ya existe una lista de precio similar. " + listaExistente.name);
                            }
                        }

                        priceList.startDate = tokenPriceList.Value<DateTime>("startDate");
                        priceList.endDate = tokenPriceList.Value<DateTime>("endDate");
                        priceList.id_calendarPriceList = tokenPriceList.Value<int>("id_calendarPriceList");
                        priceList.id_processtype = tokenPriceList.Value<int?>("id_processtype");
                        priceList.id_userResponsable = tokenPriceList.Value<int>("id_userResponsable");
                        priceList.isQuotation = tokenPriceList.Value<bool>("isQuotation");

                        if (priceList.isQuotation)
                        {
                            priceList.id_priceListBase = tokenPriceList.Value<int?>("id_priceListBase");
                            priceList.byGroup = tokenPriceList.Value<bool?>("byGroup");
                            priceList.id_groupPersonByRol = tokenPriceList.Value<int?>("id_groupPersonByRol");
                            priceList.id_certification = tokenPriceList.Value<int?>("id_certification");

                            var providersAux = db.PriceListPersonPersonRol.Where(i => i.id_PriceList == priceList.id);
                            foreach (var provider in providersAux)
                            {
                                db.PriceListPersonPersonRol.Remove(provider);
                                db.PriceListPersonPersonRol.Attach(provider);
                                db.Entry(provider).State = EntityState.Deleted;
                            }

                            if (priceList.byGroup.Value)
                            {
                                var groupPersonByRolDetails = db.GroupPersonByRolDetail.Where(g => g.id_groupPersonByRol == priceList.id_groupPersonByRol);
                                foreach (var detail in groupPersonByRolDetails)
                                {
                                    priceList.PriceListPersonPersonRol.Add(new PriceListPersonPersonRol
                                    {
                                        id_Person = detail.id_person,
                                        id_Rol = db.Rol.FirstOrDefault(r => r.name.Equals("Proveedor")).id,
                                    });
                                }
                            }
                            else
                            {
                                var id_provider = tokenPriceList.Value<int?>("id_provider");
                                priceList.PriceListPersonPersonRol.Add(new PriceListPersonPersonRol
                                {
                                    id_Person = id_provider.Value,
                                    id_Rol = db.Rol.FirstOrDefault(r => r.name.Equals("Proveedor")).id,
                                });
                            }
                        }

                        //Detalles
                        var detailsAux = db.PriceListItemSizeDetail.Where(i => i.Id_PriceList == priceList.id);
                        foreach (var detail in detailsAux)
                        {
                            db.PriceListItemSizeDetail.Remove(detail);
                            db.PriceListItemSizeDetail.Attach(detail);
                            db.Entry(detail).State = EntityState.Deleted;
                        }

                        int NviewAll = 0;
                        var _NviewAll = db.Setting
                                            .FirstOrDefault(r => r.code == "VWPP")?.value??"0";
                        int.TryParse(_NviewAll , out NviewAll);
                        ViewBag.nViewallHidden = NviewAll;


                        JArray priceListDetails = tokenPriceList.Value<JArray>("priceListDetails");
                        foreach (JToken detail in priceListDetails)
                        {
                            var detailStrId = detail.Value<string>("id");
                            int detailId;
                            if (int.TryParse(detailStrId, out detailId) == false)
                                continue;

                            decimal detailCommission = (NviewAll==1)?0:detail.Value<decimal>("commission");
                            var detailPrice = detail.Value<decimal>("price");
                            var detailPrice_PC = (NviewAll == 1) ? 0 : detail.Value<decimal>("price_PC");
                            var detailPrice_RF = (NviewAll == 1) ? 0 : detail.Value<decimal>("price_RF");
                            //if (priceList.isQuotation)
                            //    detailCommission = detail.Value<decimal>("commission");
                            var detailStrCode_classShrimp = detail.Value<string>("code_classShrimp");
                            int detailId_classShrimp = db.ClassShrimp.FirstOrDefault(fod => fod.code == detailStrCode_classShrimp).id;
                            //if (int.TryParse(detailStrId_classShrimp, out detailId_classShrimp) == false)
                            //    continue;

                            var detailStrId_class = detail.Value<string>("id_class");
                            int detailId_class;
                            if (int.TryParse(detailStrId_class, out detailId_class) == false)
                                continue;


                            var aPriceListItemSizeDetail = new PriceListItemSizeDetail
                            {
                                Id_Itemsize = detailId,
                                Id_PriceList = priceList.id,
                                price = detailPrice,
                                price_PC = detailPrice_PC,
                                price_RF = detailPrice_RF,
                                commission = detailCommission,
                                id_classShrimp = detailId_classShrimp,
                                id_class = detailId_class
                            };
                            priceList.PriceListItemSizeDetail.Add(aPriceListItemSizeDetail);
                            if (priceList.id != 0)
                            {
                                db.PriceListItemSizeDetail.Add(aPriceListItemSizeDetail);
                                db.Entry(aPriceListItemSizeDetail).State = EntityState.Added;
                            }
                        }

                        //Penalizacion
                        var penaltyAux = db.PriceListClassShrimp.Where(i => i.id_priceList == priceList.id);
                        foreach (var penalty in penaltyAux)
                        {
                            db.PriceListClassShrimp.Remove(penalty);
                            db.PriceListClassShrimp.Attach(penalty);
                            db.Entry(penalty).State = EntityState.Deleted;
                        }

                        JArray priceListPenalty = tokenPriceList.Value<JArray>("priceListPenalty");
                        foreach (JToken penalty in priceListPenalty)
                        {
                            priceList.PriceListClassShrimp.Add(new PriceListClassShrimp
                            {
                                id_classShrimp = penalty.Value<int>("id_classShrimp"),
                                value = penalty.Value<decimal>("value")
                            });
                        }

                        if (newObject)
                        {
                            db.PriceList.Add(priceList);
                            db.Entry(priceList).State = EntityState.Added;
                        }
                        else
                        {
                            db.PriceList.Attach(priceList);
                            db.Entry(priceList).State = EntityState.Modified;
                        }

                        BuildViewData("Edit", priceList, true);

                        //db.SaveChanges(false);
                        db.SaveChanges();
                        trans.Commit();
                        isSaved = true;
                        result.Data = priceList.id.ToString();
                        MetodosEscrituraLogs.EscribeMensajeLog("save", ruta, "PersonUpdateReplication", app);
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }

                }
            }

            #region Replicate Person to PriceList
            try
            {
                if (isSaved)
                {
                    #region Replicate Person to PriceList
                    if (isSaved)
                    {
                        #region 
                        var startInfo = new ProcessStartInfo()
                        {
                            FileName = _pathProgramReplication,
                            Arguments = result.Data + " RLPTP ReplicateInformation",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        };

                        Process.Start(startInfo);
                        MetodosEscrituraLogs.EscribeMensajeLog("Replicate", ruta, "PersonUpdateReplication", app);
                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "PersonUpdateReplication", "PROD");
            }
            #endregion

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Approve(int id)
        {
            bool isSaved = false;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string _pathProgramReplication = ConfigurationManager.AppSettings["pathProgramReplication"];
            var result = new ApiResult();
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {


                    try
                    {
                        result.Data = ApprovePriceList(id).name;
                        isSaved = true;
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }


                }
            }

            #region Replicate Person to PriceList
            try
            {
                if (isSaved)
                {
                    #region Replicate Person to PriceList
                    if (isSaved)
                    {
                        #region 
                        var startInfo = new ProcessStartInfo()
                        {
                            FileName = _pathProgramReplication,
                            Arguments = id.ToString() + " RLPTP ReplicateInformation",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        };

                        Process.Start(startInfo);
                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "PersonUpdateReplication", "PROD");
            }
            #endregion

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private DocumentState ApprovePriceList(int id_priceList)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    User user = DataProviderUser.UserById(ActiveUser.id);
                    int id_userGroup = user.id_group;

                    var priceList = db.PriceList.FirstOrDefault(p => p.id == id_priceList);
                    if (priceList != null)
                    {
                        if (!priceList.PriceListItemSizeDetail.Any(d => d.price != 0))
                        {
                            throw new Exception("No puede aprobar una lista sin al menos un valor en la culumna precio");
                        }
                        var settingPPC = DataProviderSettings.Setting("PPC");
                        bool aPBLPM = settingPPC != null && settingPPC.value.Equals("SI");
                        if (priceList.PriceListClassShrimp.Any(c => c.value == 0) && !aPBLPM)
                        {
                            throw new Exception("No puede aprobar una lista de precio con una penalidad en cero");
                        }

                        var settingPriceList = db.SettingPriceList.FirstOrDefault(s => s.id_userGroupApproval == id_userGroup);
                        if (settingPriceList != null)
                        {
                            var aprovedState = db.DocumentState.FirstOrDefault(d => d.id == settingPriceList.id_aprovedState);
                            if (aprovedState != null)
                            {
                                priceList.Document.id_documentState = settingPriceList.id_aprovedState;
                                priceList.Document.authorizationDate = DateTime.Now;

                                priceList.Document.DocumentStateChange.Add(new DocumentStateChange
                                {
                                    id_documentStateOld = settingPriceList.id_crateState,
                                    id_documentStateNew = settingPriceList.id_aprovedState,
                                    id_user = ActiveUser.id,
                                    id_userGroup = ActiveUser.id_group,
                                    changeTime = DateTime.Now
                                });

                                db.PriceList.Attach(priceList);
                                db.Entry(priceList).State = EntityState.Modified;

                                db.SaveChanges();
                                trans.Commit();
                            }
                            else
                            {
                                throw new Exception("No encontro el estado de aprovación del usuario registrado en el Setting de Lista de Precio");
                            }
                        }
                        else
                        {
                            throw new Exception("No existe una configuración en el Setting de Lista de Precio para el usuario registrado");
                        }
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto lista de precio seleccionado");
                    }

                    return priceList.Document.DocumentState;
                }
            }
        }

        [HttpPost]
        public JsonResult Reverse(int id)
        {
            bool isSaved = false;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string _pathProgramReplication = ConfigurationManager.AppSettings["pathProgramReplication"];
            var result = new ApiResult();
            string _result = "";
            try
            {
                ServiceProductionPurchaseOrder.ServicePriceListClient proxy = new ServiceProductionPurchaseOrder.ServicePriceListClient();
                _result = proxy.GetPriceListInformation(id);
                if (_result != "")
                {
                    result.Code = -1;
                    result.Message = "NO SE PUEDE REVERSAR LA LISTA DE PRECIO POR ESTAR UTILIZADA EN LA(S) SIGUIENTE(S) ORDEN(ES) DE COMPRA(S) DEL SISTEMA DE PRODUCCIÓN: " + _result;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                proxy.Close();
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "RevertPriceList", "PROD");
            }

            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {


                    try
                    {
                        result.Data = ReversePriceList(id).name;
                        isSaved = true;
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }


                }
            }

            #region Replicate Person to PriceList
            try
            {
                if (isSaved)
                {
                    #region Replicate Person to PriceList
                    if (isSaved)
                    {
                        #region 
                        var startInfo = new ProcessStartInfo()
                        {
                            FileName = _pathProgramReplication,
                            Arguments = id.ToString() + " RLPTP ReplicateInformation",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        };

                        Process.Start(startInfo);
                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "PersonUpdateReplication", "PROD");
            }
            #endregion

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private DocumentState ReversePriceList(int id_priceList)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    User user = DataProviderUser.UserById(ActiveUser.id);
                    int id_userGroup = user.id_group;

                    var priceList = db.PriceList.FirstOrDefault(p => p.id == id_priceList);
                    if (priceList != null)
                    {
                        var settingPriceList = db.SettingPriceList.FirstOrDefault(s => s.id_userGroupApproval == id_userGroup);
                        if (settingPriceList != null)
                        {
                            var reversedState = db.DocumentState.FirstOrDefault(d => d.id == settingPriceList.id_reversedState);
                            if (reversedState != null)
                            {
                                priceList.Document.id_documentState = settingPriceList.id_reversedState;

                                priceList.Document.DocumentStateChange.Add(new DocumentStateChange
                                {
                                    id_documentStateOld = priceList.Document.id_documentState,
                                    id_documentStateNew = settingPriceList.id_reversedState,
                                    id_user = ActiveUser.id,
                                    id_userGroup = ActiveUser.id_group,
                                    changeTime = DateTime.Now
                                });

                                db.PriceList.Attach(priceList);
                                db.Entry(priceList).State = EntityState.Modified;

                                db.SaveChanges();
                                trans.Commit();
                            }
                            else
                            {
                                throw new Exception("No encontro el estado de reversión del usuario registrado en el Setting de Lista de Precio");
                            }
                        }
                        else
                        {
                            throw new Exception("No existe una configuración en el Setting de Lista de Precio para el usuario registrado");
                        }
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto lista de precio seleccionado");
                    }

                    return priceList.Document.DocumentState;
                }
            }
        }

        [HttpPost]
        public JsonResult Close(int id)
        {
            bool isSaved = false;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string _pathProgramReplication = ConfigurationManager.AppSettings["pathProgramReplication"];
            var result = new ApiResult();
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {


                    try
                    {
                        result.Data = ClosePriceList(id).name;
                        isSaved = true;
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }


                }
            }

            #region Replicate Person to PriceList
            try
            {
                if (isSaved)
                {
                    #region Replicate Person to PriceList
                    if (isSaved)
                    {
                        #region 
                        var startInfo = new ProcessStartInfo()
                        {
                            FileName = _pathProgramReplication,
                            Arguments = id.ToString() + " RLPTP ReplicateInformation",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        };

                        Process.Start(startInfo);
                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "PersonUpdateReplication", "PROD");
            }
            #endregion

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private DocumentState ClosePriceList(int id_priceList)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var priceList = db.PriceList.FirstOrDefault(p => p.id == id_priceList);
                    if (priceList != null)
                    {
                        var stateAprobadoGerente = db.DocumentState.FirstOrDefault(s => s.code.Equals("15"));
                        var ListaAprobadoGerente = priceList.Document.id_documentState;
                        if (stateAprobadoGerente.id == ListaAprobadoGerente)
                        {
                            var stateClose = db.DocumentState.FirstOrDefault(s => s.code.Equals("04"));
                            priceList.Document.id_documentState = stateClose.id;
                            //priceList.endDate = DateTime.Now;

                            priceList.Document.DocumentStateChange.Add(new DocumentStateChange
                            {
                                id_documentStateOld = priceList.Document.id_documentState,
                                id_documentStateNew = stateClose.id,
                                id_user = ActiveUser.id,
                                id_userGroup = ActiveUser.id_group,
                                changeTime = DateTime.Now
                            });

                            db.PriceList.Attach(priceList);
                            db.Entry(priceList).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                        }
                        else
                        {
                            throw new Exception("Lista de Precio debe de estar en estado APROBADO GERENCIA GENERAL");
                        }
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto lista de precio seleccionado");
                    }

                    return priceList.Document.DocumentState;
                }
            }
        }

        [HttpPost]
        public JsonResult Annul(int id)
        {
            bool isSaved = false;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            string _pathProgramReplication = ConfigurationManager.AppSettings["pathProgramReplication"];
            var result = new ApiResult();
            string _result = "";
            try
            {
                ServiceProductionPurchaseOrder.ServicePriceListClient proxy = new ServiceProductionPurchaseOrder.ServicePriceListClient();
                _result = proxy.GetPriceListInformation(id);
                if (_result != "")
                {
                    result.Code = -1;
                    result.Message = "NO SE PUEDE ANULAR LA LISTA DE PRECIO POR ESTAR UTILIZADA EN LA(S) SIGUIENTE(S) ORDEN(ES) DE COMPRA(S) DEL SISTEMA DE PRODUCCIÓN: " + _result;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                proxy.Close();
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "AnnulPriceList", "PROD");
            }

            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {


                    try
                    {
                        result.Data = AnnulPriceList(id).name;
                        isSaved = true;
                    }
                    catch (Exception e)
                    {
                        result.Code = e.HResult;
                        result.Message = e.Message;
                        trans.Rollback();
                    }

                }
            }

            #region Replicate Person to PriceList
            try
            {
                if (isSaved)
                {
                    #region Replicate Person to PriceList
                    if (isSaved)
                    {
                        #region 
                        var startInfo = new ProcessStartInfo()
                        {
                            FileName = _pathProgramReplication,
                            Arguments = id.ToString() + " RLPTP ReplicateInformation",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        };

                        Process.Start(startInfo);
                        #endregion
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "PersonUpdateReplication", "PROD");
            }
            #endregion

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private DocumentState AnnulPriceList(int id_priceList)
        {
            using (var db = new DBContext())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    var priceList = db.PriceList.FirstOrDefault(p => p.id == id_priceList);
                    if (priceList != null)
                    {
                        var annulClose = db.DocumentState.FirstOrDefault(s => s.code.Equals("05"));
                        priceList.Document.id_documentState = annulClose.id;

                        priceList.Document.DocumentStateChange.Add(new DocumentStateChange
                        {
                            id_documentStateOld = priceList.Document.id_documentState,
                            id_documentStateNew = annulClose.id,
                            id_user = ActiveUser.id,
                            id_userGroup = ActiveUser.id_group,
                            changeTime = DateTime.Now
                        });

                        db.PriceList.Attach(priceList);
                        db.Entry(priceList).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                    else
                    {
                        throw new Exception("No se encontro el objeto lista de precio seleccionado");
                    }

                    return priceList.Document.DocumentState;
                }
            }
        }

        [HttpPost]
        public JsonResult GetCodeProcessType(int? id_processtype)
        {
            var aProcessType = db.ProcessType.FirstOrDefault(fod => fod.id == id_processtype);

            var result = new { code_processtype = aProcessType?.code ?? "" };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetIdLoteCertificacion(int? id_certification)
        {
            var aCertification = db.Certification.FirstOrDefault(fod => fod.id == id_certification);

            var result = new { IdLote = aCertification?.idLote ?? "" };

            return Json(result, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult RefreshReplicate(int id)
        {
            var result = new ApiResult();

            try
            {
                #region Replicate Person to PriceList
                string _pathProgramReplication = ConfigurationManager.AppSettings["pathProgramReplication"];

                var startInfo = new ProcessStartInfo()
                {
                    FileName = _pathProgramReplication,
                    Arguments = id.ToString() + " RLPTP ReplicateInformation",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                Process.Start(startInfo);
                #endregion
            }
            catch (Exception e)
            {
                result.Code = e.HResult;
                result.Message = e.Message;
            }


            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult PrintPriceListReport(int id_priceList)
        {
            #region Armo Parametros
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id";
            _param.Valor = id_priceList;

            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = "LPV1FC";
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);


            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }
    }
}
