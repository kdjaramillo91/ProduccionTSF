using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
	[Authorize]
	public class ProductionCostController : DefaultController
	{
		private const string m_ProductionCostModelKey = "productionCost";


		[HttpPost]
		public ActionResult Index()
		{
			return this.PartialView();
		}

		[ValidateInput(false)]
		public ActionResult ProductionCostPartial()
		{
			this.TempData.Remove(m_ProductionCostModelKey);

			return this.GetMainProductionCostPartialView();
		}

		[HttpPost]
		public ActionResult DeleteSelectedProductionCost(int[] ids)
		{
			if ((ids != null) && (ids.Length > 0))
			{
				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						var productionCostsDelete = db.ProductionCost
							.Where(i => ids.Contains(i.id));

						foreach (var productionCostDelete in productionCostsDelete)
						{
							productionCostDelete.isActive = false;

							productionCostDelete.id_userUpdate = this.ActiveUserId;
							productionCostDelete.dateUpdate = DateTime.Now;
						}

						db.SaveChanges();
						transaction.Commit();

						this.ViewBag.EditMessage = this.SuccessMessage("Costos de Producción desactivados exitosamente.");
					}
					catch (Exception exception)
					{
						transaction.Rollback();

						this.TempData.Keep(m_ProductionCostModelKey);

						this.ViewBag.EditMessage = this.ErrorMessage($"Error: {exception.Message}");
					}
				}
			}
			else
			{
				this.TempData.Keep(m_ProductionCostModelKey);

				this.ViewBag.EditMessage = this.ErrorMessage("No se recibió nada para desactivar.");
			}

			return this.GetMainProductionCostPartialView();
		}




		[HttpPost, ValidateInput(false)]
		public ActionResult ProductionCostPartialAddNew(ProductionCost item)
		{
			// Ejecutamos la operación si el modelo es válido...
			if (this.ModelState.IsValid)
			{
				var productionCostTemp = (this.TempData[m_ProductionCostModelKey] as ProductionCost);

				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						if (productionCostTemp?.ProductionCostDetails != null)
						{
							item.ProductionCostDetails = productionCostTemp
								.ProductionCostDetails
								.Select(d => new ProductionCostDetail()
								{
									code = d.code,
									name = d.name,
									order = d.order,
									description = d.description,
									isActive = d.isActive,
									id_userCreate = this.ActiveUserId,
									dateCreate = DateTime.Now,
									id_userUpdate = this.ActiveUserId,
									dateUpdate = DateTime.Now,
								})
								.ToList();
						}

						// Campos de auditoría
						item.id_userCreate = this.ActiveUserId;
						item.dateCreate = DateTime.Now;
						item.id_userUpdate = this.ActiveUserId;
						item.dateUpdate = DateTime.Now;

						// Guardamos el elemento
						db.ProductionCost.Add(item);
						db.SaveChanges();
						transaction.Commit();

						this.ViewBag.EditMessage = this.SuccessMessage("Costo de Producción agregado exitosamente.");
					}
					catch (Exception exception)
					{
						transaction.Rollback();

						this.TempData.Keep(m_ProductionCostModelKey);

						this.ViewBag.EditError = $"Error: {exception.Message}";
					}
				}
			}
			else
			{
				this.ViewBag.EditMessage = this.ErrorMessage("Hay errores de validación en los datos recibidos.");
			}

			return this.GetMainProductionCostPartialView();
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult ProductionCostPartialUpdate(ProductionCost item)
		{
			// Verificar si agregamos o actualizamos...
			var productionCostModel = db.ProductionCost
				.FirstOrDefault(t => t.id == item.id);

			if (productionCostModel == null)
			{
				return ProductionCostPartialAddNew(item);
			}

			// Ejecutamos la operación si el modelo es válido...
			if (this.ModelState.IsValid)
			{
				var productionCostTemp = (this.TempData[m_ProductionCostModelKey] as ProductionCost);

				using (var transaction = db.Database.BeginTransaction())
				{
					try
					{
						// Recuperamos los detalles actuales para controlar aquellos que fueron eliminados
						var idsProductionCostDetailsToDisable = productionCostModel.ProductionCostDetails
							.Select(d => d.id)
							.ToList();

						// Actualizamos los detalles
						if (productionCostTemp?.ProductionCostDetails != null)
						{
							foreach (var productionCostDetailUpdate in productionCostTemp.ProductionCostDetails)
							{
								var productionCostDetailModel = productionCostModel.ProductionCostDetails
									.FirstOrDefault(d => d.code == productionCostDetailUpdate.code);

								if (productionCostDetailModel != null)
								{
									productionCostDetailModel.name = productionCostDetailUpdate.name;
									productionCostDetailModel.order = productionCostDetailUpdate.order;
									productionCostDetailModel.description = productionCostDetailUpdate.description;
									productionCostDetailModel.isActive = productionCostDetailUpdate.isActive;
									productionCostDetailModel.id_userUpdate = this.ActiveUserId;
									productionCostDetailModel.dateUpdate = DateTime.Now;

									idsProductionCostDetailsToDisable.Remove(productionCostDetailModel.id);
								}
								else
								{
									productionCostModel
										.ProductionCostDetails
										.Add(new ProductionCostDetail()
										{
											code = productionCostDetailUpdate.code,
											name = productionCostDetailUpdate.name,
											order = productionCostDetailUpdate.order,
											description = productionCostDetailUpdate.description,
											isActive = productionCostDetailUpdate.isActive,
											id_userCreate = this.ActiveUserId,
											dateCreate = DateTime.Now,
											id_userUpdate = this.ActiveUserId,
											dateUpdate = DateTime.Now,
										});
								}
							}
						}

						// Inactivamos cualquier detalle que ya no aparezca en la lista
						foreach (var idProductionCostDetailToDisable in idsProductionCostDetailsToDisable)
						{
							var productionCostDetailModel = productionCostModel.ProductionCostDetails
								.First(d => d.id == idProductionCostDetailToDisable);

							if (productionCostDetailModel.isActive)
							{
								// Si está activo, se inactiva...
								productionCostDetailModel.isActive = false;
								productionCostDetailModel.id_userUpdate = this.ActiveUserId;
								productionCostDetailModel.dateUpdate = DateTime.Now;
							}
						}

						// Campos de cabecera
						productionCostModel.name = item.name;
						productionCostModel.order = item.order;
						productionCostModel.id_executionType = item.id_executionType;
						productionCostModel.description = item.description;
						productionCostModel.isActive = item.isActive;

						// Campos de auditoría
						item.id_userUpdate = this.ActiveUserId;
						item.dateUpdate = DateTime.Now;

						// Guardamos el elemento
						db.SaveChanges();
						transaction.Commit();

						this.ViewBag.EditMessage = this.SuccessMessage("Costo de Producción actualizado exitosamente.");
					}
					catch (Exception exception)
					{
						transaction.Rollback();

						this.TempData.Keep(m_ProductionCostModelKey);

						this.ViewBag.EditError = $"Error: {exception.Message}";
					}
				}
			}
			else
			{
				this.ViewBag.EditMessage = this.ErrorMessage("Hay errores de validación en los datos recibidos.");
			}

			return this.GetMainProductionCostPartialView();
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult ProductionCostPartialDelete(int id)
		{
			try
			{
				var productionCostDelete = db.ProductionCost
					.FirstOrDefault(t => t.id == id);

				if (productionCostDelete != null)
				{
					productionCostDelete.isActive = false;

					productionCostDelete.id_userUpdate = this.ActiveUserId;
					productionCostDelete.dateUpdate = DateTime.Now;

					db.SaveChanges();
				}
			}
			catch (Exception exception)
			{
				this.TempData.Keep(m_ProductionCostModelKey);

				this.ViewBag.EditError = $"Error: {exception.Message}";
			}

			return this.GetMainProductionCostPartialView();
		}


		[HttpPost]
		public JsonResult ValidateCodeProductionCost(int id_productionCost, string code)
		{
			bool isValid;
			string errorText;

			var productionCostModel = db.ProductionCost
				.FirstOrDefault(t => t.code == code && t.id != id_productionCost);

			if (productionCostModel != null)
			{
				isValid = false;
				errorText = "Código en uso por otro costo de producción";
			}
			else
			{
				isValid = true;
				errorText = "";
			}

			return this.Json(new
			{
				isValid,
				errorText,
			}, JsonRequestBehavior.AllowGet);
		}




		[ValidateInput(false)]
		public ActionResult ProductionCostDetailPartial(int id_productionCost)
		{
			var productionCost = this.GetEditingProductionCost(id_productionCost);

			this.TempData[m_ProductionCostModelKey] = productionCost;
			this.TempData.Keep(m_ProductionCostModelKey);

			return this.GetDetailsProductionCostPartialView(productionCost);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult ProductionCostDetailAddNew(int id_productionCost, ProductionCostDetail productionCostDetail)
		{
			// Recuperamos el modelo actualmente en edición
			var productionCost = this.GetEditingProductionCost(id_productionCost);

			// Verificamos que los datos sean válidos
			if (this.ModelState.IsValid)
			{
				// Recuperamos el detalle con el código indicado (si hubiera)...
				var currentDetail = productionCost.ProductionCostDetails?
					.FirstOrDefault(d => d.code == productionCostDetail.code);

				if (currentDetail != null)
				{
					if (currentDetail.isActive)
					{
						// Si ya existe y está activo, no permitimos duplicados...
						this.ViewBag.EditError = $"Ya existe un elemento con el código indicado. Código: {productionCostDetail.code}.";
					}
					else
					{
						// Si ya existe y está inactivo, lo reusamos...
						currentDetail.name = productionCostDetail.name;
						currentDetail.order = productionCostDetail.order;
						currentDetail.description = productionCostDetail.description;
						currentDetail.isActive = productionCostDetail.isActive;
					}
				}
				else
				{
					// Si no existe, lo agregamos...
					productionCost.ProductionCostDetails
						.Add(new ProductionCostDetail()
						{
							code = productionCostDetail.code,
							name = productionCostDetail.name,
							order = productionCostDetail.order,
							description = productionCostDetail.description,
							isActive = productionCostDetail.isActive,
						});
				}
			}
			else
			{
				this.ViewBag.EditError = "Hay errores de validación en los datos recibidos.";
			}

			this.TempData[m_ProductionCostModelKey] = productionCost;
			this.TempData.Keep(m_ProductionCostModelKey);

			return this.GetDetailsProductionCostPartialView(productionCost);
		}


		[HttpPost, ValidateInput(false)]
		public ActionResult ProductionCostDetailUpdate(int id_productionCost, ProductionCostDetail productionCostDetail)
		{
			// Recuperamos el modelo actualmente en edición
			var productionCost = this.GetEditingProductionCost(id_productionCost);

			// Verificamos que los datos sean válidos
			if (this.ModelState.IsValid)
			{
				// Recuperamos el detalle con el código indicado (si hubiera)...
				var currentDetail = productionCost.ProductionCostDetails?
					.FirstOrDefault(d => d.code == productionCostDetail.code);

				if (currentDetail != null)
				{
					// Si existe, lo actualizamos...
					currentDetail.name = productionCostDetail.name;
					currentDetail.order = productionCostDetail.order;
					currentDetail.description = productionCostDetail.description;
					currentDetail.isActive = productionCostDetail.isActive;
				}
				else
				{
					this.ViewBag.EditError = $"No existe el elemento con código: {productionCostDetail.code}.";
				}
			}
			else
			{
				this.ViewBag.EditError = "Hay errores de validación en los datos recibidos.";
			}

			this.TempData[m_ProductionCostModelKey] = productionCost;
			this.TempData.Keep(m_ProductionCostModelKey);

			return this.GetDetailsProductionCostPartialView(productionCost);
		}


		[HttpPost, ValidateInput(false)]
		public ActionResult ProductionCostDetailDelete(int id_productionCost, string code)
		{
			// Recuperamos el modelo actualmente en edición
			var productionCost = this.GetEditingProductionCost(id_productionCost);

			// Recuperamos el detalle con el código indicado (si hubiera)...
			var currentDetail = productionCost.ProductionCostDetails?
				.FirstOrDefault(d => d.code == code);

			if (currentDetail != null)
			{
				// Si existe, lo inactivamos...
				currentDetail.isActive = false;
			}
			else
			{
				this.ViewBag.EditError = $"No existe el elemento con código: {code}.";
			}

			this.TempData[m_ProductionCostModelKey] = productionCost;
			this.TempData.Keep(m_ProductionCostModelKey);

			return this.GetDetailsProductionCostPartialView(productionCost);
		}




		private PartialViewResult GetMainProductionCostPartialView()
		{
			var model = db.ProductionCost
				.OrderBy(t => t.order).ThenBy(t => t.code)
				.ToList();

			return PartialView("_ProductionCostsPartial", model);
		}

		private PartialViewResult GetDetailsProductionCostPartialView(ProductionCost productionCost)
		{
			var model = productionCost.ProductionCostDetails?
				.OrderBy(t => t.order).ThenBy(t => t.code)
				.ToList() ?? new List<ProductionCostDetail>();

			return PartialView("_ProductionCostDetailsPartial", model);
		}

		private ProductionCost GetEditingProductionCost(int? id_productionCost)
		{
			var productionCost = (this.TempData[m_ProductionCostModelKey] as ProductionCost);

			if (productionCost == null)
			{
				productionCost = db.ProductionCost
					.FirstOrDefault(i => i.id == id_productionCost);
			}

			return productionCost ?? new ProductionCost();
		}
	}
}