function Update(approve) {
	if (tabControl.cpLoteManual) {
		// generar nueva funcioón de guardar con las validaciones a los tab's var valid = true;
		var valid = true;
		var validDocumentProductionLot = ASPxClientEdit.ValidateEditorsInContainerById("documentProductionLot", null, true);
		var validReceptionProductionLot = ASPxClientEdit.ValidateEditorsInContainerById("receptionProductionLot", null, true);
		var loteManual = tabControl.cpLoteManual;

		if (validDocumentProductionLot) {
			UpdateTabImage({ isValid: true }, "tabDocument");
		} else {
			UpdateTabImage({ isValid: false }, "tabDocument");
			valid = false;
		}

		if (validReceptionProductionLot) {
			UpdateTabImage({ isValid: true }, "tabReception");
		} else {
			UpdateTabImage({ isValid: false }, "tabReception");
			valid = false;
		}

		if (valid) {
			// 
			var id = $("#id_productionLot").val();

			var data = "id=" + id + "&" + "approve=" + approve + "&" + "loteManual=" + loteManual + "&" + $("#formEditProductionLotReception").serialize();

			var url = (id === "0") ? "ProductionLotReception/ProductionLotReceptionsAddNew"
				: "ProductionLotReception/ProductionLotReceptionsUpdate";

			showForm(url, data);
		}

	} else {
		UpdateAll(approve);
	}
}


function UpdateAll(approve) {

	var valid = true;
	var validDocumentProductionLot = ASPxClientEdit.ValidateEditorsInContainerById("documentProductionLot", null, true);
	var validReceptionProductionLot = ASPxClientEdit.ValidateEditorsInContainerById("receptionProductionLot", null, true);
	var validMainTabPurchaseOrder = ASPxClientEdit.ValidateEditorsInContainerById("mainTabPurchaseOrder", null, true);


	if (validDocumentProductionLot) {
		UpdateTabImage({ isValid: true }, "tabDocument");
	} else {
		UpdateTabImage({ isValid: false }, "tabDocument");
		valid = false;
	}

	if (validReceptionProductionLot) {
		UpdateTabImage({ isValid: true }, "tabReception");
	} else {
		UpdateTabImage({ isValid: false }, "tabReception");
		valid = false;
	}

	var codeState = $("#codeState").val();

	if (codeState == "01") {
		// 
		//if (gvProductionLotReceptionEditFormItemsDetail.cpRowsCount === 0 || gvProductionLotReceptionEditFormItemsDetail.IsEditing()) {
		//    UpdateTabImage({ isValid: false }, "tabItemsDetails");
		//    valid = false;
		//} else {
		//    UpdateTabImage({ isValid: true }, "tabItemsDetails");
		//}

		//if (gvProductionLotReceptionEditFormMaterialsDetail.IsEditing()) {
		//    UpdateTabImage({ isValid: false }, "tabMaterialsDispatch");
		//    valid = false;
		//} else {
		//    UpdateTabImage({ isValid: true }, "tabMaterialsDispatch");
		//}
	}

	if (codeState == "02" || codeState == "03") {

		if (typeof (gvProductionLotEditFormProductionLotLiquidationsDetail) !== "undefined") {
			if (gvProductionLotEditFormProductionLotLiquidationsDetail.cpRowsCount === 0 ||
				gvProductionLotEditFormProductionLotLiquidationsDetail.IsEditing()) {
				UpdateTabImage({ isValid: false }, "tabProductionLotLiquidationsDetails");
				valid = false;
			} else {
				UpdateTabImage({ isValid: true }, "tabProductionLotLiquidationsDetails");
			}
        } else {
            var validFormLayoutEditProductionLotSummaryLiq = ASPxClientEdit.ValidateEditorsInContainerById("formLayoutEditProductionLotSummaryLiq", null, true);
            if (validFormLayoutEditProductionLotSummaryLiq) {
                UpdateTabImage({ isValid: true }, "tabProductionLotLiquidationsDetails");
            } else {
                UpdateTabImage({ isValid: false }, "tabProductionLotLiquidationsDetails");
                valid = false;
            } 
			if (gvProductionLotDetailProductionLotLiquidationTotals.cpRowsCount === 0) {
				UpdateTabImage({ isValid: false }, "tabProductionLotLiquidationsDetails");
				valid = false;
			} else {
				UpdateTabImage({ isValid: true }, "tabProductionLotLiquidationsDetails");
			}
		}
		if (gvProductionLotEditFormProductionLotPackingMaterialsDetail.IsEditing()) {
			UpdateTabImage({ isValid: false }, "tabProductionLotPackingMaterialDetails");
			valid = false;
		} else {
			UpdateTabImage({ isValid: true }, "tabProductionLotPackingMaterialDetails");
		}
		if (gvProductionLotEditFormProductionLotTrashsDetail.IsEditing()) {
			UpdateTabImage({ isValid: false }, "tabProductionLotTrashsDetails");
			valid = false;
		} else {
			UpdateTabImage({ isValid: true }, "tabProductionLotTrashsDetails");
		}
	}
	if (codeState == "04" || codeState == "05") {

		var validFormLayoutEditProductionLotSummary = ASPxClientEdit.ValidateEditorsInContainerById("formLayoutEditProductionLotSummary", null, true);
		if (validFormLayoutEditProductionLotSummary) {
			UpdateTabImage({ isValid: true }, "tabProductionLotCloseDetails");
		} else {
			UpdateTabImage({ isValid: false }, "tabProductionLotCloseDetails");
			valid = false;
		}
		if (valid) {
			if (gvProductionLotReceptionEditFormClosesDetail.IsEditing()) {
				UpdateTabImage({ isValid: false }, "tabProductionLotCloseDetails");
				valid = false;
			} else {
				UpdateTabImage({ isValid: true }, "tabProductionLotCloseDetails");
			}
		}

	}

	if (codeState == "06" || codeState == "07") {
		// 
		var validFormLayoutEditProductionLotPaymentSummary = ASPxClientEdit.ValidateEditorsInContainerById("formLayoutEditProductionLotPaymentSummary", null, true);
		//console.log(validFormLayoutEditProductionLotSummary);
		if (validFormLayoutEditProductionLotPaymentSummary) {
			UpdateTabImage({ isValid: true }, "tabProductionLotPaymentDetails");
		} else {
			UpdateTabImage({ isValid: false }, "tabProductionLotPaymentDetails");
			valid = false;
		}
	}
	if (valid) {
		// 
		var id = $("#id_productionLot").val();

		var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditProductionLotReception").serialize();

		var url = (id === "0") ? "ProductionLotReception/ProductionLotReceptionsAddNew"
			: "ProductionLotReception/ProductionLotReceptionsUpdate";

		showForm(url, data);
	}
}

function UpdateLot(s, e) {



}

// DIALOG BUTTONS ACTIONS
function ButtonUpdate_Click(s, e) {
	var id = $("#id_productionLot").val();

	var messageFinal = "En caso de no haber ingresado Cantidad de Escurrido, se utilizará la cantidad remitida para el detalle del Lote. ¿Desea Continuar? ";

	//if (id === "0") {
	//    showConfirmationDialog(function () {
	//        Update(false);
	//    }, "En caso de no haber ingresado Cantidad de Escurrido, se utilizará la cantidad remitida para el detalle del Lote. ¿Desea Continuar?");
	//}
	//else {
	Update(false);
	//}
}

function ButtonUpdateClose_Click(s, e) {
	var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

	if (valid) {
		var id = $("#id_productionLot").val();

		var data = "id=" + id + "&" + $("#formEditProductionLotReception").serialize();

		var url = (id === "0") ? "ProductionLotReception/ProductionLotReceptionsAddNew"
			: "ProductionLotReception/ProductionLotReceptionsUpdate";

		if (data != null) {
			$.ajax({
				url: url,
				type: "post",
				data: data,
				async: true,
				cache: false,
				error: function(error) {
					console.log(error);
				},
				beforeSend: function() {
					showLoading();
				},
				success: function(result) {
					console.log(result);
				},
				complete: function() {
					hideLoading();
					showPage("ProductionLotReception/Index", null);
				}
			});
		}
	}
}

function ButtonCancel_Click(s, e) {
	var isBtnToReturn = $("#isBtnToReturn").val();
	if (isBtnToReturn == "True" || isBtnToReturn == "true" || isBtnToReturn == true) {
		btnToReturn_click(s, e);
	} else {
		showPage("ProductionLotReception/Index", null);
	}

}

// BUTTONS ACTION 
function AddNewLot(s, e) {

	//var data = {
	//    id: 0
	//};

	//showPage("ProductionLotReception/ProductionLotReceptionFormEditPartial", data);
	// 
	showPage("ProductionLotReception/Index", null);
	AddNewItemFromRemisionGuide();
}

function SaveLot(s, e) {
	ButtonUpdate_Click(s, e);
}

function SaveCloseLot(s, e) {
	ButtonUpdateClose_Click(s, e);
}

function CopyLot(s, e) {
	showPage("ProductionLotReception/ProductionLotReceptionCopy", { id: $("#id_productionLot").val() });
}

function ApproveLot(s, e) {
	showConfirmationDialog(function() {
		Update(true);
	}, "¿Desea aprobar el lote?");
}

function AutorizeLot(s, e) {
	//showConfirmationDialog(function () {
	//    var data = {
	//        id: $("#id_productionLot").val()
	//    };
	//    showForm("ProductionLotReception/Autorize", data);
	//}, "¿Desea autorizar el lote?");
}

function ProtectLot(s, e) {
	//showConfirmationDialog(function () {
	//    var data = {
	//        id: $("#id_productionLot").val()
	//    };
	//    showForm("ProductionLotReception/Protect", data);
	//}, "¿Desea cerrar el lote?");
}

function CancelLot(s, e) {
	showConfirmationDialog(function() {
		var data = {
			id: $("#id_productionLot").val()
		};
		showForm("ProductionLotReception/Cancel", data);
	}, "¿Desea anular el lote?");
}

function RevertLot(s, e) {
	showConfirmationDialog(function() {
		var data = {
			id: $("#id_productionLot").val()
		};
		showForm("ProductionLotReception/Revert", data);
	}, "¿Desea reversar el lote?");
}

function ConciliationLot(s, e) {
	showConfirmationDialog(function () {
		var data = {
			id: $("#id_productionLot").val()
		};
		showForm("ProductionLotReception/Conciliation", data);
	}, "¿Desea conciliar el lote?");
}

function ShowLotHistory(s, e) {
}

function PrintLot(s, e) {

	$.ajax({
		url: "ProductionLotReception/ProductionLotReceptionReport",
		type: "post",
		data: { id: $("#id_productionLot").val() },
		async: true,
		cache: false,
		error: function(error) {
			console.log(error);
		},
		beforeSend: function() {
			showLoading();
		},
		success: function(result) {
			$("#maincontent").html(result);
		},
		complete: function() {
			hideLoading();
		}
	});
}



function PrintLiquidation(s, e) {

	var data = { id_pl: $("#id_productionLot").val(), codeReport: "LPPPL" };

	$.ajax({
		url: 'ProductionLotReception/PrintReportsForDocumentGeneral',
		data: data,
		async: true,
		cache: false,
		type: 'POST',
		beforeSend: function() {
			showLoading();
		},
		success: function(result) {
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
		complete: function() {
			hideLoading();
		}
	});
}
function PrintLiquidationDistribucion(s, e) {

	var data = { id_pl: $("#id_productionLot").val(), codeReport: "RPDIS" };

	$.ajax({
		url: 'ProductionLotReception/PrintReportsForDistribucion',
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
function PrintLiquidationDistribucionImprimirPE(s, e) {

	var data = { id_pl: $("#id_productionLot").val(), codeReport: "RICPE" };

	$.ajax({
		url: 'ProductionLotReception/PrintReportsForDistribucion',
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

function PrintLiquidationEnd(s, e) {
	var data = { id_pl: $("#id_productionLot").val(), codeReport: "RPCUP" };

	$.ajax({
		url: 'ProductionLotReception/PrintReportsForDocumentGeneral',
		data: data,
		async: true,
		cache: false,
		type: 'POST',
		beforeSend: function() {
			showLoading();
		},
		success: function(result) {
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
		complete: function() {
			hideLoading();
		}
	});


	//var id_lotTmp = $("#id_productionLot").val;

	//var data = { ReportName: "ProductionLotPaymentEndReport", ReportDescription: "Liquidación Proveedor Final", ListReportParameter: [], id_productionLot: id_lotTmp };
	//$.ajax({
	//    url: 'ProductionLotReception/ProductionLotReceptionReportFilter',
	//    data: data,
	//    async: true,
	//    cache: false,
	//    type: 'POST',
	//    beforeSend: function () {
	//        showLoading();
	//    },
	//    success: function (response) {


	//        try {
	//            if (response.isvalid) {
	//                var reportModel = response.reportModel;
	//                var url = 'Report/Index?reportModel=' + reportModel;
	//                newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
	//                newWindow.focus();
	//                hideLoading();
	//            }
	//        }
	//        catch (err) {
	//            hideLoading();
	//        }

	//    },
	//    complete: function () {
	//        hideLoading();
	//    }
	//});
}

function GenerateAccounting(s, e) {

	$.ajax({
		url: "ProductionLotReception/ProductionLotReceptionGenerateAccounting",
		type: "post",
		data: { id: $("#id_productionLot").val() },
		async: true,
		cache: false,
		error: function(error) {
			console.log(error);
		},
		beforeSend: function() {
			showLoading();
		},
		success: function(result) {
			//$("#maincontent").html(result);
		},
		complete: function() {
			hideLoading();
		}
	});
}

function GenerateElectronicInvoice(s, e) {

	$.ajax({
		url: "ProductionLotReception/ProductionLotReceptionGenerateElectronicInvoice",
		type: "post",
		data: { id: $("#id_productionLot").val() },
		async: true,
		cache: false,
		error: function(error) {
			console.log(error);
		},
		beforeSend: function() {
			showLoading();
		},
		success: function(result) {
			//$("#maincontent").html(result);
		},
		complete: function() {
			hideLoading();
		}
	});
}

// TABS CONTROL ACTIONS

var IdLote = "";
function ComboBoxCertificaciones_Change() {
    var id_certificationAux = id_certification.GetValue();
    if (id_certificationAux !== null) {
        $.ajax({
            url: 'ProductionLotReception/GetIdLoteCertificacion',
            type: 'post',
            data: { id_certification: id_certificationAux },
            async: true,
            cache: false,
            success: function (result) {
                IdLote = result.IdLote;
                OnInternalNumberTextChanged();
            },
            error: function (result) {
                hideLoading();
            }
        });
    } else {
        IdLote = "";
        OnInternalNumberTextChanged();
    }


}

function ComboBoxCertificaciones_Init() {
    var id_certificationAux = id_certification.GetValue();
    if (id_certificationAux !== null) {
        $.ajax({
            url: 'ProductionLotReception/GetIdLoteCertificacion',
            type: 'post',
            data: { id_certification: id_certificationAux },
            async: true,
            cache: false,
            success: function (result) {
                IdLote = result.IdLote;
            },
            error: function (result) {
                hideLoading();
            }
        });
    } else {
        IdLote = "";
    }


}

var gv = null;

//Set Active Tab
function SetActiveTab(s, e) {
	var _tabSelected = $("#tabSelected").val();
	if (_tabSelected !== "8") {
		tabControl.SetActiveTab(tabControl.GetTab(1));
	} else {
		tabControl.SetActiveTab(tabControl.GetTab(8));
	}
	
}

// COMMON DETAILS ACTIONS BUTTONS

function AddNew(s, e) {
	if (gv !== null && gv !== undefined) {
		gv.AddNewRow();
	}
}

function Remove(s, e) {
}

function Refresh(s, e) {
	if (gv !== null && gv !== undefined) {
		gv.UnselectRows();
		gv.PerformCallback();
	}
}

// DETAILS ACTIONS

function AddNewDetail(s, e) {
	gvProductionLotReceptionEditFormItemsDetail.AddNewRow();
	//AddNew(s, e);
}

function RemoveDetail(s, e) {
	Remove(s, e);
}

function RefreshDetail(s, e) {
	//Refresh(s, e);
	gvProductionLotReceptionEditFormItemsDetail.UnselectRows();
	gvProductionLotReceptionEditFormItemsDetail.PerformCallback();
}

// DISPATCH MATERIALS ACTIONS

function AddNewDispatchMaterials(s, e) {
	gvProductionLotReceptionEditFormMaterialsDetail.AddNewRow();
	//AddNew(s, e);
}

function RemoveDispatchMaterials(s, e) {
	Remove(s, e);
}

function RefreshDispatchMaterials(s, e) {
	//Refresh(s, e);
	gvProductionLotReceptionEditFormMaterialsDetail.UnselectRows();
	gvProductionLotReceptionEditFormMaterialsDetail.PerformCallback();
}

// LIQUIDATONS ACTIONS

function AddNewLiquidation(s, e) {
	gvProductionLotEditFormProductionLotLiquidationsDetail.AddNewRow();
	//AddNew(s, e);
}

function RemoveLiquidation(s, e) {
	Remove(s, e);
}

function RefreshLiquidation(s, e) {
	//Refresh(s, e);
	gvProductionLotEditFormProductionLotLiquidationsDetail.UnselectRows();
	gvProductionLotEditFormProductionLotLiquidationsDetail.PerformCallback();
}

// PACKING MATERIAL ACTIONS

function AddNewPackingMaterial(s, e) {
	gvProductionLotEditFormProductionLotPackingMaterialsDetail.AddNewRow();
	//AddNew(s, e);
}

function RemovePackingMaterial(s, e) {
	Remove(s, e);
}

function RefreshPackingMaterial(s, e) {
	//Refresh(s, e);
	gvProductionLotEditFormProductionLotPackingMaterialsDetail.UnselectRows();
	gvProductionLotEditFormProductionLotPackingMaterialsDetail.PerformCallback();
}


// TRASH ACTIONS

function AddNewTrash(s, e) {
	gvProductionLotEditFormProductionLotTrashsDetail.AddNewRow();
	//AddNew(s, e);
}

function RemoveTrash(s, e) {
	Remove(s, e);
}

function RefreshTrash(s, e) {
	gvProductionLotEditFormProductionLotTrashsDetail.UnselectRows();
	gvProductionLotEditFormProductionLotTrashsDetail.PerformCallback();
}

// QualityAnalysis ACTIONS

function AddNewQualityAnalysis(s, e) {
	gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.AddNewRow();
	//AddNew(s, e);
}

function RemoveQualityAnalysis(s, e) {
	Remove(s, e);
}

function RefreshQualityAnalysis(s, e) {
	gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.UnselectRows();
	gvProductionLotReceptionEditFormProductionLotQualityAnalysissDetail.PerformCallback();
}

// DETAILS AND DISPATCH MATERIALS SELECTION

// DETAILS VIEW CALLBACKS

function ProductionLotReceptionDetail_OnBeginCallback(s, e) {
	e.customArgs["id_productionLot"] = $("#id_productionLot").val();
	s.cpIdProductionLot = $("#id_productionLot").val();
}

function ProductionLotReceptionDetail_Init(s, e) {
	s.PerformCallback();
}

function SetElementVisibility(id, visible) {
	var $element = $("#" + id);
	visible ? $element.show() : $element.hide();
}

function gvEditSelectAllRows() {
	gv.SelectRows();
}

// UPDATE PRODUCTION LOT TOTALS
function UpdateProductionLotTotals() {
	$.ajax({
		url: "ProductionLotReception/ProductionLotTotals",
		type: "post",
		data: null,
		async: true,
		cache: false,
		error: function(error) {
			console.log(error);
		},
		beforeSend: function() {
			//showLoading();
		},
		success: function(result) {
			if (result !== null) {
				totalQuantityOrdered.SetValue(result.totalQuantityOrdered);
				totalQuantityRemitted.SetValue(result.totalQuantityRemitted);
				totalQuantityRecived.SetValue(result.totalQuantityRecived);
				totalQuantityLiquidation.SetValue(result.totalQuantityLiquidation);
				totalQuantityTrash.SetValue(result.totalQuantityTrash);
				var codeState = $("#codeState").val();
				if (codeState != "01") {
					wholeSubtotal.SetValue(result.wholeSubtotal);
					subtotalTail.SetValue(result.subtotalTail);
					totalQuantityLiquidationSubtotalLiq.SetValue(result.totalQuantityLiquidation);
				}
			}
		},
		complete: function() {
			//hideLoading();
		}
	});
}

//DETALLE DE SOLICITUD COMBOS

function itemCombo_OnInit(s, e) {
	//store actual filtering method and override
	var actualFilteringOnClient = s.filterStrategy.FilteringOnClient;
	s.filterStrategy.FilteringOnClient = function() {
		//create a new format string for all list box columns. you could skip this bit and just set
		//filterTextFormatString to whatever you wanted for instance "{0} {2}" would only filter on
		//columns 1 and 3
		var lb = this.GetListBoxControl();
		var filterTextFormatStringItems = [];
		for (var i = 0; i < lb.columnFieldNames.length; i++) {
			filterTextFormatStringItems.push('{' + i + '}');
		}
		var filterTextFormatString = filterTextFormatStringItems.join(' ');

		//store actual format string and override with one for all columns
		var actualTextFormatString = lb.textFormatString;
		lb.textFormatString = filterTextFormatString;

		//call actual filtering method which will now work on our temporary format string
		actualFilteringOnClient.apply(this);

		//restore original format string
		lb.textFormatString = actualTextFormatString;
	};
}

// CLOSE 
function PrintLiquidationClose(s, e) {
	var data = { id_pl: $("#id_productionLot").val(), codeReport: "LPPPL" };

	$.ajax({
		url: 'ProductionLotReception/PrintReportsForDocumentGeneral',
		data: data,
		async: true,
		cache: false,
		type: 'POST',
		beforeSend: function() {
			showLoading();
		},
		success: function(result) {
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
		complete: function() {
			hideLoading();
		}
	});
}
function PrintLiquidationReport(s, e) {
	var data = { id_pl: $("#id_productionLot").val(), codeReport: "LPPLL" };

	$.ajax({
		url: 'ProductionLotReception/PrintReportsForDocumentGeneralLiquidation',
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

// CLOSE 
function PrintCloseLiq(s, e) {
	// 
	var _codeEnteroRep = $("#codeEnteroReport").val();
	var _codeColaRep = $("#codeColaReport").val();

    var data = { id_productionLot: $("#id_productionLot").val(), codeProcessType: "ENT", codeReport: "LCIERR" };


	if (_codeEnteroRep == "E") {
		$.ajax({
			url: 'ProductionLotReception/PrintReportClose',
			data: data,
			async: true,
			cache: false,
			type: 'POST',
			beforeSend: function() {
				showLoading();
			},
			success: function(result) {
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
			complete: function() {
				hideLoading();
			}
		});
	}

	data = { id_productionLot: $("#id_productionLot").val(), codeProcessType: "COL", codeReport: "LCIERR" };

	if (_codeColaRep == "C") {
		$.ajax({
			url: 'ProductionLotReception/PrintReportClose',
			data: data,
			async: true,
			cache: false,
			type: 'POST',
			beforeSend: function() {
				showLoading();
			},
			success: function(result) {
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
			complete: function() {
				hideLoading();
			}
		});
	}
}

function PrintCloseLiqRend(s, e) {
	// 
	var _codeEnteroRep = $("#codeEnteroReport").val();
	var _codeColaRep = $("#codeColaReport").val();

	var data = { id_productionLot: $("#id_productionLot").val(), codeProcessType: "ENT", codeReport: "LCIERE" };


	if (_codeEnteroRep == "E") {
		$.ajax({
			url: 'ProductionLotReception/PrintReportCloseRend',
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

	data = { id_productionLot: $("#id_productionLot").val(), codeProcessType: "COL", codeReport: "LCIERE" };

	if (_codeColaRep == "C") {
		$.ajax({
			url: 'ProductionLotReception/PrintReportCloseRend',
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
}

// UPDATE VIEW
function AutoCloseAlert() {
	if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
		$(".alert-success").fadeTo(3000, 0.45, function() {
			$(".alert-success").alert('close');
		});
	}
}

function UpdateView() {
	var id = parseInt($("#id_productionLot").val());

	// EDITING BUTTONS
	btnNew.SetEnabled(true);
	btnSave.SetEnabled(false);
	btnCopy.SetEnabled(false);

	btnAutorize.SetVisible(false);
	btnProtect.SetVisible(false);

	// STATES BUTTONS
	$.ajax({
		url: "ProductionLotReception/Actions",
		type: "post",
		data: { id: id },
		async: true,
		cache: false,
		error: function(error) {
			console.log(error);
		},
		beforeSend: function() {
			//showLoading();
		},
		success: function(result) {
			btnApprove.SetEnabled(result.btnApprove);
			btnCancel.SetEnabled(result.btnCancel);
			btnRevert.SetEnabled(result.btnRevert);
			btnConciliation.SetEnabled(result.btnConciliation);
		},
		complete: function(result) {
			//hideLoading();
		}
	});

	// HISTORY BUTTON
	btnHistory.SetEnabled(id !== 0);

	// PRINT BUTTON
	btnPrint.SetEnabled(id !== 0);

	var isBtnToReturn = $("#isBtnToReturn").val();
	if (isBtnToReturn == "True" || isBtnToReturn == "true" || isBtnToReturn == true) {
		//$("#pagination").show();
		$('.pagination').hide();
		btnNew.SetEnabled(false);
	} else {

		$('.pagination').show();
		btnNew.SetEnabled(true);
	}
}

function UpdatePagination() {
	var current_page = 1;
	$.ajax({
		url: "ProductionLotReception/InitializePagination",
		type: "post",
		data: { id_productionLot: $("#id_productionLot").val() },
		async: false,
		cache: false,
		error: function(error) {
			console.log(error);
		},
		beforeSend: function() {
		},
		success: function(result) {
			$("#pagination").attr("data-max-page", result.maximunPages);
			current_page = result.currentPage;
		},
		complete: function() {
		}
	});
	$('.pagination').current_page = current_page;
}

function init() {
	UpdatePagination();

	AutoCloseAlert();
}

function GridViewItemsCustomNewCommandButton_Click(s, e) {
	$("#GridMessageErrorsDetail").hide();
	if (e.buttonID === "btnQualityControlRow") {
		var data = "idPlQc=" + gvProductionLotReceptionDetailItemsNew.GetRowKey(e.visibleIndex) + "&" + $("#formEditProductionLotReception").serialize();
		$.ajax({
			url: "ProductionLot/UpdateProductionLot",
			type: "post",
			data: data,
			async: true,
			cache: false,
			error: function(error) {
				console.log(error);
			},
			beforeSend: function() {
				//showLoading();
			},
			success: function(result) {
				if (result !== null) {
					if (result.hasProdQuality == "YES") {
						var data2 = {
							id: 0,
							loteManual: false,
							id_productionLotDetail: gvProductionLotReceptionDetailItemsNew.GetRowKey(e.visibleIndex),
							urlToReturn: "ProductionLotReception/ProductionLotReceptionFormEditPartial",
							tabSelected: 2,//Seleccionado el tab 3 de Matreria Prima
							arrayTempDataKeep: ["productionLot"],
							hasUpdate: false
						};
						showPagefromLink("QualityControl/FormEditQualityControl", data2);
						//Mostrar Vista de Gestion de Calidad pasandole el id_QualityControl que estaria en una propiedad del grid
					} else {
						gridMessageErrorsDetail.SetText(ErrorMessage("Este detalle no tiene calidad."));
						$("#GridMessageErrorsDetail").show();
					}


				}
			},
			complete: function() {
			}
		});

	}
}

$(function() {

	var chkReadyState = setInterval(function() {
		if (document.readyState === "complete") {
			clearInterval(chkReadyState);
			UpdateView();
		}
	}, 100);

	init();
});