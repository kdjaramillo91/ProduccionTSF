
// DIALOG BUTTONS ACTIONS
function Update(approve) {

    var valid = true;
    var validMainTabQualityControl = ASPxClientEdit.ValidateEditorsInContainerById("mainTabQualityControl", null, true);
    var validMainTabDetail = ASPxClientEdit.ValidateEditorsInContainerById("mainTabDetailOld", null, true);
    var validConformityHeader = ASPxClientEdit.ValidateEditorsInContainerById("conformityHeader", null, true);
    var validMainTabSupplierTraceability = true;
    if ($("#aSettingTPC").val() === "SI") {
        validMainTabSupplierTraceability = ASPxClientEdit.ValidateEditorsInContainerById("mainTabSupplierTraceability", null, true);
    }

    if (validMainTabQualityControl) {
        UpdateTabImage({ isValid: true }, "tabQualityControl");
    } else {
        UpdateTabImage({ isValid: false }, "tabQualityControl");
        valid = false;
    }
    if (validMainTabDetail === false) {
        valid = false;
    }
    if (validConformityHeader === false) {
        valid = false;
    }

    if (validMainTabSupplierTraceability) {
        UpdateTabImage({ isValid: true }, "tabSupplierTraceability");
    } else {
        UpdateTabImage({ isValid: false }, "tabSupplierTraceability");
        valid = false;
    }

    var qualityControlTimeTmp = qualityControlTime.GetText();
    var monerTimeTmp = monerTime.GetText();

    if (valid) {

        var id = $("#id_qualityControl").val();

        var data = "id=" + id + "&qualityControlTime=" + qualityControlTimeTmp + "&" + "&monerTime=" + monerTimeTmp + "&" +
            "approve=" + approve + "&" +
            $("#formEditQualityControl").serialize();

        var url = (id === "0") ? "QualityControl/QualityControlPartialAddNew"
            : "QualityControl/QualityControlPartialUpdate";
        showForm(url, data);

    }

}

function ButtonUpdate_Click(s, e) {

    Update(false);
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
    var isBtnToReturn = false;
    if ($("#isBtnToReturn") != undefined) {
        isBtnToReturn = $("#isBtnToReturn").val();
    }

    if (isBtnToReturn == "True" || isBtnToReturn == "true" || isBtnToReturn == true) {
        btnToReturn_click(s, e);
    } else {
        showPage("QualityControl/Index", null);
    }

}

//QUALITY CONTROL BUTTONS ACTIONS

function AddNewDocument(s, e) {
    // 
    if ($("#codeQualityControlConfiguration").val() === "QARMP") {
        $.ajax({
            url: "QualityControl/Index",
            type: "post",
            data: null,
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
                AddNewQualityControlFromRMP();
            }
        });
    } else {
        var data = {
            id: 0
        };
        showPage("QualityControl/FormEditQualityControl", data);
    }

}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    //ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {
    //showPage("PurchaseOrder/PurchaseOrderCopy", { id: $("#id_order").val() });
}

function ApproveDocument(s, e) {
    showConfirmationDialog(function () {
        Update(true);
    }, "¿Desea aprobar el Análisis de Calidad?");

}

function AutorizeDocument(s, e) {
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_order").val()
    //    };
    //    showForm("PurchaseOrder/Autorize", data);
    //}, "¿Desea autorizar el documento?");
}

function ProtectDocument(s, e) {
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_order").val()
    //    };
    //    showForm("PurchaseOrder/Protect", data);
    //}, "¿Desea cerrar el documento?");
}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_qualityControl").val()
        };
        showForm("QualityControl/Cancel", data);
    }, "¿Desea anular el Análisis de Calidad?");
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_order").val()
    //    };
    //    showForm("PurchaseOrder/Cancel", data);
    //}, "¿Desea anular el documento?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_qualityControl").val()
        };
        showForm("QualityControl/Revert", data);
    }, "¿Desea reversar el Análisis de Calidad?");
    //showConfirmationDialog(function () {
    //    var data = {
    //        id: $("#id_order").val()
    //    };
    //    showForm("PurchaseOrder/Revert", data);
    //}, "¿Desea reversar el documento?");
}

function ShowDocumentHistory(s, e) {

}


function PrintDocument(s, e) {
    var _url = "QualityControl/PrintQualityCOntrolReport";
    var id = $("#id_qualityControl").val();

    if (!(id == 0) && !(id == null)) {
        $.ajax({
            url: _url,
            data: { id_Quality: id },
            async: true,
            cache: false,
            type: 'POST',
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


//QUALITY CONTROL

function QualityControlConfiguration_SelectedIndexChanged(s, e) {
    //id_documentState.ClearItems();
    //id_documentState.SetValue(null);

    //id_businessPartner.ClearItems();
    //id_businessPartner.SetValue(null);

    $.ajax({
        url: "QualityControl/RefreshQualityControlDetail",//ItemPlaningDetailsData",
        type: "post",
        data: { id_qualityControlConfiguration: s.GetValue() },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            //showLoading();
        },
        success: function (result) {
            gvQualityControlDetails.UnselectRows();
            gvQualityControlDetails.PerformCallback();
            tabSupplierTraceability.show();
            //if (result !== null) {

            //    //var arrayFieldStr = [];
            //    //arrayFieldStr.push("name");
            //    //UpdateDetailObjects(id_documentState, result.documentStates, arrayFieldStr);
            //    $("#codeBusinessOportunityDocumentType").val(result.codeBusinessOportunityDocumentType);
            //    var arrayFieldStr = [];
            //    arrayFieldStr.push("fullname_businessName");
            //    UpdateDetailObjects(id_businessPartner, result.businessPartners, arrayFieldStr);

            //    UpdateBusinessOportunityAmount();
            //}
        },
        complete: function () {
            //hideLoading();
        }
    });
}

function QualityControlDetailView_OnBeginCallback(s, e) {
    e.customArgs["id_qualityControl"] = gvQualityControlDetails.cpIdQualityControl;

}

var id_sizeIniAux = 0;

function Size_OnInit(s, e) {
    id_sizeIniAux = s.GetValue();
}

function ProcessType_SelectedIndexChanged(s, e) {
    id_size.PerformCallback();
}

function Size_OnBeginCallback(s, e) {
    e.customArgs["id_processType"] = id_processType.GetValue();
    e.customArgs["id_size"] = s.GetValue();

}

function Size_OnEndCallback(s, e) {
    if (id_sizeIniAux !== 0 && id_sizeIniAux !== null) {
        s.SetValue(id_sizeIniAux);
    }
}

//QUALITY CONTROL DETAILS
function OnBatchEditRowValidating(s, e) {
    // 
    let indexRow = e.visibleIndex;
    var cellInfo2 = e.validationInfo[2];
    if (s.cpCJATXT === false) {
        //var grid = ASPxClientGridView.Cast(s);
        //var cellInfo1 = e.validationInfo[grid.GetColumnByField("id_qualityAnalysisName").index];
        //var years = CheckYears(cellInfo1.value, cellInfo2.value);
        //if (cellInfo1.value === "FRESCO Y SANO" || cellInfo1.value === "BASURA GR") {
        var exists = false;
        if (s.cpQualityAnalysisMayor0 !== null) {
            for (var i = 0; i < s.cpQualityAnalysisMayor0.length; i++) {
                if (s.cpQualityAnalysisMayor0[i].toString() === s.GetItemKey(e.visibleIndex)) {
                    exists = true;
                    break;
                }
            }
        }


        if (exists) {
            if (cellInfo2.value > 0) {
                cellInfo2.isValid = true;
            } else {
                cellInfo2.isValid = false;
                cellInfo2.errorText = "El número debe ser mayor que 0";
            }
        } else {


            let maxlength = 100;
            if (s.cpValidaciones != null && s.cpValidaciones.length > 0 && s.keys.length > 0) {
                let qualityControlDetailId = s.keys[indexRow];

                maxlengthConfig = s.cpValidaciones.find(r => r.id_qualityControlDetail == qualityControlDetailId);
                if (maxlengthConfig != null) {
                    maxlength = maxlengthConfig.valueValidateMax;
                }
            }

            if (cellInfo2.value >= 0 && cellInfo2.value <= maxlength) {
                cellInfo2.isValid = true;
            } else {
                cellInfo2.isValid = false;
                cellInfo2.errorText = "El número debe estar en el rango de 0..." + maxlength;
            }
        }
    } else {
        cellInfo2.isValid = true;
    }

    //if (s.cpPieza === true) {
    //    if (s.cpValueSettingGRAMCLASI === true) {
    //        var totalUnit = 0.00;
    //        var rowUpdate = [];
    //        for (let rowDetail = 0; rowDetail < s.pageRowCount; rowDetail++) {
    //            if (e.visibleIndex === rowDetail) {
    //                if (e.rowValues[2].value === null) continue;
    //                totalUnit += e.rowValues[2].value;
    //                rowUpdate.push(rowDetail);
    //            } else {
    //                if (s.batchEditApi.GetCellValue(rowDetail, 2) === null || s.batchEditApi.GetCellValue(rowDetail, 2) === "") continue;
    //                if (s.batchEditApi.GetCellValue(rowDetail, 2) !== null && s.batchEditApi.GetCellValue(rowDetail, 2) !== "") {
    //                    totalUnit += parseFloat(s.batchEditApi.GetCellValue(rowDetail, 2));
    //                    rowUpdate.push(rowDetail);
    //                }
    //            }
    //        }
    //        //Actualizar totalUnit
    //        $('#totalUnit').val(totalUnit);
    //        //Actualizar Porciento de Resultado
    //        var resultAux = 0.00;
    //        for (var i = 0; i < rowUpdate.length; i++) {
    //            if (e.visibleIndex === rowUpdate[i]) {
    //                resultAux = (Math.trunc(e.rowValues[2].value / totalUnit * 10000) / 100).toFixed(2);
    //                s.batchEditApi.SetCellValue(rowUpdate[i], "result", resultAux, resultAux, true);
    //            } else {
    //                resultAux = (Math.trunc(parseFloat(s.batchEditApi.GetCellValue(rowUpdate[i], 2)) / totalUnit * 10000) / 100).toFixed(2);
    //                s.batchEditApi.SetCellValue(rowUpdate[i], "result", resultAux, resultAux, true);
    //            }

    //        }
    //        //Actualizar Gramage
    //        OnValueChangedTotalWeightSample(totalWeightSample, totalWeightSample);
    //        //var totalWeightSampleAux = totalWeightSample.GetValue();
    //        //if (totalWeightSampleAux !== null && totalUnit > 0) {
    //        //    grammageReference.SetValue((totalWeightSampleAux / totalUnit).toFixed(2));
    //        //} else {
    //        //    grammageReference.SetValue(0.00);
    //        //}
    //    }
    //}
}

function onUpdateTotal(s, e) {
    let sum = 0.00;
    let porcentaje = 0;
    var valorEditado = s.cpValorTotalEditado;
    let arreglo1 = [];
    let arreglo2 = [];
    if (s.name === "1gvQualityControlDetailsDetail2") {
        if (valorEditado > 0) {
            let sumaTotal = 0.00;
            let aux1 = 0.00;
            // Arreglo de los valores nuevos
            for (var i in e.updatedValues) {
                let sumaAux1 = e.updatedValues[i][2];
                if (sumaAux1 !== undefined && sumaAux1 !== "" && sumaAux1 !== null) {
                    aux1 = parseFloat(sumaAux1);
                    let arr1 = [parseInt(i), aux1];
                    arreglo1.push(arr1);
                }
                else if (sumaAux1 === 0) {
                    let arr2 = [parseInt(i), 0.00];
                    arreglo1.push(arr2);
                }
                else {
                    continue;
                }
            }
            // Arreglo de los valores antiguos para actualizar
            var indice = 0;
            for (var j in s.batchEditPageValues) {
                let sumaAux = s.batchEditPageValues[j][2];
                if (sumaAux === null) continue;
                if (sumaAux !== undefined && sumaAux !== "") {
                    let aux2 = parseFloat(sumaAux.replace(",", "."));
                    let arr3 = [indice, aux2];
                    arreglo2.push(arr3);
                }
                else {
                    let arr4 = [indice, 0.00];
                    arreglo2.push(arr4);
                }
                indice++;
            }

            // Recorre el arreglo 1 para cambiar el valor por el arreglo 2
            for (var i in arreglo1) {
                var indiceAux = arreglo1[i][0];
                var valorNuevo = arreglo1[i][1];
                arreglo2[indiceAux][1] = valorNuevo;
            }

            // Recorro el arreglo 2para sumar
            for (var i in arreglo2) {
                sumaTotal += arreglo2[i][1];
            }

            // Asigno el total
            totalWeightSampleNonEditable.SetValue(sumaTotal);
        }
        else {
            totalWeightSampleNonEditable.SetValue(0);
            for (let rowDetail = 0; rowDetail < s.pageRowCount; rowDetail++) {
                if (e.visibleIndex === rowDetail) {
                    if (e.rowValues[2].value === null) continue;
                    sum += e.rowValues[2].value;
                } else {
                    if (s.batchEditApi.GetCellValue(rowDetail, 2) === null || s.batchEditApi.GetCellValue(rowDetail, 2) === "") continue;
                    if (s.batchEditApi.GetCellValue(rowDetail, 2) !== null && s.batchEditApi.GetCellValue(rowDetail, 2) !== "") {
                        sum += parseFloat(s.batchEditApi.GetCellValue(rowDetail, 2));
                    }
                }
            }
            // Asigna la sumatoria al SpinEdit
            if (sum !== null && sum > 0) {
                totalWeightSampleNonEditable.SetValue(sum);
            }
        }
    }
}

function OnBatchEditEndEditing(s, e) {
    if (s.cpPieza === true) {
        if (s.cpValueSettingGRAMCLASI === true) {
            var totalUnit = 0.00;
            var rowUpdate = [];
            for (let rowDetail = 0; rowDetail < s.pageRowCount; rowDetail++) {
                if (e.visibleIndex === rowDetail) {
                    if (e.rowValues[2].value === null) continue;
                    totalUnit += e.rowValues[2].value;
                    rowUpdate.push(rowDetail);
                } else {
                    if (s.batchEditApi.GetCellValue(rowDetail, 2) === null || s.batchEditApi.GetCellValue(rowDetail, 2) === "") continue;
                    if (s.batchEditApi.GetCellValue(rowDetail, 2) !== null && s.batchEditApi.GetCellValue(rowDetail, 2) !== "") {
                        totalUnit += parseFloat(s.batchEditApi.GetCellValue(rowDetail, 2));
                        rowUpdate.push(rowDetail);
                    }
                }
            }
            //Actualizar totalUnit
            $('#totalUnit').val(totalUnit);
            //Actualizar Porciento de Resultado
            var resultAux = 0.00;
            for (var i = 0; i < rowUpdate.length; i++) {
                if (e.visibleIndex === rowUpdate[i]) {
                    resultAux = (Math.round(e.rowValues[2].value / totalUnit * 10000) / 100).toFixed(2);
                    s.batchEditApi.SetCellValue(rowUpdate[i], "result", resultAux, resultAux, true);
                } else {
                    resultAux = (Math.round(parseFloat(s.batchEditApi.GetCellValue(rowUpdate[i], 2)) / totalUnit * 10000) / 100).toFixed(2);
                    s.batchEditApi.SetCellValue(rowUpdate[i], "result", resultAux, resultAux, true);
                }

            }
            //Actualizar Gramage
            OnValueChangedTotalWeightSample(totalWeightSample, totalWeightSample);
            //var totalWeightSampleAux = totalWeightSample.GetValue();
            //if (totalWeightSampleAux !== null && totalUnit > 0) {
            //    grammageReference.SetValue((totalWeightSampleAux / totalUnit).toFixed(2));
            //} else {
            //    grammageReference.SetValue(0.00);
            //}
        }
    }
}

function OnValueChangedTotalWeightSample(s, e) {
    var totalWeightSampleAux = totalWeightSample.GetValue();
    var totalUnit = $('#totalUnit').val() !== null && $('#totalUnit').val() !== "" ? parseFloat($('#totalUnit').val()) : 0.00;
    if (totalWeightSampleAux !== null && totalUnit > 0) {
        grammageReference.SetValue(Math.trunc(totalWeightSampleAux / totalUnit * 100) / 100)/*.toFixed(2)*/;
    } else {
        grammageReference.SetValue(0.00);
    }
}

// DETAILS ACTIONS QUALITY CONTROL DETAILS

var id_itemIniAux = 0;
var id_personIniAux = 0;
var id_documentIniAux = 0;

function AddNewQualityControlDetail(s, e) {
    gvQualityControlDetails.AddNewRow();
    //AddNew(s, e);
}

function RemoveQualityControlDetail(s, e) {
    //Remove(s, e);
}

function RefreshQualityControlDetail(s, e) {
    gvQualityControlMaster.UnselectRows();
    gvQualityControlMaster.PerformCallback();
}


function WarehouseCombo_SelectedIndexChanged(s, e) {

    id_warehouseLocation.SetValue(null);
    id_warehouseLocation.ClearItems();

    id_item.SetValue(null);
    id_item.ClearItems();

    id_lot.SetValue(null);
    id_lot.ClearItems();

    var data = s.GetValue();
    if (data === null) {
        return;
    }

    if (data != null) {

        $.ajax({
            url: "QualityControl/WarehouseChangeData",
            type: "post",
            data: { id_warehouse: data },
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
                    UpdateDetailObjects(id_warehouseLocation, result.warehouseLocations, arrayFieldStr);
                    //id_warehouseLocation.SetValue(result.id_warehouseLocation);

                    //gridViewMoveDetails.UnselectRows();
                    //gridViewMoveDetails.PerformCallback();
                    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("metricUnit").SetText(result.metricUnit);
                    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouse").SetValue(result.id_warehouse);
                    //gvProductionLotReceptionEditFormItemsDetail.GetEditor("id_warehouseLocation").SetValue(result.id_warehouseLocation);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function WarehouseLocationCombo_SelectedIndexChanged(s, e) {

    id_item.SetValue(null);
    id_item.ClearItems();

    id_lot.SetValue(null);
    id_lot.ClearItems();

    var data = s.GetValue();
    if (data === null) {
        return;
    }

    if (data != null) {

        $.ajax({
            url: "QualityControl/WarehouseLocationChangeData",
            type: "post",
            data: {
                id_warehouse: id_warehouse.GetValue(),
                id_warehouseLocation: data
            },
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
                    UpdateDetailObjects(id_item, result.items, arrayFieldStr);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}

function ItemCombo_SelectedIndexChanged(s, e) {

    id_lot.SetValue(null);
    id_lot.ClearItems();

    var data = s.GetValue();
    if (data === null) {
        return;
    }

    if (data != null) {

        $.ajax({
            url: "QualityControl/ItemChangeData",
            type: "post",
            data: {
                id_warehouse: id_warehouse.GetValue(),
                id_warehouseLocation: id_warehouseLocation.GetValue(),
                id_item: data,
            },
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
                    arrayFieldStr.push("number");
                    UpdateDetailObjects(id_lot, result.lots, arrayFieldStr);
                }
            },
            complete: function () {
                //hideLoading();
            }
        });
    }
}


function ButtonUpdateQualityControlDetail_Click(s, e) {

    var id_gvQualityControlDetails = $("#id_gvQualityControlDetails").val();
    switch (String(id_gvQualityControlDetails)) {
        case "gvQualityControlDetailsDetail1":
            gvQualityControlDetailsDetail1.UpdateEdit();
            break;
        case "gvQualityControlDetailsDetail2":
            gvQualityControlDetailsDetail2.UpdateEdit();
            break;
        case "gvQualityControlDetailsDetail3":
            gvQualityControlDetailsDetail3.UpdateEdit();
            break;
        case "gvQualityControlDetailsDetail4":
            gvQualityControlDetailsDetail4.UpdateEdit();
            break;
        case "gvQualityControlDetailsDetail5":
            gvQualityControlDetailsDetail5.UpdateEdit();
            break;
        case "gvQualityControlDetailsDetail6":
            gvQualityControlDetailsDetail6.UpdateEdit();
            break;
        case "gvQualityControlDetailsDetail7":
            gvQualityControlDetailsDetail7.UpdateEdit();
            break;
        default:
    }


}

function BtnCancelQualityControlDetail_Click(s, e) {

    var id_gvQualityControlDetails = $("#id_gvQualityControlDetails").val();

    switch (String(id_gvQualityControlDetails)) {
        case "gvQualityControlDetailsDetail1":
            gvQualityControlDetailsDetail1.CancelEdit();
            break;
        case "gvQualityControlDetailsDetail2":
            gvQualityControlDetailsDetail2.CancelEdit();
            break;
        case "gvQualityControlDetailsDetail3":
            gvQualityControlDetailsDetail3.CancelEdit();
            break;
        case "gvQualityControlDetailsDetail4":
            gvQualityControlDetailsDetail4.CancelEdit();
            break;
        case "gvQualityControlDetailsDetail5":
            gvQualityControlDetailsDetail5.CancelEdit();
            break;
        case "gvQualityControlDetailsDetail6":
            gvQualityControlDetailsDetail6.CancelEdit();
            break;
        case "gvQualityControlDetailsDetail7":
            gvQualityControlDetailsDetail7.CancelEdit();
            break;
        default:
    }
}

function qualityControlDateOnInit(s, e) {
    var eTmp = document.getElementById("qualityControlDate").value;
    if (eTmp == null || eTmp == undefined || eTmp == "") {
        s.SetValue(new Date());
    }
}

function monerDateLabelOnInit(s, e) {
    var eTmp = document.getElementById("monerDate").value;
    if (eTmp == null || eTmp == undefined || eTmp == "") {
        s.SetValue(new Date());
    }
}

// Inicio de ShrimpSupplierTraceability

function SowingDate_Validation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        // 
        var aReceptionDateAux = $("#aReceptionDate").val();
        if (aReceptionDateAux !== null && aReceptionDateAux !== "") {
            var dateReceptionDateAux = new Date(aReceptionDateAux);
            if (dateReceptionDateAux < s.GetValue()) {
                e.isValid = false;
                e.errorText = "Fecha de Siembra, debe ser menor o igual a la Fecha de Recepción del Lote("
                    + dateReceptionDateAux.getDate().toString().padStart(2, 0) + "/"
                    + (dateReceptionDateAux.getMonth() + 1).toString().padStart(2, 0) + "/"
                    + dateReceptionDateAux.getFullYear().toString() + ")";
            }
        }
    }
}

function HarvestDate_Validation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        // 
        var aReceptionDateAux = $("#aReceptionDate").val();
        if (aReceptionDateAux != null && aReceptionDateAux != "") {
            var dateReceptionDateAux = new Date(aReceptionDateAux);
            if (dateReceptionDateAux < s.GetValue()) {
                e.isValid = false;
                e.errorText = "Fecha de Cosecha, debe ser menor o igual a la Fecha de Recepción del Lote("
                    + dateReceptionDateAux.getDate().toString().padStart(2, 0) + "/"
                    + (dateReceptionDateAux.getMonth() + 1).toString().padStart(2, 0) + "/"
                    + dateReceptionDateAux.getFullYear().toString() + ")";
            }
        }
    }
}

function ShrimpSupplierTraceability_Validation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}

// Fin de ShrimpSupplierTraceability

//MASTER DETAIL PROPERTIES
function OnRowCollapsing(s, e) {

    var detailRowId = '#gvQualityControlMaster' + e.visibleIndex;
    $(detailRowId).css('display', 'none');
    e.cancel = true;
}

// UPDATE VIEWx

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 2000);
    }
}

function UpdateView() {
    var id = parseInt($("#id_qualityControl").val());
    var codeState = $("#codeState").val();
    var isConformsSTR = $("#isConformsSTR").val();

    let hasUpdateTmp = $("#hasupdate").val();

    if (hasUpdateTmp === "Y") {
        // EDITING BUTTONS
        if (btnNew != undefined) {
            btnNew.SetEnabled(true);
        }
        if (btnSave != undefined) {
            btnSave.SetEnabled(false);
        }
        if (btnCopy != undefined) {
            btnCopy.SetEnabled(false);
        }

        $.ajax({
            url: "QualityControl/Actions",
            type: "post",
            data: { id: id },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
                //showLoading();
            },
            success: function (result) {
                if (btnApprove != undefined) {
                    btnApprove.SetEnabled(result.btnApprove);
                }
                if (btnAutorize != undefined) {
                    btnAutorize.SetEnabled(result.btnAutorize);
                }
                if (btnProtect != undefined) {
                    btnProtect.SetEnabled(result.btnProtect);
                }
                if (btnCancel != undefined) {
                    btnCancel.SetEnabled(result.btnCancel);
                }
                if (btnRevert != undefined) {
                    btnRevert.SetEnabled(result.btnRevert);
                }



            },
            complete: function (result) {
                //hideLoading();
            }
        });

        // HISTORY BUTTON
        //btnHistory.SetEnabled(id !== 0);

        // PRINT BUTTON
        if (btnPrint != undefined) {
            btnPrint.SetEnabled(id !== 0);
        }



    }

    //btnSave.SetEnabled(codeState == "01" || (isConformsSTR == "False" && codeState != "05"))//01: PENDIENTE y "05": ANULADO
    //btnCopy.SetEnabled(id !== 0);

    // STATES BUTTONS

    var isBtnToReturn = $("#isBtnToReturn").val();
    if (isBtnToReturn == "True" || isBtnToReturn == "true" || isBtnToReturn == true) {
        //$("#pagination").show();
        $('.pagination').hide();
    } else {
        //$("#pagination").hide();
        $('.pagination').show();
    }

}
function OnSizeListDetailClick(s, e) {
    var _totalWeightSample = totalWeightSample.GetValue();
    var _QuantityPoundsReceived = QuantityPoundsReceived.GetValue();
    var _idProcessType = id_processType.GetValue();
    var _totalUnit = $('#totalUnit').val() !== null && $('#totalUnit').val() !== "" ? parseFloat($('#totalUnit').val()) : 0.00;

    var idQualityControl_Tmp = $("#id_qualityControl").val();
    var data = {
        totalWeightSample: _totalWeightSample,
        QuantityPoundsReceived: _QuantityPoundsReceived,
        idQualityControl: idQualityControl_Tmp,
        idProcessType: _idProcessType,
        totalUnit: _totalUnit
    };

    if (_totalWeightSample > 0) {
        showThickBox("QualityControl/PopupControlTallaListDetail", data);
    }
}

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "QualityControl/InitializePagination",
        type: "post",
        data: { id_qualityControl: $("#id_qualityControl").val() },
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            $("#pagination").attr("data-max-page", result.maximunPages);
            current_page = result.currentPage;
        },
        complete: function () {
        }
    });
    $('.pagination').current_page = current_page;
}

// MAIN FUNCTIONS

function init() {
    UpdatePagination();
    AutoCloseAlert();
}

$(function () {

    var chkReadyState = setInterval(function () {
        if (document.readyState === "complete") {
            clearInterval(chkReadyState);
            UpdateView();
        }
    }, 100);

    init();
});
function OnBtnCancelPopupTallasList_Click(s, e) {
    $.fancybox.close();
}
