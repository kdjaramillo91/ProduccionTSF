
////COMBOS

//function ComboBoxCompanies_SelectedIndexChanged(s, e) {
//    address.SetText("");
//    phoneNumber.SetText("");
//    email.SetText("");
//    id_division.ClearItems();
//    id_branchOffice.ClearItems();

//    var item = id_company.GetSelectedItem();

//    if (item !== null && item !== undefined) {
//        $.ajax({
//            url: "EmissionPoint/DivisionByCompany",
//            type: "post",
//            data: { id_company: item.value },
//            async: true,
//            cache: false,
//            error: function (error) {
//                console.log(error);
//            },
//            beforeSend: function () {
//                //showLoading();
//            },
//            success: function (result) {
//                for (var i = 0; i < result.length; i++) {
//                    id_division.AddItem(result[i].name, result[i].id);
//                }
//            },
//            complete: function () {
//                //hideLoading();
//            }
//        });
//    }

//}

function ComboBoxDivisions_SelectedIndexChanged(s, e) {
    address.SetText("");
    phoneNumber.SetText("");
    email.SetText("");
    id_branchOffice.ClearItems();

    var item = id_division.GetSelectedItem();

    if (item !== null && item !== undefined) {
        $.ajax({
            url: "EmissionPoint/BranchOfficeByDivision",
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
                for (var i = 0; i < result.length; i++) {
                    id_branchOffice.AddItem(result[i].name, result[i].id);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }

}

//
function ComboBoxTipoDocument_SelectedIndexChanged(s, e) {
    address.SetText("");
    phoneNumber.SetText("");
    email.SetText("");
    id_documen.ClearItems();

    var item = id_documen.GetSelectedItem();

    if (item !== null && item !== undefined) {
        $.ajax({
            url: "EmissionPoint/BranchOfficeByDivision",
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
                for (var i = 0; i < result.length; i++) {
                    id_branchOffice.AddItem(result[i].name, result[i].id);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }

}
//

function ComboBoxBranchOffices_SelectedIndexChanged(s, e) {
    address.SetText("");
    phoneNumber.SetText("");
    email.SetText("");

    var item = id_branchOffice.GetSelectedItem();

    if (item !== null && item !== undefined) {

        $.ajax({
            url: "EmissionPoint/BranchOfficeData",
            type: "post",
            data: { id_branchOffice: item.value },
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

//Button


function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvEmissionPoints.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvEmissionPoints !== null && gvEmissionPoints !== undefined) {
        gvEmissionPoints.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
