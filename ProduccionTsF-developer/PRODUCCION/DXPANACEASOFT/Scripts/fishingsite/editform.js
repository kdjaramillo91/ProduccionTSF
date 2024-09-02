
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

function GridViewFishingSiteFishingZoneDetails_BeginCallback(s, e) {
    e.customArgs["id_fishingSite"] = $("#id_fishingSite").val();
}


function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvFishingSite.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvFishingSite !== null && gvFishingSite !== undefined) {
        gvFishingSite.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}