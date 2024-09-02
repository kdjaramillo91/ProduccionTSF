
function Init_PasswordName(s, e) {
    passwordName.SetText($("#passwordAux").val());
}

// EDIT FORM BUTTONS

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);

    if (valid) {
        var user = {
            id: $("#id_user").val(),
            username: username.GetText(),
            password: null,
            id_group: id_group.GetValue(),
            id_employee: id_employee.GetValue(),
            isActive: isActive.GetChecked(),
            EmissionPoint: []
        };

        var items = EmissionPoint.GetValue().split(",");
        //console.log("items:" + items);
        //console.log("items.length:" + items.length);
        for (var i = 0; i < items.length; i++) {
            //console.log("parseInt(items[i]):" + parseInt(items[i]));

            user.EmissionPoint.push({ id: parseInt(items[i]) });
        }

        if ($("#passwordAux").val() != passwordName.GetText()) {

            user.password = md5(passwordName.GetText());
        } else {
            user.password = $("#passwordAux").val();
        }

        var url = ($("#id_user").val() === "0") ? "User/UsersPartialAddNew"
                                                : "User/UsersPartialUpdate";

        $.ajax({
            url: url,
            type: "post",
            data: user,
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
    showPage("User/Index", null);
}

// TREEMENU CLIENTSIDE EVENTS

function UserTreeMenu_SelectionChanged(s, e) {
    tvMenu.GetSelectedNodeValues("id", function (values) {
        $.ajax({
            url: "User/UpdateMenuSelection",
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

function UserTreeView_CustomButtonClick(s, e) {
    if (e.buttonID === "btnUpdateEdit") {
        tvMenu.GetNodeValues(e.nodeKey, "id", function (value) {
            $.ajax({
                url: "User/UpdateMenuPermissions",
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
    else if (e.buttonID === "btnCancelEdit") {
        tvMenu.CancelEdit();
    }
}

// USERR EMISSION POINTS ACTION BUTTONS 

function AddNewItem(s, e) {
}

function RemoveItems(s, e) {
}

function RefreshGrid(s, e) {
}

function Print(s, e) {
}

// USER EMISSION POITS GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function GridViewUserEmissionPointsCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnEditRow") {
        s.GetRowValues(e.visibleIndex, "id", function (value) {
            var data = {
                id: value
            }
            showPage("User/EditFromUserPartial", data);
        });
    } else if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            gvUserEmissionPoints.UnselectRows();
            s.DeleteRow(e.visibleIndex);
            UpdateTitlePanel();
        });
    }


}

// USER EMISSION POITS SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvUserEmissionPoints.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvUserEmissionPoints.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvUserEmissionPoints.GetSelectedRowCount() > 0 && gvUserEmissionPoints.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvUserEmissionPoints.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvUserEmissionPoints.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvUserEmissionPoints.cpFilteredRowCountWithoutPage + gvUserEmissionPoints.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvUserEmissionPoints.SelectRows();
}

function UnselectAllRows() {
    gvUserEmissionPoints.UnselectRows();
}

// GROUP CHANGE

function GroupSelectedIndexChange(s, e) {

    if (s.GetSelectedItem() === null) {
        return;
    }

    var data = {
        id_group: s.GetSelectedItem().value
    }

    $.ajax({
        url: "User/UpdateMenuByGroup",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            hideLoading();
            callbackPanel.PerformCallback();
        },
        complete: function () {}
    });
}