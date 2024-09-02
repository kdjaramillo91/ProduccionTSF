var id_warehouseAux = null;
var id_warehouseLocationAux = null;
var name_warehouseLocationAux = null;


//Materials

//DETAILS ACTIONS BUTTONS

function AddNewDetail(s, e) {
    //if (gv !== null && gv !== undefined) {
    //    gv.AddNewRow();
    //}
    gvReceptionDispatchMaterialsDetailEditForm.AddNewRow();
}

function RemoveDetail(s, e) {
}

function RefreshDetail(s, e) {
    //if (gv !== null && gv !== undefined) {
    //    gv.UnselectRows();
    //    gv.PerformCallback();
    //}
    gvReceptionDispatchMaterialsDetailEditForm.UnselectRows();
    gvReceptionDispatchMaterialsDetailEditForm.PerformCallback();
}

var errorMessageMaterials = "";
var runningValidationMaterials = false;

// VALIDATIONS

//function OnWarehouseMaterialsDetailValidation(s, e) {
//    //gridMessageErrorMaterialsDetail.SetText(result.Message);
//    errorMessage = "";

//    $("#GridMessageErrorMaterialsDetail").hide();

//    var arrivalDestinationQuantityAux = parseFloat(arrivalDestinationQuantity.GetValue())

//    if (arrivalDestinationQuantityAux > 0) {
//        if (s.GetValue() === null) {
//            e.isValid = false;
//            e.errorText = "Campo Obligatorio";
//            errorMessage = "- Bodega: Es obligatoria, por tener una cantidad recibida.";
//        }
//    }

//}

//function OnWarehouseLocationMaterialsDetailValidation(s, e) {

//    var arrivalDestinationQuantityAux = parseFloat(arrivalDestinationQuantity.GetValue())

//    if (arrivalDestinationQuantityAux > 0) {
//        if (s.GetValue() === null) {
//            e.isValid = false;
//            e.errorText = "Campo Obligatorio";
//            if (errorMessage == null || errorMessage == "") {
//                errorMessage = "- Ubicación: Es obligatoria, por tener una cantidad recibida.";
//            } else {
//                errorMessage += "</br>- Ubicación: Es obligatoria, por tener una cantidad recibida.";
//            }
//        }
//    }

//    if (!runningValidation) {
//        ValidateMaterialsDetail();
//    }
//}

function OnSendedDestinationQuantityValidation(s, e) {
    errorMessageMaterials = "";

    $("#GridMessageErrorMaterialsDetail").hide();

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Cantidad Enviada: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Cantidad Enviada: Es incorrecta.";
        }

    }
}

function OnsendedNetQuantityValidation(s, e) {
    //errorMessageMaterials = "";

    //$("#GridMessageErrorMaterialsDetail").hide();

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Neto Enviado: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Neto Enviado Incorrecto";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Neto Enviado: Es incorrecto.";
        }

    }

    if (!runningValidationMaterials) {
        ValidateMaterialsDetail();
    }
}

function OnAmountConsumedValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessageMaterials === null || errorMessageMaterials === "") {
            errorMessageMaterials = "- Cantidad Consumida: Es obligatoria.";
        } else {
            errorMessageMaterials += "</br>- Cantidad Consumida: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessageMaterials === null || errorMessageMaterials === "") {
            errorMessageMaterials = "- Cantidad Consumida: Es incorrecta.";
        } else {
            errorMessageMaterials += "</br>- Cantidad Consumida: Es incorrecta.";
        }
    } else {
        var sendedNetQuantityRemissionGuideDispatchMaterialAux = sendedNetQuantityRemissionGuideDispatchMaterial.GetValue();
        if (sendedNetQuantityRemissionGuideDispatchMaterialAux !== null && parseFloat(s.GetValue()) > parseFloat(sendedNetQuantityRemissionGuideDispatchMaterialAux)) {
            e.isValid = false;
            e.errorText = "Cantidad Consumida: Debe ser menor e igual a Neto Enviado.";
            if (errorMessageMaterials === null || errorMessageMaterials === "") {
                errorMessageMaterials = "- Cantidad Consumida: Debe ser menor e igual a Neto Enviado.";
            } else {
                errorMessageMaterials += "</br>- Cantidad Consumida: Debe ser menor e igual a Neto Enviado.";
            }
        }
    }

    if (!runningValidationMaterials) {
        ValidateMaterialsDetail();
    }

}

function OnsendedAdjustmentQuantityValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Ajuste Enviado: Es obligatoria.";
        } else {
            errorMessageMaterials += "</br>- Ajuste Enviado: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        if (Math.abs(parseFloat(s.GetValue())) > parseFloat(sendedDestinationQuantityRemissionGuideDispatchMaterial.GetValue())) {
            e.isValid = false;
            e.errorText = "Ajuste Enviado Incorrecto";
            if (errorMessageMaterials == null || errorMessageMaterials == "") {
                errorMessageMaterials = "- Ajuste Enviado: No debe ser mayor que la Cantidad Enviada.";
            } else {
                errorMessageMaterials += "</br>- Ajuste Enviado: No debe ser mayor que la Cantidad Enviada.";
            }
            //var warningAux = WarningMessage("Ajuste Enviado no debe ser mayor que la Cantidad Enviada");
            //gridMessageWarningDetail.SetText(warningAux);
            //$("#GridMessageWarningDetail").show();
            //if ($(".alert-warning") !== undefined && $(".alert-warning") !== null) {
            //    $(".alert-warning").fadeTo(3000, 0.45, function () {
            //        $(".alert-warning").alert('close');
            //    });
            //}
        }
    }

    if (!runningValidationMaterials) {
        ValidateMaterialsDetail();
    }

}


function OnstealQuantityValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Cantidad de Robo: Es obligatoria.";
        } else {
            errorMessageMaterials += "</br>- Cantidad de Robo: Es obligatoria.";
        }
    } else
        if (parseFloat(s.GetValue()) < 0) {
            e.isValid = false;
            e.errorText = "Cantidad de Robo Incorrecta";
            if (errorMessageMaterials == null || errorMessageMaterials == "") {
                errorMessageMaterials = "- Cantidad de Robo: Debe ser mayor e igual a cero.";
            } else {
                errorMessageMaterials += "</br>- Cantidad de Robo: Debe ser mayor e igual a cero.";
            }
        } else
            if (Math.abs(parseFloat(s.GetValue())) > parseFloat(sendedNetQuantityRemissionGuideDispatchMaterial.GetValue())) {
                e.isValid = false;
                e.errorText = "Cantidad de Robo Incorrecto";
                if (errorMessageMaterials == null || errorMessageMaterials == "") {
                    errorMessageMaterials = "- Cantidad de Robo: No debe ser mayor que el Neto Enviado.";
                } else {
                    errorMessageMaterials += "</br>- Cantidad de Robo: No debe ser mayor que el Neto Enviado.";
                }
                //var warningAux = WarningMessage("Cantidad de Robo no debe ser mayor que el Neto Enviado");
                //gridMessageWarningDetail.SetText(warningAux);
                //$("#GridMessageWarningDetail").show();
                //if ($(".alert-warning") !== undefined && $(".alert-warning") !== null) {
                //    $(".alert-warning").fadeTo(3000, 0.45, function () {
                //        $(".alert-warning").alert('close');
                //    });
                //}
            }

    if (!runningValidationMaterials) {
        ValidateMaterialsDetail();
    }

}

function OntransferQuantityValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Cantidad de Transferencia: Es obligatoria.";
        } else {
            errorMessageMaterials += "</br>- Cantidad de Transferencia: Es obligatoria.";
        }
    }
    //else
    //    if (Math.abs(parseFloat(s.GetValue())) > parseFloat(sendedNetQuantityRemissionGuideDispatchMaterial.GetValue())) {
    //        e.isValid = false;
    //        e.errorText = "Cantidad de Transferencia Incorrecta";
    //        if (errorMessageMaterials == null || errorMessageMaterials == "") {
    //            errorMessageMaterials = "- Cantidad de Transferencia: No debe ser mayor que el Neto Enviado.";
    //        } else {
    //            errorMessageMaterials += "</br>- Cantidad de Transferencia: No debe ser mayor que el Neto Enviado.";
    //        }
    //    }

    if (!runningValidationMaterials) {
        ValidateMaterialsDetail();
    }

}


function OnArrivalDestinationQuantityValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Cantidad Recibida: Es obligatoria.";
        } else {
            errorMessageMaterials += "</br>- Cantidad Recibida: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Cantidad Recibida: Es incorrecta.";
        } else {
            errorMessageMaterials += "</br>- Cantidad Recibida: Es incorrecta.";
        }
    } else if (parseFloat(s.GetValue()) > parseFloat(sourceExitQuantityRemissionGuideDispatchMaterial.GetValue())) {
        //e.isValid = false;
        //e.errorText = "Cantidad Recibida no puede ser mayor que Cantidad Salida Origen";
        //if (errorMessageMaterials == null || errorMessageMaterials == "") {
        //    errorMessageMaterials = "- Cantidad Recibida: No puede ser mayor que Cantidad Salida Origen.";
        //} else {
        //    errorMessageMaterials += "</br>- Cantidad Recibida: No puede ser mayor que Cantidad Salida Origen.";
        //}
        var warningAux = WarningMessage("Cantidad Recibida no debe ser mayor que Cantidad Salida Origen");
        gridMessageWarningDetail.SetText(warningAux);

        $("#GridMessageWarningDetail").show();
        if ($(".alert-warning") !== undefined && $(".alert-warning") !== null) {
            $(".alert-warning").fadeTo(3000, 0.45, function () {
                $(".alert-warning").alert('close');
            });
        }
        s.SetValue = 0;
    }
    //else if (parseFloat(s.GetValue()) != (parseFloat(arrivalGoodConditionRemissionGuideDispatchMaterial.GetValue()) + parseFloat(arrivalBadConditionRemissionGuideDispatchMaterial.GetValue()))) {
    //    e.isValid = false;
    //    e.errorText = "Cantidad Recibida debe ser igual a Cantidad en Buen Estado más Cantidad en Mal Estado";
    //    if (errorMessageMaterials == null || errorMessageMaterials == "") {
    //        errorMessageMaterials = "- Cantidad Recibida: Debe ser igual a Cantidad en Buen Estado más Cantidad en Mal Estado.";
    //    } else {
    //        errorMessageMaterials += "</br>- Cantidad Recibida: Debe ser igual a Cantidad en Buen Estado más Cantidad en Mal Estado.";
    //    }
    //}

    if (!runningValidationMaterials) {
        ValidateMaterialsDetail();
    }

}

function OnArrivalBadConditionValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Cantidad Mal Estado: Es obligatoria.";
        } else {
            errorMessageMaterials += "</br>- Cantidad Mal Estado: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Cantidad Mal Estado: Es incorrecta.";
        } else {
            errorMessageMaterials += "</br>- Cantidad Mal Estado: Es incorrecta.";
        }
    }

    if (!runningValidationMaterials) {
        ValidateMaterialsDetail();
    }
}

function OnArrivalGoodConditionValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Cantidad Buen Estado: Es obligatoria.";
        } else {
            errorMessageMaterials += "</br>- Cantidad Buen Estado: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessageMaterials == null || errorMessageMaterials == "") {
            errorMessageMaterials = "- Cantidad Buen Estado: Es incorrecta.";
        } else {
            errorMessageMaterials += "</br>- Cantidad Buen Estado: Es incorrecta.";
        }
    }

    if (!runningValidationMaterials) {
        ValidateMaterialsDetail();
    }

    if (errorMessageMaterials != null && errorMessageMaterials != "") {
        var msgErrorAux = ErrorMessage(errorMessageMaterials);
        gridMessageErrorMaterialsDetail.SetText(msgErrorAux);
        $("#GridMessageErrorMaterialsDetail").show();

    }
}

function ValidateMaterialsDetail() {
    runningValidationMaterials = true;
    //OnWarehouseMaterialsDetailValidation(id_warehouseMaterial, id_warehouseMaterial);
    //OnWarehouseLocationMaterialsDetailValidation(id_warehouseLocationMaterial, id_warehouseLocationMaterial);
    OnSendedDestinationQuantityValidation(sendedDestinationQuantityRemissionGuideDispatchMaterial, sendedDestinationQuantityRemissionGuideDispatchMaterial);
    OnsendedAdjustmentQuantityValidation(sendedAdjustmentQuantityRemissionGuideDispatchMaterial, sendedAdjustmentQuantityRemissionGuideDispatchMaterial);//
    OnsendedNetQuantityValidation(sendedNetQuantityRemissionGuideDispatchMaterial, sendedNetQuantityRemissionGuideDispatchMaterial);//
    OnAmountConsumedValidation(amountConsumedRemissionGuideDispatchMaterial, amountConsumedRemissionGuideDispatchMaterial);
    OnstealQuantityValidation(stealQuantityRemissionGuideDispatchMaterial, stealQuantityRemissionGuideDispatchMaterial);//
    OnArrivalDestinationQuantityValidation(arrivalDestinationQuantityRemissionGuideDispatchMaterial, arrivalDestinationQuantityRemissionGuideDispatchMaterial);
    OntransferQuantityValidation(transferQuantityRemissionGuideDispatchMaterial, transferQuantityRemissionGuideDispatchMaterial);//

    OnArrivalBadConditionValidation(arrivalBadConditionRemissionGuideDispatchMaterial, arrivalBadConditionRemissionGuideDispatchMaterial);
    OnArrivalGoodConditionValidation(arrivalGoodConditionRemissionGuideDispatchMaterial, arrivalGoodConditionRemissionGuideDispatchMaterial);
    runningValidationMaterials = false;

}

function AmountConsumed_Init(s, e) {
    //console.log(gvRemissionGuideDispatchMaterialEditForm.cpEditingRowItemDetailIsConsumed);
    amountConsumedRemissionGuideDispatchMaterial.SetEnabled(gvRemissionGuideDispatchMaterialEditForm.cpEditingRowItemDetailIsConsumed);
    //amountConsumedRemissionGuideDispatchMaterial.SetEnabled(gvRemissionGuideDispatchMaterialEditForm.cpEditingRowItemDetailIsConsumed);
}

function AmountConsumed_ValueChanged(s, e) {

    UpdateNotReceivedQuantityRemissionGuideDispatchMaterial(s, e);
}

function StealQuantity_ValueChanged(s, e) {

    UpdateNotReceivedQuantityRemissionGuideDispatchMaterial(s, e);
}

function TransferQuantity_ValueChanged(s, e) {

    UpdateNotReceivedQuantityRemissionGuideDispatchMaterial(s, e);
}

function SendedAdjustmentQuantity_ValueChanged(s, e) {

    UpdateSendedNetQuantityRemissionGuideDispatchMaterial(s, e);
    UpdateNotReceivedQuantityRemissionGuideDispatchMaterial(s, e);
}

function UpdateSendedNetQuantityRemissionGuideDispatchMaterial(s, e) {

    var sendedDestinationQuantityRemissionGuideDispatchMaterialAux = sendedDestinationQuantityRemissionGuideDispatchMaterial.GetValue();
    var sendedDestinationQuantityRemissionGuideDispatchMaterialFloat = sendedDestinationQuantityRemissionGuideDispatchMaterialAux == null || sendedDestinationQuantityRemissionGuideDispatchMaterialAux == "" ? 0 : parseFloat(sendedDestinationQuantityRemissionGuideDispatchMaterialAux);

    var sendedAdjustmentQuantityRemissionGuideDispatchMaterialAux = sendedAdjustmentQuantityRemissionGuideDispatchMaterial.GetValue();
    var sendedAdjustmentQuantityRemissionGuideDispatchMaterialFloat = sendedAdjustmentQuantityRemissionGuideDispatchMaterialAux == null || sendedAdjustmentQuantityRemissionGuideDispatchMaterialAux == "" ? 0 : parseFloat(sendedAdjustmentQuantityRemissionGuideDispatchMaterialAux);


    var sendedNetQuantityRemissionGuideDispatchMaterialAux = sendedDestinationQuantityRemissionGuideDispatchMaterialFloat + sendedAdjustmentQuantityRemissionGuideDispatchMaterialFloat;
    sendedNetQuantityRemissionGuideDispatchMaterial.SetValue(sendedNetQuantityRemissionGuideDispatchMaterialAux);
}

function ArrivalDestinationQuantity_ValueChanged(s, e) {

    UpdateNotReceivedQuantityRemissionGuideDispatchMaterial(s, e);
    UpdateArrivalGoodConditionRemissionGuideDispatchMaterial(s, e);
}

function UpdateNotReceivedQuantityRemissionGuideDispatchMaterial(s, e) {

    var stealQuantityRemissionGuideDispatchMaterialAux = stealQuantityRemissionGuideDispatchMaterial.GetValue();
    var stealQuantityRemissionGuideDispatchMaterialFloat = stealQuantityRemissionGuideDispatchMaterialAux == null || stealQuantityRemissionGuideDispatchMaterialAux == "" ? 0 : parseFloat(stealQuantityRemissionGuideDispatchMaterialAux);

    var transferQuantityRemissionGuideDispatchMaterialAux = transferQuantityRemissionGuideDispatchMaterial.GetValue();
    var transferQuantityRemissionGuideDispatchMaterialFloat = transferQuantityRemissionGuideDispatchMaterialAux == null || transferQuantityRemissionGuideDispatchMaterialAux == "" ? 0 : parseFloat(transferQuantityRemissionGuideDispatchMaterialAux);

    var arrivalDestinationQuantityRemissionGuideDispatchMaterialAux = arrivalDestinationQuantityRemissionGuideDispatchMaterial.GetValue();
    var arrivalDestinationQuantityRemissionGuideDispatchMaterialFloat = arrivalDestinationQuantityRemissionGuideDispatchMaterialAux == null || arrivalDestinationQuantityRemissionGuideDispatchMaterialAux == "" ? 0 : parseFloat(arrivalDestinationQuantityRemissionGuideDispatchMaterialAux);

    var amountConsumedRemissionGuideDispatchMaterialAux = amountConsumedRemissionGuideDispatchMaterial.GetValue();
    var amountConsumedRemissionGuideDispatchMaterialFloat = amountConsumedRemissionGuideDispatchMaterialAux == null || amountConsumedRemissionGuideDispatchMaterialAux == "" ? 0 : parseFloat(amountConsumedRemissionGuideDispatchMaterialAux);

    var sendedNetQuantityRemissionGuideDispatchMaterialAux = sendedNetQuantityRemissionGuideDispatchMaterial.GetValue();
    var sendedNetQuantityRemissionGuideDispatchMaterialFloat = sendedNetQuantityRemissionGuideDispatchMaterialAux == null || sendedNetQuantityRemissionGuideDispatchMaterialAux == "" ? 0 : parseFloat(sendedNetQuantityRemissionGuideDispatchMaterialAux);

    //var notReceivedQuantityAux = sendedDestinationQuantityRemissionGuideDispatchMaterialFloat - (arrivalDestinationQuantityRemissionGuideDispatchMaterialFloat + amountConsumedRemissionGuideDispatchMaterialFloat);
    var notReceivedQuantityAux = sendedNetQuantityRemissionGuideDispatchMaterialFloat - amountConsumedRemissionGuideDispatchMaterialFloat - arrivalDestinationQuantityRemissionGuideDispatchMaterialFloat - stealQuantityRemissionGuideDispatchMaterialFloat - transferQuantityRemissionGuideDispatchMaterialFloat;
    notReceivedQuantity.SetValue(notReceivedQuantityAux);
}

function UpdateArrivalGoodConditionRemissionGuideDispatchMaterial(s, e) {

    var arrivalDestinationQuantityRemissionGuideDispatchMaterialAux = arrivalDestinationQuantityRemissionGuideDispatchMaterial.GetValue();
    var arrivalDestinationQuantityRemissionGuideDispatchMaterialFloat = arrivalDestinationQuantityRemissionGuideDispatchMaterialAux == null || arrivalDestinationQuantityRemissionGuideDispatchMaterialAux == "" ? 0 : parseFloat(arrivalDestinationQuantityRemissionGuideDispatchMaterialAux);



    var stealQuantityRemissionGuideDispatchMaterialAux = stealQuantityRemissionGuideDispatchMaterial.GetValue();//
    var stealQuantityRemissionGuideDispatchMaterialFloat = stealQuantityRemissionGuideDispatchMaterialAux == null || stealQuantityRemissionGuideDispatchMaterialAux == "" ? 0 : parseFloat(stealQuantityRemissionGuideDispatchMaterialAux);


    var arrivalBadConditionRemissionGuideDispatchMaterialAux = arrivalBadConditionRemissionGuideDispatchMaterial.GetValue();
    var arrivalBadConditionRemissionGuideDispatchMaterialFloat = arrivalBadConditionRemissionGuideDispatchMaterialAux == null || arrivalBadConditionRemissionGuideDispatchMaterialAux == "" ? 0 : parseFloat(arrivalBadConditionRemissionGuideDispatchMaterialAux);


    var arrivalGoodConditionRemissionGuideDispatchMaterialAux = arrivalDestinationQuantityRemissionGuideDispatchMaterialFloat - arrivalBadConditionRemissionGuideDispatchMaterialFloat;
    arrivalGoodConditionRemissionGuideDispatchMaterial.SetValue(arrivalGoodConditionRemissionGuideDispatchMaterialAux);
}

//Materials

var errorMessage = "";
var runningValidation = false;

var id_itemIniAux = null;
var id_warehouseIniAux = null;
var id_warehouseLocationIniAux = null;


// VALIDATIONS

function OnWarehouseReceptionDispatchMaterialsDetailValidation(s, e) {
    //gridMessageErrorsDetail.SetText(result.Message);
    errorMessage = "";

    $("#GridMessageErrorsDetail").hide();

    //var arrivalDestinationQuantityAux = parseFloat(arrivalDestinationQuantity.GetValue())

    //if (arrivalDestinationQuantityAux > 0) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Bodega: Es obligatoria.";//, por tener una cantidad recibida.";
    }
    //}

}

function OnWarehouseLocationReceptionDispatchMaterialsDetailValidation(s, e) {

    //var arrivalDestinationQuantityAux = parseFloat(arrivalDestinationQuantity.GetValue())

    //if (arrivalDestinationQuantityAux > 0) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Ubicación: Es obligatoria.";
        } else {
            errorMessage += "</br>- Ubicación: Es obligatoria.";
        }
    }
    //}

    if (!runningValidation) {
        ValidateDetail();
    }
}

function OnItemReceptionDispatchMaterialsDetailValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Nombre del Producto: Es obligatorio.";
        } else {
            errorMessage += "</br>- Nombre del Producto: Es obligatorio.";
        }
    } else {
        var data = {
            id_itemNew: s.GetValue(),
            id_warehouseNew: id_warehouseReceptionDispatchMaterialsDetail.GetValue(),
            id_warehouseLocationNew: id_warehouseLocationReceptionDispatchMaterialsDetail.GetValue()
        };
        if (data.id_itemNew != id_itemIniAux || data.id_warehouseNew != id_warehouseIniAux ||
            data.id_warehouseLocationNew != id_warehouseLocationIniAux) {
            $.ajax({
                url: "ReceptionDispatchMaterials/ItsRepeatedDetail",
                type: "post",
                data: data,
                async: false,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    //showLoading();
                },
                success: function (result) {
                    if (result !== null) {
                        //console.log("result.itsRepeated: ");
                        //console.log(result.itsRepeated);
                        //console.log("result.itsRepeated == 1: ");
                        //console.log(result.itsRepeated == 1);
                        if (result.itsRepeated == 1) {
                            e.isValid = false;
                            e.errorText = result.Error;
                            if (errorMessage == null || errorMessage == "") {
                                errorMessage = "- Nombre del Producto: " + result.Error;
                            } else {
                                errorMessage += "</br>- Nombre del Producto: " + result.Error;
                            }
                        }
                        //else {
                        //    id_itemIniAux = 0
                        //    id_warehouseIniAux = 0
                        //    id_warehouseLocationIniAux = 0
                        //}
                    }
                },
                complete: function () {
                    //hideLoading();
                }
            });
        }
    }
}

function OnArrivalDestinationQuantityReceptionDispatchMaterialsDetailValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Recibida: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Recibida: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Recibida: Es incorrecta.";
        } else {
            errorMessage += "</br>- Cantidad Recibida: Es incorrecta.";
        }
    }
    //else if (parseFloat(s.GetValue()) != (parseFloat(arrivalGoodConditionReceptionDispatchMaterialsDetail.GetValue()) + parseFloat(arrivalBadConditionReceptionDispatchMaterialsDetail.GetValue()))) {
    //    e.isValid = false;
    //    e.errorText = "Cantidad Recibida debe ser igual a Cantidad en Buen Estado más Cantidad en Mal Estado";
    //    if (errorMessage == null || errorMessage == "") {
    //        errorMessage = "- Cantidad Recibida: Debe ser igual a Cantidad en Buen Estado más Cantidad en Mal Estado.";
    //    } else {
    //        errorMessage += "</br>- Cantidad Recibida: Debe ser igual a Cantidad en Buen Estado más Cantidad en Mal Estado.";
    //    }
    //}

    if (!runningValidation) {
        ValidateDetail();
    }

}




function OnArrivalBadConditionReceptionDispatchMaterialsDetailValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Mal Estado: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Mal Estado: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Mal Estado: Es incorrecta.";
        } else {
            errorMessage += "</br>- Cantidad Mal Estado: Es incorrecta.";
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }
}

function OnArrivalGoodConditionReceptionDispatchMaterialsDetailValidation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Buen Estado: Es obligatoria.";
        } else {
            errorMessage += "</br>- Cantidad Buen Estado: Es obligatoria.";
        }
    } else if (parseFloat(s.GetValue()) < 0) {
        e.isValid = false;
        e.errorText = "Cantidad Incorrecta";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Cantidad Buen Estado: Es incorrecta.";
        } else {
            errorMessage += "</br>- Cantidad Buen Estado: Es incorrecta.";
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }

    if (errorMessage != null && errorMessage != "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorsDetail.SetText(msgErrorAux);
        $("#GridMessageErrorsDetail").show();

    }
}

function ValidateDetail() {
    runningValidation = true;
    OnWarehouseReceptionDispatchMaterialsDetailValidation(id_warehouseReceptionDispatchMaterialsDetail, id_warehouseReceptionDispatchMaterialsDetail);
    OnWarehouseLocationReceptionDispatchMaterialsDetailValidation(id_warehouseLocationReceptionDispatchMaterialsDetail, id_warehouseLocationReceptionDispatchMaterialsDetail);
    OnItemReceptionDispatchMaterialsDetailValidation(itemReceptionDispatchMaterialsDetailName, itemReceptionDispatchMaterialsDetailName);
    OnArrivalDestinationQuantityReceptionDispatchMaterialsDetailValidation(arrivalDestinationQuantityReceptionDispatchMaterialsDetail, arrivalDestinationQuantityReceptionDispatchMaterialsDetail);
    OnArrivalBadConditionReceptionDispatchMaterialsDetailValidation(arrivalBadConditionReceptionDispatchMaterialsDetail, arrivalBadConditionReceptionDispatchMaterialsDetail);
    OnArrivalGoodConditionReceptionDispatchMaterialsDetailValidation(arrivalGoodConditionReceptionDispatchMaterialsDetail, arrivalGoodConditionReceptionDispatchMaterialsDetail);
    runningValidation = false;
}

function UpdateArrivalGoodConditionReceptionDispatchMaterialsDetail(s, e) {

    var arrivalDestinationQuantityReceptionDispatchMaterialsDetailAux = arrivalDestinationQuantityReceptionDispatchMaterialsDetail.GetValue();
    var arrivalDestinationQuantityReceptionDispatchMaterialsDetailFloat = arrivalDestinationQuantityReceptionDispatchMaterialsDetailAux == null || arrivalDestinationQuantityReceptionDispatchMaterialsDetailAux == "" ? 0 : parseFloat(arrivalDestinationQuantityReceptionDispatchMaterialsDetailAux);

    var arrivalBadConditionReceptionDispatchMaterialsDetailAux = arrivalBadConditionReceptionDispatchMaterialsDetail.GetValue();
    var arrivalBadConditionReceptionDispatchMaterialsDetailFloat = arrivalBadConditionReceptionDispatchMaterialsDetailAux == null || arrivalBadConditionReceptionDispatchMaterialsDetailAux == "" ? 0 : parseFloat(arrivalBadConditionReceptionDispatchMaterialsDetailAux);


    var arrivalGoodConditionReceptionDispatchMaterialsDetailAux = arrivalDestinationQuantityReceptionDispatchMaterialsDetailFloat - arrivalBadConditionReceptionDispatchMaterialsDetailFloat;
    arrivalGoodConditionReceptionDispatchMaterialsDetail.SetValue(arrivalGoodConditionReceptionDispatchMaterialsDetailAux);
}

// EDITOR'S EVENTS

function UpdateMaterialsDetailInfo(data, s, e) {

    if (data.id_item === null) {
        return;
    }

    //purchaseOrderNumber.SetText("");
    //remissionGuideNumber.SetText("");
    metricUnitMaterial.SetText("");
    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnitMaterial").SetText("");
    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(null);// 
    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(null);// SetValue("");

    if (id_itemMaterial != null) {

        $.ajax({
            url: "ProductionLotReception/ItemDetailData",
            type: "post",
            data: data,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result !== null) {
                    metricUnitMaterial.SetText(result.metricUnit);
                    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnit").SetText(result.metricUnit);
                    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);
                    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function ItemMaterialsDetailCombo_SelectedIndexChanged(s, e) {
    UpdateMaterialsDetailInfo({
        id_item: s.GetValue()
    }, s, e);
}

function ItemMaterialsDetailCombo_DropDown(s, e) {

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionMaterialDetails",
        type: "post",
        data: null,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            for (var i = 0; i < id_itemMaterial.GetItemCount(); i++) {
                var item = id_itemMaterial.GetItem(i);
                if (result.indexOf(item.value) >= 0) {
                    id_itemMaterial.RemoveItem(i);
                    i = -1;
                }
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function ComboItem_Init(s, e) {
    var data = {
        id_item: s.GetValue()
    };

    $.ajax({
        url: "ProductionLotReception/ProductionLotReceptionItemData",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            metricUnitMaterial.SetText("");
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                metricUnitMaterial.SetText(result.metricUnit);
            }
            else {
                metricUnitMaterial.SetText("");
            }
        },
        complete: function () {
            //hideLoading();
        }
    });

    ItemCombo_OnInit(s, e);
}

function ItemCombo_OnInit(s, e) {
    //store actual filtering method and override
    var actualFilteringOnClient = s.filterStrategy.FilteringOnClient;
    s.filterStrategy.FilteringOnClient = function () {
        //create a new format string for all list box columns. you could skip this bit and just set
        //filterTextFormatString to whatever you wanted for instance "{0} {2}" would only filter on
        //columns 1 and 3
        var lb = this.GetListBoxControl();
        var filterTextFormatStringItems = [];
        for (var i = 0; i < lb.columnFieldNames.length; i++) {
            filterTextFormatStringItems.push('{' + i + '}');
        }
        var filterTextFormatString = filterTextFormatStringItems.join(' ');

        //store actual format string and override with one for all columns
        var actualTextFormatString = lb.textFormatString;
        lb.textFormatString = filterTextFormatString;

        //call actual filtering method which will now work on our temporary format string
        actualFilteringOnClient.apply(this);

        //restore original format string
        lb.textFormatString = actualTextFormatString;
    };
}

function UpdateWarehouseLocations(warehouseLocations) {

    for (var i = 0; i < id_warehouseLocationMaterial.GetItemCount(); i++) {
        var warehouseLocation = id_warehouseLocationMaterial.GetItem(i);
        var into = false;
        for (var j = 0; j < warehouseLocations.length; j++) {
            if (warehouseLocation.value == warehouseLocations[j].id) {
                into = true;
                break;
            }
        }
        if (!into) {
            id_warehouseLocationMaterial.RemoveItem(i);
            i -= 1;
        }
    }


    for (var i = 0; i < warehouseLocations.length; i++) {
        var warehouseLocation = id_warehouseLocationMaterial.FindItemByValue(warehouseLocations[i].id);
        if (warehouseLocation == null) id_warehouseLocationMaterial.AddItem(warehouseLocations[i].name, warehouseLocations[i].id);
    }

}


//function WarehouseLocationReceptionDispatchMaterialsDetailCombo_OnInit(s, e) {


//}


function ItemReceptionDispatchMaterialsDetailCombo_SelectedIndexChanged(data, s, e) {

    itemReceptionDispatchMaterialsDetailMasterCode.SetText("");
    itemReceptionDispatchMaterialsDetailMetricUnit.SetText("");

    //if (id_itemMaterial != null) {

    $.ajax({
        url: "ReceptionDispatchMaterials/ItemDetailData",
        type: "post",
        data: { id_item: itemReceptionDispatchMaterialsDetailName.GetValue() },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null) {
                itemReceptionDispatchMaterialsDetailMasterCode.SetText(result.masterCode);
                itemReceptionDispatchMaterialsDetailMetricUnit.SetText(result.metricUnit);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
    //}
}



function WarehouseReceptionDispatchMaterialsDetailCombo_SelectedIndexChanged(s, e) {

    id_warehouseLocationReceptionDispatchMaterialsDetail.SetValue(null);
    //id_warehouseLocationMaterial.ClearItems();
    id_warehouseAux = id_warehouseReceptionDispatchMaterialsDetail.GetValue();
    id_warehouseLocationAux = null;
    name_warehouseLocationAux = null;
    id_warehouseLocationReceptionDispatchMaterialsDetail.PerformCallback();

}

function WarehouseLocationReceptionDispatchMaterialsDetailCombo_OnInit(s, e) {
    id_itemIniAux = itemReceptionDispatchMaterialsDetailName.GetValue();
    id_warehouseIniAux = id_warehouseReceptionDispatchMaterialsDetail.GetValue();
    id_warehouseLocationIniAux = id_warehouseLocationReceptionDispatchMaterialsDetail.GetValue();
    //var data = {
    //    id_warehouse: id_warehouseMaterial.GetValue(),
    //    id_warehouseLocation: s.GetValue()//,
    //};

    id_warehouseAux = id_warehouseReceptionDispatchMaterialsDetail.GetValue();
    id_warehouseLocationAux = id_warehouseLocationReceptionDispatchMaterialsDetail.GetValue();
    name_warehouseLocationAux = gvReceptionDispatchMaterialsDetailEditForm.cpEditingRowWarehouseLocationName;
    if (id_warehouseAux != null && id_warehouseAux !== null && id_warehouseAux != 0) {
        id_warehouseLocationReceptionDispatchMaterialsDetail.PerformCallback();
    } else {
        $.ajax({
            url: "ReceptionDispatchMaterials/WarehouseLocationReceptionDispatchMaterialsDetailCombo_OnInit",
            type: "post",
            data: null,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
                //var arrayFieldStr = [];
                //arrayFieldStr.push("name");
                //UpdateDetailObjects(id_warehouseLocationMaterial, [], arrayFieldStr);
                //UpdateWarehouseLocations([]);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                //var arrayFieldStr = [];
                //arrayFieldStr.push("name");
                //UpdateDetailObjects(id_warehouseLocationMaterial, result.warehouseLocations, arrayFieldStr);
                //UpdateWarehouseLocations(result.warehouseLocations);
                name_warehouseAux = result.name_warehouse;
                id_warehouseAux = result.id_warehouse;
                id_warehouseLocationAux = result.id_warehouseLocation;
                name_warehouseLocationAux = result.name_warehouseLocation;

                id_warehouseLocationReceptionDispatchMaterialsDetail.PerformCallback();
            },
            complete: function () {
                //hideLoading();
            }
        });
    }

}

function WarehouseLocationReceptionDispatchMaterialsDetail_BeginCallback(s, e) {
    e.customArgs["id_warehouseCurrent"] = id_warehouseAux;
    e.customArgs["id_warehouseLocationCurrent"] = id_warehouseLocationAux;
}

function WarehouseLocationReceptionDispatchMaterialsDetail_EndCallback(s, e) {
    id_warehouseReceptionDispatchMaterialsDetail.SetValue(id_warehouseAux);
    var warehouseLocationAux = id_warehouseLocationReceptionDispatchMaterialsDetail.FindItemByValue(id_warehouseLocationAux);
    if (warehouseLocationAux == null && id_warehouseLocationAux != null) id_warehouseLocationReceptionDispatchMaterialsDetail.AddItem(name_warehouseLocationAux, id_warehouseLocationAux);
    id_warehouseLocationReceptionDispatchMaterialsDetail.SetValue(id_warehouseLocationAux);
}

// EDITOR'S EVENTS

function UpdateTitlePanelDetail() {
    var gv = gvInvoiceCommercialEditFormDetail;

    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gv.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gv.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";

    $("#lblInfoDetails").html(text);

    if ($("#selectAllMode").val() !== "AllPages") {
        SetElementVisibility("lnkSelectAllRowsDetails", gv.GetSelectedRowCount() > 0 && gv.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility("lnkClearSelectionDetails", gv.GetSelectedRowCount() > 0);
    }
}

function GetSelectedFilteredRowCount() {
    return gvInvoiceCommercialEditFormDetail.cpFilteredRowCountWithoutPage + gvInvoiceCommercialEditFormDetail.GetSelectedKeysOnPage().length;
}

function OnGridViewInitDetail(s, e) {
    UpdateTitlePanelDetail();
}

function OnGridViewSelectionChangedDetail(s, e) {
    UpdateTitlePanelDetail();

    gvInvoiceCommercialEditFormDetail.GetSelectedFieldValues("id", GetSelectedFieldValuesCallbackDetail);

}

function GetSelectedFieldValuesCallbackDetail(values) {
    var selectedRows = [];
    for (var i = 0; i < values.length; i++) {
        selectedRows.push(values[i]);
    }
}

var customCommand = "";

function OnGridViewBeginCallbackDetail(s, e) {
    customCommand = e.command;
}

function OnGridViewEndCallbackDetail(s, e) {
    UpdateTitlePanelDetail();

}

function gvEditClearSelectionDetail() {
    gvInvoiceCommercialEditFormDetail.UnselectRows();
}

function gvEditSelectAllRowsDetail() {
    gvInvoiceCommercialEditFormDetail.SelectRows();
}

//gvInvoiceCommercialEditFormDetail

var itemCurrentAux = null;

function ItemComboBox_Init(s, e) {
    itemCurrentAux = s.GetValue();
    s.PerformCallback();

}

function ItemComboBox_BeginCallback(s, e) {
    e.customArgs["id_itemCurrent"] = itemCurrentAux;
}

function ItemComboBox_EndCallback(s, e) {
    id_item.SetValue(itemCurrentAux);
}

function OnItemComboBoxValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function ItemComboBox_SelectedIndexChanged(s, e) {

    itemInvoiceCommercialAuxCode.SetText("");
    itemInvoiceCommercialMasterCode.SetText("");
    amountInvoice.SetValue(null);
    //itemInvoiceCommercialUM.SetText("");
    invoiceCommercialTotal.SetValue(null);

    var unitPriceAux = unitPrice.GetValue();
    var strUnitPrice = unitPriceAux == null ? "0" : unitPriceAux.toString();
    var resUnitPrice = strUnitPrice.replace(".", ",");

    var numBoxesAux = numBoxes.GetValue();
    var strNumBoxes = numBoxesAux == null ? "0" : numBoxesAux.toString();
    var resNumBoxes = strNumBoxes.replace(".", ",");

    UpdateInvoiceCommercialDetail(s.GetValue(), resNumBoxes, resUnitPrice);

}

function UpdateInvoiceCommercialDetail(id_itemCurrent, numBoxesCurrent, unitPriceCurrent) {
    var data = {
        id_itemCurrent: id_itemCurrent,
        numBoxesCurrent: numBoxesCurrent,
        unitPriceCurrent: unitPriceCurrent
    };

    $.ajax({
        url: "InvoiceCommercial/UpdateInvoiceCommercialDetail",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
            //id_priceList.ClearItems();
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                //UpdateOpeningClosingPlateLyingMaintenanceWarehouseLocations(result.warehouseLocations);
                //itemInvoiceCommercialAuxCode, itemInvoiceCommercialMasterCode, amount, itemInvoiceCommercialUM, invoiceCommercialTotal
                itemInvoiceCommercialAuxCode.SetText(result.itemInvoiceCommercialAuxCode);
                itemInvoiceCommercialMasterCode.SetText(result.itemInvoiceCommercialMasterCode);

                amountInvoice.SetValue(result.amountInvoice);

                //itemInvoiceCommercialUM.SetText(result.itemInvoiceCommercialUM);

                invoiceCommercialTotal.SetValue(result.invoiceCommercialTotal);

            }
            //else {
            //    id_priceList.ClearItems();
            //}
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function NumBoxes_NumberChange(s, e) {

    //itemInvoiceCommercialAuxCode.SetText("");
    //itemInvoiceCommercialMasterCode.SetText("");
    //amountInvoice.SetValue(null);
    //itemInvoiceCommercialUM.SetText("");
    //invoiceCommercialTotal.SetValue(null);

    var unitPriceAux = unitPrice.GetValue();
    var strUnitPrice = unitPriceAux == null ? "0" : unitPriceAux.toString();
    var resUnitPrice = strUnitPrice.replace(".", ",");

    var numBoxesAux = s.GetValue();
    var strNumBoxes = numBoxesAux == null ? "0" : numBoxesAux.toString();
    var resNumBoxes = strNumBoxes.replace(".", ",");

    UpdateInvoiceCommercialDetail(id_item.GetValue(), resNumBoxes, resUnitPrice);

}

function UnitPrice_NumberChange(s, e) {

    //itemInvoiceCommercialAuxCode.SetText("");
    //itemInvoiceCommercialMasterCode.SetText("");
    //amountInvoice.SetValue(null);
    //itemInvoiceCommercialUM.SetText("");
    //invoiceCommercialTotal.SetValue(null);

    var unitPriceAux = s.GetValue();
    var strUnitPrice = unitPriceAux == null ? "0" : unitPriceAux.toString();
    var resUnitPrice = strUnitPrice.replace(".", ",");

    var numBoxesAux = numBoxes.GetValue();
    var strNumBoxes = numBoxesAux == null ? "0" : numBoxesAux.toString();
    var resNumBoxes = strNumBoxes.replace(".", ",");

    UpdateInvoiceCommercialDetail(id_item.GetValue(), resNumBoxes, resUnitPrice);

}

function ButtonUpdateItemDetail_Click(s, e) {
    var valid = true;
    var validFormLayoutEditInvoiceCommercialDetail = ASPxClientEdit.ValidateEditorsInContainerById("formLayoutEditInvoiceCommercialDetail", null, true);

    if (validFormLayoutEditInvoiceCommercialDetail) {
        //UpdateTabImage({ isValid: true }, "tabDocument");
    } else {
        //UpdateTabImage({ isValid: false }, "tabDocument");
        valid = false;
    }

    if (valid) {
        gvInvoiceCommercialEditFormDetail.UpdateEdit();
    }
    //var valid = ASPxClientEdit.ValidateEditorsInContainer(null, null, true);

    //if (!valid) {
    //    UpdateTabImage({ isValid: false }, "tabOportunity");
    //}



    //if (valid) {
    //    var id = $("#id_businessOportunity").val();
    //    var data = "id=" + id + "&" + $("#formEditBusinessOportunity").serialize();
    //    var url = (id === "0") ? "BusinessOportunity/BussinesOportunityPartialAddNew"
    //                           : "BusinessOportunity/BussinesOportunityPartialUpdate";
    //    showForm(url, data);
    //}


}

function BtnCancelItemDetail_Click(s, e) {
    //$("#GridMessageErrorPhase").hide();
    gvInvoiceCommercialEditFormDetail.CancelEdit();

    //showPage("BusinessOportunity/Index", null);
}