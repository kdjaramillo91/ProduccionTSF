//Print
function Print(s, e) {    
    var id = $("#id_RequestInventoryMove").val();
    $.ajax({
        url: 'RequestInventoryMove/PrintRequerimentMove',
        data: { idRequestInventoryMove: id},
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

function PrintDetailGD(s, e) {
    // 
    var nombre = s.name;
    if (nombre != undefined) {
        var nomArr = nombre.split("_");
        if (nomArr.length == 2) {
            $.ajax({
                url: 'RequestInventoryMove/PrintInventoryMoveGenerated',
                data: { idInvMov: nomArr[1] },
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
}

//Item Detail
var indexDetail = 0;
function OnItemDetailBeginCallback(s, e) {
    // 
    if (s != undefined) {
        if (s.name.startsWith('ItemDetail')) {
            var index = parseInt(s.name.substr("ItemDetail".length));
            e.customArgs["indice"] = index;
            indexDetail = index;
        }
    }
}

function OnItemDetailEndCallback(s, e) {
}

//Details
var customCommand = "";

function OnGridViewInitDetails(s, e) {
    UpdateTitlePanelDetails();
}

function OnGridViewBeginCallbackDetails(s, e) {
    // 
    if (e.command == "UPDATEROW" || e.command == "UPDATEEDIT") {
        if (indexDetail > 0) {
            if (window["ItemDetail"+indexDetail] != undefined) {
                e.customArgs["id_item"] = window["ItemDetail" + indexDetail].GetValue();
            }
        } else if (indexDetail == 0) {
            var _index = gvRequestInventoryMoveDetails.cpRowIndex;
            var key = _index >= 0 ? gvRequestInventoryMoveDetails.cpRowKey : 0;
            e.customArgs["id_item"] = window["ItemDetail" + key].GetValue();
        }
    }
}

function OnGridViewEndCallbackDetails(s, e) {
    UpdateTitlePanelDetails();
}

function UpdateTitlePanelDetails() {
    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + gvRequestInventoryMoveDetails.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = gvRequestInventoryMoveDetails.GetSelectedRowCount() - GetSelectedFilteredRowCount();

    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";
    $("#lblInfo").html(text);

    SetElementVisibility("lnkSelectAllRows", gvRequestInventoryMoveDetails.GetSelectedRowCount() > 0 && gvRequestInventoryMoveDetails.cpVisibleRowCount > selectedFilteredRowCount);
    SetElementVisibility("lnkClearSelection", gvRequestInventoryMoveDetails.GetSelectedRowCount() > 0);
    
}

function GetSelectedFilteredRowCount() {
    return gvRequestInventoryMoveDetails.cpFilteredRowCountWithoutPage + gvRequestInventoryMoveDetails.GetSelectedKeysOnPage().length;
}

function SetElementVisibility(id, visible) {
    var $element = $("#" + id);
    visible ? $element.show() : $element.hide();
}

function OnBatchStartEditingDetailRim(s, e) {
    // 
    var key = s.GetRowKey(e.visibleIndex);
    var modelNameColumn = s.GetColumnByField("id_item");
    if (!e.rowValues.hasOwnProperty(modelNameColumn.index))
        return;

    var cellInfo = e.rowValues[modelNameColumn.index];
    ReloadComboBox(cellInfo);
}

function OnBatchEditEndEditingDetailRim(s, e) {
    // 
    var key = s.GetRowKey(e.visibleIndex);

    var modelNameColumn = s.GetColumnByField("id_item");
    if (!e.rowValues.hasOwnProperty(modelNameColumn.index))
        return;

    var cellInfo = e.rowValues[modelNameColumn.index];
    cellInfo.value = id_item.GetValue();
    cellInfo.text = id_item.GetText();
    id_item.SetValue(null);
}

//Buttons Form
function AddNewDocument() {
}

function SaveDocument() {
}

function ApproveDocument() {
    showConfirmationDialog(function () {
        var headerDataInformation = GetDataHeaderForm();
        $.ajax({
            url: "RequestInventoryMove/Approve",
            type: "post",
            data: headerDataInformation,
            async: true,
            cache: true,
            error: function (error) {
                console.log(error);
                hideLoading();
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                // 
                if (result != undefined && result != null) {
                    if (result.hasError != undefined && result.hasError == "Y") {
                        gridMessageErrorRequestInventoryMove.SetText(ErrorMessage(result.message));
                        $("#GridMessageErrorRequestInventoryMove").show();
                    }
                    else {
                        $("#mainform").html(result);
                    }
                }
                console.log("success");
            },
            complete: function () {
                console.log("complete");
                hideLoading();
            }
        });
    }, "¿Desea Aprobar el Requerimiento?");

}

function AutorizeDocument() {
}

function ProtectDocument() {
}

function CancelDocument() {

}

function RevertDocument() {
    showConfirmationDialog(function () {
        var headerDataInformation = GetDataHeaderForm();
        $.ajax({
            url: "RequestInventoryMove/Revert",
            type: "post",
            data: headerDataInformation,
            async: true,
            cache: true,
            error: function (error) {
                console.log(error);
                hideLoading();
            },
            beforeSend: function () {
                showLoading();
            },
            success: function (result) {
                // 
                if (result != undefined && result != null) {
                    if (result.hasError != undefined && result.hasError == "Y") {
                        gridMessageErrorRequestInventoryMove.SetText(ErrorMessage(result.message));
                        $("#GridMessageErrorRequestInventoryMove").show();
                    }
                    else {
                        $("#mainform").html(result);
                    }
                }
                console.log("success");
            },
            complete: function () {
                console.log("complete");
                hideLoading();
            }
        });
    }, "¿Desea Reversar el Requerimiento?");
}

//Buttons Edit Form
function ButtonUpdate_Click() {
    Update(false);
}

function ButtonCancel_Click() {
    showPage("RequestInventoryMove/Index", null);
}

function Update(Approve) {
    showLoading();
    var headerDataInformation = GetDataHeaderForm();
    var validateDataFromForm = true;
    var idRim = parseInt(document.getElementById("id_RequestInventoryMove").getAttribute("idRequestInventoryMove"));
    var urlRim = (idRim === 0 || idRim === "0") ? "RequestInventoryMove/RequestInventoryMovePartialAddNew" : "RequestInventoryMove/RequestInventoryMovePartialUpdate";

    // 
    $.ajax({
        url: urlRim,
        type: "post",
        data: headerDataInformation,
        async: true,
        cache: true,
        error: function (error) {
            console.log(error);
            hideLoading();
        },
        beforeSend: function () {
        },
        success: function (result) {
            // 
            if (result != undefined && result != null) {
                if (result.hasError != undefined && result.hasError == "Y") {
                    gridMessageErrorRequestInventoryMove.SetText(ErrorMessage(result.message));
                    $("#GridMessageErrorRequestInventoryMove").show();
                }
                else {
                    $("#mainform").html(result);
                }
            }
        },
        complete: function () {
            console.log("complete");
            hideLoading();
        }
    });
}

function GetDataHeaderForm() {

    var _yEmissionDate = 0;
    var _mEmissionDate = 0;
    var _dEmissionDate = 0;
    var _hoursEmissionDate = 0;
    var _minutesdEmissionDate = 0;
    var _secondsEmissionDate = 0;

    if (emissionDate.GetValue() != null) {
        _yEmissionDate = emissionDate.GetValue().getFullYear();
        _mEmissionDate = emissionDate.GetValue().getMonth() + 1;
        _dEmissionDate = emissionDate.GetValue().getDate();
        _hoursEmissionDate = emissionDate.GetValue().getHours();
        _minutesdEmissionDate = emissionDate.GetValue().getMinutes();
        var completeEmissionDate = emissionDate.GetDate().toJSON();
    }

    return data = {
        id_Rim: parseInt(document.getElementById("id_RequestInventoryMove").getAttribute("idRequestInventoryMove")),
        yEmissionDate : _yEmissionDate,
        mEmissionDate: _mEmissionDate,
        dEmissionDate: _dEmissionDate,
        hoursEmissionDate: _hoursEmissionDate,
        minutesEmissionDate: _minutesdEmissionDate,
        id_PersonRequest: idPersonRequestF.GetValue(),
        id_Warehouse: idWarehouseF.GetValue(),
        id_NatureMove: idNatureMoveTransferP.GetValue(),
        emissionDateDoc: completeEmissionDate
    };
}

function TabControl_Init() {
}

function TabControl_ActiveTabChanged() {
}

//Basic Functions
function GetIdDocument() {
    return parseInt(document.getElementById("id_RequestInventoryMove").getAttribute("idRequestInventoryMove"));
}

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 2000);
    }
}

function UpdateView() {
    var id = GetIdDocument();

    // EDITING BUTTONS
    btnNew.SetEnabled(true);
    btnSave.SetEnabled(false);
    
    // STATES BUTTONS
    $.ajax({
        url: "RequestInventoryMove/Actions",
        type: "post",
        data: {
            id: GetIdDocument()
        },
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            btnApprove.SetEnabled(result.btnApprove);
            btnAutorize.SetEnabled(result.btnAutorize);
            btnProtect.SetEnabled(result.btnProtect);
            btnCancel.SetEnabled(result.btnCancel);
            btnRevert.SetEnabled(result.btnRevert);
        },
        complete: function (result) {
            hideLoading();
        }
    });
}

function UpdatePagination() {
    var current_page = 1;
    $.ajax({
        url: "RequestInventoryMove/InitializePagination",
        type: "post",
        data: {
            id: GetIdDocument()
        },
        async: false,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
        },
        success: function (result) {
            // 
            $("#pagination").attr("data-max-page", result.maximunPages);
            current_page = result.currentPage;
        },
        complete: function () {
        }
    });
    // 
    $('.pagination').current_page = current_page;
}

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