// BUTTONS ACTIONS

function AddNewPermit(s, e) {

    gvPersonTypes.AddNewRow();
}

function RefreshGrid(s, e) {
    gvPersonTypes.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvPersonTypes.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvPersonTypes.AddNewRow();
            keyToCopy = 0;
        }
    });
}

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
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

function GridViewAccountingTemplateCostCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}

 
function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPersonTypes.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPersonTypes.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvPersonTypes.GetSelectedRowCount() > 0 && gvPersonTypes.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPersonTypes.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvPersonTypes.cpFilteredRowCountWithoutPage + gvPersonTypes.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}


function WarehouseUserPermitDetail_OnBeginCallback(s, e) {

    var objaccountingTemplateCostType = ASPxClientControl.GetControlCollection().GetByName("id_warehouseUserPermit");
    e.customArgs['id_warehouseUserPermit'] = $("#id_warehouseUserPermit").val();
    e.customArgs['id_warehouseUserPermitDetail'] = (objaccountingTemplateCostType != undefined) ? ((objaccountingTemplateCostType != null) ? objaccountingTemplateCostType.GetValue():0 ):0 ;
}

// MAIN FUNCTIONS

function init() {
   
}

$(function () {
    init();
});