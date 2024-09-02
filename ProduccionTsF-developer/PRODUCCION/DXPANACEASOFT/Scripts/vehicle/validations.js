

function OnVehicleCarRegistrationValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else if(e.value.length > 20) {
        e.isValid = false;
        e.errorText = "Máximo 20 caracteres";
    }  else {
            $.ajax({
                url: "Vehicle/ValidateCarRegistrationVehicle",
                type: "post",
                async: false,
                cache: false,
                data: {
                    id_vehicle: gvVehicle.cpEditingRowKey,
                    carRegistration: e.value
                },
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    //showLoading();
                },
                success: function (result) {
                    e.isValid = result.isValid;
                    e.errorText = result.errorText;
                },
                complete: function () {
                    //hideLoading();
                }
            });
        } 
}


function OnVehicleModelValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
     } else if(e.value.length > 50) {
    e.isValid = false;
    e.errorText = "Máximo 50 caracteres";
     }
}


function OnVehicleMarkValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
      } else if(e.value.length > 50) {
    e.isValid = false;
    e.errorText = "Máximo 50 caracteres";
}
}


function OnVehicleType_SelectedIndexChanged(s, e) {
    if (e.value === null) {
        id_shippingTypeT.SetValue("");
    }
    else {
        id_shippingTypeT.SetValue(id_VehicleType.GetSelectedItem().GetColumnText("shippingType"));
    }
}