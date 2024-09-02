
//COMBOS

function ComboBoxCompanies_SelectedIndexChanged(s, e) {
    ruc.SetText("");
    address.SetText("");
    phoneNumber.SetText("");
    email.SetText("");
    id_division.ClearItems();

    var item = id_company.GetSelectedItem();

    if (item !== null && item !== undefined) {
        $.ajax({
            url: "BranchOffice/DivisionByCompany",
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
                    id_division.AddItem(result[i].name, result[i].id);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }

}

function ComboBoxDivisions_SelectedIndexChanged(s, e) {
    ruc.SetText("");
    address.SetText("");
    phoneNumber.SetText("");
    email.SetText("");

    var item = id_division.GetSelectedItem();

    if (item !== null && item !== undefined) {

        $.ajax({
            url: "BranchOffice/DivisionData",
            type: "post",
            data: { id_division: item.value },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result !== null && result !== undefined) {
                    ruc.SetText(result.ruc);
                    address.SetText(result.address);
                    phoneNumber.SetText(result.phoneNumber);
                    email.SetText(result.email);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function DivisionsCombo_SelectedIndexChanged(s, e) {

}

function GridViewBranchOfficeEmissionPointsDetails_BeginCallback(s, e) {
    e.customArgs["id_branchOffice"] = $("#id_branchOffice").val();
}

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvBranchOffices.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvBranchOffices !== null && gvBranchOffices !== undefined) {
        gvBranchOffices.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
