
var id_motiveLotInit = null;
var numPersonDetailInit = null;
//StopCheckBox
function StopCheckBox_Init() {
    numPersonDetailInit = numPersonDetail.GetValue();
    if (GridViewDetails.cpRowId === 0) {
        stop.SetChecked(true);
        totalHoursDetail.SetValue("00:00");
    }
    if (stop.GetValue() === true) {
        observation.SetEnabled(true);
        numPersonDetail.SetEnabled(false);
    } else {
        observation.SetEnabled(false);
        numPersonDetail.SetEnabled(true);
        if (numPersonDetailInit === 0 || numPersonDetailInit === null) {
            numPersonDetail.SetValue(GridViewDetails.cpNumPerson);
        }
    }
    //// 
    id_motiveLotInit = id_motiveLotProcessTypeGeneral.GetValue();
    var xd = id_motiveLotProcessTypeGeneral.GetText();
    id_motiveLotProcessTypeGeneral.PerformCallback();
}

function StopCheckBox_CheckedChanged() {
    observation.SetText("");
    id_motiveLotInit = null;
    if (stop.GetValue() === true) {
        observation.SetEnabled(true);
        numPersonDetail.SetEnabled(false);
        numPersonDetail.SetValue(null);
    } else {
        observation.SetEnabled(false);
        numPersonDetail.SetEnabled(true);
        if (numPersonDetailInit !== 0 && numPersonDetailInit !== null) {
            numPersonDetail.SetValue(numPersonDetailInit);
        } else {
            numPersonDetail.SetValue(GridViewDetails.cpNumPerson);
        }
    }
    id_motiveLotProcessTypeGeneral.PerformCallback();
}

//MotivoLoteComboBox
function MotivoLoteComboBox_BeginCallback(s, e) {
    //// 
    e.customArgs["stopCurrent"] = stop.GetValue() === undefined ? false : stop.GetValue();
}

function MotivoLoteComboBox_EndCallback(s, e) {
    //// 
    id_motiveLotProcessTypeGeneral.SetValue(id_motiveLotInit);
}

function PrintItem() {
    $.ajax({
        url: "NonProductiveHour/NonProductiveHourReport",
        type: "post",
        data: {
            id: $("#id_nonProductiveHour").val(),
            codeReport: "RNPH"
        },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            try {
                if (result !== undefined) {
                    var reportTdr = result.nameQS;
                    var url = 'ReportProd/Index?trepd=' + reportTdr;
                    newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                    newWindow.focus();
                    hideLoading();
                }
            }
            catch (err) {
                hideLoading();
            }
        },
        complete: function () {
            hideLoading();
        }
    });
}

function MotivoLoteComboBox_SelectedIndexChanged() {
    //// 
    if (stop.GetValue() === false) {
        //observation.SetText(id_motiveLot.GetText());
        $.ajax({
            url: "NonProductiveHour/GetObservation",
            type: "post",
            data: { id_motiveLotProcessTypeGeneral: id_motiveLotProcessTypeGeneral.GetValue() },
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
                    observation.SetText(result.observation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

//StartDate
function StartDateDateEdit_DateChanged() {
    TimeEdit_ValueChanged();
}

//EndDate
function EndDateDateEdit_DateChanged() {
    TimeEdit_ValueChanged();
}

//StartTime
function TimeEdit_ValueChanged() {
    //// 
    var dateInicio = startDate.GetDate();
    var dateFin = endDate.GetDate();
    if (dateInicio !== null && dateFin !== null) {
        var yearInicio = dateInicio.getFullYear();
        var monthInicio = dateInicio.getMonth() + 1;
        var dayInicio = dateInicio.getDate();
        var dateInicioAux = new Date(yearInicio, dateInicio.getMonth(), dayInicio);

        var yearFin = dateFin.getFullYear();
        var monthFin = dateFin.getMonth() + 1;
        var dayFin = dateFin.getDate();
        var dateFinAux = new Date(yearFin, dateFin.getMonth(), dayFin);

        var emisionDate = dateTimeEmisionStr.GetValue().split("-");
        var yearEmisionDate = parseInt(emisionDate[2]);
        var monthEmisionDate = parseInt(emisionDate[1]);
        var dayEmisionDate = parseInt(emisionDate[0]);
        var emisionDateAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate);
        var emisionDatePlusAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate + 1);

        if (dateInicioAux.getTime() !== emisionDateAux.getTime() && dateInicioAux.getTime() !== emisionDatePlusAux.getTime() ||
            dateFinAux.getTime() !== emisionDateAux.getTime() && dateFinAux.getTime() !== emisionDatePlusAux.getTime() || dateFinAux < dateInicioAux) {
            totalHoursDetail.SetValue("00:00");
            //totalHoursDetail.SetValue(monthInicio.toString().padStart(2, 0), monthInicio.toString().padStart(2, 0));// new Date(yearInicio, dateInicio.getMonth(), dayInicio, 0, 0, 0));
        }
        else {
            var startTimeValue = startTimeDetail.GetValue();
            var endTimeValue = endTimeDetail.GetValue();

            if (startTimeValue !== null && endTimeValue !== null) {
                //var startTimeArray = startTimeValue.split("/");
                var hoursStartTime = startTimeValue.getHours();
                var minutesStartTime = startTimeValue.getMinutes();
                var totalMinutesStartTime = minutesStartTime + hoursStartTime * 60;

                //var endTimeArray = endTimeValue.split("/");
                var hoursEndTime = endTimeValue.getHours();
                var minutesEndTime = endTimeValue.getMinutes();
                var totalMinutesEndTime = minutesEndTime + hoursEndTime * 60;

                if (dateInicioAux.getTime() === dateFinAux.getTime()) {
                    if (!ValidateRangeTime(startTimeDetail.GetValue(), endTimeDetail.GetValue(), false)) {
                        //totalHoursDetail.SetValue(new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate, 0, 0, 0));
                        totalHoursDetail.SetValue("00:00");

                    } else {
                        var totalMinutesTotalHours = totalMinutesEndTime - totalMinutesStartTime;

                        var hoursTotalHours = Math.trunc(totalMinutesTotalHours / 60);
                        var minutesTotalHours = totalMinutesTotalHours - hoursTotalHours * 60;

                        //totalHoursDetail.SetValue(new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate, hoursTotalHours, minutesTotalHours, 0));
                        totalHoursDetail.SetValue(hoursTotalHours.toString().padStart(2, 0) + ":" + minutesTotalHours.toString().padStart(2, 0));
                        //totalHours.SetValue(hoursTotalHours + ":" + minutesTotalHours);
                    }
                }
                else {
                    var totalMinutesTotalHours2 = 1440 - totalMinutesStartTime + totalMinutesEndTime;

                    var hoursTotalHours2 = Math.trunc(totalMinutesTotalHours2 / 60);
                    var minutesTotalHours2 = totalMinutesTotalHours2 - hoursTotalHours2 * 60;

                    //totalHoursDetail.SetValue(new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate, hoursTotalHours2, minutesTotalHours2, 0));
                    totalHoursDetail.SetValue(hoursTotalHours2.toString().padStart(2, 0) + ":" + minutesTotalHours2.toString().padStart(2, 0));
                    //totalHours.SetValue(hoursTotalHours2 + ":" + minutesTotalHours2);
                }
            }
            else {
                totalHoursDetail.SetValue("00:00");
                //totalHoursDetail.SetValue(new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate, 0, 0, 0));
            }
        }
    } else {
        totalHoursDetail.SetValue("00:00");
        //totalHoursDetail.SetValue(new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate, 0, 0, 0));
    }
}

function StartTimeTimeEdit_ValueChanged() {
    TimeEdit_ValueChanged();
}


//EndTime
function EndTimeTimeEdit_ValueChanged() {
    TimeEdit_ValueChanged();
}

//Validations
var errorMessage = "";
var runningValidation = false;

function MotivoLoteComboBox_Validation(s, e) {
    //// 
    errorMessage = "";
    $("#GridMessageErrorDetail").hide();

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        errorMessage = "- Motivo/Lote: Es obligatorio.";
    }
    var dateInicio = startDate.GetDate();
    var dateFin = endDate.GetDate();
    var startTimeValue = startTimeDetail.GetValue();
    var endTimeValue = endTimeDetail.GetValue();
    //// 
    if (dateInicio !== null && dateFin !== null && startTimeValue !== null && endTimeValue !== null) {
        //var _index = gvLiquidationCartOnCartDetailEditForm.cpRowId;
        var yearInicio = dateInicio.getFullYear();
        var monthInicio = dateInicio.getMonth() + 1;
        var dayInicio = dateInicio.getDate();
        var dateInicioAux = new Date(yearInicio, dateInicio.getMonth(), dayInicio);

        var yearFin = dateFin.getFullYear();
        var monthFin = dateFin.getMonth() + 1;
        var dayFin = dateFin.getDate();
        var dateFinAux = new Date(yearFin, dateFin.getMonth(), dayFin);

        var emisionDate = dateTimeEmisionStr.GetValue().split("-");
        var yearEmisionDate = parseInt(emisionDate[2]);
        var monthEmisionDate = parseInt(emisionDate[1]);
        var dayEmisionDate = parseInt(emisionDate[0]);
        var emisionDateAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate);
        var emisionDatePlusAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate + 1);

        if ((dateInicioAux.getTime() === emisionDateAux.getTime() || dateInicioAux.getTime() === emisionDatePlusAux.getTime()) &&
            (dateFinAux.getTime() === emisionDateAux.getTime() || dateFinAux.getTime() === emisionDatePlusAux.getTime()) && (dateFinAux.getTime() === dateInicioAux.getTime() &&
                ValidateRangeTime(startTimeValue, endTimeValue, false) || dateFinAux > dateInicioAux)) {
            //e.isValid = false;
            //e.errorText = "La Hora Fin debe ser mayor a la Hora Inicio";
            //if (errorMessage === null || errorMessage === "") {
            //    errorMessage = "- Hora Fin: Debe ser mayor a la Hora Inicio.";
            //} else {
            //    errorMessage += "</br> - Hora Fin: Debe ser mayor a la Hora Inicio.";
            //}
            var data = {
                id: GridViewDetails.cpRowId,
                //initDate: dayInicio + "/" + monthInicio + "/" + yearInicio,
                initDate: yearInicio + "-" +
                    monthInicio.toString().padStart(2, 0) + "-" +
                    dayInicio.toString().padStart(2, 0) + "T" +
                    startTimeValue.getHours().toString().padStart(2, 0) + ":" +
                    startTimeValue.getMinutes().toString().padStart(2, 0) + ":00",
                //dayInicio + "/" + monthInicio + "/" + yearInicio + " " + startTimeValue,
                //endDate: dayFin + "/" + monthFin + "/" + yearFin,
                endDate: yearFin + "-" +
                    monthFin.toString().padStart(2, 0) + "-" +
                    dayFin.toString().padStart(2, 0) + "T" +
                    endTimeValue.getHours().toString().padStart(2, 0) + ":" +
                    endTimeValue.getMinutes().toString().padStart(2, 0) + ":00"
                //dayFin + "/" + monthFin + "/" + yearFin + " " + endTimeValue
            };
            //if (data.id_salesOrderNew != id_SalesOrderIniAux || data.id_ProductionCartNew != id_ProductionCartIniAux ||
            //    data.id_ItemLiquidationNew != id_ItemLiquidationIniAux || data.id_ItemToWarehouseNew != id_ItemToWarehouseIniAux) {
            $.ajax({
                url: "NonProductiveHour/ItsRange",
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
                        if (result.itsRange === 1) {
                            e.isValid = false;
                            e.errorText = result.Error;
                            //errorMessage = result.Error;
                            if (errorMessage === null || errorMessage === "") {
                                errorMessage = result.Error;
                            } else {
                                errorMessage += "</br> " + result.Error;
                            }
                        }
                    }
                },
                complete: function () {
                    //hideLoading();
                }
            });
        }
    }

    if (!runningValidation) {
        ValidateDetail();
    }
}

function NumPersonDetailSpinEdit_Validation(s, e) {
    //// 
    if (stop.GetValue() !== true) {
        if (s.GetValue() === null) {
            e.isValid = false;
            e.errorText = "Campo Obligatorio";
            if (errorMessage === null || errorMessage === "") {
                errorMessage = "- No. Personas: Es obligatorio.";
            } else {
                errorMessage += "</br> - No. Personas: Es obligatorio.";
            }
        } else if (parseFloat(s.GetValue()) <= 0) {
            e.isValid = false;
            e.errorText = "Cantidad Incorrecto";
            if (errorMessage === null || errorMessage === "") {
                errorMessage = "- No. Personas: Valor Incorrecto.";
            } else {
                errorMessage += "</br> - No. Personas: Valor Incorrecto.";
            }
        }
    } else {
        e.isValid = true;
    }
    if (!runningValidation) {
        ValidateDetail();
    }
}

function StartDateDateEdit_Validation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        //errorMessage = "- Producto Liquidación: Es obligatorio.";
        if (errorMessage === null || errorMessage === "") {
            errorMessage = "- Fecha Inicio: Es obligatorio.";
        } else {
            errorMessage += "</br> - Fecha Inicio: Es obligatorio.";
        }

    } else {
        var dateInicio = s.GetDate();
        var yearInicio = dateInicio.getFullYear();
        var monthInicio = dateInicio.getMonth() + 1;
        var dayInicio = dateInicio.getDate();
        var dateInicioAux = new Date(yearInicio, dateInicio.getMonth(), dayInicio);

        var emisionDate = dateTimeEmisionStr.GetValue().split("-");
        var yearEmisionDate = parseInt(emisionDate[2]);
        var monthEmisionDate = parseInt(emisionDate[1]);
        var dayEmisionDate = parseInt(emisionDate[0]);
        var emisionDateAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate);
        var emisionDatePlusAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate + 1);

        if (dateInicioAux.getTime() !== emisionDateAux.getTime() && dateInicioAux.getTime() !== emisionDatePlusAux.getTime()) {
            e.isValid = false;
            e.errorText = "La Fecha Inicio debe ser igual a la Fecha de emisión o un día mayor";
            if (errorMessage === null || errorMessage === "") {
                errorMessage = "- Fecha Inicio: Debe ser igual a la Fecha de emisión o un día mayor.";
            } else {
                errorMessage += "</br> - Fecha Inicio: Debe ser igual a la Fecha de emisión o un día mayor.";
            }
        }
    }
    if (!runningValidation) {
        ValidateDetail();
    }
}

function StartTimeTimeEdit_Validation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        //errorMessage = "- Producto Liquidación: Es obligatorio.";
        if (errorMessage === null || errorMessage === "") {
            errorMessage = "- Hora Inicio: Es obligatorio.";
        } else {
            errorMessage += "</br> - Hora Inicio: Es obligatorio.";
        }
    } else {
        var dateInicio = startDate.GetDate();
        var dateFin = endDate.GetDate();
        var startTimeValue = s.GetValue();
        var endTimeValue = endTimeDetail.GetValue();

        if (dateInicio !== null && dateFin !== null && endTimeValue !== null) {
            var yearInicio = dateInicio.getFullYear();
            var monthInicio = dateInicio.getMonth() + 1;
            var dayInicio = dateInicio.getDate();
            var dateInicioAux = new Date(yearInicio, dateInicio.getMonth(), dayInicio);

            var yearFin = dateFin.getFullYear();
            var monthFin = dateFin.getMonth() + 1;
            var dayFin = dateFin.getDate();
            var dateFinAux = new Date(yearFin, dateFin.getMonth(), dayFin);

            var emisionDate = dateTimeEmisionStr.GetValue().split("-");
            var yearEmisionDate = parseInt(emisionDate[2]);
            var monthEmisionDate = parseInt(emisionDate[1]);
            var dayEmisionDate = parseInt(emisionDate[0]);
            var emisionDateAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate);
            var emisionDatePlusAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate + 1);

            var timeInitTurnAux = $('#timeInitTurn').val();
            var timeInitTurnAuxArray = timeInitTurnAux.split(":");
            var dateInitTurnWhithTimeAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate, timeInitTurnAuxArray[0], timeInitTurnAuxArray[1], 0);

            var timeEndTurnAux = $('#timeEndTurn').val();
            var timeEndTurnAuxArray = timeEndTurnAux.split(":");
            var dateEndTurnWhithTimeAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate, timeEndTurnAuxArray[0], timeEndTurnAuxArray[1], 0);

            if (!ValidateRangeTime(dateInitTurnWhithTimeAux, dateEndTurnWhithTimeAux, false)) {
                dateEndTurnWhithTimeAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate + 1, timeEndTurnAuxArray[0], timeEndTurnAuxArray[1], 0);
            }

            var dateInicioWhithTimeAux = new Date(dateInicioAux.getFullYear(), dateInicioAux.getMonth(), dateInicioAux.getDate(), startTimeValue.getHours(), startTimeValue.getMinutes(), 0);
            if (dateInitTurnWhithTimeAux > dateInicioWhithTimeAux || dateEndTurnWhithTimeAux < dateInicioWhithTimeAux) {
                e.isValid = false;
                e.errorText = "La Fecha Inicio con Hora Inicio debe estar dentro del horario del turno: " + turn.GetValue();
                if (errorMessage === null || errorMessage === "") {
                    errorMessage = "- Fecha Inicio con Hora Inicio: Debe estar dentro del horario del turno: " + turn.GetValue();
                } else {
                    errorMessage += "</br> - Fecha Inicio con Hora Inicio: Debe estar dentro del horario del turno: " + turn.GetValue();
                }
            }
        }
    }
    if (!runningValidation) {
        ValidateDetail();
    }
}

function EndDateDateEdit_Validation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage === null || errorMessage === "") {
            errorMessage = "- Fecha Fin: Es obligatorio.";
        } else {
            errorMessage += "</br> - Fecha Fin: Es obligatorio.";
        }
    } else {
        var dateFin = s.GetDate();
        var yearFin = dateFin.getFullYear();
        var monthFin = dateFin.getMonth() + 1;
        var dayFin = dateFin.getDate();
        var dateFinAux = new Date(yearFin, dateFin.getMonth(), dayFin);

        var dateInicio = startDate.GetDate();
        if (dateInicio !== null) {
            var yearInicio = dateInicio.getFullYear();
            var monthInicio = dateInicio.getMonth() + 1;
            var dayInicio = dateInicio.getDate();
            var dateInicioAux = new Date(yearInicio, dateInicio.getMonth(), dayInicio);

            //if (dateFinAux.getTime() !== emisionDateAux.getTime() && dateFinAux.getTime() !== emisionDatePlusAux.getTime()) {
            //    e.isValid = false;
            //    e.errorText = "La Fecha Fin debe ser igual a la Fecha de emisión o un día mayor";
            //    if (errorMessage === null || errorMessage === "") {
            //        errorMessage = "- Fecha Fin: Debe ser igual a la Fecha de emisión o un día mayor.";
            //    } else {
            //        errorMessage += "</br> - Fecha Fin: Debe ser igual a la Fecha de emisión o un día mayor.";
            //    }
            //}
            //else {
            if (dateFinAux < dateInicioAux) {
                e.isValid = false;
                e.errorText = "La Fecha Fin debe ser mayor e igual a la Fecha Inicio";
                if (errorMessage === null || errorMessage === "") {
                    errorMessage = "- Fecha Fin: Debe ser mayor e igual a la Fecha Inicio.";
                } else {
                    errorMessage += "</br> - Fecha Fin: Debe ser mayor e igual a la Fecha Inicio.";
                }
            }
            //}
        }
        else {
            var emisionDate = dateTimeEmisionStr.GetValue().split("-");
            var yearEmisionDate = parseInt(emisionDate[2]);
            var monthEmisionDate = parseInt(emisionDate[1]);
            var dayEmisionDate = parseInt(emisionDate[0]);
            var emisionDateAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate);
            var emisionDatePlusAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate + 1);

            if (dateFinAux.getTime() !== emisionDateAux.getTime() && dateFinAux.getTime() !== emisionDatePlusAux.getTime()) {
                e.isValid = false;
                e.errorText = "La Fecha Fin debe ser igual a la Fecha de emisión o un día mayor";
                if (errorMessage === null || errorMessage === "") {
                    errorMessage = "- Fecha Fin: Debe ser igual a la Fecha de emisión o un día mayor.";
                } else {
                    errorMessage += "</br> - Fecha Fin: Debe ser igual a la Fecha de emisión o un día mayor.";
                }
            }
        }
    }
    if (!runningValidation) {
        ValidateDetail();
    }
}

function EndTimeTimeEdit_Validation(s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage === null || errorMessage === "") {
            errorMessage = "- Hora Fin: Es obligatorio.";
        } else {
            errorMessage += "</br> - Hora Fin: Es obligatorio.";
        }
    }
    else {
        var dateInicio = startDate.GetDate();
        var dateFin = endDate.GetDate();
        var startTimeValue = startTimeDetail.GetValue();
        var endTimeValue = s.GetValue();

        if (dateInicio !== null && dateFin !== null && startTimeValue !== null) {
            var yearInicio = dateInicio.getFullYear();
            var monthInicio = dateInicio.getMonth() + 1;
            var dayInicio = dateInicio.getDate();
            var dateInicioAux = new Date(yearInicio, dateInicio.getMonth(), dayInicio);

            var yearFin = dateFin.getFullYear();
            var monthFin = dateFin.getMonth() + 1;
            var dayFin = dateFin.getDate();
            var dateFinAux = new Date(yearFin, dateFin.getMonth(), dayFin);

            var emisionDate = dateTimeEmisionStr.GetValue().split("-");
            var yearEmisionDate = parseInt(emisionDate[2]);
            var monthEmisionDate = parseInt(emisionDate[1]);
            var dayEmisionDate = parseInt(emisionDate[0]);
            var emisionDateAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate);
            var emisionDatePlusAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate + 1);

            if ((dateInicioAux.getTime() === emisionDateAux.getTime() || dateInicioAux.getTime() === emisionDatePlusAux.getTime()) &&
                (dateFinAux.getTime() === emisionDateAux.getTime() || dateFinAux.getTime() === emisionDatePlusAux.getTime()) && dateFinAux.getTime() === dateInicioAux.getTime() &&
                !ValidateRangeTime(startTimeValue, endTimeValue, false)) {
                e.isValid = false;
                e.errorText = "La Hora Fin debe ser mayor a la Hora Inicio";
                if (errorMessage === null || errorMessage === "") {
                    errorMessage = "- Hora Fin: Debe ser mayor a la Hora Inicio.";
                } else {
                    errorMessage += "</br> - Hora Fin: Debe ser mayor a la Hora Inicio.";
                }
            }
            //// 
            var timeInitTurnAux = $('#timeInitTurn').val();
            var timeInitTurnAuxArray = timeInitTurnAux.split(":");
            var dateInitTurnWhithTimeAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate, timeInitTurnAuxArray[0], timeInitTurnAuxArray[1], 0);

            var timeEndTurnAux = $('#timeEndTurn').val();
            var timeEndTurnAuxArray = timeEndTurnAux.split(":");
            var dateEndTurnWhithTimeAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate, timeEndTurnAuxArray[0], timeEndTurnAuxArray[1], 0);

            if (!ValidateRangeTime(dateInitTurnWhithTimeAux, dateEndTurnWhithTimeAux, false)) {
                dateEndTurnWhithTimeAux = new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate + 1, timeEndTurnAuxArray[0], timeEndTurnAuxArray[1], 0);
            }

            var dateFinWhithTimeAux = new Date(dateFinAux.getFullYear(), dateFinAux.getMonth(), dateFinAux.getDate(), endTimeValue.getHours(), endTimeValue.getMinutes(), 0);
            if (dateInitTurnWhithTimeAux > dateFinWhithTimeAux || dateEndTurnWhithTimeAux < dateFinWhithTimeAux) {
                e.isValid = false;
                e.errorText = "La Fecha Fin con Hora Fin debe estar dentro del horario del turno: " + turn.GetValue();
                if (errorMessage === null || errorMessage === "") {
                    errorMessage = "- Fecha Fin con Hora Fin: Debe estar dentro del horario del turno: " + turn.GetValue();
                } else {
                    errorMessage += "</br> - Fecha Fin con Hora Fin: Debe estar dentro del horario del turno: " + turn.GetValue();
                }
            }
        }
    }
    if (!runningValidation) {
        ValidateDetail();
    }
    if (errorMessage !== null && errorMessage !== "") {
        var msgErrorAux = ErrorMessage(errorMessage);
        gridMessageErrorDetail.SetText(msgErrorAux);
        $("#GridMessageErrorDetail").show();

    }
}

function ValidateDetail() {
    runningValidation = true;
    MotivoLoteComboBox_Validation(id_motiveLotProcessTypeGeneral, id_motiveLotProcessTypeGeneral);
    if (stop.GetValue() !== true) {
        NumPersonDetailSpinEdit_Validation(numPersonDetail, numPersonDetail);
    }
    StartDateDateEdit_Validation(startDate, startDate);
    StartTimeTimeEdit_Validation(startTimeDetail, startTimeDetail);
    EndDateDateEdit_Validation(endDate, endDate);
    EndTimeTimeEdit_Validation(endTimeDetail, endTimeDetail);
    runningValidation = false;
}

function AddNewDetail(s, e) {
    GridViewDetails.AddNewRow();
}

function RefreshDetail(s, e) {

    GridViewDetails.PerformCallback();
}

function OnGridViewDetailBeginCallback(s, e) {
    if (e.command === 'UPDATEEDIT') {
        e.customArgs["enabledCurrent"] = GridViewDetails.cpEnabled;
        e.customArgs["startTimeHours"] = startTimeDetail.GetValue() === undefined ? false : startTimeDetail.GetValue().getHours();
        e.customArgs["startTimeMinutes"] = startTimeDetail.GetValue() === undefined ? false : startTimeDetail.GetValue().getMinutes();
        e.customArgs["endTimeHours"] = endTimeDetail.GetValue() === undefined ? false : endTimeDetail.GetValue().getHours();
        e.customArgs["endTimeMinutes"] = endTimeDetail.GetValue() === undefined ? false : endTimeDetail.GetValue().getMinutes();
        e.customArgs["totalHoursDetail"] = totalHoursDetail.GetValue() === undefined ? false : totalHoursDetail.GetValue();
        //e.customArgs["startTime"] = null;
        //e.customArgs["endTime"] = null;

        //e.customArgs["totalHoursHours"] = totalHoursDetail.GetValue() === undefined ? false : totalHoursDetail.GetValue().getHours();
        //e.customArgs["totalHoursMinutes"] = totalHoursDetail.GetValue() === undefined ? false : totalHoursDetail.GetValue().getMinutes();
    }
    //e.customArgs["startTime"] = startTime.GetValue() === undefined ? false : startTime.GetValue();
    //e.customArgs["endTime"] = endTime.GetValue() === undefined ? false : endTime.GetValue();
}

function OnGridViewDetailEndCallback(s, e) {
    $.ajax({
        url: "NonProductiveHour/GetTotales",
        type: "post",
        data: null,
        async: true,
        cache: false,
        error: function (error) {
            //// 
            NotifyError("Error. " + error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            //// 
            //var emisionDate = dateTimeEmisionStr.GetValue().split("-");
            //var yearEmisionDate = parseInt(emisionDate[2]);
            //var monthEmisionDate = parseInt(emisionDate[1]);
            //var dayEmisionDate = parseInt(emisionDate[0]);

            //hoursStop.SetValue(new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate, result.hoursStop.Hours, result.hoursStop.Minutes, 0));
            //hoursProduction.SetValue(new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate, result.hoursProduction.Hours, result.hoursProduction.Minutes, 0));
            //totalHours.SetValue(new Date(yearEmisionDate, monthEmisionDate - 1, dayEmisionDate, result.totalHours.Hours, result.totalHours.Minutes, 0));
            hoursStop.SetValue(result.hoursStop);
            hoursProduction.SetValue(result.hoursProduction);
            totalHours.SetValue(result.totalHours);
        },
        complete: function () {
            //errorMessage = "";
            //$("#GridMessageErrorDetail").hide();
            hideLoading();
        }
    });
}

function ShowCurrentItem(enabled) {
    var data = {
        id: $('#id_nonProductiveHour').val(),
        id_turn: 0,
        emissionDate: null,
        id_machineForProd: 0,
        enabled: enabled
    };

    showPage("NonProductiveHour/Edit", data);
}

function AddNewItem() {
    $.ajax({
        url: "NonProductiveHour/PendingNew",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            //// 
            NotifyError("Error. " + error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $("#maincontent").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

function EditCurrentItem() {
    showLoading();
    ShowCurrentItem(true);
}

function SaveCurrentItem() {
    SaveItem(false);
}

function AprovedItem() {
    showLoading();
    $.ajax({
        url: 'NonProductiveHour/Approve',
        type: 'post',
        data: { id: $('#id_nonProductiveHour').val() },
        async: true,
        cache: false,
        error: function (result) {
            hideLoading();
            NotifyError("Error. " + result.Message);
        },
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error al Aprobar. " + result.Message);
                return;
            }

            ShowCurrentItem(false);
            hideLoading();
            NotifySuccess("Control de horas de trabajo por máquina Aprobado Satisfactoriamente. " + "Estado: " + result.Data);
        }
    });
}

function AprovedCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Aprobar el Control de horas de trabajo por máquina?", "Confirmar");
    result.done(function (dialogResult) {

        if (dialogResult) {
            if ($("#enabled").val() == "true") {
                SaveItem(true);
            } else {
                AprovedItem();
            }
        }
    });
}

function ReverseCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Reversar el Control de horas de trabajo por máquina?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'NonProductiveHour/Reverse',
                type: 'post',
                data: { id: $('#id_nonProductiveHour').val() },
                async: true,
                cache: false,
                error: function (result) {
                    hideLoading();
                    NotifyError("Error. " + result.Message);
                },
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error al Reversar. " + result.Message);
                        return;
                    }

                    ShowCurrentItem(false);
                    hideLoading();
                    NotifySuccess("Control de horas de trabajo por máquina Reversado Satisfactoriamente. " + "Estado: " + result.Data);
                }
            });
        }
    });
}

function AnnulCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Anular el Control de horas de trabajo por máquina?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'NonProductiveHour/Annul',
                type: 'post',
                data: { id: $('#id_nonProductiveHour').val() },
                async: true,
                cache: false,
                error: function (result) {
                    hideLoading();
                    NotifyError("Error. " + result.Message);
                },
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error al Anular. " + result.Message);
                        return;
                    }

                    ShowCurrentItem(false);
                    hideLoading();
                    NotifySuccess("Control de horas de trabajo por máquina Anulado Satisfactoriamente. " + "Estado: " + result.Data);
                }
            });
        }
    });
}

function SaveDataUser() {

    var userData = {
        id: $('#id_nonProductiveHour').val(),
        //id_machineProdOpening: $('#id_machineProdOpening').val(),
        dateTimeEmision: DateTimeEmision.GetValue(),
        description: MemoDescription.GetText()
        //Detail

    };

    var NonProductiveHour = {
        jsonNonProductiveHour: JSON.stringify(userData)
    };

    return NonProductiveHour;
}

function SaveItem(aproved) {
    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }

    $.ajax({
        url: 'NonProductiveHour/Save',
        type: 'post',
        data: SaveDataUser(),
        async: true,
        cache: false,
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error. " + result.Message);
                return;
            }

            var id = result.Data;
            $('#id_nonProductiveHour').val(id);

            if (aproved)
                AprovedItem();
            else
                ShowCurrentItem(true);

            hideLoading();
            NotifySuccess("Control de horas de trabajo por máquina Guardado Satisfactoriamente.");
        },
        error: function (result) {
            hideLoading();
        }
    });
}

function IsValid(object) {
    return (object != null && object != undefined && object != "undefined");
}

function Validate() {
    var validate = true;
    var errors = "";

    if (!IsValid(DateTimeEmision) || DateTimeEmision.GetValue() == null) {
        errors += "Fecha Emisión es un campo Obligatorio. \n\r";
        validate = false;
    }

    if (validate == false) {
        NotifyError("Error. " + errors);
    }

    return validate;
}

function ButtonUpdate_Click() {
    SaveItem(false);
}

function ButtonCancel_Click() {
    RedirecBack();
}

function RedirecBack() {
    showPage("NonProductiveHour/Index");
}

function InitializePagination() {

    if ($("#id_nonProductiveHour").val() !== 0) {

        var current_page = 1;
        var max_page = 1;
        $.ajax({
            url: "NonProductiveHour/InitializePagination",
            type: "post",
            data: { id: $("#id_nonProductiveHour").val() },
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
            },
            success: function (result) {
                max_page = result.maximunPages;
                current_page = result.currentPage;
            },
            complete: function () {
            }
        });

        $('.pagination').jqPagination({
            current_page: current_page,
            max_page: max_page,
            page_string: "{current_page} de {max_page}",
            paged: function (page) {
                showPage("NonProductiveHour/Pagination", { page: page });
            }
        });
    }
}

function Init() {
}

$(function () {
    InitializePagination();
    Init();
});