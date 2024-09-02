
// BUTTONS ACTIONS

function AddNewItem(s, e) {
    var data = {
        id: 0
    };
    showPage("User/EditFromUserPartial", data);
}

function RemoveItems(s, e) {
}

function RefreshGrid(s, e) {
}

function Print(s, e) {
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}

// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function GridViewUsersCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        s.GetRowValues(e.visibleIndex, "id", function (value) {
            var data = {
                id: value
            }
            showPage("User/EditFromUserPartial", data);
        });
    } else if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            gvUsers.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }


}

// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvUsers.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvUsers.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvUsers.GetSelectedRowCount() > 0 && gvUsers.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvUsers.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvUsers.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvUsers.cpFilteredRowCountWithoutPage + gvUsers.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvUsers.SelectRows();
}

function UnselectAllRows() {
    gvUsers.UnselectRows();
}


// MAIN FUNCTIONS

function init() {
    
}

$(function (){
    init();
});