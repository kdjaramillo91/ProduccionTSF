var OnTipoReporteQueryTextInit = function (s, e) {
    updateFilterControls();
};
var OnTipoReporteQueryTextValidate = function (s, e) {
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
};
var OnTipoReporteQueryTextSelectedIndexChanged = function (s, e) {
    CxC();
    updateFilterControls();
};
var updateFilterControls = function () {
    if (TipoReporteQueryText.GetValue() !== 'Desconocido' && TipoReporteQueryText.GetValue() !== 'Proveedores' && TipoReporteQueryText.GetValue() !== 'Clientes'
        && TipoReporteQueryText.GetValue() !== 'Productos' && TipoReporteQueryText.GetValue() !== 'PermisosBodegas'
        && TipoReporteQueryText.GetValue() !== 'OrdenCompra' && TipoReporteQueryText.GetValue() !== 'ProcesoInternoMovimiento' && TipoReporteQueryText.GetValue() !== 'EstadoBodegasPeriodo'
        && TipoReporteQueryText.GetValue() !== 'OrdenProduccionMasterizado' && TipoReporteQueryText.GetValue() !== 'PermisosPorUsuarios' && TipoReporteQueryText.GetValue() !== 'FormulacionProductos'){
        FechaInicioQueryLabel.SetEnabled(true);
        FechaInicioQueryText.SetEnabled(true);
        FechaFinalQueryText.SetEnabled(true);
    } else {
        FechaInicioQueryLabel.SetEnabled(false);
        FechaInicioQueryText.SetEnabled(false);
        FechaFinalQueryText.SetEnabled(false);
    }
};

var CxC = function () {
    if (TipoReporteQueryText.GetValue() !== 'ReporteCuentasPorCobrar') {
        $("#ReporteButton").hide();
    } else {
        $("#ReporteButton").show();
    }

}

var OnConsultarDatosButtonClick = function (s, e) {
    if (ASPxClientEdit.ValidateEditorsInContainerById("dataExportQueryForm", "", true)) {
        var queryData = {
            tipoReporte: TipoReporteQueryText.GetValue(),
            fechaInicio: FechaInicioQueryText.GetDate(),
            fechaFinal: FechaFinalQueryText.GetDate(),
            isCallback: false
        };
        $.ajax({
            url: "DataExport/Query",
            type: "post",
            contentType: 'application/json; charset=utf-8',
            dataType: 'html',
            data: JSON.stringify(queryData),
            async: true,
            cache: false,
            error: function (error) {
                console.error(error);
                try {
                    $("#results").html(error.responseText);
                }
                catch (error2) {
                    console.error(error2);
                    showErrorMessage("Ocurrió un error al procesar la respuesta de error.");
                }
                hideLoading();
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                $("#btnCollapse").click();
                $("#results").html(result);
                updateExportFormParameters();
            },
            complete: function () {
                hideLoading();
            }
        });
    }
};

var OnLimpiarDatosButtonClick = function (s, e) {
    TipoReporteQueryText.SetValue(null);
    FechaInicioQueryText.SetDate(null);
    FechaFinalQueryText.SetDate(null);
    $("#results").empty();
    $("#ReporteButton").hide();
    $('#tipoReporte').val('');
    $('#fechaInicio').val('');
    $('#fechaFinal').val('');
    $('#isCallback').val('');
};
var onDataExportGridViewBeginCallback = function (s, e) {
    e.customArgs["tipoReporte"] = TipoReporteQueryText.GetValue();
    e.customArgs["fechaInicio"] = FechaInicioQueryText.GetDate();
    e.customArgs["fechaFinal"] = FechaFinalQueryText.GetDate();
    e.customArgs["isCallback"] = true;
};
var updateExportFormParameters = function () {
    $('#tipoReporte').val(TipoReporteQueryText.GetValue());

    var fechaInicio = FechaInicioQueryText.GetDate();
    if (fechaInicio === null) {
        $('#fechaInicio').val('');
    } else {
        $('#fechaInicio').val(fechaInicio.toISOString());
    }

    var fechaFinal = FechaFinalQueryText.GetDate();
    if (fechaFinal === null) {
        $('#fechaFinal').val('');
    } else {
        $('#fechaFinal').val(fechaFinal.toISOString());
    }

    $('#isCallback').val(true);
};
function OnDataExportEmissionDateValidation(s, e) {
    if (TipoReporteQueryText.GetValue() === 'SaldoPorDiaMatriz') {
        OnDataExportEmissionDateValidation1(e);
    }
    else if (TipoReporteQueryText.GetValue() === 'InventarioMatriz') {
        OnDataExportEmissionDateValidation2(e);
    }
}


function OnDataExportEmissionDateValidation1(e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else {
        var strInicioDate = String(FechaInicioQueryText.GetDate());
        var strInicioDateDiv2Points = strInicioDate.split(":");
        var strInicioDateDiv2PointsWithSpace = strInicioDateDiv2Points[2].split(" ");
        var strFinDate = String(FechaFinalQueryText.GetDate());
        var strFinoDateDiv2Points = strFinDate.split(":");
        var strFinDateDiv2PointsWithSpace = strFinoDateDiv2Points[2].split(" ");
        var queryData = {
            startDate: JSON.stringify(strInicioDateDiv2Points[0] + ":" + strInicioDateDiv2Points[1]),//"Mon Jul 10 2017 20:07:05"),//_emissionDate.GetDate()),
            endDate: JSON.stringify(strFinoDateDiv2Points[0] + ":" + strFinoDateDiv2Points[1]),//"Mon Jul 10 2017 20:07:05"),//_emissionDate.GetDate()),
        };
        // 
        $.ajax({
            url: "DataExport/OnDateDataExportValidation",
            type: "post",
            data: queryData,
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result !== null) {
                    if (result.itsValided == 0) {
                        e.isValid = false;
                        e.errorText = result.Error;
                    }
                }
            },
            complete: function () {
                //hideLoading();
            }
        });

    }
}
function OnDataExportEmissionDateValidation2(e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else {
        var strInicioDate = String(FechaInicioQueryText.GetDate());
        var strInicioDateDiv2Points = strInicioDate.split(":");
        var strInicioDateDiv2PointsWithSpace = strInicioDateDiv2Points[2].split(" ");
        var strFinDate = String(FechaFinalQueryText.GetDate());
        var strFinoDateDiv2Points = strFinDate.split(":");
        var strFinDateDiv2PointsWithSpace = strFinoDateDiv2Points[2].split(" ");
        var queryData = {
            startDate: JSON.stringify(strInicioDateDiv2Points[0] + ":" + strInicioDateDiv2Points[1]),//"Mon Jul 10 2017 20:07:05"),//_emissionDate.GetDate()),
            endDate: JSON.stringify(strFinoDateDiv2Points[0] + ":" + strFinoDateDiv2Points[1]),//"Mon Jul 10 2017 20:07:05"),//_emissionDate.GetDate()),
        };
        // 
        $.ajax({
            url: "DataExport/OnDateDataExportDateValidation",
            type: "post",
            data: queryData,
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (result !== null) {
                    if (result.itsValided == 0) {
                        e.isValid = false;
                        e.errorText = result.Error;
                    }
                }
            },
            complete: function () {
                //hideLoading();
            }
        });

    }
}

var OnCuentasCobrar = function (e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo obligatorio";
    } else {
        // 
        var startDate = null, endDate = null;
        var strInicioDate = String(FechaInicioQueryText.GetDate());
        var strFinDate = String(FechaFinalQueryText.GetDate());
        if (strInicioDate != "null") {
            var strInicioDateDiv2Points = strInicioDate.split(":");
            var strInicioDateDiv2PointsWithSpace = strInicioDateDiv2Points[2].split(" ");
            startDate: JSON.stringify(strInicioDateDiv2Points[0] + ":" + strInicioDateDiv2Points[1]);//"Mon Jul 10 2017 20:07:05"),//_emissionDate.GetDate()),
        }
        if (strFinDate != "null") {
            var strFinoDateDiv2Points = strFinDate.split(":");
            var strFinDateDiv2PointsWithSpace = strFinoDateDiv2Points[2].split(" ");
            endDate: JSON.stringify(strFinoDateDiv2Points[0] + ":" + strFinoDateDiv2Points[1]);//"Mon Jul 10 2017 20:07:05"),//_emissionDate.GetDate()),
        }
        var queryData = {
            startDate,
            endDate,
        };
    }
    if (queryData != null) {
        $.ajax({
            url: "DataExport/PRCuentasCobrar",
            type: "post",
            data: queryData,
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
}