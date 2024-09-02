/****** Object:  StoredProcedure [dbo].[spPar_ProcesosInternoResumen]    Script Date: 14/04/2023 09:06:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE OR ALTER PROC[dbo].[spPar_ProcesosInternoResumen]
(
--LOTE
@id varchar(50) ='',
@estado int =0,
@nlote varchar(50) ='',
@ninterno varchar(50)= '',
@unidad varchar(50)='',
@proceso varchar(50)='',
@producto varchar(50)='',
--Bodega
@bodega varchar(50)='',
@ubicacion varchar(100)='',
--fecha proceso
@fi varchar(10) = '',
@ff varchar(10) = ''
)
As 

set nocount on 

if @fi = '' set @fi = null
if @ff = '' set @ff = null

declare @fiDt date
declare @ffDt date

Declare @cad                  VARCHAR(8000) 

Create Table #Lotet (id int)

set @id = isnull(@id,'')
set @fiDt = convert(date,isnull(@fi,'1900-01-01'))
set @ffDt = convert(date,isnull(@ff,'1900-01-01'))


	Set @cad = ""  
	Set @cad = @cad + "Insert Into #Lotet" + char(13)  
	Set @cad = @cad + "select pl.id from [ProductionLot] pl " + char(13)  
	Set @cad = @cad + "inner join ProductionProcess PRODPR on " + char(13) 
	Set @cad = @cad + "PRODPR.id = pl.id_productionProcess " + char(13) 
	Set @cad = @cad + "where pl.id_ProductionLotState not in ('11')" + char(13) 
	If @fi <> '' and  @ff <> ''
		Set @Cad = @Cad + "and Convert(Varchar,pl.receptionDate,112) >= '" + Convert(Varchar,@fiDt,112) + "' And Convert(Varchar,pl.receptionDate,112) <= '" + Convert(Varchar,@ffDt,112) + "'" + Char(13)
	If @fi <> '' and  @ff = ''
		Set @Cad = @Cad + "and Convert(Varchar,pl.receptionDate,112) >= '" + Convert(Varchar,@fiDt,112) + "'" + Char(13)	 
	If @fi = '' and  @ff <> ''
		Set @Cad = @Cad + "and Convert(Varchar,pl.receptionDate,112) <= '" + Convert(Varchar,@ffDt,112) + "'" + Char(13)
	If @proceso Is Not Null and @proceso <> ''
		Set @Cad = @Cad + "and PRODPR.name = '" + Convert(Varchar,@proceso) + "'" + Char(13) 
	Else 
		Set @Cad = @Cad + " " 
	
	Exec(@cad) 
	print(@cad) 
				
	select * Into #Lotetemp from #Lotet

set nocount on
create table #TEMRPTUDP (
	idPl int,
	[MPROLIQ] [VARCHAR] (25) NOT NULL,--
	[PRODUNnameNombreUnidadLoteProduccion] [varchar](150)NOT NULL,
	[PRODPRnameProcesoLoteProduccion][varchar](150)NOT NULL,
	[PRODLreceptionDateFechaRecepcion] [datetime]NOT NULL,
	[PRODLnumberNumerodeLote] [varchar](20) NOT NULL,
	[PRODLtotalQuantityRecivedTotalCantidadRecibidaLibras] [decimal](14, 6) NOT NULL,
	[PRODLinternalNumberNumeroInterno]  [varchar](20) NOT NULL,
	[PRODLtotalQuantityRemittedTCLREM] [decimal](14, 6) NOT NULL,
	[WAREnameBodega]  [varchar](50) NOT NULL,
	[WARELOCnameUbicacionBodega]  [varchar](250) NOT NULL,
	[Estado][varchar](50) not null,
	EstadoId int not null,
	FECHADESDE  [varchar](30) NOT NULL,
	FECHAHASTA  [varchar](30) NOT NULL,
	CODIGO [VARCHAR](50) NOT NULL,
	RESPONSABLE  [varchar](250) NOT NULL,
	INTERNALLOT VARCHAR(50)  NULL,
	MERMA [decimal](18, 6) NULL,--19
	CODEESTADO VARCHAR(10) NOT NULL,
	
	----------------------------------------------------
	[RENDIMIENTO] [decimal](14, 6) NOT NULL,
	[LOTORInumberLoteOrigen]   [varchar](20) NOT NULL,
	[ITEMmasterCodeCodigoProducto] [varchar](50) NOT NULL,
	[ITEMnameNombreItem] [varchar](250) NOT NULL,
	[ITEMSZnameTalla] [varchar](50) NULL,
	[PRODLDquantityRecivedCantidadRecibidaDetalle] [decimal](14, 6) NOT NULL,--26
	[METRICcodeUnidadMedida] [varchar](50) NOT NULL,
	--[ITY2nameliqTipoProducto] [varchar](50) NOT NULL,
	[tmpquantityPoundsLiquidationliqlibrastotalrecobidas] [decimal](18, 6) NULL,--29
	[PRODLDescriptionObservaciones] [varchar](350) NULL,
	CODIGOSTATE VARCHAR(50) NOT NULL,
	suma [decimal](14, 6) NOT NULL,
	sumaliquidacion [decimal](14, 6) NOT NULL,
	sumadesperdicio [decimal](14, 6) NOT NULL,
	sumamerma decimal(14,6),
	PMINIMUM [decimal](14, 6) NOT NULL,
	PMAXIMUM INT,
	PRESEID INT
	,OCULTAR BIT
	,LibsDetalle decimal(17,2)
	,Libsliquidacion decimal(17,2)
	,mermatotal decimal (17,2)
	,desperdiciototal decimal (17,2)
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
PRODL.id as IDPL,
'MATERIA PRIMA' as Estado,
Cast (PRODUN.name as varchar(150)),
Cast (PRODPR.name as varchar(150)),
PRODL.receptionDate,
PRODL.number,
PRODL.totalQuantityRecived,
PRODL.internalNumber,
PRODL.totalQuantityLiquidation,
WARE.name,
WARELOC.name,
PRODLS.name,
PRODL.id_ProductionLotState,
convert (varchar,PRODL.receptionDate) ,
convert (varchar,PRODL.expirationDate),
PRODPR.code,
REQUEST.fullname_businessName,
LOTORI2.internalNumber2,
MERMA =( round( round(PRODL.totalQuantityLiquidation,2)/(round(prodl.totalQuantityRecived,2)-round(prodl.totalQuantityTrash,2))*100,2)),
PRODL.id_ProductionLotState,
((PRODL.totalQuantityLiquidation/PRODL.totalQuantityRecived)*100),
-----------------------------------------------------------------
isnull(LOTORI.internalNumber,''),
ITEM.masterCode,
ITEM.name,
ITEMSZ.name ,
PRODLD.quantityRecived,
mu02.code,-------------------------------------
--ITY2.name as liqTipoProducto,
ROUND((isnull(fact.factor,1 )  *  PRODLD.quantityRecived ),2)  as liqlibrastotalrecobidas,------29
PRODL.description,
PRODLS.code,
0 as suma,
0 as sumaliquidacion,
0 as sumadesperdicio,
(select sum(totalQuantityLoss) from productionlot plss where plss.id in (select id from #Lotetemp)) as Sumamerma,


PRESE.minimum,
PRESE.maximum,
PRESE.id_metricUnit

,CASE WHEN ITEM.masterCode IS NULL THEN 1 ELSE 0 end AS ocultar

,case when PRESE.id_metricUnit = 4 then
PRODLD.quantityRecived*prese.minimum*prese.maximum

else PRODLD.quantityRecived*prese.minimum*prese.maximum  * 2.2046 
--tmp.quantitytotal -- 
end as LibrasDetalle
,Libsliquidacion=0
,isnull(case when mermamu.id = 4 then
merma.quantity else 
merma.quantity * 2.2046
end,0) as MermaLibrasDetalle
,isnull(case when despmu.id = 4 then
desp.quantity else 
desp.quantity * 2.2046
end,0) as DespLibrasDetalle



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
inner join Warehouse WARE on
WARE.id = PRODPR.id_warehouse
inner join WarehouseLocation WARELOC on
WARELOC.id = PRODPR.id_warehouseLocation

left outer join 
( select d.id, 
	     case 
		   when isnull(d.number,'') != '' then d.number
		   else 
		     (case when isnull(f.number,'') != '' then f.number else '' end)
	     end as  internalNumber2       
  from Lot d 
  left outer join ProductionLot f 
  on f.id = d.id  ) as LOTORI2 on
LOTORI2.id = PRODLD.id_originLot




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
left outer join Presentation PRESEN on------------------
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
LEFT JOIN LOT L ON
L.ID = PRODL.id
left join document doc on doc.id = PRODL.id
------------------------------------------------------
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

--group by PRODL.id ,PRODUN.name ,PRODPR.name ,PRODL.receptionDate,
--PRODL.number,PRODL.totalQuantityRecived, PRODL.internalNumber,PRODL.totalQuantityLiquidation,WARE.name,WARELOC.name,PRODLS.name,PRODL.id_ProductionLotState,
--PRODL.receptionDate,PRODL.expirationDate,PRODPR.code,REQUEST.fullname_businessName,LOTORI2.internalNumber2, PRODL.id_ProductionLotState,LOTORI.internalNumber,ITEM.masterCode,
--ITEM.name,ITEMSZ.name ,PRODLD.quantityRecived,mu02.code,ITY2.name ,fact.factor,PRODLD.quantityRecived ,PRODL.description,PRODLS.code,PRODLD.quantityRecived




--where PRODL.id = @id
UNION all
Select 
PRODL.id as IDPL,
'LIQUIDACION' as Estado,
Cast (PRODUN.name as varchar(150)),
Cast (PRODPR.name as varchar(150)),
PRODL.receptionDate,
PRODL.number,
PRODL.totalQuantityRecived,
PRODL.internalNumber,
PRODL.totalQuantityRemitted,
WARE.name,
WARELOC.name,
PRODLS.name,
PRODL.id_ProductionLotState,
PRODL.receptionDate,
PRODL.expirationDate,
PRODPR.code,
REQUEST.fullname_businessName,
LOTORI2.internalNumber2,
MERMA =( round( round(PRODL.totalQuantityLiquidation,2)/(round(prodl.totalQuantityRecived,2)-round(prodl.totalQuantityTrash,2))*100,2)),
PRODL.id_ProductionLotState,
((PRODL.totalQuantityLiquidation/PRODL.totalQuantityRecived)*100),
-----------------------------------------------------------------
isnull(PRODL.internalNumber,''),
It2.masterCode as liqCodigoProducto,
it2.name as LiqProducto,
itz2.name as liqTalla,
tmp.quantity as liqCantidad,
MUT.code as liqUnidadMedida,

--ITY2.name as liqTipoProducto,

ROUND(tmp.quantityPoundsLiquidation,2) as liqlibrastotalrecobidas,
PRODL.description,
PRODLS.code,
0 as suma ,
0 as sumaliquidacion,
0 as sumadesperdicio,
0 as Sumamerma,

pres2.minimum,
pres2.maximum,
PRES2.id_metricUnit

,CASE WHEN it2.masterCode IS NULL THEN 1 ELSE 0 end AS ocultar
,case when PRES2.id_metricUnit = 4 then
tmp.quantitytotal 

else  tmp.quantitytotal  * 2.2046 
--tmp.quantitytotal -- 
end as LibrasDetalle

,Libsliquidacion=case when PRES2.id_metricUnit = 4 then
ROUND(tmp.quantitytotal,2) 

else ROUND( (tmp.quantitytotal  * 2.2046 ),2)
--tmp.quantitytotal -- 
end 
,isnull(case when mermamu.id = 4 then
merma.quantity else 
merma.quantity * 2.2046
end,0) as MermaLibrasDetalle
,isnull(case when despmu.id = 4 then
desp.quantity else 
desp.quantity * 2.2046
end,0) as DespLibrasDetalle



--select *from ProductionLotLiquidation where id_productionLot = 676610

--sum(tmp.quantityPoundsLiquidation)
-------------------------------------
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
inner join Warehouse WARE on
WARE.id = PRODPR.id_warehouse
inner join WarehouseLocation WARELOC on
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
LEFT JOIN LOT L ON
L.ID = PRODL.id
left join document doc on doc.id = PRODL.id
------------------------------------------------------
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

--where PRODL.id = @id	
 left outer join 
( select d.id, 
	     case 
		   when isnull(d.number,'') != '' then d.number
		   else 
		     (case when isnull(f.number,'') != '' then f.number else '' end)
	     end as  internalNumber2       
  from Lot d 
  left outer join ProductionLot f 
  on f.id = d.id  ) as LOTORI2 on
LOTORI2.id = PRODL.id
where PRODLS.code != '01'------------------------------------

------------------///////////////////////////////////////////////////

UNION all
Select 
PRODL.id as IDPL,
'DESPERDICIO' as Estado,
Cast (PRODUN.name as varchar(150)),
Cast (PRODPR.name as varchar(150)),
PRODL.receptionDate,
PRODL.number,
PRODL.totalQuantityRecived,
PRODL.internalNumber,
PRODL.totalQuantityRemitted,
WARE.name,
WARELOC.name,
PRODLS.name,
PRODL.id_ProductionLotState,
PRODL.receptionDate,
PRODL.expirationDate,
PRODPR.code,
REQUEST.fullname_businessName,
LOTORI2.internalNumber2,
MERMA =( round( round(PRODL.totalQuantityLiquidation,2)/(round(prodl.totalQuantityRecived,2)-round(prodl.totalQuantityTrash,2))*100,2)),
PRODL.id_ProductionLotState,
((PRODL.totalQuantityLiquidation/PRODL.totalQuantityRecived)*100),
-----------------------------------------------------------------



--
isnull(PRODL.internalNumber,''),

isnull(despit.masterCode,'') as liqCodigoProducto,
isnull(despit.name,'') as LiqProducto,
isnull(despsize.name,'') as liqTalla,
ISNULL(desp.quantity,0) as liqCantidad,--16
ISNULL(despmu.code,'') as liqUnidadMedida,

--ITY2.name as liqTipoProducto,

case when despit.id_presentation is null then
desp.quantity  
else
case when despmu.id = 4 then 
ISNULL((desp.quantity),0)
else  
ISNULL((desp.quantity)*2.2046,0) end end
as liqlibrastotalrecobidas,

PRODL.description,
PRODLS.code,
0 as suma ,
0 as sumaliquidacion,
0 as sumadesperdicio,
0 as Sumamerma,


isnull(desppre.minimum,0),
isnull(desppre.maximum,0),
isnull(desppre.id_metricUnit,0)

,CASE WHEN despit.masterCode ='' THEN 1 ELSE 0 end AS ocultar

,isnull(case when despmu.id = 4 then
desp.quantity else 
desp.quantity * 2.2046
end,0) as LibrasDetalle
, lbsliquidacion = 0
,isnull(case when mermamu.id = 4 then
merma.quantity else 
merma.quantity * 2.2046
end,0) as MermaLibrasDetalle
,isnull(case when despmu.id = 4 then
desp.quantity else 
desp.quantity * 2.2046
end,0) as DespLibrasDetalle




--sum(tmp.quantityPoundsLiquidation)
-------------------------------------
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
inner join Warehouse WARE on
WARE.id = PRODPR.id_warehouse
inner join WarehouseLocation WARELOC on
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
LEFT JOIN LOT L ON
L.ID = PRODL.id
left join document doc on doc.id = PRODL.id
------------------------------------------------------
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

--where PRODL.id = @id	
 left outer join 
( select d.id, 
	     case 
		   when isnull(d.number,'') != '' then d.number
		   else 
		     (case when isnull(f.number,'') != '' then f.number else '' end)
	     end as  internalNumber2       
  from Lot d 
  left outer join ProductionLot f 
  on f.id = d.id  ) as LOTORI2 on
LOTORI2.id = PRODL.id
where PRODLS.code != '01'------------------------------------




UNION all
Select 
PRODL.id as IDPL,
'MERMA' as Estado,
Cast (PRODUN.name as varchar(150)),
Cast (PRODPR.name as varchar(150)),
PRODL.receptionDate,
PRODL.number,
PRODL.totalQuantityRecived,
PRODL.internalNumber,
PRODL.totalQuantityRemitted,
WARE.name,
WARELOC.name,
PRODLS.name,
PRODL.id_ProductionLotState,
PRODL.receptionDate,
PRODL.expirationDate,
PRODPR.code,
REQUEST.fullname_businessName,
LOTORI2.internalNumber2,
MERMA =( round( round(PRODL.totalQuantityLiquidation,2)/(round(prodl.totalQuantityRecived,2)-round(prodl.totalQuantityTrash,2))*100,2)),
PRODL.id_ProductionLotState,
((PRODL.totalQuantityLiquidation/PRODL.totalQuantityRecived)*100),
-----------------------------------------------------------------



--
isnull(PRODL.internalNumber,''),

isnull(despit.masterCode,'') as liqCodigoProducto,
isnull(despit.name,'') as LiqProducto,
isnull(despsize.name,'') as liqTalla,
ISNULL(desp.quantity,0) as liqCantidad,--16
ISNULL(despmu.code,'') as liqUnidadMedida,

--ITY2.name as liqTipoProducto,

case when mermait.id_presentation is null then
merma.quantity  
else
case when mermamu.id = 4 then 
ISNULL((merma.quantity),0)
else  
ISNULL((merma.quantity)*2.2046,0) end end
as liqlibrastotalrecobidas,

PRODL.description,
PRODLS.code,
0 as suma ,
0 as sumaliquidacion,
0 as sumadesperdicio,
0 as Sumamerma,

isnull(mermapre.minimum,0),
isnull(mermapre.maximum,0),
isnull(mermapre.id_metricUnit,0)

,CASE WHEN mermait.masterCode ='' THEN 1 ELSE 0 end AS ocultar

,isnull(case when mermamu.id = 4 then
merma.quantity else 
merma.quantity * 2.2046
end,0) as LibrasDetalle
,lbsliquidacion=0
,isnull(case when mermamu.id = 4 then
merma.quantity else 
merma.quantity * 2.2046
end,0) as MermaLibrasDetalle

,isnull(case when despmu.id = 4 then
desp.quantity else 
desp.quantity * 2.2046
end,0) as DespLibrasDetalle
--sum(tmp.quantityPoundsLiquidation)
-------------------------------------
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
inner join Warehouse WARE on
WARE.id = PRODPR.id_warehouse
inner join WarehouseLocation WARELOC on
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
LEFT JOIN LOT L ON
L.ID = PRODL.id
left join document doc on doc.id = PRODL.id
------------------------------------------------------
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

--where PRODL.id = @id	
 left outer join 
( select d.id, 
	     case 
		   when isnull(d.number,'') != '' then d.number
		   else 
		     (case when isnull(f.number,'') != '' then f.number else '' end)
	     end as  internalNumber2       
  from Lot d 
  left outer join ProductionLot f 
  on f.id = d.id  ) as LOTORI2 on
LOTORI2.id = PRODL.id
where PRODLS.code != '01'------------------------------------





--group by PRODL.id ,PRODUN.name ,PRODPR.name ,PRODL.receptionDate,
--PRODL.number,PRODL.totalQuantityRecived, PRODL.internalNumber,PRODL.totalQuantityRemitted,WARE.name,WARELOC.name,PRODLS.name,PRODL.id_ProductionLotState,
--PRODL.receptionDate,PRODL.expirationDate,PRODPR.code,REQUEST.fullname_businessName,LOTORI2.internalNumber2,PRODL.totalQuantityLiquidation, PRODL.id_ProductionLotState,PRODL.internalNumber,It2.masterCode,
--it2.name,itz2.name ,tmp.quantity,mut.code,ITY2.name ,ITY2.name,tmp.quantityPoundsLiquidation ,PRODL.description,PRODLS.code,tmp.quantityPoundsLiquidation






select
	idPl,
	MPROLIQ,
	PRODUNnameNombreUnidadLoteProduccion ,
	PRODPRnameProcesoLoteProduccion,
	PRODLreceptionDateFechaRecepcion,
	PRODLnumberNumerodeLote,
	PRODLtotalQuantityRecivedTotalCantidadRecibidaLibras,
	PRODLinternalNumberNumeroInterno  ,
	PRODLtotalQuantityRemittedTCLREM,
	WAREnameBodega  ,
	WARELOCnameUbicacionBodega  ,
	Estado,
	FECHADESDE,
	FECHAHASTA,
	CODIGO,
	RESPONSABLE,
	INTERNALLOT,
	MERMA,
	CODEESTADO,
	----------------------------------------------------
	RENDIMIENTO ,
	LOTORInumberLoteOrigen   ,
	ITEMmasterCodeCodigoProducto ,
	ITEMnameNombreItem ,
	ITEMSZnameTalla ,
	PRODLDquantityRecivedCantidadRecibidaDetalle ,
	METRICcodeUnidadMedida ,
	--[ITY2nameliqTipoProducto],
	tmpquantityPoundsLiquidationliqlibrastotalrecobidas,
	PRODLDescriptionObservaciones,
	CODIGOSTATE,
	ROUND(ISNULL(CASE  WHEN MPROLIQ ='MATERIA PRIMA' THEN sum(tmpquantityPoundsLiquidationliqlibrastotalrecobidas) END,0),2) AS suma,
	ROUND(ISNULL(case when MPROLIQ = 'LIQUIDACION' then sum(tmpquantityPoundsLiquidationliqlibrastotalrecobidas) end,0),2) as sumaliquidacion,
	ROUND(isnull(case when MPROLIQ = 'DESPERDICIO'then sum(tmpquantityPoundsLiquidationliqlibrastotalrecobidas) end,0),2) as sumadesperdicio,
	ROUND(isnull(case when MPROLIQ = 'MERMA'then tmpquantityPoundsLiquidationliqlibrastotalrecobidas end,0),2) as sumamerma,
	PMINIMUM,
	PMAXIMUM,
	PRESEID

	,CASE WHEN MPROLIQ = 'MERMA' THEN
	case when sumamerma = 0 then 
	CASE WHEN  MPROLIQ = 'DESPERDICIO' THEN
	CASE WHEN sumadesperdicio = 0 THEN 
	'OCULTAR' ELSE 'NO OCULTAR'END END END END AS OCMERMA

	,LibsDetalle
	--SUMAEGRESO*/
	,Libsliquidacion, 
	@fiDt AS Fi,
	@ffDt AS Ff
	
from #TEMRPTUDP
where --idPl = @id
PRODLinternalNumberNumeroInterno = case when isnull(@id,'') = '' then PRODLinternalNumberNumeroInterno else @id end
and EstadoId = case when isnull(@estado,0) = 0 then  EstadoId else @estado end
and PRODLnumberNumerodeLote = case when isnull(@nlote,'') = '' then PRODLnumberNumerodeLote else @nlote end
and PRODLinternalNumberNumeroInterno = case when isnull(@ninterno,'') = '' then PRODLinternalNumberNumeroInterno else @ninterno end
and PRODUNnameNombreUnidadLoteProduccion= case when isnull(@unidad,'') = '' then PRODUNnameNombreUnidadLoteProduccion else @unidad end
and	PRODPRnameProcesoLoteProduccion = case when isnull(@proceso,'') ='' then PRODPRnameProcesoLoteProduccion else @proceso end and 
convert(date,PRODLreceptionDateFechaRecepcion) >= case when year(@fiDt) = 1900 then convert(date, PRODLreceptionDateFechaRecepcion) else @fiDt end
and convert(date,PRODLreceptionDateFechaRecepcion) <= case when year(@ffDt) = 1900 then convert(date, PRODLreceptionDateFechaRecepcion) else @ffDt end
and CODIGO != 'REC'  
And EstadoId <> 11
--AND PRODLnumberNumerodeLote = 'ETQ000001016'





group by idPl,
	MPROLIQ,
	PRODUNnameNombreUnidadLoteProduccion ,
	PRODPRnameProcesoLoteProduccion,
	PRODLreceptionDateFechaRecepcion,
	PRODLnumberNumerodeLote,
	PRODLtotalQuantityRecivedTotalCantidadRecibidaLibras,
	PRODLinternalNumberNumeroInterno  ,
	PRODLtotalQuantityRemittedTCLREM,
	WAREnameBodega  ,
	WARELOCnameUbicacionBodega , 
	Estado,
	FECHADESDE,
	FECHAHASTA,
	CODIGO,
	RESPONSABLE,
	INTERNALLOT,
	MERMA,
	CODEESTADO,
	----------------------------------------------------
	RENDIMIENTO ,
	LOTORInumberLoteOrigen   ,
	ITEMmasterCodeCodigoProducto ,
	ITEMnameNombreItem ,
	ITEMSZnameTalla ,
	PRODLDquantityRecivedCantidadRecibidaDetalle ,
	METRICcodeUnidadMedida ,
	--[ITY2nameliqTipoProducto],
	tmpquantityPoundsLiquidationliqlibrastotalrecobidas,
	PRODLDescriptionObservaciones,
	CODIGOSTATE,
	suma,
	sumaliquidacion,
	PMINIMUM,
	PMAXIMUM,
	PRESEID,
	--logo,
	OCULTAR,
	LibsDetalle
	,sumadesperdicio
	,sumamerma
	,Libsliquidacion
	,mermatotal
	,desperdiciototal


/*
	EXEC [dbo].[spPar_ProcesosInternoResumen] @id=N'',@estado=0,@nlote=N'',@ninterno=N'',@unidad=N'',@proceso=N'',@producto=N'',@bodega=N'',@ubicacion=N'',@fi=N'2022/02/01',@ff=N'2022/02/28'
*/
GO
