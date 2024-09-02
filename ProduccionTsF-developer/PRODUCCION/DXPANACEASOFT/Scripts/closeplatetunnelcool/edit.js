function DateEditCloseDate_Validation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	} else {
		var closeDateAux = s.GetDate();
		var yearCloseDate = closeDateAux.getFullYear();
		var monthCloseDate = closeDateAux.getMonth();
		var dayCloseDate = closeDateAux.getDate();
		var dateCloseDateAux = new Date(yearCloseDate, monthCloseDate, dayCloseDate);

		var emisionDate = DateTimeEmision.GetValue();
		var yearEmisionDate = emisionDate.getFullYear();
		var monthEmisionDate = emisionDate.getMonth();
		var dayEmisionDate = emisionDate.getDate();
		var emisionDateAux = new Date(yearEmisionDate, monthEmisionDate, dayEmisionDate);

		if (dateCloseDateAux < emisionDateAux) {
			e.isValid = false;
			e.errorText = "La Fecha Cierre debe ser mayor e igual a la Fecha Emisión";
		}
	}
}

function TimeEditCloseTime_Validation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
}

function ShowCurrentItem(enabled) {
	var data = {
		id: $('#id_closePlateTunnelCool').val(),
		id_MachineProdOpeningDetail: 0,
		enabled: enabled
	};

	showPage("ClosePlateTunnelCool/Edit", data);
}

function AddNewItem() {
	$.ajax({
		url: "ClosePlateTunnelCool/PendingNew",
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

function EditCurrentItem() {
	showLoading();
	ShowCurrentItem(true);
}

function SaveCurrentItem() {
	SaveItem(false);
}

function AprovedItem() {
	showLoading();
	$.ajax({
		url: 'ClosePlateTunnelCool/Approve',
		type: 'post',
		data: { id: $('#id_closePlateTunnelCool').val() },
		async: true,
		cache: false,
		error: function (result) {
			hideLoading();
			NotifyError("Error. " + result.Message);
		},
		success: function (result) {
			if (result.Code !== 0) {
				hideLoading();
				NotifyError("Error al Aprobar. " + result.Message);
				return;
			}

			ShowCurrentItem(false);
			hideLoading();
			NotifySuccess("Cierre de Placas, Túneles, Fresco Aprobado Satisfactoriamente. " + "Estado: " + result.Data);
		}
	});
}

function AprovedCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Aprobar el Cierre de Placas, Túneles, Fresco?", "Confirmar");
	result.done(function (dialogResult) {

		if (dialogResult) {
			if ($("#enabled").val() == "true") {
				SaveItem(true);
			} else {
				AprovedItem();
			}
		}
	});
}

function ReverseCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Reversar el Cierre de Placas, Túneles, Fresco?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'ClosePlateTunnelCool/Reverse',
				type: 'post',
				data: { id: $('#id_closePlateTunnelCool').val() },
				async: true,
				cache: false,
				error: function (result) {
					hideLoading();
					NotifyError("Error. " + result.Message);
				},
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error al Reversar. " + result.Message);
						return;
					}

					ShowCurrentItem(false);
					hideLoading();
					NotifySuccess("Cierre de Placas, Túneles, Fresco Reversado Satisfactoriamente. " + "Estado: " + result.Data);
				}
			});
		}
	});
}

function AnnulCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Anular el Cierre de Placas, Túneles, Fresco?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'ClosePlateTunnelCool/Annul',
				type: 'post',
				data: { id: $('#id_closePlateTunnelCool').val() },
				async: true,
				cache: false,
				error: function (result) {
					hideLoading();
					NotifyError("Error. " + result.Message);
				},
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error al Anular. " + result.Message);
						return;
					}

					ShowCurrentItem(false);
					hideLoading();
					NotifySuccess("Cierre de Placas, Túneles, Fresco Anulado Satisfactoriamente. " + "Estado: " + result.Data);
				}
			});
		}
	});
}

function SaveDataUser() {
	var closeDateAux = closeDate.GetValue();
	var closeTimeAux = closeTime.GetValue();
	var userData = {
		id: $('#id_closePlateTunnelCool').val(),
		//id_machineProdOpening: $('#id_machineProdOpening').val(),
		dateTimeEmision: DateTimeEmision.GetValue(),
		description: MemoDescription.GetText(),
		closeDateTime: closeDateAux.getFullYear() + "-" +
			(closeDateAux.getMonth() + 1).toString().padStart(2, 0) + "-" +
			closeDateAux.getDate().toString().padStart(2, 0) + "T" +
			closeTimeAux.getHours().toString().padStart(2, 0) + ":" +
			closeTimeAux.getMinutes().toString().padStart(2, 0) + ":00"

	};

	var ClosePlateTunnelCool = {
		jsonClosePlateTunnelCool: JSON.stringify(userData)
	};

	return ClosePlateTunnelCool;
}

function SaveItem(aproved) {
	showLoading();

	if (!Validate()) {
		hideLoading();
		return;
	}

	$.ajax({
		url: 'ClosePlateTunnelCool/Save',
		type: 'post',
		data: SaveDataUser(),
		async: true,
		cache: false,
		success: function (result) {
			if (result.Code !== 0) {
				hideLoading();
				NotifyError("Error. " + result.Message);
				return;
			}

			var id = result.Data;
			$('#id_closePlateTunnelCool').val(id);

			if (aproved)
				AprovedItem();
			else
				ShowCurrentItem(true);

			hideLoading();
			NotifySuccess("Cierre de Placas, Túneles, Fresco Guardado Satisfactoriamente.");
		},
		error: function (result) {
			hideLoading();
		}
	});
}

function IsValid(object) {
	return (object !== null && object !== undefined && object !== "undefined");
}

function Validate() {
	var validate = true;
	var errors = "";

	if (!IsValid(DateTimeEmision) || DateTimeEmision.GetValue() === null) {
		errors += "Fecha Emisión es un campo Obligatorio. \n\r";
		validate = false;
	}

	if (!IsValid(closeDate) || closeDate.GetValue() === null) {
		errors += "Fecha Cierre es un campo Obligatorio. \n\r";
		validate = false;
	} else {
		var closeDateAux = closeDate.GetDate();
		var yearCloseDate = closeDateAux.getFullYear();
		var monthCloseDate = closeDateAux.getMonth();
		var dayCloseDate = closeDateAux.getDate();
		var dateCloseDateAux = new Date(yearCloseDate, monthCloseDate, dayCloseDate);

		var emisionDate = DateTimeEmision.GetValue();
		var yearEmisionDate = emisionDate.getFullYear();
		var monthEmisionDate = emisionDate.getMonth();
		var dayEmisionDate = emisionDate.getDate();
		var emisionDateAux = new Date(yearEmisionDate, monthEmisionDate, dayEmisionDate);

		if (dateCloseDateAux < emisionDateAux) {
			validate = false;
			errors += "Fecha Cierre debe ser mayor e igual a la Fecha Emisión. \n\r";
		}
	}
	if (!IsValid(closeTime) || closeTime.GetValue() === null) {
		errors += "Hora Cierre es un campo Obligatorio. \n\r";
		validate = false;
	}

	if (validate === false) {
		NotifyError("Error. " + errors);
	}

	return validate;
}

function ButtonUpdate_Click() {
	SaveItem(false);
}

function ButtonCancel_Click() {
	RedirecBack();
}

function RedirecBack() {
	showPage("ClosePlateTunnelCool/Index");
}

function InitializePagination() {

	if ($("#id_closePlateTunnelCool").val() !== 0) {

		var current_page = 1;
		var max_page = 1;
		$.ajax({
			url: "ClosePlateTunnelCool/InitializePagination",
			type: "post",
			data: { id: $("#id_closePlateTunnelCool").val() },
			async: false,
			cache: false,
			error: function (error) {
				console.log(error);
			},
			beforeSend: function () {
			},
			success: function (result) {
				max_page = result.maximunPages;
				current_page = result.currentPage;
			},
			complete: function () {
			}
		});

		$('.pagination').jqPagination({
			current_page: current_page,
			max_page: max_page,
			page_string: "{current_page} de {max_page}",
			paged: function (page) {
				showPage("ClosePlateTunnelCool/Pagination", { page: page });
			}
		});
	}
}

function GeneralReport(code, processType) {

	var id = document.getElementById("id_closePlateTunnelCool").getAttribute("value");
	if (typeof id === undefined || id == null || id == 0) return;
	let _id = parseInt(id);

	$.ajax({
		url: "ClosePlateTunnelCool/PrintReport",
		type: "post",
		data: {

			id_closePlateTunnelCool: _id,
			processType: processType,
			codeReport: code
		},
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

function ReportFCam03(s, e) {
	GeneralReport("FCAM03", "COL");
}

function ReportFCam02(s, e) {
	GeneralReport("FCAM02", "ENT");
}

function Init() {
}

$(function () {
	InitializePagination();
	Init();
});