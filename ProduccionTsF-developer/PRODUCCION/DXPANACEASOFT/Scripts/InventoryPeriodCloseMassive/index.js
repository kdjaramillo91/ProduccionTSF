
// FILTER FORM BUTTONS ACTIONS
function ButtonCancel_Click(s, e) {
    showPage(" InventoryPeriodCloseMassive/Index", null);
}
function btnSearch_click() {
    var fecha = dateInit.GetDate(); // DEVOLVERRÁ UN TIPO DATE
    var fechaIso = fecha != null
        ? fecha.toISOString().substring(0, 10)
        : null;
   
    var data = $("#formFilterInventoryPeriodCloseMassive").serialize() + "&periodo=" + fechaIso + "&id_InventoryPeriodType=" + id_PeriodType.GetValue() + "&id_Estado=" + id_Estado.GetValue();
      if (data != null) {
        $.ajax({
            url: "InventoryPeriodCloseMassive/InventoryPeriodCloseMassiveResults",
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
  
    dateInit.SetValue(null);
    id_Estado.SetSelectedItem(null);
    id_warehouse.SetSelectedItem(null);
    id_PeriodType.SetSelectedItem(null);
 
}

function AddNewInventoryPeriod() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("InventoryPeriodCloseMassive/FormEditInventoryPeriodCloseMassive", data);
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvInventoryPeriod.GetSelectedFieldValues("id", function (values) {

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
                gvInventoryPeriod.PerformCallback();
                gvInventoryPeriod.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewInventoryPeriod(s, e);
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



function GridViewlgvInventoryPeriodCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvInventoryPeriod.GetRowKey(e.visibleIndex)
        };
        showPage("InventoryPeriodCloseMassive/FormEditInventoryPeriodCloseMassive", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvInventoryPeriod.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvInventoryPeriod.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";

    SetElementVisibility("lnkSelectAllRows", gvInventoryPeriod.GetSelectedRowCount() > 0 && gvInventoryPeriod.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvInventoryPeriod.GetSelectedRowCount() > 0);

}

function GetSelectedFilteredRowCount() {
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvInventoryPeriod.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvInventoryPeriod.SelectRows();
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