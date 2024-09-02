// Métodos de filtros para búsqueda
var performQuery = function () {
	var queryData = $("#formFilterProductionLotFinalize").serialize();
	if (queryData !== null) {
		$.ajax({
			url: "productionLotFinalize/ProductionLotFinalizeResults",
			type: "post",
			data: queryData,
			async: true,
			cache: false,
			error: function (error) {
				console.log(error);
				showErrorMessage(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if ($("#query-filters").is(":visible")) {
					$("#btnCollapse").click();
				}
				$("#query-results").html(result);
			},
			complete: function () {
				hideLoading();
			}
		});
	}
};


// Métodos de Filtro
var UpdateTitlePanel = function (gridView) {
	var text = "Total de elementos seleccionados: <b>" + gridView.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gridView.GetSelectedRowCount() - GetSelectedFilteredRowCount(gridView);
	if (hiddenSelectedRowCount > 0) {
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	}
	text += "<br />";
	$("#lblInfo").html(text);
};
var GetSelectedFilteredRowCount = function (gridView) {
	return gridView.cpFilteredRowCountWithoutPage + gridView.GetSelectedKeysOnPage().length;
};

function onSearchQueryButtonClick() {
	performQuery();
};
function onClearQueryButtonClick() {
	id_documentState.SetValue(null);
	number.SetText(null);
	numberLot.SetText(null);
	reference.SetText(null);

	startEmissionDate.SetValue(null);
	endEmissionDate.SetValue(null);
};
function OnRangeEmissionDateQueryValidation() {
	OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de fecha no es válido");
};
function onAddNewQueryButtonClick() {
	performQueryLotesCerrar();
};
function onRefreshQueryToolbarButtonClick() {
	gvProductionLotFinalizeResult.PerformCallback();
};
function OnQueryGridViewInit(s, e) {
	UpdateTitlePanel(s);
};
function OnQueryGridViewSelectionChanged(s, e) {
	UpdateTitlePanel(s);
};
function OnQueryGridViewBeginCallback(s, e) {
	e.customArgs["id_documentState"] = s.cpId_documentState;
	e.customArgs["number"] = s.cpNumber;
	e.customArgs["reference"] = s.cpReference;
	e.customArgs["numberLot"] = s.cpNumberLot;
	e.customArgs["startEmissionDate"] = s.cpStartEmissionDate;
	e.customArgs["endEmissionDate"] = s.cpEndEmissionDate;
	e.customArgs["isCallback"] = true;
};
function OnQueryGridViewEndCallback(s, e) {
	UpdateTitlePanel();
};
function OnQueryGridViewCustomButtonClick(s, e) {
	if (e.buttonID === "btnEditRow") {
		//showEditForm(s.GetRowKey(e.visibleIndex));
	}
};
function AddNewDocument(s, e) {
}
function SaveDocument(s, e) {
}
function CopyDocument(s, e) {
}
function ApproveDocument(s, e) {
}
function AutorizeDocument(s, e) {
}
function ProtectDocument(s, e) {
}
function CancelDocument(s, e) {
}
function RevertDocument(s, e) {
}
function ShowDocumentHistory(s, e) {
}
function OnReportButtonClick(s, e) {
	var idproductLotFinalize = parseInt($("#id_productLotFinalize").val());
	var data = { idProductionLotFinalize: idproductLotFinalize};

	$.ajax({
		url: 'productionLotFinalize/PrintProductionLotFinalizeReport',
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

function OnGenerateExcelReportButtonClick(s, e) {
}

var OnUpdateButtonClick = function () {
	PerformDocumentStoreAction(false);
};

var OnApproveButtonClick = function () {
	PerformDocumentAproveAction();
};

var OnCancelDocumentButtonClick = function () {
	PerformDocumentStateAction("Cancel");
};

var OnRevertButtonClick = function () {
	PerformDocumentStateAction("Revert");
};
var OnCancelButtonClick = function (id) {
	showPage("ProductionLotFinalize/Index", null);
};
var OnRefreshButtonClick = function () {
	var id = parseInt($("#id_productLotFinalize").val());
	showPage("productionLotFinalize/EditForm", { id: id });
};

var PerformDocumentStateAction = function (action) {
	var idproductLotFinalize = parseInt($("#id_productLotFinalize").val());
	var actionUrl = "ProductionLotFinalize/" + action;

	var operationData = {
		idProductionLotFinalize: idproductLotFinalize
	};

	$.ajax({
		url: actionUrl,
		type: "post",
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		data: JSON.stringify(operationData),
		async: true,
		cache: false,
		error: function (error) {
			showErrorMessage(error);
			hideLoading();
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if (result.isValid) {
				var successData = {
					id: result.idProductionLotFinalize,
					successMessage: result.message
				};
				showPage("ProductionLotFinalize/EditForm", successData);
				showSuccessMessage(result.message);
			} else {
				showWarningMessage(result.message);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
};
var PerformDocumentStoreAction = function () {
	if (IsValidEditForm()) {
		var idproductLotFinalize = parseInt($("#id_productLotFinalize").val());
		var actionUrl = (idproductLotFinalize === 0) ? "ProductionLotFinalize/Create" : "ProductionLotFinalize/Save";
		
		var operationData = {
			idProductionLotFinalize: idproductLotFinalize,
			emissionDate: emissionDate.GetDate(),
			description: description.GetText(),
			reference: reference.GetText(),
			idsLote: tpProductionLotFinalizeEdit.cpIdsLote
		};

		$.ajax({
			url: actionUrl,
			type: "post",
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: JSON.stringify(operationData),
			async: true,
			cache: false,
			error: function (error) {
				showErrorMessage(error);
				hideLoading();
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result.isValid) {
					var successData = {
						id: result.idProductionLotFinalize,
						successMessage: result.message
					};
					showPage("ProductionLotFinalize/EditForm", successData);
					showSuccessMessage(result.message);
				} else {
					showWarningMessage(result.message);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
};

var PerformDocumentAproveAction = function () {
	if (IsValidEditForm()) {
		var idproductLotFinalize = parseInt($("#id_productLotFinalize").val());

		var operationData = {
			idProductionLotFinalize: idproductLotFinalize
		};

		$.ajax({
			url: "productionLotFinalize/Aprove",
			type: "post",
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: JSON.stringify(operationData),
			async: true,
			cache: false,
			error: function (error) {
				showErrorMessage(error);
				hideLoading();
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result.isValid) {
					var successData = {
						id: result.idProductionLotFinalize,
						successMessage: result.message
					};
					showPage("ProductionLotFinalize/EditForm", successData);
					showSuccessMessage(result.message);
				} else {
					showWarningMessage(result.message);
				}
			},
			complete: function () {
				hideLoading();
			}
		});
	}
};

// Funciones para la consulta de lotes nuevos
var performQueryLotesCerrar = function () {
	$.ajax({
		url: "productionLotFinalize/ProductionLotPendingCloseResults",
		type: "post",
		//data: queryData,
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
			showErrorMessage(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if ($("#query-filters").is(":visible")) {
				$("#btnCollapse").click();
			}
			$("#query-results").html(result);
		},
		complete: function () {
			hideLoading();
		}
	});
};

var prepararIdsLotPendingClose = function (selectedKeys) {
	gvLotePendienteCierreResult.cpIdsLote = null;
	gvLotePendienteCierreResult.cpErrorMessage = null;
	gvLotePendienteCierreResult.cpIsValid = false;
	if (selectedKeys !== null && selectedKeys.length > 0) {
		// Validamos que no existan lotes diferentes
		var numeroLote = null;
		var estado = "CONCILIADO"; 
		var idsLote = [];
		var isValid = true;

		for (var i = 0; i < selectedKeys.length; i++) {
			var selectedKeysPart = selectedKeys[i];

			// Asignamos el primero lote
			if (numeroLote == null) {
				numeroLote = selectedKeysPart[1].substring(0,5);
			}
			if (numeroLote != selectedKeysPart[1].substring(0, 5)) {
				isValid = false;
				idsLote = null;
				gvLotePendienteCierreResult.cpErrorMessage = 'El cierre debe realizarse a un mismo número de lote.';
				break;
			} else if (estado != selectedKeysPart[2]) {
				isValid = false;
				gvLotePendienteCierreResult.cpErrorMessage = 'El lote debe de ser estado CONCILIADO.';
				break;
			}

			var agrupacionLotes = gvLotePendienteCierreResult.cpAgrupacionLotes;
			for (var j = 0; j < agrupacionLotes.length; j++){
				var agrupacionLote = agrupacionLotes[j];
				if (numeroLote == agrupacionLote.LoteJuliano && selectedKeys.length != agrupacionLote.NumerosLote) {
					isValid = false;
					gvLotePendienteCierreResult.cpErrorMessage = 'Debe seleccionar todos los lotes correspondientes a la secuencia ' + numeroLote + ' con ' + agrupacionLote.NumerosLote + ' registros.';
					break;
				}
			}

			// Asignamos las ids a la lista
			idsLote.push(selectedKeysPart[0]);
			numeroLote = selectedKeysPart[1].substring(0, 5);
			estado = selectedKeysPart[2];
		}

		gvLotePendienteCierreResult.cpIdsLote = idsLote;
		gvLotePendienteCierreResult.cpIsValid = isValid;
	} else {
		gvLotePendienteCierreResult.cpErrorMessage = 'No se han seleccionado lotes.';
	}

	// Procesamos
	if (gvLotePendienteCierreResult.cpIsValid) {
		generateNewProductionLotFinalize(gvLotePendienteCierreResult.cpIdsLote);
	}
	else {
		showWarningMessage(gvLotePendienteCierreResult.cpErrorMessage);
	}
};

function onAddNewCloseEditButtonClick(s, e) {
	debugger;
	gvLotePendienteCierreResult.GetSelectedFieldValues("IdLote;NumeroLote;Estado", prepararIdsLotPendingClose);

};

function OnProductionLotPendingCloseGridViewInit(s, e) {
	UpdateTitlePanel(s);
};
function OnProductionLotPendingCloseGridViewSelectionChanged(s, e) {
	UpdateTitlePanel(s);
};
function OnProductionLotPendingCloseGridViewBeginCallback(s, e) {
	e.customArgs["isCallback"] = true;

};
function OnProductionLotPendingCloseGridViewEndCallback(s, e) {
	UpdateTitlePanel(s);
};
function OnProductionLotPendingCloseGridViewCustomButtonClick(s, e) {
};

var OnTabPageControlInit = function (s, e) {
	if (s.cpEditMessage) {
		showSuccessMessage(s.cpEditMessage);
	}
};

function ProductionLotFinalizeSelectAllRows() {
	debugger;
	var a = gvLotePendienteCierreResult.SelectRows();
	gvLotePendienteCierreResult.SelectRows();
}

function ProductionLotFinalizeClearSelection() {
	gvLotePendienteCierreResult.UnselectRows();
}

// Funciones para métodos de edición
var showEditForm = function (id) {
	var userData = {
		id: id
	};
	showPage("productionLotFinalize/EditForm", userData);
};
var generateNewProductionLotFinalize = function (idsLote) {
	var userData = {
		idsLote: idsLote
	};
	showPage("productionLotFinalize/GenerarNuevo", userData);
};
var OnGridViewCustomButtonClick = function (s, e) {
	if (e.buttonID === "btnEditRow") {
		showEditForm(s.GetRowKey(e.visibleIndex));
	}
};

// Funciones principales
var init = function () {
	$("#btnCollapse").click(function (event) {
		if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
			$("#query-results").css("display", "");
		} else {
			$("#query-results").css("display", "none");
		}
	});
};

$(function () {
	init();
});

// Acciones del formulario
var IsValidEditForm = function () {
	var isDocumentValid = ASPxClientEdit.ValidateEditorsInContainerById("datosDocumentEditForm", "", true);
	if (isDocumentValid) {
		UpdateTabValidationImage(0, true);
	} else {
		UpdateTabValidationImage(0, false);
		tpProductionLotFinalizeEdit.SetActiveTabIndex(0);
		return false;
	}

	return true;
};

function OnGridViewKardex_BeginCallback(s, e) {
	var emissionDt = emissionDate.GetDate();
	var fecha = emissionDt != null
		? emissionDt.toISOString().substring(0, 10)
		: null;

	e.customArgs["emissionDate"] = fecha;
	e.customArgs["internalNumberLot"] = tpProductionLotFinalizeEdit.cpNumeroLote;
	e.customArgs["idproductLotFinalize"] = parseInt($("#id_productLotFinalize").val());
}
// Métodos generales

var UpdateTabValidationImage = function (tabIndex, isValid) {
	var imageUrl = isValid
		? "/Content/image/noimage.png"
		: "/Content/image/info-error.png";

	var tab = tpProductionLotFinalizeEdit.GetTab(tabIndex);
	tab.SetImageUrl(imageUrl);
	tab.SetActiveImageUrl(imageUrl);
}

//Metodos de Validación
var OnEmissionDateProductionLotFinalizeValidation = function (s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};


