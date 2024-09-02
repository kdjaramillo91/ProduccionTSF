
// VALIDATIONS
function OnNumeroContenedoresValidation(s, e) {

    if (gvInvoiceCommercialEditFormContainer.IsEditing()) {
        e.isValid = false;
        e.errorText = "Se está editando un número de contenedor";
    } else {//numeroContenedores
        if (gvInvoiceCommercialEditFormContainer.cpRowsCount != e.value) {
            e.isValid = false;
            e.errorText = "Deben adicionarse solo " + e.value + " número de contenedor en el detalle de Información Contenedores";
        } 
    }
}


function AddNewContainer(s, e) {
    if (gvInvoiceCommercialEditFormContainer !== null && gvInvoiceCommercialEditFormContainer !== undefined) {
        gvInvoiceCommercialEditFormContainer.AddNewRow();
    }
}

function RemoveContainer(s, e) {
}

function RefreshContainer(s, e) {
    if (gvInvoiceCommercialEditFormContainer !== null && gvInvoiceCommercialEditFormContainer !== undefined) {
        gvInvoiceCommercialEditFormContainer.UnselectRows();
        gvInvoiceCommercialEditFormContainer.PerformCallback();
    }
}

function OnNumberContainerValidation(s, e) {

    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } 
}
