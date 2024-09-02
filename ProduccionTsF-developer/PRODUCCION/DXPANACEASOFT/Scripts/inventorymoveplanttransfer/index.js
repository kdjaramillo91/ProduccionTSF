
function ComboBoxWarehouseLocationEntryIndex_BeginCallback(s, e) {
    e.customArgs["id_warehouseEntry"] = ComboBoxWarehouseEntryIndex.GetValue() === undefined ? null : ComboBoxWarehouseEntryIndex.GetValue();
}

function ComboBoxWarehouseEntryIndex_SelectedIndexChanged(s, e) {
    ComboBoxWarehouseLocationEntryIndex.PerformCallback();
}

function SearchClick() {

    $("#btnCollapse").click();

    var dateInicio = DateEditInit.GetDate();
    var yearInicio = dateInicio.getFullYear();
    var monthInicio = dateInicio.getMonth() + 1;
    var dayInicio = dateInicio.getDate();

    var dateFin = DateEditEnd.GetDate();
    var yearFin = dateFin.getFullYear();
    var monthFin = dateFin.getMonth() + 1;
    var dayFin = dateFin.getDate();

    var data = {
        initDate: dayInicio + "/" + monthInicio + "/" + yearInicio,
        endtDate: dayFin + "/" + monthFin + "/" + yearFin,
        id_state: ComboBoxState.GetValue(),
        number: TextBoxNumber.GetText(),
        reference: TextBoxReference.GetText(),
        id_warehouseEntry: ComboBoxWarehouseEntryIndex.GetValue(),
        id_warehouseLocationEntry: ComboBoxWarehouseLocationEntryIndex.GetValue(),
        id_inventoryReason: ComboBoxInventoryReasonIndex.GetValue(),
        id_receiver: ComboBoxReceiverIndex.GetValue(),
        numberLot: TextBoxNumberLot.GetText(),
        secTransaction: TextBoxSecTransaction.GetText(),
        id_processType: ComboBoxProcessTypeIndex.GetValue(),
        id_provider: ComboBoxProviderIndex.GetValue(),
        id_machineForProd: ComboBoxMachineForProdIndex.GetValue(),
        id_turn: ComboBoxTurnIndex.GetValue(),
        id_productionCart: ComboBoxProductionCartIndex.GetValue()
    };
    
    showPartialPage($("#grid"), 'InventoryMovePlantTransfer/SearchResult', data);
}

function ClearClick() {
    var dayNow = new Date();
    DateEditInit.SetDate(new Date(dayNow.getFullYear(), dayNow.getMonth(), 1));
    DateEditEnd.SetDate(dayNow);
    ComboBoxState.SetValue(null);
    TextBoxNumber.SetText(null);
    TextBoxReference.SetText(null);
    ComboBoxWarehouseEntryIndex.SetValue(null);
    ComboBoxWarehouseLocationEntryIndex.SetValue(null);
    ComboBoxInventoryReasonIndex.SetValue(null);
    ComboBoxReceiverIndex.SetValue(null);
    TextBoxNumberLot.SetText(null);
    TextBoxSecTransaction.SetText(null);
    ComboBoxProcessTypeIndex.SetValue(null);
    ComboBoxProviderIndex.SetValue(null);
    ComboBoxMachineForProdIndex.SetValue(null);
    ComboBoxTurnIndex.SetValue(null);
    ComboBoxProductionCartIndex.SetValue(null);
}

function NewClick() {
    $.ajax({
        url: "InventoryMovePlantTransfer/PendingNew",
        type: "post",
        data: data,
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

function Init() {
    $("#btnCollapse").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
            $("#grid").css("display", "");
        } else {
            $("#grid").css("display", "none");
        }
    });
}

$(function () {
    Init();
});

function OnClickSearchReportExcel(isPdf) {

    var dateInicio = DateEditInit.GetDate();
    var dateFin = DateEditEnd.GetDate();
    var warehouse = ComboBoxWarehouseEntryIndex.GetValue();
    var warehouseLocation = ComboBoxWarehouseLocationEntryIndex.GetValue();

    if (dateInicio == null) {
        NotifyWarning("Fecha de Inicio no puede estar vacía");
        return;
    }

    if (dateFin == null) {
        NotifyWarning("Fecha de Fin no puede estar vacía");
        return;
    }

    if (warehouse == null) {
        NotifyWarning("Debe seleccionar una bodega");
        return;
    }

    if (warehouseLocation == null) {
        NotifyWarning("Debe seleccionar una Ubicación");
        return;
    }

    var data = "startDate=" + dateInicio.toISOString() + "&endDate=" + dateFin.toISOString() + "&idWarehouse=" + warehouse + "&idarehouseLocation=" + warehouseLocation;

    if (data != null) {
        $.ajax({
            url: "InventoryMovePlantTransfer/DownloadReportPlantTransfer",
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
                try {
                    if (result != undefined) {
                        var url = 'ReportProd/DownloadExcelPlantTransfer?pdf=' + isPdf;
                        newWindow = window.open(url, '_self', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
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





