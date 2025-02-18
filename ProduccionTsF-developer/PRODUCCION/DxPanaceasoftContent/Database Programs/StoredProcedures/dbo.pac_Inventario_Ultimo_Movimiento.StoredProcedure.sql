If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'pac_Inventario_Ultimo_Movimiento'
	)
Begin
	Drop Procedure dbo.pac_Inventario_Ultimo_Movimiento
End
Go
Create Procedure dbo.pac_Inventario_Ultimo_Movimiento
(
	@str_item_inf varchar(8000),
	@dt_emissionDate varchar(10),
	@dt_houremissionDate varchar(8)
)
As
set nocount on 

declare @dt datetime
declare @pos	INTEGER
declare @i_id integer
declare @posInside	INTEGER
declare @str_item_ind varchar(8000)
declare @str_item_ind_res varchar(8000)
declare @count integer

declare @id_item integer
declare @id_warehouse integer
declare @id_warehouselocation integer

-- 1.- Desconcateno Variables
/*
*/

--declare @tmpListaItems	table
--(
--	[IdItem]				int not null,
--	[IdWarehouse]			int not null,
--	[IdWarehouseLocation]	int not null
--)

declare @tmpResult	table
(
	[IdItem]				int not null,
	[IdInventoryMoveDetail]			int not null
)
set @dt =  convert(datetime, @dt_emissionDate + ' ' + @dt_houremissionDate)


set @count = 1
WHILE CHARINDEX(';',@str_item_inf) > 0
BEGIN
	SET @pos = CHARINDEX(';',@str_item_inf)
	SET @str_item_ind = SUBSTRING(@str_item_inf,1,@pos-1)
	SET @str_item_inf = SUBSTRING(@str_item_inf,@pos+1,LEN(@str_item_inf))


	while CHARINDEX(',',@str_item_ind) > 0
	begin
		SET @posInside = CHARINDEX(',',@str_item_ind)
		SET @str_item_ind_res = SUBSTRING(@str_item_ind,1,@posInside-1)
		SET @str_item_ind = SUBSTRING(@str_item_ind,@posInside+1,LEN(@str_item_ind))

		if (@count = 1) 
		begin
			set @id_item = convert(integer,@str_item_ind_res)
		end
		else if (@count = 2)
		begin
			set @id_warehouse = convert(integer,@str_item_ind_res)
		end
		else if (@count = 3)
		begin
			set @id_warehouselocation = convert(integer,@str_item_ind_res)
		end
		set @count = @count + 1
	end
	set @count = 1
	--select @id_item,@id_warehouse,@id_warehouselocation
	insert into @tmpResult
	select top 1 @id_item, imd.[id] as [id_InventoryMoveDetail]
	from [dbo].[InventoryMoveDetail] imd
	join [dbo].[InventoryMove] im on imd.[id_inventoryMove] = im.[id] 
	join [dbo].[Document] do on im.[id] = do.[id]
	join [dbo].[DocumentState] dos on do.[id_documentState] = dos.[id] and dos.[code] = '03' 
	where imd.[id_lot] is null
	and imd.[id_item] = @id_item
	and im.[idWarehouse] = @id_warehouse
	and imd.[id_warehouseLocation] = @id_warehouselocation
	and @dt >= do.emissiondate      
	and isnull(imd.[inMaximumUnit],0) =0 
	order by do.emissionDate desc, imd.dateCreate desc
END

select * from @tmpResult



/*
[dbo].[pac_Inventario_Ultimo_Movimiento] '1353,12,4,;1324,4,5,;2345,43,35;','2018-07-13','16:00:55'
*/











Go
