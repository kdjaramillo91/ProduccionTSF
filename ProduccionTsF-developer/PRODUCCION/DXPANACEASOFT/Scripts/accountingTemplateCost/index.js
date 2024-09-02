// BUTTONS ACTIONS

function AddNewItem(s, e) {
   
    gvAccountingTemplateCost.AddNewRow();
}

function RefreshGrid(s, e) {
    gvAccountingTemplateCost.Refresh();
}

var keyToCopy = null;
function CopyItems(s, e) {
    gvAccountingTemplateCost.GetSelectedFieldValues("id", function (values) {
        if (values.length === 1) {
            keyToCopy = values[0];
            gvAccountingTemplateCost.AddNewRow();
            keyToCopy = 0;
        }
    });
}

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();
    ShowEditMessage(s, e);
}


function OnGridViewBeginCallback(s, e) {
    e.customArgs["keyToCopy"] = keyToCopy;
}

 
function UpdateTitlePanel() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvAccountingTemplateCost.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvAccountingTemplateCost.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvAccountingTemplateCost.GetSelectedRowCount() > 0 && gvAccountingTemplateCost.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvAccountingTemplateCost.GetSelectedRowCount() > 0);
}

function GetSelectedFilteredRowCount() {
    return gvAccountingTemplateCost.cpFilteredRowCountWithoutPage + gvAccountingTemplateCost.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function SelectAllRows() {
    gvAccountingTemplateCost.SelectRows();
}

function UnselectAllRows() {
    gvAccountingTemplateCost.UnselectRows();
}


function AccountingTemplateCostDetail_OnBeginCallback(s, e) {

    var objaccountingTemplateCostType = ASPxClientControl.GetControlCollection().GetByName("id_accountingTemplate");
    e.customArgs['id_accountingTemplate'] = $("#id_accountingTemplate").val();
    e.customArgs['id_accountingTemplateType'] = (objaccountingTemplateCostType != undefined) ? ((objaccountingTemplateCostType != null) ? objaccountingTemplateCostType.GetValue():0 ):0 ;
}

function GridViewAccountingTemplateCostCustomCommandButton_Click(s, e) {

    if (e.buttonID === "btnDeleteRow") {
        showConfirmationDialog(function () {
            s.DeleteRow(e.visibleIndex);
            s.UnselectRows();
        });
    }
}

function OnNameAuxiliarInit(s, e) {

    //if (nameAuxiliar.GetValue() == null) {
    //    nameAuxiliar.SetEnabled(false);
    //} else {
    //    nameAuxiliar.SetEnabled(true);
    //    //initComboAuxiliarAccount2();
    //}
}
function OnTypeAuxiliar_BeginCallback(s, e) {
    e.customArgs['typeAuxiliar'] = s.GetValue();
    e.customArgs['id_cuenta'] = code.GetValue();
}
function OnTypeAuxiliar_EndCallback(s, e) {
}
function OnTypeAuxiliarInit(s, e) {

}

function ComboTypeAuxiliarLedger_SelectedIndexChanged(s, e) {
    id_auxiliary.PerformCallback();
}

function OnNameAuxiliar_BeginCallback(s, e) {
    e.customArgs['id_auxiliar'] = s.GetValue();
    e.customArgs['type_auxiliar'] = typeAuxiliar.GetValue(); 
}
function OnNameAuxiliar_EndCallback(s, e) {
    
}

function initComboAuxiliarAccount2() {

    var data = code.GetValue();
    var codeAux = id_auxiliary.GetValue();

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
                if (codeAux != comboAux.codeAuxiliar) {
                    id_auxiliary.AddItem(comboAux.descAuxiliar, comboAux.codeAuxiliar);
				}
            }
        },
        complete: function () {
        }
    });
}

function OnNameCenterCostInit(s, e) {

    if (nameCenterCost.GetValue() == null) {
        nameCenterCost.SetEnabled(false);
    } else {
        nameCenterCost.SetEnabled(true);
        initComboCenterCost();
	}

}

function initComboCenterCost() {

    var codeCenter = nameCenterCost.GetValue();

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
                if (codeCenter != combocc.id) {
                    nameCenterCost.AddItem(combocc.name, combocc.id);
				}
                
            }
        },
        complete: function () {
        }
    });
}

function OnNameSubCenterCostInit(s, e) {

    if (nameSubCenterCost.GetValue() == null) {
        nameSubCenterCost.SetEnabled(false);
    } else {
        nameSubCenterCost.SetEnabled(true);
        initComboSubCenterCost();
    }
}

function initComboSubCenterCost() {

    var codeSubCenter = nameSubCenterCost.GetValue();
    var data = nameCenterCost.GetValue();

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
                if (codeSubCenter != combosc.id) {
                    nameSubCenterCost.AddItem(combosc.name, combosc.id);
				}
            }
        },
        complete: function () {
        }
    });
}

function OnComboCodeAccounLedgerInit(s, e) {

    $.ajax({
        url: "AccountingTemplateCost/LoadCodeComboAccountingTemplate",
        type: "post",
        data: {},
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
                code.AddItem(comboAux.CCiCuenta, comboAux.CCtTituloDetalle, comboAux.CDsCuenta);
            }
        },
        complete: function () {
        }
    });
}

function initComboCodeAccount(datosIniciales) {
    for (var i = 0; i < datosIniciales.length; i++) {
        code.AddItem(datosIniciales[i]);
    }
}
// MAIN FUNCTIONS

function init() {
   
}

$(function () {
    init();
});