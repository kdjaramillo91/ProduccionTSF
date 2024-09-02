
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
    // 
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
function OnLocationValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnReasonValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio, Seleccione un Motivo de Inventario";
    }
}

function OnWarehouseLocationValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio, Seleccione una Ubicación";
    }
}

function OnReasonEntryValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio, Seleccione un Motivo de Inventario";
    }
}

function OnWarehouseValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnInvoiceValidation(s, e) {
    if (idWarehouse.GetValue() === null && ($("#mostrarOP").val() === true || $("#mostrarOP").val() === "true" || $("#mostrarOP").val() === "True")) {
        e.isValid = false;
        id_Invoice.SetValue(null);
        e.errorText = "La Bodega es obligatoria";
	}
    else if (e.value === null && ($("#mostrarOP").val() === true || $("#mostrarOP").val() === "true" || $("#mostrarOP").val() === "True")) {
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


function OnLocationEntryValidation(s, e) {
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

function OnReceiverEntryValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnCustomerValidation(s, e) {
     
    if (e.value === null && ($("#val_liq_any_prov").val() === true || $("#mostrarOP").val() === "true" || $("#mostrarOP").val() === "True"))
    {
        e.isValid = false;
        e.errorText = "Campo Obligatorio, Seleccione un Cliente";
    }

}

function OnCustomerBeginCallback(s, e) {
     
    e.customArgs["idCompany"] = s.cpIdCompany;
    e.customArgs["idPerson"] = s.GetValue();
    e.customArgs["tipo"] = "cliente";
    let mostarop = $("#mostrarOP").val();
    let valinvfact = $("#valInvFact").val();
    e.customArgs["viewInvoice"] = (mostarop === "true") && (valinvfact == "SI");
}

// GENERAL VALIDATIONS =>  REQUIRE INVENTORY REASON
function OnIsSelectInventoryReasonValidate(s, e)
{
    var codeDocumentType = $("#codeDocumentType").val();
    // 
    var tab = s;
    var inventopryReasonValue = id_inventoryReason.GetValue();
    if (e.tab.name == "tabDetails" && inventopryReasonValue === null) {
        setTimeout(function () {
            tabControl.SetActiveTab(tabControl.GetTab(1))
            var validMainTabInventoryMove = ASPxClientEdit.ValidateEditorsInContainerById("mainTabInventoryMove", null, true);
        }, 100);
    }
    if (codeDocumentType == "129") {
        var inventopryReasonEntryValue = id_inventoryReasonEntry.GetValue();
        if (inventopryReasonEntryValue === null) {
            setTimeout(function () {
                tabControl.SetActiveTab(tabControl.GetTab(1))
                var validMainTabInventoryMove = ASPxClientEdit.ValidateEditorsInContainerById("mainTabInventoryMove", null, true);
            }, 100);
        }
    }
}

function OnEmissionDateValueChanged(s, e) {
    if (gridViewMoveDetails.IsEditing()) {
        gridViewMoveDetails.PerformCallback();
	}
}

function OnInitPageControl(s, e) {
    var esIngresoTransferencia = tabControl.cpCodeDocumentType == "34";
    emissionDate.SetEnabled(!tabControl.cpExistenRegistros || esIngresoTransferencia);
}
