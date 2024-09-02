
// FILTER FORM BUTTONS ACTIONS
function ButtonCancel_Click(s, e) {
    showPage("LiquidationFreightRiver/Index", null);
}

function btnSearch_click() {
     
    //var data = $("#formFilterPurchaseOrder").serialize();
    var data = $("#formFilterLiquidationFreightRiver").serialize() + "&carRegistration=" + carRegistration.GetValue() + "&id_providerfilter=" + id_providerremision.GetValue() + "&startEmissionDate=" + startEmissionDate.GetValue() + "&endEmissionDate=" + endEmissionDate.GetValue() + "&id_providertransport=" + id_providertransport.GetValue();
    var data = data + "&startAuthorizationDate=" + startAuthorizationDate.GetValue() + "&endAuthorizationDate=" + endAuthorizationDate.GetValue();
    if (data != null) {
        $.ajax({
            url: "LiquidationFreightRiver/LiquidationFreightRiverResults",
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
    id_documentState.SetSelectedItem(null);
    number.SetText("");
    
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
    startAuthorizationDate.SetDate(null);
    endAuthorizationDate.SetDate(null);
    id_providertransport.SetSelectedItem(null);
    carRegistration.SetText("")
    number.SetText("");
    id_providerremision.SetSelectedItem(null);
    //accessKey.SetText("");
  
    
}

function AddNewGuideRemissionRiverManual() {
     
    var data = {
        id: 0,
        requestDetails: []
    };

    showPage("LiquidationFreightRiver/RemissionGuideRiverResults", data);
}


// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvLiquidationFreightRiver.GetSelectedFieldValues("id", function (values) {

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
                gvLiquidationFreightRiver.PerformCallback();
                gvLiquidationFreightRiver.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    AddNewGuideRemissionRiverManual(s, e);
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

// LIQUIDATION RESULT GRIDVIEW EDIT ACTION

function GridViewliquidationfreightRiverCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvLiquidationFreightRiver.GetRowKey(e.visibleIndex)
        };
        showPage("LiquidationFreightRiver/FormEditLiquidationFreightRiver", data);
    }
}

// REMISSION GUIDES RESULT GRIDVIEW SELECTION

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

    var text = "Total de elementos seleccionados: <b>" + gvLiquidationFreightRiver.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvLiquidationFreightRiver.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvLiquidationFreightRiver.GetSelectedRowCount() > 0 && gvLiquidationFreightRiver.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvLiquidationFreightRiver.GetSelectedRowCount() > 0);

    btnCopy.SetEnabled(false);
    btnApprove.SetEnabled(false);
    btnAutorize.SetEnabled(false);
    btnProtect.SetEnabled(false);
    btnCancel.SetEnabled(false);
    btnRevert.SetEnabled(false);
    btnHistory.SetEnabled(false);
    btnPrint.SetEnabled(false);
}

function GetSelectedFilteredRowCount() {
    return gvLiquidationFreightRiver.cpFilteredRowCountWithoutPage + gvLiquidationFreightRiver.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvLiquidationFreightRiver.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvLiquidationFreightRiver.SelectRows();
}

// GENERATE REMISSION GUIDE FROM PURCHASE ORDERS

function GenerateRemissionGuideRiver(s, e) {
    GridMessageErrorRemissionGuide.SetText("");
    $("#GridMessageErrorRemissionGuide").hide();
    
    showLoading();
    // 
    gvRemissionGuideRiverDetails.GetSelectedFieldValues("id"/*"id_purchaseRequest;id_item"*/, function (values) {

        var selectedRemissionGuideRiverDetailsDetailsRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRemissionGuideRiverDetailsDetailsRows.push(values[i]);
        }
      
        var data = {
            id: 0,
            orderDetails: selectedRemissionGuideRiverDetailsDetailsRows
        };

        // 


        $.ajax({
            url: "LiquidationFreightRiver/ValidateSelectedRowsRemissionGuideRiver",
            type: "post",
            data: { ids: data.orderDetails },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();

            },
            success: function (result) {
                // 
                //resultFunction = result.enabledBtnGenerateLot;
                if (result.Message == "OK") {
                    
                    showPage("LiquidationFreightRiver/FormEditLiquidationFreightRiver", data);
                } else {
                    GridMessageErrorRemissionGuide.SetText(result.Message);
                    $("#GridMessageErrorRemissionGuide").show();
                    hideLoading();
                }
            },
            complete: function () {
               hideLoading();
               }
        });


    });

}

// PURCHASE ORDERS RESULT GRIDVIEW SELECTION

function OnGridViewRemissionGuideRiverDetailsInit(s, e) {
    UpdateTitlePanelOrderDetails();
}

var selectedRemissionGuideRiverDetailsRows = [];

function OnGridViewRemissionGuideRiverDetailsSelectionChanged(s, e) {
    // 
    UpdateTitlePanelOrderDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

function GetSelectedFieldDetailValuesCallback(values) {
    // 
    selectedRemissionGuideRiverDetailsRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRemissionGuideRiverDetailsRows.push(values[i]);
    }
}

function OnGridViewRemissionGuideRiverDetailsEndCallback() {
    UpdateTitlePanelOrderDetails();
}

function UpdateTitlePanelOrderDetails() {
    var selectedFilteredRowCount = GetSelectedFilteredOrderDetailsRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideRiverDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideRiverDetails.GetSelectedRowCount() - GetSelectedFilteredOrderDetailsRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideRiverDetails.GetSelectedRowCount() > 0 && gvRemissionGuideRiverDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideRiverDetails.GetSelectedRowCount() > 0);

    btnGenerateRemissionGuideRiver.SetEnabled(gvRemissionGuideRiverDetails.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredOrderDetailsRowCount() {
    return gvRemissionGuideRiverDetails.cpFilteredRowCountWithoutPage + gvRemissionGuideRiverDetails.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function GridViewRemissionGuideRiverDetailsClearSelection() {
    gvRemissionGuideRiverDetails.UnselectRows();
}

function GridViewRemissionGuideRiverDetailsSelectAllRow() {
    gvRemissionGuideRiverDetails.SelectRows();
}

// REMISSION GUIDE MASTER DETAILS FUNCTIONS

function LiquidationFreightRiverResultsDetailViewPartial_BeginCallback(s, e) {
    e.customArgs["id_LiquidationFreightRiver"] = $("#id_LiquidationFreightRiver").val();
}

function LiquidationFreightRiverDetailViewDetailsPartial_BeginCallback(s, e) {
    e.customArgs["id_LiquidationFreightRiver"] = $("#id_LiquidationFreightRiver").val();
}

function PRLiquidationRiverVehicle(s, e) {
    var codeReport = "LFRTRF";
    var data = "codeReport=" + codeReport + "&" + $("#formFilterLiquidationFreight").serialize();

    if (data != null) {
        $.ajax({
            url: "LiquidationFreightRiver/PRLiquidationRiverRotation",
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
                // 
                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/Index?trepd=' + reportTdr;
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}


// MAINS FUNCTIONS
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