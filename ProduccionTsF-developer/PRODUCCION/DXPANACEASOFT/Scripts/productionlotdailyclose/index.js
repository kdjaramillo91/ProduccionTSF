// FILTERS FORM ACTIONS
function AddNewItemFromPurchaseOrder(s, e) {
    $.ajax({
        url: "ProductionLotDailyClose/PurchaseOrderDetailsResults",
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

    //console.log(gvPurchaseOrderDetails.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPurchaseOrderDetails.GetSelectedRowCount() > 0 && gvPurchaseOrderDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseOrderDetails.GetSelectedRowCount() > 0);
    //}

    btnGenerateDocument.SetEnabled(gvPurchaseOrderDetails.GetSelectedRowCount() > 0);
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

function GenerateDocument(s, e) {

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
            ids_purchaseOrdersDetails: selectedRows
        };

        showPage("ProductionLotDailyClose/ProductionLotDailyCloseFormEditPartial", data);
    });
}

// Filter Action Buttons
function OnClickSearchProductionLotDailyClose() {
    var data = $("#ProductionLotDailyCloseFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "ProductionLotDailyClose/ProductionLotDailyCloseResultsPartial",
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

function OnClickClearFiltersProductionLotDailyClose() {
    DocumentStateCombo_Init(s, e);
    //ProviderCombo_Init(s, e);
    number.SetText("");
    reference.SetText("");
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
    PersonClosingCombo_Init(s, e);
    //filterStartAuthorizationDate.SetText("");
    //filterEndAuthorizationDate.SetText("");
    //filterAuthorizationNumber.SetText("");
    //filterAccessKey.SetText("");
    //filterItem.ClearTokenCollection();
}

function ButtonManualAddNewProductionLotDailyClose_Click() {
    var data = {
        id: 0
    };
    showPage("ProductionLotDailyClose/ProductionLotDailyCloseFormEditPartial", data);
}

function OnClickAddNewProductionLotDailyClose(trx) {

    ButtonManualAddNewProductionLotDailyClose_Click();
    //if (trx === "M")
    //showPage("ProductionLotDailyClose/ProductionLotDailyCloseFormEditPartial", data);
}

// Filter ComboBox
function DocumentStateCombo_Init() {
    id_documentState.SetValue(-1);
    id_documentState.SetText("");
}

function PersonClosingCombo_Init() {
    id_personClosing.SetValue(-1);
    id_personClosing.SetText("");
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
    //console.log("Estoy en el UpdateTitlePanel del Index");
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLotDailyCloses.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLotDailyCloses.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvProductionLotDailyCloses.GetSelectedRowCount() > 0 && gvProductionLotDailyCloses.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductionLotDailyCloses.GetSelectedRowCount() > 0);
    //}

    //btnCopy.SetEnabled(gvProductionLotDailyCloses.GetSelectedRowCount() == 1);
    //btnApprove.SetEnabled(gvProductionLotDailyCloses.GetSelectedRowCount() > 0);
    //btnAutorize.SetEnabled(gvProductionLotDailyCloses.GetSelectedRowCount() > 0);
    //btnProtect.SetEnabled(gvProductionLotDailyCloses.GetSelectedRowCount() > 0);
    //btnCancel.SetEnabled(gvProductionLotDailyCloses.GetSelectedRowCount() > 0);
    //btnRevert.SetEnabled(gvProductionLotDailyCloses.GetSelectedRowCount() > 0);
    //btnHistory.SetEnabled(gvProductionLotDailyCloses.GetSelectedRowCount() == 1);
    //btnPrint.SetEnabled(gvProductionLotDailyCloses.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvProductionLotDailyCloses.cpFilteredRowCountWithoutPage + gvProductionLotDailyCloses.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvProductionLotDailyCloses.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvProductionLotDailyCloses.SelectRows();
}

// Results GridView Acction Buttons

function PerformDocumentAction(url) {
    gvProductionLotDailyCloses.GetSelectedFieldValues("id", function (values) {

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
                console.log(result);
            },
            complete: function () {
                //hideLoading();
                gvProductionLotDailyCloses.PerformCallback();
                // gvPurchaseOrders.UnselectRows();
            }
        });

    });
}

//btnNew
function AddNewDocument(s, e) {
    OnClickAddNewProductionLotDailyClose(s, e);
}

//btnCopy
function CopyDocument(s, e) {
    gvProductionLotDailyCloses.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("ProductionLotDailyClose/ProductionLotDailyCloseCopy", { id: values[0] });
        }
    });
}

//btnApprove
function ApproveDocuments(s, e) {
    //var c = confirm("¿Desea aprobar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Appr");
    //}

    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotDailyClose/ApproveDocuments");
    }, "¿Desea aprobar los lotes seleccionados?");
}

//btnAutorize
function AutorizeDocuments(s, e) {
    //var c = confirm("¿Desea autorizar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Auth");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotDailyClose/AutorizeDocuments");
    }, "¿Desea autorizar los lotes seleccionados?");
}

//btnProtect
function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotDailyClose/ProtectDocuments");
    }, "¿Desea cerrar los lotes seleccionados?");
}

//btnCancel
function CancelDocuments(s, e) {
    //var c = confirm("¿Desea anular los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Canc");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotDailyClose/CancelDocuments");
    }, "¿Desea anular los lotes seleccionados?");
}

//btnRevert
function RevertDocuments(s, e) {
    //var c = confirm("¿Desea reversar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Rev");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("ProductionLotDailyClose/RevertDocuments");
    }, "¿Desea reversar los lotes seleccionados?");
}

//btnHistory
function ShowHistory(s, e) {

}

//btnPrint
function Print(s, e) {
    gvProductionLotDailyCloses.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "ProductionLotDailyClose/ProductionLotDailyCloseReport",
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

function OnClickUpdateProductionLotDailyClose(s, e) {

    var data = {
        id: gvProductionLotDailyCloses.GetRowKey(e.visibleIndex)
    };

    showPage("ProductionLotDailyClose/ProductionLotDailyCloseFormEditPartial", data);
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

function ProductionLotDailyCloseDetailProductionLots_BeginCallback(s, e) {
    e.customArgs["id_productionLotDailyClose"] = s.cpIdproductionLotDailyClose;
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
