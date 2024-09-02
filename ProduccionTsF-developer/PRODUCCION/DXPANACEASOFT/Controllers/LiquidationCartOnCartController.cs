using DevExpress.Web;
using DevExpress.Web.Mvc;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Models.LiquidationCartOnCartP.LiquidationCartOnCartModel;
using DXPANACEASOFT.Services;
using EntidadesAuxiliares.CrystalReport;
using EntidadesAuxiliares.General;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DXPANACEASOFT.Controllers
{
	[Authorize]
	public class LiquidationCartOnCartController : DefaultController
	{
		private LiquidationCartOnCart GetLiquidationCartOnCart()
		{
			if (!(Session["LiquidationCartOnCart"] is LiquidationCartOnCart liquidationCartOnCart))
				liquidationCartOnCart = new LiquidationCartOnCart();
			return liquidationCartOnCart;
		}

		private List<LiquidationCartOnCart> GetModel()
		{
			if (!(Session["Model"] is List<LiquidationCartOnCart> liquidationCartOnCart))
				liquidationCartOnCart = new List<LiquidationCartOnCart>();
			return liquidationCartOnCart;
		}

		private void SetLiquidationCartOnCart(LiquidationCartOnCart liquidationCartOnCart)
		{
			Session["LiquidationCartOnCart"] = liquidationCartOnCart;
		}

		private void SetModel(List<LiquidationCartOnCart> liquidationCartOnCart)
		{
			Session["Model"] = liquidationCartOnCart;
		}
		[HttpPost]
		public ActionResult Index()
		{
			return View();
		}

		#region Liquidation Cart On Cart EDITFORM

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationCartOnCartFormEditPartial(int id, int? id_productionLot)
		{
			LiquidationCartOnCart liquidationCartOnCart = db.LiquidationCartOnCart.FirstOrDefault(r => r.id == id);

			if (liquidationCartOnCart == null)
			{
				DocumentType documentType = db.DocumentType.FirstOrDefault(t => t.code.Equals("74"));//Liquidación Carro por Carro 74
				DocumentState documentState = db.DocumentState.FirstOrDefault(e => e.code == "01");//Estado Pendiente 01

				tbsysUserRecordSecurity UserRecordSecurity = null;
				int id_us = ActiveUser.id;
				if (id_us != 0)
				{
					UserRecordSecurity = db.tbsysUserRecordSecurity.FirstOrDefault(r => r.id_user == id_us && r.tbsysObjSecurityRecord.obj == "LiquidationCartOnCart");
				}

				Employee employee = ActiveUser.Employee;

				liquidationCartOnCart = new LiquidationCartOnCart
				{
					Document = new Document
					{
						id = 0,
						id_documentType = documentType?.id ?? 0,
						DocumentType = documentType,
						id_documentState = documentState?.id ?? 0,
						DocumentState = documentState,
						emissionDate = DateTime.Now
					},
					LiquidationCartOnCartDetail = new List<LiquidationCartOnCartDetail>()
				};

				if (id_productionLot != null)
				{
					liquidationCartOnCart.id_ProductionLot = id_productionLot.Value;
					liquidationCartOnCart.ProductionLot = db.ProductionLot.FirstOrDefault(fod => fod.id == id_productionLot);
				}
				if (UserRecordSecurity != null)
				{
					var id_employee = db.User.FirstOrDefault(fod => fod.id == id_us)?.id_employee ?? 0;
					if (db.Person.FirstOrDefault(fod => fod.Rol.Any(a => a.name == "Liquidador") && fod.id == id_employee) != null)
					{
						liquidationCartOnCart.id_liquidator = id_employee;
					}
				}
			}

			ViewBag.id_MachineProdOpeningDetailInit = db.MachineProdOpeningDetail
				.FirstOrDefault(fod => fod.id_MachineForProd == liquidationCartOnCart.id_MachineForProd
									&& fod.id_MachineProdOpening == liquidationCartOnCart.id_MachineProdOpening)?
				.MachineForProd?.id;

			this.ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;
			SetLiquidationCartOnCart(liquidationCartOnCart);
			//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
			//TempData.Keep("liquidationCartOnCart");



			return PartialView("_FormEditLiquidationCartOnCart", liquidationCartOnCart);
		}

		#endregion

		#region ResultGridView

		[ValidateInput(false)]
		public ActionResult LiquidationCartOnCartResultsPartial(int? id_documentState, string number, //Document 
																DateTime? startEmissionDate, DateTime? endEmissionDate, //Emission 
																string noLote, int[] processes, int[] providers, LiquidationCartOnCart liquidationCartOnCart)//ProductionLot
		{
			List<LiquidationCartOnCart> model = db.LiquidationCartOnCart.ToList();

			#region Document

			//id_documentState
			if (id_documentState != 0 && id_documentState != null)
			{
				model = model.Where(o => o.Document.id_documentState == id_documentState).ToList();
			}

			//number
			if (!string.IsNullOrEmpty(number))
			{
				model = model.Where(o => o.Document.number.Contains(number)).ToList();
			}

			#endregion

			#region Emission

			//startEmissionDate
			if (startEmissionDate != null)
			{
				model = model.Where(o => DateTime.Compare(startEmissionDate.Value.Date, o.Document.emissionDate.Date) <= 0).ToList();
			}

			//endEmissionDate
			if (endEmissionDate != null)
			{
				model = model.Where(o => DateTime.Compare(o.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
			}

			#endregion

			#region ProductionLot

			string loteManualParm = db.Setting.FirstOrDefault(fod => fod.code == "PLOM")?.value ?? "NO";
			if (loteManualParm == "SI")
			{

				if (!string.IsNullOrEmpty(noLote))
				{
					var idsProductionLot = db.LiquidationCartOnCartDetail
						.Where(o => o.ProductionLot.number.Contains(noLote) || o.ProductionLot.internalNumber.Contains(noLote))
						.Select(e => e.id_LiquidationCartOnCart)
						.Distinct()
						.ToArray();

					model = model.Where(o => idsProductionLot.Contains(o.id)).ToList();
				}
			}
			else
			{
				//noLote
				if (!string.IsNullOrEmpty(noLote))
				{
					model = model.Where(o => o.ProductionLot.number.Contains(noLote) || o.ProductionLot.internalNumber.Contains(noLote)).ToList();
				}
			}


			//processes
			if (processes != null && processes.Length > 0)
			{
				var tempModel = new List<LiquidationCartOnCart>();
				foreach (var m in model)
				{
					if (processes.Contains(m.ProductionLot.id_productionProcess))
					{
						tempModel.Add(m);
					}
				}
				model = tempModel;
			}

			//providers
			if (providers != null && providers.Length > 0)
			{
				var tempModel = new List<LiquidationCartOnCart>();
				foreach (var m in model)
				{
					if (providers.Contains(m.ProductionLot.id_provider.Value))
					{
						tempModel.Add(m);
					}
				}
				model = tempModel;
			}

			#endregion

			#region Liquidador
			if (liquidationCartOnCart.id_liquidator == null)
			{
				tbsysUserRecordSecurity UserRecordSecurity = null;
				int id_us = ActiveUser.id;
				if (id_us != 0)
				{
					UserRecordSecurity = db.tbsysUserRecordSecurity.FirstOrDefault(r => r.id_user == id_us && r.tbsysObjSecurityRecord.obj == "LiquidationCartOnCart");
				}

				if (UserRecordSecurity != null)
				{
					var id_employee = db.User.FirstOrDefault(fod => fod.id == id_us)?.id_employee ?? 0;
					if (db.Person.FirstOrDefault(fod => fod.Rol.Any(a => a.name == "Liquidador") && fod.id == id_employee) != null)
					{
						liquidationCartOnCart.id_liquidator = id_employee;
					}
				}
			}
			//Liquidador
			if (liquidationCartOnCart.id_liquidator != null)
			{
				model = model.Where(o => o.id_liquidator == liquidationCartOnCart.id_liquidator).ToList();
			}
			#endregion

			var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

			if (entityObjectPermissions != null)
			{
				var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
				if (entityPermissions != null)
				{
					var tempModel = new List<LiquidationCartOnCart>();
					foreach (var item in model)
					{
						var inventoryMoveDetail = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_MachineForProd && (fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Visualizar") != null));
						if (inventoryMoveDetail != null)
						{
							tempModel.Add(item);
						}
					}

					model = tempModel;

				}
			}

			SetModel(model);
			//TempData["model"] = model;
			//TempData.Keep("model");

			return PartialView("_LiquidationCartOnCartResultsPartial", model.OrderByDescending(o => o.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult ProductionLotsResults()
		{
			var model = db.ProductionLot
							.Where(d => (d.ProductionLotState.code == "01"// 01: PENDIENTE DE RECEPCIÓN, 02: RECEPCIONADO 
							|| d.ProductionLotState.code == "02" || d.ProductionLotState.code == "03")
							&& d.ProductionProcess.code == "REC")
							.OrderByDescending(d => d.id);
			return PartialView("_ProductionLotResultsPartial", model.ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult ProductionLotsPartial()
		{
			var model = db.ProductionLot
							.Where(d => d.ProductionLotState.code == "01"// 01: PENDIENTE DE RECEPCIÓN, 02: RECEPCIONADO 
							|| d.ProductionLotState.code == "02" || d.ProductionLotState.code == "03")
							.OrderByDescending(d => d.id);
			return PartialView("_ProductionLotsPartial", model.ToList());
		}

		#endregion

		#region Liquidation Cart On Cart

		[HttpPost]
		public ActionResult LiquidationCartOnCartPartial()
		{
			var model = GetModel();
			//var model = (TempData["model"] as List<LiquidationCartOnCart>);
			//model = model ?? new List<LiquidationCartOnCart>();

			//TempData.Keep("model");
			return PartialView("_LiquidationCartOnCartPartial", model.OrderByDescending(o => o.id).ToList());
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationCartOnCartAddNew(bool approve, LiquidationCartOnCart item, Document itemDoc)
		{
			DBContext db1 = new DBContext();

			LiquidationCartOnCart liquidationCartOnCart = GetLiquidationCartOnCart();
			//LiquidationCartOnCart liquidationCartOnCart = (TempData["liquidationCartOnCart"] as LiquidationCartOnCart);
			liquidationCartOnCart = liquidationCartOnCart ?? new LiquidationCartOnCart();

			liquidationCartOnCart.Document.emissionDate = itemDoc.emissionDate;
			liquidationCartOnCart.Document.description = itemDoc.description;

			liquidationCartOnCart.id_MachineForProd = item.id_MachineForProd;
			liquidationCartOnCart.id_MachineProdOpening = item.id_MachineProdOpening;
			//liquidationCartOnCart.id_ProductionLot = item.id_ProductionLot;

			liquidationCartOnCart.dateInit = DateTime.Parse(Request.Params["dateInitLiquidation"]);//item.dateInit;
			liquidationCartOnCart.dateEnd = DateTime.Parse(Request.Params["dateEndLiquidation"]); //item.dateEnd;

			liquidationCartOnCart.timeInit = TimeSpan.Parse(Request.Params["timeInitLiquidation"]);//item.timeInit;
			liquidationCartOnCart.timeEnd = TimeSpan.Parse(Request.Params["timeEndLiquidation"]);//item.timeEnd;
			liquidationCartOnCart.id_liquidator = item.id_liquidator;
			liquidationCartOnCart.internalNumberDetail = "";

			if (ModelState.IsValid)
			{
				using (DbContextTransaction trans = db1.Database.BeginTransaction())
				{
					try
					{
						#region Document

						item.Document = item.Document ?? new Document();
						item.Document.id_userCreate = ActiveUser.id;
						item.Document.dateCreate = DateTime.Now;
						item.Document.id_userUpdate = ActiveUser.id;
						item.Document.dateUpdate = DateTime.Now;

						DocumentType documentType = db1.DocumentType
							.FirstOrDefault(dt => dt.code == "74"); //Liquidación Carro por Carro 74
						if (documentType == null)
						{
							TempData.Keep("liquidationCartOnCart");
							ViewData["EditMessage"] = ErrorMessage("No se puede guardar la Liquidación Carro X Carro porque no existe el Tipo de Documento: Recepción de Materiales con Código: 71, configúrelo e inténtelo de nuevo");
							//if (db1 != null)
							//	db1.Dispose();
							return PartialView("_LiquidationCartOnCartEditFormPartial", liquidationCartOnCart);

						}
						item.Document.id_documentType = documentType.id;
						item.Document.DocumentType = documentType;
						item.Document.sequential = GetDocumentSequential(item.Document.id_documentType);
						item.Document.number = GetDocumentNumber(item.Document.id_documentType);


						DocumentState documentState = db1.DocumentState.FirstOrDefault(s => s.code == "01");
						if (documentState == null)
						{
							TempData.Keep("liquidationCartOnCart");
							ViewData["EditMessage"] = ErrorMessage("No se puede guardar la Liquidación Carro X Carro porque no existe el Estado de Documento: Pendiente con Código: 01, configúrelo e inténtelo de nuevo");
							return PartialView("_LiquidationCartOnCartEditFormPartial", liquidationCartOnCart);

						}
						item.Document.id_documentState = documentState.id;
						item.Document.DocumentState = documentState;

						item.Document.EmissionPoint = db1.EmissionPoint.FirstOrDefault(e => e.id == ActiveEmissionPoint.id);
						item.Document.id_emissionPoint = ActiveEmissionPoint.id;

						item.Document.emissionDate = itemDoc.emissionDate;
						item.Document.description = itemDoc.description;

						//Actualiza Secuencial
						if (documentType != null)
						{
							documentType.currentNumber = documentType.currentNumber + 1;
							db1.DocumentType.Attach(documentType);
							db1.Entry(documentType).State = EntityState.Modified;
						}

						#endregion

						#region LiquidationCartOnCart

						item.MachineForProd = db1.MachineForProd.FirstOrDefault(fod => fod.id == item.id_MachineForProd);
						liquidationCartOnCart.MachineForProd = item.MachineForProd;

						item.MachineProdOpening = db1.MachineProdOpening.FirstOrDefault(fod => fod.id == item.id_MachineProdOpening);
						liquidationCartOnCart.MachineProdOpening = item.MachineProdOpening;

						item.id_ProductionLot = liquidationCartOnCart.id_ProductionLot;
						item.ProductionLot = db1.ProductionLot.FirstOrDefault(fod => fod.id == item.id_ProductionLot);
						liquidationCartOnCart.ProductionLot = item.ProductionLot;

						item.dateInit = DateTime.Parse(Request.Params["dateInitLiquidation"]);//item.dateInit;
						item.dateEnd = DateTime.Parse(Request.Params["dateEndLiquidation"]); //item.dateEnd;

						item.timeInit = TimeSpan.Parse(Request.Params["timeInitLiquidation"]);//item.timeInit;
						item.timeEnd = TimeSpan.Parse(Request.Params["timeEndLiquidation"]);//item.timeEnd;
						item.id_liquidator = liquidationCartOnCart.id_liquidator;
						item.internalNumberDetail = liquidationCartOnCart.internalNumberDetail;

						//item.ForeignCustomer1 = db1.ForeignCustomer.FirstOrDefault(fod => fod.id == item.id_Consignee);//id_Consignee
						//item.ForeignCustomer2 = db1.ForeignCustomer.FirstOrDefault(fod => fod.id == item.id_Notifier);//id_Notifier

						//item.id_company = this.ActiveCompanyId;
						//invoiceCommercial.id_company = item.id_company;

						#endregion

						var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

						if (entityObjectPermissions != null)
						{
							var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
							if (entityPermissions != null)
							{

								var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
								if (entityValuePermissions == null)
								{
									throw new Exception("No tiene Permiso para editar y guardar la liquidacion Carro x Carro en la máquina: " + item.MachineForProd.name + ".");
								}
								if (approve)
								{
									entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
									if (entityValuePermissions == null)
									{
										throw new Exception("No tiene Permiso para aprobar la liquidacion Carro x Carro en la máquina: " + item.MachineForProd.name + ".");
									}
								}

							}
						}

						#region LiquidationCartOnCartDetail

						item.LiquidationCartOnCartDetail = new List<LiquidationCartOnCartDetail>();
						if (liquidationCartOnCart.LiquidationCartOnCartDetail != null)
						{
							foreach (var detail in liquidationCartOnCart.LiquidationCartOnCartDetail)
							{
								var liquidationCartOnCartDetail = new LiquidationCartOnCartDetail();
								liquidationCartOnCartDetail.id_LiquidationCartOnCart = item.id;
								liquidationCartOnCartDetail.id_SalesOrder = detail.id_SalesOrder;
								liquidationCartOnCartDetail.SalesOrder = db1.SalesOrder.FirstOrDefault(fod => fod.id == detail.id_SalesOrder);
								liquidationCartOnCartDetail.id_SalesOrderDetail = detail.id_SalesOrderDetail;
								liquidationCartOnCartDetail.SalesOrderDetail = db1.SalesOrderDetail.FirstOrDefault(fod => fod.id == detail.id_SalesOrderDetail);
								liquidationCartOnCartDetail.id_ProductionCart = detail.id_ProductionCart;
								liquidationCartOnCartDetail.ProductionCart = db1.ProductionCart.FirstOrDefault(fod => fod.id == detail.id_ProductionCart);

								liquidationCartOnCartDetail.id_ItemLiquidation = detail.id_ItemLiquidation;
								liquidationCartOnCartDetail.Item = db1.Item.FirstOrDefault(fod => fod.id == detail.id_ItemLiquidation);
								liquidationCartOnCartDetail.quatityBoxesIL = detail.quatityBoxesIL;
								liquidationCartOnCartDetail.quantityKgsIL = detail.quantityKgsIL;
								liquidationCartOnCartDetail.quantityPoundsIL = detail.quantityPoundsIL;

								liquidationCartOnCartDetail.id_ItemToWarehouse = detail.id_ItemToWarehouse;
								liquidationCartOnCartDetail.Item1 = db1.Item.FirstOrDefault(fod => fod.id == detail.id_ItemToWarehouse);
								liquidationCartOnCartDetail.quantityKgsITW = detail.quantityKgsITW;
								liquidationCartOnCartDetail.quantityPoundsITW = detail.quantityPoundsITW;
								liquidationCartOnCartDetail.id_Client = detail.id_Client;
								liquidationCartOnCartDetail.id_subProcessIOProductionProcess = detail.id_subProcessIOProductionProcess;
								liquidationCartOnCartDetail.boxesReceived = 0.00M;
								liquidationCartOnCartDetail.id_ProductionLotManual = detail.id_ProductionLotManual;
								liquidationCartOnCartDetail.ProductionLot = db1.ProductionLot.FirstOrDefault(fod => fod.id == detail.id_ProductionLotManual);
								liquidationCartOnCartDetail.observation = detail.observation;

								item.LiquidationCartOnCartDetail.Add(liquidationCartOnCartDetail);
							}
						}
						string loteManualParm = db.Setting.FirstOrDefault(fod => fod.code == "PLOM")?.value ?? "NO";
						if (loteManualParm == "SI")
						{

							var _liquidationCartOnCartDetail = item?.LiquidationCartOnCartDetail.ToList() ?? new List<LiquidationCartOnCartDetail>();
							var detallesLotes = _liquidationCartOnCartDetail.Select(e => e.ProductionLot.internalNumber).Distinct().ToList();
							item.internalNumberDetail = string.Join(", ", detallesLotes);

						}

						this.ViewBag.IsCopaking = (item != null && item.ProductionLot != null && item.ProductionLot.isCopackingLot != null) ? true : false;
						var it = item.LiquidationCartOnCartDetail.FirstOrDefault(fod => fod.Item.ItemType.ProcessType.id != item.idProccesType);
						if (it != null)
						{
							throw new Exception("El Proceso de la liquidación no coincide con el tipo de producto " + it.Item.name);
						}

						#endregion

						if (approve)
						{
							if (item.LiquidationCartOnCartDetail.Count == 0)
							{
								throw new Exception("No se puede aprobar una Liquidación Carro X Carro sin detalles de Producto.");
							}

							if (item.ProductionLot.ProductionLotState.code != "01"
								&& item.ProductionLot.ProductionLotState.code != "02"
								&& item.ProductionLot.ProductionLotState.code != "03")
							{
								throw new Exception("No se puede aprobar debido a que el lote ya no existe en estado PENDIENTE DE RECEPCION, RECEPCIONADO o PENDIENTE DE PROCESAMIENTO. Verifique el caso, e intente de nuevo.");
							}

							if (item.ProductionLot.ProductionLotState.code == "02"
								|| item.ProductionLot.ProductionLotState.code == "03")
							{
								int? aId_processType = 0;
								string aCode_processType = "";
								foreach (var detail in item.ProductionLot.ProductionLotDetail)
								{
									var aQualityControl = detail.ProductionLotDetailQualityControl.FirstOrDefault(fod => fod.QualityControl.Document.DocumentState.code.Equals("03"))?.QualityControl;//"03": Aprobado

									if (aId_processType == 0)
									{
										aId_processType = aQualityControl?.id_processType;
										aCode_processType = aQualityControl?.ProcessType?.code;
									}

								}

								var processTypeENT = db1.ProcessType.FirstOrDefault(fod => fod.code == "ENT");
								var processTypeCOL = db1.ProcessType.FirstOrDefault(fod => fod.code == "COL");

								if (aCode_processType != "ENT" && item.idProccesType == processTypeENT.id)
								{

									throw new Exception("No se puede aprobar liquidaciones Carro X Carro, debe revisar el Tipo de Proceso.");

								}
							}


							ServiceMachineProdOpening.UpdateMachineProdOpeningDetailTimeEnd(db, item);


							ServicePoductionLot objServicePoductionLot = new ServicePoductionLot();
							objServicePoductionLot.UpdateProductionLotLiquidationTotal(ref db1, ref item, ActiveCompany, ActiveUser);
							objServicePoductionLot = null;

							//ServicePoductionLot.UpdateProductionLotLiquidationTotal(db, item, ActiveCompany, ActiveUser);

							item.Document.DocumentState = db1.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA
						}

						db1.LiquidationCartOnCart.Add(item);
						db1.SaveChanges();
						trans.Commit();

						SetLiquidationCartOnCart(item);
						//TempData["liquidationCartOnCart"] = item;
						//TempData.Keep("liquidationCartOnCart");

						ViewData["EditMessage"] = SuccessMessage("Liquidación Carro X Carro: " + item.Document.number + " guardado exitosamente");
					}
					catch (Exception e)
					{
						//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
						//TempData.Keep("liquidationCartOnCart");
						ViewData["EditMessage"] = ErrorMessage(e.Message);
						trans.Rollback();
						//if (db1 != null)
						//	

						//db1.Dispose();
						return PartialView("_LiquidationCartOnCartEditFormPartial", liquidationCartOnCart);

						//TempData.Keep("openingClosingPlateLying");
						//ViewData["EditMessage"] = e.Message;
						//trans.Rollback();
					}
				}
			}
			else
				ViewData["EditError"] = "Por favor, corrija todos los errores.";

			//if(db1 != null)
			//db1.Dispose();
			//TempData["productionLot"] = item;
			//TempData.Keep("productionLot");

			return PartialView("_LiquidationCartOnCartEditFormPartial", item);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationCartOnCartUpdate(bool approve, LiquidationCartOnCart item, Document itemDoc)
		{
			DBContext db1 = new DBContext();
			LiquidationCartOnCart liquidationCartOnCart = GetLiquidationCartOnCart();
			//LiquidationCartOnCart liquidationCartOnCart = (TempData["liquidationCartOnCart"] as LiquidationCartOnCart);
			liquidationCartOnCart = liquidationCartOnCart ?? new LiquidationCartOnCart();

			liquidationCartOnCart.Document.emissionDate = itemDoc.emissionDate;
			liquidationCartOnCart.Document.description = itemDoc.description;

			liquidationCartOnCart.Document.emissionDate = itemDoc.emissionDate;
			liquidationCartOnCart.Document.description = itemDoc.description;

			liquidationCartOnCart.id_MachineForProd = item.id_MachineForProd;
			liquidationCartOnCart.id_MachineProdOpening = item.id_MachineProdOpening;

			liquidationCartOnCart.dateInit = DateTime.Parse(Request.Params["dateInitLiquidation"]);//item.dateInit;
			liquidationCartOnCart.dateEnd = DateTime.Parse(Request.Params["dateEndLiquidation"]); //item.dateEnd;

			liquidationCartOnCart.timeInit = TimeSpan.Parse(Request.Params["timeInitLiquidation"]);//item.timeInit;
			liquidationCartOnCart.timeEnd = TimeSpan.Parse(Request.Params["timeEndLiquidation"]);//item.timeEnd;
			liquidationCartOnCart.id_liquidator = item.id_liquidator;
			liquidationCartOnCart.internalNumberDetail = "";

			var modelItem = db1.LiquidationCartOnCart.FirstOrDefault(p => p.id == item.id);
			this.ViewBag.IsCopaking = (modelItem != null && modelItem.ProductionLot != null && modelItem.ProductionLot.isCopackingLot != null) ? true : false;
			if (ModelState.IsValid && modelItem != null)
			{
				using (DbContextTransaction trans = db1.Database.BeginTransaction())
				{
					try
					{
						#region DOCUMENT

						//modelItem.Document.emissionDate = itemDoc.emissionDate;
						modelItem.Document.description = itemDoc.description;
						modelItem.Document.id_userUpdate = ActiveUser.id;
						modelItem.Document.dateUpdate = DateTime.Now;

						#endregion

						#region LiquidationCartOnCart

						modelItem.id_MachineForProd = item.id_MachineForProd;
						modelItem.id_MachineProdOpening = item.id_MachineProdOpening;
						modelItem.idProccesType = item.idProccesType;
						modelItem.id_liquidator = item.id_liquidator;

						//modelItem.id_ProductionLot = item.id_ProductionLot;

						//modelItem.dateInit = item.dateInit;
						//modelItem.dateEnd = item.dateEnd;

						//modelItem.timeInit = item.timeInit;
						//modelItem.timeEnd = item.timeEnd;
						//modelItem.id_ProductionLot = liquidationCartOnCart.id_ProductionLot;
						//modelItem.ProductionLot = db1..ProductionLot.FirstOrDefault(fod => fod.id == item.id_ProductionLot);
						//liquidationCartOnCart.ProductionLot = item.ProductionLot;

						modelItem.dateInit = DateTime.Parse(Request.Params["dateInitLiquidation"]);//item.dateInit;
						modelItem.dateEnd = DateTime.Parse(Request.Params["dateEndLiquidation"]); //item.dateEnd;

						modelItem.timeInit = TimeSpan.Parse(Request.Params["timeInitLiquidation"]);//item.timeInit;
						modelItem.timeEnd = TimeSpan.Parse(Request.Params["timeEndLiquidation"]);//item.timeEnd;

						modelItem.MachineForProd = db1.MachineForProd.FirstOrDefault(fod => fod.id == item.id_MachineForProd);
						liquidationCartOnCart.MachineForProd = modelItem.MachineForProd;

						modelItem.MachineProdOpening = db1.MachineProdOpening.FirstOrDefault(fod => fod.id == item.id_MachineProdOpening);
						liquidationCartOnCart.MachineProdOpening = modelItem.MachineProdOpening;

						//modelItem.ProductionLot = db1..ProductionLot.FirstOrDefault(fod => fod.id == item.id_ProductionLot);
						liquidationCartOnCart.ProductionLot = modelItem.ProductionLot;

						#endregion

						var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

						if (entityObjectPermissions != null)
						{
							var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
							if (entityPermissions != null)
							{

								var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Editar") != null);
								if (entityValuePermissions == null)
								{
									throw new Exception("No tiene Permiso para editar y guardar la liquidacion Carro x Carro en la máquina: " + item.MachineForProd.name + ".");
								}
								if (approve)
								{
									entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == item.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Aprobar") != null);
									if (entityValuePermissions == null)
									{
										throw new Exception("No tiene Permiso para aprobar la liquidacion Carro x Carro en la máquina: " + item.MachineForProd.name + ".");
									}
								}

							}
						}

						#region LiquidationCartOnCartDetail

						if (modelItem.LiquidationCartOnCartDetail != null)
						{
							for (int i = modelItem.LiquidationCartOnCartDetail.Count - 1; i >= 0; i--)
							{
								var detail = modelItem.LiquidationCartOnCartDetail.ElementAt(i);
								if (detail.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "01" ||
																								  fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "03") == null)
								{
									modelItem.LiquidationCartOnCartDetail.Remove(detail);
									db1.Entry(detail).State = EntityState.Deleted;
								}
							}

							foreach (var detail in liquidationCartOnCart.LiquidationCartOnCartDetail)
							{
								var detailModelItem = modelItem.LiquidationCartOnCartDetail.FirstOrDefault(fod => fod.id == detail.id);
								//if (detail.InventoryMovePlantTransferDetail?.FirstOrDefault(fod => fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "01" ||
								//                                                                  fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "03") == null)
								if (detailModelItem == null)
								{
									var liquidationCartOnCartDetail = new LiquidationCartOnCartDetail();

									liquidationCartOnCartDetail.id_LiquidationCartOnCart = modelItem.id;
									liquidationCartOnCartDetail.id_SalesOrder = detail.id_SalesOrder;
									liquidationCartOnCartDetail.SalesOrder = db1.SalesOrder.FirstOrDefault(fod => fod.id == detail.id_SalesOrder);
									liquidationCartOnCartDetail.id_SalesOrderDetail = detail.id_SalesOrderDetail;
									liquidationCartOnCartDetail.SalesOrderDetail = db1.SalesOrderDetail.FirstOrDefault(fod => fod.id == detail.id_SalesOrderDetail);
									liquidationCartOnCartDetail.id_ProductionCart = detail.id_ProductionCart;
									liquidationCartOnCartDetail.ProductionCart = db1.ProductionCart.FirstOrDefault(fod => fod.id == detail.id_ProductionCart);

									liquidationCartOnCartDetail.id_ItemLiquidation = detail.id_ItemLiquidation;
									liquidationCartOnCartDetail.Item = db1.Item.FirstOrDefault(fod => fod.id == detail.id_ItemLiquidation);
									liquidationCartOnCartDetail.quatityBoxesIL = detail.quatityBoxesIL;
									liquidationCartOnCartDetail.quantityKgsIL = detail.quantityKgsIL;
									liquidationCartOnCartDetail.quantityPoundsIL = detail.quantityPoundsIL;

									liquidationCartOnCartDetail.id_ItemToWarehouse = detail.id_ItemToWarehouse;
									liquidationCartOnCartDetail.Item1 = db1.Item.FirstOrDefault(fod => fod.id == detail.id_ItemToWarehouse);
									liquidationCartOnCartDetail.quantityKgsITW = detail.quantityKgsITW;
									liquidationCartOnCartDetail.quantityPoundsITW = detail.quantityPoundsITW;
									liquidationCartOnCartDetail.id_Client = detail.id_Client;
									liquidationCartOnCartDetail.id_subProcessIOProductionProcess = detail.id_subProcessIOProductionProcess;
									liquidationCartOnCartDetail.SubProcessIOProductionProcess = db1.SubProcessIOProductionProcess.FirstOrDefault(fod => fod.id == detail.id_subProcessIOProductionProcess);
									liquidationCartOnCartDetail.boxesReceived = 0.00M;
									liquidationCartOnCartDetail.id_ProductionLotManual = detail.id_ProductionLotManual;
									liquidationCartOnCartDetail.ProductionLot = db1.ProductionLot.FirstOrDefault(fod => fod.id == detail.id_ProductionLotManual);
									liquidationCartOnCartDetail.observation = detail.observation;

									modelItem.LiquidationCartOnCartDetail.Add(liquidationCartOnCartDetail);
								}
								else
								{
									var aLiquidationCartOnCartDetail = modelItem.LiquidationCartOnCartDetail.FirstOrDefault(fod => fod.id == detail.id);
									aLiquidationCartOnCartDetail.quatityBoxesIL = detail.quatityBoxesIL;
									aLiquidationCartOnCartDetail.quantityKgsIL = detail.quantityKgsIL;
									aLiquidationCartOnCartDetail.quantityPoundsIL = detail.quantityPoundsIL;
									aLiquidationCartOnCartDetail.quantityKgsITW = detail.quantityKgsITW;
									aLiquidationCartOnCartDetail.quantityPoundsITW = detail.quantityPoundsITW;
									aLiquidationCartOnCartDetail.id_Client = detail.id_Client;
									aLiquidationCartOnCartDetail.id_subProcessIOProductionProcess = detail.id_subProcessIOProductionProcess;
									aLiquidationCartOnCartDetail.id_ProductionLotManual = detail.id_ProductionLotManual;
									aLiquidationCartOnCartDetail.SubProcessIOProductionProcess = db1.SubProcessIOProductionProcess.FirstOrDefault(fod => fod.id == detail.id_subProcessIOProductionProcess);

									// 20210615 : Solicitado Por Ing. Loly Astudillo
									// Descripción : Actualizar los items en el proceso de actualizacion del detalle
									aLiquidationCartOnCartDetail.id_ItemLiquidation = detail.id_ItemLiquidation;
									aLiquidationCartOnCartDetail.Item = db1.Item.FirstOrDefault(fod => fod.id == detail.id_ItemLiquidation);
									aLiquidationCartOnCartDetail.id_ItemToWarehouse = detail.id_ItemToWarehouse;
									aLiquidationCartOnCartDetail.Item1 = db1.Item.FirstOrDefault(fod => fod.id == detail.id_ItemToWarehouse);


								}
							}
						}
						string loteManualParm = db.Setting.FirstOrDefault(fod => fod.code == "PLOM")?.value ?? "NO";
						if (loteManualParm == "SI")
						{
							var _liquidationCartOnCartDetail = modelItem?.LiquidationCartOnCartDetail.ToList() ?? new List<LiquidationCartOnCartDetail>();
							var detallesLotes = _liquidationCartOnCartDetail.Select(e => e.ProductionLot.internalNumber).Distinct().ToList();
							modelItem.internalNumberDetail = string.Join(", ", detallesLotes);

						}



						var it = modelItem.LiquidationCartOnCartDetail.FirstOrDefault(fod => fod.Item.ItemType.ProcessType.id != modelItem.idProccesType);
						if (it != null)
						{
							throw new Exception("El Proceso de la liquidación no coincide con el tipo de producto " + it.Item.name);
						}

						#endregion

						if (approve)
						{
							var isPCongela = db1.Setting.FirstOrDefault(fod => fod.code == "PCONGELA")?.value ?? "";
							if (modelItem.LiquidationCartOnCartDetail.Count == 0)
							{
								throw new Exception("No se puede aprobar una Liquidación Carro X Carro sin detalles de Producto.");
							}

							if (modelItem.ProductionLot.ProductionLotState.code != "01"
								&& modelItem.ProductionLot.ProductionLotState.code != "02"
								&& modelItem.ProductionLot.ProductionLotState.code != "03")
							{
								throw new Exception("No se puede aprobar debido a que el lote ya no existe en estado PENDIENTE DE RECEPCION, RECEPCIONADO o PENDIENTE DE PROCESAMIENTO. Verifique el caso, e intente de nuevo.");
							}

							if ((isPCongela != null && isPCongela == "SI") && modelItem.LiquidationCartOnCartDetail.FirstOrDefault(fod1 => fod1.SubProcessIOProductionProcess != null && fod1.SubProcessIOProductionProcess.ProductionProcess.code == "CNG" &&
																																		   fod1.quatityBoxesIL != (fod1.boxesReceived != null ? fod1.boxesReceived : 0)) != null)
							{
								throw new Exception("Existe producto sin ingresar a los túneles o con ingreso a túneles en estado pendiente. Verifique el caso, e intente de nuevo.");
							}


							if (modelItem.ProductionLot.ProductionLotState.code == "02"
								|| modelItem.ProductionLot.ProductionLotState.code == "03")
							{
								int? aId_processType = 0;
								string aCode_processType = "";
								foreach (var detail in modelItem.ProductionLot.ProductionLotDetail)
								{
									var aQualityControl = detail.ProductionLotDetailQualityControl.FirstOrDefault(fod => fod.QualityControl.Document.DocumentState.code.Equals("03"))?.QualityControl;//"03": Aprobado

									if (aId_processType == 0)
									{
										aId_processType = aQualityControl?.id_processType;
										aCode_processType = aQualityControl?.ProcessType?.code;
									}

								}

								var processTypeENT = db1.ProcessType.FirstOrDefault(fod => fod.code == "ENT");
								var processTypeCOL = db1.ProcessType.FirstOrDefault(fod => fod.code == "COL");

								if (aCode_processType != "ENT" && item.idProccesType == processTypeENT.id)
								{

									throw new Exception("No se puede aprobar liquidaciones Carro X Carro, debe revisar el Tipo de Proceso.");

								}
							}

							ServiceMachineProdOpening.UpdateMachineProdOpeningDetailTimeEnd(db1, modelItem);

							modelItem.Document.DocumentState = db1.DocumentState.FirstOrDefault(s => s.code == "03"); //APROBADA

							ServicePoductionLot objServicePoductionLot = new ServicePoductionLot();
							objServicePoductionLot.UpdateProductionLotLiquidationTotal(ref db1, ref modelItem, ActiveCompany, ActiveUser);
							objServicePoductionLot = null;
							//ServicePoductionLot.UpdateProductionLotLiquidationTotal(db1., modelItem, ActiveCompany, ActiveUser);
						}

						db1.LiquidationCartOnCart.Attach(modelItem);
						db1.Entry(modelItem).State = EntityState.Modified;

						db1.SaveChanges();
						trans.Commit();

						SetLiquidationCartOnCart(modelItem);
						//TempData["liquidationCartOnCart"] = modelItem;
						//TempData.Keep("liquidationCartOnCart");

						ViewData["EditMessage"] = SuccessMessage("Liquidación Carro X Carro: " + modelItem.Document.number + " guardada exitosamente");
					}
					catch (Exception e)
					{
						//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
						//TempData.Keep("liquidationCartOnCart");
						ViewData["EditMessage"] = ErrorMessage(e.Message);
						trans.Rollback();
						//if (db1 != null)
						//	db1.Dispose();
						return PartialView("_LiquidationCartOnCartEditFormPartial", liquidationCartOnCart);
					}
				}
			}
			else
				ViewData["EditError"] = "Por favor , corrija todos los errores.";

			if (approve)
			{
				using (DbContextTransaction trans2 = db1.Database.BeginTransaction())
				{
					try
					{
						ProductionLot plTmp = db1.ProductionLot
													.FirstOrDefault(fod => fod.id == modelItem.id_ProductionLot);

						List<ProductionLotLiquidation> lstPllTmp = plTmp
																	 .ProductionLotLiquidation
																	 .ToList();




						#region We have to Delete some Details



						var productionLotLiquidationIds = lstPllTmp
																.Select(r => r.id)
																.ToArray();
						var lstLiquidation = db1.ProductionLotLiquidation
															.Where(r => productionLotLiquidationIds.Contains(r.id))
															.ToList();

						var productionLotLiquidationPackingMaterialDetailIds = lstPllTmp
																					.SelectMany(r => r.ProductionLotLiquidationPackingMaterialDetail)
																					.Select(r => r.id)
																					.ToArray();

						var lstDetailPacking = db1.ProductionLotLiquidationPackingMaterialDetail
														.Where(r => productionLotLiquidationPackingMaterialDetailIds.Contains(r.id))
														.ToList();

						foreach (var det in lstPllTmp)
						{
							ProductionLotLiquidation _pllTmp = lstLiquidation.FirstOrDefault(fod => fod.id == det.id);
							List<ProductionLotLiquidationPackingMaterialDetail> lstTmp = det.ProductionLotLiquidationPackingMaterialDetail.ToList();

							foreach (var detIn in lstTmp)
							{
								ProductionLotLiquidationPackingMaterialDetail pllpmdTmp = lstDetailPacking.FirstOrDefault(fod => fod.id == detIn.id);

								db1.ProductionLotLiquidationPackingMaterialDetail.Attach(pllpmdTmp);
								db1.Entry(pllpmdTmp).State = EntityState.Deleted;
							}

							db1.ProductionLotLiquidation.Attach(_pllTmp);
							db1.Entry(_pllTmp).State = EntityState.Deleted;
						}

						#endregion

						#region Update Production Lot Detail with LiquidationCartOnCartDetail

						List<int> lstLiquidations = modelItem.GetLiquidationsWithThisProductionLot();
						//Generate Data
						List<LiquidationCartOnCartDetailGroup> lstDetailGroup = modelItem.GenerateLiquidationCartOnCartDetail(lstLiquidations) as List<LiquidationCartOnCartDetailGroup>;

						//Update Data from Production Lot but First I have to get the Production Lot related
						//There are thre cases add an item, update an item, delete an item

						//Get all products list
						var lstItems = db1.Item.Where(w => w.isActive)
											.Select(s => new
											{
												id = s.id,
												id_presentation = s.id_presentation,
												id_itemType = s.id_itemType
											}).ToList();
						var lstPresentations = db1.Presentation.Where(w => w.isActive).ToList();
						var lstMetricUnits = db1.MetricUnit.Where(w => w.isActive).ToList();
						var lstInventoryItems = db1.ItemInventory
												.Select(s => new
												{
													id_item = s.id_item,
													id_warehouse = s.id_warehouse,
													id_warehouseLocation = s.id_warehouseLocation
												}).ToList();
						//var lstGeneralItems = db1..ItemGeneral.ToList();
						var lstItemTypeProcessType = db1.ItemType
														.Where(w => w.isActive)
														.Select(s => new
														{
															idItemType = s.id,
															codeProcessType = s.ProcessType.code
														}).ToList();


						var lstInformationItems = (from v in lstItems
												   join m in lstPresentations on v.id_presentation equals m.id
												   join q in lstItemTypeProcessType on v.id_itemType equals q.idItemType
												   join n in lstMetricUnits on m.id_metricUnit equals n.id
												   join p in lstInventoryItems on v.id equals p.id_item
												   select new
												   {
													   idItem = v.id,
													   idPresentation = m.id,
													   idMetricUnitPresentation = m.id_metricUnit,
													   codeMetricUnitPresentation = n.code,
													   idWarehouse = p.id_warehouse,
													   idWarehouseLocation = p.id_warehouseLocation,
													   codeProcessType = q.codeProcessType
												   }).ToList();

						if (plTmp != null)
						{
							foreach (var det in lstDetailGroup)
							{
								ProductionLotLiquidation pllTmp = lstPllTmp
																	.FirstOrDefault(fod => fod.id_item == det.IdItemLiquidation);

								var detItem = lstInformationItems.FirstOrDefault(fod => fod.idItem == det.IdItemLiquidation);

								if (pllTmp != null)
								{

									pllTmp.quantity = det.quantityBox;
									pllTmp.quantityTotal = detItem.codeMetricUnitPresentation.Equals("Lbs") ? det.quantityPoundsILSum : det.quantityKgsILSum;
									pllTmp.quantityPoundsLiquidation = det.quantityPoundsILSum;
									pllTmp.id_metricUnitPresentation = detItem.idMetricUnitPresentation;

									db1.ProductionLotLiquidation.Attach(pllTmp);
									db1.Entry(pllTmp).State = EntityState.Modified;
								}
								else
								{
									pllTmp = new ProductionLotLiquidation
									{
										id_item = det.IdItemLiquidation,
										id_warehouse = detItem.idWarehouse,
										id_warehouseLocation = detItem.idWarehouseLocation,
										//id_salesOrder 
										//id_salesOrderDetail
										quantity = det.quantityBox,
										id_metricUnit = detItem.idMetricUnitPresentation,
										quantityTotal = detItem.codeMetricUnitPresentation.Equals("Lbs") ? det.quantityPoundsILSum : det.quantityKgsILSum,
										id_metricUnitPresentation = detItem.idMetricUnitPresentation,
										quantityPoundsLiquidation = det.quantityPoundsILSum
									};
									lstPllTmp.Add(pllTmp);
								}
							}
						}

						//Delete data which is not in 
						plTmp.ProductionLotLiquidation = lstPllTmp;

						// We have to Update wholesubtotal and tail
						plTmp.wholeSubtotal = 0;
						plTmp.subtotalTail = 0;
						plTmp.totalQuantityLiquidation = 0;

						foreach (var det in lstPllTmp)
						{
							var _qTmp = lstInformationItems
											.FirstOrDefault(fod => fod.idItem == det.id_item);

							if (_qTmp != null)
							{
								if (_qTmp.codeProcessType.Equals("ENT"))
								{
									plTmp.wholeSubtotal += det.quantityPoundsLiquidation ?? 0;
								}
								else
								{
									plTmp.subtotalTail += det.quantityPoundsLiquidation ?? 0;
								}
							}
						}
						plTmp.totalQuantityLiquidation = plTmp.wholeSubtotal + plTmp.subtotalTail;

						db1.ProductionLot.Attach(plTmp);
						db1.Entry(plTmp).State = EntityState.Modified;
						#endregion

						db1.SaveChanges();
						trans2.Commit();
					}
					catch
					{
						trans2.Rollback();
					}
				}

			}

			//TempData.Keep("productionLot");
			this.ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;
			//if (db1 != null)
			//	db1.Dispose();
			return PartialView("_LiquidationCartOnCartEditFormPartial", modelItem);
		}
		#endregion

		#region LiquidationCartOnCartDetail

		[ValidateInput(false)]
		public ActionResult LiquidationCartOnCartEditFormDetailPartial(
			string nameItemFilter, int? sizeBegin, int? sizeEnd,
			int? id_inventoryLine, int? id_itemType, int? id_itemTypeCategory,
			int? id_group, int? id_subgroup, int? id_size,
			int? id_trademark, int? id_trademarkModel, int? id_color,
			string nameCodigoItemFilter, string codeProcessTypeItem, int? idProcessType,
			int? idCliente)
		{
			LiquidationCartOnCart liquidationCartOnCart = GetLiquidationCartOnCart();
			//LiquidationCartOnCart liquidationCartOnCart = (TempData["liquidationCartOnCart"] as LiquidationCartOnCart);

			//liquidationCartOnCart = liquidationCartOnCart ?? new LiquidationCartOnCart();

			RefresshDataForEditForm(
				nameItemFilter, sizeBegin, sizeEnd, id_inventoryLine, id_itemType, id_itemTypeCategory,
				id_group, id_subgroup, id_size, id_trademark, id_trademarkModel, id_color, nameCodigoItemFilter,
				codeProcessTypeItem, idProcessType);

			var aliquidationCartOnCart = db.LiquidationCartOnCart.FirstOrDefault(p => p.id == liquidationCartOnCart.id);
			var aModel = aliquidationCartOnCart?.LiquidationCartOnCartDetail.ToList() ?? new List<LiquidationCartOnCartDetail>();

			var aLiquidationCartOnCartDetail = liquidationCartOnCart?.LiquidationCartOnCartDetail.ToList() ?? new List<LiquidationCartOnCartDetail>();
			foreach (var item in aLiquidationCartOnCartDetail)
			{
				var aModelAux = aModel.FirstOrDefault(fod => fod.id == item.id);
				item.boxesReceived = aModelAux?.boxesReceived ?? 0;
				//if (aModel.FirstOrDefault(fod => fod.id == item.id) == null)
				//{
				//	aModel.Add(item);
				//}
			}

			//liquidationCartOnCart.LiquidationCartOnCartDetail = aModel;
			var model = liquidationCartOnCart?.LiquidationCartOnCartDetail.OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();

			//var model = liquidationCartOnCart?.LiquidationCartOnCartDetail.Where(w => (w.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "01" ||
			//																								fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "03") == null))
			//						  .OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();


			var aPerson = db.Person.FirstOrDefault(p => p.isActive && p.fullname_businessName == "SIN CLIENTE");
			var itemDefault = aPerson?.id;
			this.ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;
			this.ViewBag.IdCliente = idCliente.HasValue
				? idCliente.Value
				: itemDefault;
			this.ViewBag.id_ProductionLotManual = liquidationCartOnCart.id_ProductionLot;

			//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
			//TempData.Keep("liquidationCartOnCart");
			return PartialView("_LiquidationCartOnCartEditFormDetailPartial", model);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationCartOnCartEditFormDetailAddNew(
			LiquidationCartOnCartDetail item, int? id_itemLiquidation2, int? id_itemWarehouse2,
			string nameItemFilter, int? sizeBegin, int? sizeEnd,
			int? id_inventoryLine, int? id_itemType, int? id_itemTypeCategory,
			int? id_group, int? id_subgroup, int? id_size,
			int? id_trademark, int? id_trademarkModel, int? id_color,
			string nameCodigoItemFilter, string codeProcessTypeItem, int? idProcessType)
		{
			// Recuperar la liquidación en edición actualmente 
			var liquidationCartOnCart = GetLiquidationCartOnCart();
			//        var liquidationCartOnCart = (TempData["liquidationCartOnCart"] as LiquidationCartOnCart)
			//?? new LiquidationCartOnCart();

			item.id_ItemLiquidation = id_itemLiquidation2 ?? 0;
			item.id_ItemToWarehouse = id_itemWarehouse2 ?? 0;

			RefresshDataForEditForm(
				nameItemFilter, sizeBegin, sizeEnd, id_inventoryLine, id_itemType, id_itemTypeCategory,
				id_group, id_subgroup, id_size, id_trademark, id_trademarkModel, id_color, nameCodigoItemFilter,
				codeProcessTypeItem, idProcessType);

			if (item.id_ItemLiquidation <= 0)
			{
				this.ModelState.AddModelError("id_ItemLiquidation", "No existe el producto a liquidación.");
			}
			if (item.id_ItemToWarehouse <= 0)
			{
				this.ModelState.AddModelError("id_ItemToWarehouse", "No existe el producto a congelación.");
			}
			if (item.quantityKgsIL <= 0)
			{
				this.ModelState.AddModelError("quantityKgsIL", "Faltan las cantidades en kilos para el producto en liquidación. Verifique las conversiones.");
			}
			if (item.quantityPoundsIL <= 0)
			{
				this.ModelState.AddModelError("quantityPoundsIL", "Faltan las cantidades en libras para el producto en liquidación. Verifique las conversiones.");
			}
			if (item.quantityKgsITW <= 0)
			{
				this.ModelState.AddModelError("quantityKgsITW", "Faltan las cantidades en kilos para el producto a congelación. Verifique las conversiones.");
			}
			if (item.quantityPoundsITW <= 0)
			{
				this.ModelState.AddModelError("quantityPoundsITW", "Faltan las cantidades en libras para el producto a congelación. Verifique las conversiones.");
			}

			var itemRepetido = liquidationCartOnCart
				.LiquidationCartOnCartDetail
				.Any(d => d.id_ItemLiquidation == item.id_ItemLiquidation && d.id_ProductionCart == item.id_ProductionCart && d.id_Client == item.id_Client);

			if (itemRepetido)
			{
				this.ModelState.AddModelError("id_ItemLiquidation", "Ya existe un registro del producto en el carro indicado.");
			}

			if (ModelState.IsValid)
			{
				item.id = liquidationCartOnCart.LiquidationCartOnCartDetail.Count() > 0 ? liquidationCartOnCart.LiquidationCartOnCartDetail.Max(ppd => ppd.id) + 1 : 1;

				item.SalesOrder = db.SalesOrder.FirstOrDefault(fod => fod.id == item.id_SalesOrder);

				item.SalesOrderDetail = db.SalesOrderDetail.FirstOrDefault(fod => fod.id_salesOrder == item.id_SalesOrder && fod.id_item == item.id_ItemLiquidation);
				item.id_SalesOrderDetail = item.SalesOrderDetail?.id;
				item.ProductionCart = db.ProductionCart.FirstOrDefault(fod => fod.id == item.id_ProductionCart);
				item.Item = db.Item.FirstOrDefault(fod => fod.id == id_itemLiquidation2);
				item.Item1 = db.Item.FirstOrDefault(fod => fod.id == id_itemWarehouse2);

				liquidationCartOnCart.LiquidationCartOnCartDetail.Add(item);
			}
			else
			{
				foreach (var modelState in this.ModelState.Values)
				{
					if (modelState.Errors.Count > 0)
					{
						ViewData["EditError"] = modelState.Errors.Last().ErrorMessage;
						break;
					}
				}
			}

			this.ViewBag.idItemLiq = item.id_ItemLiquidation;
			this.ViewBag.idItemToWarehouse = item.id_ItemToWarehouse;

			//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
			//TempData.Keep("liquidationCartOnCart");

			var model = liquidationCartOnCart?.LiquidationCartOnCartDetail.OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();
			//        var model = liquidationCartOnCart
			//.LiquidationCartOnCartDetail.Where(w => (w.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "01" ||
			//																																fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "03") == null))
			//														  .OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();

			this.ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;
			return PartialView("_LiquidationCartOnCartEditFormDetailPartial", model);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationCartOnCartEditFormDetailUpdate(
			LiquidationCartOnCartDetail item, int? id_itemLiquidation2, int? id_itemWarehouse2,
			string nameItemFilter, int? sizeBegin, int? sizeEnd,
			int? id_inventoryLine, int? id_itemType, int? id_itemTypeCategory,
			int? id_group, int? id_subgroup, int? id_size,
			int? id_trademark, int? id_trademarkModel, int? id_color,
			string nameCodigoItemFilter, string codeProcessTypeItem, int? idProcessType)
		{
			// Recuperar la liquidación en edición actualmente 
			var liquidationCartOnCart = GetLiquidationCartOnCart();
			//        var liquidationCartOnCart = (TempData["liquidationCartOnCart"] as LiquidationCartOnCart)
			//?? new LiquidationCartOnCart();

			item.id_ItemLiquidation = id_itemLiquidation2 ?? 0;
			item.id_ItemToWarehouse = id_itemWarehouse2 ?? 0;

			RefresshDataForEditForm(
				nameItemFilter, sizeBegin, sizeEnd, id_inventoryLine, id_itemType, id_itemTypeCategory,
				id_group, id_subgroup, id_size, id_trademark, id_trademarkModel, id_color, nameCodigoItemFilter,
				codeProcessTypeItem, idProcessType);

			item.quantityKgsIL = quantityKgsIL(item.id_ItemLiquidation, item.id_ItemToWarehouse, item.quatityBoxesIL);
			item.quantityPoundsIL = quantityPoundsIL(item.id_ItemLiquidation, item.id_ItemToWarehouse, item.quatityBoxesIL);
			item.quantityKgsITW = quantityKgsITW(item.id_ItemLiquidation, item.id_ItemToWarehouse, item.quatityBoxesIL);
			item.quantityPoundsITW = quantityPoundsITW(item.id_ItemLiquidation, item.id_ItemToWarehouse, item.quatityBoxesIL);

			if (item.id_ItemLiquidation <= 0)
			{
				this.ModelState.AddModelError("id_ItemLiquidation", "No existe el producto a liquidación.");
			}
			if (item.id_ItemToWarehouse <= 0)
			{
				this.ModelState.AddModelError("id_ItemToWarehouse", "No existe el producto a congelación.");
			}
			if (item.quantityKgsIL <= 0)
			{
				this.ModelState.AddModelError("quantityKgsIL", "Faltan las cantidades en kilos para el producto en liquidación. Verifique las conversiones.");
			}
			if (item.quantityPoundsIL <= 0)
			{
				this.ModelState.AddModelError("quantityPoundsIL", "Faltan las cantidades en libras para el producto en liquidación. Verifique las conversiones.");
			}
			if (item.quantityKgsITW <= 0)
			{
				this.ModelState.AddModelError("quantityKgsITW", "Faltan las cantidades en kilos para el producto a congelación. Verifique las conversiones.");
			}
			if (item.quantityPoundsITW <= 0)
			{
				this.ModelState.AddModelError("quantityPoundsITW", "Faltan las cantidades en libras para el producto a congelación. Verifique las conversiones.");
			}

			var itemRepetido = liquidationCartOnCart
				.LiquidationCartOnCartDetail
				.Any(d => d.id != item.id && d.id_ItemLiquidation == item.id_ItemLiquidation && d.id_ProductionCart == item.id_ProductionCart && d.id_Client == item.id_Client);

			if (itemRepetido)
			{
				this.ModelState.AddModelError("id_ItemLiquidation", "Ya existe otro registro del producto en el carro indicado.");
			}

			if (ModelState.IsValid)
			{
				try
				{
					var modelItem = liquidationCartOnCart.LiquidationCartOnCartDetail
						.FirstOrDefault(it => it.id == item.id);

					if (modelItem != null)
					{
						modelItem.id_SalesOrder = item.id_SalesOrder;
						modelItem.SalesOrder = db.SalesOrder.FirstOrDefault(fod => fod.id == item.id_SalesOrder);

						modelItem.SalesOrderDetail = db.SalesOrderDetail.FirstOrDefault(fod => fod.id_salesOrder == item.id_SalesOrder && fod.id_item == modelItem.id_ItemLiquidation);
						modelItem.id_SalesOrderDetail = modelItem.SalesOrderDetail?.id;

						modelItem.id_ProductionCart = item.id_ProductionCart;
						modelItem.ProductionCart = db.ProductionCart.FirstOrDefault(fod => fod.id == item.id_ProductionCart);

						modelItem.id_ItemLiquidation = id_itemLiquidation2 ?? 0;
						modelItem.Item = db.Item.FirstOrDefault(fod => fod.id == id_itemLiquidation2);

						modelItem.id_ItemToWarehouse = id_itemWarehouse2 ?? 0;
						modelItem.Item1 = db.Item.FirstOrDefault(fod => fod.id == id_itemWarehouse2);

						modelItem.quatityBoxesIL = item.quatityBoxesIL;
						modelItem.quantityKgsIL = item.quantityKgsIL;
						modelItem.quantityPoundsIL = item.quantityPoundsIL;

						modelItem.quantityKgsITW = item.quantityKgsITW;
						modelItem.quantityPoundsITW = item.quantityPoundsITW;
						modelItem.id_Client = item.id_Client;
						modelItem.id_subProcessIOProductionProcess = item.id_subProcessIOProductionProcess;
						modelItem.id_ProductionLotManual = item.id_ProductionLotManual;

						modelItem.observation = item.observation;


						this.UpdateModel(modelItem);
						//TempData["liquidationCartOnCart"] = liquidationCartOnCart;

						foreach (var modelItemAct in liquidationCartOnCart.LiquidationCartOnCartDetail)
						{
							if (modelItemAct.id == item.id)
							{
								modelItemAct.quantityKgsIL = item.quantityKgsIL;
								modelItemAct.quantityPoundsIL = item.quantityPoundsIL;

								modelItemAct.quantityKgsITW = item.quantityKgsITW;
								modelItemAct.quantityPoundsITW = item.quantityPoundsITW;
							}

						}
					}
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
				}
			}
			else
			{
				foreach (var modelState in this.ModelState.Values)
				{
					if (modelState.Errors.Count > 0)
					{
						ViewData["EditError"] = modelState.Errors.Last().ErrorMessage;
						break;
					}
				}
			}


			this.ViewBag.idItemLiq = item.id_ItemLiquidation;
			this.ViewBag.idItemToWarehouse = item.id_ItemToWarehouse;

			//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
			//TempData.Keep("liquidationCartOnCart");

			var model = liquidationCartOnCart?.LiquidationCartOnCartDetail.OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();
			//var model = liquidationCartOnCart
			//	.LiquidationCartOnCartDetail.Where(w => (w.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "01" ||
			//																																	fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "03") == null))
			//															  .OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();

			this.ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;
			return PartialView("_LiquidationCartOnCartEditFormDetailPartial", model);
		}
		private decimal quantityKgsIL(int? id_ItemLiquidation, int? id_ItemToWarehouse, decimal quantity)
		{
			decimal quantityKgsIL;
			var id_metricUnitKgAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Kg")?.id ?? 0;
			var id_metricUnitLbsAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

			if (id_ItemLiquidation.HasValue && id_ItemLiquidation.Value > 0)
			{
				var itemLiquidation = db.Item.FirstOrDefault(fod => fod.id == id_ItemLiquidation);

				var presentation = itemLiquidation?.Presentation;
				var quantityTotal = QuantityTotalByPresentation(presentation, quantity);
				var id_metricUnitPresentation = presentation?.id_metricUnit
					?? itemLiquidation?.ItemHeadIngredient?.id_metricUnit
					?? itemLiquidation?.ItemInventory?.id_metricUnitInventory
					?? 0;

				if ((quantityTotal > 0) && (id_metricUnitPresentation > 0))
				{
					//KG
					if (id_metricUnitKgAux == id_metricUnitPresentation)
					{
						quantityKgsIL = this.Truncate2Decimals(quantityTotal);
					}
					else
					{
						var factor = db.MetricUnitConversion
							.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
										&& muc.id_metricOrigin == id_metricUnitPresentation
										&& muc.id_metricDestiny == id_metricUnitKgAux)?
							.factor ?? 0;
						decimal _factorlb = (presentation.minimum * factor);

						quantityKgsIL = Math.Truncate((quantity * _factorlb) * 10000m) / 10000m;
					}


					return quantityKgsIL;

				}
				else
				{
					return 0;
				}
			}
			else
			{
				return 0;
			}
		}

		private decimal quantityKgsITW(int? id_ItemLiquidation, int? id_ItemToWarehouse, decimal quantity)
		{
			decimal quantityKgsITW;
			var id_metricUnitKgAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Kg")?.id ?? 0;
			var id_metricUnitLbsAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

			if (id_ItemToWarehouse.HasValue && id_ItemToWarehouse.Value > 0)
			{
				var itemToWarehouse = db.Item.FirstOrDefault(fod => fod.id == id_ItemToWarehouse);

				var presentation = itemToWarehouse?.Presentation;
				var quantityTotal = QuantityTotalByPresentation(presentation, quantity);
				var id_metricUnitPresentation = presentation?.id_metricUnit
					?? itemToWarehouse?.ItemHeadIngredient?.id_metricUnit
					?? itemToWarehouse?.ItemInventory?.id_metricUnitInventory
					?? 0;

				if ((quantityTotal > 0) && (id_metricUnitPresentation > 0))
				{
					//KG
					if (id_metricUnitKgAux == id_metricUnitPresentation)
					{
						quantityKgsITW = this.Truncate2Decimals(quantityTotal);
					}
					else
					{
						var factor = db.MetricUnitConversion
							.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
										&& muc.id_metricOrigin == id_metricUnitPresentation
										&& muc.id_metricDestiny == id_metricUnitKgAux)?
							.factor ?? 0;

						decimal _factorlb = (presentation.minimum * factor);


						quantityKgsITW = Math.Truncate((quantity * _factorlb) * 10000m) / 10000m;
					}

					return quantityKgsITW;

				}
				else
				{
					return 0;
				}
			}
			else
			{
				return 0;
			}
		}
		private decimal quantityPoundsIL(int? id_ItemLiquidation, int? id_ItemToWarehouse, decimal quantity)
		{
			decimal quantityPoundsIL;
			var id_metricUnitKgAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Kg")?.id ?? 0;
			var id_metricUnitLbsAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

			if (id_ItemLiquidation.HasValue && id_ItemLiquidation.Value > 0)
			{
				var itemLiquidation = db.Item.FirstOrDefault(fod => fod.id == id_ItemLiquidation);

				var presentation = itemLiquidation?.Presentation;
				var quantityTotal = QuantityTotalByPresentation(presentation, quantity);
				var id_metricUnitPresentation = presentation?.id_metricUnit
					?? itemLiquidation?.ItemHeadIngredient?.id_metricUnit
					?? itemLiquidation?.ItemInventory?.id_metricUnitInventory
					?? 0;

				if ((quantityTotal > 0) && (id_metricUnitPresentation > 0))
				{
					if (id_metricUnitLbsAux == id_metricUnitPresentation)
					{
						quantityPoundsIL = this.Truncate2Decimals(quantityTotal);
					}
					else
					{
						var factor = db.MetricUnitConversion
							.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
										&& muc.id_metricOrigin == id_metricUnitPresentation
										&& muc.id_metricDestiny == id_metricUnitLbsAux)?
							.factor ?? 0;
						decimal _factorlb = Math.Truncate((presentation.minimum * factor) * 100000m) / 100000m;

						return quantityPoundsIL = Math.Round(quantity * _factorlb, 2);

					}
					return quantityPoundsIL;

				}
				else
				{
					return 0;
				}
			}
			else
			{
				return 0;
			}
		}
		private decimal quantityPoundsITW(int? id_ItemLiquidation, int? id_ItemToWarehouse, decimal quantity)
		{
			decimal quantityPoundsITW;
			var id_metricUnitKgAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Kg")?.id ?? 0;
			var id_metricUnitLbsAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

			if (id_ItemToWarehouse.HasValue && id_ItemToWarehouse.Value > 0)
			{
				var itemToWarehouse = db.Item.FirstOrDefault(fod => fod.id == id_ItemToWarehouse);

				var presentation = itemToWarehouse?.Presentation;
				var quantityTotal = QuantityTotalByPresentation(presentation, quantity);
				var id_metricUnitPresentation = presentation?.id_metricUnit
					?? itemToWarehouse?.ItemHeadIngredient?.id_metricUnit
					?? itemToWarehouse?.ItemInventory?.id_metricUnitInventory
					?? 0;

				if ((quantityTotal > 0) && (id_metricUnitPresentation > 0))
				{
					if (id_metricUnitLbsAux == id_metricUnitPresentation)
					{
						quantityPoundsITW = this.Truncate2Decimals(quantityTotal);
					}
					else
					{
						var factor = db.MetricUnitConversion
							.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
										&& muc.id_metricOrigin == id_metricUnitPresentation
										&& muc.id_metricDestiny == id_metricUnitLbsAux)?
							.factor ?? 0;
						decimal _factorlb = Math.Truncate((presentation.minimum * factor) * 100000m) / 100000m;

						quantityPoundsITW = Math.Round(quantity * _factorlb, 2);

					}
					return quantityPoundsITW;
				}
				else
				{
					return 0;
				}
			}
			else
			{
				return 0;
			}
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult LiquidationCartOnCartEditFormDetailDelete(System.Int32 id)
		{
			LiquidationCartOnCart liquidationCartOnCart = GetLiquidationCartOnCart();
			//LiquidationCartOnCart liquidationCartOnCart = (TempData["liquidationCartOnCart"] as LiquidationCartOnCart);

			//liquidationCartOnCart = liquidationCartOnCart ?? new LiquidationCartOnCart();

			//liquidationCartOnCart.LiquidationCartOnCartDetail = liquidationCartOnCart.LiquidationCartOnCartDetail ?? new List<LiquidationCartOnCartDetail>();

			try
			{
				var liquidationCartOnCartDetail = liquidationCartOnCart.LiquidationCartOnCartDetail.FirstOrDefault(p => p.id == id);

				if (liquidationCartOnCartDetail != null)
				{
					if (liquidationCartOnCartDetail.boxesReceived != null && liquidationCartOnCartDetail.boxesReceived > 0)
					{
						//TempData.Keep("liquidationCartOnCart");
						ViewData["EditError"] = ErrorMessage("No se pueda eliminar el detalle por tener Cajas Recibidas.");
						var modelError = liquidationCartOnCart?.LiquidationCartOnCartDetail.OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();

						this.ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;
						return PartialView("_LiquidationCartOnCartEditFormDetailPartial", modelError);
					}
					if (liquidationCartOnCartDetail.InventoryMovePlantTransferDetail != null && liquidationCartOnCartDetail.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code != "05" &&
																																												  fod.boxesToReceive != null && fod.boxesToReceive > 0) != null)
					{
						//TempData.Keep("liquidationCartOnCart");
						ViewData["EditError"] = ErrorMessage("No se pueda eliminar el detalle por tener Cajas Pendiente a Recibir.");
						var modelError = liquidationCartOnCart?.LiquidationCartOnCartDetail.OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();

						this.ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;
						return PartialView("_LiquidationCartOnCartEditFormDetailPartial", modelError);
					}
					liquidationCartOnCart.LiquidationCartOnCartDetail.Remove(liquidationCartOnCartDetail);
				}

				//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
			}
			catch (Exception e)
			{
				ViewData["EditError"] = e.Message;
			}

			//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
			//TempData.Keep("liquidationCartOnCart");

			//var model = liquidationCartOnCart?.LiquidationCartOnCartDetail.Where(w => (w.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "01" ||
			//																																	fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "03") == null))
			//															  .OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();
			var model = liquidationCartOnCart?.LiquidationCartOnCartDetail.OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();

			this.ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;
			return PartialView("_LiquidationCartOnCartEditFormDetailPartial", model);
		}

		#endregion

		#region DETAILS VIEW

		public ActionResult LiquidationCartOnCartDetailPartial(int? id_liquidationCartOnCart)
		{
			ViewData["id_liquidationCartOnCart"] = id_liquidationCartOnCart;
			var liquidationCartOnCart = db.LiquidationCartOnCart.FirstOrDefault(p => p.id == id_liquidationCartOnCart);
			var model = liquidationCartOnCart?.LiquidationCartOnCartDetail.OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();
			//var model = liquidationCartOnCart?.LiquidationCartOnCartDetail.Where(w => (w.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "01" ||
			//																																	fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "03") == null))
			//															  .OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();
			this.ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;
			//TempData.Keep("liquidationCartOnCart");
			return PartialView("_LiquidationCartOnCartViewsDetailPartial", model);
		}

		public ActionResult LiquidationCartOnCartReceivedDetailPartial(int? id_liquidationCartOnCart)
		{
			ViewData["id_liquidationCartOnCart"] = id_liquidationCartOnCart;
			var liquidationCartOnCart = db.LiquidationCartOnCart.FirstOrDefault(p => p.id == id_liquidationCartOnCart);
			var model = liquidationCartOnCart?.InventoryMovePlantTransferDetail.Where(w => (w.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "01" ||
																							w.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "03"))
																		  .OrderBy(od => od.LiquidationCartOnCartDetail.id_ProductionCart).ThenBy(x => x.LiquidationCartOnCartDetail.id_Client).ThenBy(x => x.LiquidationCartOnCartDetail.id_ItemToWarehouse).ToList() ?? new List<InventoryMovePlantTransferDetail>();
			//var model = liquidationCartOnCart?.LiquidationCartOnCartDetail.Where(w => (w.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "01" ||
			//																																	fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "03") != null))
			//															  .OrderBy(od => od.id).ToList() ?? new List<LiquidationCartOnCartDetail>();
			this.ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;
			//TempData.Keep("liquidationCartOnCart");
			return PartialView("_LiquidationCartOnCartViewsReceivedDetailPartial", model);
		}

		#endregion

		#region SINGLE CHANGE LIQUIDATION CART ON CART STATE

		[HttpPost]
		public ActionResult Cancel(int id)
		{
			DBContext db1 = new DBContext();
			LiquidationCartOnCart liquidationCartOnCart = db1.LiquidationCartOnCart.FirstOrDefault(r => r.id == id);

			this.ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;

			using (DbContextTransaction trans = db1.Database.BeginTransaction())
			{
				try
				{
					var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

					if (entityObjectPermissions != null)
					{
						var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
						if (entityPermissions != null)
						{
							var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == liquidationCartOnCart.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Anular") != null);
							if (entityValuePermissions == null)
							{
								throw new Exception("No tiene Permiso para Anular en la máquina: " + liquidationCartOnCart.MachineForProd.name + ".");
							}
						}
					}

					DocumentState documentState = db1.DocumentState.FirstOrDefault(s => s.code == "05");//ANULADA

					if (liquidationCartOnCart != null && documentState != null)
					{
						if (liquidationCartOnCart.Document.DocumentState.code == "03")//APROBADA
						{
							if (liquidationCartOnCart.MachineProdOpening.Document.DocumentState.code != "03")
							{
								throw new Exception("Que la Apertura de Máquina en que se esta procesando ya no esta, en estado APROBADA. Verifique el caso, e intente de nuevo.");
							}

							if (liquidationCartOnCart.ProductionLot.ProductionLotState.code != "02" && liquidationCartOnCart.ProductionLot.ProductionLotState.code != "03")
							{
								throw new Exception("Que el Lote que se esta procesando ya no esta, en estado RECEPCIONADO o PENDIENTE DE PROCESAMIENTO. Verifique el caso, e intente de nuevo.");
							}

							ServiceMachineProdOpening.UpdateMachineProdOpeningDetailTimeEnd(db, liquidationCartOnCart, true);

							ServicePoductionLot objServicePoductionLot = new ServicePoductionLot();
							objServicePoductionLot.UpdateProductionLotLiquidationTotal(ref db1, ref liquidationCartOnCart, ActiveCompany, ActiveUser, true);
							objServicePoductionLot = null;
							//ServicePoductionLot.UpdateProductionLotLiquidationTotal(db, liquidationCartOnCart, ActiveCompany, ActiveUser, true);
						}

						var aLiquidationCartOnCartDetail = liquidationCartOnCart?.LiquidationCartOnCartDetail.ToList() ?? new List<LiquidationCartOnCartDetail>();
						var aALiquidationCartOnCartDetail = aLiquidationCartOnCartDetail.FirstOrDefault(fod => fod.boxesReceived != null && fod.boxesReceived > 0);
						if (aALiquidationCartOnCartDetail != null)
						{
							throw new Exception("Existen cajas recibidas, Verifique el caso, e intente de nuevo.");
						}

						liquidationCartOnCart.Document.id_documentState = documentState.id;
						liquidationCartOnCart.Document.DocumentState = documentState;

						db1.LiquidationCartOnCart.Attach(liquidationCartOnCart);
						db1.Entry(liquidationCartOnCart).State = EntityState.Modified;

						db1.SaveChanges();
						trans.Commit();
					}
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
					trans.Rollback();
					//TempData.Keep("liquidationCartOnCart");
					ViewData["EditMessage"] = ErrorMessage("No se puede anular la Liquidación de Carro X Carro debido a: " + e.Message);
					//if (db1 != null)
					//	db1.Dispose();
					return PartialView("_LiquidationCartOnCartEditFormPartial", liquidationCartOnCart);
				}
			}

			//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
			//TempData.Keep("liquidationCartOnCart");
			ViewData["EditMessage"] = SuccessMessage("Liquidación de Carro X Carro: " + liquidationCartOnCart.Document.number + " anulada exitosamente");
			//if (db1 != null)
			//	db1.Dispose();
			return PartialView("_LiquidationCartOnCartEditFormPartial", liquidationCartOnCart);
		}

		[HttpPost]
		public ActionResult Revert(int id)
		{
			DBContext db1 = new DBContext();
			LiquidationCartOnCart liquidationCartOnCart = db1.LiquidationCartOnCart.FirstOrDefault(r => r.id == id);
			bool isRevert = false;

			this.ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;

			using (DbContextTransaction trans = db1.Database.BeginTransaction())
			{
				try
				{
					var entityObjectPermissions = (EntityObjectPermissions)ViewData["entityObjectPermissions"];

					if (entityObjectPermissions != null)
					{
						var entityPermissions = entityObjectPermissions.listEntityPermissions.FirstOrDefault(fod => fod.codeEntity == "MAC");
						if (entityPermissions != null)
						{
							var entityValuePermissions = entityPermissions.listValue.FirstOrDefault(fod2 => fod2.id_entityValue == liquidationCartOnCart.id_MachineForProd && fod2.listPermissions.FirstOrDefault(fod3 => fod3.name == "Reversar") != null);
							if (entityValuePermissions == null)
							{
								throw new Exception("No tiene Permiso para Reversar en la máquina: " + liquidationCartOnCart.MachineForProd.name + ".");
							}
						}
					}

					//Verificar si existe lotes relacionados al proceso de cierre
					var productionLot = db.ProductionLot.FirstOrDefault(a => a.id == liquidationCartOnCart.id_ProductionLot && a.isClosed);
					if (productionLot != null)
					{
						var productionLotClose = db.ProductionLotClose.FirstOrDefault(a => a.id_lot == liquidationCartOnCart.id_ProductionLot && a.Document.DocumentState.code != "05"
												&& a.isActive);

						if(productionLotClose != null && productionLot.receptionDate.Date <= productionLotClose.Document.emissionDate.Date 
							&& productionLot.ProductionLotState.code == "11")
                        {
							throw new Exception("El lote " + productionLot.number + " se ecuentra en un proceso de Cierre de Lote: " + ((productionLotClose != null) ? productionLotClose.Document.number : ""));
						}
					}

					DocumentState documentState = db1.DocumentState.FirstOrDefault(s => s.code == "01");//PENDIENTE

					if (liquidationCartOnCart != null && documentState != null)
					{

						if (liquidationCartOnCart.MachineProdOpening.Document.DocumentState.code != "03")
						{
							throw new Exception("Que la Apertura de Máquina en que se esta procesando ya no esta, en estado APROBADA. Verifique el caso, e intente de nuevo.");
						}

						if (liquidationCartOnCart.ProductionLot.ProductionLotState.code != "02" && liquidationCartOnCart.ProductionLot.ProductionLotState.code != "03")
						{
							throw new Exception("Que el Lote que se esta procesando ya no esta, en estado RECEPCIONADO o PENDIENTE DE PROCESAMIENTO. Verifique el caso, e intente de nuevo.");
						}

						ServiceMachineProdOpening.UpdateMachineProdOpeningDetailTimeEnd(db, liquidationCartOnCart, true);

						ServicePoductionLot objServicePoductionLot = new ServicePoductionLot();
						objServicePoductionLot.UpdateProductionLotLiquidationTotal(ref db1, ref liquidationCartOnCart, ActiveCompany, ActiveUser, true);
						objServicePoductionLot = null;
						//ServicePoductionLot.UpdateProductionLotLiquidationTotal(db, liquidationCartOnCart, ActiveCompany, ActiveUser, true);

						liquidationCartOnCart.Document.id_documentState = documentState.id;
						liquidationCartOnCart.Document.DocumentState = documentState;

						db1.LiquidationCartOnCart.Attach(liquidationCartOnCart);
						db1.Entry(liquidationCartOnCart).State = EntityState.Modified;

						db1.SaveChanges();
						trans.Commit();
						isRevert = true;
					}
				}
				catch (Exception e)
				{
					ViewData["EditError"] = e.Message;
					trans.Rollback();
					//TempData.Keep("liquidationCartOnCart");
					ViewData["EditMessage"] = ErrorMessage("No se puede Reversar la Liquidación de Carro X Carro debido a: " + e.Message);
					//if (db1 != null)
					//	db1.Dispose();
					return PartialView("_LiquidationCartOnCartEditFormPartial", liquidationCartOnCart);
				}
			}

			if (isRevert)
			{
				using (DbContextTransaction trans2 = db1.Database.BeginTransaction())
				{
					try
					{
						ProductionLot plTmp = db1.ProductionLot
												.FirstOrDefault(fod => fod.id == liquidationCartOnCart.id_ProductionLot);

						List<ProductionLotLiquidation> lstPllTmp = plTmp
																	.ProductionLotLiquidation
																	.ToList();

						var lstDetailPacking = db1.ProductionLotLiquidationPackingMaterialDetail.ToList();
						var lstLiquidation = db1.ProductionLotLiquidation.ToList();

						#region We have to Delete some Details

						foreach (var det in lstPllTmp)
						{
							ProductionLotLiquidation _pllTmp = lstLiquidation.FirstOrDefault(fod => fod.id == det.id);
							List<ProductionLotLiquidationPackingMaterialDetail> lstTmp = det.ProductionLotLiquidationPackingMaterialDetail.ToList();

							foreach (var detIn in lstTmp)
							{
								ProductionLotLiquidationPackingMaterialDetail pllpmdTmp = lstDetailPacking.FirstOrDefault(fod => fod.id == detIn.id);

								db1.ProductionLotLiquidationPackingMaterialDetail.Attach(pllpmdTmp);
								db1.Entry(pllpmdTmp).State = EntityState.Deleted;
							}

							db1.ProductionLotLiquidation.Attach(_pllTmp);
							db1.Entry(_pllTmp).State = EntityState.Deleted;
						}

						for (int i = lstPllTmp.Count - 1; i >= 0; i--)
						{
							lstPllTmp.RemoveAt(i);
						}

						db1.SaveChanges();

						#endregion

						#region Update Production Lot Detail with LiquidationCartOnCartDetail

						List<int> lstLiquidations = liquidationCartOnCart.GetLiquidationsWithThisProductionLot();
						//Generate Data
						List<LiquidationCartOnCartDetailGroup> lstDetailGroup = liquidationCartOnCart.GenerateLiquidationCartOnCartDetail(lstLiquidations) as List<LiquidationCartOnCartDetailGroup>;

						//Update Data from Production Lot but First I have to get the Production Lot related
						//There are thre cases add an item, update an item, delete an item

						//Get all products list
						var lstItems = db1.Item.Where(w => w.isActive).Select(s => new
						{
							id = s.id,
							id_presentation = s.id_presentation,
							id_itemType = s.id_itemType
						}).ToList();
						var lstPresentations = db1.Presentation.Where(w => w.isActive).ToList();
						var lstMetricUnits = db1.MetricUnit.Where(w => w.isActive).ToList();
						var lstInventoryItems = db1.ItemInventory.Select(s => new
						{
							id_item = s.id_item,
							id_warehouse = s.id_warehouse,
							id_warehouseLocation = s.id_warehouseLocation
						}).ToList();
						//var lstGeneralItems = db1.ItemGeneral.ToList();
						var lstItemTypeProcessType = db1.ItemType
														.Where(w => w.isActive)
														.Select(s => new
														{
															idItemType = s.id,
															codeProcessType = s.ProcessType.code
														}).ToList();


						var lstInformationItems = (from v in lstItems
												   join m in lstPresentations on v.id_presentation equals m.id
												   join q in lstItemTypeProcessType on v.id_itemType equals q.idItemType
												   join n in lstMetricUnits on m.id_metricUnit equals n.id
												   join p in lstInventoryItems on v.id equals p.id_item
												   select new
												   {
													   idItem = v.id,
													   idPresentation = m.id,
													   idMetricUnitPresentation = m.id_metricUnit,
													   codeMetricUnitPresentation = n.code,
													   idWarehouse = p.id_warehouse,
													   idWarehouseLocation = p.id_warehouseLocation,
													   codeProcessType = q.codeProcessType
												   }).ToList();

						if (plTmp != null)
						{
							foreach (var det in lstDetailGroup)
							{
								ProductionLotLiquidation pllTmp = lstPllTmp
																	.FirstOrDefault(fod => fod.id_item == det.IdItemLiquidation);

								var detItem = lstInformationItems.FirstOrDefault(fod => fod.idItem == det.IdItemLiquidation);

								if (pllTmp != null)
								{

									pllTmp.quantity = det.quantityBox;
									pllTmp.quantityTotal = detItem.codeMetricUnitPresentation.Equals("Lbs") ? det.quantityPoundsILSum : det.quantityKgsILSum;
									pllTmp.quantityPoundsLiquidation = det.quantityPoundsILSum;
									pllTmp.id_metricUnitPresentation = detItem.idMetricUnitPresentation;

									db1.ProductionLotLiquidation.Attach(pllTmp);
									db1.Entry(pllTmp).State = EntityState.Modified;
								}
								else
								{
									pllTmp = new ProductionLotLiquidation
									{
										id_item = det.IdItemLiquidation,
										id_warehouse = detItem.idWarehouse,
										id_warehouseLocation = detItem.idWarehouseLocation,
										quantity = det.quantityBox,
										id_metricUnit = detItem.idMetricUnitPresentation,
										quantityTotal = detItem.codeMetricUnitPresentation.Equals("Lbs") ? det.quantityPoundsILSum : det.quantityKgsILSum,
										id_metricUnitPresentation = detItem.idMetricUnitPresentation,
										quantityPoundsLiquidation = det.quantityPoundsILSum
									};
									lstPllTmp.Add(pllTmp);
								}
							}
						}

						//Delete data which is not in 

						// We have to Update wholesubtotal and tail
						plTmp.wholeSubtotal = 0;
						plTmp.subtotalTail = 0;
						plTmp.totalQuantityLiquidation = 0;

						foreach (var det in lstPllTmp)
						{
							var _qTmp = lstInformationItems
											.FirstOrDefault(fod => fod.idItem == det.id_item);

							if (_qTmp != null)
							{
								if (_qTmp.codeProcessType.Equals("ENT"))
								{
									plTmp.wholeSubtotal += det.quantityPoundsLiquidation ?? 0;
								}
								else
								{
									plTmp.subtotalTail += det.quantityPoundsLiquidation ?? 0;
								}
							}
						}
						plTmp.totalQuantityLiquidation = plTmp.wholeSubtotal + plTmp.subtotalTail;

						plTmp.ProductionLotLiquidation = lstPllTmp;

						db1.ProductionLot.Attach(plTmp);
						db1.Entry(plTmp).State = EntityState.Modified;
						#endregion

						db1.SaveChanges();
						trans2.Commit();
					}
					catch //(Exception ex)
					{
						trans2.Rollback();
					}
				}
			}

			//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
			//TempData.Keep("liquidationCartOnCart");
			ViewData["EditMessage"] = SuccessMessage("Liquidación de Carro X Carro: " + liquidationCartOnCart.Document.number + " reversada exitosamente");
			//if (db1 != null)
			//	db1.Dispose();
			return PartialView("_LiquidationCartOnCartEditFormPartial", liquidationCartOnCart);
		}

		#endregion

		#region SELECTED LIQUIDATION CART ON CART STATE CHANGE

		[HttpPost, ValidateInput(false)]
		public void ApproveDocuments(int[] ids)
		{
			if (ids != null)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "03");

						foreach (var id in ids)
						{
							PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

							if (purchasePlanning != null && documentState != null)
							{
								purchasePlanning.Document.id_documentState = documentState.id;
								purchasePlanning.Document.DocumentState = documentState;
							}
						}
						db.SaveChanges();
						trans.Commit();
					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
						trans.Rollback();
					}
				}
			}

			//var model = (TempData["model"] as List<PurchasePlanning>);
			//model = model ?? new List<PurchasePlanning>();
			//int[] filters = model.Select(i => i.id).ToArray();
			//model = db.PurchasePlanning.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			//TempData["model"] = model;
			//TempData.Keep("model");
		}

		[HttpPost, ValidateInput(false)]
		public void AutorizeDocuments(int[] ids)
		{
			if (ids != null)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "06");

						foreach (var id in ids)
						{
							PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

							if (purchasePlanning != null && documentState != null)
							{
								purchasePlanning.Document.id_documentState = documentState.id;
								purchasePlanning.Document.DocumentState = documentState;
							}
						}
						db.SaveChanges();
						trans.Commit();
					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
						trans.Rollback();
					}
				}
			}

			//var model = (TempData["model"] as List<PurchasePlanning>);
			//model = model ?? new List<PurchasePlanning>();
			//int[] filters = model.Select(i => i.id).ToArray();
			//model = db.PurchasePlanning.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			//TempData["model"] = model;
			//TempData.Keep("model");
		}

		[HttpPost, ValidateInput(false)]
		public void ProtectDocuments(int[] ids)
		{
			if (ids != null)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "04");

						foreach (var id in ids)
						{
							PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

							if (purchasePlanning != null && documentState != null)
							{
								purchasePlanning.Document.id_documentState = documentState.id;
								purchasePlanning.Document.DocumentState = documentState;
							}
						}
						db.SaveChanges();
						trans.Commit();
					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
						trans.Rollback();
					}
				}
			}

			//var model = (TempData["model"] as List<PurchasePlanning>);
			//model = model ?? new List<PurchasePlanning>();
			//int[] filters = model.Select(i => i.id).ToArray();
			//model = db.PurchasePlanning.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			//TempData["model"] = model;
			//TempData.Keep("model");
		}

		[HttpPost, ValidateInput(false)]
		public void CancelDocuments(int[] ids)
		{
			if (ids != null)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{
						DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == "05");

						foreach (var id in ids)
						{
							PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

							if (purchasePlanning != null && documentState != null)
							{
								purchasePlanning.Document.id_documentState = documentState.id;
								purchasePlanning.Document.DocumentState = documentState;
							}
						}
						db.SaveChanges();
						trans.Commit();
					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
						trans.Rollback();
					}
				}
			}

			//var model = (TempData["model"] as List<PurchasePlanning>);
			//model = model ?? new List<PurchasePlanning>();
			//int[] filters = model.Select(i => i.id).ToArray();
			//model = db.PurchasePlanning.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			//TempData["model"] = model;
			//TempData.Keep("model");
		}

		[HttpPost, ValidateInput(false)]
		public void RevertDocuments(int[] ids)
		{
			if (ids != null)
			{
				using (DbContextTransaction trans = db.Database.BeginTransaction())
				{
					try
					{


						foreach (var id in ids)
						{
							PurchasePlanning purchasePlanning = db.PurchasePlanning.FirstOrDefault(r => r.id == id);

							if (purchasePlanning != null)
							{
								var codeAux = purchasePlanning.Document.DocumentState.code == "05"
									? "04"
									: (purchasePlanning.Document.DocumentState.code == "04"
										? "06"
										: (purchasePlanning.Document.DocumentState.code == "06"
											? "03"
											: "01"));
								DocumentState documentState = db.DocumentState.FirstOrDefault(s => s.code == codeAux);
								if (documentState != null)
								{
									purchasePlanning.Document.id_documentState = documentState.id;
									purchasePlanning.Document.DocumentState = documentState;

									db.PurchasePlanning.Attach(purchasePlanning);
									db.Entry(purchasePlanning).State = EntityState.Modified;

									db.SaveChanges();
									trans.Commit();
								}
							}
						}
						db.SaveChanges();
						trans.Commit();
					}
					catch (Exception e)
					{
						ViewData["EditError"] = e.Message;
						trans.Rollback();
					}
				}
			}

			//var model = (TempData["model"] as List<PurchasePlanning>);
			//model = model ?? new List<PurchasePlanning>();
			//int[] filters = model.Select(i => i.id).ToArray();
			//model = db.PurchasePlanning.Where(r => filters.Contains(r.id)).AsEnumerable().ToList();

			//TempData["model"] = model;
			//TempData.Keep("model");
		}

		#endregion

		#region ACTIONS

		[HttpPost, ValidateInput(false)]
		public JsonResult Actions(int id)
		{
			var actions = new
			{
				btnApprove = false,
				btnAutorize = false,
				btnProtect = false,
				btnCancel = false,
				btnRevert = false,
				btnSave = false,
			};

			if (id == 0)
			{
				actions = new
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = false,
					btnSave = true,
				};
				return Json(actions, JsonRequestBehavior.AllowGet);
			}

			LiquidationCartOnCart liquidationCartOnCart = db.LiquidationCartOnCart.FirstOrDefault(r => r.id == id);
			string code_state = liquidationCartOnCart.Document.DocumentState.code;

			if (code_state == "01") // PENDIENTE
			{
				actions = new
				{
					btnApprove = true,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = true,
					btnRevert = false,
					btnSave = true,
				};
			}
			else if (code_state == "03") // APROBADA
			{
				actions = new
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = true,
					btnSave = false,
				};
			}
			else if (code_state == "05") // 05 ANULADA
			{
				actions = new
				{
					btnApprove = false,
					btnAutorize = false,
					btnProtect = false,
					btnCancel = false,
					btnRevert = false,
					btnSave = false,
				};
			}

			return Json(actions, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region PAGINATION

		[HttpPost, ValidateInput(false)]
		public JsonResult InitializePagination(int id_liquidationCartOnCart)
		{
			//TempData.Keep("liquidationCartOnCart");

			int index = db.LiquidationCartOnCart.OrderByDescending(r => r.id).ToList().FindIndex(r => r.id == id_liquidationCartOnCart);

			var result = new
			{
				maximunPages = db.LiquidationCartOnCart.Count(),
				currentPage = index + 1
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult Pagination(int page)
		{
			LiquidationCartOnCart liquidationCartOnCart = db.LiquidationCartOnCart.OrderByDescending(p => p.id).Take(page).ToList().Last();

			if (liquidationCartOnCart != null)
			{
				SetLiquidationCartOnCart(liquidationCartOnCart);
				//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
				//TempData.Keep("liquidationCartOnCart");
				ViewBag.IsCopaking = (liquidationCartOnCart != null && liquidationCartOnCart.ProductionLot != null && liquidationCartOnCart.ProductionLot.isCopackingLot != null) ? true : false;
				return PartialView("_LiquidationCartOnCartEditFormPartial", liquidationCartOnCart);
			}

			//TempData.Keep("liquidationCartOnCart");

			return PartialView("_LiquidationCartOnCartEditFormPartial", new LiquidationCartOnCart());
		}

		#endregion

		#region AXILIAR FUNCTIONS

		[HttpPost, ValidateInput(false)]
		public JsonResult InitSalesOrder(int? id_salesOrder)
		{
			LiquidationCartOnCart liquidationCartOnCart = GetLiquidationCartOnCart();
			//LiquidationCartOnCart liquidationCartOnCart = (TempData["liquidationCartOnCart"] as LiquidationCartOnCart);

			//liquidationCartOnCart = liquidationCartOnCart ?? new LiquidationCartOnCart();

			var salesOrderAux = db.SalesOrder.FirstOrDefault(fod => fod.id == id_salesOrder);

			var salesOrder = new
			{
				id = salesOrderAux?.id,
				name = salesOrderAux?.Document.number
			};

			var result = new
			{
				salesOrder = salesOrder
			};

			//TempData.Keep("liquidationCartOnCart");

			return Json(result, JsonRequestBehavior.AllowGet);


		}

		[HttpPost]
		public ActionResult GetItemsLiquidation(int? indice,
			string nameItemFilter, int? sizeBegin, int? sizeEnd,
			int? id_inventoryLine, int? id_itemType, int? id_itemTypeCategory,
			int? id_group, int? id_subgroup, int? id_size,
			int? id_trademark, int? id_trademarkModel, int? id_color,
			string nameCodigoItemFilter, string codeProcessTypeItem, int? idItemLiq, int? idProcessType)
		{
			LiquidationCartOnCart liquidationCartOnCart = GetLiquidationCartOnCart();
			//LiquidationCartOnCart liquidationCartOnCart = (TempData["liquidationCartOnCart"] as LiquidationCartOnCart);
			//liquidationCartOnCart = liquidationCartOnCart ?? new LiquidationCartOnCart();

			//TempData.Keep("liquidationCartOnCart");

			RefresshDataForEditForm(
				nameItemFilter, sizeBegin, sizeEnd, id_inventoryLine, id_itemType, id_itemTypeCategory,
				id_group, id_subgroup, id_size, id_trademark, id_trademarkModel, id_color, nameCodigoItemFilter,
				codeProcessTypeItem, idProcessType);

			var imd = liquidationCartOnCart?
				.LiquidationCartOnCartDetail
				.FirstOrDefault(fod => fod.id == indice) ?? new LiquidationCartOnCartDetail();

			if (idItemLiq > 0)
			{
				imd.id_ItemLiquidation = idItemLiq.Value;
			}

			return this.PartialView("ComponentsDetail/_ComboBoxItems", imd);
		}

		[HttpPost]
		public ActionResult GetItemsWarehouse(int? indice, int? id_itemCurrent, int? id_ItemEquivalence)
		{
			// Recuperar la liquidación en edición actualmente
			var liquidationCartOnCart = GetLiquidationCartOnCart();
			//var liquidationCartOnCart = (TempData["liquidationCartOnCart"] as LiquidationCartOnCart)
			//	?? new LiquidationCartOnCart();

			// Recuperamos el detalle en edición
			var liquidationCartOnCartDetail = liquidationCartOnCart
				.LiquidationCartOnCartDetail?
				.FirstOrDefault(d => d.id == indice) ?? new LiquidationCartOnCartDetail();

			liquidationCartOnCartDetail.id_ItemToWarehouse = id_ItemEquivalence ?? 0;

			// Conservar los detalles de la liquidación
			//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
			//TempData.Keep("liquidationCartOnCart");

			return this.PartialView("ComponentsDetail/_ComboBoxItemsWarehouse", liquidationCartOnCartDetail);
		}

		[HttpPost]
		public void RefresshDataForEditForm(
			string nameItem, int? sizeBegin, int? sizeEnd,
			int? id_inventoryLine, int? id_itemType, int? id_itemTypeCategory,
			int? id_group, int? id_subgroup, int? id_size,
			int? id_trademark, int? id_trademarkModel, int? id_color,
			string nameCodigoItem, string codeProcessTypeItem, int? idProcessType)
		{
			if (idProcessType <= 0)
			{
				ViewData["_ItemsDetailEditLiquidation"] = new List<Item>();
				return;
			}
			var codeMaster = db.Setting.FirstOrDefault(fod => fod.code == "PMASTER")?.value ?? "";
			var queryItems = db.Item
				.Where(w => (w.InventoryLine.code.Equals("PP") || w.InventoryLine.code.Equals("PT"))
						&& (w.ItemType != null) && (w.ItemType.ProcessType != null)
						&& (w.Presentation != null) //&& (w.Presentation.minimum == 1m)
						&& (w.Presentation.code.Substring(0, 1) != codeMaster)
						&& (w.ItemType.ProcessType.id == idProcessType)
						&& w.isActive);

			if (!String.IsNullOrEmpty(nameItem))
			{
				queryItems = queryItems
					.Where(w => w.name.Contains(nameItem));
			}
			if (!String.IsNullOrEmpty(nameCodigoItem))
			{
				queryItems = queryItems
					.Where(w => w.masterCode.Contains(nameCodigoItem));
			}

			if (sizeBegin > 0)
			{
				var orderSizeBegin = db.ItemSize
					.FirstOrDefault(fod => fod.id == sizeBegin)?
					.orderSize;

				if (orderSizeBegin.HasValue)
				{
					queryItems = (from v in queryItems
								  join c in db.ItemGeneral on v.id equals c.id_item
								  join d in db.ItemSize on c.id_size equals d.id
								  where d.orderSize >= orderSizeBegin.Value && d.isActive
								  select v);
				}
			}

			if (sizeEnd > 0)
			{
				var orderSizeEnd = db.ItemSize
					.FirstOrDefault(fod => fod.id == sizeEnd)?
					.orderSize;

				if (orderSizeEnd.HasValue)
				{
					queryItems = (from v in queryItems
								  join c in db.ItemGeneral on v.id equals c.id_item
								  join d in db.ItemSize on c.id_size equals d.id
								  where d.orderSize <= orderSizeEnd.Value && d.isActive
								  select v);
				}
			}

			if (id_inventoryLine > 0)
			{
				queryItems = queryItems
					.Where(w => w.id_inventoryLine == id_inventoryLine);
			}

			if (id_itemType > 0)
			{
				queryItems = queryItems
					.Where(w => w.id_itemType == id_itemType);
			}

			if (id_itemTypeCategory > 0)
			{
				queryItems = queryItems
					.Where(w => w.id_itemTypeCategory == id_itemTypeCategory);
			}

			if (id_group > 0)
			{
				queryItems = queryItems
					.Where(w => w.ItemGeneral.id_group == id_group);
			}

			if (id_subgroup > 0)
			{
				queryItems = queryItems
					.Where(w => w.ItemGeneral.id_subgroup == id_subgroup);
			}

			if (id_size > 0)
			{
				queryItems = queryItems
					.Where(w => w.ItemGeneral.id_size == id_size);
			}

			if (id_trademark > 0)
			{
				queryItems = queryItems
					.Where(w => w.ItemGeneral.id_trademark == id_trademark);
			}

			if (id_trademarkModel > 0)
			{
				queryItems = queryItems
					.Where(w => w.ItemGeneral.id_trademarkModel == id_trademarkModel);
			}

			if (id_color > 0)
			{
				queryItems = queryItems
					.Where(w => w.ItemGeneral.id_color == id_color);
			}

			ViewData["_ItemsDetailEditLiquidation"] = queryItems.ToList();
		}

		[HttpPost]
		public JsonResult ItemDetailData(int? id_item, decimal quantity)
		{
			decimal quantityKgsIL, quantityPoundsIL;
			int idItemEquivalenceTmp;
			string messageItemEquivalence;

			if (id_item.HasValue && id_item.Value > 0)
			{
				var item = db.Item
					.FirstOrDefault(fod => fod.id == id_item);

				idItemEquivalenceTmp = item?
					.ItemEquivalence?
					.id_itemEquivalence ?? 0;

				messageItemEquivalence = (idItemEquivalenceTmp > 0)
					? ""
					: "El item no tiene configurado su equivalente de Bodega por favor configúrelo en el mantenimiento de Productos";

				if (quantity > 0)
				{
					var presentation = item?.Presentation;
					var quantityTotal = QuantityTotalByPresentation(presentation, quantity);
					var id_metricUnitPresentation = presentation?.id_metricUnit
						?? item?.ItemHeadIngredient?.id_metricUnit
						?? item?.ItemInventory?.id_metricUnitInventory
						?? 0;

					if ((quantityTotal > 0) && (id_metricUnitPresentation > 0))
					{
						//KG
						var id_metricUnitKgAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Kg")?.id ?? 0;
						if (id_metricUnitKgAux == id_metricUnitPresentation)
						{
							quantityKgsIL = this.Truncate2Decimals(quantityTotal);
						}
						else
						{
							var factor = db.MetricUnitConversion
								.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
											&& muc.id_metricOrigin == id_metricUnitPresentation
											&& muc.id_metricDestiny == id_metricUnitKgAux)?
								.factor ?? 0;
							//decimal _factorlb = Math.Truncate((presentation.minimum * factor) * 10000m) / 10000m;
							decimal _factorlb = (presentation.minimum * factor);

							//quantityKgsIL = this.Truncate2Decimals(quantity * factor);
							quantityKgsIL = Math.Truncate((quantity * _factorlb) * 10000m) / 10000m;
						}

						//LBS
						var id_metricUnitLbsAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;
						if (id_metricUnitLbsAux == id_metricUnitPresentation)
						{
							quantityPoundsIL = this.Truncate2Decimals(quantityTotal);
						}
						else
						{
							var factor = db.MetricUnitConversion
								.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
											&& muc.id_metricOrigin == id_metricUnitPresentation
											&& muc.id_metricDestiny == id_metricUnitLbsAux)?
								.factor ?? 0;
							//decimal _factorlb = (presentation.minimum * factor);
							decimal _factorlb = Math.Truncate((presentation.minimum * factor) * 100000m) / 100000m;

							quantityPoundsIL = Math.Round(quantity * _factorlb, 2);
							//quantityPoundsIL = this.Truncate2Decimals(quantity * _factorlb);
							//quantityPoundsIL = Math.Truncate((quantity * _factorlb) * 10000m) / 10000m;
						}
					}
					else
					{
						quantityKgsIL = 0;
						quantityPoundsIL = 0;
					}
				}
				else
				{
					quantityKgsIL = 0;
					quantityPoundsIL = 0;
				}
			}
			else
			{
				quantityKgsIL = 0;
				quantityPoundsIL = 0;
				idItemEquivalenceTmp = 0;
				messageItemEquivalence = "";
			}

			var result = new
			{
				quantityKgsIL,
				quantityPoundsIL,
				idItemEquivalenceTmp,
				messageItemEquivalence,
			};

			//TempData.Keep("liquidationCartOnCart");
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		private decimal QuantityTotalByPresentation(Presentation presentation, decimal quantity)
		{
			if (presentation == null)
			{
				return quantity;
			}
			else
			{
				return presentation.minimum * quantity;
			}
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult ItsRepeatedLiquidation(int? id_salesOrderNew,
			int? id_ProductionCartNew, int? id_ItemLiquidationNew, int? id_ItemToWarehouseNew)
		{
			LiquidationCartOnCart liquidationCartOnCart = GetLiquidationCartOnCart();
			//LiquidationCartOnCart liquidationCartOnCart = (TempData["liquidationCartOnCart"] as LiquidationCartOnCart);

			//liquidationCartOnCart = liquidationCartOnCart ?? new LiquidationCartOnCart();
			var result = new
			{
				itsRepeated = 0,
				Error = ""

			};

			var liquidationCartOnCartDetailAux = liquidationCartOnCart.LiquidationCartOnCartDetail.FirstOrDefault(fod => fod.id_ItemLiquidation == id_ItemLiquidationNew &&
																				fod.id_ProductionCart == id_ProductionCartNew &&
																				fod.id_ItemToWarehouse == id_ItemToWarehouseNew &&
																				(db.SalesOrderDetail.FirstOrDefault(fod2 => fod2.id == fod.id_SalesOrderDetail)?.id_salesOrder == id_salesOrderNew ||
																				fod.id_SalesOrder == id_salesOrderNew));
			if (liquidationCartOnCartDetailAux != null)
			{
				var itemLiquidacionAux = db.Item.FirstOrDefault(fod => fod.id == id_ItemLiquidationNew);
				var itemToWarehouseAux = db.Item.FirstOrDefault(fod => fod.id == id_ItemToWarehouseNew);
				var productionCartNewAux = db.ProductionCart.FirstOrDefault(fod => fod.id == id_ProductionCartNew);
				result = new
				{
					itsRepeated = 1,
					Error = "- No se puede repetir el Carro: " + productionCartNewAux.name +
							",  con item de Liquidación: " + itemLiquidacionAux.name +
							" e item para Bodega: " + itemToWarehouseAux.name + ",  en los detalles de esta Liquidación"

				};

			}


			//TempData["liquidationCartOnCart"] = liquidationCartOnCart;
			//TempData.Keep("liquidationCartOnCart");

			return Json(result, JsonRequestBehavior.AllowGet);

		}

		[HttpPost]
		public JsonResult UpdateQuantityTotal(int? id_ItemLiquidation, int? id_ItemToWarehouse, decimal quantity)
		{
			decimal quantityKgsIL, quantityPoundsIL, quantityKgsITW, quantityPoundsITW;

			if (quantity > 0)
			{
				var id_metricUnitKgAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Kg")?.id ?? 0;
				var id_metricUnitLbsAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;

				#region ItemLiquidation

				if (id_ItemLiquidation.HasValue && id_ItemLiquidation.Value > 0)
				{
					var itemLiquidation = db.Item.FirstOrDefault(fod => fod.id == id_ItemLiquidation);

					var presentation = itemLiquidation?.Presentation;
					var quantityTotal = QuantityTotalByPresentation(presentation, quantity);
					var id_metricUnitPresentation = presentation?.id_metricUnit
						?? itemLiquidation?.ItemHeadIngredient?.id_metricUnit
						?? itemLiquidation?.ItemInventory?.id_metricUnitInventory
						?? 0;

					if ((quantityTotal > 0) && (id_metricUnitPresentation > 0))
					{
						//KG
						if (id_metricUnitKgAux == id_metricUnitPresentation)
						{
							quantityKgsIL = this.Truncate2Decimals(quantityTotal);
						}
						else
						{
							var factor = db.MetricUnitConversion
								.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
											&& muc.id_metricOrigin == id_metricUnitPresentation
											&& muc.id_metricDestiny == id_metricUnitKgAux)?
								.factor ?? 0;
							//decimal _factorlb = Math.Truncate((presentation.minimum * factor) * 10000m) / 10000m;
							decimal _factorlb = (presentation.minimum * factor);

							//quantityKgsIL = this.Truncate2Decimals(quantity * factor);
							quantityKgsIL = Math.Truncate((quantity * _factorlb) * 10000m) / 10000m;
						}


						//LBS
						if (id_metricUnitLbsAux == id_metricUnitPresentation)
						{
							quantityPoundsIL = this.Truncate2Decimals(quantityTotal);
						}
						else
						{
							var factor = db.MetricUnitConversion
								.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
											&& muc.id_metricOrigin == id_metricUnitPresentation
											&& muc.id_metricDestiny == id_metricUnitLbsAux)?
								.factor ?? 0;
							decimal _factorlb = Math.Truncate((presentation.minimum * factor) * 100000m) / 100000m;
							//decimal _factorlb = (presentation.minimum * factor);

							quantityPoundsIL = Math.Round(quantity * _factorlb, 2);
							//quantityPoundsIL = this.Truncate2Decimals(quantity * _factorlb);
							//quantityPoundsIL = Math.Truncate((quantity * _factorlb) * 10000m) / 10000m;
						}
					}
					else
					{
						quantityKgsIL = 0;
						quantityPoundsIL = 0;
					}
				}
				else
				{
					quantityKgsIL = 0;
					quantityPoundsIL = 0;
				}

				#endregion

				#region ItemToWarehouse

				if (id_ItemToWarehouse.HasValue && id_ItemToWarehouse.Value > 0)
				{
					var itemToWarehouse = db.Item.FirstOrDefault(fod => fod.id == id_ItemToWarehouse);

					var presentation = itemToWarehouse?.Presentation;
					var quantityTotal = QuantityTotalByPresentation(presentation, quantity);
					var id_metricUnitPresentation = presentation?.id_metricUnit
						?? itemToWarehouse?.ItemHeadIngredient?.id_metricUnit
						?? itemToWarehouse?.ItemInventory?.id_metricUnitInventory
						?? 0;

					if ((quantityTotal > 0) && (id_metricUnitPresentation > 0))
					{
						//KG
						if (id_metricUnitKgAux == id_metricUnitPresentation)
						{
							quantityKgsITW = this.Truncate2Decimals(quantityTotal);
						}
						else
						{
							var factor = db.MetricUnitConversion
								.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
											&& muc.id_metricOrigin == id_metricUnitPresentation
											&& muc.id_metricDestiny == id_metricUnitKgAux)?
								.factor ?? 0;
							//decimal _factorlb = Math.Truncate((presentation.minimum * factor) * 10000m) / 10000m;
							decimal _factorlb = (presentation.minimum * factor);

							//quantityKgsITW = this.Truncate2Decimals(quantity * factor);
							quantityKgsITW = Math.Truncate((quantity * _factorlb) * 10000m) / 10000m;
						}


						//LBS
						if (id_metricUnitLbsAux == id_metricUnitPresentation)
						{
							quantityPoundsITW = this.Truncate2Decimals(quantityTotal);
						}
						else
						{
							var factor = db.MetricUnitConversion
								.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
											&& muc.id_metricOrigin == id_metricUnitPresentation
											&& muc.id_metricDestiny == id_metricUnitLbsAux)?
								.factor ?? 0;
							decimal _factorlb = Math.Truncate((presentation.minimum * factor) * 100000m) / 100000m;
							//decimal _factorlb = (presentation.minimum * factor);

							quantityPoundsITW = Math.Round(quantity * _factorlb, 2);
							//quantityPoundsITW = this.Truncate2Decimals(quantity * _factorlb);
							//quantityPoundsITW = Math.Truncate((quantity * _factorlb) * 10000m) / 10000m;

						}
					}
					else
					{
						quantityKgsITW = 0;
						quantityPoundsITW = 0;
					}
				}
				else
				{
					quantityKgsITW = 0;
					quantityPoundsITW = 0;
				}

				#endregion
			}
			else
			{
				quantityKgsIL = 0;
				quantityPoundsIL = 0;
				quantityKgsITW = 0;
				quantityPoundsITW = 0;
			}

			var result = new
			{
				quantityKgsIL,
				quantityPoundsIL,
				quantityKgsITW,
				quantityPoundsITW,
			};

			//TempData.Keep("liquidationCartOnCart");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult GetWarehouseDetailItem(int? id_itemCurrent)
		{
			LiquidationCartOnCart liquidationCartOnCart = GetLiquidationCartOnCart();
			//LiquidationCartOnCart liquidationCartOnCart = (TempData["liquidationCartOnCart"] as LiquidationCartOnCart);
			Item itemAux = db.Item.FirstOrDefault(i => i.id == id_itemCurrent);
			itemAux = itemAux ?? new Item();

			//var codeFEXP = db.Setting.FirstOrDefault(fod => fod.code == "FEXP")?.value ?? "";

			var items = db.Item.AsEnumerable().Where(w => ((w.isSold && w.isActive && w.id_company == this.ActiveCompanyId /*&& w.InventoryLine.code == codeFEXP*/ /*&& w?.Presentation.code == codePMASTER*/ &&
														   w.id_itemType == itemAux.id_itemType && w.id_itemTypeCategory == itemAux.id_itemTypeCategory /*&& w.id_presentation == itemAux.id_presentation*/ &&
														   w.ItemGeneral?.id_trademark == itemAux.ItemGeneral?.id_trademark /*&& w.ItemGeneral?.id_color == itemAux?.ItemGeneral?.id_color*/) ||
														   w.id == id_itemCurrent))
											  /*.Select(s => new { s.id, s.auxCode, s.name, s.masterCode })*/.ToList();
			//TempData.Keep("liquidationCartOnCart");
			return GridViewExtension.GetComboBoxCallbackResult(p =>
			{
				p.ClientInstanceName = "id_ItemToWarehouse";
				p.Width = Unit.Percentage(100);

				p.DropDownStyle = DropDownStyle.DropDownList;
				p.ClearButton.DisplayMode = ClearButtonDisplayMode.OnHover;
				p.IncrementalFilteringMode = IncrementalFilteringMode.Contains;

				p.CallbackRouteValues = new { Controller = "LiquidationCartOnCart", Action = "GetWarehouseDetailItem"/*, TextField = "CityName"*/ };
				p.ClientSideEvents.BeginCallback = "LiquidationCartOnCartWarehouseDetailItem_BeginCallback";
				//p.ClientSideEvents.EndCallback = "LiquidationCartOnCartWarehouseDetailItem_EndCallback";
				p.ClientSideEvents.SelectedIndexChanged = "ItemLiquidationCartOnCartWarehouseDetailCombo_SelectedIndexChanged";
				p.ClientSideEvents.Validation = "OnItemLiquidationCartOnCartWarehouseDetailValidation";
				p.CallbackPageSize = 5;
				p.TextFormatString = "{0}";
				p.ValueField = "id";
				p.ValueType = typeof(int);

				p.Columns.Add("name", "Nombre del Producto", 400);//, Unit.Percentage(70));
				p.Columns.Add("ItemTypeCategory.name", "Clase", 70);//, Unit.Percentage(50));
				p.Columns.Add("ItemGeneral.ItemSize.name", "Talla", 70);//, Unit.Percentage(50));
																		//p.ClientSideEvents.Init = "ItemComboBox_Init";
				p.BindList(items);//.Bind(id_person);

			});

		}

		[HttpPost]
		public JsonResult ItemWarehouseDetailData(int? id_item, decimal quantity, int id_itemEquivalence)
		{
			decimal quantityKgsITW, quantityPoundsITW;

			if (id_item.HasValue && id_item.Value > 0 && quantity > 0)
			{
				var item = db.Item.FirstOrDefault(fod => fod.id == id_item);

				var presentation = item?.Presentation;
				var quantityTotal = QuantityTotalByPresentation(presentation, quantity);
				var id_metricUnitPresentation = presentation?.id_metricUnit
					?? item?.ItemHeadIngredient?.id_metricUnit
					?? item?.ItemInventory?.id_metricUnitInventory
					?? 0;

				if ((quantityTotal > 0) && (id_metricUnitPresentation > 0))
				{
					//KG
					var id_metricUnitKgAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Kg")?.id ?? 0;
					if (id_metricUnitKgAux == id_metricUnitPresentation)
					{
						quantityKgsITW = this.Truncate2Decimals(quantityTotal);
					}
					else
					{
						var factor = db.MetricUnitConversion
							.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
										&& muc.id_metricOrigin == id_metricUnitPresentation
										&& muc.id_metricDestiny == id_metricUnitKgAux)?
							.factor ?? 0;
						//decimal _factorlb = Math.Truncate((presentation.minimum * factor) * 10000m) / 10000m;
						decimal _factorlb = (presentation.minimum * factor);

						//quantityKgsITW = this.Truncate2Decimals(quantity * factor);
						quantityKgsITW = Math.Truncate((quantity * _factorlb) * 10000m) / 10000m;
					}


					//LBS
					var id_metricUnitLbsAux = db.MetricUnit.FirstOrDefault(fod => fod.code == "Lbs")?.id ?? 0;
					if (id_metricUnitLbsAux == id_metricUnitPresentation)
					{
						quantityPoundsITW = this.Truncate2Decimals(quantityTotal);
					}
					else
					{
						var factor = db.MetricUnitConversion
							.FirstOrDefault(muc => muc.id_company == this.ActiveCompanyId
										&& muc.id_metricOrigin == id_metricUnitPresentation
										&& muc.id_metricDestiny == id_metricUnitLbsAux)?
							.factor ?? 0;
						decimal _factorlb = Math.Truncate((presentation.minimum * factor) * 100000m) / 100000m;
						//decimal _factorlb = (presentation.minimum * factor);

						quantityPoundsITW = Math.Round(quantity * _factorlb, 2);
						//quantityPoundsITW = this.Truncate2Decimals(quantity * _factorlb);
						//quantityPoundsITW = Math.Truncate((quantity * _factorlb) * 10000m) / 10000m;
					}
				}
				else
				{
					quantityKgsITW = 0;
					quantityPoundsITW = 0;
				}
			}
			else
			{
				quantityKgsITW = 0;
				quantityPoundsITW = 0;
			}

			var result = new
			{
				quantityKgsITW,
				quantityPoundsITW,
			};

			//TempData.Keep("liquidationCartOnCart");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult GetDataMachineForProd(int? id_MachineForProd, int? id_MachineProdOpening)
		{
			var machineProdOpeningDetail = db.MachineProdOpeningDetail
												.FirstOrDefault(fod => fod.id_MachineProdOpening == id_MachineProdOpening
												&& fod.id_MachineForProd == id_MachineForProd);

			var result = new
			{
				Message = "Ok",
				id_MachineForProd = machineProdOpeningDetail?.id_MachineForProd,
				id_MachineProdOpening = machineProdOpeningDetail?.id_MachineProdOpening,
				nameTurno = machineProdOpeningDetail?.MachineProdOpening.Turn?.name,
				timeInitMachineProdOpeningDetail = machineProdOpeningDetail?.timeInit,
				timeInitTurn = machineProdOpeningDetail?.MachineProdOpening?.Turn?.timeInit,
				timeEndTurn = machineProdOpeningDetail?.MachineProdOpening?.Turn?.timeEnd,
				dateEmissionYear = machineProdOpeningDetail?.MachineProdOpening?.Document?.emissionDate.Date.Year,
				dateEmissionMonth = machineProdOpeningDetail?.MachineProdOpening?.Document?.emissionDate.Date.Month,
				dateEmissionDay = machineProdOpeningDetail?.MachineProdOpening?.Document?.emissionDate.Date.Day
			};

			//TempData.Keep("liquidationCartOnCart");

			return Json(result, JsonRequestBehavior.AllowGet);
		}



		public class ComboBoxMachinesProdOpeningModel
		{
			public int? id_MachineForProd { get; set; }
			public int? id_MachineProdOpening { get; set; }
			public string documentStateCode { get; set; }
			public DateTime? emissionDate { get; set; }
			public int? id_PersonProcessPlant { get; set; }
			public bool itemsReceivedTunnels { get; set; }
			public int? id_Turn { get; set; }
		}

		[HttpPost]
		public ActionResult GetMachinesForProdOpening(
			int? id_MachineForProd, int? id_MachineProdOpening, string documentStateCode, DateTime? emissionDate, int? id_PersonProcessPlant)
		{
			LiquidationCartOnCart liquidationCartOnCart = GetLiquidationCartOnCart();
			//LiquidationCartOnCart liquidationCartOnCart = (TempData["liquidationCartOnCart"] as LiquidationCartOnCart);
			var liquidationCartOnCartDetail = (liquidationCartOnCart != null && liquidationCartOnCart.LiquidationCartOnCartDetail != null) ? liquidationCartOnCart.LiquidationCartOnCartDetail.Where(w => (w.InventoryMovePlantTransferDetail.FirstOrDefault(fod => fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "01" ||
																																			fod.InventoryMovePlantTransfer.InventoryMove.Document.DocumentState.code == "03") != null)).ToList()
																																			: new List<LiquidationCartOnCartDetail>();
			var model = new ComboBoxMachinesProdOpeningModel()
			{
				id_MachineForProd = id_MachineForProd,
				id_MachineProdOpening = id_MachineProdOpening,
				documentStateCode = documentStateCode,
				emissionDate = emissionDate,
				id_PersonProcessPlant = id_PersonProcessPlant,
				itemsReceivedTunnels = (liquidationCartOnCartDetail.Count > 0)

			};
			//TempData.Keep("liquidationCartOnCart");

			return PartialView("ComponentsDetail/_ComboBoxMachinesProdOpening", model);
		}

		#endregion

		#region REPORT
		[HttpPost, ValidateInput(false)]
		public JsonResult PrintReportLiquidation(int id_liquidation, string codeProcessType, string codeReport)
		{
			//var liquidationMaterial = (TempData["liquidationCartOnCart"] as InventoryMove);
			//TempData["liquidationCartOnCart"] = liquidationMaterial;
			//TempData.Keep("liquidationCartOnCart");

			#region "Armo Parametros"
			List<ParamCR> paramLst = new List<ParamCR>();
			ParamCR _param = new ParamCR();
			_param.Nombre = "@id";
			_param.Valor = id_liquidation;

			paramLst.Add(_param);

			_param = new ParamCR();
			_param.Nombre = "@codeProcessType";
			_param.Valor = codeProcessType;

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

		private decimal Truncate2Decimals(decimal value)
		{
			return Decimal.Truncate(value * 100m) / 100m;
		}
	}
}
