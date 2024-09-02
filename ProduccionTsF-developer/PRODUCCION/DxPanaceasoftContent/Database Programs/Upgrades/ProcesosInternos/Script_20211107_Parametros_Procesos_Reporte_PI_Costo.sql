-- Parametros Procesos que se visualizan en el Reporte de Movimientos de P I con Costo
insert into Setting
(name
,code
,id_settingDataType
,id_module
,id_company
,isActive
,id_userCreate
,dateCreate
,id_userUpdate
,dateUpdate
,description)
values
( 'Procesos en Reporte de Movimientos de PI',
  'PRMPI',
  (select top 1 id from SettingDatatype where code = 'LTXT'),
  (select top 1 id from  MOdule  where code = 'PRO'),
  2,
  1,
  1,
  GETDATE(),
  1,
  GETDATE(),
  'Procesos en Reporte de Movimientos de PI con Costo'
);

insert into SettingDetail
(id_setting
,value
,valueAux
,id_userCreate
,dateCreate
,id_userUpdate
,dateUpdate)
values
((select top 1 id from Setting where code = 'PRMPI'), 'VAG',0,1,GETDATE(),1,GETDATE()),
((select top 1 id from Setting where code = 'PRMPI'), 'PRO',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'DES',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'RPC',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'RPL',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'IQS',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'COC',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'ETQ',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'REE',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'CNG',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'RRP',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'RRC',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'RRL',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'FRE',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'PEL',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'COT',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'FRV',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'TRA',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'DEG',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'DEC',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'DEV',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'DEE',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'GLA',0,1,GETDATE(),1,GETDATE()),
 ((select top 1 id from Setting where code = 'PRMPI'), 'RET',0,1,GETDATE(),1,GETDATE())
  