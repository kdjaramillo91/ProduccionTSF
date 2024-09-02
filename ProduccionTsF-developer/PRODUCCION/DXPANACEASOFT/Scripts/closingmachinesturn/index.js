
function ComboBoxProviderIndex_SelectedIndexChanged() {
	ComboBoxProductionUnitProviderIndex.PerformCallback();
}

function ComboBoxProductionUnitProviderIndex_BeginCallback(s, e) {
	e.customArgs["id_provider"] = ComboBoxProviderIndex.GetValue() === undefined ? null : ComboBoxProviderIndex.GetValue();
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
		id_turn: ComboBoxTurnIndex.GetValue(),
		id_machineForProd: ComboBoxMachineForProdIndex.GetValue(),
		id_person: ComboBoxPersonIndex.GetValue(),
		numberLot: TextBoxNumberLot.GetText(),
		id_provider: ComboBoxProviderIndex.GetValue(),
		id_productionUnitProvider: ComboBoxProductionUnitProviderIndex.GetValue()
	};

	showPartialPage($("#grid"), 'ClosingMachinesTurn/SearchResult', data);
}

function ClearClick() {
	var dayNow = new Date();
	DateEditInit.SetDate(new Date(dayNow.getFullYear(), dayNow.getMonth(), 1));
	DateEditEnd.SetDate(dayNow);
	ComboBoxState.SetValue(null);
	TextBoxNumber.SetText(null);
	ComboBoxTurnIndex.SetValue(null);
	ComboBoxMachineForProdIndex.SetValue(null);
	ComboBoxPersonIndex.SetValue(null);
	TextBoxNumberLot.SetText(null);
	ComboBoxProviderIndex.SetValue(null);
	ComboBoxProductionUnitProviderIndex.SetValue(null);
	ComboBoxProductionUnitProviderIndex.PerformCallback();
}

function NewClick() {
	$.ajax({
		url: "ClosingMachinesTurn/PendingNew",
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
