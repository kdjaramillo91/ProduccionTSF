// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvPoundsRange.AddNewRow();
}

function RemoveItems(s, e) {
    gvPoundsRange.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvPoundsRange.PerformCallback({ ids: selectedRows });
            gvPoundsRange.UnselectRows();
        });
    });


}

function RefreshGrid(s, e) {
    gvPoundsRange.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvPoundsRange.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvPoundsRange.AddNewRow();
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
    uploadFile("PoundsRange/ImportFilePoundsRange", data, function (result) {
        gvPoundsRange.Refresh();
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

    var text = "Total de elementos seleccionados: <b>" + gvPoundsRange.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPoundsRange.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPoundsRange.GetSelectedRowCount() > 0 && gvPoundsRange.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPoundsRange.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvPoundsRange.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvPoundsRange.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvPoundsRange.cpFilteredRowCountWithoutPage + gvPoundsRange.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvPoundsRange.SelectRows();
}

function UnselectAllRows() {
    gvPoundsRange.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});