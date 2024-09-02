//#region Validaciones Tab Document
function OnValidate_Anio(s, e) {

}
function OnValidate_Mes(s, e) {
      
    let mes = s.GetValue();
    if (mes < 1 && mes > 12)
    {
        e.errorText = "Debe seleccionar valores entre 1 y 12";
        e.isValid = false;
    }


}
function OnValidate_Warehouse(s, e) {
      
    let _anio = anio.GetValue();
    let _mes = mes.GetValue();
    if (_anio == 0 || _mes == 0) {
        e.isValid = false;
        //e.errorText = "Ingrese año y mes";
        NotifyError("Ingrese año y mes");
    }
}


//#endregion


