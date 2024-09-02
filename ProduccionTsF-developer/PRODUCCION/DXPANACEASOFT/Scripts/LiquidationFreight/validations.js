
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
   // OnEmissionDateDocumentValidation(e, emissionDate, "remissionGuide");
}

// REMISSION GUIDE HEADER VALIDATIONS

function OnpriceadjustmentValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }else if(parseInt(e.value <0))
    {
        e.isValid = false;
        e.errorText = "El Ajuste debe ser mayor o Igual que 0";
    }
}

function OnpriceextensionValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (parseInt(e.value < 0)) {
        e.isValid = false;
        e.errorText = "La Extncion debe ser mayor o Igual que 0";
    }
}


function OnpricedaysValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if (parseInt(e.value < 0)) {
        e.isValid = false;
        e.errorText = "El valor por Dia debe ser mayor o Igual que 0";
    }
}


function OnTransportProviderValidation(s, e) {
    if (e.value === null && $("#val_liq_any_prov").val() === 'S' ) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio, Seleccione un Proveedor de Transporte";
    }
}

function OnTransportProviderBeginCallback(s, e) {
    e.customArgs["idCompany"] = s.cpIdCompany;
    e.customArgs["idPerson"] = s.GetValue();
    e.customArgs["tipo"] = "Proveedor";
}

function OnTransportProviderValueChanged(s, e)
{
    $("#id_providertransport").val(s.GetValue());    
}


    //UpdateTabImage(e, "tabExport");


