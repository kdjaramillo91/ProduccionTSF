
// VALIDATIONS
//var Message = "Ok";

//function OnPriceListValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    } else {
//        UpdateProductionLotPaymentDetails(s.GetValue());
//        if (Message != "Ok") {
//            e.isValid = false;
//            e.errorText = Message;
//        }
//    }
//}

function UpdatePriceListInventoryLines(inventoryLinesParam) {

    //for (var i = 0; i < inventoryLines.GetItemCount() ; i++) {
    //    inventoryLines.SetValue();
    //    //var priceList = id_priceList.GetItem(i);
    //    //var into = false;
    //    //for (var j = 0; j < priceLists.length; j++) {
    //    //    if (priceList.value == priceLists[j].id) {
    //    //        into = true;
    //    //        break;
    //    //    }
    //    //}
    //    //if (!into) {
    //    //    id_priceList.RemoveItem(i);
    //    //    i -= 1;
    //    //}
    //}
    inventoryLines.SetValue(inventoryLinesParam);
    //if (inventoryLinesParam == null) return;
    //for (var i = 0; i < inventoryLinesParam.length; i++) {
    //    var inventoryLine = inventoryLines.FindItemByValue(inventoryLinesParam[i]);
    //    if (inventoryLine != null) inventoryLines.SetValue(inventoryLinesParam[i]);
    //}

}

function TokenInventoryLine_Init(s, e) {
    //var data = {
    //    id_priceList: s.GetValue(),
    //    id_provider: id_provider.GetValue()
    //};

    $.ajax({
        url: "PriceList/GetPriceListInventoryLinesFilter",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdatePriceListInventoryLines(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
              UpdatePriceListInventoryLines(result.inventoryLines);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function TokenInventoryLine_ValueChanged(s, e) {
    inventoryLinesAux = s.GetValue();
    var data = {
        inventoryLines: inventoryLinesAux.split(",")
    };
    //var data = {
    //    inventoryLines: s.GetValue()
    //};

    $.ajax({
        url: "PriceList/UpdatePriceListDetail",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //UpdatePriceListInventoryLines(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //UpdatePriceListInventoryLines(result.inventoryLines);
            gvPriceListDetails.UnselectRows();
            gvPriceListDetails.PerformCallback();
        },
        complete: function () {
            //hideLoading();
        }
    });

    
    //UpdateProductionLotPaymentDetails(s.GetValue());
    //OnPriceListValidation(s, e);
    //console.log("s.GetValue() : ");
    //console.log(s.GetValue());
}

function UpdatePriceListItemTypes(itemTypesParam) {
    itemTypes.SetValue(itemTypesParam);
    //if (itemTypesParam == null) return;
    //for (var i = 0; i < itemTypesParam.length; i++) {
    //    var itemType = itemTypes.FindItemByValue(itemTypesParam[i]);
    //    if (itemType != null) itemTypes.SetValue(itemTypesParam[i]);
    //}

}

function TokenItemType_Init(s, e) {
   
    $.ajax({
        url: "PriceList/GetPriceListItemTypesFilter",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdatePriceListItemTypes(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            UpdatePriceListItemTypes(result.itemTypes);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function TokenItemType_ValueChanged(s, e) {
    itemTypesAux = s.GetValue();
    var data = {
        itemTypes: itemTypesAux.split(",")
    };
    //var data = {
    //    itemTypes: s.GetValue()
    //};

    $.ajax({
        url: "PriceList/UpdatePriceListDetail",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //UpdatePriceListInventoryLines(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //UpdatePriceListInventoryLines(result.inventoryLines);
            gvPriceListDetails.UnselectRows();
            gvPriceListDetails.PerformCallback();
        },
        complete: function () {
            //hideLoading();
        }
    });


}

function UpdatePriceListItemGroups(itemGroupsParam) {
    itemGroups.SetValue(itemGroupsParam);
    //if (itemGroupsParam == null) return;
    //for (var i = 0; i < itemGroupsParam.length; i++) {
    //    var itemGroup = itemGroups.FindItemByValue(itemGroupsParam[i]);
    //    if (itemGroup != null) itemGroups.SetValue(itemGroupsParam[i]);
    //}

}

function TokenItemGroup_Init(s, e) {

    $.ajax({
        url: "PriceList/GetPriceListItemGroupsFilter",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdatePriceListItemGroups(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            UpdatePriceListItemGroups(result.itemGroups);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function TokenItemGroup_ValueChanged(s, e) {
    itemGroupsAux = s.GetValue();
    var data = {
        itemGroups: itemGroupsAux.split(",")
    };
    //var data = {
    //    itemGroups: s.GetValue()
    //};

    $.ajax({
        url: "PriceList/UpdatePriceListDetail",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //UpdatePriceListInventoryLines(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //UpdatePriceListInventoryLines(result.inventoryLines);
            gvPriceListDetails.UnselectRows();
            gvPriceListDetails.PerformCallback();
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function UpdatePriceListFilterShows(filterShowsParam) {
    filterShows.SetValue(filterShowsParam);
    //if (filterShowsParam == null) return;
    //for (var i = 0; i < filterShowsParam.length; i++) {
    //    var filterShow = filterShows.FindItemByValue(filterShowsParam[i]);
    //    if (filterShow != null) filterShows.SetValue(filterShowsParam[i]);
    //}

}

function TokenFilterShow_Init(s, e) {

    $.ajax({
        url: "PriceList/GetPriceListFilterShow",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdatePriceListFilterShows(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            UpdatePriceListFilterShows(result.filterShows);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function TokenFilterShow_ValueChanged(s, e) {
    //console.log("s.GetValue(): ");
    //console.log(s.GetValue());
    ////console.log("s.GetItem(): ");
    ////console.log(s.GetItem());
    //console.log("s.GetItemCount(): ");
    //console.log(s.GetItemCount());
    //console.log("s.GetSelectedIndex(): ");
    //console.log(s.GetSelectedIndex());
    //console.log("s.GetSelectedItem(): ");
    //console.log(s.GetSelectedItem());
    //console.log("s.GetText(): ");
    //console.log(s.GetText());
    //console.log("s.GetTokenCollection(): ");
    //console.log(s.GetTokenCollection());
    filterShowsAux = s.GetValue();
    var data = {
        filterShows: filterShowsAux.split(",")
    };

    $.ajax({
        url: "PriceList/UpdatePriceListDetailFilterShow",
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
            gvPriceListDetails.UnselectRows();
            gvPriceListDetails.PerformCallback();
        },
        complete: function () {
            //hideLoading();
        }
    });
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