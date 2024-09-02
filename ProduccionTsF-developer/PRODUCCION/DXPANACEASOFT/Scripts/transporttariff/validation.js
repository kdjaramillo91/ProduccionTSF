
/// TransporTariff-Partial

function OnTTCodeValidation(s, e)
{
    isRequired(s, e);
}

function OnTTNameValidation(s, e)
{
    isRequired(s, e);
}

function OnTTTransportTypeValidation(s, e) {
     
    var _id_transportTariff = $("#id_transportTariff").val();
    isRequired(s, e);

    //if(isRequired(s, e))
    //{
    //    OnTransportTariffSingleton(s, e, _id_transportTariff);
    //}

    
}

function OnDateInitTransportTariffValidation(s, e) {
    isRequired(s, e);
}

function OnDateEndTransportTariffValidation(s, e) {
    isRequired(s, e);
}

function OnTTDFishingSiteValidation(s, e)
{


    var _id_transportTariff = $("#id_transportTariff").val();

    var _id_transportTariffType = ASPxClientControl.GetControlCollection().GetByName("id_TransportTariffType").GetValue();

    // actualiza los valores ocultos de Tipo de Tarifario
    //GetInfoTransportTariffType(s.GetValue());
    
  //  gvTransportTariffDetail.PerformCallback({ id_transportTariffType: _id_transportTariffType, id_transportTariff: _id_transportTariff });

}

function OnTTDFishingSiteValidation2(s, e)
{
 
    var _isTerrestriel  = $("#hd-transporttariff-isterrestriel").val();
    var _isInternal     = $("#hd-transporttariff-isinternal").val();
    var idControl = "";
    if(!isRequired(s, e)) return;
    
    
    
    if(_isInternal)
    {
        idControl = "id_IceBagRange";        
    }

    if(_isTerrestriel)
    {
        idControl = "id_TransportSize";
    }

    ASPxClientControl.GetControlCollection().GetByName(idControl).Validate();
    

}

function OnTTDTransportSizeValidation(s, e) {

    // accion: validar si el tipo de tarifario es tipo terrestre
    var _isTerrestriel = boolInputHidden("hd-transporttariff-isterrestriel");
    if(_isTerrestriel) 
    {
        isRequired(s, e);
    }

}

function OnTTDIceBagRangeValidation(s, e) {

    // accion: validar si el tipo de tarifario es tipo terrestre
    var _isInternal = boolInputHidden("hd-transporttariff-isinternal");
    if (_isInternal)
    {
        isRequired(s, e)
    }

}
 

function OnTariffControlValidation(s, e)
{
    //isRequired(s, e);
    if (!TariffDateisValidate()) return;
    OnTransportTariffChangeDatesRange(s, e);
    

}

function OnTTChangeDatesRangeInit(s,e)
{
        
        ASPxClientControl.GetControlCollection().GetByName("dateEnd").Validate();
}



function OnTTValidateDatesRangeInit(s,e)
{
    //isRequired(s, e)
    
    if (!TariffDateisValidate()) return;
    OnTransportTariffChangeDatesRange(s, e);
}


function OnTTChangeDatesRangeEnd(s,e)
{
       // ASPxClientControl.GetControlCollection().GetByName("dateInit").Validate();
        ASPxClientControl.GetControlCollection().GetByName("dateInit").Validate();
   
}

function OnTTValidateDatesRangeEnd(s,e)
{
    //isRequired(s, e)
    if (!TariffDateisValidate()) return;
    OnTransportTariffChangeDatesRange(s, e);
}


function TariffDateisValidate()
{

    var validate = true;

    if( $("[name='dateInit']").val() == undefined  || $("[name='dateInit']").val() == undefined  ) return false;
    if( $("[name='dateInit']").val() == null  || $("[name='dateInit']").val() == null  ) return false;
    
    return validate;


}


/* accion: valida la consistencuia de las fechas */
/* accion: valida que rango de fechas no se repita para otro tarifario del mismo tipo  */
function OnTransportTariffChangeDatesRange(s,e)
{
    var _id_transportTariff = $("#id_transportTariff").val();
    var fechaIni = $("[name='dateInit']").val();
    var fechaFin = $("[name='dateEnd']").val();
    
    var id_transportTariffType = ASPxClientControl.GetControlCollection().GetByName("id_TransportTariffType").GetValue();


    if (fechaIni == undefined || fechaFin == undefined || id_transportTariffType == undefined) return;
    if (fechaIni == null || fechaFin == null || id_transportTariffType == null) return;
    if(fechaIni.length == 0 || fechaFin.length == 0 || id_transportTariffType.length == 0) return;

    $.ajax({
        url: "TransportTariff/TransportTarrifValidateConsistentDate",
        type: "post",
        data: {
            id_TrasportTariff: _id_transportTariff,
            FechaIni: fechaIni,
            FechaFin: fechaFin,
            id_tipoTarifario: id_transportTariffType
        },
        async: false,
        cache: false,
        error: function (error) {
             
        },
        beforeSend: function () {
           showLoading();
        },
        success: function (result) {
            
            if (!(result == null))
            {
                 
               e.isValid = result.isValid;
                 e.errorText = result.errorText;

                var _dateInit = ASPxClientControl.GetControlCollection().GetByName("dateInit");
                var _dateEnd = ASPxClientControl.GetControlCollection().GetByName("dateEnd");

                // 
                //_dateInit.ValueChanged.AddHandler(function (s, e) {
                //    e.isValid = result.isValid;
                //    e.errorText = result.errorText;
                //});

                //_dateEnd.ValueChanged.AddHandler(function (s, e) {
                //    e.isValid = result.isValid;
                //    e.errorText = result.errorText;
                //});

              //  ASPxClientControl.GetControlCollection().GetByName("dateEnd").isValid = false;
            }
        },
        complete: function () {
           hideLoading();
        }
    });


}


// accion: valida que un tipo de tarifario solo este una vez
function OnTransportTariffSingleton(s, e, _id_transportTariff) {


    var _id_transportTariffType = ASPxClientControl.GetControlCollection().GetByName("id_TransportTariffType").GetValue();

    if (_id_transportTariffType == undefined || _id_transportTariff == undefined) return;
    if (_id_transportTariffType == null || _id_transportTariff == null) return;

    $.ajax({
        url: "TransportTariff/TransportTarrifTypeSingleton",
        type: "post",
        data: {
            id_transportTariff: _id_transportTariff,
            id_transportTariffType: _id_transportTariffType
        },
        async: true,
        cache: false,
        error: function (error) {

        },
        beforeSend: function () {
            // showLoading();
        },
        success: function (result) {

            if (!(result == null)) {

                e.isValid = result.isValid;
                e.errorText = result.errorText;
            }
        },
        complete: function () {
            // hideLoading();
        }
    });


}

function OnFishingSiteSingleton(s, e) {

    var _id_transportTariff = $("#id_transportTariff").val();
    var _id_transportTariffDetail = 0;
    var _id_FishingSite = e.GetValue();
     

    if (_id_FishingSite == undefined || _id_FishingSite == null) return;

    $.ajax({
        url: "TransportTariff/TransportTarrifFishingSiteSingleton",
        type: "post",
        data: {
            id_transportTariff: _id_transportTariff,
            id_transportTariffDetail: _id_transportTariffDetail,
            id_fishingSite: _id_FishingSite

        },
        async: true,
        cache: false,
        error: function (error) {

        },
        beforeSend: function () {
            // showLoading();
        },
        success: function (result) {

            if (!(result == null)) {

                e.isValid = result.isValid;
                e.errorText = result.errorText;
            }
        },
        complete: function () {
            // hideLoading();
        }
    });


}


function OnFishingSiteTransportSizeSingleton(s, e) {

    var controls = ASPxClientControl.GetControlCollection();
    var _id_transportTariff = $("#id_transportTariff").val();    
    var _id_fishingSite = controls.GetByName("id_FishingSite").GetValue();
    var _id_transportSize = controls.GetByName("id_TransportSize").GetValue();
    var _id_transportTariffDetail = gvTransportTariffDetail.GetRowKey(e.visibleIndex);


    if (_id_fishingSite == undefined || _id_transportSize == undefined) return;
    if (_id_fishingSite == null || _id_transportSize == null) return;
    


    $.ajax({
        url: "TransportTariff/FishingSiteTransportSizeSingleton",
        type: "post",
        data: {
            id_transportTariff: _id_transportTariff,
            id_fishingSite: _id_fishingSite,
            id_transportSize: _id_transportSize,
            id_transportTariffDetail: _id_transportTariffDetail
        },
        async: false,
        cache: false,
        error: function (error) {

        },
        beforeSend: function () {
            // showLoading();
        },
        success: function (result) {

            if (!(result == null)) {
                
                e.isValid = result.isValid;
                e.errorText = result.errorText;
            }
        },
        complete: function () {
            // hideLoading();
        }
    });




}


function OnFishingSiteIceBagRangeSingleton(s, e) {

    var controls = ASPxClientControl.GetControlCollection();
    var _id_transportTariff = $("#id_transportTariff").val();    
    var _id_fishingSite = controls.GetByName("id_FishingSite").GetValue();
    var _id_iceBagRange = controls.GetByName("id_IceBagRange").GetValue();
    

    if (_id_fishingSite == undefined || _id_iceBagRange == undefined) return;
    if (_id_fishingSite == null || _id_iceBagRange == null) return;


    $.ajax({
        url: "TransportTariff/FishingSiteIceBagRangeSingleton",
        type: "post",
        data: 
            {
                id_transportTariff: _id_transportTariff,
                id_fishingSite: _id_fishingSite,
                id_iceBagRange: _id_iceBagRange
        },
        async: true,
        cache: false,
        error: function (error) {

        },
        beforeSend: function () {
            // showLoading();
        },
        success: function (result) {

            if (!(result == null)) {

                e.isValid = result.isValid;
                e.errorText = result.errorText;
            }
        },
        complete: function () {
            // hideLoading();
        }
    });


}
