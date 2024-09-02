
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
		id_turn: 0,
		emissionDate: null,
		id_machineForProd: 0,
		enabled: false
	};
	showPage("NonProductiveHour/Edit", data);
}

function AddNewItem() {
	$.ajax({
		url: "NonProductiveHour/PendingNew",
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
		id_turn: 0,
		emissionDate: null,
		id_machineForProd: 0,
		enabled: true
	};
	showPage("NonProductiveHour/Edit", data);
}

function AprovedItem() {
	if (rowKeySelected == undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var result = DevExpress.ui.dialog.confirm("Desea Aprobar el control de horas de trabajo por máquina Seleccionado?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'NonProductiveHour/Approve',
				type: 'post',
				data: { id: rowKeySelected },
				async: true,
				cache: false,
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error.</br>" + result.Message);
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

	var result = DevExpress.ui.dialog.confirm("Desea Reversar el control de horas de trabajo por máquina Seleccionado?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'NonProductiveHour/Reverse',
				type: 'post',
				data: { id: rowKeySelected },
				async: true,
				cache: false,
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error.</br>" + result.Message);
						return;
					}

					NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
					RefreshGrid();
				},
				error: function (result) {
					hideLoading();
				},
			});
		}
	});
}

function AnnulItem() {
	if (rowKeySelected == undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var result = DevExpress.ui.dialog.confirm("Desea Anular el control de horas de trabajo por máquina Seleccionado?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'NonProductiveHour/Annul',
				type: 'post',
				data: { id: rowKeySelected },
				async: true,
				cache: false,
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error.</br>" + result.Message);
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
		id_turn: ComboBoxTurnIndex.GetValue(),
		id_machineForProd: ComboBoxMachineForProdIndex.GetValue()
	};

	showPartialPage($("#grid"), 'NonProductiveHour/SearchResult', data);
}

function PrintItem() {
	$.ajax({
		url: "NonProductiveHour/NonProductiveHourReport",
		type: "post",
		data: {
			id: $("#id_nonProductiveHour").val(),
			codeReport: "RNPH"
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

$(function () {
	rowKeySelected = undefined;
});