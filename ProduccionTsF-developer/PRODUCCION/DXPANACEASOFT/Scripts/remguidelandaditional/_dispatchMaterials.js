// REMISSION GUIDE DETAILS FUNCTIONS
//var id_itemIniAux = 0;
//var id_purchaseOrderDetailIniAux = 0;
var errorMessage = "";
var runningValidation = false;

function ItemDispatchMaterialCombo_OnInit(s, e) {
    var tab = tabControl.GetTabByName("tabDispatchMaterials");
    if (!tab.GetVisible()) {
        return;
    }
    var data = {
        id_itemCurrent: s.GetValue()
    };

    if (data.id_itemCurrent != null) s.SetEnabled(false);

    $.ajax({
        url: "RemGuideLandAditional/DispatchMaterialDetails",
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
            //id_company
            var arrayFieldStr = [];
            arrayFieldStr.push("masterCode");
            arrayFieldStr.push("name");
            arrayFieldStr.push("ItemInventoryMetricUnitCode");
            UpdateDetailObjects(id_item, result.items, arrayFieldStr);
           
        },
        complete: function () {
        }
    });
}

function DetailsItemDispatchMaterialsCombo_SelectedIndexChanged(s, e) {
    DetailsUpdateItemDispatchMaterialsInfo({
        id_item: s.GetValue()
    });
}

function DetailsUpdateItemDispatchMaterialsInfo(data) {
    if (data.id_item === null) {
        ValidateDetailDispatchMaterial();
        return;
    }
    var tab = tabControl.GetTabByName("tabDispatchMaterials");
    if (!tab.GetVisible()) {
        return;
    }
    masterCode.SetText("");
    metricUnit.SetText("");

    if (id_item != null) {

        $.ajax({
            url: "RemGuideLandAditional/ItemDispatchMaterialDetailsData",
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
                    masterCode.SetText(result.ItemDetailData.masterCode);
                    metricUnit.SetText(result.ItemDetailData.metricUnit);
                }
            },
            complete: function () {
                //hideLoading();
                ValidateDetailDispatchMaterial();
            }
        });
    } else {
        ValidateDetailDispatchMaterial();
    }
}

function OnItemDispatchMaterialValidation(s, e) {
    //errorMessage = "";
    //var tab = tabControl.GetTabByName("tabDispatchMaterials");
    //$("#GridMessageErrorDispatchMaterialDetail").hide();
    //if (!tab.GetVisible()) {
    //    e.isValid = true;
    //    return;
    //} else {
    //    if (s.GetValue() === null) {
    //        e.isValid = false;
    //        e.errorText = "Campo Obligatorio";
    //        errorMessage = "- Nombre del Producto: Es obligatorio.";
    //    }
    //}
}

function OnSourceExitQuantityValidation(s, e) {
    //if (s.GetValue() === null) {
    //    e.isValid = false;
    //    e.errorText = "Campo obligatorio";
    //    if (errorMessage == null || errorMessage == "") {
    //        errorMessage = "- Cantidad Salida Origen: Es obligatoria.";
    //    } else {
    //        errorMessage += "</br>- Cantidad Salida Origen: Es obligatoria.";
    //    }
    //} else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
    //    e.isValid = false;
    //    e.errorText = "Máximo 20 caracteres";
    //    if (errorMessage == null || errorMessage == "") {
    //        errorMessage = "- Cantidad Salida Origen: Máximo 20 caracteres.";
    //    } else {
    //        errorMessage += "</br>- Cantidad Salida Origen: Máximo 20 caracteres.";
    //    }
    //} else if (s.GetValue() < 0) {
    //    e.isValid = false;
    //    e.errorText = "Valor Incorrecto";
    //    if (errorMessage == null || errorMessage == "") {
    //        errorMessage = "- Cantidad Salida Origen: Debe ser mayor a cero.";
    //    } else {
    //        errorMessage += "</br>- Cantidad Salida Origen: Debe ser mayor a cero.";
    //    }
    //}
    ////else {
    ////    var quantityRequiredForPurchaseAux = gvDispatchMaterials.cpEditingRowQuantityRequiredForPurchase;
    ////    if (s.GetValue() < quantityRequiredForPurchaseAux) {
    ////        e.isValid = false;
    ////        e.errorText = "Valor Incorrecto";
    ////        if (errorMessage == null || errorMessage == "") {
    ////            errorMessage = "- Cantidad Salida Origen: Debe ser mayor e igual a " + quantityRequiredForPurchaseAux + " que es la cantidad mínima que requiere la compra.";
    ////        } else {
    ////            errorMessage += "</br>- Cantidad Salida Origen: Debe ser mayor e igual a " + quantityRequiredForPurchaseAux + " que es la cantidad mínima que requiere la compra.";
    ////        }
    ////    }
    ////}
    //if (errorMessage != null && errorMessage != "") {
    //    var msgErrorAux = ErrorMessage(errorMessage);
    //    gridMessageErrorDispatchMaterialDetail.SetText(msgErrorAux);
    //    $("#GridMessageErrorDispatchMaterialDetail").show();

    //}

    //if (!runningValidation) {
    //    ValidateDetailDispatchMaterial();
    //}
    
}


function ValidateDetailDispatchMaterial() {
    runningValidation = true;
    OnItemDispatchMaterialValidation(id_item, id_item);
    OnSourceExitQuantityValidation(sourceExitQuantity, sourceExitQuantity);
    runningValidation = false;

}

$(function () {
    //var codeDocumentState = $("#codeDocumentState").val();
    //if (codeDocumentState == "01")//PENDIENTE
    //{
    //    try {
    //        console.log("gvDispatchMaterials: " + gvDispatchMaterials);
    //        gvDispatchMaterials.UnselectRows();
    //        gvDispatchMaterials.PerformCallback();
    //    }
    //    catch (e) {
    //        //exist_Id_businessOportunityPlanningDetail = false;
    //    }
        
    //}

    //init();
});
