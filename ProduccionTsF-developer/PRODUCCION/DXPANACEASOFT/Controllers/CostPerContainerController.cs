using DXPANACEASOFT.DataProviders;
using DXPANACEASOFT.Models;
using DXPANACEASOFT.Operations;
using EntidadesAuxiliares.SQL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace DXPANACEASOFT.Controllers
{
    public class CostPerContainerController : DefaultController
    {
        #region Constantes
        private const string m_TipoDocumentoCostPerContainer = "165";
        private const string m_PendienteDocumentState = "01";
        private const string m_AprobadoDocumentState = "03";
        private const string m_AnuladoDocumentState = "05";

        private const string m_CostPerContainerModelKey = "costPerContainer";
        private const string m_DetallesCustomModelKey = "DetallesCostos_Custom";
        private const string m_CodigosCostoAdicionlesModelKey = "CodigosCostosAdicionales";
        private const char m_splitCharacter = '|';
        #endregion

        [HttpPost]
        public ActionResult Index()
        {
            return PartialView();
        }

        #region Métodos de consulta principal
        [HttpPost]
        [ValidateInput(false)]
        public PartialViewResult CostPerContainerResults(
            int? id_documentState, string number, string reference, int? año, int? mes,
            DateTime? startEmissionDate, DateTime? endEmissionDate, bool isCallback = false)
        {
            var model = db.CostPerContenedor.ToList();
            if (año.HasValue)
            {
                model = model.Where(e => e.año == año).ToList();
            }

            if (mes.HasValue)
            {
                model = model.Where(e => e.mes == mes).ToList();
            }

            if (id_documentState.HasValue)
            {
                model = model.Where(e => e.Document.id_documentState == id_documentState).ToList();
            }

            if (!String.IsNullOrEmpty(number))
            {
                model = model.Where(e => e.Document.number.Contains(number)).ToList();
            }

            if (!String.IsNullOrEmpty(reference))
            {
                model = model.Where(e => e.Document.reference.Contains(reference)).ToList();
            }

            if (startEmissionDate.HasValue)
            {
                model = model.Where(e => DateTime.Compare(e.Document.emissionDate.Date, startEmissionDate.Value.Date) >= 0).ToList();
            }

            if (endEmissionDate.HasValue)
            {
                model = model.Where(e => DateTime.Compare(e.Document.emissionDate.Date, endEmissionDate.Value.Date) <= 0).ToList();
            }

            // Retornar la vista...
            this.ViewBag.QueryCriteria = new Dictionary<string, object>()
            {
                { "Id_documentState", id_documentState },
                { "Number", number },
                { "Reference", reference },
                { "Anio", año },
                { "Mes", mes },
                { "StartEmissionDate", startEmissionDate.ToIsoDateFormat() },
                { "EndEmissionDate", endEmissionDate.ToIsoDateFormat() },
            };

            // Retornamos la fecha
            if (isCallback)
            {
                return this.PartialView("_QueryGridViewPartial", model);
            }
            else
            {
                return this.PartialView("_QueryResultsPartial", model);
            }
        }
        #endregion

        #region Métodos de Edición
        [HttpPost]
        public PartialViewResult EditForm(int? id, string successMessage)
        {
            this.TempData.Remove(m_CostPerContainerModelKey);

            var costPerContainer = this.GetEditingCostPerContainer(id);
            this.TempData[m_CostPerContainerModelKey] = costPerContainer;

            this.PrepareEditViewBag(costPerContainer);

            if (!String.IsNullOrWhiteSpace(successMessage))
            {
                this.ViewBag.EditMessage = successMessage;
            }

            return PartialView("_EditForm", costPerContainer);
        }


        [HttpPost]
        public JsonResult Save(int idCostPerContainer,
            int año, int mes, DateTime emissionDate, string description,
            string reference)
        {
            int? idCostPerContainerResult;
            string message;
            bool isValid;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // Verificamos que no exista un costeo por contenedor asignado
                    var costPerContainer = db.CostPerContenedor
                        .FirstOrDefault(e => e.año == año && e.mes == mes && e.id != idCostPerContainer);

                    if(costPerContainer != null)
                    {
                        throw new Exception("Ya existe un costo por contenedor al AÑO - PERIODO indicado.");
                    }

                    idCostPerContainerResult = (idCostPerContainer != 0)
                        ? this.SaveCostPerContainer(idCostPerContainer, año, mes, emissionDate, description, reference, out message, out isValid)
                        : this.CreateCostPerContainer(año, mes, emissionDate, description, reference, out message, out isValid);

                    if (!idCostPerContainerResult.HasValue)
                    {
                        throw new Exception(message);
                    }

                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    idCostPerContainerResult = null;
                    message = exception.Message;
                    isValid = false;
                }
            }

            var result = new
            {
                idCostPerContainer = idCostPerContainerResult,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private int? CreateCostPerContainer(int año, int mes,
            DateTime emissionDate, string description, string reference,
            out string message, out bool isValid)
        {
            int? idCostPerContainer;
            try
            {
                // Recuperamos el tipo de documento
                var documentType = db.DocumentType
                    .FirstOrDefault(dt => dt.code == m_TipoDocumentoCostPerContainer
                                        && dt.id_company == this.ActiveCompanyId);

                if (documentType == null)
                {
                    throw new ApplicationException("No existe registrado el tipo de documento: Costo por Contenedor.");
                }

                // Recuperamos el estado PENDIENTE
                var documentState = DataProviderCostProductValuation
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
                    emissionDate = emissionDate,
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

                // Creamos el documento de costo por contenedor
                var costPerContenedor = new CostPerContenedor()
                {
                    Document = document,
                    año = año,
                    mes = mes,

                    idUserCreate = this.ActiveUserId,
                    dateCreate = DateTime.Now,
                    idUserUpdate = this.ActiveUserId,
                    dateUpdate = DateTime.Now,
                };

                // Agregar los detalles de la costo por contenedor
                var costPerContainerTemp = (this.TempData[m_CostPerContainerModelKey] as CostPerContenedor);
                if (costPerContainerTemp != null)
                {
                    costPerContenedor.CostPerContenedorManualPoundDetail = costPerContainerTemp
                        .CostPerContenedorManualPoundDetail
                        .Where(d => d.activo)
                        .Select(e => new CostPerContenedorManualPoundDetail()
                        {
                            orden = e.orden,
                            id_costPoundManualFactor = e.id_costPoundManualFactor,
                            valor = e.valor,

                            activo = true,
                            idUserCreate = this.ActiveUserId,
                            dateCreate = DateTime.Now,
                            idUserUpdate = this.ActiveUserId,
                            dateUpdate = DateTime.Now,
                        })
                        .ToList();

                    costPerContenedor.CostPerContenedorInvoiceDetail = costPerContainerTemp
                        .CostPerContenedorInvoiceDetail
                        .Where(d => d.activo)
                        .Select(e => new CostPerContenedorInvoiceDetail()
                        {
                            orden = e.orden,
                            id_detalleFactura = e.id_detalleFactura,
                            librasVentaNeta = e.librasVentaNeta,
                            precioUnitVentaNeta = e.precioUnitVentaNeta,
                            netoVentaNeta = e.netoVentaNeta,
                            precioUnitMP = e.precioUnitMP,
                            precioUnitEmpaque = e.precioUnitEmpaque,
                            totalOtrosCostos = e.totalOtrosCostos,
                            costoUnit = e.costoUnit,
                            costoTotal = e.costoTotal,
                            utilidad = e.utilidad,

                            activo = true,
                            idUserCreate = this.ActiveUserId,
                            dateCreate = DateTime.Now,
                            idUserUpdate = this.ActiveUserId,
                            dateUpdate = DateTime.Now,
                        })
                        .ToList();
                }


                // Guardamos el documento
                db.Document.Add(document);
                db.CostPerContenedor.Add(costPerContenedor);
                db.SaveChanges();

                idCostPerContainer = costPerContenedor.id;
                message = "Documento creado exitosamente.";
                isValid = true;
            }
            catch (Exception exception)
            {
                idCostPerContainer = null;
                var exceptionMessage = exception.GetBaseException() != null
                    ? exception.GetBaseException().Message
                    : "No se ha podido recuperar los detalles del error";
                message = "Error al crear documento: " + exceptionMessage;
                isValid = false;
            }

            return idCostPerContainer;
        }

        private int? SaveCostPerContainer(int idCostPerContainer,
            int año, int mes, DateTime emissionDate,
            string description, string reference,
            out string message, out bool isValid)
        {
            int? idProductValuationResult;
            try
            {
                // Recuperar la entidad actual
                var costPerContenedor = db.CostPerContenedor
                    .First(c => c.id == idCostPerContainer);

                // Verificamos el estado actual del documento
                var documentStateCode = costPerContenedor.Document.DocumentState.code;

                if (documentStateCode != m_PendienteDocumentState)
                {
                    throw new ApplicationException("Acción es permitida solo para Documento en estado PENDIENTE.");
                }

                // Actualizamos el documento
                var document = costPerContenedor.Document;
                document.emissionDate = emissionDate;
                document.description = description;
                document.reference = reference;
                document.id_userUpdate = this.ActiveUserId;
                document.dateUpdate = DateTime.Now;

                // Actualizamos el documento de costo por contenedor
                costPerContenedor.año = año;
                costPerContenedor.mes = mes;
                costPerContenedor.idUserCreate = this.ActiveUserId;
                costPerContenedor.dateUpdate = DateTime.Now;

                var costPerContenedorTemp = (this.TempData[m_CostPerContainerModelKey] as CostPerContenedor);
                if (costPerContenedorTemp != null)
                {
                    // Procesamos el detalle manual de libras
                    if (costPerContenedorTemp.CostPerContenedorManualPoundDetail != null)
                    {
                        var costPerContenedorManualPoundDetailsTemp = costPerContenedorTemp
                            .CostPerContenedorManualPoundDetail
                            .Where(d => d.activo)
                            .ToList();

                        // Sobreescribimos todos los detalles actuales con los nuevos valores, si hubiera...
                        foreach (var detail in costPerContenedor.CostPerContenedorManualPoundDetail)
                        {
                            if (costPerContenedorManualPoundDetailsTemp.Any())
                            {
                                // Actualizamos los detalles
                                var detailTemp = costPerContenedorManualPoundDetailsTemp[0];
                                costPerContenedorManualPoundDetailsTemp.RemoveAt(0);

                                detail.orden = detailTemp.orden;
                                detail.id_costPoundManualFactor = detailTemp.id_costPoundManualFactor;
                                detail.valor = detailTemp.valor;
                                detail.activo = true;

                                detail.idUserCreate = this.ActiveUserId;
                                detail.dateUpdate = DateTime.Now;
                            }
                            else
                            {
                                // Ya no hay detalles nuevos, desactivar...
                                detail.valor = 0;
                                detail.activo = false;

                                detail.idUserCreate = this.ActiveUserId;
                                detail.dateUpdate = DateTime.Now;
                            }
                        }

                        // Agregamos los detalles que faltan de agregar
                        foreach (var detailTemp in costPerContenedorManualPoundDetailsTemp)
                        {
                            costPerContenedor.CostPerContenedorManualPoundDetail
                                .Add(new CostPerContenedorManualPoundDetail()
                                {
                                    id_costPerContenedor = idCostPerContainer,
                                    orden = detailTemp.orden,
                                    id_costPoundManualFactor = detailTemp.id_costPoundManualFactor,
                                    valor = detailTemp.valor,

                                    activo = true,
                                    idUserUpdate = this.ActiveUserId,
                                    dateUpdate = DateTime.Now,
                                });
                        }
                    }
                    else
                    {
                        // No hay detalles: desactivar todos los elementos actuales
                        foreach (var detail in costPerContenedor.CostPerContenedorManualPoundDetail)
                        {
                            detail.valor = 0;
                            detail.activo = false;

                            detail.idUserUpdate = this.ActiveUserId;
                            detail.dateUpdate = DateTime.Now;
                        }
                    }

                    // Procesamos el detalle de costo
                    if (costPerContenedorTemp.CostPerContenedorInvoiceDetail != null)
                    {
                        var costPerContenedorInvoiceDetailTemp = costPerContenedorTemp
                            .CostPerContenedorInvoiceDetail
                            .Where(d => d.activo)
                            .ToList();

                        // Sobreescribimos todos los detalles actuales con los nuevos valores, si hubiera...
                        foreach (var detail in costPerContenedor.CostPerContenedorInvoiceDetail)
                        {
                            if (costPerContenedorInvoiceDetailTemp.Any())
                            {
                                // Actualizamos los detalles
                                var detailTemp = costPerContenedorInvoiceDetailTemp[0];
                                costPerContenedorInvoiceDetailTemp.RemoveAt(0);

                                detail.orden = detailTemp.orden;
                                detail.id_detalleFactura = detailTemp.id_detalleFactura;
                                detail.librasVentaNeta = detailTemp.librasVentaNeta;
                                detail.precioUnitVentaNeta = detailTemp.precioUnitVentaNeta;
                                detail.netoVentaNeta = detailTemp.netoVentaNeta;
                                detail.precioUnitMP = detailTemp.precioUnitMP;
                                detail.precioUnitEmpaque = detailTemp.precioUnitEmpaque;
                                detail.totalOtrosCostos = detailTemp.totalOtrosCostos;
                                detail.costoUnit = detailTemp.costoUnit;
                                detail.costoTotal = detailTemp.costoTotal;
                                detail.utilidad = detailTemp.utilidad;
                                detail.activo = true;

                                detail.idUserCreate = this.ActiveUserId;
                                detail.dateUpdate = DateTime.Now;
                            }
                            else
                            {
                                // Ya no hay detalles nuevos, desactivar...
                                detail.librasVentaNeta = null;
                                detail.precioUnitVentaNeta = null;
                                detail.netoVentaNeta = null;
                                detail.precioUnitMP = null;
                                detail.precioUnitEmpaque = null;
                                detail.totalOtrosCostos = null;
                                detail.costoUnit = null;
                                detail.costoTotal = null;
                                detail.utilidad = null;
                                detail.activo = false;

                                detail.idUserCreate = this.ActiveUserId;
                                detail.dateUpdate = DateTime.Now;
                            }
                        }

                        // Agregamos los detalles que faltan de agregar
                        foreach (var detailTemp in costPerContenedorInvoiceDetailTemp)
                        {
                            costPerContenedor.CostPerContenedorInvoiceDetail
                                .Add(new CostPerContenedorInvoiceDetail()
                                {
                                    id_costPerContenedor = idCostPerContainer,
                                    orden = detailTemp.orden,
                                    id_detalleFactura = detailTemp.id_detalleFactura,
                                    librasVentaNeta = detailTemp.librasVentaNeta,
                                    precioUnitVentaNeta = detailTemp.precioUnitVentaNeta,
                                    netoVentaNeta = detailTemp.netoVentaNeta,
                                    precioUnitMP = detailTemp.precioUnitMP,
                                    precioUnitEmpaque = detailTemp.precioUnitEmpaque,
                                    totalOtrosCostos = detailTemp.totalOtrosCostos,
                                    costoUnit = detailTemp.costoUnit,
                                    costoTotal = detailTemp.costoTotal,
                                    utilidad = detailTemp.utilidad,

                                    activo = true,
                                    idUserUpdate = this.ActiveUserId,
                                    dateUpdate = DateTime.Now,
                                });
                        }
                    }
                    else
                    {
                        // No hay detalles: desactivar todos los elementos actuales
                        foreach (var detail in costPerContenedor.CostPerContenedorInvoiceDetail)
                        {
                            detail.librasVentaNeta = null;
                            detail.precioUnitVentaNeta = null;
                            detail.netoVentaNeta = null;
                            detail.precioUnitMP = null;
                            detail.precioUnitEmpaque = null;
                            detail.totalOtrosCostos = null;
                            detail.costoUnit = null;
                            detail.costoTotal = null;
                            detail.utilidad = null;
                            detail.activo = false;

                            detail.idUserUpdate = this.ActiveUserId;
                            detail.dateUpdate = DateTime.Now;
                        }
                    }
                }

                // Guardamos el documento
                db.SaveChanges();

                idProductValuationResult = costPerContenedor.id;
                message = "Documento actualizado exitosamente.";
                isValid = true;
            }
            catch (Exception exception)
            {
                this.TempData.Keep(m_CostPerContainerModelKey);

                var exceptionMessage = exception.GetBaseException() != null
                    ? exception.GetBaseException().Message
                    : "No se ha podido recuperar los detalles del error";

                idProductValuationResult = null;
                message = "Error al actualizar documento: " + exceptionMessage;
                isValid = false;
            }

            return idCostPerContainer;
        }


        [HttpPost]
        public JsonResult Process(int idCostPerContainer, 
            int año, int mes, CostManualPoundEditing[] detalles)
        {
            int? idCostPerContainerResult;
            string message;
            bool isValid;

            try
            {
                detalles = detalles ?? new CostManualPoundEditing[] { };
                var costPerContainer = this.GetEditingCostPerContainer(idCostPerContainer);

                costPerContainer.año = año;
                costPerContainer.mes = mes;

                // Calcular los coeficientes...
                this.CalcularCostosContenedor(costPerContainer, detalles, out var codigosCostosAdicionales);

                // Guardamos los valores cálculados en memoria temporal
                this.TempData[m_CodigosCostoAdicionlesModelKey] = codigosCostosAdicionales;
                this.TempData.Keep(m_CodigosCostoAdicionlesModelKey);

                this.TempData[m_CostPerContainerModelKey] = costPerContainer;
                this.TempData.Keep(m_CostPerContainerModelKey);

                idCostPerContainerResult = costPerContainer.id;
                message = "Documento procesado exitosamente.";
                isValid = true;
            }
            catch (Exception exception)
            {
                this.TempData.Keep(m_CostPerContainerModelKey);

                idCostPerContainerResult = null;
                message = "Error al procesar documento: " + exception.Message;
                isValid = false;
            }

            var result = new
            {
                idCostPerContainer = idCostPerContainerResult,
                message,
                isValid,
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Métodos del Detalle
        public PartialViewResult CostPerContainerDetails(int? idCostPerContainer)
        {
            var detallesCustom = this.GetCustomDetailsFromTempData();
            this.ViewBag.CodigosCostoAdicionles = this.GetCodigosCostoAdicionles();

            return this.PartialView("_EditDetailsPartial", detallesCustom);
        }

        private CostoContenedorCustomModel[] ConvertToDTO(
            IEnumerable<CostPerContenedorInvoiceDetail> detallesFactura,
            IEnumerable<CostManualPoundEditing> detallesManuales)
        {
            var retorno = new List<CostoContenedorCustomModel>();

            // Procesamos los detalles manuales 
            foreach (var detalleFactura in detallesFactura)
            {
                var invoiceDetail = db.InvoiceDetail.FirstOrDefault(e => e.id == detalleFactura.id_detalleFactura);
                var invoice = invoiceDetail.Invoice;
                var invoiceExterior = invoice.InvoiceExterior;
                var document = invoice.Document;
                var item = this.GetItemFromTempData(invoiceDetail.id_item);
                var tipoItem = item.ItemType.code;

                decimal? numBoxes = Convert.ToDecimal(invoiceDetail.numBoxes);
                decimal? totalBoxes = Convert.ToDecimal(invoiceExterior.totalBoxes);
                var factorDistribucion = totalBoxes > 0
                    ? (numBoxes / totalBoxes * 1m)
                    : 0m;

                var valorFlete = invoiceExterior.valueInternationalFreight * factorDistribucion;
                var valorSeguro = invoiceExterior.valueInternationalInsurance * factorDistribucion;
                var valorTotal = detalleFactura.netoVentaNeta + valorFlete + valorSeguro;
                var ventaLibras = detalleFactura.librasVentaNeta > 0
                    ? valorTotal / detalleFactura.librasVentaNeta
                    : 0m;

                // Procesamos los costos adicionales
                var costosAdicionales = new List<CostoContenedorCustomModel.CostoAdicional>();
                for (int i = 0; i < detallesManuales.Count(); i++)
                {
                    var detalleManual = detallesManuales.ElementAt(i);
                    var costPoundManualFactor = db.CostPoundManualFactor.FirstOrDefault(e => e.id == detalleManual.IdCostPoundManualFactor);
                    // Generamos el split
                    var tipoItemAsociados = !String.IsNullOrEmpty(costPoundManualFactor.codTiposItem)
                        ? costPoundManualFactor.codTiposItem.Split(m_splitCharacter)
                        : new string[] { };

                    decimal valor = detalleManual.Valor;
                    if (tipoItemAsociados.Any())
                    {
                        valor = tipoItemAsociados.Contains(tipoItem) ? valor : 0m;
                    }

                    var total = valor * detalleFactura.librasVentaNeta;
                    costosAdicionales.Add(new CostoContenedorCustomModel.CostoAdicional()
                    {
                        Id = costPoundManualFactor.id,
                        Codigo = costPoundManualFactor.code,
                        Nombre = costPoundManualFactor.name,
                        Valor = valor,
                        Total = total != 0
                            ? total : null,
                    });
                }

                retorno.Add(new CostoContenedorCustomModel()
                {
                    NumSerie = document.number.Substring(0, 7),
                    NumFactura = Convert.ToInt32(document.number.Substring(8, 9)).ToString(),
                    FechaEmision = document.emissionDate,
                    NumContenedor = invoiceExterior.containers,
                    PasiDestino = invoiceExterior?.Port?.City?.Country?.name ?? string.Empty,
                    Cliente = invoiceExterior.Person1.fullname_businessName,
                    CodProducto = item.masterCode,
                    NombreProducto = item.name,
                    Talla = item.ItemGeneral.ItemSize.name,
                    NumCartones = invoiceDetail.numBoxes ?? 0m,
                    PrecioUnitCartones = invoiceDetail.unitPrice,
                    LibrasNetas = detalleFactura.librasVentaNeta,
                    PrecioLibra = detalleFactura.precioUnitVentaNeta,
                    ValorTotalFOB = detalleFactura.netoVentaNeta,
                    ValorFlete = valorFlete != 0 ? valorFlete : null,
                    ValorSeguro = valorSeguro != 0 ? valorSeguro : null,
                    ValorTotal = valorTotal != 0 ? valorTotal : null,
                    VentaPorLibras = ventaLibras != 0 ? ventaLibras : null,
                    ValorMateriaPrima = detalleFactura.precioUnitMP,
                    TotalMateriaPrima = detalleFactura.precioUnitMP != 0
                        ? detalleFactura.precioUnitMP * detalleFactura.librasVentaNeta : null,
                    MaterialesEmpaque = detalleFactura.precioUnitEmpaque,
                    TotalMaterialesEmpaque = detalleFactura.precioUnitEmpaque != 0
                        ? detalleFactura.precioUnitEmpaque * detalleFactura.librasVentaNeta : null,
                    CostosAdicionales = costosAdicionales.ToArray(),
                    CostoUnitario = detalleFactura.costoUnit,
                    CostoTotal = detalleFactura.costoTotal,
                    UtilidadFOB = detalleFactura.utilidad,
                    Utilidad = valorTotal - detalleFactura.costoTotal,
                    EsPerdidaFOB = detalleFactura.utilidad < 0,
                    EsPerdida = (valorTotal - detalleFactura.costoTotal) < 0,
                });
            }

            #region Ordenamos el detalle
            retorno = retorno
                .OrderBy(e => e.FechaEmision.Date)
                .ThenBy(e => e.NumFactura)
                .ToList();

            int orden = 0;
            retorno.ForEach(e =>
            {
                orden++;
                e.Orden = orden;
            });
            #endregion

            return retorno
                .ToArray();
        }

        #endregion

        #region Métodos del detalle de factores
        public class CostManualPoundEditing
        {
            public int IdCostPoundManualFactor { get; set; }
            public decimal Valor { get; set; }
        }

        public PartialViewResult CostContenedorManualPoundDetail(
            int idCostPerContainer, int? año, int? mes, CostManualPoundEditing[] detalles)
        {
            var costPerContenedor = this.GetEditingCostPerContainer(idCostPerContainer);

            detalles = detalles ?? new CostManualPoundEditing[] { };
            if (año.HasValue && mes.HasValue)
            {
                var detallesModel = this.GetCostPoundManualFactor(año.Value, mes.Value);
                for (int i = 0; i < detallesModel.Length; i++)
                {
                    var detalle = detallesModel[i];
                    var valor = detalles.FirstOrDefault(e => e.IdCostPoundManualFactor == detalle.id_costPoundManualFactor)?.Valor;

                    if (valor.HasValue)
                        detallesModel[i].valor = valor.Value;
                }

                costPerContenedor.CostPerContenedorManualPoundDetail = detallesModel;
            }
            else
            {
                costPerContenedor.CostPerContenedorManualPoundDetail = new CostPerContenedorManualPoundDetail[] { };
            }
           
            this.TempData[m_CostPerContainerModelKey] = costPerContenedor;
            this.TempData.Keep(m_CostPerContainerModelKey);

            return this.PartialView("_EditPoundDetailsPartial", costPerContenedor.CostPerContenedorManualPoundDetail.ToList());
        }

        public void UpdateCostContenedorManualPoundDetailTempData(int idCostPerContainer, CostManualPoundEditing[] detalles)
        {
            var costPerContenedor = this.GetEditingCostPerContainer(idCostPerContainer);
            detalles = detalles ?? new CostManualPoundEditing[] { };

            var factoresDetalles = costPerContenedor.CostPerContenedorManualPoundDetail.ToArray();
            for (int i = 0; i < factoresDetalles.Length; i++)
            {
                var factorDetalle = factoresDetalles[i];
                var valor = detalles.FirstOrDefault(e => e.IdCostPoundManualFactor == factorDetalle.id_costPoundManualFactor)?.Valor;

                if (valor.HasValue)
                    factoresDetalles[i].valor = valor.Value;
            }

            costPerContenedor.CostPerContenedorManualPoundDetail = factoresDetalles;
            this.TempData[m_CostPerContainerModelKey] = costPerContenedor;
            this.TempData.Keep(m_CostPerContainerModelKey);
        }
        #endregion

        #region Cálculos de distribución de costos
        private void CalcularCostosContenedor(
            CostPerContenedor costPerContenedor, CostManualPoundEditing[] detallesCostoManuales, 
            out string[] codigosCostosAdicionales)
        {
            // Recuperamos las ejecuciones aprobadas no valorizadas para el período indicado
            detallesCostoManuales = detallesCostoManuales ?? new CostManualPoundEditing[] { };
            var idAprobadoDocumentState = DataProviderCostCoefficientExecution
                .GetDocumentStateByCode(this.ActiveCompanyId, m_AprobadoDocumentState)?
                .id;

            var idUmLibra = db.MetricUnit.FirstOrDefault(e => e.code == "Lbs")?.id;
            if (!idUmLibra.HasValue)
                throw new Exception("Unidad de medida LIBRA no configurada.");

            var conversiones = db.MetricUnitConversion
                .Where(e => e.id_metricDestiny == idUmLibra)
                .ToArray();

            var año = costPerContenedor.año;
            var mes = costPerContenedor.mes;

            // Procesamos el costo unitario de las compras
            var costosMP = this.RecuperarCostoMateriaPrima(año, mes);
            var detallesFactura = this.RecuperarUtilidadFacturaExterior(año, mes);

            var idsItemFactura = detallesFactura
                .Select(e => e.IdItem)
                .Distinct()
                .ToArray();
            var costosLibrasProductoEmpaque = this.RecuperarCostoLbFormulation(
                año, mes, idsItemFactura, idUmLibra.Value, conversiones);

            #region Aplicación de costos manuales
            var manualCostPounds = db.CostPoundManualFactor
                .Where(e => e.año == año && e.mes == mes && e.isActive)
                .ToArray();

            codigosCostosAdicionales = manualCostPounds.Select(e => e.name).ToArray();
            var costosAdicionales = manualCostPounds
                .Select(e => new CostoContenedorCustomModel.CostoAdicional()
                {
                    Id = e.id,
                    Codigo = e.code,
                    Nombre = e.name,
                    Valor = 0,
                    CodigosTiposItem = !String.IsNullOrEmpty(e.codTiposItem)
                        ? e.codTiposItem.Split(m_splitCharacter) : new string[] { },
                })
                .ToArray();

            for (int i = 0; i < costosAdicionales.Length; i++)
            {
                var costoAdicional = costosAdicionales[i];

                var valor = detallesCostoManuales
                    .FirstOrDefault(e => e.IdCostPoundManualFactor == costoAdicional.Id)?.Valor;

                if (valor.HasValue)
                    costosAdicionales[i].Valor = valor.Value;
            }
            #endregion

            var detalles = new List<CostPerContenedorInvoiceDetail>();
            //foreach (var detalleFactura in detallesFactura)
            for (int i = 0; i < detallesFactura.Length; i++)
            {
                var detalleFactura = detallesFactura[i];
                var invoiceDetail = db.InvoiceDetail.FirstOrDefault(e => e.id == detalleFactura.IdDetalleFactura);
                var item = this.GetItemFromTempData(invoiceDetail.id_item);

                var costoMp = costosMP.FirstOrDefault(e => e.IdClase == detalleFactura.IdClase
                        && e.IdTalla == detalleFactura.IdTalla && e.CodItemType == item.ItemType.code)?
                    .CostoUnitario ?? 0m;

                var costoEmpaque = costosLibrasProductoEmpaque
                    .FirstOrDefault(e => e.IdItem == detalleFactura.IdItem)?
                    .CostoUnitLb ?? 0m;

                var costosAdicionalesAplicar = costosAdicionales.ToList();
                costosAdicionalesAplicar.ForEach(e =>
                {
                    if (e.CodigosTiposItem.Any())
                    {
                        e.Valor = e.CodigosTiposItem.Contains(item.ItemType.code) ? e.Valor : 0m;
                    }
                });

                var costosAdicionalesTot = costosAdicionalesAplicar.Sum(e => e.Valor);
                var costoUnitario = costoMp + costoEmpaque + costosAdicionalesTot;
                var costoTotal = costoUnitario * detalleFactura.Cantidad;
                var utilidad = detalleFactura.Total - costoTotal;


                detalles.Add(new CostPerContenedorInvoiceDetail()
                { 
                    orden = i + 1,
                    id_detalleFactura = detalleFactura.IdDetalleFactura,
                    librasVentaNeta = detalleFactura.Cantidad,
                    precioUnitVentaNeta = detalleFactura.CostoUnitario,
                    netoVentaNeta = detalleFactura.Total,
                    precioUnitMP = costoMp,
                    precioUnitEmpaque = costoEmpaque,
                    totalOtrosCostos = costosAdicionalesTot,
                    costoUnit = costoUnitario,
                    costoTotal = costoTotal,
                    utilidad = utilidad,
                    activo = true,
                    idUserCreate = this.ActiveUserId,
                    dateCreate = DateTime.Now,
                    idUserUpdate = this.ActiveUserId,
                    dateUpdate = DateTime.Now,
                    InvoiceDetail = invoiceDetail,
                });
            }

            costPerContenedor.CostPerContenedorInvoiceDetail = detalles;

            // Guardamos los detalles en memoria temporal
            var detallesCustom = this.ConvertToDTO(detalles, detallesCostoManuales); 
            this.TempData[m_DetallesCustomModelKey] = detallesCustom;
            this.TempData.Keep(m_DetallesCustomModelKey);
        }
        public partial class CostoContenedorCustomModel
        {
            public int Orden { get; set; }
            public string NumSerie { get; set; }
            public string NumFactura { get; set; }
            public DateTime FechaEmision { get; set; }
            public string NumContenedor { get; set; }
            public string PasiDestino { get; set; }
            public string Cliente { get; set; }
            public string CodProducto { get; set; }
            public string NombreProducto { get; set; }
            public string Talla { get; set; }
            public decimal? NumCartones { get; set; }
            public decimal? PrecioUnitCartones { get; set; }
            public decimal? LibrasNetas { get; set; }
            public decimal? PrecioLibra { get; set; }
            public decimal? ValorTotalFOB { get; set; }
            public decimal? ValorFlete { get; set; }
            public decimal? ValorSeguro { get; set; }
            public decimal? ValorTotal { get; set; }
            public decimal? VentaPorLibras { get; set; }
            public decimal? ValorMateriaPrima { get; set; }
            public decimal? TotalMateriaPrima { get; set; }
            public decimal? MaterialesEmpaque { get; set; }
            public decimal? TotalMaterialesEmpaque { get; set; }
            public CostoAdicional[] CostosAdicionales { get; set; }
            public decimal? CostoUnitario { get; set; }
            public decimal? CostoTotal { get; set; }
            public decimal? UtilidadFOB { get; set; }
            public decimal? Utilidad { get; set; }
            public bool EsPerdidaFOB { get; set; }
            public bool EsPerdida { get; set; }

            // Constructores
            public CostoContenedorCustomModel()
            {
                this.CostosAdicionales = new CostoAdicional[] { };
            }

            // Clases adicionales
            public class CostoAdicional
            {
                public int Id { get; set; }
                public string Codigo { get; set; }
                public string Nombre { get; set; }
                public decimal? Valor { get; set; }
                public decimal? Total { get; set; }
                public string[] CodigosTiposItem { get; set; }

                public CostoAdicional()
                {
                    this.CodigosTiposItem = new string[] { };
                }
            }
        }


        private class CostoMateriaPrima
        {
            public int IdItemType { get; set; }
            public string CodItemType { get; set; }
            public int IdClase { get; set; }
            public int IdTalla { get; set; }
            public decimal CantidadComprada { get; set; }
            public decimal CostoTotal { get; set; }
            public decimal CostoUnitario { get; set; }
        }

        private CostoMateriaPrima[] RecuperarCostoMateriaPrima(int año, int mes)
        {
            var lstParametersSql = new[]
            {
                new ParamSQL() {
                    Nombre = "@año",
                    TipoDato = DbType.Int32,
                    Valor = año,
                },
                new ParamSQL() {
                    Nombre = "@mes",
                    TipoDato = DbType.Int32,
                    Valor = mes,
                },
            }
            .ToList();

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];


            DataSet dataSet = AccesoDatos.MSSQL.MetodosDatos2
                .ObtieneDatos(_cadenaConexion, "Get_ItemClass_MP_Period", _rutaLog,
                    m_CostPerContainerModelKey, "PROD", lstParametersSql);

            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                var resultados = dataSet.Tables[0].AsEnumerable();

                return resultados
                    .Select(e => new CostoMateriaPrima()
                    {
                        IdItemType = e.Field<Int32>("IdItemType"),
                        CodItemType = e.Field<string>("CodItemType"),
                        IdClase = e.Field<Int32>("IdClass"),
                        IdTalla = e.Field<Int32>("IdSize"),
                        CantidadComprada = e.Field<decimal>("Cantidad"),
                        CostoTotal = e.Field<decimal>("Total"),
                        CostoUnitario = e.Field<decimal>("Precio"),
                    })
                    .ToArray();
            }
            else
            {
                return new CostoMateriaPrima[] { };
            }
        }

        private class UtilidadFacturaExterior
        {
            public int IdFactura { get; set; }
            public int IdDetalleFactura { get; set; }
            public int IdItem { get; set; }
            public int IdClase { get; set; }
            public int IdTalla { get; set; }
            public decimal Cantidad { get; set; }
            public decimal CostoUnitario { get; set; }
            public decimal Total { get; set; }
        }

        private UtilidadFacturaExterior[] RecuperarUtilidadFacturaExterior(int año, int mes)
        {
            var lstParametersSql = new[]
            {
                new ParamSQL() {
                    Nombre = "@año",
                    TipoDato = DbType.Int32,
                    Valor = año,
                },
                new ParamSQL() {
                    Nombre = "@mes",
                    TipoDato = DbType.Int32,
                    Valor = mes,
                },
            }
            .ToList();

            string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextNE"].ConnectionString;
            string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

            DataSet dataSet = AccesoDatos.MSSQL.MetodosDatos2
                .ObtieneDatos(_cadenaConexion, "Get_FacturaExteriorLbsPeriod_Data", _rutaLog,
                    m_CostPerContainerModelKey, "PROD", lstParametersSql);

            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                var resultados = dataSet.Tables[0].AsEnumerable();

                return resultados
                    .Select(e => new UtilidadFacturaExterior()
                    {
                        IdFactura = e.Field<Int32>("IdFactura"),
                        IdDetalleFactura = e.Field<Int32>("IdDetalleFactura"),
                        IdItem = e.Field<Int32>("IdItem"),
                        IdClase = e.Field<Int32>("IdClass"),
                        IdTalla = e.Field<Int32>("IdSize"),
                        Cantidad = e.Field<decimal>("Cantidad"),
                        Total = e.Field<decimal>("ValorNeto"),
                        CostoUnitario = e.Field<decimal>("Precio"),
                    })
                    .ToArray();
            }
            else
            {
                return new UtilidadFacturaExterior[] { };
            }
        }

        private partial class ItemPoudCost
        {
            public int IdItem { get; set; }
            public decimal CostoUnitLb { get; set; }
            public ItemPart[] Formulations { get; set; }                       

            public partial class ItemPart
            {
                public int IdItem { get; set; }
                public string AuxCode { get; set; }
                public decimal CostoTotal { get; set; }
            }

            #region Constructores
            public ItemPoudCost()
            {
                this.Formulations = new ItemPart[] { };
            }
            #endregion
        }

        private ItemPoudCost[] RecuperarCostoLbFormulation(
            int año, int mes, int[] idsItem, int idUmLibra, MetricUnitConversion[] conversiones)
        {
            var retornos = new List<ItemPoudCost>();
            for (int i = 0; i < idsItem.Length; i++)
            {
                var idItem = idsItem[i];
                var item = this.GetItemFromTempData(idItem);
                var formulations = db.Item
                    .FirstOrDefault(e => e.id == idItem)
                    .ItemIngredient;

                var ingredientes = new List<ItemPoudCost.ItemPart>();
                foreach (var formulation in formulations)
                {
                    var itemIngredient = formulation.Item1;
                    var precioUnitario = this.GetAveragePriceFromTempData(año, mes, itemIngredient.auxCode) ?? 0m;
                    var costoTotal = (precioUnitario * formulation.amount) ?? 0m;

                    ingredientes.Add(new ItemPoudCost.ItemPart() 
                    { 
                        IdItem = itemIngredient.id,
                        AuxCode = itemIngredient.auxCode,
                        CostoTotal = costoTotal,
                    });
                }

                // Convertimos el precio del master a precio unitario
                decimal factorPresentacion = 0m;
                var costoTotalMaster = ingredientes.Sum(e => e.CostoTotal);
                if (item.Presentation.id_metricUnit != idUmLibra)
                {
                    var factor = conversiones
                        .FirstOrDefault(e => e.id_metricOrigin == item.Presentation.id_metricUnit)
                        .factor;
                    factorPresentacion = item.Presentation.minimum * item.Presentation.maximum * factor;
                }
                else
                {
                    factorPresentacion = item.Presentation.minimum * item.Presentation.maximum;
                }

                var costoUnitLb = factorPresentacion > 0m
                    ? costoTotalMaster / factorPresentacion
                    : 0m;

                retornos.Add(new ItemPoudCost() 
                { 
                    IdItem = item.id,
                    Formulations = ingredientes.ToArray(),
                    CostoUnitLb = costoUnitLb,
                });
            }

            return retornos
                .ToArray();
        }

        private decimal? GetAveragePriceFromTempData(
            int año, int mes, string codigoAlternativo)
        {
            var key = $"ItemAveragePrice_{codigoAlternativo}";
            if (TempData.ContainsKey(key))
            {
                return TempData[key] as decimal?;
            }
            else
            {
                var codEmpresa = this.ActiveCompany.code;
                var codDivision = this.ActiveDivision.code;
                var codSucursal = this.ActiveSucursal.code;

                var codBodUbiMatParam = db.Setting
                    .FirstOrDefault(e => e.code == "CODBODUBICON");

                if (codBodUbiMatParam == null)
                    throw new Exception("Parámetro no encontrado: Cód. bodega - ubicación de materiales en el sistema contable.");

                var codBodUbiMatParamParts = codBodUbiMatParam.value.Split(m_splitCharacter);

                if (codBodUbiMatParamParts.Length != 2)
                    throw new Exception("Parámetro no configurado correctamente: Cód. bodega - ubicación de materiales en el sistema contable.");


                var codBodegaMateriales = codBodUbiMatParamParts[0];
                var codUbicacionMateriales = codBodUbiMatParamParts[1];

                var inicioPeriodo = new DateTime(año, mes, 1);
                var finalPeriodo = inicioPeriodo.AddMonths(1).AddDays(-1);

                // Preparamos los parametros 
                var lstParametersSql = new[]
                {
                    new ParamSQL()
                    {
                        Nombre = "@CCiCia",
                        TipoDato = DbType.String,
                        Valor = codEmpresa,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@CCiDivision",
                        TipoDato = DbType.String,
                        Valor = codDivision,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@CCiSucursal",
                        TipoDato = DbType.String,
                        Valor = codSucursal,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@CCiBodega",
                        TipoDato = DbType.String,
                        Valor = codBodegaMateriales,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@CCiUbicacion",
                        TipoDato = DbType.String,
                        Valor = codUbicacionMateriales,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@CCiProducto",
                        TipoDato = DbType.String,
                        Valor = codigoAlternativo,
                    },
                    new ParamSQL()
                    {
                        Nombre = "@DFxDesde",
                        TipoDato = DbType.DateTime,
                        Valor = inicioPeriodo.ToIsoDateFormat(),
                    },
                    new ParamSQL()
                    {
                        Nombre = "@DFxHasta",
                        TipoDato = DbType.DateTime,
                        Valor = finalPeriodo.ToIsoDateFormat(),
                    },
                }.ToList();

                string _cadenaConexion = ConfigurationManager.ConnectionStrings["DBContextCIC"].ConnectionString;
                string _rutaLog = (string)ConfigurationManager.AppSettings["rutaLog"];

                var precioUnitario = AccesoDatos.MSSQL.MetodosDatos2
                    .ExecuteDecimalFunction(_cadenaConexion, "FUN_IV_PRECIO_PROMEDIO", _rutaLog,
                        m_CostPerContainerModelKey, "PROD", lstParametersSql);

                // Guardamos en memoria temporal
                TempData[key] = precioUnitario;
                TempData.Keep(key);

                return precioUnitario;
            }
        }
        #endregion

        #region Métodos adicionales
        private CostPerContenedor GetEditingCostPerContainer(int? id_costPerContenedor)
        {
            // Recuperamos el elemento del caché local
            var costPerContenedor = (this.TempData[m_CostPerContainerModelKey] as CostPerContenedor);

            // Si no hay elemento en el caché, consultamos desde la base
            if ((costPerContenedor == null) && id_costPerContenedor.HasValue)
            {
                costPerContenedor = db.CostPerContenedor
                    .Include("Document")
                    .FirstOrDefault(e => e.id == id_costPerContenedor);
            }

            // Si no existe, creamos un nuevo elemento
            return costPerContenedor ?? this.CreateNewCostPerContainer();
        }

        private CostPerContenedor CreateNewCostPerContainer()
        {
            // Recuperamos el tipo de documento
            var documentType = db.DocumentType
                .FirstOrDefault(dt => dt.code == m_TipoDocumentoCostPerContainer
                                    && dt.id_company == this.ActiveCompanyId);

            if (documentType == null)
            {
                throw new ApplicationException("No existe registrado el tipo de documento: Costo por Contenedor.");
            }

            // Recuperamos el estado PENDIENTE
            var documentState = DataProviderCostProductValuation
                .GetDocumentStateByCode(this.ActiveCompanyId, m_PendienteDocumentState);

            if (documentState == null)
            {
                throw new ApplicationException("No existe registrado el estado PENDIENTE para los documentos.");
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
                id_userCreate = this.ActiveUserId,
                dateCreate = DateTime.Now,
                id_userUpdate = this.ActiveUserId,
                dateUpdate = DateTime.Now,
            };

            var detalles = this.GetCostPoundManualFactor(DateTime.Today.Year, DateTime.Today.Month);
            // Creamos el documento de asignación de costos
            return new CostPerContenedor()
            {
                Document = document,
                año = DateTime.Today.Year,
                mes = DateTime.Today.Month,
                idUserCreate = this.ActiveUserId,
                dateCreate = DateTime.Now,
                idUserUpdate = this.ActiveUserId,
                dateUpdate = DateTime.Now,
                CostPerContenedorManualPoundDetail = detalles,
            };
        }
        private CostPerContenedorManualPoundDetail[] GetCostPoundManualFactor(int año, int mes)
        {
            var orden = 1;
            return db.CostPoundManualFactor
                .Where(e => e.año == año && e.mes == mes && e.isActive)
                .ToArray()
                .Select(e => new CostPerContenedorManualPoundDetail()
                {
                    id_costPoundManualFactor = e.id,
                    orden = orden++,
                    valor = e.valor,
                    activo = true,
                    idUserCreate = this.ActiveUserId,
                    dateCreate = DateTime.Now,
                    idUserUpdate = this.ActiveUserId,
                    dateUpdate = DateTime.Now,

                    CostPoundManualFactor = e,
                })
                .ToArray();
        }

        private void PrepareEditViewBag(CostPerContenedor costPerContenedor)
        {
            // Verificamos el estado actual del documento
            var documentStateCode = costPerContenedor.Document.DocumentState.code;
            var documentExists = costPerContenedor.id > 0;
            var canEditDocument = (documentStateCode == m_PendienteDocumentState);

            // Agregamos los elementos al ViewBag
            this.ViewBag.DocumentoExistente = documentExists;
            this.ViewBag.PuedeEditarDocumento = canEditDocument;

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
                        IsActive = (d.id_PeriodState == 2),
                    })
                    .ToArray();

                this.ViewBag.PeriodosUsables = periodosUsables;
                this.ViewBag.AnioListModel = periodosUsables
                    .Select(d => new { d.Anio })
                    .Distinct()
                    .ToArray();

                this.ViewBag.MesListModel = periodosUsables
                    .Where(d => d.Anio == costPerContenedor.año)
                    .Select(d => new { d.Mes })
                    .Distinct()
                    .ToArray();
            }
            else
            {
                this.ViewBag.PeriodosUsables = new object[] { };

                this.ViewBag.AnioListModel = new[]
                {
                    new { Anio = costPerContenedor.año, },
                };

                this.ViewBag.MesListModel = new[]
                {
                    new { Mes = costPerContenedor.mes, },
                };
            }

            this.ViewBag.ResponsableNombre = db.User
                .FirstOrDefault(u => u.id == costPerContenedor.Document.id_userUpdate)?
                .Employee?
                .Person?
                .fullname_businessName;

            // Procesamos los detalles
            var detallesFactorLibras = costPerContenedor
                .CostPerContenedorManualPoundDetail
                .Where(e => e.activo);

            var costosAdicionales = detallesFactorLibras
                .Select(e => e.CostPoundManualFactor.name)
                .ToArray();

            this.ViewBag.CodigosCostoAdicionles = costosAdicionales;
            var detallesFactor = detallesFactorLibras
                .Select(e => new CostManualPoundEditing()
                {
                    IdCostPoundManualFactor = e.id_costPoundManualFactor,
                    Valor = e.valor,
                })
                .ToArray();
            var detallesFactura = costPerContenedor
                .CostPerContenedorInvoiceDetail
                .Where(e => e.activo)
                .ToArray();

            this.ViewBag.CostPerContenedorInvoiceDetail = this.ConvertToDTO(detallesFactura, detallesFactor);
        }
        #endregion

        #region Métodos de recuperación desde memoria temporal
        private Item GetItemFromTempData(int idItem)
        {
            var key = $"Item_{idItem}";
            if (TempData.ContainsKey(key))
            {
                return TempData[key] as Item;
            }
            else
            {
                var data = db.Item.FirstOrDefault(e => e.id == idItem);
                TempData[key] = data;
                TempData.Keep(key);

                return data;
            }
        }

        private CostoContenedorCustomModel[] GetCustomDetailsFromTempData()
        {
            CostoContenedorCustomModel[] retorno;
            if (TempData.ContainsKey(m_DetallesCustomModelKey))
            {
                retorno = TempData[m_DetallesCustomModelKey] as CostoContenedorCustomModel[];
            }
            else
            {
                retorno = new CostoContenedorCustomModel[] { };
            }

            // Grabamos nuevamente la info en memoria temporal
            TempData[m_DetallesCustomModelKey] = retorno;
            TempData.Keep(m_DetallesCustomModelKey);

            return retorno;
        }

        private string[] GetCodigosCostoAdicionles()
        {
            string[] retorno;
            if (TempData.ContainsKey(m_CodigosCostoAdicionlesModelKey))
            {
                retorno = TempData[m_CodigosCostoAdicionlesModelKey] as string[];
            }
            else
            {
                retorno = new string[] { };
            }

            // Grabamos nuevamente la info en memoria temporal
            TempData[m_CodigosCostoAdicionlesModelKey] = retorno;
            TempData.Keep(m_CodigosCostoAdicionlesModelKey);

            return retorno;
        }
        #endregion
    }
}