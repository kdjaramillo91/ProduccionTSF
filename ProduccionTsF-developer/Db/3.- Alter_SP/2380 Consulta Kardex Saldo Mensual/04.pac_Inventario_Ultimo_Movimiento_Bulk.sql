
/****** Object:  StoredProcedure [dbo].[pac_Inventario_Ultimo_Movimiento]    Script Date: 3/7/2024 10:11:06 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create or alter procedure [dbo].[pac_Inventario_Ultimo_Movimiento_Bulk]
	@str_item_inf varchar(Max),
	@dt_emissionDate varchar(10),
	@dt_houremissionDate varchar(8)
as
set nocount on 

declare @dt datetime
 
 declare @tempData	table
(
	id int not null, 
	id_item int not null, 
	id_inventoryMove int not null, 	
	id_warehouse int not null, 
	id_warehouselocation int not null
)

declare @tmpResult	table
(
	[IdItem]					int not null,
	[Id_warehouse]				int not null, 
	[Id_warehouselocation]		int not null,
	[IdInventoryMoveDetail]		int not null
)
set @dt =  convert(datetime, @dt_emissionDate + ' ' + @dt_houremissionDate)

	insert into   @tempData
	select	imd.id, 
			par.id_item, 
			imd.id_inventoryMove, 			
			par.id_warehouse, 
			par.id_warehouselocation	
	from InventoryMoveDetail imd WITH (NOLOCK)
	inner join 
	(
		SELECT	id_item,
				id_warehouse,
				id_warehouselocation
		FROM OPENJSON(@str_item_inf) 
		WITH 
		(
			id_item					INT '$.id_item',
			id_warehouse			INT '$.@id_warehouse',
			id_warehouselocation	INT '$.@@id_warehouselocation'
	
		)AS jsonValues
		group by 
		id_warehouse,
		id_warehouselocation,
		id_item
	) par
	on imd.[id_item] = par.id_item 
	and imd.[id_Warehouse] = par.id_warehouse
	and imd.[id_warehouseLocation] = par.id_warehouselocation
	where imd.[id_lot] is null
	and isnull(imd.[inMaximumUnit],0) = 0 

	insert into @tmpResult
	select  id_item,id_warehouse, id_warehouselocation,  max(imd.id)
	from @tempData imd
	inner join Document d WITH (NOLOCK) on imd.id_inventoryMove = d.id
	Where d.id_documentState in (select id from DocumentState where [code] in ( '03','16'))
	and @dt >= d.emissiondate   
	group by id_item, id_warehouse, id_warehouselocation



	select * from @tmpResult



/*
[dbo].[pac_Inventario_Ultimo_Movimiento] '617,12,14,;','2023-06-06','12:14:55'
*/










