//ForeignCustomer

function ComboBox_Init(s, e) {

    var valueAux = s.GetValue();
    if (valueAux == 0) {
        s.SetValue(null);
    }
}

var rowCountRequiredAux = 0;

function CountryComboBox_SelectedIndexChanged(s, e) {
    id_cityForeignCustomerIdentification.ClearItems();
    id_Country_IdentificationType.ClearItems();

    UpdateRowCountRequired();
    UpdateCodeCountry_IdentificationType();

    $.ajax({
        url: "ForeignCustomer/CountryDetailData",
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
                arrayFieldStr.push("code");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_cityForeignCustomerIdentification, result.cities, arrayFieldStr);
            }
        },
        complete: function () {
        }
    });
}


function OnGridViewForeignCustomerIdentificationInit(s, e) {

    btnRemoveDetail.SetVisible(false);
    UpdateRowCountRequired();
}

function UpdateRowCountRequired() {
    // 
    var a = id_countryForeignCustomerIdentification.GetValue()
    rowCountRequiredAux = 0;

    $.ajax({
        url: "ForeignCustomer/UpdateRowCountRequired",
        type: "post",
        data: {
            id_countryCurrent: id_countryForeignCustomerIdentification.GetValue()
        },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
            //hideLoading();
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                //$("#rowCountRequired").val();
                rowCountRequiredAux = result.rowCountRequired;
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function OnGridViewForeignCustomerIdentificationBeginCallback(s, e) {

    e.customArgs['id_countryForeignCustomer'] = id_countryForeignCustomerIdentification.GetValue();
    //$("#id_person").val();
}

var codeCountryAux = "";
var codeIdentificationTypeAux = "";
var id_Country_IdentificationTypeCurrent = null;
var id_Country_CityCurrent = null;



function Country_IdentificationTypeCombo_Init(s, e) {
    id_Country_IdentificationTypeCurrent = s.GetValue();
    s.PerformCallback();
    
    //$("#id_person").val();
}
function Country_CityCombo_Init(s, e) {
    // 
    id_Country_CityCurrent = s.GetValue();
    s.PerformCallback();

}
function Country_CityCombo_Validation(s, e) {
    var value = s.GetValue();
    if (value == null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
	}
}
function Country_City_SelectedIndexChanged(s, e) {

    id_Country_CityCurrent = s.GetValue();
    UpdateCityCountry();
    //$("#id_person").val();
}
function Country_IdentificationTypeCombo_SelectedIndexChanged(s, e) {

    id_Country_IdentificationTypeCurrent = s.GetValue();
    UpdateCodeCountry_IdentificationType();
    //$("#id_person").val();
}

function UpdateCodeCountry_IdentificationType() {

    codeCountryAux = "";
    codeIdentificationTypeAux = "";

    $.ajax({
        url: "ForeignCustomer/UpdateCodeCountry_IdentificationType",
        type: "post",
        data: {
            id_Country_IdentificationTypeCurrent: id_countryForeignCustomerIdentification.GetValue()
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
            if (result !== null) {
                codeCountryAux = result.codeCountry;
                codeIdentificationTypeAux = result.codeIdentificationType;
                arrayFieldStr = [];
                arrayFieldStr.push("code");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_Country_IdentificationType, result.identificationTypes, arrayFieldStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function UpdateCityCountry() {
    // 
    codeCountryAux = "";
    codeCityeAux = "";

    $.ajax({
        url: "ForeignCustomer/UpdateCityCountry",
        type: "post",
        data: {
            id_Country_CityCurrent: id_countryForeignCustomerIdentification.GetValue()
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
            if (result !== null) {
                codeCountryAux = result.codeCountry;
 
                arrayFieldStr = [];
                arrayFieldStr.push("code");
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_cityForeignCustomerIdentification, result.cityTypes, arrayFieldStr);
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function CityCountry_BeginCallback(s, e) {
    // 
    e.customArgs['id_City'] = id_Country_CityCurrent;
    e.customArgs['id_countryForeignCustomer'] = id_countryForeignCustomerIdentification.GetValue();
    //$("#id_person").val();
}
function PersonCountry_IdentificationType_BeginCallback(s, e) {

    e.customArgs['id_Country_IdentificationType'] = id_Country_IdentificationTypeCurrent;
    e.customArgs['id_countryForeignCustomer'] = id_countryForeignCustomerIdentification.GetValue();
    //$("#id_person").val();
}
function CityCountry_EndCallback(s, e) {
    // 
    id_cityForeignCustomerIdentification.SetValue(id_Country_CityCurrent);
    UpdateCityCountry();
}

function PersonCountry_IdentificationType_EndCallback(s, e) {

    id_Country_IdentificationType.SetValue(id_Country_IdentificationTypeCurrent);
    UpdateCodeCountry_IdentificationType();
}

function OnCountry_IdentificationTypeComboValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } 
}

function OnNumberIdentificationForeignCustomerIdentificationComboValidation(s, e) {
    e.isValid = true;
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
    /*  //Hacer Validación
    var valueAux = s.GetValue();
    //codeIdentificationTypeAux = result.codeIdentificationType;
    var regExp = new RegExp("[^0-9]", "i");

    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else {
        if (codeIdentificationTypeAux == "EORI") {
            if (valueAux.length > 15) {
                e.isValid = false;
                e.errorText = "La Identificación debe tener máximo 15 caracteres";
                return;
                //return { isValid: false, errorText: "El ruc debe tener 13 dígitos" };
            }
            var length_codeCountryAux = codeCountryAux.length;
            var codePartAux = valueAux.substring(0, (length_codeCountryAux));
            if (codePartAux != codeCountryAux) {
                e.isValid = false;
                e.errorText = "La Identificación no es válida debe comenzar con " + codeCountryAux;
                return;
                //return { isValid: false, errorText: "El ruc debe tener 13 dígitos" };
            }
            var codePart2Aux = valueAux.substring(length_codeCountryAux);
            
            if (regExp.test(codePart2Aux)) {
                e.isValid = false;
                e.errorText = "La Identificación no es válida debe comenzar con " + codeCountryAux + " seguido de dígitos";
                return;
                //return { isValid: false, errorText: "Solo se admiten dígitos" };
            }
        }
        if (codeIdentificationTypeAux == "VAT") {
            if (codeCountryAux == "AT"){
                regExp = new RegExp("(AT)?U[0-9]{8}", "i");
            }
            if (codeCountryAux == "BE") {
                regExp = new RegExp("(BE)?0[0-9]{9}", "i");
            }
            if (codeCountryAux == "BG") {
                regExp = new RegExp("(BG)?[0-9]{9,10}", "i");
            }
            if (codeCountryAux == "CY") {
                regExp = new RegExp("(CY)?[0-9]{8}L", "i");
            }
            if (codeCountryAux == "CZ") {
                regExp = new RegExp("(CZ)?[0-9]{8,10}", "i");
            }
            if (codeCountryAux == "DE") {
                regExp = new RegExp("(DE)?[0-9]{9}", "i");
            }
            if (codeCountryAux == "DK") {
                regExp = new RegExp("(DK)?[0-9]{8}", "i");
            }
            if (codeCountryAux == "EE") {
                regExp = new RegExp("(EE)?[0-9]{9}", "i");
            }
            if (codeCountryAux == "EL" || codeCountryAux == "GR") {
                regExp = new RegExp("(EL|GR)?[0-9]{9}", "i");
            }
            if (codeCountryAux == "ES") {
                regExp = new RegExp("(ES)?[0-9A-Z][0-9]{7}[0-9A-Z]", "i");
            }
            if (codeCountryAux == "FI") {
                regExp = new RegExp("(FI)?[0-9]{8}", "i");
            }
            if (codeCountryAux == "FR") {
                regExp = new RegExp("(FR)?[0-9A-Z]{2}[0-9]{9}", "i");
            }
            if (codeCountryAux == "GB") {
                regExp = new RegExp("(GB)?([0-9]{9}([0-9]{3})?|[A-Z]{2}[0-9]{3})", "i");
            }
            if (codeCountryAux == "HU") {
                regExp = new RegExp("(HU)?[0-9]{8}", "i");
            }
            if (codeCountryAux == "IE") {
                regExp = new RegExp("(IE)?[0-9]S[0-9]{5}L", "i");
            }
            if (codeCountryAux == "IT") {
                regExp = new RegExp("(IT)?[0-9]{11}", "i");
            }
            if (codeCountryAux == "LT") {
                regExp = new RegExp("(LT)?([0-9]{9}|[0-9]{12})", "i");
            }
            if (codeCountryAux == "LU") {
                regExp = new RegExp("(LU)?[0-9]{8}", "i");
            }
            if (codeCountryAux == "LV") {
                regExp = new RegExp("(LV)?[0-9]{11}", "i");
            }
            if (codeCountryAux == "MT") {
                regExp = new RegExp("(MT)?[0-9]{8}", "i");
            }
            if (codeCountryAux == "NL") {
                regExp = new RegExp("(NL)?[0-9]{9}B[0-9]{2}", "i");
            }
            if (codeCountryAux == "PL") {
                regExp = new RegExp("(PL)?[0-9]{10}", "i");
            }
            if (codeCountryAux == "PT") {
                regExp = new RegExp("(PT)?[0-9]{9}", "i");
            }
            if (codeCountryAux == "RO") {
                regExp = new RegExp("(RO)?[0-9]{2,10}", "i");
            }
            if (codeCountryAux == "SE") {
                regExp = new RegExp("(SE)?[0-9]{12}", "i");
            }
            if (codeCountryAux == "SI") {
                regExp = new RegExp("(SI)?[0-9]{8}", "i");
            }
            if (codeCountryAux == "SK") {
                regExp = new RegExp("(SK)?[0-9]{10}", "i");
            }
            if (!regExp.test(valueAux)) {
                e.isValid = false;
                e.errorText = "La Identificación no es válida";
                return;
                //return { isValid: false, errorText: "Solo se admiten dígitos" };
            }
        }
        if (codeIdentificationTypeAux == "TAXID") {
            //regExp = new RegExp("[0-9]{4}-?[0-9]{4}-?[0-9]{4}-?[0-9]{4}", "i");

            //if (!regExp.test(valueAux)) {
            //    e.isValid = false;
            //    e.errorText = "La Identificación no es válida.";
            //    return;
            //    //return { isValid: false, errorText: "Solo se admiten dígitos" };
            //}
        }
        if (codeIdentificationTypeAux == "IN") {

            if (regExp.test(valueAux)) {
                e.isValid = false;
                e.errorText = "El Identificación no es válida.";
                return;
                //return { isValid: false, errorText: "Solo se admiten dígitos" };
            }
        }
    }
    */
}


function OnBtnUpdatePopupForeignCustomer_Click(s, e) {
    //// 
    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);

    var valid = true;
    var validFormLayoutEditForeignCustomer = ASPxClientEdit.ValidateEditorsInContainerById("formLayoutEditForeignCustomer", null, true);

    //if (!valid) {
    //    UpdateTabImage({ isValid: false }, "tabDocument");
    //}
    if (validFormLayoutEditForeignCustomer) {
        //UpdateTabImage({ isValid: true }, "tabDocument");
    } else {
        //UpdateTabImage({ isValid: false }, "tabDocument");
        valid = false;
    }

    console.log(valid);

    if (valid) {

        //var rowCountRequired = rowCountRequiredAux;
        var messageAux = "";
        if (gvForeignCustomerIdentification.IsEditing()) {
            //UpdateTabImage({ isValid: false }, "tabProductionLotCloseDetails");
            messageAux = "No se puede guardar debido a que se está editando un Tipo de Documento.";
            valid = false;
        } else {
            //UpdateTabImage({ isValid: true }, "tabProductionLotCloseDetails");
        }
        if (gvForeignCustomerIdentification.cpRowsCount != rowCountRequiredAux) {
            //UpdateTabImage({ isValid: false }, "tabProductionLotCloseDetails");
            /*messageAux = "No se puede guardar debido a que falta Tipo de Documento requridos para este País.";
            valid = false;*/
        } else {
            //UpdateTabImage({ isValid: true }, "tabProductionLotCloseDetails");
        }

        if (!valid){
            var msgErrorAux = ErrorMessage(messageAux);
            gridMessageErrorsDetail.SetText(msgErrorAux);
            $("#GridMessageErrorsDetail").show();
        }
    }

    if (valid) {
        var id = $("#id_person").val();

        var data = {
            id: id,
            //id_countryForeignCustomer: id_countryForeignCustomer.GetValue(),
            //id_invoiceLanguageForeignCustomer: id_invoiceLanguageForeignCustomer.GetValue(),
            //bankRefForeignCustomer: bankRefForeignCustomer.GetText(),
            id_PaymentTermForeignCustomer: 0,
            //emailInternoForeignCustomer: emailInternoForeignCustomer.GetValue(),
            //emailInternoCCForeignCustomer: emailInternoCCForeignCustomer.GetValue(),
            //phone1FCForeignCustomer: phone1FCForeignCustomer.GetValue(),
            //fax1FCForeignCustomer: fax1FCForeignCustomer.GetValue(),
            //id_cityForeignCustomer: id_cityForeignCustomer.GetValue(),
            IFFC: IFFC.GetValue(),
            RCFC: RCFC.GetValue(),
            patenteFC: patenteFC.GetValue(),
            CNSSFC: CNSSFC.GetValue(),
            ICEFC: ICEFC.GetValue()

        };//"id=" + id + "&" + $("#formLayoutEditForeignCustomer").serialize();

        var url = "ForeignCustomer/PersonForeignCustomerUpdate";

        $.ajax({
            url: url,
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

                if (result.message == "OK") {
                    //popupControlRolForeignCustomer.Hide();
                    $.fancybox.close();
                } else {
                    var msgErrorAux = ErrorMessage(result.message);
                    gridMessageErrorsDetail.SetText(msgErrorAux);
                    $("#GridMessageErrorsDetail").show();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
        
    }
}

function OnBtnCancelPopupForeignCustomer_Click(s, e) {
    $.fancybox.close();
}

// DETAILS ACTIONS

function AddNewDetail(s, e) {
    gvForeignCustomerIdentification.AddNewRow();
    //AddNew(s, e);
}

function RemoveDetail(s, e) {
   // Remove(s, e);
}

function RefreshDetail(s, e) {
    //Refresh(s, e);
    gvForeignCustomerIdentification.UnselectRows();
    gvForeignCustomerIdentification.PerformCallback();
}

function init() {
    
}

$(function () {
    init();
});



function OnEmailValidation(s, e) {
    if (e.value !== null)
    {
        var validation = validarEMAIL(e.value);

        if (!validation.isValid)
        {
            e.isValid = validation.isValid;
            e.errorText = validation.errorText;
        }
    }
}