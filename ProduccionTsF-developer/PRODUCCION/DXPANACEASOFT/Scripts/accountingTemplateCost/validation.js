function OnCodeAccountingTemplateValidation(s, e) {
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    } else {
        if (e.value.length > 21) {
            e.isValid = false;
            e.errorText = "Máximo 20 caracteres";
        } else {
            $.ajax({
                url: "AccountingTemplateCost/ValidateCodeAccountingTemplate",
                type: "post",
                async: false,
                cache: false, data: {
                    id_accountingTemplate: gvAccountingTemplateCost.cpEditingRowKey,
                    code: e.value
                },
                error: function (error) {
                    console.log(error);
                },
                beforeSend: function () {
                    //showLoading();
                },
                success: function (result) {
                    e.isValid = result.isValid;
                    e.errorText = result.errorText;
                },
                complete: function () {
                    //hideLoading();
                }
            });
        }
    }
}

var errorMessage = "";

var id_costProdIniAux = null;
var id_costExpenseAux = null;
var id_codeProcessPlantAux = null;

function OnProducionCostValidation(s, e) {
    // 
    if (e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
    else {
        errorMessage = "";

        $("#GridMessageErrorsDetail").hide();
        var data = {
            id_prodCost: s.GetValue(),
            id_prodExp: id_expenseProduction.GetValue(),
            id_proPlant: id_processPlant.GetValue()
        };
        if (data.id_prodCost != id_costProdIniAux || data.id_prodExp != id_costExpenseAux || data.id_proPlant != id_codeProcessPlantAux) {
            $.ajax({
                url: "AccountingTemplateCost/ItsRepeatedCabecera",
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

function onCodeAccountingTemplateInit(s, e) {
    id_costProdIniAux = id_costProduction.GetValue();
}

function onCodeProductionExpenseAccountingTemplateInit(s, e) {
    id_costExpenseAux = id_expenseProduction.GetValue();
}

function onCodeProcessPlantAccountingTemplateInit(s, e) {
    id_codeProcessPlantAux = id_processPlant.GetValue();
}

function OnAccountingTemplateProductionExpenseValidation(s, e) {
    var value = id_costProduction.GetValue();
    if (value !== null && e.value === null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
    }
}