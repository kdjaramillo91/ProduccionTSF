using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models.DocumentOpenCloseDTO;
using DXPANACEASOFT.Models.DocumentLogDTO;
using DXPANACEASOFT.Models;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.SQL;
using EntidadesAuxiliares.General;
using AccesoDatos.MSSQL;
using System.Configuration;
using System.Data.Entity;
using Utilitarios.Logs;

namespace DXPANACEASOFT.Controllers
{
    public class DocumentOpenCloseController : DefaultController
    {
        //GET: DocumentOpenCloseController
        public ActionResult Index()
        {
            return View();
        }

        [ActionName("PopupControlDocumentOpenCloseId")]
        [HttpPost]
        public ActionResult PopupControlDocumentOpenClose(int id_doc)
        {
            try
            {
                Document _docTmp = db.Document.FirstOrDefault(fod => fod.id == id_doc);

                DocumentOpenCloseForm _docOpenCloseFormTmp = GetDocumentInformation(_docTmp);
                
                return PartialView("_FormEditPopupControlDocumentOpenClosePartial", _docOpenCloseFormTmp);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public ActionResult PopupControlDocumentOpenClose(DocumentOpenCloseForm docOpenClose)
        {
            if (ModelState.IsValid )
            {
                UpdateDocument(docOpenClose);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        private DocumentOpenCloseForm GetDocumentInformation(Document _doc)
        {
            DocumentOpenCloseForm _docOpenCloseFormTmp = new DocumentOpenCloseForm();

            _docOpenCloseFormTmp.id_doc = _doc.id;
            _docOpenCloseFormTmp.numberDoc = _doc.number;
			_docOpenCloseFormTmp.codeDocumentType = _doc.DocumentType.code;
			_docOpenCloseFormTmp.stateDoc = _doc.DocumentState.name;
			_docOpenCloseFormTmp.emissionDate = _doc.emissionDate;
            _docOpenCloseFormTmp.isOpen = _doc.isOpen ?? false;

            if (_doc.DocumentType.code == "02")
            {
                PurchaseOrder _po = db.PurchaseOrder.FirstOrDefault(fod => fod.id == _doc.id);
                _po = _po ?? new PurchaseOrder();

                _docOpenCloseFormTmp.nameProvider = db.Person
                                                        .FirstOrDefault(fod => fod.id == _po.id_provider)?
                                                        .fullname_businessName ?? "";

                _docOpenCloseFormTmp.nameProductionUnitProvider = db.ProductionUnitProvider
                                                                    .FirstOrDefault(fod => fod.id == _po.id_productionUnitProvider)?
                                                                    .name ?? "";
            }
            else if (_doc.DocumentType.code == "08")
            {
                RemissionGuide _rg = db.RemissionGuide.FirstOrDefault(fod => fod.id == _doc.id);
                _rg = _rg ?? new RemissionGuide();

                _docOpenCloseFormTmp.nameProvider = db.Person
                                                        .FirstOrDefault(fod => fod.id == _rg.id_providerRemisionGuide)?
                                                        .fullname_businessName ?? "";

                _docOpenCloseFormTmp.nameProductionUnitProvider = db.ProductionUnitProvider
                                                                    .FirstOrDefault(fod => fod.id == _rg.id_productionUnitProvider)?
                                                                    .name ?? "";
            }

            return _docOpenCloseFormTmp;

        }

        private int UpdateDocument(DocumentOpenCloseForm docOpenClose)
        {
            int _answer = 0;
            string ruta = ConfigurationManager.AppSettings["rutaLog"];
            bool _isOp = db.Document.FirstOrDefault(fod => fod.id == docOpenClose.id_doc)?.isOpen ?? false;
            string codeAction = string.Empty;
            DocumentLogDTO _doTmp = new DocumentLogDTO();

            if (_isOp != docOpenClose.isOpen)
            {
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        Document _do = db.Document.FirstOrDefault(fod => fod.id == docOpenClose.id_doc);
                        _do.isOpen = docOpenClose.isOpen;

                        if (docOpenClose.isOpen)
                            codeAction = "ABD";
                        else
                            codeAction = "CRD";

                        
                        _doTmp.id = docOpenClose.id_doc;
                        _doTmp.code_Action = codeAction;
                        _doTmp.id_User = ActiveUser.id;
                        _doTmp.description = docOpenClose.commentOnAction;

                        //Services.ServiceDocument.GenerateDocumentLog(db, _doTmp);

                        db.Document.Attach(_do);
                        db.Entry(_do).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                        _answer = 1;
                    }
                    catch (Exception ex)
                    {
                        MetodosEscrituraLogs.EscribeMensajeLog(ex.Message, ruta, "Apertura Documentos", "PROD");
                        trans.Rollback();
                    }
                }
                if (_answer == 1)
                {
                    Services.ServiceDocument.GenerateDocumentLog(db, _doTmp, ruta);
                }
            }

            return _answer;
        }
    }
}
