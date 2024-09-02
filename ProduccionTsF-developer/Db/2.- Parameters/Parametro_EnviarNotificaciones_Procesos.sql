-- PARAMETRO REQUERIDO INGRESO CONTACTO CLIENTE DEL EXTERIOR 

-- PROEXPO
insert into Setting(name,code,value,id_settingDataType,id_module,id_company,isActive,id_userCreate,dateCreate, id_userUpdate, dateUpdate,description)
values ('Notificacion de Procesos', 'NOTPROC','SI',1,5,1,1,1,GETDATE(),1,GETDATE(),'');


-- OTRAS IMPLEMENTACIONES
insert into Setting(name,code,value,id_settingDataType,id_module,id_company,isActive,id_userCreate,dateCreate, id_userUpdate, dateUpdate,description)
values ('Notificacion de Procesos', 'NOTPROC','NO',1,5,1,1,1,GETDATE(),1,GETDATE(),'');


