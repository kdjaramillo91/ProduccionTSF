
// VALIDATIONS

function OnDocumentStateNameValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

function OnDocumentStateDocumentTypesValidation(s, e) {
    if (e.value === null || e.value === "") {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// EDITNG ACTIONS

function ButtonUpdateDocumentState_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "formEditDocumentState", true);

    if (valid) {
        var documentState = {
            id: $("#id_documentState").val(),
            name: documentStateName.GetValue(),
            description: documentStateDescription.GetValue(),
            isActive: documentStateIsActive.GetValue(),
            DocumentType: []
        }

        if (typeof documentStateDocumentTypes !== "undefined" && documentStateDocumentTypes.GetValue() !== null && documentStateDocumentTypes.GetValue() !== "") {
            var values = documentStateDocumentTypes.GetValue().split(",");
            for (var i = 0; i < values.length; i++) {
                documentState.DocumentType.push({
                    id: parseInt(values[i])
                });
            }
        }

        var url = (documentState.id === "0") ? "DocumentStates/DocumentStatesPartialAddNew"
                                             : "DocumentStates/DocumentStatesPartialUpdate";
        $.ajax({
            url: url,
            type: "post",
            data: documentState,
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
                    $("#documentStateErrorMessage").html(result.message);
                    $("#documentStateAlertRow").css("display", "");
                    return;
                }

                if (CallBackFunction !== null) {
                    CallBackFunction(result);
                }

                $("#id_documentState").val(0);
                documentStateName.SetText(null);
                documentStateDescription.SetText(null);
                documentStateIsActive.SetValue(0);

                var gv = null;
                try {
                    gv = gvDocumentStates;
                } catch (exception) {
                    gv = null;
                }

                var dialog = null;
                try {
                    dialog = dialogAddDocumentState;
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

function ButtonCancelDocumentState_Click(s, e) {
    if (gvDocumentStates !== null && gvDocumentStates !== undefined) {
        gvDocumentStates.CancelEdit();
    }
}

// POPUP ACTIONS

function BtnAddDocumentType_Click() {
    CallBackFunction = function (result) {
        if (result.code > 0 && documentStateDocumentTypes !== null && documentStateDocumentTypes !== undefined) {
            documentStateDocumentTypes.AddItem(result.message, result.code);
            documentStateDocumentTypes.AddToken(result.message);
        }
    }
    dialogAddDocumentType.Show();
}

// MAIN FUNCTIONS

function init() {

    CallBackFunction = null;

    if (typeof OnDocumentTypeNameValidation !== "function") {
        $.getScript("Scripts/documenttype/editform.js", function () { });
    }
}

$(function () {
    init();
});