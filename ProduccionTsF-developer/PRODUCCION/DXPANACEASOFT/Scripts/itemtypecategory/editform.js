function UpdateItemTypeCategoriesItemTypes(itemTypesParam) {

    itemTypes.SetValue(itemTypesParam);

}

function TokenItemType_Init(s, e) {
    var data = {
        id_itemTypeCategory: $("#id_itemTypeCategory").val()
    };

    $.ajax({
        url: "ItemTypeCategory/GetItemTypeCategoriesItemTypes",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
            UpdateItemTypeCategoriesItemTypes(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            UpdateItemTypeCategoriesItemTypes(result.itemTypes);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function TokenItemType_ValueChanged(s, e) {
    itemTypesAux = s.GetValue();
    var data = {
        itemTypesCurrent: itemTypesAux.split(",")
    };

    $.ajax({
        url: "ItemTypeCategory/UpdateItemTypes",
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

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvItemTypeCategories.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvItemTypeCategories !== null && gvItemTypeCategories !== undefined) {
        gvItemTypeCategories.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}