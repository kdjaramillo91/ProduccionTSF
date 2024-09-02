function Turn_Init(s, e) {
	var id_TurnAux = id_Turn.GetValue();
	if (id_TurnAux == null || id_TurnAux == undefined || id_TurnAux == 0) {
		id_Turn.SetValue(null);
	} else {
		Turn_SelectedIndexChanged(s, e);
	}
}

function Turn_SelectedIndexChanged(s, e) {
	$.ajax({
		url: "MachineProdOpening/GetTimesTurn",
		type: "post",
		data: { id_turn: id_Turn.GetValue() },
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
		},
		success: function (result) {
			if (result !== null) {
				timeInitTurn.SetValue(new Date(2011, 1, 1, result.timeInitTurn.Hours, result.timeInitTurn.Minutes, result.timeInitTurn.Seconds))
				timeEndTurn.SetValue(new Date(2011, 1, 1, result.timeEndTurn.Hours, result.timeEndTurn.Minutes, result.timeEndTurn.Seconds))
			}
		},
		complete: function () {
		}
	});
}

function IsValidateTimeInRange(e, timeInitRange, timeEndRange, timeCurrent, msg) {
	var time00Aux = new Date("2011-01-01T00:00:00");
	var time11Aux = new Date("2011-01-01T11:59:59");
	var diurnoTimeInitRange = false;
	var diurnoTimeEndRange = false;
	var diurnoTimeCurrent = false;

	var mayorTimeCurrentTimeInitRange = false;
	var menorTimeCurrentTimeEndRange = false;
	var valido = false;
	valido = OnRangeTimeValidation(e, time00Aux, timeInitRange, msg);
	if (valido) {
		valido = OnRangeTimeValidation(e, timeInitRange, time11Aux, msg);
		if (valido) {
			diurnoTimeInitRange = valido;
		}
	}

	valido = OnRangeTimeValidation(e, time00Aux, timeEndRange, msg);
	if (valido) {
		valido = OnRangeTimeValidation(e, timeEndRange, time11Aux, msg);
		if (valido) {
			diurnoTimeEndRange = valido;
		}
	}

	valido = OnRangeTimeValidation(e, time00Aux, timeCurrent, msg);
	if (valido) {
		valido = OnRangeTimeValidation(e, timeCurrent, time11Aux, msg);
		if (valido) {
			diurnoTimeCurrent = valido;
		}
	}

	valido = OnRangeTimeValidation(e, timeInitRange, timeCurrent, msg);
	mayorTimeCurrentTimeInitRange = valido;

	valido = OnRangeTimeValidation(e, timeCurrent, timeEndRange, msg, false);
	menorTimeCurrentTimeEndRange = valido;

	valido = ((diurnoTimeInitRange == diurnoTimeCurrent && mayorTimeCurrentTimeInitRange) || diurnoTimeInitRange != diurnoTimeCurrent) &&
		((diurnoTimeEndRange == diurnoTimeCurrent && menorTimeCurrentTimeEndRange) || diurnoTimeEndRange != diurnoTimeCurrent);
	return valido;
}

function Update(approve) {
	var valid = true;
	var validDocumentMachineProdOpening = ASPxClientEdit.ValidateEditorsInContainerById("documentMachineProdOpening", null, true);
	var validGroupItem1FormLayoutEditMachineProdOpening = ASPxClientEdit.ValidateEditorsInContainerById("groupItem1FormLayoutEditMachineProdOpening", null, true);

	if (validDocumentMachineProdOpening) {
		UpdateTabImage({ isValid: true }, "tabDocument");
	} else {
		UpdateTabImage({ isValid: false }, "tabDocument");
		valid = false;
	}

	if (validGroupItem1FormLayoutEditMachineProdOpening) {
		UpdateTabImage({ isValid: true }, "tabMachineProdOpeningDetails");

		if (gvMachineProdOpeningDetailEditForm.IsEditing()) {
			UpdateTabImage({ isValid: false }, "tabMachineProdOpeningDetails");
			valid = false;
		} else {
			UpdateTabImage({ isValid: true }, "tabMachineProdOpeningDetails");
		}
	} else {
		UpdateTabImage({ isValid: false }, "tabMachineProdOpeningDetails");
		valid = false;
	}

	if (valid) {
		var id = $("#id_machineProdOpening").val();

		var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditMachineProdOpening").serialize();

		var url = (id === "0") ? "MachineProdOpening/MachineProdOpeningAddNew"
			: "MachineProdOpening/MachineProdOpeningUpdate";

		showForm(url, data);
	}
}

function ButtonUpdate_Click(s, e) {
	Update(false);
}

function ButtonUpdateClose_Click(s, e) {
	var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

	if (valid) {
		var id = $("#id_invoiceCommercial").val();

		var data = "id=" + id + "&" + $("#formEditInvoiceCommercial").serialize();

		var url = (id === "0") ? "InvoiceCommercial/InvoiceCommercialsAddNew"
			: "InvoiceCommercial/InvoiceCommercialsUpdate";

		if (data != null) {
			$.ajax({
				url: url,
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
					console.log(result);
				},
				complete: function () {
					hideLoading();
					showPage("InvoiceCommercial/Index", null);
				}
			});
		}
	}
}

function ButtonCancel_Click(s, e) {
	showPage("MachineProdOpening/Index", null);
}

function AddNewDocument(s, e) {
	AddNewItemMachineProdOpening(s, e);
}

function SaveDocument(s, e) {
	ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
	ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
}

function ApproveDocument(s, e) {
	showConfirmationDialog(function () {
		Update(true);
	}, "¿Desea aprobar la Apertura de Máquina?");
}

function AutorizeDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_invoiceCommercial").val()
		};
		showForm("InvoiceCommercial/Autorize", data);
	}, "¿Desea autorizar la Factura Comercial?");
}

function ProtectDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_machineProdOpening").val()
		};
		showForm("MachineProdOpening/Protect", data);
	}, "¿Desea cerrar la Apertura de Máquina?");
}

function CancelDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_machineProdOpening").val()
		};
		showForm("MachineProdOpening/Cancel", data);
	}, "¿Desea anular la Apertura de Máquina?");
}

function RevertDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_machineProdOpening").val()
		};
		showForm("MachineProdOpening/Revert", data);
	}, "¿Desea reversar la Apertura de Máquina?");
}

function ShowDocumentHistory(s, e) {
}

function PrintDocument(s, e) {
	$.ajax({
		url: "InvoiceCommercial/InvoiceCommercialReport",
		type: "post",
		data: { id: $("#id_invoiceCommercial").val() },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
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

function SetElementVisibility(id, visible) {
	var $element = $("#" + id);
	visible ? $element.show() : $element.hide();
}

function AutoCloseAlert() {
	if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
		setTimeout(function () {
			$(".alert-success").alert('close');
		}, 2000);
	}
}

function UpdateView() {
	var id = parseInt($("#id_machineProdOpening").val());

	btnCopy.SetVisible(false);
	btnAutorize.SetVisible(false);
	btnPrint.SetVisible(false);
	$.ajax({
		url: "MachineProdOpening/Actions",
		type: "post",
		data: { id: id },
		async: true,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
		},
		success: function (result) {
			btnApprove.SetEnabled(result.btnApprove);
			btnProtect.SetEnabled(result.btnProtect);
			btnCancel.SetEnabled(result.btnCancel);
			btnRevert.SetEnabled(result.btnRevert);
			btnSave.SetEnabled(result.btnSave);
		},
		complete: function (result) {
		}
	});

	btnHistory.SetEnabled(id !== 0);

	btnPrint.SetEnabled(id !== 0);
	btnNew.SetEnabled(id !== 0);

	if (codeDocumentState == "01") {
		btnRemoveDetail.SetVisible(false);
	}
}

function UpdatePagination() {
	var current_page = 1;
	$.ajax({
		url: "MachineProdOpening/InitializePagination",
		type: "post",
		data: { id_machineProdOpening: $("#id_machineProdOpening").val() },
		async: false,
		cache: false,
		error: function (error) {
			console.log(error);
		},
		beforeSend: function () {
		},
		success: function (result) {
			$("#pagination").attr("data-max-page", result.maximunPages);
			current_page = result.currentPage;
		},
		complete: function () {
		}
	});
	$('.pagination').current_page = current_page;
}

function init() {
	UpdatePagination();

	AutoCloseAlert();
}

$(function () {
	var chkReadyState = setInterval(function () {
		if (document.readyState === "complete") {
			clearInterval(chkReadyState);
			UpdateView();
		}
	}, 100);

	init();
});