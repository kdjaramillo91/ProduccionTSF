/****** Object:  StoredProcedure [dbo].[spPar_LiquidacionCamaron]    Script Date: 25/04/2023 10:07:10 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROC [dbo].[spPar_LiquidacionCamaron]
(
	@id int  
)
As   
set nocount on
--declare @TmpAdvanceProvider table
create table #TmpAdvanceProvider
(
[id] integer not null ,
[id_Lot] integer,
[id_provider] integer,
[id_protectiveProvider] integer,
[id_productionUnitProvider] integer,
[AdvanceValuePercentageDefault] decimal(6,2),
[AdvanceValuePercentageUsed] decimal(6,2),
[TailPerformanceDefault] decimal(6,2),
[TailPerformanceUsed] decimal(6,2),
[QuantityPoundsReceivedMinimun] decimal(18,6),
[id_CalendarPriceListType] int,
[id_priceList] int,
[id_CalendarPriceList] int,
[id_processType] int,
[id_processTypeFanUse] int,
[id_grammage] int,
[grammageLot] decimal(15,6),
[wholePerformanceLot] decimal(20,6),
[QuantityPoundReceived] decimal(20,6),
[valueAdvance] decimal(20,6),
[valueAdvanceHead] decimal(20,6),
[valueAdvanceTail] decimal(20,6),
[valueAdvanceTotal] decimal(20,6),
[valueAverage] decimal(20,6),
[valueAdvanceRounded] decimal(14,0),
[valueAdvanceTotalRounded] decimal(14,0),
[purchaseOrderDate] datetime,
[diasPlazo] int
)

insert into #TmpAdvanceProvider
select top 1 a.*
from [dbo].[AdvanceProvider] a 
join [dbo].[Document] b on a.[id] = b.[id]
join [dbo].[DocumentState] c on b.[id_documentState] = c.[id]
where a.[id_lot] = @id 
and c.[code] in('03')


select 
   PLL.[id],  
	   --PLP.[price] as [Precio],  
	   --PLP.[totalToPay] as [ValorTotal],  
	   --plp.[totalProcessMetricUnit] as kilolibras2,  
	pl.number as [N],------------------------------------------------EV  
	   PL.[internalNumber] as [Lote],  
	   PL.[receptionDate] as [FechaRecepcion],  
	   PL.[totalQuantityRemitted] as [LibrasRecibidas],  
	   PL.[liquidationDate] as [FechaProceso],  
	   PL.[wholeSubtotalAdjustProcess] as [PL_wholeSubtotalAdjustProcess],  
	   PL.[INPnumberPL] as [InpNumeroProveedor],  
	   PL.[ministerialAgreementPL] as [MinisterialAgreementPLProveedor],  
	   PL.[tramitNumberPL] as [TramitNumberPLProveedor],  
	   [LibrasDespachadas]=(select SUM(isnull(quantitydrained,0)) from ProductionLotDetail where id_productionLot =@id),  
	ProductionUnitProviderPool.[name] as [Piscina],  
	Person.[fullname_businessName] as [Nombreproveedor],  
	dbo.FUN_GetRemissionGuide(PLL.id_productionLot) as [GuiaRemision],  
	ItemSize.name as ItemSizeName,  
	PL.totalToPay AS PLTOTALTOPAY,----------------------------------------EV  
	--PLP.totalToPay AS PLPTOTALTOPAY,--------------------------------------EV  
	PL.wholeGarbagePounds AS CABEZABASURA,----------------------------------EV  
	PL.poundsGarbageTail AS COLABASURA,-------------------------------------EV  
	AP.valueAdvanceTotalRounded AS AVANCEROUNDED,---------------------------------EV  
	Item.name AS nameitem,-----------------------------------------------EV  
	--plp.totalPounds as kilolibras,  
	--plp.quantityPoundsClose as kilolibrasform,  
	 Person.identification_number as [RucProveedor],  
  Company.businessName as [NombreCompania],  
  Company.ruc as [RucCompania],  
  company.address as [DireccionCompania],  
  Company.phoneNumber as [TelefonoCompania],  
  PriceL.name as [Lista],  
  AP.valueAdvanceTotal as [Anticipo],  
  ItemT.name as [NombreTipoItem], -- agrupación por primer campo  
  Item.name as  [NombreCortoDeItem],  
  Item.id_itemType as tipoitem,  
  mu.code as [Unidades],  
  Company.[logo2] as [Logo],  
  person1.fullname_businessName as [Comprador],  
  Document.number as [Documento],  
  PROCTY.name AS TIPOPROCESO,  
 LIBRASREMITIDAS =(select SUM(quantityRecived) from ProductionLotDetail where id_productionLot= @id)  ,
  PLL.quantity AS [CANTIDAD],
  PLL.QUANTITY AS CAJAS,----------------------------------------EV  
  PL.wholeSubtotal AS LIBRASPROCESADASCABEZA,---------------------------------------------EV  
  PL.subtotalTail AS LIBRASPROCESADASCOLA,---------------------------------------------EV  
  PL.wholeLeftover  AS SOBRANTECABEZA,---------------------------------------------EV  
  pl.totalQuantityTrash as DESPERDICIOCABEZA,---------------------------------------------EV  
  PL.wholeGarbagePounds AS BASURACABEZA,---------------------------------------------EV  
  PL.poundsGarbageTail AS BASURACOLA,---------------------------------------------EV  
  PL.wholeGarbagePounds + PL.poundsGarbageTail as TOTALCOLA,----------------EV  
  RESTA = PL.wholeleftover - pl.poundsgarbagetail,  
  PL.tailLeftOver AS SOBRANTE, ----------------------------------------EV  
  [PL].[poundsGarbageTail] as [BASURACOLA],---------------------------EV  
  document.description as [Observacion],  
  (case ap.id_provider when AP.id_protectiveProvider then ' ' else pup.INPnumber end) as [INPnumberAmparante],  
  (case ap.id_provider when AP.id_protectiveProvider then ' ' else PUP.ministerialAgreement end) as [AcuCamarAmparante],  
  (case ap.id_provider when AP.id_protectiveProvider then ' ' else PUP.tramitNumber end) as  [TramAmparante],  
  (case ap.id_provider when AP.id_protectiveProvider then ' ' else p3.fullname_businessName end) as [NombreAmparante],  
  (case ap.id_provider when AP.id_protectiveProvider then ' ' else p3.identification_number end ) as [identificacionamparente],  
  plst.[name] as [EstadoDocumento],
  PL.sequentialLiquidation AS SECUENCIAL ------------------------------------EV
  , isnull(PL.wholeSubtotalAdjust,0) + isnull(PL.wholeLeftover,0) + isnull(PL.wholeGarbagePounds,0) as RecibidasEntero
  , isnull(PL.tailLeftOver, 0) as RecibidoCola
  , PUP.[name] as camaronera
  , case when ItemT.[name] = 'Cola'and (PL.tailleftover - PL.poundsGarbageTail) > 0 then ROUND(((ROUND(pl.subtotalTailAdjust,2) / (PL.tailleftover - PL.poundsGarbageTail))*100),2) else 0 end as PorcCola
  , case when ItemT.[name] = 'Cola'and (PL.wholeLeftover - PL.poundsGarbageTail) > 0 then ROUND(((ROUND(pl.subtotalTailAdjust,2) / (PL.wholeLeftover - PL.poundsGarbageTail))*100),2) else 0 end as PorcColaProductoEntero
  ,RESTA = PL.wholeleftover - pl.poundsgarbagetail
  , [PL].[wholeSubtotal] as [PL_wholeSubtotal]
  , convert (varchar(250),CLASS.DESCRIPTION) AS CLASE
  , ItemSize.ID ITSID
  , Item.auxCode as CODAUX
  , PL.[totalToPay] as [TotalPagarLote]
  , CONVERT(VARCHAR(250),PPL.[processPlant]) + ' ' + case When SUBSTRING(PL.internalNumber, 1, 1) = 'A' Then '/ ASC' Else '' End as [proceso]
from [dbo].[ProductionLotLiquidation] PLL 
inner join [dbo].[ProductionLot] PL on PL.[id] = PLL.[id_productionLot]   
inner join [dbo].[Person] PPL on PPL.[id] = PL.[id_personProcessPlant]
inner join [dbo].[ProductionLotState] plst on PL.id_ProductionLotState = plst.id
left OUTER join #TmpAdvanceProvider AP on PL.[id] = AP.[id_Lot]
left outer join [dbo].[ProductionUnitProvider] PUP on PL.id_productionUnitProvider = PUP.id  
left outer join [dbo].[person] p3 on p3.id = PL.id_providerapparent  
left join [dbo].[Pricelist] [PriceL] on (PriceL.[id] = PL.[id_priceList])  
inner join [dbo].[Item] Item  on Item.[id] = PLL.[id_item]  
inner join [dbo].[ItemType] ItemT on Item.[id_itemType] = ItemT.[id]
left outer join [dbo].[ItemTypeItemProcessType]  itp on itemt.id = itp.idItemType
left outer join [dbo].[processtype] ptt on itp.idProcessType = ptt.id
inner join [dbo].[Presentation] pre on Item.id_presentation = pre.id
inner join [dbo].[MetricUnit] mup on pre.id_metricUnit = mup.id
inner join [dbo].[ItemGeneral] ItemGeneral on ItemGeneral.id_item = Item.id  
inner join [dbo].[ItemSize] ItemSize on ItemSize.id = ItemGeneral.id_size  
left join [dbo].[itemcolor]  itemcolor on itemcolor.id = ItemGeneral.id_size  
left join [dbo].[itemtypecategory] itemtypecategory  on itemtypecategory.id = ItemGeneral.id_size  
inner join [dbo].[MetricUnit] MU on MU.id = PLL.id_metricUnitPresentation
inner join [dbo].[Person]Person on Person.[id] = PL.id_provider  
left join [dbo].[Person] Person1 on Person1.[id] = PL.id_buyer  
inner join [dbo].[ProductionUnitProviderPool] ProductionUnitProviderPool on ProductionUnitProviderPool.id = PL.id_productionUnitProviderPool  
inner join [dbo].[Document] Document on Document.id = PLL.id_productionLot  
iNNER join [dbo].[EmissionPoint] EmissionPoint on EmissionPoint.[id] = Document.[id_emissionPoint]  
INNER join [dbo].[Company] Company on Company.[id] = EmissionPoint.[id_company]  
inner join ProcessType PROCTY  ON PROCTY.id = PL.id_processtype  
--left outer join ItemSizeProcessPLOrder ispplo on ItemSize.id = ispplo.id_ItemSize And PL.id_processtype = ispplo.id_processtype
LEFT JOIN ItemTypeCategoryClassRelation ICCR
	   ON ICCR.id_ItemTypeCategory =  Item.id_itemTypeCategory
LEFT JOIN Class CLASS
	   ON CLASS.id = ICCR.id_Class
where PL.id = @id  
order by CLASS.dateCreate asc,Item.auxCode asc, ItemSize.ID asc , Item.id_itemTypeCategory 
  
/*
	EXEC [dbo].[spPar_LiquidacionCamaron] 985924
*/

GO