 

function btnSearchDocumento_click(s,e)
{
    // 
    var arDocuments = OIntegrationProcess.GetIDoc();
    var id_IntegrationProcessLote =  $("#id_IntegrationProcess").val();
    var id_DocumentType = ASPxClientControl.GetControlCollection().GetByName("id_DocumentType").GetValue();
    var id_EmissionPoint = ASPxClientControl.GetControlCollection().GetByName("id_EmissionPoint").GetValue().split(",");
    var dateEndEmission = ASPxClientControl.GetControlCollection().GetByName("dateEndEmission").GetValue();
    var dateInitEmission = ASPxClientControl.GetControlCollection().GetByName("dateInitEmission").GetValue();
    var numberDocument = ASPxClientControl.GetControlCollection().GetByName("numberDocument").GetValue();
   
    var _dateInitEmission = (dateInitEmission != null) ? dateInitEmission.toJSON() : null;
    var _dateEndEmission = (dateEndEmission != null) ? dateEndEmission.toJSON() : null;
    

    data = {
                "id_DocumentType": id_DocumentType,
                "id_EmissionPoint": id_EmissionPoint,
                "dateInitEmission": _dateInitEmission,
                "dateEndEmission": _dateEndEmission,
                "numberDocument": numberDocument,
                "id_integrationProcessDetailList": arDocuments,
                "id_IntegrationProcessLote": id_IntegrationProcessLote
            };

    //var data = $("#formFilterIntegrationProcessDocument").serialize() + "&id_DocumentType=" + id_DocumentType + "&id_integrationProcessDetailList=" + JSON.stringify(arDocuments);
    var url = "integrationprocess/finddocument";

    if (data != null) {

        genericAjaxCall(
                        url,
                        true,
                        data,
                        function (error) { console.log(error); },
                        function () { showLoading(); },
                        function (result)
                        {
                            $("#btnCollapse").click();
                            $("#results").html(result);
                        },
                        function () { hideLoading(); });
    }
    event.preventDefault();

}

function  btnClearDocumento_click(s,e)
{
    fullname_businessName.SetText("");
    identity.SetText("");
    id_documentState.SetSelectedItem(null);


    fechaEmisionDesde.SetDate(null);
    fechaEmisionHasta.SetDate(null);

    number.SetText("");

    fechaEmbarqueDesde.SetDate(null);
    fechaEmbarqueHasta.SetDate(null);

    id_shippingAgency.SetSelectedItem(null);

    id_portDischarge.SetSelectedItem(null);
    id_portDestination.SetSelectedItem(null);
}
 
function btnCloseFilter_click(s,e)
{
    $.fancybox.close();
}
