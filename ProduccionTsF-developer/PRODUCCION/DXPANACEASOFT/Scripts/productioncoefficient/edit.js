
// Métodos generales

var UpdateTabValidationImage = function(tabIndex, isValid) {
	var imageUrl = isValid
		? "/Content/image/noimage.png"
		: "/Content/image/info-error.png";

	var tab = tpProductionCoefficientEdit.GetTab(tabIndex);
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

var OnExecutionTypeProductionCostSelectedIndexChanged = function(s, e) {
	var idExecutionType = s.GetValue();

	id_productionCost.ClearItems();
	id_productionCostDetail.ClearItems();

	if (idExecutionType !== null) {
		var productionCosts = id_productionCost.cpProductionCosts;
		var numProductionCosts = productionCosts.length;
		for (var i = 0; i < numProductionCosts; i++) {
			var productionCost = productionCosts[ i ];
			if (productionCost.id_executionType === idExecutionType) {
				id_productionCost.AddItem(productionCost.name, productionCost.id);
			}
		}
	}
};
var OnExecutionTypeProductionCostValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnPlanDeCuentasSelectedIndexChanged = function(s, e) {
	if (tpProductionCoefficientEdit.cpUsarIntegracionContable) {
		gvProductionCoefficientDetail.PerformCallback();
	}
};
var OnPlanDeCuentasValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnWarehouseTypeSelectedIndexChanged = function(s, e) {
	var idWarehouseType = s.GetValue();

	id_warehouses.ClearItems();
	id_warehouseLocations.ClearItems();
	id_warehouseLocations.SetEnabled(false);

	var warehouses = id_warehouses.cpWarehouses;
	var numWarehouses = warehouses.length;
	for (var i = 0; i < numWarehouses; i++) {
		var warehouse = warehouses[ i ];
		if ((idWarehouseType === null)
			|| (warehouse.id_warehouseType === idWarehouseType)) {
			id_warehouses.AddItem(warehouse.name, warehouse.id);
		}
	}

	id_warehouses.Validate();
	id_warehouseLocations.Validate();
};

var OnWarehousesValueChanged = function(s, e) {
	var idWarehouses = s.GetValue();

	id_warehouseLocations.ClearItems();
	id_warehouseLocations.SetEnabled(false);

	if (idWarehouses !== null && idWarehouses.length > 0) {
		var idWarehousesArray = idWarehouses.split('|');
		if (idWarehousesArray.length === 1) {
			var idWarehouse = parseInt(idWarehousesArray[ 0 ]);
			var warehouseLocations = id_warehouseLocations.cpWarehouseLocations;
			var numWarehouseLocations = warehouseLocations.length;
			id_warehouseLocations.SetEnabled(true);
			for (var i = 0; i < numWarehouseLocations; i++) {
				var warehouseLocation = warehouseLocations[ i ];
				if (warehouseLocation.id_warehouse === idWarehouse) {
					id_warehouseLocations.AddItem(warehouseLocation.name, warehouseLocation.id);
				}
			}
		}
	}
};
var OnWarehousesValidation = function(s, e) {
	var idWarehouseType = id_warehouseType.GetValue();
	if (idWarehouseType === null || idWarehouseType.length === 0) {
		if (e.value === null || e.value.length === 0) {
			e.isValid = false;
			e.errorText = "Campo Obligatorio";
		}
	}
};

var OnSimpleFormulaSelectedIndexChanged = function(s, e) {
	var idSimpleFormula = s.GetValue();

	var formulaText = "";

	if (idSimpleFormula !== null) {
		var simpleFormulas = id_simpleFormula.cpSimpleFormulas;
		var numSimpleFormulas = simpleFormulas.length;
		for (var i = 0; i < numSimpleFormulas; i++) {
			var simpleFormula = simpleFormulas[ i ];
			if (simpleFormula.id === idSimpleFormula) {
				formulaText = simpleFormula.formula;
				break;
			}
		}
	}

	$("#formulacionTextField").val(formulaText);
};
var OnSimpleFormulaValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnPoundTypeValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnProductionCostSelectedIndexChanged = function (s, e) {
	gvProductionCoefficientDetail.PerformCallback();

	// Actualización de selector
	var idProductionCost = s.GetValue();
	id_productionCostDetail.ClearItems();

	if (idProductionCost !== null) {
		var productionCostDetails = id_productionCostDetail.cpProductionCostDetails;
		var numProductionCostDetails = productionCostDetails.length;
		for (var i = 0; i < numProductionCostDetails; i++) {
			var productionCostDetail = productionCostDetails[i];
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

var ProductionCoefficientDetails_OnBeginCallback = function (s, e) {
	e.customArgs['idProductionCoefficient'] = $("#id_productionCoefficient").val();
	e.customArgs['idPlanDeCuentas'] = id_planDeCuentas.GetValue();
	e.customArgs['id'] = $("#idCoefficientDetail").val();
};

var OnCuentaContableDetailSelectedIndexChanged = function(s, e) {
	var idPlanDeCuentas = id_planDeCuentas.GetValue();
	var idCuentaContable = s.GetValue();
	var cuentasContables = s.cpCuentasContables;
	var aceptaAuxiliar = false;
	var aceptaCentroCosto = false;

	if (cuentasContables && cuentasContables.length > 0) {
		var numCuentasContables = cuentasContables.length;
		for (var i = 0; i < numCuentasContables; i++) {
			var cuentaContable = cuentasContables[ i ];
			if (cuentaContable.idCuentaContable === idCuentaContable) {
				aceptaAuxiliar = cuentaContable.aceptaAuxiliar;
				aceptaCentroCosto = cuentaContable.aceptaCentroCosto;
				break;
			}
		}
	}

	id_tipoAuxContab.SetEnabled(aceptaAuxiliar);
	id_auxiliarContab.SetEnabled(aceptaAuxiliar);

	id_tipoPresContab.SetEnabled(aceptaCentroCosto);
	id_centroCtoContab.SetEnabled(aceptaCentroCosto);
	id_subcentroCtoContab.SetEnabled(aceptaCentroCosto);

	refreshTipoAuxiliarContableDetail(idPlanDeCuentas, idCuentaContable);

	if (aceptaCentroCosto) {
		id_tipoPresContab.SetSelectedIndex(0);
	} else {
		id_tipoPresContab.SetSelectedIndex(-1);
	}

	refreshCentroCostoContableDetail(id_tipoPresContab.GetValue());
};
var OnCuentaContableDetailValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnTipoAuxiliarContableDetailSelectedIndexChanged = function(s, e) {
	var idTipoAuxiliarContable = s.GetValue();
	refreshAuxiliarContableDetail(idTipoAuxiliarContable);
};
var OnTipoAuxiliarContableDetailValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};
var refreshTipoAuxiliarContableDetail = function(idPlanDeCuentas, idCuentaContable) {
	id_tipoAuxContab.ClearItems();
	id_auxiliarContab.ClearItems();
	var operationData = {
		idPlanDeCuentas: idPlanDeCuentas,
		idCuentaContable: idCuentaContable
	};
	$.ajax({
		url: "ProductionCoefficient/QueryTiposAuxiliarContables",
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
					id_tipoAuxContab.AddItem(
						[ item.idTipoAuxContable, item.tipoAuxContable ],
						item.idTipoAuxContable);
				}
			}
		}
	});
};

var OnAuxiliarContableDetailValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};
var refreshAuxiliarContableDetail = function(idTipoAuxContable) {
	id_auxiliarContab.ClearItems();
	var operationData = {
		idTipoAuxContable: idTipoAuxContable
	};
	$.ajax({
		url: "ProductionCoefficient/QueryAuxiliaresContables",
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
					id_auxiliarContab.AddItem(
						[ item.idAuxContable, item.auxContable ],
						item.idAuxContable);
				}
			}
		}
	});
};

var OnTipoPresupuestoDetailSelectedIndexChanged = function(s, e) {
	var idTipoPresupuesto = s.GetValue();
	refreshCentroCostoContableDetail(idTipoPresupuesto);
};
var OnTipoPresupuestoDetailValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnCentroCostoContableDetailSelectedIndexChanged = function(s, e) {
	var idTipoPresupuesto = id_tipoPresContab.GetValue();
	var idCentroCosto = s.GetValue();
	refreshSubcentroCostoContableDetail(idTipoPresupuesto, idCentroCosto);
};
var OnCentroCostoContableDetailValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};
var refreshCentroCostoContableDetail = function(idTipoPresupuesto) {
	id_centroCtoContab.ClearItems();
	id_subcentroCtoContab.ClearItems();
	var operationData = {
		idTipoPresupuesto: idTipoPresupuesto
	};
	$.ajax({
		url: "ProductionCoefficient/QueryCentrosCosto",
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
					id_centroCtoContab.AddItem(
						[ item.idCentroCosto, item.centroCosto ],
						item.idCentroCosto);
				}
			}
		}
	});
};

var OnSubcentroCostoContableDetailValidation = function(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};
var refreshSubcentroCostoContableDetail = function(idTipoPresupuesto, idCentroCosto) {
	id_subcentroCtoContab.ClearItems();
	var operationData = {
		idTipoPresupuesto: idTipoPresupuesto,
		idCentroCosto: idCentroCosto
	};
	$.ajax({
		url: "ProductionCoefficient/QuerySubcentrosCosto",
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
					id_subcentroCtoContab.AddItem(
						[ item.idSubcentroCosto, item.subcentroCosto ],
						item.idSubcentroCosto);
				}
			}
		}
	});
};

// Botones de edición

var OnUpdateDetailButtonClick = function(s, e) {
	var isValid = ASPxClientEdit.ValidateEditorsInContainerById("productionCoefficientTable");
	if (isValid) {
		gvProductionCoefficientDetail.UpdateEdit();
	}
};

var OnCancelDetailButtonClick = function(s, e) {
	gvProductionCoefficientDetail.CancelEdit();
};


// Acciones del formulario

var IsValidEditForm = function() {
	var isGeneralesValid = ASPxClientEdit.ValidateEditorsInContainerById("datosGeneralesEditForm", "", true);
	if (isGeneralesValid) {
		UpdateTabValidationImage(0, true);
	} else {
		UpdateTabValidationImage(0, false);
		tpProductionCoefficientEdit.SetActiveTabIndex(0);
		return false;
	}

	var isOperacionesValid = ASPxClientEdit.ValidateEditorsInContainerById("operacionesCoeficienteEditForm", "", true);
	if (isOperacionesValid) {
		UpdateTabValidationImage(1, true);
	} else {
		UpdateTabValidationImage(1, false);
		tpProductionCoefficientEdit.SetActiveTabIndex(1);
		return false;
	}

	return true;
};

var OnUpdateButtonClick = function() {
	if (IsValidEditForm()) {

		var idProductionCoefficient = parseInt($("#id_productionCoefficient").val());
		var actionUrl = (idProductionCoefficient === 0) ? "ProductionCoefficient/Create" : "ProductionCoefficient/Update";

		var idWarehouses = id_warehouses.GetValue();
		var idWarehousesArray = (idWarehouses !== null && idWarehouses.length > 0) ? idWarehouses.split('|') : [];

		var idWarehouseLocations = id_warehouseLocations.GetValue();
		var idWarehouseLocationsArray = (idWarehouseLocations !== null && idWarehouseLocations.length > 0) ? idWarehouseLocations.split('|') : [];

		var operationData = {
			idProductionCoefficient: idProductionCoefficient,
			idExecutionType: id_executionType.GetValue(),
			idPlanDeCuentas: id_planDeCuentas.GetValue(),
			idWarehouseType: id_warehouseType.GetValue(),
			idWarehouses: idWarehousesArray,
			idWarehouseLocations: idWarehouseLocationsArray,
			idPoundType: id_poundType.GetValue(),
			idSimpleFormula: id_simpleFormula.GetValue(),
			idProductionCost: id_productionCost.GetValue(),
			idProductionCostDetail: id_productionCostDetail.GetValue(),
			idProductionPlant: allProductionPlants.GetChecked() ? null : id_productionPlant.GetValue(),
			description: description.GetText(),
			isActive: isActive.GetChecked()
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
						id: result.idProductionCoefficient,
						successMessage: result.message
					};
					showPage("ProductionCoefficient/EditForm", successData);
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

var OnRefreshButtonClick = function() {
	var id = $('#id_productionCoefficient').val();
	showPage("ProductionCoefficient/EditForm", { id: id });
};

var OnCancelButtonClick = function(id) {
	showPage("ProductionCoefficient/Index", null);
};
