// DIALOG BUTTONS ACTIONS

function Update(approve) {

    var valid = true;
    var validDocumentCut = ASPxClientEdit.ValidateEditorsInContainerById("documentCut", null, true);
    var validMainTabPurchaseOrder = ASPxClientEdit.ValidateEditorsInContainerById("mainTabPurchaseOrder", null, true);
    $('#sampleMessageLabel').hide();

    //Obtengo parametro que me indica si hay detalle orden de compra
    //por Grammaje
    var gsOCDetail = $('#parOCDetail').val();

    var INPnumberTxt = INPnumber.GetText();
    var ministerialAgreementTxt = ministerialAgreement.GetText();
    var tramitNumberTxt = tramitNumber.GetText();

    if ((INPnumberTxt == "" || INPnumberTxt == null)) {
        var msgErrorAux = ErrorMessage("El campo INP no puede estar vacío, configure esta información en el StakeHolder");
        sampleMessageLabel.SetText(msgErrorAux);
        $("#sampleMessageLabel").show();
        return;
    }
    if ((ministerialAgreementTxt == "" || ministerialAgreementTxt == null) && (tramitNumberTxt == "" || tramitNumberTxt == null)) {
        var msgErrorAux = ErrorMessage("Los campos Acuerdo Ministerial y Número de Trámite no pueden estar vacíos al mismo tiempo, configure uno de ellos en la Información del StakeHolder");
        sampleMessageLabel.SetText(msgErrorAux);
        $("#sampleMessageLabel").show();
        return;
    }

    if (validDocumentCut) {
        UpdateTabImage({ isValid: true }, "tabDocument");
    } else {
        UpdateTabImage({ isValid: false }, "tabDocument");
        valid = false;
    }

    if (validMainTabPurchaseOrder) {
        UpdateTabImage({ isValid: true }, "tabPurchaseOrder");
    } else {
        UpdateTabImage({ isValid: false }, "tabPurchaseOrder");
        valid = false;
    }

    if (gsOCDetail == "1") {
        if (gvPurchaseOrderEditFormDetailsBG.cpRowsCount === 0 || gvPurchaseOrderEditFormDetailsBG.IsEditing()) {
            UpdateTabImage({ isValid: false }, "tabDetails");
            valid = false;
        } else {
            UpdateTabImage({ isValid: true }, "tabDetails");
        }
    } else {
        if (gvPurchaseOrderEditFormDetails.cpRowsCount === 0 || gvPurchaseOrderEditFormDetails.IsEditing()) {
            UpdateTabImage({ isValid: false }, "tabDetails");
            valid = false;
        } else {
            UpdateTabImage({ isValid: true }, "tabDetails");
        }
    }

    var delivery = deliveryhour.GetText()

    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        var id = $("#id_order").val();
        var data = "id=" + id + "&delivery=" + delivery + "&approve=" + approve + "&" + $("#formEditPurchaseOrder").serialize();
        var url = (id === "0") ? "PurchaseOrder/PurchaseOrdersPartialAddNew"
            : "PurchaseOrder/PurchaseOrdersPartialUpdate";

        showForm(url, data);
    }
}

function ButtonUpdate_Click(s, e) {

    Update(false);
}

function ButtonUpdateClose_Click(s, e) {

    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    if (valid) {
        var id = $("#id_order").val();

        var data = "id=" + id + "&" + $("#formEditPurchaseOrder").serialize();

        var url = (id === "0") ? "PurchaseOrder/PurchaseOrderPartialAddNew"
            : "PurchaseOrder/PurchaseOrderPartialUpdate";

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
                    console.log(result);
                },
                complete: function () {
                    hideLoading();
                    showPage("PurchaseOrder/Index", null);
                }
            });
        }
    }
}

function ButtonCancel_Click(s, e) {
    var aId = $("#id_order").val();
    $.ajax({
        url: 'PurchaseOrder/UnlockedDocument',
        data: {
            id_document: aId,
            nameDocument: "Orden de Compra",
            code_sourceLockedDocument: "OC"
        },
        async: true,
        cache: false,
        type: 'POST',
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error. " + result.Message);
                return;
            }

            $('#sampleMessageLabel').hide();
            showPage("PurchaseOrder/Index", null);

        },
        complete: function () {
            //hideLoading();
        }
    });

}

//PURCHASE ORDER BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0
    };

    showPage("PurchaseOrder/FormEditPurchaseOrder", data);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
    showPage("PurchaseOrder/PurchaseOrderCopy", { id: $("#id_order").val() });
}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        //Update(true);
        var data = {
            id: $("#id_order").val()
        };
        showForm("PurchaseOrder/Approve", data);
    }, "¿Desea aprobar la orden de compra?");
}

function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_order").val()
        };
        showForm("PurchaseOrder/Autorize", data);
    }, "¿Desea autorizar la orden de compra?");
}

function ProtectDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_order").val()
        };
        showForm("PurchaseOrder/Protect", data);
    }, "¿Desea cerrar la orden de compra?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_order").val()
        };
        showForm("PurchaseOrder/Cancel", data);
    }, "¿Desea anular la orden de compra?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_order").val()
        };
        showForm("PurchaseOrder/Revert", data);
    }, "¿Desea reversar la orden de compra?");
}

function ShowDocumentHistory(s, e) {

}

function PrintDocument(s, e) {

    var data = { id_im: $("#id_order").val(), codeReport: "ROCP" };

    $.ajax({
        url: 'PurchaseOrder/PrintReportPurchaseOrderCrystal',
        data: data,
        async: true,
        cache: false,
        type: 'POST',
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            try {
                if (result !== undefined) {
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

function ShowDetailImportation(s, e) {
    if (s.GetChecked()) {
        $("#detailImportation").show();
    } else {
        $("#detailImportation").hide();
    }
}

// PURCHASE ORDERS DETAILS

function AddNewDetail(s, e) {
    gvPurchaseOrderEditFormDetails.AddNewRow();
}

function RemoveDetail(s, e) {

    gvPurchaseOrderEditFormDetails.GetSelectedFieldValues("id_item", function (values) {
        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "PurchaseOrder/PurchaseOrderDetailsDeleteSeleted",
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
            success: function () {
                // TODO: 
            },
            complete: function () {
                gvPurchaseOrderEditFormDetails.PerformCallback();
                UpdateOrderTotals();
            }
        });
    });

}

function RefreshDetail(s, e) {
    gvPurchaseOrderEditFormDetails.UnselectRows();
    gvPurchaseOrderEditFormDetails.PerformCallback();
}

//GET PRICE LIST DETAILS
function OnPriceListDetailClick(s, e) {
    var idOc = $("#id_order").val();
    var id_Tmp = id_priceList.GetValue();
    var idProvider_Tmp = id_provider.GetValue();
    if (id_Tmp > 0) {
        showThickBox("PriceListDetail/PopupControlPriceListDetail", { id_priceList: id_Tmp, idProvider: idProvider_Tmp, idOc: idOc });
    }
}


//PURCHASE ORDERS DETAILS BY GRAMMAGE

function AddNewDetailBG(s, e) {
    gvPurchaseOrderEditFormDetailsBG.AddNewRow();
}

function RemoveDetailBG(s, e) {

    gvPurchaseOrderEditFormDetailsBG.GetSelectedFieldValues("id_item", function (values) {
        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        $.ajax({
            url: "PurchaseOrder/PurchaseOrderDetailsDeleteSeletedBG",
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
            success: function () {
                // TODO: 
            },
            complete: function () {
                gvPurchaseOrderEditFormDetailsBG.PerformCallback();
                UpdateOrderTotals();
            }
        });
    });

}

function RefreshDetailBG(s, e) {
    gvPurchaseOrderEditFormDetailsBG.UnselectRows();
    gvPurchaseOrderEditFormDetailsBG.PerformCallback();
}

//COMBOBOX PURCHASE ORDER DETAILS ACTIONS

var id_itemIniAux = 0;
var id_purchaseRequestDetailIniAux = 0;
var id_grammageIniAux = 0;
var errorMessage = "";
var runningValidation = false;



function OnGrammageValidation(s, e) {
    //gridMessageErrorPurchaseOrder.SetText(result.Message);
    //errorMessage = "";
    //$("#GridMessageErrorPurchaseOrder").hide();
    if (!runningValidation) {
        ValidateDetailBG();
    }

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Gramage: Es obligatorio.";
    } else {
        //var code_grammageFrom = "";
        //var grammageFrom = 0.00;
        //var code_grammageTo = "";
        //var grammageTo = 0.00;
        //var grammageCurrent = 0.00;
        grammageCurrent = 0.00;
        if (id_Grammage.GetValue() !== null) {
            grammageCurrent = parseFloat(id_Grammage.GetSelectedItem().texts[2]);
            if (id_purchaseRequestDetail.GetValue() !== null && (grammageFrom > grammageCurrent || grammageTo < grammageCurrent)) {
                e.isValid = false;
                e.errorText = "Gramaje fuera del rango de gramajes del requerimiento de compra";
                errorMessage = "- Gramaje: Esta fuera del rango de gramajes del requerimiento de compra(" + code_grammageFrom + " - " + code_grammageTo + ").";
            }
        }

    }
}

function OnCertification_EndCallback(s, e) {
    if (id_priceList.GetValue() !== null) {
        $.ajax({
            url: "PurchaseOrder/UpdateCertification",
            type: "post",
            data: null,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result !== null) {
                    id_certification.SetValue(result.id_certificationUpdate);
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function OnItemValidation(s, e) {
    //gridMessageErrorPurchaseOrder.SetText(result.Message);
    //Incluir  como parametro el id de la Lista de Precio  y el checd
    errorMessage = "";
    $("#GridMessageErrorPurchaseOrder").hide();
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Nombre del Producto: Es obligatorio.";
    } else {
        var data = {
            //id: gvPurchaseRequestEditFormDetail.cpEditingRowKey,
            id_itemNew: s.GetValue(),
            id_purchaseRequestDetail: id_purchaseRequestDetailIniAux,
            id_priceList: id_priceList.GetValue(),
            pricePerList: pricePerList.GetValue()
        };
        if (data.id_itemNew != id_itemIniAux) {
            $.ajax({
                url: "PurchaseOrder/ItsRepeatedItemDetail",
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
                            if (result.ErrorGeneral !== null && result.ErrorGeneral !== "") {
                                e.isValid = false;
                                e.errorText = result.ErrorGeneral;
                                errorMessage = "- Error del Producto: " + result.ErrorGeneral;
                            }
                            else {

                                id_itemIniAux = 0
                                id_purchaseRequestDetailIniAux = 0
                            }

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

//BY GRAMMAJE
function OnItemValidationBG(s, e) {

    if (!runningValidation) {
        ValidateDetailBG();
    }

    errorMessage = "";
    $("#GridMessageErrorPurchaseOrder").hide();

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Nombre del Producto: Es obligatorio.";
    } else {
        var data = {
            //id: gvPurchaseRequestEditFormDetail.cpEditingRowKey,
            id_itemNew: s.GetValue(),
            id_grammage: id_Grammage.GetValue(),
            id_purchaseRequestDetail: id_purchaseRequestDetail.GetValue(),
            id_priceList: id_priceList.GetValue(),
            pricePerList: pricePerList.GetValue()
        };
        if (data.id_itemNew !== id_itemIniAux || data.id_grammage !== id_grammageIniAux || data.id_purchaseRequestDetail !== id_purchaseRequestDetailIniAux) {
            $.ajax({
                url: "PurchaseOrder/ItsRepeatedItemDetailBG",
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
                            if (result.ErrorGeneral !== null && result.ErrorGeneral !== "") {
                                e.isValid = false;
                                e.errorText = result.ErrorGeneral;
                                errorMessage = "- Error del Producto: " + result.ErrorGeneral;
                            }
                            //else {
                            //    id_itemIniAux = 0
                            //    id_purchaseRequestDetailIniAux = 0
                            //}
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

function ItemCombo_OnInit(s, e) {

    id_itemIniAux = s.GetValue();
    id_purchaseRequestDetailIniAux = gvPurchaseOrderEditFormDetails.cpEditingRowPurchaseRequestDetail;

    var data = {
        id_purchaseReason: id_purchaseReason.GetValue(),
        id_itemCurrent: s.GetValue()
    };

    if (data.id_itemCurrent != null && id_purchaseRequestDetailIniAux != null && id_purchaseRequestDetailIniAux != 0) s.SetEnabled(false);

    $.ajax({
        url: "PurchaseOrder/PurchaseOrderDetails",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_metricUnit.SetValue(null);
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
            ////id_salesOrder
            //var salesOrderAux = s.FindItemByValue(result.salesOrder.id);
            //if (salesOrderAux == null && result.salesOrder.id != null) s.AddItem(result.salesOrder.name, result.salesOrder.id);
            //s.SetValue(result.salesOrder.id);

            ////id_item
            //UpdateProductionLotLiquidationDetailItems(result.items);

            ////id_metricUnit
            //UpdateProductionLotLiquidationDetailMetricUnits(result.metricUnits);

            //if (result !== null && result !== undefined) {
            //    metricUnit.SetText(result.metricUnit);
            //}
            //else {
            //    metricUnit.SetText("");
            //}
        },
        complete: function () {
            //hideLoading();
        }
    });

    //Busqueda por los parametros del producto que se definan
    //store actual filtering method and override
    //var actualFilteringOnClient = s.filterStrategy.FilteringOnClient;
    //s.filterStrategy.FilteringOnClient = function () {
    //    //create a new format string for all list box columns. you could skip this bit and just set
    //    //filterTextFormatString to whatever you wanted for instance "{0} {2}" would only filter on
    //    //columns 1 and 3
    //    var lb = this.GetListBoxControl();
    //    var filterTextFormatStringItems = [];
    //    for (var i = 0; i < lb.columnFieldNames.length; i++) {
    //        filterTextFormatStringItems.push('{' + i + '}');
    //    }
    //    var filterTextFormatString = filterTextFormatStringItems.join(' ');

    //    //store actual format string and override with one for all columns
    //    var actualTextFormatString = lb.textFormatString;
    //    lb.textFormatString = filterTextFormatString;

    //    //call actual filtering method which will now work on our temporary format string
    //    actualFilteringOnClient.apply(this);

    //    //restore original format string
    //    lb.textFormatString = actualTextFormatString;
    //};
    //ItemCombo_DropDown(s, e);
}

//BY GRAMMAGE
function ItemCombo_OnInitBG(s, e) {

    id_itemIniAux = s.GetValue();
    id_grammageIniAux = id_Grammage.GetValue();
    id_purchaseRequestDetailIniAux = id_purchaseRequestDetail.GetValue();
    //id_purchaseRequestDetailIniAux = gvPurchaseOrderEditFormDetailsBG.cpEditingRowPurchaseRequestDetail;

    var data = {
        id_purchaseReason: id_purchaseReason.GetValue(),
        id_itemCurrent: s.GetValue()
    };

    if (data.id_itemCurrent != null && id_purchaseRequestDetailIniAux != null && id_purchaseRequestDetailIniAux != 0) s.SetEnabled(false);

    $.ajax({
        url: "PurchaseOrder/PurchaseOrderDetailsBG",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_metricUnit.SetValue(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //id_company
            //var arrayFieldStr = [];
            //arrayFieldStr.push("masterCode");
            //arrayFieldStr.push("name");
            //arrayFieldStr.push("itemPurchaseInformationMetricUnitCode");
            //UpdateDetailObjects(id_item, result.items, arrayFieldStr);

            id_item.ClearItems();
            var itemsCount = result.items.length;

            for (var i = 0; i < itemsCount; i++) {
                var item = result.items[i];
                var itemColumnTexts = [
                    item.masterCode,
                    item.name,
                    item.itemPurchaseInformationMetricUnitCode
                ];
                id_item.AddItem(itemColumnTexts, item.id);
            }
            id_item.SetValue(id_itemIniAux);
        },
        complete: function () {
            //hideLoading();
        }
    });

}

function ItemCombo_DropDown(s, e) {

    var data = {
        id_purchaseReason: id_purchaseReason.GetValue(),
        id_itemCurrent: s.GetValue()
    };

    if (data.id_itemCurrent != null) s.SetEnabled(false);

    $.ajax({
        url: "PurchaseOrder/PurchaseOrderDetails",
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
            for (var i = 0; i < id_item.GetItemCount(); i++) {
                var item = id_item.GetItem(i);
                if (result.purchaseItemsByPurchaseReason.indexOf(item.value) >= 0 ||
                    result.purchaseOrderDetails.indexOf(item.value) >= 0) {
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

function ItemCombo_SelectedIndexChanged(s, e) {

    UpdatePrice();



    var priceAux = price.GetValue();
    var QuantityApprovedAux = quantityApproved.GetValue();
    var strQuantityApproved = priceAux == null ? "0" : QuantityApprovedAux.toString();
    var strPrice = priceAux == null ? "0" : priceAux.toString();

    var resQuantityApproved = strQuantityApproved.replace(".", ",");
    var resPrice = strPrice.replace(".", ",");

    UpdateItemInfo({
        id_item: s.GetValue(),
        quantityApproved: quantityApproved.GetValue(),
        price: resPrice
    });

}

//BY GRAMMAGE
function ItemCombo_SelectedIndexChangedBG(s, e) {

    UpdatePrice();

    var priceAux = price.GetValue();
    var QuantityApprovedAux = quantityApproved.GetValue();
    var strQuantityApproved = priceAux == null ? "0" : QuantityApprovedAux.toString();
    var strPrice = priceAux == null ? "0" : priceAux.toString();

    var resQuantityApproved = strQuantityApproved.replace(".", ",");
    var resPrice = strPrice.replace(".", ",");

    UpdateItemInfoBG({
        id_item: s.GetValue(),
        quantityApproved: quantityApproved.GetValue(),
        price: resPrice
    });

}

function ValidateDetail() {

    OnItemValidation(id_item, id_item);
    QuantityOrderedValidation(quantityOrdered, quantityOrdered);
    QuantityApprovedValidation(quantityApproved, quantityApproved);
    PriceValidation(price, price);
}
function ValidateDetailBG() {
    runningValidation = true;
    OnItemValidationBG(id_item, id_item);
    OnGrammageValidation(id_Grammage, id_Grammage);
    QuantityOrderedValidation(quantityOrdered, quantityOrdered);
    QuantityApprovedValidation(quantityApproved, quantityApproved);
    PriceValidation(price, price);
    runningValidation = false;
}

function QuantityOrderedValidation(s, e) {
    if (!runningValidation) {
        ValidateDetailBG();
    }

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Ordenada: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Ordenada: Es obligatoria.";
        }
    } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Ordenada: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Cantidad Ordenada: Máximo 20 caracteres.";
        }
    } else if (s.GetValue() <= 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Ordenada: Debe ser mayor a cero.";
        } else {
            errorMessage += "</br>- Cantidad Ordenada: Debe ser mayor a cero.";
        }
    }
}

function QuantityApprovedValidation(s, e) {
    if (!runningValidation) {
        ValidateDetailBG();
    }

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Aprobada: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Aprobada: Es obligatoria.";
        }
    } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Aprobada: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Cantidad Aprobada: Máximo 20 caracteres.";
        }
    } else if (s.GetValue() < 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Aprobada: Debe ser mayor e igual a cero.";
        } else {
            errorMessage += "</br>- Cantidad Aprobada: Debe ser mayor e igual a cero.";
        }
    }
}

function QuantityApproved_Init() {

    $.ajax({
        url: "PurchaseOrder/UpdatePurchaseOrder2",
        type: "post",
        data: null,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                console.log(result);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

//BY GRAMMAGE
function QuantityApproved_InitBG() {

    $.ajax({
        url: "PurchaseOrder/UpdatePurchaseOrder2BG",
        type: "post",
        data: null,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                console.log(result);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function QuantityOrdered_ValueChanged(s, e) {

    var data = s.GetValue();
    if (data == null || data < 0) data = 0;
    quantityApproved.SetValue(data);

    var quantityApprovedAux = data;
    var strQuantityApproved = quantityApprovedAux == null ? "0" : quantityApprovedAux.toString();

    var priceAux = price.GetValue();
    var strPrice = priceAux == null ? "0" : priceAux.toString();

    var resQuantityApproved = strQuantityApproved.replace(".", ",");
    var resPrice = strPrice.replace(".", ",");
    UpdateItemInfo({
        id_item: id_item.GetValue(),
        quantityApproved: resQuantityApproved,
        price: resPrice
    });

    ValidateDetail();
}

//BY GRAMMAGE
var code_grammageFrom = "";
var grammageFrom = 0.00;
var code_grammageTo = "";
var grammageTo = 0.00;
var grammageCurrent = 0.00;

function PurchaseRequestDetailsCombo_OnInit(s, e) {
    DetailsPurchaseRequestDetailsCombo_SelectedIndexChanged(s, e);
}

function DetailsPurchaseRequestDetailsCombo_SelectedIndexChanged(s, e) {
    if (s.GetValue() === null) {
        id_item.SetEnabled(true);
        quantityRequested.SetValue(0.00);
    } else {
        id_item.SetEnabled(false);
        $.ajax({
            url: "PurchaseOrder/GetDataPurchaseRequestDetail",
            type: "post",
            data: { id_purchaseRequestDetail: s.GetValue() },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {

                if (result !== null) {
                    if (result.mensaje === "OK") {
                        id_item.SetValue(result.id_item);
                        grammageFrom = result.grammageFrom;
                        grammageTo = result.grammageTo;
                        code_grammageFrom = result.code_grammageFrom;
                        code_grammageTo = result.code_grammageTo;
                        quantityRequested.SetValue(result.quantityRequested);

                        ItemCombo_SelectedIndexChangedBG(id_item, id_item);
                    } else {
                        grammageFrom = 0.00;
                        grammageTo = 0.00;
                        code_grammageFrom = "";
                        code_grammageTo = "";
                        quantityRequested.SetValue(0.00);
                    }

                    //console.log(result);

                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function QuantityOrdered_ValueChangedBG(s, e) {

    var data = s.GetValue();
    if (data == null || data < 0) data = 0;
    quantityApproved.SetValue(data);

    var quantityApprovedAux = data;
    var strQuantityApproved = quantityApprovedAux == null ? "0" : quantityApprovedAux.toString();

    var priceAux = price.GetValue();
    var strPrice = priceAux == null ? "0" : priceAux.toString();

    var resQuantityApproved = strQuantityApproved.replace(".", ",");
    var resPrice = strPrice.replace(".", ",");
    UpdateItemInfoBG({
        id_item: id_item.GetValue(),
        quantityApproved: resQuantityApproved,
        price: resPrice
    });

    //ValidateDetailBG();
}

function QuantityApproved_ValueChanged(s, e) {

    UpdatePrice();
    var quantityApprovedAux = s.GetValue();
    var strQuantityApproved = quantityApprovedAux == null ? "0" : quantityApprovedAux.toString();

    var priceAux = price.GetValue();
    var strPrice = priceAux == null ? "0" : priceAux.toString();

    var resQuantityApproved = strQuantityApproved.replace(".", ",");
    var resPrice = strPrice.replace(".", ",");


    UpdateItemInfo({
        id_item: id_item.GetValue(),
        quantityApproved: resQuantityApproved,
        price: resPrice
    });


}

function QuantityApproved_ValueChangedBG(s, e) {

    UpdatePrice();
    var quantityApprovedAux = s.GetValue();
    var strQuantityApproved = quantityApprovedAux == null ? "0" : quantityApprovedAux.toString();

    var priceAux = price.GetValue();
    var strPrice = priceAux == null ? "0" : priceAux.toString();

    var resQuantityApproved = strQuantityApproved.replace(".", ",");
    var resPrice = strPrice.replace(".", ",");


    UpdateItemInfoBG({
        id_item: id_item.GetValue(),
        quantityApproved: resQuantityApproved,
        price: resPrice
    });


}

function Grammage_SelectedIndexChanged(s, e) {

    UpdatePrice();



    var priceAux = price.GetValue();
    var QuantityApprovedAux = quantityApproved.GetValue();
    var strQuantityApproved = priceAux == null ? "0" : QuantityApprovedAux.toString();
    var strPrice = priceAux == null ? "0" : priceAux.toString();

    var resQuantityApproved = strQuantityApproved.replace(".", ",");
    var resPrice = strPrice.replace(".", ",");


    UpdateItemInfo({
        id_item: id_item.GetValue(),
        quantityApproved: resQuantityApproved,
        //  quantityApproved: quantityApproved.GetValue(),
        price: resPrice
    });

}

function Grammage_SelectedIndexChangedBG(s, e) {
    UpdatePrice();

    var priceAux = price.GetValue();
    var QuantityApprovedAux = quantityApproved.GetValue();
    var strQuantityApproved = priceAux == null ? "0" : QuantityApprovedAux.toString();
    var strPrice = priceAux == null ? "0" : priceAux.toString();

    var resQuantityApproved = strQuantityApproved.replace(".", ",");
    var resPrice = strPrice.replace(".", ",");


    UpdateItemInfoBG({
        id_item: id_item.GetValue(),
        quantityApproved: resQuantityApproved,
        //  quantityApproved: quantityApproved.GetValue(),
        price: resPrice
    });

}

function UpdatePrice() {
    errorMessage = "";
    //var grammage = 0;


    var data = {
        id_grammage: id_Grammage.GetValue(),
        Libras: quantityApproved.GetValue(),
        id_processtype: null,
        id_ListPrice: id_priceList.GetValue(),
        id_Item: id_item.GetValue(),
        idProvider: id_provider.GetValue()
    };

    $.ajax({
        url: "PurchaseOrder/PriceCalculationGramage",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {

            if (result.mensaje != null && result.mensaje != "") {


                if (errorMessage == null || errorMessage == "") {

                    errorMessage = result.mensaje;
                } else {

                    errorMessage += "</br>" + result.mensaje;
                }
            }
            else {
                //grammageCurrent = result.grammageCurrent;
                if (result.price > 0) {

                    var resulaux = result.price / quantityApproved.GetValue();
                    var strresulaux = resulaux.toString();
                    var resresul = strresulaux.replace(",", ".");

                    price.SetValue(resresul);
                }
            }
        },
        complete: function () {
            hideLoading();
            if (errorMessage != null && errorMessage != "") {
                var msgErrorAux = ErrorMessage(errorMessage);
                gridMessageErrorPurchaseOrder.SetText(msgErrorAux);
                $("#GridMessageErrorPurchaseOrder").show();

            }
        }
    });

}

function PriceValidation(s, e) {
    if (!runningValidation) {
        ValidateDetailBG();
    }

    //var continua = true;
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Precio: Es obligatoria.";
        } else {
            errorMessage += "</br>- Precio: Es obligatoria.";
        }
    } else if (s.GetValue() !== null && s.GetValue().toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Precio: Máximo 20 caracteres.";
        } else {
            errorMessage += "</br>- Precio: Máximo 20 caracteres.";
        }
    } else if (s.GetValue() < 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Precio: Debe ser mayor e igual a cero.";
        } else {
            errorMessage += "</br>- Precio: Debe ser mayor e igual a cero.";
        }
    } else {


        //continua = false;


        var priceAux = price.GetValue();
        var strPrice = priceAux == null ? "0" : priceAux.toString();

        var resPrice = strPrice.replace(".", ",");
        //price.GetValue()

        var data = { id_priceList: id_priceList.GetValue(), id_item: id_item.GetValue() };
        $.ajax({
            url: "PurchaseOrder/PurchaseOrderValidatePriceDetails?price=" + resPrice,
            type: "post",
            data: data,
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {

                if (result.mensaje != null && result.mensaje != "") {
                    e.isValid = false;
                    e.errorText = "Valor Incorrecto";
                    if (errorMessage == null || errorMessage == "") {
                        //errorMessage = "- Precio: No Debe ser mayor al Ingremento Permitido.";
                        errorMessage = result.mensaje;
                    } else {
                        //errorMessage += "</br> - Precio: No Debe ser mayor al Ingremento Permitido.";
                        errorMessage += "</br>" + result.mensaje;
                    }
                }
            },
            complete: function () {
                hideLoading();
                if (errorMessage != null && errorMessage != "") {
                    var msgErrorAux = ErrorMessage(errorMessage);
                    gridMessageErrorPurchaseOrder.SetText(msgErrorAux);
                    $("#GridMessageErrorPurchaseOrder").show();

                }
            }
        });
    }


    if (errorMessage != null && errorMessage != ""/* && continua == true*/) {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorPurchaseOrder.SetText(msgErrorAux);
        $("#GridMessageErrorPurchaseOrder").show();

    }
}

function Price_ValueChanged(s, e) {

    var quantityApprovedAux = quantityApproved.GetValue();
    var strQuantityApproved = quantityApprovedAux == null ? "0" : quantityApprovedAux.toString();

    var priceAux = s.GetValue();
    var strPrice = priceAux == null ? "0" : priceAux.toString();

    var resQuantityApproved = strQuantityApproved.replace(".", ",");
    var resPrice = strPrice.replace(".", ",");
    UpdateItemInfo({
        id_item: id_item.GetValue(),
        quantityApproved: resQuantityApproved,
        price: resPrice
    });
}
//BY GRAMMAGE
function Price_ValueChangedBG(s, e) {

    var quantityApprovedAux = quantityApproved.GetValue();
    var strQuantityApproved = quantityApprovedAux == null ? "0" : quantityApprovedAux.toString();

    var priceAux = s.GetValue();
    var strPrice = priceAux == null ? "0" : priceAux.toString();

    var resQuantityApproved = strQuantityApproved.replace(".", ",");
    var resPrice = strPrice.replace(".", ",");
    UpdateItemInfoBG({
        id_item: id_item.GetValue(),
        quantityApproved: resQuantityApproved,
        price: resPrice
    });
}
function UpdateItemInfo(data) {

    if (data.id_item === null || data.quantityApproved === null || data.price === null) {
        if (data.quantityApproved === null) {
            quantityRequested.SetValue(0);
            quantityApproved.SetValue(1);
            data.quantityApproved = 1;
            quantityOrdered.SetValue(0);
            quantityReceived.SetValue(0);
            price.SetValue(0);
            data.price = 0;
        } //data.quantityOrdered = 1;
        //if (data.price === null) data.price = 0;
        if (data.id_item === null) {
            masterCode.SetText("");
            metricUnit.SetText("");
            ValidateDetail();
            return;
        }

    }

    masterCode.SetText("");
    metricUnit.SetText("");

    if (id_item != null) {
        $.ajax({
            url: "PurchaseOrder/ItemDetailData",
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
                    console.log(result);
                    masterCode.SetText(result.ItemDetailData.masterCode);
                    metricUnit.SetText(result.ItemDetailData.um);
                    price.SetValue(result.ItemDetailData.price);//.replace(",", ".")
                    iva.SetValue(result.ItemDetailData.iva);
                    subtotal.SetValue(result.ItemDetailData.subtotal);
                    total.SetValue(result.ItemDetailData.total);

                    // UPDATE ORDER TOTAL
                    orderSubtotal.SetValue(result.OrderTotal.subtotal);
                    orderSubtotalIVA12Percent.SetValue(result.OrderTotal.subtotalIVA12Percent);
                    orderTotalIVA12.SetValue(result.OrderTotal.totalIVA12);
                    //orderDiscount.SetValue(result.OrderTotal.discount);
                    //orderSubtotalIVA14Percent.SetValue(result.OrderTotal.subtotalIVA14Percent);
                    //orderTotalIVA14.SetValue(result.OrderTotal.totalIVA14);
                    orderTotal.SetValue(result.OrderTotal.total);
                }
            },
            complete: function () {
                ValidateDetail();
            }
        });
    } else {
        ValidateDetail();
    }
}
//BY GRAMMAGE
function UpdateItemInfoBG(data) {

    if (data.id_item === null || data.quantityApproved === null || data.price === null) {
        if (data.quantityApproved === null) {
            quantityRequested.SetValue(0);
            quantityApproved.SetValue(1);
            data.quantityApproved = 1;
            quantityOrdered.SetValue(0);
            quantityReceived.SetValue(0);
            price.SetValue(0);
            data.price = 0;
        }
        if (data.id_item === null) {
            masterCode.SetText("");
            metricUnit.SetText("");
            ValidateDetail();
            return;
        }

    }

    masterCode.SetText("");
    metricUnit.SetText("");

    if (id_item != null) {
        $.ajax({
            url: "PurchaseOrder/ItemDetailDataBG",
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
                    console.log(result);
                    masterCode.SetText(result.ItemDetailData.masterCode);
                    metricUnit.SetText(result.ItemDetailData.um);
                    price.SetValue(result.ItemDetailData.price);//.replace(",", ".")
                    iva.SetValue(result.ItemDetailData.iva);
                    subtotal.SetValue(result.ItemDetailData.subtotal);
                    total.SetValue(result.ItemDetailData.total);

                    // UPDATE ORDER TOTAL
                    orderSubtotal.SetValue(result.OrderTotal.subtotal);
                    orderSubtotalIVA12Percent.SetValue(result.OrderTotal.subtotalIVA12Percent);
                    orderTotalIVA12.SetValue(result.OrderTotal.totalIVA12);
                    orderTotal.SetValue(result.OrderTotal.total);
                }
            },
            complete: function () {
                //ValidateDetailBG();
            }
        });
    } else {
        //ValidateDetailBG();
    }
}

function UpdateOrderTotals() {

    $.ajax({
        url: "PurchaseOrder/OrderTotals",
        type: "post",
        data: null,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                orderSubtotal.SetValue(result.orderSubtotal);
                orderSubtotalIVA12Percent.SetValue(result.orderSubtotalIVA12Percent);
                orderTotalIVA12.SetValue(result.orderTotalIVA12);
                //orderDiscount.SetValue(result.orderDiscount);
                //orderSubtotalIVA14Percent.SetValue(result.orderSubtotalIVA14Percent);
                //orderTotalIVA14.SetValue(result.orderTotalIVA14);
                orderTotal.SetValue(result.orderTotal);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

//BY GRAMMAGE 
function UpdateOrderTotalsBG() {

    $.ajax({
        url: "PurchaseOrder/OrderTotalsBG",
        type: "post",
        data: null,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                orderSubtotal.SetValue(result.orderSubtotal);
                orderSubtotalIVA12Percent.SetValue(result.orderSubtotalIVA12Percent);
                orderTotalIVA12.SetValue(result.orderTotalIVA12);
                orderTotal.SetValue(result.orderTotal);
            }
        },
        complete: function () {
        }
    });
}

// PURCHASE ORDER DETAILS SELECTION

var customCommand = "";

function PurchaseOrderDetailsOnGridViewInit(s, e) {
    PurchaseOrderDetailsUpdateTitlePanel();
}

//BY GRAMMAGE
function PurchaseOrderDetailsOnGridViewInitBG(s, e) {
    PurchaseOrderDetailsUpdateTitlePanelBG();
}

function PurchaseOrderDetailsOnGridViewBeginCallback(s, e) {
    customCommand = e.command;
    PurchaseOrderDetailsUpdateTitlePanel();
}

//BY GRAMMAGE
function PurchaseOrderDetailsOnGridViewBeginCallbackBG(s, e) {
    customCommand = e.command;
    e.customArgs["id_purchaseRequestDetailValue"] = typeof id_purchaseRequestDetail === 'undefined' ? null : id_purchaseRequestDetail.GetValue();
    console.log(e.customArgs["id_purchaseRequestDetailValue"]);
    PurchaseOrderDetailsUpdateTitlePanelBG();

}


function PurchaseOrderDetailsOnGridViewEndCallback(s, e) {



    UpdateOrderTotals();
    PurchaseOrderDetailsUpdateTitlePanel();
    s.GetEditor("price").SetEnabled(pricePerList.GetChecked() != true);

}

//BY GRAMMAGE
function PurchaseOrderDetailsOnGridViewEndCallbackBG(s, e) {
    UpdateOrderTotalsBG();
    PurchaseOrderDetailsUpdateTitlePanelBG();
    if (s.GetEditor("price") != undefined) {
        s.GetEditor("price").SetEnabled(pricePerList.GetChecked() != true);
    }

}

function PurchaseOrderDetailsOnGridViewSelectionChanged(s, e) {
    PurchaseOrderDetailsUpdateTitlePanel();

}

//BY GRAMMAGE
function PurchaseOrderDetailsOnGridViewSelectionChangedBG(s, e) {
    PurchaseOrderDetailsUpdateTitlePanelBG();

}

function PurchaseOrderDetailsUpdateTitlePanel() {
    var selectedFilteredRowCount = PurchaseOrderDetailsGetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPurchaseOrderEditFormDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseOrderEditFormDetails.GetSelectedRowCount() - PurchaseOrderDetailsGetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPurchaseOrderEditFormDetails.GetSelectedRowCount() > 0 && gvPurchaseOrderEditFormDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseOrderEditFormDetails.GetSelectedRowCount() > 0);
    //}

    btnRemoveDetail.SetEnabled(gvPurchaseOrderEditFormDetails.GetSelectedRowCount() > 0);
}

function PurchaseOrderDetailsUpdateTitlePanelBG() {
    var selectedFilteredRowCount = PurchaseOrderDetailsGetSelectedFilteredRowCountBG();

    var text = "Total de elementos seleccionados: <b>" + gvPurchaseOrderEditFormDetailsBG.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPurchaseOrderEditFormDetailsBG.GetSelectedRowCount() - PurchaseOrderDetailsGetSelectedFilteredRowCountBG();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvPurchaseOrderEditFormDetailsBG.GetSelectedRowCount() > 0 && gvPurchaseOrderEditFormDetailsBG.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPurchaseOrderEditFormDetailsBG.GetSelectedRowCount() > 0);


    btnRemoveDetailBG.SetEnabled(gvPurchaseOrderEditFormDetailsBG.GetSelectedRowCount() > 0);
}

function PurchaseOrderDetailsGetSelectedFilteredRowCount() {
    return gvPurchaseOrderEditFormDetails.cpFilteredRowCountWithoutPage + gvPurchaseOrderEditFormDetails.GetSelectedKeysOnPage().length;
}
//BY GRAMMAGE
function PurchaseOrderDetailsGetSelectedFilteredRowCountBG() {
    return gvPurchaseOrderEditFormDetailsBG.cpFilteredRowCountWithoutPage + gvPurchaseOrderEditFormDetailsBG.GetSelectedKeysOnPage().length;
}

function PurchaseOrderDetailsSelectAllRows() {
    gvPurchaseOrderEditFormDetails.SelectRows();
}

//BY GRAMMAGE 
function PurchaseOrderDetailsSelectAllRowsBG() {
    gvPurchaseOrderEditFormDetailsBG.SelectRows();
}

function PurchaseOrderDetailsClearSelection() {
    gvPurchaseOrderEditFormDetails.UnselectRows();
}

//BY GRAMMAGE
function PurchaseOrderDetailsClearSelectionBG() {
    gvPurchaseOrderEditFormDetailsBG.UnselectRows();
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

//Set Active Tab
function SetActiveTab(s, e) {
    tabControl.SetActiveTab(tabControl.GetTab(1));
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

}

function UpdateView() {
    var id = parseInt($("#id_order").val());

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    btnCopy.SetEnabled(id !== 0);

    // STATES BUTTONS

    $.ajax({
        url: "PurchaseOrder/Actions",
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
        url: "PurchaseOrder/InitializePagination",
        type: "post",
        data: { id_purchaseOrder: $("#id_order").val() },
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

// MAIN FUNCTIONS

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
