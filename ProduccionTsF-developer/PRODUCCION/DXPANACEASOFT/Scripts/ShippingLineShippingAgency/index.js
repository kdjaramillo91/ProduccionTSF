
// FILTER FORM BUTTONS ACTIONS
function AddNewItem(s, e) {
    gvShippingLineShippingAgency.AddNewRow();
}
function RefreshGrid(s, e) {
    gvShippingLineShippingAgency.Refresh();
}

function ButtonCancel_Click(s, e) {
    showPage("ShippingLineShippingAgency/Index", null);
}
function btnSearch_click() {
   
    var data = $("#formFilterShippingLineShippingAgency").serialize() + "&scode=" + scode.GetValue();
      if (data != null) {
        $.ajax({
            url: "ShippingLineShippingAgency/ShippingLineShippingAgencyResults",
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

function AddNewShippingLineShippingAgency() {
     
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("ShippingLineShippingAgency/FormEditShippingLineShippingAgency", data);
}



// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvShippingLineShippingAgency.GetSelectedFieldValues("id", function (values) {

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
                gvShippingLineShippingAgency.PerformCallback();
                gvShippingLineShippingAgency.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewShippingLineShippingAgency(s, e);
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

function GridViewlgvShippingLineShippingAgencyCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvShippingLineShippingAgency.GetRowKey(e.visibleIndex)
        };
        showPage("ShippingLineShippingAgency/FormEditShippingLineShippingAgency", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvShippingLineShippingAgency.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvShippingLineShippingAgency.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvShippingLineShippingAgency.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvShippingLineShippingAgency.GetSelectedRowCount() > 0 && gvShippingLineShippingAgency.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvShippingLineShippingAgency.GetSelectedRowCount() > 0);
    //}



    btnCopy.SetEnabled(false);
    btnRemove.SetEnabled(false);
    btnImport.SetEnabled(false);
    btnPrint.SetEnabled(false);
 
}

function GetSelectedFilteredRowCount() {
    return gvShippingLineShippingAgency.cpFilteredRowCountWithoutPage + gvShippingLineShippingAgency.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvShippingLineShippingAgency.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvShippingLineShippingAgency.SelectRows();
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