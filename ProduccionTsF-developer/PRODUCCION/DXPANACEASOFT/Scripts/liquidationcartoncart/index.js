//Validation 

function OnValidation(s, e) {
    e.isValid = true;
}

function OnRangeEmissionDateValidation(s, e) {
    OnRangeDateValidation(e, startEmissionDate.GetValue(), endEmissionDate.GetValue(), "Rango de Fecha no válido");
}

function OnRangeDateDispatchValidation(s, e) {
    OnRangeDateValidation(e, startDateDispatch.GetValue(), endDateDispatch.GetValue(), "Rango de Fecha no válido");
}

function OnRangetPlantExitDateValidation(s, e) {
    OnRangeDateValidation(e, startPlantExitDate.GetValue(), endPlantExitDate.GetValue(), "Rango de Fecha no válido");
}

function OnRangetPlantEntryDateValidation(s, e) {
    OnRangeDateValidation(e, startPlantEntryDate.GetValue(), endPlantEntryDate.GetValue(), "Rango de Fecha no válido");
}

// Filter Action Buttons
function OnClickSearchLiquidationCartOnCart() {
    var data = $("#LiquidationCartOnCartFilterForm").serialize();

    if (data != null) {
        $.ajax({
            url: "LiquidationCartOnCart/LiquidationCartOnCartResultsPartial",
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

function OnClickClearFiltersLiquidationCartOnCart() {

    ////Customer
    //customers.ClearTokenCollection();
    //identification.SetText("");
    //consignees.ClearTokenCollection();
    //

    //Document
    DocumentStateCombo_Init();
    number.SetText("");

    //Emission
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);

    //Emission guide
    startDateDispatch.SetDate(null);
    endDateDispatch.SetDate(null);

    noLote.SetText("");
    processes.ClearTokenCollection();
    providers.ClearTokenCollection();

    //startPlantExitDate.SetDate(null);
    //endPlantExitDate.SetDate(null);

    //startPlantEntryDate.SetDate(null);
    //endPlantEntryDate.SetDate(null);
    ////Shipment
    //startDateShipment.SetDate(null);
    //endDateShipment.SetDate(null);
    //id_shippingAgencys.ClearTokenCollection();
    //id_portDischarges.ClearTokenCollection();
    //id_portDestinations.ClearTokenCollection();
    //BLNumber.SetText("");

    //ResponsableCombo_Init();
    //FreezerWarehouseCombo_Init();
    //ids_freezerWarehouseLocation.ClearTokenCollection();
    //FreezerWarehouseCombo_Init();
    //MaintenanceWarehouseCombo_Init();
    //MaintenanceWarehouseLocationCombo_Init();
    
}

function AddNewItemFromProductionLot(s, e) {
    //var data = {
    //    id: 0,
    //    id_remissionGuide: 1305
    //};

    //showPage("ReceptionDispatchMaterials/ReceptionDispatchMaterialsFormEditPartial", data);
    $.ajax({
        url: "LiquidationCartOnCart/ProductionLotsResults",
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

function GenerateLiquidationCartOnCart(s, e) {

    gridMessageErrorProductionLot.SetText("");
    $("#GridMessageErrorProductionLot").hide();
    showLoading();

    gvProductionLots.GetSelectedFieldValues("id", function (values) {
        var selectedRows = [];
        // 
        var errorMessage = "";
        var enabledbtnGenerateLot = true;
        if (values.length != 1) {
            errorMessage = "Solo debe selecionar un Lote de Producción";
            enabledbtnGenerateLot = false;
            //break;
        } 

        //btnGenerateLot.SetEnabled(enabledbtnGenerateLot);
        if (enabledbtnGenerateLot) {
            var data = {
                id : 0,
                id_productionLot: values[0]
            };

            showPage("LiquidationCartOnCart/LiquidationCartOnCartFormEditPartial", data);
        } else {
            var msgErrorAux = ErrorMessage(errorMessage);
            gridMessageErrorProductionLot.SetText(msgErrorAux);
            $("#GridMessageErrorProductionLot").show();
            hideLoading();
        }
    });
}

// GRIDVIEW PURCHASE ORDER RESULTS SELECTION

function ProductionLotsOnGridViewInit(s, e) {
    ProductionLotsUpdateTitlePanel();
}

function ProductionLotsOnGridViewSelectionChanged(s, e) {
    ProductionLotsUpdateTitlePanel();
}

function ProductionLotsOnGridViewEndCallback() {
    ProductionLotsUpdateTitlePanel();
}

function ProductionLotsUpdateTitlePanel() {
    var selectedFilteredRowCount = ProductionLotsGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvProductionLots.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductionLots.GetSelectedRowCount() - selectedFilteredRowCount;
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //console.log(gvPurchaseOrderDetails.GetSelectedRowCount());

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvProductionLots.GetSelectedRowCount() > 0 && gvProductionLots.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductionLots.GetSelectedRowCount() > 0);
    //}

    btnGenerateLiquidationCartOnCart.SetEnabled(gvProductionLots.GetSelectedRowCount() == 1);
}

function ProductionLotsGetSelectedFilteredRowCount() {
    return gvProductionLots.cpFilteredRowCountWithoutPage + gvProductionLots.GetSelectedKeysOnPage().length;
}

function ProductionLotsSelectAllRow() {
    gvProductionLots.SelectRows();
}

function ProductionLotsClearSelection() {
    gvProductionLots.UnselectRows();
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
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvLiquidationCartOnCart.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvLiquidationCartOnCart.GetSelectedRowCount() - selectedFilteredRowCount;

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvLiquidationCartOnCart.GetSelectedRowCount() > 0 && gvLiquidationCartOnCart.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvLiquidationCartOnCart.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvLiquidationCartOnCart.cpFilteredRowCountWithoutPage + gvLiquidationCartOnCart.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvResultsClearSelection() {
    gvLiquidationCartOnCart.UnselectRows();
}

function gvResultsSelectAllRows() {
    gvLiquidationCartOnCart.SelectRows();
}

// Results GridView Acction Buttons

function PerformDocumentAction(url) {

}

//btnNew
function AddNewDocument(s, e) {
    //OnClickAddNewInvoiceCommercial(s, e);
	AddNewItemFromProductionLot(s, e);
    
}

//btnCopy
function CopyDocument(s, e) {

}

//btnApprove
function ApproveDocuments(s, e) {

}

//btnAutorize
function AutorizeDocuments(s, e) {
}

//btnProtect
function ProtectDocuments(s, e) {

}

//btnCancel
function CancelDocuments(s, e) {

}

//btnRevert
function RevertDocuments(s, e) {
}

//btnHistory
function ShowHistory(s, e) {

}

//btnPrint
function Print(s, e) {

}

function OnClickUpdateLiquidationCartOnCart(s, e) {
    var data = {
        id: gvLiquidationCartOnCart.GetRowKey(e.visibleIndex)
    };

    showPage("LiquidationCartOnCart/LiquidationCartOnCartFormEditPartial", data);
    

}

function ChangeState(trx) {

}

// DETAILS VIEW CALLBACKS

function LiquidationCartOnCartDetails_BeginCallback(s, e) {
    e.customArgs["id_liquidationCartOnCart"] = s.cpIdLiquidationCartOnCart;
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
