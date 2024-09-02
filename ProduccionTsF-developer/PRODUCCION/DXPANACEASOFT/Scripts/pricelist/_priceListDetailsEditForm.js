
//Validations

function MetricUnitValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    //else {
    //    var data = {
    //        id_item: s.GetValue()
    //    };
    //    $.ajax({
    //        url: "ProductionLotReception/ExistsConversionWithLbs",
    //        type: "post",
    //        data: data,
    //        async: true,
    //        cache: false,
    //        error: function (error) {
    //            console.log(error);
    //        },
    //        beforeSend: function () {
    //            //showLoading();
    //        },
    //        success: function (result) {
    //            if (result !== null) {
    //                if (result.metricUnitConversionValue == 0) {
    //                    e.isValid = false;
    //                    e.errorText = "Unidad medida del Item no tiene factor de conversión configurado con Libras cuyo codigo se espera que sea (Lbs). Configúrelo e intente de nuevo";
    //                };
    //            }
    //        },
    //        complete: function () {
    //            //hideLoading();
    //        }
    //    });
    //}
}

function OnPurchasePriceValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value < 0) {
            e.isValid = false;
            e.errorText = "Valor Incorrecto";
        }
    }
}

function PurchasePrice_ValueChanged(s, e) {
    var purchasePriceAux = s.GetValue();
    if (purchasePriceAux != null && purchasePriceAux >= 0) {

        var strPurchasePrice = purchasePriceAux == null ? "0" : purchasePriceAux.toString();
        var resPurchasePrice = strPurchasePrice.replace(".", ",");
        var data = {
            price: resPurchasePrice,
        };
        $.ajax({
            url: "PriceList/UpdateAdjustmentItemGroupCategory",
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
                    gvPriceListDetailCategoryAdjustments.PerformCallback();
                }
            },
            complete: function () {
                //hideLoading();
            }
        });

    }
}



function OnSalePriceValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value < 0) {
            e.isValid = false;
            e.errorText = "Valor Incorrecto";
        }
    }
}

function SalePrice_ValueChanged(s, e) {
    var salePriceAux = s.GetValue();
    if (salePriceAux != null && salePriceAux >= 0) {

        var strSalePrice = salePriceAux == null ? "0" : salePriceAux.toString();
        var resSalePrice = strSalePrice.replace(".", ",");
        var data = {
            price: resSalePrice,
        };
        $.ajax({
            url: "PriceList/UpdateAdjustmentItemGroupCategory",
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
                    gvPriceListDetailCategoryAdjustments.PerformCallback();
                }
            },
            complete: function () {
                //hideLoading();
            }
        });

    };

}

function OnSpecialPriceValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value < 0) {
            e.isValid = false;
            e.errorText = "Valor Incorrecto";
        }
    }
}
// EDITOR'S EVENTS



// EDITOR'S EVENTS
var customCommand = "";

function PriceListDetailsOnGridViewInit(s, e) {
    PriceListDetailsUpdateTitlePanel();
    s.PerformCallback();
    //gvPriceListDetails.PerformCallback();
    //console.log("Se Ejecuto PriceListDetailsOnGridViewInit");
}

function PriceListDetailsOnGridViewSelectionChanged(s, e) {
    PriceListDetailsUpdateTitlePanel();
    s.GetSelectedFieldValues("id_item", GetSelectedFieldValuesCallbackItemsDetail);
}

function PriceListDetailsOnGridViewBeginCallback(s, e) {
    customCommand = e.command;
    PriceListDetailsUpdateTitlePanel();
}

function PriceListDetailsOnGridViewEndCallback(s, e) {
    PriceListDetailsUpdateTitlePanel();
    if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
        s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
    }
}

function PriceListDetailsSelectAllRows() {
    gvPriceListDetails.SelectRows();
}

function PriceListDetailsClearSelection() {
    gvPriceListDetails.UnselectRows();
}

function PriceListDetailsUpdateTitlePanel() {
    var selectedFilteredRowCount = PriceListDetailsGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPriceListDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPriceListDetails.GetSelectedRowCount() - PriceListDetailsGetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPriceListDetails.GetSelectedRowCount() > 0 && gvPriceListDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPriceListDetails.GetSelectedRowCount() > 0);
    //}
    //var columns = gvPriceListDetails.columns;
    ////console.log("columns: ");
    ////console.log(columns);
    ////console.log("columns['1']: ");
    ////console.log(columns[1]);
    //columns[1].Visible = (false);
    //btnRemoveDetail.SetEnabled(gvPriceListDetails.GetSelectedRowCount() > 0);
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function PriceListDetailsGetSelectedFilteredRowCount() {
    return gvPriceListDetails.cpFilteredRowCountWithoutPage + gvPriceListDetails.GetSelectedKeysOnPage().length;
}

function GetSelectedFieldValuesCallbackItemsDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}




function init() {
    //var id_priceListAux = $("#id_priceListAux").val();
    //var codeState = $("#codeState").val();
    //if (codeState == "07")//PENDIENTE DE PAGO
    //{
    //    UpdateProductionLotPaymentDetails(id_priceListAux);
    //}
    //var columns = gvPriceListDetails.Columns;
    //console.log(columns);
    //col0.SetColVisible(false);
    //try{
    //    gvPriceListDetails.UnselectRows();
    //    gvPriceListDetails.PerformCallback();
    //}
    //catch(e){
    //    //No se puede refrescar aun
    //}
}

$(function () {

    init();
});