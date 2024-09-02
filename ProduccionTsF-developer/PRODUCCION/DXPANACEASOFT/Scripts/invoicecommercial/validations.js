
function OnInvoiceCommercialEmissionDateValidation(s, e) {
    OnEmissionDateDocumentValidation(e, emissionDate, "invoiceCommercial");

    UpdateTabImage(e, "tabDocument");
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

//function OnInternalNumberValidation(s, e) {
//    if (e.value !== null && e.value.toString().length > 20) {
//        e.isValid = false;
//        e.errorText = "Máximo 20 caracteres";
//    }
//}

//function OnProductionUnitValidation(s, e) {
//    if(e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnProductionProcessValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnWarehouseValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}

//function OnWarehouseLocationValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}