-- Parametro Parametrizar para que codificaci�n por l�nea de inventario

insert into setting 
(	name
	,code
	,value
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
(
 'Par�metro c�digo master por l�nea de inventario',
 'PGCMLI',
  null,
  (select top 1 id from settingDataType where code = 'LTXT'),
  (select top 1 id   from module where code ='MAN'),
  (select top 1 id    from Company where code = '02'),
  1,
  1,
  '19-07-2021',
  1,
  '19-07-2021',
  'Par�metro generaci�n c�digo master por l�nea de inventario'
);
 

insert into SettingDetail
(
 id_setting
 ,value
 ,valueAux
 ,id_userCreate
 ,dateCreate
 ,id_userUpdate
 ,dateUpdate
)
values( (select top 1 id from setting where code ='PGCMLI' ),'PT','0', 1,'19-07-2021', 1,'19-07-2021' ),
    ( (select top 1 id from setting where code ='PGCMLI' ),'PP','0', 1,'19-07-2021', 1,'19-07-2021' );

