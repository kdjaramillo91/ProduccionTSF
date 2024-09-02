
// COMMON TABS FUNCTIOS VALIDATIONS

function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }

    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}

function OnEmissionDateValidation(s, e) {
    OnEmissionDateDocumentValidation(e, emissionDate, "remissionGuide");
}

// REMISSION GUIDE HEADER VALIDATIONS

function OnReciverValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnReasonValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnIsExportValidation(s, e) {
    e.isValid = true;
}

function OnStartAddressValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnRouteValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDespachureDateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


function OndespachurehourValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


function OnArrivalDateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnReturnDateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// REMISSION GUIDE DETAILS VALIDATIONS


// REMISSION GUIDE DISPATCH MATERIALS VALIDATIONS


// REMISSION GUIDE TRANSPORTATION VALIDATIONS

function OnIsOwnValidation(s, e) {
    e.isValid = true;
}

function OnCarRegistrationThirdValidation(s, e) {
    if (!isOwn.GetChecked()) {
        e.isValid = true;
        return;
    }
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabTransportation");
}

function OnDriverNameThirdValidation(s, e) {
    if (!isOwn.GetChecked()) {
        e.isValid = true;
        return;
    }
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabTransportation");
}

function OnProviderValidation(s, e) {
    if (isOwn.GetChecked()) {
        e.isValid = true;
        return;
    }
    if(isOwn.GetChecked() && e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }

    UpdateTabImage(e, "tabTransportation");
}

function OnVehicleValidation(s, e) {
    if (isOwn.GetChecked()) {
        e.isValid = true;
        return;
    }
    var incorrect = false
    var messageErrorText = "";
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    if (e.value != null) {
        // 
        if (id_vehicle.GetValue() != null && id_vehicle.GetValue() != undefined) {
            $.ajax({
                url: "RemGuideLandAditional/ValidateVehicleAnotherRemissionGuideProviderCompany",
                type: "post",
                async: false,
                cache: false,
                data: {
                    id_vehicle: id_vehicle.GetValue(),
                    id_remissionguide: parseInt(document.getElementById("id_remissionGuide").getAttribute("idremissionGuide")),
                    despachureHourTmp: despachureDate.GetText()
                },
                error: function (error) {
                },
                beforeSend: function () {
                },
                success: function (result) {
                    // 
                    if (result !== null) {
                        if (result.itsAssigned === "YES") {
                            incorrect = true;
                            messageErrorText = result.Error1;
                        }
                        else if (result.noneProvider === "YES") {
                            incorrect = true;
                            messageErrorText = result.Error2;
                        }
                        else if (result.noneProviderBilling === "YES") {
                            incorrect = true;
                            messageErrorText = result.Error3;
                        }
                    }
                },
                complete: function () {
                }
            });
        }
        
    }
    if (incorrect == true) {
        e.isValid = false;
        e.errorText = messageErrorText;
    }
    UpdateTabImage(e, "tabTransportation");
}


function OnTransportTariffType(s, e) {
    var isManualTmp = $("#isManual").val();
    if (isOwn.GetChecked())
    {
        e.isValid = true;
        return;
    }
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    UpdateTabImage(e, "tabTransportation");
}


function OnDriverValidation(s, e) {
    if (isOwn.GetChecked()) {
        e.isValid = true;
        return;
    }
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }

    UpdateTabImage(e, "tabTransportation");
}

function OnAdvancePriceValidation(s, e) {
    if (isOwn.GetChecked()) {
        e.isValid = true;
        UpdateTabImage(e, "tabTransportation");
        return;
    }
    var valuePriceTmp = valuePrice.GetText();
    if (valuePriceTmp == null) {
        valuePriceTmp = 0;
    }
    if (e.value != null) {
        if (parseFloat(e.value) > parseFloat(valuePriceTmp)) {
            e.isValid = false;
            e.errorText = "El valor del Anticipo no puede ser mayor que el valor del Flete";
        }
    }
    UpdateTabImage(e, "tabTransportation");
}

// REMISSION GUIDE SECURTY SEALS VALIDATIONS

function OnSecuritySealNumberValidation(s, e) {
    var noVale = false;
    var texto = "";
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var indice = gvSecuritySeals.cpEditingRowVisibleIndex;
        var isNew = true;
        if (indice >= 0) {
            isNew = false;
        }
        $.ajax({
            url: "RemGuideLandAditional/ValidateSecuritySealNumber",
            type: "post",
            async: false,
            cache: false,
            data: {
                id_remissionGuide: parseInt(document.getElementById("id_remissionGuide").getAttribute("idremissionGuide")),
                number: e.value,
                isNew: isNew
            },
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                
                // 
                if (result !== null) {
                    if (result.itsRepeated == 1) {
                        noVale = true;
                        texto = result.Error;
                    }
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
        if (noVale == true) {
            e.isValid = false;
            e.errorText = texto;
            UpdateTabImage(e, "tabSecuritySeals");
        }
    }
}

function OnSecuritySealTravelTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnSecuritySealExitStateValidation(s, e)
{
    e.isValid = true;
}

function OnSecuritySealArrivalStateValidation(s, e) {
    e.isValid = true;
}

// REMISSION GUIDE ASSIGNED STAFF VALIDATIONS

function OnAssigendPersonValidation(s, e) {
    // 
    var hasError = false;
    var textError = "";
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var indice = gvAssignedStaff.cpEditingRowVisibleIndex;
        var isNew = true;
        if (indice >= 0) {
            isNew = false;
        }
        $.ajax({
            url: "RemGuideLandAditional/ValidateAssigendPersonValidation",
            type: "post",
            async: false,
            cache: false,
            data: {
                id_remissionGuide: parseInt(document.getElementById("id_remissionGuide").getAttribute("idremissionGuide")),
                id_person: e.value,
                isNew: isNew
            },
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {

                // 
                if (result !== null) {
                    if (result.itsRepeated == 1) {
                        hasError = true;
                        textError = result.Error;
                    }
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
        if (hasError == true) {
            e.isValid = false;
            e.errorText = textError;
        }
    }
    UpdateTabImage(e, "tabAssignedStaff");
}

function OnAssigendPersonTravelTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnAssigendPersonRolValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnviaticPriceValidation(s, e) {
    if (isOwn.GetChecked()) {
        if (e.value > 0) {
            e.isValid = false;
            e.errorText = "No puede ser mayor que 0";
        }
    }
    UpdateTabImage(e, "tabAssignedStaff");
}

// REMISSION GUIDE EXPORTATION VALIDATIONS 

function OnUniqueCustomsDocumentValidation(s, e) {
    if (isExport.GetChecked() && e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }

    UpdateTabImage(e, "tabExport");
}

function validaterange(Dateinit, dateend) {
    valuesStart = Dateinit.split("/");
    valuesEnd = fechaFinal.split("/");

    
    var dateStart = new Date(valuesStart[2], (valuesStart[1] - 1), valuesStart[0]);
    var dateEnd = new Date(valuesEnd[2], (valuesEnd[1] - 1), valuesEnd[0]);
    if (dateEnd>=dateStart  ) {
        return 0;
    }
    return 1;
}

