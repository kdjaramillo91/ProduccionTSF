
// FILTER FORM BUTTONS ACTIONS
function AddNewItem(s, e) {
    gvBoxCardAndBank.AddNewRow();
}
function RefreshGrid(s, e) {
    gvBoxCardAndBank.Refresh();
}

function ButtonCancel_Click(s, e) {
    showPage("BoxCardAndBank/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterBoxCardAndBank").serialize() + "&scode=" + scode.GetValue();
      if (data != null) {
        $.ajax({
            url: "BoxCardAndBank/BoxCardAndBankResults",
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

function AddNewBoxCardAndBank() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("BoxCardAndBank/FormEditBoxCardAndBank", data);
}


function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}


// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvBoxCardAndBank.GetSelectedFieldValues("id", function (values) {

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
                gvBoxCardAndBank.PerformCallback();
                gvBoxCardAndBank.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewBoxCardAndBank(s, e);
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

function AprovedItem(s, e) {

}

function RemoveItems(s, e) {

}




function GridViewlgvBoxCardAndBankCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvBoxCardAndBank.GetRowKey(e.visibleIndex)
        };
        showPage("BoxCardAndBank/FormEditBoxCardAndBank", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvBoxCardAndBank.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvBoxCardAndBank.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvBoxCardAndBank.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvBoxCardAndBank.GetSelectedRowCount() > 0 && gvBoxCardAndBank.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvBoxCardAndBank.GetSelectedRowCount() > 0);
    //}



    btnCopy.SetEnabled(false);
    btnRemove.SetEnabled(false);
    btnImport.SetEnabled(false);
    btnPrint.SetEnabled(false);
    //btnAproved.SetEnabled(false);
 
}

function GetSelectedFilteredRowCount() {
    return gvBoxCardAndBank.cpFilteredRowCountWithoutPage + gvBoxCardAndBank.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvBoxCardAndBank.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvBoxCardAndBank.SelectRows();
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