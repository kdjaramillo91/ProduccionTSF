
// FILTER FORM BUTTONS ACTIONS
function AddNewItem(s, e) {
    gvShippingAgency.AddNewRow();
}
function RefreshGrid(s, e) {
    gvShippingAgency.Refresh();
}

function ButtonCancel_Click(s, e) {
    showPage("ShippingAgency/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterShippingAgency").serialize() + "&scode=" + scode.GetValue();
      if (data != null) {
        $.ajax({
            url: "ShippingAgency/ShippingAgencyResults",
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

function AddNewShippingAgency() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("ShippingAgency/FormEditShippingAgency", data);
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvShippingAgency.GetSelectedFieldValues("id", function (values) {

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
                gvShippingAgency.PerformCallback();
                gvShippingAgency.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewShippingAgency(s, e);
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



function GridViewlgvShippingAgencyCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvShippingAgency.GetRowKey(e.visibleIndex)
        };
        showPage("ShippingAgency/FormEditShippingAgency", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvShippingAgency.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvShippingAgency.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvShippingAgency.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvShippingAgency.GetSelectedRowCount() > 0 && gvShippingAgency.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvShippingAgency.GetSelectedRowCount() > 0);
    //}



    btnCopy.SetEnabled(false);
    btnRemove.SetEnabled(false);
    btnImport.SetEnabled(false);
    btnPrint.SetEnabled(false);
 
}

function GetSelectedFilteredRowCount() {
    return gvShippingAgency.cpFilteredRowCountWithoutPage + gvShippingAgency.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvShippingAgency.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvShippingAgency.SelectRows();
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