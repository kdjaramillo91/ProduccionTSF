function ComboBoxTurn_SelectedIndexChanged(s, e) {
    $.ajax({
        url: "OpeningClosingPlateLying/TurnChanged",
        type: "post",
        data: { id_turn: s.GetValue() },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
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
            hideLoading();
        }
    });
}

function CheckBoxTunnelTransferPlate_CheckedChanged(s, e) {
    ComboBoxFreezerMachineForProdDestination.SetEnabled(s.GetChecked());
    ComboBoxFreezerMachineForProdDestination.SetValue(null);
    RefreshDetail(s, e);

    var isTunnelTransferPlate = s.GetChecked();
    ComboBoxDestinyWarehouse.SetValue(null);
    ComboBoxDestinyWarehouseLocation.SetValue(null);
    ComboBoxDestinyWarehouseLocation.SetEnabled(!isTunnelTransferPlate);

    if (ComboBoxFreezerMachineForProd.cpHabModificacionUbiDestino && !isTunnelTransferPlate) {
        ActualizarBodegaUbicacionDestinoDesdeTunel(ComboBoxFreezerMachineForProd.GetValue());
	}
}

function ComboBoxFreezerMachineForProdDestination_SelectedIndexChanged(s, e) {
    RefreshDetail(s, e);
}

function ComboBoxFreezerMachineForProdDestination_BeginCallback(s, e) {
    e.customArgs["id_freezerMachineForProdOrigin"] = ComboBoxFreezerMachineForProd.GetValue() === undefined ? null : ComboBoxFreezerMachineForProd.GetValue();
}

function ComboBoxFreezerMachineForProdDestination_EndCallback(s, e) {
    RefreshDetail(s, e);
}

function ComboBoxFreezerMachineForProdDestination_Init(s, e) {
    s.SetEnabled(tunnelTransferPlate.GetChecked());
}

function ComboBoxFreezerWarehouse_BeginCallback(s, e) {
    e.customArgs["id_freezerMachineForProd"] = ComboBoxFreezerMachineForProd.GetValue() === undefined ? null : ComboBoxFreezerMachineForProd.GetValue();
}

function ComboBoxFreezerWarehouse_EndCallback(s, e) {
    TokenBoxFreezerWarehouseLocations.PerformCallback();
}

function ComboBoxFreezerMachineForProd_SelectedIndexChanged(s, e) {
    ComboBoxFreezerWarehouse.PerformCallback();
    ComboBoxFreezerMachineForProdDestination.PerformCallback();

    if (s.cpHabModificacionUbiDestino) {
        ActualizarBodegaUbicacionDestinoDesdeTunel(s.GetValue());
	}
}

function ActualizarBodegaUbicacionDestinoDesdeTunel(idTunel) {
    ComboBoxDestinyWarehouse.Clear();
    ComboBoxDestinyWarehouse.SetValue(null);

    ComboBoxDestinyWarehouseLocation.Clear();
    ComboBoxDestinyWarehouseLocation.SetValue(null);

    if (idTunel) {
        $.ajax({
            url: "OpeningClosingPlateLying/GetWarehouseCartoningProcess",
            type: "post",
            data: { idTunel: idTunel },
            async: true,
            cache: false,
            error: function (error) {
                NotifyError("Error. " + error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result.message !== "OK") {
                    NotifyError("Error. " + result.message);
                } else {
                    ComboBoxDestinyWarehouse.AddItem(result.name_warehouseCarton, result.id_warehouseCarton);
                    ComboBoxDestinyWarehouse.SetValue(result.id_warehouseCarton);

                    ComboBoxDestinyWarehouseLocation.PerformCallback({ idWarehouse: result.id_warehouseCarton});
				}
            },
            complete: function () {
                hideLoading();
            }
        });
	}
}

function TokenBoxFreezerWarehouseLocations_BeginCallback(s, e) {
    e.customArgs["id_freezerMachineForProd"] = ComboBoxFreezerMachineForProd.GetValue() === undefined ? null : ComboBoxFreezerMachineForProd.GetValue();
    e.customArgs["id_freezerWarehouse"] = ComboBoxFreezerWarehouse.GetValue() === undefined ? null : ComboBoxFreezerWarehouse.GetValue();
}

function TokenBoxFreezerWarehouseLocations_EndCallback(s, e) {
    TokenBoxLots.PerformCallback();
}
var RefreshDetailByFreezerWarehouse = false;
function ComboBoxFreezerWarehouse_SelectedIndexChanged(s, e) {
    RefreshDetailByFreezerWarehouse = true;
    TokenBoxFreezerWarehouseLocations.PerformCallback();
}

function ComboBoxDestinyWarehouseLocation_BeginCallback(s, e) {
    e.customArgs["idWarehouse"] = ComboBoxDestinyWarehouse.GetValue();
    e.customArgs["idWarehouseLocation"] = ComboBoxDestinyWarehouseLocation.GetValue();
    e.customArgs["enabled"] = s.cpEnabled;
}

function ComboBoxDestinyWarehouseLocation_SelectedIndexChanged(s, e) {
    RefreshDetail(s, e);
}

function TokenBoxFreezerWarehouseLocations_ValueChanged(s, e) {
    RefreshDetail(s, e);
}

function TokenBoxProductionCarts_ValueChanged(s, e) {
    RefreshDetail(s, e);
}

function TokenBoxItems_ValueChanged(s, e) {
    RefreshDetail(s, e);
}

function TokenBoxLots_ValueChanged(s, e) {
    RefreshDetail(s, e);
}

function TokenBoxLots_BeginCallback(s, e) {
    e.customArgs["ids_lot"] = null;
    e.customArgs["id_freezerWarehouse"] = ComboBoxFreezerWarehouse.GetValue() === undefined ? null : ComboBoxFreezerWarehouse.GetValue();
    e.customArgs["id_freezerMachineForProd"] = ComboBoxFreezerMachineForProd.GetValue() === undefined ? null : ComboBoxFreezerMachineForProd.GetValue();
    e.customArgs["ids_freezerWarehouseLocation"] = TokenBoxFreezerWarehouseLocations.GetTokenValuesCollection() === undefined ? [] : TokenBoxFreezerWarehouseLocations.GetTokenValuesCollection();
}

function TokenBoxLots_EndCallback(s, e) {
    if (RefreshDetailByFreezerWarehouse) {
        RefreshDetailByFreezerWarehouse = false;
        RefreshDetail(s, e);
    }
}

function OnGridViewDetailBeginCallback(s, e) {
    e.customArgs["id_freezerMachineForProd"] = ComboBoxFreezerMachineForProd.GetValue() === undefined ? null : ComboBoxFreezerMachineForProd.GetValue();
    e.customArgs["id_freezerWarehouse"] = ComboBoxFreezerWarehouse.GetValue() === undefined ? null : ComboBoxFreezerWarehouse.GetValue();
    e.customArgs["ids_freezerWarehouseLocation"] = TokenBoxFreezerWarehouseLocations.GetTokenValuesCollection() === undefined ? [] : TokenBoxFreezerWarehouseLocations.GetTokenValuesCollection();
    e.customArgs["ids_productionCart"] = TokenBoxProductionCarts.GetTokenValuesCollection() === undefined ? [] : TokenBoxProductionCarts.GetTokenValuesCollection();
    e.customArgs["ids_item"] = TokenBoxItems.GetTokenValuesCollection() === undefined ? [] : TokenBoxItems.GetTokenValuesCollection();
    e.customArgs["updateDetail"] = updateDetail;
    e.customArgs["id_freezerMachineForProdDestination"] = ComboBoxFreezerMachineForProdDestination.GetValue() === undefined ? null : ComboBoxFreezerMachineForProdDestination.GetValue();
    e.customArgs["tunnelTransferPlate"] = tunnelTransferPlate.GetChecked() === undefined ? false : tunnelTransferPlate.GetChecked();
    e.customArgs["ids_lot"] = TokenBoxLots.GetTokenValuesCollection() === undefined ? [] : TokenBoxLots.GetTokenValuesCollection();

    e.customArgs["id_warehouseDestiny"] = ComboBoxDestinyWarehouse.GetValue() === undefined ? [] : ComboBoxDestinyWarehouse.GetValue();
    e.customArgs["id_warehouseLocationDestiny"] = ComboBoxDestinyWarehouseLocation.GetValue() === undefined ? null : ComboBoxDestinyWarehouseLocation.GetValue();
}

function GetTotalBySelected() {
    $.ajax({
        url: "OpeningClosingPlateLying/GetTotalBySelected",
        type: "post",
        data: { ids: selectedRows },
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
            if (result.message !== "OK") {
                NotifyError("Error. " + result.message);
            }
            selectedQuantityStr.SetValue(result.selectedQuantityStr);
        },
        complete: function () {
            hideLoading();
        }
    });
}

function GetTotalBySelected_GetSelectedFieldDetailValuesCallback(values) {
    selectedRows = values;
    GetTotalBySelected();
}

var updateDetail = false;
var updateDetailSelected = false;

function OnGridViewDetailEndCallback(s, e) {
    if (updateDetail === true || updateDetailSelected === true) {
        updateDetail = false;
        updateDetailSelected = false;
        GridViewDetails.GetSelectedFieldValues("id", GetTotalBySelected_GetSelectedFieldDetailValuesCallback);
    }
}

function OnGridViewDetailInit(s, e) {
    updateDetail = false;
    GridViewDetails.SelectRows();
}

function OnGridViewDetailSelectionChanged(s, e) {
    updateDetailSelected = true;
    OnGridViewDetailEndCallback(s, e);
}

function RefreshDetail(s, e) {
    updateDetail = true;
    GridViewDetails.PerformCallback();
}

function ShowCurrentItem(enabled) {
    var data = {
        id: $('#id_openingClosingPlateLying').val(),
        enabled: enabled
    };

    showPage("OpeningClosingPlateLying/Edit", data);
}

function AddNewItem() {
    var data = {
        id: 0,
        enabled: true
    };
    showPage("OpeningClosingPlateLying/Edit", data);
}

function EditCurrentItem() {
    showLoading();
    ShowCurrentItem(true);
}

function SaveCurrentItem() {
    SaveItem(false);
}

const DOCUMENT_CODE_TUMBADA_PLACA = "37";

function AprovedItem() {
    showLoading();
    $.ajax({
        url: 'OpeningClosingPlateLying/Approve',
        type: 'post',
        data: { id: $('#id_openingClosingPlateLying').val() },
        async: true,
        cache: false,
        error: function (result)
        {
            hideLoading();
            NotifyError("Error. " + result.Message);
        },
        success: function (result)
        {
            
            if (result.Code == CODE_FOR_SCHEDULE_TRANSAC) {
                // bloquear botones
                // mensaje
                // iniciar observer
                hideLoading();
                callbackProcessControlState(false);
                observerNotification(DOCUMENT_CODE_TUMBADA_PLACA, 5000, callbackProcess);
                NotifySuccess(TRANSAC_FOR_QUEUE_MSG);
            }
            else if (result.Code !== 0 && result.Code != CODE_FOR_SCHEDULE_TRANSAC) {
                hideLoading();
                NotifyError("Error." + result.Message);
                
            }
            else {
                hideLoading();
                NotifySuccess("Proceso realizado Satisfactoriamente. " + "estado: " + result.Data);
                ShowCurrentItem(false);
                
            }

            //if (result.Code !== 0) {
            //    hideLoading();
            //    NotifyError("Error al Aprobar. " + result.Message);
            //    return;
            //}
            //
            //ShowCurrentItem(false);
            ////hideLoading();
            //NotifySuccess("Tumbada de Placa Aprobada Satisfactoriamente. " + "Estado: " + result.Data);
        }
    });
}


function callbackProcessControlState(isEnabled) {

    btnEdit.SetEnabled(isEnabled);
    btnAproved.SetEnabled(isEnabled);
    btnAnnul.SetEnabled(isEnabled);
    btnExit.SetEnabled(isEnabled);
}

function callbackProcess(status) {


    if (status == "APROBADA") {
        NotifySuccess("Proceso realizado Satisfactoriamente, estado: APROBADA");
        ShowCurrentItem(false);
    }
    else if (status == "PENDIENTE") {
        NotifyError("Ha ocurrido un error, revise las notificaciones");
        
        // habilitar botones
    }

    callbackProcessControlState(true);

}

function AprovedCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Aprobar la Tumbada de Placa?", "Confirmar");
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

// Conciliar
function ConciliateCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("¿Desea Conciliar la Tumbada de Planta?", "Confirmar");
    result.done(function (dialogResult) {

        if (dialogResult) {
            if ($("#enabled").val() == "true") {
                SaveItem(true);
            } else {
                ConciliateItem();
            }
        }
    });
}

function ConciliateItem() {
    showLoading();
    $.ajax({
        url: 'OpeningClosingPlateLying/Conciliate',
        type: 'post',
        data: { id: $('#id_openingClosingPlateLying').val() },
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
            NotifySuccess("Tumbada de Placa Conciliado Satisfactoriamente. " + "Estado: " + result.Data);
        }
    });
}

function ReverseCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Reversar la Tumbada de Placa?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'OpeningClosingPlateLying/Reverse',
                type: 'post',
                data: { id: $('#id_openingClosingPlateLying').val() },
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
                    NotifySuccess("Tumbada de Placa Reversada Satisfactoriamente. " + "Estado: " + result.Data);
                }
            });
        }
    });
}

function AnnulCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Anular la Tumbada de Placa?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'OpeningClosingPlateLying/Annul',
                type: 'post',
                data: { id: $('#id_openingClosingPlateLying').val() },
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
                    NotifySuccess("Tumbada de Placa Anulada Satisfactoriamente. " + "Estado: " + result.Data);
                }
            });
        }
    });
}

var selectedRows = [];

function GetSelectedFieldDetailValuesCallback(values) {
    selectedRows = values;//[];
}

function GetIdsSelected(s, e) {
    GridViewDetails.GetSelectedFieldValues("id", GetSelectedFieldDetailValuesCallback);
}

function SaveDataUser() {
    var emisionDate = DateTimeEmision.GetValue();
    var yearEmisionDate = emisionDate.getFullYear();
    var monthEmisionDate = emisionDate.getMonth();
    var dayEmisionDate = emisionDate.getDate();

    var dateTimeStartLyingAux = dateTimeStartLying.GetDate();
    var yearDateTimeStartLying = dateTimeStartLyingAux.getFullYear();
    var monthDateTimeStartLying = dateTimeStartLyingAux.getMonth();
    var dayDateTimeStartLying = dateTimeStartLyingAux.getDate();

    var dateTimeEndLyingAux = dateTimeEndLying.GetDate();
    var yearDateTimeEndLying = dateTimeEndLyingAux.getFullYear();
    var monthDateTimeEndLying = dateTimeEndLyingAux.getMonth();
    var dayDateTimeEndLying = dateTimeEndLyingAux.getDate();

    var userData = {
        id: $('#id_openingClosingPlateLying').val(),
        id_freezerMachineForProd: ComboBoxFreezerMachineForProd.GetValue(),
        id_freezerWarehouse: ComboBoxFreezerWarehouse.GetValue(),
        freezerWarehouse: ComboBoxFreezerWarehouse.GetText(),
        dateTimeEmision: yearEmisionDate + "-" +
            (++monthEmisionDate).toString().padStart(2, 0) + "-" +
            dayEmisionDate.toString().padStart(2, 0) + "T00:00:00",
        description: MemoDescription.GetText(),
        dateTimeStartLying: yearDateTimeStartLying + "-" +
            (++monthDateTimeStartLying).toString().padStart(2, 0) + "-" +
            dayDateTimeStartLying.toString().padStart(2, 0) + "T" +
            dateTimeStartLyingAux.getHours().toString().padStart(2, 0) + ":" +
            dateTimeStartLyingAux.getMinutes().toString().padStart(2, 0) + ":00",
        dateTimeEndLying: yearDateTimeEndLying + "-" +
            (++monthDateTimeEndLying).toString().padStart(2, 0) + "-" +
            dayDateTimeEndLying.toString().padStart(2, 0) + "T" +
            dateTimeEndLyingAux.getHours().toString().padStart(2, 0) + ":" +
            dateTimeEndLyingAux.getMinutes().toString().padStart(2, 0) + ":00",
        id_responsable: ComboBoxResponsable.GetValue(),
        temperature: temperature.GetValue(),
        ids_freezerWarehouseLocation: TokenBoxFreezerWarehouseLocations.GetTokenValuesCollection(),
        ids_productionCart: TokenBoxProductionCarts.GetTokenValuesCollection(),
        ids_item: TokenBoxItems.GetTokenValuesCollection(),
        ids_lot: TokenBoxLots.GetTokenValuesCollection(),
        selectedQuantity: selectedQuantityStr.GetValue(),
        id_turn: ComboBoxTurn.GetValue(),
        tunnelTransferPlate: tunnelTransferPlate.GetChecked(),
        id_freezerMachineForProdDestination: ComboBoxFreezerMachineForProdDestination.GetValue(),
        ids: selectedRows,
        id_warehouseDestiny: ComboBoxDestinyWarehouse.GetValue(),
        id_warehouseLocationDestiny: ComboBoxDestinyWarehouseLocation.GetValue(),
    };

    var OpeningClosingPlateLying = {
        jsonOpeningClosingPlateLying: JSON.stringify(userData)
    };

    return OpeningClosingPlateLying;
}

var aprovedGlobal = false;
function SaveItem_GetSelectedFieldDetailValuesCallback(values) {

    selectedRows = values;
    if (selectedRows.length < 1) {
        NotifyError("Debe seleccionar al menos un detalle. ");
        hideLoading();
        return;
    }
    $.ajax({
        url: 'OpeningClosingPlateLying/Save',
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
            $('#id_openingClosingPlateLying').val(id);

            if (aprovedGlobal)
                AprovedItem();
            else
                ShowCurrentItem(true);

            //hideLoading();
            NotifySuccess("La Tumbada de Placa Guardada Satisfactoriamente.");
        },
        error: function (result) {
            hideLoading();
        }
    });
}

function SaveItem(aproved) {
    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }
    aprovedGlobal = aproved;
    GridViewDetails.GetSelectedFieldValues("id", SaveItem_GetSelectedFieldDetailValuesCallback);


}

function IsValid(object) {
    return object !== null && object !== undefined && object !== "undefined";
}

function Validate() {
    var validate = true;
    var errors = "";

    if (!IsValid(DateTimeEmision) || DateTimeEmision.GetValue() === null) {
        errors += "Fecha Emisión es un campo Obligatorio. \n\r";
        validate = false;
    } else {
        var aDateTimeEmision = DateTimeEmision.GetValue();
        var yearDateTimeEmision = aDateTimeEmision.getFullYear();
        var monthDateTimeEmision = aDateTimeEmision.getMonth() + 1;
        var dayDateTimeEmision = aDateTimeEmision.getDate();
        var dateTimeEmisionAux = new Date(yearDateTimeEmision, aDateTimeEmision.getMonth(), dayDateTimeEmision);

        var dateHoy = $('#dateHoy').val().split("-");
        var yearHoyDate = parseInt(dateHoy[2]);
        var monthHoyDate = parseInt(dateHoy[1]);
        var dayHoyDate = parseInt(dateHoy[0]);
        var hoyDateAux = new Date(yearHoyDate, monthHoyDate - 1, dayHoyDate);

        var dateHoyMin = $('#dateHoyMin').val().split("-");
        var yearHoyMinDate = parseInt(dateHoyMin[2]);
        var monthHoyMinDate = parseInt(dateHoyMin[1]);
        var dayHoyMinDate = parseInt(dateHoyMin[0]);
        var hoyMinDateAux = new Date(yearHoyMinDate, monthHoyMinDate - 1, dayHoyMinDate);

        if (dateTimeEmisionAux.getTime() !== hoyDateAux.getTime() && dateTimeEmisionAux.getTime() !== hoyMinDateAux.getTime() &&
            (dateTimeEmisionAux < hoyMinDateAux || dateTimeEmisionAux > hoyDateAux)) {
            errors += "Fecha de emisión debe ser mayor o igual a fecha mínima y menor o igual a la fecha de hoy. \n\r";
            validate = false;
        }
    }
    if (!IsValid(ComboBoxResponsable) || ComboBoxResponsable.GetValue() === null) {
        errors += "Responsable es un campo Obligatorio. \n\r";
        validate = false;
    }

    if (!IsValid(dateTimeStartLying) || dateTimeStartLying.GetValue() === null) {
        errors += "Fecha Hora Inicio es un campo Obligatorio. \n\r";
        validate = false;
    } else {
        if (!IsValid(dateTimeEndLying) || dateTimeEndLying.GetValue() === null) {
            errors += "Fecha Hora Fin es un campo Obligatorio. \n\r";
            validate = false;
        } else {
            var aDateTimeStartLying = dateTimeStartLying.GetDate();
            var yearDateTimeStartLying = aDateTimeStartLying.getFullYear();
            var monthDateTimeStartLying = aDateTimeStartLying.getMonth();
            var dayDateTimeStartLying = aDateTimeStartLying.getDate();
            var dateTimeStartLyingAux = new Date(yearDateTimeStartLying, monthDateTimeStartLying, dayDateTimeStartLying, aDateTimeStartLying.getHours(), aDateTimeStartLying.getMinutes(), 0);

            var aDateTimeEndLying = dateTimeEndLying.GetDate();
            var yearDateTimeEndtLying = aDateTimeEndLying.getFullYear();
            var monthDateTimeEndtLying = aDateTimeEndLying.getMonth();
            var dayDateTimeEndtLying = aDateTimeEndLying.getDate();
            var dateTimeEndtLyingAux = new Date(yearDateTimeEndtLying, monthDateTimeEndtLying, dayDateTimeEndtLying, aDateTimeEndLying.getHours(), aDateTimeEndLying.getMinutes(), 0);

            if (dateTimeStartLyingAux > dateTimeEndtLyingAux || dateTimeStartLyingAux.getTime() === dateTimeEndtLyingAux.getTime()) {
                errors += "La Fecha Hora Fin debe ser mayor a la Fecha Hora Inicio. \n\r";
                validate = false;
            } else {
                if ($('#timeInitTurn').val() !== null && $('#timeInitTurn').val() !== "" && $('#timeEndTurn').val() !== null && $('#timeEndTurn').val() !== "") {
                    var timeInitTurnAux = $('#timeInitTurn').val();
                    var timeInitTurnAuxArray = timeInitTurnAux.split(":");
                    var dateInitTurnWhithTimeAux = new Date(yearDateTimeStartLying, monthDateTimeStartLying, dayDateTimeStartLying, timeInitTurnAuxArray[0], timeInitTurnAuxArray[1], 0);

                    var timeEndTurnAux = $('#timeEndTurn').val();
                    var timeEndTurnAuxArray = timeEndTurnAux.split(":");
                    var dateEndTurnWhithTimeAux = new Date(yearDateTimeStartLying, monthDateTimeStartLying, dayDateTimeStartLying, timeEndTurnAuxArray[0], timeEndTurnAuxArray[1], 0);

                    var aValidateRangeTime = ValidateRangeTime(dateInitTurnWhithTimeAux, dateEndTurnWhithTimeAux, false);

                    if (aValidateRangeTime && (dateInitTurnWhithTimeAux > dateTimeStartLyingAux || dateEndTurnWhithTimeAux < dateTimeStartLyingAux) ||
                        !aValidateRangeTime && dateInitTurnWhithTimeAux > dateTimeStartLyingAux && dateEndTurnWhithTimeAux < dateTimeStartLyingAux) {
                        errors += "La Fecha Hora Inicio debe estar dentro del horario del turno: " + ComboBoxTurn.GetText() + ". \n\r";
                        validate = false;
                    } else {
                        var aValidateRangeTimeAux = ValidateRangeTime(dateInitTurnWhithTimeAux, dateTimeStartLyingAux, false);
                        if (!aValidateRangeTimeAux) {
                            if (dateTimeStartLyingAux > dateTimeEndtLyingAux || dateEndTurnWhithTimeAux < dateTimeEndtLyingAux) {
                                errors += "La Fecha Hora Fin debe estar dentro del horario del turno: " + ComboBoxTurn.GetText() + ". \n\r";
                                validate = false;
                            }
                        } else {
                            if (aValidateRangeTime && (dateInitTurnWhithTimeAux > dateTimeEndtLyingAux || dateEndTurnWhithTimeAux < dateTimeEndtLyingAux) ||
                                !aValidateRangeTime && dateInitTurnWhithTimeAux > dateTimeEndtLyingAux && dateEndTurnWhithTimeAux < dateTimeEndtLyingAux) {
                                errors += "La Fecha Hora Fin debe estar dentro del horario del turno: " + ComboBoxTurn.GetText() + ". \n\r";
                                validate = false;
                            }
                        }
                    }
                }
            }
        }
    }

    //// 
    if (dateTimeStartLyingAux < dateTimeEmisionAux) {
        errors += "La Fecha Hora Inicio debe ser mayor o igual a la Fecha Emisión. \n\r";
        validate = false;
    }

    if (!IsValid(temperature) || temperature.GetValue() === null) {
        errors += "Temperatura es un campo Obligatorio. \n\r";
        validate = false;
    }

    if (!IsValid(ComboBoxTurn) || ComboBoxTurn.GetValue() === null) {
        errors += "Turno es un campo Obligatorio. \n\r";
        validate = false;
    }

    if (!IsValid(ComboBoxFreezerMachineForProd) || ComboBoxFreezerMachineForProd.GetValue() === null) {
        errors += "Túnel/Placa es un campo Obligatorio. \n\r";
        validate = false;
    }

    if (!IsValid(ComboBoxFreezerWarehouse) || ComboBoxFreezerWarehouse.GetValue() === null) {
        errors += "Bodega de Congelación es un campo Obligatorio. \n\r";
        validate = false;
    }

    if (IsValid(tunnelTransferPlate) && tunnelTransferPlate.GetChecked()) {
        if (!IsValid(ComboBoxFreezerMachineForProdDestination) || ComboBoxFreezerMachineForProdDestination.GetValue() === null) {
            errors += "Túnel – Placa de Transferencia es un campo Obligatorio. \n\r";
            validate = false;
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
    showPage("OpeningClosingPlateLying/Index");
}

function InitializePagination() {

    if ($("#id_openingClosingPlateLying").val() !== 0) {

        var current_page = 1;
        var max_page = 1;
        $.ajax({
            url: "OpeningClosingPlateLying/InitializePagination",
            type: "post",
            data: { id: $("#id_openingClosingPlateLying").val() },
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
                showPage("OpeningClosingPlateLying/Pagination", { page: page });
            }
        });
    }
}

function Init() {
    $("#btnCollapseFilterDetails").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
        }
    });

    if ($('#id_openingClosingPlateLying').val() !== 0 && $('#id_openingClosingPlateLying').val() !== "0") {
        $("#btnCollapseFilterDetails").click();
    }
}

$(function () {
    InitializePagination();
    Init();
});