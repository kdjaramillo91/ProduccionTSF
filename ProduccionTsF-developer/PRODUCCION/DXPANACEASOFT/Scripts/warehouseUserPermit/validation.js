function OnUserValidation(s, e) {

    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function ComboCodeWarehouse_SelectedIndexChanged(s, e) {

    // 
    var data = id_entityValue.GetValue();
    if (data !== null && data !== undefined) {

        $.ajax({
            url: "WarehouseUserPermit/NameWarehouseChangeData",
            type: "post",
            data: { idBodega: data },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
            },
            success: function (result) {
                if (result !== null && result !== undefined) {
                    code_entityValue.SetValue(result.nameWarehouse);
                }
            },
            complete: function () {

            }
        });
    }
}

function UserCombo_SelectedIndexChanged(s, e) {

    id_employee.SetValue(null);
    id_employee.ClearItems();

    var data = s.GetValue();
    if (data === null) {
        return;
    }

    if (data !== null) {

        $.ajax({
            url: "WarehouseUserPermit/UserEmployeeChangeData",
            type: "post",
            data: { id_usuario: data },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {

            },
            success: function (result) {
                // 
                for (var i = 0; i < result.length; i++) {
                    var comboAux = result[i];
                    id_employee.AddItem(comboAux.fullname_businessName, comboAux.id);
                }
            },
            complete: function () {

            }
        });
    }
}


var errorMessage = "";
var id_warehouseProdIniAux = null;

function OnWarehouseDetailValidation(s, e) {

    errorMessage = "";

    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "Bodega es obligatorio.";
        }
    } else {
        var data = {
            id_bodega: s.GetValue(),
            id_user: id_user.GetValue()
        };
        if (data.id_bodega != id_warehouseProdIniAux)
        {
            $.ajax({
                url: "WarehouseUserPermit/ItsRepeatedDetail",
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
                        if (result.itsRepeated == 1) {
                            e.isValid = false;
                            e.errorText = result.Error;
                            if (errorMessage == null || errorMessage == "") {
                                errorMessage = "Código: " + result.Error;
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
}