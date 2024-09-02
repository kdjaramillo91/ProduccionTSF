var OnItemOriginValidation = function (s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
};

var OnTipoProductoValidation = function (s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
};

var OnItemOriginSelectionIndexChanged = function (s, e) {
};

var OnTipoProductoSelecionIndexChanged = function (s, e) {
};

var OnLimpiarFiltrosButtonClick = function (s, e) {
    id_itemOrigin.SetValue(null);
    TipoProductoQueryText.SetValue(null);
};

var OnConsultarDatosButtonClick = function (s, e) {
    if (ASPxClientEdit.ValidateEditorsInContainerById("dataExportQueryForm", "", true)) {
        var queryData = {
            id_itemOrigen: id_itemOrigin.GetValue(),
            tipoBusquedaProducto: TipoProductoQueryText.GetValue(),
            isCallback: false,
        };
        $.ajax({
            url: "ItemFormulation/Query",
            type: "post",
            dataType: 'html',
            data: queryData,
            async: true,
            cache: false,
            error: function (error) {
                console.error(error);
                try {
                    $("#results").html(error.responseText);
                }
                catch (error2) {
                    console.error(error2);
                    showErrorMessage("Ocurrió un error al procesar la respuesta de error.");
                }
                hideLoading();
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                //$("#btnCollapse").click();
                $("#results").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });
	}
};

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}
function OnGridViewBeginCallBack(s, e) {
    e.customArgs["id_itemOrigen"] = gvProductosCopiar.cpItemOrigen;
    e.customArgs["tipoBusquedaProducto"] = gvProductosCopiar.cpTipoBusquedaProducto;
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();
    var text = "Total de elementos seleccionados: <b>" + gvProductosCopiar.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvProductosCopiar.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvProductosCopiar.GetSelectedRowCount() > 0 && gvProductosCopiar.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvProductosCopiar.GetSelectedRowCount() > 0);

    btnCopy.SetEnabled(gvProductosCopiar.GetSelectedRowCount() > 0 && gvProductosCopiar.cpPuedeCopiar);
}

function GetSelectedFilteredRowCount() {
    return gvProductosCopiar.cpFilteredRowCountWithoutPage + gvProductosCopiar.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvProductosCopiar.SelectRows();
}

function UnselectAllRows() {
    gvProductosCopiar.UnselectRows();
}

function CopyItemsFormulation(s, e) {
    var idOrigen = id_itemOrigin.GetValue();
    var tipoProducto = TipoProductoQueryText.GetValue();

    if (idOrigen != null && tipoProducto != null) {

        gvProductosCopiar.GetSelectedFieldValues("id", function (values) {
            var selectedRows = [];

            for (var i = 0; i < values.length; i++) {
                selectedRows.push(values[i]);
            }

            var copyData = {
                idItemOrigen: id_itemOrigin.GetValue(),
                tipoBusquedaProducto: TipoProductoQueryText.GetValue(),
                idsFormulationItems: selectedRows
            };
            showCopyFormulationConfirmationDialog(function () {
                $.ajax({
                    url: "ItemFormulation/CopyFormulationSelectedItem",
                    type: "post",
                    data: copyData,
                    async: true,
                    cache: false,
                    error: function (error) {
                        console.log(error);
                    },
                    beforeSend: function () {
                        showLoading();
                    },
                    success: function (result) {
                        if (result.isValid) {
                            showSuccessMessage(result.message);
                        }
                        else {
                            showErrorMessage(result.message);
                        }
                    },
                    complete: function () {
                        gvProductosCopiar.PerformCallback();
                        gvProductosCopiar.UnselectRows();
                        hideLoading();
                    }
                });
            });
        });
    } else {
        showWarningMessage('Debe completar los datos del formulario.');
	}

}

function init() {

}

$(function () {
    init();
});