If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_LiquidacionCarroCarro'
	)
Begin
	Drop Procedure dbo.par_LiquidacionCarroCarro
End
Go
Create Procedure dbo.par_LiquidacionCarroCarro
(
	@id integer
	, @codeProcessType varchar(20)
)
As
set nocount on

declare @NumeroLote		varchar(50)

select 
	lc.[id] as [IdLiquidacion]
	, pl.[internalNumber] as [NumeroLote]
	, lcd.[id_ProductionCart] as [IdCarro]
	, pc.[code] as [NombreCarro]
	, lcd.[id_ItemLiquidation] as [IdProductoPrimario]
	, convert(varchar(250),itp.[name]) as [NombreProductoPrimario]
	, itgp.[id_size] as [IdTallaProductoPrimario]
	, CONVERT(VARCHAR(50),itsp.[name]) as [NombreTallaProductoPrimario]
	, ispt.[id_ProcessType] as [IdTipoProceso]
	, Round(sum(lcd.[quatityBoxesIL]),2) as [CajasCantidad] 
	, lc.id_MachineForProd as [MaquinaNo] --------------------------------------EV
	, (sum(lcd.[quatityBoxesIL]) * (pre.[minimum])) as TotalKilos
	,(sum(lcd.[quatityBoxesIL] * pre.[minimum])) *(2.2046) as TotalLibras
	, pupp.name as [Piscina]-------------------------EV
	, pl.receptionDate as [FechaRecepcion] -------------ev
	, pl.totalQuantityRemitted as [LibrasDespachadas]-------------------ev
	, per.fullname_businessName as [Proveedor]----------------ev
	, lc.dateInit [FechaProceso]----------------------ev
	, lc.dateEnd [FechaProcesoFin]----------------------ev
	, lc.timeInit [HoraInicio]----------------------ev
	, lc.timeEnd [HoraFin]----------------------ev
	, pt.name [NombreProceso]------------------ev
	, pup.name as Camaronera--------------------------------ev
	, turn.name as Turno ----------------------------------ev
	, sum(PL.subtotalTail) as TotalLibras2----------------------ev
	, document.description as [Observacion]------------------ev
	, convert(varchar(250),pl.number) as SecTransaccional---------------------------ev
	, pt.[code] as Codigo
	, document.number As NumeroLiq
	, maq.[name] As Maquina
	, itsp.[ordersize]
from [dbo].[LiquidationCartOnCart] lc
join [dbo].[ProductionLot] pl on lc.[id_productionLot] = pl.[id]
join [dbo].[LiquidationCartOnCartDetail] lcd on lc.[id] = lcd.[id_LiquidationCartOnCart]
join [dbo].[Item] itp on lcd.[id_ItemLiquidation] = itp.[id]
join [dbo].[ItemGeneral] itgp on itp.[id] = itgp.[id_item] 
join [dbo].[ItemSize] itsp on itgp.[id_size] = itsp.[id]
join [dbo].[ItemSizeProcessType] ispt on itsp.[id] = ispt.[id_itemSize] 
join [dbo].[ProcessType] pt on ispt.[id_ProcessType] = pt.[id]
join [dbo].[Presentation] pre on itp.[id_presentation] = pre.[id]
join [dbo].[Item] its on lcd.[id_ItemToWarehouse] = its.[id]
join [dbo].[ProductionCart] pc on lcd.[id_ProductionCart] = pc.[id]
join [dbo].[Person] per on per.[id] = pl.[id_provider]------------------------------------EV
join [dbo].[MachineForProd] maq on maq.[id] = lc.[id_MachineForProd]
left join (Select Distinct id_provider,[name] from [dbo].[ProductionUnitProvider]) pup on pup.[id_provider] = per.[id]-------------------EV
join [dbo].[ProductionUnitProviderPool] pupp on pupp.id = pl.id_productionUnitProviderPool--------------EV
left join [dbo].[MachineProdOpening] mpo on mpo.id = lc.id_MachineProdOpening
left join [dbo].[turn] turn on turn.id = mpo.id_Turn
join document document on document.id = lc.id
where lc.[id] = @id
and pt.[code] = @codeProcessType
group by lc.[id]
	, pl.[internalNumber]
	, lcd.[id_ProductionCart]
	, pc.[code]
	, lcd.[id_ItemLiquidation]
	, itp.[name]
	, itgp.[id_size]
	, itsp.[name]
	, ispt.[id_ProcessType]
	, pupp.name
	, lc.id_MachineForProd
	, pl.receptionDate
	, pl.totalQuantityRemitted
	, per.fullname_businessName
	, lc.dateInit
	, lc.dateEnd 
	, lc.timeInit 
	, lc.timeEnd 
	, pup.name
	, pt.name
	, turn.name
	, pre.[minimum]
	, PL.subtotalTail
	, document.description
	, convert(varchar(250),pl.number)
	, pt.[code]
	, document.number
	, maq.[name]
	, itsp.[ordersize]
union all
select
	lc.[id] as [IdLiquidacion]
	, pl.[internalNumber] as [NumeroLote]
	, lcd.[id_ProductionCart] as [IdCarro]
	, pc.[code] as [NombreCarro]
	, lcd.[id_ItemLiquidation] as [IdProductoPrimario]
	, convert(varchar(250),itp.[name]) as [NombreProductoPrimario]
	, 0
	, case when  pt.[code] = 'COL'THEN CONVERT(VARCHAR(250),'LBS')
	  ELSE CONVERT(VARCHAR(250),'KGS') END
	, 0
	, case when pt.[code] = 'ENT'THEN Round(sum(lcd.[quatityBoxesIL] * pre.[minimum]),2)
	  ELSE round((sum(lcd.[quantityKgsIL])) * (2.2046),2) END as [CajasCantidad]
	, sum(lcd.[quatityBoxesIL] * pre.[minimum]) as TotalKilos
	, sum(((lcd.[quatityBoxesIL])) * (pre.[minimum]) * (2.2046)) as TotalLibras
	, lc.id_MachineForProd as [MaquinaNo] --------------------------------------EV
	, pupp.name as [Piscina]-------------------------EV
	, pl.receptionDate as [FechaRecepcion] -------------ev
	, pl.totalQuantityRemitted as [LibrasDespachadas]-------------------ev
	, per.fullname_businessName as [Proveedor]----------------ev
	, lc.dateInit [FechaProceso]----------------------ev
	, lc.dateEnd [FechaProcesoFin]----------------------ev
	, lc.timeInit [HoraInicio]----------------------ev
	, lc.timeEnd [HoraFin]----------------------ev
	, pt.name [NombreProceso]------------------ev
	, pup.name as Camaronera--------------------------------ev
	, turn.name as Turno
	, sum(PL.subtotalTail) as TotalLibras2
	, document.description as [Observacion]------------------ev
	, convert(varchar(250),pl.number) as SecTransaccional---------------------------ev
	, pt.[code] as Codigo
	, document.number As NumeroLiq
	, maq.[name] As Maquina
	, 999 As Ordersize
from [dbo].[LiquidationCartOnCart] lc
join [dbo].[ProductionLot] pl on lc.[id_productionLot] = pl.[id]
join [dbo].[LiquidationCartOnCartDetail] lcd on lc.[id] = lcd.[id_LiquidationCartOnCart]
join [dbo].[Item] itp on lcd.[id_ItemLiquidation] = itp.[id]
join [dbo].[ItemGeneral] itgp on itp.[id] = itgp.[id_item] 
join [dbo].[ItemSize] itsp on itgp.[id_size] = itsp.[id]
join [dbo].[ItemSizeProcessType] ispt on itsp.[id] = ispt.[id_itemSize] 
join [dbo].[ProcessType] pt on ispt.[id_ProcessType] = pt.[id]
join [dbo].[Presentation] pre on itp.[id_presentation] = pre.[id]
join [dbo].[Item] its on lcd.[id_ItemToWarehouse] = its.[id]
join [dbo].[ProductionCart] pc on lcd.[id_ProductionCart] = pc.[id]
join [dbo].[Person] per on per.[id] = pl.[id_provider]------------------------------------EV
join [dbo].[MachineForProd] maq on maq.[id] = lc.[id_MachineForProd]
left join (Select Distinct id_provider,[name] from [dbo].[ProductionUnitProvider]) pup on pup.[id_provider] = per.[id]-------------------EV
join [dbo].[ProductionUnitProviderPool] pupp on pupp.id = pl.id_productionUnitProviderPool--------------EV
left join [dbo].[MachineProdOpening] mpo on mpo.id = lc.id_MachineProdOpening
left join [dbo].[turn] turn on turn.id = mpo.id_Turn
join document document on document.id = lc.id
where lc.[id] = @id
and pt.[code] = @codeProcessType
group by 
	lc.[id]
	, pl.[internalNumber]
	, lcd.[id_ProductionCart] 
	, pc.[code]
	, lcd.[id_ItemLiquidation]
	, lc.id_MachineForProd
	, pupp.name
	, convert(varchar(250),itp.[name])
	, pl.receptionDate
	, pl.totalQuantityRemitted
	, per.fullname_businessName
	, lc.dateInit 
	, lc.dateEnd 
	, lc.timeInit 
	, lc.timeEnd 
	, pt.name
	, pup.name
	, turn.name
	, pt.[code]
	, pre.[minimum]
	, PL.subtotalTail
	, document.description
	, convert(varchar(250),pl.number) 
	, pt.[code]
	, document.number
	, maq.[name]
--exec [par_LiquidacionCarroCarro] 202755,'col'




Go
