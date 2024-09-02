
// VALIDATIONS

//function OnTailQuantityRecivedValidation(s, e) {
//    var valueAux = subtotalTailAdjustSumary.GetValue() + tailTotalQuantityTrashSumary.GetValue() + poundsGarbageTail.GetValue();
//    valueAux = (valueAux).toFixed(2);
//    var tailTotalQuantityRecivedSumaryAux = (tailTotalQuantityRecivedSumary.GetValue()).toFixed(2);
//    if (valueAux != tailTotalQuantityRecivedSumaryAux) {
//        e.isValid = false;
//        e.errorText = "La suma de Lbs Cola Ajustadas mas Desperdicio Cola mas Basura Cola deben ser igual a Lbs Cola Recibidas";
//    }
//}

function OnWholeGarbagePoundsValueChangedLiq(s, e) {
    // 
    var wholeGarbagePoundsAux = wholeGarbagePoundsLiq.GetValue();
    var poundsGarbageTailAux = poundsGarbageTailLiq.GetValue();
    var wholeSubtotalAdjustSumaryAux = wholeSubtotalAdjustSumaryLiq.GetValue();
    var wholeLeftoverAux = wholeLeftoverLiq.GetValue();
    var wholeTotalQuantityRecivedSumaryAux = wholeSubtotalAdjustSumaryAux + wholeGarbagePoundsAux + wholeLeftoverAux;
    wholeTotalQuantityRecivedSumaryAux = wholeTotalQuantityRecivedSumaryAux.toFixed(2);
    wholeTotalQuantityRecivedSumaryLiq.SetValue(wholeTotalQuantityRecivedSumaryAux);
    totalTotalQuantityRecivedSumaryLiq.SetValue(parseFloat(wholeTotalQuantityRecivedSumaryAux) + tailTotalQuantityRecivedSumaryLiq.GetValue());
    var wholeNetSumaryAux = wholeTotalQuantityRecivedSumaryAux - wholeGarbagePoundsAux;
        //- poundsGarbageTailAux;
    if (wholeGarbagePoundsAux >= 0) {
        wholeNetSumaryLiq.SetValue(wholeNetSumaryAux);
        var percentWholePerformanceSumaryAux = wholeNetSumaryAux > 0 ? wholeSubtotalAdjustSumaryAux / wholeNetSumaryAux : 0;
        percentWholePerformanceSumaryLiq.SetValue((percentWholePerformanceSumaryAux).toFixed(4));
    } else {
        wholeNetSumaryLiq.SetValue(0);
        percentWholePerformanceSumaryLiq.SetValue(0);
    }
    UpdateTotalPoundsGarbageSumaryLiq(s, e);
}

function OnPoundsGarbageTailValueChangedLiq(s, e) {
    // 
    var _tipoProceso = $("#tipoProceso").val();
    if (wholeSubtotalSumaryLiq.GetValue() === 0.00) {
        _tipoProceso = "COL";
    } else {
        _tipoProceso = "ENT";
    }
    var poundsGarbageTailAux = poundsGarbageTailLiq.GetValue();

    if (_tipoProceso === "ENT") {
        var wholeGarbagePoundsAux = wholeGarbagePoundsLiq.GetValue();
        var wholeSubtotalAdjustSumaryAux = wholeSubtotalAdjustSumaryLiq.GetValue();
        var wholeLeftoverAux = wholeLeftoverLiq.GetValue();
        var wholeTotalQuantityRecivedSumaryAux = wholeSubtotalAdjustSumaryAux + wholeGarbagePoundsAux + wholeLeftoverAux;
        wholeTotalQuantityRecivedSumaryAux = wholeTotalQuantityRecivedSumaryAux.toFixed(2);
        //var wholeGarbagePoundsAux = wholeGarbagePoundsLiq.GetValue();
        //var wholeSubtotalAdjustSumaryAux = wholeSubtotalAdjustSumaryLiq.GetValue();
        //var wholeNetSumaryAux = wholeNetSumaryLiq.GetValue();
        var wholeNetSumaryAux = wholeTotalQuantityRecivedSumaryAux - wholeGarbagePoundsAux;

        var tailNetSumaryAux = tailTotalQuantityRecivedSumaryLiq.GetValue() - poundsGarbageTailAux;
        var tailNetSumaryAux2 = wholeLeftoverAux - poundsGarbageTailAux;
        if (poundsGarbageTailAux >= 0) {
            var wholeNetSumaryAux2 = wholeNetSumaryAux - poundsGarbageTailAux;
            tailNetSumaryLiq.SetValue(tailNetSumaryAux2);
            //wholeNetSumaryLiq.SetValue(wholeNetSumaryAux2);
            var percentTailPerformanceSumaryAux = tailNetSumaryAux2 > 0 ? subtotalTailAdjustSumaryLiq.GetValue() / tailNetSumaryAux2 : 0;
            percentTailPerformanceSumaryLiq.SetValue((percentTailPerformanceSumaryAux).toFixed(4));
        } else {
            //wholeNetSumaryLiq.SetValue(wholeNetSumaryAux);
            tailNetSumaryLiq.SetValue(0);
            percentTailPerformanceSumaryLiq.SetValue(0);
        }
        UpdateTotalPoundsGarbageSumaryLiq(s, e);
		
    }
    else {
        var tailLeftOverAux = tailLeftOverLiq.GetValue();
        //var poundsGarbageTailAux = poundsGarbageTailLiq.GetValue();

        tailLeftOverAux = (tailLeftOverAux >= 0) ? tailLeftOverAux : 0;
        poundsGarbageTailAux = (poundsGarbageTailAux >= 0) ? poundsGarbageTailAux : 0;
        tailTotalQuantityRecivedSumaryLiq.SetValue(tailLeftOverAux);
        tailNetSumaryLiq.SetValue(tailLeftOverAux - poundsGarbageTailAux);
        var subtotalTailAdjustSumaryAux = subtotalTailAdjustSumaryLiq.GetValue();
        var tailNetSumaryAuxLiq = tailNetSumaryLiq.GetValue();
        var percentTailPerformanceSumaryAuxLiq = (tailNetSumaryAuxLiq > 0) ? subtotalTailAdjustSumaryAux / tailNetSumaryAuxLiq : 0;
        percentTailPerformanceSumaryLiq.SetValue((percentTailPerformanceSumaryAuxLiq).toFixed(4));
    }
    var WholeNetSumaryAux3 = wholeNetSumaryLiq.GetValue();
    var tailNetSumaryAux3 = tailNetSumaryLiq.GetValue();
    totalNetSumaryLiq.SetValue(WholeNetSumaryAux3 + tailNetSumaryAux3);
}

function UpdateTotalPoundsGarbageSumaryLiq(s, e) {
    poundsGarbageTotalLiq.SetValue(wholeGarbagePoundsLiq.GetValue() + poundsGarbageTailLiq.GetValue());
    totalNetSumaryLiq.SetValue(0);
}

function UpdateTotalWholeLeftoverSumaryAuxLiq() {
	// 
    var wholeLeftoverAux = wholeLeftoverLiq.GetValue();
    totalWholeLeftoverSumaryLiq.SetValue(wholeLeftoverAux);
        //+ tailLeftoverSumaryLiq.GetValue());
    var tailTotalQuantityRecivedSumaryAux = tailTotalQuantityRecivedSumaryLiq.GetValue();
    totalTotalQuantityRecivedSumaryLiq.SetValue(tailTotalQuantityRecivedSumaryAux + wholeTotalQuantityRecivedSumaryLiq.GetValue());
    var poundsGarbageTailAux = poundsGarbageTailLiq.GetValue();
    var tailNetSumaryAux = tailTotalQuantityRecivedSumaryAux - poundsGarbageTailAux;
    var tailNetSumaryAux2 = wholeLeftoverAux - poundsGarbageTailAux;
    if (poundsGarbageTailAux >= 0) {
        tailNetSumaryLiq.SetValue(tailNetSumaryAux2);
        var percentTailPerformanceSumaryAux = tailNetSumaryAux2 > 0 ? subtotalTailAdjustSumaryLiq.GetValue() / tailNetSumaryAux2 : 0;
        percentTailPerformanceSumaryLiq.SetValue((percentTailPerformanceSumaryAux).toFixed(4));
    } else {
        tailNetSumaryLiq.SetValue(0);
        percentTailPerformanceSumaryLiq.SetValue(0);
    }
    var WholeNetSumaryAux = wholeNetSumaryLiq.GetValue();
    totalNetSumaryLiq.SetValue(tailNetSumaryLiq.GetValue() + WholeNetSumaryAux);

    var wholeGarbagePoundsAux = wholeGarbagePoundsLiq.GetValue();
    //var poundsGarbageTailAux = poundsGarbageTailLiq.GetValue();
    var wholeSubtotalAdjustSumaryAux = wholeSubtotalAdjustSumaryLiq.GetValue();
    //var wholeLeftoverAux = wholeLeftoverLiq.GetValue();
    var wholeTotalQuantityRecivedSumaryAux = wholeSubtotalAdjustSumaryAux + wholeGarbagePoundsAux + wholeLeftoverAux;
    wholeTotalQuantityRecivedSumaryAux = wholeTotalQuantityRecivedSumaryAux.toFixed(2);
    //wholeTotalQuantityRecivedSumaryLiq.SetValue(wholeTotalQuantityRecivedSumaryAux);
    //totalTotalQuantityRecivedSumaryLiq.SetValue(parseFloat(wholeTotalQuantityRecivedSumaryAux) + tailTotalQuantityRecivedSumaryLiq.GetValue());
    var wholeNetSumaryAux = wholeTotalQuantityRecivedSumaryAux - wholeGarbagePoundsAux;
    //- poundsGarbageTailAux;
    if (wholeGarbagePoundsAux >= 0) {
        wholeNetSumaryLiq.SetValue(wholeNetSumaryAux);
        var percentWholePerformanceSumaryAux = wholeNetSumaryAux > 0 ? wholeSubtotalAdjustSumaryAux / wholeNetSumaryAux : 0;
        percentWholePerformanceSumaryLiq.SetValue((percentWholePerformanceSumaryAux).toFixed(4));
    } else {
        wholeNetSumaryLiq.SetValue(0);
        percentWholePerformanceSumaryLiq.SetValue(0);
    }

}

function UpdateTotalWholeLeftoverSumaryLiq(s, e) {
    var wholeLeftoverAux = wholeLeftoverLiq.GetValue();
    var wholeGarbagePoundsAux = wholeGarbagePoundsLiq.GetValue();
    tailTotalQuantityRecivedSumaryLiq.SetValue(wholeLeftoverAux);
    //tailTotalQuantityRecivedSumary.SetValue(0);

    var wholeSubtotalAdjustSumaryAux = wholeSubtotalAdjustSumaryLiq.GetValue();
    var wholeTotalQuantityRecivedSumaryAux = wholeSubtotalAdjustSumaryAux + wholeGarbagePoundsAux + wholeLeftoverAux;
    wholeTotalQuantityRecivedSumaryAux = wholeTotalQuantityRecivedSumaryAux.toFixed(2);
    wholeTotalQuantityRecivedSumaryLiq.SetValue(wholeTotalQuantityRecivedSumaryAux);

    UpdateTotalWholeLeftoverSumaryAuxLiq();
}

function UpdateTotaltailLeftoverSumaryLiq(s, e) {
    var tailLeftOverAux = tailLeftOverLiq.GetValue();
    var poundsGarbageTailAux = poundsGarbageTailLiq.GetValue();

    tailLeftOverAux = (tailLeftOverAux >= 0) ? tailLeftOverAux : 0;
    poundsGarbageTailAux = (poundsGarbageTailAux >= 0) ? poundsGarbageTailAux : 0;
    tailTotalQuantityRecivedSumaryLiq.SetValue(tailLeftOverAux);
    tailNetSumaryLiq.SetValue(tailLeftOverAux - poundsGarbageTailAux);

    var subtotalTailAdjustSumaryAux = subtotalTailAdjustSumaryLiq.GetValue()
    var tailNetSumaryAux = tailNetSumaryLiq.GetValue();
    var percentTailPerformanceSumaryAux = (tailNetSumaryAux > 0) ? subtotalTailAdjustSumaryAux / tailNetSumaryAux : 0;
    percentTailPerformanceSumaryLiq.SetValue((percentTailPerformanceSumaryAux).toFixed(4));
}

function Spinedit_InitLiq(s, e) {
    s.GetMainElement().style.backgroundColor = "blue";
}
function SpineditWholeLeftover_InitLiq(s, e) {
    var aLbsWholeSurplus = parseFloat($('#lbsWholeSurplusLiq').val());
    if (aLbsWholeSurplus > 0) {
        s.SetValue(aLbsWholeSurplus);
        UpdateTotalWholeLeftoverSumaryLiq(s, e);
    }
    Spinedit_InitLiq(s, e);
}

function SpineditTailLeftOver_InitLiq(s, e) {
    var aLbsDirect = parseFloat($('#lbsDirectLiq').val());
    if (aLbsDirect > 0) {
        s.SetValue(aLbsDirect);
        UpdateTotaltailLeftoverSumaryLiq(s, e);
    }
    Spinedit_InitLiq(s, e);
}

function init() {
    $.ajax({
        url: "ProductionLotReception/ProductionLotPerformances",
        type: "post",
        data: { processType: $("#tipoProceso").val(), adjust: true },
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
                //Datos del Entero
                wholeTotalQuantityTrashSumaryLiq.SetValue(0);
                totalAdjustmentWholePoundsSumaryLiq.SetValue(result.totalAdjustmentWholePounds);
                wholeSubtotalAdjustSumaryLiq.SetValue(result.wholeSubtotalAdjust);
                wholeTotalQuantityRecivedSumaryLiq.SetValue(result.wholeTotalQuantityRecivedSumary);
                wholeNetSumaryLiq.SetValue(result.wholeNetSumary);
                percentWholePerformanceSumaryLiq.SetValue(result.percentWholePerformanceSumary);

                //Datos de la Cola
                //tailLeftoverSumaryLiq.SetValue(0);

                totalAdjustmentTailPoundsSumaryLiq.SetValue(result.totalAdjustmentTailPounds);
                subtotalTailAdjustSumaryLiq.SetValue(result.subtotalTailAdjust);
                tailTotalQuantityRecivedSumaryLiq.SetValue(result.tailTotalQuantityRecivedSumary);
                tailNetSumaryLiq.SetValue(result.tailNetSumary);
                percentTailPerformanceSumaryLiq.SetValue(result.percentTailPerformanceSumary);

                //Datos Totales
                totalAdjustmentPoundsSumaryLiq.SetValue(result.totalAdjustmentPounds);
                totalQuantityLiquidationAdjustSumaryLiq.SetValue(result.totalQuantityLiquidationAdjust);
                poundsGarbageTotalLiq.SetValue(result.poundsGarbageTotal);
                totalTotalQuantityRecivedSumaryLiq.SetValue(result.totalTotalQuantityRecivedSumary);
                totalNetSumaryLiq.SetValue(result.totalNetSumary);
                //totalNetSumaryLiq.SetValue(0);
                //Datos SubTotales Entero
                //totalAdjustmentWholePoundsLiq.SetValue(result.totalAdjustmentWholePounds);
                //wholeSubtotalAdjustLiq.SetValue(result.wholeSubtotalAdjust);
                percentPerformanceWholeSubtotalAdjustLiq.SetValue(result.percentPerformanceWholeSubtotalAdjust);

                //Datos SubTotales Cola
                //totalAdjustmentTailPoundsLiq.SetValue(result.totalAdjustmentTailPounds);
                //subtotalTailAdjustLiq.SetValue(result.subtotalTailAdjust);
                percentPerformanceSubtotalTailAdjustLiq.SetValue(result.percentPerformanceSubtotalTailAdjust);

                //Datos SubTotales Total
                //totalAdjustmentPoundsLiq.SetValue(result.totalAdjustmentPounds);
                //totalQuantityLiquidationAdjustLiq.SetValue(result.totalQuantityLiquidationAdjust);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

$(function () {

    init();
});