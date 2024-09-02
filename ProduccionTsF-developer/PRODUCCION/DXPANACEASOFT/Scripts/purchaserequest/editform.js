// DIALOG BUTTONS ACTIONS
function Update(approve) {

    var valid = true;
    var validDocumentCut = ASPxClientEdit.ValidateEditorsInContainerById("documentCut", null, true);
    var validMainTabRequest = ASPxClientEdit.ValidateEditorsInContainerById("mainTabRequest", null, true);
    //var validFormEditPurchaseRequest = ASPxClientEdit.ValidateEditorsInContainerById("formEditPurchaseRequest", null, true);
    //var validTabDocument = ASPxClientEdit.ValidateEditorsInContainerById("tabDocument", null, true);
    //var validTabRequest = ASPxClientEdit.ValidateEditorsInContainerById("tabRequest", null, true);

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

    if (validMainTabRequest) {
        UpdateTabImage({ isValid: true }, "tabRequest");
    } else {
        UpdateTabImage({ isValid: false }, "tabRequest");
        valid = false;
    }

    if (gvPurchaseRequestEditFormDetail.cpRowsCount === 0 || gvPurchaseRequestEditFormDetail.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabDetails");
        valid = false;
    } else {
        UpdateTabImage({ isValid: true }, "tabDetails");
    }

    if (approve && !gvPurchaseRequestEditFormDetail.IsEditing()) {
        $.ajax({
            url: "PurchaseRequest/ItsAllBusinessOportunityOK",
            type: "post",
            data: null,
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result !== null) {
                    if (result.ItsAllBusinessOportunityOK == 0) {
                        UpdateTabImage({ isValid: false }, "tabDetails");
                        valid = false;
                        //e.errorText = result.Error;
                        var msgErrorAux = ErrorMessage(result.Error);
                        gridMessageError.SetText(msgErrorAux);
                        $("#GridMessageError").show();
                    }
                    else {
                        $.ajax({
                            url: "PurchaseRequest/ItsAllProductionScheduleOK",
                            type: "post",
                            data: null,
                            async: false,
                            cache: false,
                            error: function (error) {
                                console.log(error);
                            },
                            beforeSend: function () {
                                showLoading();
                            },
                            success: function (result) {
                                if (result !== null) {
                                    if (result.ItsAllProductionScheduleOK == 0) {
                                        UpdateTabImage({ isValid: false }, "tabDetails");
                                        valid = false;
                                        //e.errorText = result.Error;
                                        var msgErrorAux = ErrorMessage(result.Error);
                                        gridMessageError.SetText(msgErrorAux);
                                        $("#GridMessageError").show();
                                    }
                                    //else {
                                    //    id_itemIniAux = 0
                                    //    id_purchaseRequestDetailIniAux = 0
                                    //}
                                }
                            },
                            complete: function () {
                                hideLoading();
                                //showPage("PurchasePlanning/Index", null);
                            }
                        });
                    }
                }
            },
            complete: function () {
                hideLoading();
                //showPage("PurchasePlanning/Index", null);
            }
        });
    }


    if (valid) {
        var id = $("#id_purchaseRequest").val();

        var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#formEditPurchaseRequest").serialize();

        var url = (id === "0") ? "PurchaseRequest/PurchaseRequestsPartialAddNew"
            : "PurchaseRequest/PurchaseRequestsPartialUpdate";

        showForm(url, data);
    }


}

function ButtonUpdate_Click(s, e) {

    Update(false);


}

/*function ButtonUpdateClose_Click(s, e) {
    var id = $("#id_purchaseRequest").val();

    var data = "id=" + id + "&" + $("#formEditPurchaseRequest").serialize();

    var url = (id === "0") ? "PurchaseRequest/PurchaseRequestsPartialAddNew"
                           : "PurchaseRequest/PurchaseRequestsPartialUpdate";

    if (data != null) {
        $.ajax({
            url: url,
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
                $("#maincontent").html(result);

            },
            complete: function () {
                hideLoading();
                showPage("PurchaseRequest/Index", null);
            }
        });
    }

}*/

function ButtonCancel_Click(s, e) {
    showPage("PurchaseRequest/Index", null);
}

//SOLICITUD BUTTONS ACTIONS

//btnNew
function AddNewDocument(s, e) {
    var data = {
        id: 0
    };

    showPage("PurchaseRequest/FormEditPurchaseRequest", data);
}

//btnSave
function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

//btnSaveClose
function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

//btnCopy
function CopyDocument(s, e) {
    showPage("PurchaseRequest/PurchaseRequestCopy", { id: $("#id_purchaseRequest").val() });
}

//btnApprove
function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        Update(true);
    }, "¿Desea aprobar el requerimiento de compra?");

    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_purchaseRequest").val()
    //    };
    //    showForm("PurchaseRequest/Approve", data);
    //}, "¿Desea aprobar el documento?");
}

//btnAutorize
function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_purchaseRequest").val()
        };
        showForm("PurchaseRequest/Autorize", data);
    }, "¿Desea autorizar el requerimiento de compra?");
}

//btnProtect
function ProtectDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_purchaseRequest").val()
        };
        showForm("PurchaseRequest/Protect", data);
    }, "¿Desea cerrar el requerimiento de compra?");
}

//btnCancel
function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_purchaseRequest").val()
        };
        showForm("PurchaseRequest/Cancel", data);
    }, "¿Desea anular el requerimiento de compra?");
}

//btnRevert
function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_purchaseRequest").val()
        };
        showForm("PurchaseRequest/Revert", data);
    }, "¿Desea reversar el requerimiento de compra?");
}

//btnHistory
function ShowDocumentHistory(s, e) {

}

//BTNPRINT

function PrintDocument(s, e) {

    var id = $("#id_purchaseRequest").val();
    var data = { ReportName: "PurchaseRequestReport", ReportDescription: "Requerimiento Compra", ListReportParameter: [] };

    $.ajax({
        url: 'PurchaseRequest/PurchaseRequestReport',
        data: { idPurchaseRequest: id},
        async: true,
        cache: false,
        type: 'POST',
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            try {
                if (result != undefined) {
                    var reportTdr = result.nameQS;
                    var url = 'ReportProd/Index?trepd=' + reportTdr;
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

//function PrintDocument(s, e) {
//    var id = $("#id_purchaseRequest").val();

//    if (id !== null && id !== undefined) {
//        var ids = [id];
//        $.ajax({
//            url: "PurchaseRequest/PurchaseRequestReport",
//            type: "post",
//            data: { id: id },
//            async: true,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//            },
//            beforeSend: function () {
//                showLoading();
//            },
//            success: function (result) {
//                $("#maincontent").html(result);
//            },
//            complete: function () {
//                hideLoading();
//            }
//        });
//    }

//}

//DETALLE DE SOLICITUD

function PurchaseRequestDetail_OnBeginCallback(s, e) {

}

// DETALLE DE SOLICITUD BUTTONS ACTIONS

function AddNewDetail(s, e) {
    gvPurchaseRequestEditFormDetail.AddNewRow();
}

function RemoveDetail(s, e) {

    gvPurchaseRequestEditFormDetail.GetSelectedFieldValues("id", function (values) {
        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "PurchaseRequest/PurchaseRequestDetailsDeleteSelectedItems",
            type: "post",
            data: { itemIds: selectedRows },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                //$("#maincontent").html(result);
            },
            complete: function () {
                gvPurchaseRequestEditFormDetail.PerformCallback();
                gvPurchaseRequestEditFormDetail.UnselectRows();
                hideLoading();
            }
        });
    });
}

function RefreshDetail(s, e) {
    showLoading();
    gvPurchaseRequestEditFormDetail.PerformCallback();
    gvPurchaseRequestEditFormDetail.UnselectRows();
    hideLoading();
}


//DETALLE DE SOLICITUD COMBOS

var id_itemIniAux = 0;
var id_proposedProviderIniAux = 0;
var errorMessage = "";

function OnItemValidation(s, e) {
    if (!runningValidation) {
        ValidateDetail();
    }
    //gridMessageError.SetText(result.Message);
    errorMessage = "";
    $("#GridMessageError").hide();
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Nombre del Producto: Es obligatorio.";
    } else {
        var data = {
            //id: gvPurchaseRequestEditFormDetail.cpEditingRowKey,
            id_itemNew: s.GetValue(),
            id_proposedProviderNew: 0,
        };
        if (data.id_itemNew != id_itemIniAux || data.id_proposedProviderNew != id_proposedProviderIniAux) {
            $.ajax({
                url: "PurchaseRequest/ItsRepeatedItemDetail",
                type: "post",
                data: data,
                async: false,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    //showLoading();
                },
                success: function (result) {
                    if (result !== null) {
                        if (result.itsRepeated == 1) {
                            e.isValid = false;
                            e.errorText = result.Error;
                            errorMessage = "- Nombre del Producto: " + result.Error;
                        } else {
                            id_itemIniAux = 0
                            id_proposedProviderIniAux = 0
                        }
                    }
                },
                complete: function () {
                    //hideLoading();
                }
            });
        }

    }
}

function OnGrammageValidation(s, e) {
    errorMessage = "";
    $("#GridMessageError").hide();
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Gramage: Es obligatorio.";
    }
}

function OnItemColorValidation(s, e) {
    errorMessage = "";
    $("#GridMessageError").hide();
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Color Referencia: Es obligatorio.";
    }
}

function OnQuantityRequestedValidation(s, e) {
    if (!runningValidation) {
        ValidateDetail();
    }

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Requerida: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Requerida: Es obligatoria.";
        }
    } else {
        if (s.GetValue() <= 0) {
            e.isValid = false;
            e.errorText = "Valor debe ser mayor a cero";
            if (errorMessage == null || errorMessage == "") {
                errorMessage = "- Cantidad Requerida: Valor debe ser mayor a cero.";
            } else {
                errorMessage += "</br>- Cantidad Requerida: Valor debe ser mayor a cero.";
            }
        } else {
            quantityApproved.SetValue(s.GetValue());
        }
    }
}

function OnQuantityApprovedValidation(s, e) {
    if (!runningValidation) {
        ValidateDetail();
    }

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage === null || errorMessage === "") {
            errorMessage = "- Cantidad Aprobada: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Aprobada: Es obligatoria.";
        }
        quantityOutstandingPurchase.SetValue(0);
    } else {
        if (s.GetValue() < 0) {
            e.isValid = false;
            e.errorText = "Valor debe ser mayor o igual a cero";
            if (errorMessage === null || errorMessage === "") {
                errorMessage = "- Cantidad Aprobada: Valor debe ser mayor o igual a cero.";
            } else {
                errorMessage += "</br>- Cantidad Aprobada: Valor debe ser mayor o igual a cero.";
            }
            quantityOutstandingPurchase.SetValue(0);
        } else {
            quantityOutstandingPurchase.SetValue(e.value);
        }
    }



    if (errorMessage !== null && errorMessage !== "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageError.SetText(msgErrorAux);
        $("#GridMessageError").show();
        //id_item.isValid = false;

    }
}

function ItemCombo_OnInit(s, e) {

    id_itemIniAux = s.GetValue();
    id_proposedProviderIniAux = 0;

    var data = {
        id_item: s.GetValue(),
        id_proposedProvider: 0
    };

    if ((gvPurchaseRequestEditFormDetail.cpEditingRowSaleProductionSchedule != 0 &&
        gvPurchaseRequestEditFormDetail.cpEditingRowSaleProductionSchedule != null) ||
        (gvPurchaseRequestEditFormDetail.cpEditingRowBusinessOportunity != 0 &&
            gvPurchaseRequestEditFormDetail.cpEditingRowBusinessOportunity != null)) {
        s.SetEnabled(false);
        //id_proposedProvider.SetEnabled(false);
        quantityRequested.SetEnabled(false);
    }

    $.ajax({
        url: "PurchaseRequest/InitItemCombo",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //id_company
            var arrayFieldStr = [];
            arrayFieldStr.push("masterCode");
            arrayFieldStr.push("name");
            arrayFieldStr.push("itemPurchaseInformationMetricUnitCode");
            UpdateDetailObjects(id_item, result.items, arrayFieldStr);

            //id_accountFor
            //  arrayFieldStr = [];
            //    arrayFieldStr.push("name");
            //  UpdateDetailObjects(id_proposedProvider, result.proposedProviders, arrayFieldStr);

        },
        complete: function () {
            //hideLoading();
        }
    });

}

var runningValidation = false;

function ValidateDetail() {

    runningValidation = true;
    OnItemValidation(id_item, id_item);
    OnGrammageFromToValidation_selectedIndexChanged(id_grammageFrom, id_grammageFrom);
    OnGrammageUpToValidation_selectedIndexChanged(id_grammageTo, id_grammageTo);
    OnQuantityRequestedValidation(quantityRequested, quantityRequested);
    OnQuantityApprovedValidation(quantityApproved, quantityApproved);
    runningValidation = false;
}

function ItemsCombo_DropDown(s, e) {
    $.ajax({
        url: "PurchaseRequest/GetPurchaseRequestDetails",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            for (var i = 0; i < id_item.GetItemCount(); i++) {
                var item = id_item.GetItem(i);
                if (result.indexOf(item.value) >= 0) {
                    id_item.RemoveItem(i);
                    i = -1;
                }
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function ItemsCombo_SelectedIndexChanged(s, e) {
    masterCode.SetText("");
    metricUnit.SetText("");
    //id_proposedProvider.SetSelectedItem(null);
    //currentStock.SetText("");
    //minimumStock.SetText("");

    var id_item = s.GetValue();

    if (id_item !== null && id_item !== undefined) {

        $.ajax({
            url: "PurchaseRequest/ItemDetailData",
            type: "post",
            data: { id_item: id_item },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result !== null && result != undefined) {
                    masterCode.SetText(result.masterCode);
                    metricUnit.SetText(result.metricUnit);
                    //id_proposedProvider.SetValue(result.id_proposedProvider);
                    //currentStock.SetValue(result.currentStock);
                    //minimumStock.SetValue(result.minimumStock);
                }
            },
            complete: function () {
                //hideLoading();
                //ValidateDetail();
            }
        });
    } else {
        //ValidateDetail();
    }
}

function ItemProvidersCombo_BeginCallback(s, e) {
    e.customArgs['id_item'] = id_item.GetValue();
}

function ItemsCombo_BeginCallback(s, e) {
    e.customArgs['id_item'] = gvPurchaseRequestEditFormDetail.GetEditor('id_item').GetValue();
}

function ProposedProviderCombo_SelectedIndexChanged(s, e) {
    //ValidateDetail();
}

function QuantityRequested_ValueChanged(s, e) {
    //ValidateDetail();
}

function QuantityApproved_ValueChanged(s, e) {
    //ValidateDetail();
    //var quantityApprovedAux = s.GetValue();
    //if (quantityApprovedAux != null) {
    //    if (quantityApprovedAux < 0) {
    //        quantityOutstandingPurchase.SetValue(quantityApprovedAux);
    //    }
    //} else {
    //    quantityOutstandingPurchase.SetValue(0);
    //}
}

//TabRequest

function OnPersonRequesting_SelectedIndexChanged(s, e) {
    UpdateDepartament(id_personRequesting.GetValue(), employeeDepartament, "request");
}

// DETAILS VIEW CALLBACKS

function PurchaseRequestDetailViewDetails_BeginCallback(s, e) {
    e.customArgs["id_purchaseRequest"] = $("#id_purchaseRequest").val();
    s.cpIdPurchaseRequest = $("#id_purchaseRequest").val();
}

// SELECTION

var customCommand = "";

function OnGridViewDetailsInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewDetailsSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewDetailsBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewDetailsEndCallback(s, e) {
    UpdateTitlePanel();

    //if (s.GetEditor("id_item") !== null && s.GetEditor("id_item") !== undefined) {
    //    s.GetEditor("id_item").SetEnabled(customCommand === "ADDNEWROW");
    //}

    //quantityOutstandingPurchase.SetEnabled(false);
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPurchaseRequestEditFormDetail.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseRequestEditFormDetail.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPurchaseRequestEditFormDetail.GetSelectedRowCount() > 0 && gvPurchaseRequestEditFormDetail.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseRequestEditFormDetail.GetSelectedRowCount() > 0);
    //}

    btnRemoveDetail.SetEnabled(gvPurchaseRequestEditFormDetail.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvPurchaseRequestEditFormDetail.cpFilteredRowCountWithoutPage + gvPurchaseRequestEditFormDetail.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function gvEditDetailsClearSelection() {
    gvPurchaseRequestEditFormDetail.UnselectRows();
}

function gvEditDetailsSelectAllRows() {
    gvPurchaseRequestEditFormDetail.SelectRows();
}

//QUANTYTIES CHANGE

function QuantityRequested_NumberChange(s, e) {
    quantityOutstandingPurchase.SetValue(s.GetValue());
}

// UPDATE VIEW

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        $(".alert-success").fadeTo(3000, 0.45, function () {
            $(".alert-success").alert('close');
        });
        //setTimeout(function () {
        //    $(".alert-success").alert('close');
        //}, 2000);
    }
    //if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
    //    setTimeout(function () {
    //        $(".alert-success").alert('close');
    //    }, 2000);
    //}
}

function UpdateView() {
    var id = parseInt($("#id_purchaseRequest").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnCopy.SetEnabled(id !== 0);

    // STATES BUTTONS

    $.ajax({
        url: "PurchaseRequest/Actions",
        type: "post",
        data: { id: id },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            btnApprove.SetEnabled(result.btnApprove);
            btnAutorize.SetEnabled(result.btnAutorize);
            btnProtect.SetEnabled(result.btnProtect);
            btnCancel.SetEnabled(result.btnCancel);
            btnRevert.SetEnabled(result.btnRevert);
        },
        complete: function (result) {
            hideLoading();
        }
    });

    // HISTORY BUTTON
    btnHistory.SetEnabled(id !== 0);

    // PRINT BUTTON
    btnPrint.SetEnabled(id !== 0);
}

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "PurchaseRequest/InitializePagination",
        type: "post",
        data: { id_purchaseRequest: $("#id_purchaseRequest").val() },
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

//GRAMMAGE VALIDATION

function OnGrammageUpToValidation_selectedIndexChanged(s, e) {
    if (!runningValidation) {
        ValidateDetail();
    }
    //// 
    var grammaUpToTmp = s.GetValue();
    if (grammaUpToTmp === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage === null || errorMessage === "") {
            errorMessage = "- Gramaje Hasta: Es obligatorio.";
        } else {
            errorMessage += "</br>- Gramaje Hasta: Es obligatorio.";
        }
    } else {
        var grammageFromTmp = id_grammageFrom.GetValue();
        //var grammageFromTmp = id_grammageFrom.GetSelectedItem();//gvPurchaseRequestEditFormDetail.cpEditingRowGramagefrom; //gvPurchaseRequestEditFormDetail.GetRowValues(gvPurchaseRequestEditFormDetail.cpEditingRowGramage, "id_grammageFrom");

        //if (grammaUpToTmp !== null) {
        //if (!(grammageFromTmp === undefined || grammageFromTmp === "")) {
        if (grammageFromTmp !== null && grammageFromTmp !== "") {
            //  if (parseFloat(s.GetItem(grammaUpToTmp).texts[2]) < parseFloat(id_grammageFrom.GetItem(grammageFromTmp).texts[2])) {
            if (parseFloat(id_grammageTo.GetSelectedItem().texts[2]) < parseFloat(id_grammageFrom.GetSelectedItem().texts[2])) {
                e.isValid = false;
                e.errorText = "Gramaje Hasta debe ser mayor o igual que gramaje Desde";
                if (errorMessage === null || errorMessage === "") {
                    errorMessage = "- Gramaje Hasta: Debe ser mayor o igual que gramaje Desde.";
                } else {
                    errorMessage += "</br>- Gramaje Hasta: Debe ser mayor o igual que gramaje Desde.";
                }
            }
        }
        //}
    }

}
function OnGrammageFromToValidation_selectedIndexChanged(s, e) {
    if (!runningValidation) {
        ValidateDetail();
    }
    //// 
    var grammageFromTmp = s.GetValue();
    if (grammageFromTmp === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage === null || errorMessage === "") {
            errorMessage = "- Gramaje Desde: Es obligatorio.";
        } else {
            errorMessage += "</br>- Gramaje Desde: Es obligatorio.";
        }
    } else {
        //e.isValid = true;

        //var controls = ASPxClientControl.GetControlCollection();
        //var objectControl = controls.GetByName("grammageUpto");

        var grammaUpToTmp = id_grammageTo.GetValue();
        //var grammaUpToTmp = gvPurchaseRequestEditFormDetail.cpEditingRowGramage; //gvPurchaseRequestEditFormDetail.GetRowValues(gvPurchaseRequestEditFormDetail.cpEditingRowGramage, "id_grammageTo");

        // if (grammageFromTmp !== null) {
        //if (!(grammaUpToTmp === undefined || grammaUpToTmp === "")) {
        if (grammaUpToTmp !== null && grammaUpToTmp !== "") {
            //        if (parseFloat(s.GetItem(grammageFromTmp).texts[2]) > parseFloat(id_grammageTo.GetItem(grammaUpToTmp).texts[2])) {
            if (parseFloat(id_grammageFrom.GetSelectedItem().texts[2]) > parseFloat(id_grammageTo.GetSelectedItem().texts[2])) {
                e.isValid = false;
                e.errorText = "Gramaje Desde debe ser menor o igual que gramaje Hasta";
                if (errorMessage === null || errorMessage === "") {
                    errorMessage = "- Gramaje Desde: Debe ser menor o igual que gramaje Hasta.";
                } else {
                    errorMessage += "</br>- Gramaje Desde: Debe ser menor o igual que gramaje Hasta.";
                }
            }
        }
        // }
    }

}