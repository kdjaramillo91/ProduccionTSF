
function OnEmissionDateValidation(s, e) {
    OnEmissionDateDocumentValidation(e, emissionDate, "purchaseOrder");
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

function onValidateRequiredField(s, e) {
    var value = s.GetValue();
    if (value == null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else {
        e.isValid = true;
        e.errorText = "";
	}
}