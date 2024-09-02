
function ComboBoxFreezerWarehouseLocationIndex_BeginCallback(s, e) {
    e.customArgs["id_freezerWarehouse"] = ComboBoxFreezerWarehouseIndex.GetValue() === undefined ? null : ComboBoxFreezerWarehouseIndex.GetValue();
}

function ComboBoxFreezerWarehouseIndex_SelectedIndexChanged(s, e) {
    ComboBoxFreezerWarehouseLocationIndex.PerformCallback();
}

//function ComboBoxBoxedWarehouseLocationIndex_BeginCallback(s, e) {
//    e.customArgs["id_boxedWarehouse"] = ComboBoxBoxedWarehouseIndex.GetValue() === undefined ? null : ComboBoxBoxedWarehouseIndex.GetValue();
//}

//function ComboBoxBoxedWarehouseIndex_SelectedIndexChanged(s, e) {
//    ComboBoxBoxedWarehouseLocationIndex.PerformCallback();
//}

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
        reference: TextBoxReference.GetText(),
        id_responsable: ComboBoxResponsableIndex.GetValue(),
        id_freezerWarehouse: ComboBoxFreezerWarehouseIndex.GetValue(),
        id_freezerWarehouseLocation: ComboBoxFreezerWarehouseLocationIndex.GetValue(),
        id_boxedWarehouse: null,//ComboBoxBoxedWarehouseIndex.GetValue(),
        id_boxedWarehouseLocation: null,//ComboBoxBoxedWarehouseLocationIndex.GetValue(),
        numberLot: TextBoxNumberLot.GetText(),
        secTransLot: TextBoxSecTransaction.GetText(),
        items: TokenBoxItemsIndex.GetTokenValuesCollection()
    };

    showPartialPage($("#grid"), 'OpeningClosingPlateLying/SearchResult', data);
}

function ClearClick() {
    var dayNow = new Date();
    DateEditInit.SetDate(new Date(dayNow.getFullYear(), dayNow.getMonth(), 1));
    DateEditEnd.SetDate(dayNow);
    ComboBoxState.SetValue(null);
    TextBoxNumber.SetText(null);
    TextBoxReference.SetText(null);
    ComboBoxFreezerWarehouseIndex.SetValue(null);
    ComboBoxFreezerWarehouseLocationIndex.SetValue(null);
    //ComboBoxBoxedWarehouseIndex.SetValue(null);
    //ComboBoxBoxedWarehouseLocationIndex.SetValue(null);
    ComboBoxResponsableIndex.SetValue(null);
    TextBoxNumberLot.SetText(null);
    TextBoxSecTransaction.SetText(null);
    TokenBoxItemsIndex.ClearTokenCollection();
}

function NewClick() {
    var data = {
        id: 0,
        enabled: true
    };
    showPage("OpeningClosingPlateLying/Edit", data);
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
