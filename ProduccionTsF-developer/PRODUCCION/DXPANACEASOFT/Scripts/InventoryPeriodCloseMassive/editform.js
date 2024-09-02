
//DIALOG BUTTONS ACTIONS
function Update(approve) {
    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, "tabdetail", true);

     if (!valid) {
         UpdateTabImage({ isValid: false }, "tabdetail");
     }

     if (valid) {
          
         var id = $("#id_InventoryPeriod").val();
   
         
      
         var data = "id=" + id + "&" + "approve=" + approve + "&" + $("#FormEditInventoryPeriodClose").serialize();
         var url = "InventoryPeriodCloseMassive/InventoryPerioClosedMassivePartialUpdate";
         

        showForm(url, data);
    }
}

function GenerateNewFromInventoryPeriodCloseMassiveAbrir(s, e) {

    processMassiveActionPeriod("InventoryPeriodCloseMassive/CheckAbrirPeriodos", "abrir");
    //showConfirmationDialog(function () {
    //    genericSelectedFieldActionCallBack("gvInventoryPeriod", "InventoryPeriodCloseMassive/CheckAbrirPeriodos",
    //        function (result) {
    //            if (result.codeReturn == 1) {
    //                gvInventoryPeriod.UnselectRows();
    //            }
    //            if (result.message.length > 0) {
    //                $("#msgInfoInventoryPeriodCloseMassiveList").empty();
    //
    //                $("#msgInfoInventoryPeriodCloseMassiveList").append(result.message)
    //                    .show()
    //                    .delay(5000)
    //                    .hide(0);
    //            }
    //        });
    //}, "¿Desea abrir los periodos seleccionados?");
}
function processMassiveActionPeriod(methodController, action)
{
    gvInventoryPeriod.GetSelectedFieldValues("id", function (values) {
        
        if (values.length == 0) {
            viewMessageClient("msgInfoInventoryPeriodCloseMassiveList",prepareMessageClient('alert-warning', 'Debe seleccionar el/los periodo(s) de inventario'));
            return;
        }
        let valoresSeleccionados = values;
        showConfirmationDialog(function () {

            var selectedRows = [];
            for (var i = 0; i < valoresSeleccionados.length; i++) {
                selectedRows.push(valoresSeleccionados[i]);
            }

            genericAjaxCall(methodController
                , true
                , { ids: selectedRows }
                , function (error) { console.log(error); }
                , function () { showLoading(); }
                , function (result) {

                    if (result !== null) {

                        if (result.codeReturn == 1) {
                            gvInventoryPeriod.UnselectRows();
                        }
                        viewMessageClient("msgInfoInventoryPeriodCloseMassiveList",result.message);
                    }

                }
                , function () { hideLoading(); }
            );


        }, `¿Desea ${action} los periodos seleccionados?`);

    });
}

function GeneratereasignNewFromInventoryPeriodCloseMassiveCerrar(s, e) {

    processMassiveActionPeriod("InventoryPeriodCloseMassive/CheckACerrarPeriodos", "cerrar");
    //gvInventoryPeriod.GetSelectedFieldValues("id", function (values) {
    //
    //    if (values.length == 0) {
    //        viewMessageClient('Debe seleccionar el/los periodo(s) de inventario');
    //        return;
    //    }
    //    let valoresSeleccionados = values;
    //    showConfirmationDialog(function ()
    //    {
    //
    //
    //    },)
    //
    //});

    //showConfirmationDialog(function () {
    //    genericSelectedFieldActionCallBack("gvInventoryPeriod", "InventoryPeriodCloseMassive/CheckACerrarPeriodos",
    //        function (result) {
    //            if (result.codeReturn == 1) {
    //                gvInventoryPeriod.UnselectRows();
    //            }
    //
    //            if (result.message.length > 0) {
    //                $("#msgInfoInventoryPeriodCloseMassiveList").empty();
    //
    //                $("#msgInfoInventoryPeriodCloseMassiveList").append(result.message)
    //                    .show()
    //                    .delay(5000)
    //                    .hide(0);
    //            }
    //        });
    //}, "¿Desea cerrar los periodos seleccionados?");
}

function genericSelectedFieldActionCallBack(obj, url, callback) {

    if (typeof obj === 'undefined') return;
    if (obj === null) return;
    if (typeof window[obj].GetSelectedFieldValues === 'undefined') return;
    if (typeof window[obj].PerformCallback === 'undefined') return;


    window[obj].GetSelectedFieldValues("id", function (values) {
        if (values.length == 0)
        {
            let result = {
                codeReturn: -1,
                message: 'Debe seleccionar el/los periodo(s) de inventario'
            };
            callback(result);
        }
        var selectedRows = [];
        for (var i = 0; i < values.length; i++) {
            selectedRows.push(values[i]);
        }


        if (typeof url === 'undefined' || url === null) {

            if (typeof callback !== 'undefined' && callback !== null) {
                let result = {
                    codeReturn: -1,
                    message: ''
                };
                callback(result);
            };

            return;
        }


        genericAjaxCall(url
            , true
            , { ids: selectedRows }
            , function (error) { console.log(error); }
            , function () { showLoading(); }
            , function (result) {

                if (result !== null) {

                    if (typeof callback !== 'undefined' && callback !== null) {
                        
                        callback(result);
                    };
                }

            }
            , function () { hideLoading(); }
        );


    });
}
function genericAjaxCall(_url, _async, _data, actionError, actionBeforeSend, actionSuccess, actionComplete) {


    // OK
    // --
    $.ajax({
        url: _url,
        type: "post",
        data: _data,
        async: true,
        cache: false,
        error: function (error) {
            if (actionError !== undefined)
                if (actionError != null) actionError(error);

        },
        beforeSend: function () {
            if (actionBeforeSend !== undefined)
                if (actionBeforeSend != null) actionBeforeSend();
            //showLoading();
        },
        success: function (result) {

            if (actionSuccess !== undefined)
                if (actionSuccess != null) actionSuccess(result);
        },
        complete: function () {

            if (actionComplete !== undefined)
                if (actionComplete != null) actionComplete();
            //hideLoading();
        }
    });

}

function OnClosed(sender) {

    var idAtt = $(sender).attr("data-id");
    var idObj = $(sender).data();
 
    var data = {
        id: idObj.id
    };

        $.ajax({
            url: "InventoryPeriodCloseMassive/InventoryPeriodDetailPartialUpdate",
        type: "post",
        data: { id: idObj.id },
        async: true,
        cache: false,
        error: function (error) {
            //console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            if(result.id ==1)
            {

                
                var msgErrorAux = ErrorMessage(result.mensaje);
                gridMessageErrora.SetText(msgErrorAux);
                $("#gridMessageErrora").show();
                

            } else {
                gridMessageErrora.SetText("");
                $("#gridMessageErrora").hide();;
            }
           

        },
        complete: function (result) {
          //  gvDetail.PerformCallback();
     
            tabControl.reloadContentOnCallback = true;
            gvDetail.PerformCallback();
            hideLoading();
        }
    });
    }



function TabControl_ActiveTabChanged(s, e) {

   

    
}

function ButtonUpdate_Click(s, e) {

    Update(false);
  
}

function ButtonUpdateClose_Click(s, e) {

}

function ButtonCancel_Click(s, e) {
    showPage("InventoryPeriodCloseMassive/Index", null);
}

//BUTTONS ACTIONS

function AddNewDocument(s, e) {
    var data = {
        id: 0,
        orderDetails: []
    };

    showPage("InventoryPeriodCloseMassive/FormEditInventoryPeriodCloseMassive", data);
}

function SaveDocument(s, e) {
    ButtonUpdate_Click(s, e);
}

function SaveCloseDocument(s, e) {
    ButtonUpdateClose_Click(s, e);
}

function CopyDocument(s, e) {

}

function ApproveDocument(s, e) {
  


}

function AutorizeDocument(s, e) {

}

function ProtectDocument(s, e) {

}

function CancelDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_InventoryPeriod").val()
        };
        showForm("InventoryPeriodCloseMassive/Cancel", data);
    }, "¿Desea anular el InventoryPeriodio?");
}

function RevertDocument(s, e) {
    showConfirmationDialog(function () {
        var data = {
            id: $("#id_InventoryPeriod").val()
        };
        showForm("InventoryPeriodCloseMassive/Revert", data);
    }, "¿Desea Activar?");
}

function ShowDocumentHistory(s, e) {

}



function PrintDocument(s, e) {
      
}


// DETAILS BUTTONS ACTIONS
function RefreshDetail(s, e) {
    Refresh(s, e);
}

// TABS FUNCTIONS

var activeGridView = null;
function TabControl_Init(s, e) {

    activeGridView = null;

    if (tabControl.GetActiveTab().name === "tabdetail") {
     
    } 
}

function AutoCloseAlert() {
    if ($(".alert-success") !== undefined && $(".alert-success") !== null) {
        setTimeout(function () {
            $(".alert-success").alert('close');
        }, 2000);
    }
}

function UpdateView() {
  
}


// MAIN FUNCTIONS

function init() {
  
    AutoCloseAlert();
}

// SELECTION

var customCommand = "";

function OnGridViewInit(s, e) {
    UpdateTitlePanel();
}

function OnGridViewBeginCallback(s, e) {
    customCommand = e.command;
}

function OnGridViewEndCallback(s, e) {
    UpdateTitlePanel();

}

function OnGridViewSelectionChanged(s, e) {
    UpdateTitlePanel();
}

function UpdateTitlePanel() {


    if (activeGridView === null)
        return;

    var selectedFilteredRowCount = GetSelectedFilteredRowCount();

    var text = "Total de elementos seleccionados: <b>" + activeGridView.GetSelectedRowCount() + "</b>";
    var hiddenSelectedRowCount = activeGridView.GetSelectedRowCount() - GetSelectedFilteredRowCount();
    if (hiddenSelectedRowCount > 0)
        text += "Elementos ocultos por filtrado seleccionados: <b>" + hiddenSelectedRowCount + "</b>";
    text += "<br />";

    var codeDocumentState = $("#codeDocumentState").val();

    if (codeDocumentState == "01") {
        var lblInfo = null;
        var lnkSelectAllRows = "";
        var lnkClearSelection = "";

        if (activeGridView === gvDetail) {
            lblInfo = $("#lblInfoDetails");
            lnkSelectAllRows = "lnkSelectAllRowsDetails";
            lnkClearSelection = "lnkClearSelectionDetails";
        }

        if (lblInfo !== null && lblInfo !== undefined) {
            lblInfo.html(text);
        }

        SetElementVisibility(lnkSelectAllRows, activeGridView.GetSelectedRowCount() > 0 && activeGridView.cpVisibleRowCount > selectedFilteredRowCount);
        SetElementVisibility(lnkClearSelection, activeGridView.GetSelectedRowCount() > 0);

    }

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