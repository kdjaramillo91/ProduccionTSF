using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DXPANACEASOFT.Controllers
{
	[Authorize]
	public class CostCoefficientExecutionController : DefaultController
	{
		private const string m_TipoDocumentoCostCoefficientExecution = "153";

		private const string m_PendienteDocumentState = "01";
		private const string m_AprobadoDocumentState = "03";
		private const string m_AnuladoDocumentState = "05";

		private const string m_PendienteRecepcionLotDocumentState = "01";
		private const string m_AnuladoRecepcionLotDocumentState = "09";

		private const string m_MateriaPrimaInventoryLine = "MP";
		private const string m_ProductoEnProcesoInventoryLine = "PP";
		private const string m_ProductoTerminadoInventoryLine = "PT";

		private const string m_RecepcionProductionProcess = "REC";
		private const string m_RecepcionManualProductionProcess = "RMM";

		private const string m_UnidadesMetricUnit = "Un";
		private const string m_KilogramosMetricUnit = "Kg";
		private const string m_LibrasMetricUnit = "Lbs";
		private const string m_GramosMetricUnit = "Gr";

		private const string m_ResultQueryViewKeyName = "CoefficientExecution_QueryResults";
		private const string m_CostCoefficientExecutionModelKey = "coefficientExecution";


		[HttpPost]
		public PartialViewResult Index()
		{
			return this.PartialView();
		}


		#region Vista de consulta principal

		[HttpPost]
		[ValidateInput(false)]
		public PartialViewResult CostCoefficientExecutionResults(int? id_documentState,
			string number, string reference, int? anio, int? mes, int? id_allocationType,
			DateTime? startStartDate, DateTime? endStartDate,
			DateTime? startEndDate, DateTime? endEndDate)
		{
			// Preparar el Query para los datos resultantes
			var resultsQuery = db.ProductionCostCoefficientExecution
				.Include("Document")
				.Include("Document.DocumentState")
				.Include("ProductionCostAllocationType")
				.AsQueryable();

			// Aplicamos los filtros según los criterios recibidos
			if (id_documentState.HasValue)
			{
				resultsQuery = resultsQuery
					.Where(i => i.Document.id_documentState == id_documentState.Value);
			}

			if (!String.IsNullOrWhiteSpace(number))
			{
				resultsQuery = resultsQuery
					.Where(i => i.Document.number.Trim().Contains(number.Trim()));
			}

			if (!String.IsNullOrWhiteSpace(reference))
			{
				resultsQuery = resultsQuery
					.Where(i => reference.Contains(i.Document.reference));
			}

			if (anio.HasValue)
			{
				resultsQuery = resultsQuery
					.Where(i => i.anio == anio.Value);
			}

			if (mes.HasValue)
			{
				resultsQuery = resultsQuery
					.Where(i => i.mes == mes.Value);
			}

			if (id_allocationType.HasValue)
			{
				resultsQuery = resultsQuery
					.Where(i => i.id_allocationType == id_allocationType.Value);
			}

			if (startStartDate.HasValue)
			{
				resultsQuery = resultsQuery
					.Where(i => DateTime.Compare(startStartDate.Value, i.startDate) <= 0);
			}

			if (endStartDate.HasValue)
			{
				resultsQuery = resultsQuery
					.Where(i => DateTime.Compare(i.startDate, endStartDate.Value) <= 0);
			}

			if (startEndDate.HasValue)
			{
				resultsQuery = resultsQuery
					.Where(i => DateTime.Compare(startEndDate.Value, i.endDate) <= 0);
			}

			if (endEndDate.HasValue)
			{
				resultsQuery = resultsQuery
					.Where(i => DateTime.Compare(i.endDate, endEndDate.Value) <= 0);
			}


			var model = resultsQuery
				.OrderByDescending(i => i.id)
				.ToList();

			this.TempData[m_ResultQueryViewKeyName] = model;
			this.TempData.Keep(m_ResultQueryViewKeyName);

			return this.PartialView("_CoefficientExecutionQueryResultsPartial", model);
		}

		[HttpPost]
		public ActionResult CostCoefficientExecutionPartial()
		{
			var model = (this.TempData[m_ResultQueryViewKeyName] as List<ProductionCostCoefficientExecution>)
				?? new List<ProductionCostCoefficientExecution>();

			this.TempData.Keep(m_ResultQueryViewKeyName);

			return this.PartialView("_CoefficientExecutionQueryGridViewPartial", model);
		}

		#endregion

		#region Vista de edición de transacción

		[HttpPost]
		public PartialViewResult EditForm(int? id, string successMessage)
		{
			this.TempData.Remove(m_CostCoefficientExecutionModelKey);

			var coefficientExecution = this.GetEditingCostCoefficientExecution(id);
			this.TempData[m_CostCoefficientExecutionModelKey] = coefficientExecution;

			this.PrepareEditViewBag(coefficientExecution);

			if (!String.IsNullOrWhiteSpace(successMessage))
			{
				this.ViewBag.EditMessage = this.SuccessMessage(successMessage);
			}

			return PartialView("_EditForm", coefficientExecution);
		}


		[HttpPost]
		public JsonResult Create(
			int anio, int mes, DateTime startDate, DateTime endDate,
			int idAllocationType, string description, string reference,
			int[] detailsIds)
		{
			int? idCoefficientExecution;
			string message;
			bool isValid;

			detailsIds = detailsIds ?? new int[] { };

			using (var transaction = db.Database.BeginTransaction())
			{
				try
				{
					// Recuperamos el tipo de documento
					var documentType = db.DocumentType
						.FirstOrDefault(dt => dt.code == m_TipoDocumentoCostCoefficientExecution
											&& dt.id_company == this.ActiveCompanyId);

					if (documentType == null)
					{
						throw new ApplicationException("No existe registrado el tipo de documento: Ejecución de Coeficientes.");
					}

					// Recuperamos el estado PENDIENTE
					var documentState = DataProviderCostCoefficientExecution
						.GetDocumentStateByCode(this.ActiveCompanyId, m_PendienteDocumentState);

					if (documentState == null)
					{
						throw new ApplicationException("No existe registrado el estado PENDIENTE para los documentos.");
					}

					// Generamos el secuencial y el número de documento correspondiente
					var documentSequential = documentType.currentNumber;
					var documentNumber = $"{this.ActiveEmissionPoint.BranchOffice.code:000}-{this.ActiveEmissionPoint.code:000}-{documentSequential}";

					documentType.currentNumber += 1;

					// Creamos el documento
					var document = new Document()
					{
						number = documentNumber,
						sequential = documentSequential,
						emissionDate = startDate,
						description = description,
						reference = reference,
						id_emissionPoint = this.ActiveEmissionPoint.id,
						id_documentType = documentType.id,
						id_documentState = documentState.id,

						id_userCreate = this.ActiveUserId,
						dateCreate = DateTime.Now,
						id_userUpdate = this.ActiveUserId,
						dateUpdate = DateTime.Now,
					};

					// Creamos el documento de ejecución de coeficientes
					var coefficientExecution = new ProductionCostCoefficientExecution()
					{
						Document = document,

						id_company = this.ActiveCompanyId,
						id_allocationType = idAllocationType,
						anio = anio,
						mes = mes,
						startDate = startDate,
						endDate = endDate,
						processed = false,
						description = description,

						id_userCreate = this.ActiveUserId,
						dateCreate = DateTime.Now,
						id_userUpdate = this.ActiveUserId,
						dateUpdate = DateTime.Now,
					};

					// Agregar los detalles de la ejecución de coeficientes
					var coefficientExecutionTemp = (this.TempData[m_CostCoefficientExecutionModelKey] as ProductionCostCoefficientExecution);

					if ((coefficientExecutionTemp != null) && (coefficientExecutionTemp.processed))
					{
						coefficientExecution.processed = true;

						if (coefficientExecutionTemp.ProductionCostCoefficientExecutionDetails != null)
						{
							coefficientExecution.ProductionCostCoefficientExecutionDetails = coefficientExecutionTemp
								.ProductionCostCoefficientExecutionDetails
								.Where(d => d.isActive && detailsIds.Contains(d.id))
								.Select(d => new ProductionCostCoefficientExecutionDetail()
								{
									id_allocationPeriod = d.id_allocationPeriod,
									id_allocationPeriodDetail = d.id_allocationPeriodDetail,
									anio = d.anio,
									mes = d.mes,
									id_coefficient = d.id_coefficient,
									id_poundType = d.id_poundType,
									id_simpleFormula = d.id_simpleFormula,
									accountingValue = d.accountingValue,
									id_productionCost = d.id_productionCost,
									id_productionCostDetail = d.id_productionCostDetail,
									id_productionPlant = d.id_productionPlant,
									coeficiente = d.coeficiente,
									valor = d.valor,
									isActive = true,

									id_userCreate = this.ActiveUserId,
									dateCreate = DateTime.Now,
									id_userUpdate = this.ActiveUserId,
									dateUpdate = DateTime.Now,
								})
								.ToList();
						}

						if (coefficientExecutionTemp.ProductionCostCoefficientExecutionPlants != null)
						{
							coefficientExecution.ProductionCostCoefficientExecutionPlants = coefficientExecutionTemp
								.ProductionCostCoefficientExecutionPlants
								.Where(d => d.isActive)
								.Select(d => new ProductionCostCoefficientExecutionPlant()
								{
									id_productionPlant = d.id_productionPlant,
									id_inventoryLine = d.id_inventoryLine,
									id_itemType = d.id_itemType,
									libras = d.libras,
									porcentaje = d.porcentaje,
									valor = d.valor,
									coeficiente = d.coeficiente,
									isActive = true,

									id_userCreate = this.ActiveUserId,
									dateCreate = DateTime.Now,
									id_userUpdate = this.ActiveUserId,
									dateUpdate = DateTime.Now,
								})
								.ToList();
						}

						if (coefficientExecutionTemp.ProductionCostCoefficientExecutionWarehouses != null)
						{
							coefficientExecution.ProductionCostCoefficientExecutionWarehouses = coefficientExecutionTemp
								.ProductionCostCoefficientExecutionWarehouses
								.Where(d => d.isActive)
								.Select(d => new ProductionCostCoefficientExecutionWarehouse()
								{
									id_warehouse = d.id_warehouse,
									id_inventoryLine = d.id_inventoryLine,
									id_itemType = d.id_itemType,
									id_poundType = d.id_poundType,
									libras = d.libras,
									porcentaje = d.porcentaje,
									valor = d.valor,
									coeficiente = d.coeficiente,
									isActive = true,

									id_userCreate = this.ActiveUserId,
									dateCreate = DateTime.Now,
									id_userUpdate = this.ActiveUserId,
									dateUpdate = DateTime.Now,
								})
								.ToList();
						}
					}

					// Guardamos el documento
					db.Document.Add(document);
					db.ProductionCostCoefficientExecution.Add(coefficientExecution);
					db.SaveChanges();
					transaction.Commit();

					idCoefficientExecution = coefficientExecution.id;
					message = "Documento creado exitosamente.";
					isValid = true;
				}
				catch (Exception exception)
				{
					transaction.Rollback();

					this.TempData.Keep(m_CostCoefficientExecutionModelKey);

					idCoefficientExecution = null;
					message = "Error al crear documento: " + exception.Message;
					isValid = false;
				}
			}

			var result = new
			{
				idCoefficientExecution,
				message,
				isValid,
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public JsonResult Save(int idCoefficientExecution,
			int anio, int mes, DateTime startDate, DateTime endDate,
			int idAllocationType, string description, string reference,
			int[] detailsIds, bool approveDocument)
		{
			int? idCoefficientExecutionResult;
			string message;
			bool isValid;

			detailsIds = detailsIds ?? new int[] { };

			using (var transaction = db.Database.BeginTransaction())
			{
				try
				{
					// Recuperar la entidad actual
					var coefficientExecution = db.ProductionCostCoefficientExecution
						.First(c => c.id == idCoefficientExecution);

					// Verificamos el estado actual del documento
					var documentStateCode = coefficientExecution.Document.DocumentState.code;

					if (documentStateCode != m_PendienteDocumentState)
					{
						throw new ApplicationException("Acción es permitida solo para Documento en estado PENDIENTE.");
					}

					// Actualizamos el documento
					var document = coefficientExecution.Document;

					document.emissionDate = startDate;
					document.description = description;
					document.reference = reference;

					document.id_userUpdate = this.ActiveUserId;
					document.dateUpdate = DateTime.Now;

					// Actualizamos el documento de ejecución de coeficientes
					coefficientExecution.id_allocationType = idAllocationType;
					coefficientExecution.anio = anio;
					coefficientExecution.mes = mes;
					coefficientExecution.startDate = startDate;
					coefficientExecution.endDate = endDate;
					coefficientExecution.processed = false;
					coefficientExecution.description = description;

					coefficientExecution.id_userUpdate = this.ActiveUserId;
					coefficientExecution.dateUpdate = DateTime.Now;

					// Actualizar los detalles de la ejecución de coeficientes
					var coefficientExecutionTemp = (this.TempData[m_CostCoefficientExecutionModelKey] as ProductionCostCoefficientExecution);
					var tieneDetallesActivos = false;
					var tieneDetallesDistribucionPlantaActivos = false;
					var tieneDetallesDistribucionBodegaActivos = false;
					var idsWarehouses = new int[] { };

					if (coefficientExecutionTemp != null)
					{
						coefficientExecution.processed = coefficientExecutionTemp.processed;

						if (approveDocument && coefficientExecutionTemp.processed)
						{
							// Si es aprobación, se recalcula la distribución de libras
							this.CalcularDetallesDistribucionCoeficientes(coefficientExecutionTemp, detailsIds);
						}

						if ((coefficientExecutionTemp.ProductionCostCoefficientExecutionDetails != null)
							&& (coefficientExecutionTemp.processed))
						{
							var productionCoefficientDetailsTemp = coefficientExecutionTemp
								.ProductionCostCoefficientExecutionDetails
								.Where(d => d.isActive && detailsIds.Contains(d.id))
								.ToList();

							tieneDetallesActivos = tieneDetallesActivos
								|| productionCoefficientDetailsTemp.Any();

							// Sobreescribimos todos los detalles actuales con los nuevos valores, si hubiera...
							foreach (var detail in coefficientExecution.ProductionCostCoefficientExecutionDetails)
							{
								if (productionCoefficientDetailsTemp.Any())
								{
									// Actualizamos los detalles
									var detailTemp = productionCoefficientDetailsTemp[0];
									productionCoefficientDetailsTemp.RemoveAt(0);

									detail.id_allocationPeriod = detailTemp.id_allocationPeriod;
									detail.id_allocationPeriodDetail = detailTemp.id_allocationPeriodDetail;
									detail.anio = detailTemp.anio;
									detail.mes = detailTemp.mes;
									detail.id_coefficient = detailTemp.id_coefficient;
									detail.id_poundType = detailTemp.id_poundType;
									detail.id_simpleFormula = detailTemp.id_simpleFormula;
									detail.accountingValue = detailTemp.accountingValue;
									detail.id_productionCost = detailTemp.id_productionCost;
									detail.id_productionCostDetail = detailTemp.id_productionCostDetail;
									detail.id_productionPlant = detailTemp.id_productionPlant;
									detail.coeficiente = detailTemp.coeficiente;
									detail.valor = detailTemp.valor;
									detail.isActive = true;

									detail.id_userUpdate = this.ActiveUserId;
									detail.dateUpdate = DateTime.Now;
								}
								else
								{
									// Ya no hay detalles nuevos, desactivar...
									detail.anio = 0;
									detail.mes = 0;
									detail.accountingValue = false;
									detail.id_productionPlant = null;
									detail.coeficiente = false;
									detail.valor = 0;
									detail.isActive = false;

									detail.id_userUpdate = this.ActiveUserId;
									detail.dateUpdate = DateTime.Now;
								}
							}

							// Agregamos los detalles que faltan de agregar
							foreach (var detailTemp in productionCoefficientDetailsTemp)
							{
								coefficientExecution.ProductionCostCoefficientExecutionDetails
									.Add(new ProductionCostCoefficientExecutionDetail()
									{
										id_coefficientExecution = idCoefficientExecution,
										id_allocationPeriod = detailTemp.id_allocationPeriod,
										id_allocationPeriodDetail = detailTemp.id_allocationPeriodDetail,
										anio = detailTemp.anio,
										mes = detailTemp.mes,
										id_coefficient = detailTemp.id_coefficient,
										id_poundType = detailTemp.id_poundType,
										id_simpleFormula = detailTemp.id_simpleFormula,
										accountingValue = detailTemp.accountingValue,
										id_productionCost = detailTemp.id_productionCost,
										id_productionCostDetail = detailTemp.id_productionCostDetail,
										id_productionPlant = detailTemp.id_productionPlant,
										coeficiente = detailTemp.coeficiente,
										valor = detailTemp.valor,
										isActive = true,

										id_userCreate = this.ActiveUserId,
										dateCreate = DateTime.Now,
										id_userUpdate = this.ActiveUserId,
										dateUpdate = DateTime.Now,
									});
							}
						}
						else
						{
							// No hay detalles: desactivar todos los elementos actuales

							foreach (var detail in coefficientExecution.ProductionCostCoefficientExecutionDetails)
							{
								detail.anio = 0;
								detail.mes = 0;
								detail.accountingValue = false;
								detail.id_productionPlant = null;
								detail.coeficiente = false;
								detail.valor = 0;
								detail.isActive = false;

								detail.id_userUpdate = this.ActiveUserId;
								detail.dateUpdate = DateTime.Now;
							}
						}


						if ((coefficientExecutionTemp.ProductionCostCoefficientExecutionPlants != null)
							&& (coefficientExecutionTemp.processed))
						{
							var productionCoefficientPlantsTemp = coefficientExecutionTemp
								.ProductionCostCoefficientExecutionPlants
								.Where(d => d.isActive)
								.ToList();

							tieneDetallesDistribucionPlantaActivos = tieneDetallesDistribucionPlantaActivos
								|| productionCoefficientPlantsTemp.Any();

							// Sobreescribimos todos los detalles actuales con los nuevos valores, si hubiera...
							foreach (var detail in coefficientExecution.ProductionCostCoefficientExecutionPlants)
							{
								if (productionCoefficientPlantsTemp.Any())
								{
									// Actualizamos los detalles
									var detailTemp = productionCoefficientPlantsTemp[0];
									productionCoefficientPlantsTemp.RemoveAt(0);

									detail.id_productionPlant = detailTemp.id_productionPlant;
									detail.id_inventoryLine = detailTemp.id_inventoryLine;
									detail.id_itemType = detailTemp.id_itemType;
									detail.libras = detailTemp.libras;
									detail.porcentaje = detailTemp.porcentaje;
									detail.valor = detailTemp.valor;
									detail.coeficiente = detailTemp.coeficiente;
									detail.isActive = true;

									detail.id_userUpdate = this.ActiveUserId;
									detail.dateUpdate = DateTime.Now;
								}
								else
								{
									// Ya no hay detalles nuevos, desactivar...
									detail.libras = 0;
									detail.porcentaje = 0;
									detail.valor = 0;
									detail.coeficiente = 0;
									detail.isActive = false;

									detail.id_userUpdate = this.ActiveUserId;
									detail.dateUpdate = DateTime.Now;
								}
							}

							// Agregamos los detalles que faltan de agregar
							foreach (var detailTemp in productionCoefficientPlantsTemp)
							{
								coefficientExecution.ProductionCostCoefficientExecutionPlants
									.Add(new ProductionCostCoefficientExecutionPlant()
									{
										id_coefficientExecution = idCoefficientExecution,
										id_productionPlant = detailTemp.id_productionPlant,
										id_inventoryLine = detailTemp.id_inventoryLine,
										id_itemType = detailTemp.id_itemType,
										libras = detailTemp.libras,
										porcentaje = detailTemp.porcentaje,
										valor = detailTemp.valor,
										coeficiente = detailTemp.coeficiente,
										isActive = true,

										id_userCreate = this.ActiveUserId,
										dateCreate = DateTime.Now,
										id_userUpdate = this.ActiveUserId,
										dateUpdate = DateTime.Now,
									});
							}
						}
						else
						{
							// No hay detalles: desactivar todos los elementos actuales

							foreach (var detail in coefficientExecution.ProductionCostCoefficientExecutionPlants)
							{
								detail.libras = 0;
								detail.porcentaje = 0;
								detail.valor = 0;
								detail.coeficiente = 0;
								detail.isActive = false;

								detail.id_userUpdate = this.ActiveUserId;
								detail.dateUpdate = DateTime.Now;
							}
						}


						if ((coefficientExecutionTemp.ProductionCostCoefficientExecutionWarehouses != null)
							&& (coefficientExecutionTemp.processed))
						{
							var productionCoefficientWarehousesTemp = coefficientExecutionTemp
								.ProductionCostCoefficientExecutionWarehouses
								.Where(d => d.isActive)
								.ToList();

							tieneDetallesDistribucionBodegaActivos = tieneDetallesDistribucionBodegaActivos
								|| productionCoefficientWarehousesTemp.Any();

							idsWarehouses = productionCoefficientWarehousesTemp
								.Select(d => d.id_warehouse)
								.Distinct()
								.ToArray();

							// Sobreescribimos todos los detalles actuales con los nuevos valores, si hubiera...
							foreach (var detail in coefficientExecution.ProductionCostCoefficientExecutionWarehouses)
							{
								if (productionCoefficientWarehousesTemp.Any())
								{
									// Actualizamos los detalles
									var detailTemp = productionCoefficientWarehousesTemp[0];
									productionCoefficientWarehousesTemp.RemoveAt(0);

									detail.id_warehouse = detailTemp.id_warehouse;
									detail.id_inventoryLine = detailTemp.id_inventoryLine;
									detail.id_itemType = detailTemp.id_itemType;
									detail.id_poundType = detailTemp.id_poundType;
									detail.libras = detailTemp.libras;
									detail.porcentaje = detailTemp.porcentaje;
									detail.valor = detailTemp.valor;
									detail.coeficiente = detailTemp.coeficiente;
									detail.isActive = true;

									detail.id_userUpdate = this.ActiveUserId;
									detail.dateUpdate = DateTime.Now;
								}
								else
								{
									// Ya no hay detalles nuevos, desactivar...
									detail.libras = 0;
									detail.porcentaje = 0;
									detail.valor = 0;
									detail.coeficiente = 0;
									detail.isActive = false;

									detail.id_userUpdate = this.ActiveUserId;
									detail.dateUpdate = DateTime.Now;
								}
							}

							// Agregamos los detalles que faltan de agregar
							foreach (var detailTemp in productionCoefficientWarehousesTemp)
							{
								coefficientExecution.ProductionCostCoefficientExecutionWarehouses
									.Add(new ProductionCostCoefficientExecutionWarehouse()
									{
										id_coefficientExecution = idCoefficientExecution,
										id_warehouse = detailTemp.id_warehouse,
										id_inventoryLine = detailTemp.id_inventoryLine,
										id_itemType = detailTemp.id_itemType,
										id_poundType = detailTemp.id_poundType,
										libras = detailTemp.libras,
										porcentaje = detailTemp.porcentaje,
										valor = detailTemp.valor,
										coeficiente = detailTemp.coeficiente,
										isActive = true,

										id_userCreate = this.ActiveUserId,
										dateCreate = DateTime.Now,
										id_userUpdate = this.ActiveUserId,
										dateUpdate = DateTime.Now,
									});
							}
						}
						else
						{
							// No hay detalles: desactivar todos los elementos actuales

							foreach (var detail in coefficientExecution.ProductionCostCoefficientExecutionWarehouses)
							{
								detail.libras = 0;
								detail.porcentaje = 0;
								detail.valor = 0;
								detail.coeficiente = 0;
								detail.isActive = false;

								detail.id_userUpdate = this.ActiveUserId;
								detail.dateUpdate = DateTime.Now;
							}
						}
					}


					// Verificamos si se trata de la aprobación del documento
					if (approveDocument)
					{
						#region Validamos la no existencia de registros de bodegas en negativo
						var bodegasTipoCalculoLibrasNegativo = coefficientExecution
							.ProductionCostCoefficientExecutionWarehouses
							.GroupBy(d => new
							{
								d.id_warehouse,
								d.id_itemType,
							})
							.Select(g => new
							{
								g.Key.id_warehouse,
								name_warehouse = g.First().Warehouse.name,
								g.Key.id_itemType,
								name_itemType = g.First().ItemType.name,
								totalLibras = g.Sum(x => x.libras),
							})
							.Where(e => e.totalLibras < 0m)
							.ToArray();

						if (bodegasTipoCalculoLibrasNegativo.Any())
						{
							var mensajesError = bodegasTipoCalculoLibrasNegativo
								.Select(e => $"En la bodega {e.name_warehouse}, el tipo de ítem {e.name_itemType} tiene libras en negativo.");

							throw new Exception("Existen errores: " + string.Join("", mensajesError));
						}
						#endregion

						// Solo se aprueba si se tienen detalles
						if (!tieneDetallesActivos)
						{
							throw new ApplicationException("No se puede aprobar el documento sin detalles.");
						}
						if (!tieneDetallesDistribucionPlantaActivos)
						{
							throw new ApplicationException("No se puede aprobar el documento sin datos de distribución por planta.");
						}
						if (!tieneDetallesDistribucionBodegaActivos)
						{
							throw new ApplicationException("No se puede aprobar el documento sin datos de distribución por bodegas.");
						}


						// Recuperamos el estado APROBADO
						var documentState = DataProviderCostCoefficientExecution
							.GetDocumentStateByCode(this.ActiveCompanyId, m_AprobadoDocumentState);

						if (documentState == null)
						{
							throw new ApplicationException("No existe registrado el estado APROBADO para los documentos.");
						}

						document.id_documentState = documentState.id;
					}

					// Guardamos el documento
					db.SaveChanges();


					// Procedemos a actualizar los estados de períodos de las bodegas, solo si es aprobación
					if (approveDocument)
					{
						// Los estados de períodos de bodega aplican solo en el caso de asignación REAL
						var idTipoAsignacionCostoReal = this.db.ProductionCostAllocationType
							.FirstOrDefault(t => t.code == "REAL")?
							.id ?? 0;

						if (coefficientExecutionTemp.id_allocationType == idTipoAsignacionCostoReal)
						{
							this.CerrarPeriodoInventario(anio, mes, idsWarehouses);
							db.SaveChanges();
						}
					}

					transaction.Commit();

					idCoefficientExecutionResult = coefficientExecution.id;
					message = approveDocument
						? "Documento aprobado exitosamente."
						: "Documento actualizado exitosamente.";
					isValid = true;
				}
				catch (Exception exception)
				{
					transaction.Rollback();

					this.TempData.Keep(m_CostCoefficientExecutionModelKey);

					idCoefficientExecutionResult = null;
					message = approveDocument
						? "Error al aprobar documento: " + exception.Message
						: "Error al actualizar documento: " + exception.Message;
					isValid = false;
				}
			}

			var result = new
			{
				idCoefficientExecution = idCoefficientExecutionResult,
				message,
				isValid,
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public JsonResult Cancel(int idCoefficientExecution)
		{
			int? idCoefficientExecutionResult;
			string message;
			bool isValid;

			using (var transaction = db.Database.BeginTransaction())
			{
				try
				{
					// Recuperar la entidad actual
					var coefficientExecution = db.ProductionCostCoefficientExecution
						.First(c => c.id == idCoefficientExecution);

					// Verificamos el estado actual del documento
					var documentStateCode = coefficientExecution.Document.DocumentState.code;

					if (documentStateCode != m_PendienteDocumentState)
					{
						throw new ApplicationException("Acción es permitida solo para Documento en estado PENDIENTE.");
					}

					// Verificamos si el documento NO ha sido valorizado
					if (coefficientExecution.value_processed)
					{
						throw new ApplicationException("Acción NO permitida porque el Documento ya ha sido valorizado.");
					}

					// Recuperamos el estado ANULADO
					var documentState = DataProviderCostCoefficientExecution
						.GetDocumentStateByCode(this.ActiveCompanyId, m_AnuladoDocumentState);

					if (documentState == null)
					{
						throw new ApplicationException("No existe registrado el estado CANCELADO para los documentos.");
					}

					// Actualizamos el documento
					var document = coefficientExecution.Document;

					document.id_documentState = documentState.id;

					document.id_userUpdate = this.ActiveUserId;
					document.dateUpdate = DateTime.Now;

					// Anulamos el documento
					db.SaveChanges();
					transaction.Commit();

					idCoefficientExecutionResult = coefficientExecution.id;
					message = "Documento anulado exitosamente.";
					isValid = true;
				}
				catch (Exception exception)
				{
					transaction.Rollback();

					this.TempData.Keep(m_CostCoefficientExecutionModelKey);

					idCoefficientExecutionResult = null;
					message = "Error al anular documento: " + exception.Message;
					isValid = false;
				}
			}

			var result = new
			{
				idCoefficientExecution = idCoefficientExecutionResult,
				message,
				isValid,
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public JsonResult Revert(int idCoefficientExecution)
		{
			int? idCoefficientExecutionResult;
			string message;
			bool isValid;

			using (var transaction = db.Database.BeginTransaction())
			{
				try
				{
					// Recuperar la entidad actual
					var coefficientExecution = db.ProductionCostCoefficientExecution
						.First(c => c.id == idCoefficientExecution);

					// Verificamos el estado actual del documento
					var documentStateCode = coefficientExecution.Document.DocumentState.code;

					if (documentStateCode != m_AprobadoDocumentState)
					{
						throw new ApplicationException("Acción es permitida solo para Documento en estado APROBADO.");
					}

					// Verificamos si el documento NO ha sido valorizado
					if (coefficientExecution.value_processed)
					{
						throw new ApplicationException("Acción NO permitida porque el Documento ya ha sido valorizado.");
					}

					// Recuperamos el estado PENDIENTE
					var documentState = DataProviderCostCoefficientExecution
						.GetDocumentStateByCode(this.ActiveCompanyId, m_PendienteDocumentState);

					if (documentState == null)
					{
						throw new ApplicationException("No existe registrado el estado PENDIENTE para los documentos.");
					}

					// Actualizamos el documento
					var document = coefficientExecution.Document;

					document.id_documentState = documentState.id;

					document.id_userUpdate = this.ActiveUserId;
					document.dateUpdate = DateTime.Now;

					// Reversamos el documento
					db.SaveChanges();


					// Procedemos a actualizar los estados de períodos de las bodegas
					var anio = coefficientExecution.anio;
					var mes = coefficientExecution.mes;
					var idsWarehouses = coefficientExecution
						.ProductionCostCoefficientExecutionWarehouses
						.Where(d => d.isActive)
						.Select(d => d.id_warehouse)
						.Distinct()
						.ToArray();

					this.ReversarCierrePeriodoInventario(anio, mes, idsWarehouses);
					db.SaveChanges();


					transaction.Commit();

					idCoefficientExecutionResult = coefficientExecution.id;
					message = "Documento reversado exitosamente.";
					isValid = true;
				}
				catch (Exception exception)
				{
					transaction.Rollback();

					this.TempData.Keep(m_CostCoefficientExecutionModelKey);

					idCoefficientExecutionResult = null;
					message = "Error al reversar documento: " + exception.Message;
					isValid = false;
				}
			}

			var result = new
			{
				idCoefficientExecution = idCoefficientExecutionResult,
				message,
				isValid,
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public JsonResult Execute(int idCoefficientExecution,
			int anio, int mes, DateTime startDate, DateTime endDate,
			int idAllocationType)
		{
			int? idCoefficientExecutionResult;
			string message;
			bool isValid;

			try
			{
				var coefficientExecution = this.GetEditingCostCoefficientExecution(idCoefficientExecution);

				coefficientExecution.id_allocationType = idAllocationType;
				coefficientExecution.anio = anio;
				coefficientExecution.mes = mes;
				coefficientExecution.startDate = startDate;
				coefficientExecution.endDate = endDate;

				// Calcular los coeficientes...
				this.CalcularDetallesEjecucionCoeficientes(coefficientExecution);

				this.TempData[m_CostCoefficientExecutionModelKey] = coefficientExecution;
				this.TempData.Keep(m_CostCoefficientExecutionModelKey);

				idCoefficientExecutionResult = coefficientExecution.id;
				message = "Documento procesado exitosamente.";
				isValid = true;
			}
			catch (Exception exception)
			{
				this.TempData.Keep(m_CostCoefficientExecutionModelKey);

				idCoefficientExecutionResult = null;
				message = "Error al procesar documento: " + exception.Message;
				isValid = false;
			}

			var result = new
			{
				idCoefficientExecution = idCoefficientExecutionResult,
				message,
				isValid,
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public JsonResult Reset(int idCoefficientExecution)
		{
			int? idCoefficientExecutionResult;
			string message;
			bool isValid;

			try
			{
				var coefficientExecution = this.GetEditingCostCoefficientExecution(idCoefficientExecution);

				// Eliminar los detalles y marcar como no procesado...
				coefficientExecution.processed = false;
				coefficientExecution.ProductionCostCoefficientExecutionDetails = new List<ProductionCostCoefficientExecutionDetail>();
				coefficientExecution.ProductionCostCoefficientExecutionPlants = new List<ProductionCostCoefficientExecutionPlant>();
				coefficientExecution.ProductionCostCoefficientExecutionWarehouses = new List<ProductionCostCoefficientExecutionWarehouse>();

				this.TempData[m_CostCoefficientExecutionModelKey] = coefficientExecution;
				this.TempData.Keep(m_CostCoefficientExecutionModelKey);

				idCoefficientExecutionResult = coefficientExecution.id;
				message = "Documento restablecido exitosamente.";
				isValid = true;
			}
			catch (Exception exception)
			{
				this.TempData.Keep(m_CostCoefficientExecutionModelKey);

				idCoefficientExecutionResult = null;
				message = "Error al restablecer documento: " + exception.Message;
				isValid = false;
			}

			var result = new
			{
				idCoefficientExecution = idCoefficientExecutionResult,
				message,
				isValid,
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public JsonResult Distribute(int idCoefficientExecution, int[] detailsIds)
		{
			int? idCoefficientExecutionResult;
			string message;
			bool isValid;

			try
			{
				var coefficientExecution = this.GetEditingCostCoefficientExecution(idCoefficientExecution);

				// Calcular la distribución de libras...
				this.CalcularDetallesDistribucionCoeficientes(coefficientExecution, detailsIds);

				this.TempData[m_CostCoefficientExecutionModelKey] = coefficientExecution;
				this.TempData.Keep(m_CostCoefficientExecutionModelKey);

				idCoefficientExecutionResult = coefficientExecution.id;
				message = "Distribución calculada exitosamente.";
				isValid = true;
			}
			catch (Exception exception)
			{
				this.TempData.Keep(m_CostCoefficientExecutionModelKey);

				idCoefficientExecutionResult = null;
				message = "Error al calcular la distribución: " + exception.Message;
				isValid = false;
			}

			var result = new
			{
				idCoefficientExecution = idCoefficientExecutionResult,
				message,
				isValid,
			};

			return Json(result, JsonRequestBehavior.AllowGet);
		}


		#endregion

		#region Manejadores de los Detalles de la Ejecución

		public PartialViewResult CostCoefficientExecutionDetails(int idCoefficientExecution)
		{
			var coefficientExecution = this.GetEditingCostCoefficientExecution(idCoefficientExecution);

			this.TempData[m_CostCoefficientExecutionModelKey] = coefficientExecution;
			this.TempData.Keep(m_CostCoefficientExecutionModelKey);
			this.PrepareDetailsEditViewBag(true, coefficientExecution.processed);

			return this.PartialView("_CoefficientExecutionDetailsPartial", coefficientExecution);
		}

		public PartialViewResult CostCoefficientExecutionDistributionDetails(int idCoefficientExecution)
		{
			var coefficientExecution = this.GetEditingCostCoefficientExecution(idCoefficientExecution);

			this.TempData[m_CostCoefficientExecutionModelKey] = coefficientExecution;
			this.TempData.Keep(m_CostCoefficientExecutionModelKey);
			this.PrepareDetailsEditViewBag(true, coefficientExecution.processed);

			return this.PartialView("_CoefficientExecutionDistributionDetailsPartial", coefficientExecution);
		}

		public PartialViewResult CostCoefficientExecutionWarehouseDetails(int idCoefficientExecution)
		{
			var coefficientExecution = this.GetEditingCostCoefficientExecution(idCoefficientExecution);

			this.TempData[m_CostCoefficientExecutionModelKey] = coefficientExecution;
			this.TempData.Keep(m_CostCoefficientExecutionModelKey);
			this.PrepareDetailsEditViewBag(true, coefficientExecution.processed);

			return this.PartialView("_CoefficientExecutionWarehouseDetailsPartial", coefficientExecution);
		}

		#endregion

		#region Métodos adicionales

		private ProductionCostCoefficientExecution GetEditingCostCoefficientExecution(int? id_coefficientExecution)
		{
			// Recuperamos el elemento del caché local
			var coefficientExecution = (this.TempData[m_CostCoefficientExecutionModelKey] as ProductionCostCoefficientExecution);

			// Si no hay elemento en el caché, consultamos desde la base
			if ((coefficientExecution == null) && id_coefficientExecution.HasValue)
			{
				coefficientExecution = db.ProductionCostCoefficientExecution
					.Include("Document")
					.Include("ProductionCostAllocationType")
					.Include("ProductionCostCoefficientExecutionDetails")
					.Include("ProductionCostCoefficientExecutionDetails.ProductionCost")
					.Include("ProductionCostCoefficientExecutionDetails.ProductionCostDetail")
					.Include("ProductionCostCoefficientExecutionDetails.ProductionCostAllocationPeriod")
					.Include("ProductionCostCoefficientExecutionDetails.ProductionCostAllocationPeriod.Document")
					.Include("ProductionCostCoefficientExecutionPlants")
					.FirstOrDefault(i => i.id == id_coefficientExecution);
			}

			if (coefficientExecution != null)
			{
				if (coefficientExecution.processed)
				{
					coefficientExecution.ProductionCostCoefficientExecutionDetails = coefficientExecution
						.ProductionCostCoefficientExecutionDetails
						.Where(d => d.isActive)
						.ToList();

					coefficientExecution.ProductionCostCoefficientExecutionPlants = coefficientExecution
						.ProductionCostCoefficientExecutionPlants
						.Where(d => d.isActive)
						.ToList();

					coefficientExecution.ProductionCostCoefficientExecutionWarehouses = coefficientExecution
						.ProductionCostCoefficientExecutionWarehouses
						.Where(d => d.isActive)
						.ToList();
				}
				else
				{
					coefficientExecution.ProductionCostCoefficientExecutionDetails = new List<ProductionCostCoefficientExecutionDetail>();
					coefficientExecution.ProductionCostCoefficientExecutionPlants = new List<ProductionCostCoefficientExecutionPlant>();
					coefficientExecution.ProductionCostCoefficientExecutionWarehouses = new List<ProductionCostCoefficientExecutionWarehouse>();
				}
			}

			// Si no existe, creamos un nuevo elemento
			return coefficientExecution ?? this.CreateNewCostCoefficientExecution();
		}

		private ProductionCostCoefficientExecution CreateNewCostCoefficientExecution()
		{
			// Recuperamos el tipo de documento
			var documentType = db.DocumentType
				.FirstOrDefault(dt => dt.code == m_TipoDocumentoCostCoefficientExecution
									&& dt.id_company == this.ActiveCompanyId);

			if (documentType == null)
			{
				throw new ApplicationException("No existe registrado el tipo de documento: Ejecución de Coeficientes.");
			}

			// Recuperamos el estado PENDIENTE
			var documentState = DataProviderCostCoefficientExecution
				.GetDocumentStateByCode(this.ActiveCompanyId, m_PendienteDocumentState);

			if (documentState == null)
			{
				throw new ApplicationException("No existe registrado el estado PENDIENTE para los documentos.");
			}

			// Recuperamos el tipo de asignación COSTO PROYECTADO
			var allocationType = db.ProductionCostAllocationType
				.FirstOrDefault(t => t.code == "PROJ");

			if (allocationType == null)
			{
				throw new ApplicationException("No existe registrado el tipo de asignación COSTO PROYECTADO [PROJ].");
			}

			// Calculamos el secuencial y el número de documento siguiente
			var documentSequential = documentType.currentNumber;
			var documentNumber = $"{this.ActiveEmissionPoint.BranchOffice.code:000}-{this.ActiveEmissionPoint.code:000}-{documentSequential}";

			// Preparamos el documento
			var document = new Document()
			{
				number = documentNumber,
				sequential = documentSequential,
				emissionDate = DateTime.Today,

				DocumentType = documentType,
				DocumentState = documentState,
			};

			// Creamos el documento de asignación de costos
			return new ProductionCostCoefficientExecution()
			{
				Document = document,
				id_allocationType = allocationType.id,
				ProductionCostAllocationType = allocationType,
				anio = DateTime.Today.Year,
				mes = DateTime.Today.Month,
				startDate = DateTime.Today.AddDays(1 - DateTime.Today.Day),
				endDate = DateTime.Today,
			};
		}

		private void PrepareEditViewBag(ProductionCostCoefficientExecution coefficientExecution)
		{
			// Verificamos el estado actual del documento
			var documentStateCode = coefficientExecution.Document.DocumentState.code;
			var documentExists = coefficientExecution.id > 0;
			var canEditDocument = (documentStateCode == m_PendienteDocumentState);
			var estaProcesadoDocumento = coefficientExecution.processed;
			var estaValorizadoDocumento = coefficientExecution.value_processed;

			// Agregamos los elementos al ViewBag
			this.ViewBag.DocumentoExistente = documentExists;
			this.ViewBag.PuedeEditarDocumento = canEditDocument;
			this.ViewBag.EstaProcesadoDocumento = estaProcesadoDocumento;
			this.ViewBag.PuedeAprobarDocumento = documentExists && (documentStateCode == m_PendienteDocumentState);
			this.ViewBag.PuedeReversarDocumento = documentExists && !estaValorizadoDocumento && (documentStateCode == m_AprobadoDocumentState);
			this.ViewBag.PuedeAnularDocumento = documentExists && !estaValorizadoDocumento && (documentStateCode == m_PendienteDocumentState);

			// Agregamos elementos auxiliares de control de períodos
			if (canEditDocument)
			{
				// Si el documento es editable --> lista de años y periodos disponibles para uso
				var periodosUsables = this.db.InventoryValuationPeriodDetail
					.Where(d => !d.isClosed && d.InventoryValuationPeriod.isActive
								&& (d.id_PeriodState == 2 || d.id_PeriodState == 3))
					.OrderBy(d => d.InventoryValuationPeriod.year).ThenBy(d => d.periodNumber)
					.ToArray()
					.Select(d => new
					{
						Anio = d.InventoryValuationPeriod.year,
						Mes = d.periodNumber,
						FechaInicio = Int32.Parse(d.dateInit.ToString("yyyyMMdd")),
						FechaFinal = Int32.Parse(d.dateEnd.ToString("yyyyMMdd")),
						IsActive = (d.id_PeriodState == 2),
					})
					.ToArray();

				this.ViewBag.PeriodosUsables = periodosUsables;

				this.ViewBag.AnioListModel = periodosUsables
					.Select(d => new { d.Anio })
					.Distinct()
					.ToArray();

				this.ViewBag.MesListModel = periodosUsables
					.Where(d => d.Anio == coefficientExecution.anio)
					.Select(d => new { d.Mes })
					.Distinct()
					.ToArray();


				var idTipoAsignacionCostoProyectado = this.db.ProductionCostAllocationType
					.FirstOrDefault(t => t.code == "PROJ")?
					.id ?? 0;

				this.ViewBag.IdTipoAsignacionCostoProyectado = idTipoAsignacionCostoProyectado;
				this.ViewBag.PuedeEditarCamposFecha = (coefficientExecution.id_allocationType == idTipoAsignacionCostoProyectado);
			}
			else
			{
				this.ViewBag.PeriodosUsables = new object[] { };

				this.ViewBag.AnioListModel = new[]
				{
					new { Anio = coefficientExecution.anio, },
				};

				this.ViewBag.MesListModel = new[]
				{
					new { Mes = coefficientExecution.mes, },
				};

				this.ViewBag.IdTipoAsignacionCostoProyectado = 0;
				this.ViewBag.PuedeEditarCamposFecha = false;
			}

			this.PrepareDetailsEditViewBag(canEditDocument, estaProcesadoDocumento);
		}

		private void PrepareDetailsEditViewBag(bool editable, bool estaProcesado)
		{
			// Agregamos los elementos al ViewBag
			this.ViewBag.DetailsEditable = editable;
			this.ViewBag.EstaProcesadoDocumento = estaProcesado;
		}

		#endregion

		#region Cálculos de coeficientes, costos y distribución

		private void CalcularDetallesEjecucionCoeficientes(ProductionCostCoefficientExecution coefficientExecution)
		{
			// Recuperamos las asignaciones de costo aprobadas para el período indicado
			var idAprobadoDocumentState = DataProviderCostCoefficientExecution
				.GetDocumentStateByCode(this.ActiveCompanyId, m_AprobadoDocumentState)?
				.id;

			var asignacionesCosto = this.db.ProductionCostAllocationPeriod
				.Where(a => a.anio == coefficientExecution.anio
							&& a.mes == coefficientExecution.mes
							&& a.Document.id_documentState == idAprobadoDocumentState)
				.ToArray();

			// Procesamos las asignaciones y generamos los detalles de la ejecución
			var idEjecucionCoeficienteDetalle = 1;
			var costCoefficientExecutionDetails = new List<ProductionCostCoefficientExecutionDetail>();

			foreach (var asignacionCosto in asignacionesCosto)
			{
				foreach (var asignacionCostoDetalle in asignacionCosto.ProductionCostAllocationPeriodDetails)
				{
					costCoefficientExecutionDetails.Add(new ProductionCostCoefficientExecutionDetail()
					{
						id = idEjecucionCoeficienteDetalle++,

						id_allocationPeriod = asignacionCosto.id,
						ProductionCostAllocationPeriod = asignacionCosto,

						id_allocationPeriodDetail = asignacionCostoDetalle.id,
						ProductionCostAllocationPeriodDetail = asignacionCostoDetalle,

						anio = coefficientExecution.anio,
						mes = coefficientExecution.mes,

						id_coefficient = asignacionCosto.id_coefficient,
						ProductionCostCoefficient = asignacionCosto.ProductionCostCoefficient,

						id_poundType = asignacionCosto.ProductionCostCoefficient.id_poundType,
						ProductionCostPoundType = asignacionCosto.ProductionCostCoefficient.ProductionCostPoundType,

						id_simpleFormula = asignacionCosto.ProductionCostCoefficient.id_simpleFormula,
						SimpleFormula = asignacionCosto.ProductionCostCoefficient.SimpleFormula,

						accountingValue = asignacionCosto.accountingValue,

						id_productionCost = asignacionCostoDetalle.id_productionCost,
						ProductionCost = asignacionCostoDetalle.ProductionCost,

						id_productionCostDetail = asignacionCostoDetalle.id_productionCostDetail,
						ProductionCostDetail = asignacionCostoDetalle.ProductionCostDetail,

						id_productionPlant = asignacionCostoDetalle.id_productionPlant,
						ProductionPlant = asignacionCostoDetalle.ProductionPlant,

						coeficiente = asignacionCostoDetalle.coeficiente,
						valor = asignacionCostoDetalle.valor,
						isActive = true,
					});
				}
			}

			coefficientExecution.processed = true;
			coefficientExecution.ProductionCostCoefficientExecutionDetails = costCoefficientExecutionDetails;
			coefficientExecution.ProductionCostCoefficientExecutionPlants = new List<ProductionCostCoefficientExecutionPlant>();
			coefficientExecution.ProductionCostCoefficientExecutionWarehouses = new List<ProductionCostCoefficientExecutionWarehouse>();
		}



		private class ParametrosEjecucionCalculo
		{
			internal DateTime FechaInicio { get; set; }
			internal DateTime FechaFinal { get; set; }
			internal int? IdPlantaFiltro { get; set; }
			internal int[] IdsBodegas { get; set; }
			internal int[] IdsUbicacionesBodega { get; set; }
			internal int IdTipoCalculoLibras { get; set; }
		}

		private class ParametrosEjecucionCalculoCustom : ParametrosEjecucionCalculo
        {
			internal string PoundTypeCode { get; set; }
			internal string Formula { get; set; }
		}

		private class LibrasEjecucionCalculo
		{
			internal int IdPlantaProceso { get; set; }
			internal int IdBodega { get; set; }
			internal int IdLineaInventario { get; set; }
			internal int IdTipoItem { get; set; }
			internal int IdTipoCalculoLibras { get; set; }
			internal decimal TotalLibras { get; set; }
		}
		private class ResultadoEjecucionCalculo
		{
			internal bool EsEscalar { get; set; }
			internal decimal Escalar { get; set; }
			internal List<LibrasEjecucionCalculo> Vector { get; set; }
		}

		private class LibrasPorPlantaLineaTipo
		{
			internal int IdPlantaProceso { get; set; }
			internal Person PlantaProceso { get; set; }

			internal int IdLineaInventario { get; set; }
			internal InventoryLine LineaInventario { get; set; }

			internal int IdTipoItem { get; set; }
			internal ItemType TipoItem { get; set; }

			internal decimal TotalLibras { get; set; }

			internal decimal Porcentaje { get; set; }
			internal decimal Valor { get; set; }
			internal decimal Coeficiente { get; set; }
		}
		private class LibrasPorBodegaLineaTipo
		{
			internal int IdBodega { get; set; }
			internal Warehouse Bodega { get; set; }

			internal int IdLineaInventario { get; set; }
			internal InventoryLine LineaInventario { get; set; }

			internal int IdTipoItem { get; set; }
			internal ItemType TipoItem { get; set; }

			internal int IdTipoCalculoLibras { get; set; }
			internal ProductionCostPoundType TipoCalculoLibras { get; set; }

			internal decimal TotalLibras { get; set; }

			internal decimal Porcentaje { get; set; }
			internal decimal Valor { get; set; }
			internal decimal Coeficiente { get; set; }
		}

		private void CalcularDetallesDistribucionCoeficientes(ProductionCostCoefficientExecution coefficientExecution, int[] detailsIds)
		{
			// Verificamos que se puede calcular la distribución
			if (!coefficientExecution.processed
				|| (coefficientExecution.ProductionCostCoefficientExecutionDetails == null)
				|| (!coefficientExecution.ProductionCostCoefficientExecutionDetails.Any(d => d.isActive)))
			{
				// Si el coeficiente no está procesado, o no tiene detalles activos, no se puede calcular la distribución
				coefficientExecution.ProductionCostCoefficientExecutionPlants = new List<ProductionCostCoefficientExecutionPlant>();
				coefficientExecution.ProductionCostCoefficientExecutionWarehouses = new List<ProductionCostCoefficientExecutionWarehouse>();

				return;
			}


			// Nos preparamos para la ejecución de la distribución
			var montoTotalDolares = 0m;

			var parametrosEjecucionesCalculo = new List<ParametrosEjecucionCalculoCustom>();
			foreach (var coefficientExecutionDetail in coefficientExecution.ProductionCostCoefficientExecutionDetails)
			{
				// Si el detalle no está seleccionado, se lo ignora...
				if (!coefficientExecutionDetail.isActive || !detailsIds.Contains(coefficientExecutionDetail.id))
				{
					continue;
				}

				// Recuperamos el coeficiente relacionado al detalle y sus datos relacionados
				var coeficienteProduccion = db.ProductionCostCoefficient
					.Include("ProductionCostCoefficientWarehouses")
					.Include("ProductionCostCoefficientWarehouseLocations")
					.Include("ProductionCostPoundType")
					.First(d => d.id == coefficientExecutionDetail.id_coefficient);

				var idsBodegas = coeficienteProduccion
					.ProductionCostCoefficientWarehouses
					.Select(d => d.id_warehouse)
					.ToArray();

				if (!idsBodegas.Any() && coeficienteProduccion.id_warehouseType.HasValue)
				{
					// Si no hay bodegas, pero sí hay tipo de bodega, se recuperan las bodegas relacionadas al tipo
					idsBodegas = coeficienteProduccion
						.WarehouseType
						.Warehouse
						.Select(w => w.id)
						.ToArray();
				}

				var idsUbicacionesBodegas = coeficienteProduccion
					.ProductionCostCoefficientWarehouseLocations
					.Select(d => d.id_warehouseLocation)
					.ToArray();

				var poundTypeCode = coeficienteProduccion
					.ProductionCostPoundType
					.code;

				var formula = coeficienteProduccion
					.SimpleFormula
					.formulaTranslated;

				montoTotalDolares += coefficientExecutionDetail.valor;

				#region Preparamos el objeto para el procesamiento
				var index = parametrosEjecucionesCalculo
					.FindIndex(e => e.PoundTypeCode == poundTypeCode
						&& e.Formula == formula && e.FechaInicio == coefficientExecution.startDate
						&& e.FechaFinal == coefficientExecution.endDate && e.IdPlantaFiltro == coefficientExecutionDetail.id_productionPlant
						&& e.IdTipoCalculoLibras == coeficienteProduccion.id_poundType);

				if (index >= 0)
				{
					var parametro = parametrosEjecucionesCalculo.ElementAt(index);
					idsBodegas = idsBodegas.Union(parametro.IdsBodegas).Distinct().ToArray();
					idsUbicacionesBodegas = idsUbicacionesBodegas.Union(parametro.IdsUbicacionesBodega).Distinct().ToArray();

					parametro.IdsBodegas = idsBodegas;
					parametro.IdsUbicacionesBodega = idsUbicacionesBodegas;
				}
				else
				{
					parametrosEjecucionesCalculo.Add(new ParametrosEjecucionCalculoCustom()
					{
						PoundTypeCode = poundTypeCode,
						Formula = formula,
						FechaInicio = coefficientExecution.startDate,
						FechaFinal = coefficientExecution.endDate,
						IdPlantaFiltro = coefficientExecutionDetail.id_productionPlant,
						IdsBodegas = idsBodegas,
						IdsUbicacionesBodega = idsUbicacionesBodegas,
						IdTipoCalculoLibras = coeficienteProduccion.id_poundType,
					});
				}
				#endregion
			}

			// Procesamiento de libras
			var totalesLibrasEjecucionCalculo = new List<LibrasEjecucionCalculo>();
            foreach (var parametrosEjecucionCalculo in parametrosEjecucionesCalculo)
            {
				switch (parametrosEjecucionCalculo.PoundTypeCode)
				{
					case "LBREMIT":
						this.ProcesarCalculoLibrasRemitidas(totalesLibrasEjecucionCalculo, parametrosEjecucionCalculo);
						break;

					case "LBPROCE":
					case "LBTERMI":
					case "LBAMBAS":
						this.ProcesarCalculoLibrasInventarios(totalesLibrasEjecucionCalculo, parametrosEjecucionCalculo, parametrosEjecucionCalculo.Formula);
						break;

					default:
						throw new ApplicationException(
							$"Código de tipo de cálculo de libras no reconocido {parametrosEjecucionCalculo.PoundTypeCode}.");
				}
			}


			// Preparamos los detalles agrupados por planta y bodega
			var totalesLibrasPorPlantaLineaTipo = new List<LibrasPorPlantaLineaTipo>();
			var totalesLibrasPorBodegaLineaTipo = new List<LibrasPorBodegaLineaTipo>();

			foreach (var totalLibrasCalculo in totalesLibrasEjecucionCalculo)
			{
				// Acumulamos al detalle por tipo
				var detalleActualPorTipo = totalesLibrasPorPlantaLineaTipo
					.FirstOrDefault(r => r.IdPlantaProceso == totalLibrasCalculo.IdPlantaProceso
										&& r.IdLineaInventario == totalLibrasCalculo.IdLineaInventario
										&& r.IdTipoItem == totalLibrasCalculo.IdTipoItem);

				if (detalleActualPorTipo == null)
				{
					totalesLibrasPorPlantaLineaTipo.Add(new LibrasPorPlantaLineaTipo()
					{
						IdPlantaProceso = totalLibrasCalculo.IdPlantaProceso,
						IdLineaInventario = totalLibrasCalculo.IdLineaInventario,
						IdTipoItem = totalLibrasCalculo.IdTipoItem,
						TotalLibras = totalLibrasCalculo.TotalLibras,
					});
				}
				else
				{
					detalleActualPorTipo.TotalLibras += totalLibrasCalculo.TotalLibras;
				}

				// Acumulamos al detalle por bodega
				var detalleActualPorBodega = totalesLibrasPorBodegaLineaTipo
					.FirstOrDefault(r => r.IdBodega == totalLibrasCalculo.IdBodega
										&& r.IdLineaInventario == totalLibrasCalculo.IdLineaInventario
										&& r.IdTipoItem == totalLibrasCalculo.IdTipoItem
										&& r.IdTipoCalculoLibras == totalLibrasCalculo.IdTipoCalculoLibras);

				if (detalleActualPorBodega == null)
				{
					totalesLibrasPorBodegaLineaTipo.Add(new LibrasPorBodegaLineaTipo()
					{
						IdBodega = totalLibrasCalculo.IdBodega,
						IdLineaInventario = totalLibrasCalculo.IdLineaInventario,
						IdTipoItem = totalLibrasCalculo.IdTipoItem,
						IdTipoCalculoLibras = totalLibrasCalculo.IdTipoCalculoLibras,
						TotalLibras = totalLibrasCalculo.TotalLibras,
					});
				}
				else
				{
					detalleActualPorBodega.TotalLibras += totalLibrasCalculo.TotalLibras;
				}
			}

			// Agregamos las entidades faltantes a los objetos de resultado
			this.CompletarEntidadesLibrasEjecucionCalculo(
				totalesLibrasPorPlantaLineaTipo, totalesLibrasPorBodegaLineaTipo);

			this.CompletarIndicadoresEntidadesLibrasEjecucionCalculo(
				totalesLibrasPorPlantaLineaTipo, montoTotalDolares);
			this.CompletarIndicadoresEntidadesLibrasEjecucionCalculo(
				totalesLibrasPorBodegaLineaTipo, montoTotalDolares);


			// Ordenamos los elementos en las listas
			totalesLibrasPorPlantaLineaTipo = totalesLibrasPorPlantaLineaTipo
				.OrderBy(d => d.IdPlantaProceso)
					.ThenBy(d => d.LineaInventario.code).ThenBy(d => d.TipoItem.code)
				.ToList();

			totalesLibrasPorBodegaLineaTipo = totalesLibrasPorBodegaLineaTipo
				.OrderBy(d => d.IdBodega).ThenBy(d => d.TipoCalculoLibras.order)
					.ThenBy(d => d.LineaInventario.code).ThenBy(d => d.TipoItem.code)
				.ToList();


			// Asignamos los resultados del cálculo de distribución
			var idCoeficienteDetallePlantas = 1;
			var idCoeficienteDetalleBodegas = 1;

			coefficientExecution.ProductionCostCoefficientExecutionPlants = totalesLibrasPorPlantaLineaTipo
				.Select(d => new ProductionCostCoefficientExecutionPlant()
				{
					id = idCoeficienteDetallePlantas++,

					id_coefficientExecution = coefficientExecution.id,
					ProductionCostCoefficientExecution = coefficientExecution,

					id_productionPlant = d.IdPlantaProceso,
					ProductionPlant = d.PlantaProceso,

					id_inventoryLine = d.IdLineaInventario,
					InventoryLine = d.LineaInventario,

					id_itemType = d.IdTipoItem,
					ItemType = d.TipoItem,

					libras = d.TotalLibras,
					porcentaje = d.Porcentaje,
					valor = d.Valor,
					coeficiente = d.Coeficiente,

					isActive = true,
				})
				.ToList();

			coefficientExecution.ProductionCostCoefficientExecutionWarehouses = totalesLibrasPorBodegaLineaTipo
				.Select(d => new ProductionCostCoefficientExecutionWarehouse()
				{
					id = idCoeficienteDetalleBodegas++,

					id_coefficientExecution = coefficientExecution.id,
					ProductionCostCoefficientExecution = coefficientExecution,

					id_warehouse = d.IdBodega,
					Warehouse = d.Bodega,

					id_inventoryLine = d.IdLineaInventario,
					InventoryLine = d.LineaInventario,

					id_itemType = d.IdTipoItem,
					ItemType = d.TipoItem,

					id_poundType = d.IdTipoCalculoLibras,
					ProductionCostPoundType = d.TipoCalculoLibras,

					libras = d.TotalLibras,
					porcentaje = d.Porcentaje,
					valor = d.Valor,
					coeficiente = d.Coeficiente,

					isActive = true,
				})
				.ToList();
		}

		private void ProcesarCalculoLibrasRemitidas(List<LibrasEjecucionCalculo> totalesLibrasEjecucionCalculo,
			ParametrosEjecucionCalculo parametrosEjecucionCalculo)
		{
			// Recuperamos los estados no permitidos para el proceso
			var idPendienteRecepcionLotDocumentState = db.ProductionLotState
				.FirstOrDefault(e => e.code == m_PendienteRecepcionLotDocumentState
									&& e.id_company == this.ActiveCompanyId
									&& e.isActive)?
				.id;

			if (!idPendienteRecepcionLotDocumentState.HasValue)
			{
				throw new ApplicationException(
					$"No se encuentra registrado el estado PENDIENTE para la recepción de materia prima.");
			}

			var idAnuladoRecepcionLotDocumentState = db.ProductionLotState
				.FirstOrDefault(e => e.code == m_AnuladoRecepcionLotDocumentState
									&& e.id_company == this.ActiveCompanyId
									&& e.isActive)?
				.id;

			if (!idAnuladoRecepcionLotDocumentState.HasValue)
			{
				throw new ApplicationException(
					$"No se encuentra registrado el estado ANULADO para la recepción de materia prima.");
			}


			// Recuperamos los tipos de proceso de producción de Recepción
			var idRecepcionProductionProcess = db.ProductionProcess
				.FirstOrDefault(e => e.code == m_RecepcionProductionProcess
									&& e.id_company == this.ActiveCompanyId
									&& e.isActive)?
				.id;

			if (!idRecepcionProductionProcess.HasValue)
			{
				throw new ApplicationException(
					$"No se encuentra registrado el tipo de proceso de producción RECEPCIÓN DE MATERIA PRIMA.");
			}

			var idRecepcionManualProductionProcess = db.ProductionProcess
				.FirstOrDefault(e => e.code == m_RecepcionManualProductionProcess
									&& e.id_company == this.ActiveCompanyId
									&& e.isActive)?
				.id;

			if (!idRecepcionManualProductionProcess.HasValue)
			{
				throw new ApplicationException(
					$"No se encuentra registrado el tipo de proceso de producción RECEPCIÓN MANUAL DE MATERIA PRIMA.");
			}


			// Recuperamos la línea de inventario: MATERIA PRIMA
			var idMateriaPrimaInventLine = db.InventoryLine
					.FirstOrDefault(l => l.code == m_MateriaPrimaInventoryLine
										&& l.id_company == this.ActiveCompanyId
										&& l.isActive)?
					.id;

			if (!idMateriaPrimaInventLine.HasValue)
			{
				throw new ApplicationException(
					$"No se encuentra registrada la línea de inventario MATERIA PRIMA.");
			}

			// Preparamos las fechas para la consulta
			var fechaInicioConsulta = parametrosEjecucionCalculo.FechaInicio.Date;
			var fechaFinalConsulta = parametrosEjecucionCalculo.FechaFinal.Date.AddDays(1);

			var idPlantaFiltro = parametrosEjecucionCalculo.IdPlantaFiltro;

			var idsBodegas = ((parametrosEjecucionCalculo.IdsBodegas != null) && (parametrosEjecucionCalculo.IdsBodegas.Length > 0))
				? parametrosEjecucionCalculo.IdsBodegas
				: null;
			var idsUbicacionesBodega = ((parametrosEjecucionCalculo.IdsUbicacionesBodega != null) && (parametrosEjecucionCalculo.IdsUbicacionesBodega.Length > 0))
				? parametrosEjecucionCalculo.IdsUbicacionesBodega
				: null;

			var idTipoCalculoLibras = parametrosEjecucionCalculo.IdTipoCalculoLibras;

			// Recuperamos las recepciones de materia prima que cumplen con las condiciones indicadas
			var recepcionesMateriaPrimaQuery = db.ProductionLot
				.Where(p => p.id_ProductionLotState != idPendienteRecepcionLotDocumentState.Value
							&& p.id_ProductionLotState != idAnuladoRecepcionLotDocumentState.Value)
				.Where(p => p.id_productionProcess == idRecepcionProductionProcess.Value
							|| p.id_productionProcess == idRecepcionManualProductionProcess.Value)
				.Where(p => p.receptionDate >= fechaInicioConsulta
							&& p.receptionDate < fechaFinalConsulta);

			if (idPlantaFiltro.HasValue)
			{
				recepcionesMateriaPrimaQuery = recepcionesMateriaPrimaQuery
					.Where(p => p.id_personProcessPlant == idPlantaFiltro);
			}

			var recepcionesMateriaPrima = recepcionesMateriaPrimaQuery
				.Select(p => new
				{
					idProductionLot = p.id,
					idProcessPlant = p.id_personProcessPlant,
					p.number,
					p.internalNumber,
				})
				.ToArray();

			// Verificamos que no existan recepciones sin planta asignada
			var recepcionMateriaPrimaSinPlanta = recepcionesMateriaPrima
				.FirstOrDefault(p => !p.idProcessPlant.HasValue);

			if (recepcionMateriaPrimaSinPlanta != null)
			{
				throw new ApplicationException(
					$"La recepción de materia prima {recepcionMateriaPrimaSinPlanta.number} " +
					$"[{recepcionMateriaPrimaSinPlanta.internalNumber}] no tiene planta de proceso asignada.");
			}

			// Procesamos los detalles
			foreach (var recepcionMateriaPrima in recepcionesMateriaPrima)
			{
				// Nos preparamos para ejecutar la operación
				var idProductionLot = recepcionMateriaPrima.idProductionLot;
				var idProcessPlant = recepcionMateriaPrima.idProcessPlant.Value;


				// Aplicamos los filtros finales de bodegas y ubicaciones
				var recepcionesMateriaPrimaDetallesQuery = db.ProductionLotDetail
					.Where(d => d.id_productionLot == idProductionLot
								&& d.Item.id_inventoryLine == idMateriaPrimaInventLine.Value);

				if (idsBodegas != null)
				{
					recepcionesMateriaPrimaDetallesQuery = recepcionesMateriaPrimaDetallesQuery
						.Where(d => idsBodegas.Contains(d.id_warehouse));
				}

				if (idsUbicacionesBodega != null)
				{
					recepcionesMateriaPrimaDetallesQuery = recepcionesMateriaPrimaDetallesQuery
						.Where(d => idsUbicacionesBodega.Contains(d.id_warehouseLocation));
				}


				// Procesamos las libras agrupadas
				var totalesLibrasRemitidas = recepcionesMateriaPrimaDetallesQuery
					.GroupBy(d => new
					{
						d.id_warehouse,
						d.Item.id_inventoryLine,
						d.Item.id_itemType,
					})
					.Select(g => new
					{
						g.Key.id_warehouse,
						g.Key.id_inventoryLine,
						g.Key.id_itemType,
						totalLibras = g.Sum(d => d.quantityRemitted),
					})
					.Where(t => t.totalLibras > 0m)
					.ToArray();

				// Acumulamos el valor resultante
				foreach (var totalLibrasRemitidas in totalesLibrasRemitidas)
				{
					var idBodega = totalLibrasRemitidas.id_warehouse;
					var idLineaInventario = totalLibrasRemitidas.id_inventoryLine;
					var idTipoItem = totalLibrasRemitidas.id_itemType;
					var totalLibras = totalLibrasRemitidas.totalLibras;

					// Agregamos a los totales
					var totalLibraEjecucionCalculo = totalesLibrasEjecucionCalculo
						.FirstOrDefault(r => r.IdPlantaProceso == idProcessPlant
											&& r.IdBodega == idBodega
											&& r.IdLineaInventario == idLineaInventario
											&& r.IdTipoItem == idTipoItem
											&& r.IdTipoCalculoLibras == idTipoCalculoLibras);

					if (totalLibraEjecucionCalculo == null)
					{
						totalesLibrasEjecucionCalculo.Add(new LibrasEjecucionCalculo()
						{
							IdPlantaProceso = idProcessPlant,
							IdBodega = idBodega,
							IdLineaInventario = idLineaInventario,
							IdTipoItem = idTipoItem,
							IdTipoCalculoLibras = idTipoCalculoLibras,
							TotalLibras = totalLibras,
						});
					}
					else
					{
						totalLibraEjecucionCalculo.TotalLibras += totalLibras;
					}
				}
			}
		}

		private void ProcesarCalculoLibrasInventarios(List<LibrasEjecucionCalculo> totalesLibrasEjecucionCalculo,
			ParametrosEjecucionCalculo parametrosEjecucionCalculo, string formula)
		{
			// Ejecutamos la fórmula
			var resultadosEjecucion = this.ProcesarFormula(formula, parametrosEjecucionCalculo);

			if (resultadosEjecucion.EsEscalar)
			{
				throw new ApplicationException("El resultado de la fórmula debe ser un vector.");
			}

			// Acumulamos el valor resultante
			foreach (var resultadoEjecucion in resultadosEjecucion.Vector)
			{
				// Agregamos a los totales
				var totalLibraEjecucionCalculo = totalesLibrasEjecucionCalculo
					.FirstOrDefault(r => r.IdPlantaProceso == resultadoEjecucion.IdPlantaProceso
										&& r.IdBodega == resultadoEjecucion.IdBodega
										&& r.IdLineaInventario == resultadoEjecucion.IdLineaInventario
										&& r.IdTipoItem == resultadoEjecucion.IdTipoItem
										&& r.IdTipoCalculoLibras == resultadoEjecucion.IdTipoCalculoLibras);

				if (totalLibraEjecucionCalculo == null)
				{
					totalesLibrasEjecucionCalculo.Add(new LibrasEjecucionCalculo()
					{
						IdPlantaProceso = resultadoEjecucion.IdPlantaProceso,
						IdBodega = resultadoEjecucion.IdBodega,
						IdLineaInventario = resultadoEjecucion.IdLineaInventario,
						IdTipoItem = resultadoEjecucion.IdTipoItem,
						IdTipoCalculoLibras = resultadoEjecucion.IdTipoCalculoLibras,
						TotalLibras = resultadoEjecucion.TotalLibras,
					});
				}
				else
				{
					totalLibraEjecucionCalculo.TotalLibras += resultadoEjecucion.TotalLibras;
				}
			}
		}

		private void CompletarEntidadesLibrasEjecucionCalculo(
			List<LibrasPorPlantaLineaTipo> totalesLibrasPorPlantaLineaTipo,
			List<LibrasPorBodegaLineaTipo> totalesLibrasPorBodegaLineaTipo)
		{
			// Recuperamos las entidades adicionales requeridas
			var idsPlantasProceso = totalesLibrasPorPlantaLineaTipo
				.Select(d => d.IdPlantaProceso)
				.Distinct()
				.ToArray();
			var plantasProceso = db.Person
				.Where(d => idsPlantasProceso.Contains(d.id))
				.ToDictionary(d => d.id);

			var idsBodegas = totalesLibrasPorBodegaLineaTipo
				.Select(d => d.IdBodega)
				.Distinct()
				.ToArray();
			var bodegas = db.Warehouse
				.Where(d => idsBodegas.Contains(d.id))
				.ToDictionary(d => d.id);

			var idsLineasInventario = totalesLibrasPorPlantaLineaTipo
				.Select(d => d.IdLineaInventario)
				.Union(totalesLibrasPorBodegaLineaTipo.Select(d => d.IdLineaInventario))
				.Distinct()
				.ToArray();
			var lineasInventario = db.InventoryLine
				.Where(d => idsLineasInventario.Contains(d.id))
				.ToDictionary(d => d.id);

			var idsTiposItem = totalesLibrasPorPlantaLineaTipo
				.Select(d => d.IdTipoItem)
				.Union(totalesLibrasPorBodegaLineaTipo.Select(d => d.IdTipoItem))
				.Distinct()
				.ToArray();
			var tiposItem = db.ItemType
				.Where(d => idsTiposItem.Contains(d.id))
				.ToDictionary(d => d.id);

			var idsTiposCalculoLibras = totalesLibrasPorBodegaLineaTipo
				.Select(d => d.IdTipoCalculoLibras)
				.Distinct()
				.ToArray();
			var tiposCalculoLibras = db.ProductionCostPoundType
				.Where(d => idsTiposCalculoLibras.Contains(d.id))
				.ToDictionary(d => d.id);

			totalesLibrasPorPlantaLineaTipo.ForEach(d =>
			{
				d.PlantaProceso = plantasProceso.ContainsKey(d.IdPlantaProceso)
					? plantasProceso[d.IdPlantaProceso]
					: null;

				d.LineaInventario = lineasInventario.ContainsKey(d.IdLineaInventario)
					? lineasInventario[d.IdLineaInventario]
					: null;

				d.TipoItem = tiposItem.ContainsKey(d.IdTipoItem)
					? tiposItem[d.IdTipoItem]
					: null;
			});

			totalesLibrasPorBodegaLineaTipo.ForEach(d =>
			{
				d.Bodega = bodegas.ContainsKey(d.IdBodega)
					? bodegas[d.IdBodega]
					: null;

				d.LineaInventario = lineasInventario.ContainsKey(d.IdLineaInventario)
					? lineasInventario[d.IdLineaInventario]
					: null;

				d.TipoItem = tiposItem.ContainsKey(d.IdTipoItem)
					? tiposItem[d.IdTipoItem]
					: null;

				d.TipoCalculoLibras = tiposCalculoLibras.ContainsKey(d.IdTipoCalculoLibras)
					? tiposCalculoLibras[d.IdTipoCalculoLibras]
					: null;
			});
		}

		private void CompletarIndicadoresEntidadesLibrasEjecucionCalculo(
			List<LibrasPorPlantaLineaTipo> totalesLibrasPorPlantaLineaTipo, decimal montoTotalDolares)
		{
			// Redondeamos los valores de libras
			totalesLibrasPorPlantaLineaTipo.ForEach(d =>
			{
				d.TotalLibras = Decimal.Round(d.TotalLibras, 2, MidpointRounding.AwayFromZero);
			});

			// Calculamos el total de libras
			var totalLibras = totalesLibrasPorPlantaLineaTipo.Sum(d => d.TotalLibras);

			// Calculamos el porcentaje, el valor y el coeficiente
			if (totalLibras > 0m)
			{
				totalesLibrasPorPlantaLineaTipo.ForEach(d =>
				{
					d.Porcentaje = 100m * d.TotalLibras / totalLibras;
					d.Valor = d.Porcentaje * montoTotalDolares / 100m;
					d.Coeficiente = d.TotalLibras > 0m
						? d.Valor / d.TotalLibras
						: 0m;
				});
			}
		}
		private void CompletarIndicadoresEntidadesLibrasEjecucionCalculo(
			List<LibrasPorBodegaLineaTipo> totalesLibrasPorBodegaLineaTipo, decimal montoTotalDolares)
		{
			// Redondeamos los valores de libras
			totalesLibrasPorBodegaLineaTipo.ForEach(d =>
			{
				d.TotalLibras = Decimal.Round(d.TotalLibras, 2, MidpointRounding.AwayFromZero);
			});

			// Calculamos el total de libras
			var totalLibras = totalesLibrasPorBodegaLineaTipo.Sum(d => d.TotalLibras);

			// Calculamos el porcentaje, el valor y el coeficiente
			if (totalLibras > 0m)
			{
				totalesLibrasPorBodegaLineaTipo.ForEach(d =>
				{
					d.Porcentaje = 100m * d.TotalLibras / totalLibras;
					d.Valor = d.Porcentaje * montoTotalDolares / 100m;
					d.Coeficiente = d.TotalLibras > 0m 
						? d.Valor / d.TotalLibras 
						: 0m;
				});
			}
		}

		#endregion

		#region Procesador de expresiones aritméticas

		private ResultadoEjecucionCalculo ProcesarFormula(string formula, ParametrosEjecucionCalculo parametrosEjecucionCalculo)
		{
			// Procesamos la expresión...
			var indice = 0;
			var valorExpresion = this.ProcesarExpresionFormula(formula, ref indice, parametrosEjecucionCalculo);

			// Verificamos que ya no hay más información
			VerificarNoHayMasInformacionFormula(formula, ref indice);

			return valorExpresion;
		}
		private ResultadoEjecucionCalculo ProcesarExpresionFormula(string formula, ref int indice,
			ParametrosEjecucionCalculo parametrosEjecucionCalculo)
		{
			// Se espera que exista información para la expresión
			VerificarHayMasInformacionFormula(formula, ref indice, "expresión");

			// Verificar si la expresión empieza con un signo o empieza directamente el término
			ResultadoEjecucionCalculo valorExpresion;

			var caracterFormula = formula[indice];
			if (caracterFormula == '+')
			{
				// Si el símbolo es +, lo omitimos y asignamos el valor del término en positivo
				indice++;
				valorExpresion = this.ProcesarTerminoFormula(formula, ref indice, parametrosEjecucionCalculo);
			}
			else if (caracterFormula == '-')
			{
				// Si el símbolo es -, lo omitimos y asignamos el valor del término en negativo
				indice++;
				valorExpresion = this.ProcesarTerminoFormula(formula, ref indice, parametrosEjecucionCalculo);

				if (valorExpresion.EsEscalar)
				{
					valorExpresion.Escalar = -valorExpresion.Escalar;
				}
				else
				{
					valorExpresion.Vector.ForEach(d => d.TotalLibras = -d.TotalLibras);
				}
			}
			else
			{
				// Si el símbolo no es + ni -, asumimos que empieza directamente el término
				valorExpresion = this.ProcesarTerminoFormula(formula, ref indice, parametrosEjecucionCalculo);
			}

			// Procesamos la suma o resta de más términos
			var procesarOtroTermino = true;
			var formulaLength = formula.Length;

			while (procesarOtroTermino)
			{
				// Buscamos si sigue una suma o resta
				OmitirEspaciosEnBlancoFormula(formula, ref indice);

				if (indice < formulaLength)
				{
					caracterFormula = formula[indice];

					if (caracterFormula == '+')
					{
						// Si el símbolo es +, lo omitimos y sumamos el valor del término a la expresión
						indice++;
						var valorTermino = this.ProcesarTerminoFormula(formula, ref indice, parametrosEjecucionCalculo);
						valorExpresion = this.SumarTerminos(valorExpresion, valorTermino);
					}
					else if (caracterFormula == '-')
					{
						// Si el símbolo es -, lo omitimos y restamos el valor del término a la expresión
						indice++;
						var valorTermino = this.ProcesarTerminoFormula(formula, ref indice, parametrosEjecucionCalculo);
						valorExpresion = this.RestarTerminos(valorExpresion, valorTermino);
					}
					else
					{
						// En cualquier otro caso, se entiende que la expresión ha finalizado...
						procesarOtroTermino = false;
					}
				}
				else
				{
					// Si ya no hay más datos, se entiende que la expresión ha finalizado...
					procesarOtroTermino = false;
				}
			}

			return valorExpresion;
		}
		private ResultadoEjecucionCalculo ProcesarTerminoFormula(string formula, ref int indice,
			ParametrosEjecucionCalculo parametrosEjecucionCalculo)
		{
			// Se espera que exista información para el término
			VerificarHayMasInformacionFormula(formula, ref indice, "término");

			// Calcular el valor del primer factor del término
			var valorTermino = this.ProcesarFactorFormula(formula, ref indice, parametrosEjecucionCalculo);

			// Procesamos la multiplicación o división de más factores
			var procesarOtroFactor = true;
			var formulaLength = formula.Length;

			while (procesarOtroFactor)
			{
				// Buscamos si sigue una suma o resta
				OmitirEspaciosEnBlancoFormula(formula, ref indice);

				if (indice < formulaLength)
				{
					var caracterFormula = formula[indice];

					if (caracterFormula == '*')
					{
						// Si el símbolo es *, lo omitimos y multiplicamos el valor del factor al término
						indice++;
						var valorFactor = this.ProcesarFactorFormula(formula, ref indice, parametrosEjecucionCalculo);
						valorTermino = this.MultiplicarFactores(valorTermino, valorFactor);
					}
					else if (caracterFormula == '/')
					{
						// Si el símbolo es /, lo omitimos y dividimos el valor del factor al término
						indice++;
						var valorFactor = this.ProcesarFactorFormula(formula, ref indice, parametrosEjecucionCalculo);
						valorTermino = this.DividirFactores(valorTermino, valorFactor);
					}
					else
					{
						// En cualquier otro caso, se entiende que el factor ha finalizado...
						procesarOtroFactor = false;
					}
				}
				else
				{
					// Si ya no hay más datos, se entiende que el factor ha finalizado...
					procesarOtroFactor = false;
				}
			}

			return valorTermino;
		}
		private ResultadoEjecucionCalculo ProcesarFactorFormula(string formula, ref int indice,
			ParametrosEjecucionCalculo parametrosEjecucionCalculo)
		{
			// Se espera que exista información para el término
			VerificarHayMasInformacionFormula(formula, ref indice, "factor");

			// Debemos determinar si el factor es un identificador, un número entero o una expresión
			ResultadoEjecucionCalculo valorFactor;

			var caracterFormula = formula[indice];
			if (caracterFormula == '(')
			{
				// Si el símbolo es (, procesamos una expresión entre paréntesis
				indice++;
				valorFactor = this.ProcesarExpresionFormula(formula, ref indice, parametrosEjecucionCalculo);

				// Buscamos el paréntesis de cierre de la expresión
				VerificarHayMasInformacionFormula(formula, ref indice, "factor");

				if (formula[indice] == ')')
				{
					indice++;
				}
				else
				{
					throw new ApplicationException(
						$"Error de sintaxis en fórmula: se esperaba el cierre del paréntesis. Posición: {indice}. Fórmula: {formula}.");
				}
			}
			else if (caracterFormula == '{')
			{
				// Si el símbolo es {, procesamos un identificador
				if (formula.IndexOf("{{", indice) != indice)
				{
					// No es un indicador de identificador válido. Error de sintaxis
					throw new ApplicationException(
						$"Error de sintaxis en fórmula: caracter inesperado. Posición: {indice}. Fórmula: {formula}.");
				}

				// Buscamos el final del identificador
				var finIdentificadorIndice = formula.IndexOf("}}", indice);

				if (finIdentificadorIndice < indice)
				{
					// No es un indicador de identificador válido. Error de sintaxis
					throw new ApplicationException(
						$"Error de sintaxis en fórmula: formato de identificador inválido. Posición: {indice}. Fórmula: {formula}.");
				}

				// Extraemos el identificador y lo procesamos
				var identificadorToken = formula.Substring(indice + 2, finIdentificadorIndice - indice - 2);
				var identificadorTokens = identificadorToken.Split(new[] { "||" }, 2, StringSplitOptions.None);

				valorFactor = this.ProcesarIdentificadorFormula(
					identificadorTokens[1], identificadorTokens[0], parametrosEjecucionCalculo);

				indice = finIdentificadorIndice + 2;
			}
			else if (Char.IsDigit(caracterFormula))
			{
				// Si el símbolo es un dígito, procesamos un valor entero constante
				var valorEntero = $"{caracterFormula}";
				indice++;

				var formulaLength = formula.Length;

				while ((indice < formulaLength) && Char.IsDigit(formula[indice]))
				{
					valorEntero = $"{valorEntero}{formula[indice]}";
					indice++;
				}

				valorFactor = new ResultadoEjecucionCalculo()
				{
					EsEscalar = true,
					Vector = null,
					Escalar = Decimal.Parse(valorEntero),
				};
			}
			else
			{
				throw new ApplicationException(
					$"Error de sintaxis en fórmula: caracter inesperado. Posición: {indice}. Fórmula: {formula}.");
			}

			return valorFactor;
		}
		private ResultadoEjecucionCalculo ProcesarIdentificadorFormula(string tipoIdentificador, string idIdentificador,
			ParametrosEjecucionCalculo parametrosEjecucionCalculo)
		{
			if (tipoIdentificador == "InventoryReason")
			{
				if (Int32.TryParse(idIdentificador, out var idMotivoInventario))
				{
					return this.ProcesarIdentificadorInventoryReasonFormula(parametrosEjecucionCalculo, idMotivoInventario);
				}

				throw new ApplicationException(
					$"ID de identificador de motivo de inventario es inválido: {idMotivoInventario}.");
			}
			else
			{
				throw new ApplicationException(
					$"Tipo de identificador de fórmula no es soportado: {tipoIdentificador}.");
			}
		}
		private ResultadoEjecucionCalculo ProcesarIdentificadorInventoryReasonFormula(ParametrosEjecucionCalculo parametrosEjecucionCalculo, int idMotivoInventario)
		{
			var resultadoCalculo = new List<LibrasEjecucionCalculo>();

			// Valores de estados a procesar (no hay codificación única, se tiene que usar ID "quemados")
			var codesDocumentosValidos = new[] {
				"02", // Aprobado parcial
				"03", // Aprobado
				"04", // Cerrado
				"06" // Autorizado
			};

			// Recuperamos las líneas de inventario: PRODUCTRO EN PROCESO y PRODUCTO TERMINADO
			var idProductoEnProcesoInventLine = db.InventoryLine
				.FirstOrDefault(l => l.code == m_ProductoEnProcesoInventoryLine
									&& l.id_company == this.ActiveCompanyId
									&& l.isActive)?
				.id;

			if (!idProductoEnProcesoInventLine.HasValue)
			{
				throw new ApplicationException(
					$"No se encuentra registrada la línea de inventario PRODUCTO EN PROCESO.");
			}

			var idProductoTerminadoInventLine = db.InventoryLine
				.FirstOrDefault(l => l.code == m_ProductoTerminadoInventoryLine
									&& l.id_company == this.ActiveCompanyId
									&& l.isActive)?
				.id;

			if (!idProductoTerminadoInventLine.HasValue)
			{
				throw new ApplicationException(
					$"No se encuentra registrada la línea de inventario PRODUCTO TERMINADO.");
			}

			// Preparamos los filtros para la consulta
			var fechaInicioConsulta = parametrosEjecucionCalculo.FechaInicio.Date;
			var fechaFinalConsulta = parametrosEjecucionCalculo.FechaFinal.Date.AddDays(1);

			var idsBodegas = ((parametrosEjecucionCalculo.IdsBodegas != null) && (parametrosEjecucionCalculo.IdsBodegas.Length > 0))
				? parametrosEjecucionCalculo.IdsBodegas
				: null;

			var idsUbicacionesBodega = ((parametrosEjecucionCalculo.IdsUbicacionesBodega != null) && (parametrosEjecucionCalculo.IdsUbicacionesBodega.Length > 0))
				? parametrosEjecucionCalculo.IdsUbicacionesBodega.Select(i => (int?)i).ToArray()
				: null;

			var idPlantaProcesoFiltro = parametrosEjecucionCalculo.IdPlantaFiltro;
			var idTipoCalculoLibras = parametrosEjecucionCalculo.IdTipoCalculoLibras;


			// Recuperamos los movinmientos de inventarios que cumplen con las condiciones indicadas
			var movimientosInventario = db.InventoryMove
				.Where(p => p.id_inventoryReason == idMotivoInventario)
				.Where(p => p.Document.emissionDate >= fechaInicioConsulta
							&& p.Document.emissionDate < fechaFinalConsulta)
				.Where(p => codesDocumentosValidos.Contains(p.Document.DocumentState.code))
				.ToArray();


			// Procesamos los detalles
			foreach (var movimientoInventario in movimientosInventario)
			{
				var idMovimientoInventario = movimientoInventario.id;

				// Recuperamos el ID de la planta asignado al lote de producción, si tuviera
				int? idPlantaProceso = null;

				if ((movimientoInventario.ProductionLot != null)
					&& movimientoInventario.ProductionLot.id_personProcessPlant.HasValue)
				{
					idPlantaProceso = movimientoInventario.ProductionLot.id_personProcessPlant;
				}


				// Aplicamos los filtros finales de bodegas y ubicaciones
				var movimientoInventarioDetallesQuery = movimientoInventario
					.InventoryMoveDetail
					.Where(d => d.id_inventoryMove == idMovimientoInventario)
					.Where(d => d.Item.id_inventoryLine == idProductoEnProcesoInventLine.Value
								|| d.Item.id_inventoryLine == idProductoTerminadoInventLine.Value)
					.Select(d => new
					{
						d.id_warehouse,
						d.id_warehouseLocation,
						idPlantaProceso = idPlantaProceso ?? d.id_personProcessPlant,
						d.Item.masterCode,
						d.Item.id_inventoryLine,
						d.Item.id_itemType,
						amount = d.entryAmount - d.exitAmount,
						idMetricUnitDetail = d.id_metricUnit,
						presentacion = d.Item.Presentation,
					});

				if ((idsBodegas != null) && (idsBodegas.Length > 0))
				{
					movimientoInventarioDetallesQuery = movimientoInventarioDetallesQuery
						.Where(d => idsBodegas.Contains(d.id_warehouse));
				}

				if (idsUbicacionesBodega != null)
				{
					movimientoInventarioDetallesQuery = movimientoInventarioDetallesQuery
						.Where(d => idsUbicacionesBodega.Contains(d.id_warehouseLocation));
				}

				if (idPlantaProcesoFiltro.HasValue)
				{
					movimientoInventarioDetallesQuery = movimientoInventarioDetallesQuery
						.Where(d => (d.idPlantaProceso == null) || (d.idPlantaProceso == idPlantaProcesoFiltro));
				}

				// Verificamos que existan detalles con los filtros indicados
				var movimientoInventarioDetalles = movimientoInventarioDetallesQuery.ToArray();

				if (!movimientoInventarioDetalles.Any())
				{
					continue;
				}

				// Verificamos que tengamos una planta asignada
				var movimientoDetalleSinPlanta = movimientoInventarioDetalles
					.FirstOrDefault(p => !p.idPlantaProceso.HasValue);

				if (movimientoDetalleSinPlanta != null)
				{
					throw new ApplicationException(
						$"El movimiento de inventario {movimientoInventario.InventoryReason.name} #{movimientoInventario.natureSequential} " +
						$"[{movimientoInventario.id}] no tiene planta de proceso asignada.");
				}

				// Acumulamos el valor resultante
				foreach (var movimientoInventarioDetalle in movimientoInventarioDetalles)
				{
					// Verificamos que tenemos la unidad de medida
					var metricUnitDetail = DataProviderMetricUnit.MetricUnit(movimientoInventarioDetalle.idMetricUnitDetail);
					if(metricUnitDetail == null)
                    {
						throw new ApplicationException("No se ha podido recuperar la unidad de medidad del detalle de movimiento." +
							$"Id Unidad Medida: {movimientoInventarioDetalle.idMetricUnitDetail}.");
                    }

					// Verificamos que tenemos la presentación
					var presentacionProducto = movimientoInventarioDetalle.presentacion;
					if (presentacionProducto == null)
					{
						throw new ApplicationException(
							$"El producto {movimientoInventarioDetalle.masterCode} no tiene una presentación asignada.");
					}
					var factorPresentacion = presentacionProducto.minimum * presentacionProducto.maximum;

					// Convertimos las libras de acuerdo al cálculo
					decimal cantidadLibras;
					var unitMetricDetailCode = metricUnitDetail.code;
					if (unitMetricDetailCode == m_UnidadesMetricUnit)
					{						
						var metricUnitCode = presentacionProducto.MetricUnit.code;
						cantidadLibras = (metricUnitCode == m_KilogramosMetricUnit)
							? Decimal.Round(movimientoInventarioDetalle.amount * factorPresentacion * 2.2046m, 2, MidpointRounding.AwayFromZero)

							: (metricUnitCode == m_LibrasMetricUnit)
							? Decimal.Round(movimientoInventarioDetalle.amount * factorPresentacion, 2, MidpointRounding.AwayFromZero)

							: throw new ApplicationException(
								$"Unidad de medida no soportada para el cálculo actual. Código: {metricUnitCode}.");
                    }
					else if (unitMetricDetailCode == m_LibrasMetricUnit)
                    {
						cantidadLibras = Decimal.Round(movimientoInventarioDetalle.amount * factorPresentacion, 2, MidpointRounding.AwayFromZero);
					}
					else if (unitMetricDetailCode == m_KilogramosMetricUnit)
                    {
						cantidadLibras = Decimal.Round(movimientoInventarioDetalle.amount * factorPresentacion * 2.2046m, 2, MidpointRounding.AwayFromZero);
					}
					else if (unitMetricDetailCode == m_GramosMetricUnit)
                    {
						cantidadLibras = Decimal.Round(movimientoInventarioDetalle.amount * factorPresentacion * 0.00220462m, 2, MidpointRounding.AwayFromZero);
					}
                    else
                    {
						throw new ApplicationException(
							$"Unidad de medida no soportada para el cálculo actual. Código: {unitMetricDetailCode}.");
					}
					

					// Agregamos a los totales al objeto acumulador
					var idPlantaProcesoCalculo = movimientoInventarioDetalle.idPlantaProceso.Value;
					var idBodega = movimientoInventarioDetalle.id_warehouse;
					var idLineaInventario = movimientoInventarioDetalle.id_inventoryLine;
					var idTipoItem = movimientoInventarioDetalle.id_itemType;

					var totalLibraEjecucionCalculo = resultadoCalculo
						.FirstOrDefault(r => r.IdPlantaProceso == idPlantaProcesoCalculo
											&& r.IdBodega == idBodega
											&& r.IdLineaInventario == idLineaInventario
											&& r.IdTipoItem == idTipoItem
											&& r.IdTipoCalculoLibras == idTipoCalculoLibras);

					if (totalLibraEjecucionCalculo == null)
					{
						resultadoCalculo.Add(new LibrasEjecucionCalculo()
						{
							IdPlantaProceso = idPlantaProcesoCalculo,
							IdBodega = idBodega,
							IdLineaInventario = idLineaInventario,
							IdTipoItem = idTipoItem,
							IdTipoCalculoLibras = idTipoCalculoLibras,
							TotalLibras = cantidadLibras,
						});
					}
					else
					{
						totalLibraEjecucionCalculo.TotalLibras += cantidadLibras;
					}
				}
			}


			// Retornamos el resultado
			return new ResultadoEjecucionCalculo()
			{
				EsEscalar = false,
				Escalar = 0m,
				Vector = resultadoCalculo,
			};
		}


		private ResultadoEjecucionCalculo SumarTerminos(ResultadoEjecucionCalculo termino1, ResultadoEjecucionCalculo termino2)
		{
			if (termino1.EsEscalar && termino2.EsEscalar)
			{
				// Suma de dos escalares
				return new ResultadoEjecucionCalculo()
				{
					EsEscalar = true,
					Vector = null,
					Escalar = termino1.Escalar + termino2.Escalar,
				};
			}

			if (!termino1.EsEscalar && !termino2.EsEscalar)
			{
				// Suma de dos vectores
				var vectorSuma = termino1.Vector
					.Select(d => new LibrasEjecucionCalculo()
					{
						IdPlantaProceso = d.IdPlantaProceso,
						IdBodega = d.IdBodega,
						IdLineaInventario = d.IdLineaInventario,
						IdTipoItem = d.IdTipoItem,
						IdTipoCalculoLibras = d.IdTipoCalculoLibras,
						TotalLibras = d.TotalLibras,
					})
					.ToList();

				foreach (var terminoSuma in termino2.Vector)
				{
					var terminoActual = vectorSuma
						.FirstOrDefault(r => r.IdPlantaProceso == terminoSuma.IdPlantaProceso
											&& r.IdBodega == terminoSuma.IdBodega
											&& r.IdLineaInventario == terminoSuma.IdLineaInventario
											&& r.IdTipoItem == terminoSuma.IdTipoItem
											&& r.IdTipoCalculoLibras == terminoSuma.IdTipoCalculoLibras);

					if (terminoActual == null)
					{
						vectorSuma.Add(new LibrasEjecucionCalculo()
						{
							IdPlantaProceso = terminoSuma.IdPlantaProceso,
							IdBodega = terminoSuma.IdBodega,
							IdLineaInventario = terminoSuma.IdLineaInventario,
							IdTipoItem = terminoSuma.IdTipoItem,
							IdTipoCalculoLibras = terminoSuma.IdTipoCalculoLibras,
							TotalLibras = terminoSuma.TotalLibras,
						});
					}
					else
					{
						terminoActual.TotalLibras += terminoSuma.TotalLibras;
					}
				}

				return new ResultadoEjecucionCalculo()
				{
					EsEscalar = false,
					Escalar = 0,
					Vector = vectorSuma,
				};
			}

			throw new ApplicationException("Error en fórmula: no se soporta suma de escalar con vector.");
		}
		private ResultadoEjecucionCalculo RestarTerminos(ResultadoEjecucionCalculo termino1, ResultadoEjecucionCalculo termino2)
		{
			if (termino1.EsEscalar && termino2.EsEscalar)
			{
				// Resta de dos escalares
				return new ResultadoEjecucionCalculo()
				{
					EsEscalar = true,
					Vector = null,
					Escalar = termino1.Escalar - termino2.Escalar,
				};
			}

			if (!termino1.EsEscalar && !termino2.EsEscalar)
			{
				// Resta de dos vectores
				var vectorResta = termino1.Vector
					.Select(d => new LibrasEjecucionCalculo()
					{
						IdPlantaProceso = d.IdPlantaProceso,
						IdBodega = d.IdBodega,
						IdLineaInventario = d.IdLineaInventario,
						IdTipoItem = d.IdTipoItem,
						IdTipoCalculoLibras = d.IdTipoCalculoLibras,
						TotalLibras = d.TotalLibras,
					})
					.ToList();

				foreach (var terminoResta in termino2.Vector)
				{
					var terminoActual = vectorResta
						.FirstOrDefault(r => r.IdPlantaProceso == terminoResta.IdPlantaProceso
											&& r.IdBodega == terminoResta.IdBodega
											&& r.IdLineaInventario == terminoResta.IdLineaInventario
											&& r.IdTipoItem == terminoResta.IdTipoItem
											&& r.IdTipoCalculoLibras == terminoResta.IdTipoCalculoLibras);

					if (terminoActual == null)
					{
						vectorResta.Add(new LibrasEjecucionCalculo()
						{
							IdPlantaProceso = terminoResta.IdPlantaProceso,
							IdBodega = terminoResta.IdBodega,
							IdLineaInventario = terminoResta.IdLineaInventario,
							IdTipoItem = terminoResta.IdTipoItem,
							IdTipoCalculoLibras = terminoResta.IdTipoCalculoLibras,
							TotalLibras = terminoResta.TotalLibras,
						});
					}
					else
					{
						terminoActual.TotalLibras -= terminoResta.TotalLibras;
					}
				}

				return new ResultadoEjecucionCalculo()
				{
					EsEscalar = false,
					Escalar = 0,
					Vector = vectorResta,
				};
			}

			throw new ApplicationException("Error en fórmula: no se soporta resta de escalar con vector.");
		}
		private ResultadoEjecucionCalculo MultiplicarFactores(ResultadoEjecucionCalculo factor1, ResultadoEjecucionCalculo factor2)
		{
			if (factor1.EsEscalar && factor2.EsEscalar)
			{
				// Multiplicación de dos escalares
				return new ResultadoEjecucionCalculo()
				{
					EsEscalar = true,
					Vector = null,
					Escalar = factor1.Escalar * factor2.Escalar,
				};
			}

			if (factor1.EsEscalar || factor2.EsEscalar)
			{
				// Multiplicación de escalar con vector
				var escalar = factor1.EsEscalar ? factor1.Escalar : factor2.Escalar;
				var vector = factor1.EsEscalar ? factor2.Vector : factor1.Vector;

				return new ResultadoEjecucionCalculo()
				{
					EsEscalar = false,
					Escalar = 0,
					Vector = vector.Select(d => new LibrasEjecucionCalculo()
					{
						IdPlantaProceso = d.IdPlantaProceso,
						IdBodega = d.IdBodega,
						IdLineaInventario = d.IdLineaInventario,
						IdTipoItem = d.IdTipoItem,
						IdTipoCalculoLibras = d.IdTipoCalculoLibras,

						TotalLibras = escalar * d.TotalLibras,
					})
					.ToList(),
				};
			}

			throw new ApplicationException("Error en fórmula: no se soporta multiplicación de dos vectores.");
		}
		private ResultadoEjecucionCalculo DividirFactores(ResultadoEjecucionCalculo factor1, ResultadoEjecucionCalculo factor2)
		{
			if (factor1.EsEscalar && factor2.EsEscalar)
			{
				// División de dos escalares
				return new ResultadoEjecucionCalculo()
				{
					EsEscalar = true,
					Vector = null,
					Escalar = factor1.Escalar / factor2.Escalar,
				};
			}

			if (factor1.EsEscalar || factor2.EsEscalar)
			{
				if (factor1.EsEscalar)
				{
					// División de escalar a vector
					return new ResultadoEjecucionCalculo()
					{
						EsEscalar = false,
						Escalar = 0,
						Vector = factor2.Vector.Select(d => new LibrasEjecucionCalculo()
						{
							IdPlantaProceso = d.IdPlantaProceso,
							IdBodega = d.IdBodega,
							IdLineaInventario = d.IdLineaInventario,
							IdTipoItem = d.IdTipoItem,
							IdTipoCalculoLibras = d.IdTipoCalculoLibras,

							TotalLibras = factor1.Escalar / d.TotalLibras,
						})
						.ToList(),
					};
				}
				else
				{
					// División de vector a escalar
					return new ResultadoEjecucionCalculo()
					{
						EsEscalar = false,
						Escalar = 0,
						Vector = factor1.Vector.Select(d => new LibrasEjecucionCalculo()
						{
							IdPlantaProceso = d.IdPlantaProceso,
							IdBodega = d.IdBodega,
							IdLineaInventario = d.IdLineaInventario,
							IdTipoItem = d.IdTipoItem,
							IdTipoCalculoLibras = d.IdTipoCalculoLibras,

							TotalLibras = d.TotalLibras / factor2.Escalar,
						})
						.ToList(),
					};
				}
			}

			throw new ApplicationException("Error en fórmula: no se soporta división de dos vectores.");
		}


		private void VerificarHayMasInformacionFormula(string formula, ref int indice, string elemento)
		{
			// Omitir espacios en blanco
			OmitirEspaciosEnBlancoFormula(formula, ref indice);

			// Verificamos si todavía hay información
			if (indice >= formula.Length)
			{
				throw new ApplicationException(
					$"Error de sintaxis en fórmula: fin inesperado de {elemento}. Posición: {indice}. Fórmula: {formula}.");
			}
		}
		private void VerificarNoHayMasInformacionFormula(string formula, ref int indice)
		{
			// Omitir espacios en blanco
			OmitirEspaciosEnBlancoFormula(formula, ref indice);

			// Verificamos si ya no hay información
			if (indice < formula.Length)
			{
				throw new ApplicationException(
					$"Error de sintaxis en fórmula: se esperaba el fin. Posición: {indice}. Fórmula: {formula}.");
			}
		}
		private void OmitirEspaciosEnBlancoFormula(string formula, ref int indice)
		{
			// Omitir espacios en blanco
			var formulaLength = formula.Length;

			while ((indice < formulaLength) && Char.IsWhiteSpace(formula[indice]))
			{
				indice++;
			}
		}

		#endregion

		#region Manejador de estados de periodos de inventario

		private void CerrarPeriodoInventario(int anio, int mes, int[] idsWarehouses)
		{
			if (idsWarehouses.Length > 0)
			{
				var idCompany = this.ActiveCompanyId;
				var idDivision = this.ActiveDivision.id;
				var idSucursal = this.ActiveSucursal.id;

				// Recuperamos los IDs de estados de períodos de inventario
				RecuperarEstadosPeriodosInventario(out var idEstadoCerrado, out _);

				// Procesamos cada bodega
				foreach (var idWarehouse in idsWarehouses)
				{
					// Recuperamos los periodos de inventario para la bodega indicada
					var periodosBodega = db.InventoryPeriodDetail
						.Where(d => d.InventoryPeriod.id_warehouse == idWarehouse
									&& d.InventoryPeriod.year == anio
									&& d.periodNumber <= mes
									&& d.InventoryPeriod.id_Company == idCompany
									&& d.InventoryPeriod.id_Division == idDivision
									&& d.InventoryPeriod.id_BranchOffice == idSucursal
									&& d.InventoryPeriod.isActive)
						.ToArray();

					// Recuperamos el detalle del periodo actual
					var periodoBodegaActual = periodosBodega
						.FirstOrDefault(p => p.periodNumber == mes);

					if (periodoBodegaActual == null)
					{
						var bodega = db.Warehouse.First(b => b.id == idWarehouse);

						throw new ApplicationException(
							$"No se encuentra el periodo de inventario para la bodega indicada. "
							+ $"Bodega: {bodega.name} [{bodega.code}]. Año: {anio}. Mes: {mes}.");
					}

					// Verificamos el estado del periodo actual
					if (periodoBodegaActual.id_PeriodState == idEstadoCerrado)
					{
						var bodega = db.Warehouse.First(b => b.id == idWarehouse);

						throw new ApplicationException(
							$"El periodo de inventario para la bodega indicada ya se encuentra CERRADO. "
							+ $"Bodega: {bodega.name} [{bodega.code}]. Año: {anio}. Mes: {mes}.");
					}

					// Verificamos el estado de los periodos anteriores
					if (periodosBodega.Any(p => p.id_PeriodState != idEstadoCerrado && p.periodNumber < mes))
					{
						var bodega = db.Warehouse.First(b => b.id == idWarehouse);

						throw new ApplicationException(
							$"Existen periodos de inventario anteriores para la bodega indicada que NO están CERRADOS. "
							+ $"Bodega: {bodega.name} [{bodega.code}]. Año: {anio}. Mes: {mes}.");
					}


					// Todo correcto... actualizamos
					periodoBodegaActual.id_PeriodState = idEstadoCerrado;
					periodoBodegaActual.isClosed = true;
					periodoBodegaActual.dateClose = DateTime.Now;

					db.SaveChanges();
				}
			}
		}

		private void ReversarCierrePeriodoInventario(int anio, int mes, int[] idsWarehouses)
		{
			if (idsWarehouses.Length > 0)
			{
				var idCompany = this.ActiveCompanyId;
				var idDivision = this.ActiveDivision.id;
				var idSucursal = this.ActiveSucursal.id;

				// Recuperamos los IDs de estados de períodos de inventario
				RecuperarEstadosPeriodosInventario(out var idEstadoCerrado, out var idEstadoAbierto);

				// Procesamos cada bodega
				foreach (var idWarehouse in idsWarehouses)
				{
					// Recuperamos los periodos de inventario para la bodega indicada
					var periodosBodega = db.InventoryPeriodDetail
						.Where(d => d.InventoryPeriod.id_warehouse == idWarehouse
									&& d.InventoryPeriod.year == anio
									&& d.periodNumber >= mes
									&& d.InventoryPeriod.id_Company == idCompany
									&& d.InventoryPeriod.id_Division == idDivision
									&& d.InventoryPeriod.id_BranchOffice == idSucursal
									&& d.InventoryPeriod.isActive)
						.ToArray();

					// Recuperamos el detalle del periodo actual
					var periodoBodegaActual = periodosBodega
						.FirstOrDefault(p => p.periodNumber == mes);

					if (periodoBodegaActual == null)
					{
						var bodega = db.Warehouse.First(b => b.id == idWarehouse);

						throw new ApplicationException(
							$"No se encuentra el periodo de inventario para la bodega indicada. "
							+ $"Bodega: {bodega.name} [{bodega.code}]. Año: {anio}. Mes: {mes}.");
					}

					// Verificamos el estado del periodo actual
					if (periodoBodegaActual.id_PeriodState != idEstadoCerrado)
					{
						var bodega = db.Warehouse.First(b => b.id == idWarehouse);

						throw new ApplicationException(
							$"El periodo de inventario para la bodega indicada NO se encuentra CERRADO. "
							+ $"Bodega: {bodega.name} [{bodega.code}]. Año: {anio}. Mes: {mes}.");
					}

					// Verificamos el estado de los periodos posteriores
					if (periodosBodega.Any(p => p.id_InventoryPeriod == idEstadoCerrado && p.periodNumber > mes))
					{
						var bodega = db.Warehouse.First(b => b.id == idWarehouse);

						throw new ApplicationException(
							$"Existen periodos de inventario posteriores para la bodega indicada que están CERRADOS. "
							+ $"Bodega: {bodega.name} [{bodega.code}]. Año: {anio}. Mes: {mes}.");
					}


					// Todo correcto... actualizamos
					periodoBodegaActual.id_PeriodState = idEstadoAbierto;
					periodoBodegaActual.isClosed = false;
					periodoBodegaActual.dateClose = null;

					db.SaveChanges();
				}
			}
		}

		private void RecuperarEstadosPeriodosInventario(out int idEstadoCerrado, out int idEstadoAbierto)
		{
			// Recuperamos los IDs de estados de períodos de inventario
			var estadosPeriodoInventario = db.AdvanceParametersDetail
				.Where(e => e.AdvanceParameters.code == "EPIV1")
				.ToArray();

			// Recuperamos el estado CERRADO
			var idEstadoCerradoValor = estadosPeriodoInventario
				.FirstOrDefault(e => String.Equals(e.valueCode.TrimEnd(), "C", StringComparison.InvariantCultureIgnoreCase))?
				.id;

			if (!idEstadoCerradoValor.HasValue)
			{
				throw new ApplicationException(
					"No existe registrado el estado de período de inventario: CERRADO.");

			}

			// Recuperamos el estado ABIERTO
			var idEstadoAbiertoValor = estadosPeriodoInventario
				.FirstOrDefault(e => String.Equals(e.valueCode.TrimEnd(), "A", StringComparison.InvariantCultureIgnoreCase))?
				.id;

			if (!idEstadoAbiertoValor.HasValue)
			{
				throw new ApplicationException(
					"No existe registrado el estado de período de inventario: ABIERTO.");

			}

			// Asignamos los resultados
			idEstadoCerrado = idEstadoCerradoValor.Value;
			idEstadoAbierto = idEstadoAbiertoValor.Value;
		}

		#endregion
	}
}