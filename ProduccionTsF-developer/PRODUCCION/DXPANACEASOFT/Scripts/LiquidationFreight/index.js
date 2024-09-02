
// FILTER FORM BUTTONS ACTIONS
function ButtonCancel_Click(s, e) {
    showPage("LiquidationFreight/Index", null);
}

function btnSearch_click() {
     
    //var data = $("#formFilterPurchaseOrder").serialize();
    var data = $("#formFilterLiquidationFreight").serialize() + "&carRegistration=" + carRegistration.GetValue() + "&id_providerfilter=" + id_providerremision.GetValue() + "&startEmissionDate=" + startEmissionDate.GetValue() + "&endEmissionDate=" + endEmissionDate.GetValue() + "&id_providertransport=" + id_providertransport.GetValue();
    var data = data + "&startAuthorizationDate=" + startAuthorizationDate.GetValue() + "&endAuthorizationDate=" + endAuthorizationDate.GetValue();
    if (data != null) {
        $.ajax({
            url: "LiquidationFreight/LiquidationFreightResults",
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

function AddNewGuideRemissionManual() {
     
    var data = {
        id: 0,
        requestDetails: []
    };

    showPage("LiquidationFreight/RemissionGuideResults", data);
}

function AddNewGuideRemissionFromPurchaseOrder() {
    $.ajax({
        url: "Logistics/PurchaseOrderDetailsResults",
        type: "post",
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

    event.preventDefault();
}

// GRIDVIEW RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvLiquidationFreight.GetSelectedFieldValues("id", function (values) {

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
                gvLiquidationFreight.PerformCallback();
                gvLiquidationFreight.UnselectRows();
            }
        });

    });
}
function AddNewDocument(s, e) {
    AddNewGuideRemissionManual(s, e);
}
function CopyDocument(s, e) {
    //gvLiquidationFreight.GetSelectedFieldValues("id", function (values) {
    //    if (values.length > 0) {
    //        showPage("Logistics/RemissionGuideCopy", { id: values[0] });
    //    }
   // });
}
function ApproveDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("Logistics/ApproveDocuments");
    //}, "¿Desea aprobar los documentos seleccionados?");
}
function AutorizeDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("Logistics/AutorizeDocuments");
    //}, "¿Desea autorizar los documentos seleccionados?");
}
function ProtectDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("Logistics/ProtectDocuments");
    //}, "¿Desea cerrar los documentos seleccionados?");
}
function CancelDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("Logistics/CancelDocuments");
    //}, "¿Desea anular los documentos seleccionados?");
}
function RevertDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("Logistics/RevertDocuments");
    //}, "¿Desea reversar los documentos seleccionados?");
}
function ShowHistory(s, e) {

}
function Print(s, e) {
    //gvLiquidationFreight.GetSelectedFieldValues("id", function (values) {

    //    var selectedRows = [];
    //    for (var i = 0; i < values.length; i++) {
    //        selectedRows.push(values[i]);
    //    }

    //    $.ajax({
    //        url: "Logistics/RemissionGuideReport",
    //        type: "post",
    //        data: { id: selectedRows[0] },
    //        async: true,
    //        cache: false,
    //        error: function (error) {
    //            console.log(error);
    //        },
    //        beforeSend: function () {
    //            showLoading();
    //        },
    //        success: function (result) {
    //            $("#maincontent").html(result);
    //        },
    //        complete: function () {
    //            hideLoading();
    //        }
    //    });

    //});
}

// LIQUIDATION RESULT GRIDVIEW EDIT ACTION

function GridViewliquidationfreightCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        var data = {
            id: gvLiquidationFreight.GetRowKey(e.visibleIndex)
        };
        showPage("LiquidationFreight/FormEditLiquidationFreight", data);
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

    var text = "Total de elementos seleccionados: <b>" + gvLiquidationFreight.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvLiquidationFreight.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvLiquidationFreight.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvLiquidationFreight.GetSelectedRowCount() > 0 && gvLiquidationFreight.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvLiquidationFreight.GetSelectedRowCount() > 0);
    //}

    //btnCopy.SetEnabled(gvLiquidationFreight.GetSelectedRowCount() == 1);
    //btnApprove.SetEnabled(gvLiquidationFreight.GetSelectedRowCount() > 0);
    //btnAutorize.SetEnabled(gvLiquidationFreight.GetSelectedRowCount() > 0);
    //btnProtect.SetEnabled(gvLiquidationFreight.GetSelectedRowCount() > 0);
    //btnCancel.SetEnabled(gvLiquidationFreight.GetSelectedRowCount() > 0);
    //btnRevert.SetEnabled(gvLiquidationFreight.GetSelectedRowCount() > 0);
    //btnHistory.SetEnabled(gvLiquidationFreight.GetSelectedRowCount() == 1);
    //btnPrint.SetEnabled(gvLiquidationFreight.GetSelectedRowCount() == 1);

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
    return gvLiquidationFreight.cpFilteredRowCountWithoutPage + gvLiquidationFreight.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvLiquidationFreight.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvLiquidationFreight.SelectRows();
}

// GENERATE REMISSION GUIDE FROM PURCHASE ORDERS

function GenerateRemissionGuide(s, e) {
    //console.log(selectedPurchaseOrderDetailsRows);
     
    GridMessageErrorRemissionGuide.SetText("");
    $("#GridMessageErrorRemissionGuide").hide();
    
    showLoading();

    gvRemissionGuideDetails.GetSelectedFieldValues("id"/*"id_purchaseRequest;id_item"*/, function (values) {

        var selectedRemissionGuideDetailsDetailsRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRemissionGuideDetailsDetailsRows.push(values[i]);
        }
      
        var data = {
            id: 0,
            orderDetails: selectedRemissionGuideDetailsDetailsRows
        };

        


        $.ajax({
            url: "LiquidationFreight/ValidateSelectedRowsRemissionGuide",
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
                //resultFunction = result.enabledBtnGenerateLot;
                if (result.Message == "OK") {
                    
                    showPage("LiquidationFreight/FormEditLiquidationFreight", data);
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

function OnGridViewRemissionGuideDetailsInit(s, e) {
    UpdateTitlePanelOrderDetails();
}
var selectedRemissionGuideDetailsRows = [];
function OnGridViewRemissionGuideDetailsSelectionChanged(s, e) {
    UpdateTitlePanelOrderDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}
function GetSelectedFieldDetailValuesCallback(values) {
    // 
    selectedRemissionGuideDetailsRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRemissionGuideDetailsRows.push(values[i]);
    }
}
function OnGridViewRemissionGuideDetailsEndCallback() {
    UpdateTitlePanelOrderDetails();
}

function UpdateTitlePanelOrderDetails() {
    var selectedFilteredRowCount = GetSelectedFilteredOrderDetailsRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuideDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuideDetails.GetSelectedRowCount() - GetSelectedFilteredOrderDetailsRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRemissionGuideDetails.GetSelectedRowCount() > 0 && gvRemissionGuideDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuideDetails.GetSelectedRowCount() > 0);

    btnGenerateRemissionGuide.SetEnabled(gvRemissionGuideDetails.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredOrderDetailsRowCount() {
    return gvRemissionGuideDetails.cpFilteredRowCountWithoutPage + gvRemissionGuideDetails.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function GridViewRemissionGuideDetailsClearSelection() {
    gvRemissionGuideDetails.UnselectRows();
}

function GridViewRemissionGuideDetailsSelectAllRow() {
    gvRemissionGuideDetails.SelectRows();
}

// REMISSION GUIDE MASTER DETAILS FUNCTIONS

function LiquidationFreightResultsDetailViewPartial_BeginCallback(s, e) {
    e.customArgs["id_LiquidationFreight"] = $("#id_LiquidationFreight").val();
}

function LiquidationFreightDetailViewDetailsPartial_BeginCallback(s, e) {
    e.customArgs["id_LiquidationFreight"] = $("#id_LiquidationFreight").val();
}

function PRViaticTerrestrel(s, e) {
    var codeReport = "LGRVT";
    var data = "codeReport=" + codeReport + "&" + $("#formFilterLiquidationFreight").serialize();

    if (data != null) {
        $.ajax({
            url: "LiquidationFreight/PRViaticTerrestrel",
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

function PRLiquidationVehicle(s, e) {
    var codeReport = "LFRTR";
    var data = "codeReport=" + codeReport + "&" + $("#formFilterLiquidationFreight").serialize();

    if (data != null) {
        $.ajax({
            url: "LiquidationFreight/PRLiquidationRotation",
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