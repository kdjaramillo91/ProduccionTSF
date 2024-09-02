// FILTERS FORM ACTIONS
function AddNewItemFromPurchaseOrder(s, e) {
    $.ajax({
        url: "ProductionLotReception/PurchaseOrderDetailsResults",
        type: "post",
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $("#btnCollapse").click();
            $("#results").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

// GRIDVIEW PURCHASE ORDER RESULTS SELECTION

function PurchaseOrderDetailsOnGridViewInit(s, e) {
    PurchaseOrderDetailsUpdateTitlePanel();
}

function PurchaseOrderDetailsOnGridViewSelectionChanged(s, e) {
    PurchaseOrderDetailsUpdateTitlePanel();
}

function PurchaseOrderDetailsOnGridViewEndCallback() {
    PurchaseOrderDetailsUpdateTitlePanel();
}

function PurchaseOrderDetailsUpdateTitlePanel() {
    var selectedFilteredRowCount = PurchaseOrderDetailsGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPurchaseOrderDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseOrderDetails.GetSelectedRowCount() - PurchaseOrderDetailsGetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    console.log(gvPurchaseOrderDetails.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPurchaseOrderDetails.GetSelectedRowCount() > 0 && gvPurchaseOrderDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseOrderDetails.GetSelectedRowCount() > 0);
    //}

    btnGenerateLot.SetEnabled(gvPurchaseOrderDetails.GetSelectedRowCount() > 0);
}

function PurchaseOrderDetailsGetSelectedFilteredRowCount() {
    return gvPurchaseOrderDetails.cpFilteredRowCountWithoutPage + gvPurchaseOrderDetails.GetSelectedKeysOnPage().length;
}

function PurchaseOrderDetailsSelectAllRow() {
    gvPurchaseOrderDetails.SelectRows();
}

function PurchaseOrderDetailsClearSelection() {
    gvPurchaseOrderDetails.UnselectRows();
}

// GRIDVIEW PURCHASE ORDER RESULTS ACTIONS

function GenerateLot(s, e) {

    //console.log(selectedPurchaseOrderDetailsRows);

    //var data = {
    //    id: 0,
    //    orderDetails: []
    //};
    //for (var i = 0; i < selectedPurchaseOrderDetailsRows.length; i++) {
    //    data.orderDetails.push(selectedPurchaseOrderDetailsRows[i]);
    //}

    //showPage("Logistics/FormEditRemissionGuide", data);


    gvPurchaseOrderDetails.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        var data = {
            id: 0,
            loteManual: false,
            ids_purchaseOrdersDetails: selectedRows
        };

        showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
    });
}

// Filter Action Buttons
function OnClickSearchProductionLotReception() {
    var data = $("#ProductionLotReceptionFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotReception/ProductionLotReceptionResultsPartial",
            type: "post",
            data: data,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                $("#btnCollapse").click();
                $("#results").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function OnClickClearFiltersProductionLotReception() {
    //PersonRequestingCombo_Init(s, e);
    //DocumentStateCombo_Init(s, e);
    //ProviderCombo_Init(s, e);
    //filterNumber.SetText("");
    //filterReference.SetText("");
    //filterStartEmissionDate.SetText("");
    //filterEndEmissionDate.SetText("");
    //filterStartAuthorizationDate.SetText("");
    //filterEndAuthorizationDate.SetText("");
    //filterAuthorizationNumber.SetText("");
    //filterAccessKey.SetText("");
    //filterItem.ClearTokenCollection();
}

function ButtonManualAddNewProductionLotReception_Click() {
    var data = {
        id: 0,
        loteManual: false
    };
    showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
}

function OnClickAddNewProductionLotReception(trx) {
    

    if (trx === "M")
        showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
}

// Filter ComboBox
function ProductionLotStateCombo_Init() {
    id_ProductionLotState.SetValue(-1);
    id_ProductionLotState.SetText("");
}

function ProductionUnitCombo_Init() {
    id_productionUnit.SetValue(-1);
    id_productionUnit.SetText("");
}

//function ProductionProcessCombo_Init() {
//    filterProductionProcess.SetValue(-1);
//    filterProductionProcess.SetText("");
//}

//function PersonRequestingCombo_Init(s, e) {
//    filterPersonRequesting.SetValue(-1);
//    filterPersonRequesting.SetText("");
//}

function WarehouseCombo_Init() {
    filterWarehouse.SetValue(-1);
    filterWarehouse.SetText("");
}

function WarehouseLocationCombo_Init() {
    filterWarehouseLocation.SetValue(-1);
    filterWarehouseLocation.SetText("");
} 

function PersonReceivingCombo_Init() {
    id_personReceiving.SetValue(-1);
    id_personReceiving.SetText("");
} 

//function ItemTypeCombo_Init() {
//    filterItemType.SetValue(-1);
//    filterItemType.SetText("");
//}

//function ItemTypeCategoryCombo_Init() {
//    filterItemTypeCategory.SetValue(-1);
//    filterItemTypeCategory.SetText("");
//}


// Results GridView Selection
function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

function OnGridViewEndCallback() {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotReceptions.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotReceptions.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvProductionLotReceptions.GetSelectedRowCount() > 0 && gvProductionLotReceptions.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductionLotReceptions.GetSelectedRowCount() > 0);
    //}

    //btnCopy.SetEnabled(gvProductionLotReceptions.GetSelectedRowCount() == 1);
    //btnApprove.SetEnabled(gvProductionLotReceptions.GetSelectedRowCount() > 0);
    //btnAutorize.SetEnabled(gvProductionLotReceptions.GetSelectedRowCount() > 0);
    //btnProtect.SetEnabled(gvProductionLotReceptions.GetSelectedRowCount() > 0);
    //btnCancel.SetEnabled(gvProductionLotReceptions.GetSelectedRowCount() > 0);
    //btnRevert.SetEnabled(gvProductionLotReceptions.GetSelectedRowCount() > 0);
    //btnHistory.SetEnabled(gvProductionLotReceptions.GetSelectedRowCount() == 1);
    //btnPrint.SetEnabled(gvProductionLotReceptions.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvProductionLotReceptions.cpFilteredRowCountWithoutPage + gvProductionLotReceptions.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvProductionLotReceptions.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvProductionLotReceptions.SelectRows();
}

// Results GridView Acction Buttons

function PerformDocumentAction(url) {
    gvProductionLotReceptions.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: url,
            type: "post",
            data: { ids: selectedRows },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                //console.log(result);
            },
            complete: function () {
                //hideLoading();
                gvProductionLotReceptions.PerformCallback();
                // gvPurchaseOrders.UnselectRows();
            }
        });

    });
}

//btnNew
function AddNewLot(s, e) {
    OnClickAddNewProductionLotReception(s, e);
}

//btnCopy
function CopyLot(s, e) {
    gvProductionLotReceptions.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("ProductionLotReception/ProductionLotReceptionCopy", { id: values[0] });
        }
    });
}

//btnApprove
function ApproveLots(s, e) {
    //var c = confirm("¿Desea aprobar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Appr");
    //}

    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotReception/ApproveLots");
    }, "¿Desea aprobar los lotes seleccionados?");
}

//btnAutorize
function AutorizeLots(s, e) {
    //var c = confirm("¿Desea autorizar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Auth");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotReception/AutorizeLots");
    }, "¿Desea autorizar los lotes seleccionados?");
}

//btnProtect
function ProtectLots(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotReception/ProtectLots");
    }, "¿Desea cerrar los lotes seleccionados?");
}

//btnCancel
function CancelLots(s, e) {
    //var c = confirm("¿Desea anular los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Canc");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotReception/CancelLots");
    }, "¿Desea anular los lotes seleccionados?");
}

//btnRevert
function RevertLots(s, e) {
    //var c = confirm("¿Desea reversar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Rev");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotReception/RevertLots");
    }, "¿Desea reversar los lotes seleccionados?");
}

//btnHistory
function ShowHistory(s, e) {

}

//btnPrint
function Print(s, e) {
    gvProductionLotReceptions.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "ProductionLotClose/ProductionLotCloseReport",
            type: "post",
            data: { id: selectedRows[0] },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
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

    });
}

function OnClickUpdateProductionLotReception(s, e) {

    var data = {
        id: gvProductionLotReceptions.GetRowKey(e.visibleIndex),
        loteManual: false
    };

    showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
}

function ChangeState(trx) {
    //$.ajax({
    //    url: "PurchaseRequest/ChangeStateSelectedDocuments",
    //    type: "post",
    //    data: { ids: selectedRows, trx: trx },
    //    async: true,
    //    cache: false,
    //    error: function (error) {
    //        console.log(error);
    //    },
    //    beforeSend: function () {
    //        showLoading();
    //    },
    //    success: function (result) {
    //        console.log(result);
    //    },
    //    complete: function () {
    //        gvPurchaseRequests.UnselectRows();
    //        gvPurchaseRequests.PerformCallback();
    //        hideLoading();
    //    }
    //});
}

// DETAILS VIEW CALLBACKS

function ProductionLotReceptionDetailItems_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

function ProductionLotReceptionDetailDispatchMaterials_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

function ProductionLotReceptionDetailProductionLotLiquidations_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

function ProductionLotReceptionDetailProductionLotTrashs_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

function ProductionLotReceptionDetailProductionLotQualityAnalysiss_BeginCallback(s, e) {
    e.customArgs["id_productionLot"] = s.cpIdProductionLot;
}

// Init
function init() {
    $("#btnCollapse").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
            $("#results").css("display", "");
        } else {
            $("#results").css("display", "none");
        }
    });
}

$(function () {
    init();
});
