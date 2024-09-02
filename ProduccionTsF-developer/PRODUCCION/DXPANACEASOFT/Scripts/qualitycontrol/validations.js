function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }
    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}


function OnQualityControlConfigurationValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnEmissionDateValidation(s, e) {
    OnEmissionDateDocumentValidation(e, documentEmissionDate, "qualityControl");
}

function OnAnalystValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// HEADER QUALITY CONTROl

// DETAILS ACTIONS QUALITY CONTROL

function OnWarehouseValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnWarehouseLocationValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnItemValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnLotValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnResultListValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnResultTextValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnResultNumValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCorrectingPersonValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDescriptionQualityCorrectiveActionValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

