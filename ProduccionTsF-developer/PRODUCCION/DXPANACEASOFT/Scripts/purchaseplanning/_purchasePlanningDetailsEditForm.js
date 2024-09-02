
//Validations

function OnDatePlanningDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
       
        var id = $("#id_purchasePlanning").val();
        var dateAux = e.value;
        var formattedDatePlanning1 = Intl.DateTimeFormat("es-EC").format(dateAux);
        //function pad(s) { return (s < 10) ? '0' + s : s; }
        //var formattedDatePlanning2 = [pad(dateAux.getDate()), pad(dateAux.getMonth() + 1), dateAux.getFullYear()].join('/');
        var data = "datePlanning=" + formattedDatePlanning1 + "&" + "id=" + id + "&" + $("#formEditPurchasePlanning").serialize();
        //console.log("formattedDatePlanning1: ");
        //console.log(formattedDatePlanning1);
        //console.log("formattedDatePlanning2: ");
        //console.log(formattedDatePlanning2);
        var url = "PurchasePlanning/ValidateDatePlanning";
       
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
                    }
                } else {
                    e.isValid = false;
                    e.errorText = "Campo no Válido";
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
      
    }
}

function OnProviderDetailValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnBuyerDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnItemTypeCategoryDetailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnQuantityValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if(parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
    }
}

function OnQuantityRecivedValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecto";
    }
}

// EDITOR'S EVENTS

function UpdateDetailInfo(data, s, e) {

    if (data.id_item === null) {
        return;
    }

    //purchaseOrderNumber.SetText("");
    //remissionGuideNumber.SetText("");
    gvPurchasePlanningEditFormItemsDetail.GetEditor("metricUnit").SetText("");
    gvPurchasePlanningEditFormItemsDetail.GetEditor("id_itemTypeCategory").SetValue(null);// 
    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(null);// SetValue("");

    if (id_item != null) {

        $.ajax({
            url: "PurchasePlanning/ItemDetailData",
            type: "post",
            data: data,
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
                    gvPurchasePlanningEditFormItemsDetail.GetEditor("metricUnit").SetText("Lbs");
                    gvPurchasePlanningEditFormItemsDetail.GetEditor("id_itemTypeCategory").SetValue(result.id_itemTypeCategory);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function ItemDetailCombo_SelectedIndexChanged(s, e) {
    UpdateDetailInfo({
        id_item: s.GetValue()
    }, s, e);
}

function ItemDetailCombo_DropDown(s, e) {

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionItemDetails",
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
            for (var i = 0; i < id_item.GetItemCount() ; i++) {
                var item = id_item.GetItem(i);
                if (result.indexOf(item.value) >= 0) {
                    id_item.RemoveItem(i);
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

function QuantityRemitted_NumberChange(s, e) {
    //UpdateProductionLotTotals();
}

function QuantityRecived_NumberChange(s, e) {
    //UpdateProductionLotTotals();
}
