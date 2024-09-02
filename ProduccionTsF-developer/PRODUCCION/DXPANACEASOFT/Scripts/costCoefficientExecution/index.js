
// Eventos de controles de filtro para la consulta

var OnRangeStartDateQueryValidation = function(s, e) {
	OnRangeDateValidation(e, startStartDate.GetValue(), endStartDate.GetValue(), "Rango de fecha no es válido");
};
var OnRangeEndDateQueryValidation = function(s, e) {
	OnRangeDateValidation(e, startEndDate.GetValue(), endEndDate.GetValue(), "Rango de fecha no es válido");
};

var onAnioQueryInit = function() {
	var today = new Date();
	var yearText = today.getFullYear().toString();

	var numElements = anio.GetItemCount();
	for (var i = 0; i < numElements; i++) {
		var yearTextItem = anio.GetItem(i).text;
		if (yearTextItem === yearText) {
			anio.SetSelectedIndex(i);
			break;
		}
	}
};
var onMesQueryInit = function() {
	var today = new Date();
	var monthValue = today.getMonth() + 1;
	mes.SetValue(monthValue);
};

var onSearchQueryButtonClick = function(s, e) {
	performQuery();
};
var onClearQueryButtonClick = function(s, e) {
	id_documentState.SetValue(null);
	number.SetText("");
	reference.SetText("");

	startStartDate.SetDate(null);
	endStartDate.SetDate(null);

	startEndDate.SetDate(null);
	endEndDate.SetDate(null);

	onAnioQueryInit();
	onMesQueryInit();

	id_allocationType.SetValue(null);

	$("#queryResults").empty();
};
var onAddNewQueryButtonClick = function(s, e) {
	showEditForm(null);
};


var onAddNewQueryToolbarButtonClick = function(s, e) {
	showEditForm(null);
};
var onRefreshQueryToolbarButtonClick = function(s, e) {
	performQuery();
};


var performQuery = function() {
	var queryData = $("#formFilterCostCoefficientExecution").serialize();
	if (queryData !== null) {
		$.ajax({
			url: "CostCoefficientExecution/CostCoefficientExecutionResults",
			type: "post",
			data: queryData,
			async: true,
			cache: false,
			error: function(error) {
				console.log(error);
			},
			beforeSend: function() {
				showLoading();
			},
			success: function(result) {
				if ($("#filterForm").is(":visible")) {
					$("#btnCollapse").click();
				}
				$("#queryResults").html(result);
			},
			complete: function() {
				hideLoading();
			}
		});
	}
};
var showEditForm = function(id) {
	var userData = {
		id: id
	};
	showPage("CostCoefficientExecution/EditForm", userData);
};


// Eventos del GridView

var OnGridViewInit = function() {
	UpdateTitlePanel();
};

var OnGridViewSelectionChanged = function(s, e) {
	UpdateTitlePanel();
};

var OnGridViewBeginCallback = function(s, e) {
};

var OnGridViewEndCallback = function(s, e) {
	UpdateTitlePanel();
};

var OnGridViewCustomButtonClick = function(s, e) {
	if (e.buttonID === "btnEditRow") {
		showEditForm(s.GetRowKey(e.visibleIndex));
	}
};


// Manejadores de selección

var UpdateTitlePanel = function() {
	var text = "Total de elementos seleccionados: <b>" + gvCostCoefficientExecutionResult.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gvCostCoefficientExecutionResult.GetSelectedRowCount() - GetSelectedFilteredRowCount();
	if (hiddenSelectedRowCount > 0) {
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	}
	text += "<br />";
	$("#lblInfo").html(text);
};

var GetSelectedFilteredRowCount = function() {
	return gvCostCoefficientExecutionResult.cpFilteredRowCountWithoutPage + gvCostCoefficientExecutionResult.GetSelectedKeysOnPage().length;
};



// Funciones principales
var init = function() {
	$("#btnCollapse").click(function(event) {
		if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
			$("#results").css("display", "");
		} else {
			$("#results").css("display", "none");
		}
	});
};

$(function() {
	init();
});
