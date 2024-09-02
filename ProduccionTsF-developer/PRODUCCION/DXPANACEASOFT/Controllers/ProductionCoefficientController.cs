using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace DXPANACEASOFT.Controllers
{
    [Authorize]
    public class ProductionCoefficientController : DefaultController
    {
        private const string m_ProductionCoefficientModelKey = "productionCoefficient";


        [HttpPost]
        public PartialViewResult Index()
        {
            return this.PartialView();
        }


        #region Vista de consulta principal

        [ValidateInput(false)]
        public PartialViewResult ProductionCoefficientPartial()
        {
            this.TempData.Remove(m_ProductionCoefficientModelKey);

            var model = db.ProductionCostCoefficient
                .OrderByDescending(t => t.sequence)
                .ToList();

            return PartialView("_ProductionCoefficientQueryGrid", model);
        }

        #endregion

        #region Vista de edición de transacción

        [HttpPost]
        public PartialViewResult EditForm(int? id, string successMessage)
        {
            this.TempData.Remove(m_ProductionCoefficientModelKey);

            var productionCoefficient = this.GetEditingProductionCoefficient(id, null);
            this.TempData[m_ProductionCoefficientModelKey] = productionCoefficient;

            this.PrepareEditViewBag(productionCoefficient);

            if (!String.IsNullOrWhiteSpace(successMessage))
            {
                this.ViewBag.EditMessage = this.SuccessMessage(successMessage);
            }

            return PartialView("_EditForm", productionCoefficient);
        }

        [HttpPost]
        public JsonResult Create(
            int idExecutionType, string idPlanDeCuentas, int? idWarehouseType,
            int[] idWarehouses, int[] idWarehouseLocations, int idPoundType,
            int idSimpleFormula, int idProductionCost, int idProductionCostDetail,
            int? idProductionPlant, string description, bool isActive)
        {
            int? idProductionCoefficient;
            string message;
            bool isValid;

            idWarehouses = idWarehouses ?? new int[] { };
            idWarehouseLocations = idWarehouseLocations ?? new int[] { };

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Crear la entidad principal
                    var productionCoefficient = new ProductionCostCoefficient()
                    {
                        sequence = this.CalculateNextSequence(),
                        id_executionType = idExecutionType,
                        id_planDeCuentas = idPlanDeCuentas,
                        id_warehouseType = idWarehouseType,
                        id_poundType = idPoundType,
                        id_simpleFormula = idSimpleFormula,
                        description = description,
                        isActive = isActive,

                        id_productionCost = idProductionCost,
                        id_productionCostDetail = idProductionCostDetail,
                        id_productionPlant = idProductionPlant,

                        id_userCreate = this.ActiveUserId,
                        dateCreate = DateTime.Now,
                        id_userUpdate = this.ActiveUserId,
                        dateUpdate = DateTime.Now,
                    };

                    // Agregar las bodegas relacionadas
                    foreach (var idWarehouse in idWarehouses)
                    {
                        productionCoefficient
                            .ProductionCostCoefficientWarehouses
                            .Add(new ProductionCostCoefficientWarehouse()
                            {
                                id_warehouse = idWarehouse,
                            });
                    }

                    // Agregar las ubicaciones de bodega relacionadas
                    foreach (var idWarehouseLocation in idWarehouseLocations)
                    {
                        productionCoefficient
                            .ProductionCostCoefficientWarehouseLocations
                            .Add(new ProductionCostCoefficientWarehouseLocation()
                            {
                                id_warehouseLocation = idWarehouseLocation,
                            });
                    }

                    // Agregar los detalles del coeficiente
                    var productionCoefficientTemp = (this.TempData[m_ProductionCoefficientModelKey] as ProductionCostCoefficient);

                    if (productionCoefficientTemp?.ProductionCostCoefficientDetails != null)
                    {
                        productionCoefficient.ProductionCostCoefficientDetails = productionCoefficientTemp
                            .ProductionCostCoefficientDetails
                            .Select(d => new ProductionCostCoefficientDetail()
                            {
                                id_planDeCuentas = idPlanDeCuentas,
                                id_cuentaContab = d.id_cuentaContab,
                                id_tipoAuxContab = d.id_tipoAuxContab,
                                id_auxiliarContab = d.id_auxiliarContab,
                                id_tipoPresContab = d.id_tipoPresContab,
                                id_centroCtoContab = d.id_centroCtoContab,
                                id_subcentroCtoContab = d.id_subcentroCtoContab,
                                isActive = true,

                                id_userCreate = this.ActiveUserId,
                                dateCreate = DateTime.Now,
                                id_userUpdate = this.ActiveUserId,
                                dateUpdate = DateTime.Now,
                            })
                            .ToList();
                    }

                    // Guardamos el elemento
                    db.ProductionCostCoefficient.Add(productionCoefficient);
                    db.SaveChanges();
                    transaction.Commit();

                    idProductionCoefficient = productionCoefficient.id;
                    message = "Elemento creado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_ProductionCoefficientModelKey);

                    idProductionCoefficient = null;
                    message = "Error al crear elemento: " + exception.Message;
                    isValid = false;
                }
            }

            var result = new
            {
                idProductionCoefficient,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Update(int idProductionCoefficient,
            int idExecutionType, string idPlanDeCuentas, int? idWarehouseType,
            List<int> idWarehouses, List<int> idWarehouseLocations, int idPoundType,
            int idSimpleFormula, int idProductionCost, int idProductionCostDetail,
            int? idProductionPlant, string description, bool isActive)
        {
            int? idProductionCoefficientResult;
            string message;
            bool isValid;

            idWarehouses = idWarehouses ?? new List<int>();
            idWarehouseLocations = idWarehouseLocations ?? new List<int>();

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Recuperar la entidad actual
                    var productionCoefficient = db.ProductionCostCoefficient
                        .First(c => c.id == idProductionCoefficient);

                    productionCoefficient.id_executionType = idExecutionType;
                    productionCoefficient.id_planDeCuentas = idPlanDeCuentas;
                    productionCoefficient.id_warehouseType = idWarehouseType;
                    productionCoefficient.id_poundType = idPoundType;
                    productionCoefficient.id_simpleFormula = idSimpleFormula;
                    productionCoefficient.description = description;
                    productionCoefficient.isActive = isActive;

                    productionCoefficient.id_productionCost = idProductionCost;
                    productionCoefficient.id_productionCostDetail = idProductionCostDetail;
                    productionCoefficient.id_productionPlant = idProductionPlant;

                    productionCoefficient.id_userUpdate = this.ActiveUserId;
                    productionCoefficient.dateUpdate = DateTime.Now;


                    // Actualizar las bodegas relacionadas
                    foreach (var warehouse in productionCoefficient.ProductionCostCoefficientWarehouses.ToArray())
                    {
                        // Buscamos entre las que ya existen para determinar
                        // si se mantienen o se eliminaron
                        var index = idWarehouses.IndexOf(warehouse.id_warehouse);

                        if (index >= 0)
                        {
                            // Sigue activa, la conservamos y extraemos de la lista de procesamiento...
                            idWarehouses.RemoveAt(index);
                        }
                        else
                        {
                            // Ya no existe, se eliminará...
                            db.Entry(warehouse).State = EntityState.Deleted;
                        }
                    }

                    foreach (var idWarehouse in idWarehouses)
                    {
                        // Agregamos los nuevos elementos
                        productionCoefficient
                            .ProductionCostCoefficientWarehouses
                            .Add(new ProductionCostCoefficientWarehouse()
                            {
                                id_warehouse = idWarehouse,
                            });
                    }


                    // Actualizar las ubicaciones de bodega relacionadas
                    foreach (var warehouseLocation in productionCoefficient.ProductionCostCoefficientWarehouseLocations.ToArray())
                    {
                        // Buscamos entre las que ya existen para determinar
                        // si se mantienen o se eliminaron
                        var index = idWarehouseLocations.IndexOf(warehouseLocation.id_warehouseLocation);

                        if (index >= 0)
                        {
                            // Sigue activa, la conservamos y extraemos de la lista de procesamiento...
                            idWarehouseLocations.RemoveAt(index);
                        }
                        else
                        {
                            // Ya no existe, se eliminará...
                            db.Entry(warehouseLocation).State = EntityState.Deleted;
                        }
                    }

                    foreach (var idWarehouseLocation in idWarehouseLocations)
                    {
                        // Agregamos los nuevos elementos
                        productionCoefficient
                            .ProductionCostCoefficientWarehouseLocations
                            .Add(new ProductionCostCoefficientWarehouseLocation()
                            {
                                id_warehouseLocation = idWarehouseLocation,
                            });
                    }


                    // Actualizar los detalles del coeficiente
                    var productionCoefficientTemp = (this.TempData[m_ProductionCoefficientModelKey] as ProductionCostCoefficient);

                    if (productionCoefficientTemp?.ProductionCostCoefficientDetails != null)
                    {
                        var productionCoefficientDetailsTemp = productionCoefficientTemp
                            .ProductionCostCoefficientDetails
                            .ToList();

                        // Sobreescribimos todos los detalles actuales con los nuevos valores, si hubiera...
                        foreach (var detail in productionCoefficient.ProductionCostCoefficientDetails)
                        {
                            if (productionCoefficientDetailsTemp.Any())
                            {
                                // Actualizamos los detalles
                                var detailTemp = productionCoefficientDetailsTemp[0];
                                productionCoefficientDetailsTemp.RemoveAt(0);

                                detail.id_planDeCuentas = idPlanDeCuentas;
                                detail.id_cuentaContab = detailTemp.id_cuentaContab;
                                detail.id_tipoAuxContab = detailTemp.id_tipoAuxContab;
                                detail.id_auxiliarContab = detailTemp.id_auxiliarContab;
                                detail.id_tipoPresContab = detailTemp.id_tipoPresContab;
                                detail.id_centroCtoContab = detailTemp.id_centroCtoContab;
                                detail.id_subcentroCtoContab = detailTemp.id_subcentroCtoContab;
                                detail.isActive = true;

                                detail.id_userUpdate = this.ActiveUserId;
                                detail.dateUpdate = DateTime.Now;
                            }
                            else
                            {
                                // Ya no hay detalles nuevos, desactivar...
                                detail.id_planDeCuentas = null;
                                detail.id_cuentaContab = null;
                                detail.id_tipoAuxContab = null;
                                detail.id_auxiliarContab = null;
                                detail.id_tipoPresContab = null;
                                detail.id_centroCtoContab = null;
                                detail.id_subcentroCtoContab = null;
                                detail.isActive = false;

                                detail.id_userUpdate = this.ActiveUserId;
                                detail.dateUpdate = DateTime.Now;
                            }
                        }

                        // Agregamos los detalles que faltan de agregar
                        foreach (var detailTemp in productionCoefficientDetailsTemp)
                        {
                            productionCoefficient.ProductionCostCoefficientDetails
                                .Add(new ProductionCostCoefficientDetail()
                                {
                                    id_planDeCuentas = idPlanDeCuentas,
                                    id_cuentaContab = detailTemp.id_cuentaContab,
                                    id_tipoAuxContab = detailTemp.id_tipoAuxContab,
                                    id_auxiliarContab = detailTemp.id_auxiliarContab,
                                    id_tipoPresContab = detailTemp.id_tipoPresContab,
                                    id_centroCtoContab = detailTemp.id_centroCtoContab,
                                    id_subcentroCtoContab = detailTemp.id_subcentroCtoContab,
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
                        foreach (var detail in productionCoefficient.ProductionCostCoefficientDetails)
                        {
                            detail.id_planDeCuentas = null;
                            detail.id_cuentaContab = null;
                            detail.id_tipoAuxContab = null;
                            detail.id_auxiliarContab = null;
                            detail.id_tipoPresContab = null;
                            detail.id_centroCtoContab = null;
                            detail.id_subcentroCtoContab = null;
                            detail.isActive = false;

                            detail.id_userUpdate = this.ActiveUserId;
                            detail.dateUpdate = DateTime.Now;
                        }
                    }

                    // Guardamos el elemento
                    db.SaveChanges();
                    transaction.Commit();

                    idProductionCoefficientResult = productionCoefficient.id;
                    message = "Elemento actualizado exitosamente.";
                    isValid = true;
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    this.TempData.Keep(m_ProductionCoefficientModelKey);

                    idProductionCoefficientResult = null;
                    message = "Error al actualizar elemento: " + exception.Message;
                    isValid = false;
                }
            }

            var result = new
            {
                idProductionCoefficient = idProductionCoefficientResult,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Manejadores del Grid de detalles

        [ValidateInput(false)]
        public PartialViewResult ProductionCoefficientDetail(int idProductionCoefficient,
            string idPlanDeCuentas)
        {
            var productionCoefficient = this.GetEditingProductionCoefficient(
                idProductionCoefficient, idPlanDeCuentas);

            this.TempData[m_ProductionCoefficientModelKey] = productionCoefficient;
            this.TempData.Keep(m_ProductionCoefficientModelKey);
            this.ViewBag.IdPlanDeCuentas = idPlanDeCuentas;

            return this.GetProductionCoefficientDetailsPartialView(productionCoefficient);
        }

        [HttpPost, ValidateInput(false)]
        public PartialViewResult ProductionCoefficientDetailAddNew(int idProductionCoefficient,
            string idPlanDeCuentas, ProductionCostCoefficientDetail productionCoefficientDetail)
        {
            // Recuperamos el modelo actualmente en edición
            var productionCoefficient = this.GetEditingProductionCoefficient(
                idProductionCoefficient, idPlanDeCuentas);

            // Verificamos que los datos sean válidos
            if (this.ModelState.IsValid)
            {
                this.NormalizeProductionCoefficientDetail(productionCoefficientDetail);

                // Recuperamos el detalle con el código indicado (si hubiera)...
                var currentDetail = productionCoefficient.ProductionCostCoefficientDetails?
                    .FirstOrDefault(d => d.id_cuentaContab == productionCoefficientDetail.id_cuentaContab
                                            && d.id_tipoAuxContab == productionCoefficientDetail.id_tipoAuxContab
                                            && d.id_auxiliarContab == productionCoefficientDetail.id_auxiliarContab
                                            && d.id_tipoPresContab == productionCoefficientDetail.id_tipoPresContab
                                            && d.id_centroCtoContab == productionCoefficientDetail.id_centroCtoContab
                                            && d.id_subcentroCtoContab == productionCoefficientDetail.id_subcentroCtoContab);

                if (currentDetail != null)
                {
                    // Si ya existe y está activo, no permitimos duplicados...
                    this.ViewBag.EditError = $"Ya existe un elemento con el código indicado. Código: {productionCoefficientDetail.id_cuentaContab}.";
                }
                else
                {
                    // Si no existe, lo agregamos...
                    var idNew = productionCoefficient.ProductionCostCoefficientDetails.Any()
                        ? productionCoefficient.ProductionCostCoefficientDetails.Max(d => d.id) + 1
                        : 1;

                    productionCoefficient.ProductionCostCoefficientDetails
                        .Add(new ProductionCostCoefficientDetail()
                        {
                            id = idNew,
                            id_planDeCuentas = idPlanDeCuentas,
                            id_cuentaContab = productionCoefficientDetail.id_cuentaContab,
                            id_tipoAuxContab = productionCoefficientDetail.id_tipoAuxContab,
                            id_auxiliarContab = productionCoefficientDetail.id_auxiliarContab,
                            id_tipoPresContab = productionCoefficientDetail.id_tipoPresContab,
                            id_centroCtoContab = productionCoefficientDetail.id_centroCtoContab,
                            id_subcentroCtoContab = productionCoefficientDetail.id_subcentroCtoContab,
                            isActive = true,
                        });
                }
            }
            else
            {
                this.ViewBag.EditError = "Hay errores de validación en los datos recibidos.";
            }

            this.TempData[m_ProductionCoefficientModelKey] = productionCoefficient;
            this.TempData.Keep(m_ProductionCoefficientModelKey);
            this.ViewBag.IdPlanDeCuentas = idPlanDeCuentas;

            return this.GetProductionCoefficientDetailsPartialView(productionCoefficient);
        }

        [HttpPost, ValidateInput(false)]
        public PartialViewResult ProductionCoefficientDetailUpdate(int idProductionCoefficient,
            string idPlanDeCuentas, ProductionCostCoefficientDetail productionCoefficientDetail)
        {
            // Recuperamos el modelo actualmente en edición
            var productionCoefficient = this.GetEditingProductionCoefficient(
                idProductionCoefficient, idPlanDeCuentas);

            // Verificamos que los datos sean válidos
            if (this.ModelState.IsValid)
            {
                // Recuperamos el detalle con el código indicado (si hubiera)...
                var currentDetail = productionCoefficient.ProductionCostCoefficientDetails?
                    .FirstOrDefault(d => d.id == productionCoefficientDetail.id);

                if (currentDetail != null)
                {
                    // Si existe, lo actualizamos...
                    currentDetail.id_planDeCuentas = idPlanDeCuentas;
                    currentDetail.id_cuentaContab = productionCoefficientDetail.id_cuentaContab;
                    currentDetail.id_tipoAuxContab = productionCoefficientDetail.id_tipoAuxContab;
                    currentDetail.id_auxiliarContab = productionCoefficientDetail.id_auxiliarContab;
                    currentDetail.id_tipoPresContab = productionCoefficientDetail.id_tipoPresContab;
                    currentDetail.id_centroCtoContab = productionCoefficientDetail.id_centroCtoContab;
                    currentDetail.id_subcentroCtoContab = productionCoefficientDetail.id_subcentroCtoContab;
                    currentDetail.isActive = true;
                }
                else
                {
                    this.ViewBag.EditError = $"No existe el elemento a actualizar con ID: {productionCoefficientDetail.id}.";
                }
            }
            else
            {
                this.ViewBag.EditError = "Hay errores de validación en los datos recibidos.";
            }

            this.TempData[m_ProductionCoefficientModelKey] = productionCoefficient;
            this.TempData.Keep(m_ProductionCoefficientModelKey);
            this.ViewBag.IdPlanDeCuentas = idPlanDeCuentas;

            return this.GetProductionCoefficientDetailsPartialView(productionCoefficient);
        }

        [HttpPost, ValidateInput(false)]
        public PartialViewResult ProductionCoefficientDetailDelete(int idProductionCoefficient,
            string idPlanDeCuentas, int id)
        {
            // Recuperamos el modelo actualmente en edición
            var productionCoefficient = this.GetEditingProductionCoefficient(
                idProductionCoefficient, idPlanDeCuentas);

            // Recuperamos el detalle con el código indicado (si hubiera)...
            var currentDetail = productionCoefficient.ProductionCostCoefficientDetails?
                .FirstOrDefault(d => d.id == id);

            if (currentDetail != null)
            {
                // Si existe, lo inactivamos...
                productionCoefficient.ProductionCostCoefficientDetails.Remove(currentDetail);
            }
            else
            {
                this.ViewBag.EditError = $"No existe el elemento con ID: {id}.";
            }

            this.TempData[m_ProductionCoefficientModelKey] = productionCoefficient;
            this.TempData.Keep(m_ProductionCoefficientModelKey);
            this.ViewBag.IdPlanDeCuentas = idPlanDeCuentas;

            return this.GetProductionCoefficientDetailsPartialView(productionCoefficient);
        }

        #endregion

        #region Manejadores de consultas auxiliares

        [HttpPost]
        public JsonResult QueryTiposAuxiliarContables(
            string idPlanDeCuentas, string idCuentaContable)
        {
            var items = DataProviderPlanDeCuentas
                .GetTiposAuxiliaresContablesByCurrent(idPlanDeCuentas, idCuentaContable, null)
                .Select(ta => new
                {
                    idTipoAuxContable = ta.CCiTipoAuxiliar,
                    tipoAuxContable = ta.CDsTipoAuxiliar,
                })
                .OrderBy(ta => ta.idTipoAuxContable)
                .ToArray();

            var result = new
            {
                isValid = true,
                items,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult QueryAuxiliaresContables(string idTipoAuxContable)
        {
            var items = DataProviderPlanDeCuentas
                .GetAuxiliaresContablesByCurrent(idTipoAuxContable, null)
                .Select(ax => new
                {
                    idAuxContable = ax.CCiAuxiliar,
                    auxContable = ax.CDsAuxiliar,
                })
                .OrderBy(ta => ta.idAuxContable)
                .ToArray();

            var result = new
            {
                isValid = true,
                items,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult QueryCentrosCosto(string idTipoPresupuesto)
        {
            var items = DataProviderPlanDeCuentas
                .GetCentrosCostoContablesByCurrent(idTipoPresupuesto, null)
                .Select(c => new
                {
                    idCentroCosto = c.CCiProyecto,
                    centroCosto = c.CDsProyecto,
                })
                .OrderBy(c => c.idCentroCosto)
                .ToArray();

            var result = new
            {
                isValid = true,
                items,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult QuerySubcentrosCosto(
            string idTipoPresupuesto, string idCentroCosto)
        {
            var items = DataProviderPlanDeCuentas
                .GetSubcentrosCostoContablesByCurrent(idTipoPresupuesto, idCentroCosto, null)
                .Select(c => new
                {
                    idSubcentroCosto = c.CCiSubProyecto,
                    subcentroCosto = c.CDsSubProyecto,
                })
                .OrderBy(c => c.idSubcentroCosto)
                .ToArray();

            var result = new
            {
                isValid = true,
                items,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Métodos adicionales

        private ProductionCostCoefficient GetEditingProductionCoefficient(int? id_productionCoefficient,
            string idPlanDeCuentas)
        {
            // Recuperamos el elemento del caché local
            var productionCoefficient = (this.TempData[m_ProductionCoefficientModelKey] as ProductionCostCoefficient);

            // Si no hay elemento en el caché, consultamos desde la base
            if ((productionCoefficient == null) && id_productionCoefficient.HasValue)
            {
                productionCoefficient = db.ProductionCostCoefficient
                    .FirstOrDefault(i => i.id == id_productionCoefficient);

                if (productionCoefficient != null)
                {
                    idPlanDeCuentas = idPlanDeCuentas ?? productionCoefficient.id_planDeCuentas;
                }
            }

            // Si no existe, creamos un nuevo elemento
            if (productionCoefficient == null)
            {
                productionCoefficient = this.CreateNewProductionCostCoefficient();
            }

            // Eliminamos los detalles inactivos o que NO  corresponden al plan de cuentas indicado
            productionCoefficient.ProductionCostCoefficientDetails = productionCoefficient
                .ProductionCostCoefficientDetails
                .Where(d => d.id_planDeCuentas == idPlanDeCuentas && d.isActive)
                .OrderBy(d => d.id_cuentaContab)
                    .ThenBy(d => d.id_tipoAuxContab).ThenBy(d => d.id_auxiliarContab)
                    .ThenBy(d => d.id_tipoPresContab).ThenBy(d => d.id_centroCtoContab).ThenBy(d => d.id_subcentroCtoContab)
                .ToList();

            return productionCoefficient;
        }

        private ProductionCostCoefficient CreateNewProductionCostCoefficient()
        {
            var usarIntegracionContable = DataProviderPlanDeCuentas.IsEnabledIntegracionContable();
            var idPlanDeCuentas = usarIntegracionContable
                ? DataProviderPlanDeCuentas
                    .GetPlanDeCuentaByCompany(this.ActiveCompanyId)?
                    .CCiPlanCta
                : null;

            return new ProductionCostCoefficient()
            {
                sequence = this.CalculateNextSequence(),
                id_planDeCuentas = idPlanDeCuentas,
                isActive = true,
            };
        }

        private int CalculateNextSequence()
        {
            var currentSequence = db.ProductionCostCoefficient
                .OrderByDescending(c => c.sequence)
                .FirstOrDefault()?
                .sequence ?? 0;

            return currentSequence + 1;
        }

        private void PrepareEditViewBag(ProductionCostCoefficient currentCostCoefficient)
        {
            // Recuperamos el indicador de integración contable
            this.ViewBag.UsarIntegracionContable = DataProviderPlanDeCuentas.IsEnabledIntegracionContable();

            // Recuperamos las bodegas de distribución
            var bodegasDistribucionCosto = db.Warehouse
                .Where(w => w.enableProductionCost && w.isActive)
                .ToList();

            // Recuperamos los tipos de bodegas de distribución
            var idsTiposBodegasDistribucionCosto = bodegasDistribucionCosto
                .Select(w => w.id_warehouseType)
                .Distinct()
                .ToArray();
            var tiposBodegasDistribucion = db.WarehouseType
                .Where(t => idsTiposBodegasDistribucionCosto.Contains(t.id) && t.isActive)
                .ToList();

            // Recuperamos las ubicaciones de las bodegas de distribución
            var idsBodegasDistribucionCosto = bodegasDistribucionCosto
                .Select(w => w.id)
                .ToArray();
            var ubicacionesBodegasDistribucion = db.WarehouseLocation
                .Where(l => idsBodegasDistribucionCosto.Contains(l.id_warehouse) && l.isActive)
                .ToList();

            // Agregamos a las listas los códigos actuales, en caso que no sean válidos
            if ((currentCostCoefficient.WarehouseType != null)
                && (!tiposBodegasDistribucion.Any(t => t.id == currentCostCoefficient.WarehouseType.id)))
            {
                tiposBodegasDistribucion.Add(currentCostCoefficient.WarehouseType);
            }

            foreach (var bodegaActual in currentCostCoefficient.ProductionCostCoefficientWarehouses)
            {
                if (!bodegasDistribucionCosto.Any(b => b.id == bodegaActual.id_warehouse))
                {
                    bodegasDistribucionCosto.Add(bodegaActual.Warehouse);
                }
            }

            foreach (var ubicacionBodegaActual in currentCostCoefficient.ProductionCostCoefficientWarehouseLocations)
            {
                if (!ubicacionesBodegasDistribucion.Any(b => b.id == ubicacionBodegaActual.id_warehouseLocation))
                {
                    ubicacionesBodegasDistribucion.Add(ubicacionBodegaActual.WarehouseLocation);
                }
            }

            // Recuperamos la lista de plantas
            var plantasProceso = db.Person
                .Where(p => p.isActive && p.Rol.FirstOrDefault(r => r.name.Equals("Planta Proceso")) != null)
                .Select(p => new
                {
                    p.id,
                    planta = p.identification_number,
                    name = p.fullname_businessName,
                    processPlant = p.processPlant ?? p.fullname_businessName,
                })
                .ToList();

            // Agregamos los elementos al ViewBag
            this.ViewBag.TiposBodegasDistribucion = tiposBodegasDistribucion;
            this.ViewBag.BodegasDistribucionCosto = bodegasDistribucionCosto;
            this.ViewBag.UbicacionesBodegasDistribucion = ubicacionesBodegasDistribucion;
            this.ViewBag.PlantasProceso = plantasProceso;
            this.ViewBag.IdPlanDeCuentas = currentCostCoefficient.id_planDeCuentas;
        }

        private PartialViewResult GetProductionCoefficientDetailsPartialView(ProductionCostCoefficient productionCoefficient)
        {
            var model = productionCoefficient.ProductionCostCoefficientDetails?
                .OrderBy(t => t.id_cuentaContab)
                    .ThenBy(t => t.id_tipoAuxContab).ThenBy(t => t.id_auxiliarContab)
                    .ThenBy(t => t.id_tipoPresContab).ThenBy(t => t.id_centroCtoContab).ThenBy(t => t.id_subcentroCtoContab)
                .ToList() ?? new List<ProductionCostCoefficientDetail>();

            this.NormalizeProductionCoefficientDetails(model);

            return PartialView("_ProductionCoefficientDetailsPartial", model);
        }

        private void NormalizeProductionCoefficientDetails(List<ProductionCostCoefficientDetail> productionCoefficientDetails)
        {
            productionCoefficientDetails.ForEach(d => this.NormalizeProductionCoefficientDetail(d));
        }
        private void NormalizeProductionCoefficientDetail(ProductionCostCoefficientDetail productionCoefficientDetail)
        {
            if (String.IsNullOrEmpty(productionCoefficientDetail.id_tipoAuxContab)
                || String.IsNullOrEmpty(productionCoefficientDetail.id_auxiliarContab))
            {
                productionCoefficientDetail.id_tipoAuxContab = null;
                productionCoefficientDetail.id_auxiliarContab = null;
            }

            if (String.IsNullOrEmpty(productionCoefficientDetail.id_tipoPresContab)
                || String.IsNullOrEmpty(productionCoefficientDetail.id_centroCtoContab)
                || String.IsNullOrEmpty(productionCoefficientDetail.id_subcentroCtoContab))
            {
                productionCoefficientDetail.id_tipoPresContab = null;
                productionCoefficientDetail.id_centroCtoContab = null;
                productionCoefficientDetail.id_subcentroCtoContab = null;
            }
        }

        #endregion
    }
}
