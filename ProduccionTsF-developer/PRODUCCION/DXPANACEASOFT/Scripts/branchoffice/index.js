// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvBranchOffices.AddNewRow();
}

function RemoveItems(s, e) {
    gvBranchOffices.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvBranchOffices.PerformCallback({ ids: selectedRows });
            gvBranchOffices.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvBranchOffices.GetSelectedFieldValues("id", function (values) {
        if(values.length === 1) {
            keyToCopy = values[0];
            gvBranchOffices.AddNewRow();
            keyToCopy = null;
        }
    });
}

function RefreshGrid(s, e) {
    gvBranchOffices.Refresh();
}
function Print(s, e) {
    gvBranchOffices.GetSelectedFieldValues("id", function (values) {

        var _url = "BranchOffice/BranchOfficesReport";

        var data = null;
        if (values.length === 1) {
            _url = "BranchOffice/BranchOfficeDetailReport";
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


// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit(s, e) {    
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
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
            gvBranchOffices.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }
}

// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvBranchOffices.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvBranchOffices.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvBranchOffices.GetSelectedRowCount() > 0 && gvBranchOffices.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvBranchOffices.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvBranchOffices.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvBranchOffices.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvBranchOffices.cpFilteredRowCountWithoutPage + gvBranchOffices.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvBranchOffices.SelectRows();
}

function UnselectAllRows() {
    gvBranchOffices.UnselectRows();
}

// MAIN FUNCTIONS

function init() {
    
}

$(function () {
    init();
});