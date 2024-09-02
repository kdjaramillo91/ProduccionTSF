
// VALIDATIONS

//function OnProviderValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}


function init() {

    $.ajax({
        url: "ProductionLotProcess/ProductionLotPerformances",
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
                totalQuantityRecivedNet.SetValue(result.totalQuantityRecivedNet);
                //totalQuantityLiquidationAdjust.SetValue(result.totalQuantityLiquidationAdjust);
                performance.SetValue(result.performance);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });

    //var totalQuantityRecivedNetAux = totalQuantityRecived.GetValue();
    //totalQuantityRecivedNetAux -= totalQuantityTrash.GetValue();
    //totalQuantityRecivedNet.SetValue(totalQuantityRecivedNetAux);

    //totalQuantityRemitted.SetValue(result.totalQuantityRemitted);
}

$(function () {

    init();
});