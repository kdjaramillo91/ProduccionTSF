
var idSizeAux = null;
//var sumGrossWeight = 0;
//var sumPoundsTrash = 0;
//var matrix = [];

function ComboBoxSize_Init(s, e) {
    var enabledSizeAux = $("#enabledSize").val();
    if (enabledSizeAux == "true" || enabledSizeAux == true) {
        ComboBoxSize.SetEnabled(true);
    } else {
        ComboBoxSize.SetEnabled(false);
    }
}

function ComboBoxTypeRomaneo_Init(s, e) {
    idSizeAux = ComboBoxSize.GetValue();
    ComboBoxSize.PerformCallback();
    UpdateDetails();
}

function ComboBoxTypeRomaneo_Change(s, e) {
    idSizeAux = null;
    ComboBoxSize.PerformCallback();
    UpdateDetails();
}

function ComboBoxSize_BeginCallback(s, e) {
    e.customArgs["idTypeRomaneo"] = ComboBoxTypeRomaneo.GetValue();
}

function ComboBoxSize_EndCallback(s, e) {
    ComboBoxSize.SetValue(idSizeAux);
}


Number.prototype.toFixedDown = function (digits) {
    var re = new RegExp("(\\d+\\.\\d{" + digits + "})(\\d)"),
        m = this.toString().match(re);
    return m ? parseFloat(m[1]) : this.valueOf();
};

function OnBatchEditEndEditing(s, e) {

    var drawerNumberIndex = s.GetColumnByField("drawerNumber").index;
    var drawerNumberValue = parseInt(e.rowValues[drawerNumberIndex].value);

    var grossWeightIndex = s.GetColumnByField("grossWeight").index;
    var grossWeightValue = parseInt(e.rowValues[grossWeightIndex].value);

    var umIndex = s.GetColumnByField("um").index;
    var umValue = e.rowValues[umIndex].value;

    var poundsTrashIndex = s.GetColumnByField("poundsTrash").index;
    var poundsTrashValue = parseInt(e.rowValues[poundsTrashIndex].value);

    UpdateRow(drawerNumberValue, grossWeightValue, poundsTrashValue);
    RefreshDetails(umValue, drawerNumberValue, grossWeightValue, poundsTrashValue);

}

function UpdateRow(drawerNumberValue, grossWeightValue, poundsTrashValue) {
    showLoading();

    var userData = {
        drawerNumberValue: drawerNumberValue,
        grossWeightValue: grossWeightValue,
        poundsTrashValue: poundsTrashValue
    };

    $.ajax({
        url: 'Romaneo/UpdateRow',
        type: 'post',
        data: userData,
        async: true,
        cache: false,
        success: function (result) {
            hideLoading();
        },
        error: function (result) {
            hideLoading();
        },
    });
}

function RefreshDetails(umValue, drawerNumberValue, grossWeightValue, poundsTrashValue) {
    var sumGrossWeight = 0;
    var sumPoundsTrash = 0;

    for (let rowDetail = 0; rowDetail < GridViewDetails.cpDetailCount; rowDetail++) {
        if (parseInt(GridViewDetails.batchEditApi.GetCellValue(rowDetail, 1)) == drawerNumberValue) {
            sumGrossWeight += grossWeightValue;
            sumPoundsTrash += poundsTrashValue;
        }
        else {
            sumGrossWeight += parseInt(GridViewDetails.batchEditApi.GetCellValue(rowDetail, 2));
            sumPoundsTrash += parseInt(GridViewDetails.batchEditApi.GetCellValue(rowDetail, 4));
        }
    }

    UpdateFields(sumGrossWeight, umValue, sumPoundsTrash);
}

function ShowCurrentItem(enabled) {
    var data = {
        id: $('#id_romaneo').val(),
        idProductionLotReception: 0,
        enabled: enabled
    }

    showPage("Romaneo/Edit", data);
}

function AddNewItem() {
    $.ajax({
        url: "Romaneo/PendingNew",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            // 
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

function EditCurrentItem() {
    showLoading();
    ShowCurrentItem(true);
}

function SaveCurrentItem() {
    SaveItem(false);
}

function AprovedItem() {
    showLoading();
    $.ajax({
        url: 'Romaneo/Approve',
        type: 'post',
        data: { id: $('#id_romaneo').val() },
        async: true,
        cache: false,
        error: function (result) {
            hideLoading();
            NotifyError("Error. " + result.Message);
        },
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error al Aprobar. " + result.Message);
                return;
            }

            ShowCurrentItem(false);
            hideLoading();
            NotifySuccess("Elemento Aprobado Satisfactoriamente. " + "Estado: " + result.Data);
        },
    });
}

function AprovedCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Aprobar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {

        if (dialogResult) {
            if ($("#enabled").val() == "true") {
                SaveItem(true);
            } else {
                AprovedItem();
            }
        }
    });
}

function ReverseCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Reversar el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'Romaneo/Reverse',
                type: 'post',
                data: { id: $('#id_romaneo').val() },
                async: true,
                cache: false,
                error: function (result) {
                    hideLoading();
                    NotifyError("Error. " + result.Message);
                },
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error al Reversar. " + result.Message);
                        return;
                    }

                    ShowCurrentItem(false);
                    hideLoading();
                    NotifySuccess("Elemento Reversado Satisfactoriamente. " + "Estado: " + result.Data);
                },
            });
        }
    });
}

function AnnulCurrentItem() {
    var result = DevExpress.ui.dialog.confirm("Desea Anular el Item Seleccionado?", "Confirmar");
    result.done(function (dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'Romaneo/Annul',
                type: 'post',
                data: { id: $('#id_romaneo').val() },
                async: true,
                cache: false,
                error: function (result) {
                    hideLoading();
                    NotifyError("Error. " + result.Message);
                },
                success: function (result) {
                    if (result.Code !== 0) {
                        hideLoading();
                        NotifyError("Error al Anular. " + result.Message);
                        return;
                    }

                    ShowCurrentItem(false);
                    hideLoading();
                    NotifySuccess("Elemento Anulado Satisfactoriamente. " + "Estado: " + result.Data);
                },
            });
        }
    });
}

function SaveDataUser() {

    var userData = {
        id: $('#id_romaneo').val(),
        id_reception: $('#id_reception').val(),
        dateTimeEmision: DateTimeEmision.GetValue(),
        idWeigher: ComboBoxWeigher.GetValue(),
        idAnalist: ComboBoxAnalist.GetValue(),
        reference: TextBoxReference.GetText(),
        description: MemoDescription.GetText(),
        //Detail
        idTypeRomaneo: ComboBoxTypeRomaneo.GetValue(),
        poundsTrash: SpinEditPoundsTrash.GetValue(),
        drawersNumber: SpinEditDrawers.GetValue(),
        totalPoundsGrossWeight: SpinEditPoundsGrossWeight.GetValue(),
        idSize: ComboBoxSize.GetValue(),
        percentTara: SpinEditPercentTara.GetValue(),
        totalPoundsNetWeight: SpinEditPoundsNetWeight.GetValue(),

        romaneoDetails: []
    }

    for (let rowDetail = 0; rowDetail < GridViewDetails.cpDetailCount; rowDetail++) {
        userData.romaneoDetails.push({
            id: GridViewDetails.batchEditApi.GetCellValue(rowDetail, 0),
            drawerNumber: parseInt(GridViewDetails.batchEditApi.GetCellValue(rowDetail, 1)),
            grossWeight: parseInt(GridViewDetails.batchEditApi.GetCellValue(rowDetail, 2)),
            um: GridViewDetails.batchEditApi.GetCellValue(rowDetail, 3),
            poundsTrash: parseInt(GridViewDetails.batchEditApi.GetCellValue(rowDetail, 4)),
        });
    }

    var romaneo = {
        jsonRomaneo: JSON.stringify(userData)
    };

    return romaneo;
}

function SaveItem(aproved) {
    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }

    $.ajax({
        url: 'Romaneo/Save',
        type: 'post',
        data: SaveDataUser(),
        async: true,
        cache: false,
        success: function (result) {
            if (result.Code !== 0) {
                hideLoading();
                NotifyError("Error. " + result.Message);
                return;
            }

            var id = result.Data;
            $('#id_romaneo').val(id);

            if (aproved)
                AprovedItem();
            else
                ShowCurrentItem(true);

            hideLoading();
            NotifySuccess("Elemento Guardado Satisfactoriamente.");
        },
        error: function (result) {
            hideLoading();
        },
    });
}

function IsValid(object) {
    return (object != null && object != undefined && object != "undefined");
}

function Validate() {
    var validate = true;
    var errors = "";

    if (!IsValid(DateTimeEmision) || DateTimeEmision.GetValue() == null) {
        errors += "Fecha Emisión es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(ComboBoxWeigher) || ComboBoxWeigher.GetValue() == null) {
        errors += "Pesador es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(ComboBoxAnalist) || ComboBoxAnalist.GetValue() == null) {
        errors += "Analista es un campo Obligatorio. \n\r";
        validate = false;
    }

    if (!IsValid(ComboBoxTypeRomaneo) || ComboBoxTypeRomaneo.GetValue() == null) {
        errors += "Tipo de Romaneo es un campo Obligatorio. \n\r";
        validate = false;
    } else {
        if (ComboBoxTypeRomaneo.GetValue() == 1)//Entero
        {
            if (!IsValid(ComboBoxSize) || ComboBoxSize.GetValue() == null) {
                errors += "Talla es un campo Obligatorio. \n\r";
                validate = false;
            }
        }
    }

    if (!IsValid(SpinEditDrawers) || SpinEditDrawers.GetValue() == null || SpinEditDrawers.GetValue() == 0) {
        errors += "No. Gavetas es un campo Obligatorio. \n\r";
        validate = false;
    }

    if (!IsValid(SpinEditPercentTara) || SpinEditPercentTara.GetValue() == null) {
        errors += "% Tara es un campo Obligatorio. \n\r";
        validate = false;
    }
    
    if (validate == false) {
        NotifyError("Error. " + errors);
    }

    return validate;
}

function ButtonUpdate_Click() {
    SaveItem(false);
}

function ButtonCancel_Click() {
    RedirecBack();
}

function RedirecBack() {
    showPage("Romaneo/Index");
}

function SpinEditDrawers_Change() {
    UpdateDetails();
}

function UpdateDetails() {
    showLoading();

    var userData = {
        drawersNumber: SpinEditDrawers.GetValue(),
        idTypeRomaneo: ComboBoxTypeRomaneo.GetValue(),
        percentTara: SpinEditPercentTara.GetValue(),
        enabled: $("#enabled").val()
    };

    $.ajax({
        url: 'Romaneo/GridViewDetails',
        type: 'post',
        data: userData,
        async: true,
        cache: false,
        success: function (result) {
            $("#divGridViewDetails").html(result);

            $.ajax({
                url: 'Romaneo/GetTypeRomaneo',
                type: 'post',
                data: { idTypeRomaneo: ComboBoxTypeRomaneo.GetValue()},
                async: true,
                cache: false,
                success: function (result) {
                    $("#enabledSize").val(result.enabledSize);
                    ComboBoxSize_Init();
                    var umAux = result.codeTypeRomaneo == "ENT" ? "Kg" : "Lbs";
                    RefreshDetails(umAux, null, 0, 0);
                    hideLoading();
                },
                error: function (result) {
                    hideLoading();
                },
            });

            //var umAux = userData.idTypeRomaneo == 1 ? "Kg" : "LB";
            //RefreshDetails(umAux, null, 0, 0);
            //hideLoading();
        },
        error: function (result) {
            hideLoading();
        },
    });
}

function floorFigure(figure, decimals) {
    if (!decimals) decimals = 2;
    var d = Math.pow(10, decimals);
    return (parseInt(figure * d) / d).toFixed(decimals);
};

function UpdateFields(sumGrossWeight, um, sumPoundsTrash) {
    showLoading();

    var count = GridViewDetails.cpDetailCount;

    SpinEditPoundsTrash.SetValue(sumPoundsTrash);

    var sumGrossWeightAux = um == "Kg" ? (sumGrossWeight * 2.2046) : sumGrossWeight;
    sumGrossWeightAux = floorFigure(sumGrossWeightAux, 2);
    SpinEditPoundsGrossWeight.SetValue(sumGrossWeightAux);

    var sumGrossNetWeightAux = sumGrossWeightAux - (sumGrossWeightAux * (SpinEditPercentTara.GetValue() / 100));
    sumGrossNetWeightAux = floorFigure(sumGrossNetWeightAux, 2);
    SpinEditPoundsNetWeight.SetValue(sumGrossNetWeightAux);

    hideLoading();
}

function InitializePagination() {

    if ($("#id_romaneo").val() !== 0) {

        var current_page = 1;
        var max_page = 1;
        $.ajax({
            url: "Romaneo/InitializePagination",
            type: "post",
            data: { id: $("#id_romaneo").val() },
            async: false,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
            },
            success: function (result) {
                max_page = result.maximunPages;
                current_page = result.currentPage;
            },
            complete: function () {
            }
        });

        $('.pagination').jqPagination({
            current_page: current_page,
            max_page: max_page,
            page_string: "{current_page} de {max_page}",
            paged: function (page) {
                showPage("Romaneo/Pagination", { page: page });
            }
        });
    }
}

function Init() {
};

$(function () {
    InitializePagination();
    Init();
});