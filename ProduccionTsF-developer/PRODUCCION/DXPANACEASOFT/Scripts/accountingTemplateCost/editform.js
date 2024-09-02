
var errorMessage = "";

var id_codeProdIniAux = null;

/// VALIDATION DETAIL
function OnCodeAccountValidation(s, e) {
    errorMessage = "";

    $("#GridMessageErrorsDetail").hide();
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Código de cuenta es obligatorio.";
        }
    } else {
        var data = {
            id_accountingTemplate: $("#id_accountingTemplate").val(),
            cciAccount: s.GetValue(),
            typeAux: typeAuxiliar.GetValue(),
            idAux: id_auxiliary.GetValue(),            
            codeCenter: nameCenterCost.GetValue(),
            codeSubCenter: nameSubCenterCost.GetValue(),
            isNew: gvAccountingTemplateCostDetail.cpIsNewRowEdit,
            idIndex: gvAccountingTemplateCostDetail.cpEditingRowKey
        };
        $.ajax({
            url: "AccountingTemplateCost/ValidateAccountTemplateSelected",
            type: "post",
            data: data,
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
                    if (!result.isValid) {
                        e.isValid = false;
                        e.errorText = result.error;
                        if (errorMessage == null || errorMessage == "") {
                            errorMessage = "- Código: " + result.Error;
                        }
                    }
                }
            },
            complete: function () {
                hideLoading();
            }
        });
        /*
         int? id_accountingTemplate, string cciAccount, string typeAux
            , string idAux
            , string codeCenter
            , string codeSubCenter, bool isNew, int idIndex
            */
    }
}

function OnCodeDetailValidation(s, e) {
    errorMessage = "";

    $("#GridMessageErrorsDetail").hide();
    if (s.GetValue() === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        if (errorMessage == null || errorMessage == "") {
            errorMessage = "- Código de cuenta es obligatorio.";
        }
    } else {
        var data = {
            id_codeNew: s.GetValue(),
            codeAux: id_auxiliary.GetValue(),
            typeAux: typeAuxiliar.GetValue(),
            codeCenter: nameCenterCost.GetValue(),
            codeSubCenter: nameSubCenterCost.GetValue()
        };

        if (data.id_codeNew != id_codeProdIniAux) {
            $.ajax({
                url: "AccountingTemplateCost/ItsRepeatedDetail",
                type: "post",
                data: data,
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
                        if (result.itsRepeated == 1) {
                            e.isValid = false;
                            e.errorText = result.Error;
                            if (errorMessage == null || errorMessage == "") {
                                errorMessage = "- Código: " + result.Error;
                            }
                        }
                    }
                },
                complete: function () {
                    //hideLoading();
                }
            });
        }
	}
}

function onCodeAccountLedgerInit(s, e) {
    id_codeProdIniAux = code.GetValue();
}

////COMBOS

function ComboCodeAccounLedger_SelectedIndexChanged(s, e) {

    typeCount.SetText("");
    description.SetText("");
    typeAuxiliar.SetValue(null);
    
    var data = code.GetValue();
    if (data !== null && data !== undefined) {

        $.ajax({
            url: "AccountingTemplateCost/TypeDescriptionChangeData",
            type: "post",
            data: { idCode: data },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
            },
            success: function (result) {
                if (result !== null && result !== undefined) {
                    typeCount.SetValue(result.tipoCuenta);
                    description.SetValue(result.descripcion);
                    
                    //Consultar si acepta Auxiliar
                    if (result.aceptAux) {
                        id_auxiliary.SetEnabled(true);
                        typeAuxiliar.PerformCallback();
                        initComboAuxiliarAccount();
                    }
                    else {
                        typeAuxiliar.SetEnabled(false);
                        id_auxiliary.SetEnabled(false);
                        id_auxiliary.SetValue(null);
                    }

                    // Consultar si Acepta Centro de Costo
                    if (result.aceptaProy) {
                        nameCenterCost.PerformCallback();
                        nameCenterCost.SetEnabled(true);
                        nameSubCenterCost.SetEnabled(true);
                        //initComboCentroCostoAccount();
                    }
                    else {
                        nameCenterCost.SetEnabled(false);
                        nameSubCenterCost.SetEnabled(false);
                        nameCenterCost.SetValue(null);
                        nameSubCenterCost.SetValue(null);
                    }
                }
            },
            complete: function () {

            }
        });
    }
}

function initComboAuxiliarAccount() {
    var data = code.GetValue();
    $.ajax({
        url: "AccountingTemplateCost/LoadNameAuxiliarAccountingTemplate",
        type: "post",
        data: { codeCuenta: data },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            for (var i = 0; i < result.length; i++) {
                var comboAux = result[i];
                id_auxiliary.AddItem(comboAux.descAuxiliar, comboAux.codeAuxiliar);
            }
        },
        complete: function () {
        }
    });
}

function initComboCentroCostoAccount() {

    $.ajax({
        url: "AccountingTemplateCost/LoadCenterCostAccountingTemplate",
        type: "post",
        data: {},
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (resulCC) {
            for (var i = 0; i < resulCC.length; i++) {
                var combocc = resulCC[i];
                nameCenterCost.AddItem(combocc.id, combocc.name);

                //nameCenterCost.AddItem(combocc.name, combocc.id);
            }
        },
        complete: function () {
        }
    });
}

function NameSubCenterCost_BeginCallback(s, e) {
    e.customArgs["nameCenterCost"] = nameCenterCost.GetValue();
}

function NameSubCenterCost_EndCallback(s, e) {
}
function CenterCostLedge_BeginCallback(s, e) {
}
function CenterCostLedge_EndCallback(s, e) {
}

function ComboAuxiliarLedger_SelectedIndexChanged(s, e) {

    codeAuxiliar.SetText("");
    var data = id_auxiliary.GetValue();
    if (data !== null && data !== undefined) {

        $.ajax({
            url: "AccountingTemplateCost/CodeAuxiliarChangeData",
            type: "post",
            data: { nameAuxiliar: data },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
            },
            success: function (result) {
                codeAuxiliar.SetValue(result.codeAux);
            },
            complete: function () {

            }
        });
    }
}

function ComboCenterCostLedger_SelectedIndexChanged(s, e) {
    nameSubCenterCost.PerformCallback();
    //codeCenterCost.SetText("");
    //var data = nameCenterCost.GetValue();
    //if (data !== null && data !== undefined) {

    //    $.ajax({
    //        url: "AccountingTemplateCost/CodeCenterCostChangeData",
    //        type: "post",
    //        data: { nameCenter: data },
    //        async: true,
    //        cache: false,
    //        error: function (error) {
    //            console.log(error);
    //        },
    //        beforeSend: function () {
    //        },
    //        success: function (result) {
    //            codeCenterCost.SetValue(result.codeCenterCost);
    //            //initComboSubCentroCostoAccount();
    //        },
    //        complete: function () {

    //        }
    //    });
    //}
}

function initComboSubCentroCostoAccount() {

    nameSubCenterCost.SetValue(null);
    nameSubCenterCost.ClearItems();
    codeSubCenterCost.SetText("");

    var data = codeCenterCost.GetValue();
    $.ajax({
        url: "AccountingTemplateCost/LoadSubCenterCostAccountingTemplate",
        type: "post",
        data: { codeCenter: data },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (resultSub) {
            for (var i = 0; i < resultSub.length; i++) {
                var combosc = resultSub[i];
                nameSubCenterCost.AddItem(combosc.id, combosc.name);
            }
        },
        complete: function () {
        }
    });
}

function ComboSubCenterCostLedger_SelectedIndexChanged(s, e) {

    codeSubCenterCost.SetText("");
    var data = nameSubCenterCost.GetValue();
    if (data !== null && data !== undefined) {

        $.ajax({
            url: "AccountingTemplateCost/CodeSubCenterCostChangeData",
            type: "post",
            data: { nameSubCenter: data },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {
            },
            success: function (result) {
                codeSubCenterCost.SetValue(result.codeSubCenterCost);
            },
            complete: function () {

            }
        });
    }
}


function CostProductionCombo_SelectedIndexChanged(s, e) {

    id_expenseProduction.SetValue(null);
    id_expenseProduction.ClearItems();

    var data = s.GetValue();
    if (data === null) {
        return;
    }

    if (data !== null) {

        $.ajax({
            url: "AccountingTemplateCost/CostProductionChangeData",
            type: "post",
            data: { id_costPoduction: data },
            async: true,
            cache: false,
            error: function (error) {
                console.log(error);
            },
            beforeSend: function () {

            },
            success: function (result) {
                for (var i = 0; i < result.length; i++) {
                    var accountingTemplate = result[i];
                    id_expenseProduction.AddItem(accountingTemplate.name, accountingTemplate.id);
                }
            },
            complete: function () {

            }
        });
    }
}


function ButtonUpdate_Click(s, e) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null);
    
    if (gvAccountingTemplateCostDetail.GetVisibleRowsOnPage() == undefined) valid = false;
    if (gvAccountingTemplateCostDetail.GetVisibleRowsOnPage() == 0)
    {
        valid = false;
        $("#_errormsgTT").text("Debe adicionar detalles en la plantilla contable.").show(100).delay(2000).hide(200);
    } 
    if (valid) {
        gvAccountingTemplateCost.UpdateEdit();
    }
}

function ButtonCancel_Click(s, e) {
    if (gvAccountingTemplateCost !== null && gvAccountingTemplateCost !== undefined)
    {
        gvAccountingTemplateCost.CancelEdit();
        $("#_errormsgTT").hide();
    }
}
 