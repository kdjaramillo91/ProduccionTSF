If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'par_ProductionLiquidationFinallyCR'
	)
Begin
	Drop Procedure dbo.par_ProductionLiquidationFinallyCR
End
Go
Create Procedure dbo.par_ProductionLiquidationFinallyCR
(
@id int
)
As 
select [PLP].[id],
	   [PLP].[id_productionLot],
	   [PLP].[id_item],
	   [PLP].[id_metricUnit],
	   [PLP].[quantity],
	   [PLP].[adjustMore],
	   [PLP].[adjustLess],
	   [PLP].[totalMU],
	   [PLP].[price] as precioUnitario,
	   [PLP].[totalToPay] as valortotal,
	   [PLP].[totalPounds],
	   [PLP].[fitPounds],
	   [PLP].[totalProcessMetricUnit] as redimientototal,
	   [PLP].[id_metricUnitProcess],
	   [PLL].[id] as [PLL_id],
	   [PLL].[id_productionLot] as [PLL_id_productionLot],
	   [PLL].[id_item] as [PLL_id_item],
	   [PLL].[id_warehouse] as [PLL_id_warehouse],
	   [PLL].[id_warehouseLocation] as [PLL_id_warehouseLocation],
	   [PLL].[id_salesOrder] as [PLL_id_salesOrder],
	   [PLL].[id_salesOrderDetail],
	   [PLL].[quantity] as [PLL_quantity],
	   [PLL].[id_metricUnit] as [PLL_id_metricUnit],
	   [PLL].[quantityTotal] as [PLL_quantityTotal],
	   [PLL].[id_metricUnitPresentation] as [PLL_id_metricUnitPresentation],
	   [PL].[id] as [PL_id],
	   substring([PL].[number],10,12) as [liquidacion],--------------------------EV
	   [PL].[internalNumber] as [GRecepcion],
	   [PL].[number] AS [GUIARecepcion],------------------------------------EV
	   [PL].[barCode] as [PL_barCode],
	   [PL].[id_ProductionLotState] as [PL_id_ProductionLotState],
	   [PL].[id_productionUnit] as [PL_id_productionUnit],
	   [PL].[id_productionProcess] as [PL_id_productionProcess],
	   [PL].[receptionDate] as [fechaRecepcion],
	   [PL].[id_provider] as [ruc_proveedor],
	   [PL].[id_priceList] as [PL_id_priceList],
	   [PL].[id_buyer] as  [PL_id_buyer],
	   [PL].[id_personRequesting] as [PL_id_personRequesting],
	   [PL].[id_personReceiving] as [PL_id_personReceiving],
	   [PL].[description] as [PL_description],
	   [PL].[reference] as [PL_reference],
	   [PL].[expirationDate] as [PL_expirationDate],
	   [PL].[totalQuantityOrdered] as [PL_totalQuantityOrdered],
	   [PL].[totalQuantityRemitted] as [PL_totalQuantityRemitted],
	   [PL].[totalQuantityRecived] as [PL_totalQuantityRecived],
	   [PL].[totalQuantityLiquidation] as [PL_totalQuantityLiquidation],
	   [PL].[totalQuantityTrash] as [PL_totalQuantityTrash],
	   [PL].[totalQuantityLiquidationAdjust] as [PL_totalQuantityLiquidationAdjust],
	   [PL].[withPrice] as [PL_withPrice],
	   [PL].[pricePerLbs] as [PL_pricePerLbs],
	   [PL].[liquidationDate] as [fechaLiquidacion],
	   [PL].[closeDate] as [PL_closeDate],
	   [PL].[liquidationPaymentDate] as [PL_liquidationPaymentDate],
	   [PL].[id_company] as [PL_id_company],
	   [PL].[id_userCreate] as [PL_id_userCreate],
	   [PL].[dateCreate] as [PL_dateCreate],
	   [PL].[id_userUpdate] as [PL_id_userUpdate],
	   [PL].[dateUpdate] as [PL_dateUpdate],
	   [PL].[id_productionUnitProviderPool] as [PL_id_productionUnitProviderPool],
	   [PL].[id_productionUnitProvider] as [PL_id_productionUnitProvider],
	   [PL].[id_providerapparent] as [PL_id_providerapparent],
	   [PL].[id_processtype] as [PL_id_processtype],
	   [PL].[wholeSubtotal] as [PL_wholeSubtotal],
	   [PL].[subtotalTail] as [PLsubtotalTail],
	   [PL].[wholeGarbagePounds] as [PL_wholeGarbagePounds],
	   [PL].[poundsGarbageTail] as [PL_poundsGarbageTail],
	   [PL].[wholeLeftover] as [PL_wholeLeftover],
	   [PL].[totalAdjustmentPounds] as [PL_totalAdjustmentPounds],
	   [PL].[totalAdjustmentWholePounds] as [PL_totalAdjustmentWholePounds],
	   [PL].[totalAdjustmentTailPounds] as [PL_totalAdjustmentTailPounds],
	   [PL].[wholeSubtotalAdjust] as [PL_wholeSubtotalAdjust],
	   [PL].[subtotalTailAdjust] as [PL_subtotalTailAdjust],
	   [PL].[wholeSubtotalAdjustProcess] as [PL_wholeSubtotalAdjustProcess],
	   [PL].[totalToPay] as [PL_totalToPay],
	   [PL].[wholeTotalToPay] as [PL_wholeTotalToPay],
	   [PL].[tailTotalToPay] as [PL_tailTotalToPay],
	   [PL].[INPnumberPL] as [imp],
	   [PL].[ministerialAgreementPL] as [acuerdoMinisterial],
	   [PL].[tramitNumberPL] as [NumeroTramite],
	   --[QC].QuantityPoundsReceived as [librasDespachadas],
	   [librasDespachadas]  = (SELECT sum(quantitydrained) FROM ProductionLotDetail WHERE id_productionLot = plp.id),
	   [ProductionUnitProviderPool].name as [NamePool],
	   [Person].fullname_businessName as [Nameproveedor],
	   [ItemSize].description as talla,
	   convert(varchar(250),dbo.FUN_GetRemissionGuide(PLP.id_productionLot)) as guiatransporte,
	  convert(varchar(250),dbo.FUN_GetPurchaseOrder(PLP.id_productionLot)) as guia_proveedor,
	   --substring(Item.name,1,12) as nameitem,
	   Item.name as nameitem,
		[Person].identification_number as identity_prov,
		Company.businessName as name_cia,
		Company.ruc as ruc_cia,
		company.address as adreess_cia,
		Company.phoneNumber as telephone_cia,
		PriceL.name as ListaPrecio,
		AP.valueAdvanceTotal	as anticipoEntregado,
		grammage = (select SUM(grammagereference)/COUNT(grammagereference) from QualityControl qcc where qcc.id_lot = PLP.id_productionLot),
		ItemT.name as typename,
		Item.[id_itemTypeCategory],
		convert(varchar(250),isnull(ItemT.description,' ' )+' '+isnull(itemcolor.code,' ')+' '+isnull(ItemTypeCategory.code,' ')) as name_item_short ,--EV
		Item.id_itemType as tipoitem,
		company.logo2 as Logo,
		ItemSize.name as itemsizename,--------------------------------ev
		AP.valueAdvance as Anticipo, ----------------------------------------ev
		PL.wholeGarbagePounds AS wholeGarbagePounds,  ----------------------------------------ev
		PL.poundsGarbageTail AS poundsGarbageTail, ---------------------------------------------EV
		[LibrasDespachadas01]=(select SUM(isnull(quantitydrained,0)) from ProductionLotDetail where id_productionLot =@id),
		PriceL.name as [Lista],
		PL.wholeSubtotal AS LIBRASPROCESADASCABEZA,---------------------------------------------EV
		PL.subtotalTail AS LIBRASPROCESADASCOLA,---------------------------------------------EV
		PL.wholeLeftover  AS SOBRANTECABEZA,---------------------------------------------EV
		PL.TAILLEFTOVER AS SOBRANTECOLA,
		pl.totalQuantityTrash as DESPERDICIOCABEZA,---------------------------------------------EV
		PL.wholeGarbagePounds AS BASURACABEZA,---------------------------------------------EV
		PL.poundsGarbageTail AS BASURACOLA,---------------------------------------------EV
		PL.wholeGarbagePounds + PL.poundsGarbageTail as TOTALCOLA,----------------EV
		convert (varchar(250),CLASS.DESCRIPTION) AS CLASE, ---------------------------------------------EV
		RESTA = PL.wholeleftover - pl.poundsgarbagetail,----------------------------------EV
		INVL.name AS CODIGOINV,-------------------------------------------EV 
		AP.valueAdvanceTotalRounded AS AVANCEROUNDED,---------------------------------EV
		PL.tailLeftOver AS SOBRANTE, ----------------------------------------EV
		[PL].[poundsGarbageTail] as [BASURACOLA],---------------------------EV
		ItemSize.id as ITSID,
		
		DESCRIPCION =CONVERT (VARCHAR(250), (itemt.description+' ' + case when ItemGroup.code is null then '' else ItemGroup.code end
		+ ' ' + case when ItemColor.code is null then '' else ItemColor.code end + ' ' + 
		case when ItemTypeCategory.code is null then ' ' else ItemTypeCategory.code end + ' ' +
		case when itemsize.description is null then '' else itemsize.description end )),
				
		PL.sequentialLiquidation AS SECUENCIAL ------------------------------------EV
		  , isnull(PL.wholeSubtotalAdjust,0) + isnull(PL.wholeLeftover,0) + isnull(PL.wholeGarbagePounds,0) as RecibidasEntero
  , isnull(PL.tailLeftOver, 0)  as RecibidoCola
  , Item.auxCode as CODAUX
  ,FORCOLA = (PL.wholeLeftover - PL.poundsGarbageTail)
  ,FORENTERO = (PL.TAILLEFTOVER - PL.poundsGarbageTail )
  , RendimientoCola = case when ptp.code ='ENT' 
  THEN ISNULL(PL.wholeLeftover,0) ELSE ISNULL(PL.tailLeftOver ,0) END - ISNULL(PL.poundsGarbageTail,0)

  , TotalPagarLote = PL.[totalToPay]
  , itemcolor.[code] as [CodigoColor]
  , plst.[name] as [EstadoDocumento]
  from [dbo].[ProductionLotPayment]   PLP
  inner join [dbo].[ProductionLotLiquidation] PLL
   on [PLL].[id_item] = [PLP].[id_item] and PLP.id_productionLot = PLL.id_productionLot
   inner join [dbo].[ProductionLot] [PL]
       on [PL].[id] = [PLP].[id_productionLot]
       and [PL].[id] = [PLL].[id_productionLot]
   inner join [dbo].[ProductionLotState] plst 
     on PL.id_ProductionLotState = plst.id
   left join AdvanceProvider [AP]
   on ([AP].[id_lot] = [PL].[id])
   --left join QualityControl QC
   --on (QC.[id_lot] = [PLP].[id_productionLot])
   left join Pricelist PriceL
   on ([PriceL].[id] = [PL].[id_priceList])
  inner join Item Item
	   on Item.[id] = [PLP].[id_item]
  inner join ItemType ItemT
	   on Item.[id_itemType] = [ItemT].[id]
  inner join ItemGeneral [ItemGeneral]
		on ItemGeneral.id_item = Item.id
  inner join ItemSize ItemSize
		on ItemSize.id = ItemGeneral.id_size
  left join itemcolor  itemcolor
		on itemcolor.id = ItemGeneral.id_color
  left join itemtypecategory itemtypecategory
		on itemtypecategory.[id] = ITEM.id_itemTypeCategory
  inner join Person Person
	   on [Person].[id] = [PL].id_provider
  inner join ProductionUnitProviderPool ProductionUnitProviderPool
		on ProductionUnitProviderPool.id = [PL].id_productionUnitProviderPool
  inner join Document
	   on Document.id = PLP.[id_productionLot]
  INNER join [dbo].[EmissionPoint] EmissionPoint
	   on [EmissionPoint].[id] = [Document].[id_emissionPoint]
  INNER join [dbo].[Company] Company
	   on Company.[id] = [EmissionPoint].[id_company]       
inner join [dbo].[ProcessType] ptp on PL.[id_processtype] = ptp.[id]

  LEFT JOIN ItemTypeCategoryClassRelation ICCR
	   ON ICCR.id_ItemTypeCategory =  Item.id_itemTypeCategory ---------------------------------EV
	
  inner join ItemGroup ItemGroup
	   ON ItemGroup.id = ItemGeneral.id_group
	   
  LEFT JOIN Class CLASS -------------------------------------------------------EV
	   ON CLASS.id = ICCR.id_Class
	   
  LEFT JOIN InventoryLine INVL
	   ON INVL.ID = Item.id_inventoryLine   
		left outer join ItemSizeProcessPLOrder ispplo
	on ItemSize.id = ispplo.id_ItemSize 
	  where [PLP].id_productionLot = @id 
order by CLASS.dateCreate ASC ,Item.auxCode asc,ItemColor.code asc, ItemSize.[orderSize]
--Item.id_itemTypeCategory,ItemSize.name -----------------------------------------EV

--execute par_ProductionLiquidationFinallyCR 161935

--SELECT * FROM [PRODUCTIONLOT] WHERE NUMBER = 'REC000001146'


--4696.50 - 38=4658.5

--wholeSubtotal + wholeLeftover + wholeGarbagePounds

--select *from Class where name ='entero'
Go
