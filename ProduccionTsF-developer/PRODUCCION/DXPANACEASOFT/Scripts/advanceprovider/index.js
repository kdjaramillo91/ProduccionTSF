// FILTERS FORM ACTIONS

function btnSearchAP_click(s, e) {

    var data = $("#formFilterAdvanceProvider").serialize();

    if (data != null) {
        $.ajax({
            url: "AdvanceProvider/APAdvanceProviderResults",
            type: "post",
            data: data,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                $("#btnCollapse").click();
                $("#results").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });
    }
    event.preventDefault();
}

function btnSearchAPDirecto_click(s, e) {
    // 


    var data = $("#formFilterAdvanceProvider").serialize();
    var valor = internalNumber.GetValue();
    if (valor == null || valor == "") {
        alert("Falta valor de Número de Lote");
    } else {
        if (data != null) {
            $.ajax({
                url: "AdvanceProvider/APAdvanceProviderResultsDirecto",
                type: "post",
                data: data,
                async: true,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                    $("#btnCollapse").click();
                    $("#results").html(result);
                },
                complete: function () {
                    hideLoading();
                }
            });
        }
    }
    //// 

    event.preventDefault();
}

function btnSearchLot_click(s, e) {

    var data = $("#formFilterAdvanceProvider").serialize();

    if (data != null) {
        $.ajax({
            url: "AdvanceProvider/APAdvanceProviderPLResults",
            type: "post",
            data: data,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                $("#btnCollapse").click();
                $("#results").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });
    }
    event.preventDefault();
}
function btnSearchLotDirecto_click(s, e) {
    // 
    var data = $("#formFilterAdvanceProvider").serialize();
    var valor = internalNumber.GetValue();
    if (valor == null || valor == "") {
        alert("Falta valor de Número de Lote");
    } else {
        if (data != null) {
            $.ajax({
                url: "AdvanceProvider/APAdvanceProviderPLResultsDirecto",
                type: "post",
                data: data,
                async: true,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    showLoading();
                },
                success: function (result) {
                    $("#btnCollapse").click();
                    $("#results").html(result);
                },
                complete: function () {
                    hideLoading();
                }
            });
        }
    }

    event.preventDefault();
}

function btnClear_click(s, e) {

    id_documentState.SetSelectedItem(null);
    number.SetText("");
    startEmissionDate.SetDate(null);
    endEmissionDate.SetDate(null);
    items.ClearTokenCollection();

    id_provider.SetSelectedItem(null);
    id_buyer.SetSelectedItem(null);
    id_priceList.SetSelectedItem(null);
    id_paymentTerm.SetSelectedItem(null);
    id_paymentMethod.SetSelectedItem(null);
    filterLogistic.SetChecked(true);
}

function AddNewItemManualPL(s, e) {
    var data = {
        id: 0
    };

    //showPage("AdvanceProvider/FormEditAdvanceProviderPL", data);
}

function AddNewItemManual(s, e) {
    var data = {
        id: 0
    };

    //showPage("AdvanceProvider/FormEditAdvanceProvider", data);
}

// GRIDVIEW ADVANCE PROVIDER SELECTION

function OnRowDoubleClick(s, e) {
    s.GetRowValues(e.visibleIndex, "id", function (value) {
        showPage("AdvanceProvider/FormEditAdvanceProvider", { id: value });
    });
}

function AdvanceProviderOnGridViewInit(s, e) {
    AdvanceProviderUpdateTitlePanel();
}

function AdvanceProviderOnGridViewSelectionChanged(s, e) {
    AdvanceProviderUpdateTitlePanel();
}

function AdvanceProviderOnGridViewEndCallback() {
    AdvanceProviderUpdateTitlePanel();
}

function AdvanceProviderUpdateTitlePanel() {
    var selectedFilteredRowCount = AdvanceProviderGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvAdvanceProvider.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvAdvanceProvider.GetSelectedRowCount() - AdvanceProviderGetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvAdvanceProvider.GetSelectedRowCount() > 0 && gvAdvanceProvider.cpVisibleRowCountAP > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvAdvanceProvider.GetSelectedRowCount() > 0);


    btnApprove.SetEnabled(false);
    btnAutorize.SetEnabled(false);
    btnProtect.SetEnabled(false);
    btnCancel.SetEnabled(false);
    btnRevert.SetEnabled(false);
    btnHistory.SetEnabled(false);
    btnPrint.SetEnabled(false);

    btnCopy.SetEnabled(gvAdvanceProvider.GetSelectedRowCount() === 1);

}

function AdvanceProviderGetSelectedFilteredRowCount() {
    return gvAdvanceProvider.cpFilteredRowCountWithoutPageAP + gvAdvanceProvider.GetSelectedKeysOnPage().length;
}

function AdvanceProviderSelectAllRows() {
    gvAdvanceProvider.SelectRows();
}

function AdvanceProviderClearSelection() {
    gvAdvanceProvider.UnselectRows();
}

function AdvanceProviderGridViewCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnEditRow") {
        var grid = MVCxClientGridView.Cast(s);
        var data = {
            id: grid.GetRowKey(e.visibleIndex)
        };
        showPage("AdvanceProvider/FormEditAdvanceProvider", data);
    }
}

// GRIDVIEW ADVANCE PROVIDER PRODUCTION LOT SELECTION

function OnRowDoubleClickProductionLot(s, e) {
    s.GetRowValues(e.visibleIndex, "id", function (value) {
        showPage("AdvanceProvider/FormEditAdvanceProviderPL", { id: value });
    });
}

function AdvanceProviderPLOnGridViewInit(s, e) {
    AdvanceProviderPLUpdateTitlePanel();
}

function AdvanceProviderPLOnGridViewSelectionChanged(s, e) {
    AdvanceProviderPLUpdateTitlePanel();
}

function AdvanceProviderPLOnGridViewEndCallback() {
    AdvanceProviderPLUpdateTitlePanel();
}

function AdvanceProviderPLUpdateTitlePanel() {
    var selectedFilteredRowCount = AdvanceProviderPLGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvAdvanceProviderPL.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvAdvanceProviderPL.GetSelectedRowCount() - AdvanceProviderPLGetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvAdvanceProviderPL.GetSelectedRowCount() > 0 && gvAdvanceProviderPL.cpVisibleRowCountPL > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvAdvanceProviderPL.GetSelectedRowCount() > 0);


    btnApprovePL.SetEnabled(false);
    btnAutorizePL.SetEnabled(false);
    btnProtectPL.SetEnabled(false);
    btnCancelPL.SetEnabled(false);
    btnRevertPL.SetEnabled(false);
    btnHistoryPL.SetEnabled(false);
    btnPrintPL.SetEnabled(false);

    btnCopyPL.SetEnabled(gvAdvanceProviderPL.GetSelectedRowCount() === 1);

}

function AdvanceProviderPLGetSelectedFilteredRowCount() {
    return gvAdvanceProviderPL.cpFilteredRowCountWithoutPagePL + gvAdvanceProviderPL.GetSelectedKeysOnPage().length;
}

function AdvanceProviderPLSelectAllRows() {
    gvAdvanceProviderPL.SelectRows();
}

function AdvanceProviderPLClearSelection() {
    gvAdvanceProviderPL.UnselectRows();
}

function AdvanceProviderPLGridViewCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnMakeAdvanceRow") {
        var grid = MVCxClientGridView.Cast(s);
        var data = {
            id: grid.GetRowKey(e.visibleIndex)
        };
        showPage("AdvanceProvider/FormEditAdvanceProviderPL", data);
        //s.GetRowValues(e.visibleIndex, "id", function (value) {
        //    showPage("AdvanceProvider/FormEditAdvanceProviderPL", { id: value });
        //});
    }
}


// GRIDVIEW ADVANCE PROVIDER PRODUCTION LOT RESULT ACTIONS BUTTONS

function PerformDocumentActionPL(url) {
    gvAdvanceProviderPL.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: url,
            type: "post",
            data: { ids: selectedRows },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                //console.log(result);
            },
            complete: function () {
                gvAdvanceProviderPL.PerformCallback();
            }
        });

    });
}

function AddNewDocumentPL(s, e) {
    AddNewItemManualPL(s, e);
}

function CopyDocumentPL(s, e) {
    gvAdvanceProviderPL.GetSelectedFieldValues("id", function (values) {
        if (values.length > 0) {
            showPage("AdvanceProvider/AdvanceProviderPLCopy", { id: values[0] });
        }
    });
}

function ApproveDocumentsPL(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("AdvanceProvider/ApproveDocumentsPL");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocumentsPL(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("AdvanceProvider/AutorizeDocumentsPL");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocumentsPL(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("AdvanceProvider/ProtectDocumentsPL");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocumentsPL(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("AdvanceProvider/CancelDocumentsPL");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocumentsPL(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("AdvanceProvider/RevertDocumentsPL");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistoryPL(s, e) {

}

function PrintPL(s, e) {

    gvAdvanceProviderPL.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        var id = selectedRows[0];

        var data = { ReportName: "AdvanceProvidersReport", ReportDescription: "Lotes de Producción", ListReportParameter: [] };
        if (id !== 0 && id !== null) {
            var ids = [id];
            $.ajax({
                url: 'AdvanceProvider/PurchaseOrdersReport?id=' + id,
                data: data,
                async: true,
                cache: false,
                type: 'POST',
                beforeSend: function () {
                    showLoading();
                },
                success: function (response) {
                    try {
                        if (response.isvalid) {
                            var reportModel = response.reportModel;
                            var url = 'Report/Index?reportModel=' + reportModel;
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


    });

}

// GRIDVIEW ADVANCE PROVIDER RESULT ACTIONS BUTTONS

function PerformDocumentAction(url) {
    gvAdvanceProvider.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: url,
            type: "post",
            data: { ids: selectedRows },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                //console.log(result);
            },
            complete: function () {
                //hideLoading();
                gvAdvanceProvider.PerformCallback();
                // gvPurchaseOrders.UnselectRows();
            }
        });

    });
}

function AddNewDocument(s, e) {
    AddNewItemManual(s, e);
}

function CopyDocument(s, e) {
    //gvAdvanceProvider.GetSelectedFieldValues("id", function (values) {
    //    if (values.length > 0) {
    //        showPage("AdvanceProvider/AdvanceProviderCopy", { id: values[0] });
    //    }
    //});
}

function ApproveDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("AdvanceProvider/ApproveDocuments");
    }, "¿Desea aprobar los documentos seleccionados?");
}

function AutorizeDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("AdvanceProvider/AutorizeDocuments");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("AdvanceProvider/ProtectDocuments");
    }, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("AdvanceProvider/CancelDocuments");
    }, "¿Desea anular los documentos seleccionados?");
}

function RevertDocuments(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("AdvanceProvider/RevertDocuments");
    }, "¿Desea reversar los documentos seleccionados?");
}

function ShowHistory(s, e) {

}

function Print(s, e) {
    gvAdvanceProvider.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        var id = selectedRows[0];

        var data = { ReportName: "AdvanceProviderReport", ReportDescription: "Anticipo a Proveedores", ListReportParameter: [] };
        if (id !== 0 && id !== null) {
            var ids = [id];
            $.ajax({
                url: 'AdvanceProvider/AdvanceProviderReport?id=' + id,
                data: data,
                async: true,
                cache: false,
                type: 'POST',
                beforeSend: function () {
                    showLoading();
                },
                success: function (response) {
                    try {
                        if (response.isvalid) {
                            var reportModel = response.reportModel;
                            var url = 'Report/Index?reportModel=' + reportModel;
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


    });

}


// INDIVIDUAL FUNCTION BUTTONS


function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        PerformDocumentAction("AdvanceProvider/AutorizeDocument");
    }, "¿Desea autorizar los documentos seleccionados?");
}

function ProtectDocument(s, e) {
    //showConfirmationDialog(function () {
    //    PerformDocumentAction("AdvanceProvider/ProtectDocuments");
    //}, "¿Desea cerrar los documentos seleccionados?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_ap").val()
        };
        showForm("AdvanceProvider/CancelDocument", data);
    }, "¿Desea Cancelar el Documento?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_ap").val()
        };
        showForm("AdvanceProvider/RevertDocument", data);
    }, "¿Desea reversar el Documento?");
}

function ShowDocumentHistory(s, e) {
}

// COMMON GRIDVIEW AUXILIARS FUNCTIONS

function GetGridViewSelectedRows(gv, key) {
    var selectedRows = [];
    gv.GetSelectedFieldValues(key, function (values) {
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
    });
    return selectedRows;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function ButtonUpdate_Click() {
    Update(false);
}

function Update(approve) {

    var valid = true;

    if (gvAdvanceProviderPLDetail.cpRowsCount === 0 || gvAdvanceProviderPLDetail.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabAdvanceProviderPL");
        valid = false;
    } else {
        UpdateTabImage({ isValid: true }, "tabAdvanceProviderPL");
    }


    if (valid) {
        var id = parseInt(document.getElementById("id_ap").getAttribute("idAdvanceProvider"));
        var data = "id=" + id + "&approve=" + approve + "&" + $("#formEditAdvanceProviderPL").serialize();
        var url = (id === "0" || id === 0) ? "AdvanceProvider/AdvanceProviderPartialAddNew"
            : "AdvanceProvider/AdvanceProviderPartialUpdate";

        console.log("url: " + url);
        console.log("data: " + data);
        showForm(url, data);
    }
}

function ButtonCancel_Click() {
    showPage("AdvanceProvider/Index", null);
}

function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }
    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}