
// FORM EDIT BUTTONS

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    
    if (valid) {
        var userGroup = "id=" + $("#id_userGroup").val() + "&" + $("#formEditUserGroup").serialize();

        var url = ($("#id_userGroup").val() === "0") ? "UserGroup/UserGroupsPartialAddNew"
                                                     : "UserGroup/UserGroupsPartialUpdate";

        $.ajax({
            url: url,
            type: "post",
            data: userGroup,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                $("#maincontent").html(result);
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function ButtonCancel_Click(s, e) {
    showPage("UserGroup/Index", null);
}

// TREEMENU CLIENTSIDE EVENTS

function UserGroupTreeMenu_SelectionChanged(s, e) {
    tvMenu.GetSelectedNodeValues("id", function (values) {
        $.ajax({
            url: "UserGroup/UpdateMenuSelection",
            type: "post",
            data: { ids: values },
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                //$("#maincontent").html(result);
                tvMenu.PerformCallback();
            },
            complete: function () {
                //tvMenu.PerformCallback();
                //gvBranchOffices.UnselectRows();
                //hideLoading();
            }
        });
    });
    //console.log(tvMenu.cpCheckedNodes);
}

function UserGroupTreeView_CustomButtonClick(s, e) {
    if (e.buttonID === "btnUpdateEdit") {
        tvMenu.GetNodeValues(e.nodeKey, "id", function (value) {
            $.ajax({
                url: "UserGroup/UpdateMenuPermissions",
                type: "post",
                data: {
                    id: value,
                    permissions: permissions.GetValue().split(",")
                },
                async: true,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    //showLoading();
                },
                success: function (result) {
                    //$("#maincontent").html(result);
                },
                complete: function () {
                    tvMenu.CancelEdit();
                }
            });
        });
        
    }
    else if(e.buttonID === "btnCancelEdit") {
        tvMenu.CancelEdit();
    }
}

// MAIN FUNCTIONS

function init() {
    
}

$(function () {

});