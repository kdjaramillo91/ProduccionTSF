
var rowKeySelected;

function OnGridFocusedRowChanged(s, e) {
    s.GetRowValues(e.visibleIndex, 'id;canEdit;canClosed;canAproved;canReverse;canAnnul', OnGetRowValues);
}

function OnGetRowValues(values) {

	rowKeySelected = values[0];
	var canEdit = values[1];
    var canClosed = values[2];
    var canAproved = values[3];
	var canReverse = values[4];
	var canAnnul = values[5];

	if (canEdit) {
		btnEdit.SetEnabled(true);
	}
	else {
		btnEdit.SetEnabled(false);
    }

    if (canClosed) {
        btnClosed.SetEnabled(true);
    }
    else {
        btnClosed.SetEnabled(false);
    }

	if (canAproved) {
		btnAproved.SetEnabled(true);
	}
	else {
		btnAproved.SetEnabled(false);
	}

	if (canReverse) {
		btnReverse.SetEnabled(true);
	}
	else {
		btnReverse.SetEnabled(false);
	}

	if (canAnnul) {
		btnAnnul.SetEnabled(true);
	}
	else {
		btnAnnul.SetEnabled(false);
	}
}

function GridViewBtnShow_Click(s, e) {
	s.GetRowValues(e.visibleIndex, 'id', EditItem);
}

function EditItem(values) {
	var data = {
		id: values,
		ids: [],
		id_proforma: null,
		enabled: false
	};
	showPage("SalesOrder/Edit", data);
}

//function AddNewItem() {
//    $.ajax({
//        url: "SalesOrder/PendingNew",
//        type: "post",
//        data: data,
//        async: true,
//        cache: false,
//        error: function (error) {
//            //// 
//            NotifyError("Error. " + error);
//        },
//        beforeSend: function () {
//            showLoading();
//        },
//        success: function (result) {
//            $("#maincontent").html(result);
//        },
//        complete: function () {
//            hideLoading();
//        }
//    });
//}

function EditCurrentItem() {
	if (rowKeySelected == undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var data = {
		id: rowKeySelected,
		ids: [],
		id_proforma: null,
		enabled: true
	};
	showPage("SalesOrder/Edit", data);
}

function ClosedItem() {
    if (rowKeySelected == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Cerrar la Orden de Producción Seleccionada?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'SalesOrder/Close',
                type: 'post',
                data: { id: rowKeySelected },
                async: true,
                cache: false,
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error.</br>" + result.Message);
                        return;
                    }

                    NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
                    RefreshGrid();
                },
                error: function (result) {
                    hideLoading();
                }
            });
        }
    });
}

function AprovedItem() {
	if (rowKeySelected == undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var result = DevExpress.ui.dialog.confirm("Desea Aprobar la Orden de Producción Seleccionada?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'SalesOrder/Approve',
				type: 'post',
				data: { id: rowKeySelected },
				async: true,
				cache: false,
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error.</br>" + result.Message);
						return;
					}

					NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
					RefreshGrid();
				},
				error: function (result) {
					hideLoading();
				}
			});
		}
	});
}

function ReverseItem() {
	if (rowKeySelected == undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var result = DevExpress.ui.dialog.confirm("Desea Reversar la Orden de Producción Seleccionada?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'SalesOrder/Reverse',
				type: 'post',
				data: { id: rowKeySelected },
				async: true,
				cache: false,
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error.</br>" + result.Message);
						return;
					}

					NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
					RefreshGrid();
				},
				error: function (result) {
					hideLoading();
				}
			});
		}
	});
}

function AnnulItem() {
	if (rowKeySelected == undefined) {
		NotifyWarning("Seleccione un elemento");
		return;
	}

	var result = DevExpress.ui.dialog.confirm("Desea Anular la Orden de Producción Seleccionada?", "Confirmar");
	result.done(function (dialogResult) {
		if (dialogResult) {
			$.ajax({
				url: 'SalesOrder/Annul',
				type: 'post',
				data: { id: rowKeySelected },
				async: true,
				cache: false,
				success: function (result) {
					if (result.Code !== 0) {
						hideLoading();
						NotifyError("Error.</br>" + result.Message);
						return;
					}

					NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
					RefreshGrid();
				},
				error: function (result) {
					hideLoading();
				}
			});
		}
	});
}

function RefreshGrid() {
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
		reference: TextBoxReference.GetText(),

		id_documentType: ComboBoxDocumentTypeIndex.GetValue(),
		id_customer: ComboBoxCustomerIndex.GetValue(),
		id_seller: ComboBoxSellerIndex.GetValue(),
		items: TokenBoxItemsIndex.GetTokenValuesCollection(),
		id_Logistics: ComboBoxLogisticsIndex.GetValue()
	};

	showPartialPage($("#grid"), 'SalesOrder/SearchResult', data);
}

function PrintItem() {
	var id = $("#id_salesOrder").val();
	if (!(id === 0) && !(id === null)) {
		$.ajax({
			url: "SalesOrder/PrintReport",
			type: "post",
			data: {
				id: id,
				codeReport: "RPTOP"
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
}

$(function () {
	rowKeySelected = undefined;
});