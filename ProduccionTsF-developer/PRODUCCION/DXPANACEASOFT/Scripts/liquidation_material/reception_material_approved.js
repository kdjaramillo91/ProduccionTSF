function AsignarLiquidacion(s, e) {
    showLoading();

    GridViewReceptionMaterialApproved.GetSelectedFieldValues("idReceptionDispatchMaterial;idProvider;idPersonProcessPlant", function (values) {
        if (values.length <= 0) {
            hideLoading();
            NotifyWarning("Seleccione una Recepción de Materiales");
            return;
        }
        var selectedRows = [];
        var id_proveedorAux = null;

        for (var k = 0; k < values.length; k++) {
            if (id_proveedorAux === null || id_proveedorAux === values[k][1]) {
                selectedRows.push(values[k][0]);
                id_proveedorAux = values[k][1];
            } else {
                hideLoading();
                NotifyWarning("Debe seleccionar una Recepciones de Materiales para el mismo proveedor");
                return;
            }
        }
        $.ajax({
            url: "LiquidationMaterial/ExistData",
            type: "post",
            data: null,
            async: true,
            cache: false,
            error: function (error) {
                hideLoading();
                console.log(error);
                NotifyError("Error. " + error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result !== null && result.exist) {
                    hideLoading();
                    NotifyWarning("No puede realizar otra Asignación de Liquidación dentro del mismo navegador. Verifique el caso.");
                    return;
                } else {
                    var data = {
                        ids: selectedRows,
                        id: 0
                    };

                    showPage("LiquidationMaterial/Edit", data);
                }
                
            },
            complete: function () {
                //hideLoading();
            }
        });
        


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