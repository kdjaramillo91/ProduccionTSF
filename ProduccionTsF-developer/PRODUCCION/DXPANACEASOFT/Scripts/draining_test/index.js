
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
        numberLote: TextBoxLote.GetText(),
        reference: TextBoxReference.GetText(),
    }
    
    showPartialPage($("#grid"), 'DrainingTest/SearchResult', data);
}

function ClearClick() {
    var dayNow = new Date();
    DateEditInit.SetDate(new Date(dayNow.getFullYear(), dayNow.getMonth(), 1));
    DateEditEnd.SetDate(dayNow);
    ComboBoxState.SetValue(null);
    TextBoxNumber.SetText(null);
    TextBoxLote.SetText(null);
    TextBoxReference.SetText(null);
}

function NewClick() {
    $.ajax({
        url: "DrainingTest/PendingNew",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            // 
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
function PrintItemIndex() {
	var dateInicio = DateEditInit.GetDate();
	var yearInicio = dateInicio.getFullYear();
	var monthInicio = dateInicio.getMonth() + 1;
	var dayInicio = dateInicio.getDate();

	var dateFin = DateEditEnd.GetDate();
	var yearFin = dateFin.getFullYear();
	var monthFin = dateFin.getMonth() + 1;
	var dayFin = dateFin.getDate();

	var data = {
		fechaEmisionInicio: dayInicio + "/" + monthInicio + "/" + yearInicio,
		fechaEmisionFinal: dayFin + "/" + monthFin + "/" + yearFin,
	};
	// 
	$.ajax({
		url: 'DrainingTest/PrintDrainingReportIndex',
		type: 'post',
		data: data,
		async: true,
		cache: false,
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			try {

				if (result != undefined) {
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

$(function () {
    Init();
});
