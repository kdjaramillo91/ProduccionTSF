function allowOnlyDecimals(e) {
    // on input
    if (e.inputType == "insertFromPaste") {
        var validNumber = new RegExp(/^\d*\.?\d*$/);
        return (!validNumber.test(e.target.value) ? e.target.value = '' : e.target.value);
    }

    // on key press
    return e.charCode === 0 || ((e.charCode >= 48 && e.charCode <= 57) || (e.charCode == 46 && e.target.value.indexOf('.') < 0));
}

function GetDataFilterForConsultResultList() {
    let yearInicio = 0;
    let monthInicio = 0;
    let dayInicio = 0;
    let dateInicio = emissionDateInit.GetDate();

    if (dateInicio == null) {
        dateInicio = new Date();
        dayInicio = 1;
    } else
        dayInicio = dateInicio.getDate();

    yearInicio = dateInicio.getFullYear();
    monthInicio = dateInicio.getMonth() + 1;

    var dateFin = emissionDateEnd.GetDate();
    if (dateFin == null)
        dateFin = new Date();
    let yearFin = dateFin.getFullYear();
    let monthFin = dateFin.getMonth() + 1;
    let dayFin = dateFin.getDate();

    let data = {
        dateEmissionFrom: dayInicio + "/" + monthInicio + "/" + yearInicio,
        dateEmissionTo: dayFin + "/" + monthFin + "/" + yearFin,
        id_StateDocument: ComboBoxState.GetValue(),
        number: TextBoxNumber.GetText(),
        reference: TextBoxReference.GetText(),
        id_InventoryReasonExit: ComboBoxReasonExit.GetValue(),
        id_WarehouseExit: ComboBoxWarehouseExit.GetValue(),
        id_WarehouseLocationExit: ComboBoxWarehouseLocationExit.GetValue(),
        id_InventoryReasonEntry: ComboBoxReasonEntry.GetValue(),
        id_WarehouseEntry: ComboBoxWarehouseEntry.GetValue(),
        id_WarehouseLocationEntry: ComboBoxWarehouseLocationEntry.GetValue(),
        id_Dispatcher: ComboBoxUserDispatcher.GetValue(),
        id_Item: ComboBoxItem.GetValue()
    };

    return data;
}

function SearchClick() {

    $("#btnCollapse").click();
    let data = GetDataFilterForConsultResultList();

    showPartialPage($("#grid"), 'AutomaticTransfer/SearchResult', data);
}

function ClearClick() {
    let dayNow = new Date();
    emissionDateInit.SetDate(new Date(dayNow.getFullYear(), dayNow.getMonth(), 1));
    emissionDateEnd.SetDate(dayNow);
    ComboBoxState.SetValue(null);
    TextBoxNumber.SetText(null);
    TextBoxReference.SetText(null);
    ComboBoxReasonExit.SetValue(null);
    ComboBoxWarehouseExit.SetValue(null);
    ComboBoxWarehouseLocationExit.SetValue(null);
    ComboBoxReasonEntry.SetValue(null);
    ComboBoxWarehouseEntry.SetValue(null);
    ComboBoxWarehouseLocationEntry.SetValue(null);
    ComboBoxUserDispatcher.SetValue(null);
    ComboBoxItem.SetValue(null);
}

function NewClick() {
    var data = {
        id: 0,
        enabled: true
    };
    showPage("AutomaticTransfer/Edit", data);
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


function ComboBoxWarehouseEntry_SelIndexChanged(s, e) {
    ComboBoxWarehouseLocationEntry.PerformCallback();
}

function ComboBoxWarehouseExit_SelIndexChanged(s, e) {
    ComboBoxWarehouseLocationExit.PerformCallback();
}

function ComboBoxWarehouseLocationEntry_BeginCallback(s, e) {
    e.customArgs["id_Warehouse"] = ComboBoxWarehouseEntry.GetValue();
}

function ComboBoxWarehouseLocationExit_BeginCallback(s, e) {
    e.customArgs["id_Warehouse"] = ComboBoxWarehouseExit.GetValue();
}

$(function () {
    Init();
});