
// VALIDATIONS

//function OnPurchasePlanningPeriodValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    } else {
//        var url = "PurchasePlanning/ValidatePurchasePlanningPeriod";
//        $.ajax({
//            url: url,
//            type: "post",
//            data: { id_purchasePlanningPeriod: e.value },
//            async: false,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//                e.isValid = false;
//                e.errorText = "Campo no Válido";
//            },
//            beforeSend: function () {
//                //showLoading();
//            },
//            success: function (result) {
//                //console.log("result: ");
//                //console.log(result);
//                if (result !== null && result != "") {
//                    if (result.code != 0) {
//                        e.isValid = false;
//                        e.errorText = result.message;
//                    }
//                } else {
//                    e.isValid = false;
//                    e.errorText = "Campo no Válido";
//                }
//            },
//            complete: function () {
//                //hideLoading();
//            }
//        });

//    }
//}

function OnResponsableValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }

    UpdateTabImage(e, "tabTumbada");
}