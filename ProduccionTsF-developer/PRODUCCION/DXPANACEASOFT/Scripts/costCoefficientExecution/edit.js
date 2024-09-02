
// Métodos generales

var UpdateTabValidationImage = function(tabIndex, isValid) {
	var imageUrl = isValid
		? "/Content/image/noimage.png"
		: "/Content/image/info-error.png";

	var tab = tpCoefficientExecutionEdit.GetTab(tabIndex);
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

var OnAnioProductionCostInit = function(s, e) {
	var periodosUsables = anio.cpPeriodosUsables;
	var numPeriodos = periodosUsables.length;
	for (var i = 0; i < numPeriodos; i++) {
		var periodo = periodosUsables[ i ];
		periodo.FechaInicio = new Date(periodo.FechaInicio / 10000, periodo.FechaInicio / 100 % 100 - 1, periodo.FechaInicio % 100);
		periodo.FechaFinal = new Date(periodo.FechaFinal / 10000, periodo.FechaFinal / 100 % 100 - 1, periodo.FechaFinal % 100);
	}
};
var OnAnioProductionCostSelectedIndexChanged = function(s, e) {
	mes.ClearItems();
	startDate.SetDate(null);
	startDate.SetEnabled(false);
	endDate.SetDate(null);
	endDate.SetEnabled(false);

	var anioValor = anio.GetValue();
	if (anioValor !== null) {
		var periodosUsables = anio.cpPeriodosUsables;
		var numPeriodos = periodosUsables.length;
		for (var i = 0; i < numPeriodos; i++) {
			var periodo = periodosUsables[ i ];
			if (periodo.Anio === anioValor) {
				mes.AddItem([ periodo.Mes, periodo.Mes ], periodo.Mes);
			}
		}
	}

	mes.Validate();
	startDate.Validate();
	endDate.Validate();
};

var OnMesProductionCostSelectedIndexChanged = function(s, e) {
	refreshCamposFecha();

	mes.Validate();
	startDate.Validate();
	endDate.Validate();
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

var OnStartDateProductionCostInit = function(s, e) {
	var anioValor = anio.GetValue();
	var mesValor = mes.GetValue();
	if (anioValor !== null && mesValor !== null) {
		var periodosUsables = anio.cpPeriodosUsables;
		var numPeriodos = periodosUsables.length;
		for (var i = 0; i < numPeriodos; i++) {
			var periodo = periodosUsables[ i ];
			if (periodo.Anio === anioValor && periodo.Mes === mesValor) {
				startDate.SetMinDate(periodo.FechaInicio);
				startDate.SetMaxDate(periodo.FechaFinal);
				break;
			}
		}
	}
};
var OnStartDateProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnEndDateProductionCostInit = function(s, e) {
	var anioValor = anio.GetValue();
	var mesValor = mes.GetValue();
	if (anioValor !== null && mesValor !== null) {
		var periodosUsables = anio.cpPeriodosUsables;
		var numPeriodos = periodosUsables.length;
		for (var i = 0; i < numPeriodos; i++) {
			var periodo = periodosUsables[ i ];
			if (periodo.Anio === anioValor && periodo.Mes === mesValor) {
				endDate.SetMinDate(periodo.FechaInicio);
				endDate.SetMaxDate(periodo.FechaFinal);
				break;
			}
		}
	}
};
var OnEndDateProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnAllocationTypeProductionCostSelectedIndexChanged = function(s, e) {
	refreshCamposFecha();

	startDate.Validate();
	endDate.Validate();
};
var OnAllocationTypeProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var refreshCamposFecha = function() {
	startDate.SetDate(null);
	startDate.SetEnabled(false);

	endDate.SetDate(null);
	endDate.SetEnabled(false);

	var anioValor = anio.GetValue();
	var mesValor = mes.GetValue();
	if (anioValor !== null && mesValor !== null) {
		var periodosUsables = anio.cpPeriodosUsables;
		var numPeriodos = periodosUsables.length;
		for (var i = 0; i < numPeriodos; i++) {
			var periodo = periodosUsables[ i ];
			if (periodo.Anio === anioValor && periodo.Mes === mesValor) {
				startDate.SetMinDate(periodo.FechaInicio);
				startDate.SetMaxDate(periodo.FechaFinal);
				startDate.SetDate(periodo.FechaInicio);

				endDate.SetMinDate(periodo.FechaInicio);
				endDate.SetMaxDate(periodo.FechaFinal);
				endDate.SetDate(periodo.FechaFinal);

				if (id_allocationType.GetValue() === id_allocationType.cpIdTipoAsignacionCostoProyectado) {
					startDate.SetEnabled(true);
					endDate.SetEnabled(true);
				}

				break;
			}
		}
	}
};

var onEjecutarCoeficientesButtonClick = function(s, e) {
	if (IsValidEditForm()) {
		var operationData = {
			idCoefficientExecution: parseInt($("#id_coefficientExecution").val()),
			anio: anio.GetValue(),
			mes: mes.GetValue(),
			startDate: startDate.GetDate(),
			endDate: endDate.GetDate(),
			idAllocationType: id_allocationType.GetValue()
		};
		var hideLoadingDialog = true;

		$.ajax({
			url: "CostCoefficientExecution/Execute",
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
						idCoefficientExecution: result.idCoefficientExecution
					};

					$.ajax({
						url: "CostCoefficientExecution/CostCoefficientExecutionDetails",
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
							startDate.SetEnabled(false);
							endDate.SetEnabled(false);
							id_allocationType.SetEnabled(false);
							EjecutarCoeficientesEditButton.SetVisible(false);
							RestablecerCoeficientesEditButton.SetVisible(true);
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
var onRestablecerCoeficientesButtonClick = function(s, e) {
	if (IsValidEditForm()) {
		var operationData = {
			idCoefficientExecution: parseInt($("#id_coefficientExecution").val())
		};

		$.ajax({
			url: "CostCoefficientExecution/Reset",
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
					if (id_allocationType.GetValue() === id_allocationType.cpIdTipoAsignacionCostoProyectado) {
						startDate.SetEnabled(true);
						endDate.SetEnabled(true);
					}
					EjecutarCoeficientesEditButton.SetVisible(true);
					RestablecerCoeficientesEditButton.SetVisible(false);

					$("#detallesEjecucion").empty();
					$("#detallesCostoCoeficientes").empty();
					tpCoefficientExecutionEdit.SetTabEnabled(1, false);
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
var onCalcularDistribucionButtonClick = function(s, e) {
	if (IsValidEditForm()) {
		var operationData = {
			idCoefficientExecution: parseInt($("#id_coefficientExecution").val()),
			detailsIds: GetSelectedDetailsId()
		};
		var hideLoadingDialog = true;

		$.ajax({
			url: "CostCoefficientExecution/Distribute",
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
						idCoefficientExecution: result.idCoefficientExecution
					};

					$.ajax({
						url: "CostCoefficientExecution/CostCoefficientExecutionDistributionDetails",
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
							$("#detallesDistribucion").html(result);
							cargarDetallesCostosCoeficientes(viewData.idCoefficientExecution);
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

var cargarDetallesCostosCoeficientes = function(idCoefficientExecution) {
	var viewData = {
		idCoefficientExecution: idCoefficientExecution
	};

	$.ajax({
		url: "CostCoefficientExecution/CostCoefficientExecutionWarehouseDetails",
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
			$("#detallesCostoCoeficientes").html(result);
			tpCoefficientExecutionEdit.SetTabEnabled(1, true);
		},
		complete: function() {
			hideLoading();
		}
	});
};

// Botones de edición de la cuadrícula

var onUsarDetalleGrupoCostoEditCheckedChanged = function(s, e) {
	if (s.GetChecked()) {
		$("#detalleSubCosto-" + s.cpIndice).show();
	} else {
		$("#detalleSubCosto-" + s.cpIndice).hide();
	}
	$("#detallesDistribucion").empty();
	$("#detallesCostoCoeficientes").empty();
	tpCoefficientExecutionEdit.SetTabEnabled(1, false);
};


// Acciones del formulario

var IsValidEditForm = function() {
	var isGeneralesValid = ASPxClientEdit.ValidateEditorsInContainerById("datosGeneralesEditForm", "", true);
	if (isGeneralesValid) {
		UpdateTabValidationImage(0, true);
	} else {
		UpdateTabValidationImage(0, false);
		tpCoefficientExecutionEdit.SetActiveTabIndex(0);
		return false;
	}

	return true;
};
var GetSelectedDetailsId = function() {
	var controlsCollection = ASPxClientControl.GetControlCollection();
	var costGroupGridView = controlsCollection.GetByName("gvCostCoefficientExecutionCostGroup");
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

		var idCoefficientExecution = parseInt($("#id_coefficientExecution").val());
		var actionUrl = (idCoefficientExecution === 0) ? "CostCoefficientExecution/Create" : "CostCoefficientExecution/Save";

		var operationData = {
			idCoefficientExecution: idCoefficientExecution,
			anio: anio.GetValue(),
			mes: mes.GetValue(),
			startDate: startDate.GetDate(),
			endDate: endDate.GetDate(),
			idAllocationType: id_allocationType.GetValue(),
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
						id: result.idCoefficientExecution,
						successMessage: result.message
					};
					showPage("CostCoefficientExecution/EditForm", successData);
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
	var idCoefficientExecution = parseInt($("#id_coefficientExecution").val());
	var actionUrl = "CostCoefficientExecution/" + action;

	var operationData = {
		idCoefficientExecution: idCoefficientExecution
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
					id: result.idCoefficientExecution,
					successMessage: result.message
				};
				showPage("CostCoefficientExecution/EditForm", successData);
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
	var id = parseInt($("#id_coefficientExecution").val());
	showPage("CostCoefficientExecution/EditForm", { id: id });
};

var OnCancelButtonClick = function(id) {
	showPage("CostCoefficientExecution/Index", null);
};
