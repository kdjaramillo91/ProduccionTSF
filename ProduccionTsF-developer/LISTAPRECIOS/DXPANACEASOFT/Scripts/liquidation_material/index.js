
function DateEditFechaInicioEmision_Changed() {
    DateEditFechaFinEmision.SetMinDate(DateEditFechaInicioEmision.GetDate());
    if (DateEditFechaInicioEmision.GetDate() > DateEditFechaFinEmision.GetDate()) {
        DateEditFechaFinEmision.SetDate(DateEditFechaInicioEmision.GetDate());
    }
}

function DateEditFechaInicioGuia_Changed() {
    DateEditFechaFinGuia.SetMinDate(DateEditFechaInicioGuia.GetDate());
    if (DateEditFechaInicioGuia.GetDate() > DateEditFechaFinGuia.GetDate()) {
        DateEditFechaFinGuia.SetDate(DateEditFechaInicioGuia.GetDate());
    }
}
    
function BuscarClick() {

    $("#btnCollapse").click();

    var dateInicioEmision = DateEditFechaInicioEmision.GetDate();
    var yearInicioEmision = dateInicioEmision.getFullYear();
    var monthInicioEmision = dateInicioEmision.getMonth() + 1;
    var dayInicioEmision = dateInicioEmision.getDate();
    var fechaInicioEmisionAux = dayInicioEmision + "/" + monthInicioEmision + "/" + yearInicioEmision;

    var dateFinEmision = DateEditFechaFinEmision.GetDate();
    var yearFinEmision = dateFinEmision.getFullYear();
    var monthFinEmision = dateFinEmision.getMonth() + 1;
    var dayFinEmision = dateFinEmision.getDate();
    var fechaFinEmisionAux = dayFinEmision + "/" + monthFinEmision + "/" + yearFinEmision;

    
    

    var dateInicioGuia = DateEditFechaInicioGuia.GetDate();
    var fechaInicioGuiaAux = null;
    if (dateInicioGuia != "" && dateInicioGuia != null) {
        var yearInicioGuia = dateInicioGuia.getFullYear();
        var monthInicioGuia = dateInicioGuia.getMonth() + 1;
        var dayInicioGuia = dateInicioGuia.getDate();
        fechaInicioGuiaAux = dayInicioGuia + "/" + monthInicioGuia + "/" + yearInicioGuia;
    };
    
    var dateFinGuia = DateEditFechaFinGuia.GetDate();
    var fechaFinGuiaAux = null;
    if (dateFinGuia != "" && dateFinGuia != null) {
        var yearFinGuia = dateFinGuia.getFullYear();
        var monthFinGuia = dateFinGuia.getMonth() + 1;
        var dayFinGuia = dateFinGuia.getDate();
        fechaFinGuiaAux = dayFinGuia + "/" + monthFinGuia + "/" + yearFinGuia;
    };
   

    var data = {
        id_estado:           ComboBoxEstados.GetValue(),
        numeroLiquidacion:   NumeroLiquidacion.GetText(),
        fechaInicioEmision: fechaInicioEmisionAux,
        fechaFinEmision: fechaFinEmisionAux,
        id_proveedor: ComboBoxProveedores.GetValue(),  
        id_producto: ComboBoxProductos.GetValue(), 
        fechaInicioGuia: fechaInicioGuiaAux,
        fechaFinGuia: fechaFinGuiaAux,
        numeroGuia: NumeroGuia.GetText()
    }

    showPartialPage($("#grid"), 'LiquidationMaterial/SearchResult', data);
    
}

function NuevoClick() {

    $.ajax({
        url: "LiquidationMaterial/ReceptionMaterialApproved",
        type: "post",
        data: null,
        async: true,
        cache: false,
        error: function (error) {
            debugger;
            NotifyError("Error. " + error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $("#maincontent").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

function LimpiarClick() {
    var dayNow = new Date();

    ComboBoxEstados.SetValue(null);
    NumeroLiquidacion.SetText(null);
    DateEditFechaInicioEmision.SetDate(new Date(dayNow.getFullYear(), dayNow.getMonth(), 1));
    DateEditFechaFinEmision.SetDate(dayNow);
    ComboBoxProveedores.SetValue(null);
    ComboBoxProductos.SetValue(null);
    DateEditFechaInicioGuia.SetDate(null);
    DateEditFechaFinGuia.SetDate(null);
    NumeroGuia.SetText(null);
}

function Init() {
    $("#btnCollapse").click(function (event) {
        if ($("#filterFormIcon").hasClass("fa-chevron-up")) {
            $("#grid").css("display", "");
        } else {
            $("#grid").css("display", "none");
        }
    });
}

$(function () {
    Init();
});

