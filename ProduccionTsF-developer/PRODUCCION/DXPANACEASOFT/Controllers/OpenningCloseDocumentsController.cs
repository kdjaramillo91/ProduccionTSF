using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models.OpeningCloseDocumentsDTO;
using DXPANACEASOFT.Models;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.SQL;
using EntidadesAuxiliares.General;
using AccesoDatos.MSSQL;
using System.Configuration;

namespace DXPANACEASOFT.Controllers
{
    public class OpenningCloseDocumentsController : DefaultController
    {
        //GET: OpenningCloseDocuments
        public ActionResult Index()
        {
            return View();
        }

        #region OPENING CLOSE DOCUMENTS FILTERS RESULTS

        [HttpPost]
        public ActionResult OpenningCloseDocumentsResults(OpeningCloseDocumentsFilter opCloseFilter)
        {
            var model = GetDocumentsByTypeAll(opCloseFilter);
            
            TempData["model"] = model;
            TempData.Keep("model");

            return PartialView("_OpeningCloseDocumentResultsPartial", model.ToList());
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult OpenningCloseDocumentsPartial(int? id_docRq)
        {
            List<OpeningCloseDocumentResults> model = (TempData["model"] as List<OpeningCloseDocumentResults>);
            model = model ?? new List<OpeningCloseDocumentResults>();

            if (id_docRq > 0)
            {
                Document _do = db.Document.FirstOrDefault(fod => fod.id == id_docRq);
                _do = _do ?? new Document();
                OpeningCloseDocumentResults _ocdrTmp = model.FirstOrDefault(fod => fod.id == id_docRq);
                _ocdrTmp = _ocdrTmp ?? new OpeningCloseDocumentResults();
                _ocdrTmp.isOpen = _do.isOpen ?? false;
            }

            TempData.Keep("model");

            return PartialView("_OpeningCloseDocumentPartial", model.ToList());
        }
        
        #endregion

        #region METODOS PRIVADOS

        private IEnumerable<OpeningCloseDocumentResults> GetDocumentsByTypeAll(OpeningCloseDocumentsFilter opCloseFilter)
        {
            List<OpeningCloseDocumentResults> lstLPProvider = new List<OpeningCloseDocumentResults>();

            List<ParamSQL> lstParametersSql = new List<ParamSQL>();
            ParamSQL _param = new ParamSQL();
            _param.Nombre = "@id_documentType";
            _param.TipoDato = DbType.Int32;
            _param.Valor = opCloseFilter?.id_DocumentType ?? 0;
            lstParametersSql.Add(_param);

            _param = new ParamSQL();
            _param.Nombre = "@str_emissionDateStart";
            _param.TipoDato = DbType.String;
            _param.Valor = (opCloseFilter != null && opCloseFilter.emissionDateStart != null) ? opCloseFilter.emissionDateStart.Value.Date.ToString("yyyy/MM/dd") : "";
            lstParametersSql.Add(_param);

            _param = new ParamSQL();
            _param.Nombre = "@str_emissionDateEnd";
            _param.TipoDato = DbType.String;
            _param.Valor = (opCloseFilter != null && opCloseFilter.emissionDateEnd != null) ? opCloseFilter.emissionDateEnd.Value.Date.ToString("yyyy/MM/dd") : "";
            lstParametersSql.Add(_param);

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];
            DataSet ds = MetodosDatos2.ObtieneDatos(_cadenaConexion
                                                , "pac_Documentos"
                                                , _rutaLog
                                                , "OpeningCloseDocument"
                                                , "PROD"
                                                , lstParametersSql);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                lstLPProvider = dt.AsEnumerable().Select(s => new OpeningCloseDocumentResults()
                {
                    id = s.Field<Int32>("id"),
                    numberDoc = s.Field<String>("numeroDocumento"),
                    emissionDate = s.Field<DateTime>("FechaEmision"),
					codeDocumentType = s.Field<String>("codeTipoDocumento"),
					nameDocumentType = s.Field<String>("NombreTipoDocumento"),
                    nameProvider = s.Field<String>("NombreProveedor"),
                    nameDocumentState = s.Field<String>("NombreEstado"),
                    isOpen = s.Field<bool>("bEstado")
                }).ToList();
            }



            return lstLPProvider;
        }


        #endregion

        #region OPENING CLOSE DOCUMENT EDIT FORM 
        [HttpPost, ValidateInput(false)]
        public ActionResult FormEditDocumentOpenClose(int? id)
        {
            return null;
        }
        
        #endregion
    }
}
