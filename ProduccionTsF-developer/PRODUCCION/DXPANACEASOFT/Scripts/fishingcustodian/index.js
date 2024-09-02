// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvFishingCustodian.AddNewRow();
}

function RemoveItems(s, e) {
    gvFishingCustodian.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvFishingCustodian.PerformCallback({ ids: selectedRows });
            gvFishingCustodian.UnselectRows();
        });
    });


}

function RefreshGrid(s, e) {
    gvFishingCustodian.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvFishingCustodian.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvFishingCustodian.AddNewRow();
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
    uploadFile("FishingCustodian/ImportFileFishingCustodian", data, function (result) {
        gvFishingCustodian.Refresh();
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

    var text = "Total de elementos seleccionados: <b>" + gvFishingCustodian.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvFishingCustodian.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvFishingCustodian.GetSelectedRowCount() > 0 && gvFishingCustodian.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvFishingCustodian.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvFishingCustodian.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvFishingCustodian.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvFishingCustodian.cpFilteredRowCountWithoutPage + gvFishingCustodian.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvFishingCustodian.SelectRows();
}

function UnselectAllRows() {
    gvFishingCustodian.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});