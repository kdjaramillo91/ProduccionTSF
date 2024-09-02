
var rowKeySelected;

function OnGridFocusedRowChanged(s, e) {
    s.GetRowValues(e.visibleIndex, 'id;canEdit;canAproved;canReverse;canAnnul', OnGetRowValues);
}

function OnGetRowValues(values) {

    rowKeySelected = values[0];
    var canEdit = values[1];
    var canAproved = values[2];
    var canReverse = values[3];
    var canAnnul = values[4];

    if (canEdit) {
        btnEdit.SetEnabled(true);
    }
    else {
        btnEdit.SetEnabled(false);
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
        id_turn: 0,
		emissionDate: null,
		id_personProcessPlant: 0,
        enabled: false
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
    if (rowKeySelected == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var data = {
        id: rowKeySelected,
        id_turn: 0,
		emissionDate: null,
		id_personProcessPlant: 0,
        enabled: true
    };
    showPage("LiquidationTurn/Edit", data);
}

function AprovedItem() {
    if (rowKeySelected == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Aprobar la Liquidación de Turno Seleccionada?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'LiquidationTurn/Approve',
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

    var result = DevExpress.ui.dialog.confirm("Desea Reversar la Liquidación de Turno Seleccionada?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'LiquidationTurn/Reverse',
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
                },
            });
        }
    });
}

function AnnulItem() {
    if (rowKeySelected == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Anular la Liquidación de Turno Seleccionada?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'LiquidationTurn/Annul',
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
        id_turn: ComboBoxTurnIndex.GetValue()
    };

    showPartialPage($("#grid"), 'LiquidationTurn/SearchResult', data);
}

function PrintItem(data, url) {
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

function printReportLiquidacionTurno(){
	var data = {
		id: $("#id_liquidationTurn").val(),
		codeReport: "RPLT",
	};

	var url = "LiquidationTurn/ProductionLotDailyCloseReport";

	PrintItem(data, url);
}


function printReportLiquidacionTurnoTemporal() {

	var FechaEmission = DateTimeEmision.GetValue();
	var emisionDateYear = FechaEmission.getFullYear();
	var emisionDateMonth = FechaEmission.getMonth() + 1;
	var emisionDateDay = FechaEmission.getDate();
	var emisionDate = emisionDateYear + "/" + emisionDateMonth + "/" + emisionDateDay;

	var data = {
		id: $("#id_liquidationTurn").val(),
		FechaLiquidacion: emisionDate,
		codeReport: "RPLTT",
	};

	var url = "LiquidationTurn/ProductionLotDailyCloseTemporalReport";

	PrintItem(data, url);
}


function printReportLiquidacionCaja() {
    // 
    var FechaEmission = DateTimeEmision.GetValue();
    var emisionDateYear = FechaEmission.getFullYear();
    var emisionDateMonth = FechaEmission.getMonth() + 1;
    var emisionDateDay = FechaEmission.getDate();
    var emisionDate = emisionDateYear + "/" + emisionDateMonth + "/" + emisionDateDay;

    var data = {
        id: $("#id_liquidationTurn").val(),
        FechaLiquidacion: emisionDate,
        idTurno: 2,
         
        codeReport: "CTRC",
    };

    var url = "LiquidationTurn/printReportLiquidacionCaja";

    PrintItem(data, url);
}



$(function () {
    rowKeySelected = undefined;
});