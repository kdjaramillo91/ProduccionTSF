const CAccionDocumentoAgregar = 'I';
const CAccionDocumentoEliminar = 'E';


var IntegrationProcessEdit = function () {
    "use strict";


    // Edit Lotes

    // Grabar Lote Principal
    var SaveLoteMain = function () {
 
        var valid = true;
        // Validaciones

        if (valid == false) {
            // Mensaje de Error
            return;
        }

        var id = $("#id_IntegrationProcess").val();
        var id_DocumentType = ASPxClientControl.GetControlCollection().GetByName("id_DocumentType").GetValue();
        var description = ASPxClientControl.GetControlCollection().GetByName("description").GetValue();
        var _dateAccounting = ASPxClientControl.GetControlCollection().GetByName("dateAccounting").GetValue();
        var dateAccounting = (_dateAccounting != null) ? _dateAccounting.toJSON() : null;

        var url = "";
        var data = {};
        if (id == 0) {
            data = { "id_DocumentType": id_DocumentType, "description": description, "dateAccounting": dateAccounting };
            url = "integrationprocess/addlote";
        }
        else {
            var headData = { "id_IntegrationProcess": id, "id_DocumentType": id_DocumentType, "description": description, "dateAccounting": dateAccounting };
            var objDocumentsList = GetDocuments();
            var detailData = { "id_integrationProcessDetailList": objDocumentsList };

            data = $.extend(headData, detailData);
            url = "integrationprocess/updatelote";
        }

        showFormPostBack(url, data, OIntegrationProcess.ConfigControls);
    };

    // Aprobar Lote
    var approveLote = function () {

        var data = GetDataId();
        if (data == null) return;
        var url = "integrationprocess/approvelote";
        genericAjaxCall(
            url,
            true,
            data,
            function (error) { console.log(error); },
            function () { showLoading(); },
            function (result) {

                $("#integrationprocessmsg").html(result.message);
                if (result.codeReturn == -1) {
                    hideLoading();
                    return;
                }
                actionControls(result.ActionAccessList);
                valueControls(result.ValueDataList);

            },
            function () { hideLoading(); });
    }

    // Procesar Lote
    var processLote = function () {

        var data = GetDataId();
        if (data == null) return;
        var url = "integrationprocess/execlote";
        genericAjaxCall(
            url,
            true,
            data,
            function (error) { console.log(error); },
            function () { showLoading(); },
            function (result) {

                $("#integrationprocessmsg").html(result.message);
                if (result.codeReturn == -1) {
                    hideLoading();
                    return;
                }
                actionControls(result.ActionAccessList);
                valueControls(result.ValueDataList);

            },
            function () { hideLoading(); });

    };

    // Delete Lote
    var deleteLote = function () {
        var data = GetDataId();
        if (data == null) return;
        var url = "integrationprocess/dellote";
        genericAjaxCall(
            url,
            true,
            data,
            function (error) { console.log(error); },
            function () { showLoading(); },
            function (result) {

                $("#integrationprocessmsg").html(result.message);
                actionControls(result.ActionAccessList);
                valueControls(result.ValueDataList);

            },
            function () { hideLoading(); });
    };

    // Print Lote
    var printLoteMain = function () {

        // 
        var documentTypeisGroup = id_DocumentType.GetSelectedItem().GetColumnText('isGroup');
        var codeReport = "RINTRE";
        if (documentTypeisGroup == "True") {
            GenericFreeStyleShowConfirmationDialogTwoOptionsWithActionRightNow("Elija el tipo de reporte a visualizar.", "Reporte Detallado", "Reporte Agrupado", function () { OIntegrationProcess.PrintLotePost("RINTRE"); }, function () { OIntegrationProcess.PrintLotePost("RINTGP"); });
        }
        else {

            printLoteMainPost(codeReport);
        }



    };

    var printLoteMainPost = function (codeReport) {
        // 
        var OintegrationProcess = GetDataId();
        var id_IntegrationProcess = (OintegrationProcess != null) ? OintegrationProcess.id_IntegrationProcess : "";


        var data = "codeReport=" + codeReport + "&id_IntegrationProcess=" + id_IntegrationProcess;


        if (data != null) {
            $.ajax({
                url: "integrationprocess/printlote",
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
                    // 
                    try {
                        if (result != undefined) {
                            var reportTdr = result.nameQS;
                            var url2 = 'ReportProd/Index?trepd=' + reportTdr;

                            var responseWindows = GenericOpenWindow(url2, '_blank', 'toolbar=0,location=0,menubar=0, locationbar=0, resizable=yes, addressbar=0');
                            if (responseWindows.length == 2) {
                                if (responseWindows[1] != 'OK') $("#integrationprocessmsg").html(responseWindows[1]);

                            }
                            //newWindow.focus();
                            hideLoading();
                        }
                    }
                    catch (err) {
                        console.log(err);
                        hideLoading();
                    }
                },
                complete: function () {
                    hideLoading();
                }
            });
        }


    };

    // Edit Documents    // Movimiento de Documentos
    var MoveDocuments = function (obj_STDocumentsIP, int_Id_Document, const_TypeMove) {

        if (const_TypeMove == CAccionDocumentoAgregar || const_TypeMove == null || typeof (const_TypeMove) === 'undefined') {
            obj_STDocumentsIP.push(int_Id_Document);
        }
        else if (const_TypeMove == CAccionDocumentoEliminar) {
            var index = obj_STDocumentsIP.indexOf(int_Id_Document.toString());
            if (index > -1) {
                obj_STDocumentsIP.splice(index, 1);
            }

        }

        //  return obj_STDocumentsIP;
    }

    // CallBack Remover Grid Documentos
    var RemoveDocumentPostBack = function (idDocument) {

        var arDocuments = GetDocuments();
        for (var i = 0; i < (idDocument.length); i++) {
            MoveDocuments(arDocuments, idDocument[i], CAccionDocumentoEliminar);
        }
        SetDocuments(arDocuments);
        SaveLoteMain();
    };

    // CallBack Retornar Documentos Grid Documentos Busqueda
    var ReturnDocumentsToIntegrationProcess = function (values) {

        var arDocuments = GetDocuments();
        for (var i = 0; i < (values.length); i++) {
            MoveDocuments(arDocuments, values[i], CAccionDocumentoAgregar);
        }
        SetDocuments(arDocuments);
        SaveLoteMain();
        $.fancybox.close();

    };


    // Almacenamiento Documentos en Session 
    var SetDocuments = function (oDocuments) {

        sessionStorage.setItem("iDocumentsIP", oDocuments);
    };

    // Obtención Documentos en Session
    var GetDocuments = function () {
        var infoStorage = sessionStorage.getItem("iDocumentsIP");
        if (typeof infoStorage == "undefined" || infoStorage == null || infoStorage.length == 0) return new Array();
        return infoStorage.split(",");
    };

    // Eliminar Documentos en Session
    var DeleteDocuments = function () {
        sessionStorage.removeItem("iDocumentsIP");
    };

    //Obtener Data Id
    var GetDataId = function (anyWay) {
        var id = $("#id_IntegrationProcess").val();
        if ((id == 0 || id == null) && anyWay != true) return null;

        var data = { "id_IntegrationProcess": id };
        return data;
    };
    // Auxiliares
    var actionControls = function (arAction) {

        for (var i = 0; i < arAction.length; i++) {
            var obj = ASPxClientControl.GetControlCollection().GetByName(arAction[i].CodeObject);
            if (typeof obj != 'undefined' && obj != null) {
                obj.SetEnabled(arAction[i].Enabled);

            }
        }

    };
    var valueControls = function (arValues) {
        for (var i = 0; i < arValues.length; i++) {
            var obj = ASPxClientControl.GetControlCollection().GetByName(arValues[i].CodeObject);
            if (typeof obj != 'undefined' && obj != null) {
                obj.SetValue(arValues[i].valueObject);

            }
        }

    };
    return {

        // Indicar si es requerido la fecha
        SelectedIndexChangedDocumentType: function (s, e) {

            var isRequiredDate = s.GetSelectedItem().GetColumnText('isRequiredDate');
            if (isRequiredDate == "True") {
                $("[name='dateAccounting']").removeProp('readonly');

                $("td img[id*='dateAccounting']").show();

            }
            else {
                $("[name='dateAccounting']").prop('readonly', true);
                $("td img[id*='dateAccounting']").hide();

                var obj = ASPxClientControl.GetControlCollection().GetByName("dateAccounting");
                if (typeof obj != 'undefined' && obj != null) {
                    var currentTime = new Date();
                    obj.SetDate(currentTime);
                }

            }

        },
        // Propiedades Edit Lote
        // Init Lote
        InitGridLotes: function (s, e) {

        },
        // Buscar Lote
        FindLote: function (s, e) {

            var data = $("#formFilterIntegrationProcess").serialize();


            //var data = $("#formFilterIntegrationProcessDocument").serialize() + "&id_DocumentType=" + id_DocumentType + "&id_integrationProcessDetailList=" + JSON.stringify(arDocuments);
            var url = "integrationprocess/findlotes";

            if (data != null) {

                genericAjaxCall(
                    url,
                    true,
                    data,
                    function (error) { console.log(error); },
                    function () { showLoading(); },
                    function (result) {
                        $("#btnCollapse").click();
                        $("#results").html(result);
                    },
                    function () { hideLoading(); });
            }
            event.preventDefault();
        },
        // Limpiar Filtro Busqueda Lote
        ClearFilterLote: function (s, e) {
            codeLote.SetText("");
            id_DocumentType.SetSelectedItem(null);
            id_StatusLote.SetSelectedItem(null);
            description.SetText("");
            dateInitCreate.SetDate(null);
            dateEndCreate.SetDate(null);
            dateInitExec.SetDate(null);
            dateEndExec.SetDate(null);
            dateInitFullIntegration.SetDate(null);
            dateEndFullIntegration.SetDate(null);
        },
        // Agregar Nuevo Lote
        AddNewLote: function (s, e) {
            var data =
            {
                id_IntegrationProcess: 0
            };

            showPageCallBack("integrationprocess/getlote", data, OIntegrationProcess.ConfigControls);

        },
        // Editar Lote
        EditLote: function (s, e) {
            var data =
            {
                id_IntegrationProcess: gvIntegrationProcessLotes.GetRowKey(e.visibleIndex)
            };

            showPageCallBack("integrationprocess/getlote", data, OIntegrationProcess.ConfigControls);
            //showPageCallBack("integrationprocess/getlote", data,null);

        },
        // Grabar Lote DevX
        SaveLote: function (s, e) {
            SaveLoteMain();
        },
        // Anular Lote DevX
        AnularLote: function (s, e) {

            showConfirmationDialog(function () {
                deleteLote();
            }, "¿Desea eliminar el Lote de Integración?");

        },
        // Salir Edicion Lote 
        CancelEditLote: function (s, e) {
            showPage("integrationprocess/Index", data);
        },
        // Procesar lote
        AprobarLote: function (s, e) {
            showConfirmationDialog(function () {
                approveLote();
            }, "¿Desea Aprobar el Lote de Integración?");
        },

        // Procesar lote
        ProcesarLote: function (s, e) {
            showConfirmationDialog(function () {
                processLote();
            }, "¿Desea Transmitir el Lote de Integración?");
        },

        ConfigControls: function () {

            var chkReadyStateMain = setInterval(function () {
                if (document.readyState === "complete") {
                    clearInterval(chkReadyStateMain);
                    var data = GetDataId();
                    if (data == null) return;
                    var url = "integrationprocess/configcontrol";
                    genericAjaxCall(
                        url,
                        true,
                        data,
                        function (error) { console.log(error); },
                        null,
                        function (result) {

                            if (result.codeReturn < 0) return;
                            actionControls(result.ActionAccessList);
                        },
                        null);

                }
            }, 100);



        },
        // Propiedades Edit Document        
        // Inicializar Grid Documentos
        InitGridDocuments: function (s, e) {

            //DeleteDocuments();
            //var iDocumentsIP = new Array();

            //for (var i = 0; i < s.cpVisibleRowCount; i++)
            //{
            //    s.GetRowValues(i, 'id_Document', function (arDocuments)
            //    {
            //        // 
            //        MoveDocuments(iDocumentsIP, arDocuments, CAccionDocumentoAgregar);

            //        SetDocuments(iDocumentsIP);
            //    });

            //}

            //if (s.cpVisibleRowCount > 0)
            //{
            //    s.SelectRows();
            //    s.GetSelectedFieldValues('id_Document', SetDocuments);
            //    //s.UnselectRows();
            //}


            // s.GetRowValues('id_Document', SetDocuments);

            //if (iDocumentsIP.length > 0) SetDocuments(iDocumentsIP);

        },

        // Agregar Documentos 
        AddNewDocument: function (s, e) {

            // Levantar Documento
            // $('.nyroModal').click();

            showThickBox("integrationprocess/indexfind", null);

        },
        // Seleccionar documentos

        ExecSelection: function (s, e) {

            gvIntegrationProcessDetailResult.GetSelectedFieldValues('id_Document', ReturnDocumentsToIntegrationProcess);
        },
        // CallBack Remover Grid Documentos
        RemoveDocument: function (s, e) {

            showConfirmationDialog(function () {
                gvIntegrationProcessDetail.GetSelectedFieldValues('id_Document', RemoveDocumentPostBack);
            }, "¿Desea eliminar del Lote de Integración, los documentos seleccionados ?");

        },

        //Cancelar Seleccion de Documentos
        CancelEditSelection: function (s, e) {
            $.fancybox.close();
        },

        // Session Data
        GetIDoc: function () {
            return GetDocuments();
        },
        // Impresion
        PrintLote: function () {
            printLoteMain();

        },
        PrintLotePost: function (codeReport) {
            printLoteMainPost(codeReport);

        }
    };

};

OIntegrationProcess = new IntegrationProcessEdit();


//var globalDocumentsSelected = new Array();

function btnSearch_click(s, e) {

}

function btnClear_click(s, e) {

}



/* Detail Functions */


/*
// Evento Click Remove Button
function RemoveDocument(s, e)
{    
    gvIntegrationProcessDetail.GetSelectedFieldValues('id_Document', RemoveDocumentPostBack);
}
*/

function RefreshDetail(s, e) {

}


