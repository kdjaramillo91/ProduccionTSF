// Métodos de filtros para búsqueda
var performQuery = function () {
	var queryData = $("#formFilterCostPerContainer").serialize();
	if (queryData !== null) {
		$.ajax({
			url: "CostPerContainer/CostPerContainerResults",
			type: "post",
			data: queryData,
			async: true,
			cache: false,
			error: function (error) {
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
var UpdateTitlePanel = function () {
	var text = "Total de elementos seleccionados: <b>" + gvCostPerContenedorResult.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gvCostPerContenedorResult.GetSelectedRowCount() - GetSelectedFilteredRowCount();
	if (hiddenSelectedRowCount > 0) {
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	}
	text += "<br />";
	$("#lblInfo").html(text);
};
var GetSelectedFilteredRowCount = function () {
	return gvCostPerContenedorResult.cpFilteredRowCountWithoutPage + gvCostPerContenedorResult.GetSelectedKeysOnPage().length;
};

function onSearchQueryButtonClick() {
	performQuery();
};
function onClearQueryButtonClick() {
	id_documentState.SetValue(null);
	number.SetText(null);
	reference.SetText(null);
	año.SetValue(null);
	mes.SetValue(null);

	startEmissionDate.SetValue(null);
	endEmissionDate.SetValue(null);
};
function OnRangeEmissionDateQueryValidation() {
	OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de fecha no es válido");
};
function onAddNewQueryButtonClick() {
	showEditForm(null);
};
function onRefreshQueryToolbarButtonClick() {
	gvCostPerContenedorResult.PerformCallback();
};
function OnQueryGridViewInit(s, e) {
	UpdateTitlePanel();
};
function OnQueryGridViewSelectionChanged(s, e) {
	UpdateTitlePanel();
};
function OnQueryGridViewBeginCallback(s, e) {
	e.customArgs["id_documentState"] = s.cpId_documentState;
	e.customArgs["number"] = s.cpNumber;
	e.customArgs["reference"] = s.cpReference;
	e.customArgs["año"] = s.cpAnio;
	e.customArgs["mes"] = s.cpMes;
	e.customArgs["startEmissionDate"] = s.cpStartEmissionDate;
	e.customArgs["endEmissionDate"] = s.cpEndEmissionDate;
	e.customArgs["isCallback"] = true;
};
function OnQueryGridViewEndCallback(s, e) {
	UpdateTitlePanel();
};
function OnQueryGridViewCustomButtonClick(s, e) {
	if (e.buttonID === "btnEditRow") {
		showEditForm(s.GetRowKey(e.visibleIndex));
	}
};


// Funciones para métodos de edición
var showEditForm = function (id) {
	var userData = {
		id: id
	};
	showPage("CostPerContainer/EditForm", userData);
};
var PerformDocumentStoreAction = function () {
	if (IsValidEditForm()) {
		var idCostPerContainer = parseInt($("#id_costPerContainer").val());
		var actionUrl = "CostPerContainer/Save";

		var emissionDt = emissionDate.GetDate();
		var emissionIsoDt = emissionDt != null
			? emissionDt.toISOString().substring(0, 10)
			: null;

		var operationData = {
			idCostPerContainer: idCostPerContainer,
			año: año.GetValue(),
			mes: mes.GetValue(),
			emissionDate: emissionIsoDt,
			description: description.GetText(),
			reference: reference.GetText(),
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
						id: result.idCostPerContainer,
						successMessage: result.message
					};
					showPage("CostPerContainer/EditForm", successData);

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
var updateCostManualPoundDetails = function () {
	var idCostPerContainer = parseInt($("#id_costPerContainer").val());
	var operationData = {
		idCostPerContainer: idCostPerContainer,
		detalles: _detalles,
	};

	$.ajax({
		url: "CostPerContainer/UpdateCostContenedorManualPoundDetailTempData",
		type: "post",
		contentType: 'application/json; charset=utf-8',
		dataType: 'json',
		data: JSON.stringify(operationData),
		async: true,
		cache: false,
		error: function (error) {
		},
	});
}
var OnRefreshButtonClick = function () {
	var id = parseInt($("#id_costPerContainer").val());
	showPage("CostPerContainer/EditForm", { id: id });
};
var OnApproveButtonClick = function () {
	PerformDocumentStoreAction(true);
};
var OnCancelDocumentButtonClick = function () {
	PerformDocumentStateAction("Cancel");
};
var OnRevertButtonClick = function () {
	PerformDocumentStateAction("Revert");
};
var OnCancelButtonClick = function (id) {
	showPage("CostPerContainer/Index", null);
};

var OnAnioCostPerContainerSelectedIndexChanged = function (s, e) {
	mes.ClearItems();
	var añoValor = año.GetValue();
	if (añoValor !== null) {
		var periodosUsables = año.cpPeriodosUsables;
		var numPeriodos = periodosUsables.length;
		for (var i = 0; i < numPeriodos; i++) {
			var periodo = periodosUsables[i];
			if (periodo.Anio === añoValor) {
				mes.AddItem([periodo.Mes, periodo.Mes], periodo.Mes);
			}
		}
	}

	mes.Validate();

	// Ejecutamos el callback del detalle Manual
	gvCostContenedorManualPoundDetail.PerformCallback();
	// Limpiamos el procesamiento anterior
	$("#detallesEjecucion").empty();
};

var OnMesCostPerContainerSelectedIndexChanged = function (s, e) {
	mes.Validate();

	// Ejecutamos el callback del detalle Manual
	gvCostContenedorManualPoundDetail.PerformCallback();
	// Limpiamos el procesamiento anterior
	$("#detallesEjecucion").empty();

};
var OnMesCostPerContainerValidation = function (s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else if (año.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};

var OnEmissionDateCostPerContainerValidation = function (s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
};


function OnTabPageControlInit(s, e) {
	if (s.cpEditMessage) {
		showSuccessMessage(s.cpEditMessage);
	}
};
function OnUpdateButtonClick(s, e) {
	PerformDocumentStoreAction(false);
};
function OnCancelButtonClick(s, e) {
	showPage("CostPerContainer/Index", null);
};

// Funciones del procesamiento
var onProcesarCostoBodegaButtonClick = function (s, e) {
	if (IsValidEditForm()) {
		var operationData = {
			idCostPerContainer: parseInt($("#id_costPerContainer").val()),
			año: año.GetValue(),
			mes: mes.GetValue(),
			detalles: _detalles,
		};

		var hideLoadingDialog = true;
		$.ajax({
			url: "CostPerContainer/Process",
			type: "post",
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: JSON.stringify(operationData),
			async: true,
			cache: false,
			error: function (error) {
				console.error(error);
				hideLoading();
				showErrorMessage(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result.isValid) {
					hideLoadingDialog = false;
					$("#detallesEjecucion").empty();

					var viewData = {
						idCostPerContainer: result.idCostPerContainer
					};

					$.ajax({
						url: "CostPerContainer/CostPerContainerDetails",
						type: "post",
						data: viewData,
						async: true,
						cache: false,
						error: function (error) {
							showErrorMessage(error);
							console.log(error);
						},
						beforeSend: function () {
							showLoading();
						},
						success: function (result) {
							$("#detallesEjecucion").html(result);
						},
						complete: function () {
							hideLoading();
						}
					});
				} else {
					showWarningMessage(result.message);
				}
			},
			complete: function () {
				if (hideLoadingDialog) {
					hideLoading();
				}
			}
		});
	}
};

// Funciones para el detalle de factores de libras
var _detalles = [];
var initCostContenedorManualPoundDetail = function () {
	_detalles = gvCostContenedorManualPoundDetail.cpDetalles;
	if (_detalles == null) {
		_detalles = [];
	}
	updateCostManualPoundDetails();
};
function onCostContenedorManualPoundDetailsBeginCallback(s, e) {
	e.customArgs["idCostPerContainer"] = parseInt($("#id_costPerContainer").val());
	e.customArgs["año"] = año.GetValue();
	e.customArgs["mes"] = mes.GetValue();
	e.customArgs["detalles"] = _detalles;
};
function onCostContenedorManualPoundDetailsInitCallback(s, e) {
	initCostContenedorManualPoundDetail();
};
function onCostContenedorManualPoundDetailsEndCallback(s, e) {
	initCostContenedorManualPoundDetail();
};

function onValorPoundManualFactorValidation(s, e) {
	var value = s.GetValue();
	if (value == null) {
		e.isValid = false;
		e.errorText = "Valor Obligatorio";
	}
}
function onValorPoundManualFactorValueChanged(s, e) {
	var value = s.GetValue();
	for (var i = 0; i < _detalles.length; i++) {
		var detalle = _detalles[i];
		if (detalle.IdCostPoundManualFactor == s.cpId_costPoundManualFactor) {
			_detalles[i].Valor = value;
			break;
		}
	}

	s.Validate();

	if (value != null) {
		updateCostManualPoundDetails();
	}

	// Limpiamos los detalles
	$("#detallesEjecucion").empty();
}

// Funciones para el procesamiento
var IsValidEditForm = function () {
	var isGeneralesValid = ASPxClientEdit.ValidateEditorsInContainerById("datosGeneralesEditForm", "", true);
	if (isGeneralesValid) {
		UpdateTabValidationImage(0, true);
	} else {
		UpdateTabValidationImage(0, false);
		tpEdit.SetActiveTabIndex(0);
		return false;
	}
	return true;
};
var validateDetalles = function () {
	for (var i = 0; i < _detalles.length; i++) {
		var detalle = _detalles[i];

		if (detalle.Valor == null) {
			return true;;
		}
	}

	return false;
};
var UpdateTabValidationImage = function (tabIndex, isValid) {
	var imageUrl = isValid
		? "/Content/image/noimage.png"
		: "/Content/image/info-error.png";

	var tab = tpEdit.GetTab(tabIndex);
	tab.SetImageUrl(imageUrl);
	tab.SetActiveImageUrl(imageUrl);
}

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
