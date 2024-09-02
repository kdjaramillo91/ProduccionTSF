//#region Accion Botones Inferiores Edicion
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvEmailNotifyDocumentType.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvEmailNotifyDocumentType !== null && gvEmailNotifyDocumentType !== undefined) {
        gvEmailNotifyDocumentType.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}

//#endregion

//#region Acciones Token
function TokenEmailNotifyDocumentTypePersons_Init(s, e) {
    var data = {                                        
        id_emailNotifyDocumentType: $("#id_emailNotifyDocumentType").val()
    };

    $.ajax({
        url: "EmailNotify/GetEmailNotifyDocumentTypePersons",
        type: "post",
        data: data,
        async: false,
        cache: false,
        error: function (error) {
             
            console.log(error);
            UpdateEmailNotifyDocumentTypePersons(null);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
             
            UpdateEmailNotifyDocumentTypePersons(result.persons);
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function TokenEmailNotifyDocumentTypePersons_ValueChanged(s, e) {
    personsAux = s.GetValue();
    
    var data = {
        personsCurrent: personsAux.split(","),
        
    };

    $.ajax({
        url: "EmailNotify/UpdatePersons",
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
            //UpdatePriceListInventoryLines(result.inventoryLines);
        },
        complete: function () {
            //hideLoading();
        }
    });


}

function UpdateEmailNotifyDocumentTypePersons(persons) {

     
    emailNotifyDocumentTypePersons.SetValue(persons);

}


//#endregion

//#region Tipo Documento
function OnDocumentType_SelectedIndexChanged(s,e)
{
    if (s == null) return;
    description.SetValue(`Destinatarios e-mail proceso: ${s.GetText()}`);
}
//#endregion
        