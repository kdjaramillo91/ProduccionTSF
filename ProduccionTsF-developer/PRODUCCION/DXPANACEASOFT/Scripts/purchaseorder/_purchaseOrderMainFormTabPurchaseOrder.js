
function UpdatePurchaseOrderProductionUnitProviders(productionUnitProviders) {

    for (var i = 0; i < id_productionUnitProvider.GetItemCount() ; i++) {
        var productionUnitProvider = id_productionUnitProvider.GetItem(i);
        var into = false;
        for (var j = 0; j < productionUnitProviders.length; j++) {
            if (productionUnitProvider.value == productionUnitProviders[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_productionUnitProvider.RemoveItem(i);
            i -= 1;
        }
    }
    for (var i = 0; i < productionUnitProviders.length; i++) {
        var productionUnitProvider = id_productionUnitProvider.FindItemByValue(productionUnitProviders[i].id);
        if (productionUnitProvider == null) id_productionUnitProvider.AddItem(productionUnitProviders[i].name, productionUnitProviders[i].id);
    }
    // 
    //Aquí se hará cambio
    if (productionUnitProviders.length == 1)
    {
        //id_productionUnitProvider.SetValue(productionUnitProviders[0].id);

        id_productionUnitProvider.SetSelectedIndex(0);
        var data = {
            id_productionunitprovider: id_productionUnitProvider.GetValue()//,
        };

        $.ajax({
            url: "PurchaseOrder/UpdatePurchaseOrderUnitProvider",
            type: "post",
            data: data,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
                //id_priceList.ClearItems();
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result !== null && result !== undefined) {
                    if (result.address != null) FishingSiteAddress.SetValue(result.address);
                    else FishingSiteAddress.SetValue("");
                    if (result.FishingSite != null) fishingSite.SetValue(result.FishingSite);
                    if (result.FishingZone != null) fishingZone.SetValue(result.FishingZone);
                    if (result.shippingType != null) UpdatePurchaseOrdershippingType(result.shippingType);
                    if (result.INPnumber != null) INPnumber.SetValue(result.INPnumber);
                    if (result.ministerialAgreement != null) ministerialAgreement.SetValue(result.ministerialAgreement);
                    if (result.tramitNumber != null) tramitNumber.SetValue(result.tramitNumber);
                }
                else {
                    FishingSiteAddress.SetValue("");
                    fishingSite.SetValue("");
                    fishingZone.SetValue("");
                    INPnumber.SetValue("");
                    ministerialAgreement.SetValue("");
                    tramitNumber.SetValue("");
                }

            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function UpdatePurchaseOrderProductionUnitProvidersProtective(productionUnitProvidersProtective) {
    for (var i = 0; i < id_productionUnitProviderProtective.GetItemCount() ; i++) {
        var productionUnitProvider = id_productionUnitProviderProtective.GetItem(i);
        var into = false;
        for (var j = 0; j < productionUnitProvidersProtective.length; j++) {
            if (productionUnitProvider.value == productionUnitProvidersProtective[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_productionUnitProviderProtective.RemoveItem(i);
            i -= 1;
        }
    }
    for (var i = 0; i < productionUnitProvidersProtective.length; i++) {
        var productionUnitProvider = id_productionUnitProviderProtective.FindItemByValue(productionUnitProvidersProtective[i].id);
        if (productionUnitProvider == null) id_productionUnitProviderProtective.AddItem(productionUnitProvidersProtective[i].name, productionUnitProvidersProtective[i].id);
    }
}

function UpdatePurchaseOrderPriceLists(priceLists) {
    for (var i = 0; i < id_priceList.GetItemCount() ; i++) {
        var priceList = id_priceList.GetItem(i);
        var into = false;
        for (var j = 0; j < priceLists.length; j++) {
            if (priceList.value == priceLists[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_priceList.RemoveItem(i);
            i -= 1;
        }
    }
    for (var i = 0; i < priceLists.length; i++) {
        var priceList = id_priceList.FindItemByValue(priceLists[i].id);
        if (priceList == null) id_priceList.AddItem(priceLists[i].name, priceLists[i].id);
    }
}

function ComboProvider_SelectedIndexChanged(s, e) {
    //id_priceList.SetValue(null);
    if (id_provider.GetValue() != null) {
        id_priceList.PerformCallback();
    }
    id_productionUnitProvider.ClearItems();
    id_productionUnitProviderProtective.ClearItems();
    
    var id_protectiveProviderTmp = 0;
    var data = {
        id_provider: s.GetValue()//,
    };

    $.ajax({
        url: "PurchaseOrder/UpdatePurchaseOrderPriceListsAndProductionUnitProviders",
        type: "post",
        data: data,
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
                UpdatePurchaseOrderProductionUnitProviders(result.productionUnitProviders);
                //UpdatePurchaseOrderPriceLists(result.priceLists);
                if (result.id_paymentTerm != null) id_paymentTerm.SetValue(result.id_paymentTerm);
            }
        },
        complete: function () {
            $.ajax({
                url: "PurchaseOrder/UpdatePurchaseOrderProtectiveProvider",
                type: "post",
                data: data,
                async: true,
                cache: false,
                error: function (error) {
                    console.log(error);
                    //id_priceList.ClearItems();
                },
                beforeSend: function () {
                    //showLoading();
                },
                success: function (result) {
                     
                    if (result !== null && result !== undefined) {
                        if (result.id_protectiveProvider != null) {
                            id_protectiveProviderTmp = result.id_protectiveProvider;
                            UpdatePurchaseOrderProtectiveProvider(result.id_protectiveProvider);
                        }
                    }
                    else {
                        INPnumberprotectiveProvider.SetValue("");
                        ministerialAgreementProtective.SetValue("");
                        tramitNumberProtective.SetValue("");
                    }
                },
                complete: function (result) {
                     
                    if (id_protectiveProviderTmp != 0) {
                        var data2 = {
                            id_provider: id_protectiveProviderTmp//,
                        };
                        $.ajax({
                            url: "PurchaseOrder/UpdateProductionUnitProvidersProtectiveByProvider",
                            type: "post",
                            data: data2,
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
                                    UpdatePurchaseOrderProductionUnitProvidersProtective(result.productionUnitProviders);
                                }
                                else {
                                    INPnumberprotectiveProvider.SetValue("");
                                    ministerialAgreementProtective.SetValue("");
                                    tramitNumberProtective.SetValue("");
                                }
                            },
                            complete: function () {
                                //hideLoading();
                            }
                        });
                    }
                    
                }
            });
        }

    });
    id_priceList.PerformCallback();
}
function OnEmissionDateValueChanged(s, e) {
    if (id_provider.GetValue() != null) {
        id_priceList.PerformCallback();
    }
}


function ComboProtectiveProvider_SelectedIndexChanged(s, e) {
    var data = {
        id_provider: s.GetValue()//,
    };
    $.ajax({
                url: "PurchaseOrder/UpdateProductionUnitProvidersProtectiveByProvider",
                type: "post",
                data: data,
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
                        UpdatePurchaseOrderProductionUnitProvidersProtective(result.productionUnitProviders);
                    }
                    else {
                        INPnumberprotectiveProvider.SetValue("");
                        ministerialAgreementProtective.SetValue("");
                        tramitNumberProtective.SetValue("");
                    }
                },
                complete: function () {
                    //hideLoading();
                }
            });
}

function UpdatePurchaseOrderProtectiveProvider(protectiveProvider) {
    for (var i = 0; i < id_providerapparent.GetItemCount() ; i++) {
        var providerapparent = id_providerapparent.GetItem(i);
        var into = false;
      
        if (protectiveProvider == providerapparent.value) {
            id_providerapparent.selectedValue = protectiveProvider;
            id_providerapparent.SetSelectedIndex(providerapparent.index);
            break;
        } 
    }
}

function PurchaseReasonsCombo_SelectedIndexChanged(s, e) {
    // 
    var gsOCDetail = $('#parOCDetail').val();

    var data = {
        id_purchaseReason: s.GetValue()
    };

    if (gsOCDetail == "1") {
        $.ajax({
            url: "PurchaseOrder/GetCodePurchaseReasonBG",
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
                $("#code_purchaseReason").val(result.code_purchaseReason);
                if (result.code_purchaseReason != "MP") {
                    id_priceList.SetValue(null);
                    id_priceList.SetEnabled(false);

                    pricePerList.SetChecked(false);
                    pricePerList.SetEnabled(false);

                    //isImportation.SetEnabled(true);

                } else {
                    id_priceList.SetEnabled(true);

                    pricePerList.SetChecked(true);
                    pricePerList.SetEnabled(true)
                }
                gvPurchaseOrderEditFormDetailsBG.PerformCallback();
                UpdateOrderTotalsBG();


            },
            complete: function () {
                //hideLoading();
            }
        });
    }
    else {
        $.ajax({
            url: "PurchaseOrder/GetCodePurchaseReason",
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
                $("#code_purchaseReason").val(result.code_purchaseReason);
                if (result.code_purchaseReason != "MP") {
                    id_priceList.SetValue(null);
                    id_priceList.SetEnabled(false);

                    pricePerList.SetChecked(false);
                    pricePerList.SetEnabled(false);

                    //isImportation.SetEnabled(true);

                } else {
                    id_priceList.SetEnabled(true);

                    pricePerList.SetChecked(true);
                    pricePerList.SetEnabled(true)

                    //isImportation.SetChecked(false);
                    //isImportation.SetEnabled(false);
                    //$("#detailImportation").hide();
                }
                gvPurchaseOrderEditFormDetails.PerformCallback();
                UpdateOrderTotals();


            },
            complete: function () {
                //hideLoading();
            }
        });
    }
    


}

function OnInitPriceList(s, e) {

    if (pricePerList.GetValue() === true || pricePerList.GetValue() === "true" || pricePerList.GetValue() === "True" ) {
        id_priceList.SetEnabled(true);
    } else {
        id_priceList.SetValue(null);
        id_priceList.SetEnabled(false);
    }
}

function PricePerList_CheckedChanged(s, e) {
    
    if (s.GetChecked()) {
        id_priceList.SetEnabled(true);

        var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);

        if (!valid) {
            UpdateTabImage({ isValid: false }, "tabPurchaseOrder");
        }


    } else {

        id_priceList.SetValue(null);
        id_priceList.SetEnabled(false);

    }
}

if (!String.prototype.padStart) {
    String.prototype.padStart = function padStart(targetLength, padString) {
        targetLength = targetLength >> 0; //floor if number or convert non-number to 0;
        padString = String(padString || ' ');
        if (this.length > targetLength) {
            return String(this);
        }
        else {
            targetLength = targetLength - this.length;
            if (targetLength > padString.length) {
                padString += padString.repeat(targetLength / padString.length); //append to original to ensure we are longer than needed
            }
            return padString.slice(0, targetLength) + String(this);
        }
    };
}

function OnDeliveryDateChanged(s, e) {
    var ahora = new Date();
    var deliveryDateAux = s.GetValue();

    var deliveryDateMonth = (deliveryDateAux.getMonth() + 1).toString();
    var deliveryDateDay = deliveryDateAux.getDate().toString();
    var deliveryDateAux2 = deliveryDateAux.getFullYear().toString() + deliveryDateMonth.padStart(2, "0") + deliveryDateDay.padStart(2, "0");
    console.log("deliveryDate: " + deliveryDateAux2);
    var ahoraDateMonth = (ahora.getMonth() + 1).toString();
    var ahoraDateDay = ahora.getDate().toString();
    var ahoraDate = ahora.getFullYear().toString() + ahoraDateMonth.padStart(2, "0") + ahoraDateDay.padStart(2, "0");
    console.log("ahoraDate: " + ahoraDate);

    if (deliveryDateAux2 < ahoraDate) {
        $.ajax({
            url: "PurchaseOrder/UpdateViewDataEditMessage",
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
                sampleMessageLabel.SetText(result.Message);
                $("#SampleMessage").show();
                if ($(".alert-warning") !== undefined && $(".alert-warning") !== null) {
                    $(".alert-warning").fadeTo(3000, 0.45, function () {
                        $(".alert-warning").alert('close');
                    });
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function ComboproductionUnitProvider_SelectedIndexChanged(s, e) {

    var data = {
        id_productionunitprovider: s.GetValue()//,
    };

    $.ajax({
        url: "PurchaseOrder/UpdatePurchaseOrderUnitProvider",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_priceList.ClearItems();
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                if (result.address != null) FishingSiteAddress.SetValue(result.address);
                else FishingSiteAddress.SetValue("");
                if (result.FishingSite != null) fishingSite.SetValue(result.FishingSite);
                if (result.FishingZone != null) fishingZone.SetValue(result.FishingZone);
                if (result.shippingType != null) UpdatePurchaseOrdershippingType(result.shippingType);
                if (result.INPnumber != null) INPnumber.SetValue(result.INPnumber);
                if (result.ministerialAgreement != null) ministerialAgreement.SetValue(result.ministerialAgreement);
                if (result.tramitNumber != null) tramitNumber.SetValue(result.tramitNumber);
            }
            else {
                FishingSiteAddress.SetValue("");
                fishingSite.SetValue("");
                fishingZone.SetValue("");
                INPnumber.SetValue("");
                ministerialAgreement.SetValue("");
                tramitNumber.SetValue("");
            }
         
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function ComboproductionUnitProviderProtective_SelectedIndexChanged(s, e) {
     
    var data = {
        id_productionunitprovider: s.GetValue()//,
    };
    $.ajax({
        url: "PurchaseOrder/UpdatePurchaseOrderUnitProviderProtective",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_priceList.ClearItems();
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                if (result.INPnumber != null) INPnumberprotectiveProvider.SetValue(result.INPnumber);
                if (result.ministerialAgreement != null) ministerialAgreementProtective.SetValue(result.ministerialAgreement);
                if (result.tramitNumber != null) tramitNumberProtective.SetValue(result.tramitNumber);
            }
            else {
                INPnumberprotectiveProvider.SetValue("");
                ministerialAgreementProtective.SetValue("");
                tramitNumberProtective.SetValue("");
            }

        },
        complete: function () {
            //hideLoading();
        }
    });
}

function UpdatePurchasOrderFishingSite(FishingSite) {
    for (var i = 0; i < id_FishingSite.GetItemCount() ; i++) {
        var FishingSiteaux = id_FishingSite.GetItem(i);
        var into = false;

        if (FishingSite == FishingSiteaux.value) {
            id_FishingSite.selectedValue = FishingSite;
            id_FishingSite.SetSelectedIndex(FishingSiteaux.index);



            break;
        }


    }
}

function UpdatePurchaseOrdershippingType(ShippingType) {
    for (var i = 0; i < id_shippingType.GetItemCount() ; i++) {
        var ShippingTypeaux = id_shippingType.GetItem(i);
        var into = false;

        if (ShippingType == ShippingTypeaux.value) {
            id_shippingType.selectedValue = ShippingType;
            id_shippingType.SetSelectedIndex(ShippingTypeaux.index);



            break;
        }


    }
}

function OnLogisticsRequiredInit(s, e) {
    if (requiredLogistic.GetChecked()) {
        deliveryDateLabel.SetText("Fecha y Hora en Camaronera:");
    } else {
        deliveryDateLabel.SetText("Fecha y Hora en Planta:");
    }
    //gridMessageErrorsDetail.SetText(ErrorMessage("Este detalle no tiene calidad."));
}
function OnLogisticsRequiredChanged(s, e) {
    if (requiredLogistic.GetChecked()) {
        deliveryDateLabel.SetText("Fecha y Hora en Camaronera:");
    } else {
        deliveryDateLabel.SetText("Fecha y Hora en Planta:");
    }
}

function ComboCertification_SelectedIndexChanged(s, e) {
    var data = {
        id_certification: s.GetValue()//,
    };
    $.ajax({
        url: "PurchaseOrder/UpdateCertificationProtectiveByProvider",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {

            console.log("OK");
        },
        complete: function () {
            //hideLoading();
        }
    });
}
