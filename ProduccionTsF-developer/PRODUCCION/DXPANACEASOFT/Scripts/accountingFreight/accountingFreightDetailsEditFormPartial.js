function OnGridViewInit(s, e) {
	UpdateTitlePanelDetails();
}

function OnGridViewSelectionChanged(s, e) {
	UpdateTitlePanelDetails();
	s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbacksDetail);
}

function OnGridViewBeginCallback(s, e) {
	e.customArgs['id_accountingFreight'] = $("#id_accountingFreight").val();
}

function OnGridViewEndCallback(s, e) {


}

function UpdateTitlePanelDetails() {
	btnRemoveDetail.SetEnabled(false);
	btnRefreshDetails.SetEnabled(false);
}

function AddNewDetail(s, e) {
	gridViewMoveDetails.AddNewRow();
}

function OnInitCostCenterCombo(s, e) {
	if (id_costCenterDetail.GetValue() === null || id_costCenterDetail.GetValue() === "" || id_costCenterDetail.GetValue() === 0) {
		id_costCenterDetail.SetValue(id_costCenterDetail.GetValue());
		id_subCostCenterDetail.SetValue(id_subCostCenterDetail.GetValue());
	}
	id_costCenterDetail.SetEnabled(false);
	id_subCostCenterDetail.SetEnabled(false);
	id_subCostCenterDetailInit = id_subCostCenterDetail.GetValue();
	id_subCostCenterDetail.PerformCallback();
}

function OnCostCenterCombo_SelectedIndexChanged(s, e) {
	id_subCostCenterDetailInit = null;
	id_subCostCenterDetail.PerformCallback();
}

function InventoryMoveSubCostCenter_BeginCallback(s, e) {
	e.customArgs["id_costCenterDetail"] = id_costCenterDetail.GetValue();
	e.customArgs["id_subCostCenterDetail"] = id_subCostCenterDetailInit;
}

function InventoryMoveSubCostCenter_EndCallback(s, e) {
	id_subCostCenterDetail.SetValue(id_subCostCenterDetailInit);
}

var OnCuentaContableDetailSelectedIndexChanged = function (s, e) {

	var accountingAccountCode = s.GetValue();
	var cuentasContables = s.cpCuentasContables;
	var aceptaAuxiliar = false;
	var aceptaCentroCosto = false;

	if (cuentasContables && cuentasContables.length > 0) {
		var numCuentasContables = cuentasContables.length;
		for (var i = 0; i < numCuentasContables; i++) {
			var cuentaContable = cuentasContables[i];
			if (cuentaContable.idCuentaContable === accountingAccountCode) {
				aceptaAuxiliar = cuentaContable.aceptaAuxiliar;
				aceptaCentroCosto = cuentaContable.aceptaCentroCosto;
				break;
			}
		}
	}

	id_auxiliarContab.SetEnabled(aceptaAuxiliar);
	isAuxiliarCheck.SetValue(aceptaAuxiliar);
	id_costCenterDetail.SetEnabled(aceptaCentroCosto);
	id_subCostCenterDetail.SetEnabled(aceptaCentroCosto);

	refreshTipoAuxiliarContableDetail(accountingAccountCode);

};

var refreshTipoAuxiliarContableDetail = function (idCuentaContable) {
	id_tipoAuxContab.ClearItems();
	id_auxiliarContab.ClearItems();
	var operationData = {
		idCuentaContable: idCuentaContable
	};
	$.ajax({
		url: "AccountingFreight/QueryTiposAuxiliarContables",
		type: "post",
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		data: JSON.stringify(operationData),
		async: true,
		cache: false,
		error: function (error) {
			console.error(error);
		},
		success: function (result) {
			if (result.isValid && result.items && result.items.length > 0) {
				var numItems = result.items.length;
				for (var i = 0; i < numItems; i++) {
					var item = result.items[i];
					id_tipoAuxContab.AddItem(
						[item.idTipoAuxContable, item.tipoAuxContable],
						item.idTipoAuxContable);
				}
			}
		}
	});
};

var OnTipoAuxiliarContableDetailSelectedIndexChanged = function (s, e) {

	var code_Auxiliar = s.GetValue();
	refreshAuxiliarContableDetail(code_Auxiliar);
};


var OnAuxiliarContableDetailSelectedIndexChanged = function (s, e) {
	var idAuxContable = s.GetValue();
}

var refreshAuxiliarContableDetail = function (idTipoAuxContable) {
	id_auxiliarContab.ClearItems();
	var operationData = {
		idTipoAuxContable: idTipoAuxContable
	};
	$.ajax({
		url: "AccountingFreight/QueryAuxiliaresContables",
		type: "post",
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		data: JSON.stringify(operationData),
		async: true,
		cache: false,
		error: function (error) {
			console.error(error);
		},
		success: function (result) {
			if (result.isValid && result.items && result.items.length > 0) {
				var numItems = result.items.length;
				for (var i = 0; i < numItems; i++) {
					var item = result.items[i];
					id_auxiliarContab.AddItem(
						[item.idAuxContable, item.auxContable],
						item.idAuxContable);
				}
			}
		}
	});
};

var OnAuxiliarContableDetailValidation = function (s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};