
// FORM FILTER BUTTONS

function OnClickSearchKardexs_Init(s, e) {

	var KardexFilter = $("#KardexFilter").val();
	console.log("KardexFilter: " + KardexFilter);
	if (KardexFilter == "True" || KardexFilter == "true" || KardexFilter == true) {
		showLoading();
		OnClickSearchKardexs(s, e);
	}
}
function OnClickSearchKardexsExcel_Init(s, e) {

	var KardexFilter = $("#KardexFilter").val();
	console.log("KardexFilter: " + KardexFilter);
	if (KardexFilter == "True" || KardexFilter == "true" || KardexFilter == true) {
		showLoading();
		OnClickSearchKardexsExcel(s, e);
	}
}

// Warehouse && Warehouse Location

function OnWarehouseExit_SelectedIndexChanged(s, e) {
	id_warehouseLocationExit.PerformCallback();
}

function OnWarehouseLocationExitInit(s, e) {
	s.PerformCallback();
}

function OnWarehouseEntry_SelectedIndexChanged(s, e) {
	id_warehouseLocationEntry.PerformCallback();
}

function OnWarehouseLocationEntryInit(s, e) {
	s.PerformCallback();
}

function OnNatureMove_SelectedIndexChanged(s, e) {
	id_inventoryReason.PerformCallback();
}

function OnInventoryReasonInit(s, e) {
	s.PerformCallback();
}


function OnClickSearchKardexs(s, e) {
	var dateInicio = startEmissionDate.GetDate();

	if (dateInicio == null) {
		NotifyWarning("Fecha de Inicio no puede estar vacía");
		return;
	}

	var yearInicio = dateInicio.getFullYear();
	var monthInicio = dateInicio.getMonth() + 1;
	var dayInicio = dateInicio.getDate();

	var dateFin = endEmissionDate.GetDate();

	if (dateFin == null)
		dateFin = new Date();

	var yearFin = dateFin.getFullYear();
	var monthFin = dateFin.getMonth() + 1;
	var dayFin = dateFin.getDate();

	var startEmissionDateFinal = dayInicio + "/" + monthInicio + "/" + yearInicio;
	var endEmissionDateFinal = dayFin + "/" + monthFin + "/" + yearFin;

	var data = "startEmissionDateFinal=" + startEmissionDateFinal + "&" + "endEmissionDateFinal=" + endEmissionDateFinal + "&" + $("#formFilterKardex").serialize();

	$.ajax({
		url: "Kardex/KardexResultsFixedVersionOptimized",
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

function OnClickSearchKardexsExcel(s, e) {
	var dateInicio = startEmissionDate.GetDate();

	if (dateInicio == null) {
		NotifyWarning("Fecha de Inicio no puede estar vacía");
		return;
    }
		//dateInicio = new Date(2010, 0, 1);

	var yearInicio = dateInicio.getFullYear();
	var monthInicio = dateInicio.getMonth() + 1;
	var dayInicio = dateInicio.getDate();

	var dateFin = endEmissionDate.GetDate();

	if (dateFin == null)
		dateFin = new Date();

	var yearFin = dateFin.getFullYear();
	var monthFin = dateFin.getMonth() + 1;
	var dayFin = dateFin.getDate();

	var startEmissionDateFinal = dayInicio + "/" + monthInicio + "/" + yearInicio;
	var endEmissionDateFinal = dayFin + "/" + monthFin + "/" + yearFin;

	var data = "startEmissionDateFinal=" + startEmissionDateFinal + "&" + "endEmissionDateFinal=" + endEmissionDateFinal + "&" + $("#formFilterKardex").serialize();

	$.ajax({
		url: "Kardex/KardexResultsFixedVersionOptimizedExcel",
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

// Report

function KardexReport(s, e) {
	GeneralReport("IKSPV1");
}

function KardexReportLot(s, e) {
	GeneralReport("IKSPVL");
}
function KardexCostReport(s, e) {
	GeneralReportDatatableExcel("KEDST");
}

function BalanceReport(s, e) {
	GeneralReport("ISPV1");
}

function BalanceLotReport(s, e) {
	GeneralReport("ISPVL");
}

function BalanceWithOutLotReport(s, e) {
	GeneralReport("ISPWVL");
}

function BalanceWithOutLotExcelReport(s, e) {
	GeneralReportDatatableExcel("ISPWVL");
}

function BalanceWithOutLotAllReport(s, e) {
	GeneralReport("ISPWVT");

}

function InventoryMoveReport(s, e) {
	GeneralReport("IMIPV1");
}

function InventoryLotMoveReport(s, e) {
	GeneralReport("IMIPVL");
}

function KardexReportProvider(s, e) {
	GeneralReport("IKRPV");
}

function KardexReportProviderBalance(s, e) {
	GeneralReport("IKRPS");
}

function BalanceLotExcelReport(s, e) {
	GeneralReportExcel("RMOVLE");
}

function GeneralReport(code) {

	var dateInicio = startEmissionDate.GetDate();
	if (dateInicio == null)
		dateInicio = new Date(2010, 0, 1);

	var yearInicio = dateInicio.getFullYear();
	var monthInicio = dateInicio.getMonth() + 1;
	var dayInicio = dateInicio.getDate();

	var dateFin = endEmissionDate.GetDate();

	if (dateFin == null)
		dateFin = new Date();

	var yearFin = dateFin.getFullYear();
	var monthFin = dateFin.getMonth() + 1;
	var dayFin = dateFin.getDate();

	var startEmissionDateFinal = dayInicio + "/" + monthInicio + "/" + yearInicio;
	var endEmissionDateFinal = dayFin + "/" + monthFin + "/" + yearFin;

	var data = "codeReport=" + code + "&" + "startEmissionDateFinal=" + startEmissionDateFinal + "&" + "endEmissionDateFinal=" + endEmissionDateFinal + "&" + $("#formFilterKardex").serialize();

	//var data = "codeReport=" + code + "&" + $("#formFilterKardex").serialize();
	// 
	console.log(data);
	$.ajax({
		url: 'Kardex/InventoryKardexReportIndex',
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			//// 
			console.log(error);
		},
		type: 'POST',
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			try {
				if (result !== undefined) {
					//// 
					console.log(result.ServerScriptTimeout);
					var reportTdr = result.rep.nameQS;
					var url = 'ReportProd/Report?trepd=' + reportTdr;
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

function GeneralReportDatatableExcel(code) {
	var dateInicio = startEmissionDate.GetDate();
	if (dateInicio == null)
		dateInicio = new Date(2010, 0, 1);

	var yearInicio = dateInicio.getFullYear();
	var monthInicio = dateInicio.getMonth() + 1;
	var dayInicio = dateInicio.getDate();

	var dateFin = endEmissionDate.GetDate();

	if (dateFin == null)
		dateFin = new Date();

	var yearFin = dateFin.getFullYear();
	var monthFin = dateFin.getMonth() + 1;
	var dayFin = dateFin.getDate();

	var startEmissionDateFinal = dayInicio + "/" + monthInicio + "/" + yearInicio;
	var endEmissionDateFinal = dayFin + "/" + monthFin + "/" + yearFin;

	var data = "codeReport=" + code + "&" + "startEmissionDateFinal=" + startEmissionDateFinal + "&" + "endEmissionDateFinal=" + endEmissionDateFinal + "&" + $("#formFilterKardex").serialize();

	//var data = "codeReport=" + code + "&" + $("#formFilterKardex").serialize();
	$.ajax({
		url: 'Kardex/InventoryKardexReportIndex',
		data: data,
		async: true,
		cache: false,
		error: function (error) {
			//// 
			console.log(error);
		},
		type: 'POST',
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			try {
				if (result !== undefined) {

					NotifyWarning("Su reporte se esta descargando, continue con sus actividades.");
					var reportTdr = result.rep.nameQS;
					var url = 'ReportProd/ToExcelDataTable?trepd=' + reportTdr;
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

function GeneralReportExcel(code) {
	var dateInicio = startEmissionDate.GetDate();
	if (dateInicio == null)
		dateInicio = new Date(2010, 0, 1);

	var yearInicio = dateInicio.getFullYear();
	var monthInicio = dateInicio.getMonth() + 1;
	var dayInicio = dateInicio.getDate();

	var dateFin = endEmissionDate.GetDate();

	if (dateFin == null)
		dateFin = new Date();

	var yearFin = dateFin.getFullYear();
	var monthFin = dateFin.getMonth() + 1;
	var dayFin = dateFin.getDate();

	var startEmissionDateFinal = dayInicio + "/" + monthInicio + "/" + yearInicio;
	var endEmissionDateFinal = dayFin + "/" + monthFin + "/" + yearFin;

	var data = "codeReport=" + code + "&" + "startEmissionDateFinal=" + startEmissionDateFinal + "&" + "endEmissionDateFinal=" + endEmissionDateFinal + "&" + $("#formFilterKardex").serialize();


	//var data = "codeReport=" + code + "&" + $("#formFilterKardex").serialize();

	$.ajax({
		url: 'Kardex/InventoryKardexReportExcelIndex',
		data: data,
		async: true,
		cache: false,
		type: 'POST',
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {

			try {
				// 
				if (result != undefined) {
					var reportTdr = result.nameQS;
					window.open(window.location.origin + '/Kardex/DownloadExcelToLot?fileName=' + "reporte", "_blank");
					//var url = 'ReportProd/ToExcel?trepd=' + reportTdr;
					//newWindow = window.open(url, '_self', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0', false);

					// newWindow.focus();
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