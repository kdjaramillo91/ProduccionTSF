// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvBusinessGroups.AddNewRow();
}

function RemoveItems(s, e) {
    gvBusinessGroups.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvBusinessGroups.PerformCallback({ ids: selectedRows });
            gvBusinessGroups.UnselectRows();
        });
    });
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvBusinessGroups.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvBusinessGroups.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}


function ImportFile(data) {
    uploadFile("BusinessGroup/ImportFileBusinessGroup", data, function (result) {
        gvBusinessGroups.Refresh();
    });
}

function Print(s, e) {
    gvBusinessGroups.GetSelectedFieldValues("id", function (values) {

        var _url = "BusinessGroup/BusinessGroupReport";

        var data = null;
        if (values.length === 1) {
            _url = "BusinessGroup/BusinessGroupDetailReport";
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


function RefreshGrid(s, e) {
    gvBusinessGroups.Refresh();
}
// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
    s.ApplyFilter('[isActive] = true');
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

    var text = "Total de elementos seleccionados: <b>" + gvBusinessGroups.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvBusinessGroups.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvBusinessGroups.GetSelectedRowCount() > 0 && gvBusinessGroups.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvBusinessGroups.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvBusinessGroups.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvBusinessGroups.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvBusinessGroups.cpFilteredRowCountWithoutPage + gvBusinessGroups.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvBusinessGroups.SelectRows();
}

function UnselectAllRows() {
    gvBusinessGroups.UnselectRows();
}

// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
  
});