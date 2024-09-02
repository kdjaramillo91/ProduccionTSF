--exec spPar_LiquidacionCarroXCarro @id=986454,@codeProcessType=N'ENT'
GO
/****** Object:  StoredProcedure [dbo].[par_LiquidacionCarroCarro]    Script Date: 15/02/2023 02:50:44 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

-- ProductionProcess SubProcessIOProductionProcess SubProcess LiquidationCartOnCartDetail LiquidationCartOnCart

Create Procedure [dbo].[spPar_LiquidacionCarroXCarro]
(
	@id integer
	, @codeProcessType varchar(20)
)
As
set nocount on

declare @NumeroLote		varchar(50)
declare @NombreLiquidador varchar(1000)

select top 1 @NombreLiquidador = pr.[fullname_businessName]
from [dbo].[LiquidationCartOnCart] lc 
join [dbo].[Person] pr on lc.[id_liquidator] = pr.[id]
where lc.[id] = @id

select distinct
	lc.[id] as [IdLiquidacion]
	, pl.[internalNumber] as [NumeroLote]
	, lcd.[id_ProductionCart] as [IdCarro]
	, pc.[code] as [NombreCarro]
	, lcd.[id_ItemLiquidation] as [IdProductoPrimario]
	, convert(varchar(250),itp.[name] + ' / '+ person.fullname_businessName + ' / ' + convert (varchar(250),PDPR.[name]) ) as [NombreProductoPrimario]----------------------------------------------------
	, itgp.[id_size] as [IdTallaProductoPrimario]
	, '(' + CONVERT(VARCHAR(50),itsp.[name]) + ')' as [NombreTallaProductoPrimario]
	, ispt.[id_ProcessType] as [IdTipoProceso]
	, Round(sum(lcd.[quatityBoxesIL]),2) as [CajasCantidad] 
	, lc.id_MachineForProd as [MaquinaNo] --------------------------------------EV
	, (sum(lcd.[quatityBoxesIL]) * (pre.[minimum])) as TotalKilos
	--,(sum(lcd.[quatityBoxesIL] * pre.[minimum])) *(2.2046) as TotalLibras
	, sum(Round(((lcd.[quatityBoxesIL])) * (2.2046 * pre.minimum),2)) as TotalLibras  
	,lcd.[quatityBoxesIL],pre.[minimum]
	, pupp.name as [Piscina]-------------------------EV
	, pl.receptionDate as [FechaRecepcion] -------------ev
	, pl.totalQuantityRecived as [LibrasDespachadas]-------------------ev
	, lpr.LibrasProcesadas as [LibrasProcesadas]
	, per.fullname_businessName as [Proveedor]----------------ev
	, Convert(varchar,lc.dateInit,103) [FechaProceso]----------------------ev
	,Convert(varchar,lc.dateEnd,103) [FechaProcesoFin]
	--, lc.dateEnd [FechaProcesoFin]----------------------ev
	--, lc.timeInit [HoraInicio]----------------------ev
	,Convert(varchar,lc.timeInit,108) HoraInicio
	,Convert(varchar,lc.timeEnd,108) HoraFin
	--, lc.timeEnd [HoraFin]----------------------ev
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
	, docState.[name] as Estado
	, @NombreLiquidador as NombreLiquidador
	,convert (varchar(250),PDPR.[name]) as Proceso_Producto--------------------------EV-------------------------
	,itp.id_itemType as idTipoProducto
	,Isnull(itc.name,'') as nombreProductoCategory
	,ISNULL(Virt1.nameFilter, '') AS nameItemType
Into #DetalleCarroxCarro
from [dbo].[LiquidationCartOnCart] lc
join [dbo].[ProductionLot] pl on lc.[id_productionLot] = pl.[id]
join [dbo].[LiquidationCartOnCartDetail] lcd on lc.[id] = lcd.[id_LiquidationCartOnCart]
--------------------------------------------------------------------------------------------
join [dbo].[SubProcessIOProductionProcess] subpr on subpr.id = lcd.id_subProcessIOProductionProcess
join ProductionProcess PDPR on subpr.id_productionprocess = PDPR.id -----------------------EV-------------
--------------------------------------------------------------------------------------------
join [dbo].[Item] itp on lcd.[id_ItemLiquidation] = itp.[id]
join [dbo].[ItemGeneral] itgp on itp.[id] = itgp.[id_item] 
join [dbo].[ItemSize] itsp on itgp.[id_size] = itsp.[id]
join [dbo].[ItemSizeProcessType] ispt on itsp.[id] = ispt.[id_itemSize] 
join [dbo].[ProcessType] pt on ispt.[id_ProcessType] = pt.[id]
join [dbo].[ItemTypeCategory] itc on itc.id = itp.id_itemTypeCategory
join [dbo].[ItemTypeCategoryClassRelation] Ctr ON Ctr.id_ItemTypeCategory = itp.id_itemTypeCategory
join [dbo].[Class] css on css.id = Ctr.id_Class 
join [dbo].[Presentation] pre on itp.[id_presentation] = pre.[id]
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
left join (SELECT itePr.id_ItemSize AS 'idItemSize',
				prTy.id	AS 'idProcessType',
				prTy.code AS 'codeProcessType',
				clas.id AS 'idClass',
				clas.code AS 'codeClass',
				prTy.name AS 'nameProcessType',
				CASE WHEN prTy.code = 'ENT' AND clas.code = 'PRIM' THEN 'AENTERO'
				WHEN prTy.code = 'COL' AND clas.code IN ('PRIM') THEN 'BCOLAPRIMERA'
				WHEN prTy.code = 'COL' AND clas.code IN ('SEGU') THEN 'CCOLASEGUNDA'
				WHEN prTy.code = 'COL' AND clas.code IN ('VETL') THEN 'DVENTALOCAL'
				WHEN prTy.code = 'COL' AND clas.code IN ('BROK') THEN 'EBROKEN'
				ELSE '' END AS 'nameFilter'
				FROM ItemSizeProcessPLOrder itePr
				LEFT OUTER JOIN ProcessType prTy
				 ON itePr.id_ProcessType = prTy.id
				LEFT OUTER JOIN class clas
				 ON clas.id = itePr.id_class) AS Virt1
	ON Virt1.idItemSize = itgp.id_size
	AND Virt1.idClass = css.id
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
	, pl.totalQuantityRecived
	, lpr.LibrasProcesadas
	, per.fullname_businessName
	, person.fullname_businessName
	, Convert(varchar,lc.dateInit,103)
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
	, docState.[name]
	,lcd.[quatityBoxesIL],pre.[minimum]
	,PDPR.[name]
	--,convert (varchar(250),PDPR.[name])
	,itsp.id
	,itp.id_itemType
	,itc.name
	,Virt1.nameFilter
union all
select distinct
	lc.[id] as [IdLiquidacion]
	, pl.[internalNumber] as [NumeroLote]
	, lcd.[id_ProductionCart] as [IdCarro]
	, pc.[code] as [NombreCarro]
	, lcd.[id_ItemLiquidation] as [IdProductoPrimario]
	, convert(varchar(250),itp.[name]  + ' / '+ person.fullname_businessName + ' / ' + convert (varchar(250),PDPR.[name])) as [NombreProductoPrimario]
	, 0
	, case when  pt.[code] = 'COL'THEN CONVERT(VARCHAR(250),'LBS')
	  ELSE CONVERT(VARCHAR(250),'KGS') END
	, 0
	, case when pt.[code] = 'ENT'THEN Round(sum(lcd.[quatityBoxesIL] * pre.[minimum]),2)
	  ELSE round((sum(lcd.[quantityPoundsIL])),2) END as [CajasCantidad]
	, sum(lcd.[quatityBoxesIL] * pre.[minimum]) as TotalKilos
	, sum(((lcd.[quatityBoxesIL])) * (pre.[minimum]) * (2.2046)) as TotalLibras
	,lcd.[quatityBoxesIL],pre.[minimum]
	, lc.id_MachineForProd as [MaquinaNo] --------------------------------------EV
	, pupp.name as [Piscina]-------------------------EV
	, pl.receptionDate as [FechaRecepcion] -------------ev
	, pl.totalQuantityRecived as [LibrasDespachadas]-------------------ev
	, lpr.LibrasProcesadas as [LibrasProcesadas]
	, per.fullname_businessName as [Proveedor]----------------ev
	, Convert(Varchar,lc.dateInit,103) [FechaProceso]----------------------ev
	,Convert(varchar,lc.dateEnd,103) [FechaProcesoFin]
	--, lc.dateEnd [FechaProcesoFin]----------------------ev
	--, lc.timeInit [HoraInicio]----------------------ev
	,Convert(varchar,lc.timeInit,108) HoraInicio
	--, lc.timeEnd [HoraFin]----------------------ev
		,Convert(varchar,lc.timeEnd,108) HoraFin
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
	, docState.[name] as Estado
	, @NombreLiquidador as NombreLiquidador
	,convert (varchar(250),PDPR.[name]) as Proceso_Producto
	,itp.id_itemType as idTipoProducto
	,Isnull(itc.name,'') as nombreProductoCategory
	,ISNULL(Virt1.nameFilter, '') AS nameItemType
from [dbo].[LiquidationCartOnCart] lc
join [dbo].[ProductionLot] pl on lc.[id_productionLot] = pl.[id]
join [dbo].[LiquidationCartOnCartDetail] lcd on lc.[id] = lcd.[id_LiquidationCartOnCart]
--------------------------------------------------------------------------------------------
join [dbo].[SubProcessIOProductionProcess] subpr on subpr.id = lcd.id_subProcessIOProductionProcess
join ProductionProcess PDPR on subpr.id_productionprocess = PDPR.id -----------------------EV-------------
--------------------------------------------------------------------------------------------
join [dbo].[Item] itp on lcd.[id_ItemLiquidation] = itp.[id]
join [dbo].[ItemGeneral] itgp on itp.[id] = itgp.[id_item] 
join [dbo].[ItemSize] itsp on itgp.[id_size] = itsp.[id]
join [dbo].[ItemSizeProcessType] ispt on itsp.[id] = ispt.[id_itemSize] 
join [dbo].[ProcessType] pt on ispt.[id_ProcessType] = pt.[id]
join [dbo].[ItemTypeCategory] itc on itc.id = itp.id_itemTypeCategory
join [dbo].[ItemTypeCategoryClassRelation] Ctr ON Ctr.id_ItemTypeCategory = itp.id_itemTypeCategory
join [dbo].[Class] css on css.id = Ctr.id_Class 
join [dbo].[Presentation] pre on itp.[id_presentation] = pre.[id]
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
left join (SELECT itePr.id_ItemSize AS 'idItemSize',
				prTy.id	AS 'idProcessType',
				prTy.code AS 'codeProcessType',
				clas.id AS 'idClass',
				clas.code AS 'codeClass',
				prTy.name AS 'nameProcessType',
				CASE WHEN prTy.code = 'ENT' AND clas.code = 'PRIM' THEN 'AENTERO'
				WHEN prTy.code = 'COL' AND clas.code IN ('PRIM') THEN 'BCOLAPRIMERA'
				WHEN prTy.code = 'COL' AND clas.code IN ('SEGU') THEN 'CCOLASEGUNDA'
				WHEN prTy.code = 'COL' AND clas.code IN ('VETL') THEN 'DVENTALOCAL'
				WHEN prTy.code = 'COL' AND clas.code IN ('BROK') THEN 'EBROKEN'
				ELSE '' END AS 'nameFilter'
				FROM ItemSizeProcessPLOrder itePr
				LEFT OUTER JOIN ProcessType prTy
				 ON itePr.id_ProcessType = prTy.id
				LEFT OUTER JOIN class clas
				 ON clas.id = itePr.id_class) AS Virt1
	ON Virt1.idItemSize = itgp.id_size
	AND Virt1.idClass = css.id
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
	, itp.[name]
	, pl.receptionDate
	, pl.totalQuantityRecived
	, lpr.LibrasProcesadas
	, per.fullname_businessName
	, person.fullname_businessName
	, Convert(varchar,lc.dateInit,103)
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
	, docState.[name]
	,lcd.[quatityBoxesIL],pre.[minimum]
	,PDPR.[name]
	--,convert (varchar(250),PDPR.[name])
	,itsp.id
	,itp.id_itemType
	,itc.name 
	,Virt1.nameFilter
If @codeProcessType = 'ENT'
Begin
Select * from #DetalleCarroxCarro
Union all
select distinct
	lc.[id] as [IdLiquidacion]
	, pl.[internalNumber] as [NumeroLote]
	, lcd.[id_ProductionCart] as [IdCarro]
	, pc.[code] as [NombreCarro]
	, lcd.[id_ItemLiquidation] as [IdProductoPrimario]
	, convert(varchar(250),itp.[name]  + ' / '+ person.fullname_businessName + ' / ' + PDPR.[name]) as [NombreProductoPrimario]
	, 0
	,  CONVERT(VARCHAR(250),'LBS') 
	, 0
	, round(sum(Round((lcd.[quatityBoxesIL]) * (2.2046 * pre.minimum),2)),2) as [CajasCantidad]
	, sum(lcd.[quatityBoxesIL] * pre.[minimum]) as TotalKilos
	, sum(((lcd.[quatityBoxesIL])) * (pre.[minimum]) * (2.2046)) as TotalLibras
	,lcd.[quatityBoxesIL],pre.[minimum]
	, lc.id_MachineForProd as [MaquinaNo] --------------------------------------EV
	, pupp.name as [Piscina]-------------------------EV
	, pl.receptionDate as [FechaRecepcion] -------------ev
	, pl.totalQuantityRecived as [LibrasDespachadas]-------------------ev
	, lpr.LibrasProcesadas as [LibrasProcesadas]
	, per.fullname_businessName as [Proveedor]----------------ev
	, Convert(Varchar,lc.dateInit,103) [FechaProceso]----------------------ev
	,Convert(varchar,lc.dateEnd,103) [FechaProcesoFin]
	--, lc.dateEnd [FechaProcesoFin]----------------------ev
	--, lc.timeInit [HoraInicio]----------------------ev
	,Convert(varchar,lc.timeInit,108) HoraInicio
	--, lc.timeEnd [HoraFin]----------------------ev
		,Convert(varchar,lc.timeEnd,108) HoraFin
	, pt.name [NombreProceso]------------------ev
	, pup.name as Camaronera--------------------------------ev
	, turn.name as Turno
	, sum(PL.subtotalTail) as TotalLibras2
	, document.description as [Observacion]------------------ev
	, convert(varchar(250),pl.number) as SecTransaccional---------------------------ev
	, pt.[code] as Codigo
	, document.number As NumeroLiq
	, maq.[name] As Maquina
	, 9999 As Ordersize
	, docState.[name] as Estado
	, @NombreLiquidador as NombreLiquidador
	, convert(varchar(150),PDPR.[name])  as Proceso_Producto-----------------------------
	,itp.id_itemType as idTipoProducto
	,Isnull(itc.name,'') as nombreProductoCategory
	,ISNULL(Virt1.nameFilter, '') AS nameItemType
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
join [dbo].[ItemTypeCategoryClassRelation] Ctr ON Ctr.id_ItemTypeCategory = itp.id_itemTypeCategory
join [dbo].[Class] css on css.id = Ctr.id_Class 
join [dbo].[ProcessType] pt on ispt.[id_ProcessType] = pt.[id]
join [dbo].[ItemTypeCategory] itc on itc.id = itp.id_itemTypeCategory
join [dbo].[Presentation] pre on itp.[id_presentation] = pre.[id]
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
left join (SELECT itePr.id_ItemSize AS 'idItemSize',
				prTy.id	AS 'idProcessType',
				prTy.code AS 'codeProcessType',
				clas.id AS 'idClass',
				clas.code AS 'codeClass',
				prTy.name AS 'nameProcessType',
				CASE WHEN prTy.code = 'ENT' AND clas.code = 'PRIM' THEN 'AENTERO'
				WHEN prTy.code = 'COL' AND clas.code IN ('PRIM') THEN 'BCOLAPRIMERA'
				WHEN prTy.code = 'COL' AND clas.code IN ('SEGU') THEN 'CCOLASEGUNDA'
				WHEN prTy.code = 'COL' AND clas.code IN ('VETL') THEN 'DVENTALOCAL'
				WHEN prTy.code = 'COL' AND clas.code IN ('BROK') THEN 'EBROKEN'
				ELSE '' END AS 'nameFilter'
				FROM ItemSizeProcessPLOrder itePr
				LEFT OUTER JOIN ProcessType prTy
				 ON itePr.id_ProcessType = prTy.id
				LEFT OUTER JOIN class clas
				 ON clas.id = itePr.id_class) AS Virt1
	ON Virt1.idItemSize = itgp.id_size
	AND Virt1.idClass = css.id
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
	, itp.[name]
	, pl.receptionDate
	, pl.totalQuantityRecived
	, lpr.LibrasProcesadas
	, per.fullname_businessName
	, person.fullname_businessName
	, Convert(varchar,lc.dateInit,103) 
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
	, docState.[name]
	,lcd.[quatityBoxesIL],pre.[minimum]
	,PDPR.[name]
	--,convert(varchar(250),PDPR.[name])
	,itsp.id
	,itp.id_itemType
	,itc.name
	,Virt1.nameFilter
End
Else
Begin 
Select * from #DetalleCarroxCarro 
order by nameItemType, orderSize

End
--exec [par_LiquidacionCarroCarro] 535742,'ent'

