
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    if (valid) {
        var permission = "id=" + $("#id_permission").val() + "&" + $("#formEditPermission").serialize();

        var url = ($("#id_permission").val() === "0") ? "Permission/PermissionsPartialAddNew"
                                                      : "Permission/PermissionsPartialUpdate";

        $.ajax({
            url: url,
            type: "post",
            data: permission,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                if (result.code < 0) {
                    $("#permissionErrorMessage").html(result.message);
                    $("#permissionAlertRow").css("display", "");
                    return;
                }

                // TODO: Set all empty

                gvPermissions.CancelEdit();
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function ButtonCancel_Click(s, e) {
    if (gvPermissions !== null && gvPermissions !== undefined) {
        gvPermissions.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}