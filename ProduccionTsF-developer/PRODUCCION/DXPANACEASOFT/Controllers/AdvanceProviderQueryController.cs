using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;

namespace DXPANACEASOFT.Controllers
{
    public class AdvanceProviderQueryController : DefaultController
    {
        // GET: AdvanceProviderQuery
        public ActionResult Index()
        {
            return View();
        }


        #region REMISSION GUIDE FILTERS RESULTS

        [HttpPost]
        public ActionResult AdvanceProviderQueryResults(AdvanceProvider advanceProvider,
                                                  Document document,
                                                  DateTime? startEmissionDate, DateTime? endEmissionDate
                                                  )
        {
            var model = db.AdvanceProvider.ToList();

            #region AdvanceProvider FILTERS

            if (startEmissionDate != null && endEmissionDate != null)
            {
                model = model.Where(o => DateTime.Compare(startEmissionDate.Value, o.Document.emissionDate) <= 0 && DateTime.Compare(o.Document.emissionDate, endEmissionDate.Value) <= 0).ToList();
            }


            if (advanceProvider.id_provider !=null && advanceProvider.id_provider>0)
            {
                model = model.Where(o => o.id_provider == advanceProvider.id_provider).ToList();
            }

            if (document.id_documentState > 0)
            {
                model = model.Where(o => o.Document.id_documentState == document.id_documentState).ToList();
            }

            #endregion
            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_AdvanceProviderQueryResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }

        #endregion

        [HttpPost, ValidateInput(false)]
        public ActionResult AdvanceProviderQueryPartial()
        {
            var model = (TempData["model"] as List<AdvanceProvider>);
            model = model ?? new List<AdvanceProvider>();

            TempData.Keep("model");

            return PartialView("_AdvanceProviderQueryPartial", model.OrderByDescending(r => r.id).ToList());
        }

        public JsonResult PrintAdvanceProviderQuery(string codeReport, AdvanceProvider advanceProvider,
                                                  Document document,
                                                  DateTime? startEmissionDate, DateTime? endEmissionDate)
        {
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id_provider";
            _param.Valor = advanceProvider?.id_provider ?? 0;
            paramLst.Add(_param);

            string str_startEmissionDate = "";
            if (startEmissionDate != null) { str_startEmissionDate = startEmissionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR();
            _param.Nombre = "@dt_start";
            _param.Valor = str_startEmissionDate;
            paramLst.Add(_param);

            string str_startEmissionTime = "";
            if (startEmissionDate != null) { str_startEmissionTime = startEmissionDate.Value.ToString("HH:mm"); }
            _param = new ParamCR();
            _param.Nombre = "@dt_startTime";
            _param.Valor = str_startEmissionTime;
            paramLst.Add(_param);

            string str_endEmissionDate = "";
            if (endEmissionDate != null) { str_endEmissionDate = endEmissionDate.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR();
            _param.Nombre = "@dt_end";
            _param.Valor = str_endEmissionDate;
            paramLst.Add(_param);

            string str_endEmissionTime = "";
            if (endEmissionDate != null) { str_endEmissionTime = endEmissionDate.Value.ToString("HH:mm"); }
            _param = new ParamCR();
            _param.Nombre = "@dt_endTime";
            _param.Valor = str_endEmissionTime;
            paramLst.Add(_param);

            _param = new ParamCR();
            _param.Nombre = "@id_state";
            _param.Valor = document?.id_documentState ?? 0;
            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
