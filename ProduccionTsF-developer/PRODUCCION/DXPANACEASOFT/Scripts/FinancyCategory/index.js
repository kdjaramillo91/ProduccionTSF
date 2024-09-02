
// FILTER FORM BUTTONS ACTIONS
function AddNewItem(s, e) {
    gvFinancyCategory.AddNewRow();
}
function RefreshGrid(s, e) {
    gvFinancyCategory.Refresh();
}

function ButtonCancel_Click(s, e) {
    showPage("FinancyCategory/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterFinancyCategory").serialize() + "&scode=" + scode.GetValue();
      if (data != null) {
        $.ajax({
            url: "FinancyCategory/FinancyCategoryResults",
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

function AddNewFinancyCategory() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("FinancyCategory/FormEditFinancyCategory", data);
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvFinancyCategory.GetSelectedFieldValues("id", function (values) {

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
                gvFinancyCategory.PerformCallback();
                gvFinancyCategory.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewFinancyCategory(s, e);
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

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}





function GridViewlgvFinancyCategoryCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvFinancyCategory.GetRowKey(e.visibleIndex)
        };
        showPage("FinancyCategory/FormEditFinancyCategory", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvFinancyCategory.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvFinancyCategory.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvFinancyCategory.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvFinancyCategory.GetSelectedRowCount() > 0 && gvFinancyCategory.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvFinancyCategory.GetSelectedRowCount() > 0);
    //}



    btnCopy.SetEnabled(false);
    btnRemove.SetEnabled(false);
    btnImport.SetEnabled(false);
    btnPrint.SetEnabled(false);
    //btnAproved.SetEnabled(false);
 
}

function GetSelectedFilteredRowCount() {
    return gvFinancyCategory.cpFilteredRowCountWithoutPage + gvFinancyCategory.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvFinancyCategory.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvFinancyCategory.SelectRows();
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