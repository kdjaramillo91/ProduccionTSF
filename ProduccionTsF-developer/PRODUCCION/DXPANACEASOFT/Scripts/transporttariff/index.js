// BUTTONS ACTIONS

function AddNewItem(s, e) {
   
    gvTransportTariff.AddNewRow();
}

function RemoveItems(s, e) {
    gvTransportTariff.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }
        showConfirmationDialog(function () {
            gvTransportTariff.PerformCallback({ ids: selectedRows });
            gvTransportTariff.UnselectRows();
        });
    });
}

function RefreshGrid(s, e) {
    gvTransportTariff.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvTransportTariff.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvTransportTariff.AddNewRow();
            keyToCopy = 0;

        }
    });
}


function ImportFile(data) {
    /* uploadFile("ItemType/ImportFileItemType", data, function (result) {
         gvItemTypes.Refresh();
     });*/
}

function Print(s, e) {
    $.ajax({
        url: "TransportTariff/PrintTariffReport",
        type: "post",
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            try {
                if (result != undefined) {
                    var reportTdr = result.nameQS;
                    var url = 'ReportProd/Index?trepd=' + reportTdr;
                    newWindow = window.open(url, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                    newWindow.focus();
                    hideLoading();
                }
            }
            catch (err) {
                hideLoading();
            }
        },
        complete: function () {
        hideLoading();
        }
   });
}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}

/// TransporTariff-Partial

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}


function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

 
function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvTransportTariff.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvTransportTariff.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvTransportTariff.GetSelectedRowCount() > 0 && gvTransportTariff.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvTransportTariff.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvTransportTariff.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvTransportTariff.cpFilteredRowCountWithoutPage + gvTransportTariff.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvTransportTariff.SelectRows();
}

function UnselectAllRows() {
    gvTransportTariff.UnselectRows();
}

/// TransporTariffsDetail-Partial

function TransportTariffDetail_OnBeginCallback(s, e) {

    var objtransportTariffType = ASPxClientControl.GetControlCollection().GetByName("id_TransportTariffType");
    e.customArgs['id_transportTariff'] = $("#id_transportTariff").val();    
    e.customArgs['id_transportTariffType'] = (objtransportTariffType != undefined) ? ( (objtransportTariffType != null)?objtransportTariffType.GetValue():0 ):0 ;   

}

function OnTTDFishingSiteValid(s, e)
{

    if (typeof (gvTransportTariffDetail) == "undefined") {

        if (gvTransportTariffDetail != null) {

            if (gvTransportTariffDetail.editState == 0) return false;

        }

    }

    return true;
    
            
}

         
function TransportTariffType_SelectedIndexChanged(s,e)
{

    var _id_transportTariff = $("#id_transportTariff").val();
    
    /*Validacion rangos Fechas*/    
    ASPxClientControl.GetControlCollection().GetByName("dateInit").Validate();
    ASPxClientControl.GetControlCollection().GetByName("dateEnd").Validate();
    
    // actualiza los valores ocultos de Tipo de Tarifario
    GetInfoTransportTariffType(s.GetValue());
   
      
    if (typeof (gvTransportTariffDetail) != "undefined" )
    {
        if (gvTransportTariffDetail != null)
        {
            // if (gvTransportTariffDetail.editState == 0)
            gvTransportTariffDetail.PerformCallback({ id_transportTariffType: s.GetValue(), id_transportTariff: _id_transportTariff });
            return false;
        }
    }
    

    var _id_transportTariff = $("#id_transportTariff").val();
    var _id_transportTariffType = ASPxClientControl.GetControlCollection().GetByName("id_TransportTariffType").GetValue();

    if (typeof (_id_transportTariffType) == "undefined") return;
    if ( _id_transportTariffType ==  null ) return;

    
    $.ajax({
        url: "TransportTariff/TransportTariffDetail",
        type: "post",
        data: {
            id_transportTariff: _id_transportTariff,
            id_transportTariffType: _id_transportTariffType
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
                $("#objTariffTransportDetail").empty();
                $("#objTariffTransportDetail").replaceWith(result);
            }
        },
        complete: function () {
            // hideLoading();
        }
    });
 
        

    

}

function GridViewTransportTariffCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}

// Aux. Funcions
function GetInfoTransportTariffType(id_transportTariffType)
{

    $.ajax({
    url: "TransportTariff/ObtainInfoTransportType",
        type: "post",
        data: {
            id_transportTariffType: id_transportTariffType
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
                $("#hd-transporttariff-isinternal").val(result.isInternal);
                $("#hd-transporttariff-isterrestriel").val(result.isTerrestriel);
                
            }
        },
        complete: function () {
            // hideLoading();
        }
    });


}

function OnTTDFishingSiteSelectedIndexChanged(s, e)
{
    // identificar si es nuevo 
    var maControls = ASPxClientControl.GetControlCollection();

    var _isInternal = boolInputHidden("hd-transporttariff-isinternal");
    var _isTerrestriel = boolInputHidden("hd-transporttariff-isterrestriel");
    var _fishingSite = maControls.GetByName("id_FishingSite");

    if (_isInternal == undefined || _isTerrestriel == undefined) return;
    if (_isInternal == null || _isTerrestriel == null) return;
    if (!_isTerrestriel && !_isInternal) return;
    if ( _fishingSite == undefined ) return;
    if ( _fishingSite == null ) return;
     
    
    var _id_transportTariff         = $("#id_transportTariff").val();
    var _id_transportTariffType     = maControls.GetByName("id_TransportTariffType").GetValue();
    var _id_transportTariffDetail = gvTransportTariffDetail.cpEditingRowKey; //gvTransportTariffDetail.GetRowKey(e.visibleIndex);
                                    
    var _id_fishingSite = _fishingSite.GetValue();

    if (_id_fishingSite == undefined || _id_transportTariffType == undefined) return;
    if (_id_fishingSite == null  || _id_transportTariffType == null) return;


    /* Obtener valor previo */
    var strNameControl = "";    
    var arrayFieldStr = [];
   
    if ( _isInternal)
    {
        strNameControl = "id_IceBagRange";
        //arrayFieldStr.push("id");
        arrayFieldStr.push("name");
        arrayFieldStr.push("range_ini");
        arrayFieldStr.push("range_end");

    }else if(_isTerrestriel)
    {
        strNameControl = "id_TransportSize";
        //arrayFieldStr.push("id");
        arrayFieldStr.push("code");
        arrayFieldStr.push("name");
    }

    
    if (strNameControl.length == 0) return;
    
    var objDinamycControl = maControls.GetByName(strNameControl);
    var datOldValueControl = objDinamycControl.GetValue();
    objDinamycControl.ClearItems();

    $.ajax({
        url: "TransportTariff/GetDinamycComboData",
        type: "post",
        data: {
                id_transportTariff: _id_transportTariff,
                id_transportTariffDetail: _id_transportTariffDetail,
                id_transportTariffType     :_id_transportTariffType,     
                id_fishingSite             :_id_fishingSite             
        },
        async: true,
        cache: false,
        error: function (error) {

        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            
            if (!(result == null)) {


                UpdateDetailObjects(objDinamycControl, result._dynamicdata, arrayFieldStr);

                var isSameValue = findJsonData(result._dynamicdata, "id", datOldValueControl);

                if (isSameValue) {
                    objDinamycControl.SetValue(datOldValueControl);
                }


            }
        },
        complete: function () {
            hideLoading();
        }
    });
    


}

// MAIN FUNCTIONS

function init() {
   
    $("#btnImport").hide();
   // btnApprove.SetEnabled(result.btnApprove);
    //btnImport.SetEnabled(false);
}

$(function () {
    init();
});