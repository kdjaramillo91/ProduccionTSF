
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
        id_turn: ComboBoxTurnIndex.GetValue(),
        id_machineForProd: ComboBoxMachineForProdIndex.GetValue()
    };
    
    showPartialPage($("#grid"), 'NonProductiveHour/SearchResult', data);
}

function ClearClick() {
    var dayNow = new Date();
    DateEditInit.SetDate(new Date(dayNow.getFullYear(), dayNow.getMonth(), 1));
    DateEditEnd.SetDate(dayNow);
    ComboBoxState.SetValue(null);
    TextBoxNumber.SetText(null);
    ComboBoxTurnIndex.SetValue(null);
    ComboBoxMachineForProdIndex.SetValue(null);
}

function NewClick() {
    $.ajax({
        url: "NonProductiveHour/PendingNew",
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
