// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvInventoryValuationMethod.AddNewRow();
}

function RemoveItems(s, e) {
    gvInventoryValuationMethod.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvInventoryValuationMethod.PerformCallback({ ids: selectedRows });
            gvInventoryValuationMethod.UnselectRows();
        });
    });
}


function RefreshGrid(s, e) {
    gvInventoryValuationMethod.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvInventoryValuationMethod.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvInventoryValuationMethod.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function Print(s, e) {
    gvInventoryValuationMethod.GetSelectedFieldValues("id", function (values) {

        var _url = "InventoryValuationMethod/InventoryValuationMethodReport";

        var data = null;
        if (values.length === 1) {
            _url = "InventoryValuationMethod/InventoryValuationMethodDetailReport";
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
                $("#maincontent").html(result);
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


function ImportFile(data) {
    uploadFile("InventoryValuationMethod/ImportFileInventoryValuationMethod", data, function (result) {
        gvInventoryValuationMethod.Refresh();
    });
}

// SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    s.ApplyFilter('[isActive] = true');
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}


function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvInventoryValuationMethod.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvInventoryValuationMethod.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvInventoryValuationMethod.GetSelectedRowCount() > 0 && gvInventoryValuationMethod.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvInventoryValuationMethod.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvInventoryValuationMethod.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvInventoryValuationMethod.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvInventoryValuationMethod.cpFilteredRowCountWithoutPage + gvInventoryValuationMethod.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvInventoryValuationMethod.SelectRows();
}

function UnselectAllRows() {
    gvInventoryValuationMethod.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

