

function GridViewRemissionGuideTravelTypeFishingSiteDetails_BeginCallback(s, e) {
    e.customArgs["id_RemissionGuideTravelType"] = $("#id_RemissionGuideTravelType").val();
}


function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvRemissionGuideTravelType.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvRemissionGuideTravelType !== null && gvRemissionGuideTravelType !== undefined) {
        gvRemissionGuideTravelType.CancelEdit();
    } 
}