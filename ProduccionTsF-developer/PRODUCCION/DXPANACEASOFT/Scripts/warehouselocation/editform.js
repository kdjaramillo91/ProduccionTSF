function OnWarehouseLocationsPerson_Init(s, e) {
    enablePersonComboBox(false);
}

function OnWarehouseLocationsPerson_BeginCallback(s, e) {
    e.customArgs["id_warehouse"] = id_warehouse.GetValue();
}

function OnWarehouseLocationsPerson_SelectedIndexChanged(s, e) {
    $.ajax({
        url: "WarehouseLocation/GetDataPerson",
        type: "post",
        data: { id_person: id_person.GetValue(), id_warehouse: id_warehouse.GetValue() },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            code.SetText(result.code);
            code.SetValue(result.code);
            warehouseLocationName.SetText(result.warehouseLocationName);
            warehouseLocationName.SetValue(result.warehouseLocationName);
            description.SetText(result.description);
            description.SetValue(result.description);

        },
        complete: function () {
            //hideLoading();
        }
    });
}

function OnWarehouseLocationsWarehouse_SelectedIndexChanged(s, e) {
    enablePersonComboBox(true);
}

var enablePersonComboBox = function (useCallback) {
    $.ajax({
        url: "WarehouseLocation/GetCodeWarehouseType",
        type: "post",
        data: { id_warehouse: id_warehouse.GetValue() },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result.requirePerson) {
                if (useCallback) {
                    id_person.PerformCallback();
				}
                $('#id_personLabel').css('display', 'block');
                $('#id_person').css('display', 'block');

                code.SetEnabled(false);
                warehouseLocationName.SetEnabled(false);
                description.SetEnabled(false);
                isRolling.SetEnabled(false);
                isRolling.SetValue(false);
            }
            else {
                id_person.ClearItems();
                $('#id_personLabel').css('display', 'none');
                $('#id_person').css('display', 'none');
                id_person.SetValue(null);
                code.SetEnabled(true);
                warehouseLocationName.SetEnabled(true);
                description.SetEnabled(true);
                isRolling.SetEnabled(true);
            }
        },
        complete: function () {
            //hideLoading();
        }
    });
}
//COMBOS

function ComboBoxCompanies_SelectedIndexChanged(s, e) {
    id_warehouse.ClearItems();

    var item = id_company.GetSelectedItem();

    if (item !== null && item !== undefined) {
        $.ajax({
            url: "WarehouseLocation/WarehouseByCompany",
            type: "post",
            data: { id_company: item.value },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                for (var i = 0; i < result.length; i++) {
                    id_warehouse.AddItem(result[i].name, result[i].id);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }

}

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvWarehouseLocations.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvWarehouseLocations !== null && gvWarehouseLocations !== undefined) {
        gvWarehouseLocations.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
