// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvSecuritySealState.AddNewRow();
}

function RemoveItems(s, e) {
    gvSecuritySealState.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvSecuritySealState.PerformCallback({ ids: selectedRows });
            gvSecuritySealState.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvSecuritySealState.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvSecuritySealState.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function ImportFile(data) {
    uploadFile("SecuritySealState/ImportFileSecuritySealStatey", data, function (result) {
        gvSecuritySealState.Refresh();
    });
}

function RefreshGrid(s, e) {
    gvSecuritySealState.Refresh();
}

function Print(s, e) {
    gvSecuritySealState.GetSelectedFieldValues("id", function (values) {

        var _url = "SecuritySealState/SecuritySealStateReport";

        var data = null;
        if (values.length === 1) {
            _url = "SecuritySealState/SecuritySealStateDetailReport";
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

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
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

    var text = "Total de elementos seleccionados: <b>" + gvSecuritySealState.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvSecuritySealState.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvSecuritySealState.GetSelectedRowCount() > 0 && gvSecuritySealState.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvSecuritySealState.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvSecuritySealState.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvSecuritySealState.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvSecuritySealState.cpFilteredRowCountWithoutPage + gvSecuritySealState.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvSecuritySealState.SelectRows();
}

function UnselectAllRows() {
    gvSecuritySealState.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});

