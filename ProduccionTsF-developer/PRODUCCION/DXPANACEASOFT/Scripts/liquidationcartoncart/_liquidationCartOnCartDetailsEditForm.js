

var id_SalesOrderIniAux = null;
var id_SalesOrderDetailIniAux = null;
var id_ProductionCartIniAux = null;
var id_ItemLiquidationIniAux = null;
var id_ItemToWarehouseIniAux = null;

var errorMessage = "";
var runningValidation = false;

var idItemLiq = 0;
var idItemWarehouseTmp = 0;

//Item Liquidation
function OnItemLiquidationDetailBeginCallback(s, e) {
	idItemLiq = 0;
	e.customArgs["idItemLiq"] = idItemLiq;
	if (!(idItemLiq > 0)) {
		e.customArgs["idItemLiq"] = gvLiquidationCartOnCartDetailEditForm.cpidItemLiq;
		idItemLiq = gvLiquidationCartOnCartDetailEditForm.cpidItemLiq;
	}
	e.customArgs["nameItemFilter"] = nameItemFilter.GetText();
	e.customArgs["sizeBegin"] = idSizeBegin.GetValue();
	e.customArgs["sizeEnd"] = idSizeEnd.GetValue();

	e.customArgs["id_inventoryLine"] = id_inventoryLine.GetValue();
	e.customArgs["id_itemType"] = id_itemType.GetValue();
	e.customArgs["id_itemTypeCategory"] = id_itemTypeCategory.GetValue();
	e.customArgs["id_group"] = id_group.GetValue();
	e.customArgs["id_subgroup"] = id_subgroup.GetValue();
	e.customArgs["id_size"] = id_size.GetValue();
	e.customArgs["id_trademark"] = id_trademark.GetValue();
	e.customArgs["id_trademarkModel"] = id_trademarkModel.GetValue();
	e.customArgs["id_color"] = id_color.GetValue();
	e.customArgs["nameCodigoItemFilter"] = nameCodigoItemFilter.GetText();
	e.customArgs["codeProcessTypeItem"] = $('#codeProcessType').val();
	// 
	e.customArgs["idCliente"] = id_Client.GetValue();
    e.customArgs["id_subProcessIOProductionProcess"] = id_subProcessIOProductionProcess.GetValue();

	e.customArgs["idProcessType"] = idProccesType.GetValue();
	$("#GridMessageErrorDetail").hide();
	if (s !== undefined) {
		if (s.name.startsWith('ItemLiquidation')) {
			var index = parseInt(s.name.substr("ItemLiquidation".length));
			e.customArgs["indice"] = index;
			indexDetail = index;
		}
	}
}

function DetailsItemsLiquidationCombo_SelectedIndexChanged(s, e) {
	var quantity = quantityBoxesIL.GetValue();
	if (quantity === null) { quantity = 0.0; }
	// 
	$.ajax({
		url: "LiquidationCartOnCart/ItemDetailData",
		type: "post",
		data: { id_item: s.GetValue(), quantity: quantity },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
		},
		success: function (result) {
			if (result !== null) {
				quantityKgsIL.SetValue(result.quantityKgsIL);
				quantityPoundsIL.SetValue(result.quantityPoundsIL);
				idItemWarehouseTmp = result.idItemEquivalenceTmp;

				if (idItemWarehouseTmp > 0) {
					var _index = gvLiquidationCartOnCartDetailEditForm.cpRowIndex;
					var key = _index >= 0 ? gvLiquidationCartOnCartDetailEditForm.cpRowKey : 0;
					window["ItemWarehouse" + key].PerformCallback();

					gridMessageErrorDetail.SetText("");
					$("#GridMessageErrorDetail").hide();
				}
				else {
					var _index2 = gvLiquidationCartOnCartDetailEditForm.cpRowIndex;
					var key2 = _index2 >= 0 ? gvLiquidationCartOnCartDetailEditForm.cpRowKey : 0;
					window["ItemWarehouse" + key2].SetValue(null);
					if (result.messageItemEquivalence) {
						var msgErrorAux = ErrorMessage(result.messageItemEquivalence);
						gridMessageErrorDetail.SetText(msgErrorAux);
						$("#GridMessageErrorDetail").show();
					}
				}
			}
		},
		complete: function () {
		}
	});
}

function ItemWarehouseCombo_OnInit(s, e) {

}

function SubProcessIOProductionProcess_OnInit(s, e) {
    if (s.GetValue() === null) {
        s.SetValue(id_subProcessIOProductionProcessFilter.GetValue());
    }
}

function ProductionLotOrigen_OnInit(s, e) {
	// 
	if (s.GetValue() === null) {
		s.SetValue(internalNumber.GetValue());
	}
}

function OnItemWarehouseDetailBeginCallback(s, e) {
	// 
	var _index = gvLiquidationCartOnCartDetailEditForm.cpRowIndex;
	var key = _index >= 0 ? gvLiquidationCartOnCartDetailEditForm.cpRowKey : 0;
	var id_itemTmp = window["ItemLiquidation" + key].GetValue();
	e.customArgs["id_itemCurrent"] = id_itemTmp;
	e.customArgs["id_ItemEquivalence"] = idItemWarehouseTmp;
	//idItemWarehouseTmp = id_itemTmp;

	if (s != undefined) {
		if (s.name.startsWith('ItemWarehouse')) {
			var index = parseInt(s.name.substr("ItemWarehouse".length));
			e.customArgs["indice"] = index;
			indexDetail = index;
		}
	}
}

function OnItemWarehouseDetailEndCallback(s, e) {
	var idItWa = $('#idItemWarehouseC').val();
	var quantity = quantityBoxesIL.GetValue();
	if (quantity === null) { quantity = 0.0; }

	$.ajax({
		url: "LiquidationCartOnCart/ItemWarehouseDetailData",
		type: "post",
		data: { id_item: s.GetValue(), quantity: quantity, id_ItemEquivalence: idItemWarehouseTmp },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null) {
				quantityKgsITW.SetValue(result.quantityKgsITW);
				quantityPoundsITW.SetValue(result.quantityPoundsITW);
				if (idItemWarehouseTmp > 0) {
					s.SetValue(idItemWarehouseTmp);
					Quantity_NumberChange(quantityBoxesIL, null);
				}
			}
		},
		complete: function () {
			//hideLoading();
		}
	});
}

function DetailsItemsWarehouseCombo_SelectedIndexChanged(s, e) {
	var quantity = quantityBoxesIL.GetValue();
	if (quantity === null) { quantity = 0.0; }

	$.ajax({
		url: "LiquidationCartOnCart/ItemWarehouseDetailData",
		type: "post",
		data: { id_item: s.GetValue(), quantity: quantity },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null) {

				quantityKgsITW.SetValue(result.quantityKgsITW);
				quantityPoundsITW.SetValue(result.quantityPoundsITW);

			}
		},
		complete: function () {
			//hideLoading();
		}
	});
}

function OnGridViewDetailBeginCallback(s, e) {
	e.customArgs["nameItemFilter"] = nameItemFilter.GetText();
	e.customArgs["sizeBegin"] = idSizeBegin.GetValue();
	e.customArgs["sizeEnd"] = idSizeEnd.GetValue();

	e.customArgs["id_inventoryLine"] = id_inventoryLine.GetValue();
	e.customArgs["id_itemType"] = id_itemType.GetValue();
	e.customArgs["id_itemTypeCategory"] = id_itemTypeCategory.GetValue();
	e.customArgs["id_group"] = id_group.GetValue();
	e.customArgs["id_subgroup"] = id_subgroup.GetValue();
	e.customArgs["id_size"] = id_size.GetValue();
	e.customArgs["id_trademark"] = id_trademark.GetValue();
	e.customArgs["id_trademarkModel"] = id_trademarkModel.GetValue();
	e.customArgs["id_color"] = id_color.GetValue();
	e.customArgs["nameCodigoItemFilter"] = nameCodigoItemFilter.GetText();
	e.customArgs["idCliente"] = id_Person.GetValue();

	e.customArgs["codeProcessTypeItem"] = $('#codeProcessType').val();
	e.customArgs["idProcessType"] = idProccesType.GetValue();
	var indexDetail = 0;
	if (e.command == "UPDATEROW" || e.command == "UPDATEEDIT") {
		var _index = gvLiquidationCartOnCartDetailEditForm.cpRowIndex;
		var key = _index >= 0 ? gvLiquidationCartOnCartDetailEditForm.cpRowKey : 0;
		if (window["ItemWarehouse" + key].GetValue() == null) {
			e.customArgs["id_ItemToWarehouse"] = null;
		}

		e.customArgs["id_itemLiquidation2"] = window["ItemLiquidation" + key].GetValue();
		e.customArgs["id_itemWarehouse2"] = window["ItemWarehouse" + key].GetValue();
	}
}

function OnGridViewDetailEndCallback(s, e) {
	// 
	gridMessageErrorDetail.SetText("");
	$("#GridMessageErrorDetail").hide();
	gvLiquidationCartOnCartReceivedDetailEditForm.PerformCallback();
	if (gvLiquidationCartOnCartDetailEditForm.cpidItemLiq > 0) {
		var _index = gvLiquidationCartOnCartDetailEditForm.cpRowIndex;
		var key = _index >= 0 ? gvLiquidationCartOnCartDetailEditForm.cpRowKey : 0;

		//if (window["ItemLiquidation" + key] && window["ItemLiquidation" + key].GetMainElement()) {
		//	window["ItemLiquidation" + key].PerformCallback();
		//}
		idItemLiq = gvLiquidationCartOnCartDetailEditForm.cpidItemLiq;
	}
}

//Validations

function OnProductionCartLiquidationCartOnCartDetailValidation(s, e) {
	errorMessage = "";
	$("#GridMessageErrorDetail").hide();

	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		errorMessage = "- Carro: Es obligatorio.";
    }

    if (!runningValidation) {
        ValidateDetail();
	}
}

function OnItemLiquidationCartOnCartDetailValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		//errorMessage = "- Producto Liquidación: Es obligatorio.";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Producto Liquidación: Es obligatorio.";
		} else {
			errorMessage += "</br> - Producto Liquidación: Es obligatorio.";
		}
		//} else {
		//	var _index = gvLiquidationCartOnCartDetailEditForm.cpRowIndex;
		//	var key = _index >= 0 ? gvLiquidationCartOnCartDetailEditForm.cpRowKey : 0;
		//	var id_itemWarehouseTmp = window["ItemWarehouse" + key].GetValue();

		//	var data = {
		//		id_salesOrderNew: null,
		//		//id_SalesOrder.GetValue(),
		//		id_ProductionCartNew: id_ProductionCart.GetValue(),
		//		id_ItemLiquidationNew: s.GetValue(),
		//		id_ItemToWarehouseNew: id_itemWarehouseTmp
		//	};
		//	if (data.id_salesOrderNew != id_SalesOrderIniAux || data.id_ProductionCartNew != id_ProductionCartIniAux ||
		//		data.id_ItemLiquidationNew != id_ItemLiquidationIniAux || data.id_ItemToWarehouseNew != id_ItemToWarehouseIniAux) {
		//		$.ajax({
		//			url: "LiquidationCartOnCart/ItsRepeatedLiquidation",
		//			type: "post",
		//			data: data,
		//			async: false,
		//			cache: false,
		//			error: function(error) {
		//				console.log(error);
		//			},
		//			beforeSend: function() {
		//				//showLoading();
		//			},
		//			success: function(result) {
		//				if (result !== null) {
		//					if (result.itsRepeated == 1) {
		//						e.isValid = false;
		//						e.errorText = result.Error;
		//						//errorMessage = result.Error;
		//						if (errorMessage == null || errorMessage == "") {
		//							errorMessage = result.Error;
		//						} else {
		//							errorMessage += "</br> " + result.Error;
		//						}
		//					}
		//				}
		//			},
		//			complete: function() {
		//				//hideLoading();
		//			}
		//		});
		//	}
	}

	if (!runningValidation) {
        ValidateDetail();
	}
}

function OnQuantityBoxesILLiquidationCartOnCartDetailValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage === null || errorMessage === "") {
            errorMessage = "- Cajas: Es obligatorio.";
        } else {
            errorMessage += "</br> - Cajas: Es obligatorio.";
        }
    } else if (parseFloat(s.GetValue()) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecto";
        if (errorMessage === null || errorMessage === "") {
            errorMessage = "- Cajas: Valor Incorrecto.";
        } else {
            errorMessage += "</br> - Cajas: Valor Incorrecto.";
        }
    } else {
        var boxesReceivedAux = boxesReceived.GetValue() === null ? 0.00 : parseFloat(boxesReceived.GetValue());
        var quantityBoxesILAux = parseFloat(s.GetValue());
        if (quantityBoxesILAux < boxesReceivedAux) {
            e.isValid = false;
            e.errorText = "Cajas debe ser mayor o igual Cj. Recibidas";
            if (errorMessage === null || errorMessage === "") {
                errorMessage = "- Cajas: Debe ser mayor o igual Cj. Recibidas.";
            } else {
                errorMessage += "</br> - Cajas: Debe ser mayor o igual Cj. Recibidas";
            }
        }
    } 

    if (!runningValidation) {
        ValidateDetail();
    }

}

function OnItemLiquidationCartOnCartWarehouseDetailValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Producto a Bodega: Es obligatorio.";
		} else {
			errorMessage += "</br> - Producto a Bodega: Es obligatorio.";
		}
	}

    if (!runningValidation) {
        ValidateDetail();
    }
}

function OnClientLiquidationCartOnCartDetailValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cliente: Es obligatorio.";
        } else {
            errorMessage += "</br> - Cliente: Es obligatorio.";
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }
}

function OnSubProcessIOProductionProcessLiquidationCartOnCartDetailValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Destino: Es obligatorio.";
        } else {
            errorMessage += "</br> - Destino: Es obligatorio.";
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }
    //console.log("A Mostrar el error");
    if (errorMessage != null && errorMessage != "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorDetail.SetText(msgErrorAux);
        $("#GridMessageErrorDetail").show();

    }
}

function OnProductionLotManualLiquidationCartOnCartDetailValidation(s, e) {
	// 
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- No. Lote: Es obligatorio.";
		} else {
			errorMessage += "</br> - No. Lote: Es obligatorio.";
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}

	if (errorMessage != null && errorMessage != "") {
		var msgErrorAux = ErrorMessage(errorMessage);
		gridMessageErrorDetail.SetText(msgErrorAux);
		$("#GridMessageErrorDetail").show();

	}
}

function ValidateDetail() {
	runningValidation = true;
    OnProductionCartLiquidationCartOnCartDetailValidation(id_ProductionCart, id_ProductionCart);
    var _index = gvLiquidationCartOnCartDetailEditForm.cpRowIndex;
    var key = _index >= 0 ? gvLiquidationCartOnCartDetailEditForm.cpRowKey : 0;
    var id_ItemLiquidationAux = window["ItemLiquidation" + key];
    OnItemLiquidationCartOnCartDetailValidation(id_ItemLiquidationAux, id_ItemLiquidationAux);
	OnQuantityBoxesILLiquidationCartOnCartDetailValidation(quantityBoxesIL, quantityBoxesIL);
    var id_ItemWarehouseAux = window["ItemWarehouse" + key];
    OnItemLiquidationCartOnCartWarehouseDetailValidation(id_ItemWarehouseAux, id_ItemWarehouseAux);
    OnClientLiquidationCartOnCartDetailValidation(id_Client, id_Client);
    OnSubProcessIOProductionProcessLiquidationCartOnCartDetailValidation(id_subProcessIOProductionProcess, id_subProcessIOProductionProcess);
	runningValidation = false;
}

// ItemLiquidationquantityBoxesIL
function LiquidationCartOnCartDetailItem_BeginCallback(s, e) {
	// 
	var _index = gvLiquidationCartOnCartDetailEditForm.cpRowIndex;
	var key = _index >= 0 ? gvLiquidationCartOnCartDetailEditForm.cpRowKey : 0;
	var id_itemLiquidationTmp = window["ItemLiquidation" + key].GetValue();

	e.customArgs["id_itemIni"] = id_ItemLiquidationIniAux;
	e.customArgs["id_item"] = id_itemLiquidationTmp;
	e.customArgs["id_salesOrder"] = null;
	//id_SalesOrder.GetValue();
	e.customArgs["id_salesOrderDetailIni"] = id_SalesOrderDetailIniAux;
}

function LiquidationCartOnCartDetailItem_EndCallback(s, e) {
	// 
	//id_ItemToWarehouse.PerformCallback();
}

function ItemLiquidationCartOnCartDetailCombo_SelectedIndexChanged(s, e) {
	var quantity = quantityBoxesIL.GetValue();
	if (quantity === null) { quantity = 0.0; }

	$.ajax({
		url: "LiquidationCartOnCart/ItemDetailData",
		type: "post",
		data: { id_item: s.GetValue(), quantity: quantity },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null) {

				quantityKgsIL.SetValue(result.quantityKgsIL);
				quantityPoundsIL.SetValue(result.quantityPoundsIL);

				//id_ItemToWarehouse.PerformCallback();

			}
		},
		complete: function () {
			//hideLoading();
		}
	});
}

//SalesOrderLiquidationCartOnCart
function SalesOrderLiquidationCartOnCartDetailCombo_SelectedIndexChanged(s, e) {
	//id_ItemLiquidation.ClearItems();
	quantityKgsIL.SetValue(0);
	quantityPoundsIL.SetValue(0);

	//id_ItemToWarehouse.ClearItems();
	quantityKgsITW.SetValue(0);
	quantityPoundsITW.SetValue(0);

	//id_ItemLiquidation.PerformCallback();

}

function SalesOrderLiquidationCartOnCartDetailCombo_Init(s, e) {


	id_SalesOrderIniAux = s.GetValue();
	id_SalesOrderDetailIniAux = gvLiquidationCartOnCartDetailEditForm.cpEditingRowSalesOrderDetail;
	id_ProductionCartIniAux = id_ProductionCart.GetValue();

	var _index = gvLiquidationCartOnCartDetailEditForm.cpRowIndex;
	var key = _index >= 0 ? gvLiquidationCartOnCartDetailEditForm.cpRowKey : 0;
	var id_itemTmp = window["ItemLiquidation" + key].GetValue();
	var id_itemWarehouseTmp = window["ItemWarehouse" + key].GetValue();
	id_ItemLiquidationIniAux = id_itemTmp;
	id_ItemToWarehouseIniAux = id_itemWarehouseTmp;


	var data = {
		id_salesOrder: s.GetValue(),
	};

	$.ajax({
		url: "LiquidationCartOnCart/InitSalesOrder",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			id_metricUnit.SetValue(null);
			id_metricUnitIniAux = null;
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			//id_salesOrder
			var salesOrderAux = s.FindItemByValue(result.salesOrder.id);
			if (salesOrderAux == null && result.salesOrder.id != null) s.AddItem(result.salesOrder.name, result.salesOrder.id);
			s.SetValue(result.salesOrder.id);

			//id_ItemLiquidation.PerformCallback();

		},
		complete: function () {
			//hideLoading();
		}
	});

	//ItemProductionLotLiquidationDetailCombo_OnInit(s, e);
}


//QuantityBoxesILLiquidationCart
function Quantity_NumberChange(s, e) {
	// 
	var quantity = s.GetValue();
	if (quantity === null) { quantity = 0.0; }

	var _index = gvLiquidationCartOnCartDetailEditForm.cpRowIndex;
	var key = _index >= 0 ? gvLiquidationCartOnCartDetailEditForm.cpRowKey : 0;
	var id_itemLiquidationTmp = window["ItemLiquidation" + key].GetValue();
	var id_itemWarehouseTmp = window["ItemWarehouse" + key].GetValue();

	$.ajax({
		url: "LiquidationCartOnCart/UpdateQuantityTotal",
		type: "post",
		data: {
			id_ItemLiquidation: id_itemLiquidationTmp,
			id_ItemToWarehouse: id_itemWarehouseTmp,
			quantity: quantity
		},
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null) {
				quantityKgsIL.SetValue(result.quantityKgsIL);
				quantityPoundsIL.SetValue(result.quantityPoundsIL);

				quantityKgsITW.SetValue(result.quantityKgsITW);
				quantityPoundsITW.SetValue(result.quantityPoundsITW);
			}
		},
		complete: function () {
			//hideLoading();
		}
	});
}


//ItemLiquidationCartOnCartWarehouse
function LiquidationCartOnCartWarehouseDetailItem_BeginCallback(s, e) {
	var _index = gvLiquidationCartOnCartDetailEditForm.cpRowIndex;
	var key = _index >= 0 ? gvLiquidationCartOnCartDetailEditForm.cpRowKey : 0;
	var id_itemLiquidationTmp = window["ItemLiquidation" + key].GetValue();
	e.customArgs["id_itemCurrent"] = id_itemLiquidationTmp;
}

function LiquidationCartOnCartWarehouseDetailItem_EndCallback(s, e) {

	//id_ItemToWarehouse.SetValue(id_ItemToWarehouseIniAux);
}

function ItemLiquidationCartOnCartWarehouseDetailCombo_SelectedIndexChanged(s, e) {

	var quantity = quantityBoxesIL.GetValue();
	if (quantity === null) { quantity = 0.0; }

	$.ajax({
		url: "LiquidationCartOnCart/ItemWarehouseDetailData",
		type: "post",
		data: { id_item: s.GetValue(), quantity: quantity },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null) {

				quantityKgsITW.SetValue(result.quantityKgsITW);
				quantityPoundsITW.SetValue(result.quantityPoundsITW);

			}
		},
		complete: function () {
			//hideLoading();
		}
	});
}


//DETAILS ACTIONS BUTTONS

function AddNewDetail(s, e) {
	//if (gv !== null && gv !== undefined) {
	//    gv.AddNewRow();
	//}
	gvLiquidationCartOnCartDetailEditForm.AddNewRow();
}

function RemoveDetail(s, e) {
}

function RefreshDetail(s, e) {
	//if (gv !== null && gv !== undefined) {
	//    gv.UnselectRows();
	//    gv.PerformCallback();
	//}
	// 
	gvLiquidationCartOnCartDetailEditForm.UnselectRows();
	gvLiquidationCartOnCartDetailEditForm.PerformCallback();
}

// EDITOR'S EVENTS
function ItemComboBox_BeginCallback(s, e) {
	e.customArgs["id_itemCurrent"] = itemCurrentAux;
}

function ItemComboBox_EndCallback(s, e) {
	id_item.SetValue(itemCurrentAux);
}

function OnItemComboBoxValidation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}


// Manejadores de  controles de filtro de productos
var InventoryLinesComboBox_SelectedIndexChanged = function (s, e) {
	id_itemType.ClearItems();
	id_itemTypeCategory.ClearItems();

	var idLinea = id_inventoryLine.GetValue();
	var numTipos = id_itemType.cpTipos ? id_itemType.cpTipos.length : 0;
	for (var i = 0; i < numTipos; i++) {
		var tipo = id_itemType.cpTipos[i];
		if (tipo.id_inventoryLine === idLinea) {
			id_itemType.AddItem(tipo.name, tipo.id);
		}
	}
};

var ItemTypesComboBox_SelectedIndexChanged = function (s, e) {
	id_itemTypeCategory.ClearItems();

	var idTipo = id_itemType.GetValue();
	var numCategorias = id_itemTypeCategory.cpCategorias ? id_itemTypeCategory.cpCategorias.length : 0;
	for (var i = 0; i < numCategorias; i++) {
		var categoria = id_itemTypeCategory.cpCategorias[i];
		if (categoria.id_itemType === idTipo) {
			id_itemTypeCategory.AddItem(categoria.name, categoria.id);
		}
	}
};

var ItemGroupsComboBox_SelectedIndexChanged = function (s, e) {
	id_subgroup.ClearItems();

	var idGrupo = id_group.GetValue();
	var numSubgrupos = id_subgroup.cpSubgrupos ? id_subgroup.cpSubgrupos.length : 0;
	for (var i = 0; i < numSubgrupos; i++) {
		var subgrupo = id_subgroup.cpSubgrupos[i];
		if (subgrupo.id_parentGroup === idGrupo) {
			id_subgroup.AddItem(subgrupo.name, subgrupo.id);
		}
	}
};
