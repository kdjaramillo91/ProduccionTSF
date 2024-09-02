
// VALIDATIONS

function OnDocumentTypeNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDocumentTypeCodeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDocumentTypeDaysToExpirationValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDocumentTypeDocumentStatesValidation(s, e) {
    if (e.value === null || e.value === "") {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// EDITNG ACTIONS

function ButtonUpdateDocumentType_Click(s, e) {

    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "formEditDocumentType", true);

    if (valid) {
        var documentType = {
            id: $("#id_documentType").val(),
            name: documentTypeName.GetValue(),
            code: documentTypeCode.GetValue(),
            daysToExpiration: documentTypeDaysToExpiration.GetValue(),
            description: documentTypeDescription.GetValue(),
            isActive: documentTypeIsActive.GetValue(),
            DocumentState: []
        }

        if (typeof documentTypeDocumentStates !== "undefined" && documentTypeDocumentStates.GetValue() !== null && documentTypeDocumentStates.GetValue() !== "") {
            var values = documentTypeDocumentStates.GetValue().split(",");
            console.log(values);
            for (var i = 0; i < values.length; i++) {
                documentType.DocumentState.push({
                    id: parseInt(values[i])
                });
            }
        }

        var url = (documentType.id === "0") ? "DocumentType/DocumentTypePartialAddNew"
                                             : "DocumentType/DocumentTypePartialUpdate";

        $.ajax({
            url: url,
            type: "post",
            data: documentType,
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
                    $("#documentTypeErrorMessage").html(result.message);
                    $("#documentTypeAlertRow").css("display", "");
                    return;
                }

                if(CallBackFunction !== null) {
                    CallBackFunction(result);
                }

                $("#id_documentType").val(0);
                documentTypeName.SetText(null);
                documentTypeCode.SetText(null);
                documentTypeDaysToExpiration.SetValue(0);
                documentTypeDescription.SetText(null);
                documentTypeIsActive.SetChecked(true);

                var gv = null;
                try {
                    gv = gvDocumentType;
                } catch (exception) {
                    gv = null;
                }

                var dialog = null;
                try {
                    dialog = dialogAddDocumentType;
                } catch (exception) {
                    dialog = null;
                }

                if (gv !== null && gv !== undefined) {
                    gv.CancelEdit();
                } else if (dialog !== null && dialog !== undefined) {
                    dialog.Hide();
                }
            },
            complete: function () {
                hideLoading();
            }
        });
    }
}

function ButtonCancelDocumentType_Click(s, e) {
    if (gvDocumentType !== null && gvDocumentType !== undefined) {
        gvDocumentType.CancelEdit();
    } else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();    
    }
}

// POPUP ACTIONS

function BtnAddDocumentState_Click() {
    CallBackFunction = function (result) {
        if (result.code > 0 && documentTypeDocumentStates !== null && documentTypeDocumentStates !== undefined) {
            documentTypeDocumentStates.AddItem(result.message, result.code);
            documentTypeDocumentStates.AddToken(result.message);
        }
    }
    dialogAddDocumentState.Show();
}

// MAIN FUNCTIONS

function init() {

    CallBackFunction = null;

    if (typeof OnDocumentStateNameValidation !== "function") {
        $.getScript("Scripts/documentstates/editform.js", function () { });
    }
    
}

$(function () {
    init();
});