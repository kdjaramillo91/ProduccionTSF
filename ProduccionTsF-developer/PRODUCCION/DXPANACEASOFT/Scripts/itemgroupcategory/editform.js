
function UpdateItemGroupCategoriesItemGroups(itemGroupsParam) {

    itemGroups.SetValue(itemGroupsParam);

}

function TokenItemGroup_Init(s, e) {
    var data = {
        id_itemGroupCategory: $("#id_itemGroupCategory").val()
    };

    $.ajax({
        url: "ItemGroupCategory/GetItemGroupCategoriesItemGroups",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdateItemGroupCategoriesItemGroups(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            UpdateItemGroupCategoriesItemGroups(result.itemGroups);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function TokenItemGroup_ValueChanged(s, e) {
    itemGroupsAux = s.GetValue();
    var data = {
        itemGroupsCurrent: itemGroupsAux.split(",")
    };

    $.ajax({
        url: "ItemGroupCategory/UpdateItemGroups",
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
            //UpdatePriceListInventoryLines(result.inventoryLines);
        },
        complete: function () {
            //hideLoading();
        }
    });


}

//COMBOS

function ComboBoxCompanies_SelectedIndexChanged(s, e) {
    id_itemGroup.ClearItems();

    var item = id_company.GetSelectedItem();
    

    if (item !== null && item !== undefined) {

        $.ajax({
            url: "ItemGroupCategory/ItemGroupByCompany",
            type: "post",
            data: { id_company: item.value},
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
                    id_itemGroup.AddItem(result[i].name, result[i].id);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

//Button
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvItemGroupCategories.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvItemGroupCategories !== null && gvItemGroupCategories !== undefined) {
        gvItemGroupCategories.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
