
 $(document).ready(function () {
       

     $("td").on("click",".ps-bar-collapse", function () {
           var className = $(this).attr("dtx");           
            $("." + className).toggle("slow");
        });

 });


// isRequired
// return true cumple condicion
// return false no cumple condicion
 function isRequired(s, e)
 {
     if (  e.value != undefined )
         if (e.value != null) return true;

     with (e) { e.isValid = false; e.errorText = "Campo Obligatorio"; }; return false;

 }


 function findJsonData(Obj,Key,Value )
 {

     var boolIsData = false;
     if (Obj == undefined) return false;
     if (Obj == null) return false;
     

     for (let i = 0; i < Obj.length; i++)
     {
         var _lvalue = Obj[i][Key];
         if (_lvalue == Value)
         {
             boolIsData = true;
             break;
         }
           
     }
      
     return boolIsData;

 }

 function boolInputHidden(strObj)
 {
     if (strObj == undefined) return null;
     if (strObj == null || strObj.length ==0) return null;
     if ($("#" + strObj).val() == undefined) return null;
     var valBooleanObj = $("#" + strObj).val() ;
     if ( valBooleanObj  == null) return null;
     return ((valBooleanObj == 'True' || valBooleanObj == 'true') ? true : false);
 }



 function btnSearchGeneric(s, e)
 {
    
     var objForm = s.cpobjForm;
     var mvcController = s.cpmvcController;
     var htmlContainer = s.cphtmlContainer;


     if (objForm == undefined) return false;
     if (objForm == null) return false;
     if ($("#"+objForm) == undefined) return false;
     if ($("#"+objForm) == null) return false;

     if (mvcController == undefined) return false;
     if (mvcController == null) return false;

     if (htmlContainer == undefined) return false;
     if (htmlContainer == null) return false;


     var data = $("#" + objForm).serialize();

     if (data != null)
     {
         genericAjaxCall(mvcController, true , data, null, showLoading(), function (result) {
             $("#btnCollapse").click();
             $("#" + htmlContainer).html(result);

         }, hideLoading() );
     }
     event.preventDefault();
 }
 

 function genericAjaxCall(_url, _async, _data, actionError, actionBeforeSend, actionSuccess, actionComplete)
 {


     // OK
     // --
     $.ajax({
         url: _url,
         type: "post",
         data: _data,
         async: true,
         cache: false,
        // dataType:(format===undefine)?json:format,
         error: function (error)
         {
             if (actionError !== undefined)
                 if (actionError != null) actionError(error);
             
         },
         beforeSend: function ()
         {
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

 function getFormatter ()
 {
     return formatter = new Intl.NumberFormat('en-US', {
         style: 'currency',
         currency: 'USD',
         minimumFractionDigits: 2,
         // the default value for minimumFractionDigits depends on the currency
         // and is usually already 2
     });

 }
  
 
 function genericSetElementVisibility(id, visible)
 {
     var $element = $("#" + id);
     visible ? $element.show() : $element.hide();

 }
 
          
 function genericPerformDocumentAction(obj, url) {

     if (typeof obj === 'undefined') return;
     if (obj === null) return;
     if (typeof obj === 'url') return;
     if (url === null) return;
     if (typeof window[obj].GetSelectedFieldValues === 'undefined') return;
     if (typeof window[obj].PerformCallback === 'undefined') return;
     

     
     window[obj].GetSelectedFieldValues("id", function (values)
     {

         var selectedRows = [];
         for (var i = 0; i < values.length; i++) {
             selectedRows.push(values[i]);
         }

         genericAjaxCall(url, true, { ids: selectedRows }, function (error) { console.log(error); }, null, null, window[obj].PerformCallback());


     });
 }


 function number_format(amount, decimals) {

     amount += ''; // por si pasan un numero en vez de un string
     amount = parseFloat(amount.replace(/[^0-9\.]/g, '')); // elimino cualquier cosa que no sea numero o punto

     decimals = decimals || 0; // por si la variable no fue fue pasada

     // si no es un numero o es igual a cero retorno el mismo cero
     if (isNaN(amount) || amount === 0)
         return parseFloat(0).toFixed(decimals);

     // si es mayor o menor que cero retorno el valor formateado como numero
     amount = '' + amount.toFixed(decimals);

     var amount_parts = amount.split('.'),
         regexp = /(\d+)(\d{3})/;

     while (regexp.test(amount_parts[0]))
         amount_parts[0] = amount_parts[0].replace(regexp, '$1' + ',' + '$2');

     return amount_parts.join('.');
 }



// Fork de SCRIPTS STANDAR:: 
// Agregar funcion callback
  function OnUpdateImagenWhenRequiredField2(s, e) {
     
     var messageErrorControl = "";
     if (s.cpHasTab != undefined) {
         if (s.cpHasTab == "false") {
             if (s.cpIsRequired == "true") {
                 if (e.value == null) {
                     e.errorText = s.cpMessageError;
                     e.isValid = false;
                     return;
                 } else {
                     if (s.cpInitialCondition != undefined) {
                         if (e.value == s.cpInitialCondition) {
                             e.errorText = s.cpMessageError;
                             e.isValid = false;
                             return;
                         }
                     }
                 }
             }
             if (s.cpMinimunLength != undefined) {
                 if (e.value != null) {
                     if (e.value.length < s.cpMinimunLength) {
                         e.errorText = "La longitud Mínimo es " + s.cpMinimunLength;
                         e.isValid = false;
                         return;
                     }
                 }
             }
             if (s.cpMaximunLength != undefined) {
                 if (e.value != null) {
                     if (e.value.length > s.cpMaximunLength) {
                         imageUrl = "/Content/image/info-error.png";
                         e.errorText = "La Longitud Máximo es " + s.cpMaximunLength;
                         if (tab !== null) {
                             tab.SetImageUrl(imageUrl);
                             tab.SetActiveImageUrl(imageUrl);
                         }
                         return;
                     }
                 }
             }
             if (!e.isValid) {
                 if (s.cpMessageErrorFormart != undefined) {
                     e.errorText = s.cpMessageErrorFormart;
                     e.isValid = false;
                     return;
                 }
             }
         }
     } else {
         if (s.cpTabContainer == undefined || s.cpTabControl == undefined) {
             return;
         }
     }

     if (s.cpTabContainer == undefined || s.cpTabControl == undefined) {
         return;
     }
     var controls = ASPxClientControl.GetControlCollection();
     var genericTabControl = controls.GetByName(s.cpTabControl);
     var tab = genericTabControl.GetTabByName(s.cpTabContainer);

     if (tab.GetVisible() === false) {
         e.isValid = true;
         return;
     }
     var imageUrl = "/Content/image/noimage.png";
     tab.SetImageUrl(imageUrl);
     tab.SetActiveImageUrl(imageUrl);

     if (s.cpIsRequired == "true") {
         if (e.value == null) {
             imageUrl = "/Content/image/info-error.png";
             e.errorText = s.cpMessageError;
             e.isValid = false;
             if (tab !== null) {
                 tab.SetImageUrl(imageUrl);
                 tab.SetActiveImageUrl(imageUrl);
             }
             return;
         } else {
             if (s.cpInitialCondition != undefined) {
                 if (e.value == s.cpInitialCondition) {
                     imageUrl = "/Content/image/info-error.png";
                     e.errorText = s.cpMessageError;
                     e.isValid = false;
                     if (tab !== null) {
                         tab.SetImageUrl(imageUrl);
                         tab.SetActiveImageUrl(imageUrl);
                     }
                     return;
                 }
             }
         }
     }
     if (s.cpMinimunLength != undefined) {
         if (s.cpMinimunLength != 0) {
             if (e.value != null) {
                 if (e.value.length < s.cpMinimunLength) {
                     imageUrl = "/Content/image/info-error.png";
                     e.errorText = "La longitud Mínimo es " + s.cpMinimunLength;
                     e.isValid = false;
                     if (tab !== null) {
                         tab.SetImageUrl(imageUrl);
                         tab.SetActiveImageUrl(imageUrl);
                     }
                     return;
                 }
             }
         }

     }
     if (s.cpMaximunLength != undefined) {
         if (e.value != null) {
             if (e.value.length > s.cpMaximunLength) {
                 imageUrl = "/Content/image/info-error.png";
                 e.errorText = "La Longitud Máximo es " + s.cpMaximunLength;
                 e.isValid = false;
                 if (tab !== null) {
                     tab.SetImageUrl(imageUrl);
                     tab.SetActiveImageUrl(imageUrl);
                 }
                 return;
             }
         }

     }
     if (s.cpCallBack != undefined)
     {
         //console.log(s.cpCallBack);
         e.isValid  =  window[s.cpCallBack](s, e);
     }
     if (!e.isValid) {
         if (s.cpMessageErrorFormart != undefined) {
             imageUrl = "/Content/image/info-error.png";
             e.errorText = s.cpMessageErrorFormart;
             e.isValid = false;
             if (tab !== null) {
                 tab.SetImageUrl(imageUrl);
                 tab.SetActiveImageUrl(imageUrl);
             }
             return;
         }
     }
 }
