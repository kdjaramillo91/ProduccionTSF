
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
		enabled: false
	};
	showPage("Mastered/Edit", data);
}

function AddNewItem() {
	var data = {
		id: 0,
		enabled: true
	};
	showPage("Mastered/Edit", data);
}

function EditCurrentItem() {
	if (rowKeySelected === undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var data = {
		id: rowKeySelected,
		enabled: true
	};
	showPage("Mastered/Edit", data);
}

function AprovedItem() {
	if (rowKeySelected === undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

    var result = DevExpress.ui.dialog.confirm("Desea Aprobar el Masterizado Seleccionado?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'Mastered/Approve',
				type: 'post',
				data: { id: rowKeySelected },
				async: true,
				cache: false,
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error." + result.Message);
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
	if (rowKeySelected === undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

    var result = DevExpress.ui.dialog.confirm("Desea Reversar el Masterizado Seleccionado?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'Mastered/Reverse',
				type: 'post',
				data: { id: rowKeySelected },
				async: true,
				cache: false,
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error." + result.Message);
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
	if (rowKeySelected === undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

    var result = DevExpress.ui.dialog.confirm("Desea Anular el Masterizado Seleccionado?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'Mastered/Annul',
				type: 'post',
				data: { id: rowKeySelected },
				async: true,
				cache: false,
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error." + result.Message);
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
        id_responsable: ComboBoxResponsableIndex.GetValue(),
        id_turn: ComboBoxTurnIndex.GetValue(),
        id_boxedWarehouse: ComboBoxBoxedWarehouseIndex.GetValue(),
        id_boxedWarehouseLocation: ComboBoxBoxedWarehouseLocationIndex.GetValue(),
        boxedItems: TokenBoxBoxedItemsIndex.GetTokenValuesCollection(),
        boxedNumberLot: TextBoxBoxedNumberLot.GetText(),
        id_masteredWarehouse: ComboBoxMasteredWarehouseIndex.GetValue(),
        id_masteredWarehouseLocation: ComboBoxMasteredWarehouseLocationIndex.GetValue(),
        masteredItems: TokenBoxMasteredItemsIndex.GetTokenValuesCollection(),
        masteredNumberLot: TextBoxMasteredNumberLot.GetText(),
    };

	showPartialPage($("#grid"), 'Mastered/SearchResult', data);
}

function print(codereport) {
	$.ajax({
		url: "Mastered/PrintReportEgreso",
		type: "post",
		data: {
			id: $("#id_mastered").val(),
			codeReport: codereport
		},
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
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
function PrintItem() {
	print("MASGEN");
}
function printIngresoBodega() {
	print("MASING");
}
function PrintEgresosTransferencia() {
	print("MASEGR");
}
function PrintEgresosPers() {
	print("MASPER");
}
function PrintResumen() {
	print("MAREEN");
}
function PrintResumenTipo() {
	print("MARETP");
}

$(function () {
	rowKeySelected = undefined;
});