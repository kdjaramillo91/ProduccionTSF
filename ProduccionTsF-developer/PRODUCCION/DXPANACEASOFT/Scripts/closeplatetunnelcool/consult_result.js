
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
        id_MachineProdOpeningDetail: 0,
        enabled: false
    };
    showPage("ClosePlateTunnelCool/Edit", data);
}

function AddNewItem() {
    $.ajax({
        url: "ClosePlateTunnelCool/PendingNew",
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
    if (rowKeySelected == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var data = {
        id: rowKeySelected,
        id_MachineProdOpeningDetail: 0,
        enabled: true
    };
    showPage("ClosePlateTunnelCool/Edit", data);
}

function AprovedItem() {
    if (rowKeySelected == undefined) {
        NotifyWarning("Seleccione un elemento");
        return;
    }

    var result = DevExpress.ui.dialog.confirm("Desea Aprobar el Cierre de Placas, Túneles, Fresco Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'ClosePlateTunnelCool/Approve',
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

    var result = DevExpress.ui.dialog.confirm("Desea Reversar el Cierre de Placas, Túneles, Fresco Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'ClosePlateTunnelCool/Reverse',
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

    var result = DevExpress.ui.dialog.confirm("Desea Anular el Cierre de Placas, Túneles, Fresco Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            $.ajax({
                url: 'ClosePlateTunnelCool/Annul',
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
        id_turn: ComboBoxTurnIndex.GetValue(),
        id_machineForProd: ComboBoxMachineForProdIndex.GetValue(),
        id_person: ComboBoxPersonIndex.GetValue(),
        numberLot: TextBoxNumberLot.GetText(),
        id_provider: ComboBoxProviderIndex.GetValue(),
        id_productionUnitProvider: ComboBoxProductionUnitProviderIndex.GetValue(),
        items: TokenBoxItemsIndex.GetTokenValuesCollection(),
        customers: TokenBoxCustomersIndex.GetTokenValuesCollection()
    };

    showPartialPage($("#grid"), 'ClosePlateTunnelCool/SearchResult', data);
}

function PrintItem() {

}

$(function () {
    rowKeySelected = undefined;
});