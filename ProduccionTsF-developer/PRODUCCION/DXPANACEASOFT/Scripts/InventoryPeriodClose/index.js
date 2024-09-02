
// FILTER FORM BUTTONS ACTIONS
function ButtonCancel_Click(s, e) {
    showPage(" InventoryPeriodClose/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterInventoryPeriodClose").serialize() + "&anio=" + year.GetValue() + "&id_InventoryPeriodType=" + id_PeriodType.GetValue();
      if (data != null) {
        $.ajax({
            url: "InventoryPeriodClose/InventoryPeriodCloseResults",
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

function AddNewInventoryPeriod() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("InventoryPeriodClose/FormEditInventoryPeriodClose", data);
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
        showPage("InventoryPeriodClose/FormEditInventoryPeriodClose", data);
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
    $("#lblInfo").html(text);

    //console.log(gvInventoryPeriod.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvInventoryPeriod.GetSelectedRowCount() > 0 && gvInventoryPeriod.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvInventoryPeriod.GetSelectedRowCount() > 0);
    //}

    //btnCopy.SetEnabled(gvInventoryPeriod.GetSelectedRowCount() == 1);

//    btnCopy.SetEnabled(false);
//    btnApprove.SetEnabled(false);
//    btnAutorize.SetEnabled(false);
//    btnProtect.SetEnabled(false);
//    btnCancel.SetEnabled(false);
//    btnRevert.SetEnabled(false);
//    btnHistory.SetEnabled(false);
//    btnPrint.SetEnabled(false);
}

function GetSelectedFilteredRowCount() {
    //return gvInventoryPeriod.cpFilteredRowCountWithoutPage + gvInventoryPeriod.GetSelectedKeysOnPage().length;
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