
////COMBOS

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    
    if (gvWarehouseUserPermitDetail.GetVisibleRowsOnPage() == undefined) valid = false;
    if (gvWarehouseUserPermitDetail.GetVisibleRowsOnPage() == 0)
    {
        valid = false;
        $("#_errormsgTT").text("Debe elegir un usuario para guadar permisos a bodegas").show(100).delay(2000).hide(200);
    } 
    if (valid) {
        gvPersonTypes.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvPersonTypes !== null && gvPersonTypes !== undefined)
    {
        gvPersonTypes.CancelEdit();
        $("#_errormsgTT").hide();
    }
}

function OnUserInit(s, e) {
    var data = id_user.GetValue();

    id_userEntity.SetValue(data);

    $("#id_user").prop("readonly", false);
}


 