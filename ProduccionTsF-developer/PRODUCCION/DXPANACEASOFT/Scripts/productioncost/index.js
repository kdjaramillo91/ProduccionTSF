
// Métodos de barra de herramientas principal

var AddNewItem = function(s, e) {
	gvProductionCost.AddNewRow();
};

var RemoveItems = function(s, e) {
	gvProductionCost.GetSelectedFieldValues("id", function(values) {
		var selectedRows = [];
		for (var i = 0; i < values.length; i++) {
			selectedRows.push(values[ i ]);
		}

		showConfirmationDialog(function() {
			gvProductionCost.PerformCallback({ ids: selectedRows });
			gvProductionCost.UnselectRows();
		});
	});
};

var RefreshGrid = function(s, e) {
	gvProductionCost.Refresh();
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

var GridViewItemsCustomCommandButton_Click = function(s, e) {
	if (e.buttonID === "btnDeleteRow") {
		showConfirmationDialog(function() {
			s.DeleteRow(e.visibleIndex);
			s.UnselectRows();
		}, "<p>Se dispone a desactivar el/los registro(s) seleccionado(s).</p><p>¿Está seguro?</p>");
	}
};


// Manejadores de selección

var UpdateTitlePanel = function() {
	var selectedFilteredRowCount = GetSelectedFilteredRowCount();

	var text = "Total de elementos seleccionados: <b>" + gvProductionCost.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gvProductionCost.GetSelectedRowCount() - GetSelectedFilteredRowCount();
	if (hiddenSelectedRowCount > 0) {
		text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
	}
	text += "<br />";
	$("#lblInfo").html(text);

	SetElementVisibility("lnkSelectAllRows", gvProductionCost.GetSelectedRowCount() > 0 && gvProductionCost.cpVisibleRowCount > selectedFilteredRowCount);
	SetElementVisibility("lnkClearSelection", gvProductionCost.GetSelectedRowCount() > 0);

	btnRemove.SetEnabled(gvProductionCost.GetSelectedRowCount() > 0);
};

var GetSelectedFilteredRowCount = function() {
	return gvProductionCost.cpFilteredRowCountWithoutPage + gvProductionCost.GetSelectedKeysOnPage().length;
};

var SetElementVisibility = function(id, visible) {
	var $element = $("#" + id);
	visible ? $element.show() : $element.hide();
};

var SelectAllRows = function() {
	gvProductionCost.SelectRows();
};

var UnselectAllRows = function() {
	gvProductionCost.UnselectRows();
};


// Funciones principales
var init = function() {
};

$(function() {
	init();
});