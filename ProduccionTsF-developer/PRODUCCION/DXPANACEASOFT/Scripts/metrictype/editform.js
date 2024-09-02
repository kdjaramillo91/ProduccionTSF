
//function GridViewMetricTypeMetricUnitsDetails_BeginCallback(s, e) {
//    e.customArgs["id_metricType"] = $("#id_metricType").val();
//}

function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvMetricTypes.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvMetricTypes !== null && gvMetricTypes !== undefined) {
        gvMetricTypes.CancelEdit();
    } /*else if (dialogAddDocumentType !== null || dialogAddDocumentType !== undefined) {
        dialogAddDocumentType.Hide();
    }*/
}
