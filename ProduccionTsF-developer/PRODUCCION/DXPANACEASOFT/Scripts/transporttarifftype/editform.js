
//COMBOS

//function ComboBoxCompanies_SelectedIndexChanged(s, e) {
//    id_warehouseType.ClearItems();
//    id_inventoryLine.ClearItems();

//    var item = id_company.GetSelectedItem();

//    if (item !== null && item !== undefined) {
//        $.ajax({
//            url: "Warehouse/WarehouseTypeByCompany",
//            type: "post",
//            data: { id_company: item.value },
//            async: true,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//            },
//            beforeSend: function () {
//                //showLoading();
//            },
//            success: function (result) {
//                for (var i = 0; i < result.length; i++) {
//                    id_warehouseType.AddItem(result[i].name, result[i].id);
//                }
//            },
//            complete: function () {
//                //hideLoading();
//            }
//        });

//        $.ajax({
//            url: "Warehouse/InventoryLineByCompany",
//            type: "post",
//            data: { id_company: item.value },
//            async: true,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//            },
//            beforeSend: function () {
//                //showLoading();
//            },
//            success: function (result) {
//                for (var i = 0; i < result.length; i++) {
//                    id_inventoryLine.AddItem(result[i].name, result[i].id);
//                }
//            },
//            complete: function () {
//                //hideLoading();
//            }
//        });
//    }

//}



function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvTransportTariffType.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvTransportTariffType !== null && gvTransportTariffType !== undefined) {
        gvTransportTariffType.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}