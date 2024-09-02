
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
	showPage("OpeningClosingPlateLying/Edit", data);
}

function AddNewItem() {
	var data = {
		id: 0,
		enabled: true
	};
	showPage("OpeningClosingPlateLying/Edit", data);
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
	showPage("OpeningClosingPlateLying/Edit", data);
}

function AprovedItem() {
	if (rowKeySelected === undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var result = DevExpress.ui.dialog.confirm("Desea Aprobar la Tumbada de Placa Seleccionada?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'OpeningClosingPlateLying/Approve',
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

	var result = DevExpress.ui.dialog.confirm("Desea Reversar la Tumbada de Placa Seleccionada?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'OpeningClosingPlateLying/Reverse',
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

	var result = DevExpress.ui.dialog.confirm("Desea Anular la Tumbada de Placa Seleccionada?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'OpeningClosingPlateLying/Annul',
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
		reference: TextBoxReference.GetText(),
		id_responsable: ComboBoxResponsableIndex.GetValue(),
		id_freezerWarehouse: ComboBoxFreezerWarehouseIndex.GetValue(),
		id_freezerWarehouseLocation: ComboBoxFreezerWarehouseLocationIndex.GetValue(),
        id_boxedWarehouse: null,//ComboBoxBoxedWarehouseIndex.GetValue(),
        id_boxedWarehouseLocation: null,//ComboBoxBoxedWarehouseLocationIndex.GetValue(),
		numberLot: TextBoxNumberLot.GetText(),
		secTransLot: TextBoxSecTransaction.GetText(),
		items: TokenBoxItemsIndex.GetTokenValuesCollection()
	};

	showPartialPage($("#grid"), 'OpeningClosingPlateLying/SearchResult', data);
}

function print(codereport) {
	$.ajax({
		url: "OpeningClosingPlateLying/PrintReport",
		type: "post",
		data: {
			id: $("#id_openingClosingPlateLying").val(),
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
	print("FCAM05");
}
function PrintIngresosTransferencia() {
	print("RPTIT");
}
function PrintEgresosTransferencia() {
	print("IDMET");
}

$(function () {
	rowKeySelected = undefined;
});