using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Auxiliares;
using DXPANACEASOFT.Services;
using DXPANACEASOFT.Auxiliares.IntegrationProcessService;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using DXPANACEASOFT.Models.IntegrationProcessDetailDTO;

namespace DXPANACEASOFT.Controllers
{
    public class IntegrationProcessController : DefaultController
    {

        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        // CallBack Grid Integration Controller
        #region CallBackGrid
        [HttpPost, ValidateInput(false)]
        public ActionResult CallBackIntegrationProcess()
        {
            var model = (TempData["integrationProcessLotes"] as List<IntegrationProcess>);
            model = model ?? new List<IntegrationProcess>();

            TempData.Keep("integrationProcessLotes");
            return PartialView("_IntegrationProcessResultsPartial", model);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult CallBackDocumentProcessDocument()
        {
            var model = (TempData["integrationProcess"] as IntegrationProcess);
            var modelDet = model?.IntegrationProcessDetail ?? new List<IntegrationProcessDetail>();
            IntegrationState statusDeleted = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Deleted);
            if (modelDet.Count > 0)
            {
                modelDet = modelDet.Where(r => r.id_StatusDocument != statusDeleted.id).ToList();
            }


            TempData.Keep("integrationProcess");
            return PartialView("_IntegrationProcessDetailEditPartial", IntegrationProcessDetailDTO.GetListIntegrationProcessDetailDTO(modelDet.ToList()));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CallBackDocumentProcessDocumentFmi()
        {
            var model = (TempData["integrationProcess"] as IntegrationProcess);
            var modelDet = model?.IntegrationProcessDetail ?? new List<IntegrationProcessDetail>();
            IntegrationState statusDeleted = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Deleted);
            if (modelDet.Count > 0)
            {
                modelDet = modelDet.Where(r => r.id_StatusDocument != statusDeleted.id).ToList();
            }


            TempData.Keep("integrationProcess");
            return PartialView("_IntegrationProcessDetailEditLMIPartial", IntegrationProcessDetailFmiDTO.GetListIntegrationProcessDetailDTO(modelDet.ToList()));
        }

        // CallBackIntegrationProcessLogView
        public ActionResult CallBackIntegrationProcessLogView()
        {

            var model = (TempData["integrationProcess"] as IntegrationProcess);


            TempData.Keep("integrationProcess");
            return PartialView("_IntegrationProcessLogFormPartial", model.IntegrationProcessLogViewList);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CallBackDocument()
        {
            var model = (TempData["documentProcessList"] as List<IntegrationProcessDetail>);
            model = model ?? new List<IntegrationProcessDetail>();

            TempData.Keep("documentProcessList");
            return PartialView("IntegrationProcessFindDocuments/_IntegrationProcessDocumentResultsPartialGrid", model.OrderByDescending(o => o.id).ToList());
        }

        #endregion

        #region Integration Process Lote Find

        [HttpPost, ValidateInput(false)]
        [ActionName("findlotes")]
        public PartialViewResult LotesResultForm
                                (
                                    int? id_DocumentType,
                                    DateTime? dateInitCreate,
                                    DateTime? dateEndCreate,
                                    DateTime? dateInitExec,
                                    DateTime? dateEndExec,
                                    DateTime? dateInitFullIntegration,
                                    DateTime? dateEndFullIntegration,
                                    int? id_IntegrationProcessState,
                                    int? id_Lote,
                                    string description
                                )
        {

            List<IntegrationProcess> integrationProcessLotes = new List<IntegrationProcess>();
            try
            {
                integrationProcessLotes
                                    = Services
                                        .ServiceIntegrationProcess
                                        .FindLotes
                                        (
                                            db,
                                            ActiveUser.id,
                                            this.ActiveCompanyId,
                                            id_DocumentType,
                                            dateInitCreate,
                                            dateEndCreate,
                                            dateInitExec,
                                            dateEndExec,
                                            dateInitFullIntegration,
                                            dateEndFullIntegration,
                                            id_IntegrationProcessState,
                                            id_Lote,
                                            description
                                        );

                TempData["integrationProcessLotes"] = integrationProcessLotes;

            }
            catch (Exception e)
            {
                ViewData["EditError"] = ErrorMessage("Error al realizar búsqueda de Lotes.");
                LogWrite(e, null, "LotesResultForm");
            }
            finally
            {

                TempData.Keep("integrationProcessLotes");
            }



            return PartialView("_IntegrationProcessResultsMainPartial", integrationProcessLotes.OrderByDescending(r => r.dateCreate));
        }



        [HttpPost, ValidateInput(false)]
        [ActionName("getlote")]
        public PartialViewResult LotePartialForm
                                (
                                    int id_IntegrationProcess
                                )
        {
            IntegrationProcess integrationProcess = (id_IntegrationProcess == 0) ? new IntegrationProcess() : db.IntegrationProcess.FirstOrDefault(r => r.id == id_IntegrationProcess);
            string msgXtraInfo = "";

            try
            {
                int id_statusEliminar = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Deleted)?.id ?? 0;

                msgXtraInfo = "Services.ServiceIntegrationProcess.GetXGlosaForView";
                Services
                    .ServiceIntegrationProcess
                    .GetXGlosaForView(ref integrationProcess, id_statusEliminar, 4);

                msgXtraInfo = "Services.ServiceIntegrationProcess.GetLogView";
                Services
                    .ServiceIntegrationProcess
                    .GetLogView(ref integrationProcess);

                msgXtraInfo = "Services.ServiceIntegrationProcess.GetRequeridDate";
                integrationProcess.isRequeridDate = Services
                                                        .ServiceIntegrationProcess
                                                        .GetRequeridDate(db, integrationProcess.id_DocumentType);


                TempData["integrationProcess"] = integrationProcess;
            }
            catch (Exception e)
            {
                LogWrite(e, null, "LotePartialForm==>" + msgXtraInfo);
            }
            finally
            {

                TempData.Keep("integrationProcess");
            }


            return PartialView("_IntegrationProcessMainFormPartial", integrationProcess);
        }


        #endregion


        #region Gestion Integration Process

        [HttpPost, ValidateInput(false)]
        [ActionName("addlote")]
        public PartialViewResult LotesPartialAddNew
                                 (
                                    int id_DocumentType,
                                    string description,
                                    DateTime? dateAccounting
                                 )
        {

            IntegrationProcess integrationProcessLote = new IntegrationProcess();
            tbsysIntegrationDocumentConfig integrationConfig = new tbsysIntegrationDocumentConfig();
            string codeLote = "";
            string codeLabelTypoDocumento = "";
            int secuencialLoteCode = 0;
            //string msgGeneral = "";
            string msgXtraInfo = "";
            try
            {

                DocumentType documentType = db.DocumentType.FirstOrDefault(r => r.id == id_DocumentType);
                if (documentType == null)
                {
                    msgXtraInfo = "Obtención Tipo de Documento";
                    throw new Exception("Configuracion de tipo de Documento para el tipo de documento seleccionado, no definido.");
                }

                integrationConfig = db.tbsysIntegrationDocumentConfig
                                            .FirstOrDefault(r => r.codeDocumentType == documentType.code && r.isActive);

                if (integrationConfig == null)
                {
                    msgXtraInfo = "Obtención Configuración de Integracion";
                    throw new Exception("Configuracion de Integracioón para el tipo de documento seleccionado, no definido.");
                }

                codeLabelTypoDocumento = integrationConfig.code ?? "";

                int id_StatusCreate = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Create)?.id ?? 0;
                if (id_StatusCreate == 0)
                {
                    msgXtraInfo = "Obtención Tipo Status";
                    throw new Exception("Tipo de Status Lote no Definido.");
                }

                // TODO : OPTIMIZAR
                msgXtraInfo = "Secencial Número de Lote";
                int secuencia = integrationConfig.secuence;
                secuencialLoteCode = (secuencia == 0) ? 1 : (secuencia + 1);
                codeLote = codeLabelTypoDocumento + DateTime.Now.Year.ToString() + DateTime.Now.ToString("MM") + secuencialLoteCode.ToString();


                #region IntegrationProcess
                integrationProcessLote.id_DocumentType = id_DocumentType;
                integrationProcessLote.description = description;
                integrationProcessLote.codeLote = codeLote;
                integrationProcessLote.dateAccounting = (DateTime)dateAccounting;
                integrationProcessLote.dateCreate = DateTime.Now;
                integrationProcessLote.dateUpdate = DateTime.Now;
                integrationProcessLote.id_userCreate = ActiveUser.id;
                integrationProcessLote.id_userUpdate = ActiveUser.id;
                integrationProcessLote.id_StatusLote = id_StatusCreate;
                integrationProcessLote.id_company = this.ActiveCompanyId;
                #endregion


                #region Log
                msgXtraInfo = "Services.ServiceIntegrationProcess.SaveLog";
                Services.ServiceIntegrationProcess.SaveLog
                                                    (
                                                        db,
                                                        ActiveUser.id,
                                                        ref integrationProcessLote,
                                                        id_StatusCreate
                                                    );

                #endregion

                TempData["integrationProcess"] = integrationProcessLote;


                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        msgXtraInfo = "SaveChanges";


                        integrationConfig.secuence = secuencialLoteCode;

                        db.tbsysIntegrationDocumentConfig.Attach(integrationConfig);
                        db.Entry(integrationConfig).State = EntityState.Modified;
                        db.SaveChanges();

                        db.IntegrationProcess.Add(integrationProcessLote);
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        trans.Dispose();
                        throw e;
                    }

                }

                msgXtraInfo = "Services.ServiceIntegrationProcess.GetLogView";
                Services
                    .ServiceIntegrationProcess
                    .GetLogView(ref integrationProcessLote);

                msgXtraInfo = "Services.ServiceIntegrationProcess.GetRequeridDate";
                integrationProcessLote.isRequeridDate = Services
                                                        .ServiceIntegrationProcess
                                                        .GetRequeridDate(db, integrationProcessLote.id_DocumentType);

                ViewData["EditMessage"] = SuccessMessage("Lote de Integración de Datos creado exitosamente.");


            }
            catch (Exception e)
            {

                ViewData["EditError"] = ErrorMessage(e.Message);
                LogWrite(e, null, "LotesPartialAddNew==>" + msgXtraInfo);
            }
            finally
            {
                TempData.Keep("integrationProcess");
            }

            return PartialView("_IntegrationProcessWrapperPartial", integrationProcessLote);

        }


        [HttpPost, ValidateInput(false)]
        [ActionName("updatelote")]
        public PartialViewResult LotesPartialUpdate
                                (
                                   int id_IntegrationProcess,
                                   string description,
                                   int[] id_integrationProcessDetailList
                                )
        {


            IntegrationProcess integrationProcessLote = new IntegrationProcess();
            //string msgGeneral = "";
            string msgXtraInfo = "";
            //Boolean hasChange = false;

            try
            {


                integrationProcessLote = db.IntegrationProcess.FirstOrDefault(r => r.id == id_IntegrationProcess);

                int id_StatusCreate = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Create)?.id ?? 0;

                if (id_StatusCreate == 0)
                {
                    msgXtraInfo = "Obtención Status Creado Lote";
                    throw new Exception("Tipo de Status Lote no Definido.");
                }



                #region IntegrationProcessDetail
                // No existen documentos
                if (integrationProcessLote.IntegrationProcessDetail == null)
                {
                    integrationProcessLote.IntegrationProcessDetail = new List<IntegrationProcessDetail>();
                }

                int id_statusEliminar = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Deleted)?.id ?? 0;
                int id_statusAgregar = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Added)?.id ?? 0;

                if (id_statusEliminar == 0 || id_statusAgregar == 0)
                {

                    msgXtraInfo = "Obtención Status Lote Eliminar /Agregar ";
                    throw new Exception("Tipo de Status Lote no Definido.");
                }

                IIntegrationProcessActionOutput integrationImplementation = ServiceIntegrationProcess
                                                                            .IntegrationProcessDefinition(db, integrationProcessLote.id_DocumentType);

                // Documentos que deben estar incluidos
                if (id_integrationProcessDetailList != null)
                {

                    id_integrationProcessDetailList.ToList().ForEach(r =>
                    {

                        Document _document = db.Document.FirstOrDefault(r2 => r2.id == r);

                        IntegrationProcessDetail _integrationProcessDetail =
                                                       integrationProcessLote
                                                            .IntegrationProcessDetail
                                                            .FirstOrDefault(r2 => r2.id_Document == r);

                        msgXtraInfo = "new IntegrationProcessDetail:" + r.ToString();
                        if (_integrationProcessDetail == null)
                        {
                            //hasChange = true;
                            _integrationProcessDetail
                                    = new IntegrationProcessDetail
                                    {
                                        id_Document = r,
                                        id_UserCreate = ActiveUser.id,
                                        dateCreate = DateTime.Now,
                                        Document = _document,
                                        dateLastUpdateDocument = _document.dateUpdate

                                    };

                            integrationProcessLote
                                 .IntegrationProcessDetail
                                 .Add(_integrationProcessDetail);
                        };

                        _integrationProcessDetail.id_UserUpdate = ActiveUser.id;
                        _integrationProcessDetail.dateUpdate = DateTime.Now;

                        int id_documentType = integrationProcessLote.id_DocumentType;
                        string code_documentType = integrationProcessLote.DocumentType.code;


                        msgXtraInfo = "Services.ServiceIntegrationProcess.GetTotalValue";
                        _integrationProcessDetail.totalValueDocument = Services
                                                                            .ServiceIntegrationProcess
                                                                            .GetTotalValue(_document, integrationImplementation.GetTotalValue);




                        msgXtraInfo = "Services.ServiceIntegrationProcess.GetGloss";
                        _integrationProcessDetail.glossData = Services
                                                                .ServiceIntegrationProcess
                                                                .GetXGlosaForSave(db, _document.id, code_documentType, integrationImplementation.GetGlossX);

                        _integrationProcessDetail.id_StatusDocument = id_statusAgregar;

                        msgXtraInfo = "Services.ServiceIntegrationProcess.SaveDetailLog";
                        Services
                            .ServiceIntegrationProcess
                            .SaveDetailLog
                                        (
                                            db,
                                            ActiveUser.id,
                                            ref _integrationProcessDetail,
                                            id_statusAgregar
                                        );

                    });

                    // Documentos que deben estar Excluidos
                    msgXtraInfo = "Documentos que deben estar Excluidos";
                    integrationProcessLote
                        .IntegrationProcessDetail
                        .ToList()
                        .ForEach(r => {
                            if (!id_integrationProcessDetailList.Contains(r.id_Document))
                            {
                                //hasChange = true;
                                r.id_StatusDocument = id_statusEliminar;
                                r.id_UserUpdate = ActiveUser.id;
                                r.dateUpdate = DateTime.Now;
                                Services
                                    .ServiceIntegrationProcess
                                    .SaveDetailLog
                                                (
                                                    db,
                                                    ActiveUser.id,
                                                    ref r,
                                                    id_statusEliminar
                                                );
                            }

                        });

                }
                else
                {

                    msgXtraInfo = "Excluir todos los documentos";
                    //hasChange = true;
                    integrationProcessLote
                        .IntegrationProcessDetail
                        .ToList()
                        .ForEach(r => {

                            r.id_StatusDocument = id_statusEliminar;
                            r.id_UserUpdate = ActiveUser.id;
                            r.dateUpdate = DateTime.Now;
                            Services
                                .ServiceIntegrationProcess
                                .SaveDetailLog
                                            (
                                                db,
                                                ActiveUser.id,
                                                ref r,
                                                id_statusEliminar
                                            );


                        });
                }

                #endregion


                #region IntegrationProcess
                //if (description != integrationProcessLote.description)
                //{
                //    hasChange = true;
                //}

                integrationProcessLote.description = description;
                integrationProcessLote.dateUpdate = DateTime.Now;
                integrationProcessLote.id_userUpdate = ActiveUser.id;
                integrationProcessLote.id_StatusLote = id_StatusCreate;
                List<IntegrationProcessDetail> _integrationProcessDetail2 = integrationProcessLote
                                                                            .IntegrationProcessDetail
                                                                            .Where(r => r.id_StatusDocument == id_statusAgregar)
                                                                            .ToList();

                integrationProcessLote.countTotalDocuments = _integrationProcessDetail2.Count();
                integrationProcessLote.totalValue = _integrationProcessDetail2.Sum(r => r.totalValueDocument);
                #endregion






                #region Transaccion
                TempData["integrationProcess"] = integrationProcessLote;
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {

                        foreach (var i in integrationProcessLote
                                .IntegrationProcessPrintGroup
                                .ToList())
                        {

                            IntegrationProcessPrintGroup _PrintGroup = db.IntegrationProcessPrintGroup.FirstOrDefault(r => r.id == i.id);
                            db.IntegrationProcessPrintGroup.Remove(_PrintGroup);
                            db.Entry(_PrintGroup).State = EntityState.Deleted;


                        };



                        #region IntegrationProcessPrintGroup

                        msgXtraInfo = "Preparar Informacion para Impresión";
                        Services
                            .ServiceIntegrationProcess
                            .PreparePrintGroup(db, ref integrationProcessLote);

                        #endregion


                        msgXtraInfo = "SaveChanges";
                        db.IntegrationProcess.Attach(integrationProcessLote);
                        db.Entry(integrationProcessLote).State = EntityState.Modified;

                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        trans.Dispose();
                        throw e;
                    }
                }

                #endregion


                msgXtraInfo = "Services.ServiceIntegrationProcess.GetXGlosaForView";
                Services
                    .ServiceIntegrationProcess
                    .GetXGlosaForView(ref integrationProcessLote, id_statusEliminar, 4);


                msgXtraInfo = "Services.ServiceIntegrationProcess.GetLogView";
                Services
                    .ServiceIntegrationProcess
                    .GetLogView(ref integrationProcessLote);

                msgXtraInfo = "Services.ServiceIntegrationProcess.GetRequeridDate";
                integrationProcessLote.isRequeridDate = Services
                                                        .ServiceIntegrationProcess
                                                        .GetRequeridDate(db, integrationProcessLote.id_DocumentType);

                ViewData["EditMessage"] = SuccessMessage("Lote de Integración de Datos actualizado exitosamente.");


            }
            catch (Exception e)
            {
                ViewData["EditError"] = ErrorMessage(e.Message);
                LogWrite(e, null, "LotesPartialUpdate==>" + msgXtraInfo);

            }
            finally
            {
                TempData.Keep("integrationProcess");
            }

            return PartialView("_IntegrationProcessWrapperPartial", integrationProcessLote);

        }

        [HttpPost, ValidateInput(false)]
        [ActionName("approvelote")]
        public JsonResult LotesPartialApprove
                                (
                                    int id_IntegrationProcess
                                )
        {

            GenericResultJson JsonResult = new GenericResultJson();
            IntegrationProcess integrationProcessLote = new IntegrationProcess();
            //string msgGeneral = "";
            string msgXtraInfo = "";
            string msgError = "";

            try
            {
                integrationProcessLote = db.IntegrationProcess.FirstOrDefault(r => r.id == id_IntegrationProcess);
                IntegrationState _integrationState = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Approved);
                int id_StatusLote = _integrationState?.id ?? 0;

                if (id_StatusLote == 0)
                {
                    msgXtraInfo = "Obtención Status Lote";
                    throw new Exception("Tipo de Status Lote no Definido.");
                }

                integrationProcessLote.dateUpdate = DateTime.Now;
                integrationProcessLote.id_userUpdate = ActiveUser.id;
                integrationProcessLote.id_StatusLote = id_StatusLote;

                msgXtraInfo = "Validación Número de Documentos en Lote";
                List<IntegrationProcessDetail> integrationProcessDetailList = integrationProcessLote
                                                                                   .IntegrationProcessDetail
                                                                                   .Where(r => r.IntegrationState.code == EnumIntegrationProcess.States.Added)
                                                                                   .ToList();



                if (integrationProcessDetailList == null || integrationProcessDetailList.Count() == 0)
                {
                    throw new Exception("No existen documentos en el Lote #:" + integrationProcessLote.codeLote);
                }


                msgXtraInfo = "Services.ServiceIntegrationProcess.ProcesingQueue";
                msgError = Services
                            .ServiceIntegrationProcess
                            .ProcesingQueue
                            (
                                db,
                                ActiveUser.id,
                                ref integrationProcessLote
                            );

                if (!string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(msgError);
                }



                #region Log
                msgXtraInfo = "Services.ServiceIntegrationProcess.SaveLog";
                Services
                    .ServiceIntegrationProcess
                    .SaveLog
                    (
                        db,
                        ActiveUser.id,
                        ref integrationProcessLote,
                        id_StatusLote
                    );

                #endregion


                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {

                        msgXtraInfo = "SaveChanges";
                        db.IntegrationProcess.Attach(integrationProcessLote);
                        db.Entry(integrationProcessLote).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        trans.Dispose();
                        throw e;
                    }
                }


                JsonResult.codeReturn = 1;
                JsonResult.ActionAccessList = setControlsAccess(EnumIntegrationProcess.States.Approved);
                JsonResult.message = SuccessMessage("Lote de Integración de Datos: Nª" + integrationProcessLote.codeLote + " Aprobado.");

                JsonResult.ValueDataList = new List<ValueData>();
                JsonResult.ValueDataList.Add(new ValueData { CodeObject = "integrationState", valueObject = _integrationState.description });

            }
            catch (Exception e)
            {

                JsonResult.codeReturn = -1;
                JsonResult.message = ErrorMessage(e.Message);
                LogWrite(e, null, "LotesPartialExecute==>" + msgXtraInfo);
            }
            finally
            {

                TempData.Keep("integrationProcess");
            }

            return Json(JsonResult, JsonRequestBehavior.AllowGet);

        }


        [HttpPost, ValidateInput(false)]
        [ActionName("execlote")]
        public JsonResult LotesPartialExecute
                                (
                                    int id_IntegrationProcess
                                )
        {

            GenericResultJson JsonResult = new GenericResultJson();
            IntegrationProcess integrationProcessLote = new IntegrationProcess();
            //string msgGeneral = "";
            string msgXtraInfo = "";
            string msgError = "";
            int id_DocumentType = 0;

            try
            {


                integrationProcessLote = db.IntegrationProcess.FirstOrDefault(r => r.id == id_IntegrationProcess);
                IntegrationState _integrationStateApproved = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Approved);
                IntegrationState _integrationState = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Transmitted);
                IntegrationState _integrationStateDeleted = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Deleted);

                if (_integrationStateApproved == null)
                {
                    msgXtraInfo = "Obtención Status Aprobado Lote";
                    throw new Exception("Tipo de Status Aprobado no Definido.");
                }

                if (_integrationState == null)
                {
                    msgXtraInfo = "Obtención Status Transmitido Lote";
                    throw new Exception("Tipo de Status Transmitido no Definido.");
                }

                if (_integrationStateDeleted == null)
                {
                    msgXtraInfo = "Obtención Status Eliminado Lote";
                    throw new Exception("Tipo de Status Eliminado no Definido.");
                }


                int id_StatusLote = _integrationState?.id ?? 0;


                if (integrationProcessLote.id_StatusLote != _integrationStateApproved.id)
                {
                    msgXtraInfo = "Validar Estatus Lote";
                    throw new Exception("Lote no se encuentra en estado aprobado.");
                }

                id_DocumentType = integrationProcessLote.id_DocumentType;

                integrationProcessLote.dateUpdate = DateTime.Now;
                integrationProcessLote.id_userUpdate = ActiveUser.id;
                integrationProcessLote.id_StatusLote = id_StatusLote;


                msgXtraInfo = "Validación Número de Documentos en Lote";
                List<IntegrationProcessDetail> integrationProcessDetailList = integrationProcessLote
                                                                                    .IntegrationProcessDetail
                                                                                    .Where(r => r.IntegrationState.code == EnumIntegrationProcess.States.Added)
                                                                                    .ToList();


                if (integrationProcessDetailList == null || integrationProcessDetailList.Count() == 0)
                {
                    throw new Exception("No existen documentos en el Lote #:" + integrationProcessLote.codeLote);
                }

                IIntegrationProcessActionOutput integrationImplementation = ServiceIntegrationProcess
                                                                                       .IntegrationProcessDefinition(db, id_DocumentType);

                msgXtraInfo = "Services.ServiceIntegrationProcess.TransferData";
                msgError = ServiceIntegrationProcess
                                .TransferData
                                (
                                    db,
                                    ActiveUser.id,
                                    integrationProcessLote.IntegrationProcessOutput.Where(r => r.idStatusOutput != _integrationStateDeleted.id).ToList(),
                                    id_IntegrationProcess,
                                    id_DocumentType,
                                    integrationImplementation.TransferData
                                 );


                if (!string.IsNullOrEmpty(msgError))
                {
                    throw new Exception(msgError);
                }

                #region Log
                msgXtraInfo = "Services.ServiceIntegrationProcess.SaveLog";
                Services
                    .ServiceIntegrationProcess
                    .SaveLog
                    (
                        db,
                        ActiveUser.id,
                        ref integrationProcessLote,
                        id_StatusLote
                    );

                #endregion


                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {

                        msgXtraInfo = "SaveChanges";
                        db.IntegrationProcess.Attach(integrationProcessLote);
                        db.Entry(integrationProcessLote).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();

                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        trans.Dispose();
                        throw e;
                    }
                }



                JsonResult.codeReturn = 1;
                JsonResult.ActionAccessList = setControlsAccess(EnumIntegrationProcess.States.Transmitted);
                JsonResult.message = SuccessMessage("Lote de Integración de Datos: Nª" + integrationProcessLote.codeLote + " Transmitido.");


                JsonResult.ValueDataList = new List<ValueData>();
                JsonResult.ValueDataList.Add(new ValueData { CodeObject = "integrationState", valueObject = _integrationState.description });


            }
            catch (Exception e)
            {

                JsonResult.codeReturn = -1;
                JsonResult.message = ErrorMessage(e.Message);
                LogWrite(e, null, "LotesPartialExecute==>" + msgXtraInfo);
            }
            finally
            {

                TempData.Keep("integrationProcess");
            }

            return Json(JsonResult, JsonRequestBehavior.AllowGet);

        }


        [HttpPost, ValidateInput(false)]
        [ActionName("dellote")]
        public JsonResult LotesPartialDelete
                               (
                                   int id_IntegrationProcess
                               )
        {

            GenericResultJson JsonResult = new GenericResultJson();
            string msgXtraInfo = "";
            IntegrationProcess integrationProcessLote = new IntegrationProcess();

            try
            {
                integrationProcessLote = db.IntegrationProcess.FirstOrDefault(r => r.id == id_IntegrationProcess);


                // obtener estado factible de eliminacion :
                int id_StatusCanDelete = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Create)?.id ?? 0;
                if (id_StatusCanDelete == 0)
                {
                    msgXtraInfo = "Obtención Estatus Creación";
                    throw new Exception("Tipo de Estatus de Integración: Creado, No Definido.");
                }

                // Obtener id estado Eliminación
                IntegrationState _integrationStateDelete = db.IntegrationState.FirstOrDefault(r => r.code == EnumIntegrationProcess.States.Deleted);
                int id_StatusLote = _integrationStateDelete?.id ?? 0;
                if (id_StatusLote == 0)
                {
                    msgXtraInfo = "Obtención Estatus Eliminación";
                    throw new Exception("Tipo de Estatus de Integracióin: Elimnado, No Definido.");
                }

                if (integrationProcessLote.id_StatusLote != id_StatusCanDelete)
                {
                    msgXtraInfo = "Validación Factibilidad Anulacion Lote Integración";
                    throw new Exception("Lote de Integracion no puede ser Anulado");
                }

                integrationProcessLote.dateUpdate = DateTime.Now;
                integrationProcessLote.id_userUpdate = ActiveUser.id;
                integrationProcessLote.id_StatusLote = id_StatusLote;

                #region Log
                msgXtraInfo = "Services.ServiceIntegrationProcess.SaveLog";
                Services
                   .ServiceIntegrationProcess
                   .SaveLog
                    (
                        db,
                        ActiveUser.id,
                        ref integrationProcessLote,
                        id_StatusLote
                    );

                #endregion
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        msgXtraInfo = "SaveChanges";
                        db.IntegrationProcess.Attach(integrationProcessLote);
                        db.Entry(integrationProcessLote).State = EntityState.Modified;
                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception e)
                    {

                        trans.Rollback();
                        trans.Dispose();
                        throw e;
                    }
                }

                JsonResult.codeReturn = 1;
                JsonResult.ActionAccessList = setControlsAccess(EnumIntegrationProcess.States.Deleted);
                JsonResult.message = SuccessMessage("Lote de Integración de Datos: Nª" + integrationProcessLote.codeLote + " Anulado.");

                JsonResult.ValueDataList = new List<ValueData>();
                JsonResult.ValueDataList.Add(new ValueData { CodeObject = "integrationState", valueObject = _integrationStateDelete.description });


            }
            catch (Exception e)
            {

                JsonResult.codeReturn = -1;
                JsonResult.message = ErrorMessage(e.Message);
                LogWrite(e, null, "LotesPartialDelete==>" + msgXtraInfo);

            }
            finally
            {

                TempData.Keep("integrationProcess");
            }


            return Json(JsonResult, JsonRequestBehavior.AllowGet);

        }



        [HttpPost, ValidateInput(false)]
        [ActionName("printlote")]
        public JsonResult LotePartialPrint(string codeReport, int id_IntegrationProcess)
        {
            #region "Armo Parametros"

            List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR();

            _param.Nombre = "@id";
            _param.Valor = id_IntegrationProcess.ToString();
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

            #endregion
        }

        #endregion


        #region Document Process Find

        [HttpPost, ValidateInput(false)]
        [ActionName("indexfind")]
        public PartialViewResult DocumentFlterForm()
        {
            IntegrationProcessDetail integrationProcessDetail = new IntegrationProcessDetail();

            return PartialView("IntegrationProcessFindDocuments/_IntegrationProcessInitFind", integrationProcessDetail);
        }


        [HttpPost, ValidateInput(false)]
        [ActionName("finddocument")]
        public PartialViewResult DocumentResultForm
                                (
                                    int? id_DocumentType,
                                    DateTime? dateInitEmission,
                                    DateTime? dateEndEmission,
                                    int?[] id_EmissionPoint,
                                    string numberDocument,
                                    int[] id_integrationProcessDetailList,
                                    int id_IntegrationProcessLote
                                )
        {


            List<IntegrationProcessDetail> documentProcessList
                                            = Services
                                                .ServiceIntegrationProcess
                                                .FindDocuments
                                                (
                                                    db,
                                                    ActiveUser.id,
                                                    this.ActiveCompanyId,
                                                    id_DocumentType,
                                                    dateInitEmission,
                                                    dateEndEmission,
                                                    id_EmissionPoint,
                                                    numberDocument,
                                                    id_integrationProcessDetailList,
                                                    id_IntegrationProcessLote
                                                );

            TempData["documentProcessList"] = documentProcessList;
            TempData.Keep("documentProcessList");

            return PartialView("IntegrationProcessFindDocuments/_IntegrationProcessDocumentResultsPartial", documentProcessList);

        }


        [HttpPost, ValidateInput(false)]
        [ActionName("getdocument")]
        public PartialViewResult DocumentPartialForm
                                (
                                    int id_document
                                )
        {

            Document documentProcess = db.Document.FirstOrDefault(r => r.id == id_document);


            TempData["documentProcess"] = documentProcess;
            TempData.Keep("documentProcess");

            return PartialView("_DocumentProcessMainFormPartial", documentProcess);

        }
        #endregion



        #region Auxiliares


        [HttpPost, ValidateInput(false)]
        [ActionName("configcontrol")]
        public JsonResult LotesPartialConfigControl
                               (
                                   int id_IntegrationProcess
                               )
        {

            GenericResultJson oJsonResult = new GenericResultJson();
            IntegrationProcess integrationProcessLote = new IntegrationProcess();
            string msgXtraInfo = "";


            try
            {
                msgXtraInfo = "Obtener Lote";
                string StatusCode = null;
                integrationProcessLote = db.IntegrationProcess.FirstOrDefault(r => r.id == id_IntegrationProcess);
                if (integrationProcessLote != null)
                {
                    StatusCode = integrationProcessLote.IntegrationState.code;
                }

                msgXtraInfo = "Obtener Acceso Controles";
                oJsonResult.codeReturn = 1;
                oJsonResult.ActionAccessList = setControlsAccess(StatusCode);
                oJsonResult.message = SuccessMessage("");

            }
            catch (Exception e)
            {

                oJsonResult.codeReturn = -1;
                oJsonResult.message = ErrorMessage(e.Message);
                LogWrite(e, null, "LotesPartialConfigControl==>" + msgXtraInfo);
            }
            finally
            {

                TempData.Keep("integrationProcess");
            }

            return Json(oJsonResult, JsonRequestBehavior.AllowGet);

        }



        private List<ActionAccess> setControlsAccess(string stateIntegration)
        {
            List<ActionAccess> setControlsAccessReturn = new List<ActionAccess>();

            switch (stateIntegration)
            {
                case EnumIntegrationProcess.States.Process:
                case EnumIntegrationProcess.States.Transmitted:
                case EnumIntegrationProcess.States.Deleted:
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnNew", Enabled = true });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnSave", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnCancel", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnApprove", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnProcesar", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnNewDetail", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnRemoveDetail", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "description", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnPrint", Enabled = true });
                    break;

                case EnumIntegrationProcess.States.Approved:

                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnNew", Enabled = true });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnSave", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnCancel", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnApprove", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnProcesar", Enabled = true });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnNewDetail", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnRemoveDetail", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "description", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnPrint", Enabled = true });
                    break;
                case EnumIntegrationProcess.States.Create:
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnNew", Enabled = true });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnSave", Enabled = true });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnCancel", Enabled = true });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnApprove", Enabled = true });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnProcesar", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnNewDetail", Enabled = true });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnRemoveDetail", Enabled = true });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "description", Enabled = true });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnPrint", Enabled = true });
                    break;
                default:
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnNew", Enabled = true });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnSave", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnCancel", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnApprove", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnProcesar", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnNewDetail", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnRemoveDetail", Enabled = false });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "description", Enabled = true });
                    setControlsAccessReturn.Add(new ActionAccess { CodeObject = "btnPrint", Enabled = false });


                    break;

            }

            return setControlsAccessReturn;
        }
        #endregion
    }
}