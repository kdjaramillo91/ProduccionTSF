--USE [PanaceaListaPrecio]
--GO
/****** Object:  StoredProcedure [dbo].[par_ListaPreciosDestinoClase3CR]    Script Date: 10/08/2021 23:14:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

alter procedure [dbo].[par_ListaPreciosDestinoClase3CR_Dest] 
	@id int 
as
set nocount on

declare  @id_classTodos int = (select top 1 id from ClassShrimp where code ='D0' ); 

select  f.id,
		NombreTalla,
		[CHINA], 
		[EUROPA],
		[TODOS],
		[AMERICA],
		[ASIA],
		[OTROS],
		f.PrecioReferencial,
		f.EsCotizacion
from 
(

    select  g.id, 
			h.id_class,
			g.name NombreTalla,
			g.orderSize
	from ItemSize g 
		 inner join  ItemSizeProcessPLOrder h 
		 on h.id_ItemSize = g.id 
	     inner join ProcessType i 
		 on i.id = h.id_ProcessType
		 inner join Class d on 
		 d.id =  h.id_class
	 where i.code = 'COL'
		 and d.code = 'BROK'	

) g
left join 
(
	select	
			[CHINA], 
			[EUROPA],
			[TODOS],
			[AMERICA],
			[ASIA],
			[OTROS],
			Id_Itemsize,
			id_class,
			PrecioReferencial,
			EsCotizacion,
			id
	
	from 
	(
			select b.id,
				   c.name,
				   a.price,
				   Isnull(pldb.[price],0) as PrecioReferencial,
				   b.isQuotation as EsCotizacion, 
				   a.Id_Itemsize,
				   a.id_class

			from   PriceList b inner join 
				   PriceListItemSizeDetail a 
				   on a.Id_PriceList = b.id
				   inner join ClassShrimp c on 
				   a.id_classShrimp = c.id
				   inner join Class d on 
				   d.id =  a.id_class
				   ------------------------
				   -- Precio Referencial
				   ------------------------
				   left outer join [dbo].[PriceListItemSizeDetail] pldb 					on  case 					when b.isQuotation = 1 then b.[id_priceListBase] 					else b.[id] end = pldb.[Id_PriceList] 					and pldb.[id_ItemSize] = a.[id_ItemSize]					and pldb.id_class = a.id_class 
					and pldb.id_classShrimp =  @id_classTodos
					
			where b.id =  @id 
				  and d.code = 'BROK' --'PRIM'	
	
	)
	AS SourceTable PIVOT
	( SUM(price)
	 FOR name in
	  ([CHINA], 
	   [EUROPA],
	   [TODOS],
	   [AMERICA],
	   [ASIA],
	   [OTROS]
	   )
	) AS PivotTable

) f  
on  g.id = f.Id_Itemsize
and g.id_class = f.id_class
order by g.orderSize;-- exec par_ListaPreciosDestinoClase3CR 1792-- exec par_ListaPreciosDestinoClase3CR_Dest 1792