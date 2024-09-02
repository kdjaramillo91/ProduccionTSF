
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



function OnPersonRequestingValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    }
}

function OnEmissionDateValidation(s, e) {
    OnEmissionDateDocumentValidation(e, emissionDate, "request");
}

