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
using DXPANACEASOFT.Services;
using DXPANACEASOFT.Models.BackgroundProcessManagement;
using DevExpress.Web.Internal;
using DXPANACEASOFT.Dapper;

namespace DXPANACEASOFT.Controllers
{
    public class InventoryPeriodController : DefaultController
    {

        public ActionResult Index()
        {
            return View();
        }
       public  List<InventoryPeriodDetail> cargaFecha(int year,string typeperiodo, int id_estado ) {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd HH:mm:ss";
            culture.DateTimeFormat.LongTimePattern = "";
            Thread.CurrentThread.CurrentCulture = culture;

            List< InventoryPeriodDetail > result = new List<InventoryPeriodDetail>();
            var dfechaini = new DateTime(year, 1, 1);
            List<InventoryPeriodDetail> resulttemp = new List<InventoryPeriodDetail>();
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

                    
                        InventoryPeriodDetail det = new InventoryPeriodDetail()
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
                    List<InventoryPeriodDetail> resulttemQp = new List<InventoryPeriodDetail>();
                    int qnumber = 1;
                    for (int i = 1; i < 13; i++)
                    {
                         fechaini = new DateTime(year, i, 1);
                         fechafin = (new DateTime(year, i , 15));

                        InventoryPeriodDetail det = new InventoryPeriodDetail()
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
                        det = new InventoryPeriodDetail()
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
                case "D":

                    var fechaFin = dfechaini.AddYears(1).AddDays(-1);
                    var fechaPivote = dfechaini;
                    var listaPeriodos = new List<InventoryPeriodDetail>();

                    int cont = 1;
                    while(fechaPivote <= fechaFin)
                    {
                        listaPeriodos.Add(new InventoryPeriodDetail() {
                            periodNumber = cont,
                            dateInit = fechaPivote,
                            dateEnd = fechaPivote,
                            id_PeriodState = id_estado,
                        });

                        fechaPivote = fechaPivote.AddDays(1);
                        cont++;
                    }

                    result = listaPeriodos;

                    //var primerafechad = dfechaini.AddDays(1 - (int)(dfechaini.Day));
                    //IEnumerable<InventoryPeriodDetail> lfDias = Enumerable.Range(0, 365)
                    //    .Select(i => new {
                    //        dinit = primerafechad.AddDays(i + 1)
                    //    }
                    //).TakeWhile(x => x.dinit.Year <= dfechaini.Year)
                    //.Select(x => new { x.dinit, dfin = x.dinit })
                    //.SkipWhile(x => x.dfin < dfechaini.AddDays(1))
                    //.Select((x, i) => new InventoryPeriodDetail { periodNumber = i + 1, dateInit = x.dinit, dateEnd = x.dfin, id_PeriodState = id_estado });
                    //result = lfDias.ToList();
                    break;

                //case "D":
                //    List<InventoryPeriodDetail> resulttemDr = new List<InventoryPeriodDetail>();
                //    int qnumberD = 1;
                //    for (int i = 1; i < 13; i++)
                //    {
                //        fechaini = new DateTime(year, i, 1);
                //        fechafin = (new DateTime(year, i, 15));

                //        InventoryPeriodDetail det = new InventoryPeriodDetail()
                //        {
                //            periodNumber = qnumberD,
                //            dateInit = fechaini,
                //            dateEnd = fechafin,
                //            id_PeriodState = id_estado
                //        };

                //        resulttemDr.Add(det);
                //        qnumberD = qnumberD + 1;
                //        fechaini = new DateTime(year, i, 16);
                //        if (i < 12)
                //        {
                //            fechafin = (new DateTime(year, i + 1, 1)).AddDays(-1);
                //        }
                //        else
                //        {
                //            fechafin = (new DateTime(year, 12, 31));
                //        }
                //        det = new InventoryPeriodDetail()
                //        {
                //            periodNumber = qnumberD,
                //            dateInit = fechaini,
                //            dateEnd = fechafin,
                //            id_PeriodState = id_estado
                //        };

                //        qnumberD = qnumberD + 1;
                //        resulttemDr.Add(det);
                //    };
                //    result = resulttemDr;

                //    break;

                default:
                    var primerafecha = dfechaini.AddDays(1 - (int)(dfechaini.DayOfWeek));
                    IEnumerable<InventoryPeriodDetail> lfsemanas = Enumerable.Range(0, 54)
                        .Select(i => new {
                            dinit = primerafecha.AddDays(i * 7)
                        }
                    ).TakeWhile(x => x.dinit.Year <= dfechaini.Year)
                    .Select(x => new { x.dinit, dfin = x.dinit.AddDays(6) })
                    .SkipWhile(x => x.dfin < dfechaini.AddDays(1))
                    .Select((x, i) => new InventoryPeriodDetail { periodNumber = i + 1, dateInit = x.dinit, dateEnd = x.dfin, id_PeriodState = id_estado });
                    result = lfsemanas.ToList();
                    break;
            }
       

            return result;
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult InventoryPeriodResults(InventoryPeriod InventoryPeriod,

                                                  int? anio, 


                                                  int? id_InventoryPeriodType

                                                  )
        {
            var model = db.InventoryPeriod.ToList();

            #region  FILTERS

          

            if (id_InventoryPeriodType != null && id_InventoryPeriodType > 0)
            {
                model = model.Where(o => o.id_PeriodType == id_InventoryPeriodType).ToList();
            }



            if (/*InventoryPeriod.id_warehouse != null &&*/ InventoryPeriod.id_warehouse > 0)
            {
                model = model.Where(o => o.id_warehouse == InventoryPeriod.id_warehouse).ToList();
            }

            if (anio != null && anio > 0)
            {
                model = model.Where(o => o.year == anio).ToList();
            }

            model = model.Where(o => o.id_Company == this.ActiveCompanyId && o.id_Division== ActiveDivision.id  && o.id_BranchOffice == ActiveSucursal.id ).ToList();
            #endregion

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_InventoryPeriodResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }


        #endregion

        #region InventoryPeriod HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryPeriodPartial()
        {
            var model = (TempData["model"] as List<InventoryPeriod>);
            model = model ?? new List<InventoryPeriod>();
            TempData.Keep("model");
            return PartialView("_InventoryPeriodPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion

        #region Edit InventoryPeriod
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditInventoryPeriod(int id, int[] orderDetails)
        {
            InventoryPeriod InventoryPeriod = db.InventoryPeriod.Where(o => o.id == id).FirstOrDefault();

            if (InventoryPeriod == null)
            {

                InventoryPeriod = new InventoryPeriod
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
            TempData["InventoryPeriod"] = InventoryPeriod;
            TempData.Keep("InventoryPeriod");

            return PartialView("_FormEditInventoryPeriod", InventoryPeriod);
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
                return PartialView("_InventoryPeriodMainFormPartial", InventoryPeriod);
            }

            TempData.Keep("InventoryPeriod");

            return PartialView("_InventoryPeriodMainFormPartial", new InventoryPeriod());
        }
        #endregion
        #region Validacion

        public Boolean validate(InventoryPeriod item)
        {
            Boolean wsreturn = true;
            try
            {
                InventoryPeriod conInventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);
         

          
              
                 var cantre = db.InventoryPeriod.Where(x =>  x.id_Company == item.id_Company && item.id_Division == x.id_Division && item.id_BranchOffice == x.id_BranchOffice && x.id_PeriodType== item.id_PeriodType && x.year == item.year && x.id_warehouse == item.id_warehouse).ToList().Count();

                if (cantre > 0)
                {
                    TempData.Keep("InventoryPeriod");
                    ViewData["EditMessage"] = ErrorMessage("Periodo ya se encuentra creado para la Bodega indicada ");
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
        public ActionResult InventoryPeriodPartialAddNew(bool approve, InventoryPeriod item)
        {
           
            InventoryPeriod conInventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);
            item.InventoryPeriodDetail = new List<InventoryPeriodDetail>();
            item.dateCreate = DateTime.Now;
            item.dateUpdate = DateTime.Now;
            item.id_Company = ActiveUser.id_company;
            item.id_Division = ActiveDivision.id;
            item.id_BranchOffice = ActiveSucursal.id;
            item.id_userUpdate = ActiveUser.id;
            item.id_userCreate = ActiveUser.id;
            
            if (!validate(item))
            {
                return PartialView("_InventoryPeriodMainFormPartial", item);
            }
             var vestado = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
            if (vestado == null)
            {
                TempData.Keep("InventoryPeriod");
                ViewData["EditMessage"] = ErrorMessage("No esta definido los Estados...");
                return PartialView("_InventoryPeriodMainFormPartial", item);
            }
            var vTPGV1 = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("TPGV1");
            if (vTPGV1 == null)
            {
                TempData.Keep("InventoryPeriod");
                ViewData["EditMessage"] = ErrorMessage("No esta definido el Tipo de Perido...");
                return PartialView("_InventoryPeriodMainFormPartial", item);

            }

            var idestado = (from es in vestado
                            where es.valueCode.ToUpper().Trim() == "P"
                            select es).FirstOrDefault();

            var idTPGV1 = (from es in vTPGV1
                           where es.id == item.id_PeriodType
                           select es).FirstOrDefault();

            List<InventoryPeriodDetail> detail = cargaFecha(item.year, idTPGV1.valueCode, idestado.id);
            DBContext dbemp = new DBContext();
            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {
                    #region InventoryPeriod

                    item.InventoryPeriodDetail = new List<InventoryPeriodDetail>();
                    item.InventoryPeriodDetail = detail;
                    item.dateCreate = DateTime.Now;
                    item.dateUpdate = DateTime.Now;
                    item.id_Company = ActiveUser.id_company;
                    item.id_Division = ActiveDivision.id;
                    item.id_BranchOffice = ActiveSucursal.id;
                    item.id_userUpdate = ActiveUser.id;
                    item.id_userCreate = ActiveUser.id;
                    dbemp.InventoryPeriod.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();


                   TempData["InventoryPeriod"] = item;
                    TempData.Keep("InventoryPeriod");
                    ViewData["EditMessage"] = SuccessMessage("Periodo: " + item.year + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("InventoryPeriod");
                    item = (TempData["InventoryPeriod"] as InventoryPeriod);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            return PartialView("_InventoryPeriodMainFormPartial", item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryPeriodPartialUpdate(bool approve, InventoryPeriod item)
        {
            

            var vestado = DataProviders.DataProviderAdvanceParameters.AdvanceParametersDetailByCode("EPIV1");
            if (vestado == null)
            {
                TempData.Keep("InventoryPeriod");
                ViewData["EditMessage"] = ErrorMessage("No esta definido los Estados...");
                return PartialView("_InventoryPeriodMainFormPartial", item);
            }
          
      
            var idestado = (from es in vestado
                            where es.valueCode.ToUpper().Trim() == "C"
                            select es).FirstOrDefault();


            var idActivo = (from es in vestado
                            where es.valueCode.ToUpper().Trim() == "A"
                            select es).FirstOrDefault();


            DBContext dbemp = new DBContext();

            InventoryPeriod modelItem = dbemp.InventoryPeriod.Where(r => r.id == item.id).FirstOrDefault();
            if (modelItem != null)
            {

                InventoryPeriod conInventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);


            var act=    conInventoryPeriod.InventoryPeriodDetail.Where(x => x.id_PeriodState == idActivo.id).ToList().Count;
                if(act<=0)
                {
                    TempData.Keep("InventoryPeriod");
                    ViewData["EditMessage"] = ErrorMessage("Debe existir al menos un periodo ABIERTO...");
                    return PartialView("_InventoryPeriodMainFormPartial", conInventoryPeriod);
                }


                using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
                {
                    try
                    {
                        #region  DETAILS
                        if (conInventoryPeriod?.InventoryPeriodDetail != null)
                        {
                            var details = conInventoryPeriod.InventoryPeriodDetail.ToList();
                            foreach (var detail in details)
                            {
                                InventoryPeriodDetail inventoryPeriodDetail = modelItem.InventoryPeriodDetail.Where(d => d.id == detail.id).FirstOrDefault();

                                if (inventoryPeriodDetail != null)
                                {
                                    inventoryPeriodDetail.id_PeriodState = detail.id_PeriodState;
                                    inventoryPeriodDetail.dateInit = detail.dateInit;
                                    inventoryPeriodDetail.dateEnd = detail.dateEnd;
                                    //inventoryPeriodDetail.id = detail.id;
                                    //inventoryPeriodDetail.id_InventoryPeriod = detail.id_InventoryPeriod;

                                    if (idestado.id == detail.id_PeriodState)
                                    {
                                        inventoryPeriodDetail.dateClose = DateTime.Now;
                                        inventoryPeriodDetail.isClosed = true;
                                    }
                                    else if (idActivo.id == detail.id_PeriodState)
                                    {
                                        inventoryPeriodDetail.dateClose = null;
                                        inventoryPeriodDetail.isClosed = false;
                                    }
                                    dbemp.InventoryPeriodDetail.Attach(inventoryPeriodDetail);
                                    dbemp.Entry(inventoryPeriodDetail).State = EntityState.Modified;
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
                        modelItem.id_warehouse = item.id_warehouse;
                        dbemp.InventoryPeriod.Attach(modelItem);
                        dbemp.Entry(modelItem).State = EntityState.Modified;
                        dbemp.SaveChanges();
                        trans.Commit();
                        TempData["InventoryPeriod"] = modelItem;
                        TempData.Keep("InventoryPeriod");
                        ViewData["EditMessage"] = SuccessMessage("Periodo: " + item.year + " guardada exitosamente");

                    }
                    catch (Exception e)
                    {
                        TempData.Keep("InventoryPeriod");
                        ViewData["EditMessage"] = ErrorMessage(e.Message);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage();
            }

            TempData.Keep("InventoryPeriod");
            return PartialView("_InventoryPeriodMainFormPartial", modelItem);
        }
        #endregion

        #region InventoryPeriod Gridview

        [ValidateInput(false)]
        public ActionResult InventoryPeriodPartial(int? id)
        {
            if (id != null)
            {
                ViewData["InventoryPeriodToCopy"] = db.InventoryPeriod.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.InventoryPeriod.Where(o => o.id_Company == this.ActiveCompanyId);
            return PartialView("_InventoryPeriodPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryPeriodPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.InventoryPeriod.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.InventoryPeriod.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("InventoryPeriodio : " + (item?.year.ToString() ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.InventoryPeriod.Where(o => o.id_Company == this.ActiveCompanyId);
            return PartialView("_InventoryPeriodPartial", model.ToList());
        }

        public ActionResult DeleteSelectedInventoryPeriod(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var InventoryPeriods = db.InventoryPeriod.Where(i => ids.Contains(i.id));
                        foreach (var vInventoryPeriod in InventoryPeriods)
                        {
                            vInventoryPeriod.isActive = false;

                            vInventoryPeriod.id_userUpdate = ActiveUser.id;
                            vInventoryPeriod.dateUpdate = DateTime.Now;

                            db.InventoryPeriod.Attach(vInventoryPeriod);
                            db.Entry(vInventoryPeriod).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("InventoryPeriodio desactivadas exitosamente");
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

            var model = db.InventoryPeriod.Where(o => o.id_Company == this.ActiveCompanyId);
            return PartialView("_InventoryPeriodPartial", model.ToList());
        }

        #endregion


        #region  Inventory details

        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryPeriodDetailPartial()
        {
            InventoryPeriod inventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);
            inventoryPeriod = inventoryPeriod ?? new InventoryPeriod();
            inventoryPeriod.InventoryPeriodDetail = inventoryPeriod.InventoryPeriodDetail ?? new List<InventoryPeriodDetail>();

            var model = inventoryPeriod.InventoryPeriodDetail;

            TempData.Keep("InventoryPeriod");

            return PartialView("_Details", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult InventoryPeriodDetailPartialUpdate(InventoryPeriodDetail inventoryPeriodDetail)
        {
            InventoryPeriod inventoryPeriod = (TempData["InventoryPeriod"] as InventoryPeriod);

            inventoryPeriod = inventoryPeriod ?? db.InventoryPeriod
                                                            .Where(i => i.id == inventoryPeriod.id).FirstOrDefault();
            inventoryPeriod = inventoryPeriod ?? new InventoryPeriod();
            InventoryPeriodDetail modelItem = null;

            if (ModelState.IsValid)
            {
                try
                {

                    modelItem = inventoryPeriod.InventoryPeriodDetail.Where(it => it.id == inventoryPeriodDetail.id).FirstOrDefault();
                 

                    if (modelItem != null)
                    {
                        string mensaje = "";
                        int   IdSiguiente = 0;
                        int IdSEstadoA = 0;

                        if (!validarDetalleInventoryPeriod(inventoryPeriodDetail, ref mensaje, ref IdSiguiente, ref IdSEstadoA))
                        {
                            ViewData["EditError"] = mensaje;

                            TempData.Keep("InventoryPeriod");
                            var modelre = inventoryPeriod?.InventoryPeriodDetail.ToList() ?? new List<InventoryPeriodDetail>();

                            return PartialView("_Details", modelre.ToList());
                        }
                        modelItem.dateInit = inventoryPeriodDetail.dateInit;
                        modelItem.dateEnd = inventoryPeriodDetail.dateEnd;
                        modelItem.id_PeriodState = inventoryPeriodDetail.id_PeriodState;
                        this.UpdateModel(modelItem);

                        if(IdSiguiente >0 && IdSEstadoA>0)
                        {
                             modelItem = inventoryPeriod.InventoryPeriodDetail.Where(it => it.id == IdSiguiente).FirstOrDefault();
                             modelItem.id_PeriodState = IdSEstadoA;                           
                        }
                        TempData["InventoryPeriod"] = inventoryPeriod;
                    }
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }

                if (modelItem != null)
                {

                    try
                    {
                        var periodState = db.AdvanceParametersDetail.FirstOrDefault(r => r.id == modelItem.id_PeriodState);
                        var tipoPeriodo = db.AdvanceParametersDetail.FirstOrDefault(r => r.id == inventoryPeriod.id_PeriodType);
                        var codigoTipoPeriodo = tipoPeriodo?.valueCode;
                        var descripTipoPeriodo = tipoPeriodo?.description;

                        if (periodState.valueCode == "C")
                        {
                            ServiceInventoryBalance.Execute(db, new MonthlyBalanceProcessMessageDto
                            {
                                anio = inventoryPeriod.year,
                                mes = modelItem.periodNumber,
                                codigoTipoPeriodo = codigoTipoPeriodo,
                                descripcionTipoPeriodo = descripTipoPeriodo,
                                idProcess = "",
                                idUser = this.ActiveUserId,
                                isMassive = false,
                                idWarehouse = inventoryPeriod.id_warehouse,
                                idWarehouseLocation = null,
                                idItem = null,
                                id_company = this.ActiveCompanyId

                            });
                        }
                        else
                        {
                            List<MonthlyBalanceControl> monthlyBalanceControls = new List<MonthlyBalanceControl>();
                            monthlyBalanceControls.Add(new MonthlyBalanceControl
                            {
                                id_company = this.ActiveCompanyId,
                                Anio = inventoryPeriod.year,
                                Mes = modelItem.dateInit.Month,
                                id_warehouse = inventoryPeriod.id_warehouse,
                                DateIsNotValid = DateTime.Now,
                                LastDateProcess = modelItem.dateInit,
                            });
                            var balanceControl = ServiceInventoryBalance.BalanceControlActualizar(this.ActiveCompanyId, monthlyBalanceControls.ToArray());

                            DapperConnection.BulkUpdate(balanceControl);
                        }

                        
                    }
                    catch (Exception e)
                    {

                    }
                }    
                
            }
            else
                ViewData["EditError"] = "Por Favor, corrija todos los errores.";

            TempData.Keep("InventoryPeriod");

            var model = inventoryPeriod?.InventoryPeriodDetail.ToList() ?? new List<InventoryPeriodDetail>();

            return PartialView("_Details", model.ToList());
        }




        #endregion
    }
}
