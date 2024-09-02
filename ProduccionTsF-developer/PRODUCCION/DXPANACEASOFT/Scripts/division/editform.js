
//COMBOS

function ComboBoxCompanies_SelectedIndexChanged(s, e) {
    ruc.SetText("");
    address.SetText("");
    phoneNumber.SetText("");
    email.SetText("");

    var item = id_company.GetSelectedItem();

    if (item !== null && item !== undefined) {

        $.ajax({
            url: "Division/CompanyData",
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

function GridViewDivisionBranchOfficesDetails_BeginCallback(s, e) {
    e.customArgs["id_division"] = $("#id_division").val();
}

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    
    if (valid) {
        var division = {
            id: $("#id_division").val(),
            id_company: id_company.GetValue(),
            ruc: ruc.GetText(),
            name: divisionName.GetText(),
            description: description.GetText(),
            address: address.GetText(),
            email: email.GetText(),          
            phoneNumber: phoneNumber.GetText(),
            isActive: isActive.GetChecked()
        };

        var url = (division.id == "0") ? "Division/DivisionsPartialAddNew"
                                       : "Division/DivisionsPartialUpdate";

        $.ajax({
            url: url,
            type: "post",
            data: division,
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
                    $("#divisionErrorMessage").html(result.message);
                    $("#divisionAlertRow").css("display", "");
                    return;
                }

                // TODO: Set all empty

                gvDivisions.CancelEdit();
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function ButtonCancel_Click(s, e) {
    if (gvDivisions !== null && gvDivisions !== undefined) {
        gvDivisions.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
