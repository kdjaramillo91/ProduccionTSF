
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
        idProductionLotReceptionDetail: 0,
        enabled: false,
    }
    showPage("DrainingTest/Edit", data);
}

function AddNewItem() {
    $.ajax({
        url: "DrainingTest/PendingNew",
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
        idProductionLotReceptionDetail: 0,
        enabled: true,
    }
    showPage("DrainingTest/Edit", data);
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
                url: 'DrainingTest/Approve',
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
                url: 'DrainingTest/Reverse',
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
                url: 'DrainingTest/Annul',
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
        numberLote: TextBoxLote.GetText(),
        reference: TextBoxReference.GetText(),
    }

    showPartialPage($("#grid"), 'DrainingTest/SearchResult', data);
}

function PrintItem() {

}

$(function () {
    rowKeySelected = undefined;
});