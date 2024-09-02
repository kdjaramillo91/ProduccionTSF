
function OnEmissionDateValidation(s, e) {
    OnEmissionDateDocumentValidation(e, emissionDate, "purchaseOrder");
}
function OnDocumentResaignacionSelectedIndexChanged(s, e) {
    var _selectItem = s.GetSelectedItem();
    if (_selectItem == undefined || _selectItem == null) {
        emissionDate.SetText(null);
        accessKey.SetText(null);
        return;
    }

    var _emissionDate = _selectItem.GetColumnText("emissionDate");
    var _accessKey = _selectItem.GetColumnText("accessKey");

    emissionDate.SetText(_emissionDate);
    accessKey.SetText(_accessKey);

}
function OnNumberValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "El número de documento es obligatorio";
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

