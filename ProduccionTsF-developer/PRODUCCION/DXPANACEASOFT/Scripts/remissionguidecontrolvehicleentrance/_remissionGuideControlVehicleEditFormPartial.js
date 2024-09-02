function entranceDateProductionUnitProviderBuildingValidation(s, e) {
     
    if (e.value == null) {
        //e.isValid = false;
        //e.ErrorText = "Campo Obligatorio.";
        return;
    } else {
        var exitDateProductionBuildingTmp3 = $("#exitDateProductionBuildingT").val();
        var exitDateProductionBuildingTmp2 = "";
        if (exitDateProductionBuildingTmp3 != null) {
            var tmp10 = exitDateProductionBuildingTmp3.split("/");
            if (tmp10.length == 3) {
                exitDateProductionBuildingTmp2 = tmp10[1] + "/" + tmp10[0] + "/" + tmp10[2];
            }
        }
        //var exitDateProductionBuildingTmp2 = exitDateProductionBuildingTmp.substring(0, 10);
         
        var entranceDatePUPTmp2 = entranceDateProductionUnitProviderBuilding.GetText();
        var entranceDatePUPTmp = "";
        var tmp = entranceDatePUPTmp2.split("/");
        if (tmp.length == 3) {
            entranceDatePUPTmp = tmp[1] + "/" + tmp[0] + "/" + tmp[2];
        }

        if (exitDateProductionBuildingTmp2 != null) {
            if ((new Date(exitDateProductionBuildingTmp2).getTime() > new Date(entranceDatePUPTmp).getTime())) {
                e.isValid = false;
                e.errorText = "Fecha de Salida de Planta no puede ser mayor que Entrada a Camaronera.";
                return;
            }
        }
    }
}

function entranceDateProductionUnitProviderBuildingValueChanged(s, e) {
    if (exitDateProductionUnitProviderBuilding.GetText() != null) {
        exitDateProductionUnitProviderBuilding.Validate();
    }
    if (entranceDateProductionBuilding.GetText() != null) {
        entranceDateProductionBuilding.Validate();
    }
    if (entranceTimeProductionUnitProviderBuilding.GetText() != null) {
        entranceTimeProductionUnitProviderBuilding.Validate();
    }
}

function exitDateProductionUnitProviderBuildingValueChanged(s, e) {

    if (entranceDateProductionUnitProviderBuilding.GetText() != null) {
        entranceDateProductionUnitProviderBuilding.Validate();
    }
    if (entranceDateProductionBuilding.GetText() != null) {
        entranceDateProductionBuilding.Validate();
    }
    if (exitTimeProductionUnitProviderBuilding.GetText() != null) {
        exitTimeProductionUnitProviderBuilding.Validate();
    }
}

function entranceDateProductionBuildingValueChanged(s, e) {
    if (entranceDateProductionUnitProviderBuilding.GetText() != null) {
        entranceDateProductionUnitProviderBuilding.Validate();
    }
    if (exitDateProductionUnitProviderBuilding.GetText() != null) {
        exitDateProductionUnitProviderBuilding.Validate();
    }
    if (entranceTimeProductionBuilding.GetText() != null) {
        entranceTimeProductionBuilding.Validate();
    }
}

function entranceTimeProductionUnitProviderBuildingValueChanged(s, e) {
    if (exitTimeProductionUnitProviderBuilding.GetText() != null) {
        exitTimeProductionUnitProviderBuilding.Validate();
    }
    if (entranceTimeProductionBuilding.GetText() != null) {
        entranceTimeProductionBuilding.Validate();
    }
}

function exitTimeProductionUnitProviderBuildingValueChanged(s, e) {
    if (entranceTimeProductionUnitProviderBuilding.GetText() != null) {
        entranceTimeProductionUnitProviderBuilding.Validate();
    }
    if (entranceTimeProductionBuilding.GetText() != null) {
        entranceTimeProductionBuilding.Validate();
    }
}

function entranceTimeProductionBuildingValueChanged(s, e) {
    if (entranceTimeProductionUnitProviderBuilding.GetText() != null) {
        entranceTimeProductionUnitProviderBuilding.Validate();
    }
    if (exitTimeProductionUnitProviderBuilding.GetText() != null) {
        exitTimeProductionUnitProviderBuilding.Validate();
    }
}

function exitDateProductionUnitProviderBuildingValidation(s, e) {
    // 
    if (e.value != null) {
        var exitDateProductionBuildingTmp3 = $("#exitDateProductionBuildingT").val();
        var exitDateProductionBuildingTmp2 = "";
        if (exitDateProductionBuildingTmp3 != null) {
            var tmp10 = exitDateProductionBuildingTmp3.split("/");
            if (tmp10.length == 3) {
                exitDateProductionBuildingTmp2 = tmp10[1] + "/" + tmp10[0] + "/" + tmp10[2];
            }
        }
        
        //var exitDateProductionBuildingTmp2 = exitDateProductionBuildingTmp.substring(0, 10);

        var entranceDatePUPTmp2 = entranceDateProductionUnitProviderBuilding.GetText();
        var entranceDatePUPTmp = "";
        var exitDatePUPTmp2 = exitDateProductionUnitProviderBuilding.GetText();
        var exitDatePUPTmp = "";
        var tmp = exitDatePUPTmp2.split("/");
        if (tmp.length == 3) {
            exitDatePUPTmp = tmp[1] + "/" + tmp[0] + "/" + tmp[2];
        }

        if (exitDateProductionBuildingTmp2 != null) {
            if ((new Date(exitDateProductionBuildingTmp2).getTime() > new Date(exitDatePUPTmp).getTime())) {
                e.isValid = false;
                e.errorText = "Fecha de Salida de Planta no puede ser mayor que Salida de Camaronera.";
                return;
            }
        }
        if (entranceDatePUPTmp != null) {
            var tmp2 = entranceDatePUPTmp2.split("/");
            if (tmp2.length == 3) {
                entranceDatePUPTmp = tmp2[1] + "/" + tmp2[0] + "/" + tmp2[2];
            }
            if ((new Date(entranceDatePUPTmp).getTime() > new Date(exitDatePUPTmp).getTime())) {
                e.isValid = false;
                e.errorText = "Fecha de Entrada a Camaronera no puede ser mayor que Salida de Camaronera.";
                return;
            }
        }
    }
}

function entranceDateProductionBuildingValidation(s, e) {
    if (e.value == null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        return;
    }
    if (e.value != null) {
        var exitDateProductionBuildingTmp3 = $("#exitDateProductionBuildingT").val();
        var exitDateProductionBuildingTmp2 = "";
        if (exitDateProductionBuildingTmp3 != null) {
            var tmp10 = exitDateProductionBuildingTmp3.split("/");
            if (tmp10.length == 3) {
                exitDateProductionBuildingTmp2 = tmp10[1] + "/" + tmp10[0] + "/" + tmp10[2];
            }
        }
        //var exitDateProductionBuildingTmp2 = exitDateProductionBuildingTmp.substring(0, 10);

        var entranceDatePUPTmp2 = entranceDateProductionUnitProviderBuilding.GetText();
        var entranceDatePUPTmp = "";
        var exitDatePUPTmp2 = exitDateProductionUnitProviderBuilding.GetText();
        var exitDatePUPTmp = "";
        var entranceDateProductionBuildingTmp2 = entranceDateProductionBuilding.GetText();

        var entranceDateProductionBuildingTmp = "";

        
        
        var tmp = entranceDateProductionBuildingTmp2.split("/");
        if (tmp.length == 3) {
            entranceDateProductionBuildingTmp = tmp[1] + "/" + tmp[0] + "/" + tmp[2];
        }

        if (exitDateProductionBuildingTmp2 != null) {
            if ((new Date(exitDateProductionBuildingTmp2).getTime() > new Date(entranceDateProductionBuildingTmp).getTime())) {
                e.isValid = false;
                e.errorText = "Fecha de Salida de Planta no puede ser mayor que Entrada a Camaronera.";
                UpdateTabImage(e, "tabRemissionGuideControlEntrance");
                return;
            }
        }
        if (entranceDatePUPTmp2 != null) {
            var tmp3 = entranceDatePUPTmp2.split("/");
            if (tmp3.length == 3) {
                entranceDatePUPTmp = tmp3[1] + "/" + tmp3[0] + "/" + tmp3[2];
            }
            if ((new Date(entranceDatePUPTmp).getTime() > new Date(entranceDateProductionBuildingTmp).getTime())) {
                e.isValid = false;
                e.errorText = "Fecha de Entrada a Camaronera no puede ser mayor que Entrada a Camaronera.";
                UpdateTabImage(e, "tabRemissionGuideControlEntrance");
                return;
            }
        }
        if (exitDatePUPTmp2 != null) {
            var tmp2 = exitDatePUPTmp2.split("/");
            if (tmp2.length == 3) {
                exitDatePUPTmp = tmp2[1] + "/" + tmp2[0] + "/" + tmp2[2];
            }
            if ((new Date(exitDatePUPTmp).getTime() > new Date(entranceDateProductionBuildingTmp).getTime())) {
                e.isValid = false;
                e.errorText = "Fecha de Salida de Camaronera no puede ser mayor que Entrada a Camaronera.";
                UpdateTabImage(e, "tabRemissionGuideControlEntrance");
                return;
            }
        }

    }
}
function entrancePlantaDateProductionBuildingValidation(s, e) {
    if (e.value == null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        return;
    }
    if (e.value != null) {
        var exitDateProductionBuildingTmp3 = $("#exitDateProductionBuildingT").val();
        var exitDateProductionBuildingTmp2 = "";
        if (exitDateProductionBuildingTmp3 != null) {
            var tmp10 = exitDateProductionBuildingTmp3.split("/");
            if (tmp10.length == 3) {
                exitDateProductionBuildingTmp2 = tmp10[1] + "/" + tmp10[0] + "/" + tmp10[2];
            }
        }

        // 
        var entranceDateProductionBuildingTmp2 = entranceDateProductionBuilding.GetText();

        var entranceDateProductionBuildingTmp = "";

        var tmp = entranceDateProductionBuildingTmp2.split("/");
        if (tmp.length == 3) {
            entranceDateProductionBuildingTmp = tmp[1] + "/" + tmp[0] + "/" + tmp[2];
        }
        
        if (exitDateProductionBuildingTmp2 != null) {
            if ((new Date(exitDateProductionBuildingTmp2).getTime() > new Date(entranceDateProductionBuildingTmp).getTime())) {
                e.isValid = false;
                e.errorText = "Fecha de Entrada de Planta no puede ser mayor que Salida de Planta.";
                UpdateTabImage(e, "tabRemissionGuideControlEntrance");
                return;
            }
        }

    }
}


function entranceTimeProductionUnitProviderBuildingValidation(s, e) {
    // 
    if (e.value == null) {
        //e.isValid = false;
        //e.errorText = "Campo Obligatorio";
        return;
    }
    if (e.value != null) {
        //Hora
        var exitTimeProductionBuildingTmp = $("#exitTimeProductionBuildingT").val();
        var exitTimeProductionBuildingTmp2 = exitTimeProductionBuildingTmp.substring(0, 5);
        //Fecha
        var exitDateProductionBuildingTmp3 = $("#exitDateProductionBuildingT").val();
        var exitDateProductionBuildingTmp2 = "";
        if (exitDateProductionBuildingTmp3 != null) {
            var tmp10 = exitDateProductionBuildingTmp3.split("/");
            if (tmp10.length == 3) {
                exitDateProductionBuildingTmp2 = tmp10[1] + "/" + tmp10[0] + "/" + tmp10[2];
            }
        }
        //var exitDateProductionBuildingTmp2 = exitDateProductionBuildingTmp.substring(0, 10);
        //Hora
        var entranceTimePUPTmp = entranceTimeProductionUnitProviderBuilding.GetText();
        var entranceTimePUPTmp2 = entranceTimePUPTmp.substring(0, 5);
        //Fecha
        var entranceDatePUPTmp2 = entranceDateProductionUnitProviderBuilding.GetText();
        var entranceDatePUPTmp = "";
        if (entranceDatePUPTmp2 != null) {
            var tmp = entranceDatePUPTmp2.split("/");
            if (tmp.length == 3) {
                entranceDatePUPTmp = tmp[1] + "/" + tmp[0] + "/" + tmp[2];
            }
        }

        
        if (exitTimeProductionBuildingTmp2 != null) {
            if (exitDateProductionBuildingTmp2 != null && entranceDatePUPTmp != null) {
                if ((new Date(exitDateProductionBuildingTmp2).getTime() == new Date(entranceDatePUPTmp).getTime())) {
                    var anio = exitDateProductionBuildingTmp2.substr(6, 4);
                    var mes = exitDateProductionBuildingTmp2.substr(3, 2);
                    var dia = exitDateProductionBuildingTmp2.substr(0, 2);
                    var horas = exitTimeProductionBuildingTmp2.substr(0, 2);
                    var minutos = exitTimeProductionBuildingTmp2.substr(3, 2);

                    var anio2 = entranceDatePUPTmp.substr(6, 4);
                    var mes2 = entranceDatePUPTmp.substr(3, 2);
                    var dia2 = entranceDatePUPTmp.substr(0, 2);
                    var horas2 = entranceTimePUPTmp2.substr(0, 2);
                    var minutos2 = entranceTimePUPTmp2.substr(3, 2);
                     
                    //Compara Horas y Minutos
                    if (new Date(anio, mes, dia, horas, minutos, 0).getTime() > new Date(anio2, mes2, dia2, horas2, minutos2, 0).getTime()) {
                        e.isValid = false;
                        e.errorText = "Hora de Salida de Planta no puede ser mayor que Entrada a Camaronero";
                        
                        return;
                    }
                }
            }
        }

    }
}

function exitTimeProductionUnitProviderBuildingValidation(s, e) {
    // 
    if (e.value == null) {
        //e.isValid = false;
        //e.errorText = "Campo Obligatorio";
        return;
    }
    if (e.value != null) {
        //Hora
        var exitTimeProductionBuildingTmp = $("#exitTimeProductionBuildingT").val();
        var exitTimeProductionBuildingTmp2 = exitTimeProductionBuildingTmp.substring(0, 5);
        //Fecha
        var exitDateProductionBuildingTmp3 = $("#exitDateProductionBuildingT").val();
        var exitDateProductionBuildingTmp2 = "";
        if (exitDateProductionBuildingTmp3 != null) {
            var tmp10 = exitDateProductionBuildingTmp3.split("/");
            if (tmp10.length == 3) {
                exitDateProductionBuildingTmp2 = tmp10[1] + "/" + tmp10[0] + "/" + tmp10[2];
            }
        }
        //var exitDateProductionBuildingTmp2 = exitDateProductionBuildingTmp.substring(0, 10);

        //Hora
        var entranceTimePUPTmp = entranceTimeProductionUnitProviderBuilding.GetText();
        var entranceTimePUPTmp2 = entranceTimePUPTmp.substring(0, 5);
        //Fecha
        var entranceDatePUPTmp2 = entranceDateProductionUnitProviderBuilding.GetText();


        //Hora
        var exitTimePUPTmp = exitTimeProductionUnitProviderBuilding.GetText();
        var exitTimePUPTmp2 = exitTimePUPTmp.substring(0, 5);
        //Fecha
        var exitDatePUPTmp2 = exitDateProductionUnitProviderBuilding.GetText();
        var exitDatePUPTmp = "";
        if (exitTimeProductionBuildingTmp2 != null) {
            if (exitDateProductionBuildingTmp2 != null && exitDatePUPTmp2 != null) {
                var tmp = exitDatePUPTmp2.split("/");
                if (tmp.length == 3) {
                    exitDatePUPTmp = tmp[1] + "/" + tmp[0] + "/" + tmp[2];
                }
                if ((new Date(exitDateProductionBuildingTmp2).getTime() == new Date(exitDatePUPTmp).getTime())) {
                    var anio = exitDateProductionBuildingTmp2.substr(6, 4);
                    var mes = exitDateProductionBuildingTmp2.substr(3, 2);
                    var dia = exitDateProductionBuildingTmp2.substr(0, 2);
                    var horas = exitTimeProductionBuildingTmp2.substr(0, 2);
                    var minutos = exitTimeProductionBuildingTmp2.substr(3, 2);

                    var anio2 = exitDatePUPTmp.substr(6, 4);
                    var mes2 = exitDatePUPTmp.substr(3, 2);
                    var dia2 = exitDatePUPTmp.substr(0, 2);
                    var horas2 = exitTimePUPTmp2.substr(0, 2);
                    var minutos2 = exitTimePUPTmp2.substr(3, 2);
                     
                    //Compara Horas y Minutos
                    if (new Date(anio, mes, dia, horas, minutos, 0).getTime() > new Date(anio2, mes2, dia2, horas2, minutos2, 0).getTime()) {
                        e.isValid = false;
                        e.errorText = "Hora de Salida de Planta no puede ser mayor que Salida de Camaronera.";
                        return;
                    }
                }
            }
        }
         
        if (entranceTimePUPTmp2 != null) {
            if (entranceDatePUPTmp2 != null && exitDatePUPTmp2 != null) {
                var tmp2 = exitDatePUPTmp2.split("/");
                if (tmp2.length == 3) {
                    exitDatePUPTmp = tmp2[1] + "/" + tmp2[0] + "/" + tmp2[2];
                }
                var tmp3 = entranceDatePUPTmp2.split("/");
                if (tmp3.length == 3) {
                    entranceDatePUPTmp = tmp3[1] + "/" + tmp3[0] + "/" + tmp3[2];
                }

                if ((new Date(entranceDatePUPTmp).getTime() == new Date(exitDatePUPTmp).getTime())) {
                    var anio3 = entranceDatePUPTmp.substr(6, 4);
                    var mes3 = entranceDatePUPTmp.substr(3, 2);
                    var dia3 = entranceDatePUPTmp.substr(0, 2);
                    var horas3 = entranceTimePUPTmp.substr(0, 2);
                    var minutos3 = entranceTimePUPTmp.substr(3, 2);

                    var anio4 = exitDatePUPTmp.substr(6, 4);
                    var mes4 = exitDatePUPTmp.substr(3, 2);
                    var dia4 = exitDatePUPTmp.substr(0, 2);
                    var horas4 = exitTimePUPTmp2.substr(0, 2);
                    var minutos4 = exitTimePUPTmp2.substr(3, 2);

                    if (new Date(anio3, mes3, dia3, horas3, minutos3, 0).getTime() > new Date(anio4, mes4, dia4, horas4, minutos4, 0).getTime()) {
                        e.isValid = false;
                        e.errorText = "Hora de Entrada a Camaronera no puede ser mayor que Salida de Camaronera.";
                        return;
                    }
                }
            }
        }

    }
}

function entranceTimeProductionBuildingValidation(s, e) {
    // 
    if (e.value == null) {
        e.isValid = false;
        e.errorText = "Campo Obligatorio";
        return;
    }
    if (e.value != null) {
        //Hora
        var exitTimeProductionBuildingTmp = $("#exitTimeProductionBuildingT").val();
        var exitTimeProductionBuildingTmp2 = exitTimeProductionBuildingTmp.substring(0, 5);
        //Fecha
        var exitDateProductionBuildingTmp3 = $("#exitDateProductionBuildingT").val();
        var exitDateProductionBuildingTmp2 = "";
        if (exitDateProductionBuildingTmp3 != null) {
            var tmp10 = exitDateProductionBuildingTmp3.split("/");
            if (tmp10.length == 3) {
                exitDateProductionBuildingTmp2 = tmp10[1] + "/" + tmp10[0] + "/" + tmp10[2];
            }
        }
        //var exitDateProductionBuildingTmp2 = exitDateProductionBuildingTmp.substring(0, 10);

        //Hora
        var entranceTimePUPTmp = entranceTimeProductionUnitProviderBuilding.GetText();
        var entranceTimePUPTmp2 = entranceTimePUPTmp.substring(0, 5);
        //Fecha
        var entranceDatePUPTmp2 = entranceDateProductionUnitProviderBuilding.GetText();
        var entranceDatePUPTmp = "";

        //Hora
        var exitTimePUPTmp = exitTimeProductionUnitProviderBuilding.GetText();
        var exitTimePUPTmp2 = exitTimePUPTmp.substring(0, 5);
        //Fecha
        var exitDatePUPTmp2 = exitDateProductionUnitProviderBuilding.GetText();
        var exitDatePUPTmp = "";

        //Hora
        var entranceTimeProductionBuildingTmp = entranceTimeProductionBuilding.GetText();
        var entranceTimeProductionBuildingTmp2 = entranceTimeProductionBuildingTmp.substring(0, 5);
        //Fecha
        var entranceDateProductionBuildingTmp2 = entranceDateProductionBuilding.GetText();
        var entranceDateProductionBuildingTmp = "";

        if (exitTimeProductionBuildingTmp2 != null) {
            if (exitDateProductionBuildingTmp2 != null && entranceDateProductionBuildingTmp2 != null) {
                var tmp = entranceDateProductionBuildingTmp2.split("/");
                if (tmp.length == 3) {
                    entranceDateProductionBuildingTmp = tmp[1] + "/" + tmp[0] + "/" + tmp[2];
                }
                if ((new Date(exitDateProductionBuildingTmp2).getTime() == new Date(entranceDateProductionBuildingTmp).getTime())) {
                    var anio = exitDateProductionBuildingTmp2.substr(6, 4);
                    var mes = exitDateProductionBuildingTmp2.substr(3, 2);
                    var dia = exitDateProductionBuildingTmp2.substr(0, 2);
                    var horas = exitTimeProductionBuildingTmp2.substr(0, 2);
                    var minutos = exitTimeProductionBuildingTmp2.substr(3, 2);

                    var anio2 = entranceDateProductionBuildingTmp.substr(6, 4);
                    var mes2 = entranceDateProductionBuildingTmp.substr(3, 2);
                    var dia2 = entranceDateProductionBuildingTmp.substr(0, 2);
                    var horas2 = entranceTimeProductionBuildingTmp2.substr(0, 2);
                    var minutos2 = entranceTimeProductionBuildingTmp2.substr(3, 2);
                     
                    //Compara Horas y Minutos
                    if (new Date(anio, mes, dia, horas, minutos, 0).getTime() > new Date(anio2, mes2, dia2, horas2, minutos2, 0).getTime()) {
                        e.isValid = false;
                        e.errorText = "Hora de Salida de Planta no puede ser mayor que Entrada a Planta.";
                        UpdateTabImage(e, "tabRemissionGuideControlEntrance");
                        return;
                    }
                }
            }
        }

        if (entranceTimePUPTmp2 != null) {
            if (entranceDatePUPTmp2 != null && entranceDateProductionBuildingTmp2 != null) {
                var tmp2 = entranceDateProductionBuildingTmp2.split("/");
                if (tmp2.length == 3) {
                    entranceDateProductionBuildingTmp = tmp2[1] + "/" + tmp2[0] + "/" + tmp2[2];
                }
                var tmp3 = entranceDatePUPTmp2.split("/");
                if (tmp3.length == 3) {
                    entranceDatePUPTmp = tmp3[1] + "/" + tmp3[0] + "/" + tmp3[2];
                }
                if ((new Date(entranceDatePUPTmp).getTime() == new Date(entranceDateProductionBuildingTmp).getTime())) {
                    var anio3 = entranceDatePUPTmp.substr(6, 4);
                    var mes3 = entranceDatePUPTmp.substr(3, 2);
                    var dia3 = entranceDatePUPTmp.substr(0, 2);
                    var horas3 = entranceTimePUPTmp.substr(0, 2);
                    var minutos3 = entranceTimePUPTmp.substr(3, 2);

                    var anio4 = entranceDateProductionBuildingTmp.substr(6, 4);
                    var mes4 = entranceDateProductionBuildingTmp.substr(3, 2);
                    var dia4 = entranceDateProductionBuildingTmp.substr(0, 2);
                    var horas4 = entranceTimeProductionBuildingTmp2.substr(0, 2);
                    var minutos4 = entranceTimeProductionBuildingTmp2.substr(3, 2);
                     
                    //Compara Horas y Minutos
                    if (new Date(anio3, mes3, dia3, horas3, minutos3, 0).getTime() > new Date(anio4, mes4, dia4, horas4, minutos4, 0).getTime()) {
                        e.isValid = false;
                        e.errorText = "Hora de Entrada de Camaronera no puede ser mayor que Entrada a Planta.";
                        UpdateTabImage(e, "tabRemissionGuideControlEntrance");
                        return;
                    }
                }
            }
        }

        if (exitTimePUPTmp2 != null) {
            if (exitDatePUPTmp2 != null && entranceDateProductionBuildingTmp2 != null) {
                var tmp4 = entranceDateProductionBuildingTmp2.split("/");
                if (tmp4.length == 3) {
                    entranceDateProductionBuildingTmp = tmp4[1] + "/" + tmp4[0] + "/" + tmp4[2];
                }
                var tmp5 = exitDatePUPTmp2.split("/");
                if (tmp5.length == 3) {
                    exitDatePUPTmp = tmp5[1] + "/" + tmp5[0] + "/" + tmp5[2];
                }
                if ((new Date(exitDatePUPTmp).getTime() == new Date(entranceDateProductionBuildingTmp).getTime())) {
                    var anio5 = exitDatePUPTmp.substr(6, 4);
                    var mes5 = exitDatePUPTmp.substr(3, 2);
                    var dia5 = exitDatePUPTmp.substr(0, 2);
                    var horas5 = exitTimePUPTmp.substr(0, 2);
                    var minutos5 = exitTimePUPTmp.substr(3, 2);

                    var anio6 = entranceDateProductionBuildingTmp.substr(6, 4);
                    var mes6 = entranceDateProductionBuildingTmp.substr(3, 2);
                    var dia6 = entranceDateProductionBuildingTmp.substr(0, 2);
                    var horas6 = entranceTimeProductionBuildingTmp2.substr(0, 2);
                    var minutos6 = entranceTimeProductionBuildingTmp2.substr(3, 2);
                     
                    //Compara Horas y Minutos
                    if (new Date(anio5, mes5, dia5, horas5, minutos5, 0).getTime() > new Date(anio6, mes6, dia6, horas6, minutos6, 0).getTime()) {
                        e.isValid = false;
                        e.errorText = "Hora de Salida de Camaronera no puede ser mayor que Entrada a Planta.";
                        UpdateTabImage(e, "tabRemissionGuideControlEntrance");
                        return;
                    }
                }
            }
        }
    }
}