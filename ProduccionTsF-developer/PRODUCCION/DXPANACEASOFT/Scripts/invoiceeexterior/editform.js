
/* Funciones requeridas por _DocumentFormEditActionsButtons */
function AddNewDocument(s, e) {
    AddNewInvoiceExteriorManual(s, e);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SavePartialDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
    showPage("InvoiceExterior/InvoiceCopy", { id: $("#id_invoice").val() });
}

function ApprovePartialDocument(s, e) {
    showConfirmationDialog(function () {
        Update(true, false);
    }, "¿Desea aprobar parcialmente la presente Factura?");
}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        Update(false, true);
    }, "¿Desea aprobar la presente Factura?");
}

function AutorizeDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_invoice").val()
        };
        showForm("InvoiceExterior/Autorize", data);
    }, "¿Desea autorizar la Factura?");

}

function CheckAutorizeRSIDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_invoice").val()
        };
        showForm("InvoiceExterior/CheckAutorizeRSI", data);
    }, "¿Desea Verificar la Autorización en el SRI de la Factura?");

}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_invoice").val()
        };
        showForm("InvoiceExterior/Cancel", data);
    }, "¿Desea anular la Factura?");
}

function DesvincularDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_invoice").val()
        };
        showForm("InvoiceExterior/DesvincularFactura", data);
    }, "¿Se anulará la factura por motivo de reasignación de proforma. Está seguro (Sí o No)?");
}

function RevertDocument(s, e) {


    showConfirmationDialog(function () {
        var data = {
            id: $("#id_invoice").val()
        };
        showForm("InvoiceExterior/Revert", data);
    }, "¿Desea reversar la Factura?");


}

function ExecuteRecalculatePrices(ForButton) {

    var data = {
        id: $("#id_invoice").val(),
        valueInternationalFreight: (valueInternationalFreight.GetValue() == null) ? 0 : valueInternationalFreight.GetValue().toString().replace(".", ","),
        valueInternationalInsurance: (valueInternationalInsurance.GetValue() == null) ? 0 : valueInternationalInsurance.GetValue().toString().replace(".", ","),
        valueCustomsExpenditures: (valueCustomsExpenditures.GetValue() == null) ? 0 : valueCustomsExpenditures.GetValue().toString().replace(".", ","),
        valueTransportationExpenses: (valueTransportationExpenses.GetValue() == null) ? 0 : valueTransportationExpenses.GetValue().toString().replace(".", ",")
    };
    $.ajax({
        url: 'InvoiceExterior/RecalculatePrices',
        data: data,
        async: true,
        cache: false,
        type: 'POST',
        beforeSend: function () {
            showLoading();
        },
        error: function (error) {
            console.log(error);
            hideLoading();
        },
        success: function (result) {

            try {
                if (result !== undefined && result !== null && result.Message === "OK") {
                    gvInvoiceDetail.PerformCallback();
                    //hideLoading();
                } else {
                    NotifyError(result.Message);
                }
            }
            catch (err) {
                hideLoading();
            }
        },
        complete: function () {
            if (ForButton) hideLoading();
        }
    });

}

function RecalculatePrices(s, e) {

    if (gvInvoiceDetail.cpRowsCount === 0 || gvInvoiceDetail.IsEditing()) {
        NotifyError("Debe agregar por lo menos un detalle para realizar el Recálculo");
        return;
    }

    showConfirmationDialog(function () {
        ExecuteRecalculatePrices(true);
        //var data = {
        //    id: $("#id_invoice").val(),
        //    valueInternationalFreight: (valueInternationalFreight.GetValue() == null) ? 0 : valueInternationalFreight.GetValue().toString().replace(".", ","),
        //    valueInternationalInsurance: (valueInternationalInsurance.GetValue() == null) ? 0 : valueInternationalInsurance.GetValue().toString().replace(".", ","),
        //    valueCustomsExpenditures: (valueCustomsExpenditures.GetValue() == null) ? 0 : valueCustomsExpenditures.GetValue().toString().replace(".", ","),
        //    valueTransportationExpenses: (valueTransportationExpenses.GetValue() == null) ? 0 : valueTransportationExpenses.GetValue().toString().replace(".", ",")
        //    //valueInternationalFreight: valueInternationalFreight.GetValue(),
        //    //valueInternationalInsurance: valueInternationalInsurance.GetValue(),
        //    //valueCustomsExpenditures: valueCustomsExpenditures.GetValue(),
        //    //valueTransportationExpenses: valueTransportationExpenses.GetValue()
        //};
        //$.ajax({
        //    url: 'InvoiceExterior/RecalculatePrices',
        //    data: data,
        //    async: true,
        //    cache: false,
        //    type: 'POST',
        //    beforeSend: function () {
        //        showLoading();
        //    },
        //    error: function (error) {
        //        console.log(error);
        //        hideLoading();
        //    },
        //    success: function (result) {

        //        try {
        //            
        //            if (result !== undefined && result !== null && result.Message === "OK") {
        //                gvInvoiceDetail.PerformCallback();
        //                //hideLoading();
        //            } else {
        //                NotifyError(result.Message);
        //            }
        //        }
        //        catch (err) {
        //            hideLoading();
        //        }
        //    },
        //    complete: function () {
        //        hideLoading();
        //    }
        //});
        //showForm("InvoiceExterior/RecalculatePrices", data);
    }, "¿Desea Recalcular Precios de la Factura?");

}

function ShowDocumentHistory(s, e) { }

/* Implementacion Funciones _DocumentFormEditActionsButtons*/
function ButtonUpdate_Click(s, e) {

    Update(false, false);
}

function ButtonUpdateClose_Click(s, e) {


    Update(false, close);

}

function ButtonCancel_Click(s, e) {
    showPage("InvoiceExterior/Index", null);
}

function OnInvoiceExteriorRemissionGuideCodeValidation(s, e) {
    if (e.value !== null && e.value.length !== 17) {
        e.isValid = false;
        e.errorText = "Longitud de 17 digitos requerida";
    }
}

/* Funciones Transaccion */
function Update(partialapprove, approve, close) {
    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);
    $.ajax({
        url: 'InvoiceExterior/ValidAndUpdateDetailFromProforma',
        data: { "id_metricUnitInvoice": id_metricUnitInvoice.GetValue() },
        async: true,
        cache: false,
        type: 'POST',
        beforeSend: function () {
            showLoading();
        },
        error: function (error) {
            console.log(error);
            hideLoading();
        },
        success: function (result) {
            try {
                if (result.Message !== "OK") {
                    NotifySuccess(result.Message);
                }
                var data = {
                    id: $("#id_invoice").val(),
                    valueInternationalFreight: (valueInternationalFreight.GetValue() == null) ? 0 : valueInternationalFreight.GetValue().toString().replace(".", ","),
                    valueInternationalInsurance: (valueInternationalInsurance.GetValue() == null) ? 0 : valueInternationalInsurance.GetValue().toString().replace(".", ","),
                    valueCustomsExpenditures: (valueCustomsExpenditures.GetValue() == null) ? 0 : valueCustomsExpenditures.GetValue().toString().replace(".", ","),
                    valueTransportationExpenses: (valueTransportationExpenses.GetValue() == null) ? 0 : valueTransportationExpenses.GetValue().toString().replace(".", ",")
                };
                $.ajax({
                    url: 'InvoiceExterior/RecalculatePrices',
                    data: data,
                    async: true,
                    cache: false,
                    type: 'POST',
                    beforeSend: function () {
                        showLoading();
                    },
                    error: function (error) {
                        console.log(error);
                        hideLoading();
                    },
                    success: function (result) {
                        try {
                            if (result !== undefined && result !== null && result.Message === "OK") {
                                $.ajax({
                                    url: 'InvoiceExterior/InvoiceExternalTotales',
                                    data: data,
                                    async: true,
                                    cache: false,
                                    type: 'POST',
                                    beforeSend: function () {
                                        showLoading();


                                    },
                                    error: function (error) {
                                        console.log(error);
                                        hideLoading();
                                    },
                                    success: function (result2) {
                                        try {
                                            if (result2 !== null) {
                                                $("#objTotalesPartial").empty();
                                                $("#objTotalesPartial").html(result2);
                                                InvoiceExteriorTermsNegotiation_SelectedIndexChanged(null, null);

                                                var data3 = {
                                                    idPaymentTerm: id_PaymentTerm.GetValue(),
                                                    emissionDate: emissionDate.GetValue().getFullYear().toString() + "-" +
                                                        (emissionDate.GetValue().getMonth() + 1).toString().padStart(2, "0") + "-" +
                                                        emissionDate.GetValue().getDate().toString().padStart(2, "0"),//emissionDate.GetValue(),
                                                    invoiceTotal: valuetotalCIF.GetValue(),
                                                    canEditPaymentTerm: gvInvoiceExteriorPaymentTermDetailView.cpCanEditPaymentTerm,
                                                    currentPaymentTermDetails: gvInvoiceExteriorPaymentTermDetailView.cpCurrentPaymentTermDetails
                                                };
                                                $.ajax({
                                                    url: 'InvoiceExterior/InvoiceExteriorPaymentTermViewDetailsPartial',
                                                    data: data3,
                                                    async: true,
                                                    cache: false,
                                                    type: 'POST',
                                                    beforeSend: function () {
                                                        showLoading();
                                                        //OnInvoiceTotalValueReady();
                                                        //gvInvoiceExteriorPaymentTermDetailView.PerformCallback();
                                                    },
                                                    error: function (error) {
                                                        console.log(error);
                                                        hideLoading();
                                                    },
                                                    success: function (result3) {
                                                        try {
                                                            if (result3 !== null) {
                                                                $("#div_InvoiceExteriorPaymentTerm").empty();
                                                                $("#div_InvoiceExteriorPaymentTerm").html(result3);
                                                                UpdateFinal(partialapprove, approve, close);
                                                            } else {
                                                                NotifyError("No se pudo actualizar Plazo de Pago");
                                                                hideLoading();
                                                            }
                                                        }
                                                        catch (err) {
                                                            hideLoading();
                                                        }
                                                    },
                                                    complete: function () {
                                                        //UpdateFinal(partialapprove, approve, close);
                                                    }
                                                });
                                                //gvInvoiceExteriorPaymentTermDetailView.PerformCallback();
                                            } else {
                                                NotifyError("No se pudo actualizar los Totales de la Factura Fiscal");
                                                hideLoading();
                                            }
                                        }
                                        catch (err) {
                                            hideLoading();
                                        }
                                    },
                                    complete: function () {
                                    }
                                });
                            } else {
                                NotifyError(result.Message);
                                hideLoading();
                            }
                        }
                        catch (err) {
                            hideLoading();
                        }
                    },
                    complete: function () {
                        //hideLoading();
                    }
                });

                //else {
                //    //ExecuteRecalculatePrices(false);
                //    $.ajax({
                //        url: 'InvoiceExterior/RecalculatePrices',
                //        data: data,
                //        async: true,
                //        cache: false,
                //        type: 'POST',
                //        beforeSend: function () {
                //            showLoading();
                //        },
                //        error: function (error) {
                //            console.log(error);
                //            hideLoading();
                //        },
                //        success: function (result) {
                //            try {
                //                
                //                if (result !== undefined && result !== null && result.Message === "OK") {
                //                    UpdateFinal(partialapprove, approve, close);
                //                } else {
                //                    NotifyError(result.Message);
                //                    hideLoading();
                //                }
                //            }
                //            catch (err) {
                //                hideLoading();
                //            }
                //        },
                //        complete: function () {
                //            //hideLoading();
                //        }
                //    });

                //}
            }
            catch (err) {
                hideLoading();
            }
        },
        complete: function () {
            //hideLoading();
        }
    });

}

function UpdateFinal(partialapprove, approve, close) {
    var valid = true;
    var validDocumentCut = ASPxClientEdit.ValidateEditorsInContainerById("documentCut", null, true);
    var validMainTabInvoiceExterio = ASPxClientEdit.ValidateEditorsInContainerById("mainTabInvoiceExterior", null, true);
    var validMainTabCobranzas = ASPxClientEdit.ValidateEditorsInContainerById("mainTabCobranzas", null, true);
    var validMainTabDocumentacion = ASPxClientEdit.ValidateEditorsInContainerById("mainTabDocumentacion", null, true);


    if (validDocumentCut) {
        UpdateTabImage({ isValid: true }, "tabDocument");
    } else {
        UpdateTabImage({ isValid: false }, "tabDocument");
        valid = false;
    }

    if (validMainTabInvoiceExterio) {
        UpdateTabImage({ isValid: true }, "tabInvoiceExterior");
    } else {
        UpdateTabImage({ isValid: false }, "tabInvoiceExterior");
        valid = false;
    }

    if (gvInvoiceDetail.cpRowsCount === 0 || gvInvoiceDetail.IsEditing()) {
        UpdateTabImage({ isValid: false }, "tabInvoiceExteriorDetails");
        valid = false;
    } else {
        UpdateTabImage({ isValid: true }, "tabInvoiceExteriorDetails");
    }

    if (validMainTabCobranzas) {
        UpdateTabImage({ isValid: true }, "tabCobranzas");
    } else {
        UpdateTabImage({ isValid: false }, "tabCobranzas");
        valid = false;
    }

    if (validMainTabDocumentacion) {
        UpdateTabImage({ isValid: true }, "tabDocumentacion");
    } else {
        UpdateTabImage({ isValid: false }, "tabDocumentacion");
        valid = false;
    }

    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    // partialApprove, bool approve
    console.log("id_consignee: " + $("#id_consignee").val());
    if (valid) {
        var id = $("#id_invoice").val();
        var data = "id=" + id + "&partialApprove=" + partialapprove + "&approve=" + approve +
            "&jsonPaymentTermDetails=" + encodeURIComponent(JSON.stringify(gvInvoiceExteriorPaymentTermDetailView.cpCurrentPaymentTermDetails)) +
            "&" + $("#formEditInvoiceExterior").serialize();
        var url = (id === "0")
            ? "InvoiceExterior/InvoiceExteriorPartialAddNew"
            : "InvoiceExterior/InvoiceExteriorPartialUpdate";
        if (close !== undefined) {
            if (close != null && close) {
                genericAjaxCall(url, true, data, null, showLoading(), null, function () {
                    hideLoading();
                    showPage("InvoiceExterior/Index", null);
                })
                return;
            }
        }

        var callBackAction = function () {
            btnNew.SetEnabled(true);
            setTimeout(function () { $("#successMessage").hide() }, 3000);
        }

        showForm(url, data, callBackAction);
    } else {
        hideLoading();
    }

}

/* Funciones Cabecera */
function OnChangeBuyer(s, e) {
    var route = "Person/getBuyerData";
    if (s.GetSelectedItem() == null) {
        fullname_businessName.SetText(null);
        identification_number.SetText(null);
        //address.SetText(null);
        id_addressCustomer.SetValue(null);
        email.SetText(null);
        emailInterno.SetText(null);
        return;
    }

    var id_personBuyer = s.GetSelectedItem().value;
    var data = "id_person=" + id_personBuyer;

    if (id_buyer.GetValue() == null) {
        id_buyer.SetValue(id_personBuyer);
    }
    if (id_notifier.GetValue() == null) {
        id_notifier.SetValue(id_personBuyer);
    }
    InvoiceExteriorAddressCustomerUpdate(id_personBuyer)

    genericAjaxCall(route, true, data, null, null, function (result) {

        if (result === undefined) return;
        if (result == null) return;
        try {
            fullname_businessName.SetText(result.fullname_businessName);
            identification_number.SetText(result.identification_number);
            //address.SetText(result.address);
            email.SetText(result.email);
            emailInterno.SetText(result.emailInterno);
        }
        catch (err) {
            console.log(err);
        }

    }, null);
}
function InvoiceExteriorAddressCustomerUpdate(id_personBuyerAux) {
    id_addressCustomer.ClearItems();
    id_addressCustomer.SetValue(null);


    $.ajax({
        url: "InvoiceExterior/SetAddressCustomer",
        type: "post",
        data: {
            id_consignee: id_personBuyerAux
        },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
            hideLoading();
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {

            if (result !== null && result.length > 0) {
                for (var i = 0; i < result.length; i++) {
                    id_addressCustomer.AddItem([result[i].tipoDireccion, result[i].name, result[i].emailInterno, result[i].emailInternoCC, result[i].phone1FC, result[i].fax1FC], result[i].id);
                }

                var value = result.length === 1 ? result[0].id : null;
                id_addressCustomer.SetValue(value);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}
var OnIdAddressCustomerSelectedIndexChanged = function (s, e) {
    var _selectItem = s.GetSelectedItem();
    var _emailInterno = "", _emailInternoCC = "", _phone1FC = "", _fax1FC = "";
    if (_selectItem != null) {

        _emailInterno = _selectItem.GetColumnText("emailInterno");
        _emailInternoCC = _selectItem.GetColumnText("emailInternoCC");
        _phone1FC = _selectItem.GetColumnText("phone1FC");
        _fax1FC = _selectItem.GetColumnText("fax1FC");
	}

    email.SetText(_emailInterno);
    emailInterno.SetText(_emailInternoCC);
    fax1FC.SetText(_fax1FC);
};
function OnChangedExpirationDate(s, e) {
    concessionDate.SetMinDate(s.GetDate());
    if (s.GetDate() > concessionDate.GetDate())
        concessionDate.SetDate(s.GetDate());
}

function OnInitComisionAgent(s, e) {

    if (s.GetValue()) {
        namecommissionAgent.SetVisible(true);
    } else {
        namecommissionAgent.SetVisible(false);
    }
}

function OnChangeComisionAgent(s, e) {

    if (s.GetValue()) {
        namecommissionAgent.SetVisible(true);
    } else {
        namecommissionAgent.SetVisible(false);
    }
}

function InvoiceExteriorPaymentMethod_SelectedIndexChanged(s, e) {

    id_PaymentTerm.ClearItems();
    id_PaymentTerm.SetValue(null);
    var route = 'InvoiceExterior/SetPaymentMethod'
    var data = { "id_invoice": $("#id_invoice").val(), "id_paymentMethod": ((s.GetSelectedItem() == null) ? null : s.GetSelectedItem().value) };
    genericAjaxCall(route, true, data, function (error) { console.log(error) }, null, function (result) {

        if (result != null && result.error) {
            console.log(result.msgerr);
            return;
        }
        id_PaymentTerm.PerformCallback();
    }, null)
}

function InvoiceExteriorShippingAgency_SelectedIndexChanged(s, e) {

    id_ShippingLine.ClearItems();
    id_ShippingLine.SetValue(null);
    var route = 'InvoiceExterior/SetShippingAgency'
    var data = { "id_invoice": $("#id_invoice").val(), "id_shippingAgency": ((s.GetSelectedItem() == null) ? null : s.GetSelectedItem().value) };
    genericAjaxCall(route, true, data, function (error) { console.log(error) }, null, function (result) {

        if (result != null && result.error) {
            console.log(result.msgerr);
            return;
        }
        id_ShippingLine.PerformCallback();
    }, null)
}

function OnshippingLine_EndCallback(s, e) {
    if (s.GetItemCount() == 0) return
    s.SetSelectedIndex(0);
}

function InvoiceExteriorTermsNegotiation_SelectedIndexChanged(s, e) {
    if (s == null) s = id_termsNegotiation;
    if (s.GetSelectedItem() == null) {
        valueTransportationExpenses.SetValue(0);
        valueCustomsExpenditures.SetValue(0);
        valueInternationalInsurance.SetValue(0);
        valueInternationalFreight.SetValue(0);

        $("input[name='valueInternationalFreight']").prop("disabled", true);
        $("input[name='valueTransportationExpenses']").prop("disabled", true);
        $("input[name='valueCustomsExpenditures']").prop("disabled", true);
        $("input[name='valueInternationalInsurance']").prop("disabled", true);

        return;
    }

    var _TermsNegotiationCode = s.GetSelectedItem().GetColumnText("code"); //.GetSelectedItem()


    switch (_TermsNegotiationCode) {
        case "FOB":
            valueInternationalInsurance.SetValue(0);

            $("input[name='valueInternationalInsurance']").prop("disabled", true);
            $("input[name='valueInternationalFreight']").prop("disabled", true);
            break;
        case "CIF":
            $("input[name='valueInternationalInsurance']").prop("disabled", false);
            $("input[name='valueInternationalFreight']").prop("disabled", false);
            break;
        case "CFR":
            valueInternationalInsurance.SetValue(0);
            $("input[name='valueInternationalInsurance']").prop("disabled", true);
            $("input[name='valueInternationalFreight']").prop("disabled", false);
            break;

    }

    $("input[name='valueTransportationExpenses']").prop("disabled", false);
    $("input[name='valueCustomsExpenditures']").prop("disabled", false);

}

function InvoiceExteriordateShipmentInit(s, e) {
    if (dateShipment.GetValue() == null) dateShipment.SetValue(new Date());
}

/*  Funciones Detalle */
function AddNewDetail(s, e) {

    gvInvoiceDetail.AddNewRow();
    btnNew.SetEnabled(false);
}

function RemoveDetail(s, e) {

    gvInvoiceDetail.GetSelectedFieldValues("id_item", function (values) {
        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }


        genericAjaxCall('InvoiceExterior/InvoiceExteriorDetailsDeleteSeleted', true, { ids: selectedRows }, function (error) { console.log(error) }, null, null, function () {

            gvInvoiceDetail.PerformCallback();
            reloadInvoiceTotal();
            validateExistMetricUnitInvoice(null, null);
        })


    });

}

function RefreshDetail(s, e) {
    gvInvoiceDetail.UnselectRows();
    gvInvoiceDetail.PerformCallback();
    reloadInvoiceTotal();

}

var id_itemInit = null;
var id_itemCurrent = null;
function ItemCombo_OnInit(s, e) {
    id_itemInit = s.GetValue();
    id_itemCurrent = s.GetValue();
    if (gvInvoiceDetail.cpDocumentOrigen !== null && gvInvoiceDetail.cpDocumentOrigen !== "") {
        if (s.GetValue() !== null) {
            s.SetEnabled(false);
        }
        id_item.PerformCallback();
    }
}

function ItemCombo_OnBeginCallback(s, e) {
    e.customArgs["id_itemCurrent"] = id_itemCurrent;//typeof id_itemCurrent !== 'undefined' ? id_itemCurrent : null

}

function ItemCombo_OnEndCallback(s, e) {
    if (id_itemInit !== null) {
        //id_item.InsertItem(0, ["id", "PT000614", "20/30 PROEXPO ENTERO 10X1.2 KG", "CCRBQA320PPRCJM05", "Kg", "20/30 PROEXPO ENTERO 10X1.2 KG"], [id_itemInit, "masterCode", "name", "auxCode", "code", "description2"])
        id_item.SetValue(id_itemInit);
    }
    genericDetailCalculate();
}

// Accion al cambiar seleccion de item
function ItemCombo_SelectedIndexChanged(s, e) {

    id_itemCurrent = s.GetValue();
    var _selectItem = s.GetSelectedItem();
    if (_selectItem == undefined || _selectItem == null) {
        masterCode.SetText(null);
        auxCode.SetText(null);
        description2.SetText(null);
        return;
    }


    var _masterCode = _selectItem.GetColumnText("masterCode");
    var _auxCode = _selectItem.GetColumnText("auxCode");
    var _description2 = _selectItem.GetColumnText("description2");

    masterCode.SetText(_masterCode);
    auxCode.SetText(_auxCode);
    description2.SetText(_description2);
    
    if (gvInvoiceDetail.cpDocumentOrigen !== null && gvInvoiceDetail.cpDocumentOrigen !== "") {
        var data = { id_item: s.GetValue() };

        $.ajax({
            url: 'InvoiceExterior/UpdateWithSalesQuotationExterior',
            data: data,
            async: true,
            cache: false,
            type: 'POST',
            beforeSend: function () {
                showLoading();
            },
            error: function (error) {
                console.log(error);
                hideLoading();
            },
            success: function (result) {

                try {
                    if (result !== undefined && result !== null && result.mensaje === "OK") {
                        numBoxes.SetValue(result.numBoxes);
                        totalPriceWithoutTax.SetValue(result.totalPriceWithoutTax);
                        unitPrice.SetValue(result.unitPrice);
                        unitPriceProforma.SetValue(result.unitPrice);
                        proformaWeight.SetValue(result.proformaWeight);
                        totalProforma.SetValue(result.totalProforma);
                        discount.SetValue(result.discount);
                        numBoxesValueChanged(null, null);
                        genericDetailCalculate();
                        //hideLoading();
                    }
                }
                catch (err) {
                    hideLoading();
                }
                finally
                {
                    numBoxes.Focus();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    } else {
        numBoxesValueChanged(null, null);
        genericDetailCalculate();
        numBoxes.Focus();
    }

}
function proformaWeightValueChanged(s, e) {
    amountProformaDisplay.SetValue(0);

    if (typeof numBoxes.GetValue() === "undefined" || numBoxes.GetValue() == null) return;
    if (typeof proformaWeight.GetValue() === "undefined" || proformaWeight.GetValue() == null) return;

    var NumeroCajas = numBoxes.GetValue();
    var PesoProforma = proformaWeight.GetValue();

    var _totalCantidadProforma = (NumeroCajas == 0 || PesoProforma == 0) ? 0 : (NumeroCajas * PesoProforma);

    amountProformaDisplay.SetValue(_totalCantidadProforma);
    unitPriceProformaValueChanged();

}

function numBoxesValueChanged(s, e) {

    if (id_item.GetValue() == undefined || id_item.GetValue() == null) return;
    if (id_metricUnitInvoice.GetValue() == undefined || id_metricUnitInvoice.GetValue() == null) return;

    // validacion 0
    var NumeroCajas = 0;
    if (s == undefined || s == null) {
        NumeroCajas = numBoxes.GetValue();
    }
    else {
        NumeroCajas = s.GetValue();
    }

    if (NumeroCajas == undefined || NumeroCajas == null) return;

    if (NumeroCajas == 0 || id_item.GetValue() == 0 || id_metricUnitInvoice.GetValue() == 0) {
        amountDisplay.SetText(0);
        amountInvoiceDisplay.SetText(0);
        //amountproforma.SetText(0);
        totalPriceWithoutTax.SetValue(0);

        $("#amount").val(0);
        $("#id_amountInvoice").val(0);
        discount.SetValue(0);
        return;

    }
    var data = { "id_item": id_item.GetValue(), "numCajas": NumeroCajas, "id_metricUnitInvoice": id_metricUnitInvoice.GetValue() };

    genericAjaxCall('InvoiceExterior/CalculaCantidad', true, data, function (error) { console.log(error); }, null,
        function (result) {
            if (result != null && result.error) {
                console.log(result.msgerr);
                return;
            }
            if (gvInvoiceDetail.cpDocumentOrigen !== null) {
                totalProforma.SetValue(result.totalProforma);
                amountProformaDisplay.SetValue(result.cantidadProformaDisplay);
            }
            
            amountDisplay.SetValue(result.cantidadDisplay);
            amountInvoiceDisplay.SetValue(result.cantidadInvoiceDisplay);
            $("#amount").val(result.cantidadItem);
            $("#id_amountInvoice").val(result.cantidadFactura);
            $("#amountProforma").val(result.cantidadProforma);
            unitPriceValueChanged(null, null);
        }, null);
    if (gvInvoiceDetail.cpDocumentOrigen === null) {
        proformaWeightValueChanged();
    }
}

function unitPriceValueChanged(s, e) {
    totalPriceWithoutTax.SetValue(0);

    if (typeof $("#id_amountInvoice").val() === "undefined" || $("#id_amountInvoice").val() == null) return;
    if (typeof $("#amountProforma").val() === "undefined" || $("#amountProforma").val() == null) return;
    if (typeof unitPrice.GetValue() === "undefined" || unitPrice.GetValue() == null) return;
    if (typeof discount.GetValue() === "undefined" || discount.GetValue() == null) return;


    //var cantidad = $("#id_amountInvoice").val();
    var cantidad = $("#amountProforma").val();
    if (cantidad == 0) cantidad = $("#id_amountInvoice").val();
    var precioUnitario = unitPrice.GetValue();
    if (cantidad == 0 && precioUnitario == 0) {
        discount.SetValue(0);
    }
    else {
        var descuento = discount.GetValue();
    }


    var total = (cantidad == 0 || precioUnitario == 0) ? 0 : ((cantidad * precioUnitario)); //- descuento);

    total = parseFloat(total);


    //    totalPriceWithoutTax.displayFormat("$ " + total);
    totalPriceWithoutTax.SetValue(total);

}
function unitPriceProformaValueChanged(s, e) {
    totalProforma.SetValue(0);

    if (typeof amountProformaDisplay.GetValue() === "undefined" || amountProformaDisplay.GetValue() == null) return;
    if (typeof unitPriceProforma.GetValue() === "undefined" || unitPriceProforma.GetValue() == null) return;

    var cantidadProforma = amountProformaDisplay.GetValue();
    var precioUnitarioProforma = unitPriceProforma.GetValue();

    var _totalProforma = (cantidadProforma == 0 || precioUnitarioProforma == 0) ? 0 : (cantidadProforma * precioUnitarioProforma);

    _totalProforma = parseFloat(_totalProforma);

    totalProforma.SetValue(_totalProforma);

}

function genericDetailCalculate() {
    numBoxesValueChanged(null, null);
    unitPriceValueChanged(null, null);
}

function discountValidation(s, e) {
    if (typeof $("#id_amountInvoice").val() === "undefined" || $("#id_amountInvoice").val() == null) return;
    if (typeof unitPrice.GetValue() === "undefined" || unitPrice.GetValue() == null) return;
    if (typeof discount.GetValue() === "undefined" || discount.GetValue() == null) return;

    var cantidad = $("#id_amountInvoice").val();
    var precioUnitario = unitPrice.GetValue();
    var _discount = discount.GetValue();


    if (_discount > (cantidad * precioUnitario)) {

        e.isValid = false;
        e.errorText = "Valor Descuento no puede ser mayor a Cantidad x Precio Unitario";
        e.value = 0;

    }
}

function BankTransfer_SelectedIndexChanged(s, e) {
    //// 
    var idBankTransfer = s.GetValue();
    var data = { id_BankTransfer: idBankTransfer };
    var route = 'InvoiceExterior/GetInfoBank';
    genericAjaxCall(route, true, data, function (error) { console.log(error) }, null, function (result) {
        //// 
        var oInfoBankTransfer = ASPxClientControl.GetControlCollection().GetByName("infoBankTransfer");
        if (result != null && result.codeReturn == -1) {
            console.log(result.msgerr);
            oInfoBankTransfer.GetValue("");
            $("[name=infoBankTransfer]").attr('rows', 1);
            return;
        }

        oInfoBankTransfer.SetValue(result.ValueDataList[0].valueObject);
        $("[name=infoBankTransfer]").attr('rows', 10);


    }, null)

}

// MAIN FUNCTIONS
function UpdateView() {

    var id = parseInt($("#id_invoice").val());
    // STATES BUTTONS
    genericAjaxCall('InvoiceExterior/Actions', true, { id: id }, function (error) { console.log(error) }, showLoading(),
        function (result) {
            if (result != null) {

                btnSave.SetEnabled(result.btnSave);
                btnApprovePartial.SetEnabled(result.btnApprovePartial);
                btnApprove.SetEnabled(result.btnApprove);
                btnAutorize.SetEnabled(result.btnAutorize);
                btnCheckAutorizeRSI.SetEnabled(result.btnCheckAutorizeRSI);
                btnCancel.SetEnabled(result.btnCancel);
                btnPrint.SetEnabled(result.btnPrint);
                btnPrintPartial.SetEnabled(result.btnPrintPartial);
                btnExportExcel.SetEnabled(result.btnExportExcel);
                btnDesvincular.SetEnabled(result.btnDesvincular);
                btnRecalculatePrices.SetEnabled(result.btnRecalculatePrices);
                // btnRevert.SetEnabled(result.btnRevert);
            }


        }, hideLoading())

}
// PRINT BUTTON EXCEL
function ExportExcel(s, e) {
    var data = { id_invoice: $("#id_invoice").val() };

    $.ajax({
        url: 'InvoiceExterior/InvoiceExteriorExporExcel',
        data: data,
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
                    var url = 'ReportProd/ToExcel?trepd=' + reportTdr;
                    newWindow = window.open(url, '_self', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0', false);

                    // newWindow.focus();
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

//print
function PrintInvoice(s, e) {

    var idsInvoice = [];
    idsInvoice.push($("#id_invoice").val());
    PrintInvoiceGeneric(idsInvoice);


}

//Print Propia
function PrintInvoicePropia(s, e) {

    var idsInvoice = [];
    idsInvoice.push($("#id_invoice").val());
    PrintInvoiceGenericPropia(idsInvoice);
}

//Print Certificada
function PrintInvoicePropiaCert(s, e) {

    var idsInvoice = [];
    idsInvoice.push($("#id_invoice").val());
    PrintInvoiceGenericCertificada(idsInvoice);
}
//----------------------------------------------------------------------------

//Print Pesos
function PrintInvoicePeso(s, e) {

    var idsInvoice = [];
    idsInvoice.push($("#id_invoice").val());
    PrintInvoiceHeight(idsInvoice);
}
//----------------------------------------------------------------------------
function PrintISF(s, e) {

 
    var data = {
        id: $("#id_invoice").val(),
        codeReport: "FEISF",
    };

    var url = "InvoiceExterior/PrintInvoiceExteriorISFTempReport";

    PrintItem(data, url);

}
function PrintBL(s, e) {


    var data = {
        id: $("#id_invoice").val(),
        codeReport: "FEBL",
    };

    var url = "InvoiceExterior/PrintInvoiceExteriorISFTempReport";

    PrintItem(data, url);

}
function PrintItem(data, url) {
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

function PrintTemper(s, e) {


    var data = {
        id: $("#id_invoice").val(),
        codeReport: "FETEM",
    };

    var url = "InvoiceExterior/PrintInvoiceExteriorISFTempReport";

    PrintItem(data, url);

}

function PrintItem(data, url) {
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


















//-----------------------------



//Print PakingList
function PrintInvoicePakingList(s, e) {

    var idsInvoice = [];
    idsInvoice.push($("#id_invoice").val());
    PrintInvoiceGenericPakingList(idsInvoice);
}

//Print NonWood
function PrintInvoiceNonWood(s, e) {

    var idsInvoice = [];
    idsInvoice.push($("#id_invoice").val());
    PrintInvoiceGenericNonWood(idsInvoice);
}

//Print Partial
function PrintInvoicePartial(s, e) {

    var data = { id_invoice: $("#id_invoice").val() };

    $.ajax({
        url: 'InvoiceExterior/InvoiceExteriorReportFilterPartial',
        data: data,
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

function validateItemContainer(s, e) {

    var numRows = gvInvoiceDetail.cpVisibleRowCount;

    if (numRows > 0 && (numeroContenedores.GetValue() == 0 || numeroContenedores.GetValue() == null)) {
        e.isValid = false;
    }
    else {
        e.isValid = true;
    }

    return e.isValid;

}

function validateCapacityContainer(s, e) {
    return !(numeroContenedores.GetValue() > 0 && s.GetSelectedItem() == null);
}

function validateShippingLine(s, e) {

    return !(id_shippingAgency.GetSelectedItem() != null && id_ShippingLine.GetSelectedItem() == null);
}

function validateShipNumberTrip(s, e) {
    return !(shipName.GetValue() != null && s.GetValue() == 0);
}

function OnDaeTextChanged(s, e) {

    var valorDae = s.GetValue();
    if (typeof valorDae == undefined || valorDae == null) {
        return;
    }

    var lenValorDae = (valorDae.length) + 1;
    if (lenValorDae >= s.cpMaxLength) {
        var obj = ASPxClientControl.GetControlCollection().GetByName(s.cpNextControl);
        if (typeof obj == undefined || obj == null) {
            return;
        }
        obj.Focus();

    }

}

function PortDischarge_SelectedIndexChanged(s, e) {

    var obj = ASPxClientControl.GetControlCollection().GetByName("id_portDestination");
    if (typeof obj === 'undefined' || obj === null) {
        return;
    }

    if (s.GetValue() != null) {
        obj.SetValue(s.GetValue());
    }

}

var InvoiceExteriorPaymentTermDetailView_InitEdit = function (s, e) {
    emissionDate.DateChanged.AddHandler(OnInvoiceEmissionDateChanged);
};

var InvoiceExteriorPaymentTermDetailView_BeginCallback = function (s, e) {
    e.customArgs["idPaymentTerm"] = id_PaymentTerm.GetValue();
    e.customArgs["emissionDate"] = emissionDate.GetValue();
    e.customArgs["invoiceTotal"] = valuetotalCIF.GetValue();
    e.customArgs["canEditPaymentTerm"] = s.cpCanEditPaymentTerm;
    e.customArgs["currentPaymentTermDetails"] = gvInvoiceExteriorPaymentTermDetailView.cpCurrentPaymentTermDetails;
};

var OnFechaVencimientoDateChanged = function (s, e) {
    var details = gvInvoiceExteriorPaymentTermDetailView.cpCurrentPaymentTermDetails;
    var numDetails = details !== null ? details.length : 0;
    for (var i = 0; i < numDetails; i++) {
        var detail = details[i];
        if (detail.orderPayment === s.cpOrderPayment) {
            detail.dueDate = getISODateString(s.GetDate());
            break;
        }
    }
};

var OnFechaVencimientoValidate = function (s, e) {
    var value = s.GetValue();
    if (value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
};

var getISODateString = function (date) {
    if (date !== null) {
        return date.getFullYear() + "-"
            + (date.getMonth() < 9 ? "0" : "") + (date.getMonth() + 1) + "-"
            + (date.getDate() < 10 ? "0" : "") + date.getDate();
    } else {
        return null;
    }
};

var OnIdPaymentTermSelectedIndexChanged = function (s, e) {
    gvInvoiceExteriorPaymentTermDetailView.PerformCallback();
};

var OnInvoiceEmissionDateChanged = function (s, e) {
    gvInvoiceExteriorPaymentTermDetailView.PerformCallback();
};

var OnInvoiceTotalValueReady = function (s, e) {
    gvInvoiceExteriorPaymentTermDetailView.PerformCallback();
};

var OnInitCommissionAgent = function (s, e) {
    //s.SetSelectedIndex(1);
};

function OnValueSubscribed_Changed(s, e) {
    if (!s.isValid) return;
    var valEnd = valueTotal.GetValue() - s.GetValue();
    balance.SetValue(valEnd);
};

function OnValueSubscribed_Validate(s, e) {
    

    //var originValue = s.inputValueBeforeFocus;
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Ingreso valor igual o superior a cero";
        //e.value = originValue;
    }
    if (s.GetValue() > valueTotal.GetValue())
    {
        e.isValid = false;
        e.errorText = "El abono no puede ser mayor al total";
        //e.value = originValue;
    } 

};

function OnFinalPayment_Changed(s, e) {
    if (!s.isValid) return;
    var valEnd = valueTotal.GetValue() - valueSubscribed.GetValue() - s.GetValue();
    balance.SetValue(valEnd);
};

function OnFinalPayment_Validate(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Ingreso valor igual o superior a cero";
    }
    if ((s.GetValue() + valueSubscribed.GetValue()) > valueTotal.GetValue()) {
        e.isValid = false;
        e.errorText = "El pago final no puede ser mayor al total";
    }
};


function init() {

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
