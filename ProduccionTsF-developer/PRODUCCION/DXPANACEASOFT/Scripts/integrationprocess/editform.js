var globalDocumentsSelected = new Array();


/*
function SaveLote(documentsSelected, accion)
{
    // 
    var valid = true;
    // Validaciones

    if (valid == false) {
        // Mensaje de Error
        return;
    }

    var id = $("#id_IntegrationProcess").val();
    var id_DocumentType = ASPxClientControl.GetControlCollection().GetByName("id_DocumentType").GetValue();
    var description = ASPxClientControl.GetControlCollection().GetByName("description").GetValue();

    var url = "";
    var data = {};
    if (id == 0) {
        data = { "id_DocumentType": id_DocumentType, "description": description };
        url = "integrationprocess/addlote";
    }
    else {
        headData = { "id_IntegrationProcess": id, "id_DocumentType": id_DocumentType, "description": description };
        //  detailData = {"id_integrationProcessDetailList": JSON.stringify(globalDocumentsSelected)};
        detailData = { "id_integrationProcessDetailList": globalDocumentsSelected };

        data = $.extend(headData, detailData);
        url = "integrationprocess/updatelote";
    }

    showFormPostBack(url, data, null);
}
*/


function CancelLote(s,e)
{

}

function ProcessingLote(s,e)
{

}

function PrintLote(s,e)
{ }
/*
function ButtonUpdate_Click(s,e)
{
    OIntegrationProcess.SaveLote(null, null);
}
*/

function ButtonCancel_Click(s,e)
{

}

function init()
{}


$(function () {

    var chkReadyState = setInterval(function ()
    {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);


        }
    }, 100);


    init();
});

//function OnDateValidation(s, e) {
//    if (e.value === null) {
//        e.isValid = false;
//        e.errorText = "Campo Obligatorio";
//    }
//}