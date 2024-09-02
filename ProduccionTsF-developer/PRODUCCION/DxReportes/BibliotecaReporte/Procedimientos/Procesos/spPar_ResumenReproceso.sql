/****** Object:  StoredProcedure [dbo].[spPar_ResumenReproceso]    Script Date: 24/04/2023 04:39:46 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROC [dbo].[spPar_ResumenReproceso]
(
	@estado int =0,
	@unidad varchar(50)='',
	@proceso varchar(50)='',
	@fi varchar(10) = '',
	@ff varchar(10) = ''
)
As 
set nocount on 
if @fi = '' set @fi = null
if @ff = '' set @ff = null

declare @fiDt date
declare @ffDt date

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
and pl.id_ProductionLotState not in (11,13)

Create Table #TempProductionLotLiquidation (
id_productionProcess int,
id_inventoryLine int,
id_itemType int,
librasReproceso decimal(18,6)
)

Create Table #TempProductionLotMateriaPrima (
id_productionProcess int,
id_inventoryLine int,
id_itemType int,
librasEgreso decimal(18,6),
)

Insert Into #TempProductionLotLiquidation
SELECT
pl.id_productionProcess
,It.id_inventoryLine
,It.id_itemType
,Sum(ing.quantityPoundsLiquidation) librasReproceso 


FROM PRODUCTIONLOT PL 
inner join ProductionProcess prp on prp.id = pl.id_productionProcess
Inner Join #TempDocument tp On tp.id = pl.id
LEFT JOIN ProductionLotLiquidation Ing ON Ing.id_productionLot = PL.ID
LEFT JOIN Item It ON It.id = Ing.id_item
group by pl.id_productionProcess, It.id_inventoryLine, It.id_itemType


Insert Into #TempProductionLotMateriaPrima
SELECT
pl.id_productionProcess
,It.id_inventoryLine
,It.id_itemType
,Sum(((ps.maximum * ps.minimum)*Dt.quantityRecived)*2.2046) librasEgreso


FROM PRODUCTIONLOT PL 
inner join ProductionProcess prp on prp.id = pl.id_productionProcess
Inner Join #TempDocument tp On tp.id = pl.id
Inner Join ProductionLotDetail Dt ON Dt.id_productionLot = PL.ID
Inner Join Item It ON It.id = Dt.id_item
Inner Join Presentation Ps On Ps.id = It.id_presentation
group by pl.id_productionProcess, It.id_inventoryLine, It.id_itemType

--select * from #TempProductionLotLiquidation
--select * from #TempProductionLotMateriaPrima 

Select Il.name LineaInventario, PP.name Proceso, Ity.name TipoProducto,PP.id AS G1,Ity.id_inventoryLine as G2,Ity.id as G3,
TP2.librasEgreso,
CONVERT(VARCHAR, @fiDt, 103) AS Fi,
CONVERT(VARCHAR, @ffDt, 103) AS Ff,
TP1.librasReproceso, TP2.librasEgreso - TP1.librasReproceso LibrasBajas, ((TP2.librasEgreso - TP1.librasReproceso)/TP2.librasEgreso)*100 as Porcentaje
from #TempProductionLotLiquidation TP1
Inner Join #TempProductionLotMateriaPrima TP2 On TP2.id_productionProcess = TP1.id_productionProcess
And TP2.id_inventoryLine = TP1.id_inventoryLine
--And TP2.id_itemType = TP1.id_itemType
Inner Join InventoryLine Il On Il.id = TP1.id_inventoryLine
Inner Join ProductionProcess PP On PP.id = TP1.id_productionProcess
Inner Join ItemType Ity On Ity.id = TP1.id_itemType

/*
	exec spPar_ResumenReproceso @estado=0,@unidad=N'',@proceso=N'',@fi=N'2022/02/01',@ff=N'2022/03/31'
*/
GO