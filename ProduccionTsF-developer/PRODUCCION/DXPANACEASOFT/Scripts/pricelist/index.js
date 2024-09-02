// DETAILS VIEW CALLBACKS

function PriceListDetail_OnBeginCallback(s, e) {
    //e.customArgs["id_priceList"] = s.cpIdPriceList;
    e.customArgs["id_priceList"] = $("#id_priceList").val();
    //s.cpIdPriceList = $("#id_priceList").val();
}

function PriceListDetail_Init(s, e) {
    s.PerformCallback();
}

function PriceListDetailViewCategoryAdjustment_OnBeginCallback(s, e) {
    //e.customArgs["id_priceList"] = s.cpIdPriceList;
    e.customArgs["id_priceList"] = $("#id_priceList").val();
    //s.cpIdPriceList = $("#id_priceList").val();
}

// ACTIONS BUTTONS

function AddNewItem(s, e) {
    showPage("PriceList/PriceListEditForm", { id: 0 });
}

function RemoveItems(s, e) {
    gvPriceLists.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvPriceLists.PerformCallback({ ids: selectedRows });
            gvPriceLists.UnselectRows();
        });
    });
}

function CopyItems(s, e) {
    gvPriceLists.GetSelectedFieldValues("id", function (values) {
        if(values.length > 0) {
            showPage("PriceList/PriceListCopy", { id: values[0] });
        }
    });
}

function RefreshGrid(s, e) {
    gvPriceLists.Refresh();
}

function Print(s, e) {
    showPage("PriceList/PriceListReport", null);
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}


function GridViewPriceListCustomCommandButton_Click(s, e) {
    
    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvPriceLists.GetRowKey(e.visibleIndex)
        };
        showPage("PriceList/PriceListEditForm", data);
    }else if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
        });
    }
}

// SELECTION

function OnRowDoubleClick(s, e) {
    s.GetRowValues(e.visibleIndex, "id", function (value) {
        showPage("PriceList/PriceListEditForm", { id: value });
    });
    
}

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {

}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPriceLists.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPriceLists.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPriceLists.GetSelectedRowCount() > 0 && gvPriceLists.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPriceLists.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvPriceLists.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvPriceLists.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvPriceLists.cpFilteredRowCountWithoutPage + gvPriceLists.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvPriceLists.SelectRows();
}

function ClearSelection() {
    gvPriceLists.UnselectRows();
}

// VIEW FUNCTIONS

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 5000);
    }
}

function init() {
    AutoCloseAlert();
}

$(function() {
    init();
});