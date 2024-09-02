
var id_metricDestinyAux = 0;

function ComboBoxMetricOrigins_Init(s, e) {

    id_metricDestinyAux = id_metricDestiny.GetValue();
}

function ComboBoxMetricOrigins_SelectedIndexChanged(s, e) {
    id_metricDestiny.ClearItems();
    id_metricDestiny.SetValue(null);
    //var id_metricUnitConversion = $("#id_metricUnitConversion").val();
    //var esNuevo = (id_metricUnitConversion === "0");
    //var itemCompany = id_company.GetSelectedItem();
    //var item = id_metricOrigin.GetSelectedItem();
    //id_warehouse: s.GetValue()//,
    //if (item !== null && item !== undefined) {
    $.ajax({
        url: "MetricUnitConversion/MetricDestinyByMetricOrigin",
        type: "post",
        data: { id_metricOrigin: s.GetValue(), id_metricDestinyIni: id_metricDestinyAux },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            if (result !== null && result !== undefined) {
                var arrayFieldStr = [];
                arrayFieldStr.push("name");
                UpdateDetailObjects(id_metricDestiny, result.metricDestinys, arrayFieldStr);
            }
            //for (var i = 0; i < result.length; i++) {
            //    id_metricDestiny.AddItem(result[i].name, result[i].id);
            //}
        },
        complete: function () {
            //hideLoading();
        }
    });
    //}
}

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvMetricUnitConversions.UpdateEdit();
    }
}


function ButtonCancel_Click(s, e) {
    if (gvMetricUnitConversions !== null && gvMetricUnitConversions !== undefined) {
        gvMetricUnitConversions.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
