// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvEconomicGroups.AddNewRow();
}

function RemoveItems(s, e) {
    gvEconomicGroups.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvEconomicGroups.PerformCallback({ ids: selectedRows });
            gvEconomicGroups.UnselectRows();
        });
    });
}

function ImportFile(data) {
    uploadFile("EconomicGroup/ImportFileEconomicGroup", data, function (result) {
        gvWarehousesTypes.Refresh();
    });
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}

function RefreshGrid(s, e) {
    gvEconomicGroups.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvEconomicGroups.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvEconomicGroups.AddNewRow();
        }
    });
}

function Print(s, e) {
    gvEconomicGroups.GetSelectedFieldValues("id", function (values) {

        var _url = "EconomicGroup/EconomicGroupReport";

        var data = null;
        if (values.length === 1) {
            _url = "EconomicGroup/EconomicGroupDetailReport";
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



// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit() {
    UpdateTitlePanel();
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

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvEconomicGroups.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvEconomicGroups.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvEconomicGroups.GetSelectedRowCount() > 0 && gvEconomicGroups.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvEconomicGroups.GetSelectedRowCount() > 0);

    btnRemove.SetEnabled(gvEconomicGroups.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvEconomicGroups.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvEconomicGroups.cpFilteredRowCountWithoutPage + gvEconomicGroups.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvEconomicGroups.SelectRows();
}

function UnselectAllRows() {
    gvEconomicGroups.UnselectRows();
}


// MAIN FUNCTIONS

function init() {

}

$(function () {
    init();
});
