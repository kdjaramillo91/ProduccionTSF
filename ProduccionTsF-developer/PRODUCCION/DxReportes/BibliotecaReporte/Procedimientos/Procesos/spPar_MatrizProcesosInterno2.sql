GO
/****** Object:  StoredProcedure [dbo].[Par_ProcesosInternos]    Script Date: 01/03/2023 01:26:51 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[spPar_MatrizProcesosInterno2]
@id int ,
@fechaInicio DATETIME,
@fechaFin Datetime
as

set nocount on
create table #TEMRPTUDP (
	[MPROLIQ] [VARCHAR] (25) NOT NULL,--
	[PRODUNnameNombreUnidadLoteProduccion] [varchar](150)NOT NULL,
	[PRODPRnameProcesoLoteProduccion][varchar](150)NOT NULL,
	[PRODLreceptionDateFechaRecepcion] [datetime]NOT NULL,
	[PRODLnumberNumerodeLote] [varchar](20) NOT NULL,
	[PRODLtotalQuantityRecivedTotalCantidadRecibidaLibras] [decimal](14, 6) NOT NULL,
	[PRODLinternalNumberNumeroInterno]  [varchar](20) NOT NULL,
	[PRODLtotalQuantityRemittedTCLREM] [decimal](14, 6) NOT NULL,
	[WAREnameBodega]  [varchar](50) NOT NULL,--9
	[WARELOCnameUbicacionBodega]  [varchar](250) NOT NULL,--10
	----------------------------------------------------
	[RENDIMIENTO] [decimal](14, 6) NOT NULL,
	[LOTORInumberLoteOrigen]   [varchar](20) NOT NULL,
	[ITEMmasterCodeCodigoProducto] [varchar](50) NOT NULL,
	[ITEMnameNombreItem] [varchar](150) NOT NULL,
	[ITEMSZnameTalla] [varchar](50) NOT NULL,
	[PRODLDquantityRecivedCantidadRecibidaDetalle] [decimal](14, 6) NOT NULL,--16
	[METRICcodeUnidadMedida] [varchar](20) NOT NULL,--17
	[ITY2nameliqTipoProducto] [varchar](50) NOT NULL,
	[tmpquantityPoundsLiquidationliqlibrastotalrecobidas] [decimal](18, 6) NULL,--19
	[PRODLDescriptionObservaciones] [varchar](250) NULL,
	----------------------------------------------
	PLtotalQuantityTrash decimal (17,2) null,
	PLLibrasNetas decimal(17,2)NULL
	,PLRendimiento decimal(17,2)NULL
	,PLMerma decimal (17,2)NULL
	,logo image
	,NDocumento varchar(100)
	,LBMerma decimal(17,5)
	-----------------DEPERDICIO---------------------
	--,DespProducto varchar(350)
	--,DespUM varchar(50)
	--,DespBodega varchar(350)
	--,DespUbicacion varchar(350)
	--,DespCantidad decimal(17,2)
	)
	----------------------------------------------------------------------------
	--[masterCodeliqCodigoProducto] [varchar](50) NOT NULL,
	--[it2nameLiqProducto] [varchar](150) NOT NULL,
	--[itz2nameliqTalla] [varchar](50) NOT NULL,
	--[MT2codeliqUnidadMedida] [varchar](20) NOT NULL,
	--[ITY2nameliqTipoProducto] [varchar](50) NOT NULL,
	--[tmpquantityliqCantidad]  [decimal](14, 6) NOT NULL,
	--[tmpquantityPoundsLiquidationliqlibrastotalrecobidas] [decimal](18, 6) NULL,)

	insert into #TEMRPTUDP
select 
'MATERIA PRIMA' as Estado,--EGRESO
Cast (PRODUN.name as varchar),
Cast (PRODPR.name as varchar),
PRODL.receptionDate,
PRODL.number,
PRODL.totalQuantityRecived,
PRODL.internalNumber,
PRODL.totalQuantityLiquidation,
Isnull(WARE.name,''),
Isnull(WARELOC.name,''),
((PRODL.totalQuantityLiquidation/PRODL.totalQuantityRecived-prodl.totalQuantityTrash)*100),
-----------------------------------------------------------------
isnull(LOTORI.internalNumber,''),
ITEM.masterCode,
ITEM.name,
ITEMSZ.name ,
case when METRIC.id = 1 then PRODLD.quantityRecived else 
PRODLD.quantityRecived *2.2046 end as quantityrecived,
METRIC.code,-------------------------------------
ITY2.name as liqTipoProducto,

(isnull(fact.factor,1 )  *  PRODLD.quantityRecived )  as liqlibrastotalrecobidas,
PRODL.description
----------------------------------------------------------------------
,PRODL.totalquantitytrash as PLtotalQuantityTrash
,PRODL.totalQuantityRecived - PRODL.totalquantitytrash  as PLLibrasNetas
,(prodl.totalQuantityLiquidation / (PRODL.totalQuantityRecived - PRODL.totalquantitytrash))*100 as Rendimiento---------------------------------
,(((prodl.totalQuantityLiquidation / (PRODL.totalQuantityRecived - PRODL.totalquantitytrash))*100) - 100) *-1 as Merma
,cia.logo as Logo


,

(SELECT top 1
 b.name +'   - N.- ' +convert(varchar,a.sequential)  from inventorymove a
inner join InventoryReason b on b.id = a.id_inventoryReason
left join DocumentSource c on c.id_document = b.id
left join document d on d.id = a.id
where a.id_productionLot = @id and b.id  in(6)) as  NDOCUMENTO

,(select
case when d.id_presentation is null then
a.quantity  
else
(case when 
a.id_metricUnit =4  then 
a.quantity*e.maximum*e.minimum
else 
(a.quantity*e.maximum*e.minimum)*2.2046
end)
 end 
cantidad 
from productionlotloss a 
left join productionlot b on b.id = a.id_productionLot
--inner join MetricUnit c on c.id = a.id_metricUnit
left join item d on d.id = a.id_item
left join Presentation e on e.id = d.id_presentation
where a.id_productionLot = @id) as LibrasMerma

---------------------------------------
--,despit.name as ProductoDesperdicio
--,despmu.name as UMDesperdicio
--,despbod.name as BodegaDesperdicio
--,ubidesp.name as UbicacionDesperdicio
--,desp.quantity as CantidadDesperdicio


--select *from productionlot where id = 845897

from ProductionLot PRODL 
inner join ProductionLotState PRODLS on 
PRODL.id_ProductionLotState = PRODLS.id
inner join ProductionUnit PRODUN on 
PRODUN.id = PRODL.id_productionUnit 
inner join ProductionProcess PRODPR on
PRODPR.id = PRODL.id_productionProcess
left outer join person PROVID on
PROVID.id = PRODL.id_provider
left outer join Person BUYER on
BUYER.id = PRODL.id_buyer
left outer join Person REQUEST on
REQUEST.id = PRODL.id_personRequesting
left outer join Person RECIEVER on
RECIEVER.id = PRODL.id_personReceiving
inner join Company COMP on
COMP.id = PRODL.id_company
--------inner join ProcessType PROCTY on
--------PROCTY.id = PRODL.id_processtype
left outer join Person PERSPROPL on
PERSPROPL.id = PRODL.id_personProcessPlant
left outer join Person USERAUTH on
USERAUTH.id = PRODL.id_userAuthorizedBy
left outer join Certification CERTI on
CERTI.id = PRODL.id_certification
-----------------------------------------
inner join ProductionLotDetail PRODLD on
PRODLD.id_productionLot = PRODL.id 
inner join item ITEM on
ITEM.id = PRODLD.id_item
inner join ItemInventory iin on
iin.id_item = ITEM.id
inner join MetricUnit mu02 on 
mu02.id = iin.id_metricUnitInventory
Left outer join Warehouse WARE on
WARE.id = PRODPR.id_warehouse
Left outer join WarehouseLocation WARELOC on
WARELOC.id = PRODPR.id_warehouseLocation
left outer join 
( select d.id, 
	     case 
		   when isnull(d.internalNumber,'') != '' then d.internalNumber
		   else 
		     (case when isnull(f.internalNumber,'') != '' then f.internalNumber else '' end)
	     end as  internalNumber       
  from Lot d 
  left outer join ProductionLot f 
  on f.id = d.id  ) as LOTORI on
LOTORI.id = PRODLD.id_originLot
left outer join Presentation PRESEN on
PRESEN.id = ITEM.id
left outer join( 
		select (a.minimum * a.maximum) * 
				(case when b.code ='Kg' then 2.20462
				when b.code ='Lbs' then 1
				end) factor, a.id
		from Presentation a inner join MetricUnit b on a.id_metricUnit = b.id
) as fact on  fact.id = ITEM.id_presentation

inner join ItemGeneral ITGEN on 
ITGEN.id_item = ITEM.id
left outer join ItemSize ITEMSZ on
ITEMSZ.ID = ITGEN.id_size
inner join ItemType ITY2 on
ITY2.id = ITEM.id_itemType
inner  join Presentation PRESE on
PRESE.id = ITEM.id_presentation
inner join MetricUnit METRIC on
METRIC.id = PRESE.id_metricUnit
left join document doc on doc.id = PRODL.id
left join EmissionPoint ep on ep.id = doc.id_emissionPoint
left join Company cia on cia.id = ep.id_company
------------------------DESP-------------------
LEFT JOIN ProductionLotTrash desp on desp.id_productionLot = prodl.id
left join item despit on despit.id = desp.id_item
left join ItemGeneral despge on despge.id_item = desp.id_item
left join itemsize despsize on despsize.id = despge.id_item
left join Presentation desppre on desppre.id = despit.id_presentation
left join MetricUnit despmu on despmu.id = desp.id_metricUnit
left join warehouse despbod on despbod.id = desp.id_warehouse
left join WarehouseLocation ubidesp on ubidesp.id = desp.id_warehouseLocation
--------------------MERMA---------------------------------------
LEFT JOIN ProductionLotLoss merma on merma.id_productionLot = prodl.id
left join item mermait on mermait.id = merma.id_item
left join ItemGeneral mermage on mermage.id_item = merma.id_item
left join ItemSize mermasize on mermasize.id = mermage.id_size
left join Presentation mermapre on mermapre.id = mermait.id_presentation
left join MetricUnit mermamu on mermamu.id = merma.id_metricUnit
left join warehouse mermabod on mermabod.id = merma.id_warehouse
left join WarehouseLocation mermaubi on mermaubi.id = merma.id_warehouseLocation

--left join documentsource ds on ds.id_documentOrigin = PRODL.id 
--left join inventorymove mpe on mpe.id = ds.id_document and mpe.id_inventoryReason = 6
--left join document docim on docim.id = mpe.id 

where PRODL.id = @id







UNION ALL
Select 
'LIQUIDACION' as Estado,  ----INGRESO
Cast (PRODUN.name as varchar),
Cast (PRODPR.name as varchar),
PRODL.receptionDate,
PRODL.number,
PRODL.totalQuantityRecived,
PRODL.internalNumber,
PRODL.totalQuantityRemitted,
Isnull(WARE.name,''),
Isnull(WARELOC.name,''),
((PRODL.totalQuantityLiquidation/PRODL.totalQuantityRecived-prodl.totalQuantityTrash)*100),
-----------------------------------------------------------------
isnull(PRODL.internalNumber,''),
It2.masterCode as liqCodigoProducto,
it2.name as LiqProducto,
itz2.name as liqTalla,

tmp.quantity as liqCantidad,-------------------------------------------------
MT2.code as liqUnidadMedida,

ITY2.name as liqTipoProducto,

tmp.quantityPoundsLiquidation as liqlibrastotalrecobidas,
PRODL.description
-------------------------------------
,PRODL.totalquantitytrash as PLtotalQuantityTrash
,PRODL.totalQuantityRecived - PRODL.totalquantitytrash  as PLLibrasNetas
,(prodl.totalQuantityLiquidation / prodl.totalQuantityRecived-prodl.totalQuantityTrash)*100 as Rendimiento
,(((prodl.totalQuantityLiquidation / prodl.totalQuantityRecived)*100) - 100) *-1 as Merma
,cia.logo as Logo

,

(SELECT top 1
 b.name +'   - N.- ' +convert(varchar,a.sequential)  from inventorymove a
inner join InventoryReason b on b.id = a.id_inventoryReason
left join DocumentSource c on c.id_document = b.id
left join document d on d.id = a.id
where a.id_productionLot = @id and b.id  in(4)) as NDOCUMENTO

,(select
case when d.id_presentation is null then
a.quantity  
else
(case when 
a.id_metricUnit =4  then 
a.quantity*e.maximum*e.minimum
else 
(a.quantity*e.maximum*e.minimum)*2.2046
end)
 end 
cantidad 
from productionlotloss a 
left join productionlot b on b.id = a.id_productionLot
--inner join MetricUnit c on c.id = a.id_metricUnit
left join item d on d.id = a.id_item
left join Presentation e on e.id = d.id_presentation
where a.id_productionLot = @id) as LibrasMerma
-------------------------------------------
--,despit.name as ProductoDesperdicio
--,despmu.name as UMDesperdicio
--,despbod.name as BodegaDesperdicio
--,ubidesp.name as UbicacionDesperdicio
--,desp.quantity as CantidadDesperdicio
from ProductionLot PRODL 
inner join ProductionLotState PRODLS on 
PRODL.id_ProductionLotState = PRODLS.id
inner join ProductionUnit PRODUN on 
PRODUN.id = PRODL.id_productionUnit 
inner join ProductionProcess PRODPR on
PRODPR.id = PRODL.id_productionProcess
left outer join person PROVID on
PROVID.id = PRODL.id_provider
left outer join Person BUYER on
BUYER.id = PRODL.id_buyer
left outer join Person REQUEST on
REQUEST.id = PRODL.id_personRequesting
left outer join Person RECIEVER on
RECIEVER.id = PRODL.id_personReceiving
inner join Company COMP on
COMP.id = PRODL.id_company
-------------------------------------
Left outer join Warehouse WARE on
WARE.id = PRODPR.id_warehouse
Left outer join WarehouseLocation WARELOC on
WARELOC.id = PRODPR.id_warehouseLocation
-------------------------------------
inner join  ProductionLotLiquidation TMP on
TMP.id_productionLot = PRODL.id
inner join Item It2 on
it2.id = Tmp.id_item

INNER JOIN MetricUnit MUT ON 
MUT.id = TMP.id_metricUnit

inner join ItemType ITY2 on
ITY2.id = It2.id_itemType
inner join Presentation pres2 on
pres2.id = it2.id_presentation
inner join MetricUnit MT2 on
MT2.id = TMP.id_metricUnit
inner join ItemGeneral itg2 on
itg2.id_item = it2.id
inner join ItemSize itz2 on
itz2.id = itg2.id_size
left join document doc on doc.id = PRODL.id
left join EmissionPoint ep on ep.id = doc.id_emissionPoint
left join Company cia on cia.id = ep.id_company
--------------------------------------------------
LEFT JOIN ProductionLotTrash desp on desp.id_productionLot = prodl.id
left join item despit on despit.id = desp.id_item
left join ItemGeneral despge on despge.id_item = desp.id_item
left join itemsize despsize on despsize.id = despge.id_item
left join Presentation desppre on desppre.id = despit.id_presentation
left join MetricUnit despmu on despmu.id = desp.id_metricUnit
left join warehouse despbod on despbod.id = desp.id_warehouse
left join WarehouseLocation ubidesp on ubidesp.id = desp.id_warehouseLocation
--------------------MERMA---------------------------------------
LEFT JOIN ProductionLotLoss merma on merma.id_productionLot = prodl.id
left join item mermait on mermait.id = merma.id_item
left join ItemGeneral mermage on mermage.id_item = merma.id_item
left join ItemSize mermasize on mermasize.id = mermage.id_size
left join Presentation mermapre on mermapre.id = mermait.id_presentation
left join MetricUnit mermamu on mermamu.id = merma.id_metricUnit
left join warehouse mermabod on mermabod.id = merma.id_warehouse
left join WarehouseLocation mermaubi on mermaubi.id = merma.id_warehouseLocation
--left join documentsource ds on ds.id_documentOrigin = PRODL.id 
--left join inventorymove mpe on mpe.id = ds.id_document and mpe.id_inventoryReason = 4
--left join document docim on docim.id = mpe.id 

where PRODL.id = @id	
 

 
UNION ALL
Select 
'DESPERDICIO' as Estado,
Cast (PRODUN.name as varchar),
Cast (PRODPR.name as varchar),
PRODL.receptionDate,
PRODL.number,
PRODL.totalQuantityRecived,
PRODL.internalNumber,
--PRODL.totalQuantityRemitted,
ISNULL(merma.quantity,0),
Isnull(despbod.name,''),
Isnull(ubidesp.name,''),
((PRODL.totalQuantityLiquidation/PRODL.totalQuantityRecived-prodl.totalQuantityTrash)*100),
-----------------------------------------------------------------
isnull(PRODL.internalNumber,''),
isnull(despit.masterCode,'') as liqCodigoProducto,
isnull(despit.name,'') as LiqProducto,
isnull(despsize.name,'') as liqTalla,
ISNULL(desp.quantity,0) as liqCantidad,--16
ISNULL(despmu.code,'') as liqUnidadMedida,


'',--ITY2.name as liqTipoProducto,

--tmp.quantityPoundsLiquidation as liqlibrastotalrecobidas,
case when despit.id_presentation is null then 
desp.quantity else 
case when despmu.id = 4 then 
ISNULL((desp.quantity * desppre.minimum*desppre.maximum ),0)
else  
ISNULL((desp.quantity * desppre.minimum*desppre.maximum )*2.2046,0) end end
as liqlibrastotalrecobidas,




PRODL.description
-------------------------------------
,PRODL.totalquantitytrash as PLtotalQuantityTrash
,PRODL.totalQuantityRecived - PRODL.totalquantitytrash  as PLLibrasNetas
,(prodl.totalQuantityLiquidation / prodl.totalQuantityRecived-prodl.totalQuantityTrash)*100 as Rendimiento
,(((prodl.totalQuantityLiquidation / prodl.totalQuantityRecived)*100) - 100) *-1 as Merma
,cia.logo as Logo
,(SELECT  top 1
 b.name +'   - N.- ' +convert(varchar,a.sequential)  from inventorymove a
inner join InventoryReason b on b.id = a.id_inventoryReason
left join DocumentSource c on c.id_document = b.id
left join document d on d.id = a.id
where a.id_productionLot = @id and b.id  in(5)
--order by a.sequential desc
)  AS NDOCUMENTO

,(select
case when d.id_presentation is null then
a.quantity  
else
(case when 
a.id_metricUnit =4  then 
a.quantity*e.maximum*e.minimum
else 
(a.quantity*e.maximum*e.minimum)*2.2046
end)
 end 
cantidad 
from productionlotloss a 
left join productionlot b on b.id = a.id_productionLot
--inner join MetricUnit c on c.id = a.id_metricUnit
left join item d on d.id = a.id_item
left join Presentation e on e.id = d.id_presentation
where a.id_productionLot = @id) as LibrasMerma

---------------------------------------------
--,despit.name as ProductoDesperdicio
--,despmu.name as UMDesperdicio
--,despbod.name as BodegaDesperdicio
--,ubidesp.name as UbicacionDesperdicio
--,desp.quantity as CantidadDesperdicio
from ProductionLot PRODL 
inner join ProductionLotState PRODLS on 
PRODL.id_ProductionLotState = PRODLS.id
inner join ProductionUnit PRODUN on 
PRODUN.id = PRODL.id_productionUnit 
inner join ProductionProcess PRODPR on
PRODPR.id = PRODL.id_productionProcess
left outer join person PROVID on
PROVID.id = PRODL.id_provider
left outer join Person BUYER on
BUYER.id = PRODL.id_buyer
left outer join Person REQUEST on
REQUEST.id = PRODL.id_personRequesting
left outer join Person RECIEVER on
RECIEVER.id = PRODL.id_personReceiving
inner join Company COMP on
COMP.id = PRODL.id_company
-------------------------------------
Left outer join Warehouse WARE on
WARE.id = PRODPR.id_warehouse
Left outer join WarehouseLocation WARELOC on
WARELOC.id = PRODPR.id_warehouseLocation
-------------------------------------
--inner join  ProductionLotLiquidation TMP on
--TMP.id_productionLot = PRODL.id
--inner join Item It2 on
--it2.id = Tmp.id_item

--INNER JOIN MetricUnit MUT ON 
--MUT.id = TMP.id_metricUnit

--inner join ItemType ITY2 on
--ITY2.id = It2.id_itemType
--inner join Presentation pres2 on
--pres2.id = it2.id_presentation
--inner join MetricUnit MT2 on
--MT2.id = TMP.id_metricUnit
--inner join ItemGeneral itg2 on
--itg2.id_item = it2.id
--inner join ItemSize itz2 on
--itz2.id = itg2.id_size
left join document doc on doc.id = PRODL.id
left join EmissionPoint ep on ep.id = doc.id_emissionPoint
left join Company cia on cia.id = ep.id_company
-------------------desp---------------------------
LEFT JOIN ProductionLotTrash desp on desp.id_productionLot = prodl.id
left join item despit on despit.id = desp.id_item
left join ItemGeneral despge on despge.id_item = desp.id_item
left join itemsize despsize on despsize.id = despge.id_item
left join Presentation desppre on desppre.id = despit.id_presentation
left join MetricUnit despmu on despmu.id = desp.id_metricUnit
left join warehouse despbod on despbod.id = desp.id_warehouse
left join WarehouseLocation ubidesp on ubidesp.id = desp.id_warehouseLocation
--------------------MERMA---------------------------------------
LEFT JOIN ProductionLotLoss merma on merma.id_productionLot = prodl.id
left join item mermait on mermait.id = merma.id_item
left join ItemGeneral mermage on mermage.id_item = merma.id_item
left join ItemSize mermasize on mermasize.id = mermage.id_size
left join Presentation mermapre on mermapre.id = mermait.id_presentation
left join MetricUnit mermamu on mermamu.id = merma.id_metricUnit
left join warehouse mermabod on mermabod.id = merma.id_warehouse
left join WarehouseLocation mermaubi on mermaubi.id = merma.id_warehouseLocation
--------------------------------------------------------------------------
--left join documentsource ds on ds.id_documentOrigin = PRODL.id 
--left join inventorymove mpe on mpe.id = ds.id_document and mpe.id_inventoryReason = 5
--left join document docim on docim.id = mpe.id 


where PRODL.id = @id	
 


 UNION ALL
Select 
'MERMA' as Estado,
Cast (PRODUN.name as varchar),
Cast (PRODPR.name as varchar),
PRODL.receptionDate,
PRODL.number,
PRODL.totalQuantityRecived,
PRODL.internalNumber,
--PRODL.totalQuantityRemitted,
ISNULL(MERMA.quantity,0),
Isnull(despbod.name,''),
Isnull(ubidesp.name,''),
((PRODL.totalQuantityLiquidation/PRODL.totalQuantityRecived-prodl.totalQuantityTrash)*100),
-----------------------------------------------------------------
isnull(PRODL.internalNumber,''),
isnull(mermait.masterCode,'') as liqCodigoProducto,
isnull(mermait.name,'') as LiqProducto,
isnull(mermasize.name,'') as liqTalla,
ISNULL(MERMA.quantity,0) as liqCantidad,--16
ISNULL(MERMAMU.code,'') as liqUnidadMedida,

'',--ITY2.name as liqTipoProducto,

--tmp.quantityPoundsLiquidation as liqlibrastotalrecobidas,

case when mermait.id_presentation is null then 
merma.quantity else 
case when desppre.id_metricUnit = 1 then 
ISNULL((MERMA.quantity * MERMAPRE.minimum*MERMAPRE.maximum )*2.2046,0)
else  
ISNULL((MERMA.quantity * MERMAPRE.minimum*MERMAPRE.maximum ),0) end end
as liqlibrastotalrecobidas,




PRODL.description
-------------------------------------
,PRODL.totalquantitytrash as PLtotalQuantityTrash
,PRODL.totalQuantityRecived - PRODL.totalquantitytrash  as PLLibrasNetas
,(prodl.totalQuantityLiquidation / prodl.totalQuantityRecived-prodl.totalQuantityTrash)*100 as Rendimiento
,(((prodl.totalQuantityLiquidation / prodl.totalQuantityRecived)*100) - 100) *-1 as Merma
,cia.logo as Logo


,
(SELECT top 1
 b.name +'   - N.- ' +convert(varchar,a.sequential)  from inventorymove a
inner join InventoryReason b on b.id = a.id_inventoryReason
left join DocumentSource c on c.id_document = b.id
left join document d on d.id = a.id
where a.id_productionLot = @id and b.id  in(1217)
order by a.sequential) as NDOCUMENTO

,(select
case when d.id_presentation is null then
a.quantity  
else
(case when 
a.id_metricUnit =4  then 
a.quantity*e.maximum*e.minimum
else 
(a.quantity*e.maximum*e.minimum)*2.2046
end)
 end 
cantidad 
from productionlotloss a 
left join productionlot b on b.id = a.id_productionLot
--inner join MetricUnit c on c.id = a.id_metricUnit
left join item d on d.id = a.id_item
left join Presentation e on e.id = d.id_presentation
where a.id_productionLot = @id) as LibrasMerma

---------------------------------------------
--,despit.name as ProductoDesperdicio
--,despmu.name as UMDesperdicio
--,despbod.name as BodegaDesperdicio
--,ubidesp.name as UbicacionDesperdicio
--,desp.quantity as CantidadDesperdicio
from ProductionLot PRODL 
inner join ProductionLotState PRODLS on 
PRODL.id_ProductionLotState = PRODLS.id
inner join ProductionUnit PRODUN on 
PRODUN.id = PRODL.id_productionUnit 
inner join ProductionProcess PRODPR on
PRODPR.id = PRODL.id_productionProcess
left outer join person PROVID on
PROVID.id = PRODL.id_provider
left outer join Person BUYER on
BUYER.id = PRODL.id_buyer
left outer join Person REQUEST on
REQUEST.id = PRODL.id_personRequesting
left outer join Person RECIEVER on
RECIEVER.id = PRODL.id_personReceiving
inner join Company COMP on
COMP.id = PRODL.id_company
-------------------------------------
Left outer join Warehouse WARE on
WARE.id = PRODPR.id_warehouse
Left outer join WarehouseLocation WARELOC on
WARELOC.id = PRODPR.id_warehouseLocation
-------------------------------------
--inner join  ProductionLotLiquidation TMP on
--TMP.id_productionLot = PRODL.id
--inner join Item It2 on
--it2.id = Tmp.id_item

--INNER JOIN MetricUnit MUT ON 
--MUT.id = TMP.id_metricUnit

--inner join ItemType ITY2 on
--ITY2.id = It2.id_itemType
--inner join Presentation pres2 on
--pres2.id = it2.id_presentation
--inner join MetricUnit MT2 on
--MT2.id = TMP.id_metricUnit
--inner join ItemGeneral itg2 on
--itg2.id_item = it2.id
--inner join ItemSize itz2 on
--itz2.id = itg2.id_size
left join document doc on doc.id = PRODL.id
left join EmissionPoint ep on ep.id = doc.id_emissionPoint
left join Company cia on cia.id = ep.id_company
--------------------------------------------------
LEFT JOIN ProductionLotTrash desp on desp.id_productionLot = prodl.id
left join item despit on despit.id = desp.id_item
left join ItemGeneral despge on despge.id_item = desp.id_item
left join itemsize despsize on despsize.id = despge.id_item
left join Presentation desppre on desppre.id = despit.id_presentation
left join MetricUnit despmu on despmu.id = desp.id_metricUnit
left join warehouse despbod on despbod.id = desp.id_warehouse
left join WarehouseLocation ubidesp on ubidesp.id = desp.id_warehouseLocation
--------------------MERMA---------------------------------------
LEFT JOIN ProductionLotLoss merma on merma.id_productionLot = prodl.id
left join item mermait on mermait.id = merma.id_item
left join ItemGeneral mermage on mermage.id_item = merma.id_item
left join ItemSize mermasize on mermasize.id = mermage.id_size
left join Presentation mermapre on mermapre.id = mermait.id_presentation
left join MetricUnit mermamu on mermamu.id = merma.id_metricUnit
left join warehouse mermabod on mermabod.id = merma.id_warehouse
left join WarehouseLocation mermaubi on mermaubi.id = merma.id_warehouseLocation
--------------------------------------
--left join documentsource ds on ds.id_documentOrigin = PRODL.id 
--left join inventorymove mpe on mpe.id = ds.id_document and mpe.id_inventoryReason = 2187
--left join document docim on docim.id = mpe.id 




where PRODL.id = @id	
 





select MPROLIQ,
	PRODUNnameNombreUnidadLoteProduccion ,
	PRODPRnameProcesoLoteProduccion,
	PRODLreceptionDateFechaRecepcion,
	PRODLnumberNumerodeLote,
	PRODLtotalQuantityRecivedTotalCantidadRecibidaLibras,
	PRODLinternalNumberNumeroInterno  ,
	PRODLtotalQuantityRemittedTCLREM,
	WAREnameBodega  ,
	Isnull(WARELOCnameUbicacionBodega ,'') Ubicacion,
	----------------------------------------------------
	RENDIMIENTO ,
	LOTORInumberLoteOrigen   ,
	ITEMmasterCodeCodigoProducto ,
	ITEMnameNombreItem ,
	isnull(ITEMSZnameTalla,'') as ITEMSZnameTalla ,
	PRODLDquantityRecivedCantidadRecibidaDetalle ,
	METRICcodeUnidadMedida ,
	tmpquantityPoundsLiquidationliqlibrastotalrecobidas,
	PRODLDescriptionObservaciones
	----------------------------
	,PLtotalQuantityTrash 
	,PLLibrasNetas 
	,PLRendimiento
	,PLMerma
	,logo
	,NDocumento
	,ISNULL(LBMerma,0) LbMerma
	--,DespProducto
	--,DespUM 
	--,DespBodega 
	--,DespUbicacion 
	--,DespCantidad


from #TEMRPTUDP

--Par_ProcesosInternos 68130


--select *from productionlotloss where id_productionLot = 766416



--libras porcesadas / remitidas




--select desp.quantity from productionlot prodl
--LEFT JOIN ProductionLotTrash desp on desp.id_productionLot = prodl.id
--left join item despit on despit.id = desp.id_item
--left join ItemGeneral despge on despge.id_item = desp.id_item
--left join itemsize despsize on despsize.id = despge.id_item
--left join Presentation desppre on desppre.id = despit.id_presentation
--left join MetricUnit despmu on despmu.id = desp.id_metricUnit
--left join warehouse despbod on despbod.id = desp.id_warehouse
--left join WarehouseLocation ubidesp on ubidesp.id = desp.id_warehouseLocation

--where prodl.id = 766416

--select merma.quantity from productionlot prodl
--LEFT JOIN ProductionLotLoss merma on merma.id_productionLot = prodl.id
--left join item mermait on mermait.id = merma.id_item
--left join ItemGeneral mermage on mermage.id_item = merma.id_item
--left join ItemSize mermasize on mermasize.id = mermage.id_size
--left join Presentation mermapre on mermapre.id = mermait.id_presentation
--left join MetricUnit mermamu on mermamu.id = merma.id_metricUnit
--left join warehouse mermabod on mermabod.id = merma.id_warehouse
--left join WarehouseLocation mermaubi on mermaubi.id = merma.id_warehouseLocation
--where
--prodl.id = 766416