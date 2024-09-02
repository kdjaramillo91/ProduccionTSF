
var sumQuantity = 0;
var matrix = [];

function Pad(n, width, z) {
    z = z || '0';
    n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}

function OnBatchEditEndEditing(s, e) {

    var rowQuantityIndex = s.GetColumnByField("quantity").index;
    var rowQuantityValue = e.rowValues[rowQuantityIndex].value;

    var rowOrderIndex = s.GetColumnByField("order").index;
    var rowOrderValue = e.rowValues[rowOrderIndex].value;

     
    var esta = false;
    for (let i = 0; i < matrix.length; i++) {
        if (matrix[i].rowOrderValue == rowOrderValue) {
            esta = true;
            sumQuantity -= matrix[i].rowQuantityValue;
            matrix[i].rowQuantityValue = rowQuantityValue;
            sumQuantity += rowQuantityValue;
            break;
        }
    }

    if (!esta) {
        matrix.push({
            rowOrderValue: rowOrderValue,
            rowQuantityValue: rowQuantityValue
        });
        sumQuantity += rowQuantityValue;
    }

    //UpdatePoundsFields(sumQuantity);
}

function ShowCurrentItem(enabled) {
    var data = {
        id: $('#id_draining').val(),
        idProductionLotReceptionDetail: 0,
        enabled: enabled
    }

    showPage("DrainingTest/Edit", data);
}

function AddNewItem() {
    $.ajax({
        url: "DrainingTest/PendingNew",
        type: "post",
        data: data,
        async: true,
        cache: false,
        error: function (error) {
             
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
        url: 'DrainingTest/Approve',
        type: 'post',
        data: { id: $('#id_draining').val() },
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
    result.done(function(dialogResult) {
        if (dialogResult) {
            showLoading();
            $.ajax({
                url: 'DrainingTest/Reverse',
                type: 'post',
                data: { id: $('#id_draining').val() },
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
                url: 'DrainingTest/Annul',
                type: 'post',
                data: { id: $('#id_draining').val() },
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

function ConvertDateToString(date) {
    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();

    return Pad(day, 2) + "/" + Pad(month, 2) + "/" + Pad(year, 4);
}

function ConvertDateTimeToString(dateTime) {

    var hours = dateTime.getHours();
    var minutes = dateTime.getMinutes();

    return ConvertDateToString(dateTime) + " " + Pad(hours, 2) + ":" + Pad(minutes, 2);
}

function SaveDataUser() {
    
    var userData = {
        id: $('#id_draining').val(),
        id_receptionDetail: $('#id_receptionDetail').val(),
        emissionDate: ConvertDateToString(DateEmision.GetValue()),
        idAnalist: ComboBoxAnalist.GetValue(),
        dateTimeTesting: ConvertDateTimeToString(DateTimeTesting.GetValue()),
        temp: SpinEditTemp.GetValue(),
        drawersNumberSampling: SpinEditDrawers.GetValue(),
        reference: TextBoxReference.GetText(),
        description: MemoDescription.GetText(),
        numberSampling: SpinEditNumberSampling.GetValue(),
        poundsDrained: 0,
        poundsAverage: 0,
        poundsProjected: Math.trunc(SpinEditPoundsProjected.GetValue()),

        DrainingTestSampling: []
    }
    // 

    userData.DrainingTestSampling = GetODrainingTestSamplingWthiValues();
    //for (let rowDetail = 0; rowDetail < GridViewDetails.cpDetailCount; rowDetail++) {
    //    userData.drainingTestDetails.push({
    //        id: GridViewDetails.batchEditApi.GetCellValue(rowDetail, 0),
    //        order: parseInt(GridViewDetails.batchEditApi.GetCellValue(rowDetail, 1)),
    //        quantity: parseFloat(GridViewDetails.batchEditApi.GetCellValue(rowDetail, 2)).toFixed(4),
    //    });
    //}

    var drainingTest = {
        jsonDrainingTest: JSON.stringify(userData)
    };

    return drainingTest;
}

function SaveItem(aproved) {
    showLoading();

    if (!Validate()) {
        hideLoading();
        return;
    }

    $.ajax({
        url: 'DrainingTest/Save',
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
            $('#id_draining').val(id);

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

    if (!IsValid(DateTimeTesting) || DateTimeTesting.GetValue() == null) {
        errors += "Fecha y Hora de la Prueba es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(ComboBoxAnalist) || ComboBoxAnalist.GetValue() == null) {
        errors += "Analista es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(SpinEditTemp) || SpinEditTemp.GetValue() == null) {
        errors += "Temperatura es un campo Obligatorio. \n\r";
        validate = false;
    }
    if (!IsValid(SpinEditDrawers) || SpinEditDrawers.GetValue() == null || SpinEditDrawers.GetValue() == 0) {
        errors += "No. Gavetas Muestreo es un campo Obligatorio. \n\r";
        validate = false;
    }

   
    // validar numero de Muestreos y numero de registros Grid
    if (GridViewDrainingTestSampling.cpDetailCount !== SpinEditNumberSampling.GetValue() )
    {
        errors += "No. Gavetas Muestreo no tiene conconrdancia con las capacidades ingresadas. \n\r";
        validate = false;
    }

    var boRecErr = false;
    var boRecErrDetail = false;
    // validar Grid Muestreos
     
    for (let rowSampling = 0; rowSampling < GridViewDrainingTestSampling.cpDetailCount; rowSampling++)
    {
        boRecErrDetail = false;
        var _id = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowSampling, 0);
        var _capacidad = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowSampling, 3);
        var _nameMetricUnit = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowSampling, 6);
        var _value = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowSampling, 2);

        var _totalMuestreo = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowSampling, 7);
        var _totalPromedio = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowSampling, 8);
        var _totalProyectado = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowSampling, 9);


        if ( (_capacidad === undefined || _capacidad === null || _capacidad == 0 ||
              _value === undefined || _value === null || _value == 0) 
        )
        {

            if (boRecErr) continue;

            errors += "Debe ingresar valores en  Capacidad y/o Número de Kavetas. \n\r";
            boRecErr = true;
            validate = false;
            continue;
        }

        if ((_totalMuestreo === undefined || _totalMuestreo === null || _totalMuestreo == 0 ||
            _totalPromedio === undefined || _totalPromedio === null || _totalPromedio == 0 ||
            _totalProyectado === undefined || _totalProyectado === null || _totalProyectado == 0 
           )
        ) {

            if (boRecErr) continue;

            errors += "Debe realizar Cálculo Previo Muestreo. \n\r";
            boRecErr = true;
            validate = false;
            continue;
        }
         

        // var nameCell = "ID_" + _id
        //for (let rowDetail = 0; rowDetail < GridViewDetails.cpDetailCount; rowDetail++)
        //{
        //    var _index = GridViewDetails.batchEditApi.GetCellValue(rowDetail, 0);
        //    var _value = GridViewDetails.batchEditApi.GetCellValue(rowDetail, nameCell);
        //    if (_value !== null && _value == 0)
        //    {
                    
        //        errors += "Debe ingresar valores de Muestreo para Kavetas de " + _capacidad + " " + _nameMetricUnit + " posición #" + _index+"\n\r";
        //        boRecErrDetail = true;
        //        validate = false;
        //    }
        //}

        

    }



    if (validate == false) {
        NotifyError("Error. " + errors, TimeForView(errors));
    }

    return validate;
}

function ButtonCancel_Click() {
    RedirecBack();
}

function RedirecBack() {
    showPage("DrainingTest/Index");
}



function SpinEditDrawers_Change() {
   // UpdateDetails();
}

function UpdateDetails() {

    var userData = {
        drawersNumberSampling: SpinEditDrawers.GetValue()
    };

    $.ajax({
        url: 'DrainingTest/GridViewDetails',
        type: 'post',
        data: userData,
        async: true,
        cache: false,
        success: function (result) {
            $("#divGridViewDetails").html(result);
            sumQuantity = 0;
            matrix = [];
            UpdatePoundsFields(sumQuantity);
            hideLoading();
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

function UpdatePoundsFields(sumQuantity) {
    showLoading();
     
    var sumQuantityPounds = 0;
    var quantityIndividual = 0;
    for (let rowDetail = 0; rowDetail <= GridViewDetails.cpDetailCount; rowDetail++) {
        quantityIndividual = GridViewDetails.batchEditApi.GetCellValue(rowDetail, 2);
        quantityIndividual = quantityIndividual * 10;
        sumQuantityPounds += quantityIndividual;
    }

    sumQuantityPounds = sumQuantityPounds / 10;
    var count = GridViewDetails.cpDetailCount;
    sumQuantity = sumQuantityPounds;

    SpinEditPoundsDrained.SetValue(sumQuantity);
    
    var numFilas = GridViewDetails.cpDetailCount;
    var average = ((sumQuantity * 10 / count) / 10);
    average = floorFigure(average, 1);
    SpinEditPoundsAverage.SetValue(average);
    var faverage = parseFloat(average);

    var receptionDrawers = parseFloat(SpinEditReceptionDrawers.GetValue());
    var projected = (faverage * 10 * receptionDrawers) / 10;

    SpinEditPoundsProjected.SetValue(projected);

    hideLoading();
}

function InitializePagination() {

    if ($("#id_draining").val() !== 0) {
        var current_page = 1;
        var max_page = 1;
        $.ajax({
            url: "DrainingTest/InitializePagination",
            type: "post",
            data: { id: $("#id_draining").val() },
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
                showPage("DrainingTest/Pagination", { page: page });
            }
        });
    }
}

function PrintItem() {
    var data = { id_drainingTest: $('#id_draining').val() };
     
    $.ajax({
        url: 'DrainingTest/PrintDrainingReport',
        type: 'post',
        data: data,
        async: true,
        cache: false,
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
$(function () {
    InitializePagination();
    sumQuantity = 0;
    matrix = [];
});

/* Cambios Dosificacion
 * 2018-10-22
 * 
 * */
 

  

/* return Boolean 
   verdadero si hay cambios
   falso si no
*/
function ValidateRecordChangeInGridViewCapacity()
{
    // 

    var isValidate = false;
    for (let rowDetail = 0; rowDetail < GridViewDrainingTestSampling.cpDetailCount; rowDetail++)
    {
        
        var valueCapacity = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 3);
        var valueKavetas = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 2);

        if (valueCapacity != 0 || valueKavetas != 0)
        {
            isValidate = true;
            break;
        }

    }


    return isValidate;
}


function UpdateCapacity(s, e)
{

}
 

function UpdateDrawersNumber(s, e) {
    // 6 index
    // GridViewDrainingTestSampling
    
    var _drawersNumber = 0;
    for (let rowDetail = 0; rowDetail < GridViewDrainingTestSampling.cpDetailCount; rowDetail++)
    {
        gdrawersNumber += GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 2);
        //if (_drawersNumber > SpinEditDrawers.GetValue()) {

        //    NotifyError("Número de Kavetas ingresada es mayor al número de Kavetas del muestreo" );
        //}
    }

}



function onValidateDrawersNumber(s, e) {
    // 6 index
    // GridViewDrainingTestSampling
    // 
    var _drawerstotal = SpinEditDrawers.GetValue();
    var _samplingNumber = SpinEditNumberSampling.GetValue();
    var intCurrentRow = e.visibleIndex;
    var intColumnValue = 6;
    var intColumnDrawer = 2;
    var oldValue = GridViewDrainingTestSampling.batchEditApi.GetCellValue(intCurrentRow, intColumnValue);
    var newValue = e.itemValues[intColumnValue].value; 
    var id = e.itemValues[0].value; 
    var _drawersNumber = 0;
    for (let rowDetail = 0; rowDetail < GridViewDrainingTestSampling.cpDetailCount; rowDetail++)
    {
        if (rowDetail == intCurrentRow) {
            _drawersNumber += newValue
        }
        else
        {
            _drawersNumber += GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 2);
        }
        
    }

    if (_drawersNumber > SpinEditDrawers.GetValue())
    {

        NotifyError("Número de Kavetas ingresada es mayor al número de Kavetas del muestreo");
        e.cancel = true;
        //s.SetValue(currentValueOnChange);
    }

    var newValueDrawers = GridViewDrainingTestSampling.batchEditApi.GetCellValue(intCurrentRow, intColumnDrawer);
    if (_samplingNumber == 1 && newValueDrawers != _drawerstotal)
    {
        NotifyError("Número de Kavetas ingresado no puede ser distinto al número de Kavetas del muestreo");
        GridViewDrainingTestSampling.batchEditApi.SetCellValue(intCurrentRow, intColumnDrawer, _drawerstotal);
        e.cancel = true;
    }
    //else
    //{
    //    // 
    //    onChangeDrawersNumber(id, intCurrentRow, s, oldValue,e);
    //}

    
}

 

function onChangeDrawersNumber(id,row, o,oldValue,e)
{
    var eCancel = false;
    var directExecute = true;
    var numRecordTest = GridViewDetails.cpDetailCount;
    if (numRecordTest > 0)
    {
        directExecute = false;
        var nameCell = "ID_" + id;
        var hasChange = false;
        for (let rowDetail = 0; rowDetail < GridViewDetails.cpDetailCount; rowDetail++)
        {
            var _value = GridViewDetails.batchEditApi.GetCellValue(rowDetail, nameCell);
            if (_value !== null || _value > 0)
            {
                hasChange = true;
                break;
            }
        }

    }

    if (hasChange )
    {

        var okFunction = function ()
        {

            blankDrainingTestSamplinPounds(row);
            onGenerate_DrainingTestDetail(null, null, false);
        };

        var cancelFunction = function ()
        {
            // 
            GridViewDrainingTestSampling.batchEditApi.SetCellValue(row, 2, oldValue);
            
           // oEvent.cancel = true;
        }

        GenericFreeStyleShowConfirmationDialogTwoOptionsWithActionRightNow("¿Ha realizado modificaciones en los valores de muestreo, desea continuar, perderá los cambios no grabados?", " Ok ", " Cancel ", okFunction, cancelFunction);

    }

    if (directExecute)
    {
        blankDrainingTestSamplinPounds(row);
        onGenerate_DrainingTestDetail(null, null, false);
    }
    //ASPxClientUtils.PreventEventAndBubble(e);
   // event.preventDefault();
}


function blankDrainingTestSamplinPounds(row)
{

    GridViewDrainingTestSampling.batchEditApi.SetCellValue(row, 7, 0.0);
    GridViewDrainingTestSampling.batchEditApi.SetCellValue(row, 8, 0.0);
    GridViewDrainingTestSampling.batchEditApi.SetCellValue(row, 9, 0.0);

}

function SpinEditNumberSampling_Change(s, e)
{
    var _totalDrawers = SpinEditDrawers.GetValue();
    var _drawersNumber = s.GetValue();
    var okFunction = function ()
    {
        GridViewDrainingTestSampling.batchEditApi.EndEdit();
        UpdateGridViewDrainingTestSampling(_drawersNumber, _totalDrawers);
        onGenerate_DrainingTestDetail(null, null, true);
    };

    var cancelFunction = function ()
    {        
        var numSampling = GridViewDrainingTestSampling.cpDetailCount;
        SpinEditNumberSampling.SetValue(numSampling);
    }

    if (ValidateRecordChangeInGridViewCapacity()) {

        GenericFreeStyleShowConfirmationDialogTwoOptionsWithActionRightNow("¿Ha realizado modificaciones en las Capacidades de los Muestreos, desea continuar, perderá los cambios no grabados?", " Ok ", " Cancel ", okFunction, cancelFunction);
        
    }
    else
    {
        UpdateGridViewDrainingTestSampling(_drawersNumber, _totalDrawers);
        onGenerate_DrainingTestDetail(null, null,true);
    }

}


/* 
 * Parámetros
 * int Número de Muestreos
 * */
function UpdateGridViewDrainingTestSampling(_drawersNumber, _totalDrawers)
{
    var param = { drawersNumber: _drawersNumber, drawersTotal:_totalDrawers,isChange: false };
    GridViewDrainingTestSampling.PerformCallback(param);
}


function GetODrainingTestSampling()
{

    var DrainingTestDetail = [];
    var _SpinEditDrawers = SpinEditDrawers.GetValue();

    for (let rowDetail = 0; rowDetail < GridViewDrainingTestSampling.cpDetailCount; rowDetail++) {
        var _id = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 0);
        var _capacity = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 3);
        var _idMetricUnitCapacity = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 4);
        var _codeMetricUnitCapacity = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 5);
        var _nameMetricUnitCapacity = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 6);
        var _drawersNumber = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 2);
        var _poundsDrained = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 7);
        var _poundsAverage = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 8);
        var _poundsProjected = GridViewDrainingTestSampling.batchEditApi.GetCellValue(rowDetail, 9);

        if (_id === null || _capacity === null || _drawersNumber === null) continue;
        var _drainingTestSampling =
            {
                id: _id,
                capacity: _capacity,
                idMetricUnitCapacity: _idMetricUnitCapacity,
                codeMetricUnitCapacity: _codeMetricUnitCapacity,
                nameMetricUnitCapacity: _nameMetricUnitCapacity,
                drawersNumber: _drawersNumber,
                drawersNumberSamplig: _SpinEditDrawers,
                poundsDrained: _poundsDrained,
                poundsAverage  : _poundsAverage,
                poundsProjected: _poundsProjected,
                valuesDrainingTestDetail:[]
            }
        DrainingTestDetail.push(_drainingTestSampling);

    }
    return DrainingTestDetail;
}

function GetDrainingTestDetailValues(ODrainingTestSampling)
{
    var _DrainingTestDetail = ODrainingTestSampling;
    for (let i = 0; i < _DrainingTestDetail.length; i++) {

        //var _drawersNumberSamplig = 0;

        if (_DrainingTestDetail[i] === undefined || _DrainingTestDetail[i] === null) continue;
        var nameCell = "ID_" + _DrainingTestDetail[i].id;

        for (let rowDetail = 0; rowDetail < GridViewDetails.cpDetailCount; rowDetail++) {
            var _value = GridViewDetails.batchEditApi.GetCellValue(rowDetail, nameCell);

            if (_value === undefined || _value === null || _value === 0) continue;
            //_drawersNumberSamplig++;
            _DrainingTestDetail[i].valuesDrainingTestDetail.push(_value);
        }


       // _DrainingTestDetail[i].drawersNumberSamplig = _drawersNumberSamplig;
    }

    return _DrainingTestDetail;

}


function GetODrainingTestSamplingWthiValues()
{
    var _DrainingTestDetail = GetODrainingTestSampling();
    _DrainingTestDetail = GetDrainingTestDetailValues(_DrainingTestDetail);

    return _DrainingTestDetail;

}

/**
 * 
 * @param {any} s
 * @param {any} e
 * @param {any} isInit
 * Boolean: True si el Grid se presenta vacio;
 * 
 */
function onGenerate_DrainingTestDetail(s,e,isInit)
{
    // 
    var vInit = (isInit === undefined || isInit === null) ? false : isInit;

    if (vInit)
    {

        var drainingInfo = { jDrainingTestSamplingInfo: null };
    }
    else
    {
        var _DrainingTestDetail = GetODrainingTestSampling();
        var drainingInfo = { jDrainingTestSamplingInfo: JSON.stringify(_DrainingTestDetail) };
    }
    
    showPartialPage($("#divGridViewDetails"), "DrainingTest/GridViewDetailsDrainingTest", drainingInfo);


}

function onCalculate_DrainingTestDetail(s, e)
{
    
    var _DrainingTestDetail = GetODrainingTestSamplingWthiValues();    
    var callBackErr = function (err) { console.log(err) }
    var drainingInfo = { jDrainingTestSamplingInfo: JSON.stringify(_DrainingTestDetail) };
        genericAjaxCall("DrainingTest/CalculateDrainingTestSampling", true, drainingInfo, callBackErr, showLoading(), UpdateDrainingTestSamplingValues, hideLoading());
}

function UpdateDrainingTestSamplingValues(ODrainingTestSampling)
{

    if (ODrainingTestSampling === undefined || ODrainingTestSampling === null || ODrainingTestSampling.length == 0) return;

    for (let i = 0; i < ODrainingTestSampling.length; i++)
    {
        GridViewDrainingTestSampling.batchEditApi.EndEdit();        
        GridViewDrainingTestSampling.batchEditApi.SetCellValue(i, "poundsDrained", ODrainingTestSampling[i].poundsDrained);
        GridViewDrainingTestSampling.batchEditApi.SetCellValue(i, "poundsAverage", ODrainingTestSampling[i].poundsAverage);
        GridViewDrainingTestSampling.batchEditApi.SetCellValue(i, "poundsProjected", ODrainingTestSampling[i].poundsProjected);
    }

    SpinEditPoundsProjected.SetValue(ODrainingTestSampling[0].poundsTotalProjected);


}

function onEditGriViewDetail(s, e)
{
    //var indexColumn = e.cellInfo.column.index;
    //var indexRow = e.cellInfo.rowVisibleIndex;
    //var _value = GridViewDetails.batchEditApi.GetCellValue(indexRow, indexColumn);

    var indexColumn = e.focusedColumn.index;
    if (indexColumn == 0)
    {
        var editor = s.GetEditor(e.focusedColumn.fieldName);
        editor.SetEnabled((_value != null));
        return;

    }
    var _value = e.itemValues[indexColumn].value;    
    var editor = s.GetEditor(e.focusedColumn.fieldName);
    editor.SetEnabled((_value != null));
     
}