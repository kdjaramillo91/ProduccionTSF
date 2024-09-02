
// FILTER FORM BUTTONS ACTIONS
function AddNewItem(s, e) {
    gvInventoryReason.AddNewRow();
}
function RefreshGrid(s, e) {
    gvInventoryReason.Refresh();
}

function ButtonCancel_Click(s, e) {
    showPage("InventoryReason/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterInventoryReason").serialize() + "&scode=" + scode.GetValue();
      if (data != null) {
        $.ajax({
            url: "InventoryReason/InventoryReasonResults",
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
                $("#btnCollapse").click();
                $("#results").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });
    }
    event.preventDefault();
}

function btnClear_click() {
  
    scode.SetValue(null);
    id_country.SetSelectedItem(null);
 
}

function AddNewInventoryReason() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("InventoryReason/FormEditInventoryReason", data);
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvInventoryReason.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
         
        $.ajax({
            url: url,
            type: "post",
            data: { ids: selectedRows },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                //console.log(result);
            },
            complete: function () {
                //hideLoading();
                gvInventoryReason.PerformCallback();
                gvInventoryReason.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewInventoryReason(s, e);
}
function RemoveItems(s, e) {

}
function CopyDocument(s, e) {
   
}
function ApproveDocuments(s, e) {

}
function AutorizeDocuments(s, e) {
 
}
function ProtectDocuments(s, e) {

}
function CancelDocuments(s, e) {

}
function RevertDocuments(s, e) {

}
function ShowHistory(s, e) {

}
function Print(s, e) {
    
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}



function GridViewlgvInventoryReasonCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvInventoryReason.GetRowKey(e.visibleIndex)
        };
        showPage("InventoryReason/FormEditInventoryReason", data);
    }
}



function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

var selectedRows = [];

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvInventoryReason.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvInventoryReason.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvInventoryReason.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvInventoryReason.GetSelectedRowCount() > 0 && gvInventoryReason.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvInventoryReason.GetSelectedRowCount() > 0);
    //}



    btnCopy.SetEnabled(false);
    btnRemove.SetEnabled(false);
    btnImport.SetEnabled(false);
    btnPrint.SetEnabled(false);
 
}

function GetSelectedFilteredRowCount() {
    return gvInventoryReason.cpFilteredRowCountWithoutPage + gvInventoryReason.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvInventoryReason.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvInventoryReason.SelectRows();
}

function init() {
    $("#btnCollapse").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
            $("#results").css("display", "");
        } else {
            $("#results").css("display", "none");
        }
    });
}
$(function () {
    init();
});