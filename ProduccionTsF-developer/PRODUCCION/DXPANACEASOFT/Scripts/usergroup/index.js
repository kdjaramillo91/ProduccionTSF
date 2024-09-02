
// BUTTONS ACTIONS

function AddNewItem(s, e) {
    var data = {
        id: 0
    };
    showPage("UserGroup/FormEditUserGroup", data);
}

function RemoveItems(s, e) {
    gvUserGroup.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            $.ajax({
                url: "UserGroup/DeleteSelectedUserGroups",
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
                    //gvUserGroup.PerformCallback();
                    gvUserGroup.UnselectRows();
                    hideLoading();
                }
            });
        });
    });
}

function RefreshGrid(s, e) {
    gvUserGroup.PerfomCallback();
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

function GridViewUserGroupsCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        s.GetRowValues(e.visibleIndex, "id", function (value) {
        var data = {
            id: value
        }
            showPage("UserGroup/FormEditUserGroup", data);
        });
    } else if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            gvUserGroup.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }

    
}

// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvUserGroup.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvUserGroup.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvUserGroup.GetSelectedRowCount() > 0 && gvUserGroup.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvUserGroup.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvUserGroup.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvUserGroup.cpFilteredRowCountWithoutPage + gvUserGroup.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvUserGroup.SelectRows();
}

function UnselectAllRows() {
    gvUserGroup.UnselectRows();
}

// MAIN FUNCTIONS

function init() {
    
}

$(function () {
    init();
});