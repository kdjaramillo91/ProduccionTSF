----------------------------------------------------------------------
-- Tarea: Requerimiento permitir que en la liquidacion de la guia 
-- se acepten varios proveedores 
-- Implementacion: Expotuna
-- Responsable: Ronny Avilés Morales
-- Fecha Entrega: 2023-10-02
-- Descripcion: Parametro para que se pueda aceptar varios proveedores en 
-- en la misma guia. 
----------------------------------------------------------------------

IF NOT EXISTS (SELECT * FROM setting where code ='LIQANYPROV' )
begin
	declare @isActivo int = 1;
	declare @userId int = 1;

	insert into setting
	(name
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
	  'Liquidacion Guia,permite distintos Proveedores',
	  'LIQANYPROV',
	  'S',
	  (select  top 1 Id from SettingDataType where code = 'TXT'),
	  (select  top 1 Id from Module where code = 'LOG'),
	  (select  top 1 Id from  Company where isActive = 1),
	  @isActivo,
	  @userId,
	  GETDATE(),
	  @userId,
	  GETDATE(),
	  'Liquidacion Guia, permite elegir distintos Proveedores de Transporte'
	);
end;
