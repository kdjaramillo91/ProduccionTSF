// FILTERS FORM ACTIONS
function AddNewItemFromPurchaseOrder(s, e) {
    $.ajax({
        url: "PurchasePlanning/PurchaseOrderDetailsResults",
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

        showPage("PurchasePlanning/PurchasePlanningFormEditPartial", data);
    });
}

// Filter Action Buttons
function OnClickSearchPurchasePlanning() {
    var data = $("#PurchasePlanningFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "PurchasePlanning/PurchasePlanningResultsPartial",
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

function OnClickClearFiltersPurchasePlanning() {
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

function ButtonManualAddNewPurchasePlanning_Click() {
    var data = {
        id: 0
    };
    showPage("PurchasePlanning/PurchasePlanningFormEditPartial", data);
}

function OnClickAddNewPurchasePlanning(trx) {

    ButtonManualAddNewPurchasePlanning_Click();
    //if (trx === "M")
    //showPage("PurchasePlanning/PurchasePlanningFormEditPartial", data);
}

// Filter ComboBox
function DocumentStateCombo_Init() {
    id_documentState.SetValue(-1);
    id_documentState.SetText("");
}

function PurchasePlanningPeriodCombo_Init() {
    id_purchasePlanningPeriod.SetValue(-1);
    id_purchasePlanningPeriod.SetText("");
}

function PersonPlanningCombo_Init() {
    filterPersonPlanning.SetValue(-1);
    filterPersonPlanning.SetText("");
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

    var text = "Total de elementos seleccionados: <b>" + gvPurchasePlannings.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchasePlannings.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPurchasePlannings.GetSelectedRowCount() > 0 && gvPurchasePlannings.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchasePlannings.GetSelectedRowCount() > 0);
    //}

    btnCopy.SetEnabled(gvPurchasePlannings.GetSelectedRowCount() == 1);
    btnApprove.SetEnabled(gvPurchasePlannings.GetSelectedRowCount() > 0);
    btnAutorize.SetEnabled(gvPurchasePlannings.GetSelectedRowCount() > 0);
    btnProtect.SetEnabled(gvPurchasePlannings.GetSelectedRowCount() > 0);
    btnCancel.SetEnabled(gvPurchasePlannings.GetSelectedRowCount() > 0);
    btnRevert.SetEnabled(gvPurchasePlannings.GetSelectedRowCount() > 0);
    btnHistory.SetEnabled(gvPurchasePlannings.GetSelectedRowCount() == 1);
    //btnPrint.SetEnabled(gvPurchasePlannings.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvPurchasePlannings.cpFilteredRowCountWithoutPage + gvPurchasePlannings.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvPurchasePlannings.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvPurchasePlannings.SelectRows();
}

// Results GridView Acction Buttons

function PerformDocumentAction(url) {
    gvPurchasePlannings.GetSelectedFieldValues("id", function (values) {

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
                gvPurchasePlannings.PerformCallback();
                // gvPurchaseOrders.UnselectRows();
            }
        });

    });
}

//btnNew
function AddNewDocument(s, e) {
    OnClickAddNewPurchasePlanning(s, e);
}

//btnCopy
function CopyDocument(s, e) {
    gvPurchasePlannings.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("PurchasePlanning/PurchasePlanningCopy", { id: values[0] });
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
        PerformDocumentAction("PurchasePlanning/ApproveDocuments");
    }, "¿Desea aprobar los lotes seleccionados?");
}

//btnAutorize
function AutorizeDocuments(s, e) {
    //var c = confirm("¿Desea autorizar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Auth");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchasePlanning/AutorizeDocuments");
    }, "¿Desea autorizar los lotes seleccionados?");
}

//btnProtect
function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchasePlanning/ProtectDocuments");
    }, "¿Desea cerrar los lotes seleccionados?");
}

//btnCancel
function CancelDocuments(s, e) {
    //var c = confirm("¿Desea anular los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Canc");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchasePlanning/CancelDocuments");
    }, "¿Desea anular los lotes seleccionados?");
}

//btnRevert
function RevertDocuments(s, e) {
    //var c = confirm("¿Desea reversar los documentos seleccionados?");
    //if (c === true) {
    //    ChangeState("Rev");
    //}
    showConfirmationDialog(function () {
        PerformDocumentAction("PurchasePlanning/RevertDocuments");
    }, "¿Desea reversar los lotes seleccionados?");
}

//btnHistory
function ShowHistory(s, e) {

}

//btnPrint
function Print(s, e) {
    gvPurchasePlannings.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "PurchasePlanning/PurchasePlanningReport",
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

function OnClickUpdatePurchasePlanning(s, e) {

    var data = {
        id: gvPurchasePlannings.GetRowKey(e.visibleIndex)
    };

    showPage("PurchasePlanning/PurchasePlanningFormEditPartial", data);
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

function PurchasePlanningDetailItems_BeginCallback(s, e) {
    e.customArgs["id_purchasePlanning"] = s.cpIdPurchasePlanning;
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
