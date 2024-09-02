SET NOCOUNT ON
GO

CREATE OR ALTER PROCEDURE [dbo].[spPar_LiquidacionCarroCarroResumen]
(
	@id integer
	, @codeProcessType varchar(20)
)
As
set nocount on

declare @NumeroLote		varchar(50)

select distinct
	convert(varchar(30),itp.masterCode) AS CodigoResumen
	,itsp.name as Talla
	,case when metu.id = 1 then  sum(Round(((lcd.[quatityBoxesIL])) * (2.2046 * pre.minimum),2)) else sum(Round(((lcd.[quatityBoxesIL])) * (pre.minimum),2)) end as TotalLibras  
	--,case when itgp.[id_size] = 1 then sum(Round(((lcd.[quatityBoxesIL])) * (2.2046 * pre.minimum),2)) else sum(Round(((lcd.[quatityBoxesIL])) * (pre.minimum),2)) end as TotalLibras
	, Round(sum(lcd.[quatityBoxesIL]),3) As CajasCantidad
	, convert(varchar(150),PDPR.[name])  as Proceso_Producto-----------------------------
	
from [dbo].[LiquidationCartOnCart] lc
join [dbo].[ProductionLot] pl on lc.[id_productionLot] = pl.[id]
join [dbo].[LiquidationCartOnCartDetail] lcd on lc.[id] = lcd.[id_LiquidationCartOnCart]
----------------------------------------------------------------------------------------
join [dbo].[SubProcessIOProductionProcess] subpr on subpr.id = lcd.id_subProcessIOProductionProcess
join ProductionProcess PDPR on subpr.id_productionprocess = PDPR.id -----------------------EV-------------
--------------------------------------------------------------------------------------------


join [dbo].[Item] itp on lcd.[id_ItemLiquidation] = itp.[id]
join [dbo].[ItemGeneral] itgp on itp.[id] = itgp.[id_item] 
join [dbo].[ItemSize] itsp on itgp.[id_size] = itsp.[id]
join [dbo].[ItemSizeProcessType] ispt on itsp.[id] = ispt.[id_itemSize] 
join [dbo].[ProcessType] pt on ispt.[id_ProcessType] = pt.[id]
join [dbo].[Presentation] pre on itp.[id_presentation] = pre.[id]

join [dbo].metricunit metu on metu.id = pre.id_metricunit

join [dbo].[Item] its on lcd.[id_ItemToWarehouse] = its.[id]
join [dbo].[ProductionCart] pc on lcd.[id_ProductionCart] = pc.[id]
join [dbo].[Person] per on per.[id] = pl.[id_provider]------------------------------------EV
join [dbo].[Person] person on person.[id] = lcd.[id_Client]
join [dbo].[MachineForProd] maq on maq.[id] = lc.[id_MachineForProd]
left join (Select Distinct id, id_provider,[name] from [dbo].[ProductionUnitProvider]) pup on pup.[id_provider] = per.[id] and pup.[id] = pl.id_productionUnitProvider-------------------EV
join [dbo].[ProductionUnitProviderPool] pupp on pupp.id = pl.id_productionUnitProviderPool--------------EV
left join [dbo].[MachineProdOpening] mpo on mpo.id = lc.id_MachineProdOpening
left join [dbo].[turn] turn on turn.id = mpo.id_Turn
join document document on document.id = lc.id
inner join DocumentState docState on docState.id=document.id_documentState
Inner Join (Select lc.[id],Case When @codeProcessType = 'ENT' then round(sum(Round((lcd.[quatityBoxesIL]) * (2.2046 * pre.minimum),2)),2) 
							Else Round(Sum(lcd.quantityPoundsIl),2) End as [LibrasProcesadas] 
			from [dbo].[LiquidationCartOnCart] lc
			join [dbo].[ProductionLot] pl on lc.[id_productionLot] = pl.[id]
			join [dbo].[LiquidationCartOnCartDetail] lcd on lc.[id] = lcd.[id_LiquidationCartOnCart]
			join [dbo].[Item] itp on lcd.[id_ItemLiquidation] = itp.[id]
			join [dbo].[Presentation] pre on itp.[id_presentation] = pre.[id]
			where lc.[id] = @id
			Group By lc.[id])lpr on lpr.[id] = lc.[id]
where lc.[id] = @id
and pt.[code] = @codeProcessType
group by 
	itsp.[id]
	,itgp.[id_size] 
	,itsp.code
	,metu.id
	,PDPR.[name]
	,itp.masterCode
	,itsp.name