
function ComboBoxBoxedWarehouseLocationIndex_BeginCallback(s, e) {
    e.customArgs["id_boxedWarehouse"] = ComboBoxBoxedWarehouseIndex.GetValue() === undefined ? null : ComboBoxBoxedWarehouseIndex.GetValue();
}

function ComboBoxBoxedWarehouseIndex_SelectedIndexChanged(s, e) {
    ComboBoxBoxedWarehouseLocationIndex.PerformCallback();
}

function ComboBoxMasteredWarehouseLocationIndex_BeginCallback(s, e) {
    e.customArgs["id_masteredWarehouse"] = ComboBoxMasteredWarehouseIndex.GetValue() === undefined ? null : ComboBoxMasteredWarehouseIndex.GetValue();
}

function ComboBoxMasteredWarehouseIndex_SelectedIndexChanged(s, e) {
    ComboBoxMasteredWarehouseLocationIndex.PerformCallback();
}

function DateEditEnd_Init(s, e) {
    SearchClick();
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
        id_responsable: ComboBoxResponsableIndex.GetValue(),
        id_turn: ComboBoxTurnIndex.GetValue(),
        id_boxedWarehouse: ComboBoxBoxedWarehouseIndex.GetValue(),
        id_boxedWarehouseLocation: ComboBoxBoxedWarehouseLocationIndex.GetValue(),
        boxedItems: TokenBoxBoxedItemsIndex.GetTokenValuesCollection(),
        boxedNumberLot: TextBoxBoxedNumberLot.GetText(),
        id_masteredWarehouse: ComboBoxMasteredWarehouseIndex.GetValue(),
        id_masteredWarehouseLocation: ComboBoxMasteredWarehouseLocationIndex.GetValue(),
        masteredItems: TokenBoxMasteredItemsIndex.GetTokenValuesCollection(),
        masteredNumberLot: TextBoxMasteredNumberLot.GetText(),
    };

    showPartialPage($("#grid"), 'Mastered/SearchResult', data);
}

function ClearClick() {
    var dayNow = new Date();
    DateEditInit.SetDate(new Date(dayNow.getFullYear(), dayNow.getMonth(), 1));
    DateEditEnd.SetDate(dayNow);
    ComboBoxState.SetValue(null);
    TextBoxNumber.SetText(null);
    ComboBoxResponsableIndex.SetValue(null);
    ComboBoxTurnIndex.SetValue(null);
    ComboBoxBoxedWarehouseIndex.SetValue(null);
    ComboBoxBoxedWarehouseLocationIndex.SetValue(null);
    TokenBoxBoxedItemsIndex.ClearTokenCollection();
    TextBoxBoxedNumberLot.SetText(null);
    ComboBoxMasteredWarehouseIndex.SetValue(null);
    ComboBoxMasteredWarehouseLocationIndex.SetValue(null);
    TokenBoxMasteredItemsIndex.ClearTokenCollection();
    TextBoxMasteredNumberLot.SetText(null);
}

//function MSTReport(s, e) {
//    GeneralReport("RMOVLE");
//}

function MSTReportEntero(s, e){

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
        id_responsable: ComboBoxResponsableIndex.GetValue(),
        id_turn: ComboBoxTurnIndex.GetValue(),
        id_boxedWarehouse: ComboBoxBoxedWarehouseIndex.GetValue(),
        id_boxedWarehouseLocation: ComboBoxBoxedWarehouseLocationIndex.GetValue(),
        boxedItems: TokenBoxBoxedItemsIndex.GetTokenValuesCollection(),
        boxedNumberLot: TextBoxBoxedNumberLot.GetText(),
        id_masteredWarehouse: ComboBoxMasteredWarehouseIndex.GetValue(),
        id_masteredWarehouseLocation: ComboBoxMasteredWarehouseLocationIndex.GetValue(),
        masteredItems: TokenBoxMasteredItemsIndex.GetTokenValuesCollection(),
        masteredNumberLot: TextBoxMasteredNumberLot.GetText(),
    };

    console.log(data);
    $.ajax({
        url: 'Mastered/MasteredReportIndex',
        type: 'post',
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
                if (result !== undefined) {
                    //console.log(result.ServerScriptTimeout);
                    var reportTdr = result.nameQS;
                    var url = 'ReportProd/Index?trepd=' + reportTdr;
                    newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
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

function MSTReportCola(s, e) {

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
        id_responsable: ComboBoxResponsableIndex.GetValue(),
        id_turn: ComboBoxTurnIndex.GetValue(),
        id_boxedWarehouse: ComboBoxBoxedWarehouseIndex.GetValue(),
        id_boxedWarehouseLocation: ComboBoxBoxedWarehouseLocationIndex.GetValue(),
        boxedItems: TokenBoxBoxedItemsIndex.GetTokenValuesCollection(),
        boxedNumberLot: TextBoxBoxedNumberLot.GetText(),
        id_masteredWarehouse: ComboBoxMasteredWarehouseIndex.GetValue(),
        id_masteredWarehouseLocation: ComboBoxMasteredWarehouseLocationIndex.GetValue(),
        masteredItems: TokenBoxMasteredItemsIndex.GetTokenValuesCollection(),
        masteredNumberLot: TextBoxMasteredNumberLot.GetText(),
    };

    console.log(data);
    $.ajax({
        url: 'Mastered/MasteredColaReportIndex',
        type: 'post',
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
                if (result !== undefined) {
                    //console.log(result.ServerScriptTimeout);
                    var reportTdr = result.nameQS;
                    var url = 'ReportProd/Index?trepd=' + reportTdr;
                    newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
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

function MSTExcel(s, e) {
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
    };

    console.log(data);
    if (data != null) {
        $.ajax({
            url: "Mastered/PrintMatrixMasterizadoExcel",
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
                // 
                try {
                    if (result != undefined) {
                        var reportTdr = result.nameQS;
                        var url = 'ReportProd/ToExcel?trepd=' + reportTdr;
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

function NewClick() {
    var data = {
        id: 0,
        enabled: true
    };
    showPage("Mastered/Edit", data);
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
