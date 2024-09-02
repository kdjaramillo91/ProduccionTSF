using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Helper;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.DocumentLogDTO;
using DXPANACEASOFT.Models.DocumentP.DocumentModels;
using DXPANACEASOFT.Models.DocumentStateP.DocumentStateModel;
using DXPANACEASOFT.Models.General;
using DXPANACEASOFT.Models.RequestInventoryMoveDTO;
using DXPANACEASOFT.Models.RequestInventoryMoveModel;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Utilitarios.Logs;
using Utilitarios.ProdException;

namespace DXPANACEASOFT.Controllers
{
	[Authorize]
	public class RequestInventoryMoveController : DefaultController
	{
		#region INDEX
		[HttpPost]
		public ActionResult Index()
		{
			RefreshComboBoxFilterForm();
			return View();
		}
		#endregion

		#region REQUEST INVENTORY MOVE RESULTS
		[HttpPost, ValidateInput(false)]
		[Authorize]
		public ActionResult RequesInventoryMoveResults(FilterQueryRequestInventoryMove filterForm)
		{
			RefreshComboBoxFilterForm();
			List<ResultRequestInventoryMoveAll> lstAllRequestInventoryMove = ServiceRequestInventoryMove.GetAllRequestInventoryMove(db, filterForm);
			FilterWarehouseResult(ref lstAllRequestInventoryMove);
			TempData["filterForm"] = filterForm;
			return PartialView("_RequestInventoryMoveAllRegisterResults", lstAllRequestInventoryMove.OrderByDescending(o => o.sequentialRemissionGuide).ToList());
		}
		[HttpPost, ValidateInput(false)]
		[Authorize]
		public ActionResult RequestInventoryMoveAll(FilterQueryRequestInventoryMove filterForm)
		{
			RefreshComboBoxFilterForm();
			List<ResultRequestInventoryMoveAll> lstAllRequestInventoryMove = ServiceRequestInventoryMove.GetAllRequestInventoryMove(db, filterForm);
			FilterWarehouseResult(ref lstAllRequestInventoryMove);
			return PartialView("_RequestInventoryMoveAllRegister", lstAllRequestInventoryMove.OrderByDescending(o => o.sequentialRemissionGuide).ToList());
		}

		#endregion

		#region REQUEST INVENTORY MOVE HEADER

		[HttpPost, ValidateInput(false)]
		public ActionResult FormEditRequestInventoryMove(int idRim)
		{
			RequestInventoryMoveEditForm rqInvMov = ServiceRequestInventoryMove.GetRequestInventoryMove(db, idRim);
			SaveDataEditForm(rqInvMov);
			RefresshDataForEditForm(rqInvMov);
			return PartialView("_FormEditRequestInventoryMove", rqInvMov);
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult RequestInventoryMovePartialAddNew(RequestInventoryMoveTransferPUpdateEntity _headerRim)
		{
			#region Validate Data 

			#endregion

			#region Update Data
			#endregion

			RequestInventoryMoveEditForm rqInvMov = GetTempDataEditForm();
			rqInvMov = ServiceRequestInventoryMove.GetRequestInventoryMove(db, rqInvMov.rqModel.idRIMModelP);
			SaveDataEditForm(rqInvMov);
			RefresshDataForEditForm();
			return PartialView("_FormEditRequestInventoryMove", rqInvMov);

		}
		[HttpPost, ValidateInput(false)]
		public ActionResult RequestInventoryMovePartialUpdate(RequestInventoryMoveTransferPUpdateEntity _headerRim)
		{
			AnswerValidateForm result = new AnswerValidateForm();
            RequestInventoryMoveEditForm rqInvMov = new RequestInventoryMoveEditForm();

            try
			{
                #region Validate Data 
                result = ValidateDataFormForUpdate(_headerRim);
                if (result != null && result.hasError == "Y")
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                #endregion

                rqInvMov = GetTempDataEditForm();

                #region Update Data
                rqInvMov = AddUpdateDataFromForm(rqInvMov);
                #endregion

                SaveDataEditForm(rqInvMov);
                RefresshDataForEditForm();
            }
			catch (ProdHandlerException e)
			{
				result.hasError = "Y";
				result.message += e.Message;

            }
			catch (Exception e)
			{
                result.hasError = "Y";
                result.message += GenericError.ErrorGeneralSaveRequerimientoInventario;
                FullLog(e);
			}

			if((result?.hasError??"N") == "Y")
			{
                return Json(result, JsonRequestBehavior.AllowGet);
            }
			
			return PartialView("_RequestInventoryMoveMainFormPartial", rqInvMov);
		}
		#endregion

		#region REQUEST INVENTORY MOVE HEADER INFORMATION 
		[HttpPost, ValidateInput(false)]
		[Authorize]
		public ActionResult DocsGeneratedFromApprovedIM()
		{
			SaveDataEditForm(null);
			RequestInventoryMoveEditForm rqTmp = GetTempDataEditForm();
			var model = rqTmp?.rqTransfer?.lstReqAppDocsIM.OrderBy(o => o.nameNatureMove).ToList();

			return PartialView("_RequestInventoryMoveDocGeneratedIMDetailViewDetailsPartial", model.ToList());
		}
		#endregion

		#region DETAILS REQUEST INVENTORY MOVE

		[HttpPost, ValidateInput(false)]
		[Authorize]
		public ActionResult RequestInventoryMoveDetailsPartial()
		{

			SaveDataEditForm(null);
			RequestInventoryMoveEditForm rqTmp = GetTempDataEditForm();
			var model = rqTmp?.rqTransfer?.lstRequestInvDetail.Where(d => d.isActive);

			return PartialView("_RequestInventoryMoveDetails", model.ToList());
		}

		[HttpPost, ValidateInput(false)]
		[Authorize]
		public ActionResult RequestInventoryMoveDetailsPartialAddNew(RequestInventoryMoveDetailTransferP rqDetailTmp)
		{
			RequestInventoryMoveEditForm rqTmp = GetTempDataEditForm();

			try
			{
				rqTmp = ServiceRequestInventoryMove.AddNewDetailRequestInventoryMove(rqDetailTmp, rqTmp);
			}
			catch (Exception e)
			{
				ViewData["EditError"] = e.Message;
			}

			SaveDataEditForm(rqTmp);
			var model = rqTmp?.rqTransfer?.lstRequestInvDetail.Where(w => w.isActive).ToList();

			return PartialView("_RequestInventoryMoveDetails", model);
		}

		[HttpPost, ValidateInput(false)]
		[Authorize]
		public ActionResult RequestInventoryMoveDetailsPartialUpdate(RequestInventoryMoveDetailTransferP rqDetailTmp)
		{
			RequestInventoryMoveEditForm rqTmp = GetTempDataEditForm();

			try
			{
				rqTmp = ServiceRequestInventoryMove.UpdateDetailRequestInventoryMove(rqDetailTmp, rqTmp);
			}
			catch (Exception e)
			{
				ViewData["EditError"] = e.Message;
			}

			SaveDataEditForm(rqTmp);
			var model = rqTmp?.rqTransfer?.lstRequestInvDetail.Where(w => w.isActive).ToList();

			return PartialView("_RequestInventoryMoveDetails", model);
		}

		[HttpPost, ValidateInput(false)]
		[Authorize]
		public ActionResult RequestInventoryMoveDetailsPartialDelete(int id)
		{
			RequestInventoryMoveEditForm rqTmp = GetTempDataEditForm();

			try
			{
				ServiceRequestInventoryMove.DeleteDetailRequestInventoryMove(id, rqTmp);
			}
			catch (Exception e)
			{
				ViewData["EditError"] = e.Message;
			}

			SaveDataEditForm(rqTmp);
			var model = rqTmp?.rqTransfer?.lstRequestInvDetail.Where(w => w.isActive).ToList();

			return PartialView("_RequestInventoryMoveDetails", model);
		}

		#endregion

		#region CALLBACKS COMBOBOX
		[HttpPost]
		[Authorize]
		public PartialViewResult GetItemDetails(int? indice)
		{
			SaveDataEditForm(null);
			RequestInventoryMoveEditForm rqTmp = GetTempDataEditForm();
			RequestInventoryMoveDetailTransferP rqDetail = rqTmp?.rqTransfer?.lstRequestInvDetail.FirstOrDefault(fod => fod.id == indice);
			return this.PartialView("ComponentsDetail/_ComboBoxItems", rqDetail ?? new RequestInventoryMoveDetailTransferP());
		}
		#endregion

		#region ACTIONS
		[HttpPost, ValidateInput(false)]
		[Authorize]
		public JsonResult Actions(int id)
		{
			var actions = new
			{
				btnApprove = false,
				btnAutorize = false,
				btnProtect = false,
				btnCancel = false,
				btnRevert = false
			};

			if (id == 0)
			{
				return Json(actions, JsonRequestBehavior.AllowGet);
			}

			DocumentModelP _doc = DataProviderDocument.GetOneDocumentModelP(id);

			string state = DataProviderDocumentState.QueryDocumentStatesByDocumentType(db, "79")
								.FirstOrDefault(fod => fod.idDocumentStateModelP == _doc.idDocumentStateModelP)?
								.codeDocumentStateModelP;

			if (state == "01") // PENDIENTE
			{
				actions = new
				{
					btnApprove = true,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = false
				};
			}
			else if (state == "03")//|| state == 3) // APROBADA
			{
				actions = new
				{
					btnApprove = false,
					btnAutorize = true,
					btnProtect = false,
					btnCancel = false,
					btnRevert = true
				};
			}
			else if (state == "04" || state == "05") // CERRADA O ANULADA
			{
				actions = new
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = false
				};
			}
			else if (state == "06") // AUTORIZADA
			{
				actions = new
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = true
				};
			}
			return Json(actions, JsonRequestBehavior.AllowGet);
		}


		#endregion

		#region APROVEIDS
		[HttpPost, ValidateInput(false)]
		public ActionResult ApproveIds(int[] ids)
		{
			if (ids != null)
			{
				foreach (var id in ids)
				{
					using (DbContextTransaction trans = db.Database.BeginTransaction())
					{
						try
						{
							RequestInventoryMoveTransferPUpdateEntity _headerRim = new RequestInventoryMoveTransferPUpdateEntity();
							var rqInvMov = ServiceRequestInventoryMove.GetRequestInventoryMove(db, id);

							if (rqInvMov != null)
							{
								_headerRim.dEmissionDate = rqInvMov.rqTransfer.documentRequestTransferP.emissionDateTransferP.Day;
								_headerRim.emissionDateDoc = rqInvMov.rqTransfer.documentRequestTransferP.emissionDateTransferP;
								_headerRim.hoursEmissionDate = rqInvMov.rqTransfer.documentRequestTransferP.emissionDateTransferP.Hour;
								_headerRim.mEmissionDate = rqInvMov.rqTransfer.documentRequestTransferP.emissionDateTransferP.Month;
								_headerRim.minutesEmissionDate = rqInvMov.rqTransfer.documentRequestTransferP.emissionDateTransferP.Minute;
								_headerRim.yEmissionDate = rqInvMov.rqTransfer.documentRequestTransferP.emissionDateTransferP.Year;
								_headerRim.id_Rim = id;
								_headerRim.id_Warehouse = rqInvMov.rqTransfer.documentRequestTransferP.idWarehouseHeadP;
								_headerRim.id_NatureMove = rqInvMov.rqTransfer.documentRequestTransferP.idNatureMoveHeadP;
								_headerRim.id_PersonRequest = rqInvMov.rqTransfer.idPersonRTransferP;
							}

							TempData["_reqInvMovEditForm"] = rqInvMov;

							AnswerValidateForm result = ValidateDataFormForUpdate(_headerRim);
							SaveDataEditForm(null);
							if (result != null && result.hasError == "Y")
							{
								//throw new Exception("Error al aprobar.");
								return Json(result, JsonRequestBehavior.AllowGet);
							}

							RequestInventoryMoveEditForm rqInvMov2 = GetTempDataEditForm();

							if (!FilterWarehouseResult(rqInvMov2, "Aprobar"))
							{
								result = new AnswerValidateForm
								{

									hasError = "Y",
									message = "El usuario no puede reversar porque el usuario no tiene permisos para esta bodega."
								};
								//throw new Exception("El usuario no puede reversar porque el usuario no tiene permisos para esta bodega.");
								return Json(result, JsonRequestBehavior.AllowGet);
							}

							ApproveRequestInventoryMoveFromRemissionGuide(rqInvMov, (int)_headerRim.id_Rim);
							db.SaveChanges();
							trans.Commit();
						}
						catch (Exception ex)
						{
							ViewData["EditError"] = ex.Message;
							trans.Rollback();
							return Json(new AnswerValidateForm
							{
								hasError = "Y",
								message = ex.Message
							}, JsonRequestBehavior.AllowGet);

							//RequesInventoryMoveResults(TempData["filterForm"] as FilterQueryRequestInventoryMove);
						}
					}
				}
			}

			var filterForm = TempData["filterForm"] as FilterQueryRequestInventoryMove;
			RefreshComboBoxFilterForm();
			List<ResultRequestInventoryMoveAll> lstAllRequestInventoryMove = ServiceRequestInventoryMove.GetAllRequestInventoryMove(db, filterForm);
			FilterWarehouseResult(ref lstAllRequestInventoryMove);

			return PartialView("_RequestInventoryMoveAllRegisterResults", lstAllRequestInventoryMove.OrderByDescending(o => o.sequentialRemissionGuide).ToList());
		}
		#endregion

		#region SINGLE CHANGE STATE DOCUMENT
		[HttpPost]
		public ActionResult Approve(RequestInventoryMoveTransferPUpdateEntity _headerRim)
		{
			AnswerValidateForm result = new AnswerValidateForm();
			RequestInventoryMoveEditForm rqInvMov = new RequestInventoryMoveEditForm();

            try
			{

                #region Validate Data 
                result = ValidateDataFormForUpdate(_headerRim);
                SaveDataEditForm(null);
                if (result != null && result.hasError == "Y")
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                #endregion

                rqInvMov = GetTempDataEditForm();

                #region Validate Warehouse
                if (!FilterWarehouseResult(rqInvMov, "Aprobar"))
                {
                    result = new AnswerValidateForm
                    {
                        hasError = "Y",
                        message = "El usuario no puede aprobar porque el usuario no tiene permisos para esta bodega."
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region Optimizacion  Completar Modelo 
                var queryDocSour = DataProviderDocumentSource.QueryDocumentsSource(db, (int)_headerRim.id_Rim);
                var dtmp = DataProviderDocumentType.GetOneDocumentTypeByCode("08");
                var queryDocuments = DataProviderDocument.QueryDocumentModelP(db, dtmp.idDocumentTypeModelP);
                var lstRemissionGuideDocument = (from _lsDs in queryDocSour
                                                 join _ltD in queryDocuments on _lsDs.id_Document_Origin_P equals _ltD.idDocumentModelP
                                                 select new DocumentModelP
                                                 {
                                                     idDocumentModelP = _ltD.idDocumentModelP,
                                                     numberModelP = _ltD.numberModelP,
                                                     sequentialModelP = _ltD.sequentialModelP,
                                                     emissionDateModelP = _ltD.emissionDateModelP,
                                                     authorizationDateModelP = _ltD.authorizationDateModelP,
                                                     authorizationNumberModelP = _ltD.authorizationNumberModelP,
                                                     accessKeyModelP = _ltD.accessKeyModelP,
                                                     descriptionModelP = _ltD.descriptionModelP,
                                                     referenceModelP = _ltD.referenceModelP,
                                                     idEmissionPointModelP = _ltD.idEmissionPointModelP,
                                                     idDocumentTypeModelP = _ltD.idDocumentTypeModelP,
                                                     idDocumentStateModelP = _ltD.idDocumentStateModelP,
                                                     isOpenModelP = _ltD.isOpenModelP
                                                 }).ToList();

                var docStateNulled = DataProviderDocumentState.GetDocumentStateByCode("05");
				int id_remissionGuide = 0;
                if (lstRemissionGuideDocument != null && lstRemissionGuideDocument.Count() > 0)
                {
                    int idDocStateNulled = docStateNulled?.idDocumentStateModelP ?? 0;
                    DocumentModelP dmp = lstRemissionGuideDocument.Where(w => w.idDocumentStateModelP != idDocStateNulled).FirstOrDefault();
                    id_remissionGuide = dmp?.idDocumentModelP ?? 0;
                }

                RequestInventoryMove rimDB = db.RequestInventoryMove
													.Include("Document")
                                                    .Include("RequestInventoryMoveDetail") 
													.FirstOrDefault(fod => fod.id == rqInvMov.rqModel.idRIMModelP);
				DocumentState[] documentStates = db.DocumentState.ToArray();

                Document _do = db.Document.FirstOrDefault(fod => fod.id == (int)_headerRim.id_Rim);

                int idDa = db.tbsysActionOnDocument.FirstOrDefault(fod => fod.code.Equals("AD")).id;
				
				EmissionPoint[] emissionPoints = db.EmissionPoint.ToArray();

                RemissionGuide rg = db.RemissionGuide
										.Include("RemissionGuideDispatchMaterial") 
                                        .Include("RemissionGuideDispatchMaterial.Item")
                                        .Include("RemissionGuideDispatchMaterial.Item.InventoryLine")
                                        .FirstOrDefault(fod => fod.id == id_remissionGuide);

				Setting[] settings = db.Setting.ToArray();
				WarehouseLocation[] warehouseLocations = db.WarehouseLocation.ToArray();
                Provider providerAux = db.Provider.FirstOrDefault(fod => fod.id == rg.id_providerRemisionGuide);

				Warehouse[] warehouses = db.Warehouse.ToArray();

 				SettingDetail[] settingDetails = db.SettingDetail.ToArray();
				InventoryReason[] inventoryReasons = db.InventoryReason.ToArray();
				Employee employee = db.Employee.FirstOrDefault(fod => fod.id == ActiveUser.id_employee);
				MetricUnitConversion[] metricUnitConversions = db.MetricUnitConversion.ToArray();

                #endregion

                #region Complete Transaction

                SaveDataEditForm(null);
                using (DbContextTransaction trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        ApproveRequestInventoryMoveFromRemissionGuideOP(
							rqInvMov, 
							(int)_headerRim.id_Rim,
							lstRemissionGuideDocument: lstRemissionGuideDocument.ToArray(),
							documentStateModelP: docStateNulled,
							requestInventoryMove: rimDB,
							documentStates: documentStates,
							document: _do, 
							tbsysActionOnDocumentId: idDa,
							emissionPoints: emissionPoints,
							id_remissionGuide: id_remissionGuide,
							remissionGuide: rg,
							settings: settings,
							warehouseLocations: warehouseLocations,
							providerAux: providerAux,
							warehouses: warehouses,
							settingDetails: settingDetails,
							inventoryReasons: inventoryReasons,
							employee: employee,
							metricUnitConversions: metricUnitConversions);

                        db.SaveChanges();
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        SaveDataEditForm(rqInvMov);
						FullLog(ex);
                        return Json(new AnswerValidateForm
                        {
                            hasError = "Y",
                            message = ex.Message
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion

				// ETAPA 2 Tiempo
                rqInvMov = ServiceRequestInventoryMove.GetRequestInventoryMove(db, (int)_headerRim.id_Rim);
                SaveDataEditForm(rqInvMov);
                RefresshDataForEditForm();
            }
            catch (ProdHandlerException e)
            {
                result.hasError = "Y";
                result.message += e.Message;
            }
            catch (Exception e)
            {
                result.hasError = "Y";
                result.message += GenericError.ErrorGeneralAproveeRequerimientoInventario;
                FullLog(e);
            }

            if ((result?.hasError ?? "N") == "Y")
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return PartialView("_RequestInventoryMoveMainFormPartial", rqInvMov);
		}
		[HttpPost]
		public ActionResult Revert(RequestInventoryMoveTransferPUpdateEntity _headerRim)
		{
			#region Validate Data 
			AnswerValidateForm result = ValidateDataFormForUpdate(_headerRim);
			if (result != null && result.hasError == "Y")
			{
				SaveDataEditForm(null);
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			#endregion

			RequestInventoryMoveEditForm rqInvMov = GetTempDataEditForm();

			#region Validate Warehouse
			if (!FilterWarehouseResult(rqInvMov, "Reversar"))
			{
				result = new AnswerValidateForm
				{
					hasError = "Y",
					message = "El usuario no puede reversar porque el usuario no tiene permisos para esta bodega."
				};
				return Json(result, JsonRequestBehavior.AllowGet);
			}
			#endregion

			#region Complete Transaction

			SaveDataEditForm(null);
			using (DbContextTransaction trans = db.Database.BeginTransaction())
			{
				try
				{
					RevertRequestInventoryMoveFromRemissionGuide(rqInvMov, (int)_headerRim.id_Rim);
					db.SaveChanges();
					trans.Commit();
				}
				catch (Exception ex)
				{
					trans.Rollback();

					return Json(new AnswerValidateForm
					{
						hasError = "Y",
						message = ex.Message
					}, JsonRequestBehavior.AllowGet);
				}
			}
			#endregion

			rqInvMov = ServiceRequestInventoryMove.GetRequestInventoryMove(db, (int)_headerRim.id_Rim);
			SaveDataEditForm(rqInvMov);
			RefresshDataForEditForm();
			return PartialView("_RequestInventoryMoveMainFormPartial", rqInvMov);
		}

		#endregion

		#region PAGINATION
		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id)
		{
			SaveDataEditForm(null);
			RefresshDataForEditForm();
			int index = DataProviderRequestInventoryMove.GetIndexRequestInventoryMove(db, id);

			var result = new
			{
				maximunPages = DataProviderRequestInventoryMove.GetCountRequestInventoryMove(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			RefresshDataForEditForm();
			int idLast = DataProviderRequestInventoryMove.GetLastIdRequestInventoryMove(db, page);
			RequestInventoryMoveEditForm rqInvMov = ServiceRequestInventoryMove.GetRequestInventoryMove(db, idLast);

			if (rqInvMov != null)
			{
				SaveDataEditForm(rqInvMov);
				return PartialView("_RequestInventoryMoveMainFormPartial", rqInvMov);
			}

			return PartialView("_RequestInventoryMoveMainFormPartial", new RequestInventoryMoveEditForm());
		}
		#endregion

		#region REPORTS
		[HttpPost, ValidateInput(false)]
		public JsonResult PrintRequerimentMove(int idRequestInventoryMove)
		{
			SaveDataEditForm(null);
			RequestInventoryMoveEditForm rqInvMov = GetTempDataEditForm();
			if (rqInvMov != null) SaveDataEditForm(rqInvMov);

			#region "Armo Parametros"

			List<ParamCR> paramLst = new List<ParamCR>();
            ParamCR _param = new ParamCR
            {
                Nombre = "@id",
                Valor = idRequestInventoryMove,
            };

            paramLst.Add(_param);

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = "RMIRFE";
			_repMod.conex = objConex;
			_repMod.paramCRList = paramLst;

			rep = GetTmpDataName(20);

			TempData[rep.nameQS] = _repMod;
			TempData.Keep(rep.nameQS);

			var result = rep;

			return Json(result, JsonRequestBehavior.AllowGet);

            #endregion
        }


        [HttpPost, ValidateInput(false)]
		public JsonResult PrintInventoryMoveGenerated(int idInvMov)
		{
			SaveDataEditForm(null);
			RequestInventoryMoveEditForm rqInvMov = GetTempDataEditForm();
			if (rqInvMov != null) SaveDataEditForm(rqInvMov);

			#region "Armo Parametros"
			List<ParamCR> paramLst = new List<ParamCR>();
			ParamCR _param = new ParamCR();
			_param.Nombre = "@id";
			_param.Valor = idInvMov;

			paramLst.Add(_param);

			Conexion objConex = GetObjectConnection("DBContextNE");
			ReportParanNameModel rep = new ReportParanNameModel();

			ReportProdModel _repMod = new ReportProdModel();
			_repMod.codeReport = "IDGV1";
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

		#region AUXILIAR FUNCTIONS
		private RequestInventoryMoveEditForm AddUpdateDataFromForm(RequestInventoryMoveEditForm rqinMov)
		{
			RequestInventoryMove rimDB = new RequestInventoryMove();

			#region Add Section
			if (rqinMov.rqModel.idRIMModelP == 0)
			{

			}
			#endregion

			#region Update Section
			else
			{
				rimDB = db.RequestInventoryMove
								.Include("Document")
                                .Include("RequestInventoryMoveDetail")
								.FirstOrDefault(fod => fod.id == rqinMov.rqModel.idRIMModelP);

				#region UPDATE HEADER
				rimDB.id_Warehouse = rqinMov.rqModel.idWarehouseModelP;
				rimDB.id_NatureMove = rqinMov.rqModel.idNatureMoveModelP;
				rimDB.id_PersonRequest = rqinMov.rqModel.idPersonRModelP;
				rimDB.Document.emissionDate = rqinMov.rqModel.documentModelP.emissionDateModelP;
				#endregion

				#region UPDATE DETAIL
				List<RequestInventoryMoveDetail> lstRimDetail = rimDB.RequestInventoryMoveDetail.ToList();
				List<RequestInventoryMoveDetailModelP> lstRimDetailModelP = rqinMov.rqModel.lstRequestInvDetail.ToList();

				foreach (var det in lstRimDetailModelP)
				{
					RequestInventoryMoveDetail rimd = lstRimDetail.FirstOrDefault(fod => fod.id_item == det.id_item);
					if (rimd != null)
					{
						rimd.quantityRequest = det.quantityRequest;
						rimd.quantityUpdate = det.quantityUpdate;
						rimd.isActive = det.isActive;
						db.Entry(rimd).State = EntityState.Modified;
					}
					else
					{
						rimd = new RequestInventoryMoveDetail
						{
							id_item = det.id_item,
							quantityRequest = det.quantityRequest,
							quantityUpdate = det.quantityUpdate,
							isActive = det.isActive
						};
						lstRimDetail.Add(rimd);
					}
				}
				rimDB.RequestInventoryMoveDetail = lstRimDetail;
				#endregion


				#region UPDATE TRANSACTION
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						//db.RequestInventoryMove.Attach(rimDB);
						db.Entry(rimDB).State = EntityState.Modified;

						db.SaveChanges();
						trans.Commit();
					}
					catch(Exception ex)
					{
                        trans.Rollback();
                        FullLog(ex);
						throw new ProdHandlerException(GenericError.ErrorGeneralSaveRequerimientoInventario);
					}

				}
				#endregion

			}
			#endregion

			return ServiceRequestInventoryMove.GetRequestInventoryMove(db, rimDB.id);
		}

		private void RefreshComboBoxFilterForm()
		{

			ViewData["_NatureMoveFilter"] = DataProviderRequestInventoryMove.GetNatureMove();

			ViewData["Estados"] = db.tbsysDocumentTypeDocumentState
				.Where(d => d.DocumentType.code.Equals("79"))
				.Select(s => new SelectListItem
				{
					Text = s.DocumentState.name,
					Value = s.id_DocumenteState.ToString()
				}).Distinct().ToList();
			ViewData["EstadoPredeterminado"] = "1";

			var lstWarehouses = DataProviderRequestInventoryMove.GetWarehouse() as List<WarehouseRIMFilter>;

			#region Filtra Bodegas
			var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
				if (entityPermissions != null)
				{
					var tempModel = new List<WarehouseRIMFilter>();
					foreach (var item in lstWarehouses)
					{
						var warehousePermission = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.idWarehouseF
														&& fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null);
						if (warehousePermission != null)
						{
							tempModel.Add(item);
						}
					}

					lstWarehouses = tempModel;
				}
			}
			#endregion

			ViewData["_WarehouseFilter"] = lstWarehouses;
			ViewData["_PersonRequestFilter"] = DataProviderRequestInventoryMove.GetPersonRequest();
		}

		private void RefresshDataForEditForm(RequestInventoryMoveEditForm rqInvMov = null)
		{
			ViewData["_NatureMoveEdit"] = DataProviderRequestInventoryMove.GetNatureMove();
			ViewData["_WarehouseEdit"] = DataProviderRequestInventoryMove.GetWarehouse();
			ViewData["_PersonRequestEdit"] = DataProviderRequestInventoryMove.GetPersonRequest();
			ViewData["_ItemsDetailEdit"] = DataProviderItem.GetListItemModelP();
		}

		private void SaveDataEditForm(RequestInventoryMoveEditForm rimEf)
		{
			RefresshDataForEditForm();
			if (rimEf != null)
			{
				TempData["_reqInvMovEditForm"] = rimEf;
				TempData["_ReadOnlyBeRemissionGuide"] = rimEf.rqTransfer.ReadOnlyBeRemissionGuide ? "Y" : "N";
				TempData["_codeStateDocumentRim"] = rimEf?.rqTransfer?.documentRequestTransferP?.codeDocumentStateTransferP ?? "";

				TempData.Keep("_reqInvMovEditForm");
				TempData.Keep("_ReadOnlyBeRemissionGuide");
				TempData.Keep("_codeStateDocumentRim");
			}
			if (TempData["_reqInvMovEditForm"] != null)
			{
				TempData.Keep("_reqInvMovEditForm");
			}
			if (TempData["_ReadOnlyBeRemissionGuide"] != null)
			{
				TempData.Keep("_ReadOnlyBeRemissionGuide");
			}
			if (TempData["_codeStateDocumentRim"] != null)
			{
				TempData.Keep("_codeStateDocumentRim");
			}
		}

		private RequestInventoryMoveEditForm GetTempDataEditForm()
		{
			if (TempData["_reqInvMovEditForm"] != null)
			{
				return (TempData["_reqInvMovEditForm"] as RequestInventoryMoveEditForm);
			}
			return new RequestInventoryMoveEditForm();
		}

		public AnswerValidateForm ValidateDataFormForUpdate(RequestInventoryMoveTransferPUpdateEntity _headerRim)
		{
			string _message = "";
			string _hasError = "N";

			#region Validate Header
			if (_headerRim != null)
			{
				if (_headerRim.yEmissionDate == null && _headerRim.yEmissionDate <= 1900)
				{
					_message += "La fecha de Emisión es obligatoria. ";
					_hasError = "Y";
				}
				if (_headerRim.id_PersonRequest == null)
				{
					_message += "La persona que requiere es obligatoria. ";
					_hasError = "Y";
				}
				if (_headerRim.id_Warehouse == null)
				{
					_message += "La bodega es obligatoria. ";
					_hasError = "Y";
				}
				if (_headerRim.id_NatureMove == null)
				{
					_message += "La Naturaleza de Movimiento obligatoria. ";
					_hasError = "Y";
				}
			}
			#endregion

			#region Update TempData
			if (_headerRim != null)
			{
				RequestInventoryMoveEditForm rqTmp = GetTempDataEditForm();
				if (rqTmp != null)
				{
					RequestInventoryMoveEditForm rqInvMov = ServiceRequestInventoryMove.UpdateHeaderRequestInventoryMoveEditForm(db, _headerRim, rqTmp);
					SaveDataEditForm(rqInvMov);
				}
			}
			#endregion

			return new AnswerValidateForm
			{
				hasError = _hasError,
				message = (!String.IsNullOrEmpty(_message)) ? ErrorMessage(_message) : ""
			};
		}

		#region Optimizacion | Aprobacion
		private void ApproveRequestInventoryMoveFromRemissionGuideOP(
			RequestInventoryMoveEditForm rimEf,
			int idRim,
			DocumentModelP[] lstRemissionGuideDocument,
			DocumentStateModelP documentStateModelP,
			RequestInventoryMove requestInventoryMove, // include Document, RequestInventoryMoveDetail
			DocumentState[] documentStates,
			Document document,
			int tbsysActionOnDocumentId,
			EmissionPoint[] emissionPoints,
			int id_remissionGuide,
			RemissionGuide remissionGuide,
			Setting[] settings,
			WarehouseLocation[] warehouseLocations,
			Provider providerAux,
			Warehouse[] warehouses,
            SettingDetail[] settingDetails,
			InventoryReason[] inventoryReasons,
			Employee employee,
			MetricUnitConversion[] metricUnitConversions)
        {
            //int id_remissionGuide = 0;
            int idDa = 0;
            int idUs = 0;

            #region Get RemissionGuide Information
            if (idRim > 0)
            {
                //var queryDocSour = DataProviderDocumentSource.QueryDocumentsSource(db, idRim);
                //var dtmp = DataProviderDocumentType.GetOneDocumentTypeByCode("08");
                //var queryDocuments = DataProviderDocument.QueryDocumentModelP(db, dtmp.idDocumentTypeModelP);
                //var lstRemissionGuideDocument = (from _lsDs in queryDocSour
                //                                 join _ltD in queryDocuments on _lsDs.id_Document_Origin_P equals _ltD.idDocumentModelP
                //                                 select new DocumentModelP
                //                                 {
                //                                     idDocumentModelP = _ltD.idDocumentModelP,
                //                                     numberModelP = _ltD.numberModelP,
                //                                     sequentialModelP = _ltD.sequentialModelP,
                //                                     emissionDateModelP = _ltD.emissionDateModelP,
                //                                     authorizationDateModelP = _ltD.authorizationDateModelP,
                //                                     authorizationNumberModelP = _ltD.authorizationNumberModelP,
                //                                     accessKeyModelP = _ltD.accessKeyModelP,
                //                                     descriptionModelP = _ltD.descriptionModelP,
                //                                     referenceModelP = _ltD.referenceModelP,
                //                                     idEmissionPointModelP = _ltD.idEmissionPointModelP,
                //                                     idDocumentTypeModelP = _ltD.idDocumentTypeModelP,
                //                                     idDocumentStateModelP = _ltD.idDocumentStateModelP,
                //                                     isOpenModelP = _ltD.isOpenModelP
                //                                 }).ToList();

                //if (lstRemissionGuideDocument != null && lstRemissionGuideDocument.Count() > 0)
                //{
                //    int idDocStateNulled = documentStateModelP?.idDocumentStateModelP ?? 0;
                //    DocumentModelP dmp = lstRemissionGuideDocument.Where(w => w.idDocumentStateModelP != idDocStateNulled).FirstOrDefault();
                //    id_remissionGuide = dmp?.idDocumentModelP ?? 0;
                //}
            }
            #endregion

            #region Update Data From Model
            RequestInventoryMove rimDB = new RequestInventoryMove();
			//rimDB = db.RequestInventoryMove.FirstOrDefault(fod => fod.id == rimEf.rqModel.idRIMModelP);
			rimDB = requestInventoryMove;

            #region UPDATE HEADER
            rimDB.id_Warehouse = rimEf.rqModel.idWarehouseModelP;
            rimDB.id_NatureMove = rimEf.rqModel.idNatureMoveModelP;
            rimDB.id_PersonRequest = rimEf.rqModel.idPersonRModelP;
            #endregion

            #region UPDATE DETAIL
            List<RequestInventoryMoveDetail> lstRimDetail = rimDB.RequestInventoryMoveDetail.ToList();
            List<RequestInventoryMoveDetailModelP> lstRimDetailModelP = rimEf.rqModel.lstRequestInvDetail.ToList();

            foreach (var det in lstRimDetailModelP)
            {
                RequestInventoryMoveDetail rimd = lstRimDetail.FirstOrDefault(fod => fod.id_item == det.id_item);
                if (rimd != null)
                {
                    rimd.quantityRequest = det.quantityRequest;
                    rimd.quantityUpdate = det.quantityUpdate;
                    rimd.isActive = det.isActive;
                    db.Entry(rimd).State = EntityState.Modified;
                }
                else
                {
                    rimd = new RequestInventoryMoveDetail
                    {
                        id_item = det.id_item,
                        quantityRequest = det.quantityRequest,
                        quantityUpdate = det.quantityUpdate,
                        isActive = det.isActive
                    };
                    lstRimDetail.Add(rimd);
                }
            }
            rimDB.RequestInventoryMoveDetail = lstRimDetail;
            #endregion

            #endregion

            #region Change Document State
            int idDs = documentStates.FirstOrDefault(fod => fod.code == "03")?.id ?? 0;
			//Document _do = db.Document.FirstOrDefault(fod => fod.id == idRim);
			Document _do = document;
            _do.id_documentState = idDs;

            //db.Document.Attach(_do);
            db.Entry(_do).State = EntityState.Modified;
			#endregion

			#region Document Track State
			idDa = tbsysActionOnDocumentId; //db.tbsysActionOnDocument.FirstOrDefault(fod => fod.code.Equals("AD")).id;
            idUs = ActiveUser.id;
            ServiceDocument.SaveTrackState(_do, idDa, idUs, db);
            #endregion

            #region Execute Service Inventory Move
            if (id_remissionGuide > 0)
            {
				EmissionPoint ep = emissionPoints.FirstOrDefault(fod => fod.id == ActiveEmissionPoint.id);  //db.EmissionPoint.FirstOrDefault(fod => fod.id == ActiveEmissionPoint.id);

				RemissionGuide rg = remissionGuide; //db.RemissionGuide
													//    .FirstOrDefault(fod => fod.id == id_remissionGuide);

                var inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == idRim && w.Document.DocumentState.code.Equals("05") &&
                                                                  (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDL") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
                InventoryMove lastInventoryMoveEPTAMDL = (inventoryMoveDetailAux.Count > 0)
                                                    ? inventoryMoveDetailAux.FirstOrDefault().Document.InventoryMove
                                                    : null;
                inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == idRim && w.Document.DocumentState.code.Equals("05") &&
                                                                     (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDL") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();

                InventoryMove lastInventoryMoveIPTAMDL = (inventoryMoveDetailAux.Count > 0)
                                                    ? inventoryMoveDetailAux.FirstOrDefault().Document.InventoryMove
                                                    : null;

				var result = ServiceInventoryMove.UpdateInventaryMoveTransferDispatchMaterialsLogisticFixedOP(
													ActiveUser,
													ActiveCompany,
													ep,
													rg,
													db,
													false,
													rimEf.rqModel.documentModelP.emissionDateModelP,
													settings: settings,
                                                    warehouseLocations: warehouseLocations,
													providerAux: providerAux,
													warehouses: warehouses,
													settingDetails: settingDetails,
													documentStates: documentStates,
													emissionPoints: emissionPoints,
                                                    inventoryReasons: inventoryReasons,
													employee: employee,
													metricUnitConversions: metricUnitConversions,
                                                    lastInventoryMoveEPTAMDL,
													lastInventoryMoveIPTAMDL,
													true,
													rimEf.rqModel.lstRequestInvDetail,
													idRim,
													rimEf.rqModel.idWarehouseModelP); ;

				if (!string.IsNullOrEmpty(result))
				{
					throw new ProdHandlerException(result);
				}

                #region Update Quantity Send in RemissionGuide
                if (id_remissionGuide > 0)
                {
                    foreach (var det in rimEf.rqModel.lstRequestInvDetail)
                    {
                        RemissionGuideDispatchMaterial rgTmp = rg.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.id_item == det.id_item);
                        if (rgTmp != null)
                        {
                            rgTmp.sendedDestinationQuantity = det.quantityUpdate ?? det.quantityRequest;
                            //db.RemissionGuideDispatchMaterial.Attach(rgTmp);
                            db.Entry(rgTmp).State = EntityState.Modified;
                        }
                    }
                }
                #endregion

                //db.RemissionGuide.Attach(rg);
                db.Entry(rg).State = EntityState.Modified;
            }
            #endregion
        }
        #endregion
        private void ApproveRequestInventoryMoveFromRemissionGuide(RequestInventoryMoveEditForm rimEf, int idRim)
		{
			int id_remissionGuide = 0;
			int idDa = 0;
			int idUs = 0;

			#region Get RemissionGuide Information
			if (idRim > 0)
			{
				var queryDocSour = DataProviderDocumentSource.QueryDocumentsSource(db, idRim);
				var dtmp = DataProviderDocumentType.GetOneDocumentTypeByCode("08");
				var queryDocuments = DataProviderDocument.QueryDocumentModelP(db, dtmp.idDocumentTypeModelP);
				var lstRemissionGuideDocument = (from _lsDs in queryDocSour
												 join _ltD in queryDocuments on _lsDs.id_Document_Origin_P equals _ltD.idDocumentModelP
												 select new DocumentModelP
												 {
													 idDocumentModelP = _ltD.idDocumentModelP,
													 numberModelP = _ltD.numberModelP,
													 sequentialModelP = _ltD.sequentialModelP,
													 emissionDateModelP = _ltD.emissionDateModelP,
													 authorizationDateModelP = _ltD.authorizationDateModelP,
													 authorizationNumberModelP = _ltD.authorizationNumberModelP,
													 accessKeyModelP = _ltD.accessKeyModelP,
													 descriptionModelP = _ltD.descriptionModelP,
													 referenceModelP = _ltD.referenceModelP,
													 idEmissionPointModelP = _ltD.idEmissionPointModelP,
													 idDocumentTypeModelP = _ltD.idDocumentTypeModelP,
													 idDocumentStateModelP = _ltD.idDocumentStateModelP,
													 isOpenModelP = _ltD.isOpenModelP
												 }).ToList();

				if (lstRemissionGuideDocument != null && lstRemissionGuideDocument.Count() > 0)
				{
					int idDocStateNulled = DataProviderDocumentState.GetDocumentStateByCode("05")?.idDocumentStateModelP ?? 0;
					DocumentModelP dmp = lstRemissionGuideDocument.Where(w => w.idDocumentStateModelP != idDocStateNulled).FirstOrDefault();
					id_remissionGuide = dmp?.idDocumentModelP ?? 0;
				}
			}
			#endregion

			#region Update Data From Model
			RequestInventoryMove rimDB = new RequestInventoryMove();
			rimDB = db.RequestInventoryMove.FirstOrDefault(fod => fod.id == rimEf.rqModel.idRIMModelP);

			#region UPDATE HEADER
			rimDB.id_Warehouse = rimEf.rqModel.idWarehouseModelP;
			rimDB.id_NatureMove = rimEf.rqModel.idNatureMoveModelP;
			rimDB.id_PersonRequest = rimEf.rqModel.idPersonRModelP;
			#endregion

			#region UPDATE DETAIL
			List<RequestInventoryMoveDetail> lstRimDetail = rimDB.RequestInventoryMoveDetail.ToList();
			List<RequestInventoryMoveDetailModelP> lstRimDetailModelP = rimEf.rqModel.lstRequestInvDetail.ToList();

			foreach (var det in lstRimDetailModelP)
			{
				RequestInventoryMoveDetail rimd = lstRimDetail.FirstOrDefault(fod => fod.id_item == det.id_item);
				if (rimd != null)
				{
					rimd.quantityRequest = det.quantityRequest;
					rimd.quantityUpdate = det.quantityUpdate;
					rimd.isActive = det.isActive;
					db.Entry(rimd).State = EntityState.Modified;
				}
				else
				{
					rimd = new RequestInventoryMoveDetail
					{
						id_item = det.id_item,
						quantityRequest = det.quantityRequest,
						quantityUpdate = det.quantityUpdate,
						isActive = det.isActive
					};
					lstRimDetail.Add(rimd);
				}
			}
			rimDB.RequestInventoryMoveDetail = lstRimDetail;
			#endregion

			#endregion

			#region Change Document State
			int idDs = db.DocumentState.FirstOrDefault(fod => fod.code == "03")?.id ?? 0;
			Document _do = db.Document.FirstOrDefault(fod => fod.id == idRim);
			_do.id_documentState = idDs;

			db.Document.Attach(_do);
			db.Entry(_do).State = EntityState.Modified;
			#endregion

			#region Document Track State
			idDa = db.tbsysActionOnDocument.FirstOrDefault(fod => fod.code.Equals("AD")).id;
			idUs = ActiveUser.id;
			ServiceDocument.SaveTrackState(_do, idDa, idUs, db);
			#endregion

			#region Execute Service Inventory Move
			if (id_remissionGuide > 0)
			{
				EmissionPoint ep = db.EmissionPoint.FirstOrDefault(fod => fod.id == ActiveEmissionPoint.id);

				RemissionGuide rg = db.RemissionGuide
										.FirstOrDefault(fod => fod.id == id_remissionGuide);

				var inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == idRim && w.Document.DocumentState.code.Equals("05") &&
																  (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDL") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
				InventoryMove lastInventoryMoveEPTAMDL = (inventoryMoveDetailAux.Count > 0)
													? inventoryMoveDetailAux.FirstOrDefault().Document.InventoryMove
													: null;
				inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == idRim && w.Document.DocumentState.code.Equals("05") &&
																	 (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDL") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();

				InventoryMove lastInventoryMoveIPTAMDL = (inventoryMoveDetailAux.Count > 0)
													? inventoryMoveDetailAux.FirstOrDefault().Document.InventoryMove
													: null;

				ServiceInventoryMove.UpdateInventaryMoveTransferDispatchMaterialsLogisticFixed(ActiveUser
						, ActiveCompany, ep, rg, db, false, rimEf.rqModel.documentModelP.emissionDateModelP
						, lastInventoryMoveEPTAMDL, lastInventoryMoveIPTAMDL, true
						, rimEf.rqModel.lstRequestInvDetail, idRim, rimEf.rqModel.idWarehouseModelP);

				#region Update Quantity Send in RemissionGuide
				if (id_remissionGuide > 0)
				{
					foreach (var det in rimEf.rqModel.lstRequestInvDetail)
					{
						RemissionGuideDispatchMaterial rgTmp = rg.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.id_item == det.id_item);
						if (rgTmp != null)
						{
							rgTmp.sendedDestinationQuantity = det.quantityUpdate ?? det.quantityRequest;
							db.RemissionGuideDispatchMaterial.Attach(rgTmp);
							db.Entry(rgTmp).State = EntityState.Modified;
						}
					}
				}
				#endregion

				db.RemissionGuide.Attach(rg);
				db.Entry(rg).State = EntityState.Modified;
			}
			#endregion
		}

		private void RevertRequestInventoryMoveFromRemissionGuide(RequestInventoryMoveEditForm rimEf, int idRim)
		{
			int id_remissionGuide = 0;
			int idDa = 0;
			int idUs = 0;

			#region Get RemissionGuide Information
			if (idRim > 0)
			{
				var queryDocSour = DataProviderDocumentSource.QueryDocumentsSource(db, idRim);
				var dtmp = DataProviderDocumentType.GetOneDocumentTypeByCode("08");
				var queryDocuments = DataProviderDocument.QueryDocumentModelP(db, dtmp.idDocumentTypeModelP);
				var lstRemissionGuideDocument =
											(from _lsDs in queryDocSour
											 join _ltD in queryDocuments on _lsDs.id_Document_Origin_P equals _ltD.idDocumentModelP
											 select new DocumentModelP
											 {
												 idDocumentModelP = _ltD.idDocumentModelP,
												 numberModelP = _ltD.numberModelP,
												 sequentialModelP = _ltD.sequentialModelP,
												 emissionDateModelP = _ltD.emissionDateModelP,
												 authorizationDateModelP = _ltD.authorizationDateModelP,
												 authorizationNumberModelP = _ltD.authorizationNumberModelP,
												 accessKeyModelP = _ltD.accessKeyModelP,
												 descriptionModelP = _ltD.descriptionModelP,
												 referenceModelP = _ltD.referenceModelP,
												 idEmissionPointModelP = _ltD.idEmissionPointModelP,
												 idDocumentTypeModelP = _ltD.idDocumentTypeModelP,
												 idDocumentStateModelP = _ltD.idDocumentStateModelP,
												 isOpenModelP = _ltD.isOpenModelP
											 }).ToList();

				if (lstRemissionGuideDocument != null && lstRemissionGuideDocument.Count() > 0)
				{
					int idDocStateNulled = DataProviderDocumentState.GetDocumentStateByCode("05")?.idDocumentStateModelP ?? 0;
					DocumentModelP dmp = lstRemissionGuideDocument.Where(w => w.idDocumentStateModelP != idDocStateNulled).FirstOrDefault();
					id_remissionGuide = dmp?.idDocumentModelP ?? 0;
				}
			}
			#endregion

			#region Validation Information
			if (1 == 0)
			{
				throw new Exception("No se puede Reversar por el Motivo que 1 no puede ser igual 0.");
			}
			#endregion

			#region Execute Service Inventory Move
			if (id_remissionGuide > 0)
			{
				EmissionPoint ep = db.EmissionPoint.FirstOrDefault(fod => fod.id == ActiveEmissionPoint.id);

				RemissionGuide rg = db.RemissionGuide
										.FirstOrDefault(fod => fod.id == id_remissionGuide);

				var inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == idRim && !w.Document.DocumentState.code.Equals("05") &&
																				  (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("EPTAMDL") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
				InventoryMove lastInventoryMoveEPTAMDL = (inventoryMoveDetailAux.Count > 0)
													? inventoryMoveDetailAux.FirstOrDefault().Document.InventoryMove
													: null;
				inventoryMoveDetailAux = db.DocumentSource.Where(w => w.id_documentOrigin == idRim && !w.Document.DocumentState.code.Equals("05") &&
																	 (w.Document.InventoryMove != null ? w.Document.InventoryMove.InventoryReason.code.Equals("IPTAMDL") : false)).OrderByDescending(d => d.Document.emissionDate).ToList();
				InventoryMove lastInventoryMoveIPTAMDL = (inventoryMoveDetailAux.Count > 0)
													? inventoryMoveDetailAux.FirstOrDefault().Document.InventoryMove
													: null;

				int idNullState = db.DocumentState.FirstOrDefault(fod => fod.code == "05")?.id ?? 0;
				if (lastInventoryMoveEPTAMDL != null)
				{
					Document _doEPTAMDL = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveEPTAMDL.id);
					_doEPTAMDL.id_documentState = idNullState;
					db.Document.Attach(_doEPTAMDL);
					db.Entry(_doEPTAMDL).State = EntityState.Modified;

					var lstDetailInvExit = lastInventoryMoveEPTAMDL?.InventoryMoveDetail.ToList();
					foreach (var det in lstDetailInvExit)
					{
						ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
							det.id_warehouse, det.id_warehouseLocation, det.exitAmount, db);
					}
				}

				if (lastInventoryMoveIPTAMDL != null)
				{
					Document _doIPTAMDL = db.Document.FirstOrDefault(fod => fod.id == lastInventoryMoveIPTAMDL.id);
					_doIPTAMDL.id_documentState = idNullState;
					db.Document.Attach(_doIPTAMDL);
					db.Entry(_doIPTAMDL).State = EntityState.Modified;

					var lstDetailInvEntry = lastInventoryMoveIPTAMDL?.InventoryMoveDetail.ToList();
					foreach (var det in lstDetailInvEntry)
					{
						ServiceInventoryMove.UpdateStockInventoryItem(det.id_item,
							det.id_warehouse, det.id_warehouseLocation, -det.entryAmount, db);
					}
				}

				#region Update Quantity Send in RemissionGuide
				if (id_remissionGuide > 0)
				{
					foreach (var det in rimEf.rqModel.lstRequestInvDetail)
					{
						RemissionGuideDispatchMaterial rgTmp = rg.RemissionGuideDispatchMaterial.FirstOrDefault(fod => fod.id_item == det.id_item);
						if (rgTmp != null)
						{
							rgTmp.sendedDestinationQuantity = 0;
							db.RemissionGuideDispatchMaterial.Attach(rgTmp);
							db.Entry(rgTmp).State = EntityState.Modified;
						}
					}
				}
				#endregion

				db.RemissionGuide.Attach(rg);
				db.Entry(rg).State = EntityState.Modified;
			}

			#endregion

			#region Change Document State
			int idDs = db.DocumentState.FirstOrDefault(fod => fod.code == "01")?.id ?? 0;
			Document _do = db.Document.FirstOrDefault(fod => fod.id == idRim);
			_do.id_documentState = idDs;
			#endregion

			#region Document TrackState
			idDa = db.tbsysActionOnDocument.FirstOrDefault(fod => fod.code.Equals("RD")).id;
			idUs = ActiveUser.id;
			ServiceDocument.SaveTrackState(_do, idDa, idUs, db);
			#endregion

			



			db.Document.Attach(_do);
			db.Entry(_do).State = EntityState.Modified;

		}

		private DataSet GetRequerimentMoveInventoryDataset(RequestInventoryMoveEditForm rqInmo, string nameDt)
		{
			DataSet ds = new DataSet();
			if (rqInmo != null && rqInmo.rqTransfer != null)
			{
				DataTable dt = new DataTable(nameDt);
				dt.Clear();

				#region Inserto Columnas
				DataColumn dtc1 = new DataColumn("idRequerimientoMovimiento");
				dtc1.DataType = System.Type.GetType("System.Int32");
				dt.Columns.Add(dtc1);
				DataColumn dtc2 = new DataColumn("naturalezaMovimiento");
				dtc2.DataType = System.Type.GetType("System.String");
				dt.Columns.Add(dtc2);
				DataColumn dtc3 = new DataColumn("numeroRequerimiento");
				dtc3.DataType = System.Type.GetType("System.String");
				dt.Columns.Add(dtc3);
				DataColumn dtc4 = new DataColumn("estado");
				dtc4.DataType = System.Type.GetType("System.String");
				dt.Columns.Add(dtc4);
				DataColumn dtc5 = new DataColumn("fechaEmision");
				dtc5.DataType = System.Type.GetType("System.DateTime");
				dt.Columns.Add(dtc5);
				DataColumn dtc6 = new DataColumn("nombreBodega");
				dtc6.DataType = System.Type.GetType("System.String");
				dt.Columns.Add(dtc6);
				DataColumn dtc8 = new DataColumn("tipoDocumento");
				dtc8.DataType = System.Type.GetType("System.String");
				dt.Columns.Add(dtc8);
				DataColumn dtc9 = new DataColumn("fechaEmisionLogistica");
				dtc9.DataType = System.Type.GetType("System.DateTime");
				dt.Columns.Add(dtc9);
				DataColumn dtc10 = new DataColumn("personaRequiere");
				dtc10.DataType = System.Type.GetType("System.String");
				dt.Columns.Add(dtc10);
				DataColumn dtc11 = new DataColumn("numeroGuia");
				dtc11.DataType = System.Type.GetType("System.String");
				dt.Columns.Add(dtc11);
				DataColumn dtc12 = new DataColumn("fechaGuia");
				dtc12.DataType = System.Type.GetType("System.DateTime");
				dt.Columns.Add(dtc12);
				DataColumn dtc13 = new DataColumn("numeroRequisicion");
				dtc13.DataType = System.Type.GetType("System.String");
				dt.Columns.Add(dtc13);
				DataColumn dtc14 = new DataColumn("codigoProducto");
				dtc14.DataType = System.Type.GetType("System.String");
				dt.Columns.Add(dtc14);
				DataColumn dtc15 = new DataColumn("nombreProducto");
				dtc15.DataType = System.Type.GetType("System.String");
				dt.Columns.Add(dtc15);
				DataColumn dtc16 = new DataColumn("medida");
				dtc16.DataType = System.Type.GetType("System.String");
				dt.Columns.Add(dtc16);
				DataColumn dtc17 = new DataColumn("ubicacion");
				dtc17.DataType = System.Type.GetType("System.String");
				dt.Columns.Add(dtc17);
				DataColumn dtc18 = new DataColumn("cantidadRequerida");
				dtc18.DataType = System.Type.GetType("System.Decimal");
				dt.Columns.Add(dtc18);
				DataColumn dtc19 = new DataColumn("cantidadEntregada");
				dtc19.DataType = System.Type.GetType("System.Decimal");
				dt.Columns.Add(dtc19);
				#endregion

				#region Inserto Filas
				foreach (var det in rqInmo.rqTransfer.lstRequestInvDetail)
				{
					DataRow dr = dt.NewRow();
					dr["idRequerimientoMovimiento"] = rqInmo.rqTransfer.idRIMTransferP;
					dr["naturalezaMovimiento"] = rqInmo.rqTransfer.nameNatureMoveTransferP;
					dr["numeroRequerimiento"] = rqInmo.rqTransfer.numberRequerimentTransferP;
					dr["estado"] = rqInmo.rqTransfer.documentRequestTransferP.nameDocumentStateTransferP;
					dr["fechaEmision"] = rqInmo.rqTransfer.documentRequestTransferP.emissionDateTransferP;
					dr["nombreBodega"] = rqInmo.rqTransfer.nameWarehouseTransferP;
					dr["tipoDocumento"] = rqInmo.rqTransfer.documentRequestTransferP.nameDocumentTypeTransferP;
					dr["fechaEmisionLogistica"] = rqInmo.rqTransfer.emissionDateHeadPOrigin;
					dr["personaRequiere"] = rqInmo.rqTransfer.namePersonRTransferp;
					dr["numeroGuia"] = rqInmo.rqTransfer.sequential_remissionGuide;
					dr["fechaGuia"] = rqInmo.rqTransfer.emissionDateRemissionGuide;
					dr["numeroRequisicion"] = rqInmo.rqTransfer.sequentialWarehouseRequisitionModelHeadP;
					dr["codigoProducto"] = det.master_code_item;
					dr["nombreProducto"] = det.name_item;
					dr["medida"] = det.nameMetricUnit;
					dr["ubicacion"] = det.nameWarehouseLocation;
					dr["cantidadRequerida"] = det.quantityRequest;
					dr["cantidadEntregada"] = det.quantityUpdate;
					dt.Rows.Add(dr);
				}
				#endregion

				ds.Tables.Add(dt);

			}
			return ds;
		}

		private IEnumerable FilterWarehouseResult(ref List<ResultRequestInventoryMoveAll> lstAllRequestInventoryMove)
		{
			var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
				if (entityPermissions != null)
				{
					var tempModel = new List<ResultRequestInventoryMoveAll>();
					foreach (var item in lstAllRequestInventoryMove)
					{
						var warehousePermission = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.idWarehouseR
														&& fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null);
						if (warehousePermission != null)
						{
							tempModel.Add(item);
						}
					}

					lstAllRequestInventoryMove = tempModel;
				}
			}
			return lstAllRequestInventoryMove;
		}

		private bool FilterWarehouseResult(RequestInventoryMoveEditForm rqInvMov, string nameAction = "Editar")
		{
			bool _respuesta = false;

			var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "WAH");
				if (entityPermissions != null)
				{
					_respuesta = (entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == rqInvMov.rqModel.idWarehouseModelP
														&& fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == nameAction) != null) != null);
				}
			}
			return _respuesta;

		}
		#endregion
	}
}
