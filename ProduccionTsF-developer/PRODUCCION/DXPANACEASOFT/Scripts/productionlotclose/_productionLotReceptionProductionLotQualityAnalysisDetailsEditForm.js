
//Validations

function OnQualityAnalysisProductionLotQualityAnalysisDetailValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnResultProductionLotQualityAnalysisDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

//function OnWarehouseProductionLotQualityAnalysisDetailValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnWarehouseLocationProductionLotQualityAnalysisDetailValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnQuantityProductionLotQualityAnalysisDetailValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    } else if(parseFloat(e.value) <= 0) {
//        e.isValid = false;
//        e.errorText = "Cantidad Incorrecto";
//    }
//}


// EDITOR'S EVENTS

//function UpdateProductionLotQualityAnalysisDetailInfo(data, s, e) {

//    if (data.id_qualityAnalysis === null) {
//        return;
//    }

//    //purchaseOrderNumber.SetText("");
//    //remissionGuideNumber.SetText("");
//    gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.GetEditor("result").SetText("");
//    //gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.GetEditor("id_warehouse").SetValue(null);//ClearSelection();// SetValue("");
//    //gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.GetEditor("id_warehouseLocation").SetValue(null);//.ClearSelection();// SetValue("");

//    if (id_qualityAnalysis != null) {

//        $.ajax({
//            url: "ProductionLotReception/QualityAnalysisDetailData",
//            type: "post",
//            data: data,
//            async: true,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//            },
//            beforeSend: function () {
//                //showLoading();
//            },
//            success: function (result) {
//                if (result !== null) {
//                    gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.GetEditor("metricUnit").SetText(result.metricUnit);
//                    gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);
//                    gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
//                }
//            },
//            complete: function () {
//                //hideLoading();
//            }
//        });
//    }
//}

//function QualityAnalysisProductionLotQualityAnalysisDetailCombo_SelectedIndexChanged(s, e) {
//    UpdateProductionLotQualityAnalysisDetailInfo({
//        id_qualityAnalysis: s.GetValue()
//    }, s, e);
//}

function QualityAnalysisProductionLotQualityAnalysisDetailCombo_DropDown(s, e) {

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionQualityAnalysisDetails",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            for (var i = 0; i < id_qualityAnalysis.GetItemCount() ; i++) {
                var qualityAnalysis = id_qualityAnalysis.GetItem(i);
                if (result.indexOf(qualityAnalysis.value) >= 0) {
                    id_qualityAnalysis.RemoveItem(i);
                    i = -1;
                }
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// EDITOR'S EVENTS



//function Quantity_NumberChange(s, e) {
//    //UpdateProductionLotTotals();
//}


