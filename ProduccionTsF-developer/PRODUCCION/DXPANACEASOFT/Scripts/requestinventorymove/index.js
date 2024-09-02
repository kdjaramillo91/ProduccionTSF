// GRIDVIEW INDEX ALL REQUEST INVENTORY MOVE
function OnGridViewBeginCallBack(s, e) {
	e.customArgs["filterForm"] = GetDataFormFilter();
}

function OnGridViewInit(s, e) {
	UpdateTitlePanel();
}

var selectedRows = [];

function OnGridViewSelectionChanged(s, e) {
	UpdateTitlePanel();
	s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
	selectedRows = [];
	for (var i = 0; i < values.length; i++) {
		selectedRows.push(values[i]);
	}
}

function OnGridViewEndCallback() {
	UpdateTitlePanel();
}

function UpdateTitlePanel() {
	var selectedFilteredRowCount = GetSelectedFilteredRowCount();

	var text = "Total de elementos seleccionados: <b>" + gvRequestInventoryMoveAll.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gvRequestInventoryMoveAll.GetSelectedRowCount() - GetSelectedFilteredRowCount();

	if (hiddenSelectedRowCount > 0)
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	text += "<br />";
	$("#lblInfo").html(text);

	SetElementVisibility("lnkSelectAllRows", gvRequestInventoryMoveAll.GetSelectedRowCount() > 0 && gvRequestInventoryMoveAll.cpVisibleRowCount > selectedFilteredRowCount);
	SetElementVisibility("lnkClearSelection", gvRequestInventoryMoveAll.GetSelectedRowCount() > 0);

	btnApprove.SetEnabled(true);
	btnNew.SetEnabled(false);
	btnCopy.SetEnabled(false);
	btnAutorize.SetEnabled(false);
	btnProtect.SetEnabled(false);
	btnCancel.SetEnabled(false);
	btnRevert.SetEnabled(false);
	btnHistory.SetEnabled(false);
	btnPrint.SetEnabled(false);

}

function GetSelectedFilteredRowCount() {
	return gvRequestInventoryMoveAll.cpFilteredRowCountWithoutPage + gvRequestInventoryMoveAll.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
	var $element = $("#" + id);
	visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
	gvRequestInventoryMoveAll.UnselectRows();
}

function gvResultsSelectAllRows() {
	gvRequestInventoryMoveAll.SelectRows();
}

function GridViewRequestInventoryMoveButtonEdit_Click(s, e) {
	if (e.buttonID === "btnEditRow") {
		var data = {
			idRim: gvRequestInventoryMoveAll.GetRowKey(e.visibleIndex)
		};
		showPage("RequestInventoryMove/FormEditRequestInventoryMove", data);
	}
}
// FILTER FORM BUTTONS
function btnSearch_click(s, e) {
	var data = GetDataFormFilter();

	if (data !== null) {
		$.ajax({
			url: "RequestInventoryMove/RequesInventoryMoveResults",
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
				$("#btnCollapse").click();
				$("#results").html(result);
			},
			complete: function () {
				hideLoading();
			}
		});
	}
}

function btnClear_click(s, e) {
	idNatureMoveF.SetValue(null);
	idWarehouseF.SetValue(null);
	idPersonRequestF.SetValue(null);
	startEmissionDate.SetValue(null);
	endEmissionDate.SetValue(null);
}

function AddNewRequestInventoryMoveManual() {
}

function GetDataFormFilter() {
	return data = {
		idNatureMove: idNatureMoveF.GetValue(),
		idDocumentState: EstadoQueryComboBox.GetValue(),
		idWarehouse: idWarehouseF.GetValue(),
		idPersonRequest: idPersonRequestF.GetValue(),
		startEmissionDate: startEmissionDate.GetDate() != null ? startEmissionDate.GetDate().toJSON() : null,
		endEmissionDate: endEmissionDate.GetDate() != null ? endEmissionDate.GetDate().toJSON() : null,
	};
}

// MAIN FUNCTIONS
function init() {
	$("#btnCollapse").click(function (event) {
		if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
			$("#results").css("display", "");
		} else {
			$("#results").css("display", "none");
		}
	});
}

function AddNewDocument() {
}

function CopyDocument() {
}

function ApproveDocuments() {
	showConfirmationDialog(function () {
		gvRequestInventoryMoveAll.GetSelectedFieldValues("id", function (values) {
			var selectedRows = [];

			for (var i = 0; i < values.length; i++) {
				selectedRows.push(values[i]);
			}

			var data = {
				ids: selectedRows
			};
			$.ajax({
				url: "RequestInventoryMove/ApproveIds",
				type: "post",
				data: { ids: selectedRows },
				async: true,
				cache: false,
				error: function (error) {
					console.log(error);
				},
				beforeSend: function () {
					showLoading();
				},
				success: function (result) {
					if (result != undefined && result != null) {
						if (result.hasError != undefined && result.hasError == "Y") {
							//gridMessageErrorRequestInventoryMove.SetText(ErrorMessage(result.message));
							//$("#GridMessageErrorRequestInventoryMove").show();
						}
						else {
							$("#mainform").html(result);
						}
					}
					console.log("success");
					// 
					gvRequestInventoryMoveAll.Refresh();
				},
				complete: function () {
					console.log("complete");
					hideLoading();
					// 
					gvRequestInventoryMoveAll.Refresh();
				}
			});
		});
	}, "¿Desea aprobar los lotes seleccionados?");
}

function AutorizeDocuments() {
}

function ProtectDocuments() {
}

function CancelDocuments() {
}

function RevertDocuments() {
}

function ShowHistory() {
}

function Print() {
}

$(function () {
	init();
});
