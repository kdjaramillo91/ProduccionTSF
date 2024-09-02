// TAB IMAGES

function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }
    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}


// PRICELIST HEADER VALIDATIONS

function OnPriceListTypeValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }

    UpdateTabImage(e, "tabHeader");
}



// PRICELIST DETAILS VALIDATIONS

function ItemValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnPurchasePriceValidation(s, e) {
    if (e.value === null && isForPurchase.GetChecked()) {
        e.isValid = false;
        e.errorText = "CampoObligatorio";
    } else if (e.value !== null && e.value.toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
    } else if (e.value <= 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
    }
}

function OnSalePriceValidation(s, e) {
    if (e.value === null && isForSold.GetChecked()) {
        e.isValid = false;
        e.errorText = "CampoObligatorio";
    } else if (e.value !== null && e.value.toString().length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
    } else if (e.value <= 0) {
        e.isValid = false;
        e.errorText = "Valor Incorrecto";
    }
}