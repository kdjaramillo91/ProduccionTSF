//Validation 

function OnValidation(s, e) {
    e.isValid = true;
}

function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
}


// Filter Action Buttons
function OnClickSearchMachineProdOpening() {
    var data = $("#MachineProdOpeningFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "MachineProdOpening/MachineProdOpeningResultsPartial",
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

function OnClickClearFiltersMachineProdOpening() {

    //Document
    DocumentStateCombo_Init();
    number.SetText("");

    //Emission
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);

    //Machine Prod Opening
    turns.ClearTokenCollection();
    machineForProds.ClearTokenCollection();
    persons.ClearTokenCollection();
    
}

function AddNewItemMachineProdOpening(s, e) {
    var data = {
        id: 0
    };

    showPage("MachineProdOpening/MachineProdOpeningFormEditPartial", data);
    //$.ajax({
    //    url: "ReceptionDispatchMaterials/RemissionGuidesResults",
    //    type: "post",
    //    async: true,
    //    cache: false,
    //    error: function (error) {
    //        console.log(error);
    //    },
    //    beforeSend: function () {
    //        showLoading();
    //    },
    //    success: function (result) {
    //        $("#btnCollapse").click();
    //        $("#results").html(result);
    //    },
    //    complete: function () {
    //        hideLoading();
    //    }
    //});
}

function ButtonManualAddNewInvoiceCommercial_Click() {
    var data = {
        id: 0
    };
    showPage("InvoiceCommercial/InvoiceCommercialFormEditPartial", data);
}

function OnClickAddNewFromRemisionGuide(trx) {
ButtonManualAddNewOpeningClosingPlateLying_Click();
    
    //if (trx === "M")
    //showPage("ProductionLotDailyClose/ProductionLotDailyCloseFormEditPartial", data);
}

// Filter ComboBox
function DocumentStateCombo_Init() {
    id_documentState.SetValue(null);
    //id_documentState.SetText("");
}

// GRIDVIEW PURCHASE ORDER RESULTS ACTIONS

function GenerateReceptionDispatchMaterials(s, e) {

    gridMessageErrorRemisionGuide.SetText("");
    $("#GridMessageErrorRemisionGuide").hide();
    showLoading();

    gvRemissionGuides.GetSelectedFieldValues("id", function (values) {
        var selectedRows = [];
       
        var errorMessage = "";
        var enabledbtnGenerateLot = true;
        if (values.length != 1) {
            errorMessage = "Solo debe selecionar una Guia de Remisión";
            enabledbtnGenerateLot = false;
            //break;
        } 

        //btnGenerateLot.SetEnabled(enabledbtnGenerateLot);
        if (enabledbtnGenerateLot) {
            var data = {
                id : 0,
                id_remissionGuide: values[0]
            };

            showPage("ReceptionDispatchMaterials/ReceptionDispatchMaterialsFormEditPartial", data);
        } else {
            var msgErrorAux = ErrorMessage(errorMessage);
            gridMessageErrorRemisionGuide.SetText(msgErrorAux);
            $("#GridMessageErrorRemisionGuide").show();
            hideLoading();
        }
    });
}

// GRIDVIEW PURCHASE ORDER RESULTS SELECTION

function RemissionGuidesOnGridViewInit(s, e) {
    RemissionGuidesUpdateTitlePanel();
}

function RemissionGuidesOnGridViewSelectionChanged(s, e) {
    RemissionGuidesUpdateTitlePanel();
}

function RemissionGuidesOnGridViewEndCallback() {
    RemissionGuidesUpdateTitlePanel();
}

function RemissionGuidesUpdateTitlePanel() {
    var selectedFilteredRowCount = RemissionGuidesGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRemissionGuides.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRemissionGuides.GetSelectedRowCount() - selectedFilteredRowCount;
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvPurchaseOrderDetails.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvRemissionGuides.GetSelectedRowCount() > 0 && gvRemissionGuides.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRemissionGuides.GetSelectedRowCount() > 0);
    //}

    btnGenerateReceptionDispatchMaterials.SetEnabled(gvRemissionGuides.GetSelectedRowCount() == 1);
}

function RemissionGuidesGetSelectedFilteredRowCount() {
    return gvRemissionGuides.cpFilteredRowCountWithoutPage + gvRemissionGuides.GetSelectedKeysOnPage().length;
}

function RemissionGuidesSelectAllRow() {
    gvRemissionGuides.SelectRows();
}

function RemissionGuidesClearSelection() {
    gvRemissionGuides.UnselectRows();
}

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

    var text = "Total de elementos seleccionados: <b>" + gvMachineProdOpening.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvMachineProdOpening.GetSelectedRowCount() - selectedFilteredRowCount;

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvMachineProdOpening.GetSelectedRowCount() > 0 && gvMachineProdOpening.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvMachineProdOpening.GetSelectedRowCount() > 0);
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
    return gvMachineProdOpening.cpFilteredRowCountWithoutPage + gvMachineProdOpening.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvMachineProdOpening.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvMachineProdOpening.SelectRows();
}

// Results GridView Acction Buttons

function PerformDocumentAction(url) {
    gvInvoiceCommercials.GetSelectedFieldValues("id", function (values) {

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
                gvInvoiceCommercials.PerformCallback();
                // gvPurchaseOrders.UnselectRows();
            }
        });

    });
}

//btnNew
function AddNewDocument(s, e) {
    //OnClickAddNewInvoiceCommercial(s, e);
    AddNewItemFromRemisionGuide(s, e);
    
}

//btnCopy
function CopyDocument(s, e) {
    gvInvoiceCommercials.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("InvoiceCommercial/InvoiceCommercialCopy", { id: values[0] });
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
        PerformDocumentAction("ReceptionDispatchMaterials/AutorizeDocuments");
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

function OnClickUpdateMachineProdOpening(s, e) {
    var data = {
        id: gvMachineProdOpening.GetRowKey(e.visibleIndex)
    };

    showPage("MachineProdOpening/MachineProdOpeningFormEditPartial", data);
    

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

function MachineProdOpeningDetails_BeginCallback(s, e) {
    e.customArgs["id_machineProdOpening"] = s.cpIdMachineProdOpening;
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
