var rowKeySelectedPendin;

function PrintItem() {

}

function AddNewItem() {

    var data = {
        id: rowKeySelectedPendin == undefined ? 0 : rowKeySelectedPendin,
        enabled: true,
        from: "pending"
    }
    showPage("PriceListResponsive/Edit", data);
}

function AprovedItem() {

    if (rowKeySelectedPendin == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Aprobar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'PriceListResponsive/Approve',
                type: 'post',
                data: { id: rowKeySelectedPendin },
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
                    rowKeySelectedPendin = undefined;
                },
                error: function (result) {
                    hideLoading();
                },
            });
        }
    });
}

function ReverseItem() {

    if (rowKeySelectedPendin == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Reversar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'PriceListResponsive/Reverse',
                type: 'post',
                data: { id: rowKeySelectedPendin },
                async: true,
                cache: false,
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error.</br>" + result.Message);
                        return;
                    }

                    NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
                    rowKeySelectedPendin = undefined;
                    RefreshGrid(); 
                },
                error: function (result) {
                    hideLoading();
                },
            });
        }
    });
}

function CloseItem() {
    if (rowKeySelectedPendin == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Cerrar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'PriceListResponsive/Close',
                type: 'post',
                data: { id: rowKeySelectedPendin },
                async: true,
                cache: false,
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error.</br>" + result.Message);
                        return;
                    }

                    NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
                    rowKeySelectedPendin = undefined;
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
    if (rowKeySelectedPendin == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Anular el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'PriceListResponsive/Annul',
                type: 'post',
                data: { id: rowKeySelectedPendin },
                async: true,
                cache: false,
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error.</br>" + result.Message);
                        return;
                    }

                    NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
                    rowKeySelectedPendin = undefined;
                    RefreshGrid();
                },
                error: function (result) {
                    hideLoading();
                },
            });
        }
    });
}

function ExitItem() {
    showPage("PriceListResponsive/Index");
}

function EditCurrentItem() {

    if (rowKeySelectedPendin == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var data = {
        id: rowKeySelectedPendin,
        enabled: true,
        from: "pending"
    }
    showPage("PriceListResponsive/Edit", data);
}

function OnGridFocusedRowChanged(s, e) {
    GridViewPriceListPendingAproval.GetRowValues(e.visibleIndex, 'id;id_state;isUsed;canClose;canAnnul', OnGetRowValues);
}

function OnGetRowValues(values) {

    rowKeySelectedPendin = values[0];
    var id_state = values[1];
    var isUsed = values[2];
    var canClose = values[3];
    var canAnnul = values[4];

    if (id_state == $("#id_crateState").val() || id_state == $("#id_reversedState").val()) {
        btnAproved.SetEnabled(true);
        btnEdit.SetEnabled(true);
    }
    else {
        btnAproved.SetEnabled(false);
        btnEdit.SetEnabled(false);
    }

    if (id_state == $("#id_aprovedState").val() && !isUsed) {
        btnReverse.SetEnabled(true);
    }
    else {
        btnReverse.SetEnabled(false);
    }

    if (canClose) {
        btnClose.SetEnabled(true);
    }
    else {
        btnClose.SetEnabled(false);
    }

    if (canAnnul) {
        btnAnnul.SetEnabled(true);
    }
    else {
        btnAnnul.SetEnabled(false);
    }
}

function RefreshGrid() {
    $.ajax({
        url: "PriceListResponsive/PendingApprove",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            debugger;
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

function GridViewPriceListShow_Click(sender, e) {
    sender.GetRowValues(e.visibleIndex, 'id', function (value) {
        var data = {
            id: value,
            enabled: false,
            from: "pending"
        }
        showPage("PriceListResponsive/Edit", data);
    });
}

$(function () {
});