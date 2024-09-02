// FILTERS FORM ACTIONS
function AddNewItemFromSalesRequest(s, e) {
    $.ajax({
        url: "ProductionSchedule/SalesRequestDetailsResults",
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
}

// GRIDVIEW SALES REQUEST RESULTS SELECTION

function SalesRequestDetailsOnGridViewInit(s, e) {
    SalesRequestDetailsUpdateTitlePanel();
}

function SalesRequestDetailsOnGridViewSelectionChanged(s, e) {
    SalesRequestDetailsUpdateTitlePanel();
}

function SalesRequestDetailsOnGridViewEndCallback() {
    SalesRequestDetailsUpdateTitlePanel();
}

function SalesRequestDetailsUpdateTitlePanel() {
    var selectedFilteredRowCount = SalesRequestDetailsGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvSalesRequestDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvSalesRequestDetails.GetSelectedRowCount() - SalesRequestDetailsGetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvPurchaseOrderDetails.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvSalesRequestDetails.GetSelectedRowCount() > 0 && gvSalesRequestDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvSalesRequestDetails.GetSelectedRowCount() > 0);
    //}

    btnGenerateProductionSchedule.SetEnabled(gvSalesRequestDetails.GetSelectedRowCount() > 0);
}

function SalesRequestDetailsGetSelectedFilteredRowCount() {
    return gvSalesRequestDetails.cpFilteredRowCountWithoutPage + gvSalesRequestDetails.GetSelectedKeysOnPage().length;
}

function SalesRequestDetailsSelectAllRow() {
    gvSalesRequestDetails.SelectRows();
}

function SalesRequestDetailsClearSelection() {
    gvSalesRequestDetails.UnselectRows();
}

// GRIDVIEW PURCHASE ORDER RESULTS ACTIONS

function GenerateProductionSchedule(s, e) {

    //gridMessageErrorSalesRequestDetail.SetText("");
    //$("#GridMessageErrorSalesRequestDetail").hide();

    gvSalesRequestDetails.GetSelectedFieldValues("id"/*"id_purchaseRequest;id_item"*/, function (values) {

        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        var data = {
            id: 0,
            //ids: selectedRows
            ids_salesRequestDetail: selectedRows
        };

        //for (var k = 0; k < selectedRows.length; k++) {
        //    data.requestDetails.push({
        //        id_purchaseRequest: selectedRows[k][0],
        //        id_item: selectedRows[k][1]
        //    });
        //}

        showPage("ProductionSchedule/ProductionScheduleFormEditPartial", data);
        //$.ajax({
        //    url: "ProductionSchedule/ValidateSelectedRowsSalesRequestDetail",
        //    type: "post",
        //    data: { ids: selectedRows },
        //    async: true,
        //    cache: false,
        //    error: function (error) {
        //        console.log(error);
        //    },
        //    beforeSend: function () {
        //        showLoading();

        //    },
        //    success: function (result) {
        //        //resultFunction = result.enabledBtnGenerateLot;
        //        if (result.Message == "OK") {
        //            showPage("ProductionSchedule/ProductionScheduleFormEditPartial", data);
        //        } else {
        //            gridMessageErrorSalesRequestDetail.SetText(result.Message);
        //            $("#GridMessageErrorSalesRequestDetail").show();
        //            hideLoading();
        //        }
        //    },
        //    complete: function () {
        //        //hideLoading();
        //        //gvProductionLotReceptions.PerformCallback();
        //        // gvPurchaseOrders.UnselectRows();
        //    }
        //});


    });

    //console.log(selectedPurchaseOrderDetailsRows);

    //var data = {
    //    id: 0,
    //    orderDetails: []
    //};
    //for (var i = 0; i < selectedPurchaseOrderDetailsRows.length; i++) {
    //    data.orderDetails.push(selectedPurchaseOrderDetailsRows[i]);
    //}

    //showPage("Logistics/FormEditRemissionGuide", data);


    //gvPurchaseOrderDetails.GetSelectedFieldValues("id", function (values) {

    //    var selectedRows = [];

    //    for (var i = 0; i < values.length; i++) {
    //        selectedRows.push(values[i]);
    //    }

    //    var data = {
    //        id: 0,
    //        ids_purchaseOrdersDetails: selectedRows
    //    };

    //    showPage("PurchasePlanning/PurchasePlanningFormEditPartial", data);
    //});
}

//Validation 



function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
}

function OnRangeDatePlanningValidation(s, e) {
    OnRangeDateValidation(e, startDatePlanning.GetValue(), endDatePlanning.GetValue(), "Rango de Fecha no válido");
}

// Filter Action Buttons
function OnClickSearchProductionSchedule() {
    var data = $("#ProductionScheduleFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionSchedule/ProductionScheduleResultsPartial",
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
}

function OnClickClearFiltersProductionSchedule() {
    DocumentStateCombo_Init(null, null);

    number.SetText("");

    startEmissionDate.SetValue(null);
    endEmissionDate.SetValue(null);
    
    ProductionSchedulePeriodCombo_Init(null, null);

    PersonScheduleCombo_Init(null, null);

    providers.ClearTokenCollection();
    buyers.ClearTokenCollection();
    items.ClearTokenCollection();

    startDatePlanning.SetValue(null);
    endDatePlanning.SetValue(null);
}

function ButtonManualAddNewProductionSchedule_Click() {
    var data = {
        id: 0
    };
    showPage("ProductionSchedule/ProductionScheduleFormEditPartial", data);
}

//function OnClickAddNewPurchasePlanning(trx) {

//    ButtonManualAddNewPurchasePlanning_Click();
//    //if (trx === "M")
//    //showPage("PurchasePlanning/PurchasePlanningFormEditPartial", data);
//}

// Filter ComboBox
function DocumentStateCombo_Init(s, e) {
    id_documentState.SetValue(null);
    id_documentState.SetText("");
}

function ProductionSchedulePeriodCombo_Init(s, e) {
    id_productionSchedulePeriod.SetValue(null);
    id_productionSchedulePeriod.SetText("");
}

function PersonScheduleCombo_Init(s, e) {
    filterPersonSchedule.SetValue(null);
    filterPersonSchedule.SetText("");
}


//function ItemTypeCombo_Init() {
//    filterItemType.SetValue(-1);
//    filterItemType.SetText("");
//}

//function ItemTypeCategoryCombo_Init() {
//    filterItemTypeCategory.SetValue(-1);
//    filterItemTypeCategory.SetText("");
//}


// Results GridView Selection
function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    //console.log("Estoy en el UpdateTitlePanel del Index");
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvProductionSchedules.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionSchedules.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvProductionSchedules.GetSelectedRowCount() > 0 && gvProductionSchedules.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductionSchedules.GetSelectedRowCount() > 0);
    //}

    //btnCopy.SetEnabled(gvProductionSchedules.GetSelectedRowCount() == 1);
    //btnApprove.SetEnabled(gvProductionSchedules.GetSelectedRowCount() > 0);
    //btnAutorize.SetEnabled(gvProductionSchedules.GetSelectedRowCount() > 0);
    //btnProtect.SetEnabled(gvProductionSchedules.GetSelectedRowCount() > 0);
    //btnCancel.SetEnabled(gvProductionSchedules.GetSelectedRowCount() > 0);
    //btnRevert.SetEnabled(gvProductionSchedules.GetSelectedRowCount() > 0);
    //btnHistory.SetEnabled(gvProductionSchedules.GetSelectedRowCount() == 1);
    //btnPrint.SetEnabled(gvPurchasePlannings.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvProductionSchedules.cpFilteredRowCountWithoutPage + gvProductionSchedules.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvProductionSchedules.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvProductionSchedules.SelectRows();
}

// Results GridView Acction Buttons

function PerformDocumentAction(url) {
    gvPurchasePlannings.GetSelectedFieldValues("id", function (values) {

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
                console.log(result);
            },
            complete: function () {
                //hideLoading();
                gvPurchasePlannings.PerformCallback();
                // gvPurchaseOrders.UnselectRows();
            }
        });

    });
}

//btnNew
function AddNewDocument(s, e) {
    ButtonManualAddNewProductionSchedule_Click();
}

//btnCopy
function CopyDocument(s, e) {
    gvPurchasePlannings.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("PurchasePlanning/PurchasePlanningCopy", { id: values[0] });
        }
    });
}

//btnApprove
function ApproveDocuments(s, e) {
    //var c = confirm("¿Desea aprobar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Appr");
    //}

    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchasePlanning/ApproveDocuments");
    //}, "¿Desea aprobar los lotes seleccionados?");
}

//btnAutorize
function AutorizeDocuments(s, e) {
    //var c = confirm("¿Desea autorizar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Auth");
    //}
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchasePlanning/AutorizeDocuments");
    //}, "¿Desea autorizar los lotes seleccionados?");
}

//btnProtect
function ProtectDocuments(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchasePlanning/ProtectDocuments");
    //}, "¿Desea cerrar los lotes seleccionados?");
}

//btnCancel
function CancelDocuments(s, e) {
    //var c = confirm("¿Desea anular los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Canc");
    //}
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchasePlanning/CancelDocuments");
    //}, "¿Desea anular los lotes seleccionados?");
}

//btnRevert
function RevertDocuments(s, e) {
    //var c = confirm("¿Desea reversar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Rev");
    //}
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("PurchasePlanning/RevertDocuments");
    //}, "¿Desea reversar los lotes seleccionados?");
}

//btnHistory
function ShowHistory(s, e) {

}

//btnPrint
function Print(s, e) {
    gvPurchasePlannings.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "PurchasePlanning/PurchasePlanningReport",
            type: "post",
            data: { id: selectedRows[0] },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                $("#maincontent").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });

    });
}

function OnClickUpdateProductionSchedule(s, e) {

    var data = {
        id: gvProductionSchedules.GetRowKey(e.visibleIndex)
    };

    showPage("ProductionSchedule/ProductionScheduleFormEditPartial", data);
}

function ChangeState(trx) {
    //$.ajax({
    //    url: "PurchaseRequest/ChangeStateSelectedDocuments",
    //    type: "post",
    //    data: { ids: selectedRows, trx: trx },
    //    async: true,
    //    cache: false,
    //    error: function (error) {
    //        console.log(error);
    //    },
    //    beforeSend: function () {
    //        showLoading();
    //    },
    //    success: function (result) {
    //        console.log(result);
    //    },
    //    complete: function () {
    //        gvPurchaseRequests.UnselectRows();
    //        gvPurchaseRequests.PerformCallback();
    //        hideLoading();
    //    }
    //});
}

// DETAILS VIEW CALLBACKS

//function PurchasePlanningDetailItems_BeginCallback(s, e) {
//    e.customArgs["id_purchasePlanning"] = s.cpIdPurchasePlanning;
//}


// Init
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
