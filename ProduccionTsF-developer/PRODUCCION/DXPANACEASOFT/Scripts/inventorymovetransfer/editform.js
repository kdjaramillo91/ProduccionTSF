// GLOBAL BUTTONS ACTIONS

function Update(approve) {
    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);

    var valid = true;
    var validDocumentCut = ASPxClientEdit.ValidateEditorsInContainerById("documentCut", null, true);
    var validMainTabInventoryMove = ASPxClientEdit.ValidateEditorsInContainerById("mainTabInventoryMove", null, true);

    //console.log("validDocumentCut: " + validDocumentCut);
    //console.log("validMainTabRequest: " + validMainTabRequest);
    //console.log("validFormEditPurchaseRequest: " + validFormEditPurchaseRequest);
    //console.log("validTabDocument: " + validTabDocument);
    //console.log("validTabRequest: " + validTabRequest);

    if (validDocumentCut) {
        UpdateTabImage({ isValid: true }, "tabDocument");
    } else {
        UpdateTabImage({ isValid: false }, "tabDocument");
        valid = false;
    }

    if (validMainTabInventoryMove) {
        UpdateTabImage({ isValid: true }, "tabInventoryMove");
    } else {
        UpdateTabImage({ isValid: false }, "tabInventoryMove");
        valid = false;
    }

    //if (!valid) {
    //    UpdateTabImage({ isValid: false }, "tabInventoryMove");
    //}

    if (gridViewMoveDetails.cpRowsCount === 0 || gridViewMoveDetails.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabDetails");
        valid = false;
    } else {
        UpdateTabImage({ isValid: true }, "tabDetails");
    }

    if (valid) {
        var id = $("#id_inventoryMove").val();
        var codeDocumentType = $("#codeDocumentType").val();
        var natureMoveIMTmp = $("#natureMoveIM").val();
        
        var data = "id=" + id + "&" + "approve=" + approve + "&" + "codeDocumentType=" + codeDocumentType + "&" + "natureMoveIMTmp=" + natureMoveIMTmp + "&" + $("#formEditInventoryMove").serialize();

        var url = (id === "0") ? "InventoryMoveTransfer/InventoryMovesPartialAddNew"
                               : "InventoryMoveTransfer/InventoryMovesPartialUpdate";
        showForm(url, data);
    }

}

function ButtonUpdate_Click(s, e) {

    Update(false);

}

function ButtonClose_Click(s, e) {
    var codeDocumentType = $("#codeDocumentType").val();
    if (codeDocumentType == "04") {//Ingreso x Orden de Compra
        showPage("InventoryMoveTransfer/IndexEntryMovePurchaseOrder", null);

    } else {
        if (codeDocumentType == "03") {//Ingreso
            showPage("InventoryMoveTransfer/IndexEntryMove", null);

        } else {
            if (codeDocumentType == "05") {//Egreso
                showPage("InventoryMoveTransfer/IndexExitMove", null);

            } else {
                if (codeDocumentType == "32") {//Egreso por Transferencia
                    showPage("InventoryMoveTransfer/IndexTransferExitMove", null);

                } else {
                    if (codeDocumentType == "34") {//Ingreso Por Transferencia
                        showPage("InventoryMoveTransfer/IndexTransferEntryMove", null);

                    } else {
                        showPage("InventoryMoveTransfer/Index", null);

                    }

                }

            }

        }

    }
}

function OnSelectedInventoryReasonChanged(s, e) {
    if (s.GetValue() != null) {

        $.ajax({
            url: "InventoryMoveTransfer/InventoryReasonChanged",
            type: "post",
            data: { idIR: s.GetValue() },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result !== null && result !== undefined) {
                    $("#codeDocumentType").val(result.codeIR);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}
// EDIT FORM ACTIONS

function AddNewDocument(s, e) {
    var codeDocumentType = $("#codeDocumentType").val();
    AddNewInventoryMoveGeneral(codeDocumentType);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
    showPage("InventoryMoveTransfer/InventoryMoveCopy", { id: $("#id_inventoryMove").val() });
}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        Update(true);
    }, "¿Desea aprobar el movimiento de inventario?");
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_inventoryMove").val()
    //    };
    //    showForm("InventoryMoveTransfer/Approve", data);
    //}, "¿Desea aprobar el documento?");
}

function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_inventoryMove").val()
        };
        showForm("InventoryMoveTransfer/Autorize", data);
    }, "¿Desea autorizar el documento?");
}

function ProtectDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_inventoryMove").val()
        };
        showForm("InventoryMoveTransfer/Protect", data);
    }, "¿Desea cerrar el documento?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_inventoryMove").val()
        };
        showForm("InventoryMoveTransfer/Cancel", data);
    }, "¿Desea anular el movimiento de inventario?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_inventoryMove").val()
        };
        showForm("InventoryMoveTransfer/Revert", data);
    }, "¿Desea reversar el movimiento de inventario?");
}

function ShowDocumentHistory(s, e) {

}

function PrintDocument(s, e) {
    
}

// EDITOR'S EVENTS

function WarehouseCombo_SelectedIndexChanged(s, e) {

    id_warehouseLocation.SetValue(null);
    id_warehouseLocation.ClearItems();

    var data = s.GetValue();
    if (data === null) {
        return;
    }
    //var codeDocumentType = $("#codeDocumentType").val();

    //purchaseOrderNumber.SetText("");
    //remissionGuideNumber.SetText("");
    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnit").SetText("");
    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(null);// 
    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(null);// SetValue("");

    if (data != null) {

        $.ajax({
            url: "InventoryMoveTransfer/WarehouseChangeData",
            type: "post",
            data: { id_warehouse: data },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result !== null && result !== undefined) {
                    var arrayFieldStr = [];
                    arrayFieldStr.push("name");
                    UpdateDetailObjects(id_warehouseLocation, result.warehouseLocations, arrayFieldStr);
                    id_warehouseLocation.SetValue(result.id_warehouseLocation);

                    gridViewMoveDetails.UnselectRows();
                    gridViewMoveDetails.PerformCallback();
                    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnit").SetText(result.metricUnit);
                    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);
                    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function WarehouseLocationCombo_SelectedIndexChanged(s, e) {
    var data = s.GetValue();
    if (data === null) {
        return;
    }

    if (data != null) {

        $.ajax({
            url: "InventoryMoveTransfer/WarehouseLocationChangeData",
            type: "post",
            data: { id_warehouseLocation: data },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result !== null && result !== undefined) {
                    gridViewMoveDetails.UnselectRows();
                    gridViewMoveDetails.PerformCallback();
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}


//Grid View

//function OnGridViewInit(s, e) {
//    UpdateTitlePanelDetails();
//}

//function UpdateTitlePanelDetails() {
//    //if (gv === null || gv === undefined)
//    //    return;
//    var selectedFilteredRowCount = GetSelectedFilteredRowCountDetails();
//    var text = "Total de elementos seleccionados: <b>" + gridViewMoveDetails.GetSelectedRowCount() + "</b>";
//    var hiddenSelectedRowCount = gridViewMoveDetails.GetSelectedRowCount() - GetSelectedFilteredRowCountDetails();
//    if (hiddenSelectedRowCount > 0)
//        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
//    text += "<br />";
//    $("#lblInfo").html(text);
//    if ($("#selectAllMode").val() !== "AllPages") {
//        SetElementVisibility("lnkSelectAllRows", gridViewMoveDetails.GetSelectedRowCount() > 0 && gridViewMoveDetails.cpVisibleRowCount > selectedFilteredRowCount);
//        SetElementVisibility("lnkClearSelection", gridViewMoveDetails.GetSelectedRowCount() > 0);
//    }
//    btnRemove.SetEnabled(gridViewMoveDetails.GetSelectedRowCount() > 0);
//    var codeDocumentType = $("#codeDocumentType").val();
//    if(codeDocumentType == "04")//Ingreso x Orden de Compra
//    {
//        btnRemove.SetEnabled(false);
//        btnAdd.SetEnabled(false);
//    }
//}

//function GetSelectedFilteredRowCountDetails() {
//    return gridViewMoveDetails.cpFilteredRowCountWithoutPage +
//           gridViewMoveDetails.GetSelectedKeysOnPage().length;
//}

//function OnGridViewSelectionChanged(s, e) {
//    UpdateTitlePanelDetails();
//    s.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbacksDetail);
//}

//function GetSelectedFieldValuesCallbacksDetail(values) {
//    var selectedRows = [];
//    for (var i = 0; i < values.length; i++) {
//        selectedRows.push(values[i]);
//    }
//}

//var customCommand = "";

//function OnGridViewBeginCallback(s, e) {
//    customCommand = e.command;
//}

//function OnGridViewEndCallback(s, e) {
//    UpdateTitlePanelDetails();
//    //if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
//    //    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
//    //}
//    //if (gv !== gvProductionLotEditFormProductionLotQualityAnalysissDetail) {
//    //    if (gv !== gvProductionLotEditFormItemsDetail) {
//    //        if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
//    //            s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
//    //        }
//    //    } else {
//    //        if (s.GetEditor("id") !== null && s.GetEditor("id") !== undefined) {
//    //            s.GetEditor("id").SetEnabled(customCommand === "ADDNEWROW");
//    //        }
//    //    }

//    //} else {
//    //    if (s.GetEditor("id_qualityAnalysis") !== null && s.GetEditor("id_qualityAnalysis") !== undefined) {
//    //        s.GetEditor("id_qualityAnalysis").SetEnabled(customCommand === "ADDNEWROW");
//    //    }
//    //}

//    //UpdateProductionLotTotals();
//    //gvProductionLotEditFormProductionLotPackingMaterialsDetail.PerformCallback();
//}

//function gvEditDetailsClearSelection() {
//    gridViewMoveDetails.UnselectRows();
//}

//function gvEditDetailsSelectAllRows() {
//    gridViewMoveDetails.SelectRows();
//}

// DETAILS ACTIONS BUTTONS

function AddNewDetail(s, e) {
    gridViewMoveDetails.AddNewRow();
}

function RemoveDetail(s, e) {
    gridViewMoveDetails.UnselectRows();
    gridViewMoveDetails.PerformCallback();
}

function RefreshDetail(s, e) {
    gridViewMoveDetails.UnselectRows();
    gridViewMoveDetails.PerformCallback();
}

function UpdateView() {

    var id = parseInt($("#id_inventoryMove").val());

    // FORM BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(id === 0);
    btnCopy.SetEnabled(false);//id !== 0);
    btnHistory.SetEnabled(id !== 0);
    btnPrint.SetEnabled(id !== 0);

    // STATES BUTTONS

    $.ajax({
        url: "InventoryMoveTransfer/Actions",
        type: "post",
        data: { id: id },
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            btnApprove.SetEnabled(result.btnApprove);
            btnAutorize.SetEnabled(result.btnAutorize);
            btnProtect.SetEnabled(result.btnProtect);
            btnCancel.SetEnabled(result.btnCancel);
            btnRevert.SetEnabled(result.btnRevert);
        },
        complete: function (result) {
            //hideLoading();
        }
    });

    //// DETAILS BUTTONS
    //btnNewDetail.SetEnabled(id === 0);

}

function UpdatePagination() {

    var current_page = 1;
    $.ajax({
        url: "InventoryMoveTransfer/InitializePagination",
        type: "post",
        data: { id_inventoryMove: $("#id_inventoryMove").val(), codeDocumentType: $("#codeDocumentType").val() },
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            $("#pagination").attr("data-max-page", result.maximunPages);
            current_page = result.currentPage;
        },
        complete: function () {
        }
    });

    $('.pagination').current_page = current_page;
}

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 2000);
    }
}

function init() {
    UpdatePagination();
    AutoCloseAlert();
}

$(function () {

    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateView();
        }
    }, 100);

    init();
});