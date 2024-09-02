function UpdateTabImage(e, tabName) {
    var imageUrl = "/Content/image/noimage.png";
    if (!e.isValid) {
        imageUrl = "/Content/image/info-error.png";
    }
    var tab = tabControl.GetTabByName(tabName);
    tab.SetImageUrl(imageUrl);
    tab.SetActiveImageUrl(imageUrl);
}

function OnDocumentTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}



function OnPersonValidation(s, e) {
    if(e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnPriceListValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnEmissionDateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnStartDateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnEndDateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnBusinessOportunityResultsValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// DETAILS ACTIONS NOTES

function OnReferenciaValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnEstimatedEndDateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        OnRangeDateValidation(e, startDate.GetValue(), estimatedEndDate.GetValue(), "Fecha de Cierre Estimado debe ser mayor o igual a la Fecha Inicio de la Oportunidad.");
    }

}

// DETAILS ACTIONS PLANNINGS


function AmountValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var valueMin = 0.01;
        var valueMax = 9999999999999.99;
        var msgErrorAux = "Monto debe estar entre $0.01 y $9,999,999,999,999.99";
        OnRangeNumberValidation(e, parseFloat(e.value), valueMin, valueMax, msgErrorAux);
    }
}

function EstimatedProfitValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var valueMin = 0.01;
        var valueMax = 100.00;
        var msgErrorAux = "% Ganacia Estimada debe estar entre 0.01% y 100.00%";
        OnRangeNumberValidation(e, parseFloat(e.value), valueMin, valueMax, msgErrorAux);
    }
}

function QuantityValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var valueMin = 0.01;
        var valueMax = 9999999999999.99;
        var msgErrorAux = "Cantidad debe estar entre 0.01 y 9,999,999,999,999.99";
        OnRangeNumberValidation(e, parseFloat(e.value), valueMin, valueMax, msgErrorAux);
    }
}

function PriceValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        var valueMin = 0.01;
        var valueMax = 9999999999999.99;
        var msgErrorAux = "Precio debe estar entre $0.01 y $9,999,999,999,999.99";
        OnRangeNumberValidation(e, parseFloat(e.value), valueMin, valueMax, msgErrorAux);
    }
}

// DETAILS ACTIONS PHASE

var errorMessage = "";
var runningValidation = false;

function OnStartDatePhaseValidation(s, e) {
    //gridMessageErrorPhase.SetText(result.Message);
    errorMessage = "";
    $("#GridMessageErrorPhase").hide();
    if (startDatePhase.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Fecha de Inicio: Es obligatorio.";
    } else {
        var isValidAux = OnRangeDateValidation(e, startDate.GetValue(), startDatePhase.GetValue(), "Fecha de Inicio debe ser mayor o igual a la Fecha Inicio de la Oportunidad.");
        if (isValidAux) {
            var startDatePhaseAux = JSON.stringify(startDatePhase.GetDate());
            //var formattedDatestartDatePhaseAux = $.jqx.dataFormat.formatdate(startDatePhaseAux, "dd/MM/yyyy hh:mm:ss tt");
            var data = {
                startDatePhase: startDatePhaseAux,
                idCurrent: gvBusinessOportunityPhases.cpEditingRowIDPhaseDetail,
            };
            $.ajax({
                url: "BusinessOportunity/ItsStartDatePhaseValidWithLastEndDatePhaseDetail",//ItsRepeatedLiquidation",
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
                        //console.log("result: " + result);
                        //console.log("result.itsValided: " + result.itsValided);

                        if (result.itsValided == 0) {
                            e.isValid = false;
                            e.errorText = result.Error;
                            errorMessage = "- Fecha de Inicio: " + result.Error;
                        } else {
                            //console.log("e.isValid: " + e.isValid);
                            if (e.isValid) {
                                isValidAux = OnRangeDateValidation(e, startDatePhase.GetValue(), endDatePhase.GetValue(), "Fecha de Inicio debe ser menor o igual a la Fecha Fin.");
                                if (!isValidAux) {
                                    errorMessage = "- Fecha de Inicio: Debe ser menor o igual a la Fecha Fin.";
                                }
                            }
                            //console.log("e.isValid: " + e.isValid);
                        }
                    }
                },
                complete: function () {
                    //hideLoading();
                }
            });
        } else {
            errorMessage = "- Fecha de Inicio: Debe ser mayor o igual a la Fecha Inicio de la Oportunidad.";
        }
    }
    
    if (!runningValidation) {
        ValidatePhase();
    }
}

function OnEndDatePhaseValidation(s, e) {
    if (endDatePhase.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Fecha Fin: Es obligatoria.";
        } else {
            errorMessage += "</br>- Fecha Fin: Es obligatoria.";
        }
    } else {
        var isValidAux = OnRangeDateValidation(e, startDatePhase.GetValue(), endDatePhase.GetValue(), "Fecha de Fin debe ser mayor o igual a la Fecha Inicio.");
        if (!isValidAux) {
            if (errorMessage == null || errorMessage == "") {
                errorMessage = "- Fecha Fin: Debe ser mayor o igual a la Fecha Inicio.";
            } else {
                errorMessage += "</br>- Fecha Fin: Debe ser mayor o igual a la Fecha Inicio.";
            }
        }
    }

    if (!runningValidation) {
        ValidatePhase();
    }
}

function OnExecutivePersonPhaseValidation(s, e) {
    if (id_executivePersonPhase.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Ejecutivo: Es obligatorio.";
        } else {
            errorMessage += "</br>- Ejecutivo: Es obligatorio.";
        }
    }

    if (!runningValidation) {
        ValidatePhase();
    }
}

function OnPhaseValidation(s, e) {
    if (id_businessOportunityDocumentTypePhase.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Etapa: Es obligatoria.";
        } else {
            errorMessage += "</br>- Etapa: Es obligatoria.";
        }
    }

    if (errorMessage != null && errorMessage != "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorPhase.SetText(msgErrorAux);
        $("#GridMessageErrorPhase").show();

    }

    if (!runningValidation) {
        ValidatePhase();
    }
}

function ValidatePhase() {
    runningValidation = true;
    OnStartDatePhaseValidation(startDatePhase, startDatePhase);
    OnEndDatePhaseValidation(endDatePhase, endDatePhase);
    OnExecutivePersonPhaseValidation(id_executivePersonPhase, id_executivePersonPhase);
    OnPhaseValidation(id_businessOportunityDocumentTypePhase, id_businessOportunityDocumentTypePhase);
    runningValidation = false;

}

function OnAttachmentValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnBusinessOportunityActivityValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnStateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}


//ATTACHMENT


