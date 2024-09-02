
// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvInventoryControlType.AddNewRow();
}

function RemoveItems(s, e) {
    gvInventoryControlType.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
         
        }
        showConfirmationDialog(function () {
            gvInventoryControlType.PerformCallback({ ids: selectedRows });
            gvInventoryControlType.UnselectRows();
        });
    });
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
    gvInventoryControlType.Refresh();
}


var keyToCopy = null;
function CopyItems(s, e) {
    gvInventoryControlType.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvInventoryControlType.AddNewRow();
            keyToCopy = 0;

        }
    });
}


function Print(s, e) {
    gvInventoryControlType.GetSelectedFieldValues("id", function (values) {

        var _url = "InventoryControlType/InventoryControlTypeReport";

        var data = null;
        if (values.length === 1) {
            _url = "InventoryControlType/InventoryControlTypeDetailReport";
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
    uploadFile("InventoryControlType/ImportFileInventoryControlType", data, function (result) {
        gvInventoryControlType.Refresh();
    });
}

// SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    s.ApplyFilter('[isActive] = true');
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvInventoryControlType.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvInventoryControlType.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvInventoryControlType.GetSelectedRowCount() > 0 && gvInventoryControlType.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvInventoryControlType.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvInventoryControlType.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvInventoryControlType.GetSelectedRowCount() === 1);
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

function GetSelectedFilteredRowCount() {
    return gvInventoryControlType.cpFilteredRowCountWithoutPage + gvInventoryControlType.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvInventoryControlType.SelectRows();
}

function UnselectAllRows() {
    gvInventoryControlType.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});


