
//COMBOS

function ComboBoxCompanies_SelectedIndexChanged(s, e) {
    id_inventoryLine.ClearItems();

    var item = id_company.GetSelectedItem();

    if (item !== null && item !== undefined) {
        $.ajax({
            url: "ItemType/InventoryLineByCompany",
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
                    id_inventoryLine.AddItem(result[i].name, result[i].id);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }

}

/*
function GridViewFishingZoneFishingSiteDetails_BeginCallback(s, e) {
    e.customArgs["id_fishingZone"] = $("#id_fishingZone").val();
}
*/

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (gvTransportTariffDetail.GetVisibleRowsOnPage() === undefined) valid = false;
    if (gvTransportTariffDetail.GetVisibleRowsOnPage() === 0)
    {
        valid = false;
        $("#_errormsgTT").text("Debe ingresar un producto para guadar tarifario").show(100).delay(2000).hide(200);
    } 

    if (valid) {
        gvTransportTariff.UpdateEdit();
    }
}


function ButtonCancel_Click(s, e) {
    if (gvTransportTariff !== null && gvTransportTariff !== undefined)
    {
        gvTransportTariff.CancelEdit();
        $("#_errormsgTT").hide();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
 