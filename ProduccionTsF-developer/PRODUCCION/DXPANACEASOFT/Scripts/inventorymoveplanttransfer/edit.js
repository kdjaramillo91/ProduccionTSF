
var errorMessage = "";

function OnAmountToEnterInventoryMovePlantTransferDetailValidation(s, e) {
    errorMessage = "";
    $("#GridMessageErrorDetail").hide();

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Cantidad a Ingresar: Es obligatorio.";
    } else if (parseFloat(s.GetValue()) <= 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecto";
        errorMessage = "- Cantidad a Ingresar: Valor Incorrecto.";
    } else {
        var pendingAux = pending.GetValue() === null ? 0.00 : parseFloat(pending.GetValue());
        var amountToEnterAux = parseFloat(s.GetValue());
        if (pendingAux < amountToEnterAux) {
            e.isValid = false;
            e.errorText = "Cantidad a Ingresar: debe ser menor o igual a lo Pendiente";
            errorMessage = "- Cantidad a Ingresar: debe ser menor o igual a lo Pendiente";
        }
    } 

    if (errorMessage !== null && errorMessage !== "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorDetail.SetText(msgErrorAux);
        $("#GridMessageErrorDetail").show();

    }
}

function ComboBoxMachineProdOpeningDetailCogellingFresh_BeginCallback(s, e) {
	var emisionDate = DateTimeEmision.GetValue();
	var yearEmisionDate = emisionDate.getFullYear();
	var monthEmisionDate = emisionDate.getMonth();
	var dayEmisionDate = emisionDate.getDate();

	e.customArgs["dateTimeEmision"] = emisionDate === undefined ? null : yearEmisionDate + "-" +
		(++monthEmisionDate).toString().padStart(2, 0) + "-" +
		dayEmisionDate.toString().padStart(2, 0) + "T00:00:00";
}

function DateTimeEmision_DateChanged(s, e) {
	//ComboBoxMachineProdOpeningDetailCogellingFresh.PerformCallback();
	ComboBoxMachineProdOpeningDetailCogellingFresh.SetValue(null);
	RefreshComboBoxMachineProdOpeningDetailCogellingFresh(true);
}

function ComboBoxMachineProdOpeningDetailCogellingFresh_SelectedIndexChanged(s, e) {
	RefreshComboBoxMachineProdOpeningDetailCogellingFresh(false);
	//$.ajax({
	//    url: "InventoryMovePlantTransfer/MachineForProdCogellingFreshChanged",
	//    type: "post",
	//    data: { id_machineProdOpeningDetailCogellingFresh: ComboBoxMachineProdOpeningDetailCogellingFresh.GetValue() },
	//    async: true,
	//    cache: false,
	//    error: function (error) {
	//        console.log(error);
	//    },
	//    beforeSend: function () {
	//        //showLoading();
	//    },
	//    success: function (result) {
	//        if (result !== null) {
	//            turnCogellingFresh.SetText(result.turnCogellingFresh);
	//            warehouseEntry.SetText(result.warehouseEntry);
	//            $('#timeInitTurn').val(result.timeInitTurn);
	//            $('#timeEndTurn').val(result.timeEndTurn);
	//            GridViewDetails.PerformCallback();
	//        }
	//    },
	//    complete: function () {
	//        //hideLoading();
	//    }
	//});

}

function RefreshComboBoxMachineProdOpeningDetailCogellingFresh(refreshComboBox) {
	$.ajax({
		url: "InventoryMovePlantTransfer/MachineForProdCogellingFreshChanged",
		type: "post",
		data: { id_machineProdOpeningDetailCogellingFresh: ComboBoxMachineProdOpeningDetailCogellingFresh.GetValue() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
			//showLoading();
		},
		success: function (result) {
			if (result !== null) {
				if (result.message !== null && result.message !== "") {
					NotifyError("Error. " + result.message);
				}
				turnCogellingFresh.SetText(result.turnCogellingFresh);
				warehouseEntry.SetText(result.warehouseEntry);
				$('#timeInitTurn').val(result.timeInitTurn);
				$('#timeEndTurn').val(result.timeEndTurn);
				GridViewDetails.PerformCallback();
				if (refreshComboBox) {
					ComboBoxMachineProdOpeningDetailCogellingFresh.PerformCallback();
				}
			}
		},
		complete: function () {
			//hideLoading();
		}
	});
}

function RefreshDetail(s, e) {
	GridViewDetails.PerformCallback();
}

function ShowCurrentItem(enabled) {
	var data = {
		id: $('#id_inventoryMovePlantTransfer').val(),
		ids: [],
		enabled: enabled
	};

	showPage("InventoryMovePlantTransfer/Edit", data);
}

function AddNewItem() {
	$.ajax({
		url: "InventoryMovePlantTransfer/PendingNew",
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

const DOCUMENT_CODE_TRANSFERENCIA_PLANTA = "135";

function AprovedItem() {
	showLoading();
	$.ajax({
		url: 'InventoryMovePlantTransfer/Approve',
		type: 'post',
		data: { id: $('#id_inventoryMovePlantTransfer').val() },
		async: true,
		cache: false,
		error: function (result) {
			hideLoading();
			NotifyError("Error. " + result.Message);
		},
		success: function (result)
		{
			if (result.Code == CODE_FOR_SCHEDULE_TRANSAC) {
				
				hideLoading();
				callbackProcessControlState(false);
				observerNotification(DOCUMENT_CODE_TRANSFERENCIA_PLANTA, 5000, callbackProcess);
				NotifySuccess(TRANSAC_FOR_QUEUE_MSG);
			}
			else if (result.Code !== 0 && result.Code != CODE_FOR_SCHEDULE_TRANSAC) {
				hideLoading();
				NotifyError("Error al Aprobar." + result.Message);
			}
			else {
				hideLoading();
				NotifySuccess("Transferencia de Planta Aprobada Satisfactoriamente. " + "estado: " + result.Data);
				ShowCurrentItem(false);

			}

			//if (result.Code !== 0) {
			//	hideLoading();
			//	NotifyError("Error al Aprobar. " + result.Message);
			//	return;
			//}
			//
			//ShowCurrentItem(false);
			////hideLoading();
			//NotifySuccess("Transferencia de Planta Aprobada Satisfactoriamente. " + "Estado: " + result.Data);
		}
	});
}

function callbackProcessControlState(isEnabled) {

	btnEdit.SetEnabled(isEnabled);
	btnAproved.SetEnabled(isEnabled);
	btnAnnul.SetEnabled(isEnabled);
	btnExit.SetEnabled(isEnabled);
}

function callbackProcess(status) {

	if (status == "APROBADA") {
		NotifySuccess("Proceso realizado Satisfactoriamente, estado: APROBADA");
		ShowCurrentItem(false);
	}
	else if (status == "PENDIENTE") {
		NotifyError("Ha ocurrido un error, revise las notificaciones");
	}

	callbackProcessControlState(true);

}


function AprovedCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Aprobar la Transferencia de Planta?", "Confirmar");
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

// Conciliar
function ConciliateCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("¿Desea Conciliar la Transferencia de Planta?", "Confirmar");
	result.done(function (dialogResult) {

		if (dialogResult) {
			if ($("#enabled").val() == "true") {
				SaveItem(true);
			} else {
				ConciliateItem();
			}
		}
	});
}

function ConciliateItem() {
	showLoading();
	$.ajax({
		url: 'InventoryMovePlantTransfer/Conciliate',
		type: 'post',
		data: { id: $('#id_inventoryMovePlantTransfer').val() },
		async: true,
		cache: false,
		error: function (result) {
			hideLoading();
			NotifyError("Error. " + result.Message);
		},
		success: function (result) {
			if (result.Code !== 0) {
				hideLoading();
				NotifyError("Error al Conciliar. " + result.Message);
				return;
			}

			ShowCurrentItem(false);
			//hideLoading();
			NotifySuccess("Transferencia de Planta Conciliada Satisfactoriamente. " + "Estado: " + result.Data);
		}
	});
}

function ReverseCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Reversar la Transferencia de Planta?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'InventoryMovePlantTransfer/Reverse',
				type: 'post',
				data: { id: $('#id_inventoryMovePlantTransfer').val() },
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
					//hideLoading();
					NotifySuccess("Transferencia de Planta Reversada Satisfactoriamente. " + "Estado: " + result.Data);
				}
			});
		}
	});
}

function AnnulCurrentItem() {
	var result = DevExpress.ui.dialog.confirm("Desea Anular la Transferencia de Planta?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			showLoading();
			$.ajax({
				url: 'InventoryMovePlantTransfer/Annul',
				type: 'post',
				data: { id: $('#id_inventoryMovePlantTransfer').val() },
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
					//hideLoading();
					NotifySuccess("Transferencia de Planta Anulada Satisfactoriamente. " + "Estado: " + result.Data);
				}
			});
		}
	});
}

function SaveDataUser() {
	var emisionDate = DateTimeEmision.GetValue();
	var yearEmisionDate = emisionDate.getFullYear();
	var monthEmisionDate = emisionDate.getMonth();
	var dayEmisionDate = emisionDate.getDate();

	var dateTimeEntryAux = dateTimeEntry.GetDate();
	var yearLiquidationDate = dateTimeEntryAux.getFullYear();
	var monthLiquidationDate = dateTimeEntryAux.getMonth();
	var dayLiquidationDate = dateTimeEntryAux.getDate();

	var userData = {
		id: $('#id_inventoryMovePlantTransfer').val(),
		//id_machineProdOpening: $('#id_machineProdOpening').val(),
		dateTimeEmision: yearEmisionDate + "-" +
			(++monthEmisionDate).toString().padStart(2, 0) + "-" +
			dayEmisionDate.toString().padStart(2, 0) + "T00:00:00",
		//DateTimeEmision.GetValue(),
		dateTimeEntry: yearLiquidationDate + "-" +
			(++monthLiquidationDate).toString().padStart(2, 0) + "-" +
			dayLiquidationDate.toString().padStart(2, 0) + "T" +
			dateTimeEntryAux.getHours().toString().padStart(2, 0) + ":" +
			dateTimeEntryAux.getMinutes().toString().padStart(2, 0) + ":00",
		id_machineProdOpeningDetail: ComboBoxMachineProdOpeningDetailCogellingFresh.GetValue(),
		id_receiver: ComboBoxReceiver.GetValue(),
		description: MemoDescription.GetText()
		//Detail

	};

	var InventoryMovePlantTransfer = {
		jsonInventoryMovePlantTransfer: JSON.stringify(userData)
	};

	return InventoryMovePlantTransfer;
}

function SaveItem(aproved) {
	showLoading();

	if (!Validate()) {
		hideLoading();
		return;
	}

	$.ajax({
		url: 'InventoryMovePlantTransfer/Save',
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
			$('#id_inventoryMovePlantTransfer').val(id);

			if (aproved)
				AprovedItem();
			else {
				ShowCurrentItem(true);

				//hideLoading();
			}
			NotifySuccess("La Transferencia de Planta Guardada Satisfactoriamente.");
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

	if (!IsValid(DateTimeEmision) || DateTimeEmision.GetValue() === null) {
		errors += "Fecha Emisión es un campo Obligatorio. \n\r";
		validate = false;
	}
	if (!IsValid(ComboBoxMachineProdOpeningDetailCogellingFresh) || ComboBoxMachineProdOpeningDetailCogellingFresh.GetValue() === null) {
		errors += "Máquina(Congelamiento y Freso) es un campo Obligatorio. \n\r";
		validate = false;
	}
	if (!IsValid(dateTimeEntry) || dateTimeEntry.GetValue() === null) {
		errors += "Fecha y Hora de Ingreso es un campo Obligatorio. \n\r";
		validate = false;
	} else {
		var emisionDate = DateTimeEmision.GetValue();
		var yearEmisionDate = emisionDate.getFullYear();
		var monthEmisionDate = emisionDate.getMonth();
		var dayEmisionDate = emisionDate.getDate();
		//var emisionDateAux = new Date(yearEmisionDate, monthEmisionDate, dayEmisionDate);

		//if (dateLiquidationDateAux < emisionDateAux) {
		//    validate = false;
		//    errors += "Fecha Cierre debe ser mayor e igual a la Fecha Emisión. \n\r";
		//}
		//// 
		if ($('#timeInitTurn').val() !== null && $('#timeInitTurn').val() !== "" && $('#timeEndTurn').val() !== null && $('#timeEndTurn').val() !== "") {
			var dateTimeEntryAux = dateTimeEntry.GetDate();
			var yearLiquidationDate = dateTimeEntryAux.getFullYear();
			var monthLiquidationDate = dateTimeEntryAux.getMonth();
			var dayLiquidationDate = dateTimeEntryAux.getDate();
			var dateLiquidationDateAux = new Date(yearLiquidationDate, monthLiquidationDate, dayLiquidationDate, dateTimeEntryAux.getHours(), dateTimeEntryAux.getMinutes(), 0);

			//var emisionDate = dateTimeEmisionStr.GetValue().split("-");
			//var yearEmisionDate = parseInt(emisionDate[2]);
			//var monthEmisionDate = parseInt(emisionDate[1]);
			//var dayEmisionDate = parseInt(emisionDate[0]);
			//var emisionDateAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate);
			//var emisionDatePlusAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate + 1);

			var timeInitTurnAux = $('#timeInitTurn').val();
			var timeInitTurnAuxArray = timeInitTurnAux.split(":");
            var dateInitTurnWhithTimeAux = new Date(yearLiquidationDate, monthLiquidationDate, dayLiquidationDate, timeInitTurnAuxArray[0], timeInitTurnAuxArray[1], 0);
            //var dateInitTurnWhithTimeAux = new Date(yearEmisionDate, monthEmisionDate, dayEmisionDate, timeInitTurnAuxArray[0], timeInitTurnAuxArray[1], 0);

			var timeEndTurnAux = $('#timeEndTurn').val();
			var timeEndTurnAuxArray = timeEndTurnAux.split(":");
            var dateEndTurnWhithTimeAux = new Date(yearLiquidationDate, monthLiquidationDate, dayLiquidationDate, timeEndTurnAuxArray[0], timeEndTurnAuxArray[1], 0);
            //var dateEndTurnWhithTimeAux = new Date(yearEmisionDate, monthEmisionDate, dayEmisionDate, timeEndTurnAuxArray[0], timeEndTurnAuxArray[1], 0);

			if (!ValidateRangeTime(dateInitTurnWhithTimeAux, dateEndTurnWhithTimeAux, false)) {
				dateEndTurnWhithTimeAux = new Date(yearEmisionDate, monthEmisionDate, dayEmisionDate + 1, timeEndTurnAuxArray[0], timeEndTurnAuxArray[1], 0);
			}

			//var dateInicioWhithTimeAux = new Date(dateInicioAux.getFullYear(), dateInicioAux.getMonth(), dateInicioAux.getDate(), startTimeValue.getHours(), startTimeValue.getMinutes(), 0);
            if ((dateInitTurnWhithTimeAux <= dateEndTurnWhithTimeAux && (dateInitTurnWhithTimeAux > dateLiquidationDateAux || dateEndTurnWhithTimeAux < dateLiquidationDateAux)) ||
                (dateInitTurnWhithTimeAux > dateEndTurnWhithTimeAux && dateInitTurnWhithTimeAux > dateLiquidationDateAux && dateEndTurnWhithTimeAux < dateLiquidationDateAux))  {
				//var emisionDateStr = dayEmisionDate.toString().padStart(2, 0) + "/" + (++monthEmisionDate).toString().padStart(2, 0) + "/" + yearEmisionDate;
				errors += "La Fecha y Hora de Ingreso debe estar dentro del horario del turno: " + turnCogellingFresh.GetValue() + /*", con la fecha de emisión del documento(" + emisionDateStr + ")*/". \n\r";
				validate = false;
			}
		}
	}
	if (!IsValid(ComboBoxReceiver) || ComboBoxReceiver.GetValue() === null) {
		errors += "Recibidor es un campo Obligatorio. \n\r";
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
	showPage("InventoryMovePlantTransfer/Index");
}

function InitializePagination() {

	if ($("#id_inventoryMovePlantTransfer").val() !== 0) {

		var current_page = 1;
		var max_page = 1;
		$.ajax({
			url: "InventoryMovePlantTransfer/InitializePagination",
			type: "post",
			data: { id: $("#id_inventoryMovePlantTransfer").val() },
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
				showPage("InventoryMovePlantTransfer/Pagination", { page: page });
			}
		});
	}
}

function PrintItem() {
	$.ajax({
		url: "InventoryMovePlantTransfer/PrintReport",
		type: "post",
		data: {
			id_inventoryMovePlantTransfer: $("#id_inventoryMovePlantTransfer").val(),
			codeReport: "FCAM01"
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

function Init() {
}

$(function () {
	InitializePagination();
	Init();
});