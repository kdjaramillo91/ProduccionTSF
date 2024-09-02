using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using DevExpress.Web.Mvc;
using DevExpress.Web;
using System.Web.UI.WebControls;
using DXPANACEASOFT.DataProviders;
using DevExpress.Utils;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
	public class AccountingTemplateCostController : DefaultController
	{

        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        [ValidateInput(false)]
        public ActionResult AccountingTemplateCostPartial()
        {
            AccountingTemplate accountingTemplate = (TempData["accountingTemplate"] as AccountingTemplate);
            if (accountingTemplate != null) TempData.Remove("accountingTemplate");


            int? _id_accountingTemplate = (int?)TempData["id_accountingTemplateType"];
            if (_id_accountingTemplate != null || _id_accountingTemplate != 0) TempData.Remove("id_accountingTemplateType");


            var model = db.AccountingTemplate.OrderByDescending(p => p.id);

            return PartialView("_AccountingTemplateCostPartial", model.ToList());
        }


        [HttpPost]
        public ActionResult DeleteSelectedAccountingTemplateCost(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var modelAccountingTemplateCost = db.AccountingTemplate.Where(i => ids.Contains(i.id));
                        foreach (var accountingTemplateCost in modelAccountingTemplateCost)
                        {
                            accountingTemplateCost.isActive = false;

                            accountingTemplateCost.id_userUpdate = ActiveUser.id;
                            accountingTemplateCost.dateUpdate = DateTime.Now;

                            db.AccountingTemplate.Attach(accountingTemplateCost);
                            db.Entry(accountingTemplateCost).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();

                        ViewData["EditMessage"] = SuccessMessage("Plantilla Contable de Costos de Producción");
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        TempData.Keep("accountingTemplate");
                        TempData.Keep("id_accountingTemplateType");
                        ViewData["EditMessage"] = ErrorMessage();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.AccountingTemplate.Where(o => o.id_company == this.ActiveCompanyId);
            return PartialView("_AccountingTemplateCostPartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult AccountingTemplateCostPartialAddNew(DXPANACEASOFT.Models.AccountingTemplate accountingTemplate)
        {
            var model = db.AccountingTemplate;
            AccountingTemplate tempAccountingTemplateCost = (TempData["accountingTemplate"] as AccountingTemplate);

            using (DbContextTransaction trans = db.Database.BeginTransaction())
            {
                try
                {
                    //var dbCI = new DBContextIntegration();
                    
                    if (tempAccountingTemplateCost?.AccountLedger != null)
                    {
                        
                        accountingTemplate.AccountLedger = new List<AccountLedger>();

                        var accountingTemplateCostDetails = tempAccountingTemplateCost.AccountLedger.ToList();

                        foreach (var accountingTemplateCostDetail in accountingTemplateCostDetails)
                        {
                            int tmp1 = 0, tmp2 =0;
                            int.TryParse(accountingTemplateCostDetail.nameCenterCost, out tmp1);
                            int.TryParse(accountingTemplateCostDetail.nameCenterCost, out tmp2);
                            var tempaccountingTemplateCostDetail = new AccountLedger
                            {
                                code = accountingTemplateCostDetail.code,
                                id_company = accountingTemplateCostDetail.id_company,
                                description = accountingTemplateCostDetail.description,
                                typeCount = accountingTemplateCostDetail.typeCount,
                                id_auxiliary = accountingTemplateCostDetail.id_auxiliary,
                                id_costCenter = tmp1,
                                id_costSubCenter = tmp2,
                                typeAuxiliar = accountingTemplateCostDetail.typeAuxiliar,
                                nameAuxiliar = accountingTemplateCostDetail.nameAuxiliar,
                                nameCenterCost = accountingTemplateCostDetail.nameCenterCost,
                                nameSubCenterCost = accountingTemplateCostDetail.nameSubCenterCost,
                                isActive = accountingTemplateCostDetail.isActive,
                                id_userCreate = accountingTemplateCostDetail.id_userCreate,
                                id_userUpdate = accountingTemplateCostDetail.id_userUpdate,
                                dateCreate = accountingTemplateCostDetail.dateCreate,
                                dateUpdate = accountingTemplateCostDetail.dateUpdate
                            };

                            accountingTemplate.AccountLedger.Add(tempaccountingTemplateCostDetail);
                        }
                    }

                    // CAMPOS DE AUDITORIA 
                    accountingTemplate.id_userCreate = ActiveUser.id;
                    accountingTemplate.dateCreate = DateTime.Now;
                    accountingTemplate.id_userUpdate = ActiveUser.id;
                    accountingTemplate.dateUpdate = DateTime.Now;

                    // FORMA DE QUEMAR EL ID DEL LA COMPANIA
                    accountingTemplate.id_company = this.ActiveCompanyId;

                    model.Add(accountingTemplate);
                    db.SaveChanges();
                    trans.Commit();

                    TempData.Keep("accountingTemplate");
                    TempData.Keep("id_accountingTemplateType");

                    ViewData["EditMessage"] = SuccessMessage("Plantilla Contable de Costos de Producción agregada exitosamente");
                }
                catch (DbEntityValidationException e)
                {
                    TempData.Keep("accountingTemplate");
                    TempData.Keep("id_accountingTemplateType");
                    string msgErr = "";
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        msgErr += "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" + eve.Entry.Entity.GetType().Name + " " + eve.Entry.State;

                        foreach (var ve in eve.ValidationErrors)
                        {
                            msgErr += "- Property: \"{0}\", Error: \"{1}\"" + ve.PropertyName + " " + ve.ErrorMessage;
                        }
                    }

                    ViewData["EditError"] = e.Message;
                    trans.Rollback();
                }
            }
            return PartialView("_AccountingTemplateCostPartial", model.OrderByDescending(p => p.id).ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult AccountingTemplateCostPartialUpdate(DXPANACEASOFT.Models.AccountingTemplate accountingTemplate)
        {
            var modelAccountingTemplateCost = db.AccountingTemplate.FirstOrDefault(it => it.id == accountingTemplate.id);
            AccountingTemplate tempAccountingTemplateCost = (TempData["accountingTemplate"] as AccountingTemplate);

            if (ModelState.IsValid)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (modelAccountingTemplateCost != null)
                        {
                            if (tempAccountingTemplateCost?.AccountLedger != null)
                            {
                                var dbaccountingTemplateCostDetails = modelAccountingTemplateCost.AccountLedger.ToList();
                                var uptdbaccountingTemplateCostDetailsDetails = tempAccountingTemplateCost.AccountLedger.ToList();

                                foreach (var accountingTemplateCostDetail in uptdbaccountingTemplateCostDetailsDetails)
                                {
                                    int tmp1 = 0, tmp2 = 0;

                                    int.TryParse(accountingTemplateCostDetail.nameCenterCost, out tmp1);
                                    int.TryParse(accountingTemplateCostDetail.nameCenterCost, out tmp2);
                                    var oriAccountingTemplateCostDetail = dbaccountingTemplateCostDetails.FirstOrDefault(r => r.id == accountingTemplateCostDetail.id);
                                    if (oriAccountingTemplateCostDetail != null)
                                    {
                                        oriAccountingTemplateCostDetail.code = accountingTemplateCostDetail.code;
                                        oriAccountingTemplateCostDetail.id_company = accountingTemplateCostDetail.id_company;
                                        oriAccountingTemplateCostDetail.description = accountingTemplateCostDetail.description;
                                        oriAccountingTemplateCostDetail.typeCount = accountingTemplateCostDetail.typeCount;
                                        oriAccountingTemplateCostDetail.id_auxiliary = accountingTemplateCostDetail.id_auxiliary;
                                        oriAccountingTemplateCostDetail.id_costCenter = tmp1;
                                        oriAccountingTemplateCostDetail.id_costSubCenter = tmp2;
                                        oriAccountingTemplateCostDetail.nameAuxiliar = accountingTemplateCostDetail.nameAuxiliar;
                                        oriAccountingTemplateCostDetail.typeAuxiliar = accountingTemplateCostDetail.typeAuxiliar;
                                        oriAccountingTemplateCostDetail.nameCenterCost = accountingTemplateCostDetail.nameCenterCost;
                                        oriAccountingTemplateCostDetail.nameSubCenterCost = accountingTemplateCostDetail.nameSubCenterCost;
                                        oriAccountingTemplateCostDetail.isActive = accountingTemplateCostDetail.isActive;
                                        oriAccountingTemplateCostDetail.id_userUpdate = accountingTemplateCostDetail.id_userUpdate;
                                        oriAccountingTemplateCostDetail.dateUpdate = accountingTemplateCostDetail.dateUpdate;

                                    }
                                    else
                                    {
                                        var _tempAccountingTemplateCostDetail = new AccountLedger
                                        {
                                            code = accountingTemplateCostDetail.code,
                                            description = accountingTemplateCostDetail.description,
                                            typeCount = accountingTemplateCostDetail.typeCount,
                                            id_company = accountingTemplateCostDetail.id_company,
                                            id_auxiliary = accountingTemplateCostDetail.id_auxiliary,
                                            typeAuxiliar = accountingTemplateCostDetail.typeAuxiliar,
                                            id_costCenter = tmp1,
                                            id_costSubCenter = tmp2,
                                            nameAuxiliar = accountingTemplateCostDetail.nameAuxiliar,
                                            nameCenterCost = accountingTemplateCostDetail.nameCenterCost,
                                            nameSubCenterCost = accountingTemplateCostDetail.nameSubCenterCost,
                                            isActive = accountingTemplateCostDetail.isActive,
                                            id_userCreate = accountingTemplateCostDetail.id_userCreate,
                                            id_userUpdate = accountingTemplateCostDetail.id_userUpdate,
                                            dateCreate = accountingTemplateCostDetail.dateCreate,
                                            dateUpdate = accountingTemplateCostDetail.dateUpdate

                                        };
                                        modelAccountingTemplateCost.AccountLedger.Add(_tempAccountingTemplateCostDetail);
                                    }

                                }

                            }

                            modelAccountingTemplateCost.code = accountingTemplate.code;
                            modelAccountingTemplateCost.description = accountingTemplate.description;
                            modelAccountingTemplateCost.id_costProduction = accountingTemplate.id_costProduction;
                            modelAccountingTemplateCost.id_expenseProduction = accountingTemplate.id_expenseProduction;
                            modelAccountingTemplateCost.id_processPlant = accountingTemplate.id_processPlant;
                            modelAccountingTemplateCost.isActive = accountingTemplate.isActive;

                            modelAccountingTemplateCost.id_userUpdate = ActiveUser.id;
                            modelAccountingTemplateCost.dateUpdate = DateTime.Now;

                            db.AccountingTemplate.Attach(modelAccountingTemplateCost);
                            db.Entry(modelAccountingTemplateCost).State = EntityState.Modified;

                            db.SaveChanges();
                            trans.Commit();
                            //TempData.Keep("accountingTemplate");
                            //TempData.Keep("id_accountingTemplateType");
                            ViewData["EditMessage"] = SuccessMessage("Plantilla Contable de Costos de Producción actualizada exitosamente");

                        }
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("accountingTemplate");
                        TempData.Keep("id_accountingTemplateType");

                        ViewData["EditError"] = e.Message;
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            if (modelAccountingTemplateCost == null)
            {
                return AccountingTemplateCostPartialAddNew(accountingTemplate);
            }

            return PartialView("_AccountingTemplateCostPartial", db.AccountingTemplate.OrderByDescending(m => m.id).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ValidateAccountTemplateSelected(
            int? id_accountingTemplate, string cciAccount, string typeAux
            , string idAux
            , string codeCenter
            , string codeSubCenter, bool isNew, int idIndex)
        {
            var result = new { isValid = true, error= ""};
            AccountingTemplate codeCount = (TempData["accountingTemplate"] as AccountingTemplate);
            if (codeCount == null)
                codeCount = db.AccountingTemplate.FirstOrDefault(fod => fod.id == id_accountingTemplate);

            codeCount = codeCount ?? new AccountingTemplate();

            var lsAccountLedger = codeCount
                                .AccountLedger;

            lsAccountLedger = lsAccountLedger ?? new List<AccountLedger>();

            TempData["accountingTemplate"] = codeCount;
            TempData.Keep("accountingTemplate");

            if (lsAccountLedger.Count == 0)
                return Json(result, JsonRequestBehavior.AllowGet);
            string _cciAccount = string.IsNullOrEmpty(cciAccount) ? "" : cciAccount.Trim();
            string _typeAux = string.IsNullOrEmpty(typeAux) ? "" : typeAux.Trim();
            string _idAux = string.IsNullOrEmpty(idAux) ? "" : idAux.Trim();
            string _codeCenter = string.IsNullOrEmpty(codeCenter) ? "" : codeCenter.Trim();
            string _codeSubCenter = string.IsNullOrEmpty(codeSubCenter) ? "" : codeSubCenter.Trim();
            bool existe = false;
            bool aceptaAuxiliar = false;
            bool aceptaCentroCosto = false;
            using (var dbInt = new DBContextIntegration())
            {
                var tmp = dbInt.TblciCuenta.FirstOrDefault(fod => fod.CCiCuenta == cciAccount);
                if (tmp != null)
                {
                    existe = true;
                    aceptaAuxiliar = tmp.BSnAceptaAux ?? false;
                    aceptaCentroCosto = tmp.BsnAceptaProyecto ?? false;
                }
            }

            if (!existe)
            {
                result = new { isValid = false, error = "La cuenta Seleccionada no existe" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            if (aceptaAuxiliar)
            {
                if (string.IsNullOrEmpty(typeAux) || string.IsNullOrWhiteSpace(typeAux))
                {
                    result = new { isValid = false, error = "Debe seleccionar Tipo Auxiliar" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(idAux) || string.IsNullOrWhiteSpace(idAux))
                {
                    result = new { isValid = false, error = "Debe seleccionar Código Auxiliar" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else 
            {
                _typeAux = _typeAux.Trim();
                if (_typeAux.Length > 0) 
                {
                    result = new { isValid = false, error = "La cuenta no soporta Auxiliar." };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                _idAux = _idAux.Trim();
                if (_idAux.Length > 0)
                {
                    result = new { isValid = false, error = "La cuenta no soporta Auxiliar." };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            if (aceptaCentroCosto)
            {
                if (string.IsNullOrEmpty(codeCenter) || string.IsNullOrWhiteSpace(codeCenter))
                {
                    result = new { isValid = false, error = "Debe seleccionar el centro de costo" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                _codeCenter = _codeCenter.Trim();
                if (_codeCenter.Length > 0)
                {
                    result = new { isValid = false, error = "La cuenta no acepta centro de costo" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                _codeSubCenter = _codeSubCenter.Trim();
                if (_codeSubCenter.Length > 0)
                {
                    result = new { isValid = false, error = "La cuenta no acepta sub centro de costo" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            if (isNew)
            {
                bool isRepeated = false;
                foreach (var det in lsAccountLedger)
                {
                    var __code = string.IsNullOrEmpty(det.code) ? "":det.code.Trim();
                    var __typeAuxiliar = string.IsNullOrEmpty(det.typeAuxiliar) ? "" : det.typeAuxiliar.Trim();
                    var __idAuxiliar = string.IsNullOrEmpty(det.id_auxiliary) ? "" : det.id_auxiliary.Trim();
                    var __nameCenterCost = string.IsNullOrEmpty(det.nameCenterCost) ? "" : det.nameCenterCost.Trim();
                    var __nameSubCenterCost = string.IsNullOrEmpty(det.nameSubCenterCost) ? "" : det.nameSubCenterCost.Trim();

                    if (__code == _cciAccount
                        && __typeAuxiliar == _typeAux
                        && __idAuxiliar == _idAux
                        && __nameCenterCost == _codeCenter
                        && __nameSubCenterCost == _codeSubCenter)
                    {
                        isRepeated = true;
                        break;
                    }
                }
                //var tmp = lsAccountLedger
                //    .FirstOrDefault(fod => fod.code == cciAccount
                //    && fod.typeAuxiliar == _typeAux
                //    && fod.id_auxiliary == _idAux
                //    && fod.nameCenterCost == _codeCenter
                //    && fod.nameSubCenterCost == _codeSubCenter);
                if (isRepeated)
                {
                    result = new { isValid = false, error = "El registro seleccionado está repetido" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                bool isRepeated = false;
                foreach (var det in lsAccountLedger)
                {
                    var __code = string.IsNullOrEmpty(det.code) ? "" : det.code.Trim();
                    var __typeAuxiliar = string.IsNullOrEmpty(det.typeAuxiliar) ? "" : det.typeAuxiliar.Trim();
                    var __idAuxiliar = string.IsNullOrEmpty(det.id_auxiliary) ? "" : det.id_auxiliary.Trim();
                    var __nameCenterCost = string.IsNullOrEmpty(det.nameCenterCost) ? "" : det.nameCenterCost.Trim();
                    var __nameSubCenterCost = string.IsNullOrEmpty(det.nameSubCenterCost) ? "" : det.nameSubCenterCost.Trim();

                    if (det.id != idIndex
                        && __code == _cciAccount
                        && __typeAuxiliar == _typeAux
                        && __idAuxiliar == _idAux
                        && __nameCenterCost == _codeCenter
                        && __nameSubCenterCost == _codeSubCenter)
                    {
                        isRepeated = true;
                        break;
                    }
                }
                //var tmp = lsAccountLedger
                //    .FirstOrDefault(fod => fod.code == cciAccount
                //    && fod.typeAuxiliar == _typeAux
                //    && fod.id_auxiliary == _idAux
                //    && fod.nameCenterCost == _codeCenter
                //    && fod.nameSubCenterCost == _codeSubCenter);
                if (isRepeated)
                {
                    result = new { isValid = false, error = "El registro seleccionado está repetido" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //var tmp = lsAccountLedger
                //                .FirstOrDefault(fod => fod.code == cciAccount
                //                && fod.typeAuxiliar == _typeAux
                //                && fod.id_auxiliary == _idAux
                //                && fod.nameCenterCost == _codeCenter
                //                && fod.nameSubCenterCost == _codeSubCenter
                //                && fod.id != idIndex);

                //if (tmp != null)
                //{
                //    result = new { isValid = false, error = "El registro seleccionado está repetido" };
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedDetail(string id_codeNew, string codeAux, string typeAux, string codeCenter, string codeSubCenter)
        {
            AccountingTemplate codeCount = (TempData["accountingTemplate"] as AccountingTemplate);
            codeCount = codeCount ?? new AccountingTemplate();

            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            var codeCountAll = codeCount
                                .AccountLedger
                                .FirstOrDefault(fod => fod.code == id_codeNew 
                                && fod.id_auxiliary == codeAux 
                                && fod.typeAuxiliar == typeAux
                                && fod.nameCenterCost == codeCenter && fod.nameSubCenterCost == codeSubCenter);
            var codeCountCc = codeCount
                                .AccountLedger
                                .FirstOrDefault(fod => fod.code == id_codeNew && fod.nameCenterCost == codeCenter && fod.nameSubCenterCost == codeSubCenter);
            var codeCountAux = codeCount.AccountLedger
                                            .FirstOrDefault(fod => fod.code == id_codeNew 
                                            && fod.typeAuxiliar == typeAux
                                            && fod.id_auxiliary == codeAux);
            var codeCountTypeAux = codeCount.AccountLedger
                                .FirstOrDefault(fod => fod.code == id_codeNew && fod.typeAuxiliar == typeAux);
            var codeCountCode = codeCount.AccountLedger.FirstOrDefault(fod => fod.code == id_codeNew);
            bool aux = true;

            if (codeCountCode != null)
            {
                if (codeCountAll != null)
                {
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se pueden repetir detalles iguales: Cuenta/Tipo/ Auxiliar/ Cc/ SubCC."
                    };
                    aux = false;
                }
                if (codeCountTypeAux != null)
                {
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se pueden repetir detalles iguales: Cuenta/ Tipo Auxiliar"
                    };
                    aux = false;
                }
                if (codeCountCc != null)
                {
                    if(codeCenter != null && codeSubCenter != null)
                    {
                        result = new
                        {
                            itsRepeated = 1,
                            Error = "No se pueden repetir detalles iguales: Cuenta/ Cc/ SubCC."

                        };
                        aux = false;
                    }
                }

                if (codeCountAux != null)
                {
                    result = new
                    {
                        itsRepeated = 1,
                        Error = "No se pueden repetir detalles iguales: Cuenta/ Tipo/ Auxiliar."

                    };
                    aux = false;
                }

                if(aux)
                {
                    if(codeCountCode.typeCount == "Título")
                    {
                        result = new
                        {
                            itsRepeated = 1,
                            Error = "No se pueden repetir detalles iguales: Cuenta."
                        };
                    }
                }
                
            }

            TempData["accountingTemplate"] = codeCount;
            TempData.Keep("accountingTemplate");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpPost, ValidateInput(false)]
        public JsonResult ItsRepeatedCabecera(int? id_prodCost, int? id_prodExp, int? id_proPlant)
        {
            var registrosCabecera = db.AccountingTemplate.FirstOrDefault(e => e.id_costProduction == id_prodCost && e.id_expenseProduction == id_prodExp && e.id_processPlant == id_proPlant);

            var result = new
            {
                itsRepeated = 0,
                Error = ""
            };

            if (registrosCabecera != null)
            {
                result = new
                {
                    itsRepeated = 1,
                    Error = "Existe registro con la misma información guardada: Costo/ Gastos/ Planta Proceso."
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult AccountingTemplateCostPartialDelete(System.Int32 id)
        {
            var model = db.AccountingTemplate;
            if (id >= 0)
            {
                try
                {
                    var item = model.FirstOrDefault(it => it.id == id);
                    if (item != null)

                        item.isActive = false;
                    item.id_userUpdate = ActiveUser.id;
                    item.dateUpdate = DateTime.Now;

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    TempData.Keep("accountingTemplate");
                    TempData.Keep("id_accountingTemplateType");

                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_AccountingTemplateCostPartial", model.OrderByDescending(m => m.id).ToList());
        }


        ///*  Detalle*/

        [ValidateInput(false)]
        public ActionResult AccountingTemplateCostDetail(int id_accountingTemplate, int id_accountingTemplateType)
        {
            AccountingTemplate accountingTemplate = ObtainAccountingTemplateCost(id_accountingTemplate);

            var model = accountingTemplate.AccountLedger?.Where(r => r.isActive).ToList() ?? new List<AccountLedger>();

            TempData["id_accountingTemplateType"] = id_accountingTemplateType;
            TempData.Keep("id_accountingTemplateType");

            TempData["accountingTemplate"] = TempData["accountingTemplate"] ?? accountingTemplate;
            TempData.Keep("accountingTemplate");

            ViewBag.Key = accountingTemplate.AccountLedger.Select(e => e.code).ToList();

            return PartialView("_AccountingTemplateCostDetailPartial", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AccountingTemplateCostDetailChangePartial(int id_accountingTemplateType, int id_accountingTemplate)
        {
            AccountingTemplate accountingTemplate = ObtainAccountingTemplateCost(id_accountingTemplate);
            var model = accountingTemplate.AccountLedger?.Where(r => r.isActive).ToList() ?? new List<AccountLedger>();

            TempData["accountingTemplate"] = accountingTemplate;
            TempData.Keep("accountingTemplate");
            TempData["id_accountingTemplate"] = id_accountingTemplateType;
            TempData.Keep("id_accountingTemplate");

            return PartialView("_AccountingTemplateCostDetailPartial", model);
        }


        private AccountingTemplate ObtainAccountingTemplateCost(int? id_accountingTemplate)
        {

            AccountingTemplate accountingTemplate = (TempData["accountingTemplate"] as AccountingTemplate);

            accountingTemplate = accountingTemplate ?? db.AccountingTemplate.FirstOrDefault(i => i.id == id_accountingTemplate);
            accountingTemplate = accountingTemplate ?? new AccountingTemplate();

            return accountingTemplate;

        }

        ///* ADD */
        [HttpPost, ValidateInput(false)]
        public ActionResult AccountingTemplateCostDetailAddNew(int id_accountingTemplate, AccountLedger accountingTemplateCostDetail)
        {

            AccountingTemplate accountingTemplate = ObtainAccountingTemplateCost(id_accountingTemplate);

            if (ModelState.IsValid)
            {
                try
                {
                    accountingTemplateCostDetail.id = accountingTemplate.AccountLedger.Count() > 0 ? accountingTemplate.AccountLedger.Max(pld => pld.id) + 1 : 1;

                    using (var dbInt = new DBContextIntegration())
                    {
                        var tmp = dbInt.TblciCuenta.FirstOrDefault(fod => fod.CCiCuenta == accountingTemplateCostDetail.code);
                        if (tmp != null)
                            accountingTemplateCostDetail.AceptaAuxiliar = tmp.BSnAceptaAux ?? false;
                    }
                    var tmpCenterCost = accountingTemplateCostDetail.nameCenterCost;
                    var tmpSubCenterCost = accountingTemplateCostDetail.nameSubCenterCost;

                    if (string.IsNullOrEmpty(tmpCenterCost))
                        accountingTemplateCostDetail.nameCenterCost = tmpCenterCost;

                    if (string.IsNullOrEmpty(tmpSubCenterCost))
                        accountingTemplateCostDetail.nameSubCenterCost = tmpSubCenterCost;

                    accountingTemplateCostDetail.dateCreate = DateTime.Now;
                    accountingTemplateCostDetail.dateUpdate = DateTime.Now;
                    accountingTemplateCostDetail.id_userCreate = ActiveUser.id;
                    accountingTemplateCostDetail.id_userUpdate = ActiveUser.id;
                    accountingTemplateCostDetail.id_company = (int)ViewData["id_company"];
                    accountingTemplateCostDetail.isActive = true;

                    accountingTemplate.AccountLedger.Add(accountingTemplateCostDetail);
                    TempData["accountingTemplate"] = accountingTemplate;
                    TempData.Keep("accountingTemplate");
                    TempData.Keep("id_accountingTemplateType");

                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
            {
                ViewData["EditError"] = "Por favor, corrija todos los errores.";
            }

            var model = accountingTemplate.AccountLedger.Where(r => r.isActive);
            if (model != null)
            {
                model = model.ToList();
            }
            else
            {
                model = new List<AccountLedger>();
            }

            return PartialView("_AccountingTemplateCostDetailPartial", model);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult AccountingTemplateCostDetailUpdate(AccountLedger accountingTemplateCost)
        {

            AccountingTemplate accountingTemplate = ObtainAccountingTemplateCost(accountingTemplateCost.accountingTemplate);

            List<AccountLedger> model = AccountingTemplateCostDetailUpdateDelete(accountingTemplate, accountingTemplateCost, accountingTemplateCost.id, false);
            TempData.Keep("accountingTemplate");
            TempData.Keep("id_accountingTemplateType");

            return PartialView("_AccountingTemplateCostDetailPartial", model);
        }

        private List<AccountLedger> AccountingTemplateCostDetailUpdateDelete(AccountingTemplate accountingTemplate
            , AccountLedger accountTmp, int id_AccountingTemplateCostDetail, Boolean isDelete)
        {

            if (ModelState.IsValid && accountingTemplate != null)
            {
                try
                {
                    var modelaccountingTemplateCostDetailDetail = accountingTemplate.AccountLedger.FirstOrDefault(i => i.id == id_AccountingTemplateCostDetail);
                    if (modelaccountingTemplateCostDetailDetail != null)
                    {

                        if (isDelete) modelaccountingTemplateCostDetailDetail.isActive = false;
                        else 
                        {
                            

                            modelaccountingTemplateCostDetailDetail.code = accountTmp.code;
                            modelaccountingTemplateCostDetailDetail.description = accountTmp.description;
                            modelaccountingTemplateCostDetailDetail.id_auxiliary = accountTmp.nameAuxiliar;
                            modelaccountingTemplateCostDetailDetail.typeAuxiliar = accountTmp.typeAuxiliar;
                            modelaccountingTemplateCostDetailDetail.nameCenterCost = accountTmp.nameCenterCost;
                            modelaccountingTemplateCostDetailDetail.nameSubCenterCost = accountTmp.nameSubCenterCost;

                            using (var dbInt = new DBContextIntegration())
                            {
                                var tmp = dbInt.TblciCuenta.FirstOrDefault(fod => fod.CCiCuenta == modelaccountingTemplateCostDetailDetail.code);
                                if (tmp != null)
                                    modelaccountingTemplateCostDetailDetail.AceptaAuxiliar = tmp.BSnAceptaAux ?? false;
                            }
                            var tmpCenterCost = modelaccountingTemplateCostDetailDetail.nameCenterCost;
                            var tmpSubCenterCost = modelaccountingTemplateCostDetailDetail.nameSubCenterCost;

                            if (string.IsNullOrEmpty(tmpCenterCost))
                                modelaccountingTemplateCostDetailDetail.nameCenterCost = tmpCenterCost;

                            if (string.IsNullOrEmpty(tmpSubCenterCost))
                                modelaccountingTemplateCostDetailDetail.nameSubCenterCost = tmpSubCenterCost;

                        }
                        modelaccountingTemplateCostDetailDetail.id_userUpdate = ActiveUser.id;
                        modelaccountingTemplateCostDetailDetail.dateUpdate = DateTime.Now;
                        this.UpdateModel(modelaccountingTemplateCostDetailDetail);
                    }

                    TempData["accountingTemplate"] = accountingTemplate;
                    TempData.Keep("id_accountingTemplateType");
                }
                catch (DbEntityValidationException e)
                {
                    ViewData["EditError"] = e.Message;
                    string msgErr = "";
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        msgErr += "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" + eve.Entry.Entity.GetType().Name + " " + eve.Entry.State;

                        foreach (var ve in eve.ValidationErrors)
                        {
                            msgErr += "- Property: \"{0}\", Error: \"{1}\"" + ve.PropertyName + " " + ve.ErrorMessage;
                        }
                    }
                }
            }
            else
                ViewData["EditError"] = "Por favor, corrija todos los errores.";


            TempData.Keep("accountingTemplate");
            TempData.Keep("id_accountingTemplateType");


            List<AccountLedger> model = accountingTemplate.AccountLedger?.Where(x => x.isActive).ToList() ?? new List<AccountLedger>();
            model = (model.Count() != 0) ? model : new List<AccountLedger>();

            return model;
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult AccountingTemplateCostDetailDelete(int id_accountingTemplate, int id)
        {
            AccountingTemplate _accountTemplate = ObtainAccountingTemplateCost(id_accountingTemplate);
            AccountLedger _accountLedgerTmp = new AccountLedger();
            _accountLedgerTmp.id = id;
            List<AccountLedger> model = AccountingTemplateCostDetailUpdateDelete(_accountTemplate, _accountLedgerTmp, id, true);

            return PartialView("_AccountingTemplateCostDetailPartial", model);
        }

        public JsonResult LoadSubCenterCostAccountingTemplate(string codeCenter)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //codeAccount
            var dbCI = new DBContextIntegration();
            var codeAccount = dbCI.TblCiSubProyecto.Where(e => e.CCiProyecto == codeCenter && e.CCeSubProyecto == "A").Select(e => new { id = e.CCiSubProyecto, name = e.CDsSubProyecto }).ToList();

            return Json(codeAccount, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TypeDescriptionChangeData(string idCode)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //codeAccount
            var dbCI = new DBContextIntegration();
            var codeAccount = dbCI.TblciCuenta.FirstOrDefault(e => e.CCiCuenta == idCode);

            if (codeAccount == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            string typeCount;
            if (codeAccount.CCtTituloDetalle == "T")
            {
                typeCount = "Título";
            }
            else
            {
                typeCount = "Detalle";
            }

            var result = new
            {
                tipoCuenta = typeCount,
                descripcion = codeAccount.CDsCuenta,
                aceptAux = codeAccount.BSnAceptaAux,
                aceptaProy = codeAccount.BsnAceptaProyecto
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CodeAuxiliarChangeData(string nameAuxiliar)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //codeAccount
            var dbCI = new DBContextIntegration();
            var codeAccount = dbCI.TblCiAuxiliar.FirstOrDefault(e => e.CCiAuxiliar == nameAuxiliar);

            if (codeAccount == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                codeAux = codeAccount.CCiAuxiliar
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CodeCenterCostChangeData(string nameCenter)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //codeAccount
            var dbCI = new DBContextIntegration();
            var codeAccount = dbCI.TblCiProyecto.FirstOrDefault(e => e.CCiProyecto == nameCenter);

            if (codeAccount == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                codeCenterCost = codeAccount.CCiProyecto,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult LoadCenterCostAccountingTemplate()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);

            var dbCI = new DBContextIntegration();
            var resulCC = dbCI.TblCiProyecto.Where(e => e.CCeProyecto == "A").Select(e => new { id = e.CCiProyecto, name = e.CDsProyecto }).ToList();

            TempData.Keep("accountingTemplate");
            return Json(resulCC, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CodeSubCenterCostChangeData(string nameSubCenter)
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //codeAccount
            var dbCI = new DBContextIntegration();
            var codeAccount = dbCI.TblCiSubProyecto.FirstOrDefault(e => e.CCiSubProyecto == nameSubCenter);

            if (codeAccount == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var result = new
            {
                codeSubCenterCost = codeAccount.CCiSubProyecto
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult ValidateCodeAccountingTemplate(int id_accountingTemplate, string code)
        {
            AccountingTemplate accountingTemplate = db.AccountingTemplate.FirstOrDefault(b => b.id_company == this.ActiveCompanyId
                                                                            && b.code == code
                                                                            && b.id != id_accountingTemplate);

            if (accountingTemplate == null)
            {
                return Json(new { isValid = true, errorText = "" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { isValid = false, errorText = "Código en uso por otra Plantilla Contable" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CostProductionChangeData(int id_costPoduction)
        {
            if (TempData["accountingTemplate"] != null)
            {
                TempData.Keep("accountingTemplate");
            }
            var expenseProduction = db.ProductionExpense.Where(w => w.id_productionCostType == id_costPoduction && w.isActive)
                                       .Select(s => new
                                       {
                                           id = s.id,
                                           name = s.name
                                       });

            return Json(expenseProduction, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateInput(false)]
        public JsonResult LoadCodeComboAccountingTemplate()
        {
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //codeAccount
            var lista = TempData["codeCuentas"];

            var dbCI = new DBContextIntegration();
            var codeAccount = dbCI.TblciCuenta.Where(e => e.CCeCuenta == "A").ToList();

            //var result = new
            //{
            //    codigoCuentas = codeAccount,
            //};

            TempData.Keep("accountingTemplate");
            return Json(codeAccount, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ValidateInput(false)]
        public JsonResult LoadNameAuxiliarAccountingTemplate(string codeCuenta)
        {
            int id_company = (int)ViewData["id_company"];
            UpdateArrayTempDataKeep(TempData["arrayTempDataKeep"] as string[]);
            //codeAccount
            var dbCI = new DBContextIntegration();

            var result = (from cias in dbCI.tblCiCias
                          from cuen in dbCI.TblciCuenta
                          from rel1 in dbCI.TblCiRel_CtaMy_GpoTipAux
                          from gtip in dbCI.TblCiGpoTipoAuxiliar
                          from rel2 in dbCI.TblCiRel_Gpo_Tip_Aux
                          from taux in dbCI.TblCiTipoAuxiliar
                          from rel3 in dbCI.TblCiRel_Auxiliar_TipoAuxiliar
                          from auxi in dbCI.TblCiAuxiliar
                          where cuen.CCiPlanCta == cias.CCiPlanCta
                          && cuen.CCiCuenta == codeCuenta
                          && cuen.CCeCuenta == "A"
                          && rel1.CCiPlanCta == cuen.CCiPlanCta
                          && rel1.CCiCuenta == cuen.CCiCuenta
                          && rel1.CCeRel_CtaMy_GpoTipAux == "A"
                          && gtip.CCiGpoTipoAuxiliar == rel1.CCiGpoTipoAuxiliar
                          && gtip.CCeGpoTipoAuxiliar == "A"
                          && rel2.CCiGpoTipoAuxiliar == gtip.CCiGpoTipoAuxiliar
                          && rel2.CCeRel_Gpo_Tip_Aux == "A"
                          && taux.CCiTipoAuxiliar == rel2.CCiTipoAuxiliar
                          && taux.CCeTipoAuxiliar == "A"
                          && rel3.CCiTipoAuxiliar == taux.CCiTipoAuxiliar
                          && rel3.CCeRel_Aux_TipoAux == "A"
                          && auxi.CCiAuxiliar == rel3.CCiAuxiliar
                          && auxi.CCeAuxiliar == "A"
                          select new
                          {
                              codeAuxiliar = auxi.CCiAuxiliar,
                              descAuxiliar = auxi.CDsAuxiliar,
                              codeCountAuxiliar = auxi.CCeAuxiliar
                          }).ToList();

            TempData.Keep("accountingTemplate");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTypeAuxiliar(string typeAuxiliar, string id_cuenta)
        {
            bool aceptaAuxiliar = false;
            using (var dbInt = new DBContextIntegration())
            {
                aceptaAuxiliar=dbInt.TblciCuenta.FirstOrDefault(fod => fod.CCiCuenta == id_cuenta)?.BSnAceptaAux ?? false;
            }
            AccountLedger accountLedger = new AccountLedger();
            accountLedger.typeAuxiliar = typeAuxiliar;
            accountLedger.code = id_cuenta;
            accountLedger.AceptaAuxiliar = aceptaAuxiliar;
            return PartialView("_ComboBoxAcountingTemplateTypeAuxiliar", accountLedger);
        }
        public ActionResult GetNameAuxiliar(string id_auxiliar, string type_auxiliar)
        {
            AccountLedger accountLedger = new AccountLedger();
            accountLedger.id_auxiliary = id_auxiliar;
            accountLedger.typeAuxiliar = type_auxiliar;
            return PartialView("_ComboBoxAcountingTemplate", accountLedger);
        }
        [HttpPost]
        public ActionResult GetCenterCosts(string nameCenterCost)
        {
            AccountingTemplate item = (TempData["accountingTemplate"] as AccountingTemplate);
            item = item ?? new AccountingTemplate();
            TempData["accountingTemplate"] = item;
            TempData.Keep("accountingTemplate");

            ViewData["nameCenterCost"] = nameCenterCost;

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "nameCenterCost";
                p.Width = Unit.Percentage(92);
                p.ValueField = "id";
                p.ValueType = typeof(string);
                p.TextFormatString = "{0} - {1}";
                p.CallbackPageSize = 15;
                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;


                p.Columns.Add("id", "Código", Unit.Percentage(20));
                p.Columns.Add("name", "Descripción", Unit.Percentage(70));
                p.CallbackRouteValues = new { Controller = "AccountingTemplateCost", Action = "GetCenterCosts" };
                p.ClientSideEvents.Init = "OnNameCenterCostInit";
                p.ClientSideEvents.BeginCallback = "CenterCostLedge_BeginCallback";
                p.ClientSideEvents.SelectedIndexChanged = "ComboCenterCostLedger_SelectedIndexChanged";
                p.ClientSideEvents.EndCallback = "CenterCostLedge_EndCallback";

                p.BindList(DataProviderAccountingTemplate.AccountingTemplateCenterCost());

            });

        }
        [HttpPost]
        public ActionResult GetSubCenterCosts(string nameCenterCost, string nameSubCenterCost)
        {
            /*
             settings.Name = "nameSubCenterCost";
		settings.Properties.ClientInstanceName = "nameSubCenterCost";
		settings.Width = Unit.Percentage(92);

		settings.Properties.DropDownStyle = DropDownStyle.DropDownList;
		settings.Properties.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
		settings.Properties.EnableSynchronization = DefaultBoolean.False;
		settings.Properties.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

		settings.CallbackRouteValues = new { Controller = "AccountingTemplateCost", Action = "GetSubCenterCosts" };
		settings.Properties.CallbackPageSize = 10;

		settings.Properties.TextFormatString = "{0} - {1}";
		settings.Properties.ValueField = "id";
		settings.Properties.Columns.Clear();
		settings.Properties.ValueType = typeof(string);
		settings.Properties.Columns.Add("id", "Código", Unit.Percentage(20));
		settings.Properties.Columns.Add("CDsSubProyecto", "Descripción", Unit.Percentage(70));
		settings.Properties.ClientSideEvents.BeginCallback = "NameSubCenterCost_BeginCallback";
		settings.Properties.ClientSideEvents.SelectedIndexChanged = "ComboSubCenterCostLedger_SelectedIndexChanged";
		settings.Properties.ClientSideEvents.EndCallback = "NameSubCenterCost_EndCallback";
		settings.Properties.ClientSideEvents.Init = "OnNameSubCenterCostInit";
		settings.ItemTextCellPrepared = (sender, e) =>
		{
			e.TextCell.ToolTip = HttpUtility.HtmlDecode(e.TextCell.Text);
		};
             */
            AccountingTemplate item = (TempData["accountingTemplate"] as AccountingTemplate);
            item = item ?? new AccountingTemplate();
            TempData["accountingTemplate"] = item;
            TempData.Keep("accountingTemplate");

            ViewData["nameSubCenterCost"] = nameSubCenterCost;

            return GridViewExtension.GetComboBoxCallbackResult(p =>
            {
                p.ClientInstanceName = "nameSubCenterCost";

                p.Width = Unit.Percentage(92);
                p.ValueField = "id";
                p.ValueType = typeof(string);
                p.TextFormatString = "{0} - {1}";
                p.CallbackPageSize = 15;
                p.DropDownStyle = DropDownStyle.DropDownList;
                p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
                p.EnableSynchronization = DefaultBoolean.False;
                p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

                p.Columns.Add("id", "Código", Unit.Percentage(20));
                p.Columns.Add("CDsSubProyecto", "Descripción", Unit.Percentage(70));
                p.CallbackRouteValues = new { Controller = "AccountingTemplateCost", Action = "GetSubCenterCosts" };
                p.ClientSideEvents.Init = "OnNameSubCenterCostInit";
                p.ClientSideEvents.BeginCallback = "NameSubCenterCost_BeginCallback";
                p.ClientSideEvents.SelectedIndexChanged = "ComboSubCenterCostLedger_SelectedIndexChanged";
                p.ClientSideEvents.EndCallback = "NameSubCenterCost_EndCallback";

                p.BindList(DataProviderAccountingTemplate.AccountingTemplateSubCenterCost(nameCenterCost));

            });

        }

    }
}
