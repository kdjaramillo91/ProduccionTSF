
var rowKeySelected;

function OnGridFocusedRowChanged(s, e) {
	s.GetRowValues(e.visibleIndex, 'id;canEdit;canAproved;canReverse;canAnnul', OnGetRowValues);
}

function OnGetRowValues(values) {

	rowKeySelected = values[0];
	var canEdit = values[1];
	var canAproved = values[2];
	var canReverse = values[3];
	var canAnnul = values[4];

	if (canEdit) {
		btnEdit.SetEnabled(true);
	}
	else {
		btnEdit.SetEnabled(false);
	}

	if (canAproved) {
		btnAproved.SetEnabled(true);
	}
	else {
		btnAproved.SetEnabled(false);
	}

	if (canReverse) {
		btnReverse.SetEnabled(true);
	}
	else {
		btnReverse.SetEnabled(false);
	}

	if (canAnnul) {
		btnAnnul.SetEnabled(true);
	}
	else {
		btnAnnul.SetEnabled(false);
	}
}

function GridViewBtnShow_Click(s, e) {
	s.GetRowValues(e.visibleIndex, 'id', EditItem);
}

function EditItem(values) {
	var data = {
		id: values,
		ids: [],
		enabled: false
	};
	showPage("InventoryMovePlantTransfer/Edit", data);
}

function AddNewItem() {
	$.ajax({
		url: "InventoryMovePlantTransfer/PendingNew",
		type: "post",
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			//// 
			NotifyError("Error. " + error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			$("#maincontent").html(result);
		},
		complete: function () {
			hideLoading();
		}
	});
}

function EditCurrentItem() {
	if (rowKeySelected == undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var data = {
		id: rowKeySelected,
		ids: [],
		enabled: true
	};
	showPage("InventoryMovePlantTransfer/Edit", data);
}

function AprovedItem() {
	if (rowKeySelected == undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var result = DevExpress.ui.dialog.confirm("Desea Aprobar la Transferencia de Planta Seleccionada?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'InventoryMovePlantTransfer/Approve',
				type: 'post',
				data: { id: rowKeySelected },
				async: true,
				cache: false,
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error. " + result.Message);
						return;
					}

					NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
					RefreshGrid();
				},
				error: function (result) {
					hideLoading();
				}
			});
		}
	});
}

function ReverseItem() {
	if (rowKeySelected == undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var result = DevExpress.ui.dialog.confirm("Desea Reversar la Transferencia de Planta Seleccionada?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'InventoryMovePlantTransfer/Reverse',
				type: 'post',
				data: { id: rowKeySelected },
				async: true,
				cache: false,
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error. " + result.Message);
						return;
					}

					NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
					RefreshGrid();
				},
				error: function (result) {
					hideLoading();
				}
			});
		}
	});
}

function AnnulItem() {
	if (rowKeySelected == undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var result = DevExpress.ui.dialog.confirm("Desea Anular la Transferencia de Planta Seleccionada?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'InventoryMovePlantTransfer/Annul',
				type: 'post',
				data: { id: rowKeySelected },
				async: true,
				cache: false,
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error. " + result.Message);
						return;
					}

					NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
					RefreshGrid();
				},
				error: function (result) {
					hideLoading();
				}
			});
		}
	});
}

function RefreshGrid() {
	var dateInicio = DateEditInit.GetDate();
	var yearInicio = dateInicio.getFullYear();
	var monthInicio = dateInicio.getMonth() + 1;
	var dayInicio = dateInicio.getDate();

	var dateFin = DateEditEnd.GetDate();
	var yearFin = dateFin.getFullYear();
	var monthFin = dateFin.getMonth() + 1;
	var dayFin = dateFin.getDate();

	var data = {
		initDate: dayInicio + "/" + monthInicio + "/" + yearInicio,
		endtDate: dayFin + "/" + monthFin + "/" + yearFin,
		id_state: ComboBoxState.GetValue(),
		number: TextBoxNumber.GetText(),
		reference: TextBoxReference.GetText(),
		id_warehouseEntry: ComboBoxWarehouseEntryIndex.GetValue(),
		id_warehouseLocationEntry: ComboBoxWarehouseLocationEntryIndex.GetValue(),
		id_inventoryReason: ComboBoxInventoryReasonIndex.GetValue(),
		id_receiver: ComboBoxReceiverIndex.GetValue(),
		numberLot: TextBoxNumberLot.GetText(),
		secTransaction: TextBoxSecTransaction.GetText(),
		id_processType: ComboBoxProcessTypeIndex.GetValue(),
		id_provider: ComboBoxProviderIndex.GetValue(),
		id_machineForProd: ComboBoxMachineForProdIndex.GetValue(),
		id_turn: ComboBoxTurnIndex.GetValue(),
		id_productionCart: ComboBoxProductionCartIndex.GetValue()
	};

	showPartialPage($("#grid"), 'InventoryMovePlantTransfer/SearchResult', data);
}

function PrintItem() {

}

$(function () {
	rowKeySelected = undefined;
});