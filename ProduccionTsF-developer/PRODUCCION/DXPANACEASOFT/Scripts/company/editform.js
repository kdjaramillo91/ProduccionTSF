
function GridViewCompanyDivisionsDetails_BeginCallback(s, e) {
    e.customArgs["id_company"] = $("#id_company").val();
}

//CERTIFICATES 

function ClearCertificateClick(s, e) {

}

function LoadCertificateClick(s, e) {
    $("#certificateFile").on('change', function (event) {
        if ($("#certificateFile").val() !== null && $("#certificateFile").val() !== "") {
            var path = $("#certificateFile").val().split("\\");
            certificate.SetText(path[path.length - 1]);

            // UPLOAD FILE
            var files = $('#certificateFile').prop('files');
            var data = new FormData();
            if (files !== null && files !== undefined && files.length > 0) {
                data.append("certificate", files[0]);
            } else {
                data.append("certificate", null);  
            }

            $.ajax({
                url: "Company/UploadCertificate",
                type: "post",
                data: data,
                processData: false,
                contentType: false,
                async: true,
                cache: false,
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    //showLoading();
                },
                success: function (result) {
                    
                },
                complete: function () {
                    //hideLoading();
                }
            });

        } else {
            certificate.SetText("");
        }
        certificate.Validate();
    });
    $("#certificateFile").click();
}

// BUTTONS

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvCompanies.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvCompanies !== null && gvCompanies !== undefined) {
        gvCompanies.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
