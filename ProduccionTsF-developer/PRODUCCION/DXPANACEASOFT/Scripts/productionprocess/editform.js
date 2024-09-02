
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvProductionProcess.UpdateEdit();
    }
}

function WarehouseCombo_SelectedIndexChanged(s, e) {
	id_WarehouseLocation.SetValue(null);
	id_WarehouseLocation.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "ProductionProcess/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: data },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				//showLoading();
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					var arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(id_WarehouseLocation, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}

function OnWarehouseLocationEntryInit(s, e) {
	//// 
	var idwarehouse = id_warehouse.GetValue();
	if (idwarehouse === null) {
		id_WarehouseLocation.ClearItems();
	} else {
		$.ajax({
			url: "ProductionProcess/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: id_warehouse.GetValue() },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					var arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(id_WarehouseLocation, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function SubCostCenterCombo_SelectedIndexChanged(s, e) {
	id_SubCostCenter.SetValue(null);
	id_SubCostCenter.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "ProductionProcess/CostCenterChangeData",
			type: "post",
			data: { id_costCenter: data },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				//showLoading();
			},
			success: function (result) {
				for (var i = 0; i < result.length; i++) {
					var subCostCenter = result[i];
					id_SubCostCenter.AddItem(subCostCenter.name, subCostCenter.id);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}

function OnSubCostCenterEntryInit(s, e) {
	// 
	var idCostCenter = id_CostCenter.GetValue();
	if (idCostCenter === null) {
		id_SubCostCenter.ClearItems();
		id_SubCostCenter.SetValue(null);
	} else {
		var id = $("#idSubCenter").val();
		$.ajax({
			url: "ProductionProcess/CostCenterChangeData",
			type: "post",
			data: { id_costCenter: idCostCenter },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				var index;
				id_SubCostCenter.ClearItems();
				if (result !== null && result !== undefined) {
					for (var i = 0; i < result.length; i++) {
						var warehouse = result[i];
						if (id === warehouse.id.toString()) {
							index = i;
						}
						id_SubCostCenter.AddItem(warehouse.name, warehouse.id);
					}
					id_SubCostCenter.SetSelectedIndex(index);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function ButtonCancel_Click(s, e) {
    if (gvProductionProcess !== null && gvProductionProcess !== undefined) {
        gvProductionProcess.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}

