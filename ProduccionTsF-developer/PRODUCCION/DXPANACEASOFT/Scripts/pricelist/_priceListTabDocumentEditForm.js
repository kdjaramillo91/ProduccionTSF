
// VALIDATIONS
//var Message = "Ok";

function OnPriceListEmissionDateValidation(s, e) {
    //if (e.value === null) {
    //    e.isValid = false;
    //    e.errorText = "Campo Obligatorio";
    //}
    OnEmissionDateDocumentValidation(e, emissionDate, "priceList");
    UpdateTabImage(e, "tabDocument");
}

function OnPriceListDocumentTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }

    UpdateTabImage(e, "tabDocument");
}

//function UpdateProductionLotPriceLists(priceLists) {

//    for (var i = 0; i < id_priceList.GetItemCount() ; i++) {
//        var priceList = id_priceList.GetItem(i);
//        var into = false;
//        for (var j = 0; j < priceLists.length; j++) {
//            if (priceList.value == priceLists[j].id) {
//                into = true;
//                break;
//            }
//        }
//        if (!into) {
//            id_priceList.RemoveItem(i);
//            i -= 1;
//        }
//    }


//    for (var i = 0; i < priceLists.length; i++) {
//        var priceList = id_priceList.FindItemByValue(priceLists[i].id);
//        if (priceList == null) id_priceList.AddItem(priceLists[i].name, priceLists[i].id);
//    }

//}

//function ComboPriceList_Init(s, e) {
//    var data = {
//        id_priceList: s.GetValue(),
//        id_provider: id_provider.GetValue()
//    };

//    $.ajax({
//        url: "ProductionLotReception/ProductionLotReceptionPriceListData",
//        type: "post",
//        data: data,
//        async: false,
//        cache: false,
//        error: function (error) {
//            console.log(error);
//            id_priceList.ClearItems();
//        },
//        beforeSend: function () {
//            //showLoading();
//        },
//        success: function (result) {
//            if (result !== null && result !== undefined) {
//                UpdateProductionLotPriceLists(result.priceLists);
//            }
//            else {
//                id_priceList.ClearItems();
//            }
//        },
//        complete: function () {
//            //hideLoading();
//        }
//    });
//}

function ComboDocumentType_SelectedIndexChanged(s, e) {

    var data = {
        id_documentType: s.GetValue()
    };

    $.ajax({
        url: "PriceList/GetCodeDocumentType",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdateTabPurchaseAndTabSale(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                UpdateTabPurchaseAndTabSale(result);
            }
            else {
                UpdateTabPurchaseAndTabSale(null);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
    //gvProductionLotReceptionEditFormPaymentsDetail.UnselectRows();
    //gvProductionLotReceptionEditFormPaymentsDetail.PerformCallback();
    //UpdateProductionLotPaymentDetails(s.GetValue());
    //OnPriceListValidation(s, e);
    //console.log("s.GetValue() : ");
    //console.log(s.GetValue());
}
function UpdatePriceListBase(id_priceListBase, priceListBase) {
    for (var i = 0; i < priceListBase.length; i++) {
        var plb = id_priceListBase.FindItemByValue(priceListBase[i].id);
        if (plb == null) id_priceListBase.AddItem(priceListBase[i].name, priceListBase[i].id);
    }
}
function UpdateTabPurchaseAndTabSale(result) {
    var codeDocumentType = result != null ? result.codeDocumentType : "";
    $("#codeDocumentType").val(codeDocumentType);
    id_calendarPriceList.SetEnabled(codeDocumentType != "19" && codeDocumentType != "21");
    var tabPurchase = tabControl.GetTab(1);//"tabPurchase"
    var tabSale = tabControl.GetTab(2);//"tabSale"
    if(codeDocumentType == "19")//COTIZACIÓN - COMPRA
    {
        //tabPurchase.Activar y tabSale.Desactivar
        tabPurchase.SetVisible(true);
        tabSale.SetVisible(false);
        UpdatePriceListBase(id_priceListBasePurchase, result.priceListBase);
        id_priceListBaseSale.ClearItems();
        //var isVisible = s.GetChecked();
        //tab.SetVisible(isVisible);
    } else {
        if (codeDocumentType == "21")//COTIZACIÓN - VENTA
        {
            //tabPurchase.Desactivar y tabSale.Activar
            tabPurchase.SetVisible(false);
            tabSale.SetVisible(true);
            UpdatePriceListBase(id_priceListBaseSale, result.priceListBase);
            id_priceListBasePurchase.ClearItems();
        } else {
            //tabPurchase.Desactivar y tabSale.Desactivar
            tabPurchase.SetVisible(false);
            tabSale.SetVisible(false);
            id_priceListBasePurchase.ClearItems();
            id_priceListBaseSale.ClearItems();
        }
    }
    gvPriceListDetails.UnselectRows();
    gvPriceListDetails.PerformCallback();
}

function init() {
    //var id_priceListAux = $("#id_priceListAux").val();
    //var codeState = $("#codeState").val();
    //if (codeState == "07")//PENDIENTE DE PAGO
    //{
    //    UpdateProductionLotPaymentDetails(id_priceListAux);
    //}
    
}

$(function () {

    init();
});