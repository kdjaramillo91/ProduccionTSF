//#region Valiciones Controles
function OnDocumentTypeValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        return;
    }
    ValidateUniqueDocumentType(s.GetValue(), e);
}
function OnEmailNotifyDocumentTypePersonsValidation(s, e) {
     
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        return;
    }
    

}

function ValidateUniqueDocumentType(id_DocumentType, e) {
    
    emailNotifyDocumentTypeId = $("#emailNotifyDocumentTypeId").val();
    
    var data = {
        id_DocumentType: id_DocumentType,
        emailNotifyDocumentTypeId: emailNotifyDocumentTypeId 
    };
    let ex = e;
    $.ajax({
        url: "EmailNotify/ValidateExistEmailDocumentType",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);

        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            
            if (result.exists == "S") {
                
                ex.isValid = false;
                ex.errorText = "Documento ya esta configurado";
                //id_DocumentType.SetValue(null);
                //showErrorMessage("Documento ya esta configurado");

                // reversar el valor a cero
                // presentarr mensaje
            }

        },
        complete: function () {
            //hideLoading();
        }
    });
}
//#endregion