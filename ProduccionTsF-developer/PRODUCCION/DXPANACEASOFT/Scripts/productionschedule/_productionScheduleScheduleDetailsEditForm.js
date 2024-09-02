var datePlanningIniAux = null;
var id_providerIniAux = null;
var id_buyerIniAux = null;
var id_productionSchedulePurchaseRequestDetailIniAux = 0;
var errorMessage = "";

//Validations

function OnDatePlanningDetailValidation(s, e) {
    //gridMessageErrorProductionScheduleScheduleDetail.SetText(result.Message);
    errorMessage = "";
    $("#GridMessageErrorProductionScheduleScheduleDetail").hide();

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Fecha Planificada: Es obligatoria.";
    } else {
       
        //var id = $("#id_purchasePlanning").val();
        var dateAux = s.GetValue();
        var formattedDatePlanning1 = Intl.DateTimeFormat("es-EC").format(dateAux);
        //function pad(s) { return (s < 10) ? '0' + s : s; }
        //var formattedDatePlanning2 = [pad(dateAux.getDate()), pad(dateAux.getMonth() + 1), dateAux.getFullYear()].join('/');
        var data = "datePlanning=" + formattedDatePlanning1 + "&" + "id_productionSchedulePeriod=" + id_productionSchedulePeriod.GetValue(); //+ "&" + $("#formEditPurchasePlanning").serialize();
        //console.log("formattedDatePlanning1: ");
        //console.log(formattedDatePlanning1);
        //console.log("formattedDatePlanning2: ");
        //console.log(formattedDatePlanning2);
        var url = "ProductionSchedule/ValidateDatePlanning";
       
        $.ajax({
            url: url,
            type: "post",
            data: data,
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
                e.isValid = false;
                e.errorText = "Campo no Válido";
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                //console.log("result: ");
                //console.log(result);
                if (result !== null && result != "") {
                    if (result.code != 0) {
                        e.isValid = false;
                        e.errorText = result.message;
                        errorMessage = "- Fecha Planificada: " + result.message;
                    }
                } else {
                    e.isValid = false;
                    e.errorText = "Campo no Válido";
                    errorMessage = "- Fecha Planificada: Campo no Válido";
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
      
    }
}

//function OnProviderDetailValidation(s, e) {
//    if(e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}
//function OnBuyerDetailValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

function OnProductionSchedulePurchaseRequestDetailComboValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Nombre del Producto: Es obligatorio.";
        } else {
            errorMessage += "</br>- Nombre del Producto: Es obligatorio.";
        }
    } else {
        var data = {
            datePlanningNew: Intl.DateTimeFormat("es-EC").format(datePlanning.GetValue()),
            //id: gvPurchaseRequestEditFormDetail.cpEditingRowKey,
            id_providerNew: id_provider.GetValue(),
            id_buyerNew: id_buyer.GetValue(),
            id_productionSchedulePurchaseRequestDetailNew: s.GetValue()
        };
        if (data.datePlanningNew != datePlanningIniAux || data.id_providerNew != id_providerIniAux ||
            data.id_buyerNew != id_buyerIniAux || data.id_productionSchedulePurchaseRequestDetailNew != id_productionSchedulePurchaseRequestDetailIniAux) {
            $.ajax({
                url: "ProductionSchedule/ItsRepeatedProductionSchedulePurchaseRequestDetail",
                type: "post",
                data: data,
                async: false,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    //showLoading();
                },
                success: function (result) {
                    if (result !== null) {
                        if (result.itsRepeated == 1) {
                            e.isValid = false;
                            e.errorText = result.Error;
                            if (errorMessage == null || errorMessage == "") {
                                errorMessage = "- Nombre del Producto: " + result.Error;
                            } else {
                                errorMessage += "</br>- Nombre del Producto: " + result.Error;
                            }
                        }
                        //else {
                        //    id_itemIniAux = 0
                        //    id_purchaseRequestDetailIniAux = 0
                        //}
                    }
                },
                complete: function () {
                    //hideLoading();
                }
            });
        }

    }
}

//function OnItemTypeCategoryDetailValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

function OnQuantityScheduleRequestValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Planificada: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Planificada: Es obligatoria.";
        }
    } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Planificada: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Cantidad: Máximo 20 caracteres.";
        }
    } else if (s.GetValue() <= 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Planificada: Debe ser mayor a cero.";
        } else {
            errorMessage += "</br>- Cantidad Planificada: Debe ser mayor a cero.";
        }
    }

    if (errorMessage != null && errorMessage != "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorProductionScheduleScheduleDetail.SetText(msgErrorAux);
        $("#GridMessageErrorProductionScheduleScheduleDetail").show();

    }

}

//function OnQuantityRecivedValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    } else if (parseFloat(e.value) <= 0) {
//        e.isValid = false;
//        e.errorText = "Cantidad Incorrecto";
//    }
//}

function ValidateScheduleDetail() {
    OnDatePlanningDetailValidation(datePlanning, datePlanning);
    OnProductionSchedulePurchaseRequestDetailComboValidation(id_productionSchedulePurchaseRequestDetail, id_productionSchedulePurchaseRequestDetail);
    OnQuantityScheduleRequestValidation(quantityScheduleRequest, quantityScheduleRequest);
}

// EDITOR'S EVENTS

function ProductionSchedulePurchaseRequestDetailCombo_OnInit(s, e) {

    //id_itemIniAux = s.GetValue();
    //id_saleRequestDetailIniAux = gvProductionScheduleRequestDetail.cpEditingRowSaleRequestDetail;
    datePlanningIniAux = Intl.DateTimeFormat("es-EC").format(datePlanning.GetValue());
    id_providerIniAux = id_provider.GetValue();
    id_buyerIniAux = id_buyer.GetValue();
    id_productionSchedulePurchaseRequestDetailIniAux = s.GetValue();

    var data = {
        id_productionSchedulePurchaseRequestDetail: s.GetValue(),
        id_provider: id_provider.GetValue(),
        id_buyer: id_buyer.GetValue()
    };

    //if (data.id_itemCurrent != null && id_saleRequestDetailIniAux != null && id_saleRequestDetailIniAux != 0) s.SetEnabled(false);

    $.ajax({
        url: "ProductionSchedule/ProductionScheduleItemPurchaseRequestDetails",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_metricUnit.SetValue(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //id_itemRequest
            var arrayFieldStr = [];
            arrayFieldStr.push("numberRequest");
            arrayFieldStr.push("itemRequest");
            arrayFieldStr.push("codeRequest");
            arrayFieldStr.push("namePurchase");
            arrayFieldStr.push("codePurchase");
            UpdateDetailObjects(id_productionSchedulePurchaseRequestDetail, result.productionSchedulePurchaseRequestDetails, arrayFieldStr);
            
            //id_provider
            arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_provider, result.providers, arrayFieldStr);

            //id_buyer
            arrayFieldStr = [];
            arrayFieldStr.push("fullname_businessName");
            UpdateDetailObjects(id_buyer, result.buyers, arrayFieldStr);

            salesPurchaseRequest.SetText(result.salesPurchaseRequest);
            outstandingAmountRequired.SetValue(result.outstandingAmountRequired);
            metricUnitPurchaseRequest.SetText(result.metricUnitPurchaseRequest);
            metricUnitScheduleRequest.SetText(result.metricUnitScheduleRequest);
            
        },
        complete: function () {
            //hideLoading();
        }
    });

}

function UpdateProductionSchedulePurchaseRequestDetailInfo(data, s, e) {

    if (data.id_productionSchedulePurchaseRequestDetail === null) {
        salesPurchaseRequest.SetText("");
        outstandingAmountRequired.SetValue(0);
        metricUnitPurchaseRequest.SetText("");
        metricUnitScheduleRequest.SetText("");
        ValidateScheduleDetail();
        return;
    }

    $.ajax({
        url: "ProductionSchedule/ProductionScheduleItemRequestDetailData",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_metricUnit.SetValue(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {

            salesPurchaseRequest.SetText(result.salesPurchaseRequest);
            outstandingAmountRequired.SetValue(result.outstandingAmountRequired);
            metricUnitPurchaseRequest.SetText(result.metricUnitPurchaseRequest);
            metricUnitScheduleRequest.SetText(result.metricUnitScheduleRequest);

        },
        complete: function () {
            //hideLoading();
            ValidateScheduleDetail();
        }
    });
}

function ProductionSchedulePurchaseRequestDetailCombo_SelectedIndexChanged(s, e) {
    UpdateProductionSchedulePurchaseRequestDetailInfo({
        id_productionSchedulePurchaseRequestDetail: s.GetValue()
    }, s, e);
}

function DatePlanningDetail_ValueChanged(s, e) {
    ValidateScheduleDetail();
}

function DatePlanningCustomDisabledDates(s, e) {
    //if (e.date.getDay() == 3)
    //    e.isDisabled = true;
    var dateStarProductionSchedulePeriod = $("#dateStarProductionSchedulePeriod").val();
    var dateEndProductionSchedulePeriod = $("#dateEndProductionSchedulePeriod").val();

    if (dateStarProductionSchedulePeriod === null || dateStarProductionSchedulePeriod == "" ||
        dateEndProductionSchedulePeriod === null || dateEndProductionSchedulePeriod === "") {
        e.isDisabled = true;
    } else {
        dateStarProductionSchedulePeriod = dateStarProductionSchedulePeriod.split("/");
        var dateStarProductionSchedulePeriodDate = new Date(dateStarProductionSchedulePeriod[2], parseInt(dateStarProductionSchedulePeriod[1]) - 1, dateStarProductionSchedulePeriod[0]);

        dateEndProductionSchedulePeriod = dateEndProductionSchedulePeriod.split("/");
        var dateEndProductionSchedulePeriodDate = new Date(dateEndProductionSchedulePeriod[2], parseInt(dateEndProductionSchedulePeriod[1]) - 1, dateEndProductionSchedulePeriod[0]);

        //console.log("dateStarProductionSchedulePeriodDate: " + dateStarProductionSchedulePeriodDate);
        //console.log("dateEndProductionSchedulePeriodDate: " + dateEndProductionSchedulePeriodDate);

        if (dateStarProductionSchedulePeriodDate === null ||
            dateEndProductionSchedulePeriodDate === null) {
            e.isDisabled = true;
        } else {
            if (dateStarProductionSchedulePeriodDate <= e.date &&
            dateEndProductionSchedulePeriodDate >= e.date)
                e.isDisabled = false;
            else
                e.isDisabled = true;
        }
    }
    
    

}

// EDITOR'S EVENTS
var customCommand = "";

function OnGridViewInitScheduleDetail(s, e) {
    ScheduleDetailsUpdateTitlePanel();
}

function OnGridViewBeginCallbackScheduleDetail(s, e) {
    customCommand = e.command;
    ScheduleDetailsUpdateTitlePanel();
}

function OnGridViewEndCallbackScheduleDetail(s, e) {
    ScheduleDetailsUpdateTitlePanel();
    //if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
    //    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
    //}
    //UpdateOrderTotals();
    //gvProductionScheduleInventoryReservationDetail.PerformCallback();
    //gvProductionScheduleProductionOrderDetail.PerformCallback();
    //gvProductionSchedulePurchaseRequestDetail.PerformCallback();
    //gvProductionScheduleScheduleDetail.PerformCallback();
}

function OnGridViewSelectionChangedScheduleDetail(s, e) {
    ScheduleDetailsUpdateTitlePanel();

}

function ScheduleDetailsUpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCountScheduleDetail();

    var text = "Total de elementos seleccionados: <b>" + gvProductionScheduleScheduleDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionScheduleScheduleDetail.GetSelectedRowCount() - GetSelectedFilteredRowCountScheduleDetail();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfoScheduleDetail").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRowsScheduleDetail", gvProductionScheduleScheduleDetail.GetSelectedRowCount() > 0 && gvProductionScheduleScheduleDetail.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelectionScheduleDetail", gvProductionScheduleScheduleDetail.GetSelectedRowCount() > 0);
    //}

    btnRemoveProductionScheduleScheduleDetail.SetEnabled(false);
    //btnRemoveDetail.SetEnabled(gvProductionScheduleScheduleDetail.GetSelectedRowCount() > 0);
    btnNewProductionScheduleScheduleDetail.SetEnabled(gvProductionSchedulePurchaseRequestDetail.cpRowsCount > 0);
}

function GetSelectedFilteredRowCountScheduleDetail() {
    return gvProductionScheduleScheduleDetail.cpFilteredRowCountWithoutPage + gvProductionScheduleScheduleDetail.GetSelectedKeysOnPage().length;
}

function EditSelectAllRowsScheduleDetail() {
    gvProductionScheduleScheduleDetail.SelectRows();
}

function EditClearSelectionScheduleDetail() {
    gvProductionScheduleScheduleDetail.UnselectRows();
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}
