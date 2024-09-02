// BUTTONS ACTIONS

function AddNewItem(s, e) {
    gvSimpleFormula.AddNewRow();
}

function RemoveItems(s, e) {
    gvSimpleFormula.GetSelectedFieldValues("id", function (values) {

        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }

        showConfirmationDialog(function () {
            gvSimpleFormula.PerformCallback({ ids: selectedRows });
            gvSimpleFormula.UnselectRows();
        });
    });
}

function RefreshGrid(s, e) {
    gvSimpleFormula.Refresh();
}

var keyToCopy = null;

function CopyItems(s, e) {
    gvSimpleFormula.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvSimpleFormula.AddNewRow();
            keyToCopy = 0;

        }
    });
}

function Print(s, e) {
    gvSimpleFormula.GetSelectedFieldValues("id", function (values) {

        var _url = "SimpleFormula/SimpleFormulaReport";

        var data = null;
        if (values.length === 1) {
            _url = "SimpleFormula/SimpleFormulaDetailReport";
            data = { id: values[0] };
        }
        $.ajax({
            url: _url,
            type: "post",
            data: data,
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
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


    });

}

function importFile(s, e) {
    console.log('Funcionalidad no implementada');
}

function ImportFile(data) {
    uploadFile("SimpleFormula/ImportFileSimpleFormula", data, function (result) {
        gvSimpleFormula.Refresh();
    });
}

// GRIDVIEW CLIENT SIDE EVENTES

function OnGridViewInit() {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    if (!(DataHashTag == null || DataHashTag == undefined)) {
        if (!(dataSources == null || dataSources == undefined)) {
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
                            DataHashTag.onSetDataHashTagWithDataSources(result.lsData);
                            //console.log(result.lsData);
                        }
                    }
                },
                complete: function () {
                }
            });
        }
        //DataHashTag.onSetDataHashTag();
    }
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

function GridViewItemsCustomCommandButton_Click(s, e) {
    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}


// SELECTION

function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvSimpleFormula.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvSimpleFormula.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    //if ($("#selectAllMode").val() != "AllPages") {
    SetElementVisibility("lnkSelectAllRows", gvSimpleFormula.GetSelectedRowCount() > 0 && gvSimpleFormula.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvSimpleFormula.GetSelectedRowCount() > 0);
    //}

    btnRemove.SetEnabled(gvSimpleFormula.GetSelectedRowCount() > 0);
    btnCopy.SetEnabled(gvSimpleFormula.GetSelectedRowCount() === 1);
}

function GetSelectedFilteredRowCount() {
    return gvSimpleFormula.cpFilteredRowCountWithoutPage + gvSimpleFormula.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvSimpleFormula.SelectRows();
}

function UnselectAllRows() {
    gvSimpleFormula.UnselectRows();
}


// MAIN FUNCTIONS


function init() {

}

$(function () {
    init();
});
