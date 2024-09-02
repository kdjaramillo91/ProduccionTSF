// BUTTONS ACTIONS
function AddNewItem(s, e) {
	gvProductionCart.AddNewRow();
}

function RemoveItems(s, e) {
	gvProductionCart.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
			gvProductionCart.PerformCallback({ ids: selectedRows });
            gvProductionCart.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
	gvProductionCart.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
			gvProductionCart.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function ImportFile(data) {
	uploadFile("ProductionCart/ImportFileProductionCart", data, function (result) {
		gvProductionCart.Refresh();
    });
}

function Print(s, e) {
    
	gvProductionCart.GetSelectedFieldValues("id", function (values) {

		var _url = "ProductionCart/ProductionCartReport";

        var data = null;
        if (values.length === 1) {
			_url = "ProductionCart/ProductionCartDetailReport";
            data = { id: values[0] };
        }
        $.ajax({
            url: _url,
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
                var w = window.open();
                $(w.document.body).html(result);
                //window.open(_url)
                //$("#maincontent").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });


    });

}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}


function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {   
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();    
        });
    }
}

function RefreshGrid(s, e) {
	gvProductionCart.Refresh();
}


// SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    // DEFUALT FILTER
    s.ApplyFilter('[isActive] = true');
}


function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function GetSelectedFieldValuesCallback(values) {
    selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}


function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}
function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

	var text = "Total de elementos seleccionados: <b>" + gvProductionCart.GetSelectedRowCount() + "</b>";
	var hiddenSelectedRowCount = gvProductionCart.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    
	SetElementVisibility("lnkSelectAllRows", gvProductionCart.GetSelectedRowCount() > 0 && gvProductionCart.cpVisibleRowCount > selectedFilteredRowCount);
	SetElementVisibility("lnkClearSelection", gvProductionCart.GetSelectedRowCount() > 0);
 

	btnRemove.SetEnabled(gvProductionCart.GetSelectedRowCount() > 0);
	btnCopy.SetEnabled(gvProductionCart.GetSelectedRowCount() === 1);
}
function GetSelectedFilteredRowCount() {
	return gvProductionCart.cpFilteredRowCountWithoutPage + gvProductionCart.GetSelectedKeysOnPage().length;
}
function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

$(function () {
    $("form").on("click", "#lnkSelectAllRows", function () {
		gvProductionCart.SelectRows();
    });
    $("form").on("click", "#lnkClearSelection", function () {
		gvProductionCart.UnselectRows();
    });
});
function SelectAllRows() {
	gvProductionCart.SelectRows();
}

function UnselectAllRows() {
	gvProductionCart.UnselectRows();
}

// MAIN FUNCTIONS

function init() {
   
}

$(function () {
    init();
});

