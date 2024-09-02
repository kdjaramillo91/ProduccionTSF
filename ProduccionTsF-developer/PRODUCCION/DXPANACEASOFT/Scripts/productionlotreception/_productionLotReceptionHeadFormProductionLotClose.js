
// VALIDATIONS

function OnTailQuantityRecivedValidation(s, e) {
    var valueAux = subtotalTailAdjustSumary.GetValue() + tailTotalQuantityTrashSumary.GetValue() + poundsGarbageTail.GetValue();
    valueAux = (valueAux).toFixed(2);
    var tailTotalQuantityRecivedSumaryAux = (tailTotalQuantityRecivedSumary.GetValue()).toFixed(2);
    if (valueAux != tailTotalQuantityRecivedSumaryAux) {
        e.isValid = false;
        e.errorText = "La suma de Lbs Cola Ajustadas mas Desperdicio Cola mas Basura Cola deben ser igual a Lbs Cola Recibidas";
    }
}

function OnWholeGarbagePoundsValueChanged(s, e) {
    // 
    var wholeGarbagePoundsAux = wholeGarbagePounds.GetValue();
    var poundsGarbageTailAux = poundsGarbageTail.GetValue();
    var wholeSubtotalAdjustSumaryAux = wholeSubtotalAdjustSumary.GetValue();
    var wholeLeftoverAux = wholeLeftover.GetValue();
    var wholeTotalQuantityRecivedSumaryAux = wholeSubtotalAdjustSumaryAux + wholeGarbagePoundsAux + wholeLeftoverAux;
    wholeTotalQuantityRecivedSumaryAux = wholeTotalQuantityRecivedSumaryAux.toFixed(2);
    wholeTotalQuantityRecivedSumary.SetValue(wholeTotalQuantityRecivedSumaryAux);
    totalTotalQuantityRecivedSumary.SetValue(wholeTotalQuantityRecivedSumaryAux + tailTotalQuantityRecivedSumary.GetValue());
    var wholeNetSumaryAux = wholeTotalQuantityRecivedSumaryAux - wholeGarbagePoundsAux;
        //- poundsGarbageTailAux;
    if (wholeGarbagePoundsAux >= 0) {
        wholeNetSumary.SetValue(wholeNetSumaryAux);
        var percentWholePerformanceSumaryAux = wholeNetSumaryAux > 0 ? wholeSubtotalAdjustSumaryAux / wholeNetSumaryAux : 0;
        percentWholePerformanceSumary.SetValue((percentWholePerformanceSumaryAux).toFixed(4));
    } else {
        wholeNetSumary.SetValue(0);
        percentWholePerformanceSumary.SetValue(0);
    }
    UpdateTotalPoundsGarbageSumary(s, e);
}

function OnPoundsGarbageTailValueChanged(s, e) {
    // 
    var _tipoProceso = $("#tipoProceso").val();
    if (_tipoProceso == "ENT") {
        var wholeGarbagePoundsAux = wholeGarbagePounds.GetValue();
        var wholeSubtotalAdjustSumaryAux = wholeSubtotalAdjustSumary.GetValue();
        var wholeLeftoverAux = wholeLeftover.GetValue();
        var wholeTotalQuantityRecivedSumaryAux = wholeSubtotalAdjustSumaryAux + wholeGarbagePoundsAux + wholeLeftoverAux;
        wholeTotalQuantityRecivedSumaryAux = wholeTotalQuantityRecivedSumaryAux.toFixed(2);
        var poundsGarbageTailAux = poundsGarbageTail.GetValue();
        var wholeGarbagePoundsAux = wholeGarbagePounds.GetValue();
        var wholeSubtotalAdjustSumaryAux = wholeSubtotalAdjustSumary.GetValue();
        var wholeNetSumaryAux = wholeNetSumary.GetValue();
        var wholeNetSumaryAux = wholeTotalQuantityRecivedSumaryAux - wholeGarbagePoundsAux;

        var tailNetSumaryAux = tailTotalQuantityRecivedSumary.GetValue() - poundsGarbageTailAux;
        var tailNetSumaryAux2 = wholeLeftoverAux - poundsGarbageTailAux;
        if (poundsGarbageTailAux >= 0) {
            var wholeNetSumaryAux2 = wholeNetSumaryAux - poundsGarbageTailAux;
            tailNetSumary.SetValue(tailNetSumaryAux2);
            //wholeNetSumary.SetValue(wholeNetSumaryAux2);
            var percentTailPerformanceSumaryAux = tailNetSumaryAux2 > 0 ? subtotalTailAdjustSumary.GetValue() / tailNetSumaryAux2 : 0;
            percentTailPerformanceSumary.SetValue((percentTailPerformanceSumaryAux).toFixed(4));
        } else {
            //wholeNetSumary.SetValue(wholeNetSumaryAux);
            tailNetSumary.SetValue(0);
            percentTailPerformanceSumary.SetValue(0);
        }
        UpdateTotalPoundsGarbageSumary(s, e);
		
    }
    else {
        var tailLeftOverAux = tailLeftOver.GetValue();
        var poundsGarbageTailAux = poundsGarbageTail.GetValue();

        tailLeftOverAux = (tailLeftOverAux >= 0) ? tailLeftOverAux : 0;
        poundsGarbageTailAux = (poundsGarbageTailAux >= 0) ? poundsGarbageTailAux : 0;
        tailTotalQuantityRecivedSumary.SetValue(tailLeftOverAux);
        tailNetSumary.SetValue(tailLeftOverAux - poundsGarbageTailAux);
        var subtotalTailAdjustSumaryAux = subtotalTailAdjustSumary.GetValue()
        var tailNetSumaryAux = tailNetSumary.GetValue();
        var percentTailPerformanceSumaryAux = (tailNetSumaryAux > 0) ? subtotalTailAdjustSumaryAux / tailNetSumaryAux : 0;
        percentTailPerformanceSumary.SetValue((percentTailPerformanceSumaryAux).toFixed(4));
    }
	var WholeNetSumaryAux3 = wholeNetSumary.GetValue();
	var tailNetSumaryAux3 = tailNetSumary.GetValue();
	totalNetSumary.SetValue(WholeNetSumaryAux3 + tailNetSumaryAux3);
}

function UpdateTotalPoundsGarbageSumary(s, e) {
    poundsGarbageTotal.SetValue(wholeGarbagePounds.GetValue() + poundsGarbageTail.GetValue());
    totalNetSumary.SetValue(0);
}

function UpdateTotalWholeLeftoverSumaryAux() {
	// 
    var wholeLeftoverAux = wholeLeftover.GetValue();
    totalWholeLeftoverSumary.SetValue(wholeLeftoverAux);
        //+ tailLeftoverSumary.GetValue());
    var tailTotalQuantityRecivedSumaryAux = tailTotalQuantityRecivedSumary.GetValue();
    totalTotalQuantityRecivedSumary.SetValue(tailTotalQuantityRecivedSumaryAux + wholeTotalQuantityRecivedSumary.GetValue());
    var poundsGarbageTailAux = poundsGarbageTail.GetValue();
    var tailNetSumaryAux = tailTotalQuantityRecivedSumaryAux - poundsGarbageTailAux;
    var tailNetSumaryAux2 = wholeLeftoverAux - poundsGarbageTailAux;
    if (poundsGarbageTailAux >= 0) {
        tailNetSumary.SetValue(tailNetSumaryAux2);
        var percentTailPerformanceSumaryAux = tailNetSumaryAux2 > 0 ? subtotalTailAdjustSumary.GetValue() / tailNetSumaryAux2 : 0;
        percentTailPerformanceSumary.SetValue((percentTailPerformanceSumaryAux).toFixed(4));
    } else {
        tailNetSumary.SetValue(0);
        percentTailPerformanceSumary.SetValue(0);
    }
    var WholeNetSumaryAux = wholeNetSumary.GetValue();
    totalNetSumary.SetValue(tailNetSumary.GetValue() + WholeNetSumaryAux);
}

function UpdateTotalWholeLeftoverSumary(s, e) {
    var wholeLeftoverAux = wholeLeftover.GetValue();
    var wholeGarbagePoundsAux = wholeGarbagePounds.GetValue();
    tailTotalQuantityRecivedSumary.SetValue(wholeLeftoverAux);
    //tailTotalQuantityRecivedSumary.SetValue(0);

    var wholeSubtotalAdjustSumaryAux = wholeSubtotalAdjustSumary.GetValue();
    var wholeTotalQuantityRecivedSumaryAux = wholeSubtotalAdjustSumaryAux + wholeGarbagePoundsAux + wholeLeftoverAux;
    wholeTotalQuantityRecivedSumaryAux = wholeTotalQuantityRecivedSumaryAux.toFixed(2);
    wholeTotalQuantityRecivedSumary.SetValue(wholeTotalQuantityRecivedSumaryAux);

    UpdateTotalWholeLeftoverSumaryAux();
}

function UpdateTotaltailLeftoverSumary(s, e) {
    var tailLeftOverAux = tailLeftOver.GetValue();
    var poundsGarbageTailAux = poundsGarbageTail.GetValue();

    tailLeftOverAux = (tailLeftOverAux >= 0) ? tailLeftOverAux : 0;
    poundsGarbageTailAux = (poundsGarbageTailAux >= 0) ? poundsGarbageTailAux : 0;
    tailTotalQuantityRecivedSumary.SetValue(tailLeftOverAux);
    tailNetSumary.SetValue(tailLeftOverAux - poundsGarbageTailAux);

    var subtotalTailAdjustSumaryAux = subtotalTailAdjustSumary.GetValue()
    var tailNetSumaryAux = tailNetSumary.GetValue();
    var percentTailPerformanceSumaryAux = (tailNetSumaryAux > 0) ? subtotalTailAdjustSumaryAux / tailNetSumaryAux : 0;
    percentTailPerformanceSumary.SetValue((percentTailPerformanceSumaryAux).toFixed(4));
}

function Spinedit_Init(s, e) {
    s.GetMainElement().style.backgroundColor = "blue";
}
function SpineditWholeLeftover_Init(s, e) {
    var aLbsWholeSurplus = parseFloat($('#lbsWholeSurplus').val());
    if (aLbsWholeSurplus > 0) {
        s.SetValue(aLbsWholeSurplus);
        UpdateTotalWholeLeftoverSumary(s, e);
    }
    Spinedit_Init(s, e);
}

function SpineditTailLeftOver_Init(s, e) {
    var aLbsDirect = parseFloat($('#lbsDirect').val())
    if (aLbsDirect > 0) {
        s.SetValue(aLbsDirect);
        UpdateTotaltailLeftoverSumary(s, e);
    }
    Spinedit_Init(s, e);
}

function init() {
    $.ajax({
        url: "ProductionLotReception/ProductionLotPerformances",
        type: "post",
        data: { processType : $("#tipoProceso").val() },
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
                wholeTotalQuantityTrashSumary.SetValue(0);
                totalAdjustmentWholePoundsSumary.SetValue(result.totalAdjustmentWholePounds);
                wholeSubtotalAdjustSumary.SetValue(result.wholeSubtotalAdjust);
                wholeTotalQuantityRecivedSumary.SetValue(result.wholeTotalQuantityRecivedSumary);
                wholeNetSumary.SetValue(result.wholeNetSumary);
                percentWholePerformanceSumary.SetValue(result.percentWholePerformanceSumary);

                //Datos de la Cola
                //tailLeftoverSumary.SetValue(0);

                totalAdjustmentTailPoundsSumary.SetValue(result.totalAdjustmentTailPounds);
                subtotalTailAdjustSumary.SetValue(result.subtotalTailAdjust);
                tailTotalQuantityRecivedSumary.SetValue(result.tailTotalQuantityRecivedSumary);
                tailNetSumary.SetValue(result.tailNetSumary);
                percentTailPerformanceSumary.SetValue(result.percentTailPerformanceSumary);

                //Datos Totales
                totalAdjustmentPoundsSumary.SetValue(result.totalAdjustmentPounds);
                totalQuantityLiquidationAdjustSumary.SetValue(result.totalQuantityLiquidationAdjust);
                poundsGarbageTotal.SetValue(result.poundsGarbageTotal);
                totalTotalQuantityRecivedSumary.SetValue(result.totalTotalQuantityRecivedSumary);
                totalNetSumary.SetValue(result.totalNetSumary);
                //totalNetSumary.SetValue(0);
                //Datos SubTotales Entero
                //totalAdjustmentWholePounds.SetValue(result.totalAdjustmentWholePounds);
                //wholeSubtotalAdjust.SetValue(result.wholeSubtotalAdjust);
                percentPerformanceWholeSubtotalAdjust.SetValue(result.percentPerformanceWholeSubtotalAdjust);

                //Datos SubTotales Cola
                //totalAdjustmentTailPounds.SetValue(result.totalAdjustmentTailPounds);
                //subtotalTailAdjust.SetValue(result.subtotalTailAdjust);
                percentPerformanceSubtotalTailAdjust.SetValue(result.percentPerformanceSubtotalTailAdjust);

                //Datos SubTotales Total
                //totalAdjustmentPounds.SetValue(result.totalAdjustmentPounds);
                //totalQuantityLiquidationAdjust.SetValue(result.totalQuantityLiquidationAdjust);
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