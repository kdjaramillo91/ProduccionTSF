function Update(approve) {
	emissionDate.SetEnabled(true);

	var valid = true;
	var validDocumentCut = ASPxClientEdit.ValidateEditorsInContainerById("documentCut", null, true);
	var validMainTabInventoryMove = ASPxClientEdit.ValidateEditorsInContainerById("mainTabInventoryMove", null, true);

	if (validDocumentCut) {
		UpdateTabImage({ isValid: true }, "tabDocument");
	} else {
		UpdateTabImage({ isValid: false }, "tabDocument");
		valid = false;
	}

	if (validMainTabInventoryMove) {
		UpdateTabImage({ isValid: true }, "tabInventoryMove");
	} else {
		UpdateTabImage({ isValid: false }, "tabInventoryMove");
		valid = false;
	}

	if (gridViewMoveDetails.cpRowsCount === 0 || gridViewMoveDetails.IsEditing()) {
		UpdateTabImage({ isValid: false }, "tabDetails");
		valid = false;
	} else {
		UpdateTabImage({ isValid: true }, "tabDetails");
	}

	if (valid) {
		var id = $("#id_inventoryMove").val();
		var codeDocumentType = $("#codeDocumentType").val();
		var natureMoveIMTmp = $("#natureMoveIM").val();
		var customParamOPTmp = $("#customParamOP").val();
		var data = "";
		if (customParamOPTmp === "IPXM") {
			data = "id=" + id + "&"
				+ "approve=" + approve + "&"
				+ "codeDocumentType=" + codeDocumentType + "&"
				+ "natureMoveIMTmp=" + natureMoveIMTmp + "&"
				+ "multiple=" + false + "&"
				+ $("#formEditInventoryMove").serialize()
				+ "&id_productionLot=" + idLoteFilter.GetValue() + "&";
			+ "&customParamOP=" + customParamOPTmp + "&";

		}
		else {
			data = "id=" + id + "&"
				+ "approve=" + approve + "&"
				+ "codeDocumentType=" + codeDocumentType + "&"
				+ "natureMoveIMTmp=" + natureMoveIMTmp + "&"
				+ "multiple=" + false + "&"
				+ $("#formEditInventoryMove").serialize();
		}


		var url = (id === "0") ? "InventoryMove/InventoryMovesPartialAddNew"
			: "InventoryMove/InventoryMovesPartialUpdate";
		showForm(url, data);
	}
	else {
		emissionDate.SetEnabled(false);
	}
}

function ButtonUpdate_Click(s, e) {

	Update(false);

}

function ButtonClose_Click(s, e) {
	var codeDocumentType = $("#codeDocumentType").val();
	var natureMoveIMTmp = $("#natureMoveIM").val();
	var customParamOPTmp = $("#customParamOP").val();
	var tranferMove = $("#tranferMove").val();

	if (codeDocumentType == "04") {    
		showPage("InventoryMove/IndexEntryMovePurchaseOrder", null);

	} else {
		if (codeDocumentType == "03") {
			showPage("InventoryMove/IndexEntryMove", null);

		} else {
			if (codeDocumentType == "05") {
				showPage("InventoryMove/IndexExitMove", null);

			} else {
				if (codeDocumentType == "32") {  
					showPage("InventoryMove/IndexTransferExitMove", null);

				} else {
					if (codeDocumentType == "34") {  
						showPage("InventoryMove/IndexTransferEntryMove", null);

					} else {
						if (codeDocumentType == "129") {      
							showPage("InventoryMove/IndexTransferExitAutomaticMove", null);
						} else {
							if (customParamOPTmp === "IPXM") {
								showPage("InventoryMove/IndexExitMovePackagingMaterials", null);
							} else {
								if (natureMoveIMTmp == "I") {
									if (tranferMove === true || tranferMove === "true" || tranferMove === "True") {
										showPage("InventoryMove/IndexTransferEntryMove", null);
									} else {
										showPage("InventoryMove/IndexEntryMove", null);
									}
								} else {
									if (natureMoveIMTmp == "E") {
										if (tranferMove === true || tranferMove === "true" || tranferMove === "True") {
											showPage("InventoryMove/IndexTransferExitMove", null);
										} else {
											showPage("InventoryMove/IndexExitMove", null);
										}
									} else {
										showPage("InventoryMove/Index", null);
									}
								}
							}
						}
					}
				}
			}
		}
	}
}

function CallBackGridCC() {
    if (gridViewMoveDetails.cpVisibleRowCount === 0) return;

    var _idWarehouse = (idWarehouse.GetValue() === null) ? 0 : idWarehouse.GetValue();
    var _idWarehouseLocation = (idWarehouseLocation.GetValue() === null) ? 0 : idWarehouseLocation.GetValue();
    var _id_costCenter = (id_costCenter.GetValue() === null) ? 0 : id_costCenter.GetValue();
    var _id_subCostCenter = (id_subCostCenter.GetValue() === null) ? 0 : id_subCostCenter.GetValue();

    var data =
    {
        code: $("#codeDocumentType").val(),
		idWarehouse: _idWarehouse,
		idWarehouseLocation: _idWarehouseLocation,
        id_costCenter: _id_costCenter,
        id_subCostCenter: _id_subCostCenter,
        deletedAll: false
    }

    gridViewMoveDetails.PerformCallback(data);
}


function CallBackGridCCEntry() {
    if (gridViewMoveDetails.cpVisibleRowCount === 0) return;

    var _idWarehouse = (idWarehouse.GetValue() === null) ? 0 : idWarehouse.GetValue();
    var _id_costCenter = (id_costCenter.GetValue() === null) ? 0 : id_costCenter.GetValue();
    var _id_subCostCenter = (id_subCostCenterEntry.GetValue() === null) ? 0 : id_subCostCenterEntry.GetValue();

    var data =
    {
        code: $("#codeDocumentType").val(),
        idWarehouse: _idWarehouse,
        id_costCenter: _id_costCenter,
        id_subCostCenter: _id_subCostCenter,
        deletedAll: false
    }
    gridViewMoveDetails.PerformCallback(data);
}

function OnReasonInit() {
	if ($("#mostrarOP").val() === false || $("#mostrarOP").val() === "false" || $("#mostrarOP").val() === "False" || $("#mostrarOP").val() === null || $("#mostrarOP").val() === "") {
        ShowHideElemetOP(false);
    } else {
		ShowHideElemetOP(true);
    }
}

function ShowHideElemetOP(mostrarOPVal) {
	if (mostrarOPVal === true) {
		$("#id_customerLabel").show();
		$("#id_customer").show();
		$("#id_sellerLabel").show();
		$("#id_seller").show();
		$("#noFacturaLabel").show();
		$("#id_Invoice").show();
		$("#noFactura").show();
		$("#contenedorLabel").show();
		$("#contenedor").show();
		$("#numberRemGuide_I").prop("readonly", false);
		id_customer.PerformCallback();
	} else {
		$("#id_sellerLabel").hide();
		$("#id_seller").hide();
		$("#noFacturaLabel").hide();
		$("#id_Invoice").hide();
		$("#noFactura").hide();
		$("#contenedorLabel").hide();
		$("#contenedor").hide();
		$("#numberRemGuide_I").val('');
		$("#numberRemGuide_I").prop("readonly", true);
	}
}

function EnableDisableRemissionGuide() {
	if (mostrarOPVal === false) {
		$("#numberRemGuide").SetEnabled = true;
	}
}

function OnSelectedInventoryReasonChanged(s, e) {
	if (s.GetValue() !== null) {
		$.ajax({
			url: "InventoryMove/InventoryReasonChanged",
			type: "post",
			data: { idIR: s.GetValue() },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					if (result.codeReason === "SI") { $("#id_provider").show(); } else { $("#id_provider").hide(); }
					if (result.codeReason === "SI") { $("#id_providerLabel").show(); } else { $("#id_providerLabel").hide(); }

					if (result.requiereSystemLotNubmber !== null) $("#withLotSystem").val(result.requiereSystemLotNubmber);
					if (result.requiereUserLotNubmber !== null) $("#withLotCustomer").val(result.requiereUserLotNubmber);
					if (result.op !== null) {
						$("#mostrarOP").val(result.op);

						if (result.op === false) {
							ShowHideElemetOP(false)
						} else {
							ShowHideElemetOP(true)
						}
						
					}

					RecargaGridDetail(result.op, null);
				}
			},
			complete: function () {
			}
		});
	}
}

function OnSelectedWareHouseLocationChanged(s, e) {
	var id_WareHouseLocation = s.GetValue();
	if (s.GetValue() !== null) {
		$.ajax({
			url: "InventoryMove/InventoryReasonChanged",
			type: "post",
			data: {	idIR: id_inventoryReason.GetValue()	},
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
            success: function (result) {
				if (result !== null && result !== undefined) {
					if (result.codeReason === "SI") { $("#id_provider").show(); } else { $("#id_provider").hide(); }
					if (result.codeReason === "SI") { $("#id_providerLabel").show(); } else { $("#id_providerLabel").hide(); }
					if (result.requiereSystemLotNubmber !== null) $("#withLotSystem").val(result.requiereSystemLotNubmber);
					if (result.requiereUserLotNubmber !== null) $("#withLotCustomer").val(result.requiereUserLotNubmber);
                    if (result.op !== null) {
                        if (result.op === false) {
                            ShowHideElemetOP(false)
                        } else {
                            ShowHideElemetOP(true)
                        }
                        $("#mostrarOP").val(result.op);
                    } 
					RecargaGridDetail(result.op, id_WareHouseLocation);
				}
			},
			complete: function () {
			}
		});
	}
}

function RecargaGridDetail(op, idWareHouseLocation) {
    $.ajax({
        url: "InventoryMove/RecargaGridDetail",
        type: "post",
		data: { op: op, idWareHouseLocation: idWareHouseLocation },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $("#wrapper_InventoryMoveDetailsEditFormPartial").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

function OnSelectedInventoryCostCenterChanged(s, e) {
    id_subCostCenter.SetValue(null);
    id_subCostCenter.PerformCallback();
    CallBackGridCC();
}

function OnSelectedInventoryCostCenterEntryChanged(s, e) {
	id_subCostCenterEntry.SetValue(null);
	id_subCostCenterEntry.PerformCallback();
    CallBackGridCC();
}

function OnSubCostCenter_BeginCallback(s, e) {
    e.customArgs["id_costCenter"] = id_costCenter.GetValue();
    e.customArgs["id_subCostCenter"] = s.GetValue();
}

function OnSubCostCenter_SelectedIndexChanged(s, e) {
    CallBackGridCC();
}
function AddNewDocument(s, e) {
	var customParamOPTmp = $("#customParamOP").val();
	var codeDocumentType = $("#codeDocumentType").val();
	var natureMoveIMTmp = $("#natureMoveIM").val();
	if (customParamOPTmp === "IPXM") {

		AddNewInventoryMove("05", "E", customParamOPTmp);


	}
	else {
		if (codeDocumentType == "34") {
			$.ajax({
				url: "InventoryMove/IndexTransferEntryMove",
				type: "post",
				data: null,
				async: true,
				cache: false,
				error: function (error) {
					console.log(error);
				},
				beforeSend: function () {
					showLoading();
				},
				success: function (result) {
					$("#maincontent").html(result);
					AddNewInventoryMoveGeneral(s, e, codeDocumentType, natureMoveIMTmp);
				},
				complete: function () {
					hideLoading();
				}
			});
		} else {
			if (natureMoveIMTmp == "I" && codeDocumentType != "04" && codeDocumentType != "34" && codeDocumentType != "130") {
				codeDocumentType = "03";
			} else if (natureMoveIMTmp == "E" && codeDocumentType != "32" && codeDocumentType != "129") {
				codeDocumentType = "05";
			}
			AddNewInventoryMoveGeneral(s, e, codeDocumentType, natureMoveIMTmp);
		}
	}
}

function SaveDocument(s, e) {
	ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
	ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
	showPage("InventoryMove/InventoryMoveCopy", { id: $("#id_inventoryMove").val() });
}

function ApproveDocument(s, e) {
	showConfirmationDialog(function () {
		Update(true);
	}, "¿Desea aprobar el movimiento de inventario?");
}

function AutorizeDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_inventoryMove").val()
		};
		showForm("InventoryMove/Autorize", data);
	}, "¿Desea autorizar el documento?");
}

function ProtectDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_inventoryMove").val()
		};
		showForm("InventoryMove/Protect", data);
	}, "¿Desea cerrar el documento?");
}

function CancelDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_inventoryMove").val()
		};
		showForm("InventoryMove/Cancel", data);
	}, "¿Desea anular el movimiento de inventario?");
}

function ConciliateCurrentItem() {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_inventoryMove").val()
		};
		showForm("InventoryMove/Conciliate", data);
	}, "¿Desea conciliar el movimiento de inventario?");
}

function ConciliateItem() {
	showLoading();
	$.ajax({
		url: "InventoryMove/Conciliate",
		type: "Post",
		data: {
			id: $("#id_inventoryMove").val()
		},
		async: true,
		cache: false,
		error: function (result) {
			hideLoading();
			NotifyError("Error. " + result.Message);
		},
		success: function (result) {
			if (result.Code !== 0) {
				hideLoading();
				NotifyError("Error al Conciliar. " + result.Message);
				return;
			}
			NotifySuccess("Movimiento de Inventario Conciliado Satisfactoriamente. " + "Estado: " + result.Data);
		}
	});	
}

function RevertDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_inventoryMove").val()
		};
		showForm("InventoryMove/Reverse", data);
	}, "¿Desea reversar el movimiento de inventario?");
}

function ShowDocumentHistory(s, e) {

}

function PrintDocument(s, e) {
	
	var data = {
		id_im: $("#id_inventoryMove").val(),
		codeReport: "IDGV1",
		codeNatureMove: $("#natureMoveIM").val(),
		documentType: $("#codeDocumentType").val()
	};

	$.ajax({
		url: 'InventoryMove/PrintReportInventoryMove',
		data: data,
		async: true,
		cache: false,
		type: 'POST',
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			try {
				if (result != undefined) {
					var reportTdr = result.nameQS;
					var url = 'ReportProd/Index?trepd=' + reportTdr;
					newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
					newWindow.focus();
					hideLoading();
				}
			}
			catch (err) {
				hideLoading();
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}

function OnLocation_SelectedIndexChanged(s, e) {
		var _warehouseLocation = s.GetValue();
		var data =
		{
			idWarehouseLocation: _warehouseLocation
		}
		gridViewMoveDetails.PerformCallback(data);
	}

	function OnLocationEntry_SelectedIndexChanged(s, e) {
	}
function OnWarehouse_SelectedIndexChanged(s, e) {
	var idWarehouseTmp = s.GetValue();

	var _cDocumentType = $("#codeDocumentType").val();
	var _cNatureMove = $("#natureMoveIM").val();
	var _customParamOP = $("#customParamOP").val();
	if (_customParamOP === "IPXM") {
		var idWarehouseFilter = s.GetValue();
		id_LocationFilter.PerformCallback({ idWareHouse: idWarehouseFilter });
		CallBackGrid();

	}


	if (idWarehouseTmp != null) {
		if (_cDocumentType == "32" || _cDocumentType == "129") {   
			var idWarehouseEntryTmp = idWarehouseEntry.GetValue();
			if (idWarehouseEntryTmp != null) {
				gridViewMoveDetails.PerformCallback();
			}
		}
		else {
			gridViewMoveDetails.PerformCallback();
		}
	}
}



function DetailsItemsCombo_DropDown(s, e) {
	s.PerformCallback();
}

function DetailsItemsComboAuto_SelectedIndexChanged(s, e) {
	DetailsUpdateItemInfo({
		id_item: s.GetValue()
	});
}

function DetailsUpdateItemInfo(data) {


	masterCode.SetText("");
	metricUnitInventoryPurchase.SetText("");

	SetValueUnitPriceMove(0);

	unitPriceMoveAux = 0;
	id_metrictUnitMoveAux = 0;

	var _index = gridViewMoveDetails.cpRowIndex;
	var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;

	var id_itemTmp = window["ItemDetail" + key].GetValue();

	var codeDocumentType = $("#codeDocumentType").val();

	var natureMoveIMTmp2 = $("#natureMoveIM").val();
	var isExit = natureMoveIMTmp2 == "E";

	if (isExit) {
		id_lot.ClearItems();
		id_lot.SetValue(null);

		remainingBalance.SetValue(0);
	}

	if (data.id_item === null) {
		ValidateDetail();
		return;
	}

	if (id_itemTmp != null) {
		var fecha = emissionDate.GetDate().toISOString();

		var dato = {
			id_inventoryMoveDetail: gridViewMoveDetails.cpRowKey,
			id_itemCurrent: data.id_item,
			codeDocumentType: codeDocumentType,
			id_metricUnitMove: id_metricUnitMove.GetValue(),
			id_warehouse: idWarehouse.GetValue(),
			id_warehouseLocation: id_warehouseLocationDetail.GetValue(),
			id_warehouseEntry: (codeDocumentType == "129" ? idWarehouseEntry.GetValue() : null),
			id_warehouseLocationEntry: (codeDocumentType == "129" ? idWarehouseLocationEntry.GetValue() : null),
			id_lot: gridViewMoveDetails.cpRowIdLot,
			withLotSystem: $("#withLotSystem").val(),
			withLotCustomer: $("#withLotCustomer").val(),
			fechaEmision: fecha,
		};

		$.ajax({
			url: "InventoryMove/InventoryMoveItemDetails",
			type: "post",
			data: dato,
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result !== null) {
					masterCode.SetText(result.masterCode);
					metricUnitInventoryPurchase.SetValue(result.metricUnitInventoryPurchase);
					
					id_metricUnitMove.IsEditing = true;
					
					var controls = ASPxClientControl.GetControlCollection();
					var id_lotControl = controls.GetByName('id_lot');

					if (result.lots != null) {
						for (var i = 0; i < result.lots.length; i++) {
							if (result.lots[i].number != "") {
								id_lot.AddItem([result.lots[i].number, result.lots[i].Saldo, result.lots[i].FechaLoteStr], result.lots[i].id);
							}
						}
						
						if (id_lotControl != null)
						id_lot.SetEnabled = true;
					} else {
						id_lot.SetEnabled = false;
						id_lot.IsEditing = false;
                    }

					if (result.metricUnits != null) {
						id_metricUnitMove.ClearItems();
						for (var i = 0; i < result.metricUnits.length; i++) {
							if (result.metricUnits[i].name != "") {
								id_metricUnitMove.AddItem(result.metricUnits[i].name, result.metricUnits[i].id);
                            }
						}
						id_metricUnitMove.SetEnabled = true;
					} else {
						id_metricUnitMove.SetEnabled = false;
						id_metricUnitMove.IsEditing = false;
					}

					if (gridViewMoveDetails.cpVerCosto) {
						unitPriceMove.SetValue(result.unitPriceMove);
					}

					 
					//unitPriceMove.SetValue(result.unitPriceMove);

					var arrayFieldStr = [];
					arrayFieldStr.push("code");
					unitPriceMoveAux = result.unitPriceMove;

					OnAmountOrPriceMoveValueChanged();

					if (id_lotControl != null) {
						id_lot.SetEnabled = true;
						id_lot.IsEditing = true;
					}

					var controlsRBalance = ASPxClientControl.GetControlCollection();
					var remainingBalanceControl = controlsRBalance.GetByName('remainingBalance');

					if (remainingBalanceControl !== null) 
						remainingBalance.SetValue(result.remainingBalance);
					if (gridViewMoveDetails.cpVerCosto) {
						unitPriceMove.SetValue(result.averagePrice);
					}
					unitPriceMoveAux = result.averagePrice;

					if (isExit) {
						arrayFieldStr = [];
						arrayFieldStr.push("number");

						if (remainingBalanceControl !== null)
						remainingBalance.SetValue(result.remainingBalance);
						if (gridViewMoveDetails.cpVerCosto) {
							unitPriceMove.SetValue(result.averagePrice);
						}
						
						unitPriceMoveAux = result.averagePrice;

						OnAmountOrPriceMoveValueChanged();
					}
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	} else {
	}
}




function OnWarehouseAutoExit_SelectedIndexChanged(s, e) {
	var idWarehouseTmp = s.GetValue();
	idLocationExit.SetValue(null);
	idLocationExit.PerformCallback();
}

function OnWarehouseEntry_SelectedIndexChanged(s, e) {
	var idWarehouseTmp = idWarehouse.GetValue();
	var idWarehouseEntryTmp = s.GetValue();

	if (idWarehouseTmp != null && idWarehouseEntryTmp != null) {
		gridViewMoveDetails.PerformCallback();
	}
}

function WarehouseCombo_SelectedIndexChanged(s, e) {

	idWarehouseLocation.SetValue(null);
	idWarehouseLocation.ClearItems();
	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data != null) {

		$.ajax({
			url: "InventoryMove/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: data },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					var arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(idWarehouseLocation, result.warehouseLocations, arrayFieldStr);
					idWarehouseLocation.SetValue(result.idWarehouseLocation);

					gridViewMoveDetails.UnselectRows();
					gridViewMoveDetails.PerformCallback();
				}
			},
			complete: function () {
			}
		});
	}
}



function WarehouseComboEntry_SelectedIndexChanged(s, e) {
	idWarehouseLocationEntry.SetValue(null);
	idWarehouseLocationEntry.ClearItems();

	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data != null) {

		$.ajax({
			url: "InventoryMove/WarehouseChangeData",
			type: "post",
			data: { id_warehouse: data },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					var arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(idWarehouseLocationEntry, result.warehouseLocations, arrayFieldStr);
					idWarehouseLocationEntry.SetValue(result.idWarehouseLocationEntry);

					gridViewMoveDetails.UnselectRows();
					gridViewMoveDetails.PerformCallback();
				}
			},
			complete: function () {
			}
		});
	}
}

function WarehouseLocationCombo_SelectedIndexChanged(s, e) {
	var data = s.GetValue();
	if (data === null) {
		return;
	}

	if (data != null) {

		$.ajax({
			url: "InventoryMove/WarehouseLocationChangeData",
			type: "post",
			data: { id_warehouseLocation: data },
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
			success: function (result) {
				if (result !== null && result !== undefined) {
					gridViewMoveDetails.UnselectRows();
					gridViewMoveDetails.PerformCallback();
				}
			},
			complete: function () {
			}
		});
	}
}

function OnReceiverInit(s, e) {
	var idReceiverTmp = $('#IdReceiver').val();
	var idInventoryMoveTmp = $('#IdInventoryMov').val();

	var iidInventoryMoveTmp = 0;
	if (idInventoryMoveTmp != undefined) {
		iidInventoryMoveTmp = parseInt(idInventoryMoveTmp);
		if (iidInventoryMoveTmp == 0) {
			s.SetValue(idReceiverTmp);
		}
	}
}

function OnDispatcherInit(s, e) {
	var idDispatcherTmp = $('#IdDispatcher').val();
	var idInventoryMoveTmp = $('#IdInventoryMov').val();

	var iidInventoryMoveTmp = 0;
	if (idInventoryMoveTmp != undefined) {
		iidInventoryMoveTmp = parseInt(idInventoryMoveTmp);
		if (iidInventoryMoveTmp == 0) {
			s.SetValue(idDispatcherTmp);
		}
	}
}

function AddNewDetail(s, e) {
	gridViewMoveDetails.AddNewRow();
}

function RemoveDetail(s, e) {
	gridViewMoveDetails.UnselectRows();
	gridViewMoveDetails.PerformCallback();
}

function RefreshDetail(s, e) {
	gridViewMoveDetails.UnselectRows();
	gridViewMoveDetails.PerformCallback();
}

function PrintMovement(s, e) {

}

function UpdateView() {
	var id = parseInt($("#id_inventoryMove").val());

	btnNew.SetEnabled(true);
	btnSave.SetEnabled(id === 0);
	btnCopy.SetEnabled(false);
	btnHistory.SetEnabled(id !== 0);
	btnPrint.SetEnabled(id !== 0);
	btnConciliate.SetEnabled(id !== 0);

	$.ajax({
		url: "InventoryMove/Actions",
		type: "post",
		data: { id: id },
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
		},
		success: function (result) {
			btnApprove.SetEnabled(result.btnApprove);
			btnAutorize.SetEnabled(result.btnAutorize);
			btnProtect.SetEnabled(result.btnProtect);
			btnCancel.SetEnabled(result.btnCancel);
			btnRevert.SetEnabled(result.btnRevert);
			btnConciliate.SetEnabled(result.btnConciliate);
		},
		complete: function (result) {
		}
	});

}

function UpdatePagination() {

	var current_page = 1;
	$.ajax({
		url: "InventoryMove/InitializePagination",
		type: "post",
		data: { id_inventoryMove: $("#id_inventoryMove").val(), codeDocumentType: $("#codeDocumentType").val() },
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
		},
		success: function (result) {
			$("#pagination").attr("data-max-page", result.maximunPages);
			current_page = result.currentPage;
		},
		complete: function () {
		}
	});

	$('.pagination').current_page = current_page;
}

function AutoCloseAlert() {
	if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
		setTimeout(function () {
			$(".alert-success").alert('close');
		}, 2000);
	}
}

function OnMessageInit(s, e) {
	var message = $('#Message').val();
	if (!(message == undefined || message == "")) {
		if (gridMessage != undefined) {
			gridMessage.SetText(message);
			$("#GridMessage").show();
		}
	} else {
		if ($("#GridMessage") != undefined) {
			$("#GridMessage").hide();
		}

	}
}

function OnErrorMessageInit(s, e) {
	var messageErr = $('#MessageErr').val();
	if (!(messageErr == undefined || messageErr == "")) {
		if (gridMessageErrorIM != undefined) {
			gridMessageErrorIM.SetText(ErrorMessage(messageErr));
			$("#GridMessageErrorIM").show();
		}
	} else {
		if ($("#GridMessageErrorIM") != undefined) {
			$("#GridMessageErrorIM").hide();
		}

	}
}

function init() {
	UpdatePagination();
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

// #region Combo Invoice OP True
//function OnInvoice_SelectedIndexChanged(s, e) {
//	id_customer.SetValue(null);
//	id_customer.ClearItems();
//
//	if (s.GetValue()) {
//		var data = {
//			id_warehouse: idWarehouse.GetValue(),
//			id_document: s.GetValue(),
//		};
//
//		if (data != null) {
//
//			$.ajax({
//				url: "InventoryMove/InvoiceChangeData",
//				type: "post",
//				data: data,
//				async: true,
//				cache: false,
//				error: function (error) {
//					console.log(error);
//				},
//				beforeSend: function () {
//				},
//				success: function (result) {
//					if (result !== null && result !== undefined) {
//						id_customer.AddItem(
//							[result.id_customer, result.customer], result.id_customer);
//						id_customer.SetValue(result.id_customer);
//					}
//				},
//				complete: function () {
//				}
//			});
//		}
//	}
//}
function OnInvoice_BeginCallback(s, e)
{
	
	e.customArgs['id_customer'] = id_customer.GetValue();
	e.customArgs['codeState'] = $("#codeState").val();
}

function OnIdCustomer_SelectedIndexChanged(s, e)
{
	 	
	let v_mostrarOP = $("#mostrarOP").val();
	let v_valInvFact = $("#valInvFact").val();
	
	
	if (v_mostrarOP == "true" && v_valInvFact == "SI" )
	{
		id_Invoice.PerformCallback();
	}
	
}
// #endregion