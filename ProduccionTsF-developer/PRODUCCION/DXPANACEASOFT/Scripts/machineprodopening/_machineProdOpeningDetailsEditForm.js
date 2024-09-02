
//DETAILS ACTIONS BUTTONS

function AddNewDetail(s, e) {
	//if (gv !== null && gv !== undefined) {
	//    gv.AddNewRow();
	//}
	gvMachineProdOpeningDetailEditForm.AddNewRow();
}

function RemoveDetail(s, e) {
}

function RefreshDetail(s, e) {
	//if (gv !== null && gv !== undefined) {
	//    gv.UnselectRows();
	//    gv.PerformCallback();
	//}
	gvMachineProdOpeningDetailEditForm.UnselectRows();
	gvMachineProdOpeningDetailEditForm.PerformCallback();
}




var errorMessage = "";
var runningValidation = false;

var id_machineForProdIniAux = null;
var id_personIniAux = null;


// VALIDATIONS

function OnNameMachineProdOpeningDetailValidation(s, e) {

	errorMessage = "";

	$("#GridMessageErrorsDetail").hide();

	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Máquina: Es obligatorio.";
		} else {
			errorMessage += "</br>- Máquina: Es obligatorio.";
		}
	} else {
		var data = {
			id_machineForProdNew: s.GetValue()
		};
        if (data.id_machineForProdNew != id_machineForProdIniAux) {
            $.ajax({
                url: "MachineProdOpening/ItsRepeatedDetail",
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
                        if (result.itsRepeated == 1) {
                            e.isValid = false;
                            e.errorText = result.Error;
                            if (errorMessage == null || errorMessage == "") {
                                errorMessage = "- Máquina: " + result.Error;
                            } else {
                                errorMessage += "</br>- Máquina: " + result.Error;
                            }
                        }
                        //else {
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
        } else {
            $.ajax({
                url: "MachineProdOpening/ItsAvailable",
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
                        if (result.itsAvailable == 1) {
                            e.isValid = false;
                            e.errorText = result.Error;
                            if (errorMessage == null || errorMessage == "") {
                                errorMessage = "- Máquina: " + result.Error;
                            } else {
                                errorMessage += "</br>- Máquina: " + result.Error;
                            }
                        }
                    }
                },
                complete: function () {
                    //hideLoading();
                }
            });
        }
	}
}

function OnPersonMachineProdOpeningDetailValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Responsable: Es obligatorio.";
		} else {
			errorMessage += "</br>- Responsable: Es obligatorio.";
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}

}

function OnNumPersonDetailValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- No. Persona: Es obligatorio.";
		} else {
			errorMessage += "</br>- No. Persona: Es obligatorio.";
		}
	}

	//if (!runningValidation) {
	//    ValidateDetail();
	//}

}


function OnTimeInitDetailValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Hora Inicio: Es obligatoria.";
		} else {
			errorMessage += "</br>- Hora Inicio: Es obligatoria.";
		}
	} else {
		//e.isValid = IsValidateTimeInRange(e, timeInitTurn.GetValue(), timeEndTurn.GetValue(), timeInitDetail.GetValue(), "Hora de Inicio debe estar dentro del Horario del Turno.")
		e.isValid = IsValidateTimeInRange(e, timeInitTurn.GetValue(), timeEndTurn.GetValue(), timeInitDetail.GetValue(), "Hora de Inicio debe estar dentro del Horario del Turno.")
		//console.log("valido1: " + valido1);
		//console.log("e.isValid: " + e.isValid);
		if (!e.isValid) {
			if (errorMessage == null || errorMessage == "") {
				errorMessage = "- Hora Inicio: Debe estar dentro del Horario del Turno.";
			} else {
				errorMessage += "</br>-Hora Inicio: Debe estar dentro del Horario del Turno.";
			}
		}
		//e.errorText = "Hora de Inicio debe estar dentro del Horario del Turno.";
		//OnRangeTimeValidation(e, timeInitTurn.GetValue(), timeInitDetail.GetValue(), "Hora de Inicio debe ser mayor o igual a la Hora de Inicio del Turno.");
		//if (!e.isValid) {
		//    //if (errorMessage == null || errorMessage == "") {
		//    //    errorMessage = "- Hora Inicio: Debe ser mayor o igual a la Hora de Inicio del Turno.";
		//    //} else {
		//    //    errorMessage += "</br>- Hora Inicio: Debe ser mayor o igual a la Hora de Inicio del Turno.";
		//    //}
		//    OnRangeTimeValidation(e, timeInitDetail.GetValue(), timeEndTurn.GetValue(), "Hora de Inicio debe ser menor a la Hora de Fin del Turno.", false);
		//    if (!e.isValid) {
		//        if (errorMessage == null || errorMessage == "") {
		//            errorMessage = "- Hora Inicio: Debe ser mayor o igual a la Hora de Inicio del Turno o menor a la Hora de Fin del Turno";
		//        } else {
		//            errorMessage += "</br>- Hora Inicio: Debe ser mayor o igual a la Hora de Inicio del Turno o menor a la Hora de Fin del Turno.";
		//        }
		//    }
		//} else {

		//}
	}

	if (!runningValidation) {
		ValidateDetail();
	}

	if (errorMessage != null && errorMessage != "") {
		var msgErrorAux = ErrorMessage(errorMessage);
		gridMessageErrorsDetail.SetText(msgErrorAux);
		$("#GridMessageErrorsDetail").show();

	}

}

//function OnCodeMachineProdOpeningDetailValidation(s, e) {
//    if (errorMessage != null && errorMessage != "") {
//        e.isValid = false;
//        e.errorText = "";
//    } 

//    if (!runningValidation) {
//        ValidateDetail();
//    }

//    if (errorMessage != null && errorMessage != "") {
//        var msgErrorAux = ErrorMessage(errorMessage);
//        gridMessageErrorsDetail.SetText(msgErrorAux);
//        $("#GridMessageErrorsDetail").show();

//    }
//}



function ValidateDetail() {
	runningValidation = true;
	OnNameMachineProdOpeningDetailValidation(nameMachineForProd, nameMachineForProd);
	OnPersonMachineProdOpeningDetailValidation(personMachineProdOpening, personMachineProdOpening);
	OnTimeInitDetailValidation(timeInitDetail, timeInitDetail);
	//OnNumPersonDetailValidation(numPerson, numPerson);
	//OnCodeMachineProdOpeningDetailValidation(codeMachineForProd, codeMachineForProd);
	runningValidation = false;
}

function TimeInitCombo_ValueChanged(s, e) {
	timeEndDetail.SetValue(timeInitDetail.GetValue());
}


function TimeInitCombo_Init(s, e) {
	//var auxCP = gvMachineProdOpeningDetailEditForm.cpTimeInit;
	//console.log("gvMachineProdOpeningDetailEditForm.cpTimeInit: " + gvMachineProdOpeningDetailEditForm.cpTimeInit);
	timeInitDetail.SetValue(new Date("2011-01-01T" + gvMachineProdOpeningDetailEditForm.cpTimeInit));
}

function NameMachineProdOpeningDetailDetailCombo_SelectedIndexChanged(s, e) {

	codeMachineForProd.SetText("");

	//if (id_itemMaterial != null) {

	$.ajax({
		url: "MachineProdOpening/NameMachineProdOpeningDetailDetailCombo_SelectedIndexChanged",
		type: "post",
		data: { id_MachineForProd: nameMachineForProd.GetValue() },
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
				codeMachineForProd.SetText(result.codeMachineForProd);
				id_personIniAux = null;
				personMachineProdOpening.PerformCallback();
			}
		},
		complete: function () {
			//hideLoading();
		}
	});
	//}
}

function NameMachineProdOpeningDetailDetailCombo_OnInit(s, e) {
	id_machineForProdIniAux = nameMachineForProd.GetValue();
	id_personIniAux = personMachineProdOpening.GetValue();
	personMachineProdOpening.PerformCallback();
}

function PersonMachineProdOpening_BeginCallback(s, e) {
	e.customArgs["id_MachineForProd"] = nameMachineForProd.GetValue();
}

function PersonMachineProdOpening_EndCallback(s, e) {

	if (id_personIniAux === null) {
		if (personMachineProdOpening.GetItemCount() == 1) {
			var aPersonMachineProdOpening = personMachineProdOpening.GetItem(0);
			personMachineProdOpening.SetValue(aPersonMachineProdOpening.value);
		}
	} else {
		personMachineProdOpening.SetValue(id_personIniAux);
	}


	//for (var i = 0; i < id_warehouseLocationMaterial.GetItemCount(); i++) {
	//    var warehouseLocation = id_warehouseLocationMaterial.GetItem(i);
	//    var into = false;
	//    for (var j = 0; j < warehouseLocations.length; j++) {
	//        if (warehouseLocation.value == warehouseLocations[j].id) {
	//            into = true;
	//            break;
	//        }
	//    }
	//    if (!into) {
	//        id_warehouseLocationMaterial.RemoveItem(i);
	//        i -= 1;
	//    }
	//}


	//for (var i = 0; i < warehouseLocations.length; i++) {
	//    var warehouseLocation = id_warehouseLocationMaterial.FindItemByValue(warehouseLocations[i].id);
	//    if (warehouseLocation == null) id_warehouseLocationMaterial.AddItem(warehouseLocations[i].name, warehouseLocations[i].id);
	//}

}

// EDITOR'S EVENTS

function UpdateMaterialsDetailInfo(data, s, e) {

	if (data.id_item === null) {
		return;
	}

	//purchaseOrderNumber.SetText("");
	//remissionGuideNumber.SetText("");
	metricUnitMaterial.SetText("");
	//gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnitMaterial").SetText("");
	//gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(null);// 
	//gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(null);// SetValue("");

	if (id_itemMaterial != null) {

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
					metricUnitMaterial.SetText(result.metricUnit);
					//gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnit").SetText(result.metricUnit);
					//gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);
					//gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
				}
			},
			complete: function () {
				//hideLoading();
			}
		});
	}
}

function ItemMaterialsDetailCombo_SelectedIndexChanged(s, e) {
	UpdateMaterialsDetailInfo({
		id_item: s.GetValue()
	}, s, e);
}

function ItemMaterialsDetailCombo_DropDown(s, e) {

	$.ajax({
		url: "ProductionLotReception/ProductionLotReceptionMaterialDetails",
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
			for (var i = 0; i < id_itemMaterial.GetItemCount(); i++) {
				var item = id_itemMaterial.GetItem(i);
				if (result.indexOf(item.value) >= 0) {
					id_itemMaterial.RemoveItem(i);
					i = -1;
				}
			}
		},
		complete: function () {
			//hideLoading();
		}
	});
}

function ComboItem_Init(s, e) {
	var data = {
		id_item: s.GetValue()
	};

	$.ajax({
		url: "ProductionLotReception/ProductionLotReceptionItemData",
		type: "post",
		data: data,
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
			metricUnitMaterial.SetText("");
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null && result !== undefined) {
				metricUnitMaterial.SetText(result.metricUnit);
			}
			else {
				metricUnitMaterial.SetText("");
			}
		},
		complete: function () {
			//hideLoading();
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

	for (var i = 0; i < id_warehouseLocationMaterial.GetItemCount(); i++) {
		var warehouseLocation = id_warehouseLocationMaterial.GetItem(i);
		var into = false;
		for (var j = 0; j < warehouseLocations.length; j++) {
			if (warehouseLocation.value == warehouseLocations[j].id) {
				into = true;
				break;
			}
		}
		if (!into) {
			id_warehouseLocationMaterial.RemoveItem(i);
			i -= 1;
		}
	}


	for (var i = 0; i < warehouseLocations.length; i++) {
		var warehouseLocation = id_warehouseLocationMaterial.FindItemByValue(warehouseLocations[i].id);
		if (warehouseLocation == null) id_warehouseLocationMaterial.AddItem(warehouseLocations[i].name, warehouseLocations[i].id);
	}

}

//gvMachineProdOpeningDetailEditForm

function OnGridViewMachineProdOpeningDetailEditForm_BeginCallback(s, e) {
	e.customArgs["errorMessage"] = errorMessage;
	//var timeInitDetailAux = null;
	//try {
	//    var timeInitDetailAux = timeInitDetail == undefined || timeInitDetail === undefined ? null : timeInitDetail.GetValue();

	//} catch (e) {
	//    timeInitDetailAux = null;
	//}
	//console.log("timeInitDetailAux: " + timeInitDetailAux);
	//var timeInitDetailStr = timeInitDetailAux != null && timeInitDetailAux != "" ? timeInitDetailAux.getHours() + ":" + timeInitDetailAux.getMinutes() + ":" + timeInitDetailAux.getSeconds() : "00:00:00";
	//e.customArgs["timeInitDetail"] = timeInitDetailStr;
}

//function WarehouseLocationReceptionDispatchMaterialsDetailCombo_OnInit(s, e) {


//}


function ItemReceptionDispatchMaterialsDetailCombo_SelectedIndexChanged(data, s, e) {

	itemReceptionDispatchMaterialsDetailMasterCode.SetText("");
	itemReceptionDispatchMaterialsDetailMetricUnit.SetText("");

	//if (id_itemMaterial != null) {

	$.ajax({
		url: "ReceptionDispatchMaterials/ItemDetailData",
		type: "post",
		data: { id_item: itemReceptionDispatchMaterialsDetailName.GetValue() },
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
				itemReceptionDispatchMaterialsDetailMasterCode.SetText(result.masterCode);
				itemReceptionDispatchMaterialsDetailMetricUnit.SetText(result.metricUnit);
			}
		},
		complete: function () {
			//hideLoading();
		}
	});
	//}
}



function WarehouseReceptionDispatchMaterialsDetailCombo_SelectedIndexChanged(s, e) {

	id_warehouseLocationReceptionDispatchMaterialsDetail.SetValue(null);
	//id_warehouseLocationMaterial.ClearItems();
	id_warehouseAux = id_warehouseReceptionDispatchMaterialsDetail.GetValue();
	id_warehouseLocationAux = null;
	id_warehouseLocationReceptionDispatchMaterialsDetail.PerformCallback();

}

function WarehouseLocationReceptionDispatchMaterialsDetailCombo_OnInit(s, e) {
	id_itemIniAux = itemReceptionDispatchMaterialsDetailName.GetValue();
	id_warehouseIniAux = id_warehouseReceptionDispatchMaterialsDetail.GetValue();
	id_warehouseLocationIniAux = id_warehouseLocationReceptionDispatchMaterialsDetail.GetValue();
	//var data = {
	//    id_warehouse: id_warehouseMaterial.GetValue(),
	//    id_warehouseLocation: s.GetValue()//,
	//};

	id_warehouseAux = id_warehouseReceptionDispatchMaterialsDetail.GetValue();
	id_warehouseLocationAux = id_warehouseLocationReceptionDispatchMaterialsDetail.GetValue();
	if (id_warehouseAux != null && id_warehouseAux !== null && id_warehouseAux != 0) {
		id_warehouseLocationReceptionDispatchMaterialsDetail.PerformCallback();
	} else {
		$.ajax({
			url: "ReceptionDispatchMaterials/WarehouseLocationReceptionDispatchMaterialsDetailCombo_OnInit",
			type: "post",
			data: null,
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
				//var arrayFieldStr = [];
				//arrayFieldStr.push("name");
				//UpdateDetailObjects(id_warehouseLocationMaterial, [], arrayFieldStr);
				//UpdateWarehouseLocations([]);
			},
			beforeSend: function () {
				//showLoading();
			},
			success: function (result) {
				//var arrayFieldStr = [];
				//arrayFieldStr.push("name");
				//UpdateDetailObjects(id_warehouseLocationMaterial, result.warehouseLocations, arrayFieldStr);
				//UpdateWarehouseLocations(result.warehouseLocations);
				id_warehouseAux = result.id_warehouse;
				id_warehouseLocationAux = result.id_warehouseLocation;
				id_warehouseLocationReceptionDispatchMaterialsDetail.PerformCallback();
			},
			complete: function () {
				//hideLoading();
			}
		});
	}

}

function WarehouseLocationReceptionDispatchMaterialsDetail_BeginCallback(s, e) {
	e.customArgs["id_warehouseCurrent"] = id_warehouseAux;
}

function WarehouseLocationReceptionDispatchMaterialsDetail_EndCallback(s, e) {
	id_warehouseReceptionDispatchMaterialsDetail.SetValue(id_warehouseAux);
	id_warehouseLocationReceptionDispatchMaterialsDetail.SetValue(id_warehouseLocationAux);
}

// EDITOR'S EVENTS

function UpdateTitlePanelDetail() {
	var gv = gvInvoiceCommercialEditFormDetail;

	var selectedFilteredRowCount = GetSelectedFilteredRowCount();

	var text = "Total de elementos seleccionados: <b>" + gv.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gv.GetSelectedRowCount() - GetSelectedFilteredRowCount();
	if (hiddenSelectedRowCount > 0)
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	text += "<br />";

	$("#lblInfoDetails").html(text);

	if ($("#selectAllMode").val() !== "AllPages") {
		SetElementVisibility("lnkSelectAllRowsDetails", gv.GetSelectedRowCount() > 0 && gv.cpVisibleRowCount > selectedFilteredRowCount);
		SetElementVisibility("lnkClearSelectionDetails", gv.GetSelectedRowCount() > 0);
	}
}

function GetSelectedFilteredRowCount() {
	return gvInvoiceCommercialEditFormDetail.cpFilteredRowCountWithoutPage + gvInvoiceCommercialEditFormDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewInitDetail(s, e) {
	UpdateTitlePanelDetail();
}

function OnGridViewSelectionChangedDetail(s, e) {
	UpdateTitlePanelDetail();

	gvInvoiceCommercialEditFormDetail.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackDetail);

}

function GetSelectedFieldValuesCallbackDetail(values) {
	var selectedRows = [];
	for (var i = 0; i < values.length; i++) {
		selectedRows.push(values[i]);
	}
}

var customCommand = "";

function OnGridViewBeginCallbackDetail(s, e) {
	customCommand = e.command;
}

function OnGridViewEndCallbackDetail(s, e) {
	UpdateTitlePanelDetail();

}

function gvEditClearSelectionDetail() {
	gvInvoiceCommercialEditFormDetail.UnselectRows();
}

function gvEditSelectAllRowsDetail() {
	gvInvoiceCommercialEditFormDetail.SelectRows();
}

//gvInvoiceCommercialEditFormDetail

var itemCurrentAux = null;

function ItemComboBox_Init(s, e) {
	itemCurrentAux = s.GetValue();
	s.PerformCallback();

}

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

function ItemComboBox_SelectedIndexChanged(s, e) {

	itemInvoiceCommercialAuxCode.SetText("");
	itemInvoiceCommercialMasterCode.SetText("");
	amountInvoice.SetValue(null);
	//itemInvoiceCommercialUM.SetText("");
	invoiceCommercialTotal.SetValue(null);

	var unitPriceAux = unitPrice.GetValue();
	var strUnitPrice = unitPriceAux == null ? "0" : unitPriceAux.toString();
	var resUnitPrice = strUnitPrice.replace(".", ",");

	var numBoxesAux = numBoxes.GetValue();
	var strNumBoxes = numBoxesAux == null ? "0" : numBoxesAux.toString();
	var resNumBoxes = strNumBoxes.replace(".", ",");

	UpdateInvoiceCommercialDetail(s.GetValue(), resNumBoxes, resUnitPrice);

}

function UpdateInvoiceCommercialDetail(id_itemCurrent, numBoxesCurrent, unitPriceCurrent) {
	var data = {
		id_itemCurrent: id_itemCurrent,
		numBoxesCurrent: numBoxesCurrent,
		unitPriceCurrent: unitPriceCurrent
	};

	$.ajax({
		url: "InvoiceCommercial/UpdateInvoiceCommercialDetail",
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
				//UpdateOpeningClosingPlateLyingMaintenanceWarehouseLocations(result.warehouseLocations);
				//itemInvoiceCommercialAuxCode, itemInvoiceCommercialMasterCode, amount, itemInvoiceCommercialUM, invoiceCommercialTotal
				itemInvoiceCommercialAuxCode.SetText(result.itemInvoiceCommercialAuxCode);
				itemInvoiceCommercialMasterCode.SetText(result.itemInvoiceCommercialMasterCode);

				amountInvoice.SetValue(result.amountInvoice);

				//itemInvoiceCommercialUM.SetText(result.itemInvoiceCommercialUM);

				invoiceCommercialTotal.SetValue(result.invoiceCommercialTotal);

			}
			//else {
			//    id_priceList.ClearItems();
			//}
		},
		complete: function () {
			//hideLoading();
		}
	});
}

function NumBoxes_NumberChange(s, e) {

	//itemInvoiceCommercialAuxCode.SetText("");
	//itemInvoiceCommercialMasterCode.SetText("");
	//amountInvoice.SetValue(null);
	//itemInvoiceCommercialUM.SetText("");
	//invoiceCommercialTotal.SetValue(null);

	var unitPriceAux = unitPrice.GetValue();
	var strUnitPrice = unitPriceAux == null ? "0" : unitPriceAux.toString();
	var resUnitPrice = strUnitPrice.replace(".", ",");

	var numBoxesAux = s.GetValue();
	var strNumBoxes = numBoxesAux == null ? "0" : numBoxesAux.toString();
	var resNumBoxes = strNumBoxes.replace(".", ",");

	UpdateInvoiceCommercialDetail(id_item.GetValue(), resNumBoxes, resUnitPrice);

}

function UnitPrice_NumberChange(s, e) {

	//itemInvoiceCommercialAuxCode.SetText("");
	//itemInvoiceCommercialMasterCode.SetText("");
	//amountInvoice.SetValue(null);
	//itemInvoiceCommercialUM.SetText("");
	//invoiceCommercialTotal.SetValue(null);

	var unitPriceAux = s.GetValue();
	var strUnitPrice = unitPriceAux == null ? "0" : unitPriceAux.toString();
	var resUnitPrice = strUnitPrice.replace(".", ",");

	var numBoxesAux = numBoxes.GetValue();
	var strNumBoxes = numBoxesAux == null ? "0" : numBoxesAux.toString();
	var resNumBoxes = strNumBoxes.replace(".", ",");

	UpdateInvoiceCommercialDetail(id_item.GetValue(), resNumBoxes, resUnitPrice);

}

function ButtonUpdateItemDetail_Click(s, e) {
	var valid = true;
	var validFormLayoutEditInvoiceCommercialDetail = ASPxClientEdit.ValidateEditorsInContainerById("formLayoutEditInvoiceCommercialDetail", null, true);

	if (validFormLayoutEditInvoiceCommercialDetail) {
		//UpdateTabImage({ isValid: true }, "tabDocument");
	} else {
		//UpdateTabImage({ isValid: false }, "tabDocument");
		valid = false;
	}

	if (valid) {
		gvInvoiceCommercialEditFormDetail.UpdateEdit();
	}
	//var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);

	//if (!valid) {
	//    UpdateTabImage({ isValid: false }, "tabOportunity");
	//}



	//if (valid) {
	//    var id = $("#id_businessOportunity").val();
	//    var data = "id=" + id + "&" + $("#formEditBusinessOportunity").serialize();
	//    var url = (id === "0") ? "BusinessOportunity/BussinesOportunityPartialAddNew"
	//                           : "BusinessOportunity/BussinesOportunityPartialUpdate";
	//    showForm(url, data);
	//}


}

function BtnCancelItemDetail_Click(s, e) {
	//$("#GridMessageErrorPhase").hide();
	gvInvoiceCommercialEditFormDetail.CancelEdit();

	//showPage("BusinessOportunity/Index", null);
}