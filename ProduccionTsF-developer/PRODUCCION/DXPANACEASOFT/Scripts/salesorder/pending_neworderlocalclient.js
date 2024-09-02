
var selectedPendingNewRows = [];

function GetSelectedFieldDetailValuesCallback(values) {
   
    selectedPendingNewRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedPendingNewRows.push(values[i]);
    }
    var data = {
        id: 0,
        ids: [],
        id_proforma: null,
        enabled: true
    };
    if (selectedPendingNewRows.length < 1) {
        NotifyError("Debe seleccionar al menos un detalle de Requerimiento. ");
        hideLoading();
        btnGenerateOrderLocalClient.SetEnabled(true);
        return;
    }
    var numberRequestProformaAux = null;
    for (i = 0; i < selectedPendingNewRows.length; i++) {
        if (numberRequestProformaAux === null) {
            numberRequestProformaAux = selectedPendingNewRows[i][1];
        } else {
            if (numberRequestProformaAux !== selectedPendingNewRows[i][1]) {
                NotifyError("Debe seleccionar detalle de Requerimiento, del mismo Requerimiento de Pedido");
                hideLoading();
                btnGenerateOrderLocalClient.SetEnabled(true);
                return;
            }
        }
        data.ids.push(selectedPendingNewRows[i][0]);
    }

    hideLoading();
    showPage("SalesOrder/Edit", data);
}

function GenerateOrderLocalClient(s, e) {
    showLoading();
    btnGenerateOrderLocalClient.SetEnabled(false);
    GridViewPendingNewOrderLocalClient.GetSelectedFieldValues("id_salesRequestDetail;numberRequestProforma", GetSelectedFieldDetailValuesCallback);
}

function Init() {
    $("#btnCollapsePendiente").click(function (event) {
        showPage("SalesOrder/Index");
    });
}

$(function () {
    Init();
});