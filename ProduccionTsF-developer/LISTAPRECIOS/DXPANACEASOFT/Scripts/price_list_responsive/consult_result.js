
var rowKeySelected;

function PrintItem() {
    if (rowKeySelected === undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    } else {
        var data = { id_priceList: rowKeySelected };

        $.ajax({
            url: 'PriceListResponsive/PrintPriceListReport',
            type: 'post',
            data: data,
            async: true,
            cache: false,
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

function AddNewItem() {

    var data = {
        id: rowKeySelected === undefined ? 0 : rowKeySelected,
        enabled: true
    };
    showPage("PriceListResponsive/AddWhithBase", data);
}

function AprovedItem() {

    if (rowKeySelected === undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Aprobar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'PriceListResponsive/Approve',
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
                    BuscarClick();
                },
                error: function (result) {
                    hideLoading();
                },
            });
        }
    });
}

function ReverseItem() {

    if (rowKeySelected === undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Reversar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'PriceListResponsive/Reverse',
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
                    BuscarClick();
                },
                error: function (result) {
                    hideLoading();
                }
            });
        }
    });
}

function CloseItem() {
    if (rowKeySelected === undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Cerrar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'PriceListResponsive/Close',
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
                    BuscarClick();
                },
                error: function (result) {
                    hideLoading();
                }
            });
        }
    });
}

function AnnulItem() {
    if (rowKeySelected === undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Anular el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'PriceListResponsive/Annul',
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
                    BuscarClick();
                },
                error: function (result) {
                    hideLoading();
                }
            });
        }
    });
}

function EditCurrentItem() {

    if (rowKeySelected === undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var data = {
        id: rowKeySelected,
        enabled: true
    };
    showPage("PriceListResponsive/Edit", data);
}

function DuplicateCurrentItem() {

    if (rowKeySelected === undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var data = {
        id: rowKeySelected
    };
    showPage("PriceListResponsive/Duplicate", data);
}

function RefreshReplicateIndex() {
    showLoading();

    if (rowKeySelected === undefined) {
        NotifyWarning("Seleccione un elemento");
        hideLoading();
        return;
    }
    $.ajax({
        url: 'PriceListResponsive/RefreshReplicate',
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
            hideLoading();
            NotifySuccess("Se refresco la replicación de la lista de manera Satisfactoria. ");
        },
        error: function (result) {
            hideLoading();
        }
    });
}

function OnGridFocusedRowChanged(s, e) {
    GridViewPriceList.GetRowValues(e.visibleIndex, 'id;id_state;isUsed;canClose;canAnnul;canRefreshReplicate', OnGetRowValues);
}

function OnGetRowValues(values) {

    rowKeySelected = values[0];
    var id_state = values[1];
    var isUsed = values[2];
    var canClose = values[3];
    var canAnnul = values[4];
    var canRefreshReplicate = values[5];

    if (id_state === $("#id_crateState").val() || id_state === $("#id_reversedState").val()) {
        btnAproved.SetEnabled(true);
        btnEdit.SetEnabled(true);
    }
    else {
        btnAproved.SetEnabled(false);
        btnEdit.SetEnabled(false);
    }


    if (id_state === $("#id_aprovedState").val() && !isUsed) {
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

    if (canRefreshReplicate) {
        btnRefreshReplicateIndex.SetEnabled(true);
    }
    else {
        btnRefreshReplicateIndex.SetEnabled(false);
    }
}

function RefreshGrid() {
    BuscarClick();
    rowKeySelected = undefined;
}

function GridViewPriceListShow_Click(sender, e) {
    sender.GetRowValues(e.visibleIndex, 'id', function (values) {
        var data = {
            id: values,
            enabled: false
        };
        showPage("PriceListResponsive/Edit", data);
    });
}

$(function () {
    rowKeySelected = undefined;
});