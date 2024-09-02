
// Manejadores de control del Grid de Detalles
var ProductionCostDetails_OnBeginCallback = function(s, e) {
	e.customArgs[ 'id_productionCost' ] = $("#id_productionCost").val();
};


// Botones de edición

var ButtonUpdate_Click = function(s, e) {
	var isValid = ASPxClientEdit.ValidateEditorsInContainer(null);
	if (isValid) {
		gvProductionCost.UpdateEdit();
	}
};

var ButtonCancel_Click = function(s, e) {
	if (gvProductionCost !== null && gvProductionCost !== undefined) {
		gvProductionCost.CancelEdit();
	}
};