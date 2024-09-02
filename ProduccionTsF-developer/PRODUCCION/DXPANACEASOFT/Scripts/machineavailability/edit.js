
function ShowCurrentItem(enabled) {
    var data = {
        id: $('#id_machineAvailabilityResultConsult').val(),
        enabled: enabled
    };

    showPage("MachineAvailability/Edit", data);
}

function SaveDataUser() {
    // 
    var userData = [];
    for (let rowDetail = 0; rowDetail < GridViewDetails.pageRowCount; rowDetail++) {
        if (GridViewDetails.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
        userData.push({
            id: GridViewDetails.batchEditApi.GetCellValue(rowDetail, 0),
            nameMachineForProd: GridViewDetails.batchEditApi.GetCellValue(rowDetail, 2),
            available: GridViewDetails.batchEditApi.GetCellValue(rowDetail, 6),
            reason: GridViewDetails.batchEditApi.GetCellValue(rowDetail, 7)
        });
    }

    var MachineAvailability = {
        jsonMachineAvailability: JSON.stringify(userData)
    };

    return MachineAvailability;
}

function SaveItem() {
    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }

    $.ajax({
        url: 'MachineAvailability/Save',
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
            $('#id_machineAvailabilityResultConsult').val(id);

            ShowCurrentItem(true);

            hideLoading();
            NotifySuccess("Disponibilidad de Máquina Guardada Satisfactoriamente.");
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

    for (let rowDetail = 0; rowDetail < GridViewDetails.pageRowCount; rowDetail++) {
        if (GridViewDetails.batchEditApi.GetCellValue(rowDetail, 0) === null) continue;
        var availableAux = GridViewDetails.batchEditApi.GetCellValue(rowDetail, 6);
        if (availableAux === true || availableAux === "True" || availableAux === "true") {
            GridViewDetails.batchEditApi.SetCellValue(rowDetail, "reason", "", "", true);
        } else {
            var reasonAux = GridViewDetails.batchEditApi.GetCellValue(rowDetail, 7);
            if (reasonAux === "" || reasonAux === null) {
                errors += "Debe definir un motivo de no Disponibilidad de la bodega: " + GridViewDetails.batchEditApi.GetCellValue(rowDetail, 2) +". \n\r";
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
    SaveItem();
}

function ButtonCancel_Click() {
    RedirecBack();
}

function RedirecBack() {
    showPage("MachineAvailability/Index");
}

function Init() {
}

$(function () {
    Init();
});