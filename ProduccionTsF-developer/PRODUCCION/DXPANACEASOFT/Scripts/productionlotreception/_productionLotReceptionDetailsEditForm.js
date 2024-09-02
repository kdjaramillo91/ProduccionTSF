//var id_IniAux = 0
var id_itemIniAux = 0;
var id_warehouseIniAux = 0;
var id_warehouseLocationIniAux = 0;

var errorMessage = "";
var runningValidation = false;

//Validations

function OnPurcharseOrderValidation(s, e) {

	//gridMessageErrorMaterialsDetail.SetText(result.Message);
	errorMessage = "";

	$("#GridMessageErrorsDetail").hide();

	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage === null || errorMessage === "") {
			errorMessage = "- Orden de Compra: Debe elegir una Orden de Compra.";
		} else {
			errorMessage += "</br>- Orden de Compra: Debe elegir una Orden de Compra.";
		}
	}

	if (!runningValidation) {
		ValidateDetailMP();
	}

}

function OnRemissionGuideValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage === null || errorMessage === "") {
			errorMessage = "- Guía de Remisión: Debe elegir una Guía de Remisión.";
		} else {
			errorMessage += "</br>- Guía de Remisión: Debe elegir una Guía de Remisión.";
		}
	}

	if (!runningValidation) {
		ValidateDetailMP();
	}

}

function OnItemDetailValidation(s, e) {
	////gridMessageErrorMaterialsDetail.SetText(result.Message);
	//errorMessage = "";

	//$("#GridMessageErrorsDetail").hide();

	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		//errorMessage = "- Nombre del Producto: Es obligatorio.";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Nombre del Producto: Es obligatorio.";
		} else {
			errorMessage += "</br>- Nombre del Producto: Es obligatorio.";
		}
		//console.log("e.isValid: " + e.errorText);
	} else {
		var data = {
			id_item: s.GetValue()
		};
		$.ajax({
			url: "ProductionLotReception/ExistsConversionWithLbs",
			type: "post",
			data: data,
			async: false,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				//showLoading();
			},
			success: function (result) {
				if (result !== null) {
					if (result.metricUnitConversionValue === 0) {
						e.isValid = false;
						e.errorText = "Unidad medida del Producto no tiene factor de conversión configurado con " + result.metricUnitName + " cuyo codigo se espera que sea (" + result.metricUnitCode + "). Configúrelo e intente de nuevo.";
						//errorMessage = "- Nombre del Producto: " + e.errorText;
						if (errorMessage === null || errorMessage === "") {
							errorMessage = "- Nombre del Producto: " + e.errorText;
						} else {
							errorMessage += "</br>- Nombre del Producto: " + e.errorText;
						}
						//console.log("e.isValid: " + e.errorText);

					} else {
						//console.log("gvProductionLotReceptionEditFormItemsDetail.cpEditingRowKey: ");
						//console.log(gvProductionLotReceptionEditFormItemsDetail.cpEditingRowKey);

						//var idAux = gvProductionLotReceptionEditFormItemsDetail.GetRowValues(gvProductionLotReceptionEditFormItemsDetail.EditingRowVisibleIndex, "id");
						//console.log("idAux: ");
						//console.log(idAux);
						var data = {
							id: gvProductionLotReceptionEditFormItemsDetail.cpEditingRowKey,
							id_itemNew: s.GetValue(),
							//id_itemOld: id_itemIniAux,
							id_warehouseNew: id_warehouse.GetValue(),
							//id_warehouseOld: id_warehouseIniAux,
							id_warehouseLocationNew: id_warehouseLocation.GetValue()
							//id_warehouseLocationOld: id_warehouseLocationIniAux
						};
						//console.log("id_itemIniAux: " + id_itemIniAux);
						//console.log("data.id_itemNew: " + data.id_itemNew);
						//console.log("id_warehouseIniAux: " + id_warehouseIniAux);
						//console.log("data.id_warehouseNew: " + data.id_warehouseNew);
						//console.log("id_warehouseLocationIniAux: " + id_warehouseLocationIniAux);
						//console.log("data.id_warehouseLocationNew: " + data.id_warehouseLocationNew);
						if (data.id_itemNew !== id_itemIniAux || data.id_warehouseNew !== id_warehouseIniAux ||
							data.id_warehouseLocationNew !== id_warehouseLocationIniAux) {
							$.ajax({
								url: "ProductionLotReception/ItsRepeatedItem",
								type: "post",
								data: data,
								async: false,
								cache: false,
								error: function (error) {
									console.log(error);
								},
								beforeSend: function () {
									//showLoading();
								},
								success: function (result) {
									if (result !== null) {
										//console.log("result.itsRepeated: ");
										//console.log(result.itsRepeated);
										//console.log("result.itsRepeated == 1: ");
										//console.log(result.itsRepeated == 1);
										if (result.itsRepeated === 1) {
											e.isValid = false;
											e.errorText = result.Error;
											//errorMessage = "- Nombre del Producto: " + e.errorText;
											//console.log("e.isValid: " + e.errorText);
											if (errorMessage === null || errorMessage === "") {
												errorMessage = "- Nombre del Producto: " + e.errorText;
											} else {
												errorMessage += "</br>- Nombre del Producto: " + e.errorText;
											}
										}
										//else {
										//    //id_IniAux = 0
										//    id_itemIniAux = 0
										//    id_warehouseIniAux = 0
										//    id_warehouseLocationIniAux = 0
										//}
									}
								},
								complete: function () {
									//hideLoading();
								}
							});
						}

					}
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}

	if (!runningValidation) {
		ValidateDetailMP();
	}
}

function OnWarehouseDetailValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage === null || errorMessage === "") {
			errorMessage = "- Bodega: Es obligatoria.";
		} else {
			errorMessage += "</br>- Bodega: Es obligatoria.";
		}
	}

	if (!runningValidation) {
		ValidateDetailMP();
	}
}

function OnWarehouseLocationDetailValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage === null || errorMessage === "") {
			errorMessage = "- Ubicación: Es obligatoria.";
		} else {
			errorMessage += "</br>- Ubicación: Es obligatoria.";
		}
	}

	if (!runningValidation) {
		ValidateDetailMP();
	}

}

//function OnQuantityRemittedValidation(s, e) {
//    if (s.GetValue() === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//        if (errorMessage == null || errorMessage == "") {
//            errorMessage = "- Cantidad Remitida: Es obligatoria.";
//        } else {
//            errorMessage += "</br>- Cantidad Remitida: Es obligatoria.";
//        }
//    } else if (parseFloat(s.GetValue()) <= 0) {
//        e.isValid = false;
//        e.errorText = "Cantidad Incorrecto";
//        if (errorMessage == null || errorMessage == "") {
//            errorMessage = "- Cantidad Remitida: Es incorrecta.";
//        } else {
//            errorMessage += "</br>- Cantidad Remitida: Es incorrecta.";
//        }
//    }

//    if (!runningValidation) {
//        ValidateDetailMP();
//    }
//}

function OnQuantityRecivedValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage === null || errorMessage === "") {
			errorMessage = "- Cantidad Remitida: Es obligatoria.";
		} else {
			errorMessage += "</br>- Cantidad Remitida: Es obligatoria.";
		}
	} else if (parseFloat(s.GetValue()) <= 0) {
		e.isValid = false;
		e.errorText = "Cantidad Incorrecto";
		if (errorMessage === null || errorMessage === "") {
			errorMessage = "- Cantidad Remitida: Es incorrecta.";
		} else {
			errorMessage += "</br>- Cantidad Remitida: Es incorrecta.";
		}
	}

	if (!runningValidation) {
		ValidateDetailMP();
	}

	//if (errorMessage != null && errorMessage != "") {
	//    var msgErrorAux = ErrorMessage(errorMessage);
	//    gridMessageErrorsDetail.SetText(msgErrorAux);
	//    $("#GridMessageErrorsDetail").show();

	//}

}

function OnDrawersNumberValidationPO(s, e) {
	//// 
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage === null || errorMessage === "") {
			errorMessage = "- Número de Kavetas: Es obligatoria.";
		} else {
			errorMessage += "</br>- Número de Kavetas: Es obligatoria.";
		}
	} else if (parseInt(s.GetValue()) <= 0) {
		e.isValid = false;
		e.errorText = "Cantidad Incorrecto";
		if (errorMessage === null || errorMessage === "") {
			errorMessage = "- Número de Kavetas: Es incorrecta.";
		} else {
			errorMessage += "</br>- Número de Kavetas: Es incorrecta.";
		}
	}

	if (!runningValidation) {
		ValidateDetailMP();
	}
	if (errorMessage !== null && errorMessage !== "") {
		var msgErrorAux = ErrorMessage(errorMessage);
		gridMessageErrorsDetail.SetText(msgErrorAux);
		$("#GridMessageErrorsDetail").show();

	}
}

function OnquantitydrainedValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage === null || errorMessage === "") {
			errorMessage = "- Cantidad Recibida - Escurrido: Es obligatoria.";
		} else {
			errorMessage += "</br>- Cantidad Recibida - Escurrido: Es obligatoria.";
		}
	} else if (parseFloat(s.GetValue()) <= 0) {
		e.isValid = false;
		e.errorText = "Cantidad Incorrecto";
		if (errorMessage === null || errorMessage === "") {
			errorMessage = "- Cantidad Recibida - Escurrido: Es incorrecta.";
		} else {
			errorMessage += "</br>- Cantidad Recibida - Escurrido: Es incorrecta.";
		}
	}

	if (!runningValidation) {
		ValidateDetailMP();
	}
	//if (errorMessage !== null && errorMessage !== "") {
	//    var msgErrorAux = ErrorMessage(errorMessage);
	//    gridMessageErrorsDetail.SetText(msgErrorAux);
	//    $("#GridMessageErrorsDetail").show();

	//}
}





function ValidateDetailMP() {
	runningValidation = true;
	OnPurcharseOrderValidation(id_purchaseOrder, id_purchaseOrder);
	OnRemissionGuideValidation(id_remissionGuide, id_remissionGuide);
	OnItemDetailValidation(id_item, id_item);
	OnWarehouseDetailValidation(id_warehouse, id_warehouse);
	OnWarehouseLocationDetailValidation(id_warehouseLocation, id_warehouseLocation);
	//OnQuantityRemittedValidation(quantityRemitted, quantityRemitted);
	OnQuantityRecivedValidation(quantityRecived, quantityRecived);
	OnDrawersNumberValidationPO(drawersNumberPO, drawersNumberPO);
	//OnquantitydrainedValidation(quantitydrained, quantitydrained);
	runningValidation = false;
}

// EDITOR'S EVENTS

var id_purcharseOrderIniAux = null;
var id_remissionGuideIniAux = null;
var quantityRecivedIniAux = 0.00;
var quantityOrderedIniAux = 0.00;
var quantityRemittedIniAux = 0.00;

var PurcharseOrder_BeginCallback = function (s, e) {
	e.customArgs["id_purchaseOrder"] = id_purchaseOrder.GetValue() !== null ? id_purchaseOrder.GetValue() : id_purcharseOrderIniAux;
};

var PurcharseOrder_EndCallback = function (s, e) {
	if (id_purcharseOrderIniAux !== null) {
		id_purchaseOrder.SetValue(id_purcharseOrderIniAux);
	}
	var data = {
		id: gvProductionLotReceptionEditFormItemsDetail.cpEditingRowKey,
		id_item: id_item.GetValue(),
		id_purchaseOrder: id_purchaseOrder.GetValue()
	};
	$.ajax({
		url: "ProductionLotReception/ProductionLotReceptionItemData",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			metricUnitPO.SetText("");
			process.SetText("");
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null && result !== undefined) {
				process.SetText(result.process);
				metricUnitPO.SetText(result.metricUnit);
				//s.SetEnabled(id_itemIniAux === null || id_itemIniAux === 0 || (!result.hasOrderDetail));
			}
			else {
				metricUnitPO.SetText("");
				process.SetText("");
			}
		},
		complete: function () {
			//hideLoading();
			id_remissionGuide.PerformCallback();
		}
	});
};

function PurcharseOrder_SelectedIndexChanged(s, e) {
	id_purcharseOrderIniAux = null;
	id_remissionGuideIniAux = null;
	id_remissionGuide.SetValue(null);
	id_remissionGuide.PerformCallback();
}

var RemissionGuide_BeginCallback = function (s, e) {
	e.customArgs["id_purchaseOrder"] = id_purchaseOrder.GetValue() !== null ? id_purchaseOrder.GetValue() : id_purcharseOrderIniAux;
	e.customArgs["id_remissionGuide"] = id_remissionGuide.GetValue() !== null ? id_remissionGuide.GetValue() : id_remissionGuideIniAux;
};

var RemissionGuide_EndCallback = function (s, e) {
	if (id_remissionGuideIniAux !== null) {
		id_remissionGuide.SetValue(id_remissionGuideIniAux);
	}
	UpdateDetailQuantity();
};

function RemissionGuide_SelectedIndexChanged(s, e) {
	id_remissionGuideIniAux = null;
	quantityRecivedIniAux = 0.00;
	quantityOrderedIniAux = 0.00;
	quantityRemittedIniAux = 0.00;
	UpdateDetailQuantity();
}

function UpdateDetailQuantity() {

	var data = {
		id: gvProductionLotReceptionEditFormItemsDetail.cpEditingRowKey,
		id_item: id_item.GetValue(),
		id_purchaseOrder: id_purchaseOrder.GetValue(),
		id_remissionGuide: id_remissionGuide.GetValue()
	};
	$.ajax({
		url: "ProductionLotReception/ProductionLotReceptionItemDataRemissionGuide",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			quantityOrdered.SetValue(0);
			quantityRemitted.SetValue(0);
			quantityRecived.SetValue(0.00);
			id_item.SetValue(null);
			process.SetText("");
			id_externalGuide.SetText("");
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null && result !== undefined) {
				debugger;
				if (quantityOrderedIniAux === 0.00) {
					quantityOrdered.SetValue(result.quantityOrdered);
				} else {
					quantityOrdered.SetValue(quantityOrderedIniAux);
				}

				if (quantityRemittedIniAux === 0.00) {
					quantityRemitted.SetValue(result.quantityRemitted);
				} else {
					quantityRemitted.SetValue(quantityRemittedIniAux);
				}

				if (quantityRecivedIniAux === 0.00) {
					quantityRecived.SetValue(result.quantityRemitted);
				} else {
					quantityRecived.SetValue(quantityRecivedIniAux);
				}

				id_item.SetValue(result.id_item);
				process.SetText(result.process);
				id_externalGuide.SetText(result.remissionGuideGuiaExterna);
			}
			else {
				quantityOrdered.SetValue(0);
				quantityRemitted.SetValue(0);
				quantityRecived.SetValue(0.00);
				id_item.SetValue(null);
				process.SetText("");
				id_externalGuide.SetText("");
			}
		},
		complete: function () {
			//hideLoading();
			ItemDetailCombo_SelectedIndexChanged(id_item, id_item);
		}
	});
}

function UpdateDetailInfo(data, s, e) {

	if (data.id_item === null) {
		return;
	}

	//purchaseOrderNumber.SetText("");
	//remissionGuideNumber.SetText("");
	metricUnitPO.SetText("");
	//gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnit").SetText("");
	gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(null);// 
	gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(null);// SetValue("");

	if (id_item !== null) {

		$.ajax({
			url: "ProductionLotReception/ItemDetailData",
			type: "post",
			data: data,
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
					metricUnitPO.SetText(result.metricUnit);
					//gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnit").SetText(result.metricUnit);
					gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);

					arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(id_warehouseLocation, result.warehouseLocations, arrayFieldStr);
					gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
				}
			},
			complete: function () {
				//hideLoading();
				// UpdateDetailQuantity();
			}
		});
	}
}

function ItemDetailCombo_SelectedIndexChanged(s, e) {
	UpdateDetailInfo({
		id_item: s.GetValue()
	}, s, e);
}

function ItemDetailCombo_DropDown(s, e) {

	$.ajax({
		url: "ProductionLotReception/ProductionLotReceptionItemDetails",
		type: "post",
		data: null,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			for (var i = 0; i < id_item.GetItemCount(); i++) {
				var item = id_item.GetItem(i);
				if (result.indexOf(item.value) >= 0) {
					id_item.RemoveItem(i);
					i = -1;
				}
			}
		},
		complete: function () {
			//hideLoading();
		}
	});
}

function ItemDetailCombo_Init(s, e) {
	//// 
	s.SetEnabled(false);
	//s.GetRowKey(e.elementIndex);
	//id_IniAux = id_aux.GetValue();
	id_purcharseOrderIniAux = gvProductionLotReceptionEditFormItemsDetail.cpEditingIdPurcharseOrder;
	id_purchaseOrder.SetValue(id_purcharseOrderIniAux);
	id_remissionGuideIniAux = gvProductionLotReceptionEditFormItemsDetail.cpEditingIdRemissionGuide;
	id_remissionGuide.SetValue(id_remissionGuideIniAux);
	quantityRecivedIniAux = quantityRecived.GetValue();
	quantityOrderedIniAux = quantityOrdered.GetValue();
	quantityRemittedIniAux = quantityRemitted.GetValue();

	id_itemIniAux = s.GetValue();
	id_warehouseIniAux = id_warehouse.GetValue();
	id_warehouseLocationIniAux = id_warehouseLocation.GetValue();
	//console.log("id_itemIniAux: ");
	//console.log(id_itemIniAux);
	//console.log("id_warehouseIniAux: ");
	//console.log(id_warehouseIniAux);
	//console.log("id_warehouseLocationIniAux: ");
	//console.log(id_warehouseLocationIniAux);

	//console.log("id_itemIniAux: ");
	//console.log(id_itemIniAux);
	//console.log("id_itemIniAux == null || id_itemIniAux == 0: ");
	//console.log(id_itemIniAux == null || id_itemIniAux == 0);

	//s.SetEnabled(id_itemIniAux == null || id_itemIniAux == 0);

	var data = {
		id: gvProductionLotReceptionEditFormItemsDetail.cpEditingRowKey,
		id_item: s.GetValue(),
		id_purchaseOrder: id_purcharseOrderIniAux
	};

	$.ajax({
		url: "ProductionLotReception/ProductionLotReceptionItemData",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			metricUnitPO.SetText("");
			process.SetText("");
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null && result !== undefined) {
				process.SetText(result.process);
				metricUnitPO.SetText(result.metricUnit);
				//s.SetEnabled(id_itemIniAux === null || id_itemIniAux === 0 || (! result.hasOrderDetail));
			}
			else {
				metricUnitPO.SetText("");
				process.SetText("");
			}
		},
		complete: function () {
			//hideLoading();
			id_purchaseOrder.PerformCallback();
		}
	});

	ItemCombo_OnInit(s, e);
}

function ItemCombo_OnInit(s, e) {
	//store actual filtering method and override
	var actualFilteringOnClient = s.filterStrategy.FilteringOnClient;
	s.filterStrategy.FilteringOnClient = function () {
		//create a new format string for all list box columns. you could skip this bit and just set
		//filterTextFormatString to whatever you wanted for instance "{0} {2}" would only filter on
		//columns 1 and 3
		var lb = this.GetListBoxControl();
		var filterTextFormatStringItems = [];
		for (var i = 0; i < lb.columnFieldNames.length; i++) {
			filterTextFormatStringItems.push('{' + i + '}');
		}
		var filterTextFormatString = filterTextFormatStringItems.join(' ');

		//store actual format string and override with one for all columns
		var actualTextFormatString = lb.textFormatString;
		lb.textFormatString = filterTextFormatString;

		//call actual filtering method which will now work on our temporary format string
		actualFilteringOnClient.apply(this);

		//restore original format string
		lb.textFormatString = actualTextFormatString;
	};
}


function UpdateWarehouseLocations(warehouseLocations) {

	for (var i = 0; i < id_warehouseLocation.GetItemCount(); i++) {
		var warehouseLocation = id_warehouseLocation.GetItem(i);
		var into = false;
		for (var j = 0; j < warehouseLocations.length; j++) {
			if (warehouseLocation.value == warehouseLocations[j].id) {
				into = true;
				break;
			}
		}
		if (!into) {
			id_warehouseLocation.RemoveItem(i);
			i -= 1;
		}
	}


	for (var i = 0; i < warehouseLocations.length; i++) {
		var warehouseLocation = id_warehouseLocation.FindItemByValue(warehouseLocations[i].id);
		if (warehouseLocation == null) id_warehouseLocation.AddItem(warehouseLocations[i].name, warehouseLocations[i].id);
	}

}

function ComboWarehouse_SelectedIndexChanged(s, e) {

	id_warehouseLocation.SetValue(null);
	id_warehouseLocation.ClearItems();
	var data = {
		id_warehouse: s.GetValue()//,
	};

	$.ajax({
		url: "ProductionLotReception/UpdateProductionLotReceptionWarehouseLocation",
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			//id_priceList.ClearItems();
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null && result !== undefined) {
				var arrayFieldStr = [];
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_warehouseLocation, result.warehouseLocations, arrayFieldStr);
				//UpdateWarehouseLocations(result.warehouseLocations);
			}
			//else {
			//    id_priceList.ClearItems();
			//}
		},
		complete: function () {
			//hideLoading();
			ValidateDetailMP();
		}
	});
}

function ComboWarehouseLocation_Init(s, e) {
	var data = {
		id_warehouse: id_warehouse.GetValue(),
		id_warehouseLocation: s.GetValue()//,
	};
	$.ajax({
		url: "ProductionLotReception/GetProductionLotReceptionWarehouseLocation",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			var arrayFieldStr = [];
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_warehouseLocation, [], arrayFieldStr);
			//UpdateWarehouseLocations([]);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			var arrayFieldStr = [];
			arrayFieldStr.push("name");
			UpdateDetailObjects(id_warehouseLocation, result.warehouseLocations, arrayFieldStr);
			//UpdateWarehouseLocations(result.warehouseLocations);
		},
		complete: function () {
			//hideLoading();
		}
	});
}


// EDITOR'S EVENTS
function OnGridViewItemDetailsInit(s, e) {
	UpdateTitlePanelItemDetails();
}

function UpdateTitlePanelItemDetails() {

	//if (gv === null || gv === undefined)
	//    return;

	var selectedFilteredRowCount = GetSelectedFilteredRowCountItemDetails();

	var text = "Total de elementos seleccionados: <b>" + gvProductionLotReceptionEditFormItemsDetail.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gvProductionLotReceptionEditFormItemsDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountItemDetails();
	if (hiddenSelectedRowCount > 0)
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	text += "<br />";


	$("#lblInfoItems").html(text);

	if ($("#selectAllMode").val() !== "AllPages") {
		SetElementVisibility("lnkSelectAllRowsItems", gvProductionLotReceptionEditFormItemsDetail.GetSelectedRowCount() > 0 && gvProductionLotReceptionEditFormItemsDetail.cpVisibleRowCount > selectedFilteredRowCount);
		SetElementVisibility("lnkClearSelectionItems", gvProductionLotReceptionEditFormItemsDetail.GetSelectedRowCount() > 0);
	}

	btnRemoveDetail.SetEnabled(gvProductionLotReceptionEditFormItemsDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountItemDetails() {
	return gvProductionLotReceptionEditFormItemsDetail.cpFilteredRowCountWithoutPage +
		gvProductionLotReceptionEditFormItemsDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewItemsDetailSelectionChanged(s, e) {
	UpdateTitlePanelItemDetails();
	s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackItemsDetail);

}

function GetSelectedFieldValuesCallbackItemsDetail(values) {
	var selectedRows = [];
	for (var i = 0; i < values.length; i++) {
		selectedRows.push(values[i]);
	}
}

var customCommand = "";

function OnGridViewItemDetailsBeginCallback(s, e) {
	//// 

	customCommand = e.command;
}

function OnGridViewItemDetailsEndCallback(s, e) {
	UpdateTitlePanelItemDetails();
	//if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
	//    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
	//}
	//if (gv !== gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail) {
	//    if (gv !== gvProductionLotReceptionEditFormItemsDetail) {
	//        if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
	//            s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
	//        }
	//    } else {
	//        if (s.GetEditor("id") !== null && s.GetEditor("id") !== undefined) {
	//            s.GetEditor("id").SetEnabled(customCommand === "ADDNEWROW");
	//        }
	//    }

	//} else {
	//    if (s.GetEditor("id_qualityAnalysis") !== null && s.GetEditor("id_qualityAnalysis") !== undefined) {
	//        s.GetEditor("id_qualityAnalysis").SetEnabled(customCommand === "ADDNEWROW");
	//    }
	//}

	UpdateProductionLotTotals();

}

function gvEditItemDetailsClearSelection() {
	gvProductionLotReceptionEditFormItemsDetail.UnselectRows();
}

function gvEditItemDetailsSelectAllRows() {
	gvProductionLotReceptionEditFormItemsDetail.SelectRows();
}

function GridViewItemsCustomCommandButton_Click(s, e) {

	$("#GridMessageErrorsDetail").hide();
	//document.getElementById("gridMessageErrorsDetail").style.visibility = "hidden";

	if (e.buttonID === "btnQualityControlRow") {
		var data = "idPlQc=" + gvProductionLotReceptionEditFormItemsDetail.GetRowKey(e.visibleIndex) + "&" + $("#formEditProductionLotReception").serialize();
		showLoading();
		$.ajax({
			url: "ProductionLot/UpdateProductionLot",
			type: "post",
			data: data,
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				//showLoading();
			},
			success: function (result) {
				// 
				hideLoading();
				if (result !== null) {
					if (result.hasProdQuality == "YES") {
						var data2 = {
							id: 0,
							loteManual: false,
							id_productionLotDetail: gvProductionLotReceptionEditFormItemsDetail.GetRowKey(e.visibleIndex),
							urlToReturn: "ProductionLotReception/ProductionLotReceptionFormEditPartial",
							tabSelected: 2,//Seleccionado el tab 3 de Matreria Prima
							arrayTempDataKeep: ["productionLot"],
							hasUpdate: false
						};
						//console.log("data2.id: " + data2.id);
						//console.log("data2: " + data2);
						showPagefromLink("QualityControl/FormEditQualityControl", data2);
						//Mostrar Vista de Gestion de Calidad pasandole el id_QualityControl que estaria en una propiedad del grid
					}
					else {
						//document.getElementById("gridMessageErrorsDetail").style.visibility = "visible";
						gridMessageErrorsDetail.SetText(ErrorMessage("Este detalle no tiene calidad."));
						//document.getElementById('GridMessageErrorsDetail').style.display = ''
						$("#GridMessageErrorsDetail").show();

					}
				}
			},
			complete: function () {
				//hideLoading();
			}
		});

	}
}