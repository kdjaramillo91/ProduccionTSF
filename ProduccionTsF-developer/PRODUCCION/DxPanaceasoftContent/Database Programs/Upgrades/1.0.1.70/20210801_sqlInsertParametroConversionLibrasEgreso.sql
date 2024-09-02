-- Parametro Parametrizar que Razon de Movimiento toma el valor de configuracion de conversion
-- y acepta redondeo o no

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
 'Parámetro Razon Inventario Egreso Factor Item',
 'PRIEF',
  null,
  (select top 1 id from settingDataType where code = 'LTXT'),
  (select top 1 id   from module where code ='MAN'),
  (select top 1 id    from Company where code = '02'),
  1,
  1,
  '27-07-2021',
  1,
  '27-07-2021',
  'Parámetro Razon Inventario Egreso Factor Item'
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
values( (select top 1 id from setting where code ='PRIEF' ),'MBPT005','0', 1,'27-07-2021', 1,'27-07-2021' );

