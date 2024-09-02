using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using System.Data.Entity;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;

namespace DXPANACEASOFT.Controllers
{
    public class CalendarController : DefaultController
    {

        public ActionResult Index()
        {
            return View();
        }

        #region FILTERS RESULTS

        [HttpPost]
        public ActionResult CalendarResults(CalendarPriceList calendarPriceList,
                                           
                                                  DateTime? startDate, DateTime? endDate,
                                     
                                
                                                  int? id_calendarPriceListType
                                      
                                                  )
        {
           var model = db.CalendarPriceList.ToList();

            #region  FILTERS

            if (startDate != null && endDate != null)
            {
                model = model.Where(o => DateTime.Compare(startDate.Value.Date, o.startDate.Date) <= 0 && DateTime.Compare(o.startDate.Date, endDate.Value.Date) <= 0).ToList();
            }

            if (calendarPriceList != null && calendarPriceList.id_calendarPriceListType > 0)
            {
                model = model.Where(o => o.id_calendarPriceListType == calendarPriceList.id_calendarPriceListType).ToList();
            }

            if (id_calendarPriceListType != null && id_calendarPriceListType > 0)
            {
                model = model.Where(o => o.id_calendarPriceListType == id_calendarPriceListType).ToList();
            }

            #endregion

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_CalendarPriceListResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }


        #endregion

        #region Calendar HEADER
        [HttpPost, ValidateInput(false)]
        public ActionResult CalendarPriceListPartial()
        {
            var model = (TempData["model"] as List<CalendarPriceList>);
            model = model ?? new List<CalendarPriceList>();
            TempData.Keep("model");
            return PartialView("_CalendarPriceListPartial", model.OrderByDescending(r => r.id).ToList());
        }
        #endregion

        #region Edit CalendarPriceList
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditCalendarPriceList(int id, int[] orderDetails)
        {
            CalendarPriceList calendarPriceList = db.CalendarPriceList.Where(o => o.id == id) .FirstOrDefault();

            if (calendarPriceList == null)
            {
             
                calendarPriceList = new CalendarPriceList
                {
                    id_company = ActiveUser.id_company,
                    id_userUpdate = ActiveUser.id,
                    dateUpdate = DateTime.Now,
                    startDate = getFirstDateCalendar(),
                    isActive =true,
             
                CalendarPriceListType = new CalendarPriceListType(),
                };
            }
            TempData["calendarPriceList"] = calendarPriceList;
            TempData.Keep("calendarPriceList");

            return PartialView("_FormEditCalendarPriceList", calendarPriceList);
        }
        #endregion

        #region PAGINATION
        [HttpPost, ValidateInput(false)]
        public JsonResult InitializePagination(int id_CalendarPriceList)
        {
            TempData.Keep("CalendarPriceList");
            int index = db.CalendarPriceList.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_CalendarPriceList);
            var result = new
            {
                maximunPages = db.CalendarPriceList.Count(),
                currentPage = index + 1
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Pagination(int page)
        {
            CalendarPriceList CalendarPriceList = db.CalendarPriceList.OrderByDescending(p => p.id).Take(page).ToList().Last();

            if (CalendarPriceList != null)
            {
                TempData["CalendarPriceList"] = CalendarPriceList;
                TempData.Keep("CalendarPriceList");
                return PartialView("_CalendarPriceListMainFormPartial", CalendarPriceList);
            }

            TempData.Keep("CalendarPriceList");

            return PartialView("_CalendarPriceListMainFormPartial", new CalendarPriceList());
        }
        #endregion
        #region Validacion

        public  Boolean validate(CalendarPriceList item, ref bool bAUTO)
        {
            Boolean wsreturn = true;
            try
            {
                CalendarPriceList conCalendarPriceList = (TempData["CalendarPriceList"] as CalendarPriceList);
                Setting setting = DataProviders.DataProviderSettings.Setting("CLAUTO");


           if(   setting != null )
                {
                    bAUTO = (setting.value.Trim() == "AUTO");
               
                }
                else
                {
                    bAUTO = false;
                }

                if (String.IsNullOrEmpty(item.name) && !bAUTO)
                {
                    TempData.Keep("CalendarPriceList");
                    ViewData["EditMessage"] = ErrorMessage("Ingrese el Nombre del Calendario");
                    wsreturn = false;
                }
               

                var cantre = db.CalendarPriceList.Where(x => x.id != item.id 
                                                            //&& x.id_calendarPriceListType == item.id_calendarPriceListType 
                                                            && item.startDate >= x.startDate 
                                                            && item.startDate <= x.endDate 
                                                            && x.isActive
                                                            ).ToList().Count();
                if (cantre > 0)
                {
                    TempData.Keep("CalendarPriceList");
                    ViewData["EditMessage"] = ErrorMessage("Y existe un calendario con el mismo rango de fecha");
                    wsreturn = false;


                }
            }
            catch (Exception)
            {

           
            }

            return wsreturn;

        }
        #endregion

        #region Save and Update
        [HttpPost, ValidateInput(false)]
        public ActionResult CalendarPriceListPartialAddNew(bool approve, CalendarPriceList item)
        {
            CalendarPriceList conCalendarPriceList = (TempData["CalendarPriceList"] as CalendarPriceList);
            bool bauto = false;
            if (!validate(item, ref bauto))
            {
                return PartialView("_CalendarPriceListMainFormPartial", item);
            }
            DBContext dbemp = new DBContext();
           if( bauto)
            {
                item.name = (db.CalendarPriceListType.Where(x => x.id == item.id_calendarPriceListType).FirstOrDefault()?.name ?? "")+ "  " + item.startDate.ToString("dd/MM/yyyy") + " - " + item.endDate.ToString("dd/MM/yyyy");


            }
            using (DbContextTransaction trans = dbemp.Database.BeginTransaction())
            {
                try
                {                    
                    #region CalendarPriceList
                    item.CalendarPriceListType = null;
                    #endregion

                    item.startDate = item.startDate;
                    item.endDate = item.endDate;
                    item.id_company = ActiveCompany.id;
                    item.id_userCreate = ActiveUser.id;
                    item.dateCreate = DateTime.Now;
                    item.id_userUpdate = ActiveUser.id;
                    item.dateUpdate = DateTime.Now;

                    if (approve)
                    {item.isActive = true;}

                    dbemp.CalendarPriceList.Add(item);
                    dbemp.SaveChanges();
                    trans.Commit();

                    item.CalendarPriceListType = dbemp.CalendarPriceListType.Where(x => x.id == item.id_calendarPriceListType).FirstOrDefault();
                    TempData["CalendarPriceList"] = item;
                    TempData.Keep("CalendarPriceList");
                    ViewData["EditMessage"] = SuccessMessage("Calendario: " + item.id + " guardada exitosamente");
                }
                catch (Exception e)
                {
                    TempData.Keep("CalendarPriceList");
                    item = (TempData["CalendarPriceList"] as CalendarPriceList);
                    ViewData["EditMessage"] = ErrorMessage(e.Message);
                    trans.Rollback();
                }
            }
            return PartialView("_CalendarPriceListMainFormPartial", item);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult CalendarPriceListPartialUpdate(bool approve, CalendarPriceList item)
        {
            string messageInactive = "";
            string[] pricesLists =  new string[] { };

            CalendarPriceList modelItem = db.CalendarPriceList.FirstOrDefault(r => r.id == item.id);
            if (modelItem != null)
            {

                CalendarPriceList conCalendarPriceList = (TempData["CalendarPriceList"] as CalendarPriceList);

                bool bauto = false;
                if (!validate(item, ref bauto))
                {
                    return PartialView("_CalendarPriceListMainFormPartial", item);
                }

                if (bauto)
                {
                    item.name =( db.CalendarPriceListType.Where(x => x.id == item.id_calendarPriceListType).FirstOrDefault()?.name ?? "" ) + "  " + item.startDate.ToString("dd/MM/yyyy") + " - " + item.endDate.ToString("dd/MM/yyyy");


                }

                if(!item.isActive )
                {
                    Boolean canInactive =  validateInactive(item.id, ref pricesLists);
                    if(!canInactive)
                    {
                        item.isActive = true;
                        messageInactive = Environment.NewLine + " <br> Calendario No se puede Inactivar, esta siendo utilizado en las Listas de Precios: <br>" + pricesLists.ToList().Aggregate( (i,j) => i+ " <br> " + j);
                    }

                }

                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                       #region CalendarPriceList
                        modelItem.id_company = item.id_company;
                        modelItem.id_userUpdate = ActiveUser.id;
                        modelItem.dateUpdate =DateTime.Now; 
                        modelItem.name = item.name;
                        modelItem.isActive = item.isActive;
                        modelItem.id_calendarPriceListType = item.id_calendarPriceListType;
                        modelItem.startDate = item.startDate;
                        modelItem.endDate = item.endDate;
         
                        modelItem.CalendarPriceListType = db.CalendarPriceListType.Where(x=> x.id == modelItem.id_calendarPriceListType) .FirstOrDefault();
                       #endregion


                        if (approve)
                        { modelItem.isActive = true;
                        }

                        db.CalendarPriceList.Attach(modelItem);
                        db.Entry(modelItem).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                        TempData["CalendarPriceList"] = modelItem;
                        TempData.Keep("CalendarPriceList");
                        ViewData["EditMessage"] = SuccessMessage("Calendario: " + modelItem.id + " guardada exitosamente"+
                                                                    messageInactive                                                                    
                                                                 );
                    }
                    catch (Exception e)
                    {
                        TempData.Keep("CalendarPriceList");
                        ViewData["EditMessage"] = ErrorMessage(e.Message + messageInactive);
                        trans.Rollback();
                    }
                }
            }
            else
            {
                ViewData["EditMessage"] = ErrorMessage(messageInactive);
            }

            TempData.Keep("CalendarPriceList");
            return PartialView("_CalendarPriceListMainFormPartial", modelItem);
        }
        #endregion

        #region Calendar Gridview

        [ValidateInput(false)]
        public ActionResult CalendarPartial(int? id)
        {
            if (id != null)
            {
                ViewData["CalendarToCopy"] = db.CalendarPriceList.Where(b => b.id == id).FirstOrDefault();
            }
            var model = db.CalendarPriceList.Where(o => o.id_company == ActiveCompany.id);
            return PartialView("_CalendarPriceListPartial", model.ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CalendarPriceListPartialDelete(System.Int32 id)
        {
            if (id >= 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var item = db.CalendarPriceList.FirstOrDefault(it => it.id == id);
                        if (item != null)
                        {
                            item.isActive = false;
                            item.id_userUpdate = ActiveUser.id;
                            item.dateUpdate = DateTime.Now;

                        }
                        db.CalendarPriceList.Attach(item);
                        db.Entry(item).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Calendario : " + (item?.name ?? "") + " desactivada exitosamente");
                    }
                    catch (Exception)
                    {
                        ViewData["EditMessage"] = ErrorMessage();
                        trans.Rollback();
                    }
                }

            }

            var model = db.CalendarPriceListType.Where(o => o.id_company == ActiveCompany.id);
            return PartialView("_CalendarPriceListPartial", model.ToList());
        }

        public ActionResult DeleteSelectedCalendarPriceList(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var CalendarPriceLists = db.CalendarPriceList.Where(i => ids.Contains(i.id));
                        foreach (var vCalendarPriceList in CalendarPriceLists)
                        {
                            vCalendarPriceList.isActive = false;

                            vCalendarPriceList.id_userUpdate = ActiveUser.id;
                            vCalendarPriceList.dateUpdate = DateTime.Now;

                            db.CalendarPriceList.Attach(vCalendarPriceList);
                            db.Entry(vCalendarPriceList).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        trans.Commit();
                        ViewData["EditMessage"] = SuccessMessage("Calendario desactivadas exitosamente");
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

            var model = db.CalendarPriceListType.Where(o => o.id_company == ActiveCompany.id);
            return PartialView("_CalendarPriceListPartial", model.ToList());
        }

        #endregion

        #region Metodos Auxiliares
        private Boolean validateInactive(int idCalendarPriceList, ref string[] arPriceList)
        {
            Boolean isValid = false;
            try
            {

                 arPriceList =
                        db.PriceList
                          .Where(r => r.id_calendarPriceList == idCalendarPriceList)
                          .Select(r=> r.name)
                          .ToArray();

                isValid = (arPriceList.Count()== 0);

            }
            catch(Exception e)
            {

                throw e;
            }


            return isValid;


        }
        private DateTime getFirstDateCalendar()
        {
            DateTime FirstDateCalendar = DateTime.Now;

            try
            {

                DateTime lastDateCalendar = db.CalendarPriceList.Max(r => r.endDate);
                FirstDateCalendar = lastDateCalendar.AddDays(1);

            }
            catch(Exception e)
            {
                throw e;
            }


            return FirstDateCalendar;


        }

        [HttpPost, ValidateInput(false)]
        public JsonResult PrintRemissionGuideReportList(RGRFilterWindow rgrfw)
        {
            CalendarPriceList calendarPriceList = (TempData["CalendarPriceList"] as CalendarPriceList);
            calendarPriceList = calendarPriceList ?? new CalendarPriceList();
            TempData["CalendarPriceList"] = calendarPriceList;
            TempData.Keep("CalendarPriceList");

            #region Armo Parametros
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@str_FEmisionDateStart";
            _param.Valor = rgrfw.str_emissionDateStart;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@str_FEmisionDateEnd";
            _param.Valor = rgrfw.str_emissionDateEnd;
            paramLst.Add(_param);

            #endregion

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = rgrfw.codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
