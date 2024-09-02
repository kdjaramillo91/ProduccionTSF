
function DateEditFechaInicio_Changed() {
    DateEditFechaFin.SetMinDate(DateEditFechaInicio.GetDate());
    if (DateEditFechaInicio.GetDate() > DateEditFechaFin.GetDate()) {
        DateEditFechaFin.SetDate(DateEditFechaInicio.GetDate());
    }
}

function BuscarClick() {

    var dateInicio = DateEditFechaInicio.GetDate();
    var yearInicio = dateInicio.getFullYear(); 
    var monthInicio = dateInicio.getMonth() + 1;
    var dayInicio = dateInicio.getDate();

    var dateFin = DateEditFechaFin.GetDate();
    var yearFin = dateFin.getFullYear();
    var monthFin = dateFin.getMonth() + 1;
    var dayFin = dateFin.getDate();

    var data = {
        fechaInicio: dayInicio + "/" + monthInicio + "/" + yearInicio,
        fechaFin: dayFin + "/" + monthFin + "/" + yearFin,
        id_estado: ComboBoxEstados.GetValue(),
        id_tipoLista: ComboBoxTipoLista.GetValue(),
        id_proveedor: ComboBoxProveedores.GetValue(),
        id_grupo: ComboBoxGrupos.GetValue(),
        id_tipoListaCamaron: ComboBoxTipoListaCamaron.GetValue(),
        id_responsable: ComboBoxCompradores.GetValue(),
        id_certification: ComboBoxCertificaciones.GetValue()
    };

    showPartialPage($("#grid"), 'PriceListResponsive/SearchResult', data);
    
}

function NuevoClick() {

    var data = {
        id: 0,
        enabled: true
    };

    $.ajax({
        url: "PriceListResponsive/Edit",
        type: "post",
        data: data,
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

function PendintClick() {

    $.ajax({
        url: "PriceListResponsive/PendingApprove",
        type: "post",
        data: data,
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

    DateEditFechaInicio.SetDate(new Date(dayNow.getFullYear(), dayNow.getMonth(), 1));
    DateEditFechaFin.SetDate(dayNow);

    ComboBoxEstados.SetValue(null);
    ComboBoxGrupos.SetValue(null);
    ComboBoxProveedores.SetValue(null);
    ComboBoxCompradores.SetValue(null);
    ComboBoxTipoListaCamaron.SetValue(null);
    ComboBoxTipoLista.SetValue(null);
    ComboBoxCertificaciones.SetValue(null);
}

$(function () {
});

