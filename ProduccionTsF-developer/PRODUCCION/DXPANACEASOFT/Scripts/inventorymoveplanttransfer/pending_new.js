
var selectedPendingNewRows = [];

function GetSelectedFieldDetailValuesToUpdateTotalBoxCallback(values) {

    //selectedPendingNewRows = [];
    var totalBox = 0.00; 
    for (var i = 0; i < values.length; i++) {
        totalBox += parseFloat(values[i]);
        //selectedPendingNewRows.push(values[i]);
    }
    totalSelectedBoxes.SetValue(totalBox);
    //var data = {
    //    id: 0,
    //    ids: [],
    //    enabled: true
    //};
    //if (selectedPendingNewRows.length < 1) {
    //    NotifyError("Debe seleccionar al menos un detalle de Carro. ");
    //    hideLoading();
    //    btnGenerateMoveTransfer.SetEnabled(true);
    //    return;
    //}
    //var numberLiquidationCartOnCartAux = null;
    ////var productionCartAux = null;
    //for (i = 0; i < selectedPendingNewRows.length; i++) {
    //    if (numberLiquidationCartOnCartAux === null) {
    //        numberLiquidationCartOnCartAux = selectedPendingNewRows[i][1];
    //        //productionCartAux = selectedPendingNewRows[i][2];
    //    } else {
    //        if (numberLiquidationCartOnCartAux !== selectedPendingNewRows[i][1]// ||
    //            /*productionCartAux !== selectedPendingNewRows[i][2]*/) {
    //            NotifyError("Debe seleccionar detalle de Carro, con igual No.Liq. y Carro");
    //            hideLoading();
    //            btnGenerateMoveTransfer.SetEnabled(true);
    //            return;
    //        }
    //    }
    //    data.ids.push(selectedPendingNewRows[i][0]);
    //}

    //hideLoading();
    //showPage("InventoryMovePlantTransfer/Edit", data);
}

function OnGridViewPendingNewSelectionChanged(s, e) {
    s.GetSelectedFieldValues("box", GetSelectedFieldDetailValuesToUpdateTotalBoxCallback);
}

function GetSelectedFieldDetailValuesCallback(values) {
   
    selectedPendingNewRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedPendingNewRows.push(values[i]);
    }
    var data = {
        id: 0,
        ids: [],
        enabled: true
    };
    if (selectedPendingNewRows.length < 1) {
        NotifyError("Debe seleccionar al menos un detalle de Carro. ");
        hideLoading();
        btnGenerateMoveTransfer.SetEnabled(true);
        return;
    }
    var numberLiquidationCartOnCartAux = null;
    
    for (i = 0; i < selectedPendingNewRows.length; i++) {
        if (numberLiquidationCartOnCartAux === null) {
            numberLiquidationCartOnCartAux = selectedPendingNewRows[i][1];
            //productionCartAux = selectedPendingNewRows[i][2];
        }
        //else {
        //    if (numberLiquidationCartOnCartAux !== selectedPendingNewRows[i][1]// ||
        //        /*productionCartAux !== selectedPendingNewRows[i][2]*/) {
        //        NotifyError("Debe seleccionar detalle, con igual No.Liq.");
        //        hideLoading();
        //        btnGenerateMoveTransfer.SetEnabled(true);
        //        return;
        //    }
        //}
        data.ids.push(selectedPendingNewRows[i][0]);
    }

    hideLoading();
    showPage("InventoryMovePlantTransfer/Edit", data);
}

function GenerateInventoryMoveTransfer(s, e) {
    showLoading();
    btnGenerateMoveTransfer.SetEnabled(false);
    GridViewPendingNew.GetSelectedFieldValues("id_liquidationCartOnCartDetail;numberLiquidationCartOnCart", GetSelectedFieldDetailValuesCallback);
}

function Init() {
    $("#btnCollapsePendiente").click(function (event) {
        showPage("InventoryMovePlantTransfer/Index");
    });
}

$(function () {
    Init();
});