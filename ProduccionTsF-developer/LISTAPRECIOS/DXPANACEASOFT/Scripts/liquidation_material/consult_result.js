var rowKeySelected;

function OnGridFocusedRowChanged(s, e) {
    s.GetRowValues(e.visibleIndex, 'id;canEdit;canAproved;canAuthorize;canAnnul;canReverse', OnGetRowValues);
}

function OnGetRowValues(values) {

    rowKeySelected = values[0];
    var canEdit = values[1];
    var canAproved = values[2];
    var canAuthorize = values[3];
    var canAnnul = values[4];
    var canReverse = values[5];

    btnEdit.SetEnabled(canEdit);
    btnAproved.SetEnabled(canAproved);
    btnAuthorize.SetEnabled(canAuthorize);
    btnAnnul.SetEnabled(canAnnul);
    btnReverse.SetEnabled(canReverse);

    //if (canAproved) {
    //    btnAproved.SetEnabled(true);
    //}
    //else {
    //    btnAproved.SetEnabled(false);
    //}

    //if (canAuthorize) {
    //    btnAuthorize.SetEnabled(true);
    //}
    //else {
    //    btnAuthorize.SetEnabled(false);
    //}

    //if (canAnnul) {
    //    btnAnnul.SetEnabled(true);
    //}
    //else {
    //    btnAnnul.SetEnabled(false);
    //}

    //if (canReverse) {
    //    btnReverse.SetEnabled(true);
    //}
    //else {
    //    btnReverse.SetEnabled(false);
    //}

}

function AddNewItem() {
    $.ajax({
        url: "LiquidationMaterial/ReceptionMaterialApproved",
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

function EditCurrentItem() {
    if (rowKeySelected == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var data = {
        id: rowKeySelected,
        ids: null,
        enabled: true,
    }
    showPage("LiquidationMaterial/Edit", data);
}

function AprovedItem() {
    if (rowKeySelected == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Aprobar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'LiquidationMaterial/Approve',
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

function AuthorizeItem() {
    if (rowKeySelected == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Autorizar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'LiquidationMaterial/Authorize',
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

    var result = DevExpress.ui.dialog.confirm("Desea Anular el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'LiquidationMaterial/Annul',
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

function ReverseItem() {
    if (rowKeySelected == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Reversar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'LiquidationMaterial/Reverse',
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

function RefreshGrid() {
    var dateInicioEmision = DateEditFechaInicioEmision.GetDate();
    var yearInicioEmision = dateInicioEmision.getFullYear();
    var monthInicioEmision = dateInicioEmision.getMonth() + 1;
    var dayInicioEmision = dateInicioEmision.getDate();

    var dateFinEmision = DateEditFechaFinEmision.GetDate();
    var yearFinEmision = dateFinEmision.getFullYear();
    var monthFinEmision = dateFinEmision.getMonth() + 1;
    var dayFinEmision = dateFinEmision.getDate();

    var dateInicioGuia = DateEditFechaInicioGuia.GetDate();
    var yearInicioGuia = dateInicioGuia.getFullYear();
    var monthInicioGuia = dateInicioGuia.getMonth() + 1;
    var dayInicioGuia = dateInicioGuia.getDate();

    var dateFinGuia = DateEditFechaFinGuia.GetDate();
    var yearFinGuia = dateFinGuia.getFullYear();
    var monthFinGuia = dateFinGuia.getMonth() + 1;
    var dayFinGuia = dateFinGuia.getDate();

    var data = {
        id_estado: ComboBoxEstados.GetValue(),
        numeroLiquidacion: NumeroLiquidacion.GetText(),
        fechaInicioEmision: dayInicioEmision + "/" + monthInicioEmision + "/" + yearInicioEmision,
        fechaFinEmision: dayFinEmision + "/" + monthFinEmision + "/" + yearFinEmision,
        id_proveedor: ComboBoxProveedores.GetValue(),
        id_producto: ComboBoxProductos.GetValue(),
        fechaInicioGuia: dayInicioGuia + "/" + monthInicioGuia + "/" + yearInicioGuia,
        fechaFinGuia: dayFinGuia + "/" + monthFinGuia + "/" + yearFinGuia,
        numeroGuia: NumeroGuia.GetText()
    }

    showPartialPage($("#grid"), 'LiquidationMaterial/SearchResult', data);
}

function GridViewLiquidationMaterialShow_Click(sender, e) {
    sender.GetRowValues(e.visibleIndex, 'id', function (value) {
        var data = {
            id: value,
            ids: null,
            enabled: false
        }
        showPage("LiquidationMaterial/Edit", data);
    });
}

function PrintItem() {

}

$(function () {
    rowKeySelected = undefined;
});