
function ComboBoxTurn_SelectedIndexChanged(s, e) {
    $.ajax({
        url: "Headless/TurnChanged",
        type: "post",
        data: { id_turn: ComboBoxTurn.GetValue() },
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
                if (result.message !== null && result.message !== "") {
                    NotifyError("Error. " + result.message);
                }
                $('#timeInitTurn').val(result.timeInitTurn);
                $('#timeEndTurn').val(result.timeEndTurn);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function RefreshDetail(s, e) {
    GridViewDetails.PerformCallback();
}

function ShowCurrentItem(enabled) {
    var data = {
        id: $('#id_headless').val(),
        id_productionLot: 0,
        enabled: enabled
    };

    showPage("Headless/Edit", data);
}

function AddNewItem() {
    $.ajax({
        url: "Headless/PendingNew",
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
        url: 'Headless/Approve',
        type: 'post',
        data: { id: $('#id_headless').val() },
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
            //hideLoading();
            NotifySuccess("Descabezado Aprobado Satisfactoriamente. " + "Estado: " + result.Data);
        }
    });
}

function AprovedCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Aprobar el Descabezado?", "Confirmar");
    result.done(function (dialogResult) {

        if (dialogResult) {
            if ($("#enabled").val() === "true") {
                SaveItem(true);
            } else {
                AprovedItem();
            }
        }
    });
}

function ReverseCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Reversar el Descabezado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'Headless/Reverse',
                type: 'post',
                data: { id: $('#id_headless').val() },
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
                    //hideLoading();
                    NotifySuccess("Descabezado Reversado Satisfactoriamente. " + "Estado: " + result.Data);
                }
            });
        }
    });
}

function AnnulCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Anular el Descabezado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'Headless/Annul',
                type: 'post',
                data: { id: $('#id_headless').val() },
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
                    //hideLoading();
                    NotifySuccess("Descabezado Anulado Satisfactoriamente. " + "Estado: " + result.Data);
                }
            });
        }
    });
}

function SaveDataUser() {
    var emisionDate = DateTimeEmision.GetValue();
    var yearEmisionDate = emisionDate.getFullYear();
    var monthEmisionDate = emisionDate.getMonth();
    var dayEmisionDate = emisionDate.getDate();

    var dateStartTimeAux = dateStartTime.GetDate();
    var yearDateStartTime = dateStartTimeAux.getFullYear();
    var monthDateStartTime = dateStartTimeAux.getMonth();
    var dayDateStartTime = dateStartTimeAux.getDate();

    var endDateTimeAux = endDateTime.GetDate();
    var yearEndDateTime = endDateTimeAux.getFullYear();
    var monthEndDateTime = endDateTimeAux.getMonth();
    var dayEndDateTime = endDateTimeAux.getDate();

    var userData = {
        id: $('#id_headless').val(),
        dateTimeEmision: yearEmisionDate + "-" +
            (++monthEmisionDate).toString().padStart(2, 0) + "-" +
            dayEmisionDate.toString().padStart(2, 0) + "T00:00:00",
        dateStartTime: yearDateStartTime + "-" +
            (++monthDateStartTime).toString().padStart(2, 0) + "-" +
            dayDateStartTime.toString().padStart(2, 0) + "T" +
            dateStartTimeAux.getHours().toString().padStart(2, 0) + ":" +
            dateStartTimeAux.getMinutes().toString().padStart(2, 0) + ":00",
        endDateTime: yearEndDateTime + "-" +
            (++monthEndDateTime).toString().padStart(2, 0) + "-" +
            dayEndDateTime.toString().padStart(2, 0) + "T" +
            endDateTimeAux.getHours().toString().padStart(2, 0) + ":" +
            endDateTimeAux.getMinutes().toString().padStart(2, 0) + ":00",
        id_turn: ComboBoxTurn.GetValue(),
        id_programmer: ComboBoxProgrammer.GetValue(),
        id_supervisor: ComboBoxSupervisor.GetValue(),
        noOfPeople: noOfPeople.GetValue(),
        grammage: grammage.GetValue(),
        id_color: ComboBoxColor.GetValue(),
        manualPerformance: manualPerformance.GetValue(),
        noOfDrawers: noOfDrawers.GetValue(),
        lbsWholeSurplus: lbsWholeSurplus.GetValue(),
        lbsDirect: lbsDirect.GetValue(),
        description: MemoDescription.GetText()
    };

    var Headless = {
        jsonHeadless: JSON.stringify(userData)
    };

    return Headless;
}

function SaveItem(aproved) {
    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }

    $.ajax({
        url: 'Headless/Save',
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
            $('#id_headless').val(id);

            if (aproved)
                AprovedItem();
            else {
                ShowCurrentItem(true);

                //hideLoading();
            }
            NotifySuccess("El Descabezado Guardado Satisfactoriamente.");
        },
        error: function (result) {
            hideLoading();
        }
    });
}

function IsValid(object) {
    return (object !== null && object !== undefined && object !== "undefined");
}

function Validate() {
    var validate = true;
    var errors = "";

    if (!IsValid(DateTimeEmision) || DateTimeEmision.GetValue() === null) {
        errors += "Fecha Emisión es un campo Obligatorio. \n\r";
        validate = false;
    } else {
        if (!IsValid(ComboBoxTurn) || ComboBoxTurn.GetValue() === null) {
            errors += "Turno es un campo Obligatorio. \n\r";
            validate = false;
        } else {
            if (!IsValid(dateStartTime) || dateStartTime.GetValue() === null) {
                errors += "Fecha - Hora Inicio es un campo Obligatorio. \n\r";
                validate = false;
            } else {
                var emisionDate = DateTimeEmision.GetValue();
                var yearEmisionDate = emisionDate.getFullYear();
                var monthEmisionDate = emisionDate.getMonth();
                var dayEmisionDate = emisionDate.getDate();

                //// 
                if ($('#timeInitTurn').val() !== null && $('#timeInitTurn').val() !== "" && $('#timeEndTurn').val() !== null && $('#timeEndTurn').val() !== "") {
                    var dateStartTimeAux = dateStartTime.GetDate();
                    var yearDateStartTime = dateStartTimeAux.getFullYear();
                    var monthDateStartTime = dateStartTimeAux.getMonth();
                    var dayDateStartTime = dateStartTimeAux.getDate();
                    var aDateStartTimeAux = new Date(yearDateStartTime, monthDateStartTime, dayDateStartTime, dateStartTimeAux.getHours(), dateStartTimeAux.getMinutes(), 0);

                    var timeInitTurnAux = $('#timeInitTurn').val();
                    var timeInitTurnAuxArray = timeInitTurnAux.split(":");
                    var dateInitTurnWhithTimeAux = new Date(yearEmisionDate, monthEmisionDate, dayEmisionDate, timeInitTurnAuxArray[0], timeInitTurnAuxArray[1], 0);

                    var timeEndTurnAux = $('#timeEndTurn').val();
                    var timeEndTurnAuxArray = timeEndTurnAux.split(":");
                    var dateEndTurnWhithTimeAux = new Date(yearEmisionDate, monthEmisionDate, dayEmisionDate, timeEndTurnAuxArray[0], timeEndTurnAuxArray[1], 0);

                    if (!ValidateRangeTime(dateInitTurnWhithTimeAux, dateEndTurnWhithTimeAux, false)) {
                        dateEndTurnWhithTimeAux = new Date(yearEmisionDate, monthEmisionDate, dayEmisionDate + 1, timeEndTurnAuxArray[0], timeEndTurnAuxArray[1], 0);
                    }
                    var emisionDateStr = dayEmisionDate.toString().padStart(2, 0) + "/" + (++monthEmisionDate).toString().padStart(2, 0) + "/" + yearEmisionDate;

                    if (dateInitTurnWhithTimeAux > aDateStartTimeAux || dateEndTurnWhithTimeAux < aDateStartTimeAux) {
                        errors += "Fecha - Hora Inicio debe estar dentro del horario del turno: " + ComboBoxTurn.GetText() + ", con la fecha de emisión del documento(" + emisionDateStr + "). \n\r";
                        validate = false;
                    } else {
                        if (!IsValid(endDateTime) || endDateTime.GetValue() === null) {
                            errors += "Fecha - Hora Fin es un campo Obligatorio. \n\r";
                            validate = false;
                        } else {
                            //// 
                            var endDateTimeAux = endDateTime.GetDate();
                            var yearEndDateTime = endDateTimeAux.getFullYear();
                            var monthEndDateTime = endDateTimeAux.getMonth();
                            var dayEndDateTime = endDateTimeAux.getDate();
                            var aEndDateTimeAux = new Date(yearEndDateTime, monthEndDateTime, dayEndDateTime, endDateTimeAux.getHours(), endDateTimeAux.getMinutes(), 0);

                            if (aDateStartTimeAux >= aEndDateTimeAux || dateEndTurnWhithTimeAux < aEndDateTimeAux) {
                                errors += "Fecha - Hora Fin debe estar dentro del horario del turno: " + ComboBoxTurn.GetText() + ", con la fecha de emisión del documento(" + emisionDateStr + ") y la hora mayor a la del Inicio. \n\r";
                                validate = false;
                            }
                        }
                    }
                }
            }
        }
    }
    if (!IsValid(ComboBoxProgrammer) || ComboBoxProgrammer.GetValue() === null) {
        errors += "Programador es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(ComboBoxSupervisor) || ComboBoxSupervisor.GetValue() === null) {
        errors += "Supervisor es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(noOfPeople) || noOfPeople.GetValue() === null) {
        errors += "No. de Personas es un campo Obligatorio. \n\r";
        validate = false;
    } else {
        if (noOfPeople.GetValue() <= 0) {
            errors += "No. de Personas no es un valor correcto. \n\r";
            validate = false;
        }
    }
    if (!IsValid(grammage) || grammage.GetValue() === null) {
        errors += "Gramaje es un campo Obligatorio. \n\r";
        validate = false;
    } else {
        if (grammage.GetValue() <= 0.00) {
            errors += "Gramaje no es un valor correcto. \n\r";
            validate = false;
        }
    }
    if (!IsValid(ComboBoxColor) || ComboBoxColor.GetValue() === null) {
        errors += "Color es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(manualPerformance) || manualPerformance.GetValue() === null) {
        errors += "Rendimiento Manual es un campo Obligatorio. \n\r";
        validate = false;
    } else {
        if (manualPerformance.GetValue() <= 0.00) {
            errors += "Rendimiento Manual no es un valor correcto. \n\r";
            validate = false;
        }
    }
    if (!IsValid(noOfDrawers) || noOfDrawers.GetValue() === null) {
        errors += "No. de Gavetas es un campo Obligatorio. \n\r";
        validate = false;
    } else {
        if (noOfDrawers.GetValue() <= 0) {
            errors += "No. de Gavetas no es un valor correcto. \n\r";
            validate = false;
        }
    }
    if ($('#isWholeShrimp').val() === "True") {
        if (!IsValid(lbsWholeSurplus) || lbsWholeSurplus.GetValue() === null) {
            if (!IsValid(lbsDirect) || lbsDirect.GetValue() === null) {
                errors += "Lbs. Sobrante Entero o Lbs. Directo debe ser Obligatorio. \n\r";
                validate = false;
            } else {
                if (lbsDirect.GetValue() <= 0) {
                    errors += "Lbs. Directo no es un valor correcto. \n\r";
                    validate = false;
                }
            }
            //errors += "Lbs. Sobrante Entero es un campo Obligatorio. \n\r";
            //validate = false;
        } else {
            if (!IsValid(lbsDirect) || lbsDirect.GetValue() === null) {
                if (lbsWholeSurplus.GetValue() <= 0) {
                    errors += "Lbs. Sobrante Entero no es un valor correcto. \n\r";
                    validate = false;
                }
                //errors += "Lbs. Sobrante Entero o Lbs. Directo debe ser Obligatorio. \n\r";
                //validate = false;
            } else {
                errors += "Solo es obligatorio uno de los dos, Lbs. Sobrante Entero o Lbs. Directo. \n\r";
                validate = false;
                //if (lbsDirect.GetValue() <= 0) {
                //    errors += "Lbs. Directo no es un valor correcto. \n\r";
                //    validate = false;
                //}
            }
            //if (lbsWholeSurplus.GetValue() <= 0) {
            //    errors += "Lbs. Sobrante Entero no es un valor correcto. \n\r";
            //    validate = false;
            //}
        }
    } else {
        if (!IsValid(lbsDirect) || lbsDirect.GetValue() === null) {
            errors += "Lbs. Directo es un campo Obligatorio. \n\r";
            validate = false;
        } else {
            if (lbsDirect.GetValue() <= 0) {
                errors += "Lbs. Directo no es un valor correcto. \n\r";
                validate = false;
            }
        }
    }

    if (validate === false) {
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
    showPage("Headless/Index");
}

function InitializePagination() {

    if ($("#id_headless").val() !== 0) {

        var current_page = 1;
        var max_page = 1;
        $.ajax({
            url: "Headless/InitializePagination",
            type: "post",
            data: { id: $("#id_headless").val() },
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
                showPage("Headless/Pagination", { page: page });
            }
        });
    }
}

function PrintItem() {
    $.ajax({
        url: "Headless/PrintReport",
        type: "post",
        data: {
			id_headless: $("#id_headless").val(),
			codeReport: "DESCA"
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

function Init() {
}

$(function () {
    InitializePagination();
    Init();
});