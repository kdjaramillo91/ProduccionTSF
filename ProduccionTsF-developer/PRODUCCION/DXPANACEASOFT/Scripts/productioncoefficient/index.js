
// Métodos de barra de herramientas principal

var AddNewItem = function(s, e) {
	showEditForm(null);
};

var RefreshGrid = function(s, e) {
	gvProductionCoefficient.Refresh();
};

var showEditForm = function(id) {
	var userData = {
		id: id
	};
	showPage("ProductionCoefficient/EditForm", userData);
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
	ShowEditMessage(s, e);
};

var OnGridViewCustomButtonClick = function(s, e) {
	if (e.buttonID === "btnEditRow") {
		showEditForm(s.GetRowKey(e.visibleIndex));
	}
};


// Manejadores de selección

var UpdateTitlePanel = function() {
	var text = "Total de elementos seleccionados: <b>" + gvProductionCoefficient.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gvProductionCoefficient.GetSelectedRowCount() - GetSelectedFilteredRowCount();
	if (hiddenSelectedRowCount > 0) {
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	}
	text += "<br />";
	$("#lblInfo").html(text);
};

var GetSelectedFilteredRowCount = function() {
	return gvProductionCoefficient.cpFilteredRowCountWithoutPage + gvProductionCoefficient.GetSelectedKeysOnPage().length;
};


// Funciones principales
var init = function() {
};

$(function() {
	init();
});