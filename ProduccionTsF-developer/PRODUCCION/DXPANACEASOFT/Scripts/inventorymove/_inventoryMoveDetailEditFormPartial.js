var errorMessage = "";
var runningValidation = false;
var unitPriceMoveAux = 0;
var id_metrictUnitMoveAux = 0;

var lotAux = "";
var lotCliAux = "";

var lotNumberIniAux = "";
var lotInternalNumberIniAux = "";
var id_lotIniAux = null;

var id_subCostCenterDetailInit = null;

var id_itemIniAux = null;
var id_itemCurrentAux = null;

function OnItemDetailBeginCallback(s, e) {
	var valInvFact = $("#valInvFact").val();
	if (s != undefined) {
		if (s.name.startsWith('ItemDetail')) {
			var index = parseInt(s.name.substr("ItemDetail".length));
			var fecha = emissionDate.GetDate().toISOString();

			let id_itemTypeVal = null;
			let id_size = null;
			let id_trademark = null;
			let id_presentation = null;
			let codigoProducto = null;
			let categoriaProducto = null;
			let modeloProducto = null;

			e.customArgs["fechaEmision"] = fecha;
			e.customArgs["indice"] = index;
			e.customArgs["id_itemIni"] = id_itemCurrentAux;
			e.customArgs["withLotSystem"] = $("#withLotSystem").val();
			e.customArgs["withLotCustomer"] = $("#withLotCustomer").val();
			e.customArgs["idWarehouse"] = idWarehouse.GetValue();
			e.customArgs["idWarehouseLocation"] = id_warehouseLocationDetail.GetValue();
			e.customArgs["code"] = $("#codeDocumentType").val();

			if ($("#codeDocumentType").val() != 34) {
				id_itemTypeVal = ComboBoxItemType.GetValue() === undefined ? null : ComboBoxItemType.GetValue();
				id_size = ComboBoxSize.GetValue() === undefined ? null : ComboBoxSize.GetValue();;
				id_trademark = ComboBoxTrademark.GetValue() === undefined ? null : ComboBoxTrademark.GetValue();
				id_presentation = ComboBoxPresentation.GetValue() === undefined ? null : ComboBoxPresentation.GetValue();
				codigoProducto = TextBoxCodigoProducto.GetText() === undefined ? null : TextBoxCodigoProducto.GetText();
				categoriaProducto = ComboBoxItemGroupCategory.GetValue() === undefined ? null : ComboBoxItemGroupCategory.GetValue();
				modeloProducto = ComboBoxItemTrademarkModel.GetValue() === undefined ? null : ComboBoxItemTrademarkModel.GetValue();
			}


			e.customArgs["id_itemType"] = id_itemTypeVal;
			e.customArgs["id_size"] = id_size;
			e.customArgs["id_trademark"] = id_trademark;
			e.customArgs["id_presentation"] = id_presentation;
			e.customArgs["codigoProducto"] = codigoProducto;
			e.customArgs["categoriaProducto"] = categoriaProducto;
			e.customArgs["modeloProducto"] = modeloProducto;


			if (valInvFact == "SI")
			e.customArgs["idInvoice"] = id_Invoice.GetValue();
			 
			indexDetail = index;
		}
	}
}

function OnItemDetailEndCallback(s, e) {
	 
	if (id_itemCurrentAux != null) {
		window["ItemDetail" + indexDetail].SetValue(id_itemCurrentAux);
		unitPriceMoveAux = GetValueUnitPriceMove();
		var valInvFact = $("#valInvFact").val();

		var codeDocumentType = $("#codeDocumentType").val();
		var customParamOP = $("#customParamOP").val();
		var natureMoveIMTmp = $("#natureMoveIM").val();
		var isExit = natureMoveIMTmp == "E";
		var fecha = emissionDate.GetDate().toISOString();
		
		var data = {
			id_inventoryMoveDetail: gridViewMoveDetails.cpRowKey,
			id_itemCurrent: id_itemCurrentAux,
			codeDocumentType: codeDocumentType,
			id_metricUnitMove: id_metricUnitMove.GetValue(),
			id_warehouse: idWarehouse.GetValue(),
			id_warehouseLocation: id_warehouseLocationDetail.GetValue(),
			id_warehouseEntry: (codeDocumentType == "129" ? idWarehouseEntry.GetValue() : null),
			id_warehouseLocationEntry: (codeDocumentType == "129" ? id_warehouseLocationEntryDetail.GetValue() : null),
			id_lot: gridViewMoveDetails.cpRowIdLot,
			withLotSystem: $("#withLotSystem").val(),
			withLotCustomer: $("#withLotCustomer").val(),
			fechaEmision: fecha,
			idInvoice: (valInvFact == "SI" ? id_Invoice.GetValue() : null),
		};

		$.ajax({
			url: "InventoryMove/InventoryMoveItemDetails",
			type: "post",
			data: data,
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				 
				id_itemCurrentAux = null;
				masterCode.SetText(result.masterCode);

				metricUnitInventoryPurchase.SetText(result.metricUnitInventoryPurchase);

				id_warehouseDetail.SetText(idWarehouse.GetText());

				if (codeDocumentType == "32" || codeDocumentType == "129") {
					id_warehouseEntry.SetText(idWarehouseEntry.GetText());
				}

				if (codeDocumentType == "03" || codeDocumentType == "04" || customParamOP == "IPXM") {
					var lotNumberAux = lotNumber.GetText();
					if (lotNumberAux != "" && lotNumberAux != null) lotNumber.SetText(result.lotNumber);
					var lotInternalNumberAux = lotInternalNumber.GetText();
					if (lotInternalNumberAux != "" && lotInternalNumberAux != null) lotInternalNumber.SetText(result.lotInternalNumber);
				}

				var arrayFieldStr = [];
				arrayFieldStr = [];
				arrayFieldStr.push("code");
				UpdateDetailObjects(id_metricUnitMove, result.metricUnits, arrayFieldStr);
				id_metricUnitMove.SetValue(result.id_metricUnitMove);

				id_metrictUnitMoveAux = result.id_metricUnitMove;

				arrayFieldStr = [];
				arrayFieldStr.push("name");
				UpdateDetailObjects(id_warehouseLocationDetail, result.warehouseLocations, arrayFieldStr);

				if (codeDocumentType == "129") {
					arrayFieldStr = [];
					arrayFieldStr.push("name");
					UpdateDetailObjects(id_warehouseLocationEntryDetail, result.warehouseLocationsEntry, arrayFieldStr);
				}

				if (isExit) {
					arrayFieldStr = [];
					arrayFieldStr.push("number");
					UpdateDetailObjects(id_lot, result.lots, arrayFieldStr);
					
					remainingBalance.SetValue(result.remainingBalance);
					if (id_lot.GetValue() == 0) id_lot.SetValue(null);
				}
				if (gridViewMoveDetails.cpRowKey == null || gridViewMoveDetails.cpRowKey == 0) {
					OnCheckedGenSecTransChanged();
				} else {
					if (gridViewMoveDetails.cpRowGenSecTrans) lotNumber.SetEnabled(false);
				}
			},
			complete: function () {
				MetricUnitMoveCombo_SelectedIndexChanged(id_metricUnitMove, e);
				var withLotSystemAux = $("#withLotSystem").val();
				var withLotCustomerAux = $("#withLotCustomer").val();
				var es03o04oIPXM = (codeDocumentType == "03" || codeDocumentType == "04" || customParamOP == "IPXM");
				if (es03o04oIPXM) {
					if ((withLotSystemAux == "False" || withLotSystemAux == "false" || withLotSystemAux == false) &&
						(withLotCustomerAux == "False" || withLotCustomerAux == "false" || withLotCustomerAux == false)) {
						lotNumber.SetEnabled(false);
						lotInternalNumber.SetEnabled(false);
						genSecTransCheck.SetEnabled(false);
						genSecTransCheck.SetEnabled(false);
					}
				}
				var es05o32o34o129 = (codeDocumentType == "05" || codeDocumentType == "32" || codeDocumentType == "34" || codeDocumentType == "129");
				if (es05o32o34o129) {
					if ((withLotSystemAux == "False" || withLotSystemAux == "false" || withLotSystemAux == false) &&
						(withLotCustomerAux == "False" || withLotCustomerAux == "false" || withLotCustomerAux == false)) {
						id_lot.SetEnabled(false);
					} else {
						id_lot.SetEnabled(true);
					}
				}
				hideLoading();
			}
		});
	}
}

function OnItemValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Nombre del Producto: Es obligatorio.";
		} else {
			errorMessage += "</br>- Nombre del Producto: Es obligatorio.";
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}
}

function OnWarehouseDetailValidation(s, e) {
	var codeDocumentType = $("#codeDocumentType").val();
	var caption = (codeDocumentType == "34") ? "Bodega Ingreso" : ((codeDocumentType == "32") ? "Bodega Egreso" : ("Bodega"));
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- " + caption + ": Es obligatoria.";
		} else {
			errorMessage += "</br>- " + caption + ": Es obligatoria.";
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}
}

function OnWarehouseLocationDetailValidation(s, e) {
	var codeDocumentType = $("#codeDocumentType").val();
	var _lotMarked = null;
	if (gridViewMoveDetails.cpIsParLotMarked) {
		_lotMarked = lotMarked.GetText();
	}

	errorMessage = "";
	$("#GridMessageErrorMaterialsDetail").hide();
	var caption = (codeDocumentType == "34") ? "Ubicación Ingreso" : (codeDocumentType == "129") ? "Ubicación Egreso" : "Ubicación";

	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- " + caption + ": Es obligatoria.";
		} else {
			errorMessage += "</br>- " + caption + ": Es obligatoria.";
		}
	} else {
		if (codeDocumentType == "34" && gridViewMoveDetails.cpEditingRowIdWarehouseExit == idWarehouse.GetValue() &&
			gridViewMoveDetails.cpEditingRowIdWarehouseLocationExit == id_warehouseLocationDetail.GetValue()) {
			e.isValid = false;
			e.errorText = "No se puede ingresar el producto en la misma ubicación en que se egresa.";
			if (errorMessage == null || errorMessage == "") {
				errorMessage = "- " + caption + ": No se puede ingresar el producto en la misma ubicación en que se egresa.";
			} else {
				errorMessage += "</br>- " + caption + ": No se puede ingresar el producto en la misma ubicación en que se egresa.";
			}
		}
		{
			var _index = gridViewMoveDetails.cpRowIndex;
			var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;

			var objItem = window["ItemDetail" + key];
			var es03o04oIPXM = (codeDocumentType == "03" || codeDocumentType == "04" || customParamOP == "IPXM");
			var data = {
				id: gridViewMoveDetails.cpRowKey,
				id_itemNew: objItem.GetValue(),
				id_warehouseNew: idWarehouse.GetValue(),
				id_warehouseLocationNew: id_warehouseLocationDetail.GetValue(),
				lotNumberNew: es03o04oIPXM ? lotNumber.GetText() : "",
				lotInternalNumberNew: es03o04oIPXM ? lotInternalNumber.GetText() : "",
				id_lotNew: es03o04oIPXM ? null : id_lot.GetValue(),
				lotMarked: _lotMarked
			};
			if (data.id_itemNew != id_itemIniAux || data.id_warehouseNew != id_warehouseIniAux ||
				data.id_warehouseLocationNew != id_warehouseLocationIniAux || data.lotNumberNew != lotNumberIniAux ||
				data.lotInternalNumberNew != lotInternalNumberIniAux || data.id_lotNew != id_lotIniAux) {
				$.ajax({
					url: "InventoryMove/ItsRepeatedItem",
					type: "post",
					data: data,
					async: false,
					cache: false,
					error: function (error) {
						console.log(error);
					},
					beforeSend: function () {
					},
					success: function (result) {
						if (result !== null) {
							if (result.itsRepeated == 1) {
								e.isValid = false;
								e.errorText = result.Error;
								if (errorMessage == null || errorMessage == "") {
									errorMessage = e.errorText;
								} else {
									errorMessage += "</br>- " + e.errorText;
								}
								//viewMessageClient("GridMessageErrorMaterialsDetail", prepareMessageClient('alert-danger', errorMessage));
								var msgErrorAux = ErrorMessage(errorMessage);
								gridMessageErrorMaterialsDetail.SetText(msgErrorAux);
								("#GridMessageErrorMaterialsDetail").show();
							}
						}
					},
					complete: function () {
					}
				});
			}
		}
	}
}

function OnWarehouseLocationEntryDetailValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Ubicación Ingreso: Es obligatoria.";
		} else {
			errorMessage += "</br>- Ubicación Ingreso: Es obligatoria.";
		}
	} else {
		if (idWarehouse.GetValue() == idWarehouseEntry.GetValue() &&
			id_warehouseLocationDetail.GetValue() == id_warehouseLocationEntryDetail.GetValue()) {
			e.isValid = false;
			e.errorText = "No se puede ingresar el producto en la misma ubicación en que se egresa.";
			if (errorMessage == null || errorMessage == "") {
				errorMessage = "- Ubicación Ingreso: No se puede ingresar el producto en la misma ubicación en que se egresa.";
			} else {
				errorMessage += "</br>- Ubicación Ingreso: No se puede ingresar el producto en la misma ubicación en que se egresa.";
			}
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}
}

function OnWarehouseEntryValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Bodega Ingreso: Es obligatoria.";
		} else {
			errorMessage += "</br>- Bodega Ingreso: Es obligatoria.";
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}
}
function OnUnitPriceMoveDetailValidation(s, e) {
	if (s.GetValue() === null || s.GetValue() === 0) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Costo: Es obligatorio.";
		} else {
			errorMessage += "</br>- Costo: Es obligatorio.";
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}
}

function OnPersonProcessPlantDetailValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Planta Proceso: Es obligatoria.";
		} else {
			errorMessage += "</br>- Planta Proceso: Es obligatoria.";
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}
}

function OnAmountValidation(s, e) {
	var codeDocumentType = $("#codeDocumentType").val();
	var id_inventoryMove = $("#id_inventoryMove").val();
	var caption = (codeDocumentType == "03" || codeDocumentType == "04" || codeDocumentType == "34") ? (id_inventoryMove == 0) ? "Cantidad a Ingresar" : "Cantidad a Ingresada" :
		((codeDocumentType == "05" || codeDocumentType == "32") ? ((id_inventoryMove == 0) ? "Cantidad a Egresar" : "Cantidad a Egresada") :
			((codeDocumentType == "129") ? ((id_inventoryMove == 0) ? "Cantidad a Transferir" : "Cantidad Transferida") : ""));
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- " + caption + ": Es obligatoria.";
		} else {
			errorMessage += "</br>- " + caption + ": Es obligatoria.";
		}
	} else if (s.GetValue().toString().length > 20) {
		e.isValid = false;
		e.errorText = "Valor Incorrecto";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- " + caption + ": Valor Incorrecto.";
		} else {
			errorMessage += "</br>- " + caption + ": Valor Incorrecto.";
		}
	} else {
		try {
			var entryAmount = parseFloat(s.GetValue());
			if (entryAmount < 0) {
				e.isValid = false;
				e.errorText = "La cantidad no puede ser menor a 0";
				if (errorMessage == null || errorMessage == "") {
					errorMessage = "- " + caption + ": La cantidad no puede ser menor a 0.";
				} else {
					errorMessage += "</br>- " + caption + ": La cantidad no puede ser menor a 0.";
				}
			} else {
				if (codeDocumentType == "05" || codeDocumentType == "32" || codeDocumentType == "129") {
					var saldo = parseFloat(remainingBalance.GetValue());
					if (entryAmount > saldo) {
						e.isValid = false;
						e.errorText = "La cantidad no puede ser mayor al Saldo";
						if (errorMessage == null || errorMessage == "") {
							errorMessage = "- " + caption + ": La cantidad no puede ser mayor al Saldo.";
						} else {
							errorMessage += "</br>- " + caption + ": La cantidad no puede ser ser mayor al Saldo.";
						}
					}
				} else {
					if (codeDocumentType == "04" || codeDocumentType == "34") {
						var pendiente = parseFloat(remainingQuantity.GetValue());
						if (entryAmount > pendiente) {
							e.isValid = false;
							e.errorText = "La cantidad no puede ser mayor al Pendiente";
							if (errorMessage == null || errorMessage == "") {
								errorMessage = "- " + caption + ": La cantidad no puede ser mayor al Pendiente.";
							} else {
								errorMessage += "</br>- " + caption + ": La cantidad no puede ser ser mayor al Pendiente.";
							}
						}
					}
				}
			}
		} catch (err) {
			e.isValid = false;
			e.errorText = "Valor Incorrecto";
			if (errorMessage == null || errorMessage == "") {
				errorMessage = "- " + caption + ": Valor Incorrecto.";
			} else {
				errorMessage += "</br>- " + caption + ": Valor Incorrecto.";
			}
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}
	//var detallesFactura = gridViewMoveDetails.cpInvoiceDetailValid;
	//if (detallesFactura != null) {
	//	var _index = gridViewMoveDetails.cpRowIndex;
	//	var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;

	//	// Validamos los detalles del documento
	//	var controls = ASPxClientControl.GetControlCollection();
	//	var itemDetalleControl = controls.GetByName('ItemDetail' + key);
	//	var idItem = itemDetalleControl.GetValue();
	//	var numDetalles = detallesFactura.length;

	//	var cantidad = s.GetValue();
	//	for (var i = 0; i < numDetalles; i++) {
	//		var detalle = detallesFactura[i];
	//		if (detalle.IdItem == idItem) {
	//			if (cantidad != detalle.Cantidad) {
	//				e.isValid = false;
	//				e.errorText = "Valor debe ser igual al indicado en la factura";
	//				errorMessage += "Valor debe ser igual al indicado en la factura";
	//			}

	//			break;
	//		}
	//	}
	//}
}

function OnMetricUnitMoveValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- UM Mov.: Es obligatoria.";
		} else {
			errorMessage += "</br>- UM Mov.: Es obligatoria.";
		}
	}

	var codeDocumentType = $("#codeDocumentType").val();
	var customParamOP = $("#customParamOP").val();

	var isEntry = codeDocumentType == "03" || codeDocumentType == "04" || customParamOP == "IPXM";
	if (isEntry) {
		lotAux = lotNumber.GetText();
		lotCliAux = lotInternalNumber.GetText();
	}

	if (!runningValidation) {
		ValidateDetail();
	}
}

function OnLotNumberValidation(s, e) {
	var valueAux = s.GetValue();
	var withLotSystemAux = $("#withLotSystem").val();
	if ((withLotSystemAux == true || withLotSystemAux == "True" || withLotSystemAux == "true") && (valueAux === null || valueAux.trim() == "")) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Sec. Transacc: Es obligatorio.";
		} else {
			errorMessage += "</br>- Sec. Transacc: Es obligatorio.";
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}
}

function OnLotInternalNumberValidation(s, e) {
	var valueAux = s.GetValue();
	var withLotCustomerAux = $("#withLotCustomer").val();
	if ((withLotCustomerAux == true || withLotCustomerAux == "True" || withLotCustomerAux == "true") && (valueAux === null || valueAux.trim() == "")) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Lote: Es obligatorio.";
		} else {
			errorMessage += "</br>- Lote: Es obligatorio.";
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}
}

function OnSubCostCenterDetailValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- SubCentro Costo: Es obligatorio.";
		} else {
			errorMessage += "</br>- SubCentro Costo: Es obligatorio.";
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}
}

function OnCostCenterDetailValidation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- Centro Costo: Es obligatorio.";
		} else {
			errorMessage += "</br>- Centro Costo: Es obligatorio.";
		}
	}

	if (!runningValidation) {
		ValidateDetail();
	}
}

function ValidateDetail() {
	var codeDocumentType = $("#codeDocumentType").val();
	runningValidation = true;
	OnWarehouseLocationDetailValidation(id_warehouseLocationDetail, id_warehouseLocationDetail);

	if (codeDocumentType == "129") {
		OnWarehouseLocationEntryDetailValidation(id_warehouseLocationEntryDetail, id_warehouseLocationEntryDetail);
	}
	var _index = gridViewMoveDetails.cpRowIndex;
	var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;

	var objItem = window["ItemDetail" + key];
	if (objItem != undefined && objItem != null) {
		OnItemValidation(objItem, objItem);
	}

	var customParamOP = $("#customParamOP").val();
	if (codeDocumentType == "32") {
		OnWarehouseEntryValidation(id_warehouseEntry, id_warehouseEntry);
	}
	if (codeDocumentType != "129") {
		OnCostCenterDetailValidation(id_costCenterDetail, id_costCenterDetail);
		OnSubCostCenterDetailValidation(id_subCostCenterDetail, id_subCostCenterDetail);
	}
	OnAmountValidation(amountMove, amountMove);
	OnMetricUnitMoveValidation(id_metricUnitMove, id_metricUnitMove);
	if (codeDocumentType == "03" || codeDocumentType == "04" || customParamOP == "IPXM") {
		OnLotNumberValidation(lotNumber, lotNumber);
		OnLotInternalNumberValidation(lotInternalNumber, lotInternalNumber);
	}

	if (errorMessage != null && errorMessage != "") {
		var msgErrorAux = ErrorMessage(errorMessage);
		gridMessageErrorMaterialsDetail.SetText(msgErrorAux);
		$("#GridMessageErrorMaterialsDetail").show();
	}
	runningValidation = false;
}

function OnAmountValueChanged(s, e) {
	OnAmountOrPriceMoveValueChanged();
}

function GetValueUnitPriceMove() {
	try {
		return unitPriceMove.GetValue();
	} catch (e) {
		return gridViewMoveDetails.cpEditingRowUnitPriceMove;
	}
}

function SetValueUnitPriceMove(value) {
	try {
		unitPriceMove.SetValue(value);
	} catch (e) {
		gridViewMoveDetails.cpEditingRowUnitPriceMove = value;
	}
}

function OnUnitPriceMoveValueChanged(s, e) {
	unitPriceMoveAux = GetValueUnitPriceMove();

	OnAmountOrPriceMoveValueChanged();
}

function SetValueBalanceCost(value) {
	try {
		balanceCost.SetValue(value);
	} catch (e) {
		gridViewMoveDetails.cpEditingRowBalanceCost = value;
	}
}

function OnAmountOrPriceMoveValueChanged() {
	try {
		var amountMoveAux = parseFloat(amountMove.GetValue());
		var balanceCostAux = amountMoveAux * unitPriceMoveAux;
		balanceCostAux = isNaN(balanceCostAux) ? 0 : balanceCostAux;
		SetValueBalanceCost(balanceCostAux);
	} catch (e) {
		SetValueBalanceCost(0);
	}
}

function OnInitCostCenterCombo(s, e) {
	if (id_costCenterDetail.GetValue() === null || id_costCenterDetail.GetValue() === "" || id_costCenterDetail.GetValue() === 0) {
		id_costCenterDetail.SetValue(id_costCenter.GetValue());
		id_subCostCenterDetail.SetValue(id_subCostCenter.GetValue());
	}
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
	e.customArgs["idWarehouse"] = idWarehouse.GetValue();
}

function InventoryMoveSubCostCenter_EndCallback(s, e) {
	id_subCostCenterDetail.SetValue(id_subCostCenterDetailInit);
}

function SetElementVisibility(id, visible) {
	var $element = $("#" + id);
	visible ? $element.show() : $element.hide();
}

function OnGridViewInit(s, e) {
	UpdateTitlePanelDetails();
}

function UpdateTitlePanelDetails() {
	var codeDocumentType = $("#codeDocumentType").val();
	var readOnlyCode = codeDocumentType !== "03" && codeDocumentType !== "04" && codeDocumentType !== "34" && codeDocumentType !== "32" &&
		codeDocumentType !== "05" && codeDocumentType !== "129";
	var natureMoveIMTmp = $("#natureMoveIM").val();
	var customParamOPTmp = $("#customParamOP").val();

	var selectedFilteredRowCount = GetSelectedFilteredRowCountDetails();

	var text = "Total de elementos seleccionados: <b>" + gridViewMoveDetails.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gridViewMoveDetails.GetSelectedRowCount() - GetSelectedFilteredRowCountDetails();
	if (hiddenSelectedRowCount > 0)
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	text += "<br />";

	$("#lblInfo").html(text);

	if ($("#selectAllMode").val() !== "AllPages") {
		SetElementVisibility("lnkSelectAllRows", gridViewMoveDetails.GetSelectedRowCount() > 0 && gridViewMoveDetails.cpVisibleRowCount > selectedFilteredRowCount);
		SetElementVisibility("lnkClearSelection", gridViewMoveDetails.GetSelectedRowCount() > 0);
	}

	if (!(customParamOPTmp === "IPXM" || readOnlyCode)) {
		btnRemoveDetail.SetEnabled(gridViewMoveDetails.GetSelectedRowCount() > 0);
	}
	if (codeDocumentType == "04" || codeDocumentType == "34") {
		btnRemoveDetail.SetEnabled(false);
		btnNewDetail.SetEnabled(false);
	}
}

function GetSelectedFilteredRowCountDetails() {
	return gridViewMoveDetails.cpFilteredRowCountWithoutPage +
		gridViewMoveDetails.GetSelectedKeysOnPage().length;
}

function OnGridViewSelectionChanged(s, e) {
	UpdateTitlePanelDetails();
	s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbacksDetail);
}

function GetSelectedFieldValuesCallbacksDetail(values) {
	var selectedRows = [];
	for (var i = 0; i < values.length; i++) {
		selectedRows.push(values[i]);
	}
}

var customCommand = "";
var idItemGlobal = 0;
var indexDetail = 0;

function OnGridViewBeginCallback(s, e) {
	
	var valInvFact = $("#valInvFact").val();
	customCommand = e.command;
	e.customArgs['code'] = $("#codeDocumentType").val();
	e.customArgs['customParamOP'] = $("#customParamOP").val();
	e.customArgs['id_metricUnitInventoryPurchase'] = id_metrictUnitMoveAux;
	e.customArgs['lotNumber'] = lotAux;
	e.customArgs['lotInternalNumber'] = lotCliAux;
	e.customArgs["errorMessage"] = errorMessage;
	e.customArgs["cpEditingRowUnitPriceMove"] = gridViewMoveDetails.cpEditingRowUnitPriceMove;
	e.customArgs["cpEditingRowBalanceCost"] = gridViewMoveDetails.cpEditingRowBalanceCost;
	e.customArgs["cpEditingVerCosto"] = gridViewMoveDetails.cpEditingVerCosto;
	e.customArgs["idWarehouse"] = idWarehouse.GetValue();
	e.customArgs["idWarehouseEntry"] = ($("#codeDocumentType").val() == "32" || $("#codeDocumentType").val() == "129") ? idWarehouseEntry.GetValue() : null;
	e.customArgs["withLotSystem"] = $("#withLotSystem").val();
	e.customArgs["withLotCustomer"] = $("#withLotCustomer").val();
	e.customArgs['mostrarOP'] = $("#mostrarOP").val();
	e.customArgs['id_inventoryReason'] = id_inventoryReason.GetValue();

	let id_itemTypeVal = null;
	let id_size				= null;
	let id_trademark		= null;
	let id_presentation		= null;
	let codigoProducto		= null;
	let categoriaProducto	= null;
	let modeloProducto = null;


	if ($("#codeDocumentType").val() != 34)
	{
		id_itemTypeVal = ComboBoxItemType.GetValue() === undefined ? null : ComboBoxItemType.GetValue();
		id_size = ComboBoxSize.GetValue() === undefined ? null : ComboBoxSize.GetValue();;
		id_trademark = ComboBoxTrademark.GetValue() === undefined ? null : ComboBoxTrademark.GetValue();
		id_presentation = ComboBoxPresentation.GetValue() === undefined ? null : ComboBoxPresentation.GetValue();
		codigoProducto = TextBoxCodigoProducto.GetText() === undefined ? null : TextBoxCodigoProducto.GetText();
		categoriaProducto = ComboBoxItemGroupCategory.GetValue() === undefined ? null : ComboBoxItemGroupCategory.GetValue();
		modeloProducto = ComboBoxItemTrademarkModel.GetValue() === undefined ? null : ComboBoxItemTrademarkModel.GetValue();
	}
	e.customArgs["id_itemType"] = id_itemTypeVal;
	e.customArgs["id_size"] = id_size
	e.customArgs["id_trademark"] = id_trademark;
	e.customArgs["id_presentation"] = id_presentation;
	e.customArgs["codigoProducto"] = codigoProducto;
	e.customArgs["categoriaProducto"] = categoriaProducto;
	e.customArgs["modeloProducto"] = modeloProducto;

	if (valInvFact == "SI")
	e.customArgs['id_Invoice'] = id_Invoice.GetValue();

	var fecha = emissionDate.GetDate().toISOString();
	e.customArgs["fechaEmision"] = fecha;

	if (e.command == "UPDATEROW" || e.command == "UPDATEEDIT") {
		var _lotMarked = null;
		if (gridViewMoveDetails.cpIsParLotMarked) {
			_lotMarked = lotMarked.GetText();
		}

		if (indexDetail > 0) {
			var _index = gridViewMoveDetails.cpRowIndex;
			var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;

			if (window["ItemDetail" + key] != undefined) {
				e.customArgs["id_item2"] = window["ItemDetail" + key].GetValue();
				idItemGlobal = window["ItemDetail" + key].GetValue();
				e.customArgs["lotMarked"] = _lotMarked;
			}
		} else if (indexDetail == 0) {
			var _index = gridViewMoveDetails.cpRowIndex;
			var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;

			e.customArgs["id_item2"] = window["ItemDetail" + key].GetValue();
			idItemGlobal = window["ItemDetail" + key].GetValue();
			e.customArgs["lotMarked"] = _lotMarked;
		}

		e.customArgs["id_warehouseLocation"] = id_warehouseLocationDetail.GetValue();
		e.customArgs["id_warehouseLocationEntry"] = ($("#codeDocumentType").val() == "129") ? id_warehouseLocationEntryDetail.GetValue() : null;
		e.customArgs["id_subCostCenter"] = ($("#codeDocumentType").val() == "129") ? id_subCostCenter.GetValue() : id_subCostCenterDetail.GetValue();

		if ($("#codeDocumentType").val() != 34) {
			id_itemTypeVal = ComboBoxItemType.GetValue() === undefined ? null : ComboBoxItemType.GetValue();
			id_size = ComboBoxSize.GetValue() === undefined ? null : ComboBoxSize.GetValue();;
			id_trademark = ComboBoxTrademark.GetValue() === undefined ? null : ComboBoxTrademark.GetValue();
			id_presentation = ComboBoxPresentation.GetValue() === undefined ? null : ComboBoxPresentation.GetValue();
			codigoProducto = TextBoxCodigoProducto.GetText() === undefined ? null : TextBoxCodigoProducto.GetText();
			categoriaProducto = ComboBoxItemGroupCategory.GetValue() === undefined ? null : ComboBoxItemGroupCategory.GetValue();
			modeloProducto = ComboBoxItemTrademarkModel.GetValue() === undefined ? null : ComboBoxItemTrademarkModel.GetValue();
		}

		e.customArgs["id_itemType"] = id_itemTypeVal;
		e.customArgs["id_size"] = id_size;
		e.customArgs["id_trademark"] = id_trademark;
		e.customArgs["id_presentation"] = id_presentation;
		e.customArgs["codigoProducto"] = codigoProducto;
		e.customArgs["categoriaProducto"] = categoriaProducto;
		e.customArgs["modeloProducto"] = modeloProducto;
	}
}

function OnGridViewEndCallback(s, e) {
	UpdateTitlePanelDetails();
	if (id_itemIniAux > 0) {
		var _index = gridViewMoveDetails.cpRowIndex;
		var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;

		if (window["ItemDetail" + key] != undefined) {
			window["ItemDetail" + key].SetValue(id_itemIniAux);
			idItemGlobal = 0;
		}
	}
	errorMessage = "";
	$("#GridMessageErrorMaterialsDetail").hide();

	// Bloqueamos la fecha si existen registros
	var esIngresoTransferencia = tabControl.cpCodeDocumentType == "34";
	emissionDate.SetEnabled(!gridViewMoveDetails.cpExistenRegistros || esIngresoTransferencia);
}

function gvEditDetailsClearSelection() {
	gridViewMoveDetails.UnselectRows();
}

function gvEditDetailsSelectAllRows() {
	gridViewMoveDetails.SelectRows();
}

function ItemCombo_OnInit(s, e) {
	id_itemIniAux = s.GetValue();
	id_itemCurrentAux = id_itemIniAux;
	id_warehouseIniAux = idWarehouse.GetValue();
	id_warehouseLocationIniAux = id_warehouseLocationDetail.GetValue();
	var codeDocumentType = $("#codeDocumentType").val();

	var es03o04oIPXM = (codeDocumentType == "03" || codeDocumentType == "04" || customParamOP == "IPXM");
	lotNumberIniAux = es03o04oIPXM ? lotNumber.GetText() : "";
	lotInternalNumberIniAux = es03o04oIPXM ? lotInternalNumber.GetText() : "";
	id_lotIniAux = es03o04oIPXM ? null : id_lot.GetValue();

	var id_purchaseOrderDetail = gridViewMoveDetails.cpEditingRowPurchaseOrderDetail;
	var id_inventoryMoveExit = gridViewMoveDetails.cpEditingRowInventoryMoveExit;
	if (es03o04oIPXM) {
		if (genSecTransCheck.GetValue() == null) genSecTransCheck.SetValue(0);
		else {
			if (genSecTransCheck.GetValue() == true) lotNumber.SetEnabled(false);
		}
	}

	if (id_itemCurrentAux != null && (id_purchaseOrderDetail != 0 || codeDocumentType == "34" || codeDocumentType == "129")) {
		var _index = gridViewMoveDetails.cpRowIndex;
		var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;
		window["ItemDetail" + key].SetEnabled(false);
	}

	var natureMoveIMTmp2 = $("#natureMoveIM").val();
	var isExit = natureMoveIMTmp2 == "E";
	id_warehouseDetail.SetText(idWarehouse.GetText());
	if (gridViewMoveDetails.cpIdWarehouseLocation) {
		id_warehouseLocationDetail.SetValue(gridViewMoveDetails.cpIdWarehouseLocation);
		OnWarehouseLocationCombo_SelectedIndexChanged(gridViewMoveDetails.cpIdWarehouseLocation,e);
	}

	if (codeDocumentType == "32") {
		id_warehouseEntry.SetText(idWarehouseEntry.GetText());
	}

	if (codeDocumentType == "129") {
		id_warehouseEntry.SetText(idWarehouseEntry.GetText());
	}

	var withLotSystemAux = $("#withLotSystem").val();
	var withLotCustomerAux = $("#withLotCustomer").val();
	if (es03o04oIPXM) {
		if ((withLotSystemAux == "False" || withLotSystemAux == "false" || withLotSystemAux == false) &&
			(withLotCustomerAux == "False" || withLotCustomerAux == "false" || withLotCustomerAux == false)) {
			lotNumber.SetEnabled(false);
			lotInternalNumber.SetEnabled(false);
			genSecTransCheck.SetEnabled(false);
			genSecTransCheck.SetEnabled(false);
		}
	}
	var es05o32o34o129 = (codeDocumentType == "05" || codeDocumentType == "32" || codeDocumentType == "34" || codeDocumentType == "129");
	if (es05o32o34o129) {
		if ((withLotSystemAux == "False" || withLotSystemAux == "false" || withLotSystemAux == false) &&
			(withLotCustomerAux == "False" || withLotCustomerAux == "false" || withLotCustomerAux == false)) {
			id_lot.SetEnabled(false);
		}
		else {
			id_lot.SetEnabled(true);
		}
	}
}

function DetailsItemsCombo_DropDown(s, e) {
	s.PerformCallback();
}

function DetailsItemsCombo_SelectedIndexChanged(s, e) {
	DetailsUpdateItemInfo({
		id_item: s.GetValue()
	});
}

function DetailsUpdateItemInfo(data) {

	 
	masterCode.SetText("");
	id_metricUnitMove.SetValue(null);
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
		id_lot.ClearItems(); // limpiamos el detalle de ítems
		id_lot.SetValue(null);

		remainingBalance.SetValue(0);
	}

	if (data.id_item === null) {
		ValidateDetail();
		return;
	}

	if (id_itemTmp != null) {

		var fechaemision = emissionDate.GetDate().toISOString();
		
		var dato = {
			id_inventoryMoveDetail: gridViewMoveDetails.cpRowKey,
			id_itemCurrent: data.id_item,
			codeDocumentType: codeDocumentType,
			id_metricUnitMove: id_metricUnitMove.GetValue(),
			id_warehouse: idWarehouse.GetValue(),
			id_warehouseLocation: id_warehouseLocationDetail.GetValue(),
			id_warehouseEntry: (codeDocumentType == "129" ? idWarehouseEntry.GetValue() : null),
			id_warehouseLocationEntry: (codeDocumentType == "129" ? id_warehouseLocationEntryDetail.GetValue() : null),
			id_lot: gridViewMoveDetails.cpRowIdLot,
			withLotSystem: $("#withLotSystem").val(),
			withLotCustomer: $("#withLotCustomer").val(),
			fechaEmision: fechaemision,
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
					metricUnitInventoryPurchase.SetText(result.metricUnitInventoryPurchase);
					id_warehouseDetail.SetText(idWarehouse.GetText());
					if (codeDocumentType == "32" || codeDocumentType == "129") {
						id_warehouseEntry.SetText(idWarehouseEntry.GetText());
					}
					var arrayFieldStr = [];
					arrayFieldStr.push("code");
					UpdateDetailObjects(id_metricUnitMove, result.metricUnits, arrayFieldStr);
					id_metricUnitMove.SetValue(result.id_metricUnitMove);

					id_metrictUnitMoveAux = result.id_metricUnitMove;

					if (gridViewMoveDetails.cpVerCosto) {
						unitPriceMove.SetValue(result.unitPriceMove);
					}
					unitPriceMoveAux = result.unitPriceMove;

					OnAmountOrPriceMoveValueChanged();
					if (isExit) {

						if (result.lots != null) {

							id_lot.PerformCallback();

							//for (var i = 0; i < maxItems; i++) {
							//	if (result.lots[i].number != "") {
							//		id_lot.AddItem([result.lots[i].number, result.lots[i].Saldo, result.lots[i].FechaLoteStr], result.lots[i].id);
							//	}
							//}

							//dataLoteList = result.lots;
							//pageIndex = 0;
							//cargarDatos();

						}

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
				ValidateDetail();
			}
		});
	} else {
		ValidateDetail();
	}
}

function OnInitWarehouseLocationExit(s, e) {
	var data = s.GetValue();
	if (data === null) {
		var id_warehouseLocationExit = $("#id_warehouseLocationExit").val();
		console.log(id_warehouseLocationExit);
		s.SetValue(id_warehouseLocationExit);
	}
}

function OnInitWarehouseEntry(s, e) {
	var data = s.GetValue();
	if (data === null) {
		var id_warehouseEntry = $("#id_warehouseEntry").val();
		console.log(id_warehouseEntry);
		s.SetValue(id_warehouseEntry);
	}
}

function OnInitWarehouseLocationEntry(s, e) {
	var data = s.GetValue();
	if (data === null) {
		var id_warehouseLocationEntry = $("#id_warehouseLocationEntry").val();
		console.log(id_warehouseLocationEntry);
		s.SetValue(id_warehouseLocationEntry);
	}
}

function OnWarehouseDetailCombo_SelectedIndexChanged(s, e) {
	id_warehouseLocationDetail.SetValue(null);
	id_warehouseLocationDetail.ClearItems();

	var codeDocumentType = $("#codeDocumentType").val();

	var natureMoveIMTmp2 = $("#natureMoveIM").val();
	var isExit = natureMoveIMTmp2 == "E";
	if (isExit) {
		SetValueUnitPriceMove(0);
		unitPriceMoveAux = 0;

		id_metrictUnitMoveAux = 0;
		id_metricUnitMove.SetValue(null);

		OnAmountOrPriceMoveValueChanged();
	}

	var data = {
		id_warehouse: s.GetValue()
	};

	$.ajax({
		url: "InventoryMove/UpdateWarehouseLocation",
		type: "post",
		data: data,
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
				UpdateDetailObjects(id_warehouseLocationDetail, result.warehouseLocations, arrayFieldStr);
			}
		},
		complete: function () {
		}
	});
}

function OnWarehouseLocationCombo_SelectedIndexChanged(s, e) {
	var _index = gridViewMoveDetails.cpRowIndex;
	var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;


	

	var id_itemTmp = window["ItemDetail" + key].GetValue();
	id_itemCurrentAux = null;
	window["ItemDetail" + key].PerformCallback();


	var _index = gridViewMoveDetails.cpRowIndex;
	var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;
	var id_itemTmp = window["ItemDetail" + key].GetValue();
	var codeDocumentType = $("#codeDocumentType").val();

	id_lot.PerformCallback();
}

function OnLotDetailCombo_SelectedIndexChanged(s, e) {
	var _index = gridViewMoveDetails.cpRowIndex;
	var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;
	var id_itemTmp = window["ItemDetail" + key].GetValue();
	var codeDocumentType = $("#codeDocumentType").val();
	var isExit = codeDocumentType == "05" || codeDocumentType == "32";
	var _id_lot = s.lastSuccessValue;
	var texto = s.GetText();
	var _lotMarked = null;
	var splits = texto.split("/");
	if (splits.length > 2) {
		_lotMarked = splits[2].trim();
	}

	if (isExit) {
		id_metricUnitMove.SetValue(null);
		
		id_metrictUnitMoveAux = 0;
		if (gridViewMoveDetails.cpIsParLotMarked) {
			lotMarked.SetText("");
		}
		SetValueUnitPriceMove(0);
		unitPriceMoveAux = 0;

		OnAmountOrPriceMoveValueChanged();
	}

	var fecha = emissionDate.GetDate().toISOString();
	var data = {
		id_item: id_itemTmp,
		id_warehouse: idWarehouse.GetValue(),
		id_warehouseLocation: id_warehouseLocationDetail.GetValue(),
		id_lot: _id_lot,
		idWarehouse: idWarehouse.GetValue(),
		lotMarked: _lotMarked,
		fechaEmision: fecha,
	};

	$.ajax({
		url: "InventoryMove/LotDetail",
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
		},
		success: function (result) {
			if (result !== null && result !== undefined) {
				remainingBalance.SetValue(result.remainingBalance);

				if (gridViewMoveDetails.cpVerCosto) {
					unitPriceMove.SetValue(result.averagePrice);
				}

				unitPriceMoveAux = result.averagePrice;

				id_metricUnitMove.SetValue(result.id_metricUnitMove);

				id_metrictUnitMoveAux = result.id_metricUnitMove;

				if (gridViewMoveDetails.cpIsParLotMarked) {
					lotMarked.SetText(_lotMarked);
				}

				OnAmountOrPriceMoveValueChanged();
			}
		},
		complete: function () {
		}
	});
}

function OnLotDetailComboValidation(s, e)
{
	var codeDocumentType = $("#codeDocumentType").val();
	var _lotMarked = null;
	if (gridViewMoveDetails.cpIsParLotMarked) {
		_lotMarked = lotMarked.GetText();
	}
	errorMessage = "";
	$("#GridMessageErrorMaterialsDetail").hide();
	var caption = (codeDocumentType == "34") ? "Ubicación Ingreso" : (codeDocumentType == "129") ? "Ubicación Egreso" : "Ubicación";

	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		if (errorMessage == null || errorMessage == "") {
			errorMessage = "- " + caption + ": Es obligatoria.";
		} else {
			errorMessage += "</br>- " + caption + ": Es obligatoria.";
		}
	} else {
			var _index = gridViewMoveDetails.cpRowIndex;
			var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;

			var objItem = window["ItemDetail" + key];
			var es03o04oIPXM = (codeDocumentType == "03" || codeDocumentType == "04" || customParamOP == "IPXM");
			var data = {
				id: gridViewMoveDetails.cpRowKey,
				id_itemNew: objItem.GetValue(),
				id_warehouseNew: idWarehouse.GetValue(),
				id_warehouseLocationNew: id_warehouseLocationDetail.GetValue(),
				lotNumberNew: es03o04oIPXM ? lotNumber.GetText() : "",
				lotInternalNumberNew: es03o04oIPXM ? lotInternalNumber.GetText() : "",
				id_lotNew: es03o04oIPXM ? null : id_lot.GetValue(),
				lotMarked: _lotMarked
			};
			if (data.id_itemNew != id_itemIniAux || data.id_warehouseNew != id_warehouseIniAux ||
				data.id_warehouseLocationNew != id_warehouseLocationIniAux || data.lotNumberNew != lotNumberIniAux ||
				data.lotInternalNumberNew != lotInternalNumberIniAux || data.id_lotNew != id_lotIniAux) {
				$.ajax({
					url: "InventoryMove/ItsRepeatedItem",
					type: "post",
					data: data,
					async: false,
					cache: false,
					error: function (error) {
						console.log(error);
					},
					beforeSend: function () {
					},
					success: function (result) {
						 
						if (result !== null) {
							if (result.itsRepeated == 1) {
								e.isValid = false;
								e.errorText = result.Error;
								if (errorMessage == null || errorMessage == "") {
									errorMessage = e.errorText;
									
								} else {
									errorMessage += "</br>- " + e.errorText;
								}
								//viewMessageClient("GridMessageErrorMaterialsDetail", prepareMessageClient('alert-danger', errorMessage));
								var msgErrorAux = ErrorMessage(errorMessage);
								gridMessageErrorMaterialsDetail.SetText(msgErrorAux);
								$("#GridMessageErrorMaterialsDetail").show();
							}
						}
					},
					complete: function () {
					}
				});
			}
		 
	}


}


function UpdateQuantityTotal(data) {
	if (data.id_metricUnitMove === null) {
		SetValueBalanceCost(0);
		return;
	}

	$.ajax({
		url: "InventoryMove/UpdateQuantityTotal",
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
		},
		success: function (result) {
			if (result !== null && result != undefined) {
				if (result.Message == "OK") {
					SetValueBalanceCost(result.balanceCost);
					unitPriceMoveAux = result.unitPriceMove;
					SetValueUnitPriceMove(result.unitPriceMove);
				} else {
					id_metricUnitMove.SetValue(result.id_metricUnitMove);
					var msgAux = WarningMessage(result.Message);
					gridMessageErrorMaterialsDetail.SetText(msgAux);
					$("#GridMessageErrorMaterialsDetail").show();
					if ($(".alert-warning") !== undefined && $(".alert-warning") !== null) {
						$(".alert-warning").fadeTo(3000, 0.45, function () {
							$(".alert-warning").alert('close');
						});
					}
				}
			}
		},
		complete: function () {
		}
	});
}

function MetricUnitMoveCombo_SelectedIndexChanged(s, e) {
	var _unitPriceMove = 0;
	if (gridViewMoveDetails.cpVerCosto) {
		_unitPriceMove = unitPriceMove.GetValue();
	}

	var unitPriceMoveAux = _unitPriceMove;
	var strUnitPriceMove = unitPriceMoveAux == null ? "0" : unitPriceMoveAux.toString();
	var resUnitPriceMove = strUnitPriceMove.replace(".", ",");

	var amountMoveAux = amountMove.GetValue();
	var strAmountMove = amountMoveAux == null ? "0" : amountMoveAux.toString();
	var resAmountMove = strAmountMove.replace(".", ",");

	if (s.GetValue()) {
		id_metrictUnitMoveAux = s.GetValue();
		UpdateQuantityTotal({
			unitPriceMove: resUnitPriceMove,
			id_metricUnitMove: id_metrictUnitMoveAux,
			id_metricUnitMoveCurrent: s.GetValue(),
			amountMove: resAmountMove
		});
	}
}

function OnCheckedGenSecTransChanged(s, e) {
	try {
		if (genSecTransCheck.GetChecked()) {
			$.ajax({
				url: "InventoryMove/GetSecTrans",
				type: "post",
				data: {
					id_inventoryMoveDetail: gridViewMoveDetails.cpRowKey,
					id_inventoryReason: id_inventoryReason.GetValue()
				},
				async: true,
				cache: false,
				error: function (error) {
					console.log(error);
				},
				beforeSend: function () {
				},
				success: function (result) {
					if (result !== null) {
						lotNumber.SetText(result.secTrans);
						lotNumber.SetEnabled(false);
					}
				},
				complete: function () {
				}
			});
		} else {
			lotNumber.SetText("");
			lotNumber.SetEnabled(true);
		}
	} catch (e) {
	}
}

function AddNewDetail(s, e) {
	gridViewMoveDetails.AddNewRow();
}


function OnLotDetailBeginCallback(s, e)
{
	 
	var _index = gridViewMoveDetails.cpRowIndex;
	var key = _index >= 0 ? gridViewMoveDetails.cpRowKey : 0;
	let id_item = window["ItemDetail" + key].GetValue();

	var fecha = emissionDate.GetDate().toISOString();
	
	e.customArgs["inventoryMoveDetail"] = {
		id_item: id_item,
		id_lot: 0,
		id_warehouseLocation: id_warehouseLocationDetail.GetValue(),
	};
	e.customArgs["code"] = $("#codeDocumentType").val();
	e.customArgs["idWarehouse"] = idWarehouse.GetValue();
	e.customArgs["fechaEmision"] = fecha;
	e.customArgs["withLotSystem"] = $("#withLotSystem").val();
	e.customArgs["withLotCustomer"] = $("#withLotCustomer").val();
	e.customArgs["forceResetLots"] = (id_item == null || id_item == 0 ) ? true : false;
	
}


$(function () {
});