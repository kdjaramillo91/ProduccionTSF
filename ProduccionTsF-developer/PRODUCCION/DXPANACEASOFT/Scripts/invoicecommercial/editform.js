function Update(approve) {
	var valid = true;
	var validDocumentInvoiceCommercial = ASPxClientEdit.ValidateEditorsInContainerById("documentInvoiceCommercial", null, true);
	var validFormLayoutEditInvoiceCommercialMainFormTab = ASPxClientEdit.ValidateEditorsInContainerById("formLayoutEditInvoiceCommercialMainFormTab", null, true);



	if (validDocumentInvoiceCommercial) {
		UpdateTabImage({ isValid: true }, "tabDocument");
	} else {
		UpdateTabImage({ isValid: false }, "tabDocument");
		valid = false;
	}

	if (validFormLayoutEditInvoiceCommercialMainFormTab) {
		UpdateTabImage({ isValid: true }, "tabInvoiceCommercial");
		UpdateTabImage({ isValid: true }, "tabInvoiceCommercial");
	} else {
		UpdateTabImage({ isValid: false }, "tabInvoiceCommercial");
		valid = false;
	}

	if (gvInvoiceCommercialEditFormDetail.cpRowsCount === 0 || gvInvoiceCommercialEditFormDetail.IsEditing()) {
		UpdateTabImage({ isValid: false }, "tabInvoiceCommercialDetails");
		valid = false;
	} else {
		UpdateTabImage({ isValid: true }, "tabInvoiceCommercialDetails");
	}

	if (valid) {

		var id = $("#id_invoiceCommercial").val();

		var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditInvoiceCommercial").serialize();

		var url = (id === "0") ? "InvoiceCommercial/InvoiceCommercialsAddNew"
			: "InvoiceCommercial/InvoiceCommercialsUpdate";

		var enableAdd = function () {
			btnNew.SetEnabled(true);
		}

		showForm(url, data, enableAdd);

	}
}

// DIALOG BUTTONS ACTIONS
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
	showPage("InvoiceCommercial/Index", null);
}

function RefreshDetail(s, e) {
	gvInvoiceCommercialEditFormDetail.UnselectRows();
	gvInvoiceCommercialEditFormDetail.PerformCallback();

}

// BUTTONS ACTION 
function AddNewDocument(s, e) {
	var data =
	{
		id: 0
	};
	showPage("InvoiceCommercial/InvoiceCommercialFormEditPartial", data);
}

function SaveDocument(s, e) {
	console.log("llego save");
	ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
	ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
	showPage("InvoiceCommercial/InvoiceCopy", { id: $("#id_invoiceCommercial").val() });
}

function ApproveDocument(s, e) {

	showConfirmationDialog(function () {
		Update(true);
	}, "¿Desea aprobar la Factura Comercial?");
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
}

function CancelDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_invoiceCommercial").val()
		};
		showForm("InvoiceCommercial/Cancel", data);
	}, "¿Desea anular la Factura?");
}

function RevertDocument(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_invoiceCommercial").val()
		};
		showForm("InvoiceCommercial/Revert", data);
	}, "¿Desea reversar la Factura Comercial?");
}

function ShowDocumentHistory(s, e) {
}

function PrintDocument(s, e) {

	$.ajax({
		url: "InvoiceCommercial/InvoiceCommercialExporExcel",
		type: "post",
		data: { id_invoice: $("#id_invoiceCommercial").val() },
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

//Print Partial
function ExportExcel(s, e) {
	var data = { id_invoice: $("#id_invoiceCommercial").val() };

	$.ajax({
		url: 'InvoiceCommercial/InvoiceCommercialExporExcel',
		data: data,
		async: true,
		cache: false,
		type: 'POST',
		beforeSend: function () {
			showLoading();
		},
		success: function (result) {
			try {
				if (result != undefined) {
					var reportTdr = result.nameQS;
					var url = 'ReportProd/DownLoadExcelInvoiceComercial';
					newWindow = window.open(url, '_self', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0', false);
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

// DETAILS ACTIONS

function AddNewDetail(s, e) {
	gvInvoiceCommercialEditFormDetail.AddNewRow();
	btnNew.SetEnabled(false);
}

function RemoveDetail(s, e) {

	gvInvoiceCommercialEditFormDetail.GetSelectedFieldValues("id_item", function (values) {
		var selectedRows = [];
		for (var i = 0; i < values.length; i++) {
			selectedRows.push(values[i]);
		}
		genericAjaxCall('InvoiceCommercial/InvoiceCommercialDetailsDeleteSeleted', true, { ids: selectedRows }, function (error) { console.log(error) }, null, null, function () {
			gvInvoiceCommercialEditFormDetail.PerformCallback();
		})
	});
}

function SetElementVisibility(id, visible) {
	var $element = $("#" + id);
	visible ? $element.show() : $element.hide();
}

function AddNewAttachedDocumentDetail(s, e) {
	gvAttachedDocuments.AddNewRow();
}

function RemoveAttachedDocumentDetail(s, e) {
}

function RefreshAttachedDocumentDetail(s, e) {
	gvAttachedDocuments.UnselectRows();
	gvAttachedDocuments.PerformCallback();
}

function AttachedUploadComplete(s, e) {
	var userData = JSON.parse(e.callbackData);
	$("#guid").val(userData.guid);
	$("#url").val(userData.url);
	attachmentName.SetText(userData.filename);
}

var attachmentNameIniAux = null;

function AttachedName_OnInit(s, e) {
	attachmentNameIniAux = s.GetText();
}

function AttachedNameValidate(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Archivo Obligatorio";
	} else {
		var guid = $("#guidAttachment").val();
		if (guid === null || guid.length === 0) {
			e.isValid = false;
			e.errorText = "Archivo No Cargado Completamente.Intentelo de nuevo";
		} else {
			var url = $("#urlAttachment").val();
			if (guid === null || guid.length === 0) {
				e.isValid = false;
				e.errorText = "Archivo No Cargado Completamente.Intentelo de nuevo";
			} else {
				var data = {
					attachmentNameNew: e.value
				};
				if (data.attachmentNameNew != attachmentNameIniAux) {
					$.ajax({
						url: "InvoiceCommercial/ItsRepeatedAttachmentDetail",
						type: "post",
						data: data,
						async: false,
						cache: false,
						error: function (error) {
							console.log(error);
						},
						beforeSend: function () {
						},
						success: function (result) {
							if (result !== null) {
								if (result.itsRepeated == 1) {
									e.isValid = false;
									e.errorText = result.Error;
								}
							}
						},
						complete: function () {
						}
					});
				}
			}
		}
	}

}

function InvoiceCommercial_OnBeginCallback(s, e) {
	e.customArgs["id_invoice"] = s.cpIdInvoiceCommercial;
}

function gvAttachedDocumentsCustomCommandButton_Click(s, e) {
	if (e.buttonID === "btnUpdate") {
		console.log("e.buttonID: " + e.buttonID);
		var valid = true;
		var validAttachmentFormUpLoad = ASPxClientEdit.ValidateEditorsInContainerById("attachment-form-upLoad", null, true);
		console.log("validAttachmentFormUpLoad: " + validAttachmentFormUpLoad);
		if (validAttachmentFormUpLoad) {
			UpdateTabImage({ isValid: true }, "tabAttachedDocument");
		} else {
			UpdateTabImage({ isValid: false }, "tabAttachedDocument");
			valid = false;
		}
		console.log("valid: " + valid);

		if (valid) {
			gvAttachedDocuments.UpdateEdit();
		}
	}
}


// UPDATE VIEW
function AutoCloseAlert() {
	if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
		setTimeout(function () {
			$(".alert-success").alert('close');
		}, 2000);
	}
}

function UpdateView() {
	var id = parseInt($("#id_invoiceCommercial").val());

	btnCopy.SetEnabled(id !== 0);
	btnAutorize.SetVisible(false);
	btnProtect.SetVisible(false);

	// STATES BUTTONS
	$.ajax({
		url: "InvoiceCommercial/Actions",
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
			btnCancel.SetEnabled(result.btnCancel);
			btnRevert.SetEnabled(result.btnRevert);
			btnSave.SetEnabled(result.btnSave);
		},
		complete: function (result) {
		}
	});

	// HISTORY BUTTON
	btnHistory.SetEnabled(id !== 0);

	// PRINT BUTTON
	btnPrint.SetEnabled(id !== 0);
	btnExcel.SetEnabled(id !== 0);

	var codeDocumentState = $("#codeDocumentState").val();

	//if (codeDocumentState == "01") {
	//	btnRemoveContainer.SetVisible(false);
	//}
}

function UpdatePagination() {
	var current_page = 1;
	$.ajax({
		url: "InvoiceCommercial/InitializePagination",
		type: "post",
		data: { id_invoiceCommercial: $("#id_invoiceCommercial").val() },
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
function ValueChangeValueDiscount(s, e) {
	UpdateTotalValues();
	gvInvoiceCommercialEditFormDetail.PerformCallback();
}

function ValueChangeTotalFreight(s, e) {
	UpdateTotalValues();
}

function ValidateTotalFreight(s, e) {

	var oTermsNegotiation = ASPxClientControl.GetControlCollection().GetByName("id_termsNegotiation");//.GetValue();
	if (typeof oTermsNegotiation != 'undefined' && oTermsNegotiation != null && oTermsNegotiation.GetSelectedItem() != null) {
		var codeTermsNegotiation = oTermsNegotiation.GetSelectedItem().GetColumnText("code");
		if (typeof codeTermsNegotiation != 'undefined' && codeTermsNegotiation != null) {
			if (codeTermsNegotiation === 'FOBFLET') {
				if (s.GetValue() == null || s.GetValue() == 0) {

					e.isValid = false;
					e.errorText = "Término de Negociación: " + 'PRECIO FOB+FLETE' + ", requiere el ingreso de valor de Transporte.";
				}
			}
		}
	}
	else {
		if (s.GetValue() != null || s.GetValue() != 0) {
			e.SetValue(0);
			e.isValid = false;
			e.errorText = "Término de Negociación seleccionado no requiere el ingreso de valor de Transporte.";

		}
	}
	e.isValid = true;
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
