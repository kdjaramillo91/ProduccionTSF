GO
/****** Object:  StoredProcedure [dbo].[SP_TransferenciaAutomatica]    Script Date: 14/02/2023 04:37:25 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[spPar_MovimientoInventarioIngresoTransferenciaA] (
@id int )
As 

set nocount on 

--exit es envio entry ingreso

select 
case when cia.businessName like '%PROCAMARONEX%' then
'PROCAMARONEX S.A' 
WHEN  cia.businessName like '%EXPOTUNA%' then
'EXPOTUNA S.A.' 
 when cia.businessName like '%TOTAL%' THEN 
 'TOTAL SEA FOOD S.A.'
 WHEN  cia.businessName like '%PROEXPO%' THEN
 'PROEXPO S.A.' end as Compania
,doc.emissionDate as FechaEmision
,doct.name as TipoDocumento
,docs.name as Estado
,doc.number as NDocumento
,atr.id as ID
--//////---EGRESO-----////----------
,wexit.name as ProductoEnvio 
,wlexit.name as UbicacionBenvio
,irexit.name as Motivoenvio
,ccexit.name as CCostoEnvio
,sccexit.name as SCCostoEnvio
,atr.RequerimentNumber as NRequerimiento
,desp.fullname_businessName as Despachador
,imoex.natureSequential as NMEgreso 
--/////---INGRESO-----///---------
,wentry.name as BodegaIngreso
,wlentry.name as UbicacionBIngreso
,irentry.name as MotivoIngreso
,ccentry.name as CCostoIngreso
,sccentry.name as SCCostoIngreso
,imoen.natureSequential as NMIngreso
--/////detalle//----------------------
,item.masterCode as Codigo
,item.name as Producto
,muinv.name as UnidadMedidaInv
,mumov.name as UnidadMedidaMov
,atrd.quantity as cantidad
,case when pres.id_metricUnit = 4 then
atrd.quantity *pres.minimum*pres.maximum 
else (atrd.quantity *pres.minimum*pres.maximum) *2.2046 end as CntLibras
,case when pres.id_metricUnit = 1 then
atrd.quantity *pres.minimum*pres.maximum 
else (atrd.quantity *pres.minimum*pres.maximum) /2.2046 end as CntKilos
,pl.internalNumber as Lote
,pl.number as SecuenciaTransaccional
,atrd.saldo as Saldo
,ity.name as Tipo
,size.name as Talla

from AutomaticTransfer atr 
inner join warehouse wexit on wexit.id = atr.id_WarehouseExit 
inner join WarehouseLocation wlexit on wlexit.id = atr.id_WarehouseLocationExit
inner join InventoryReason irexit on irexit.id = atr.id_InventoryReasonExit
inner join CostCenter ccexit on ccexit.id = atr.id_CostCenterExit
inner join CostCenter sccexit on sccexit.id = atr.id_SubCostCenterExit
inner join Warehouse wentry on wentry.id = atr.id_WarehouseEntry
inner join WarehouseLocation wlentry on wlentry.id = atr.id_WarehouseLocationExit
inner join InventoryReason irentry on irentry.id = atr.id_InventoryReasonExit
inner join CostCenter ccentry on ccentry.id = atr.id_CostCenterExit
inner join CostCenter sccentry on sccentry.id = atr.id_SubCostCenterExit
inner join document doc on doc.id = atr.id 
inner join DocumentType doct on doct.id = doc.id_documentType
inner join DocumentState docs on docs.id = doc.id_documentState
inner join AutomaticTransferDetail atrd on atrd.id_AutomaticTransfer = atr.id 
inner join item item on item.id = atrd.id_item 
inner join ItemType ity on ity.id = item.id_itemType
inner join MetricUnit muinv on muinv.id = atrd.id_MetricUnitInv 
inner join MetricUnit mumov on mumov.id = atrd.id_MetricUnitMov
inner join Presentation pres on pres.id = item.id_presentation
left join "User" us On us.id = atr.id_Despachador 
left join person desp on desp.id = us.id_employee 
left join ItemGeneral itg on itg.id_item = item.id 
left join ItemSize size on size.id = itg.id_size 
left join EmissionPoint ep on ep.id = doc.id_emissionPoint
left join  Company cia on cia.id = ep.id_company
--inner join MetricUnit mu on mu.id = pres.id_metricUnit
left join productionlot pl on pl.id = atrd.id_Lot
left join inventorymove imoex on imoex.id = atr.id_InventoryMoveExit
left join inventorymove imoen on imoen.id = atr.id_InventoryMoveEntry


where atr.id = @id



--select *from AutomaticTransferDetail


--select *from document where id = 532684