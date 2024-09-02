

// ProviderAll
var id_companyIniAux = 0;
var id_divisionIniAux = 0;
var id_branchOfficeIniAux = 0;


//ProviderAccountingAccounts

function ProviderAccountingAccountsCompanyCombo_Init(s, e) {

    //id_companyIniAux = s.GetValue();
    //id_divisionIniAux = id_division.GetValue();
    //id_branchOfficeIniAux = id_branchOffice.GetValue();

    var data = {
        id_company: s.GetValue(),
        id_division: id_division.GetValue(),
        id_branchOffice: id_branchOffice.GetValue(),
        id_accountFor: id_accountFor.GetValue(),
        id_accountPlan: id_accountPlan.GetValue(),
        id_account: id_account.GetValue(),
        id_accountingAssistantDetailType: id_accountingAssistantDetailType.GetValue()
    };

    $.ajax({
        url: "Person/InitProviderAccountingAccountsCompanyCombo",
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
            arrayFieldStr.push("code");
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_company, result.companies, arrayFieldStr);

            //id_division
            UpdateDetailObjects(id_division, result.divisions, arrayFieldStr);

            //id_branchOffice
            UpdateDetailObjects(id_branchOffice, result.branchOffices, arrayFieldStr);

            //id_accountPlan
            UpdateDetailObjects(id_accountPlan, result.accountPlans, arrayFieldStr);

            //id_accountFor
            arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_accountFor, result.accountFors, arrayFieldStr);

            //id_account
            var arrayFieldStr = [];
            arrayFieldStr.push("number");
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_account, result.accounts, arrayFieldStr);

            //id_accountingAssistantDetailType
            var arrayFieldStr = [];
            arrayFieldStr.push("assistantTypeCode");
            arrayFieldStr.push("accountingAssistantCode");
            arrayFieldStr.push("accountingAssistantName");
            UpdateDetailObjects(id_accountingAssistantDetailType, result.accountingAssistantDetailTypes, arrayFieldStr);

        },
        complete: function () {
            //hideLoading();
        }
    });
}

function ProviderAccountingAccountsAccountPlanCombo_SelectedIndexChanged(s, e) {

    id_account.ClearItems();
    id_accountingAssistantDetailType.ClearItems();

    $.ajax({
        url: "Person/ProviderAccountingAccountsAccountPlanDetailData",
        type: "post",
        data: { id_accountPlan: s.GetValue() },
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
            if (result !== null) {
                var arrayFieldStr = [];
                arrayFieldStr.push("number");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_account, result.accounts, arrayFieldStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function ProviderAccountingAccountsAccountCombo_SelectedIndexChanged(s, e) {

    id_accountingAssistantDetailType.ClearItems();

    $.ajax({
        url: "Person/ProviderAccountingAccountsAccountDetailData",
        type: "post",
        data: { id_account: s.GetValue() },
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
            if (result !== null) {
                var arrayFieldStr = [];
                arrayFieldStr.push("assistantTypeCode");
                arrayFieldStr.push("accountingAssistantCode");
                arrayFieldStr.push("accountingAssistantName");
                UpdateDetailObjects(id_accountingAssistantDetailType, result.accountingAssistantDetailTypes, arrayFieldStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

//ProviderMailByComDivBra

function OnProviderMailByComDivBraEmailValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else
        if (e.value !== null) {
            var validation = null;
            validation = validarEMAIL(e.value);
            if (validation !== null && validation !== undefined && !validation.isValid) {
                e.isValid = validation.isValid;
                e.errorText = validation.errorText;
                UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
                UpdateTabImage(e, "tabProvider");
            }
        }
}

//ProviderItem

var id_itemIniAux = 0;



//ProviderRelatedCompany

function ProviderRelatedCompanyCompanyCombo_Init(s, e) {

    id_companyIniAux = s.GetValue();
    id_divisionIniAux = id_division.GetValue();
    id_branchOfficeIniAux = id_branchOffice.GetValue();

    var data = {
        id_company: s.GetValue(),
        id_division: id_division.GetValue(),
        id_branchOffice: id_branchOffice.GetValue()
    };

    $.ajax({
        url: "Person/InitProviderPaymentTermCompanyCombo",
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
            arrayFieldStr.push("code");
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_company, result.companies, arrayFieldStr);

            //id_division
            //arrayFieldStr = [];
            //arrayFieldStr.push("code");
            //arrayFieldStr.push("name");
            UpdateDetailObjects(id_division, result.divisions, arrayFieldStr);

            //id_branchOffice
            //arrayFieldStr = [];
            //arrayFieldStr.push("name");
            UpdateDetailObjects(id_branchOffice, result.branchOffices, arrayFieldStr);

        },
        complete: function () {
            //hideLoading();
        }
    });
}

function ProviderRelatedCompanyCompanyCombo_SelectedIndexChanged(s, e) {

    id_division.ClearItems();
    id_branchOffice.ClearItems();

    $.ajax({
        url: "Person/ProviderPaymentTermCompanyDetailData",
        type: "post",
        data: { id_company: s.GetValue() },
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
            if (result !== null) {
                var arrayFieldStr = [];
                arrayFieldStr.push("code");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_division, result.divisions, arrayFieldStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function ProviderRelatedCompanyDivisionCombo_SelectedIndexChanged(s, e) {

    id_branchOffice.ClearItems();

    $.ajax({
        url: "Person/ProviderPaymentTermDivisionDetailData",
        type: "post",
        data: { id_company: id_company.GetValue(), id_division: s.GetValue() },
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
            if (result !== null) {
                var arrayFieldStr = [];
                arrayFieldStr.push("code");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_branchOffice, result.branchOffices, arrayFieldStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function OnProviderRelatedCompanyBranchOfficeComboValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else {
        var data = {
            id_companyNew: id_company.GetValue(),
            id_divisionNew: id_division.GetValue(),
            id_branchOfficeNew: id_branchOffice.GetValue()
        };
        if (data.id_companyNew != id_companyIniAux || data.id_divisionNew != id_divisionIniAux ||
            data.id_branchOfficeNew != id_branchOfficeIniAux) {
            $.ajax({
                url: "Person/ItsRepeatedRelatedCompany",
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
                            UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
                            UpdateTabImage(e, "tabProvider");
                        } else {
                            id_companyIniAux = 0;
                            id_divisionIniAux = 0;
                            id_branchOfficeIniAux = 0;
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

//ProviderPersonAuthorizedToPayTheBill

function OnProviderPersonAuthorizedToPayTheBillIdentificationNumberComboValidation(s, e) {

    var item = id_identificationTypeProviderPersonAuthorizedToPayTheBill.GetSelectedItem();

    if (item === null) {
        e.isValid = false;
        e.errorText = "Seleccione el ID.";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else {
        var validation = null;

        if (item.value === 2) {
            validation = validarCI(e.value);
        } else if (item.value === 1) {
            validation = validarRUC(e.value);
        }

        if (validation !== null && validation !== undefined && !validation.isValid) {
            e.isValid = validation.isValid;
            e.errorText = validation.errorText;
            if (validation.isValid == false) {
                UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
                UpdateTabImage(e, "tabProvider");
            }
        }
    }

}

function OnProviderPersonAuthorizedToPayTheBillPhoneNumber1Validation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else
        if (e.value !== null) {
            var validation = null;
            validation = validarPhoneNumber(e.value);
            if (validation !== null && validation !== undefined && !validation.isValid) {
                e.isValid = validation.isValid;
                e.errorText = validation.errorText;
                UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
                UpdateTabImage(e, "tabProvider");
            }
        }
}

function OnProviderPersonAuthorizedToPayTheBillPhoneNumber2Validation(s, e) {
    if (e.value !== null) {
        var validation = null;
        validation = validarPhoneNumber(e.value);
        if (validation !== null && validation !== undefined && !validation.isValid) {
            e.isValid = validation.isValid;
            e.errorText = validation.errorText;
            UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
            UpdateTabImage(e, "tabProvider");
        }
    }
}

function OnProviderPersonAuthorizedToPayTheBillCodeValidation(s, e) {
    if (e.value !== null && e.value.toString().length > 5) {
        e.isValid = false;
        e.errorText = "Máximo 5 caracteres";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    }
}

function ProviderPersonAuthorizedToPayTheBillIdentificationTypeCombo_Init(s, e) {

    //id_retentionIniAux = id_retention.GetValue();

    var data = {
        id_identificationType: id_identificationTypeProviderPersonAuthorizedToPayTheBill.GetValue(),
        id_country: id_country.GetValue(),
        id_bank: id_bank.GetValue(),
        id_accountType: id_accountType.GetValue()
    };

    $.ajax({
        url: "Person/InitProviderPersonAuthorizedToPayTheBillIdentificationTypeCombo",
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
            arrayFieldStr.push("code");
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_identificationTypeProviderPersonAuthorizedToPayTheBill, result.identificationTypes, arrayFieldStr);

            //id_division
            //arrayFieldStr = [];
            //arrayFieldStr.push("code");
            //arrayFieldStr.push("name");
            UpdateDetailObjects(id_country, result.countries, arrayFieldStr);

            //id_branchOffice
            //arrayFieldStr = [];
            //arrayFieldStr.push("name");
            UpdateDetailObjects(id_bank, result.banks, arrayFieldStr);


            UpdateDetailObjects(id_accountType, result.accountTypes, arrayFieldStr);


        },
        complete: function () {
            //hideLoading();
        }
    });
}

function OnProviderPersonAuthorizedToPayTheBillAmountValidation(s, e) {
    if (e.value !== null) {
        if (parseFloat(e.value) < 0) {
            e.isValid = false;
            e.errorText = "Monto debe ser mayor e igual a cero o nulo";
            UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
            UpdateTabImage(e, "tabProvider");
        }
    }
}

function OnProviderPersonAuthorizedToPayTheBillNoPaymentsValidation(s, e) {
    if (e.value !== null) {
        if (parseFloat(e.value) < 0) {
            e.isValid = false;
            e.errorText = "Número de Pagos debe ser mayor e igual a cero o nulo";
            UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
            UpdateTabImage(e, "tabProvider");
        }
    }
}

//ProviderRetention
var id_retentionIniAux = false;

function OnProviderRetentionComboValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else {
        var data = {
            id_retentionNew: id_retention.GetValue()
        };
        if (data.id_retentionNew != id_retentionIniAux) {
            
            $.ajax({
                url: "Person/ItsRepeatedRetention",
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
                            UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
                            UpdateTabImage(e, "tabProvider");
                        } else {
                            id_retentionIniAux = 0;
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

function ProviderRetentionRetentionTypeCombo_Init(s, e) {

    id_retentionIniAux = id_retention.GetValue();

    var data = {
        id_retentionType: s.GetValue(),
        id_retentionGroup: id_retentionGroup.GetValue(),
        id_retention: id_retention.GetValue()
    };

    $.ajax({
        url: "Person/InitProviderRetentionRetentionTypeCombo",
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
            arrayFieldStr.push("code");
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_retentionType, result.retentionTypes, arrayFieldStr);

            //id_division
            //arrayFieldStr = [];
            //arrayFieldStr.push("code");
            //arrayFieldStr.push("name");
            UpdateDetailObjects(id_retentionGroup, result.retentionGroups, arrayFieldStr);

            //id_branchOffice
            //arrayFieldStr = [];
            //arrayFieldStr.push("name");
            UpdateDetailObjects(id_retention, result.retentions, arrayFieldStr);

        },
        complete: function () {
            //hideLoading();
        }
    });
}

function ProviderRetentionRetentionTypeCombo_SelectedIndexChanged(s, e) {
    UpdateProviderRetentionRetentionCombo({ id_retentionType: s.GetValue(), id_retentionGroup: id_retentionGroup.GetValue() });
}

function ProviderRetentionRetentionGroupCombo_SelectedIndexChanged(s, e) {
    UpdateProviderRetentionRetentionCombo({ id_retentionType: id_retentionType.GetValue(), id_retentionGroup: s.GetValue() });
}

function UpdateProviderRetentionRetentionCombo(data) {

    id_retention.ClearItems();
    percentRetencion.SetValue(0);

    $.ajax({
        url: "Person/UpdateProviderRetentionRetentionCombo",
        type: "post",
        data: data,
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
            if (result !== null) {
                var arrayFieldStr = [];
                arrayFieldStr.push("code");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_retention, result.retentions, arrayFieldStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function ProviderRetentionRetentionCombo_SelectedIndexChanged(s, e) {

    $.ajax({
        url: "Person/ProviderRetentionRetentionDetailData",
        type: "post",
        data: { id_retention : s.GetValue()},
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
            if (result !== null) {
                percentRetencion.SetValue(result.percentRetencion);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

//ProviderSeriesForDocuments

function OnProviderInitialNumberValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else if (parseFloat(e.value) < 0) {
        e.isValid = false;
        e.errorText = "Número Inicial debe ser mayor e igual a cero";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else {
        var finalNumberAux = finalNumber.GetValue();
        finalNumberAux = finalNumberAux == null ? "0" : finalNumberAux;
        if (parseFloat(e.value) > parseFloat(finalNumberAux)) {
            e.isValid = false;
            e.errorText = "Número Inicial no puede ser mayor que el Número Final";
            UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
            UpdateTabImage(e, "tabProvider");
        } else {
            var currentNumberAux = currentNumber.GetValue();
            currentNumberAux = currentNumberAux == null ? "0" : currentNumberAux;
            if (parseFloat(e.value) > parseFloat(currentNumberAux)) {
                e.isValid = false;
                e.errorText = "Número Inicial no puede ser mayor que el Número Actual";
                UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
                UpdateTabImage(e, "tabProvider");
            }
        }
    }
}

function OnProviderFinalNumberValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else if (parseFloat(e.value) <= 0) {
        e.isValid = false;
        e.errorText = "Número Final debe ser mayor que cero";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else {
        var initialNumberAux = initialNumber.GetValue();
        initialNumberAux = initialNumberAux == null ? "0" : initialNumberAux;
        if (parseFloat(e.value) <= parseFloat(initialNumberAux)) {
            e.isValid = false;
            e.errorText = "Número Final no puede ser menor o igual que el Número Inicial";
            UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
            UpdateTabImage(e, "tabProvider");
        } else {
            var currentNumberAux = currentNumber.GetValue();
            currentNumberAux = currentNumberAux == null ? "0" : currentNumberAux;
            if (parseFloat(e.value) < parseFloat(currentNumberAux)) {
                e.isValid = false;
                e.errorText = "Número Final no puede ser menor que el Número Actual";
                UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
                UpdateTabImage(e, "tabProvider");
            }
        }
    }
}

function OnProviderCurrentNumberValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else if (parseFloat(e.value) < 0) {
        e.isValid = false;
        e.errorText = "Número Actual debe ser mayor e igual a cero";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else {
        var initialNumberAux = initialNumber.GetValue();
        initialNumberAux = initialNumberAux == null ? "0" : initialNumberAux;
        if (parseFloat(e.value) < parseFloat(initialNumberAux)) {
            e.isValid = false;
            e.errorText = "Número Actual no puede ser menor que el Número Inicial";
            UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
            UpdateTabImage(e, "tabProvider");
        } else {
            var finalNumberAux = finalNumber.GetValue();
            finalNumberAux = finalNumberAux == null ? "0" : finalNumberAux;
            if (parseFloat(e.value) > parseFloat(finalNumberAux)) {
                e.isValid = false;
                e.errorText = "Número Actual no puede ser mayor que el Número Final";
                UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
                UpdateTabImage(e, "tabProvider");
            }
        }
    }
}

function ProviderSeriesForDocumentsDocumentTypeCombo_Init(s, e) {

    var data = {
        id_documentType: s.GetValue(),
        id_retentionSeriesForDocumentsType: id_retentionSeriesForDocumentsType.GetValue()
    };

    $.ajax({
        url: "Person/InitProviderSeriesForDocumentsDocumentTypeCombo",
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
            //id_documentType
            var arrayFieldStr = [];
            arrayFieldStr.push("code");
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_documentType, result.documentTypes, arrayFieldStr);

            //id_retentionSeriesForDocumentsType
            //arrayFieldStr = [];
            //arrayFieldStr.push("code");
            //arrayFieldStr.push("name");
            UpdateDetailObjects(id_retentionSeriesForDocumentsType, result.retentionSeriesForDocumentsTypes, arrayFieldStr);

        },
        complete: function () {
            //hideLoading();
        }
    });
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

function OnDateOfExpiryValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else {
        var ahora = new Date();
        var dateOfExpiryAux = s.GetValue();

        var dateOfExpiryMonth = (dateOfExpiryAux.getMonth() + 1).toString();
        var dateOfExpiryDay = dateOfExpiryAux.getDate().toString();
        var dateOfExpiryAux2 = dateOfExpiryAux.getFullYear().toString() + dateOfExpiryMonth.padStart(2, "0") + dateOfExpiryDay.padStart(2, "0");
        //console.log("dateOfExpiry: " + dateOfExpiryAux2);
        var ahoraDateMonth = (ahora.getMonth() + 1).toString();
        var ahoraDateDay = ahora.getDate().toString();
        var ahoraDate = ahora.getFullYear().toString() + ahoraDateMonth.padStart(2, "0") + ahoraDateDay.padStart(2, "0");
        //console.log("ahoraDate: " + ahoraDate);

        if (dateOfExpiryAux2 <= ahoraDate) {
            e.isValid = false;
            e.errorText = "La Fecha de Caducidad debe ser mayor que la Fecha Actual";
            UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
            UpdateTabImage(e, "tabProvider");

        }
    }
    


}


// ProviderPaymentTermMethod
var isPredeterminedIniAux = false;
var isActiveIniAux = false;

// ProviderPaymentMethod

var id_paymentMethodIniAux = 0;

function OnProviderPaymentMethodPaymentMethodComboValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else {
        var data = {
            id_companyNew: id_companyPM.GetValue(),
            id_divisionNew: id_divisionPM.GetValue(),
            id_branchOfficeNew: id_branchOfficePM.GetValue(),
            id_paymentMethodNew: id_paymentMethod.GetValue(),
            isPredeterminedNew: isPredeterminedPM.GetValue(),
            onlyBecauseIsPredetermined: false
        };
        if (data.id_companyNew != id_companyIniAux || data.id_divisionNew != id_divisionIniAux ||
            data.id_branchOfficeNew != id_branchOfficeIniAux || data.id_paymentMethodNew != id_paymentMethodIniAux ||
            data.isPredeterminedNew != isPredeterminedIniAux) {
            data.onlyBecauseIsPredetermined = (data.id_companyNew == id_companyIniAux && data.id_divisionNew == id_divisionIniAux &&
                                        data.id_branchOfficeNew == id_branchOfficeIniAux && data.id_paymentMethodNew == id_paymentMethodIniAux);
            $.ajax({
                url: "Person/ItsRepeatedPaymentMethod",
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
                            UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
                            UpdateTabImage(e, "tabProvider");
                        } else {
                            id_companyIniAux = 0;
                            id_divisionIniAux = 0;
                            id_branchOfficeIniAux = 0;
                            id_paymentMethodIniAux = 0;
                            isPredeterminedIniAux = false;
                            isActiveIniAux = false;
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

function ProviderPaymentMethodCompanyCombo_Init(s, e) {

    id_companyIniAux = s.GetValue();
    id_divisionIniAux = id_divisionPM.GetValue();
    id_branchOfficeIniAux = id_branchOfficePM.GetValue();
    id_paymentMethodIniAux = id_paymentMethod.GetValue();
    isPredeterminedIniAux = isPredeterminedPM.GetValue();
    isActiveIniAux = isActive.GetValue();

    var data = {
        id_company: s.GetValue(),
        id_division: id_divisionPM.GetValue(),
        id_branchOffice: id_branchOfficePM.GetValue(),
        id_paymentMethod: id_paymentMethod.GetValue()
    };

    $.ajax({
        url: "Person/InitProviderPaymentMethodCompanyCombo",
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
            arrayFieldStr.push("code");
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_companyPM, result.companies, arrayFieldStr);

            UpdateDetailObjects(id_divisionPM, result.divisions, arrayFieldStr);

            UpdateDetailObjects(id_branchOfficePM, result.branchOffices, arrayFieldStr);

            //id_paymentMethod
            UpdateDetailObjects(id_paymentMethod, result.paymentMethods, arrayFieldStr);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function ProviderPaymentMethodCompanyCombo_SelectedIndexChanged(s, e) {

    id_divisionPM.ClearItems();
    id_branchOfficePM.ClearItems();
    id_paymentMethod.ClearItems();

    $.ajax({
        url: "Person/ProviderPaymentMethodCompanyDetailData",
        type: "post",
        data: { id_company: s.GetValue() },
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
            if (result !== null) {
                var arrayFieldStr = [];
                arrayFieldStr.push("code");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_divisionPM, result.divisions, arrayFieldStr);
                UpdateDetailObjects(id_paymentMethod, result.paymentMethods, arrayFieldStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function OnshippingTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    }
}

function OnFishingSiteValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabShrimpProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    }
}

function OnFishingZoneValidation(s, e) {


}


function FishingZone_Init(s, e)
{    
    FishingZone_SelectedIndexChanged(s, e);   
}

  
function FishingZone_SelectedIndexChanged(s, e)
{
    var _id_FishingSite = ASPxClientControl.GetControlCollection().GetByName("id_FishingSite").GetValue();
    id_FishingSite.ClearItems();
     
    var id = s.GetValue();

    $.ajax({
        url: "FishingZone/FishingZoneSiteData",
        type: "post",
        data: { id_fishingZone: id },
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
                var arrayFieldStr = [];
                arrayFieldStr.push("id");
                arrayFieldStr.push("name"); 
                UpdateDetailObjects(id_FishingSite, result._fishingSite, arrayFieldStr);
             
                var  isSameValue =  findJsonData(result._fishingSite,"id",_id_FishingSite);
                
                if(isSameValue)
                {
                        ASPxClientControl.GetControlCollection().GetByName("id_FishingSite").SetValue(_id_FishingSite);
                }
                
            }
        },
        complete: function () {
            hideLoading();
        }
    });
    
}



// ProviderPaymentTerm
var id_paymentTermIniAux = 0;


function OnProviderControlComboValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    }
}


function OnProviderPaymentTermPaymentTermComboValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
        UpdateTabImage(e, "tabProvider");
    } else {
        var data = {
            id_companyNew: id_company.GetValue(),
            id_divisionNew: id_division.GetValue(),
            id_branchOfficeNew: id_branchOffice.GetValue(),
            id_paymentTermNew: id_paymentTerm.GetValue(),
            isPredeterminedNew: isPredetermined.GetValue(),
            onlyBecauseIsPredetermined: false
        };
        if (data.id_companyNew != id_companyIniAux || data.id_divisionNew != id_divisionIniAux ||
            data.id_branchOfficeNew != id_branchOfficeIniAux || data.id_paymentTermNew != id_paymentTermIniAux ||
            data.isPredeterminedNew != isPredeterminedIniAux) {
            data.onlyBecauseIsPredetermined = (data.id_companyNew == id_companyIniAux && data.id_divisionNew == id_divisionIniAux &&
                                        data.id_branchOfficeNew == id_branchOfficeIniAux && data.id_paymentTermNew == id_paymentTermIniAux);
            $.ajax({
                url: "Person/ItsRepeatedPaymentTerm",
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
                            UpdateTabControlImage(e, "tabSpecificDatesProvider", tabControlProvider);
                            UpdateTabImage(e, "tabProvider");
                        } else {
                            id_companyIniAux = 0;
                            id_divisionIniAux = 0;
                            id_branchOfficeIniAux = 0;
                            id_paymentTermIniAux = 0;
                            isPredeterminedIniAux = false;
                            isActiveIniAux = false;
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


function ProviderPaymentTerm_OnBeginCallback(s, e) {
    e.customArgs['id_person'] = $("#id_person").val();
}

function ProductionShrimpUnitProvider_OnBeginCallback(s, e) {
    e.customArgs['id_person'] = $("#id_person").val();
}

function ProviderPaymentTermCompanyCombo_Init(s, e) {

    id_companyIniAux = s.GetValue();
    id_divisionIniAux = id_division.GetValue();
    id_branchOfficeIniAux = id_branchOffice.GetValue();
    id_paymentTermIniAux = id_paymentTerm.GetValue();
    isPredeterminedIniAux = isPredetermined.GetValue();
    isActiveIniAux = isActive.GetValue();

    var data = {
        id_company: s.GetValue(),
        id_division: id_division.GetValue(),
        id_branchOffice: id_branchOffice.GetValue(),
        id_paymentTerm: id_paymentTerm.GetValue()
    };

    $.ajax({
        url: "Person/InitProviderPaymentTermCompanyCombo",
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
            arrayFieldStr.push("code");
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_company, result.companies, arrayFieldStr);

            //id_division
            //arrayFieldStr = [];
            //arrayFieldStr.push("code");
            //arrayFieldStr.push("name");
            UpdateDetailObjects(id_division, result.divisions, arrayFieldStr);

            //id_branchOffice
            //arrayFieldStr = [];
            //arrayFieldStr.push("name");
            UpdateDetailObjects(id_branchOffice, result.branchOffices, arrayFieldStr);

            //id_paymentTerm
            UpdateDetailObjects(id_paymentTerm, result.paymentTerms, arrayFieldStr);
        },
        complete: function () {
            //hideLoading();
        }
    });


}


function ProviderPaymentTermDivisionCombo_SelectedIndexChanged(s, e) {
    
    id_branchOffice.ClearItems();

    $.ajax({
        url: "Person/ProviderPaymentTermDivisionDetailData",
        type: "post",
        data: { id_company: id_company.GetValue(), id_division: s.GetValue() },
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
            if (result !== null) {
                var arrayFieldStr = [];
                arrayFieldStr.push("code");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_branchOffice, result.branchOffices, arrayFieldStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function ProviderPaymentMethodDivisionCombo_SelectedIndexChanged(s, e) {
    id_branchOfficePM.ClearItems();

    $.ajax({
        url: "Person/ProviderPaymentMethodDivisionDetailData",
        type: "post",
        data: { id_company : id_companyPM.GetValue(), id_division: s.GetValue() },
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
            if (result !== null) {
                var arrayFieldStr = [];
                arrayFieldStr.push("code");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_branchOfficePM, result.branchOffices, arrayFieldStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function ProviderPaymentTermCompanyCombo_SelectedIndexChanged(s, e) {

    id_division.ClearItems();
    id_branchOffice.ClearItems();
    id_paymentTerm.ClearItems();

    $.ajax({
        url: "Person/ProviderPaymentTermCompanyDetailData",
        type: "post",
        data: { id_company: s.GetValue() },
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
            if (result !== null) {
                var arrayFieldStr = [];
                arrayFieldStr.push("code");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_division, result.divisions, arrayFieldStr);
                UpdateDetailObjects(id_paymentTerm, result.paymentTerms, arrayFieldStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

//ProviderGeneralData

function ComboOrigin_Init(s, e) {

    var data = {
        id_origin: s.GetValue(),
        id_country: id_country.GetValue(),
        id_city: id_city.GetValue(),
        id_stateOfContry: id_stateOfContry.GetValue()
    };

    $.ajax({
        url: "Person/InitComboOrigin",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_metricUnit.SetValue(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //id_country
            var arrayFieldStr = [];
            arrayFieldStr.push("code");
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_country, result.countries, arrayFieldStr);

            //id_city
            arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_city, result.cities, arrayFieldStr);

            //id_stateOfContry
            arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_stateOfContry, result.stateOfContries, arrayFieldStr);
        },
        complete: function () {
            //hideLoading();
        }
    });

}

function ComboOrigin_SelectedIndexChanged(s, e) {

    id_country.ClearItems();
    id_city.ClearItems();
    id_stateOfContry.ClearItems();

    $.ajax({
        url: "Person/OriginDetailData",
        type: "post",
        data: { id_origin: s.GetValue()},
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
            if (result !== null) {
                var arrayFieldStr = [];
                arrayFieldStr.push("code");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_country, result.countries, arrayFieldStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function ComboContry_SelectedIndexChanged(s, e) {

    id_city.ClearItems();
    id_stateOfContry.ClearItems();

    $.ajax({
        url: "Person/CountryDetailData",
        type: "post",
        data: { id_country: s.GetValue() },
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            if (result !== null) {
                //id_city
                arrayFieldStr = [];
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_city, result.cities, arrayFieldStr);

                //id_stateOfContry
                arrayFieldStr = [];
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_stateOfContry, result.stateOfContries, arrayFieldStr);

                
                //codeCountry
                if (result.codeCountry == codeCountrySystem.GetValue()) {
                    id_stateOfContry.SetVisible(false);
                    id_stateOfContryLabel.SetVisible(false);
                } else {
                    id_stateOfContry.SetVisible(true);
                    id_stateOfContryLabel.SetVisible(true);
                }
            }
        },
        complete: function () {
        }
    });
}

//Rise
function ComboCategoryRise_SelectedIndexChanged(s, e) {
    GetInvoiceAmountRise({
        id_categoryRise: s.GetValue(),
        id_activityRise: id_activityRise.GetValue()
    });
}

function ComboActivityRise_SelectedIndexChanged(s, e) {
    GetInvoiceAmountRise({
        id_categoryRise: id_categoryRise.GetValue(),
        id_activityRise: s.GetValue()
    });
}

function GetInvoiceAmountRise(data) {

    $.ajax({
        url: "Person/GetInvoiceAmountRise",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            invoiceAmountRise.SetValue(0);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                invoiceAmountRise.SetValue(result.invoiceAmountRise);
            } else {
                invoiceAmountRise.SetValue(0);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

//Additional Data

function ComboTypeBoxCardAndBankAD_Init(s, e) {

    var data = {
        id_typeBoxCardAndBankAD: s.GetValue(),
        id_boxCardAndBankAD: id_boxCardAndBankAD.GetValue()
    };

    $.ajax({
        url: "Person/InitComboTypeBoxCardAndBankAD",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_metricUnit.SetValue(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            //id_boxCardAndBankAD
            var arrayFieldStr = [];
            arrayFieldStr.push("code");
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_boxCardAndBankAD, result.boxCardAndBankADs, arrayFieldStr);

        },
        complete: function () {
            //hideLoading();
        }
    });

}

function ComboTypeBoxCardAndBankAD_SelectedIndexChanged(s, e) {

    id_boxCardAndBankAD.ClearItems();

    $.ajax({
        url: "Person/TypeBoxCardAndBankADDetailData",
        type: "post",
        data: { id_typeBoxCardAndBankAD: s.GetValue() },
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
                //id_boxCardAndBankAD
                var arrayFieldStr = [];
                arrayFieldStr.push("code");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_boxCardAndBankAD, result.boxCardAndBankADs, arrayFieldStr);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

// Provider Type changed
function ComboProviderType_SelectedIndexChanged(s, e) {
    var id_providerType = s.GetValue();
    console.log(id_providerType);
    if (id_providerType != 0 && id_providerType != null && id_providerType !== undefined) {
        $.ajax({
            url: "Person/ProviderTypeWhatProviderIs",
            type: "post",
            data: { id_providerType: s.GetValue() },
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforedSend: function () {
                showLoading();
            },
            success: function (result) {
                debugger;
                if (result.isShrimpProvider == "SI") {
                    //$("#detailShrimpProvider").show();
                    document.getElementById("isProviderShrimpBit").value = "SI";
                    tabControlProvider.GetTabByName("tabShrimpProvider").SetVisible(true);
                } else {
                    //$("#detailShrimpProvider").hide();
                    document.getElementById("isProviderShrimpBit").value = "NO";
                    tabControlProvider.GetTabByName("tabShrimpProvider").SetVisible(false);
                }
                if (result.isTransportist == "SI") {
                    document.getElementById("isProviderTransportistBit").value = "SI";
                    tabControlProvider.GetTabByName("tabTransportist").SetVisible(true);
                } else {
                    document.getElementById("isProviderTransportistBit").value = "NO";
                    tabControlProvider.GetTabByName("tabTransportist").SetVisible(false);
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

// Checked Changed

function ShowDetailElectronicPayment(s, e) {
    if (s.GetChecked()) {
        $("#detailElectronicPayment").show();
    } else {
        $("#detailElectronicPayment").hide();
    }
}

function ShowDetailRise(s, e) {
    if (s.GetChecked()) {
        $("#detailRise").show();
    } else {
        $("#detailRise").hide();
    }
}

//
function ComboIdentificationType_SelectedIndexChanged(s, e) {
    //OnIdentificationNumberValidationGeneral()
    identification_number.Validate();
    //var resultTmp = OnIdentificationNumberValidationGeneral(identification_number.GetText());
    //if (resultTmp != undefined){
    //    identification_number.isValid = resultTmp.isValid;
    //    identification_number.errorText = resultTmp.errorTextMessage;
        
    //}
        

    
    //OnIdentificationNumberValidation(identification_number,identification_number.)
    var id_provider = $("#id_provider").val();
    if (id_provider != 0 && id_provider != null && id_provider !== undefined) {
        $.ajax({
            url: "Person/PersonsGetCodSRIIdentificationType",
            type: "post",
            data: { id_identificationType: s.GetValue() },
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result.codSRI_IdentificationType == "06") { // Es el codigo del SRI del tipo de identificación Pasaporte
                    $("#detailPassportImportData").show();
                } else {
                    $("#detailPassportImportData").hide();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
    
}



function AddValidation(grid, name) {
    if (grid.editorIDList.indexOf(name) < 0)
        grid.editorIDList.push(name);
}

function RemoveValidation(grid, name) {
    var index = grid.editorIDList.indexOf(name);
    if (index > 0) {
        grid.editorIDList.splice(index, 1);
    }
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

function ClearValidators() {
    RemoveValidation(gvPerson, "Rol");

    // PROVAIDER
    RemoveValidation(gvPerson, "id_pymentMethod");
    RemoveValidation(gvPerson, "id_pymentMean");

    // EMEPLOYEE
    RemoveValidation(gvPerson, "id_department");
}

// ROL CHANGE
function RolTokenBox_TokensChanged(s, e) {
    var values = s.GetValue();
    
    var roles = values.split(",");


    var isProvider = (roles.indexOf("1") !== -1);
    
   
    var isEmployee = (roles.indexOf("2") !== -1);


    
    var isCustomerExterior = (roles.indexOf("6") !== -1);


    var isProviderShrimp = false;
    var isProviderTransportist = false;

    var id_providerType = s.GetValue();
    console.log(id_providerType);
    if (id_providerType != 0 && id_providerType != null && id_providerType !== undefined) {
        $.ajax({
            url: "Person/ProviderTypeWhatProviderIs",
            type: "post",
            data: { id_providerType: s.GetValue() },
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforedSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result.isShrimpProvider == "SI") {
                    isProviderShrimp = true;
                } else {
                    isProviderShrimp = false;
                }
                if (result.isTransportist == "SI") {
                    isProviderTransportist = true;
                } else {
                    isProviderTransportist = false;
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
    debugger;
    tabControl.GetTabByName("tabProvider").SetVisible(isProvider);
    tabControl.GetTabByName("tabEmployee").SetVisible(isEmployee);
    
    tabControlProvider.GetTabByName("tabGeneralDataProvider").SetVisible(isProvider);
    tabControlProvider.GetTabByName("tabSpecificDatesProvider").SetVisible(isProvider);
    tabControlProvider.GetTabByName("tabRelatedInformationProvider").SetVisible(isProvider);

    
}

function AddNewItem(s, e) {
    gvPerson.AddNewRow();
}

function RemoveItems(s, e) {
    gvPerson.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];

        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            $.ajax({
                url: "Person/DeleteSelectedPersons",
                type: "post",
                data: { ids: selectedRows },
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
                    gvPerson.PerformCallback();
                    gvPerson.UnselectRows();
                    hideLoading();
                }
            });
        });
    });
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
        });
    }
}

function RefreshGrid(s, e) {
    gvPerson.PerformCallback();
}

function Print(s, e) {

}

//SELECTION

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvPerson.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvPerson.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvPerson.GetSelectedRowCount() > 0 && gvPerson.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvPerson.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvPerson.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvPerson.cpFilteredRowCountWithoutPage + gvPerson.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvPerson.SelectRows();
}

function UnselectAllRows() {
    gvPerson.UnselectRows();
}

function ValidateTabDetailsProvider(gv, validDetailsProvider, validProvider, tabDetailsProvider) {
    if (gv.IsEditing()) {//gv.cpRowsCount === 0 ||
        UpdateTabControlImage({ isValid: false }, tabDetailsProvider, tabControlProvider);
        UpdateTabImage({ isValid: false }, "tabProvider");
        return false;
    } else {
        if (validDetailsProvider) {
            UpdateTabControlImage({ isValid: true }, tabDetailsProvider, tabControlProvider);
        }
        if (validProvider) {
            UpdateTabImage({ isValid: true }, "tabProvider");
        }
        return true;
    }
}

function ValidateTabSpecificDatesProvider(gv, validSpecificDatesProvider, validProvider) {
    return ValidateTabDetailsProvider(gv, validSpecificDatesProvider, validProvider, "tabSpecificDatesProvider");
}

function ValidateTabRelatedInformationProvider(gv, validRelatedInformationProvider, validProvider) {
    return ValidateTabDetailsProvider(gv, validRelatedInformationProvider, validProvider, "tabRelatedInformationProvider");
}

function ButtonUpdate_Click(s, e) {


    //var r = confirm("Press a button!");


    var valid = true;
    var id_PersonTmp = 0;

    var validGeneralDataPersonTable = ASPxClientEdit.ValidateEditorsInContainerById("generalDataPersonTable", null, true);

    if (!validGeneralDataPersonTable) {
        valid = false;
    } 

    var validProvider = true;
    var validSpecificDatesProvider = true;
    var validRelatedInformationProvider = true;

    var values = id_roles.GetValue();

    var roles = values.split(",");

    var isProvider = (roles.indexOf("1") !== -1);

    

    if (isProvider) {
        var validTabGeneralDataProviderTable = ASPxClientEdit.ValidateEditorsInContainerById("tabGeneralDataProviderTable", null, true);

        if (validTabGeneralDataProviderTable) {
            UpdateTabControlImage({ isValid: true }, "tabGeneralDataProvider", tabControlProvider);
            UpdateTabImage({ isValid: true }, "tabProvider");
        } else {
            UpdateTabControlImage({ isValid: false }, "tabGeneralDataProvider", tabControlProvider);
            UpdateTabImage({ isValid: false }, "tabProvider");
            valid = false;
            validProvider = false;
        }
        //ValidateSpecificDatesProvider
        //ProviderPaymentTerms
        validSpecificDatesProvider = (validSpecificDatesProvider && ValidateTabSpecificDatesProvider(ProviderPaymentTerms, validSpecificDatesProvider, validProvider));
        validProvider = (validProvider && validSpecificDatesProvider);
        valid = (valid && validProvider);

        //ProviderPaymentMethods
        validSpecificDatesProvider = (validSpecificDatesProvider && ValidateTabSpecificDatesProvider(ProviderPaymentMethods, validSpecificDatesProvider, validProvider));
        validProvider = (validProvider && validSpecificDatesProvider);
        valid = (valid && validProvider);

        //ProviderSeriesForDocumentss
        validSpecificDatesProvider = (validSpecificDatesProvider && ValidateTabSpecificDatesProvider(ProviderSeriesForDocumentss, validSpecificDatesProvider, validProvider));
        validProvider = (validProvider && validSpecificDatesProvider);
        valid = (valid && validProvider);

        //ProviderRetentions
        validSpecificDatesProvider = (validSpecificDatesProvider && ValidateTabSpecificDatesProvider(ProviderRetentions, validSpecificDatesProvider, validProvider));
        validProvider = (validProvider && validSpecificDatesProvider);
        valid = (valid && validProvider);

        //ProviderPersonAuthorizedToPayTheBills
        validSpecificDatesProvider = (validSpecificDatesProvider && ValidateTabSpecificDatesProvider(ProviderPersonAuthorizedToPayTheBills, validSpecificDatesProvider, validProvider));
        validProvider = (validProvider && validSpecificDatesProvider);
        valid = (valid && validProvider);

        validRelatedInformationProvider = (validRelatedInformationProvider && ValidateTabRelatedInformationProvider(ProviderRelatedCompanies, validRelatedInformationProvider, validProvider));
        validProvider = (validProvider && validRelatedInformationProvider);
        valid = (valid && validProvider);

        //ProviderAccountingAccounts
        validRelatedInformationProvider = (validRelatedInformationProvider && ValidateTabRelatedInformationProvider(ProviderAccountingAccounts, validRelatedInformationProvider, validProvider));
        validProvider = (validProvider && validRelatedInformationProvider);
        valid = (valid && validProvider);



        //ProviderMailByComDivBras
        validRelatedInformationProvider = (validRelatedInformationProvider && ValidateTabRelatedInformationProvider(ProviderMailByComDivBras, validRelatedInformationProvider, validProvider));
        validProvider = (validProvider && validRelatedInformationProvider);
        valid = (valid && validProvider);

    }


    var isEmployee = (roles.indexOf("2") !== -1);

    if (isEmployee) {
        var validTabEmployeeTable = ASPxClientEdit.ValidateEditorsInContainerById("tabEmployeeTable", null, true);

        if (validTabEmployeeTable) {
            UpdateTabImage({ isValid: true }, "tabEmployee");
        } else {
            UpdateTabImage({ isValid: false }, "tabEmployee");
            valid = false;
        }
    }

    
    
    if (valid) {
        var person = "id=" + $("#id_person").val() + "&" + $("#formEditPerson").serialize();
        
        var url = ($("#id_person").val() === "0") ? "Person/PersonsPartialAddNew"
                                                  : "Person/PersonsPartialUpdate";

        $.ajax({
            url: url,
            type: "post",
            data: person,
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                 
                if (result != null) {
                    id_PersonTmp = result.idPerson;
                    if (id_PersonTmp != 0 && id_PersonTmp != undefined && id_PersonTmp != null) {
                        if ( isProvider) {
                            $.ajax({
                                url: "Person/MigrarIndividual",
                                type: "post",
                                data: "id=" + id_PersonTmp,
                                async: false,
                                cache: false,
                                error: function (error) {
                                    console.log(error);
                                },
                                beforedSend: function () {
                                },
                                success: function (result) {
                                    if (result != null) {
                                        if (result.Data != null) {
                                            if (result.Data.respuestaProveedor != null){
                                                console.log(result.Data.respuestaProveedor);
                                            }
                                            if (result.Data.respuestaCliente != null) {
                                                console.log(result.Data.respuestaCliente);
                                            }
                                        }
                                    }
                                },
                                complete: function () {
                                }
                            });
                        }
                    }
                }
                gvPerson.CancelEdit();
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function BtnCancel_Click(s, e) {
    gvPerson.CancelEdit();
}

function init() {
    
}

$(function () {
    init();
});




