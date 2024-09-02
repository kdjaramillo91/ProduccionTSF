/****** Object:  StoredProcedure [dbo].[Par_ProcesosInternosDetalladoTipos]    Script Date: 20/01/2023 1:12:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROC [dbo].[spPar_DetalleDeReproceso]

(--LOTE
--@id varchar(50) ='',
@estado int =0,
--@nlote varchar(50) ='',
--@ninterno varchar(50)= '',
@unidad varchar(50)='',
@proceso varchar(50)='',
--@producto varchar(50)='',
--Bodega
--@bodega varchar(50)='',
--@ubicacion varchar(100)='',
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

--set @id = isnull(@id,'')
set @fiDt = convert(date,isnull(@fi,'1900-01-01'))
set @ffDt = convert(date,isnull(@ff,'1900-01-01'))

Create Table #TempDocument (
id int
)

Insert Into #TempDocument
 select pl.id 
 from ProductionLot pl
 inner join ProductionProcess prp on prp.id = pl.id_productionProcess
 LEFT JOIN PRODUCTIONLOTDETAIL EGR ON EGR.id_productionLot = PL.ID
 LEFT JOIN ITEM ITEGR ON ITEGR.ID = EGR.id_item
 inner join ProductionUnit PU on PU.id = pl.id_productionUnit
 inner join productionlotstate pls on pls.id = pl.id_ProductionLotState
where 
pls.id = case when isnull(@estado,0) = 0 then  pls.id else @estado end and
 pu.name = case when isnull(@unidad,'') = '' then pu.name  else @unidad end and
	prp.name = case when isnull(@proceso,'') ='' then prp.name  else @proceso end and 
convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.receptionDate) else @fiDt end
and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date,pl.receptionDate) else @ffDt end
and 
prp.code != 'REC' and prp.code !='RMM' 
--AND NumeroLote = 'ETQ000001016'
and pl.id_ProductionLotState not in (11,13)


--------AGRUPADA POR LOTE-------------------
Create Table #TempProductionLot (
id_pl int,
cantidadLibras decimal(18,6),
numeroDetalles int
)

Insert Into #TempProductionLot
 select pl.id, sum(EGR.quantityRecived), Count(EGR.id) from ProductionLot pl
 LEFT JOIN PRODUCTIONLOTDETAIL EGR ON EGR.id_productionLot = PL.ID
 Inner Join #TempDocument DC On DC.id = pl.id
group by pl.id

--------AGRUPADA POR PROCESO-------------------

Create Table #TempProductionLotPRP (
id_pl int,
id_itty int,
id_PRP int,
id_il int,
librasRecibidasPRP decimal(17,2)
)

Insert Into #TempProductionLotPRP
 select pl.id ,itegr.id_itemType, PRP.id,ITEGR.id_inventoryLine, sum(EGR.quantityRecived) from ProductionLot pl
 inner join ProductionProcess prp on prp.id = pl.id_productionProcess
 LEFT JOIN PRODUCTIONLOTDETAIL EGR ON EGR.id_productionLot = PL.ID
 LEFT JOIN ITEM ITEGR ON ITEGR.ID = EGR.id_item
 inner join ProductionUnit PU on PU.id = pl.id_productionUnit
 inner join productionlotstate pls on pls.id = pl.id_ProductionLotState
where 
pls.id = case when isnull(@estado,0) = 0 then  pls.id else @estado end and
--and NumeroLote = case when isnull(@nlote,'') = '' then NumeroLote else @nlote end
--and NumeroInterno = case when isnull(@ninterno,'') = '' then NumeroInterno else @ninterno end
 pu.name = case when isnull(@unidad,'') = '' then pu.name  else @unidad end and
	prp.name = case when isnull(@proceso,'') ='' then prp.name  else @proceso end and 
convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.receptionDate) else @fiDt end
and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date,pl.receptionDate) else @ffDt end
and 
prp.code != 'REC' and prp.code !='RMM' 
--AND NumeroLote = 'ETQ000001016'
and pl.id_ProductionLotState not in (11,13)-- and ity.id_inventoryLine in (12)

group by PRP.ID,pl.id,pl.id,ITEGR.id_itemType,ITEGR.id_inventoryLine

--select *from #TempProductionLotPRP


------AGRUPADA POR TIPO-------------------

Create Table #TempProductionLotitty (
id_plot int,
id_ITEMTYPE int,
id_PRP int,
id_il int,
librasRecibidasitty decimal(17,2)
)

Insert Into #TempProductionLotitty 
 select pl.id, ITEGR.id_itemType,pl.id_productionProcess,itegr.id_inventoryLine, sum(EGR.quantityRecived) from ProductionLot pl
 inner join ProductionProcess prp on prp.id = pl.id_productionProcess
 LEFT JOIN PRODUCTIONLOTDETAIL EGR ON EGR.id_productionLot = PL.ID
 LEFT JOIN ITEM ITEGR ON ITEGR.ID = EGR.id_item
 inner join ProductionUnit PU on PU.id = pl.id_productionUnit
 inner join productionlotstate pls on pls.id = pl.id_ProductionLotState
where 
pls.id = case when isnull(@estado,0) = 0 then  pls.id else @estado end and
--and NumeroLote = case when isnull(@nlote,'') = '' then NumeroLote else @nlote end
--and NumeroInterno = case when isnull(@ninterno,'') = '' then NumeroInterno else @ninterno end
 pu.name = case when isnull(@unidad,'') = '' then pu.name  else @unidad end and
	prp.name = case when isnull(@proceso,'') ='' then prp.name  else @proceso end and 
convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.receptionDate) else @fiDt end
and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date,pl.receptionDate) else @ffDt end
and 
prp.code != 'REC' and prp.code !='RMM' 
--AND NumeroLote = 'ETQ000001016'
and pl.id_ProductionLotState not in (11,13)-- and ity.id_inventoryLine in (12)
group by ITEGR.id_itemType,pl.id,ITEGR.id_inventoryLine,pl.id_productionProcess,itegr.id_inventoryLine

--select *from #TempProductionLotitty

--------AGRUPADA POR LINEA INVENTARIO-------------------

Create Table #TempProductionLotIL (
id_pl int,
id_itemType int,
id_inventoryLine int,
id_prp int,
librasRecibidasIL decimal(17,2)
)

Insert Into #TempProductionLotIL 
 select pl.id,itegr.id_itemType, ITEGR.id_inventoryLine, pl.id_productionProcess,sum(EGR.quantityRecived) from ProductionLot pl
 inner join ProductionProcess prp on prp.id = pl.id_productionProcess
 LEFT JOIN PRODUCTIONLOTDETAIL EGR ON EGR.id_productionLot = PL.ID
 LEFT JOIN ITEM ITEGR ON ITEGR.ID = EGR.id_item
 inner join ProductionUnit PU on PU.id = pl.id_productionUnit
 inner join productionlotstate pls on pls.id = pl.id_ProductionLotState
where 
pls.id = case when isnull(@estado,0) = 0 then  pls.id else @estado end and
--and NumeroLote = case when isnull(@nlote,'') = '' then NumeroLote else @nlote end
--and NumeroInterno = case when isnull(@ninterno,'') = '' then NumeroInterno else @ninterno end
 pu.name = case when isnull(@unidad,'') = '' then pu.name  else @unidad end and
	prp.name = case when isnull(@proceso,'') ='' then prp.name  else @proceso end and 
convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.receptionDate) else @fiDt end
and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date,pl.receptionDate) else @ffDt end
and 
prp.code != 'REC' and prp.code !='RMM'   
--AND NumeroLote = 'ETQ000001016'
and pl.id_ProductionLotState not in (11,13)-- and ity.id_inventoryLine in (12) 
group by ITEGR.id_inventoryLine,pl.id,itegr.id_itemType,pl.id_productionProcess


--select *from #TempProductionLotIL



SELECT
ISNULL(com.businessName, '') AS NombreCia,
ISNULL(com.ruc,'') AS RucCia,
ISNULL(com.address, '') AS DireccionCia,
PL.ID IDPLOT
,pl.internalNumber 
,pls.name as Estado
,prp.id as Agr1Proceso
,prp.name  as PRProceso
,prp.id as Agr2Prproc--------------------------- ProductionProcess ag1
,il.NAME as LineaInv

,pl.receptionDate as Fecha
,ITEGR.description ITEMEGRESO
,egr.quantityRecived lbsrecibidas ,
--((Presentation.maximum*Presentation.minimum)*egr.quantityRecived ),
((pre.maximum*pre.minimum)*egr.quantityRecived)*2.2046 AS RESULT,
--egr.quantityRecived AS RESULT,
ITING.description AS ITEMINGRESO  
,ing.quantityPoundsLiquidation LibrasIngreso
,(((pre.maximum*pre.minimum)*egr.quantityRecived)*2.2046)  - ing.quantityPoundsLiquidation as  LbrxBajas
,round(((((((pre.maximum*pre.minimum)*egr.quantityRecived)*2.2046)  - ing.quantityPoundsLiquidation )/(((pre.maximum*pre.minimum)*egr.quantityRecived)*2.2046)) *100),2) as Rend 
,ity.id as agr3Tipo --------------------AGR3 TIPO
,ity.name as tipo
,ity.id_inventoryLine agr4invline ---------AGR2 INVENT 
,il.name as il
,ilT.librasRecibidasitty,ity.id 
,ilt2.librasRecibidasIL ,il.id
,ILT3.librasRecibidasPRP ,prp.id
, ISNULL(@fiDt, '') AS Fi
, ISNULL(@ffDt, '') AS Ff

FROM PRODUCTIONLOT PL 
inner join productionlotstate pls on pls.id = pl.id_ProductionLotState
inner join document doc on doc.id = pl.id
Inner join EmissionPoint emi on doc.id_emissionPoint=emi.id
Inner join Company com on emi.id_company=com.id
inner join ProductionUnit PU on PU.id = pl.id_productionUnit
inner join ProductionProcess prp on prp.id = pl.id_productionProcess
LEFT JOIN PRODUCTIONLOTDETAIL EGR ON EGR.id_productionLot = PL.ID
LEFT JOIN ITEM ITEGR ON ITEGR.ID = EGR.id_item
--LEFT JOIN ITEM ITEGR ON ITEGR.id_presentation = Presentation.id
left join ItemType ity on ity.id = ITEGR.id_itemType
LEFT JOIN ProductionLotLiquidation ING ON ING.id_productionLot = PL.ID
LEFT JOIN ITEM ITING ON ITING.id = ING.id_item
left join InventoryLine il on il.id = ity.id_inventoryLine
--
left join Presentation pre on pre.id =ITEGR.id_presentation
left join #TempProductionLotitty ilT on ilT.id_ITEMTYPE = ITEGR.id_itemType and ilt.id_plot = pl.id  and ilt.id_PRP = pl.id_productionProcess and ilt.id_il = ITEGR.id_inventoryLine
left join #TempProductionLotIL ilt2 on ilt2.id_inventoryLine = itegr.id_inventoryLine and ilt2.id_pl = pl.id and ilt2.id_itemType = ITEGR.id_itemType and pl.id_productionProcess = ilt2.id_prp 
LEFT JOIN #TempProductionLotPRP ILT3 ON ILT3.id_PRP = PL.id_productionProcess and ilt3.id_pl = pl.id and ILT3.id_itty = ITEGR.id_itemType and ILT3.id_il = itegr.id_inventoryLine



where 
pls.id = case when isnull(@estado,0) = 0 then  pls.id else @estado end and
--and NumeroLote = case when isnull(@nlote,'') = '' then NumeroLote else @nlote end
--and NumeroInterno = case when isnull(@ninterno,'') = '' then NumeroInterno else @ninterno end
 pu.name = case when isnull(@unidad,'') = '' then pu.name  else @unidad end and
	prp.name = case when isnull(@proceso,'') ='' then prp.name  else @proceso end and 
convert(date,pl.receptionDate) >= case when year(@fiDt) = 1900 then convert(date, pl.receptionDate) else @fiDt end
and convert(date,pl.receptionDate) <= case when year(@ffDt) = 1900 then convert(date,pl.receptionDate) else @ffDt end
and 
prp.code != 'REC' and prp.code !='RMM' -- and pl.id = 849796
--AND NumeroLote = 'ETQ000001016'
and pl.id_ProductionLotState not in (11,13)

/*
	EXEC [dbo].[spPar_DetalleDeReproceso] @estado=0,@unidad=N'',@proceso=N'',@fi=N'',@ff=N''
*/
GO
