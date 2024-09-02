
// FORM FILTER BUTTONS

function OnClickSearchKardexs(s, e) {

    var data = $("#formFilterKardex").serialize();

    //var itemDocumentType = documentType.GetSelectedItem();
    //if(itemDocumentType != null && itemDocumentType != undefined) {
    //    data = "id_documentType=" + itemDocumentType.value + "&" + data;
    //}

    $.ajax({
        url: "Kardex/KardexResults",
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

function OnClickClearFiltersKardex() {
    DocumentTypeCombo_Init();
    //DocumentStateCombo_Init();
    number.SetText("");
    reference.SetText("");
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
    WarehouseExitCombo_Init();
    WarehouseLocationExitCombo_Init();
    DispatcherCombo_Init();
    WarehouseEntryCombo_Init();
    WarehouseLocationEntryCombo_Init();
    ReceiverCombo_Init();
    InventoryReasonCombo_Init();
    items.ClearTokenCollection();
    items.SetValue(null);
    numberLot.SetText("");
    internalNumberLot.SetText("");
}

function InventoryReasonCombo_Init() {
    id_inventoryReason.SetValue(null);
    id_inventoryReason.SetText("");
}

function DocumentTypeCombo_Init() {
    id_documentType.SetValue(null);
    id_documentType.SetText("");
}

function DocumentStateCombo_Init() {
    id_documentState.SetValue(null);
    id_documentState.SetText("");
}

function WarehouseExitCombo_Init() {
    id_warehouseExit.SetValue(null);
    id_warehouseExit.SetText("");
}

function WarehouseLocationExitCombo_Init() {
    id_warehouseLocationExit.SetValue(null);
    id_warehouseLocationExit.SetText("");
}

function DispatcherCombo_Init() {
    id_dispatcher.SetValue(null);
    id_dispatcher.SetText("");
}

function WarehouseEntryCombo_Init() {
    id_warehouseEntry.SetValue(null);
    id_warehouseEntry.SetText("");
}

function WarehouseLocationEntryCombo_Init() {
    id_warehouseLocationEntry.SetValue(null);
    id_warehouseLocationEntry.SetText("");
}

function ReceiverCombo_Init() {
    id_receiver.SetValue(null);
    id_receiver.SetText("");
}

function init() {
    $("#btnCollapse").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
            $("#results").css("display", "");
        } else {
            $("#results").css("display", "none");
        }
    });
}

$(function() {
    init();
});