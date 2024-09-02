
function DateEditLiquidationDate_Validation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		//if (errorMessage === null || errorMessage === "") {
		//    errorMessage = "- Fecha Fin: Es obligatorio.";
		//} else {
		//    errorMessage += "</br> - Fecha Fin: Es obligatorio.";
		//}
	} else {
		var liquidationDateAux = s.GetDate();
		var yearLiquidationDate = liquidationDateAux.getFullYear();
		var monthLiquidationDate = liquidationDateAux.getMonth();
		var dayLiquidationDate = liquidationDateAux.getDate();
		var dateLiquidationDateAux = new Date(yearLiquidationDate, monthLiquidationDate, dayLiquidationDate);

		var emisionDate = DateTimeEmision.GetValue();
		var yearEmisionDate = emisionDate.getFullYear();
		var monthEmisionDate = emisionDate.getMonth();
		var dayEmisionDate = emisionDate.getDate();
		var emisionDateAux = new Date(yearEmisionDate, monthEmisionDate, dayEmisionDate);

		if (dateLiquidationDateAux < emisionDateAux) {
			e.isValid = false;
			e.errorText = "La Fecha Cierre debe ser mayor e igual a la Fecha Emisión";
		}
	}
	//if (!runningValidation) {
	//    ValidateDetail();
	//}
}

function TimeEditLiquidationTime_Validation(s, e) {
	if (s.GetValue() === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
		//if (errorMessage === null || errorMessage === "") {
		//    errorMessage = "- Fecha Fin: Es obligatorio.";
		//} else {
		//    errorMessage += "</br> - Fecha Fin: Es obligatorio.";
		//}
	}

	//if (!runningValidation) {
	//    ValidateDetail();
	//}
}

function ShowCurrentItem(enabled) {
	var data = {
		id: $('#id_liquidationTurn').val(),
		id_turn: 0,
		emissionDate: null,
		id_personProcessPlant: 0,
		enabled: enabled
	};

	showPage("LiquidationTurn/Edit", data);
}

function AddNewItem() {
	$.ajax({
		url: "LiquidationTurn/PendingNew",
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
		url: 'LiquidationTurn/Approve',
		type: 'post',
		data: { id: $('#id_liquidationTurn').val() },
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
			NotifySuccess("Liquidación de Turno Aprobada Satisfactoriamente. " + "Estado: " + result.Data);
		}
	});
}

function AprovedCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Aprobar la Liquidación de Turno?", "Confirmar");
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
	var result = DevExpress.ui.dialog.confirm("Desea Reversar la Liquidación de Turno?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'LiquidationTurn/Reverse',
				type: 'post',
				data: { id: $('#id_liquidationTurn').val() },
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
					NotifySuccess("Liquidación de Turno Reversada Satisfactoriamente. " + "Estado: " + result.Data);
				}
			});
		}
	});
}

function AnnulCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Anular la Liquidación de Turno?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'LiquidationTurn/Annul',
				type: 'post',
				data: { id: $('#id_liquidationTurn').val() },
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
					NotifySuccess("Liquidación de Turno Anulada Satisfactoriamente. " + "Estado: " + result.Data);
				}
			});
		}
	});
}

function SaveDataUser() {

	var liquidationDateAux = liquidationDate.GetValue();
	var liquidationTimeAux = liquidationTime.GetValue();
	var userData = {
		id: $('#id_liquidationTurn').val(),
		//id_machineProdOpening: $('#id_machineProdOpening').val(),
		dateTimeEmision: DateTimeEmision.GetValue(),
		description: MemoDescription.GetText(),
		liquidationDateTime: liquidationDateAux.getFullYear() + "-" +
			(liquidationDateAux.getMonth() + 1).toString().padStart(2, 0) + "-" +
			liquidationDateAux.getDate().toString().padStart(2, 0) + "T" +
			liquidationTimeAux.getHours().toString().padStart(2, 0) + ":" +
			liquidationTimeAux.getMinutes().toString().padStart(2, 0) + ":00"
		//Detail

	};

	var LiquidationTurn = {
		jsonLiquidationTurn: JSON.stringify(userData)
	};

	return LiquidationTurn;
}

function SaveItem(aproved) {
	showLoading();

	if (!Validate()) {
		hideLoading();
		return;
	}

	$.ajax({
		url: 'LiquidationTurn/Save',
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
			$('#id_liquidationTurn').val(id);

			if (aproved)
				AprovedItem();
			else
				ShowCurrentItem(true);

			hideLoading();
			NotifySuccess("Liquidación de Turno Guardada Satisfactoriamente.");
		},
		error: function (result) {
			hideLoading();
		}
	});
}

function IsValid(object) {
	return (object != null && object != undefined && object != "undefined");
}

function Validate() {
	var validate = true;
	var errors = "";

	if (!IsValid(DateTimeEmision) || DateTimeEmision.GetValue() == null) {
		errors += "Fecha Emisión es un campo Obligatorio. \n\r";
		validate = false;
	}
	if (!IsValid(liquidationDate) || liquidationDate.GetValue() == null) {
		errors += "Fecha Cierre es un campo Obligatorio. \n\r";
		validate = false;
	} else {
		var liquidationDateAux = liquidationDate.GetDate();
		var yearLiquidationDate = liquidationDateAux.getFullYear();
		var monthLiquidationDate = liquidationDateAux.getMonth();
		var dayLiquidationDate = liquidationDateAux.getDate();
		var dateLiquidationDateAux = new Date(yearLiquidationDate, monthLiquidationDate, dayLiquidationDate);

		var emisionDate = DateTimeEmision.GetValue();
		var yearEmisionDate = emisionDate.getFullYear();
		var monthEmisionDate = emisionDate.getMonth();
		var dayEmisionDate = emisionDate.getDate();
		var emisionDateAux = new Date(yearEmisionDate, monthEmisionDate, dayEmisionDate);

		if (dateLiquidationDateAux < emisionDateAux) {
			validate = false;
			errors += "Fecha Cierre debe ser mayor e igual a la Fecha Emisión. \n\r";
		}
	}
	if (!IsValid(liquidationTime) || liquidationTime.GetValue() == null) {
		errors += "Hora Cierre es un campo Obligatorio. \n\r";
		validate = false;
	}

	if (validate == false) {
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
	showPage("LiquidationTurn/Index");
}

function InitializePagination() {

	if ($("#id_liquidationTurn").val() !== 0) {

		var current_page = 1;
		var max_page = 1;
		$.ajax({
			url: "LiquidationTurn/InitializePagination",
			type: "post",
			data: { id: $("#id_liquidationTurn").val() },
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
				showPage("LiquidationTurn/Pagination", { page: page });
			}
		});
	}
}

function Init() {
}

$(function () {
	InitializePagination();
	Init();
});