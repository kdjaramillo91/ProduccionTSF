// TABIMAGE

function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }
    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}

function UpdateTabControlImage(e, tabName, tabControlCurrent) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }
    var tab = tabControlCurrent.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}

function OnIdentificationNumberValidation(s, e) {
    e.isValid = true;
    e.errorText = "";
    var result = OnIdentificationNumberValidationGeneral(e.value);
    debugger;
    if (result != undefined) {
        debugger;
        e.isValid = result.isValid;
        e.errorText = result.errorTextMessage
    }
    
}

function OnIdentificationNumberValidationGeneral(parametroIdentificacion) {
    //var item = id_identificationType.GetSelectedItem();
    //var errorTextMessage = "";
    //var isValidMessage = true;

    //if (parametroIdentificacion === null || parametroIdentificacion === undefined) {
    //    isValid = false;
    //    errorTextMessage = "Campo Obligatorio";
    //} else {
    //    var validation = null;

    //    var id_personTmp = $("#id_person").val();
    //    $.ajax({
    //        url: "Person/VerifyIdentificationCode",
    //        type: "post",
    //        data: { id_code: parametroIdentificacion, id_person: id_personTmp },
    //        async: false,
    //        cache: false,
    //        error: function (error) {
    //        },
    //        beforedSend: function () {

    //        },
    //        success: function (result) {

    //            if (result.isRepeated === "SI") {
    //                var isRepeatedTmp = "SI";
    //                isValidMessage = false;
    //                errorTextMessage = "Número de Identificación pertenece a la persona " + result.personName;
    //            }
    //        },
    //        complete: function () {

    //        }
    //    });

    //    if (isRepeatedTmp == "SI") {
    //        return { isValid: isValidMessage, errorTextMessage: errorTextMessage }
    //    }
    //    if (item != null) {
    //        if (item.value === 2) {
    //            validation = validarCI(parametroIdentificacion);
    //        } else if (item.value === 1) {
    //            validation = validarRUC(parametroIdentificacion);
    //        }
    //    }


    //    if (validation !== null && validation !== undefined && !validation.isValid) {
    //        isValidMessage = validation.isValid;
    //        errorTextMessage = validation.errorText;
    //    }
    //    return { isValid: isValidMessage, errorTextMessage: errorTextMessage }
    //}
    debugger;
    var item = id_identificationType.GetSelectedItem();
    var errorTextMessage = "";
    var isValidMessage = true;
    var isRepeatedTmp = "";

    if (parametroIdentificacion == null || parametroIdentificacion == undefined) {
        isValid = false;
        errorTextMessage = "Campo Obligatorio";
    } else {
        var validation = null;

        var id_personTmp = $("#id_person").val();
        $.ajax({
            url: "Person/VerifyIdentificationCode",
            type: "post",
            data: { id_code: parametroIdentificacion, id_person: id_personTmp },
            async: false,
            cache: false,
            error: function (error) {
            },
            beforedSend: function () {

            },
            success: function (result) {
                
                if (result.isRepeated == "SI") {
                    isRepeatedTmp = "SI";
                    isValidMessage = false;
                    errorTextMessage = "Número de Identificación pertenece a la persona " + result.personName;
                }
            },
            complete: function () {

            }
        });

        if (isRepeatedTmp == "SI") {
            return { isValid: isValidMessage, errorTextMessage: errorTextMessage }
        }
        if (item != null) {
            debugger;
            if (item.value == 2) {
                validation = validarCI(parametroIdentificacion);
            } else if (item.value == 1) {
                validation = validarRUC(parametroIdentificacion);
            }
        }
        

        if (validation != null && validation != undefined && validation.isValid == false) {
            isValidMessage = validation.isValid;
            errorTextMessage = validation.errorText;
        }
        return { isValid: isValidMessage, errorTextMessage: errorTextMessage }
    }
}


function OnEmailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var validation = validarEMAIL(e.value);

        if (!validation.isValid) {
            e.isValid = validation.isValid;
            e.errorText = validation.errorText;
        }
    }
}

// PROVIDER TYPE SHRIMP VALIDATOR
function OnMinisterialAgreementValidation(s, e) {
    var tab = tabControl.GetTabByName("tabProvider");

    if (!tab.GetVisible()) {
        e.isValid = true;
        UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else if (e.value === null) {
        var id_providerType2 = id_providerType.GetValue();

        if (id_providerType2 != 0 && id_providerType2 != null && id_providerType2 !== undefined) {
            $.ajax({
                url: "Person/ProviderTypeWhatProviderIs",
                type: "post",
                data: { id_providerType: id_providerType2 },
                async: false,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforedSend: function () {

                },
                success: function (result) {
                    if (result.isShrimpProvider == "SI") {
                        e.isValid = false;
                        e.errorText = "Campo obligatorio";
                        UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
                        UpdateTabImage(e, "tabProvider");
                    } else {
                        e.isValid = true;
                        UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
                        UpdateTabImage(e, "tabProvider");
                    }
                },
                complete: function () {

                }
            });
        }


    }

   // UpdateTabImage(e, "tabWeight");
}

//TRADENAME VALIDATOR
function OnTradeNameValidation(s, e) {
    e.isValid = true;
    var itemTmp = id_identificationType.GetSelectedItem();
    if (itemTmp != null)
    {
        if (itemTmp.value == 1) {
            if (e.value == null) {
                e.isValid = false;
                e.errorText = "Campo Obligatorio cuando es RUC";
            }
        }
    }
}

// PROVIDER VALIDATOR

function OnPaymentMethodValidation(s, e) {
    var tab = tabControl.GetTabByName("tabProvider");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }

    UpdateTabImage(e, "tabProvider");
}

function OnPaymentMeanValidation(s, e) {
    var tab = tabControl.GetTabByName("tabProvider");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }

    UpdateTabImage(e, "tabProvider");
}


// PROVIDER GENERAL DATA EP VALIDATOR

function OnIdentificationTypeEPValidation(s, e) {
    if (!electronicPayment.GetChecked()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    }
}

function OnPaymentMethodEPValidation(s, e) {
    if (!electronicPayment.GetChecked()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    }
}

function OnNoAccountEPValidation(s, e) {
    if (!electronicPayment.GetChecked()) {
        e.isValid = true;
    } else
        if (e.value !== null && e.value.toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    }
}

function OnNoRefTransferValidation(s, e) {
    if (!electronicPayment.GetChecked()) {
        e.isValid = true;
    } else
        if (e.value !== null && e.value.toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    }
}

function OnNoRouteValidation(s, e) {
    if (!electronicPayment.GetChecked()) {
        e.isValid = true;
    } else
        if (e.value !== null && e.value.toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
        UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    }
}

function OnEmailEPValidation(s, e) {
    if (!electronicPayment.GetChecked()) {
        e.isValid = true;
    } else
        if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else
        if (e.value !== null) {
            var validation = null;
            validation = validarEMAIL(e.value);
            if (validation !== null && validation !== undefined && !validation.isValid) {
                e.isValid = validation.isValid;
                e.errorText = validation.errorText;
                UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
                UpdateTabImage(e, "tabProvider");
            }
        }
}

// PROVIDER GENERAL DATA RISE VALIDATOR

function OnCategoryRiseValidation(s, e) {
    if (!rise.GetChecked()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    }
}

function OnActivityRiseValidation(s, e) {
    if (!rise.GetChecked()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabGeneralDataProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    }
}

// EMPLOYEE VALIDATOR

function OnEmployeeDepartmentValidation(s, e) {
    var tab = tabControl.GetTabByName("tabEmployee");
    if (!tab.GetVisible()) {
        e.isValid = true;
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }

    UpdateTabImage(e, "tabEmployee");
}

