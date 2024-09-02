
// VALIDATIONS
function OnEmissionDateValidation(s, e) {
    //console.log("emissionDate: " + emissionDate);
    //console.log("emissionDate.GetDate(): " + emissionDate.GetDate());
    //console.log("emissionDate.GetValue(): " + emissionDate.GetValue());
    //console.log("emissionDate.GetDay(): " + emissionDate.GetDay());
    //console.log("emissionDate.GetMo(): " + emissionDate.GetValue());
    //console.log("emissionDate.GetValue(): " + emissionDate.GetValue());
    //console.log("emissionDate.GetValue(): " + emissionDate.GetValue());
    //console.log("emissionDate.GetValue(): " + emissionDate.GetValue());
    OnEmissionDateDocumentValidation(e, emissionDate, "inventoryMove");

    //UpdateTabImage(e, "tabDocument");
}
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

// ENTRY VALIDATIONS
function OnWarehouseEntryValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnLocationEntryValidation(s, e) {
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


function OnReceiverValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// EXIT VALIDATIONS

function OnWarehouseExitValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnLocationExitValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDispacherExitValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}