using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.AdvanceParametersDetailP.AdvanceParametersDetailModels;
using DXPANACEASOFT.Models.DocumentP.DocumentDTO;
using DXPANACEASOFT.Models.DocumentP.DocumentModels;
using DXPANACEASOFT.Models.DocumentSourceP.DocumentsourceModels;
using DXPANACEASOFT.Models.General;
using DXPANACEASOFT.Models.InventoryMoveP.InventoryMoveModel;
using DXPANACEASOFT.Models.ItemP.ItemModel;
using DXPANACEASOFT.Models.RequestInventoryMoveDTO;
using DXPANACEASOFT.Models.RequestInventoryMoveModel;
using DXPANACEASOFT.Models.WarehouseP.WarehouseModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DXPANACEASOFT.Services
{
	public class ServiceRequestInventoryMove
	{
		#region QUERY REQUEST INVENTORY MOVE RESULT
		public static List<ResultRequestInventoryMoveAll> GetAllRequestInventoryMove(DBContext db, FilterQueryRequestInventoryMove filter)
		{
			//This is going to be changed by a Compiled Function
			#region SELECT DATA

			var queryRequestInventoryMove = DataProviderRequestInventoryMove.QueryRequestInventoryMoveModelP(db);

			var idDocumentType = db.DocumentType.FirstOrDefault(fod => fod.code.Equals("79"))?.id ?? 0;
			var queryDocuments = DataProviderDocument.QueryDocumentModelP(db, idDocumentType);

			var queryPersonsInformation = DataProviderPerson.QueryPersonBasicModelInformation(db);

			var queryDocumentStates = DataProviderDocumentState.QueryDocumentStatesByDocumentType(db, "79");

			var apModel = DataProviderAdvanceParametersDetail.GetAdvanceParameterModelPByCode("NMMGI");
			var queryNatureMove = DataProviderAdvanceParametersDetail.QueryAdvanceParametersModelPById(db, apModel?.idAdvanceModelP ?? 0);

			var queryWarehouseInformation = DataProviderWarehouse.QueryWarehouseModelP(db);

			#endregion

			#region GET FINAL COLLECTION

			var queryReqInventoryMoveAll = (from _rim in queryRequestInventoryMove
											join _do in queryDocuments on _rim.idRIMModelP equals _do.idDocumentModelP
											join _pe in queryPersonsInformation on _rim.idPersonRModelP equals _pe.idPersonP
											join _dost in queryDocumentStates on _do.idDocumentStateModelP equals _dost.idDocumentStateModelP
											join _nm in queryNatureMove on _rim.idNatureMoveModelP equals _nm.idAdvanceDetailModelP
											join _wa in queryWarehouseInformation on _rim.idWarehouseModelP equals _wa.idWarehouseModelP into _waps
											from _wa in _waps.DefaultIfEmpty()
											select new ResultRequestInventoryMoveAll
											{
												id = _rim.idRIMModelP,
												dateEmissionR = _do.emissionDateModelP,
												sequentialR = _do.sequentialModelP,
												nameSequentialR = _do.sequentialModelP.ToString(),
												idWarehouseR = _rim.idWarehouseModelP,
												descWarehouseR = (_wa == null) ? "SIN BODEGA" : _wa.nameWarehouseModelP,
												idPersonRequestR = _rim.idPersonRModelP,
												namePersonRequestR = _pe.fullNamePersonP,
												idDocumentStateR = _do.idDocumentStateModelP,
												nameDocumentStateR = _dost.descDocumentStateModelP,
												idNatureMoveR = _rim.idNatureMoveModelP,
												nameNatureMoveR = _nm.nameAdvanceDetailModelP
											});

			#region FILTROS
			if (filter != null)
			{
				if (filter.idNatureMove != null && filter.idNatureMove > 0)
				{
					queryReqInventoryMoveAll = queryReqInventoryMoveAll.Where(w => w.idNatureMoveR == filter.idNatureMove);
				}
				if (filter.idDocumentState != null && filter.idDocumentState > 0)
				{
					queryReqInventoryMoveAll = queryReqInventoryMoveAll.Where(w => w.idDocumentStateR == filter.idDocumentState);
				}
				if (filter.idPersonRequest != null && filter.idPersonRequest > 0)
				{
					queryReqInventoryMoveAll = queryReqInventoryMoveAll.Where(w => w.idPersonRequestR == filter.idPersonRequest);
				}
				if (filter.idWarehouse != null && filter.idWarehouse > 0)
				{
					queryReqInventoryMoveAll = queryReqInventoryMoveAll.Where(w => w.idWarehouseR == filter.idWarehouse);
				}
				if (filter.startEmissionDate != null)
				{
					var filterValue = filter.startEmissionDate.Value.Date;
					queryReqInventoryMoveAll = queryReqInventoryMoveAll.Where(w => w.dateEmissionR >= filterValue);
				}
				if (filter.endEmissionDate != null)
				{
					var filterValue = filter.endEmissionDate.Value.Date.AddDays(1).AddSeconds(-1);
					queryReqInventoryMoveAll = queryReqInventoryMoveAll.Where(w => w.dateEmissionR <= filterValue);
				}
			}
			#endregion

			#endregion

			#region GET REMISSION GUIDE INFORMATION

			var queryDocSource = DataProviderDocumentSource.QueryDocumentsSource(db);
			var idDocumentTypeRemissionGuide = db.DocumentType.FirstOrDefault(fod => fod.code.Equals("08"))?.id ?? 0;
			var queryDocumentsRGs = DataProviderDocument.QueryDocumentModelP(db, idDocumentTypeRemissionGuide);
			var lstRimRgInformation = (from _Dds in queryDocSource
									   join _Drg in queryDocumentsRGs on _Dds.id_Document_Origin_P equals _Drg.idDocumentModelP
									   select new DocumentDocumentOriginInformationModelP
									   {
										   idDocumentModelP = _Dds.id_Document_P,
										   idDocumentOriginModelP = _Drg.idDocumentModelP,
										   sequentialDocumentOriginModelP = _Drg.sequentialModelP.ToString(),
									   }).ToList();

			var lstReqInventoryMoveAll = queryReqInventoryMoveAll.ToList();
			lstReqInventoryMoveAll = (from _rim in lstReqInventoryMoveAll
									  join _Dds in lstRimRgInformation on _rim.id equals _Dds.idDocumentModelP into _rimDs
									  from _Dds in _rimDs.DefaultIfEmpty()
									  select new ResultRequestInventoryMoveAll
									  {
										  id = _rim.id,
										  dateEmissionR = _rim.dateEmissionR,
										  sequentialR = _rim.sequentialR,
										  nameSequentialR = _rim.nameSequentialR,
										  idWarehouseR = _rim.idWarehouseR,
										  descWarehouseR = _rim.descWarehouseR,
										  idPersonRequestR = _rim.idPersonRequestR,
										  namePersonRequestR = _rim.namePersonRequestR,
										  idDocumentStateR = _rim.idDocumentStateR,
										  nameDocumentStateR = _rim.nameDocumentStateR,
										  idNatureMoveR = _rim.idNatureMoveR,
										  nameNatureMoveR = _rim.nameNatureMoveR,
										  idRemGuiDispatchMaterial = (_Dds == null) ? 0 : _Dds.idDocumentOriginModelP,
										  sequentialRemissionGuide = (_Dds == null) ? "" : _Dds.sequentialDocumentOriginModelP,
									  }).ToList();

			var lstDisMat = DataProviderRemissionGuide.QueryAllDispatchMaterial(db).ToList();
			lstReqInventoryMoveAll = (from _rim in lstReqInventoryMoveAll
									  join _dm in lstDisMat
									  on new { a = (int?)_rim.idRemGuiDispatchMaterial, b = (int?)_rim.idWarehouseR }
									  equals new { a = (int?)_dm.idRemGuideDispatchMaterialSequentialModelP, b = (int?)_dm.idWareDispatchMaterialSequentialModelP } into lj
									  from lj2 in lj.DefaultIfEmpty()
									  select new ResultRequestInventoryMoveAll
									  {
										  id = _rim.id,
										  dateEmissionR = _rim.dateEmissionR,
										  sequentialR = _rim.sequentialR,
										  nameSequentialR = _rim.nameSequentialR,
										  idWarehouseR = _rim.idWarehouseR,
										  descWarehouseR = _rim.descWarehouseR,
										  idPersonRequestR = _rim.idPersonRequestR,
										  namePersonRequestR = _rim.namePersonRequestR,
										  idDocumentStateR = _rim.idDocumentStateR,
										  nameDocumentStateR = _rim.nameDocumentStateR,
										  idNatureMoveR = _rim.idNatureMoveR,
										  nameNatureMoveR = _rim.nameNatureMoveR,
										  idRemGuiDispatchMaterial = _rim.idRemGuiDispatchMaterial,
										  sequentialRemissionGuide = _rim.sequentialRemissionGuide,
										  sequentialDispatchMaterial = lj2 == null ? "" : lj2.sequentialDispatchMaterialSequentialModelP
									  }).ToList();
			#endregion

			return lstReqInventoryMoveAll;
		}

		public static RequestInventoryMoveEditForm GetRequestInventoryMove(DBContext db, int idRim)
		{
			#region GET DATA INFORMATION
			RequestInventoryMoveModelP rqInMo = DataProviderRequestInventoryMove.GetRequestInventoryMove(idRim);
			#endregion

			#region TRANSFORM INFORMATION FROM MODEL TO TRANSFER OBJECT
			RequestInventoryMoveTransferP rqInMoTransfer = GetRequestInventoryMoveTransfer(db, rqInMo);
			#endregion

			return new RequestInventoryMoveEditForm { rqModel = rqInMo, rqTransfer = rqInMoTransfer };
		}

		private static RequestInventoryMoveTransferP GetRequestInventoryMoveTransfer(DBContext db, RequestInventoryMoveModelP rqInMo)
		{
			AdvanceParametersModelP apModel = DataProviderAdvanceParametersDetail.GetAdvanceParameterModelPByCode("NMMGI");
			var queryNatureMove = DataProviderAdvanceParametersDetail.QueryAdvanceParametersModelPById(db, apModel?.idAdvanceModelP ?? 0);
			var queryDocumentStates = DataProviderDocumentState.QueryDocumentStatesByDocumentType(db, "79");
			int idLp1 = DataProviderDocumentType.GetOneDocumentTypeByCode("18").idDocumentTypeModelP;
			int idLp2 = DataProviderDocumentType.GetOneDocumentTypeByCode("19").idDocumentTypeModelP;
			int idLp3 = DataProviderDocumentType.GetOneDocumentTypeByCode("20").idDocumentTypeModelP;
			int idLp4 = DataProviderDocumentType.GetOneDocumentTypeByCode("21").idDocumentTypeModelP;
			int[] lstExDocuments = { idLp1, idLp2, idLp3, idLp4 };


			var idItems = rqInMo.lstRequestInvDetail.Select(r => r.id_item).ToArray();
			var idWarehouseLocations = rqInMo.lstRequestInvDetail.Select(r => r.id_warehouseLocation).ToArray();

			var lstItemModelP = db.Item
				.Where(w => w.isActive && idItems.Contains(w.id))
				.Select(s => new ItemModelP()
				{
					idModelP = s.id,
					masterCodeModelP = s.masterCode,
					auxCodeModelP = s.auxCode,
					nameModelP = s.name,
					descriptionModelP = s.description
				}).ToList();

			var queryInven = db.ItemInventory
				.Where(w => idItems.Contains(w.id_item))
				.Select(s => new ItemInventoryModelP()
				{
					idItemModelP = s.id_item,
					idWaresHouseModelP = s.id_warehouse,
					idWarehouseLocationModelP = s.id_warehouseLocation,
					idMetricUnitInventoryModelP = s.id_metricUnitInventory
				});
			var queryMetricUnits = db.MetricUnit
				.Select(s => new MetricUnitModelP()
				{
					idMetricUnitModelP = s.id,
					codeMetricUnitModelP = s.code,
					nameMetricUnitModelP = s.name
				});
			var lstItemMetricUnit = (from _it in queryInven
									 join _me in queryMetricUnits on _it.idMetricUnitInventoryModelP equals _me.idMetricUnitModelP
									 select new
									 {
										 idItem = _it.idItemModelP,
										 codeMetricUnit = _me.codeMetricUnitModelP,
										 nameMetricUnit = _me.nameMetricUnitModelP
									 }).ToList();


			var lstWareLoca = db.WarehouseLocation
				.Where(w => idWarehouseLocations.Contains(w.id))
				.Select(s => new WarehouseLocationModelP()
				{
					idWarehouseLocationModelP = s.id,
					idWarehouseModelP = s.id_warehouse,
					codeWarehouseLocationModelP = s.code,
					nameWarehouseLocationModelP = s.name
				}).ToList();

			var queryDocumentsIm = db.Document
				.Where(w => !lstExDocuments.Contains(w.id_documentType))
				.Select(s => new DocumentModelP()
				{
					idDocumentModelP = s.id,
					numberModelP = s.number,
					sequentialModelP = s.sequential,
					emissionDateModelP = s.emissionDate,
					authorizationDateModelP = s.authorizationDate,
					authorizationNumberModelP = s.authorizationNumber,
					accessKeyModelP = s.accessKey,
					descriptionModelP = s.description,
					referenceModelP = s.reference,
					idEmissionPointModelP = s.id_emissionPoint,
					idDocumentTypeModelP = s.id_documentType,
					idDocumentStateModelP = s.id_documentState,
					isOpenModelP = s.isOpen
				});



			int idDocStateNulled = DataProviderDocumentState.GetDocumentStateByCode("05")?.idDocumentStateModelP ?? 0;
			int idDocPending = DataProviderDocumentState.GetDocumentStateByCode("01")?.idDocumentStateModelP ?? 0;
			int? warehouseDoc = rqInMo.idWarehouseModelP;


			string nameDocState = queryDocumentStates
									.FirstOrDefault(fod => fod.idDocumentStateModelP == rqInMo.documentModelP.idDocumentStateModelP)?
									.descDocumentStateModelP;

			string codeDocState = queryDocumentStates
									.FirstOrDefault(fod => fod.idDocumentStateModelP == rqInMo.documentModelP.idDocumentStateModelP)?
									.codeDocumentStateModelP;

			string nameNatureMove = queryNatureMove
										.FirstOrDefault(fod => fod.idAdvanceDetailModelP == rqInMo.idNatureMoveModelP)?
										.nameAdvanceDetailModelP;


			#region CABECERA
			RequestInventoryMoveTransferP rqTr = new RequestInventoryMoveTransferP();
			rqTr.idRIMTransferP = rqInMo.idRIMModelP;
			rqTr.idPersonRTransferP = rqInMo.idPersonRModelP;
			rqTr.namePersonRTransferp = DataProviderPerson.GetOnePersonBasicModelInformation(rqInMo.idPersonRModelP)?.fullNamePersonP;
			rqTr.idWarehouseTransferP = rqInMo.idWarehouseModelP;
			rqTr.nameWarehouseTransferP = DataProviderWarehouse.GetOneWarehouseModelP((int)rqInMo.idWarehouseModelP)?.nameWarehouseModelP;
			rqTr.idProviderTransferP = rqInMo.idProviderModelP;
			rqTr.nameProviderTransferP = "";
			rqTr.idCustomerTransferP = rqInMo.idCustomerModelP;
			rqTr.nameCustomerTransferP = "";
			rqTr.idNatureMoveTransferP = rqInMo.idNatureMoveModelP;
			rqTr.nameNatureMoveTransferP = nameNatureMove;
			rqTr.ReadOnlyBeRemissionGuide = false;
			rqTr.nameDocumenTypeHeadP = DataProviderDocumentType.GetOneDocumentType(rqInMo.documentModelP.idDocumentTypeModelP)?.nameModelP;
			rqTr.idDocumentTypeHeadP = rqInMo.documentModelP.idDocumentTypeModelP;
			rqTr.emissionDateHeadPOrigin = null;
			rqTr.emissionDateRemissionGuide = null;
			rqTr.sequentialInventoryMoveHeadP = "";
			rqTr.sequentialWarehouseRequisitionModelHeadP = "";
			#endregion

			#region FILL REMISSIONGUIDE INFORMATION
			if (rqInMo.idRIMModelP > 0)
			{
				var queryDocSour = DataProviderDocumentSource.QueryDocumentsSource(db, rqInMo.idRIMModelP);
				var dtmp = DataProviderDocumentType.GetOneDocumentTypeByCode("08");
				var queryDocuments = queryDocumentsIm
					.Where(w => w.idDocumentTypeModelP == dtmp.idDocumentTypeModelP);
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
					DocumentModelP dmp = lstRemissionGuideDocument.Where(w => w.idDocumentStateModelP != idDocStateNulled).FirstOrDefault();
					rqTr.id_remissionguide_origin = dmp?.idDocumentModelP;
					rqTr.sequential_remissionGuide = dmp?.sequentialModelP.ToString();
					rqTr.emissionDateRemissionGuide = dmp.emissionDateModelP;
					rqTr.sequentialWarehouseRequisitionModelHeadP = DataProviderRemissionGuide.QueryAllDispatchMaterial(db)
						.FirstOrDefault(fod => fod.idRemGuideDispatchMaterialSequentialModelP == rqTr.id_remissionguide_origin
									&& fod.idWareDispatchMaterialSequentialModelP == warehouseDoc)?
						.sequentialDispatchMaterialSequentialModelP;

					if (dmp != null)
						rqTr.nameProviderRGTransferP = db.RemissionGuide.FirstOrDefault(fod => fod.id == dmp.idDocumentModelP)?.Provider1?.Person?.fullname_businessName ?? "";
					#region VALIDATION BY CONTROLS BY REMISSION GUIDE
					rqTr.ReadOnlyBeRemissionGuide = true;
					#endregion


				}
			}
			#endregion

			#region DOCUMENTO

			DocumentTansferP dtp = new DocumentTansferP();
			dtp.idDocumentTransferP = rqInMo.documentModelP.idDocumentModelP;
			dtp.numberTransferP = rqInMo.documentModelP.numberModelP;
			dtp.sequentialTransferP = rqInMo.documentModelP.sequentialModelP;
			dtp.emissionDateTransferP = rqInMo.documentModelP.emissionDateModelP;
			dtp.authorizationDateTransferP = rqInMo.documentModelP.authorizationDateModelP;
			dtp.authorizationNumberTransferP = rqInMo.documentModelP.authorizationNumberModelP;
			dtp.accessKeyTransferP = rqInMo.documentModelP.accessKeyModelP;
			dtp.descriptionTransferP = rqInMo.documentModelP.descriptionModelP;
			dtp.referenceTransferP = rqInMo.documentModelP.referenceModelP;
			dtp.idEmissionPointTransferP = rqInMo.documentModelP.idEmissionPointModelP;
			dtp.idDocumentTypeTransferP = rqInMo.documentModelP.idDocumentTypeModelP;
			dtp.nameDocumentTypeTransferP = DataProviderDocumentType.GetOneDocumentType(rqInMo.documentModelP.idDocumentTypeModelP)?.nameModelP;
			dtp.idDocumentStateTransferP = rqInMo.documentModelP.idDocumentStateModelP;
			dtp.codeDocumentStateTransferP = codeDocState;
			dtp.nameDocumentStateTransferP = nameDocState;
			dtp.isOpenTransferP = rqInMo.documentModelP.isOpenModelP;

			//pide Yliana que se muestre en la pestaña documento información que es de cabecera
			//se personaliza el DTO de transferencia de Documento agregando Bodega y Ubicación 21 Agosto 12:39
			dtp.nameWarehouseHeadP = DataProviderWarehouse.GetOneWarehouseModelP((int)rqInMo.idWarehouseModelP)?.nameWarehouseModelP;
			dtp.idNatureMoveHeadP = rqInMo.idNatureMoveModelP;
			dtp.idWarehouseHeadP = (int)rqInMo.idWarehouseModelP;
			dtp.ReadOnlyBeRemissionGuide = rqTr.ReadOnlyBeRemissionGuide;


			rqTr.documentRequestTransferP = dtp;

			#endregion

			#region DETALLE
			rqTr.lstRequestInvDetail = (from _lsr in rqInMo.lstRequestInvDetail
										join _ltp in lstItemModelP on _lsr.id_item equals _ltp.idModelP
										join _lin in lstItemMetricUnit on _lsr.id_item equals _lin.idItem
										join _lwi in lstWareLoca on _lsr.id_warehouseLocation equals _lwi.idWarehouseLocationModelP into _wi
										from _lwi in _wi.DefaultIfEmpty()
										select new RequestInventoryMoveDetailTransferP
										{
											id = _lsr.id,
											id_item = _lsr.id_item,
											master_code_item = _ltp.masterCodeModelP,
											aux_code_item = _ltp.auxCodeModelP,
											codeMetricUnit = _lin.codeMetricUnit,
											nameMetricUnit = _lin.nameMetricUnit,
											idWarehouseLocation = _lsr.id_warehouseLocation,
											nameWarehouseLocation = _lwi != null ? _lwi.nameWarehouseLocationModelP : "",
											name_item = _ltp.nameModelP,
											quantityRequest = _lsr.quantityRequest,
											quantityUpdate = _lsr.quantityUpdate,
											isActive = _lsr.isActive
										}).ToList();

			#endregion

			#region FILL EMISSIONDATE ORIGIN DOCUMENT
			List<DocumentTrackStateModelP> lstDocumentTrack = DataProviderDocument.GetDocumentTrackState(rqInMo.idRIMModelP) as List<DocumentTrackStateModelP>;
			List<ActionOnDocumentModelP> lstActionOnDocument = DataProviderDocument.GetActionOnDocuments() as List<ActionOnDocumentModelP>;

			int idCD = lstActionOnDocument.FirstOrDefault(fod => fod.codeActionOnDocumentModelP.Equals("CD")).idActionOnDocumentModelP;
			int idMD = lstActionOnDocument.FirstOrDefault(fod => fod.codeActionOnDocumentModelP.Equals("MD")).idActionOnDocumentModelP;

			DocumentTrackStateModelP _dlmp = lstDocumentTrack
										.Where(w => w.idActionOnDocumentModelP == idMD)
										.OrderByDescending(o => o.dtDateModelP).FirstOrDefault();

			if (_dlmp != null)
			{
				rqTr.emissionDateHeadPOrigin = _dlmp.dtDateModelP;
			}
			else
			{
				_dlmp = lstDocumentTrack
							.Where(w => w.idActionOnDocumentModelP == idCD)
							.OrderByDescending(o => o.dtDateModelP).FirstOrDefault();
				if (_dlmp != null)
				{
					rqTr.emissionDateHeadPOrigin = _dlmp.dtDateModelP;
				}
			}

			#endregion

			#region FILL INVENTORYMOVE

			var queryDocumentSourceOrigin = db.DocumentSource
				.Where(w => w.id_documentOrigin == rqInMo.idRIMModelP)
				.Select(s => new DocumentSourceModelP()
				{
					id_ds = s.id,
					id_Document_P = s.id_document,
					id_Document_Origin_P = s.id_documentOrigin
				});

			string natMovDet = queryNatureMove.FirstOrDefault(fod => fod.idAdvanceDetailModelP == rqInMo.idNatureMoveModelP).codeAdvanceDetailModelP.Trim();



			List<DocumentDocumentOriginInformationModelP> lstFinalList = (from _lstDoc in queryDocumentSourceOrigin
																		  join _lstTy in queryDocumentsIm on _lstDoc.id_Document_P equals _lstTy.idDocumentModelP
																		  where !(_lstTy.idDocumentStateModelP == idDocStateNulled || _lstTy.idDocumentStateModelP == idDocPending)
																		  orderby _lstTy.emissionDateModelP descending, _lstTy.idDocumentModelP descending
																		  select new DocumentDocumentOriginInformationModelP
																		  {
																			  idDocumentModelP = _lstDoc.id_Document_P,
																			  idDocumentOriginModelP = _lstDoc.id_Document_Origin_P,
																			  sequentialDocumentOriginModelP = _lstTy.sequentialModelP.ToString(),
																			  emissionDate = _lstTy.emissionDateModelP
																		  }).ToList();

			#region First Filter Not Reversed InventoryMove

			List<InventoryMoveModelP> lstInMov = DataProviderInventoryMove.GetInventoryMoves() as List<InventoryMoveModelP>;
			List<InventoryReasonModelP> lstInRea = DataProviderInventoryMove.GetInventoryMoveReason() as List<InventoryReasonModelP>;

			List<RequestInventoryMoveDocsFromApprovedIM> lstAppDocIM = (from _lfl in lstFinalList
																		join _lim in lstInMov on _lfl.idDocumentModelP equals _lim.idInventoryMoveModelP
																		join _linr in lstInRea on _lim.idInventoryReasonModelP equals _linr.idInventoryReasonModelP
																		join _lnm in queryNatureMove.ToList() on _linr.idNatureMoveInventoryReasonModelP equals _lnm.idAdvanceDetailModelP
																		orderby _lfl.emissionDate descending
																		select new RequestInventoryMoveDocsFromApprovedIM
																		{
																			idDocumentDGModelP = _lfl.idDocumentModelP,
																			sequentialDGModelP = _lim.iSequential.ToString(),
																			nameNatureMove = _lnm.nameAdvanceDetailModelP,
																			nameInventoryReasonDGModelP = _linr.nameInventoryReasonModelP
																		}).ToList();
			rqTr.lstReqAppDocsIM = lstAppDocIM;
			#endregion

			#endregion

			return rqTr;
		}

		#endregion

		#region DETAILS REQUEST INVENTORY MOVE MANAGEMENT

		public static RequestInventoryMoveEditForm AddNewDetailRequestInventoryMove(RequestInventoryMoveDetailTransferP rqTmp
															, RequestInventoryMoveEditForm rqEditForm)
		{
			var _rqDetailTmp = rqEditForm?.rqTransfer?.lstRequestInvDetail.FirstOrDefault(fod => fod.id == rqTmp.id);
			if (_rqDetailTmp == null)
			{
				RequestInventoryMoveDetailModelP rqMNew = AddNewModelObjectForTempData(rqEditForm, rqTmp);
				RequestInventoryMoveDetailTransferP rqTNew = AddNewTransferObjectForTempData(rqEditForm, rqTmp);

				rqEditForm.rqTransfer.lstRequestInvDetail.Add(rqTNew);
				rqEditForm.rqModel.lstRequestInvDetail.Add(rqMNew);
			}
			return rqEditForm;
		}
		public static RequestInventoryMoveEditForm UpdateDetailRequestInventoryMove(RequestInventoryMoveDetailTransferP rqTmp
															, RequestInventoryMoveEditForm rqEditForm)
		{
			RequestInventoryMoveDetailTransferP _rqDetailTmp = rqEditForm?
																	.rqTransfer?
																	.lstRequestInvDetail
																	.FirstOrDefault(fod => fod.id == rqTmp.id);

			RequestInventoryMoveDetailModelP _rqMDetailTmp = rqEditForm?
														.rqModel?
														.lstRequestInvDetail
														.FirstOrDefault(fod => fod.id == rqTmp.id);
			if (_rqDetailTmp != null)
			{
				UpdateTransferObjectForTempData(ref _rqDetailTmp, rqTmp);
				UpdateModelObjectForTempData(ref _rqMDetailTmp, rqTmp);
			}
			return rqEditForm;
		}

		public static RequestInventoryMoveEditForm DeleteDetailRequestInventoryMove(int id
															, RequestInventoryMoveEditForm rqEditForm)
		{
			RequestInventoryMoveDetailTransferP _rqDetailTmp = rqEditForm?
																	.rqTransfer?
																	.lstRequestInvDetail
																	.FirstOrDefault(fod => fod.id == id);

			RequestInventoryMoveDetailModelP _rqMDetailTmp = rqEditForm?
														.rqModel?
														.lstRequestInvDetail
														.FirstOrDefault(fod => fod.id == id);
			if (_rqDetailTmp != null)
			{
				DeleteTransferObjectForTempData(ref _rqDetailTmp);
				DeleteModelObjectForTempData(ref _rqMDetailTmp);
			}
			return rqEditForm;
		}
		private static int GenerateNextIdForDetailRim(RequestInventoryMoveEditForm rqEditForm)
		{
			return rqEditForm?.rqTransfer?.lstRequestInvDetail.Max(x => x.id) + 1 ?? 0;
		}

		private static RequestInventoryMoveDetailTransferP AddNewTransferObjectForTempData(RequestInventoryMoveEditForm rqEditForm, RequestInventoryMoveDetailTransferP rqDetail)
		{
			RequestInventoryMoveDetailTransferP _rqNew = new RequestInventoryMoveDetailTransferP();
			List<ItemModelP> lstItems = DataProviderItem.GetListItemModelP();
			List<ItemInventoryModelP> lstInven = DataProviderItem.GetListItemInventory();
			List<MetricUnitModelP> lstMetricUnits = DataProviderItem.GetMetricUnit() as List<MetricUnitModelP>;
			List<WarehouseLocationModelP> lstWareLoca = DataProviderWarehouse.GetWarehouseLocationModelP();

			var lstItemMetricUnit = (from _it in lstInven
									 join _me in lstMetricUnits on _it.idMetricUnitInventoryModelP equals _me.idMetricUnitModelP
									 select new
									 {
										 idItem = _it.idItemModelP,
										 codeMetricUnit = _me.codeMetricUnitModelP,
										 nameMetricUnit = _me.nameMetricUnitModelP
									 }).ToList();

			if (rqDetail.id == 0)
			{
				rqDetail.id = GenerateNextIdForDetailRim(rqEditForm);
			}
			_rqNew.id = rqDetail.id;
			_rqNew.id_item = rqDetail.id_item;
			_rqNew.master_code_item = lstItems.FirstOrDefault(fod => fod.idModelP == rqDetail.id_item)?.masterCodeModelP;
			_rqNew.name_item = lstItems.FirstOrDefault(fod => fod.idModelP == rqDetail.id_item)?.nameModelP;
			_rqNew.codeMetricUnit = lstItemMetricUnit.FirstOrDefault(fod => fod.idItem == rqDetail.id_item)?.codeMetricUnit;
			_rqNew.idWarehouseLocation = rqDetail.idWarehouseLocation;
			_rqNew.nameWarehouseLocation = lstWareLoca.FirstOrDefault(fod => fod.idWarehouseLocationModelP == rqDetail.idWarehouseLocation)?.nameWarehouseLocationModelP ?? "";
			_rqNew.quantityRequest = rqDetail.quantityRequest;
			_rqNew.quantityUpdate = rqDetail.quantityUpdate;
			_rqNew.isActive = true;

			return _rqNew;
		}

		private static RequestInventoryMoveDetailModelP AddNewModelObjectForTempData(RequestInventoryMoveEditForm rqEditForm, RequestInventoryMoveDetailTransferP rqDetail)
		{
			RequestInventoryMoveDetailModelP _rqNew = new RequestInventoryMoveDetailModelP();
			List<ItemModelP> lstItems = DataProviderItem.GetListItemModelP();

			if (rqDetail.id == 0)
			{
				rqDetail.id = GenerateNextIdForDetailRim(rqEditForm);
			}
			_rqNew.id = rqDetail.id;
			_rqNew.id_item = rqDetail.id_item;
			_rqNew.id_warehouseLocation = rqDetail.idWarehouseLocation;
			_rqNew.quantityRequest = rqDetail.quantityRequest;
			_rqNew.quantityUpdate = rqDetail.quantityUpdate;
			_rqNew.isActive = true;

			return _rqNew;
		}

		private static void UpdateTransferObjectForTempData(ref RequestInventoryMoveDetailTransferP rqToUpdate
															, RequestInventoryMoveDetailTransferP rqDetail)
		{
			List<ItemModelP> lstItemModelP = DataProviderItem.GetListItemModelP();
			List<ItemInventoryModelP> lstInven = DataProviderItem.GetListItemInventory();
			List<MetricUnitModelP> lstMetricUnits = DataProviderItem.GetMetricUnit() as List<MetricUnitModelP>;
			List<WarehouseLocationModelP> lstWareLoca = DataProviderWarehouse.GetWarehouseLocationModelP();
			int idWa = rqToUpdate.idWarehouseLocation ?? 0;
			var lstItemMetricUnit = (from _it in lstInven
									 join _me in lstMetricUnits on _it.idMetricUnitInventoryModelP equals _me.idMetricUnitModelP
									 select new
									 {
										 idItem = _it.idItemModelP,
										 codeMetricUnit = _me.codeMetricUnitModelP,
										 nameMetricUnit = _me.nameMetricUnitModelP
									 }).ToList();

			rqToUpdate.id_item = rqDetail.id_item;
			rqToUpdate.master_code_item = lstItemModelP.FirstOrDefault(fod => fod.idModelP == rqDetail.id_item)?.masterCodeModelP ?? "";
			rqToUpdate.name_item = lstItemModelP.FirstOrDefault(fod => fod.idModelP == rqDetail.id_item)?.nameModelP ?? "";
			rqToUpdate.codeMetricUnit = lstItemMetricUnit.FirstOrDefault(fod => fod.idItem == rqDetail.id_item)?.codeMetricUnit ?? "";
			rqToUpdate.idWarehouseLocation = rqDetail.idWarehouseLocation ?? rqToUpdate.idWarehouseLocation;
			rqToUpdate.nameWarehouseLocation = rqToUpdate.nameWarehouseLocation ?? (lstWareLoca.FirstOrDefault(fod => fod.idWarehouseLocationModelP == idWa)?.nameWarehouseLocationModelP ?? "");
			rqToUpdate.quantityRequest = rqDetail.quantityRequest;
			rqToUpdate.quantityUpdate = rqDetail.quantityUpdate;
			rqToUpdate.isActive = true;
		}

		private static void UpdateModelObjectForTempData(ref RequestInventoryMoveDetailModelP rqMToUpdate
														, RequestInventoryMoveDetailTransferP rqDetail)
		{
			List<ItemModelP> lstItemModelP = DataProviderItem.GetListItemModelP();

			rqMToUpdate.id_item = rqDetail.id_item;
			rqMToUpdate.quantityRequest = rqDetail.quantityRequest;
			rqMToUpdate.quantityUpdate = rqDetail.quantityUpdate;
			rqMToUpdate.id_warehouseLocation = rqDetail.idWarehouseLocation ?? rqMToUpdate.id_warehouseLocation;
			rqMToUpdate.isActive = true;
		}

		private static void DeleteTransferObjectForTempData(ref RequestInventoryMoveDetailTransferP rqToUpdate)
		{
			rqToUpdate.isActive = false;
		}

		private static void DeleteModelObjectForTempData(ref RequestInventoryMoveDetailModelP rqMToUpdate)
		{
			rqMToUpdate.isActive = false;
		}

		#endregion

		#region REQUEST INVENTORY MOVE AUXILIAR
		public static RequestInventoryMoveEditForm UpdateHeaderRequestInventoryMoveEditForm(DBContext db, RequestInventoryMoveTransferPUpdateEntity rimUpdate, RequestInventoryMoveEditForm rqOrigin)
		{
			RequestInventoryMoveEditForm rimEditForm = new RequestInventoryMoveEditForm();

			#region Update Model
			rimEditForm.rqModel = new RequestInventoryMoveModelP();

			rimEditForm.rqModel.idRIMModelP = rqOrigin.rqModel.idRIMModelP;
			rimEditForm.rqModel.idPersonRModelP = rimUpdate.id_PersonRequest != null ? (int)rimUpdate.id_PersonRequest : rqOrigin.rqModel.idPersonRModelP;
			rimEditForm.rqModel.idWarehouseModelP = rimUpdate.id_Warehouse != null ? (int)rimUpdate.id_Warehouse : rqOrigin.rqModel.idWarehouseModelP;
			rimEditForm.rqModel.idProviderModelP = rqOrigin.rqModel.idProviderModelP;
			rimEditForm.rqModel.idCustomerModelP = rqOrigin.rqModel.idCustomerModelP;
			rimEditForm.rqModel.idNatureMoveModelP = rimUpdate.id_NatureMove != null ? (int)rimUpdate.id_NatureMove : rqOrigin.rqModel.idNatureMoveModelP;

			rimEditForm.rqModel.documentModelP = new DocumentModelP();
			rimEditForm.rqModel.documentModelP.idDocumentModelP = rqOrigin.rqModel.documentModelP.idDocumentModelP;
			rimEditForm.rqModel.documentModelP.numberModelP = rqOrigin.rqModel.documentModelP.numberModelP;
			rimEditForm.rqModel.documentModelP.sequentialModelP = rqOrigin.rqModel.documentModelP.sequentialModelP;

			//rimEditForm.rqModel.documentModelP.emissionDateModelP = rimUpdate.emissionDateDoc;
			rimEditForm.rqModel.documentModelP.emissionDateModelP =
				rimUpdate.yEmissionDate != null ?
				new DateTime((int)rimUpdate.yEmissionDate, (int)rimUpdate.mEmissionDate, (int)rimUpdate.dEmissionDate, (int)rimUpdate.hoursEmissionDate, (int)rimUpdate.minutesEmissionDate, 0) :
				rqOrigin.rqModel.documentModelP.emissionDateModelP;

			rimEditForm.rqModel.documentModelP.authorizationDateModelP = rqOrigin.rqModel.documentModelP.authorizationDateModelP;
			rimEditForm.rqModel.documentModelP.authorizationNumberModelP = rqOrigin.rqModel.documentModelP.authorizationNumberModelP;
			rimEditForm.rqModel.documentModelP.accessKeyModelP = rqOrigin.rqModel.documentModelP.accessKeyModelP;
			rimEditForm.rqModel.documentModelP.descriptionModelP = rqOrigin.rqModel.documentModelP.descriptionModelP;
			rimEditForm.rqModel.documentModelP.referenceModelP = rqOrigin.rqModel.documentModelP.referenceModelP;
			rimEditForm.rqModel.documentModelP.idEmissionPointModelP = rqOrigin.rqModel.documentModelP.idEmissionPointModelP;
			rimEditForm.rqModel.documentModelP.idDocumentTypeModelP = rqOrigin.rqModel.documentModelP.idDocumentTypeModelP;
			rimEditForm.rqModel.documentModelP.idDocumentStateModelP = rqOrigin.rqModel.documentModelP.idDocumentStateModelP;
			rimEditForm.rqModel.documentModelP.isOpenModelP = rqOrigin.rqModel.documentModelP.isOpenModelP;

			#region Update Details
			if (rqOrigin.rqModel.lstRequestInvDetail != null)
			{
				rimEditForm.rqModel.lstRequestInvDetail = new List<RequestInventoryMoveDetailModelP>();
				foreach (var detail in rqOrigin.rqModel.lstRequestInvDetail)
				{
					rimEditForm.rqModel.lstRequestInvDetail.Add(new RequestInventoryMoveDetailModelP
					{
						id = detail.id,
						id_item = detail.id_item,
						quantityRequest = detail.quantityRequest,
						id_warehouseLocation = detail.id_warehouseLocation,
						quantityUpdate = detail.quantityUpdate,
						isActive = detail.isActive
					});
				}
			}
			#endregion

			#endregion

			#region Update Transfer Object
			RequestInventoryMoveTransferP rqInMoTransfer = GetRequestInventoryMoveTransfer(db, rimEditForm.rqModel);
			#endregion

			rimEditForm.rqTransfer = rqInMoTransfer;
			return rimEditForm;
		}
		#endregion

		#region REQUEST INVENTORY MOVE APPOVE

		#endregion
	}
}