
// FILTER FORM BUTTONS ACTIONS
function AddNewItem(s, e) {
    gvTermsNegotiation.AddNewRow();
}
function RefreshGrid(s, e) {
    gvTermsNegotiation.Refresh();
}

function ButtonCancel_Click(s, e) {
    showPage("TermsNegotiation/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterTermsNegotiation").serialize() + "&scode=" + scode.GetValue();
      if (data != null) {
        $.ajax({
            url: "TermsNegotiation/TermsNegotiationResults",
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

function AddNewTermsNegotiation() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("TermsNegotiation/FormEditTermsNegotiation", data);
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvTermsNegotiation.GetSelectedFieldValues("id", function (values) {

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
                gvTermsNegotiation.PerformCallback();
                gvTermsNegotiation.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewTermsNegotiation(s, e);
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

function AprovedItem(s, e) {

}

function RemoveItems(s, e) {

}




function GridViewlgvTermsNegotiationCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvTermsNegotiation.GetRowKey(e.visibleIndex)
        };
        showPage("TermsNegotiation/FormEditTermsNegotiation", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvTermsNegotiation.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvTermsNegotiation.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvTermsNegotiation.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvTermsNegotiation.GetSelectedRowCount() > 0 && gvTermsNegotiation.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvTermsNegotiation.GetSelectedRowCount() > 0);
    //}


    btnNew.SetEnabled(true);
    btnCopy.SetEnabled(false);
    btnRemove.SetEnabled(false);
    btnImport.SetEnabled(false);
    btnPrint.SetEnabled(false);
    //btnAproved.SetEnabled(false);
 
}

function GetSelectedFilteredRowCount() {
    return gvTermsNegotiation.cpFilteredRowCountWithoutPage + gvTermsNegotiation.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvTermsNegotiation.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvTermsNegotiation.SelectRows();
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