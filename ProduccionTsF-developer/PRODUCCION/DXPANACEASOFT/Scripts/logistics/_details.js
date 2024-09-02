// REMISSION GUIDE DETAILS FUNCTIONS
var id_itemIniAux = 0;
var id_purchaseOrderDetailIniAux = 0;
var errorMessage = "";
var runningValidation = false;

function ItemCombo_OnInit(s, e) {

    id_itemIniAux = s.GetValue();
    id_purchaseOrderDetailIniAux = gvDetails.cpEditingRowPurchaseOrderDetail;

    var data = {
        id_itemCurrent: s.GetValue(),
        InventoryLine:"MP"
    };

    if (data.id_itemCurrent != null && id_purchaseOrderDetailIniAux != null && id_purchaseOrderDetailIniAux != 0) s.SetEnabled(false);

    //console.log("data.id_itemCurrent: " + data.id_itemCurrent);
    //console.log("id_purchaseOrderDetailIniAux: " + id_purchaseOrderDetailIniAux);
    //console.log("data.id_itemCurrent != null: " + (data.id_itemCurrent != null));
    //console.log("id_purchaseOrderDetailIniAux != null: " + (id_purchaseOrderDetailIniAux != null));
    //console.log("id_purchaseOrderDetailIniAux != 0: " + (id_purchaseOrderDetailIniAux != 0));

    $.ajax({
        url: "Logistics/PurchaseOrderDetails",
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
            //id_company
            var arrayFieldStr = [];
            arrayFieldStr.push("masterCode");
            arrayFieldStr.push("name");
            arrayFieldStr.push("itemPurchaseInformationMetricUnitCode");
            UpdateDetailObjects(id_itemDetail, result.items, arrayFieldStr);
           
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function DetailsItemsCombo_SelectedIndexChanged(s, e) {
    DetailsUpdateItemInfo({
        id_item: s.GetValue()
    });
}

function DetailsUpdateItemInfo(data) {
    if (data.id_item === null) {
        ValidateDetail();
        return;
    }

    masterCodeItemDetail.SetText("");
    metricUnitItemDetail.SetText("");
    var exist_Id_businessOportunityPlanningDetail = true;
    try {
        id_businessOportunityPlanningDetail.ClearItems();
    }
    catch (e) {
        exist_Id_businessOportunityPlanningDetail = false;
    }


    if (data.id_item != null) {

        $.ajax({
            url: "Logistics/ItemDetailData",
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
                    masterCodeItemDetail.SetText(result.ItemDetailData.masterCode);
                    metricUnitItemDetail.SetText(result.ItemDetailData.metricUnit);
                    if (exist_Id_businessOportunityPlanningDetail) UpdateBusinessOportunityPlanningDetails(result.ItemDetailData.businessOportunityPlanningDetails);
                }
            },
            complete: function () {
                //hideLoading();
                ValidateDetail();
            }
        });
    } else {
        ValidateDetail();
    }
}


function UpdateBusinessOportunityPlanningDetails(businessOportunityPlanningDetails) {
    for (var i = 0; i < id_businessOportunityPlanningDetail.GetItemCount() ; i++) {
        var businessOportunityPlanningDetail = id_businessOportunityPlanningDetail.GetItem(i);
        var into = false;
        for (var j = 0; j < businessOportunityPlanningDetails.length; j++) {
            if (businessOportunityPlanningDetail.value == businessOportunityPlanningDetails[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_businessOportunityPlanningDetail.RemoveItem(i);
            i -= 1;
        }
    }

    for (var i = 0; i < businessOportunityPlanningDetails.length; i++) {
        var businessOportunityPlanningDetail = id_businessOportunityPlanningDetail.FindItemByValue(businessOportunityPlanningDetails[i].id);
        if (businessOportunityPlanningDetail == null) id_businessOportunityPlanningDetail.AddItem(businessOportunityPlanningDetails[i].name, businessOportunityPlanningDetails[i].id);

    }
}

function DetailsBusinessOportunityPlanningDetailCombo_Init(s, e) {

    var id_itemSelected = id_itemDetail.GetValue();
    var data = {
        id_item: id_itemSelected
    };

    $.ajax({
        url: "Logistics/BusinessOportunityPlanningDetailData",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            id_businessOportunityPlanningDetail.ClearItems();
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                //metricUnit.SetText(result.metricUnit);

                UpdateBusinessOportunityPlanningDetails(result.businessOportunityPlanningDetails);
                //s.SetValue(id_itemSelected);
                //ComboItem_SelectedIndexChanged(s, e);
                //ComboWarehouse_Init(id_warehouse, e);
            }
            else {
                id_businessOportunityPlanningDetail.ClearItems();
                //metricUnit.SetText("");
                //currentStock.SetValue(0);
                //currentStockAux = 0;
                //id_warehouse.ClearItems();
                //id_warehouseLocation.ClearItems();
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function OnQuantityProgrammed_NumberChanged(s, e) {
    quantityDispatchPending.SetValue(s.GetValue());
    //ValidateDetail();
}

// REMISSION GUIDE DETAILS VALIDATIONS

function OnItemValidation(s, e) {
    //gridMessageErrorPurchaseOrderDetail.SetText(result.Message);
    errorMessage = "";
    $("#GridMessageErrorPurchaseOrderDetail").hide();

    //try {
    //    var valueItem = s.GetValue();
    //    console.log("valueItem: " + valueItem);
    //}
    //catch (e) {
    //    return;
    //}

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Nombre del Producto: Es obligatorio.";
    } else {
        var data = {
            //id: gvPurchaseRequestEditFormDetail.cpEditingRowKey,
            id_itemNew: s.GetValue(),
            id_purchaseOrderDetail: id_purchaseOrderDetailIniAux,
        };
        if (data.id_itemNew != id_itemIniAux) {
            $.ajax({
                url: "Logistics/ItsRepeatedItemDetail",
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
                            errorMessage = "- Nombre del Producto: " + result.Error;
                        } else {
                            id_itemIniAux = 0
                            id_purchaseOrderDetailIniAux = 0
                        }
                    }
                },
                complete: function () {
                    //hideLoading();
                }
            });
        }

    }
}

function OnGrammageValidation(s, e) {
    $("#GridMessageErrorPurchaseOrderDetail").hide();
    //gridMessageErrorPurchaseOrder.SetText(result.Message);
    errorMessage = "";
    //$("#GridMessageErrorPurchaseOrder").hide();
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Gramage: Es obligatorio.";
    }
}
function OnQuantityProgrammedValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Programada: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Programada: Es obligatoria.";
        }
    } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Programada: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Cantidad Programada: Máximo 20 caracteres.";
        }
    } else if (s.GetValue() <= 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Programada: Debe ser mayor a cero.";
        } else {
            errorMessage += "</br>- Cantidad Programada: Debe ser mayor a cero.";
        }
    }

    if (errorMessage != null && errorMessage != "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorPurchaseOrderDetail.SetText(msgErrorAux);
        $("#GridMessageErrorPurchaseOrderDetail").show();

    }

    if (!runningValidation) {
        ValidateDetail();
    }
}

function ValidateDetail() {
    runningValidation = true;
    OnItemValidation(id_itemDetail, id_itemDetail);
    OnQuantityProgrammedValidation(quantityProgrammed, quantityProgrammed);
    runningValidation = false;

}

$(function () {
});
