
// GRIDVIEW PURCHASE ORDERS RESULT ACTIONS BUTTONS

function Print(s, e) {
    var dateInicio = startEmissionDate.GetDate();
    if (dateInicio == null)
        dateInicio = new Date(2010, 0, 1);

    var yearInicio = dateInicio.getFullYear();
    var monthInicio = dateInicio.getMonth() + 1;
    var dayInicio = dateInicio.getDate();

    var dateFin = endEmissionDate.GetDate();

    if (dateFin == null)
        dateFin = new Date();

    var yearFin = dateFin.getFullYear();
    var monthFin = dateFin.getMonth() + 1;
    var dayFin = dateFin.getDate();

    var startEmissionDateFinal = dayInicio + "/" + monthInicio + "/" + yearInicio;
    var endEmissionDateFinal = dayFin + "/" + monthFin + "/" + yearFin;

    var data = "startEmissionDateFinal=" + startEmissionDateFinal + "&" + "endEmissionDateFinal=" + endEmissionDateFinal + "&" + $("#formFilterKardex").serialize();

    //var data = $("#formFilterKardex").serialize();
    $.ajax({
        url: 'Kardex/InventoryKardexReport',
        data: data,
        async: true,
        cache: false,
        type: 'POST',
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            try {
                if (result !== undefined) {
                    var reportTdr = result.nameQS;
                    var url = 'ReportProd/Report?trepd=' + reportTdr;
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

// SELECTION
function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewKardex_BeginCallback(s, e) {
    e.customArgs["settingKardex"] = gvKardexDetails.cpSettingKardex;
}

function OnGridViewKardexExcel_BeginCallback(s, e) {
    e.customArgs["settingKardex"] = gvKardexExcelDetails.cpSettingKardex;
}

function onExportExcelClick(s, e) {
    ExportExcel();
}

var ExportExcel = function () {
    $.ajax({
        url: "Kardex/ExportKardexExcel",
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
            if ((result === null || result === undefined)) {
                return;
            }
            if (!(result.isOk)) {
                NotifyWarning(result.message);
            } else {
                window.open(window.location.origin + '/Kardex/DownloadExcel?fileName=' + result.fileName, "_blank");
            }
        },
        complete: function () {
            hideLoading();
        }
    });
};

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvKardexDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvKardexDetails.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvKardexDetails.GetSelectedRowCount() > 0 && gvKardexDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvKardexDetails.GetSelectedRowCount() > 0);
    //}

}

function GetSelectedFilteredRowCount() {
    return gvKardexDetails.cpFilteredRowCountWithoutPage + gvKardexDetails.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvKardexDetails.SelectRows();
}

function UnSelectRows() {
    gvKardexDetails.UnselectRows();
}

//function OnClickEditInventoryMove(s, e) {
//    var data = {
//        id: gvKardexDetails.GetRowKey(e.visibleIndex)
//    };
//    showPage("InventoryMove/InventoryMoveEditFormPartial", data);
//}

function GridViewShowPagefromLinkButton_Click(id, type) {
    var data2 = {
        id: id,
        urlToReturn: "Kardex/Index",
        //tabSelected: 2,//Seleccionado el tab 3 de Matreria Prima
        arrayTempDataKeep: ["KardexFilter"]
    };
    if (type === "Item") {
        
        //console.log("data2.id: " + data2.id);
        //console.log("data2: " + data2);
        showPagefromLink("Item/FormEditItem", data2);

    } else {
        if (type === "Recepcion") {
            showPagefromLink("ProductionLotReception/ProductionLotReceptionFormEditPartial", data2);
        } else {
            if (type === "Proceso") {
                showPagefromLink("ProductionLotProcess/ProductionLotProcessFormEditPartial", data2);
            } else {
                console.log("nothing");
            }
        }
    }
    //$.ajax({
    //    url: "Kardex/UpdateKardexFilterAndGridSetting",
    //    type: "post",
    //    data: data,
    //    async: true,
    //    cache: false,
    //    error: function (error) {
    //        console.log(error);
    //    },
    //    beforeSend: function () {
    //        //showLoading();
    //    },
    //    success: function (result) {
    //        if (result !== null) {
                
    //        }
    //    },
    //    complete: function () {
    //        //hideLoading();
    //    }
    //});
    
}

// MAIN FUNCTIONS

function init() {
    
}

$(function () {
    init();
});