
// Métodos generales

var UpdateTabValidationImage = function(tabIndex, isValid) {
	var imageUrl = isValid
		? "/Content/image/noimage.png"
		: "/Content/image/info-error.png";

	var tab = tpProductValuationEdit.GetTab(tabIndex);
	tab.SetImageUrl(imageUrl);
	tab.SetActiveImageUrl(imageUrl);
}

var ShowEditMessage = function(message) {
	if (message !== null && message.length > 0) {
		$("#messageAlert").html(message);

		$(".close").click(function() {
			$(".alert").alert('close');
			$("#messageAlert").empty();
		});
	}
}

// Eventos de controles de edición

var OnTabPageControlInit = function(s, e) {
	if (s.cpEditMessage) {
		ShowEditMessage(s.cpEditMessage);
	}
};

var OnAnioProductionCostSelectedIndexChanged = function(s, e) {
	mes.ClearItems();

	var anioValor = anio.GetValue();
	var esProyectado = (id_allocationType.GetValue() === id_allocationType.cpIdTipoAsignacionCostoProyectado);
	if (anioValor !== null) {
		var periodosUsables = anio.cpPeriodosUsables;
		var numPeriodos = periodosUsables.length;
		for (var i = 0; i < numPeriodos; i++) {
			var periodo = periodosUsables[ i ];
			if (periodo.Anio === anioValor) {
				if (esProyectado || periodo.IsActive) {
					mes.AddItem([ periodo.Mes, periodo.Mes ], periodo.Mes);
				}
			}
		}
	}

	mes.Validate();
};

var OnMesProductionCostSelectedIndexChanged = function(s, e) {
	onCalcularUltimaValorizacion();
};
var OnMesProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else if (anio.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnEmissionDateProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};
var OnFechaFinCostValidation = function(s, e) {
	var fechaFin = s.GetDate();
	var fechaFinInteger = null;
	if (fechaFin != null) {
		fechaFinInteger = parseInt(fechaFin.toISOString().substring(0, 10).replace(/-/g, ""));
	}

	if (fechaFinInteger == null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
	else if (fechaFinInteger > mes.cpFechaFinalInteger) {
		e.isValid = false;
		e.errorText = "Fecha de fin debe estar dentro del periodo indicado";
	}
};

var OnAllocationTypeProductionCostSelectedIndexChanged = function(s, e) {
	mes.ClearItems();

	var anioValor = anio.GetValue();
	var esProyectado = (id_allocationType.GetValue() === id_allocationType.cpIdTipoAsignacionCostoProyectado);
	if (anioValor !== null) {
		var periodosUsables = anio.cpPeriodosUsables;
		var numPeriodos = periodosUsables.length;
		for (var i = 0; i < numPeriodos; i++) {
			var periodo = periodosUsables[ i ];
			if (periodo.Anio === anioValor) {
				if (esProyectado || periodo.IsActive) {
					mes.AddItem([ periodo.Mes, periodo.Mes ], periodo.Mes);
				}
			}
		}
	}

	mes.Validate();
};
var OnAllocationTypeProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var onRecuperarEjecucionesButtonClick = function(s, e) {
	if (IsValidEditForm()) {
		var operationData = {
			idProductValuation: parseInt($("#id_productValuation").val()),
			anio: anio.GetValue(),
			mes: mes.GetValue(),
			idAllocationType: id_allocationType.GetValue()
		};
		var hideLoadingDialog = true;

		$.ajax({
			url: "CostProductValuation/Execute",
			type: "post",
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: JSON.stringify(operationData),
			async: true,
			cache: false,
			error: function(error) {
				console.error(error);
				hideLoading();
			},
			beforeSend: function() {
				showLoading();
			},
			success: function(result) {
				if (result.isValid) {
					hideLoadingDialog = false;

					var viewData = {
						idProductValuation: result.idProductValuation
					};

					$.ajax({
						url: "CostProductValuation/CostProductValuationDetails",
						type: "post",
						data: viewData,
						async: true,
						cache: false,
						error: function(error) {
							console.log(error);
							showErrorTitle(error);
						},
						beforeSend: function() {
							showLoading();
						},
						success: function(result) {
							$("#detallesEjecucion").html(result);

							anio.SetEnabled(false);
							mes.SetEnabled(false);
							fechaFin.SetEnabled(false);
							id_allocationType.SetEnabled(false);
							RecuperarEjecucionesEditButton.SetVisible(false);
							RestablecerEjecucionesEditButton.SetVisible(true);
						},
						complete: function() {
							hideLoading();
						}
					});
				} else {
					ShowEditMessage(result.message);
				}
			},
			complete: function() {
				if (hideLoadingDialog) {
					hideLoading();
				}
			}
		});
	}
};
var onRestablecerEjecucionesButtonClick = function (s, e) {
	if (IsValidEditForm()) {
		var operationData = {
			idProductValuation: parseInt($("#id_productValuation").val())
		};

		$.ajax({
			url: "CostProductValuation/Reset",
			type: "post",
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: JSON.stringify(operationData),
			async: true,
			cache: false,
			error: function(error) {
				console.error(error);
				hideLoading();
			},
			beforeSend: function() {
				showLoading();
			},
			success: function(result) {
				if (result.isValid) {
					anio.SetEnabled(true);
					mes.SetEnabled(true);
					id_allocationType.SetEnabled(true);
					fechaFin.SetEnabled(true);
					RecuperarEjecucionesEditButton.SetVisible(true);
					RestablecerEjecucionesEditButton.SetVisible(false);
					gvNovedadesValorizacion.PerformCallback();
					gvMovimientoValorizacion.PerformCallback();
					gvSaldosValorizacion.PerformCallback();
					ResumenCallbackPanel.PerformCallback();

					$("#detallesEjecucion").empty();
				} else {
					ShowEditMessage(result.message);
				}
			},
			complete: function() {
				hideLoading();
			}
		});
	}
};
var onCalcularUltimaValorizacion = function () {
	if (IsValidEditForm()) {
		var operationData = {
			año: anio.GetValue(),
			mes: mes.GetValue(),
			id_allocationType: id_allocationType.GetValue()
		};

		$.ajax({
			url: "CostProductValuation/CalcularUltimaValorizacion",
			type: "post",
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: JSON.stringify(operationData),
			async: true,
			cache: false,
			error: function (error) {
				console.error(error);
				hideLoading();
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result.isValid) {
					fechaInicio.SetText(result.fechaInicio);
					fechaFin.SetText(result.fechaFin);
					mes.cpFechaFinalInteger = result.fechaFinInteger;

					ResumenCallbackPanel.PerformCallback();
				} else {
					ShowEditMessage(result.message);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
};



// Acciones del formulario

var IsValidEditForm = function() {
	var isGeneralesValid = ASPxClientEdit.ValidateEditorsInContainerById("datosGeneralesEditForm", "", true);
	if (isGeneralesValid) {
		UpdateTabValidationImage(0, true);
	} else {
		UpdateTabValidationImage(0, false);
		tpProductValuationEdit.SetActiveTabIndex(0);
		return false;
	}

	return true;
};
var GetSelectedDetailsId = function () {
	var controlsCollection = ASPxClientControl.GetControlCollection();
	var costGroupGridView = controlsCollection.GetByName("gvCostProductValuationCostGroup");
	if (costGroupGridView === null) {
		return [];
	}

	var gruposCostoInfo = costGroupGridView.cpGruposCosto;
	if (gruposCostoInfo === null || gruposCostoInfo.length === 0) {
		return [];
	}

	var result = [];
	var numItems = gruposCostoInfo.length;
	for (var i = 0; i < numItems; i++) {
		var checkBox = controlsCollection.GetByName("UsarDetalleGrupoCostoEditCheckBox_" + i);
		if (checkBox !== null && checkBox.GetChecked()) {
			result = result.concat(gruposCostoInfo[ i ].idsDetalles);
		}
	}

	return result;
};

var PerformDocumentStoreAction = function(approveDocument) {
	if (IsValidEditForm()) {

		var idProductValuation = parseInt($("#id_productValuation").val());
		var actionUrl = (idProductValuation === 0) ? "CostProductValuation/Create" : "CostProductValuation/Save";
		var fechaInicioDate = fechaInicio.GetDate();
		var fechaInicioVal = fechaInicioDate != null
			? fechaInicioDate.toISOString().substring(0, 10)
			: null;
		var fechaFinDate = fechaFin.GetDate();
		var ffechaFinVal = fechaFinDate != null
			? fechaFinDate.toISOString().substring(0, 10)
			: null;

		var operationData = {
			idProductValuation: idProductValuation,
			anio: anio.GetValue(),
			mes: mes.GetValue(),
			emissionDate: emissionDate.GetDate(),
			idAllocationType: id_allocationType.GetValue(),
			fechaInicio: fechaInicioVal,
			fechaFin: ffechaFinVal,

			description: description.GetText(),
			reference: reference.GetText(),
			detailsIds: GetSelectedDetailsId(),
			approveDocument: approveDocument
		};

		$.ajax({
			url: actionUrl,
			type: "post",
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: JSON.stringify(operationData),
			async: true,
			cache: false,
			error: function(error) {
				console.error(error);
				hideLoading();
			},
			beforeSend: function() {
				showLoading();
			},
			success: function(result) {
				if (result.isValid) {
					var successData = {
						id: result.idProductValuation,
						successMessage: result.message
					};
					showPage("CostProductValuation/EditForm", successData);
				} else {
					ShowEditMessage(result.message);
				}
			},
			complete: function() {
				hideLoading();
			}
		});
	}
};
var PerformDocumentStateAction = function(action) {
	var idProductValuation = parseInt($("#id_productValuation").val());
	var actionUrl = "CostProductValuation/" + action;

	var operationData = {
		idProductValuation: idProductValuation
	};

	$.ajax({
		url: actionUrl,
		type: "post",
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		data: JSON.stringify(operationData),
		async: true,
		cache: false,
		error: function(error) {
			console.error(error);
			hideLoading();
		},
		beforeSend: function() {
			showLoading();
		},
		success: function(result) {
			if (result.isValid) {
				var successData = {
					id: result.idProductValuation,
					successMessage: result.message
				};
				showPage("CostProductValuation/EditForm", successData);
			} else {
				ShowEditMessage(result.message);
			}
		},
		complete: function() {
			hideLoading();
		}
	});
};

var OnUpdateButtonClick = function() {
	PerformDocumentStoreAction(false);
};

var OnApproveButtonClick = function() {
	PerformDocumentStoreAction(true);
};

var OnCancelDocumentButtonClick = function() {
	PerformDocumentStateAction("Cancel");
};

var OnRevertButtonClick = function() {
	PerformDocumentStateAction("Revert");
};

var OnRefreshButtonClick = function() {
	var id = parseInt($("#id_productValuation").val());
	showPage("CostProductValuation/EditForm", { id: id });
};

var OnGenerateExcelReportButtonClick = function (tipo) {
	var id = parseInt($("#id_productValuation").val());

	if (id != null && id > 0) {
		
		var operationData = {
			id: id,
		};

		var hideLoadingDialog = true;
		$.ajax({
			url: "CostProductValuation/Report",
			type: "post",
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: JSON.stringify(operationData),
			async: true,
			cache: false,
			error: function (error) {
				console.error(error);
				hideLoading();
				showErrorMessage(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result.isValid) {
					NotifyWarning("Su reporte se está descargando, continúe con sus actividades.");
					var url = getFullPath('CostProductValuation/GenerateExcelDynamic');
					$('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
					hideLoading();
				} else {
					showWarningMessage(result.message);
				}
			},
			complete: function () {
				if (hideLoadingDialog) {
					hideLoading();
				}
			}
		});
	}
};

var OnCancelButtonClick = function(id) {
	showPage("CostProductValuation/Index", null);
};

// Procesamiento de Novedades
var onRecargarNovedadesValorizacionButtonClick = function () {
	gvNovedadesValorizacion.PerformCallback();
};

// Procesamiento de saldos
var onRecargarSaldosValorizacionButtonClick = function () {
	gvSaldosValorizacion.PerformCallback();
};
var onQuerySaldosValorizacionDetailsBeginCallback = function (s, e) {
	var id = parseInt($("#id_productValuation").val());
	e.customArgs["idProductValuation"] = id;
};

// Procesamiento de valoración
var GetSelectedWrehousesId = function () {
	var controlsCollection = ASPxClientControl.GetControlCollection();
	var valuationWarehouseGridView = controlsCollection.GetByName("gvProductValuationWarehouse");
	if (valuationWarehouseGridView === null) {
		return [];
	}
	var detallesBodegaInfo = valuationWarehouseGridView.cpDetallesBodegas;
	if (detallesBodegaInfo === null || detallesBodegaInfo.length === 0) {
		return [];
	}
	var result = [];
	var numItems = detallesBodegaInfo.length;
	for (var i = 0; i < numItems; i++) {
		var checkBox = controlsCollection.GetByName("UsarBodegaValorizacionEditCheckBox_" + i);
		if (checkBox !== null && checkBox.GetChecked()) {
			result = result.concat(detallesBodegaInfo[i].id_warehouse);
		}
	}
	return result;
};

var onCalcularValoracionButtonClick = function () {
	if (IsValidEditForm()) {
		var fechaInicioDate = fechaInicio.GetDate();
		var fechaInicioVal = fechaInicioDate != null
			? fechaInicioDate.toISOString().substring(0, 10)
			: null;

		var fechaFinDate = fechaFin.GetDate();
		var ffechaFinVal = fechaFinDate != null
			? fechaFinDate.toISOString().substring(0, 10)
			: null;

		var operationData = {
			idProductValuation: parseInt($("#id_productValuation").val()),
			fechaInicio: fechaInicioVal,
			fechaFin: ffechaFinVal,
			idProducto: producto.GetValue(),
			warehouseIds: GetSelectedWrehousesId()
		};
		var hideLoadingDialog = true;
		$.ajax({
			url: "CostProductValuation/Valorize",
			type: "post",
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: JSON.stringify(operationData),
			async: true,
			cache: false,
			error: function (error) {
				console.error(error);
				hideLoading();
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result.isValid) {
					hideLoadingDialog = false;
					
					// Procesamos las novedades
					gvNovedadesValorizacion.PerformCallback();
					gvMovimientoValorizacion.PerformCallback();
					gvSaldosValorizacion.PerformCallback();
					ResumenCallbackPanel.PerformCallback();

					var url = getFullPath('CostProductValuation/GenerateExcelDynamic');
					$('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");

					hideLoading();
				} else {
					ShowEditMessage(result.message);
				}
			},
			complete: function () {
				if (hideLoadingDialog) {
					hideLoading();
				}
			}
		});
	}
};

var onQueryMovimientosValorizacionDetailsBeginCallback = function (s, e) {
	var id = parseInt($("#id_productValuation").val());
	e.customArgs["idProductValuation"] = id;
};
var onQueryResumenCallbackBeginCallback = function (s, e) {
	var id = parseInt($("#id_productValuation").val());
	e.customArgs["idProductValuation"] = id;
};
var onRecargarMovimientosValorizacionButtonClick = function () {
	gvMovimientoValorizacion.PerformCallback();
};

