
// Eventos de controles de filtro para la consulta

var onRefreshProcessStateClick = function () {
	buscarEstadoProceso();
};

var onClearFilterPanelClick = function () {
	idPeriod.SetValue(null);
	idWarehouse.SetValue(null);
	idLocation.SetValue(null);
	idItem.SetValue(null);
	//isMassiveProcess.SetChecked(false);
};

var onExportExcelClick = function () {
	ExportExcel();
}

var onProcessClick = function () {
	processData();
};

var onEstadoProcesoInit = function () {
	buscarEstadoProceso();
	let status = TextBoxEstadoProceso.GetValue();
	if (status === "EN PROCESO")
	{
		observerNotification("169", 5000, callbackProcess);
	}
};

var ExportExcel = function () {
	$.ajax({
		url: "BackgroundProcessManagement/ExportExcel",
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
			if ((result === null || result === undefined)) {
				return;
			}
			if (!(result.isOk)) {
				NotifyWarning(result.message);
			} else {
				window.open(window.location.origin + '/BackgroundProcessManagement/DownloadExcel?fileName=' + result.fileName,"_blank");
			}
		},
		complete: function () {
			hideLoading();
		}
	});
};

// Funciones principales
var buscarEstadoProceso = function () {
	NotifySuccess("Buscando actualización del estado del proceso ...");
	$.ajax({
		url: "BackgroundProcessManagement/GetProcessState",
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
			if ((result === null || result === undefined)) {
				return;
			}
			TextBoxEstadoProceso.SetText(result.description);
		},
		complete: function () {
			hideLoading();
		}
	});
};

var processData = function () {
	let codePeriod= idPeriod.GetValue();
	if (codePeriod === null || (typeof codePeriod === "undefined"))
	{
		NotifyError("Debe selecionar un periodo a generar saldo");
		return;
	}

	let data = {
		codePeriod: codePeriod,
		isMassive: 0,
		idWarehouse: idWarehouse.GetValue(),
		idWarehouseLocation: idLocation.GetValue(),
		idItem: idItem.GetValue()
	};

	$.ajax({
		url: "BackgroundProcessManagement/SendDataToProcess",
		type: "post",
		async: true,
		cache: false,
		data: data,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			if ((result === null || result === undefined)) {
				return;
			}
			if (!(result.isOk)) {

				
				NotifyWarning(result.description);
			} else {
				observerNotification("169", 5000, callbackProcess);
				TextBoxEstadoProceso.SetValue("EN PROCESO");
				NotifySuccess(result.description);
			}
		},
		complete: function () {
			hideLoading();
		}
	});
};


var init = function () {
	$("#btnCollapse").click(function(event) {
		if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
			$("#results").css("display", "");
		} else {
			$("#results").css("display", "none");
		}
	});
};

$(function() {
	init();
});


function callbackProcess(status) {
	if (status == "PROCESADO")
	{
		downloadResultSp();
	}
	
	if (typeof TextBoxEstadoProceso !== "undefined" && ASPxClientUtils.IsExists(TextBoxEstadoProceso)) {
		TextBoxEstadoProceso.SetValue("DISPONIBLE");
	}  
}

function downloadResultSp() {
	
	let nombreFile = `SaldosMensuales_${GetDateFormat()}.xlsx`;
	$('#download-area-general').empty();
	$('#download-area-general').html(`<iframe style='height:0;width:0;border:0;' src='BackgroundProcessManagement/DownloadExcel?fileName=${nombreFile}'></iframe>`);
}