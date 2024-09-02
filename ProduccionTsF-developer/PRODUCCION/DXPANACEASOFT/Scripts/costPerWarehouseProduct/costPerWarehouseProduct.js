var _rootPath = "../../../";
var getFullPath = function (relativeUrl) {
	if (relativeUrl !== null && typeof relativeUrl === 'string' && relativeUrl.length > 0) {
		if (relativeUrl.slice(0, 1) === "/") {
			return _rootPath + relativeUrl.slice(1);
		} else {
			return _rootPath + relativeUrl;
		}
	}
	return _rootPath;
};

// Funciones del procesamiento
var onProcesarCostoBodegaButtonClick = function () {
	onProcesarArchivoCostoButtonClick("COSTO")
};
var onProcesarSaldoBodegaButtonClick = function () {
	onProcesarArchivoCostoButtonClick("SALDO")
};
var onProcesarArchivoCostoButtonClick = function (tipo) {
	var emissionDt = FechaCorteEditText.GetDate();
	var fecha = emissionDt != null
		? emissionDt.toISOString().substring(0, 10)
		: null;

	if (fecha != null) {
		var operationData = {
			fecha: fecha,
			tipo: tipo,
		};

		var hideLoadingDialog = true;
		$.ajax({
			url: "CostPerWarehouseProduct/Process",
			type: "post",
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			data: JSON.stringify(operationData),
			async: true,
			cache: false,
			error: function (error) {
				console.error(error);
				hideLoading();
				showErrorMessage(error);
			},
			beforeSend: function () {
				showLoading();
			},
			success: function (result) {
				if (result.isValid) {
					var reportArgs = {
						tipo: tipo,
					};
					var url = getFullPath('CostPerWarehouseProduct/GenerateExcel?' + $.param(reportArgs));
					$('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
					hideLoading();
				} else {
					showWarningMessage(result.message);
				}
			},
			complete: function () {
				if (hideLoadingDialog) {
					hideLoading();
				}
			}
		});
	}
};

// Funciones principales
var init = function () {
	$("#btnCollapse").click(function (event) {
		if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
			$("#query-results").css("display", "");
		} else {
			$("#query-results").css("display", "none");
		}
	});
};

$(function () {
	init();
});
