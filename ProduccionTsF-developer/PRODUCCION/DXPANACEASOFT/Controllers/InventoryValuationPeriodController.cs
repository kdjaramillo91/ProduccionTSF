using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;
using System.Data;
using System.Globalization;
using System.Threading;

namespace DXPANACEASOFT.Controllers
{
    public class InventoryValuationPeriodController : DefaultController
    {

        public ActionResult Index()
        {
            return View();
        }
       public  List<InventoryValuationPeriodDetail> cargaFecha(int year,string typeperiodo, int id_estado ) {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd HH:mm:ss";
            culture.DateTimeFormat.LongTimePattern = "";
            Thread.CurrentThread.CurrentCulture = culture;

            List< InventoryValuationPeriodDetail > result = new List<InventoryValuationPeriodDetail>();
            var dfechaini = new DateTime(year, 1, 1);
            List<InventoryValuationPeriodDetail> resulttemp = new List<InventoryValuationPeriodDetail>();
            DateTime fechafin;
            DateTime fechaini;
            switch (typeperiodo.ToUpper().Trim())
            {
                case "M":
             
                    for (int i=1; i<13; i++)
                    {
                        fechaini = new DateTime(year, i, 1);

                        if (i < 12)
                        {
                            fechafin = (new DateTime(year, i + 1, 1)).AddDays(-1);
                        }
                        else
                        {
                            fechafin = (new DateTime(year, 12, 31));
                        }

                    
                        InventoryValuationPeriodDetail det = new InventoryValuationPeriodDetail()
                        {
                            periodNumber = i,
                            dateInit = fechaini,
                            dateEnd = fechafin,
                            id_PeriodState = id_estado
                        };
                        resulttemp.Add(det);
                        };
                    result = resulttemp;


                    break;
                case "Q":
                    List<InventoryValuationPeriodDetail> resulttemQp = new List<InventoryValuationPeriodDetail>();
                    int qnumber = 1;
                    for (int i = 1; i < 13; i++)
                    {
                         fechaini = new DateTime(year, i, 1);
                         fechafin = (new DateTime(year, i , 15));

                        InventoryValuationPeriodDetail det = new InventoryValuationPeriodDetail()
                        {
                            periodNumber = qnumber,
                            dateInit = fechaini,
                            dateEnd = fechafin,
                            id_PeriodState = id_estado
                        };

                        resulttemQp.Add(det);
                        qnumber = qnumber + 1;
                        fechaini = new DateTime(year, i, 16);
                        if(i<12)
                        { 
                        fechafin = (new DateTime(year, i + 1, 1)).AddDays(-1);
                        }
                        else
                        {
                            fechafin = (new DateTime(year, 12, 31));
                        }
                        det = new InventoryValuationPeriodDetail()
                        {
                            periodNumber = qnumber,
                            dateInit = fechaini,
                            dateEnd = fechafin,
                            id_PeriodState = id_estado
                        };

                        qnumber = qnumber + 1;
                        resulttemQp.Add(det);
                    };
                    result = resulttemQp;

                    break;
                default:
                    var primerafecha = dfechaini.AddDays(1 - (int)(dfechaini.DayOfWeek));
                    IEnumerable<InventoryValuationPeriodDetail> lfsemanas = Enumerable.Range(0, 54)
                        .Select(i => new {
                            dinit = primerafecha.AddDays(i * 7)
                        }
                    ).TakeWhile(x => x.dinit.Year <= dfechaini.Year)
                    .Select(x => new { x.dinit, dfin = x.dinit.AddDays(6) })
                    .SkipWhile(x => x.dfin < dfechaini.AddDays(1))
                    .Select((x, i) => new InventoryValuationPeriodDetail { periodNumber = i + 1, dateInit = x.dinit, dateEnd = x.dfin, id_PeriodState = id_estado });
                    result = lfsemanas.ToList();
                    break;
            }
       

            return result;
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult InventoryValuationPeriodResults(InventoryValuationPeriod InventoryValuationPeriod, int? anio, int? id_InventoryValuationPeriodType)
        {
            var model = db.InventoryValuationPeriod.ToList();

            #region  FILTERS

          

            if (id_InventoryValuationPeriodType != null && id_InventoryValuationPeriodType > 0)
            {
                model = model.Where(o => o.id_PeriodType == id_InventoryValuationPeriodType).ToList();
            }

            if (anio != null && anio > 0)
            {
                model = model.Where(o => o.year == anio).ToList();
            }

            model = model.Where(o => o.id_Company == this.ActiveCompanyId && o.id_Division== ActiveDivision.id  && o.id_BranchOffice == ActiveSucursal.id ).ToList();
            #endregion

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_InventoryValuationPeriodResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }


        #endregion

        #region InventoryValuationPeriod HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryValuationPeriodPartial()
        {
            var model = (TempData["model"] as List<InventoryValuationPeriod>);
            model = model ?? new List<InventoryValuationPeriod>();
            TempData.Keep("model");
            return PartialView("_InventoryValuationPeriodPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion

        #region Edit InventoryValuationPeriod
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditInventoryValuationPeriod(int id, int[] orderDetails)
        {
            InventoryValuationPeriod InventoryValuationPeriod = db.InventoryValuationPeriod.Where(o => o.id == id).FirstOrDefault();

            if (InventoryValuationPeriod == null)
            {

                InventoryValuationPeriod = new InventoryValuationPeriod
                {
                     id_Company = ActiveUser.id_company,
                     id_Division= ActiveDivision.id,
                     id_BranchOffice= ActiveSucursal.id,
                     id_userUpdate = ActiveUser.id,
                     dateUpdate = DateTime.Now,
                     year = DateTime.Now.Year,
                     isActive = true,
                     id_userCreate= ActiveUser.id,
                     dateCreate = DateTime.Now,
      
                };
            }
            TempData["InventoryValuationPeriod"] = InventoryValuationPeriod;
            TempData.Keep("InventoryValuationPeriod");

            return PartialView("_FormEditInventoryValuationPeriod", InventoryValuationPeriod);
        }
        #endregion

        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_InventoryValuationPeriod)
        {
            TempData.Keep("InventoryValuationPeriod");
            int index = db.InventoryValuationPeriod.Where(o => o.id_Company == this.ActiveCompanyId && o.id_Division == ActiveDivision.id && o.id_BranchOffice == ActiveSucursal.id).OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_InventoryValuationPeriod);
            var result = new
            {
                maximunPages = db.InventoryValuationPeriod.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            InventoryValuationPeriod InventoryValuationPeriod = db.InventoryValuationPeriod.Where(o => o.id_Company == this.ActiveCompanyId && o.id_Division == ActiveDivision.id && o.id_BranchOffice == ActiveSucursal.id).OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (InventoryValuationPeriod != null)
            {
                TempData["InventoryValuationPeriod"] = InventoryValuationPeriod;
                TempData.Keep("InventoryValuationPeriod");
                return PartialView("_InventoryValuationPeriodMainFormPartial", InventoryValuationPeriod);
            }

            TempData.Keep("InventoryValuationPeriod");

            return PartialView("_InventoryValuationPeriodMainFormPartial", new InventoryValuationPeriod());
        }
        #endregion
        #region Validacion

        public Boolean validate(InventoryValuationPeriod item)
        {
            Boolean wsreturn = true;
            try
            {
                InventoryValuationPeriod conInventoryValuationPeriod = (TempData["InventoryValuationPeriod"] as InventoryValuationPeriod);

                 var cantre = db.InventoryValuationPeriod.Where(x =>  x.id_Company == item.id_Company && item.id_Division == x.id_Division && item.id_BranchOffice == x.id_BranchOffice && x.id_PeriodType== item.id_PeriodType && x.year == item.year).ToList().Count();

                if (cantre > 0)
                {
                    TempData.Keep("InventoryValuationPeriod");
                    ViewData["EditMessage"] = ErrorMessage("Periodo ya se encuentra creado");
                    wsreturn = false;
                }
            }
            catch (Exception)
            {


            }

            return wsreturn;

        }
        #endregion


        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryValuationPeriodPartialAddNew(bool approve, InventoryValuationPeriod item)
        {

            InventoryValuationPeriod conInventoryPeriod = (TempData["InventoryPeriod"] as InventoryValuationPeriod);
            item.InventoryValuationPeriodDetail = new List<InventoryValuationPeriodDetail>();
            item.dateCreate = DateTime.Now;
            item.dateUpdate = DateTime.Now;
            item.id_Company = ActiveUser.id_company;
            item.id_Division = ActiveDivision.id;
            item.id_BranchOffice = ActiveSucursal.id;
            item.id_userUpdate = ActiveUser.id;
            item.id_userCreate = ActiveUser.id;
            
            if (!validate(item))
            {
                return PartialView("_InventoryValuationPeriodMainFormPartial", item);
            }
             var vestado = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
            if (vestado == null)
            {
                TempData.Keep("InventoryValuationPeriod");
                ViewData["EditMessage"] = ErrorMessage("No esta definido los Estados...");
                return PartialView("_InventoryValuationPeriodMainFormPartial", item);
            }
            var vTPGV1 = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("TPGV1");
            if (vTPGV1 == null)
            {
                TempData.Keep("InventoryValuationPeriod");
                ViewData["EditMessage"] = ErrorMessage("No esta definido el Tipo de Perido...");
                return PartialView("_InventoryValuationPeriodMainFormPartial", item);

            }

            var idestado = (from es in vestado
                            where es.valueCode.ToUpper().Trim() == "P"
                            select es).FirstOrDefault();

            var idTPGV1 = (from es in vTPGV1
                           where es.id == item.id_PeriodType
                           select es).FirstOrDefault();

            List<InventoryValuationPeriodDetail> detail = cargaFecha(item.year, idTPGV1.valueCode, idestado.id);
            DBContext dbemp = new DBContext();
            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region InventoryValuationPeriod

                    item.InventoryValuationPeriodDetail = new List<InventoryValuationPeriodDetail>();
                    item.InventoryValuationPeriodDetail = detail;
                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;
                    item.id_Company = ActiveUser.id_company;
                    item.id_Division = ActiveDivision.id;
                    item.id_BranchOffice = ActiveSucursal.id;
                    item.id_userUpdate = ActiveUser.id;
                    item.id_userCreate = ActiveUser.id;
                    dbemp.InventoryValuationPeriod.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();


                   TempData["InventoryValuationPeriod"] = item;
                    TempData.Keep("InventoryValuationPeriod");
                    ViewData["EditMessage"] = SuccessMessage("Periodo: " + item.year + " guardado exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("InventoryValuationPeriod");
                    item = (TempData["InventoryValuationPeriod"] as InventoryValuationPeriod);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            return PartialView("_InventoryValuationPeriodMainFormPartial", item);
        }
        [HttpPost, ValidateInput(false)]

        public ActionResult InventoryValuationPeriodPartialUpdate(bool approve, InventoryValuationPeriod item)
        {
            var vestado = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
            if (vestado == null)
            {
                TempData.Keep("InventoryValuationPeriod");
                ViewData["EditMessage"] = ErrorMessage("No esta definido los Estados...");
                return PartialView("_InventoryValuationPeriodMainFormPartial", item);
            }
          
            var idestado = (from es in vestado
                            where es.valueCode.ToUpper().Trim() == "C"
                            select es).FirstOrDefault();


            var idActivo = (from es in vestado
                            where es.valueCode.ToUpper().Trim() == "A"
                            select es).FirstOrDefault();


            DBContext dbemp = new DBContext();

            InventoryValuationPeriod modelItem = dbemp.InventoryValuationPeriod.Where(r => r.id == item.id).FirstOrDefault();
            if (modelItem != null)
            {

                InventoryValuationPeriod conInventoryValuationPeriod = (TempData["InventoryValuationPeriod"] as InventoryValuationPeriod);


            var act =  conInventoryValuationPeriod.InventoryValuationPeriodDetail.Where(x => x.id_PeriodState == idActivo.id).ToList().Count;
                if(act<=0)
                {
                    TempData.Keep("InventoryValuationPeriod");
                    ViewData["EditMessage"] = ErrorMessage("Debe existir al menos un periodo ABIERTO...");
                    return PartialView("_InventoryValuationPeriodMainFormPartial", conInventoryValuationPeriod);
                }


                using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
                {
                    try
                    {
                        #region  DETAILS
                        if (conInventoryValuationPeriod?.InventoryValuationPeriodDetail != null)
                        {
                            var details = conInventoryValuationPeriod.InventoryValuationPeriodDetail.ToList();
                            foreach (var detail in details)
                            {
                                InventoryValuationPeriodDetail inventoryValuationPeriodDetail = modelItem.InventoryValuationPeriodDetail.Where(d => d.id == detail.id).FirstOrDefault();

                                if (inventoryValuationPeriodDetail != null)
                                {
                                    inventoryValuationPeriodDetail.id_PeriodState = detail.id_PeriodState;
                                    inventoryValuationPeriodDetail.dateInit = detail.dateInit;
                                    inventoryValuationPeriodDetail.dateEnd = detail.dateEnd;

                                    if (idestado.id== detail.id_PeriodState)
                                    {
                                        inventoryValuationPeriodDetail.dateClose = DateTime.Now;
                                        inventoryValuationPeriodDetail.isClosed =true;
                                    }
                                    dbemp.InventoryValuationPeriodDetail.Attach(inventoryValuationPeriodDetail);
                                    dbemp.Entry(inventoryValuationPeriodDetail).State = EntityState.Modified;
                                 }
                            }
                        }
                        #endregion
                        modelItem.dateUpdate = DateTime.Now;
                        modelItem.id_Company = ActiveUser.id_company;
                        modelItem.id_Division = ActiveDivision.id;
                        modelItem.id_BranchOffice = ActiveSucursal.id;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.id_userCreate = ActiveUser.id;

                        modelItem.isActive = item.isActive;
                        dbemp.InventoryValuationPeriod.Attach(modelItem);
                        dbemp.Entry(modelItem).State = EntityState.Modified;
                        dbemp.SaveChanges();
                        trans.Commit();
                        TempData["InventoryValuationPeriod"] = modelItem;
                        TempData.Keep("InventoryValuationPeriod");
                        ViewData["EditMessage"] = SuccessMessage("Periodo: " + item.year + " guardado exitosamente");

                    }
                    catch (Exception e)
                    {
                        TempData.Keep("InventoryValuationPeriod");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("InventoryValuationPeriod");
            return PartialView("_InventoryValuationPeriodMainFormPartial", modelItem);
        }
        #endregion

        #region InventoryValuationPeriod Gridview

        [ValidateInput(false)]
        public ActionResult InventoryValuationPeriodPartial(int? id)
        {
            if (id != null)
            {
                ViewData["InventoryValuationPeriodToCopy"] = db.InventoryValuationPeriod.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.InventoryValuationPeriod.Where(o => o.id_Company == this.ActiveCompanyId);
            return PartialView("_InventoryValuationPeriodPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryValuationPeriodPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.InventoryValuationPeriod.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.InventoryValuationPeriod.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("InventoryValuationPeriodo : " + (item?.year.ToString() ?? "") + " desactivado exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.InventoryValuationPeriod.Where(o => o.id_Company == this.ActiveCompanyId);
            return PartialView("_InventoryValuationPeriodPartial", model.ToList());
        }

        public ActionResult DeleteSelectedInventoryValuationPeriod(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var InventoryValuationPeriods = db.InventoryValuationPeriod.Where(i => ids.Contains(i.id));
                        foreach (var vInventoryValuationPeriod in InventoryValuationPeriods)
                        {
                            vInventoryValuationPeriod.isActive = false;

                            vInventoryValuationPeriod.id_userUpdate = ActiveUser.id;
                            vInventoryValuationPeriod.dateUpdate = DateTime.Now;

                            db.InventoryValuationPeriod.Attach(vInventoryValuationPeriod);
                            db.Entry(vInventoryValuationPeriod).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("InventoryValuationPeriodo desactivados exitosamente");
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        ViewData["EditMessage"] = ErrorMessage();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            var model = db.InventoryValuationPeriod.Where(o => o.id_Company == this.ActiveCompanyId);
            return PartialView("_InventoryValuationPeriodPartial", model.ToList());
        }

        #endregion

        #region  Inventory details

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryValuationPeriodDetailPartial()
        {
            InventoryValuationPeriod inventoryValuationPeriod = (TempData["InventoryValuationPeriod"] as InventoryValuationPeriod);
            inventoryValuationPeriod = inventoryValuationPeriod ?? new InventoryValuationPeriod();
            inventoryValuationPeriod.InventoryValuationPeriodDetail = inventoryValuationPeriod.InventoryValuationPeriodDetail ?? new List<InventoryValuationPeriodDetail>();

            var model = inventoryValuationPeriod.InventoryValuationPeriodDetail;

            TempData.Keep("InventoryValuationPeriod");

            return PartialView("_Details", model.ToList());
        }

    private bool  validarDetalle(InventoryValuationPeriodDetail inventoryValuationPeriodDetail, ref string mensaje, ref int IdSiguiente, ref int IdEStadoA)
        {
            bool result = true;
            IdSiguiente = 0;
            int idEstadActivre = 0;
            int idEstadPendiente = 0;
            int idEstadCerrado = 0;
            InventoryValuationPeriod inventoryValuationPeriod = (TempData["InventoryValuationPeriod"] as InventoryValuationPeriod);

            inventoryValuationPeriod = inventoryValuationPeriod ?? db.InventoryValuationPeriod.Where(i => i.id == inventoryValuationPeriod.id).FirstOrDefault();
            inventoryValuationPeriod = inventoryValuationPeriod ?? new InventoryValuationPeriod();

            var cantre = inventoryValuationPeriod.InventoryValuationPeriodDetail.Where(x => x.id != inventoryValuationPeriodDetail.id && inventoryValuationPeriodDetail.dateInit >= x.dateInit && inventoryValuationPeriodDetail.dateInit <= x.dateEnd ).ToList().Count();
            if (cantre > 0)
            {
                mensaje = ErrorMessage("Ya existe un Periodo con el mismo rango de fecha");
                TempData.Keep("InventoryValuationPeriod");
                ViewData["EditMessage"] = mensaje;
                result = false;
                return result;
            }


            var cantrefin = inventoryValuationPeriod.InventoryValuationPeriodDetail.Where(x => x.id != inventoryValuationPeriodDetail.id  && inventoryValuationPeriodDetail.dateEnd >= x.dateInit && inventoryValuationPeriodDetail.dateEnd <= x.dateEnd ).ToList().Count();
            if (cantrefin > 0)
            {
                mensaje = ErrorMessage("Ya existe un Periodo con el mismo rango de fecha");
                TempData.Keep("InventoryValuationPeriod");
                ViewData["EditMessage"] = mensaje;
                result = false;
                return result;
            }

            var vestado = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
            if (vestado != null)
            {
                var idestado = (from es in vestado
                                where es.valueCode.ToUpper().Trim() == "A"
                                select es).FirstOrDefault();

                if(idestado != null)
                {

                    idEstadActivre = idestado.id;

                    var idPendiente = (from es in vestado
                                    where es.valueCode.ToUpper().Trim() == "P"
                                    select es).FirstOrDefault();
                     idEstadPendiente = idPendiente.id;
                    
                    var idCerrado = (from es in vestado
                                       where es.valueCode.ToUpper().Trim() == "C"
                                       select es).FirstOrDefault();

                    idEstadCerrado = idCerrado.id;

                    if (inventoryValuationPeriodDetail.id_PeriodState== idestado.id)
                    {
                        var cantestado = inventoryValuationPeriod.InventoryValuationPeriodDetail.Where(x => x.id != inventoryValuationPeriodDetail.id && inventoryValuationPeriodDetail.id_PeriodState == x.id_PeriodState).ToList().Count();

                        if (cantestado > 0)
                        {
                            TempData.Keep("InventoryValuationPeriod");
                            ViewData["EditMessage"] = ErrorMessage("Ya existe un Periodo Activo");
                            mensaje = ErrorMessage("Ya existe un Periodo Activo");
                            result = false;
                            return result;
                        }

                        var cantCerrado = inventoryValuationPeriod.InventoryValuationPeriodDetail.Where(x => x.id != inventoryValuationPeriodDetail.id && (x.id_PeriodState == idEstadPendiente || x.id_PeriodState == idEstadActivre) && x.periodNumber < inventoryValuationPeriodDetail.periodNumber).ToList().Count();

                        if (cantCerrado > 0 && result ==true)
                        {
                            TempData.Keep("InventoryValuationPeriod");
                            ViewData["EditMessage"] = ErrorMessage(" Todos los periodos anteriores  deben estar como CERRADO");
                            mensaje = ErrorMessage("Todos los periodos anteriores  deben estar como CERRADO");
                            result = false;
                            return result;
                        }


                        var cantPendiente = inventoryValuationPeriod.InventoryValuationPeriodDetail.Where(x => x.id != inventoryValuationPeriodDetail.id && (x.id_PeriodState == idEstadCerrado || x.id_PeriodState == idEstadActivre)  && x.periodNumber > inventoryValuationPeriodDetail.periodNumber).ToList().Count();

                        if (cantPendiente > 0 && result == true)
                        {
                            TempData.Keep("InventoryValuationPeriod");
                            ViewData["EditMessage"] = ErrorMessage(" Todos los periodos anteriores  deben estar como PENDIENTE");
                            mensaje = ErrorMessage("Todos los periodos anteriores  deben estar como PENDIENTE");
                            result = false;
                            return result;
                        }


                        if (!((inventoryValuationPeriodDetail.dateInit.Year == DateTime.Now.Year && inventoryValuationPeriodDetail.dateInit.Month <= DateTime.Now.Month) ||
                            (inventoryValuationPeriodDetail.dateInit.Year < DateTime.Now.Year)
                            )  && result == true)
                        {
                            TempData.Keep("InventoryValuationPeriod");
                           ViewData["EditMessage"] = ErrorMessage("El Periodo abierto debe ser igual o menor a la fecha actual");
                            mensaje = ErrorMessage("El Periodo abierto debe ser igual o menor a la fecha actual");
                            result = false;
                            return result;
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
                    if (inventoryValuationPeriodDetail.id_PeriodState == idestado.id)
                    {
                        var idsi = inventoryValuationPeriod.InventoryValuationPeriodDetail.Where(x => x.id != inventoryValuationPeriodDetail.id && idestado.id != x.id_PeriodState && x.periodNumber > inventoryValuationPeriodDetail.periodNumber).FirstOrDefault();

                        if (idsi ==null)
                        {
                            TempData.Keep("InventoryValuationPeriod");
                            ViewData["EditMessage"] = ErrorMessage("No existe otro periodo Pendiente");
                            mensaje = ErrorMessage("No existe otro periodo Pendiente");
                            result = false;
                            return result;
                        }
                        else
                        {

                            var registroactivo = inventoryValuationPeriod.InventoryValuationPeriodDetail.Where(x => x.id != inventoryValuationPeriodDetail.id && idEstadActivre == x.id_PeriodState).ToList().Count();

                            if(registroactivo<=0)
                            { 
                                IdSiguiente = idsi.id;
                            }
                        }
                    }

                }

            }

            return result;

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryValuationPeriodDetailPartialUpdate(InventoryValuationPeriodDetail inventoryValuationPeriodDetail)
        {
            InventoryValuationPeriod inventoryValuationPeriod = (TempData["InventoryValuationPeriod"] as InventoryValuationPeriod);

            inventoryValuationPeriod = inventoryValuationPeriod ?? db.InventoryValuationPeriod.Where(i => i.id == inventoryValuationPeriod.id).FirstOrDefault();
            inventoryValuationPeriod = inventoryValuationPeriod ?? new InventoryValuationPeriod();

            if (ModelState.IsValid)
            {
                try
                {

                    var modelItem = inventoryValuationPeriod.InventoryValuationPeriodDetail.Where(it => it.id == inventoryValuationPeriodDetail.id).FirstOrDefault();
                 

                    if (modelItem != null)
                    {
                        string mensaje = "";
                     int   IdSiguiente = 0;
                        int IdSEstadoA = 0;

                        if (!validarDetalle(inventoryValuationPeriodDetail, ref mensaje, ref IdSiguiente, ref IdSEstadoA))
                        {
                            ViewData["EditError"] = mensaje;

                            TempData.Keep("InventoryValuationPeriod");
                            var modelre = inventoryValuationPeriod?.InventoryValuationPeriodDetail.ToList() ?? new List<InventoryValuationPeriodDetail>();

                            return PartialView("_Details", modelre.ToList());
                        }



                        modelItem.dateInit = inventoryValuationPeriodDetail.dateInit;
                        modelItem.dateEnd = inventoryValuationPeriodDetail.dateEnd;
                        modelItem.id_PeriodState = inventoryValuationPeriodDetail.id_PeriodState;
                        this.UpdateModel(modelItem);

                        if(IdSiguiente >0 && IdSEstadoA>0)
                        {
                             modelItem = inventoryValuationPeriod.InventoryValuationPeriodDetail.Where(it => it.id == IdSiguiente).FirstOrDefault();
                             modelItem.id_PeriodState = IdSEstadoA;                           

                        }
                        TempData["InventoryValuationPeriod"] = inventoryValuationPeriod;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("InventoryValuationPeriod");

            var model = inventoryValuationPeriod?.InventoryValuationPeriodDetail.ToList() ?? new List<InventoryValuationPeriodDetail>();

            return PartialView("_Details", model.ToList());
        }




        #endregion
    }
}
