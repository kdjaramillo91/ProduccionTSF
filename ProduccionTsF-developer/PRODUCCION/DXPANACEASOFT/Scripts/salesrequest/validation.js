
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

// SALE REQUEST VALIDATION

function OnCustomerValitation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
	UpdateTabImage(e, "tabRequest");
}

function OnPriceListValitation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
	UpdateTabImage(e, "tabRequest");
}

function OnPaymentMethodValitation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
	UpdateTabImage(e, "tabRequest");
}

function OnPaymentTermValitation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
	UpdateTabImage(e, "tabRequest");
}

function OnPortDestinationValitation(s, e) {
	if (e.value === null) {
		e.isValid = false;
		e.errorText = "Campo Obligatorio";
	}
	UpdateTabImage(e, "tabRequest");
}
function OnEmissionDateValidation(s, e) {
	OnEmissionDateDocumentValidation(e, emissionDate, "request");
}