//*************
// FORMULATION
var id_inventoryLineIngredientItemInit = null;
var id_itemTypeIngredientItemInit = null;
var id_itemTypeCategoryIngredientItemInit = null;
var id_ingredientItemInit = null;
var id_metricUnitIngredientItemInit = null;

// InventoryLineIngredientItem
function InventoryLineIngredientItem_BeginCallback(s, e) {
    // 
    e.customArgs['id_inventoryLineIngredientItem'] = id_inventoryLineIngredientItemInit;//s.GetValue();//$("#itemId").val();
}

function InventoryLineIngredientItem_EndCallback(s, e) {
    s.SetValue(id_inventoryLineIngredientItemInit);
    id_itemType.PerformCallback();
}

function ComboInventoryLineIngredientItem_SelectedIndexChanged(s, e) {

    id_itemTypeIngredientItemInit = null;
    id_itemTypeCategoryIngredientItemInit = null;
    id_ingredientItemInit = null;
    id_metricUnitIngredientItemInit = null;

    id_itemType.ClearItems();
    id_itemTypeCategory.ClearItems();
    id_item.ClearItems();
    id_metricUnit.ClearItems();

    id_itemType.PerformCallback();
}

function OnInventoryLineIngredientItemValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
}

function OnInventoryLineIngredientItemInit(s, e) {
    // 

    id_inventoryLineIngredientItemInit = s.GetValue();
    id_itemTypeIngredientItemInit = id_itemType.GetValue();
    id_itemTypeCategoryIngredientItemInit = id_itemTypeCategory.GetValue();
    id_ingredientItemInit = id_item.GetValue();
    id_metricUnitIngredientItemInit = id_metricUnit.GetValue();

    //if (codProducts.GetText() !== "" && codProducts.GetText() !== null) {
    //    s.SetEnabled(false);
    //    id_itemType.SetEnabled(false);
    //    id_itemTypeCategory.SetEnabled(false);
    //    id_item.SetEnabled(false);
    //    id_metricUnit.SetEnabled(false);
    //}
    id_inventoryLine.PerformCallback();
}


// id_itemTypeIngredientItem
function ItemTypeIngredientItem_BeginCallback(s, e) {
    e.customArgs['id_inventoryLineIngredientItem'] = id_inventoryLine.GetValue();//$("#itemId").val();
    e.customArgs['id_itemTypeIngredientItem'] = id_itemTypeIngredientItemInit;//$("#itemId").val();
}

function ItemTypeIngredientItem_EndCallback(s, e) {
    s.SetValue(id_itemTypeIngredientItemInit);
    id_itemTypeCategory.PerformCallback();
}

function ComboItemTypeIngredientItem_SelectedIndexChanged(s, e) {

    id_itemTypeCategoryIngredientItemInit = null;
    id_ingredientItemInit = null;
    id_metricUnitIngredientItemInit = null;

    id_itemTypeCategory.ClearItems();
    id_item.ClearItems();
    id_metricUnit.ClearItems();

    id_itemTypeCategory.PerformCallback();
}

function OnItemTypeIngredientItemValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
}

// id_itemTypeCategoryIngredientItem
function ItemTypeCategoryIngredientItem_BeginCallback(s, e) {
    e.customArgs['id_itemTypeIngredientItem'] = id_itemType.GetValue();//$("#itemId").val();
    e.customArgs['id_itemTypeCategoryIngredientItem'] = id_itemTypeCategoryIngredientItemInit;//$("#itemId").val();
}

function ItemTypeCategoryIngredientItem_EndCallback(s, e) {
    s.SetValue(id_itemTypeCategoryIngredientItemInit);
    id_item.PerformCallback();
}

function ComboItemTypeCategoryIngredientItem_SelectedIndexChanged(s, e) {

    id_ingredientItemInit = null;
    id_metricUnitIngredientItemInit = null;

    id_item.ClearItems();
    id_metricUnit.ClearItems();

    id_item.PerformCallback();
}

function OnItemTypeCategoryIngredientItemValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
}


// id_product
var id_detailItemInit = null;

function OnItemDetailItemInit(s, e) {
    id_detailItemInit = s.GetValue();
    s.PerformCallback();
}

function DetailItem_BeginCallback(s, e) {
    e.customArgs['id_product'] = s.GetValue();//id_detailItemInit;//$("#itemId").val();
}

function DetailItem_EndCallback(s, e) {
    if (id_detailItemInit !== null)
        s.SetValue(id_detailItemInit);
}

function OnItemDetailItemValidation(s, e) {
    //var tab = tabControl.GetTabByName("tabFormulation");
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
    //else if (e.value !== id_ingredientItemInit || id_metricUnit.GetValue() !== id_metricUnitIngredientItemInit) {
    //    $.ajax({
    //        url: "SalesOrder/ItsRepeatedIngredientItem",
    //        type: "post",
    //        data: {
    //            id_ingredientItem: e.value,
    //            id_metricUnitIngredientItem: id_metricUnit.GetValue()
    //        },
    //        async: false,
    //        cache: false,
    //        error: function (error) {
    //            console.log(error);
    //        },
    //        beforeSend: function () {
    //            showLoading();
    //        },
    //        success: function (result) {
    //            if (result !== null) {
    //                if (result.itsRepeated == 1) {
    //                    e.isValid = false;
    //                    e.errorText = result.Error;
    //                }
    //            }
    //        },
    //        complete: function () {
    //            hideLoading();
    //        }
    //    });
    //}

    //UpdateTabImage(e, "tabFormulation");
}


// id_ingredientItem
function IngredientItem_BeginCallback(s, e) {
    e.customArgs['id_inventoryLineIngredientItem'] = id_inventoryLine.GetValue();//$("#itemId").val();
    e.customArgs['id_itemTypeIngredientItem'] = id_itemType.GetValue();//$("#itemId").val();
    e.customArgs['id_itemTypeCategoryIngredientItem'] = id_itemTypeCategory.GetValue();//$("#itemId").val();
    e.customArgs['id_ingredientItem'] = id_ingredientItemInit;//$("#itemId").val();
}

function IngredientItem_EndCallback(s, e) {
    if (id_ingredientItemInit !== null)
        s.SetValue(id_ingredientItemInit);
    id_metricUnit.PerformCallback();
}

function ComboItemIngredientItem_SelectedIndexChanged(s, e) {

    id_metricUnitIngredientItemInit = null;

    id_metricUnit.ClearItems();

    id_metricUnit.PerformCallback();
}

function AmountIngredientItem_ValueChanged(s, e) {
    //var valueAmountIngredientItem = s.GetValue();
    //var valueMetricUnitIngredientItem = id_metricUnitIngredientItem.GetValue();
    //var valueAmountMaxIngredientItem = amountMaxIngredientItem.GetValue();
    //var valueMetricUnitMaxIngredientItem = id_metricUnitMaxIngredientItem.GetValue();


    //UpdateEnabledAmountMetricUnit(valueAmountIngredientItem, valueMetricUnitIngredientItem, valueAmountMaxIngredientItem, valueMetricUnitMaxIngredientItem);
}

function OnAmountIngredientItemValidation(s, e) {
    // 
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else {
        var aQuantityRequiredForFormulation = quantityRequiredForFormulation.GetValue() != null ? parseFloat(quantityRequiredForFormulation.GetValue()) : 0.00;
        if (aQuantityRequiredForFormulation > e.value || 0 >= e.value) {
            //if (e.value <= 0) {
            e.isValid = false;
            e.errorText = aQuantityRequiredForFormulation === 0.00 ? "Cantidad debe ser mayor o igual que cero" : "Cantidad debe ser mayor o igual a la Cantidad Requerido en la Formulación";
        }
    }

}

function OnItemIngredientItemInit(s, e) {

    //id_ingredientItemAux = s.GetValue();//id_ingredientItem

    //var valueAmountIngredientItem = quantity.GetValue();
    //var valueMetricUnitIngredientItem = id_metricUnit.GetValue();


    //UpdateEnabledAmountMetricUnit(valueAmountIngredientItem, valueMetricUnitIngredientItem, valueAmountMaxIngredientItem, valueMetricUnitMaxIngredientItem);
}

function OnItemIngredientItemValidation(s, e) {
    //var tab = tabControl.GetTabByName("tabFormulation");
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
    else {
        var id_productAux = id_product !== undefined ? id_product.GetValue() : null;
        if (e.value !== id_ingredientItemInit || id_metricUnit.GetValue() !== id_metricUnitIngredientItemInit || id_productAux !== id_detailItemInit) {
            $.ajax({
                url: "SalesOrder/ItsRepeatedIngredientItem",
                type: "post",
                data: {
                    id_ingredientItem: e.value,
                    id_metricUnitIngredientItem: id_metricUnit.GetValue(),
                    id_product: id_productAux
                },
                async: false,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                    if (result !== null) {
                        if (result.itsRepeated == 1) {
                            e.isValid = false;
                            e.errorText = result.Error;
                        }
                    }
                },
                complete: function () {
                    hideLoading();
                }
            });
        }
    }

    //UpdateTabImage(e, "tabFormulation");
}

//function UpdateEnabledAmountMetricUnit(valueAmountIngredientItem, valueMetricUnitIngredientItem) {
//    if (valueAmountIngredientItem == null && valueMetricUnitIngredientItem == null) {
//        amountMaxIngredientItem.SetEnabled(true);
//        id_metricUnitMaxIngredientItem.SetEnabled(true);
//    } else {
//        //amountMaxIngredientItem.SetEnabled(false);
//        //id_metricUnitMaxIngredientItem.SetEnabled(false);
//    }
//}

// id_metricUnitIngredientItem
function MetricUnitIngredientItem_BeginCallback(s, e) {
    e.customArgs['id_ingredientItem'] = id_item.GetValue();//$("#itemId").val();
    e.customArgs['id_metricUnitIngredientItem'] = id_metricUnitIngredientItemInit;//$("#itemId").val();
}

function MetricUnitIngredientItem_EndCallback(s, e) {
    //s.SetValue(id_metricUnitIngredientItemInit);
    if (id_metricUnitIngredientItemInit !== null) {
        s.SetValue(id_metricUnitIngredientItemInit);
    } else {
        var id_ingredientItemAux = id_item.GetValue()
        if (id_ingredientItemAux !== null && id_ingredientItemAux !== undefined) {
            $.ajax({
                url: "SalesOrder/GetValueMetricUnitIngredientItem",
                type: "post",
                data: {
                    id_ingredientItem: id_ingredientItemAux
                },
                async: false,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                    if (result !== null) {
                        s.SetValue(result.id_metricUnitIngredientItem);
                    }
                },
                complete: function () {
                    hideLoading();
                }
            });
        }
    }
}

function ComboMetricUnitIngredientItem_SelectedIndexChanged(s, e) {
    //var valueAmountIngredientItem = amountIngredientItem.GetValue();
    //var valueMetricUnitIngredientItem = s.GetValue();
    //var valueAmountMaxIngredientItem = amountMaxIngredientItem.GetValue();
    //var valueMetricUnitMaxIngredientItem = id_metricUnitMaxIngredientItem.GetValue();


    //UpdateEnabledAmountMetricUnit(valueAmountIngredientItem, valueMetricUnitIngredientItem, valueAmountMaxIngredientItem, valueMetricUnitMaxIngredientItem);
}

function OnMetricUnitIngredientItemValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
}

function OnGridViewMPMaterialDetailEndCallback(s, e) {
    if (GridViewMPMaterialDetails.cpError !== null && GridViewMPMaterialDetails.cpError !== "") {
        NotifyError(GridViewMPMaterialDetails.cpError);
    }

    GridViewMPMaterialSummaryDetails.PerformCallback();
}

function AddNewMPMaterialDetail(s, e) {
    GridViewMPMaterialDetails.AddNewRow();
}

function RefreshMPMaterialDetail(s, e) {

    GridViewMPMaterialDetails.PerformCallback();
}

//**************


var id_itemInit = null;
var id_itemInit1 = null;
var commandAux = null;

//id_item
function ItemComboBox_Init() {
    //if (GridViewDetails.cpRowId === 0) {
    //    stop.SetChecked(true);
    //    totalHoursDetail.SetValue("00:00");
    //}

    id_itemInit = id_item.GetValue();
    id_itemInit1 = id_itemInit;
    id_item.PerformCallback();
}

function ItemComboBox_BeginCallback(s, e) {
    //// 
    //commandAux = e.command;
    e.customArgs["id_item"] = id_itemInit === undefined ? "" : id_itemInit;
    try {
        e.customArgs["nameItem"] = nameItemFilter.GetValue() === undefined ? "" : nameItemFilter.GetValue();
        e.customArgs["sizeBegin"] = ComboBoxSizeBegin.GetValue() === undefined ? null : ComboBoxSizeBegin.GetValue();
        e.customArgs["sizeEnd"] = ComboBoxSizeEnd.GetValue() === undefined ? null : ComboBoxSizeEnd.GetValue();
        e.customArgs["inventoryLine"] = ComboBoxInventoryLine.GetValue() === undefined ? null : ComboBoxInventoryLine.GetValue();
        e.customArgs["itemType"] = ComboBoxItemType.GetValue() === undefined ? null : ComboBoxItemType.GetValue();
        e.customArgs["itemTypeCategory"] = ComboBoxItemTypeCategory.GetValue() === undefined ? null : ComboBoxItemTypeCategory.GetValue();
        e.customArgs["itemGroup"] = ComboBoxItemGroup.GetValue() === undefined ? null : ComboBoxItemGroup.GetValue();
        e.customArgs["itemSubGroup"] = ComboBoxItemSubGroup.GetValue() === undefined ? null : ComboBoxItemSubGroup.GetValue();
        e.customArgs["itemSize"] = ComboBoxItemSize.GetValue() === undefined ? null : ComboBoxItemSize.GetValue();
        e.customArgs["itemTrademark"] = ComboBoxItemTrademark.GetValue() === undefined ? null : ComboBoxItemTrademark.GetValue();
        e.customArgs["itemTrademarkModel"] = ComboBoxItemTrademarkModel.GetValue() === undefined ? null : ComboBoxItemTrademarkModel.GetValue();
        e.customArgs["itemColor"] = ComboBoxItemColor.GetValue() === undefined ? null : ComboBoxItemColor.GetValue();
    } catch (e) {

    }
}

function ItemComboBox_EndCallback(s, e) {
    //// 
    if (id_itemInit1 != null) {
        id_item.SetValue(id_itemInit1);
        id_itemInit1 = null;
    }
}

function ItemComboBox_SelectedIndexChanged() {
    $.ajax({
        url: "SalesOrder/GetItem",
        type: "post",
        data: { id_item: id_item.GetValue(), quantityApproved: parseFloat(quantityApproved.GetValue()) },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            if (result !== null) {
                description_item.SetText(result.description_item);
                cod_item.SetText(result.cod_item);
                if (result.noRequestProforma !== "" && result.noRequestProforma !== null) {
                    noRequestProforma.SetText(result.noRequestProforma);
                }
                quantityProgrammed.SetValue(result.quantityProgrammed);
                quantityApproved.SetValue(result.quantityApproved);
                //codAux_item.SetText(result.codAux_item);
                //originQuantityStr.SetText(result.originQuantityStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

//Validations
var errorMessage = "";
var runningValidation = false;

function ItemComboBox_Validation(s, e) {
    errorMessage = "";
    $("#GridMessageErrorDetail").hide();

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Nombre del Producto: Es obligatorio.";
    } else {
        var data = {
            id_itemNew: s.GetValue()
        };
        if (data.id_itemNew !== id_itemInit) {
            $.ajax({
                url: "SalesOrder/ItsRepeat",
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
                        if (result.itsRepeat === 1) {
                            e.isValid = false;
                            e.errorText = result.Error;
                            //errorMessage = result.Error;
                            if (errorMessage === null || errorMessage === "") {
                                errorMessage = result.Error;
                            } else {
                                errorMessage = result.Error;
                            }
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

//function CartonsSpinEdit_Validation(s, e) {
//    if (s.GetValue() === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//        //errorMessage = "- Producto Liquidación: Es obligatorio.";
//        if (errorMessage === null || errorMessage === "") {
//            errorMessage = "- Cartones: Es obligatorio.";
//        } else {
//            errorMessage += "</br> - Cartones: Es obligatorio.";
//        }
//    } else {
//        if (parseFloat(s.GetValue()) < 0) {
//            e.isValid = false;
//            e.errorText = "Debe ser Mayor e igual que cero";
//            //errorMessage = "- Producto Liquidación: Es obligatorio.";
//            if (errorMessage === null || errorMessage === "") {
//                errorMessage = "- Cartones: Debe ser Mayor e igual que cero.";
//            } else {
//                errorMessage += "</br> - Cartones: Debe ser Mayor e igual que cero.";
//            }
//        }
//    }

//    if (!runningValidation) {
//        ValidateDetail();
//    }

//    if (errorMessage !== null && errorMessage !== "") {
//        var msgErrorAux = ErrorMessage(errorMessage);
//        gridMessageErrorDetail.SetText(msgErrorAux);
//        $("#GridMessageErrorDetail").show();
//    }
//}

function QuantityApprovedSpinEdit_Validation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Cant. Aprobada: Campo Obligatorio";
        //errorMessage = "- Producto Liquidación: Es obligatorio.";
        if (errorMessage === null || errorMessage === "") {
            errorMessage = "- Cant. Aprobada: Es obligatoria.";
        } else {
            errorMessage += "</br> - Cant. Aprobada: Es obligatoria.";
        }
    } else {
        var aQuantityApproved = parseFloat(s.GetValue());
        if (aQuantityApproved <= 0) {
            e.isValid = false;
            e.errorText = "Cant. Aprobada: Debe ser Mayor que cero";
            //errorMessage = "- Producto Liquidación: Es obligatorio.";
            if (errorMessage === null || errorMessage === "") {
                errorMessage = "- Cant. Aprobada: Debe ser Mayor que cero.";
            } else {
                errorMessage += "</br> - Cant. Aprobada: Debe ser Mayor que cero.";
            }
        } else {
            //var aQuantityProgrammed = quantityProgrammed.GetValue() != null ? parseFloat(quantityProgrammed.GetValue()) : 0.00;
            //if (aQuantityProgrammed > 0 && aQuantityProgrammed > aQuantityApproved) {
            //    e.isValid = false;
            //    e.errorText = "Cant. Aprobada: Debe ser Mayor e igual que Cant. Programada";
            //    //errorMessage = "- Producto Liquidación: Es obligatorio.";
            //    if (errorMessage === null || errorMessage === "") {
            //        errorMessage = "- Cant. Aprobada: Debe ser Mayor e igual que Cant. Programada.";
            //    } else {
            //        errorMessage += "</br> - Cant. Aprobada: Debe ser Mayor e igual que Cant. Programada.";
            //    }
            //} else {
                //var aQuantityProduced = quantityProduced.GetValue() != null ? parseFloat(quantityProduced.GetValue()) : 0.00;
                //if (aQuantityProduced > 0 && aQuantityProduced > aQuantityApproved) {
                //    e.isValid = false;
                //    e.errorText = "Cant. Aprobada: Debe ser Mayor e igual que Cant. Producida";
                //    //errorMessage = "- Producto Liquidación: Es obligatorio.";
                //    if (errorMessage === null || errorMessage === "") {
                //        errorMessage = "- Cant. Aprobada: Debe ser Mayor e igual que Cant. Producida.";
                //    } else {
                //        errorMessage += "</br> - Cant. Aprobada: Debe ser Mayor e igual que Cant. Producida.";
                //    }
                //}
           // }
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }

    if (errorMessage !== null && errorMessage !== "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorDetail.SetText(msgErrorAux);
        $("#GridMessageErrorDetail").show();
    }
}

function ValidateDetail() {
    runningValidation = true;
    ItemComboBox_Validation(id_item, id_item);
    QuantityApprovedSpinEdit_Validation(quantityApproved, quantityApproved);
    runningValidation = false;
}

function ComboBoxItemSubGroup_BeginCallback(s, e) {
    e.customArgs["id_itemGroup"] = ComboBoxItemGroup.GetValue() === undefined ? null : ComboBoxItemGroup.GetValue();
}

function ComboBoxItemGroup_SelectedIndexChanged(s, e) {
    ComboBoxItemSubGroup.PerformCallback();
}

function ComboBoxItemTypeCategory_BeginCallback(s, e) {
    e.customArgs["id_itemType"] = ComboBoxItemType.GetValue() === undefined ? null : ComboBoxItemType.GetValue();
}

function ComboBoxItemType_SelectedIndexChanged(s, e) {
    ComboBoxItemTypeCategory.PerformCallback();
}

function ComboBoxItemType_EndCallback(s, e) {
    ComboBoxItemTypeCategory.PerformCallback();
}

function ComboBoxItemType_BeginCallback(s, e) {
    e.customArgs["id_inventoryLine"] = ComboBoxInventoryLine.GetValue() === undefined ? null : ComboBoxInventoryLine.GetValue();
}

function ComboBoxInventoryLine_SelectedIndexChanged(s, e) {
    ComboBoxItemType.PerformCallback();
}

function AddNewDetail(s, e) {
    GridViewDetails.AddNewRow();
}

function RefreshDetail(s, e) {

    GridViewDetails.PerformCallback();
}


function OnGridViewDetailBeginCallback(s, e) {
    if (e.command === 'UPDATEEDIT') {
        e.customArgs["enabledCurrent"] = GridViewDetails.cpEnabled;
    }
}

function OnGridViewDetailEndCallback(s, e) {
    $.ajax({
        url: "SalesOrder/GetTotales",
        type: "post",
        data: null,
        async: true,
        cache: false,
        error: function (error) {
            //// 
            NotifyError("Error. " + error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $('#labelTotalCM').text(result.totalCM);
            $('#labelNetoLbs').text(result.netoLbs);
            $('#labelNetoKg').text(result.netoKg);
        },
        complete: function () {
            if (GridViewDetails.cpError !== null && GridViewDetails.cpError !== "") {
                NotifyError(GridViewDetails.cpError);
            }
            GridViewMPMaterialDetails.PerformCallback();
            hideLoading();
        }
    });
}

function ShowCurrentItem(enabled) {
    var data = {
        id: $('#id_salesOrder').val(),
        ids: [],
        id_proforma: null,
        enabled: enabled
    };

    showPage("SalesOrder/Edit", data);
}

function AddNewItem() {
    if ($('#code_documentType').val() === "139") {
        var data = {
            id: 0,
            ids: [],
            id_proforma: null,
            enabled: true
        };
        showPage("SalesOrder/Edit", data);
    } else {
        if ($('#code_documentType').val() === "140") {
            $.ajax({
                url: "SalesOrder/PendingNewOrderForeignCustomers",
                type: "post",
                data: null,
                async: true,
                cache: false,
                error: function (error) {
                    //// 
                    NotifyError("Error. " + error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                    $("#maincontent").html(result);
                },
                complete: function () {
                    hideLoading();
                }
            });
        } else {
            if ($('#code_documentType').val() === "141") {
                $.ajax({
                    url: "SalesOrder/PendingNewOrderLocalClient",
                    type: "post",
                    data: null,
                    async: true,
                    cache: false,
                    error: function (error) {
                        //// 
                        NotifyError("Error. " + error);
                    },
                    beforeSend: function () {
                        showLoading();
                    },
                    success: function (result) {
                        $("#maincontent").html(result);
                    },
                    complete: function () {
                        hideLoading();
                    }
                });
            }
        }
    }
}

function EditCurrentItem() {
    showLoading();
    ShowCurrentItem(true);
}

function SaveCurrentItem() {
    SaveItem(false);
}

function ClosedCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Cerrar la Orden de Producción?", "Confirmar");
    result.done(function (dialogResult) {

        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'SalesOrder/Close',
                type: 'post',
                data: { id: $('#id_salesOrder').val() },
                async: true,
                cache: false,
                error: function (result) {
                    hideLoading();
                    NotifyError("Error. " + result.Message);
                },
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error al Cerrar. " + result.Message);
                        return;
                    }

                    ShowCurrentItem(false);
                    hideLoading();
                    NotifySuccess("Orden de Producción Cerrada Satisfactoriamente. " + "Estado: " + result.Data);
                }
            });
        }
    });
}

function AprovedItem() {
    showLoading();
    $.ajax({
        url: 'SalesOrder/Approve',
        type: 'post',
        data: { id: $('#id_salesOrder').val() },
        async: true,
        cache: false,
        error: function (result) {
            hideLoading();
            NotifyError("Error. " + result.Message);
        },
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error al Aprobar. " + result.Message);
                return;
            }

            ShowCurrentItem(false);
            hideLoading();
            NotifySuccess("Orden de Producción Aprobada Satisfactoriamente. " + "Estado: " + result.Data);
        }
    });
}

function AprovedCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Aprobar la Orden de Producción?", "Confirmar");
    result.done(function (dialogResult) {

        if (dialogResult) {
            if ($("#enabled").val() == "true") {
                SaveItem(true);
            } else {
                AprovedItem();
            }
        }
    });
}

function ReverseCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Reversar la Orden de Producción?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'SalesOrder/Reverse',
                type: 'post',
                data: { id: $('#id_salesOrder').val() },
                async: true,
                cache: false,
                error: function (result) {
                    hideLoading();
                    NotifyError("Error. " + result.Message);
                },
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error al Reversar. " + result.Message);
                        return;
                    }

                    ShowCurrentItem(false);
                    hideLoading();
                    NotifySuccess("Orden de Producción Reversada Satisfactoriamente. " + "Estado: " + result.Data);
                }
            });
        }
    });
}

function AnnulCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Anular la Orden de Producción?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'SalesOrder/Annul',
                type: 'post',
                data: { id: $('#id_salesOrder').val() },
                async: true,
                cache: false,
                error: function (result) {
                    hideLoading();
                    NotifyError("Error. " + result.Message);
                },
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error al Anular. " + result.Message);
                        return;
                    }

                    ShowCurrentItem(false);
                    hideLoading();
                    NotifySuccess("Orden de Producción Anulada Satisfactoriamente. " + "Estado: " + result.Data);
                }
            });
        }
    });
}

function SaveDataUser() {
    var emisionDate = DateTimeEmision.GetValue();
    var yearEmisionDate = emisionDate.getFullYear();
    var monthEmisionDate = emisionDate.getMonth();
    var dayEmisionDate = emisionDate.getDate();

    var dateShipmentAux = dateShipment.GetValue();
    var yearDateShipment = dateShipmentAux.getFullYear();
    var monthDateShipment = dateShipmentAux.getMonth();
    var dayDateShipment = dateShipmentAux.getDate();

    var controls = ASPxClientControl.GetControlCollection();
    var ComboBoxProvider = controls.GetByName('ComboBoxProvider');

    var userData = {
        id: $('#id_salesOrder').val(),
        dateTimeEmision: yearEmisionDate + "-" +
            (++monthEmisionDate).toString().padStart(2, 0) + "-" +
            dayEmisionDate.toString().padStart(2, 0) + "T00:00:00",
        reference: TextBoxReference.GetText(),
        description: MemoDescription.GetText(),
        id_customer: ComboBoxCustomer.GetValue(),
        id_provider: ComboBoxProvider == null ? null : ComboBoxProvider.GetValue(),
        additionalInstructions: additionalInstructions.GetText(),
        shippingDocument: shippingDocument.GetText(),
        id_orderReason: ($('#code_documentType').val() === "140") ? null : ComboBoxReason.GetValue(),
        numeroLote: ($('#code_documentType').val() === "140") ? null : TextBoxNumeroLote.GetText(),
        requiredLogistic: logistics.GetValue(),
        dateShipment: yearDateShipment + "-" +
            (++monthDateShipment).toString().padStart(2, 0) + "-" +
            dayDateShipment.toString().padStart(2, 0) + "T00:00:00",
        id_portDestination: ComboBoxPortDestination.GetValue(),
        id_portDischarge: ($('#code_documentType').val() === "140") ? ComboBoxPortDischarge.GetValue() : null,
        id_employeeApplicant: ComboBoxEmployeeApplicant.GetValue()
    };

    var SalesOrder = {
        jsonSalesOrder: JSON.stringify(userData)
    };

    return SalesOrder;
}

function SaveItem(aproved) {
    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }

    $.ajax({
        url: 'SalesOrder/Save',
        type: 'post',
        data: SaveDataUser(),
        async: true,
        cache: false,
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error. " + result.Message);
                return;
            }

            var id = result.Data;
            $('#id_salesOrder').val(id);

            if (aproved)
                AprovedItem();
            else
                ShowCurrentItem(true);

            hideLoading();
            NotifySuccess("La Orden de Producción Guardada Satisfactoriamente.");
        },
        error: function (result) {
            hideLoading();
        }
    });
}

function IsValid(object) {
    return (object != null && object != undefined && object != "undefined");
}

function Validate() {
    var validate = true;
    var errors = "";

    if (!IsValid(DateTimeEmision) || DateTimeEmision.GetValue() === null) {
        errors += "Fecha Emisión es un campo Obligatorio. \n\r";
        validate = false;
    } else {
        var aDateTimeEmision = DateTimeEmision.GetValue();
        var yearDateTimeEmision = aDateTimeEmision.getFullYear();
        var monthDateTimeEmision = aDateTimeEmision.getMonth() + 1;
        var dayDateTimeEmision = aDateTimeEmision.getDate();
        var dateTimeEmisionAux = new Date(yearDateTimeEmision, aDateTimeEmision.getMonth(), dayDateTimeEmision);

        var dateHoy = $('#dateHoy').val().split("-");
        var yearHoyDate = parseInt(dateHoy[2]);
        var monthHoyDate = parseInt(dateHoy[1]);
        var dayHoyDate = parseInt(dateHoy[0]);
        var hoyDateAux = new Date(yearHoyDate, monthHoyDate - 1, dayHoyDate);

        var dateHoyMin = $('#dateHoyMin').val().split("-");
        var yearHoyMinDate = parseInt(dateHoyMin[2]);
        var monthHoyMinDate = parseInt(dateHoyMin[1]);
        var dayHoyMinDate = parseInt(dateHoyMin[0]);
        var hoyMinDateAux = new Date(yearHoyMinDate, monthHoyMinDate - 1, dayHoyMinDate);

        if (dateTimeEmisionAux.getTime() !== hoyDateAux.getTime() && dateTimeEmisionAux.getTime() !== hoyMinDateAux.getTime() &&
            (dateTimeEmisionAux < hoyMinDateAux || dateTimeEmisionAux > hoyDateAux)) {
            errors += "Fecha de emisión debe ser mayor o igual a fecha mínima y menor o igual a la fecha de hoy. \n\r";
            validate = false;
        }
    }
    if (!IsValid(ComboBoxEmployeeApplicant) || ComboBoxEmployeeApplicant.GetValue() === null) {
        errors += "Solicitante es un campo Obligatorio. \n\r";
        validate = false;
    }
    if ($('#code_documentType').val() === "139") {
        if (!IsValid(ComboBoxCustomer) || ComboBoxCustomer.GetValue() === null) {
            errors += "Cliente es un campo Obligatorio. \n\r";
            validate = false;
        }
    }
    var controls = ASPxClientControl.GetControlCollection();
    var ComboBoxProvider = controls.GetByName('ComboBoxProvider');
    if ($('#code_documentType').val() === "140") {
        if ((ComboBoxProvider != null) && ((!IsValid(ComboBoxProvider) || ComboBoxProvider.GetValue() === null))) {
            errors += "Proveedor es un campo Obligatorio. \n\r";
            validate = false;
        }
    }
    if ($('#code_documentType').val() === "140") {
        if (!IsValid(dateShipment) || dateShipment.GetValue() === null) {
            errors += "Fecha de Embarque es un campo Obligatorio. \n\r";
            validate = false;
        } else {
            var aDateTimeEmision2 = DateTimeEmision.GetValue();
            var yearDateTimeEmision2 = aDateTimeEmision2.getFullYear();
            var monthDateTimeEmision2 = aDateTimeEmision2.getMonth() + 1;
            var dayDateTimeEmision2 = aDateTimeEmision2.getDate();
            var dateTimeEmisionAux2 = new Date(yearDateTimeEmision2, aDateTimeEmision2.getMonth(), dayDateTimeEmision2);

            var aDateShipment = dateShipment.GetValue();
            var yearDateShipment = aDateShipment.getFullYear();
            var monthDateShipment = aDateShipment.getMonth() + 1;
            var dayDateShipment = aDateShipment.getDate();
            var dateShipmentAux = new Date(yearDateShipment, aDateShipment.getMonth(), dayDateShipment);

            if (dateShipmentAux.getTime() !== dateTimeEmisionAux2.getTime() && dateShipmentAux < dateTimeEmisionAux2) {
                errors += "Fecha de Embarque debe ser mayor o igual a Fecha de Emisión. \n\r";
                validate = false;
            }

        }
        if (!IsValid(ComboBoxPortDischarge) || ComboBoxPortDischarge.GetValue() === null) {
            errors += "Puerto Descarga es un campo Obligatorio. \n\r";
            validate = false;
        }
        if (!IsValid(ComboBoxPortDestination) || ComboBoxPortDestination.GetValue() === null) {
            errors += "Puerto Destino es un campo Obligatorio. \n\r";
            validate = false;
        }
    } else {
        if (!IsValid(dateShipment) || dateShipment.GetValue() === null) {
            errors += "Fecha de Entrega es un campo Obligatorio. \n\r";
            validate = false;
        }
        if (!IsValid(ComboBoxPortDestination) || ComboBoxPortDestination.GetValue() === null) {
            errors += "Punto de Entrega es un campo Obligatorio. \n\r";
            validate = false;
        }
    }

    if (GridViewMPMaterialDetails.IsEditing()) {
        //UpdateTabImage({ isValid: false }, "tabDetails");
        errors += "Debe terminar de editar los MP & Materiales. \n\r";
        validate = false;
    }
    //else {
    //    UpdateTabImage({ isValid: true }, "tabMPMaterialDetails");
    //}

    if (validate === false) {
        NotifyError("Error. " + errors);
    }

    return validate;
}

function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }

    var tab = pcFeatures.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}

function ButtonUpdate_Click() {
    SaveItem(false);
}

function ButtonCancel_Click() {
    RedirecBack();
}

function PrintItem() {
    var id = $("#id_salesOrder").val();
    if (!(id === 0) && !(id === null)) {
        $.ajax({
            url: "SalesOrder/PrintReport",
            type: "post",
            data: {
                id: id,
                codeReport: "RPTOP"
            },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                try {
                    if (result !== undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/Index?trepd=' + reportTdr;
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function PrintItemVentas() {
    var id = $("#id_salesOrder").val();
    if (!(id === 0) && !(id === null)) {
        $.ajax({
            url: "SalesOrder/PrintReport",
            type: "post",
            data: {
                id: id,
                codeReport: "RVENOP"
            },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                try {
                    if (result !== undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/Index?trepd=' + reportTdr;
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function PrintItemMaterial() {
    var id = $("#id_salesOrder").val();
    if (!(id === 0) && !(id === null)) {
        $.ajax({
            url: "SalesOrder/PrintReport",
            type: "post",
            data: {
                id: id,
                codeReport: "RPTOPD"
            },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                try {
                    if (result !== undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/Index?trepd=' + reportTdr;
                        newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                        newWindow.focus();
                        hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function RedirecBack() {
    showPage("SalesOrder/Index");
}

function InitializePagination() {

    if ($("#id_salesOrder").val() !== 0) {

        var current_page = 1;
        var max_page = 1;
        $.ajax({
            url: "SalesOrder/InitializePagination",
            type: "post",
            data: { id: $("#id_salesOrder").val() },
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
            },
            success: function (result) {
                max_page = result.maximunPages;
                current_page = result.currentPage;
            },
            complete: function () {
            }
        });

        $('.pagination').jqPagination({
            current_page: current_page,
            max_page: max_page,
            page_string: "{current_page} de {max_page}",
            paged: function (page) {
                showPage("SalesOrder/Pagination", { page: page });
            }
        });
    }
}

function Init() {
}

$(function () {
    InitializePagination();
    Init();
});