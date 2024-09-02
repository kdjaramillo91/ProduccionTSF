// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvIceBagRange.AddNewRow();
}

function RemoveItems(s, e) {
    gvIceBagRange.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvIceBagRange.PerformCallback({ ids: selectedRows });
            gvIceBagRange.UnselectRows();
        });
    });


}

function RefreshGrid(s, e) {
    gvIceBagRange.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvIceBagRange.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvIceBagRange.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function Print(s, e) {

}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}


function ImportFile(data) {
    uploadFile("IceBagRange/ImportFileIceBagRange", data, function (result) {
        gvIceBagRange.Refresh();
    });
}

// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit() {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
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

// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvIceBagRange.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvIceBagRange.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvIceBagRange.GetSelectedRowCount() > 0 && gvIceBagRange.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvIceBagRange.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvIceBagRange.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvIceBagRange.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvIceBagRange.cpFilteredRowCountWithoutPage + gvIceBagRange.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvIceBagRange.SelectRows();
}

function UnselectAllRows() {
    gvIceBagRange.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});