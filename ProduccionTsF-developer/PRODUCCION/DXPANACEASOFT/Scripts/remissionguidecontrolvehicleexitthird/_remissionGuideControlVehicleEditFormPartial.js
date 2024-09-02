
function exitDateProductionBuildingValueChanged(s, e) {
    if (exitTimeProductionBuilding.GetText() != null) {
        exitTimeProductionBuilding.Validate();
    }
}

function exitTimeProductionBuildingValueChanged(s, e) {

}

function exitDateProductionBuildingValidation(s, e) {
    if (e.value == null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        return;
    }
    if (e.value != null) {
        // 
        var entranceDateProductionBuildingTmp3 = $("#entranceDateProductionBuildingT").val();
        var entranceDateProductionBuildingTmp2 = "";
        if (entranceDateProductionBuildingTmp3 != null) {
            var tmp10 = entranceDateProductionBuildingTmp3.split("/");
            if (tmp10.length == 3) {
                entranceDateProductionBuildingTmp2 = tmp10[1] + "/" + tmp10[0] + "/" + tmp10[2];
            }
        }

        var exitDateProductionBuildingTmp2 = exitDateProductionBuilding.GetText();

        var exitDateProductionBuildingTmp = "";

        
        
        var tmp = exitDateProductionBuildingTmp2.split("/");
        if (tmp.length == 3) {
            exitDateProductionBuildingTmp = tmp[1] + "/" + tmp[0] + "/" + tmp[2];
        }

        if (exitDateProductionBuildingTmp2 != null) {
            if ((new Date(entranceDateProductionBuildingTmp2).getTime() > new Date(exitDateProductionBuildingTmp).getTime())) {
                e.isValid = false;
                e.errorText = "Fecha de Salida de Planta no puede ser mayor que Entrada a Planta.";
                UpdateTabImage(e, "tabRemissionGuideControlEntrance");
                return;
            }
        }

    }
}

function exitTimeProductionBuildingValidation(s, e) {
    // 
    if (e.value == null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        return;
    }
    if (e.value != null) {
        //Hora
        var entranceTimeProductionBuildingTmp = $("#entranceTimeProductionBuildingT").val();
        var entranceTimeProductionBuildingTmp2 = entranceTimeProductionBuildingTmp.substring(0, 5);
        //Fecha
        var entranceDateProductionBuildingTmp3 = $("#entranceDateProductionBuildingT").val();
        var entranceDateProductionBuildingTmp2 = "";

        if (entranceDateProductionBuildingTmp3 != null) {
            var tmp10 = entranceDateProductionBuildingTmp3.split("/");
            if (tmp10.length == 3) {
                entranceDateProductionBuildingTmp2 = tmp10[1] + "/" + tmp10[0] + "/" + tmp10[2];
            }
        }

        //Hora
        var exitTimeProductionBuildingTmp = exitTimeProductionBuilding.GetText();
        var exitTimeProductionBuildingTmp2 = exitTimeProductionBuildingTmp.substring(0, 5);
        //Fecha
        var exitDateProductionBuildingTmp2 = exitDateProductionBuilding.GetText();
        var exitDateProductionBuildingTmp = "";

        if (entranceTimeProductionBuildingTmp2 != null) {
            if (entranceDateProductionBuildingTmp2 != null && exitDateProductionBuildingTmp2 != null) {
                var tmp = exitDateProductionBuildingTmp2.split("/");
                if (tmp.length == 3) {
                    exitDateProductionBuildingTmp = tmp[1] + "/" + tmp[0] + "/" + tmp[2];
                }
                if ((new Date(entranceDateProductionBuildingTmp2).getTime() == new Date(exitDateProductionBuildingTmp).getTime())) {
                    var anio = entranceDateProductionBuildingTmp2.substr(6, 4);
                    var mes = entranceDateProductionBuildingTmp2.substr(3, 2);
                    var dia = entranceDateProductionBuildingTmp2.substr(0, 2);
                    var horas = entranceTimeProductionBuildingTmp2.substr(0, 2);
                    var minutos = entranceTimeProductionBuildingTmp2.substr(3, 2);

                    var anio2 = exitDateProductionBuildingTmp.substr(6, 4);
                    var mes2 = exitDateProductionBuildingTmp.substr(3, 2);
                    var dia2 = exitDateProductionBuildingTmp.substr(0, 2);
                    var horas2 = exitTimeProductionBuildingTmp2.substr(0, 2);
                    var minutos2 = exitTimeProductionBuildingTmp2.substr(3, 2);
                     
                    //Compara Horas y Minutos
                    if (new Date(anio, mes, dia, horas, minutos, 0).getTime() > new Date(anio2, mes2, dia2, horas2, minutos2, 0).getTime()) {
                        e.isValid = false;
                        e.errorText = "Hora de Entrada a Planta no puede ser mayor que Salida de Planta.";
                        UpdateTabImage(e, "tabRemissionGuideControlEntrance");
                        return;
                    }
                }
            }
        }
    }
}