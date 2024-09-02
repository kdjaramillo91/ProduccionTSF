function AsignarLiquidacion(s, e) {

    GridViewReceptionMaterialApproved.GetSelectedFieldValues("idReceptionDispatchMaterial;idProvider", function (values) {
        if (values.length <= 0) {
            NotifyWarning("Seleccione una Recepción de Materiales");
            return;
        }
        var selectedRows = [];
        var id_proveedorAux = null;

        for (var k = 0; k < values.length; k++) {
            if (id_proveedorAux == null || id_proveedorAux == values[k][1]) {
                selectedRows.push(values[k][0]);
                id_proveedorAux = values[k][1];
            } else {
                NotifyWarning("Debe seleccionar una Recepciones de Materiales para el mismo proveedor");
                return;
            }
        }
        var data = {
            ids: selectedRows,
            id: 0
        };

        showPage("LiquidationMaterial/Edit", data);


    });
}

function Init() {
    $("#btnCollapsePendiente").click(function (event) {
        showPage("LiquidationMaterial/Index");
    });
}

$(function () {
    Init();
});