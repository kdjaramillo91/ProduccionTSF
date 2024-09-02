using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.LiquidationPayProviderDTO;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.SQL;
using EntidadesAuxiliares.General;
using AccesoDatos.MSSQL;
using System.Configuration;

namespace DXPANACEASOFT.Controllers
{
    public class LiquidationPayProviderQueryController : DefaultController
    {
        // GET: LiquidationPayProviderQuery
        public ActionResult Index()
        {
            return View();
        }


        #region REMISSION GUIDE FILTERS RESULTS

        [HttpPost]
        public ActionResult LiquidationPayProviderQueryResults(LiquidationPayProviderFilter liqPayFilter)
        {
            var model = GetLiquidationPayProviderAll(liqPayFilter);

            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_LiquidationPayProviderQueryResultsPartial", model.OrderByDescending(r => r.id).ToList());
        }

        #endregion

        [HttpPost, ValidateInput(false)]
        public ActionResult LiquidationPayProviderQueryPartial()
        {
            var model = (TempData["model"] as List<LiquidationPayProviderResults>);
            model = model ?? new List<LiquidationPayProviderResults>();

            TempData.Keep("model");

            return PartialView("_LiquidationPayProviderQueryPartial", model.OrderByDescending(r => r.id).ToList());
        }

        [HttpPost]
        public JsonResult PrintLiquidationPayProviderQuery(LiquidationPayProviderFilter liqPayFilter)
        {
            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();
            _param.Nombre = "@id_provider";
            _param.Valor = liqPayFilter?.id_provider ?? 0;
            paramLst.Add(_param);

            string str_starLiquidationDate = "";
            if (liqPayFilter.liquidationDateStart != null) { str_starLiquidationDate = liqPayFilter.liquidationDateStart.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR();
            _param.Nombre = "@str_liquidationDateStart";
            _param.Valor = str_starLiquidationDate;
            paramLst.Add(_param);

            string str_endLiquidationDate = "";
            if (liqPayFilter.liquidationDateStart != null) { str_endLiquidationDate = liqPayFilter.liquidationDateStart.Value.Date.ToString("yyyy/MM/dd"); }
            _param = new ParamCR();
            _param.Nombre = "@str_liquidationDateEnd";
            _param.Valor = str_endLiquidationDate;
            paramLst.Add(_param);


            paramLst.Add(_param);

            Conexion objConex = GetObjectConnection("DBContextNE");
            ReportParanNameModel rep = new ReportParanNameModel();

            ReportProdModel _repMod = new ReportProdModel();
            _repMod.codeReport = liqPayFilter.codeReport;
            _repMod.conex = objConex;
            _repMod.paramCRList = paramLst;

            rep = GetTmpDataName(20);

            TempData[rep.nameQS] = _repMod;
            TempData.Keep(rep.nameQS);

            var result = rep;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<LiquidationPayProviderResults> GetLiquidationPayProviderAll(LiquidationPayProviderFilter liqPayPro)
        {
            List<LiquidationPayProviderResults> lstLPProvider = new List<LiquidationPayProviderResults>();
            List<ParamSQL> lstParametersSql = new List<ParamSQL>();
            ParamSQL _param = new ParamSQL();
            _param.Nombre = "@id_provider";
            _param.TipoDato = DbType.Int32;
            _param.Valor = liqPayPro?.id_provider ?? 0;
            lstParametersSql.Add(_param);

            _param = new ParamSQL();
            _param.Nombre = "@str_liquidationDateStart";
            _param.TipoDato = DbType.String;
            _param.Valor = (liqPayPro != null && liqPayPro.liquidationDateStart != null ) ? liqPayPro.liquidationDateStart.Value.Date.ToString("yyyy/MM/dd"):"";
            lstParametersSql.Add(_param);

            _param = new ParamSQL();
            _param.Nombre = "@str_liquidationDateEnd";
            _param.TipoDato = DbType.String;
            _param.Valor = (liqPayPro != null && liqPayPro.liquidationDateEnd != null) ? liqPayPro.liquidationDateEnd.Value.Date.ToString("yyyy/MM/dd") : "";
            lstParametersSql.Add(_param);

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];
            //int id;
            DataSet ds = MetodosDatos2.ObtieneDatos(_cadenaConexion
                                                , "parc_Liquidacion_Compra_Proveedores_Lista"
                                                , _rutaLog
                                                , "LiquidationPayProvider"
                                                , "PROD"
                                                , lstParametersSql);

            if (ds != null && ds.Tables.Count > 0 )
            {
                DataTable dt = ds.Tables[0];
                //foreach (DataRow dr in dt.Rows)
                //{
                //    id = (Int32)dr["Id"];
                //}
                lstLPProvider = dt.AsEnumerable().Select(s => new LiquidationPayProviderResults()
                {
                    id = s.Field<Int32>("Id"),
                    id_productionLot = s.Field<Int32>("IdLoteProduccion"),
                    nameProvider = s.Field<String>("NombreProveedor"),
                    liquidationDate = s.Field<DateTime>("FechaLiquidacion"),
                    numberOoc = s.Field<String>("SecuenciaTransaccional"),
                    internalNumberDoc = s.Field<String>("NumeroLoteInterno"),
                    poundsQuantityReceived = s.Field<Decimal>("LibrasRecibidas"),
                    valueToPay = s.Field<Decimal>("TotalPagar"),
                    processType = s.Field<String>("NombreProceso"),
                    nameState = s.Field<String>("EstadoDocumento"),
					personProcessPlant = s.Field<String>("personProcesPlant")
				}).ToList();
            }

         

            return lstLPProvider;
        }
    }
}
