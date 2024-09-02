
//DIALOG BUTTONS ACTIONS
function Update(approve) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvMachineForProd.UpdateEdit();
    }

}

function TabControl_ActiveTabChanged(s, e) {
}

function ButtonUpdate_Click(s, e) {

    Update(false);
  
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
    //showPage("MachineForProd/Index", null);
    if (gvMachineForProd !== null && gvMachineForProd !== undefined) {
        gvMachineForProd.CancelEdit();
    }
}

//BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("MachineForProd/FormEditMachineForProd", data);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {

}

function ApproveDocument(s, e) {
  


}

function AutorizeDocument(s, e) {

}

function ProtectDocument(s, e) {

}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_MachineForProd").val()
        };
        showForm("MachineForProd/Cancel", data);
    }, "¿Desea anular el Estado?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_MachineForProd").val()
        };
        showForm("MachineForProd/Revert", data);
    }, "¿Desea Activar?");
}

function ShowDocumentHistory(s, e) {

}



function PrintDocument(s, e) {
   
}

// DETAILS BUTTONS ACTIONS

function RefreshDetail(s, e) {
    Refresh(s, e);
}


// TABS FUNCTIONS

var activeGridView = null;

function TabControl_Init(s, e) {

}


function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 2000);
    }
}

function UpdateView() {

}

function OnThirdMaterialWarehouseLocationValidation(s, e) {
	var idThirdwarehouse = id_materialthirdWarehouse.GetValue();
	var idThirdWarehouseLocation = id_materialthirdWarehouseLocation.GetValue();
	if (idThirdwarehouse !== null && idThirdWarehouseLocation === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function OnMaterialthirdSubCostCenterValidation(s, e) {
	var idMaterialthirdCostCenter = id_materialthirdCostCenter.GetValue();
	var idMaterialthirdSubCostCenter = id_materialthirdSubCostCenter.GetValue();
	if (idMaterialthirdCostCenter !== null && idMaterialthirdSubCostCenter === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function ThirdWarehouseMaterialCombo_SelectedIndexChanged(s, e) {
	id_materialthirdWarehouseLocation.SetValue(null);
	id_materialthirdWarehouseLocation.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "MachineForProd/WarehouseChangeData",
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
					UpdateDetailObjects(id_materialthirdWarehouseLocation, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}

function WarehouseTypeCombo_SelectedIndexChanged(s, e) {
	
	id_materialWarehouse.ClearItems();
	id_materialthirdWarehouse.ClearItems();
	id_materialWarehouseLocation.ClearItems();
	id_materialthirdWarehouseLocation.ClearItems();

	id_materialthirdWarehouse.SetValue(null);
	id_materialWarehouse.SetValue(null);
	id_materialthirdWarehouseLocation.SetValue(null);
	id_materialWarehouseLocation.SetValue(null);

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "MachineForProd/WarehouseTypeChangeData",
			type: "post",
			data: { id_warehouseType: data },
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
					for (var i = 0; i < result.length; i++) {
						var warehouse = result[i];
						id_materialWarehouse.AddItem(warehouse.name, warehouse.id);
						id_materialthirdWarehouse.AddItem(warehouse.name, warehouse.id);
					}
					
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}

function WarehouseMaterialCombo_SelectedIndexChanged(s, e) {
	id_materialWarehouseLocation.SetValue(null);
	id_materialWarehouseLocation.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "MachineForProd/WarehouseChangeData",
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
					UpdateDetailObjects(id_materialWarehouseLocation, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}

function MaterialSubCostCenterCombo_SelectedIndexChanged(s, e) {
	id_materialSubCostCenter.SetValue(null);
	id_materialSubCostCenter.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "MachineForProd/CostCenterChangeData",
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
					id_materialSubCostCenter.AddItem(subCostCenter.name, subCostCenter.id);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}

function MaterialThirdSubCostCenterCombo_SelectedIndexChanged(s, e) {
	id_materialthirdSubCostCenter.SetValue(null);
	id_materialthirdSubCostCenter.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data !== null) {

		$.ajax({
			url: "MachineForProd/CostCenterChangeData",
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
					id_materialthirdSubCostCenter.AddItem(subCostCenter.name, subCostCenter.id);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}

function OnThirdMaterialWarehouseLocationEntryInit(s, e) {
	var idwarehouse = id_materialthirdWarehouse.GetValue();
	if (idwarehouse === null) {
		id_materialthirdWarehouseLocation.ClearItems();
	} else {
		$.ajax({
			url: "MachineForProd/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: id_materialthirdWarehouse.GetValue() },
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
					UpdateDetailObjects(id_materialthirdWarehouseLocation, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function OnMaterialWarehouseLocationEntryInit(s, e) {
	var idwarehouse = id_materialWarehouse.GetValue();
	if (idwarehouse === null) {
		id_materialWarehouseLocation.ClearItems();
	} else {
		$.ajax({
			url: "MachineForProd/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: id_materialWarehouse.GetValue() },
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
					UpdateDetailObjects(id_materialWarehouseLocation, result.warehouseLocations, arrayFieldStr);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function OnWarehouseEntryInit(s, e) {
	var idwarehouseType = id_warehouseType.GetValue();
	if (idwarehouseType === null) {
		id_materialWarehouse.ClearItems();
		id_materialthirdWarehouse.ClearItems();
	} else {

		var idMaterialWarehouse = $("#idWarehouseMaterial").val();
		var idMaterialthirdWarehouse = $("#idWarehouseMaterialThird").val();
		$.ajax({
			type: "post",
			url: "MachineForProd/WarehouseTypeChangeData",
			data: { id_warehouseType: idwarehouseType },
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
					id_materialWarehouse.ClearItems();
					id_materialthirdWarehouse.ClearItems();
					var indexMaterial;
					var indexMaterialThird;
					for (var i = 0; i < result.length; i++) {
						var warehouse = result[i];
						if (idMaterialWarehouse === warehouse.id.toString()) {
							indexMaterial = i;
						}
						if (idMaterialthirdWarehouse === warehouse.id.toString()) {
							indexMaterialThird = i;
						}
						id_materialWarehouse.AddItem(warehouse.name, warehouse.id);
						id_materialthirdWarehouse.AddItem(warehouse.name, warehouse.id);
					}
					id_materialWarehouse.SetSelectedIndex(indexMaterial);
					id_materialthirdWarehouse.SetSelectedIndex(indexMaterialThird);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function OnMaterialSubCostCenterEntryInit(s, e) {
	var idmaterialCostCenter = id_materialCostCenter.GetValue();
	if (idmaterialCostCenter === null) {
		id_materialSubCostCenter.ClearItems();
		id_materialSubCostCenter.SetValue(null);
	} else {
		var id = $("#idSubCenterMaterial").val();
		$.ajax({
			url: "MachineForProd/CostCenterChangeData",
			type: "post",
			data: { id_costCenter: idmaterialCostCenter },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				var indexMaterial;
				id_materialSubCostCenter.ClearItems();
				if (result !== null && result !== undefined) {
					for (var i = 0; i < result.length; i++) {
						var warehouse = result[i];
						if (id === warehouse.id.toString()) {
							indexMaterial = i;
						}
						id_materialSubCostCenter.AddItem(warehouse.name, warehouse.id);
					}
					id_materialSubCostCenter.SetSelectedIndex(indexMaterial);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function OnMaterialthirdSubCostCenterInit(s, e) {
	var idmaterialthirdCostCenter = id_materialthirdCostCenter.GetValue();
	if (idmaterialthirdCostCenter === null) {
		id_materialthirdSubCostCenter.ClearItems();
		id_materialthirdSubCostCenter.SetValue(null);
	} else {
		var id = $("#idSubCenterMaterialThird").val();
		$.ajax({
			url: "MachineForProd/CostCenterChangeData",
			type: "post",
			data: { id_costCenter: idmaterialthirdCostCenter },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				var indexMaterial;
				id_materialthirdSubCostCenter.ClearItems();
				if (result !== null && result !== undefined) {
					for (var i = 0; i < result.length; i++) {
						var warehouse = result[i];
						if (id === warehouse.id.toString()) {
							indexMaterial = i;
						}
						id_materialthirdSubCostCenter.AddItem(warehouse.name, warehouse.id);
					}
					id_materialthirdSubCostCenter.SetSelectedIndex(indexMaterial);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}
// MAIN FUNCTIONS

function init() {
	
    AutoCloseAlert();
}

$(function () {
    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateView();
        }
    }, 100);

    init();
});