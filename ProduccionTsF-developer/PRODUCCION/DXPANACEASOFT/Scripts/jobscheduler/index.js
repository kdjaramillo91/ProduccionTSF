function OnRangeInitDateValidation(s, e)
{

}

function OnRangeEndDateValidation(s, e) {

}
 

function Validate() {

    var errors = "";

    
    if (dateInit.GetValue() == null) {
        errors = "Fecha de inicio es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
        
    }
    if (dateEnd.GetValue() == null) {
        errors = "Fecha final es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
    }

    if (serverHost.GetValue() == null || (typeof serverHost.GetValue() === "undefined") || serverHost.GetValue() == '') {
        errors = "El nombre del servidor host es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
    }

    if (databaseHost.GetValue() == null || (typeof databaseHost.GetValue() === "undefined") || databaseHost.GetValue() == '') {
        errors = "El nombre de la base de datos es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
    }

    if (userdb.GetValue() == null || (typeof userdb.GetValue() === "undefined") || userdb.GetValue() == '') {
        errors = "El usuario de la base de datos es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
    }

    if (passwordb.GetValue() == null || (typeof passwordb.GetValue() === "undefined") || passwordb.GetValue() == '') {
        errors = "El password de la base de datos es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
    }
    if (storeProcedure.GetValue() == null || (typeof storeProcedure.GetValue() === "undefined") || storeProcedure.GetValue() == '') {
        errors = "El nombre del store prodecure a ejecutar es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
    }
    return true;
     
    
}

function messageValida(input) {
    let valida = true;
    if (input.length > 0) {
        valida = false;
        NotifyError("Error. " + input);
    }

    return valida;

}

function ButtonExecute_Click(s, e)
{
    executeSP();

} 

function validaExecuteSp()
{
    var errors = "";

    if (dateInit.GetValue() == null) {
        errors = "Fecha de inicio es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;

    }
    if (dateEnd.GetValue() == null) {
        errors = "Fecha final, es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
    }

    if (IsNullOrUndefinedOrEmpty(serverHost.GetValue()))
    {
        errors = "Nombre del servidor, es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
    }
    if (IsNullOrUndefinedOrEmpty(databaseHost.GetValue())) {
        errors = "Nombre de la base de datos, es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
    }
    if (IsNullOrUndefinedOrEmpty(userdb.GetValue())) {
        errors = "Usuario de la base de datos, es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
    }
    if (IsNullOrUndefinedOrEmpty(passwordb.GetValue())) {
        errors = "Password de la base de datos, es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
    }
    if (IsNullOrUndefinedOrEmpty(storeProcedure.GetValue())) {
        errors = "Store Procedure, es un campo Obligatorio. \n\r";
        messageValida(errors);
        return false;
    }
    return true;
}

function executeSP()
{
    showLoading();

    if (!validaExecuteSp()) {
        hideLoading();
        return;
    }

    setButtons(false);
    let idjob = $("#id_jobScheduleOne").val();
    //let isRefresh = $("#modo_form").val();
    let isRefresh = false;
    let dataToSend = "id=" + idjob + "&" + $("#formJobSchedule").serialize() + "&isRefresh=" + isRefresh;
    $.ajax({
        url: 'JobScheduler/JobScheduleExecution',
        type: 'post',
        data: dataToSend,
        async: true,
        cache: false,
        error: function (result) {
            hideLoading();
            NotifyError("Error. " + result.message);
            setButtons(true);
        },
        success: function (result) {
            hideLoading();

            if (result != null) {
                $("#id_jobScheduleOne").val(result.id_jobSchedule);
                observerNotification("168", 5000, callbackProcess);

                NotifySuccess("Información en proceso");

                $("#statusJob").html("EN PROCESO");

            }
            else
            {
                setButtons(true);
            }

            //let notifyMessage = "";
            //let executeButtomText = "";
            //let enabledButtomDown = false;
            //let statusText = ""
            //let isRefreshNew = false;  
            //
            //switch (result.status)
            //{
            //
            //    case 1:
            //        notifyMessage = "Información en proceso";
            //        executeButtomText = "Actualizar";
            //        enabledButtomDown = false;
            //        statusText = "EN PROCESO";
            //        if (!isRefreshNew)
            //        {
            //            $("#id_jobScheduleOne").val(result.id_jobSchedule);
            //            observerNotification("168", 5000, callbackProcess);
            //        }
            //        isRefreshNew = true;
            //    case 2:
            //        notifyMessage = "Información en proceso";
            //        executeButtomText = "Actualizar";
            //        enabledButtomDown = false;
            //        statusText = "EN PROCESO";
            //        isRefreshNew = true;
            //
            //        break;
            //    case 3:
            //        notifyMessage = "Ha finalizado con éxito el procesamiento de la información";
            //        executeButtomText = "Ejecutar";
            //        enabledButtomDown = true;
            //        statusText = "FINALIZADO";
            //        isRefreshNew = false;
            //        break;
            //    case 4:
            //        notifyMessage = "Ha ocurrido un error. Comunique al dep. de sistemas";
            //        executeButtomText = "Ejecutar";
            //        enabledButtomDown = false;
            //        statusText = "CON ERROR";
            //        isRefreshNew = false;
            //        break;
            //
            //}
            //
            //NotifySuccess(notifyMessage);
            //btnExecute.SetText(executeButtomText);
            //btnDescargar.SetEnabled(enabledButtomDown);
            //$("#statusJob").html(statusText);
            //$("#modo_form").val(isRefreshNew);
            //
            //dateInit.SetReadOnly(isRefreshNew);
            //dateEnd.SetReadOnly(isRefreshNew);
            //serverHost.SetReadOnly(isRefreshNew);
            //databaseHost.SetReadOnly(isRefreshNew);
            //userdb.SetReadOnly(isRefreshNew);
            //passwordb.SetReadOnly(isRefreshNew);
            //storeProcedure.SetReadOnly(isRefreshNew);
            
        }
    });


}

function ButtonDescargar_Click(s, e)
{
    downloadResultSp();
}
function setButtons(enabled)
{
    if (typeof btnExecuteJobSchedule !== "undefined" && ASPxClientUtils.IsExists(btnExecuteJobSchedule)) {
        btnExecuteJobSchedule.SetEnabled(enabled);
    }  
    if (typeof btnDescargarJobSchedule !== "undefined" && ASPxClientUtils.IsExists(btnDescargarJobSchedule)) {
        btnDescargarJobSchedule.SetEnabled(enabled);
    }  
    
}
function callbackProcess()
{
    downloadResultSp();
    setButtons(true);
    if ($('#statusJob').length) {
        $("#statusJob").html("FINALIZADO");
        NotifySuccess("Ha finalizado con éxito el procesamiento de la información");
    }

    

}
function downloadResultSp()
{
    $('#download-area-general').empty();
    $('#download-area-general').html("<iframe style='height:0;width:0;border:0;' src='JobScheduler/DownloadTemplateJobResult'></iframe>");
}


$(function () {
    init();
});


function init() {
    
    let ejecutarObserver = $('#ejecutarObserver').val();
    if (ejecutarObserver == "S")
    {
        console.log("DISPARANDO OBSERVER POST REFRESH");
        observerNotification("168", 5000, callbackProcess);
        //setButtons(false);
    }
}