
// VALIDATIONS

function DateValidation(expirationDateAux, receptionDateAux) {

    var expirationDateFullYear = expirationDateAux.getFullYear();
    var expirationDateMonth = expirationDateAux.getMonth();
    var expirationDateDay = expirationDateAux.getDate();
    var receptionDateAuxFullYear = receptionDateAux.getFullYear();
    var receptionDateAuxMonth = receptionDateAux.getMonth();
    var receptionDateAuxDay = receptionDateAux.getDate();
    if (expirationDateFullYear < receptionDateAuxFullYear) {
        return ("La Fecha de Caducidad debe ser mayor que la Fecha de Recepción del Lote");
    }
    else {
        if (expirationDateFullYear == receptionDateAuxFullYear && expirationDateMonth < receptionDateAuxMonth) {
            return ("La Fecha de Caducidad debe ser mayor que la Fecha de Recepción del Lote");
        }
        else {
            if (expirationDateFullYear == receptionDateAuxFullYear && expirationDateMonth == receptionDateAuxMonth && expirationDateDay <= receptionDateAuxDay) {
                return ("La Fecha de Caducidad debe ser mayor que la Fecha de Recepción del Lote");
            }
            //else {
            //    if (expirationDateFullYear == receptionDateAuxFullYear && expirationDateMonth == receptionDateAuxMonth && expirationDateDay < receptionDateAuxDay) {
            //        return ("La Fecha de expiración debe ser mayor que la Fecha de Recepción del Lote");
            //    }
                else {
                    return ("Ok");
                }
            //}
        }
    }
}
function Turn_SelectedIndexChanged(s, e) {
    $.ajax({
        url: "ProductionLotProcess/GetTimesTurn",
        type: "post",
        data: { id_turn: id_Turn.GetValue() },
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
                timeInitTurn.SetValue(new Date(2011, 1, 1, result.timeInitTurn.Hours, result.timeInitTurn.Minutes, result.timeInitTurn.Seconds))
                timeEndTurn.SetValue(new Date(2011, 1, 1, result.timeEndTurn.Hours, result.timeEndTurn.Minutes, result.timeEndTurn.Seconds))
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}
function OnProcessLotTurnValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function OnExpirationDateValidation(s, e) {
    let msg = "Ok";

    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        let dia = receptionDate.GetValue().getDate();
        let mesIndex = receptionDate.GetValue().getMonth();
        let anio = receptionDate.GetValue().getFullYear();
        let fechaProceso = new Date(anio, mesIndex, dia);
        let fechaCaducidad = e.value;

        if (fechaCaducidad < fechaProceso)
        {
            msg = "Fecha de Caducidad no puede ser menor a la fecha del proceso";
        }

        if (s.cpMaxDiasCaducidad > 0)
        {
            let nValuem = (fechaProceso.getDate() + s.cpMaxDiasCaducidad);
            fechaProceso.setDate(nValuem );
            if (fechaCaducidad > fechaProceso)
            {
                msg = `Fecha de Caducidad no puede exceder en ${s.cpMaxDiasCaducidad} días a la fecha del proceso`;
            }
        }
        
        
        //var msg = DateValidation(e.value, receptionDate.GetValue())
        if (msg != "Ok") {
            e.isValid = false;
            e.errorText = msg;
        }
    }
        
}

function OnWarehouseProductionLotLiquidationDetailValidation(s, e) {

    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function ComboWarehouseProductionLotLiquidationDetailTransfer_SelectedIndexChanged(s, e) {
    id_wareHouseLocationTransfer.SetValue(null);
    id_wareHouseLocationTransfer.ClearItems();
    var data = {
        id_wareHouse: s.GetValue()
    };

    $.ajax({
        url: "ProductionLotProcess/UpdateProductionLotWarehouseLocation",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                var arrayFieldStr = [];
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_wareHouseLocationTransfer, result.warehouseLocations, arrayFieldStr);
                //RefreshComboBoxWareHouse()
            }
        },
        complete: function () {
        }
    });
}
function ComboWarehouseLocationProductionLotLiquidationDetailTransfer_SelectedIndexChanged(s, e) {
    RefreshComboBoxWareHouse()
}


function ComboWarehouseLocationProductionLotLiquidationDetailTranfer_Init(s, e) {
    var data = {
        id_wareHouse: id_wareHouseTransfer.GetValue(),
        id_wareHouseLocation: s.GetValue()
    };
    $.ajax({
        url: "ProductionLotProcess/GetProductionLotWarehouseLocation",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_wareHouseLocationTransfer, [], arrayFieldStr);
        },
        beforeSend: function () {
        },
        success: function (result) {
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_wareHouseLocationTransfer, result.warehouseLocations, arrayFieldStr);
        },
        complete: function () {
        }
    });
}
function OnWarehouseLocationProductionLotLiquidationDetailValidation(s, e) {

    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}
function ComboWarehouseProductionLotLiquidationDetail_SelectedIndexChanged(s, e) {

    id_wareHouseLocation.SetValue(null);
    id_wareHouseLocation.ClearItems();
    var data = {
        id_wareHouser: s.GetValue()
    };

    $.ajax({
        url: "ProductionLotProcess/UpdateProductionLotWarehouseLocation",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                var arrayFieldStr = [];
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_wareHouseLocation, result.warehouseLocations, arrayFieldStr);
            }
        },
        complete: function () {
        }
    });
}
function ComboWarehouseProductionLotLiquidationDetailTransferDetail_SelectedIndexChanged(s, e) {
    id_wareHouseLocationDetailTransfer.SetValue(null);
    id_wareHouseLocationDetailTransfer.ClearItems();
    var data = {
        id_wareHouse: s.GetValue()
    };

    $.ajax({
        url: "ProductionLotProcess/UpdateProductionLotWarehouseLocation",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                var arrayFieldStr = [];
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_wareHouseLocationDetailTransfer, result.warehouseLocations, arrayFieldStr);
            }
        },
        complete: function () {
        }
    });
}
function ComboWarehouseLocationProductionLotLiquidationDetail_Init(s, e) {
    var data = {
        id_wareHouse: id_wareHouse.GetValue(),
        id_wareHouseLocation: s.GetValue()
    };
    $.ajax({
        url: "ProductionLotProcess/GetProductionLotWarehouseLocation",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_wareHouseLocation, [], arrayFieldStr);
        },
        beforeSend: function () {
        },
        success: function (result) {
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_wareHouseLocation, result.warehouseLocations, arrayFieldStr);
        },
        complete: function () {
        }
    });
}
function ComboWarehouseLocationProductionLotLiquidationDetail_Init(s, e) {
    var data = {
        id_wareHouse: id_wareHouse.GetValue(),
        id_wareHouseLocation: s.GetValue()
    };
    $.ajax({
        url: "ProductionLotProcess/GetProductionLotWarehouseLocation",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_wareHouseLocation, [], arrayFieldStr);
        },
        beforeSend: function () {
        },
        success: function (result) {
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_wareHouseLocation, result.warehouseLocations, arrayFieldStr);
        },
        complete: function () {
        }
    });
}
function ComboWarehouseLocationProductionLotLiquidationDetailTransfer_Init(s, e) {
    var data = {
        id_wareHouse: id_wareHouseDetailTransfer.GetValue(),
        id_wareHouseLocation: s.GetValue()
    };
    $.ajax({
        url: "ProductionLotProcess/GetProductionLotWarehouseLocation",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_wareHouseLocationDetailTransfer, [], arrayFieldStr);
        },
        beforeSend: function () {
        },
        success: function (result) {
            var arrayFieldStr = [];
            arrayFieldStr.push("name");
            UpdateDetailObjects(id_wareHouseLocationDetailTransfer, result.warehouseLocations, arrayFieldStr);
        },
        complete: function () {
        }
    });
}
function OnDischargeReasonValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio, Seleccione un Motivo de Inventario";
    }
}
function OnPersonProcessPlantValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio, Seleccione un Proceso";
    }
}
function OnIncomeReasonValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio, Seleccione un Motivo de Inventario";
    }
}
function RefreshComboBoxWareHouse() {
    if (id_wareHouseTransfer.GetValue() && id_wareHouseLocationTransfer.GetValue()) {
        gvProductionLotEditFormProductionLotLiquidationsTransferDetail.PerformCallback({ Act: true, id_wareHouse: id_wareHouseTransfer.GetValue(), id_wareHouseLocation : id_wareHouseLocationTransfer.GetValue() });
	}
}
// Cambio de Numero Juliano
function OnReceptionDateChanged(s, e) {
     

    var dateR = receptionDate.GetDate();
    var year = dateR.getFullYear();
    var month = dateR.getMonth() + 1;
    var day = dateR.getDate();

    var data = {
        id_pl: $("#id_productionLot").val(),
        intNumber: internalNumber.GetText(),
        yearR: year,
        monthR: month,
        dayR: day,
        emissionDate: receptionDate.GetDate().toISOString()
    }

    $.ajax({
        url: "ProductionLotProcess/UpdateProductionLotJulianoNumber",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            if (result !== null) {
                julianoNumber.SetText(result.julianoNumberTmp);
                internalNumber.SetText(result.internalNumberTmp);
                internalNumberConcatenated.SetText(result.internalNumberConcatenatedTmp);
                id_MachineProdOpeningDetail.PerformCallback();
            }
        },
        complete: function () {
        }
    });

}