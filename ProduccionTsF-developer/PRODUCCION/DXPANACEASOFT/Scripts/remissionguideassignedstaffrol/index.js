// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvRemissionGuideAssignedStaffRol.AddNewRow();
}

function RemoveItems(s, e) {
    gvRemissionGuideAssignedStaffRol.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvRemissionGuideAssignedStaffRol.PerformCallback({ ids: selectedRows });
            gvRemissionGuideAssignedStaffRol.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvRemissionGuideAssignedStaffRol.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvRemissionGuideAssignedStaffRol.AddNewRow();
            keyToCopy = 0;

        }
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
    gvRemissionGuideAssignedStaffRol.Refresh();
}

function ImportFile(data) {
    uploadFile("RemissionGuideAssignedStaffRol/ImportFileRemissionGuideAssignedStaffRol", data, function (result) {
        gvRemissionGuideAssignedStaffRol.Refresh();
    });
}

function Print(s, e) {
    gvRemissionGuideAssignedStaffRol.GetSelectedFieldValues("id", function (values) {

        var _url = "RemissionGuideAssignedStaffRol/RemissionGuideAssignedStaffRolReport";

        var data = null;
        if (values.length === 1) {
            _url = "RemissionGuideAssignedStaffRol/RemissionGuideAssignedStaffRolDetailReport";
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

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideAssignedStaffRol.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideAssignedStaffRol.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideAssignedStaffRol.GetSelectedRowCount() > 0 && gvRemissionGuideAssignedStaffRol.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideAssignedStaffRol.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvRemissionGuideAssignedStaffRol.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvRemissionGuideAssignedStaffRol.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvRemissionGuideAssignedStaffRol.cpFilteredRowCountWithoutPage + gvRemissionGuideAssignedStaffRol.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvRemissionGuideAssignedStaffRol.SelectRows();
}

function UnselectAllRows() {
    gvRemissionGuideAssignedStaffRol.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

