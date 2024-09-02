var aprovedGlobal = false;

function ComboBoxWarehouseExitEdit_SelIndexChanged(s, e) {
	ComboBoxWarehouseLocationExitEdit.PerformCallback();
}

function ComboBoxWarehouseEntryEdit_SelIndexChanged(s, e) {
	ComboBoxWarehouseLocationEntryEdit.PerformCallback();
}

function ComboBoxWarehouseLocationEntryEdit_BeginCallback(s, e) {
	e.customArgs['id_Warehouse'] = ComboBoxWarehouseEntryEdit.GetValue();
}
function ComboBoxWarehouseLocationEntryEdit_EndCallback(s, e) {
	if (s.cpValSelected != null) {
		s.SetValue(s.cpValSelected);
	}
}

function ComboBoxWarehouseLocationExitEdit_BeginCallback(s, e) {
	e.customArgs['id_Warehouse'] = ComboBoxWarehouseExitEdit.GetValue();
}
function ComboBoxWarehouseLocationExitEdit_EndCallback(s, e) {
	if (s.cpValSelected != null) {
		s.SetValue(s.cpValSelected);
	}
}

function ComboBoxCostCenterEntryEdit_BeginCallback(s, e) {
	let _sel = ComboBoxCostCenterExitEdit.GetValue();
	e.customArgs['change_cost_center'] = changedCostCenterExit;
	if (_sel != null) {
		e.customArgs["id_costcenter"] = _sel;
	} else {
		e.customArgs["id_costcenter"] = s.GetValue();
	}
}

function ComboBoxCostCenterEntryEdit_EndCallback(s, e) {
	changedCostCenterExit = "";
	if (s.GetValue() == null) {
		if (s.cpValSelected != null) {
			s.SetValue(s.cpValSelected);
			ComboBoxSubCostCenterEntryEdit.PerformCallback();
		}
	}
}

function ComboBoxCostCenterEntryEdit_SelIndexChanged(s, e) {
	ComboBoxSubCostCenterEntryEdit.PerformCallback();
}
function ComboBoxCostCenterExitEdit_SelIndexChanged(s, e) {
	ComboBoxSubCostCenterExitEdit.PerformCallback();

	if (ComboBoxCostCenterEntryEdit.GetValue() == null) {
		changedCostCenterExit = "Y";
		ComboBoxCostCenterEntryEdit.PerformCallback();
	}
}
var changedCostCenterExit = "";
var changedSubCostCenterExit = "";
function ComboBoxSubCostCenterExitEdit_SelectedIndexChanged(s, e) {
	if (ComboBoxSubCostCenterEntryEdit.GetValue() == null &&
		(ComboBoxCostCenterExitEdit.GetValue() == ComboBoxCostCenterEntryEdit.GetValue())
		&& ComboBoxCostCenterEntryEdit.GetValue() > 0
	) {
		changedSubCostCenterExit = "Y";
		ComboBoxSubCostCenterEntryEdit.PerformCallback();
	}
}

function ComboBoxSubCostCenterEntryEdit_BeginCallback(s, e) {
	e.customArgs['id_costcenter'] = ComboBoxCostCenterEntryEdit.GetValue();
	e.customArgs['change_sub_cost_center'] = changedSubCostCenterExit;
	if (s.GetValue() == null) {
		if (ComboBoxSubCostCenterExitEdit != null) {
			e.customArgs['id_subcostcenter'] = ComboBoxSubCostCenterExitEdit.GetValue();
		}
	} else {
		e.customArgs['id_subcostcenter'] = s.GetValue();
	}
}

function ComboBoxSubCostCenterEntryEdit_EndCallback(s, e) {
	changedSubCostCenterExit = "";
	if (s.GetValue() == null) {
		if (s.cpValSelected != null) {
			s.SetValue(s.cpValSelected);
		}
	}
}

function ComboBoxSubCostCenterExitEdit_BeginCallback(s, e) {
	e.customArgs['id_costcenter'] = ComboBoxCostCenterExitEdit.GetValue();
}

function quantity_valuechanged(s, e) {
	let valQuantity = s.GetValue();
	let _valCost = cost.GetValue();

	total.SetValue(valQuantity * _valCost);
}

function cost_valuechanged(s, e) {
	let valQuantity = quantity.GetValue();
	let _valCost = cost.GetValue();

	total.SetValue(valQuantity * _valCost);
}

function ButtonUpdate_Click() {
	SaveItem(false);
}

function ButtonCancel_Click() {
	RedirecBack();
}

function RedirecBack() {
	showPage("AutomaticTransfer/Index");
}

function ShowCurrentItem(enabled) {
	var data = {
		id: $('#id_automatictransfer').val(),
		enabled: enabled
	};

	showPage("AutomaticTransfer/Edit", data);
}

function AddNewItem() {
	var data = {
		id: 0,
		enabled: true
	};
	showPage("AutomaticTransfer/Edit", data);
}

function EditCurrentItem() {
	showLoading();
	ShowCurrentItem(true);
}

function CopyDocument() {
	var id = $("#id_automatictransfer").val();
	if (!(id > 0)) {
		NotifyError("No puede copiar un documento que no existe.");
		return;
	}
	showPage("AutomaticTransfer/DocumentCopy", { id: id });
}

function SaveCurrentItem() {
	SaveItem(false);
}

function SaveItem(aproved) {
	// Habilitamos la fecha de emisión
	DateTimeEmision.SetEnabled(true);

	showLoading();
	var respuesta = ValidateSaveHeader();
	if (!respuesta.valid) {
		setTimeout(function () {
			pcAutomaticTransfer.SetActiveTab(pcAutomaticTransfer.GetTab(1));
			NotifyWarning(respuesta.mensaje);
		}, 100);
		hideLoading();
		return;
	}

	aprovedGlobal = aproved;
	$.ajax({
		url: 'AutomaticTransfer/Save',
		type: 'post',
		data: SaveDataUser(),
		async: true,
		cache: false,
		success: function (result) {
			hideLoading();
			if (result.Code !== 0) {
				hideLoading();
				NotifyError("Error. " + result.Message);
				return;
			}

			var id = result.Data;
			$('#id_automatictransfer').val(id);

			if (aprovedGlobal)
				AprovedItem();
			else
				ShowCurrentItem(true);

			NotifySuccess("La Transferencia Automática fue Guardado Satisfactoriamente.");
			DateTimeEmision.SetEnabled(false);
		},
		error: function (result) {
			DateTimeEmision.SetEnabled(false);
			hideLoading();
		}
	});
}

function SaveDataUser() {
	var emisionDate = DateTimeEmision.GetValue();
	var yearEmisionDate = emisionDate.getFullYear();
	var monthEmisionDate = emisionDate.getMonth();
	var dayEmisionDate = emisionDate.getDate();

	var userData = {
		id: $('#id_automatictransfer').val(),

		id_WarehouseExit: ComboBoxWarehouseExitEdit.GetValue(),
		id_WarehouseLocationExit: ComboBoxWarehouseLocationExitEdit.GetValue(),
		id_WarehouseEntry: ComboBoxWarehouseEntryEdit.GetValue(),
		id_WarehouseLocationEntry: ComboBoxWarehouseLocationEntryEdit.GetValue(),
		id_InventoryReasonExit: ComboBoxReasonExitEdit.GetValue(),
		id_InventoryReasonEntry: ComboBoxReasonEntryEdit.GetValue(),
		id_CostCenterExit: ComboBoxCostCenterExitEdit.GetValue(),
		id_SubCostCenterExit: ComboBoxSubCostCenterExitEdit.GetValue(),
		id_CostCenterEntry: ComboBoxCostCenterEntryEdit.GetValue(),
		id_SubCostCenterEntry: ComboBoxSubCostCenterEntryEdit.GetValue(),
		RequerimentNumber: RequerimentNumber.GetText(),
		id_WarehouseExit: ComboBoxWarehouseExitEdit.GetValue(),

		idProcessPlantEntry: processPlantEntry.GetValue(),
		idProcessPlantExit: processPlantExit.GetValue(),

		dateTimeEmision: yearEmisionDate + "-" +
			(++monthEmisionDate).toString().padStart(2, 0) + "-" +
			dayEmisionDate.toString().padStart(2, 0) + "T00:00:00",
		description: MemoDescription.GetText()
	};

	var AutomaticTransfer = {
		jsonAutomaticTransfer: JSON.stringify(userData)
	};

	return AutomaticTransfer;
}

function AprovedItem() {
	showLoading();
	$.ajax({
		url: 'AutomaticTransfer/Approve',
		type: 'post',
		data: { id: $('#id_automatictransfer').val() },
		async: true,
		cache: false,
		error: function (result) {
			hideLoading();
			NotifyError("Error. " + result.Message);
		},
		success: function (result) {
			if (result.Code !== 0) {
				hideLoading();
				NotifyError("Error al Aprobar. " + result.Message);
				return;
			}
			ShowCurrentItem(false);
			NotifySuccess("La Trasnferencia Automática fue Aprobado Satisfactoriamente. ");
		}
	});
}

function AprovedCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Aprobar la Transferencia Automática?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			if ($("#enabled").val() === "true") {
				SaveItem(true);
			} else {
				AprovedItem();
			}
		}
	});
}

function ReverseCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Reversar la Transferencia Automática?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'AutomaticTransfer/Reverse',
				type: 'post',
				data: { id: $('#id_automatictransfer').val() },
				async: true,
				cache: false,
				error: function (result) {
					hideLoading();
					NotifyError("Error. " + result.Message);
				},
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error al Reversar. " + result.Message);
						return;
					}

					ShowCurrentItem(false);
					NotifySuccess("La Transferencia Automática fue Reversado Satisfactoriamente. ");
				}
			});
		}
	});
}

function AnnulCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Anular la Transferencia Automática?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'AutomaticTransfer/Annul',
				type: 'post',
				data: { id: $('#id_automatictransfer').val() },
				async: true,
				cache: false,
				error: function (result) {
					hideLoading();
					NotifyError("Error. " + result.Message);
				},
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error al Anular. " + result.Message);
						return;
					}

					ShowCurrentItem(false);
					NotifySuccess("La Transferencia Automática fue Anulado Satisfactoriamente. ");
				}
			});
		}
	});
}

function ConciliatedItem() {
	showLoading();
	$.ajax({
		url: 'AutomaticTransfer/Conciliate',
		type: 'post',
		data: { id: $('#id_automatictransfer').val() },
		async: true,
		cache: false,
		error: function (result) {
			hideLoading();
			NotifyError("Error. " + result.Message);
		},
		success: function (result) {
			if (result.Code !== 0) {
				hideLoading();
				NotifyError("Error al conciliar. " + result.Message);
				return;
			}
			ShowCurrentItem(false);
			NotifySuccess("La Transferencia Automática fue Conciliada Satisfactoriamente. ");
		}
	});
}

function ConciliatedCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Conciliar la Transferencia Automática?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			ConciliatedItem();
		}
	});
}


// Header
function OnIsSetHeaderControls(s, e) {
	let _selWarehouseEdit = ComboBoxWarehouseExitEdit.GetValue();
	let _selWarehouseLocationEdit = ComboBoxWarehouseLocationExitEdit.GetValue();
	let _selReasonExitEdit = ComboBoxReasonExitEdit.GetValue();
	if (e.tab.name == "tabDetails" && _selWarehouseEdit === null) {
		setTimeout(function () {
			pcAutomaticTransfer.SetActiveTab(pcAutomaticTransfer.GetTab(1));
			NotifyWarning("Debe seleccionar la Bodega de Salida");
		}, 100);
		return;
	}
	if (e.tab.name == "tabDetails" && _selWarehouseLocationEdit === null) {
		setTimeout(function () {
			pcAutomaticTransfer.SetActiveTab(pcAutomaticTransfer.GetTab(1));
			NotifyWarning("Debe seleccionar la Ubicacion de Salida");
		}, 100);
		return;
	}
	if (e.tab.name == "tabDetails" && _selReasonExitEdit === null) {
		setTimeout(function () {
			pcAutomaticTransfer.SetActiveTab(pcAutomaticTransfer.GetTab(1));
			NotifyWarning("Debe seleccionar la Motivo de Salida");
		}, 100);
		return;
	}
}
function pcAutomaticTransfer_Init(s, e) {
	DateTimeEmision.SetEnabled(!pcAutomaticTransfer.cpExistenRegistros);
}

// Detail
var cost_hidden = 0;
function GridviewDetailItem_OnBeginCallback(s, e) {
	e.customArgs["id_warehouse_exit"] = ComboBoxWarehouseExitEdit.GetValue();
	e.customArgs["id_warehouse_location_exit"] = ComboBoxWarehouseLocationExitEdit.GetValue();
	e.customArgs["id_warehouse_entry"] = ComboBoxWarehouseEntryEdit.GetValue();
	e.customArgs["id_warehouse_location_entry"] = ComboBoxWarehouseLocationEntryEdit.GetValue();

	e.customArgs["cost_hidden"] = cost_hidden;
	e.customArgs["enabled"] = s.cpEnabled;
	e.customArgs["id_itemType"] = ComboBoxItemType.GetValue() === undefined ? null : ComboBoxItemType.GetValue();
	e.customArgs["id_size"] = ComboBoxSize.GetValue() === undefined ? null : ComboBoxSize.GetValue();
	e.customArgs["id_trademark"] = ComboBoxTrademark.GetValue() === undefined ? null : ComboBoxTrademark.GetValue();
	e.customArgs["id_presentation"] = ComboBoxPresentation.GetValue() === undefined ? null : ComboBoxPresentation.GetValue();
	e.customArgs["codigoProducto"] = TextBoxCodigoProducto.GetText() === undefined ? null : TextBoxCodigoProducto.GetText();
	e.customArgs["categoriaProducto"] = ComboBoxItemGroupCategory.GetValue() === undefined ? null : ComboBoxItemGroupCategory.GetValue();
	e.customArgs["modeloProducto"] = ComboBoxItemTrademarkModel.GetValue() === undefined ? null : ComboBoxItemTrademarkModel.GetValue();

	let _itemReasonExitSelected = ComboBoxReasonExitEdit.GetSelectedItem();
	let tipovalorizacion = "";
	if (!(_itemReasonExitSelected == null || _itemReasonExitSelected == undefined)) {
		tipovalorizacion = _itemReasonExitSelected.GetColumnText(4);
	}
	e.customArgs["valorization"] = tipovalorizacion;

	let itemReasonExitSelected = ComboBoxReasonExitEdit.GetSelectedItem();
	let requiereUsuarioLote = "";
	let requiereSistemaLote = "";
	if (!(itemReasonExitSelected == null || itemReasonExitSelected == undefined)) {
		requiereUsuarioLote = itemReasonExitSelected.GetColumnText(2);
		requiereSistemaLote = itemReasonExitSelected.GetColumnText(3);
	}
	e.customArgs["requiereUsuarioLote"] = requiereUsuarioLote;
	e.customArgs["requiereSistemaLote"] = requiereSistemaLote;

	var fecha = DateTimeEmision.GetDate().toISOString();
	e.customArgs["fechaEmision"] = fecha;

}

function GridviewDetailItem_OnEndCallback(s, e) {
	DateTimeEmision.SetEnabled(!GridviewDetailItem.cpExistenRegistros);
}

function quantity_init(s, e) {
	cost_hidden = cost.GetValue();
}
function ComboBoxItemEdit_Init(s, e) {
	s.PerformCallback();
}
function ComboBoxItemEdit_BeginCallback(s, e) {
	e.customArgs["fechaEmision"] = DateTimeEmision.GetDate().toISOString();
	e.customArgs["id_warehouse_exit"] = ComboBoxWarehouseExitEdit.GetValue();
	e.customArgs["id_warehouse_location"] = ComboBoxWarehouseLocationExitEdit.GetValue();
	let itemReasonExitSelected = ComboBoxReasonExitEdit.GetSelectedItem();
	let requiereUsuarioLote = "";
	let requiereSistemaLote = "";
	e.customArgs["id_itemType"] = ComboBoxItemType.GetValue() === undefined ? null : ComboBoxItemType.GetValue();
	e.customArgs["id_size"] = ComboBoxSize.GetValue() === undefined ? null : ComboBoxSize.GetValue();
	e.customArgs["id_trademark"] = ComboBoxTrademark.GetValue() === undefined ? null : ComboBoxTrademark.GetValue();
	e.customArgs["id_presentation"] = ComboBoxPresentation.GetValue() === undefined ? null : ComboBoxPresentation.GetValue();
	e.customArgs["codigoProducto"] = TextBoxCodigoProducto.GetText() === undefined ? null : TextBoxCodigoProducto.GetText();
	e.customArgs["categoriaProducto"] = ComboBoxItemGroupCategory.GetValue() === undefined ? null : ComboBoxItemGroupCategory.GetValue();
	e.customArgs["modeloProducto"] = ComboBoxItemTrademarkModel.GetValue() === undefined ? null : ComboBoxItemTrademarkModel.GetValue();


	if (!(itemReasonExitSelected == null || itemReasonExitSelected == undefined)) {
		requiereUsuarioLote = itemReasonExitSelected.GetColumnText(2);
		requiereSistemaLote = itemReasonExitSelected.GetColumnText(3);
	}
	e.customArgs["requiereUsuarioLote"] = requiereUsuarioLote;
	e.customArgs["requiereSistemaLote"] = requiereSistemaLote;
	if (s.GetValue() != null) {
		e.customArgs["id_item_selected"] = s.GetValue();
	} else {
		e.customArgs["id_item_selected"] = s.cpValSelected;
	}
}
function ComboBoxItemEdit_EndCallback(s, e) {
	if (s.GetValue() == null) {
		if (s.cpValSelected != null) {
			s.SetValue(s.cpValSelected);
		}
	}
	if (s.GetValue() != null) {
		let selItem = s.GetSelectedItem();
	}
}
function ComboBoxItemEdit_SelIndexChanged(s, e) {
	let _id_metricUnitMoveTmp = 0;
	let itemReasonExitSelected = ComboBoxReasonExitEdit.GetSelectedItem();
	let _withLotSystem = false;
	let _withLotCustomer = false;

	let requiereUsuarioLote = "";
	let requiereSistemaLote = "";
	if (!(itemReasonExitSelected == null || itemReasonExitSelected == undefined)) {
		requiereUsuarioLote = itemReasonExitSelected.GetColumnText(2);
		requiereSistemaLote = itemReasonExitSelected.GetColumnText(3);
	}

	if (requiereUsuarioLote == "S") {
		_withLotCustomer = true;
	}
	if (requiereSistemaLote == "S") {
		_withLotSystem = true;
	}

	let _selIt = s.GetSelectedItem();
	if (_selIt != null) {
		id_MetricUnitInv.SetValue(_selIt.GetColumnText(2));
	}
	if (id_MetricUnitMov.GetValue() == null) {
		if (_selIt != null) {
			_id_metricUnitMoveTmp = _selIt.GetColumnText(2);
			id_MetricUnitMov.SetValue(_selIt.GetColumnText(2));
		}
	} else {
		_id_metricUnitMoveTmp = id_MetricUnitMov.GetValue();
	}
	id_lot.PerformCallback();

	var fecha = DateTimeEmision.GetDate().toISOString();
	var dato = {
		id_itemCurrent: s.GetValue(),
		id_metricUnitMove: _id_metricUnitMoveTmp,
		id_warehouse: ComboBoxWarehouseExitEdit.GetValue(),
		id_warehouseLocation: ComboBoxWarehouseLocationExitEdit.GetValue(),
		id_warehouseEntry: ComboBoxWarehouseEntryEdit.GetValue(),
		id_warehouseLocationEntry: ComboBoxWarehouseLocationEntryEdit.GetValue(),
		withLotSystem: _withLotSystem,
		withLotCustomer: _withLotCustomer,
		fechaEmision: fecha,
	};

	$.ajax({
		url: "AutomaticTransfer/InventoryMoveItemDetails",
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
				cost.SetValue(result.unitPriceMove);
				saldo.SetValue(result.remainingBalance);

				cost_hidden = result.unitPriceMove;
			}
		},
		complete: function () {
			hideLoading();
		}
	});
}
function btnUpdateDetail_Click(s, e) {
	var respuesta = ValidateSaveDetail();
	if (!respuesta.valid) {
		NotifyWarning(respuesta.mensaje);
		return;
	}

	// Valida Detalles
	if (!(GridviewDetailItem.cpGridVals == null ||
		GridviewDetailItem.cpGridVals == undefined
	)) {
		let arr = GridviewDetailItem.cpGridVals;
		if (arr.length > 0) {
			let _idItemTmp = id_Item.GetValue();

			let idEditing = GridviewDetailItem.cpEditingRowKey;

			if (idEditing > 0) {
				arr = arr.filter(f => f !== _idItemTmp);
			}
		}
	}
	GridviewDetailItem.UpdateEdit();
}
function btnCancelClick_Click(s, e) {
	GridviewDetailItem.CancelEdit();
}

function ComboBoxLotEdit_Init(s, e) {
	s.PerformCallback();
}

function ComboBoxLotEdit_BeginCallback(s, e) {
	e.customArgs["id_warehouse"] = ComboBoxWarehouseExitEdit.GetValue();
	e.customArgs["id_warehouse_location"] = ComboBoxWarehouseLocationExitEdit.GetValue();
	e.customArgs["id_item"] = id_Item.GetValue();

	e.customArgs["id_MetricUnitMov"] = id_MetricUnitMov.GetValue();
	e.customArgs["id_warehouse_entry"] = ComboBoxWarehouseEntryEdit.GetValue();
	e.customArgs["id_warehouse_location_entry"] = ComboBoxWarehouseLocationEntryEdit.GetValue();

	let itemReasonExitSelected = ComboBoxReasonExitEdit.GetSelectedItem();
	let requiereUsuarioLote = "";
	let requiereSistemaLote = "";
	if (!(itemReasonExitSelected == null || itemReasonExitSelected == undefined)) {
		requiereUsuarioLote = itemReasonExitSelected.GetColumnText(2);
		requiereSistemaLote = itemReasonExitSelected.GetColumnText(3);
	}
	e.customArgs["requiereUsuarioLote"] = requiereUsuarioLote;
	e.customArgs["requiereSistemaLote"] = requiereSistemaLote;

	var fechaEmision = DateTimeEmision.GetDate().toISOString();
	e.customArgs["fechaEmision"] = fechaEmision;
}
function ComboBoxLotEdit_EndCallback(s, e) {
	if (s.GetValue() == null) {
		if (s.cpValSelected != null) {
			s.SetValue(s.cpValSelected);
		}
	}
}

function ComboBoxLotEdit_SelectedIndexChanged(s, e) {
	var fecha = DateTimeEmision.GetDate().toISOString();
	var data = {
		id_item: id_Item.GetValue(),
		id_warehouse: ComboBoxWarehouseExitEdit.GetValue(),
		id_warehouseLocation: ComboBoxWarehouseLocationExitEdit.GetValue(),
		id_lot: s.GetValue(),
		idWarehouse: ComboBoxWarehouseExitEdit.GetValue(),
		fechaEmision: fecha,
	};

	$.ajax({
		url: "AutomaticTransfer/LotDetail",//UpdateWarehouseLocation",
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
				cost.SetValue(result.averagePrice);
				saldo.SetValue(result.remainingBalance);

				id_MetricUnitMov.SetValue(result.id_metricUnitMove);
			}
		},
		complete: function () {
		}
	});
}

function AddNewDetail(s, e) {
	GridviewDetailItem.AddNewRow();
}

function RefreshDetail(s, e) {
	GridviewDetailItem.PerformCallback();
}

// Validation
function ValidateSaveHeader() {
	let data = { valid: true, mensaje: "" };
	if (ComboBoxWarehouseExitEdit.GetValue() == null) {
		data.valid = false;
		data.mensaje = "Debe seleccionar la Bodega de Salida";
		return data;
	}
	if (ComboBoxWarehouseLocationExitEdit.GetValue() == null) {
		data.valid = false;
		data.mensaje = "Debe seleccionar la Ubicacion de Salida";
		return data;
	}
	if (ComboBoxWarehouseEntryEdit.GetValue() == null) {
		data.valid = false;
		data.mensaje = "Debe seleccionar la Bodega de Entrada";
		return data;
	}
	if (ComboBoxWarehouseLocationEntryEdit.GetValue() == null) {
		data.valid = false;
		data.mensaje = "Debe seleccionar la Ubicacion de Entrada";
		return data;
	}

	if (ComboBoxWarehouseExitEdit.GetValue() == ComboBoxWarehouseEntryEdit.GetValue()) {
		if (ComboBoxWarehouseLocationExitEdit.GetValue() == ComboBoxWarehouseLocationEntryEdit.GetValue()) {
			data.valid = false;
			data.mensaje = "Debe seleccionar una ubicación de Ingreso diferente a la de salida";
			return data;
		}
	}

	if (ComboBoxReasonExitEdit.GetValue() == null) {
		data.valid = false;
		data.mensaje = "Debe seleccionar el Motivo de salida";
		return data;
	}
	if (ComboBoxReasonEntryEdit.GetValue() == null) {
		data.valid = false;
		data.mensaje = "Debe seleccionar el Motivo de Entrada";
		return data;
	}
	if (ComboBoxCostCenterExitEdit.GetValue() == null) {
		data.valid = false;
		data.mensaje = "Debe seleccionar el Centro de Costo de Salida";
		return data;
	}
	if (ComboBoxCostCenterEntryEdit.GetValue() == null) {
		data.valid = false;
		data.mensaje = "Debe seleccionar el Centro de Costo de Entrada";
		return data;
	}
	if (ComboBoxSubCostCenterExitEdit.GetValue() == null) {
		data.valid = false;
		data.mensaje = "Debe seleccionar el Sub Centro de Costo de Salida";
		return data;
	}
	if (ComboBoxSubCostCenterEntryEdit.GetValue() == null) {
		data.valid = false;
		data.mensaje = "Debe seleccionar el Sub Centro de Costo de Entrada";
		return data;
	}

	if (DateTimeEmision.GetValue() === null) {
		data.valid = false;
		data.mensaje = "Fecha Emisión es un campo Obligatorio.";
		return data;
	} else {
		var aDateTimeEmision = DateTimeEmision.GetValue();
		var yearDateTimeEmision = aDateTimeEmision.getFullYear();
		var monthDateTimeEmision = aDateTimeEmision.getMonth() + 1;
		var dayDateTimeEmision = aDateTimeEmision.getDate();
		var dateTimeEmisionAux = new Date(yearDateTimeEmision, aDateTimeEmision.getMonth(), dayDateTimeEmision);

		var dateHoy = $('#dateHoy').val().split("-");
		var yearHoyDate = parseInt(dateHoy[2]);
		var monthHoyDate = parseInt(dateHoy[1]);
		var dayHoyDate = parseInt(dateHoy[0]);
		var hoyDateAux = new Date(yearHoyDate, monthHoyDate - 1, dayHoyDate);

		var dateHoyMin = $('#dateHoyMin').val().split("-");
		var yearHoyMinDate = parseInt(dateHoyMin[2]);
		var monthHoyMinDate = parseInt(dateHoyMin[1]);
		var dayHoyMinDate = parseInt(dateHoyMin[0]);
		var hoyMinDateAux = new Date(yearHoyMinDate, monthHoyMinDate - 1, dayHoyMinDate);

		if (dateTimeEmisionAux.getTime() !== hoyDateAux.getTime() && dateTimeEmisionAux.getTime() !== hoyMinDateAux.getTime() &&
			(dateTimeEmisionAux < hoyMinDateAux || dateTimeEmisionAux > hoyDateAux)) {
			data.valid = false;
			data.mensaje = "Fecha de emisión debe ser mayor o igual a fecha mínima y menor o igual a la fecha de hoy.";
			return data;
		}
	}

	return data;
}

function OnProcessPlantExitValidation(s, e) {
	var value = s.GetValue();
	if (value == null) {
		e.isValid = false;
		e.errorText = 'Campo obligatorio';
	}
}

function OnProcessPlantEntryValidation(s, e) {
	var value = s.GetValue();
	if (value == null) {
		e.isValid = false;
		e.errorText = 'Campo obligatorio';
	}
}


function ValidateSaveDetail() {
	let data = { valid: true, mensaje: "" };
	if (id_Item.GetValue() == null) {
		data.valid = false;
		data.mensaje = "Debe seleccionar el producto.";
		return data;
	}
	if (id_MetricUnitMov.GetValue() == null) {
		data.valid = false;
		data.mensaje = "Debe seleccionar la unidad de medida del movimiento.";
		return data;
	}
	let quantityTmp = quantity.GetValue();
	let saldoTmp = saldo.GetValue();

	if (!(quantityTmp > 0)) {
		data.valid = false;
		data.mensaje = "Debe indicar la cantidad a transferir.";
		return data;
	}

	var selWarehouseExit = ComboBoxWarehouseExitEdit.GetSelectedItem();

	if (selWarehouseExit != null) {
		let permiteValoresNegativos = selWarehouseExit.GetColumnText(1);
		if (!(permiteValoresNegativos == "S")) {
			if (quantityTmp > saldoTmp) {
				data.valid = false;
				data.mensaje = "La bodega seleccionada no acepta valores negativos.";
				return data;
			}
		}
	}

	let _itemReasonExitSelected = ComboBoxReasonExitEdit.GetSelectedItem();
	let requiereUsuarioLote = "";
	let requiereSistemaLote = "";
	if (!(_itemReasonExitSelected == null || _itemReasonExitSelected == undefined)) {
		requiereUsuarioLote = _itemReasonExitSelected.GetColumnText(2);
		requiereSistemaLote = _itemReasonExitSelected.GetColumnText(3);
	}

	if (requiereUsuarioLote == "S" || requiereSistemaLote == "S") {
		if (id_lot.GetValue() == null) {
			data.valid = false;
			data.mensaje = "Se requiere especificar un Lote";
			return data;
		}
	}

	var idProcessPlantExit = processPlantExit.GetValue();
	if (idProcessPlantExit == null) {
		data.valid = false;
		data.mensaje = "Proceso de envío es obligatorio.";
		return data;
	}

	var idProcessPlantEntry = processPlantEntry.GetValue();
	if (idProcessPlantEntry == null) {
		data.valid = false;
		data.mensaje = "Proceso de entrada es obligatorio.";
		return data;
	}

	return data;
}
///////////////////////////////////////////////////////////////////////////////////////////////////////// falta
function PrintItem() {
	var data = { id: $("#id_automatictransfer").val(), codeReport: "TRAEG" };
	$.ajax({
		url: 'AutomaticTransfer/PrintReportEGRING',
		data: data,
		async: true,
		cache: false,
		type: 'POST',
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			try {
				if (result !== undefined) {
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

function PrintItem2() {
	var data = { id: $("#id_automatictransfer").val(), codeReport: "TRAIN" };
	$.ajax({
		url: 'AutomaticTransfer/PrintReportEGRING2',
		data: data,
		async: true,
		cache: false,
		type: 'POST',
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			try {
				if (result !== undefined) {
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

function cost_init() {
	let _itemReasonExitSelected = ComboBoxReasonExitEdit.GetSelectedItem();
	let tipovalorizacion = "";
	if (!(_itemReasonExitSelected == null || _itemReasonExitSelected == undefined)) {
		tipovalorizacion = _itemReasonExitSelected.GetColumnText(4);
	}
	if (tipovalorizacion !== "Manual") {
		cost.SetEnabled(false);
	}
}

function DateTimeEmision_DateChanged(s, e) {
	if (GridviewDetailItem.IsEditing()) {
		GridviewDetailItem.PerformCallback();
	}
}

function InitializePagination() {
	if ($("#id_automatictransfer").val() !== 0) {
		var current_page = 1;
		var max_page = 1;
		$.ajax({
			url: "AutomaticTransfer/InitializePagination",
			type: "post",
			data: { id: $("#id_automatictransfer").val() },
			async: false,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
			success: function (result) {
				max_page = result.maximunPages;
				current_page = result.currentPage;
			},
			complete: function () {
			}
		});

		$('.pagination').jqPagination({
			current_page: current_page,
			max_page: max_page,
			page_string: "{current_page} de {max_page}",
			paged: function (page) {
				showPage("AutomaticTransfer/Pagination", { page: page });
			}
		});
	}
}

function Init() {
	$("#btnCollapseFilterDetails").click(function (event) {
		if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
		} else {
		}
	});

	if ($('#id_automatictransfer').val() !== 0 && $('#id_automatictransfer').val() !== "0") {
		$("#btnCollapseFilterDetails").click();
	}
}

$(function () {
	InitializePagination();
	Init();
});