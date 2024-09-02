-- Parametro Lista de Precios VIsualizar Columna de COmiin y Referencia

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
 'Visualizar Solo Precios',
 'VWPP',
  1,
  (select top 1 id  from settingDataType where code = 'ENT'),
  (select top 1 id   from module where code ='COM'),
  (select top 1 id    from Company where code = '01'),
  1,
  1,
  '16-07-2021',
  1,
  '16-07-2021',
  'Parametro para visualizar solo las columnas de Precios, No Comisiones ni Referencia'
);
