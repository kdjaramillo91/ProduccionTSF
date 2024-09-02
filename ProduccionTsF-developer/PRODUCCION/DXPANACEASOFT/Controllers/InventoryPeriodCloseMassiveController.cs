using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Internal;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.Dapper;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Operations;
using DXPANACEASOFT.Services;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
//using System.Web.Http.Result;
using System.Web.Mvc;
using static Spire.Pdf.General.Render.Decode.Jpeg2000.j2k.codestream.HeaderInfo;

namespace DXPANACEASOFT.Controllers
{
    public class InventoryPeriodCloseMassiveController : DefaultController
    {
        

        public ActionResult Index()
        {
            return View();
        }
  

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult InventoryPeriodCloseMassiveResults(InventoryPeriod InventoryPeriod,
                                                  DateTime? periodo,
                                                  int? id_InventoryPeriodType, int? id_Estado
                                                  )
        {
            List<InventoryPeriodDetail> model;
            if (periodo.HasValue)
            {
                model = db.InventoryPeriodDetail
                    .Where(e => e.dateInit.Year == periodo.Value.Year && e.dateInit.Month == periodo.Value.Month &&
                            (e.AdvanceParametersDetail.valueCode == "A") && e.InventoryPeriod.isActive)
                    .ToList();
            }
            else
            {
                model = db.InventoryPeriodDetail.Where(o => o.InventoryPeriod.isActive &&
                            (o.AdvanceParametersDetail.valueCode == "A")).ToList();
            }


            #region  FILTERS

            if (id_InventoryPeriodType != null && id_InventoryPeriodType > 0)
            {
                model = model.Where(o => o.InventoryPeriod.id_PeriodType == id_InventoryPeriodType).ToList();
            }

            if (id_Estado != null && id_Estado > 0)
            {
                model = model.Where(o => o.id_PeriodState == id_Estado).ToList();
            }


            if (InventoryPeriod.id_warehouse > 0)
            {
                model = model.Where(o => o.InventoryPeriod.id_warehouse == InventoryPeriod.id_warehouse).ToList();
            }
            else
            {
                EntityObjectPermissions entityObjectPermissions = ViewData["entityObjectPermissions"] != null ? (EntityObjectPermissions)ViewData["entityObjectPermissions"] : null;
                if (entityObjectPermissions != null)
                {
                    var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
                    if (entityPermissions != null)
                    {
                        var entityValuePermissions = entityPermissions.listValue.Where(w => w.listPermissions.FirstOrDefault(fod => fod.name == "Visualizar") != null);
                        model = model.Where(w => entityValuePermissions.FirstOrDefault(fod => fod.id_entityValue == w.InventoryPeriod.id_warehouse) != null).ToList();
                    }
                }
            }

            model = model
                .Where(o => o.InventoryPeriod.id_Company == this.ActiveCompanyId && 
                        o.InventoryPeriod.id_Division == ActiveDivision.id && o.InventoryPeriod.id_BranchOffice == ActiveSucursal.id).ToList();
            #endregion

            TempData["model_rs_invpermas"] = model;
            TempData.Keep("model_rs_invpermas");

            return PartialView("_InventoryPeriodCloseMassiveResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }


        #endregion

        #region InventoryPeriod HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryPeriodCloseMassivePartial()
        {
            var model = (TempData["model_rs_invpermas"] as List<InventoryPeriodDetail>);
            model = model ?? new List<InventoryPeriodDetail>();
            TempData.Keep("model_rs_invpermas");
            return PartialView("_InventoryPeriodCloseMassivePartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion

        #region Edit InventoryPeriod
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditInventoryPeriodCloseMassive(int id, int[] orderDetails)
        {
            InventoryPeriod InventoryPeriod = db.InventoryPeriod.Where(o => o.id == id).FirstOrDefault();

            if (InventoryPeriod == null)
            {

                InventoryPeriod = new InventoryPeriod
                {
                    id_Company = ActiveUser.id_company,
                    id_Division = ActiveDivision.id,
                    id_BranchOffice = ActiveSucursal.id,
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now,
                    year = DateTime.Now.Year,
                    isActive = true,
                    id_userCreate = ActiveUser.id,
                    dateCreate = DateTime.Now,

                };
            }
            TempData["InventoryPeriod"] = InventoryPeriod;
            TempData.Keep("InventoryPeriod");

            return PartialView("_FormEditInventoryPeriodCloseMassive", InventoryPeriod);
        }
        #endregion

        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_InventoryPeriod)
        {
            TempData.Keep("InventoryPeriod");
            int index = db.InventoryPeriod.Where(o => o.id_Company == this.ActiveCompanyId && o.id_Division == ActiveDivision.id && o.id_BranchOffice == ActiveSucursal.id).OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_InventoryPeriod);
            var result = new
            {
                maximunPages = db.InventoryPeriod.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            InventoryPeriod InventoryPeriod = db.InventoryPeriod.Where(o => o.id_Company == this.ActiveCompanyId && o.id_Division == ActiveDivision.id && o.id_BranchOffice == ActiveSucursal.id).OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (InventoryPeriod != null)
            {
                TempData["InventoryPeriod"] = InventoryPeriod;
                TempData.Keep("InventoryPeriod");
                return PartialView("_InventoryPeriodCloseMassivePartial", InventoryPeriod);
            }

            TempData.Keep("InventoryPeriod");

            return PartialView("_InventoryPeriodCloseMassivePartial", new InventoryPeriod());
        }
        #endregion

        [HttpPost, ValidateInput(false)]
        public JsonResult CheckAbrirPeriodos(int[] ids)
        {
            TempData.Keep("InventoryPeriod");
            string currentBodega = "";
            GenericResultJson oJsonResult = new GenericResultJson();
            if (ids != null)
            {
                bool isValidTransaction = false;
                List<MonthlyBalanceControl> monthlyBalanceControls = new List<MonthlyBalanceControl>();
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            InventoryPeriodDetail inventoryPeriodDetail = db.InventoryPeriodDetail.FirstOrDefault(o => o.id == id);
                            InventoryPeriod inventoryPeriod = db.InventoryPeriod.FirstOrDefault(o => o.id == inventoryPeriodDetail.id_InventoryPeriod);

                            string codigoTipoPeriodo = inventoryPeriod.AdvanceParametersDetail.valueCode;
                            int anio = inventoryPeriod.year;
                            int periodo = inventoryPeriodDetail.periodNumber;
                            int id_Warehouse = inventoryPeriod.id_warehouse;
                            int mes = 0;

                            if (codigoTipoPeriodo == "D")
                            {
                                DateTime fechaInicio = GlobalUtils.DateFromDayYear(anio, periodo);
                                mes = fechaInicio.Month;
                            }
                            else
                            {
                                mes = periodo;

                            }

                            var modelItem = inventoryPeriod.InventoryPeriodDetail.Where(it => it.id == id).FirstOrDefault();

                            if (modelItem != null)
                            {

                                string mensaje = "";
                                int IdAnterior = 0;
                                int IdSiguiente = 0;
                                int IdSEstadoA = 0;
                                int IdSEstadoC = 0;

                                if (!validarDetalleInventoryPeriod(inventoryPeriodDetail, true, ref mensaje, ref IdAnterior, ref IdSiguiente, ref IdSEstadoA, ref IdSEstadoC))
                                {
                                    TempData.Keep("InventoryPeriod");
                                    throw new Exception(mensaje);
                                }

                                currentBodega = inventoryPeriod.Warehouse.name;

                                if (IdAnterior > 0 && IdSEstadoA > 0)
                                {
                                    modelItem = db.InventoryPeriodDetail.Where(it => it.id == IdAnterior).FirstOrDefault();
                                    if (modelItem != null)
                                    {
                                        modelItem.id_PeriodState = IdSEstadoA;
                                        modelItem.isClosed = false;
                                        modelItem.InventoryPeriod.id_userUpdate = ActiveUser.id;
                                        modelItem.InventoryPeriod.dateUpdate = DateTime.Now;
                                    }
                                    this.UpdateModel(modelItem);
                                    
                                }
                                TempData["InventoryPeriod"] = inventoryPeriod;

                                db.InventoryPeriodDetail.Attach(modelItem);
                                db.Entry(modelItem).State = EntityState.Modified;

                            }

                            monthlyBalanceControls.Add(new MonthlyBalanceControl
                            {
                                id_company = this.ActiveCompanyId,
                                Anio = anio,
                                Mes = mes,
                                id_warehouse = id_Warehouse,
                                DateIsNotValid = DateTime.Now,
                                LastDateProcess = modelItem.dateInit
                            });
                        }
                        db.SaveChanges();
                        trans.Commit();
                        oJsonResult.codeReturn = 1;
                        oJsonResult.message = SuccessMessage("Periodos abiertos con éxito.");

                        isValidTransaction = true; 
                    }
                    catch (Exception e)
                    {
                        oJsonResult.codeReturn = -1;
                        oJsonResult.message = ErrorMessage(e.Message);

                        trans.Rollback();
                        isValidTransaction = false;
                    }
                }
                if (isValidTransaction)
                {
                    try
                    {

                        var balanceControl = ServiceInventoryBalance.BalanceControlActualizar(this.ActiveCompanyId, monthlyBalanceControls.ToArray());

                        DapperConnection.BulkUpdate(balanceControl);
                    }
                    catch (Exception e)
                    {
                        oJsonResult.codeReturn = -1;
                        oJsonResult.message = ErrorMessage("Ha ocurrido un error al desactualizar el periodo/mes Saldo de Bodega");
                    }
                }
                
                
            }

            TempData.Keep("InventoryPeriod");

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult CheckACerrarPeriodos(int[] ids)
        {
            TempData.Keep("InventoryPeriod");
            string currentBodega = "";
            GenericResultJson oJsonResult = new GenericResultJson();
            if (ids != null)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in ids)
                        {
                            InventoryPeriodDetail inventoryPeriodDetail = db.InventoryPeriodDetail.FirstOrDefault(o => o.id == id);
                            InventoryPeriod inventoryPeriod = db.InventoryPeriod.FirstOrDefault(o => o.id == inventoryPeriodDetail.id_InventoryPeriod);

                            var modelItem = inventoryPeriod.InventoryPeriodDetail.Where(it => it.id == id).FirstOrDefault();

                            if (modelItem != null)
                            {

                                string mensaje = "";
                                int IdAnterior = 0;
                                int IdSiguiente = 0;
                                int IdSEstadoA = 0;
                                int IdSEstadoC = 0;
                             
                                if (!validarDetalleInventoryPeriod(inventoryPeriodDetail, false, ref mensaje, ref IdAnterior, ref IdSiguiente, ref IdSEstadoA, ref IdSEstadoC))
                                {
                                    throw new Exception(mensaje);
                                }

                                currentBodega = inventoryPeriod.Warehouse.name;
                                

                                if (IdAnterior > 0 && IdSEstadoC > 0)
                                {
                                    modelItem = db.InventoryPeriodDetail.Where(it => it.id == IdAnterior).FirstOrDefault();
                                    if(modelItem != null)
                                    {
                                        modelItem.id_PeriodState = IdSEstadoC;
                                        modelItem.isClosed = true;
                                        modelItem.InventoryPeriod.id_userUpdate = ActiveUser.id;
                                        modelItem.InventoryPeriod.dateUpdate = DateTime.Now;
                                    }
                                }
                                if (IdSEstadoC > 0)
                                {
                                    modelItem = inventoryPeriod.InventoryPeriodDetail.Where(it => it.id == id).FirstOrDefault();
                                    if (modelItem != null)
                                    {
                                        modelItem.id_PeriodState = IdSEstadoC;
                                        modelItem.isClosed = true;
                                        modelItem.InventoryPeriod.id_userUpdate = ActiveUser.id;
                                        modelItem.InventoryPeriod.dateUpdate = DateTime.Now;
                                    }
                                }
                                if (IdSiguiente > 0 && IdSEstadoA > 0)
                                {
                                    //modelItem = inventoryPeriod.InventoryPeriodDetail.Where(it => it.id == IdSiguiente).FirstOrDefault();
                                    modelItem = db.InventoryPeriodDetail.Where(it => it.id == IdSiguiente).FirstOrDefault();
                                    if (modelItem != null)
                                    {
                                        modelItem.id_PeriodState = IdSEstadoA;
                                        modelItem.isClosed = false;
                                        modelItem.InventoryPeriod.id_userUpdate = ActiveUser.id;
                                        modelItem.InventoryPeriod.dateUpdate = DateTime.Now;
                                    }

                                }
                            
                                TempData["InventoryPeriod"] = inventoryPeriod;

                                db.InventoryPeriodDetail.Attach(modelItem);
                                db.Entry(modelItem).State = EntityState.Modified;

                            }


                        }
                        db.SaveChanges();
                        trans.Commit();
                        oJsonResult.codeReturn = 1;
                        oJsonResult.message = SuccessMessage("Periodos cerrados con éxito.");

                    }
                    catch (Exception e)
                    {
                        oJsonResult.codeReturn = -1;
                        oJsonResult.message = ErrorMessage(e.Message);

                        trans.Rollback();
                    }
                }
            }

            TempData.Keep("InventoryPeriod");

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }

        #region InventoryPeriod Gridview

        [ValidateInput(false)]
        public ActionResult InventoryPeriodPartial(int? id)
        {
            if (id != null)
            {
                ViewData["InventoryPeriodToCopy"] = db.InventoryPeriod.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.InventoryPeriod.Where(o => o.id_Company == this.ActiveCompanyId);
            return PartialView("_InventoryPeriodCloseMassivePartial", model.ToList());
        }

        #endregion


        #region  Inventory details

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryPeriodCloseMassiveDetailPartial()
        {
            InventoryPeriod inventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);
            inventoryPeriod = inventoryPeriod ?? new InventoryPeriod();
            inventoryPeriod.InventoryPeriodDetail = inventoryPeriod.InventoryPeriodDetail ?? new List<InventoryPeriodDetail>();

            var model = inventoryPeriod.InventoryPeriodDetail;

            TempData.Keep("InventoryPeriod");

            return PartialView("_Details", model.ToList());
        }

        private bool validarDetalle(InventoryPeriodDetail inventoryPeriodDetail, ref string mensaje, ref int IdSiguiente, ref int IdEStadoA)
        {
            bool result = true;
            IdSiguiente = 0;
            int idEstadActivre = 0;
            TempData["EditMessage"] = null;
           InventoryPeriod inventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);

            inventoryPeriod = inventoryPeriod ?? db.InventoryPeriod.Where(i => i.id == inventoryPeriod.id).FirstOrDefault();
            inventoryPeriod = inventoryPeriod ?? new InventoryPeriod();

            var cantre = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && inventoryPeriodDetail.dateInit >= x.dateInit && inventoryPeriodDetail.dateInit <= x.dateEnd).ToList().Count();
            if (cantre > 0)
            {
                mensaje = ErrorMessage("Ya existe un Periodo con el mismo rango de fecha");
                TempData.Keep("InventoryPeriod");
                ViewData["EditMessage"] = mensaje;

                TempData["EditMessage"] = mensaje;

                TempData.Keep("EditMessage");
                result = false;
            }


            var cantrefin = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && inventoryPeriodDetail.dateEnd >= x.dateInit && inventoryPeriodDetail.dateEnd <= x.dateEnd).ToList().Count();
            if (cantrefin > 0)
            {
                mensaje = ErrorMessage("Y existe un Periodo con el mismo rango de fecha");
                TempData.Keep("InventoryPeriod");
                ViewData["EditMessage"] = mensaje;
                TempData["EditMessage"] = mensaje;

                TempData.Keep("EditMessage");
                result = false;
            }

            var vestado = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
            if (vestado != null)
            {
                var idestado = (from es in vestado
                                where es.valueCode.ToUpper().Trim() == "A"
                                select es).FirstOrDefault();

                if (idestado != null)
                {

                    idEstadActivre = idestado.id;
                    if (inventoryPeriodDetail.id_PeriodState == idestado.id)
                    {
                        var cantestado = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && inventoryPeriodDetail.id_PeriodState == x.id_PeriodState).ToList().Count();

                        if (cantestado > 0)
                        {
                            TempData.Keep("InventoryPeriod");
                            ViewData["EditMessage"] = ErrorMessage("Y existe un Periodo Activo");
                            mensaje = ErrorMessage("Y existe un Perido Activo");
                            TempData["EditMessage"] = mensaje;

                            TempData.Keep("EditMessage");
                            result = false;
                        }
                    }
                    else
                    {
                        IdEStadoA = idestado.id;
                    }

                }


                idestado = (from es in vestado
                            where es.valueCode.ToUpper().Trim() == "C"
                            select es).FirstOrDefault();

                if (idestado != null)
                {
                    if (inventoryPeriodDetail.id_PeriodState == idestado.id)
                    {
                        var idsi = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && idestado.id != x.id_PeriodState && x.periodNumber > inventoryPeriodDetail.periodNumber).FirstOrDefault();

                        if (idsi == null)
                        {
                            var idsig = db.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && idestado.id != x.id_PeriodState && x.InventoryPeriod.year > inventoryPeriodDetail.InventoryPeriod.year
                            && x.InventoryPeriod.id_Company  == inventoryPeriodDetail.InventoryPeriod.id_Company && x.InventoryPeriod.id_Division == inventoryPeriodDetail.InventoryPeriod.id_Division
                            && x.InventoryPeriod.id_BranchOffice == inventoryPeriodDetail.InventoryPeriod.id_BranchOffice && x.InventoryPeriod.id_warehouse == inventoryPeriodDetail.InventoryPeriod.id_warehouse

                            ).FirstOrDefault();
                            if (idsig == null)
                            {
                                TempData.Keep("InventoryPeriod");
                                ViewData["EditMessage"] = ErrorMessage("No existe otro periodo Pendiente");
                                mensaje = ErrorMessage("No existe otro periodo Pendiente");
                                TempData["EditMessage"] = mensaje;

                                TempData.Keep("EditMessage");

                                ViewData["EditMessage"] = mensaje;

                                result = false;
                            }
                            else
                            {

                                var registroactivoa = db.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && idEstadActivre == x.id_PeriodState && x.InventoryPeriod.year > inventoryPeriodDetail.InventoryPeriod.year
                           && x.InventoryPeriod.id_Company == inventoryPeriodDetail.InventoryPeriod.id_Company && x.InventoryPeriod.id_Division == inventoryPeriodDetail.InventoryPeriod.id_Division
                           && x.InventoryPeriod.id_BranchOffice == inventoryPeriodDetail.InventoryPeriod.id_BranchOffice && x.InventoryPeriod.id_warehouse == inventoryPeriodDetail.InventoryPeriod.id_warehouse).ToList().Count();

                                if (registroactivoa <= 0)
                                {
                                    IdSiguiente = idsig.id;
                                }
                            }
                        }
                        else
                        {

                            var registroactivo = inventoryPeriod.InventoryPeriodDetail.Where(x => x.id != inventoryPeriodDetail.id && idEstadActivre == x.id_PeriodState).ToList().Count();

                            if (registroactivo <= 0)
                            {
                                IdSiguiente = idsi.id;
                            }
                        }
                    }

                }

            }

            return result;

        }

        protected bool validarDetalleInventoryPeriod(InventoryPeriodDetail inventoryPeriodDetail, bool abrir, ref string mensaje, ref int IdAnterior, ref int IdSiguiente, ref int IdEStadoA, ref int IdEStadoC)
        {
            bool result = true;
            IdAnterior = 0;
            IdSiguiente = 0;
            int idEstadActive = 0;
            int idEstadPendiente = 0;
            int idEstadCerrado = 0;
            int anioPeriodo = 0;
            string mensajeCantPeriodo = "";
            string mensajeCantEstado = "";
            var inventoryPeriodSelecionado = db.InventoryPeriod.Where(i => i.id == inventoryPeriodDetail.id_InventoryPeriod).FirstOrDefault();
            inventoryPeriodSelecionado = inventoryPeriodSelecionado ?? new InventoryPeriod();

            string mesName = inventoryPeriodDetail.dateInit.ToString("MMM").ToUpper();
            var vestadosPeriodosInventario = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
            if (vestadosPeriodosInventario != null)
            {
                var idestado = (from es in vestadosPeriodosInventario
                                where es.valueCode.ToUpper().Trim() == "A"
                                select es).FirstOrDefault();

                if (idestado != null)
                {

                    idEstadActive = idestado.id;

                    var idPendiente = (from es in vestadosPeriodosInventario
                                       where es.valueCode.ToUpper().Trim() == "P"
                                       select es).FirstOrDefault();
                    idEstadPendiente = idPendiente.id;

                    var idCerrado = (from es in vestadosPeriodosInventario
                                     where es.valueCode.ToUpper().Trim() == "C"
                                     select es).FirstOrDefault();

                    idEstadCerrado = idCerrado.id;


                    var PeriodoModelSeleccionado = inventoryPeriodSelecionado.InventoryPeriodDetail.Where(x => x.id == inventoryPeriodDetail.id).FirstOrDefault();
                    var PeriodoAnterior = PeriodoModelSeleccionado.periodNumber - 1;
                    var PeriodoSiguiente = PeriodoModelSeleccionado.periodNumber + 1;
                    var anioSeleccionado = PeriodoModelSeleccionado.InventoryPeriod.year;
                    var añoAnterior = PeriodoModelSeleccionado.InventoryPeriod.year - 1;
                    var anioSiguiente = PeriodoModelSeleccionado.InventoryPeriod.year + 1;
                    var Bodega = PeriodoModelSeleccionado.InventoryPeriod.Warehouse.name;
                    InventoryPeriodDetail cantCerrado = null;

                    #region -- Validaciones Parametrizadas --
                    if (PeriodoModelSeleccionado != null && !abrir)
                    {
                        var tipo_bodega = db.WarehouseType.Where(a => a.code == PeriodoModelSeleccionado.InventoryPeriod.Warehouse.WarehouseType.code).FirstOrDefault();
                        if (tipo_bodega != null && (tipo_bodega.code == "BPT" || tipo_bodega.code == "DES01"))
                        {
                            var _periodoPP = db.InventoryPeriodDetail.Where(a => a.InventoryPeriod.Warehouse.WarehouseType.code == "BMP01" && (a.id_PeriodState == idEstadPendiente || a.id_PeriodState == idEstadActive) && a.periodNumber == PeriodoModelSeleccionado.periodNumber).ToList();
                            if (_periodoPP != null)
                            {
                                TempData.Keep("InventoryPeriod");
                                ViewData["EditMessage"] = ErrorMessage("Todas las bodegas de productos en proceso deben estar en estado CERRADO, del periodo " + PeriodoModelSeleccionado.periodNumber + "");
                                mensaje = ErrorMessage("Todas las bodegas de productos en proceso deben estar en estado CERRADO, del periodo " + PeriodoModelSeleccionado.periodNumber + "");
                                result = false;
                                return result;
                            }
                        }
                        if (tipo_bodega != null && tipo_bodega.code == "BPIN")
                        {
                            var inventoryMoveDetailAux = db.InventoryMoveDetail.Where(w => w.id_warehouse == PeriodoModelSeleccionado.InventoryPeriod.id_warehouse &&
                                                        w.InventoryMove.Document.DocumentState.code.Equals("03") &&
                                                        w.InventoryMove.Document.emissionDate.Year == PeriodoModelSeleccionado.InventoryPeriod.year &&
                                                        w.InventoryMove.Document.emissionDate.Month == PeriodoModelSeleccionado.periodNumber).ToList();
                            decimal remainingBalance = inventoryMoveDetailAux.Count() > 0 ? inventoryMoveDetailAux.Sum(s => s.entryAmount - s.exitAmount) : 0;

                            if (remainingBalance != 0)
                            {
                                TempData.Keep("InventoryPeriod");
                                ViewData["EditMessage"] = ErrorMessage("La bodega " + Bodega + " periodo " + PeriodoModelSeleccionado.periodNumber + " debe tener saldo cero.");
                                mensaje = ErrorMessage("La bodega " + Bodega + " periodo " + PeriodoModelSeleccionado.periodNumber + " debe tener saldo cero.");
                                result = false;
                                return result;
                            }
                        }
                    }
                    #endregion
                    #region -- Validacion y Asignacion Apertura --
                    if (abrir)
                    {
                        if (PeriodoAnterior == 0)
                        {
                            var _inventoryPeriodDetailAnterior = db.InventoryPeriodDetail.Where(a => a.InventoryPeriod.year == añoAnterior && a.InventoryPeriod.id_warehouse == PeriodoModelSeleccionado.InventoryPeriod.id_warehouse).OrderByDescending(r => r.periodNumber).FirstOrDefault();
                            if (_inventoryPeriodDetailAnterior == null)
                            {
                                TempData.Keep("InventoryPeriod");
                                ViewData["EditMessage"] = ErrorMessage("No existe creado periodo anterior, Año: " + añoAnterior + " de la Bodega " + Bodega + ".");
                                mensaje = ErrorMessage("No existe creado periodo anterior, Año: " + añoAnterior + " de la Bodega " + Bodega + ".");
                                result = false;
                                return result;
                            }
                            var _idAnterior = _inventoryPeriodDetailAnterior.id;
                            IdAnterior = _idAnterior;
                            PeriodoAnterior = _inventoryPeriodDetailAnterior.periodNumber;
                            cantCerrado = db.InventoryPeriodDetail?.Where(x => x.id == _idAnterior && (x.id_PeriodState == idEstadPendiente || x.id_PeriodState == idEstadActive))?.FirstOrDefault();
                            mensajeCantPeriodo = $" anterior {añoAnterior}/{PeriodoAnterior} ";
                            mensajeCantEstado = "CERRADO";
                        }
                        else
                        {
                            int? IdAnteriorResult = inventoryPeriodSelecionado.InventoryPeriodDetail.Where(x => x.periodNumber == PeriodoAnterior)?.FirstOrDefault()?.id;
                            if (!IdAnteriorResult.HasValue)
                            {
                                TempData.Keep("InventoryPeriod");
                                ViewData["EditMessage"] = ErrorMessage("No existe creado periodo anterior, Año: " + anioSeleccionado + " de la Bodega " + Bodega + ".");
                                mensaje = ErrorMessage("No existe creado periodo anterior, Año: " + anioSeleccionado + " de la Bodega " + Bodega + ".");
                                result = false;
                                return result;
                            }
                            IdAnterior = IdAnteriorResult.Value;
                            cantCerrado = inventoryPeriodSelecionado.InventoryPeriodDetail?
                                                                .Where(x => x.id != inventoryPeriodDetail.id && (x.id_PeriodState == idEstadPendiente || x.id_PeriodState == idEstadActive) && x.periodNumber < inventoryPeriodDetail.periodNumber)?
                                                                .FirstOrDefault();
                            mensajeCantPeriodo = $" anterior {anioSeleccionado}/{PeriodoAnterior} ";
                            mensajeCantEstado = "CERRADO";
                        }
                    }
                    #endregion
                    #region -- Validaciones y Asignacion Cierre --
                    if (!abrir)
                    {
                        if (PeriodoAnterior == 0)
                        {
                            var _inventoryPeriodDetailAnterior = db.InventoryPeriodDetail.Where(a => a.InventoryPeriod.year == añoAnterior && a.InventoryPeriod.id_warehouse == PeriodoModelSeleccionado.InventoryPeriod.id_warehouse).OrderByDescending(r => r.periodNumber).FirstOrDefault();
                            if (_inventoryPeriodDetailAnterior != null)
                            {
                                var _idAnterior = _inventoryPeriodDetailAnterior.id;
                                IdAnterior = _idAnterior;
                                PeriodoAnterior = _inventoryPeriodDetailAnterior.periodNumber;
                                cantCerrado = db.InventoryPeriodDetail?.Where(x => x.id == _idAnterior && (x.id_PeriodState == idEstadPendiente || x.id_PeriodState == idEstadActive))?.FirstOrDefault();
                                mensajeCantPeriodo = $" anterior {añoAnterior}/{PeriodoAnterior} ";
                                mensajeCantEstado = "CERRADO";
                            }
                            
                        }
                        else
                        {
                            int? IdAnteriorResult = inventoryPeriodSelecionado.InventoryPeriodDetail.Where(x => x.periodNumber == PeriodoAnterior)?.FirstOrDefault()?.id;
                            if (IdAnteriorResult.HasValue)
                            {
                                IdAnterior = IdAnteriorResult.Value;
                                cantCerrado = inventoryPeriodSelecionado.InventoryPeriodDetail?
                                                                    .Where(x => x.id != inventoryPeriodDetail.id && (x.id_PeriodState == idEstadPendiente || x.id_PeriodState == idEstadActive) && x.periodNumber < inventoryPeriodDetail.periodNumber)?
                                                                    .FirstOrDefault();
                                mensajeCantPeriodo = $" anterior {anioSeleccionado}/{PeriodoAnterior} ";
                                mensajeCantEstado = "CERRADO";
                            }
                           
                        }
                        if (cantCerrado == null)
                        {
                            if (PeriodoSiguiente == 13)
                            {
                                var _inventoryPeriodDetailPosterior = db.InventoryPeriodDetail.Where(a => a.InventoryPeriod.year == anioSiguiente && a.InventoryPeriod.id_warehouse == PeriodoModelSeleccionado.InventoryPeriod.id_warehouse).OrderByDescending(r => r.periodNumber).FirstOrDefault();
                                if (_inventoryPeriodDetailPosterior == null)
                                {
                                    TempData.Keep("InventoryPeriod");
                                    ViewData["EditMessage"] = ErrorMessage("No existe creado periodo posterior, Año: " + anioSiguiente + " de la Bodega " + Bodega + ".");
                                    mensaje = ErrorMessage("No existe creado periodo posterior, Año: " + anioSiguiente + " de la Bodega " + Bodega + ".");
                                    result = false;
                                    return result;
                                }
                                //var _IdSiguiente = _inventoryPeriodDetailPosterior.id;
                                //PeriodoSiguiente = _inventoryPeriodDetailPosterior.periodNumber;
                                //
                                //var periodoMesAbierto = db.InventoryPeriodDetail?.Where(x => x.id == _IdSiguiente && x.id_PeriodState == idEstadActive)?.FirstOrDefault();
                                //if (periodoMesAbierto == null)
                                //{
                                //    IdSiguiente = _IdSiguiente;
                                //}
                                
                            }
                            else
                            {
                                int? IdSiguienteResult = inventoryPeriodSelecionado.InventoryPeriodDetail.Where(x => x.periodNumber == PeriodoSiguiente)?.FirstOrDefault()?.id;
                                if (!IdSiguienteResult.HasValue)
                                {
                                    TempData.Keep("InventoryPeriod");
                                    ViewData["EditMessage"] = ErrorMessage("No existe creado periodo posterior, Año: " + anioSeleccionado + " de la Bodega " + Bodega + ".");
                                    mensaje = ErrorMessage("No existe creado periodo posterior, Año: " + anioSeleccionado + " de la Bodega " + Bodega + ".");
                                    result = false;
                                    return result;
                                }
                                //var _IdSiguiente = IdSiguienteResult.Value;
                                //
                                //var periodoMesAbierto = inventoryPeriodSelecionado.InventoryPeriodDetail?
                                //                                    .Where(x => x.id != inventoryPeriodDetail.id && x.id_PeriodState == idEstadActive && x.periodNumber > inventoryPeriodDetail.periodNumber)?
                                //                                    .FirstOrDefault();
                                //if (periodoMesAbierto == null)
                                //{
                                //    IdSiguiente = _IdSiguiente;
                                //}
                                
                            }
                        }

                    }
                    #endregion
                     
                    //IdSiguiente = (inventoryPeriodSelecionado.InventoryPeriodDetail?.Where(x => x.periodNumber == PeriodoSiguiente)?.FirstOrDefault()?.id ?? 0);

                    if (cantCerrado != null && result == true)
                    {
                        TempData.Keep("InventoryPeriod");
                        mensaje = ErrorMessage($"El periodo {mensajeCantPeriodo} de la Bodega {Bodega} debe tener estado {mensajeCantEstado}");
                        ViewData["EditMessage"] = mensaje;
                        
                        result = false;
                        return result;
                    }

                    IdEStadoA = idEstadActive;
                    IdEStadoC = idEstadCerrado;
                }

            }

            return result;

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryPeriodDetailPartialUpdate(int id)
        {

            
            InventoryPeriod inventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);

            InventoryPeriodDetail inventoryPeriodDetail = inventoryPeriod.InventoryPeriodDetail.Where(it => it.id == id).FirstOrDefault();


            var id_PeriodState = inventoryPeriodDetail.id_PeriodState;
            var vestado = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
            if (vestado != null)
            {
                var idestado = (from es in vestado
                                where es.valueCode.ToUpper().Trim() == "C"
                                select es).FirstOrDefault();



                inventoryPeriodDetail.id_PeriodState = idestado.id;
                   
            }
            
            inventoryPeriod = inventoryPeriod ?? db.InventoryPeriod.Where(i => i.id == inventoryPeriod.id).FirstOrDefault();
            inventoryPeriod = inventoryPeriod ?? new InventoryPeriod();

            if (ModelState.IsValid)
            {
                try
                {

                    var modelItem = inventoryPeriod.InventoryPeriodDetail.Where(it => it.id == inventoryPeriodDetail.id).FirstOrDefault();


                    if (modelItem != null)
                    {
                        string mensaje = "";
                        int IdSiguiente = 0;
                        int IdSEstadoA = 0;

                        if (!validarDetalle(inventoryPeriodDetail, ref mensaje, ref IdSiguiente, ref IdSEstadoA))
                        {
                            inventoryPeriodDetail.id_PeriodState = id_PeriodState;
                            ViewData["EditError"] = mensaje;

                            TempData.Keep("InventoryPeriod");
                            var modelre = inventoryPeriod?.InventoryPeriodDetail.ToList() ?? new List<InventoryPeriodDetail>();

                            TempData.Keep("InventoryPeriod");

                            var resultd = new
                            {
                                id = 1,
                                mensaje= mensaje
                            };
                            return Json(resultd, JsonRequestBehavior.AllowGet);

                        }

                        modelItem.dateInit = inventoryPeriodDetail.dateInit;
                        modelItem.dateEnd = inventoryPeriodDetail.dateEnd;
                        modelItem.id_PeriodState = inventoryPeriodDetail.id_PeriodState;
                        this.UpdateModel(modelItem);

                        if (IdSiguiente > 0 && IdSEstadoA > 0)
                        {
                            modelItem = inventoryPeriod.InventoryPeriodDetail.Where(it => it.id == IdSiguiente).FirstOrDefault();
                            if(modelItem !=null)
                            { 
                            modelItem.id_PeriodState = IdSEstadoA;
                            }


                        }
                        TempData["InventoryPeriod"] = inventoryPeriod;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("InventoryPeriod");

            var model = inventoryPeriod?.InventoryPeriodDetail.ToList() ?? new List<InventoryPeriodDetail>();

            var result = new
            {
                id = 0
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }




        #endregion

        
    }
}
