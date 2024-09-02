
//COMBOS

function ComboBoxCompanies_SelectedIndexChanged(s, e) {
    id_parentGroup.ClearItems();

    var item = id_company.GetSelectedItem();

    if (item !== null && item !== undefined) {
        var id_itemGroup = $("#id_itemGroup").val();
        $.ajax({
            url: "ItemGroup/ItemGroupByCompany",
            type: "post",
            data: { id_company: item.value, id_itemGroup: id_itemGroup },
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
                    id_parentGroup.AddItem(result[i].name, result[i].id);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function GridViewItemGroupItemSubGroupsDetails_BeginCallback(s, e) {
    e.customArgs["id_itemGroup"] = $("#id_itemGroup").val();
}

function GridViewItemGroupItemGroupCategoriesDetails_BeginCallback(s, e) {
    e.customArgs["id_itemGroup"] = $("#id_itemGroup").val();
}

//Button

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvItemGroups.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvItemGroups !== null && gvItemGroups !== undefined) {
        gvItemGroups.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
