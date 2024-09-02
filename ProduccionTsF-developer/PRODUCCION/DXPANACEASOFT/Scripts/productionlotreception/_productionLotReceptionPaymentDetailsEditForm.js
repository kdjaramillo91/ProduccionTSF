
// EVENTS
function OnGridViewPaymentDetailsInit(s, e) {
    //UpdateTitlePanelPaymentDetails();
    gvProductionLotReceptionEditFormPaymentsDetail.PerformCallback();
}

function UpdateTitlePanelPaymentDetails() {

    //if (gv === null || gv === undefined)
    //    return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCountPaymentDetails();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotReceptionEditFormPaymentsDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotReceptionEditFormPaymentsDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountPaymentDetails();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";

    
    $("#lblInfoPayments").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsPayments", gvProductionLotReceptionEditFormPaymentsDetail.GetSelectedRowCount() > 0 && gvProductionLotReceptionEditFormPaymentsDetail.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionPayments", gvProductionLotReceptionEditFormPaymentsDetail.GetSelectedRowCount() > 0);
    }

   // btnRemoveDetail.SetEnabled(gvProductionLotReceptionEditFormPaymentsDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCountPaymentDetails() {
    return gvProductionLotReceptionEditFormPaymentsDetail.cpFilteredRowCountWithoutPage +
           gvProductionLotReceptionEditFormPaymentsDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewPaymentsDetailSelectionChanged(s, e) {
    UpdateTitlePanelPaymentDetails();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackPaymentsDetail);

}

function GetSelectedFieldValuesCallbackPaymentsDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewPaymentDetailsBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewPaymentDetailsEndCallback(s, e) {

    UpdateProductionLotPaymentTotals();
}

// UPDATE PRODUCTION LOT Performances

function UpdateProductionLotPaymentTotals() {
    $.ajax({
        url: "ProductionLotReception/UpdateProductionLotPaymentTotals",
        type: "post",
        data: null,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                //Resumen Entero
                wholeAveragePerformancePricePaymentSummary.SetValue(result.wholeAveragePerformancePricePaymentSummary);
                if (result.valueDistribution == "SI") {
                    FullAverageDistributedThroughputPricePaymentSummary.SetValue(result.FullAverageDistributedThroughputPricePaymentSummary);
                    wholeAverageTotalQuantityReceivedPrizePaymentDistibutedSummary.SetValue(result.wholeAverageTotalQuantityReceivedPrizePaymentDistibutedSummary);
				}
                percentWholePerformancePaymentSummary.SetValue(result.percentWholePerformancePaymentSummary);
                wholeTotalQuantityRecivedPaymentSummary.SetValue(result.wholeTotalQuantityRecivedPaymentSummary);
                wholeAverageTotalQuantityRecivedPricePaymentSummary.SetValue(result.wholeAverageTotalQuantityRecivedPricePaymentSummary);
                percentWholeTotalQuantityRecivedPaymentSummary.SetValue(result.percentWholeTotalQuantityRecivedPaymentSummary);
                wholeLbsProcessedReceivedPaymentSummary.SetValue(result.wholeLbsProcessedReceivedPaymentSummary);

                //Resumen Cola
                tailTotalQuantityRecivedPaymentSummary.SetValue(result.tailTotalQuantityRecivedPaymentSummary);
                tailAveragePerformancePricePaymentSummary.SetValue(result.tailAveragePerformancePricePaymentSummary);
                if (result.valueDistribution == "SI") {
                    tailAverageYieldDistributedPricePaymentSummary.SetValue(result.tailAverageYieldDistributedPricePaymentSummary);
                    tailAverageTotalQuantityRecivedPriceDistributedPaymentSummary.SetValue(result.tailAverageTotalQuantityRecivedPriceDistributedPaymentSummary);
				}
                percentTailPerformancePaymentSummary.SetValue(result.percentTailPerformancePaymentSummary);
                tailAverageTotalQuantityRecivedPricePaymentSummary.SetValue(result.tailAverageTotalQuantityRecivedPricePaymentSummary);
                percentTailTotalQuantityRecivedPaymentSummary.SetValue(result.percentTailTotalQuantityRecivedPaymentSummary);
                tailLbsProcessedReceivedPaymentSummary.SetValue(result.tailLbsProcessedReceivedPaymentSummary);

                //Resumen Total
                totalTotalQuantityRecivedPaymentSummary.SetValue(result.totalTotalQuantityRecivedPaymentSummary);
                totalAveragePerformancePricePaymentSummary.SetValue(result.totalAveragePerformancePricePaymentSummary);
                if (result.valueDistribution == "SI") {
                    totalAveragePerformanceDistributedPricePaymentSummary.SetValue(result.totalAveragePerformanceDistributedPricePaymentSummary);
                    totalAverageTotalQuantityRecivedPriceDistributedPaymentSummary.SetValue(result.totalAverageTotalQuantityRecivedPriceDistributedPaymentSummary);
				}
                
                totalAverageTotalQuantityRecivedPricePaymentSummary.SetValue(result.totalAverageTotalQuantityRecivedPricePaymentSummary);
                totalLbsProcessedReceivedPaymentSummary.SetValue(result.totalLbsProcessedReceivedPaymentSummary);

                //Resumen General
                //totalToPayIvaRate0PaymentSummary.SetValue(result.totalToPayIvaRate0PaymentSummary);
                //totalToPayTotalLiquidationPaymentSummary.SetValue(result.totalToPayTotalLiquidationPaymentSummary);
                //totalToPayAdvancePaymentSummary.SetValue(result.totalToPayAdvancePaymentSummary);

                //totalToPayToReceivePaymentSummary.SetValue(result.totalToPayToReceivePaymentSummary);

                //Entero
                //wholeTotalToPay.SetValue(result.wholeTotalToPayPaymentSummary);
                //percentPerformanceWholePaymentSummary.SetValue(result.percentPerformanceWholePaymentSummary);

                //Cola
                //tailTotalToPay.SetValue(result.tailTotalToPayPaymentSummary);
                //percentPerformanceTailPaymentSummary.SetValue(result.percentPerformanceTailPaymentSummary);

                //Total
                //totalToPay.SetValue(result.totalToPayPaymentSummary);
                //percentPerformanceTotalPaymentSummary.SetValue(result.percentPerformanceTotalPaymentSummary);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function gvEditPaymentDetailsClearSelection() {
    gvProductionLotReceptionEditFormPaymentsDetail.UnselectRows();
}

function gvEditPaymentDetailsSelectAllRows() {
    gvProductionLotReceptionEditFormPaymentsDetail.SelectRows();
}

