
function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    if (valid) {
        gvSimpleFormula.UpdateEdit();
    }
    DataHashTag.onSetTributeNull();
    DataHashTag.onClearDataSet();
}

function ButtonCancel_Click(s, e) {
    
    if (gvSimpleFormula !== null && gvSimpleFormula !== undefined) {
        gvSimpleFormula.CancelEdit();
        DataHashTag.onSetTributeNull();
        DataHashTag.onClearDataSet();
    }
}

function OnDataSources_SelectedIndexChanged(s, e) {
    let dataSourcesValue = dataSources.GetValue();
    
    $.ajax({
        url: "SimpleFormula/GetDataSourceByName",
        type: "post",
        data: { id_datasource: dataSourcesValue },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {

        },
        success: function (result) {
            if (result == undefined) {
                console.log("No hay Datos para Data Source de HashTag 1");
                return;
            }
            if (result == null) {
                console.log("No hay Datos para Data Source de HashTag 2");
                return;
            }
            if (!result.isValid) {
                console.log("No hay Datos para Data Source de HashTag 2");
                return;
            }
            if (!(result.lsData == null || result.lsData == undefined)) {
                if (result.lsData.length > 0) {
                    DataHashTag.onSetDataHashTagWithDataSources(result.lsData, dataSourcesValue);
                    
                }
            }
        },
        complete: function () {
        }
    });
}
