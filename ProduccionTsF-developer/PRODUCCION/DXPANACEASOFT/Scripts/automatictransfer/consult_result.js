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
	if (rowKeySelected === undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var data = {
		id: rowKeySelected,
		enabled: true
	};
	showPage("AutomaticTransfer/Edit", data);
}

function AprovedItem() {
	if (rowKeySelected === undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var result = DevExpress.ui.dialog.confirm("Desea Aprobar la Transferencia Automática?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'AutomaticTransfer/Approve',
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
					hideLoading();
					NotifySuccess("Proceso realizado Satisfactoriamente. ");
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

	var result = DevExpress.ui.dialog.confirm("Desea Reversar la Transferencia Automática?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'AutomaticTransfer/Reverse',
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
					hideLoading();
					NotifySuccess("Proceso realizado Satisfactoriamente. ");
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

	var result = DevExpress.ui.dialog.confirm("Desea Anular la Transferencia Automática?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'AutomaticTransfer/Annul',
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
					hideLoading();
					NotifySuccess("Proceso realizado Satisfactoriamente. ");
					RefreshGrid();
				},
				error: function (result) {
					hideLoading();
				}
			});
		}
	});
}

function PrintItem() {
}

function PrintItem2() {
}

function print(codereport) {
	
}

function RefreshGrid() {
	
	let data = GetDataFilterForConsultResultList();
	showPartialPage($("#grid"), 'AutomaticTransfer/SearchResult', data);
}

$(function () {
	rowKeySelected = undefined;
});