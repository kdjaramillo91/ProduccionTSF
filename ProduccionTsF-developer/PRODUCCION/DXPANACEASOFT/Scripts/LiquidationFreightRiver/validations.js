
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




    //UpdateTabImage(e, "tabExport");


