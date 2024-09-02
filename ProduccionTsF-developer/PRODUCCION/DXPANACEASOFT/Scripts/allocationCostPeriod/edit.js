
// Métodos generales

var UpdateTabValidationImage = function(tabIndex, isValid) {
	var imageUrl = isValid
		? "/Content/image/noimage.png"
		: "/Content/image/info-error.png";

	var tab = tpAllocationCostPeriodEdit.GetTab(tabIndex);
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

var OnEmissionDateProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnAccountingValueProductionCostCheckedChanged = function(s, e) {
	var idExecutionType = id_executionType.GetValue();
	var useAccountingValue = accountingValue.GetChecked();
	refreshCoefficientProductionCost(idExecutionType, useAccountingValue);

	ObtenerCostosEditButton.SetEnabled(useAccountingValue);
	gvAllocationProductionCostDetail.PerformCallback();
};

var OnExecutionTypeProductionCostSelectedIndexChanged = function(s, e) {
	var idExecutionType = id_executionType.GetValue();
	var useAccountingValue = accountingValue.GetChecked();
	refreshCoefficientProductionCost(idExecutionType, useAccountingValue);

	gvAllocationProductionCostDetail.PerformCallback();
};
var OnExecutionTypeProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnCoefficientProductionCostSelectedIndexChanged = function(s, e) {
	gvAllocationProductionCostDetail.PerformCallback();
};
var OnCoefficientProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};
var refreshCoefficientProductionCost = function(idExecutionType, useAccountingValue) {
	id_coefficient.ClearItems();
	var operationData = {
		idExecutionType: idExecutionType,
		accountingValue: useAccountingValue
	};
	$.ajax({
		url: "AllocationCostPeriod/QueryProductionCoefficients",
		type: "post",
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		data: JSON.stringify(operationData),
		async: true,
		cache: false,
		error: function(error) {
			console.error(error);
		},
		success: function(result) {
			if (result.isValid && result.items && result.items.length > 0) {
				var numItems = result.items.length;
				for (var i = 0; i < numItems; i++) {
					var item = result.items[ i ];
					id_coefficient.AddItem([ item.id, item.sequence ], item.id);
				}
			}
		}
	});
};

var OnAnioProductionCostSelectedIndexChanged = function(s, e) {
	mes.Validate();
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

var onObtenerValoresCostosButtonClick = function(s, e) {
	if (accountingValue.GetChecked()) {
		mes.Validate();
		if (!mes.GetIsValid()) {
			ShowEditMessage("Se necesita un período para ejecutar esta acción.");
			anio.Focus();
			return;
		}

		id_executionType.Validate();
		if (!id_executionType.GetIsValid()) {
			ShowEditMessage("Se necesita un tipo de ejecución para ejecutar esta acción.");
			id_executionType.Focus();
			return;
		}

		id_coefficient.Validate();
		if (!id_coefficient.GetIsValid()) {
			ShowEditMessage("Se necesita un número de operación para ejecutar esta acción.");
			id_coefficient.Focus();
			return;
		}

		var operationData = {
			idAllocationCostPeriod: $("#id_allocationCostPeriod").val(),
			accountingValue: true,
			idExecutionType: id_executionType.GetValue(),
			idCoefficient: id_coefficient.GetValue(),
			anio: anio.GetValue(),
			mes: mes.GetValue()
		};
		$.ajax({
			url: "AllocationCostPeriod/QueryAccountValue",
			type: "post",
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: JSON.stringify(operationData),
			async: true,
			cache: false,
			error: function(error) {
				console.error(error);
			},
			success: function(result) {
				if (result.isValid) {
					ShowEditMessage("Costo actualizado exitosamente.");
					gvAllocationProductionCostDetail.PerformCallback();
				} else {
					ShowEditMessage("No se pudo actualizar el valor del costo.");
				}
			}
		});
	} else {
		ObtenerCostosEditButton.SetEnabled(false);
	}
};


// Botones de edición de la cuadrícula

var AllocationProductionCostDetails_OnBeginCallback = function(s, e) {
	e.customArgs[ 'idAllocationCostPeriod' ] = $("#id_allocationCostPeriod").val();
	e.customArgs[ 'accountingValue' ] = accountingValue.GetChecked();
	e.customArgs[ 'idExecutionType' ] = id_executionType.GetValue();
	e.customArgs[ 'idCoefficient' ] = id_coefficient.GetValue();
	e.customArgs[ 'editable' ] = gvAllocationProductionCostDetail.cpPuedeEditarse;
	e.customArgs[ 'id' ] = $("#idAllocationCostPeriodDetail").val();
};

var OnProductionCostSelectedIndexChanged = function(s, e) {
	var idProductionCost = s.GetValue();

	id_productionCostDetail.ClearItems();

	if (idProductionCost !== null) {
		var productionCostDetails = id_productionCostDetail.cpProductionCostDetails;
		var numProductionCostDetails = productionCostDetails.length;
		for (var i = 0; i < numProductionCostDetails; i++) {
			var productionCostDetail = productionCostDetails[ i ];
			if (productionCostDetail.id_productionCost === idProductionCost) {
				id_productionCostDetail.AddItem(productionCostDetail.name, productionCostDetail.id);
			}
		}
	}
};
var OnProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnProductionCostDetailValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnProductionPlantDetailValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnAllProductionPlantsCheckedChanged = function(s, e) {
	if (s.GetChecked()) {
		id_productionPlant.SetEnabled(false);
	} else {
		id_productionPlant.SetEnabled(true);
	}
};

var OnValorDetailValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else if (e.value <= 0) {
		e.isValid = false;
		e.errorText = "Valor debe ser mayor a cero";
	}
};


// Botones de edición de detalles

var OnUpdateDetailButtonClick = function(s, e) {
	var isValid = ASPxClientEdit.ValidateEditorsInContainerById("allocationCostPeriodTable");
	if (isValid) {
		gvAllocationProductionCostDetail.UpdateEdit();
	}
};

var OnCancelDetailButtonClick = function(s, e) {
	gvAllocationProductionCostDetail.CancelEdit();
};


// Acciones del formulario

var IsValidEditForm = function() {
	var isGeneralesValid = ASPxClientEdit.ValidateEditorsInContainerById("datosGeneralesEditForm", "", true);
	if (isGeneralesValid) {
		UpdateTabValidationImage(0, true);
	} else {
		UpdateTabValidationImage(0, false);
		tpAllocationCostPeriodEdit.SetActiveTabIndex(0);
		return false;
	}

	return true;
};

var PerformDocumentStoreAction = function(approveDocument) {
	if (IsValidEditForm()) {

		var idAllocationCostPeriod = parseInt($("#id_allocationCostPeriod").val());
		var actionUrl = (idAllocationCostPeriod === 0) ? "AllocationCostPeriod/Create" : "AllocationCostPeriod/Save";

		var operationData = {
			idAllocationCostPeriod: idAllocationCostPeriod,
			emissionDate: emissionDate.GetDate(),
			anio: anio.GetValue(),
			mes: mes.GetValue(),
			accountingValue: accountingValue.GetChecked(),
			idExecutionType: id_executionType.GetValue(),
			idCoefficient: id_coefficient.GetValue(),
			description: description.GetText(),
			reference: reference.GetText(),
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
						id: result.idAllocationCostPeriod,
						successMessage: result.message
					};
					showPage("AllocationCostPeriod/EditForm", successData);
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
	var idAllocationCostPeriod = parseInt($("#id_allocationCostPeriod").val());
	var actionUrl = "AllocationCostPeriod/" + action;

	var operationData = {
		idAllocationCostPeriod: idAllocationCostPeriod
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
					id: result.idAllocationCostPeriod,
					successMessage: result.message
				};
				showPage("AllocationCostPeriod/EditForm", successData);
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
	var id = parseInt($("#id_allocationCostPeriod").val());
	showPage("AllocationCostPeriod/EditForm", { id: id });
};

var OnCancelButtonClick = function(id) {
	showPage("AllocationCostPeriod/Index", null);
};
