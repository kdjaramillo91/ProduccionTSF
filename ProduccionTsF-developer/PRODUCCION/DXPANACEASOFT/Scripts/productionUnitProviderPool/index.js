// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvProductionUnitProviderPools.AddNewRow();
}

function RemoveItems(s, e) {
    gvProductionUnitProviderPools.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvProductionUnitProviderPools.PerformCallback({ ids: selectedRows });
            gvProductionUnitProviderPools.UnselectRows();
        });
    });
}

function RefreshGrid(s, e) {
    gvProductionUnitProviderPools.Refresh();
}

var keyToCopy = null;

function CopyItems(s, e) {
    gvProductionUnitProviderPools.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvProductionUnitProviderPools.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function Print(s, e) {
   
}
function ImportFile(data) {
    //uploadFile("WarehouseLocation/ImportFileWarehouseLocation", data, function (result) {
    //    gvWarehouseLocations.Refresh();
    //});
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
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

function OnGridViewBeginCallback(s, e) {
    e.customArgs["idProvider"] = id_provider.GetValue();
    e.customArgs["id_productionUnitProvider"] = id_productionUnitProvider.GetValue();
    e.customArgs["isCallback"] = true;    
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

    var text = "Total de elementos seleccionados: <b>" + gvProductionUnitProviderPools.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionUnitProviderPools.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvProductionUnitProviderPools.GetSelectedRowCount() > 0 && gvProductionUnitProviderPools.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductionUnitProviderPools.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvProductionUnitProviderPools.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvProductionUnitProviderPools.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvProductionUnitProviderPools.cpFilteredRowCountWithoutPage + gvProductionUnitProviderPools.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvProductionUnitProviderPools.SelectRows();
}

function UnselectAllRows() {
    gvProductionUnitProviderPools.UnselectRows();
}

// definition
function productionUnitProviderInit() {
    id_productionUnitProvider.ClearItems();
    ActiveCreationButton();
}

function ComboProvider_SelectedIndexChanged() {
    id_productionUnitProvider.ClearItems();
    id_productionUnitProvider.SetValue(null);

    var idProvider = id_provider.GetValue();

    if (idProvider !== null && idProvider !== undefined) {
        $.ajax({
            url: "ProductionUnitProviderPool/GetProductionUnitProviderByProvider",
            type: "post",
            data: { idProvider: idProvider },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                for (var i = 0; i < result.length; i++) {
                    id_productionUnitProvider.AddItem(result[i].name, result[i].id);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }

    ActiveCreationButton();
    gvProductionUnitProviderPools.Refresh();
};

function ComboProductionUnitProvider_SelectedIndexChanged() {
    ActiveCreationButton();
    gvProductionUnitProviderPools.Refresh();
}

function ActiveCreationButton() {
    var visibleProvider = id_provider.GetValue() !== null;
    var visibleUnitProvider = id_productionUnitProvider.GetValue() !== null;
    btnNew.SetEnabled(visibleProvider && visibleUnitProvider);
}

// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});
