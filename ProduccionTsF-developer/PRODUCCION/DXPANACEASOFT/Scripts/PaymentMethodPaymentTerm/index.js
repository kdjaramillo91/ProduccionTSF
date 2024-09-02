
// FILTER FORM BUTTONS ACTIONS
function AddNewItem(s, e) {
    gvPaymentMethodPaymentTerm.AddNewRow();
}
function RefreshGrid(s, e) {
    gvPaymentMethodPaymentTerm.Refresh();
}
function AprovedItem(s, e) {

}

function RemoveItems(s, e) {

}

function ButtonCancel_Click(s, e) {
    showPage("PaymentMethodPaymentTerm/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterPaymentMethodPaymentTerm").serialize() + "&scode=" + scode.GetValue();
      if (data != null) {
        $.ajax({
            url: "PaymentMethodPaymentTerm/PaymentMethodPaymentTermResults",
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

function AddNewPaymentMethodPaymentTerm() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("PaymentMethodPaymentTerm/FormEditPaymentMethodPaymentTerm", data);
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvPaymentMethodPaymentTerm.GetSelectedFieldValues("id", function (values) {

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
                gvPaymentMethodPaymentTerm.PerformCallback();
                gvPaymentMethodPaymentTerm.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewPaymentMethodPaymentTerm(s, e);
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



function GridViewlgvPaymentMethodPaymentTermCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvPaymentMethodPaymentTerm.GetRowKey(e.visibleIndex)
        };
        showPage("PaymentMethodPaymentTerm/FormEditPaymentMethodPaymentTerm", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvPaymentMethodPaymentTerm.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPaymentMethodPaymentTerm.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvPaymentMethodPaymentTerm.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPaymentMethodPaymentTerm.GetSelectedRowCount() > 0 && gvPaymentMethodPaymentTerm.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPaymentMethodPaymentTerm.GetSelectedRowCount() > 0);
    //}



    btnCopy.SetEnabled(false);
    btnRemove.SetEnabled(false);
    btnImport.SetEnabled(false);
    btnPrint.SetEnabled(false);
    //btnAproved.SetEnabled(false);
 
}

function GetSelectedFilteredRowCount() {
    return gvPaymentMethodPaymentTerm.cpFilteredRowCountWithoutPage + gvPaymentMethodPaymentTerm.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvPaymentMethodPaymentTerm.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvPaymentMethodPaymentTerm.SelectRows();
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