
// FILTER FORM BUTTONS ACTIONS
function AddNewItem(s, e) {
    gvAdvanceParameters.AddNewRow();
}
function RefreshGrid(s, e) {
    gvAdvanceParameters.Refresh();
}

function ButtonCancel_Click(s, e) {
    showPage("AdvanceParameters/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterAdvanceParameters").serialize() + "&scode=" + scode.GetValue();
      if (data !== null) {
        $.ajax({
            url: "AdvanceParameters/AdvanceParametersResults",
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

function AddNewAdvanceParameters() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("AdvanceParameters/FormEditAdvanceParameters", data);
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvAdvanceParameters.GetSelectedFieldValues("id", function (values) {

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
                gvAdvanceParameters.PerformCallback();
                gvAdvanceParameters.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewAdvanceParameters(s, e);
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




function GridViewlgvAdvanceParametersCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvAdvanceParameters.GetRowKey(e.visibleIndex)
        };
        showPage("AdvanceParameters/FormEditAdvanceParameters", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvAdvanceParameters.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvAdvanceParameters.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvAdvanceParameters.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvAdvanceParameters.GetSelectedRowCount() > 0 && gvAdvanceParameters.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvAdvanceParameters.GetSelectedRowCount() > 0);
    //}



    btnCopy.SetEnabled(false);
    btnRemove.SetEnabled(false);
    btnImport.SetEnabled(false);
    btnPrint.SetEnabled(false);
    //btnAproved.SetEnabled(false);
 
}

function GetSelectedFilteredRowCount() {
    return gvAdvanceParameters.cpFilteredRowCountWithoutPage + gvAdvanceParameters.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvAdvanceParameters.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvAdvanceParameters.SelectRows();
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

