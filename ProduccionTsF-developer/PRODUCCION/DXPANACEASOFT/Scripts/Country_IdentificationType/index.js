
// FILTER FORM BUTTONS ACTIONS
function AddNewItem(s, e) {
    gvCountry_IdentificationType.AddNewRow();
}
function RefreshGrid(s, e) {
    gvCountry_IdentificationType.Refresh();
}

function ButtonCancel_Click(s, e) {
    showPage("Country_IdentificationType/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterCountry_IdentificationType").serialize() + "&scode=" + scode.GetValue();
      if (data != null) {
        $.ajax({
            url: "Country_IdentificationType/Country_IdentificationTypeResults",
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

function AddNewCountry_IdentificationType() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("Country_IdentificationType/FormEditCountry_IdentificationType", data);
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvCountry_IdentificationType.GetSelectedFieldValues("id", function (values) {

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
                gvCountry_IdentificationType.PerformCallback();
                gvCountry_IdentificationType.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewCountry_IdentificationType(s, e);
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



function GridViewlgvCountry_IdentificationTypeCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvCountry_IdentificationType.GetRowKey(e.visibleIndex)
        };
        showPage("Country_IdentificationType/FormEditCountry_IdentificationType", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvCountry_IdentificationType.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvCountry_IdentificationType.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvShippingLineShippingAgency.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvCountry_IdentificationType.GetSelectedRowCount() > 0 && gvCountry_IdentificationType.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvCountry_IdentificationType.GetSelectedRowCount() > 0);
    //}



    btnCopy.SetEnabled(false);
    btnRemove.SetEnabled(false);
    btnImport.SetEnabled(false);
    btnPrint.SetEnabled(false);
 
}

function GetSelectedFilteredRowCount() {
    return gvCountry_IdentificationType.cpFilteredRowCountWithoutPage + gvCountry_IdentificationType.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvCountry_IdentificationType.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvCountry_IdentificationType.SelectRows();
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