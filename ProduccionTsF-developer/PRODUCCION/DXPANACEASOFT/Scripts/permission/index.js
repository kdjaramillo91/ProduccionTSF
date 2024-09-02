
// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvPermissions.AddNewRow();
}

function RemoveItems(s, e) {
    gvPermissions.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            $.ajax({
                url: "Permission/DeleteSelectedPermissions",
                type: "post",
                data: { ids: selectedRows },
                async: true,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                    //$("#maincontent").html(result);
                },
                complete: function () {
                    gvPermissions.PerformCallback();
                    gvPermissions.UnselectRows();
                    hideLoading();
                }
            });
        });
    });
}

function RefreshGrid(s, e) {
    gvPermissions.PerformCallback();
}

function Print(s, e) {
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}


// GRIDVIEW CUSTOM BUTTONS ACTIONS

function GridViewPermissionsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            gvPermissions.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }
}

// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit(s, e) {
    UpdateTitlePanel();

    // DEFUALT FILTER
    //s.ApplyFilter('[isActive] = true');
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();

    //s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPermissions.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPermissions.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPermissions.GetSelectedRowCount() > 0 && gvPermissions.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPermissions.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvPermissions.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvPermissions.cpFilteredRowCountWithoutPage + gvPermissions.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvPermissions.SelectRows();
}

function UnselectAllRows() {
    gvPermissions.UnselectRows();
}

// MAIN FUNCTIONS

function init() {
    
}

$(function () {
    init();
});