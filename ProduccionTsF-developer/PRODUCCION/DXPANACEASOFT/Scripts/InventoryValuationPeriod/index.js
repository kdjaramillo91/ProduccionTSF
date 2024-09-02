
// FILTER FORM BUTTONS ACTIONS
function ButtonCancel_Click(s, e) {
    showPage(" InventoryValuationPeriod/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterInventoryValuationPeriod").serialize() + "&anio=" + year.GetValue() + "&id_InventoryValuationPeriodType=" + id_PeriodType.GetValue();
      if (data != null) {
        $.ajax({
            url: "InventoryValuationPeriod/InventoryValuationPeriodResults",
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
  
    year.SetValue(null);

    id_PeriodType.SetSelectedItem(null);
 
}

function AddNewInventoryValuationPeriod() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("InventoryValuationPeriod/FormEditInventoryValuationPeriod", data);
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvInventoryValuationPeriod.GetSelectedFieldValues("id", function (values) {

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
            },
            success: function (result) {
            },
            complete: function () {
                gvInventoryValuationPeriod.PerformCallback();
                gvInventoryValuationPeriod.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewInventoryValuationPeriod(s, e);
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



function GridViewlgvInventoryValuationPeriodCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvInventoryValuationPeriod.GetRowKey(e.visibleIndex)
        };
        showPage("InventoryValuationPeriod/FormEditInventoryValuationPeriod", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvInventoryValuationPeriod.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvInventoryValuationPeriod.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);


    SetElementVisibility("lnkSelectAllRows", gvInventoryValuationPeriod.GetSelectedRowCount() > 0 && gvInventoryValuationPeriod.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvInventoryValuationPeriod.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvInventoryValuationPeriod.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvInventoryValuationPeriod.SelectRows();
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