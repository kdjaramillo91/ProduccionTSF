

/****** Object:  StoredProcedure [dbo].[SP_ResumenLiquidacionesProveedor_Resumen]    Script Date: 21/03/2023 15:31:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO
--exec SP_ResumenLiquidacionesProveedor_Resumen '', '', ''

CREATE OR ALTER PROCEDURE [dbo].[spPar_ResumenLiquidacionesProveedorResumen]
	@proveedor int,
	@fi varchar(10) = '',
	@ff varchar(10) = '',
	@idPersonaProceso	INT = 0	
	
	AS	

	SET NOCOUNT ON

	Declare @cad                  VARCHAR(8000)  
	Declare @cade                 VARCHAR(8000) 
	Declare @cadena               VARCHAR(MAX) 

	Create Table #Lotet (id int)
	Create Table #LoteGuia (id int, guia varchar(Max))
	Create Table #LoteOC (
		id int, 
		oc varchar(100), 
		id_buyer int,
		nameBuyer varchar(500),
		buyerLabel varchar(100)
	)

	Set @cad = ""  
	Set @cad = @cad + "Insert Into #Lotet" + char(13)  
	Set @cad = @cad + "select id from [ProductionLot] pl " + char(13)  
	Set @cad = @cad + "where pl.id_ProductionLotState not in ('11')" + char(13) 
	If @fi <> '' and  @ff <> ''
		Set @Cad = @Cad + "and Convert(Varchar,pl.receptionDate,111) >= '" + Convert(Varchar,@fi ,111) + "' And Convert(Varchar,pl.receptionDate,111) <= '" + Convert(Varchar,@ff,111) + "'" + Char(13)
	If @fi <> '' and  @ff = ''
		Set @Cad = @Cad + "and Convert(Varchar,pl.receptionDate,111) >= '" + Convert(Varchar,@fi ,111) + "'" + Char(13)	 
	If @fi = '' and  @ff <> ''
		Set @Cad = @Cad + "and Convert(Varchar,pl.receptionDate,111) <= '" + Convert(Varchar,@ff,111) + "'" + Char(13)
	If @proveedor Is Not Null and @proveedor <> 0
		Set @Cad = @Cad + "and pl.id_provider = '" + Convert(Varchar,@proveedor) + "'" + Char(13)
	Else 
		Set @Cad = @Cad + " " 
	IF(@idPersonaProceso IS NOT NULL AND @idPersonaProceso <> 0)
	BEGIN
		Set @Cad = @Cad + "AND pl.id_personProcessPlant = '" + Convert(Varchar,@idPersonaProceso) + "'" + Char(13)
	END
	Exec(@cad) 

	
	select * Into #Lotetemp from #Lotet

	DECLARE @guia varchar(200)
	DECLARE @oc varchar(200)
	DECLARE @idBuyer int
	DECLARE @nameBuyer varchar(500)
	DECLARE @buyerLabel varchar(100)

	DECLARE @lote int
	SET @guia = '' 
	SET @oc = '' 
	WHILE (select count(*) from #Lotet) > 0
	BEGIN 

		set @lote = (select TOP 1 id from #Lotet)

		select @guia = COALESCE(@guia + ', ', '') + RTRIM(LTRIM(f.[sequential]))
		from [dbo].[ProductionLotDetailPurchaseDetail] a 
		join [dbo].[RemissionGuideDetail] b on a.[id_remissionGuideDetail] = b.[id]
		join [dbo].[ProductionLotDetail] c on a.[id_productionLotDetail] = c.[id]
		join [dbo].[RemissionGuide] d on b.[id_remisionGuide] = d.[id]
		join [dbo].[ProductionLot] e on c.[id_productionLot] = e.[id]
		join [dbo].[Document] f on d.[id] = f.[id]
		where e.[id] = @lote

		select 
			@OC = COALESCE(@OC + ', ', '') + RTRIM(LTRIM(tp.[sequential])),
			@idBuyer = tp.id_buyer,
			@nameBuyer = tp.nameBuyer,
			@buyerLabel = tp.buyerLabel
		from [dbo].[ProductionLot] pla
		Join (select distinct e.id,f.sequential,
				pb.id as id_buyer,
				pb.fullname_businessName as nameBuyer,
				pgd.buyerLabel
			from [dbo].[ProductionLotDetailPurchaseDetail] a 
			join [dbo].[PurchaseOrderDetail] b on a.[id_purchaseOrderDetail] = b.[id]
			join [dbo].[ProductionLotDetail] c on a.[id_productionLotDetail] = c.[id]
			join [dbo].[PurchaseOrder] d on b.[id_PurchaseOrder] = d.[id]
			join [dbo].[ProductionLot] e on c.[id_productionLot] = e.[id]
			join [dbo].[Document] f on d.[id] = f.[id]
			join [dbo].[Person] pb on d.id_buyer = pb.id
			join [dbo].[ProviderGeneralData] pgd ON pb.id = pgd.id_provider
			where e.[id] = @lote) tp 
		On tp.id = pla.id

		Insert into #LoteGuia 
		select @lote, @guia
		
		Insert into #LoteOc
		select @lote, @oc, @idBuyer, @nameBuyer, @buyerLabel

		SET @guia = '' 
		SET @oc = '' 

		DELETE TOP (1) #Lotet
	END

	select 
		pl.number [Secuencia Transaccional], Isnull(internalnumber,'') [no.lote], pl.receptionDate [Fecha de Recepcion], TFLq.[FechaEmision] [Fecha de Proceso],
		Isnull(ps.processPlant,'') [Proceso], Isnull(ps1.fullname_businessName,'') [Proveedor], Isnull(pu.name,'') [Camaronera], 
		pl.totalQuantityOrdered [Libras Compradas], 
		case 
			when len(oc.oc) > 2	then isnull(RIGHT(oc.oc, LEN(oc.oc) - 2),'') else ''
		end [OC],
		--isnull(RIGHT(oc.oc, LEN(oc.oc) - 2),'') [OC],
		pl.totalQuantityRemitted [Libras Programadas], 
		isnull(RIGHT(gui.guia, LEN(gui.guia) - 2),'') [Guias],
		pl.totalQuantityRecived [Libras Remitidas], 
		isnull((select sum(isnull(quantitydrained,0))
							from [dbo].[ProductionLotDetail] pldTmp3 
							where pldTmp3.[id_productionLot] = pl.[id]),0) [Libras Proyectadas Escurrido], 
		pl.wholeSubtotal + pl.wholeLeftover + wholeGarbagePounds + pl.wholeLeftover [Libras Recibidas],
		pl.wholeSubtotal [Libras Procesadas Entero],  
		pl.subtotalTail [Libras Procesadas Cola], isnull(pl.wholeSubtotal,0) + isnull(pl.subtotalTail,0) [Libras Procesadas],
		Isnull(TEnte.[PoundsProcess],0) [Lbs. Parciales Entero], Isnull(TCola.[PoundsProcess],0) [Lbs. Parciales Cola],
		Isnull(TEnte.[PoundsProcess],0) + Isnull(TCola.[PoundsProcess],0) [Lbs. Parciales],
		(isnull(pl.wholeSubtotal,0) + isnull(pl.subtotalTail,0)) - (Isnull(TEnte.[PoundsProcess],0) + Isnull(TCola.[PoundsProcess],0)) [Lbs. Diferencia],
		Isnull(TEnt.LibrasEntero,0) [Libs liq. Entero], Isnull(TEnt.ValorEntero,0) [US$ Entero], case when Isnull(TEnt.LibrasEntero,0) > 0 then Isnull(TEnt.ValorEntero,0) / Isnull(TEnt.LibrasEntero,0) else 0 end [Promedio US$ Entero],
		Isnull(TCol.LibrasCola,0) [Libs liq. Cola], Isnull(TCol.ValorCola,0) [US$ Cola], case when Isnull(TCol.LibrasCola,0) > 0 then Isnull(TCol.ValorCola,0) / Isnull(TCol.LibrasCola,0) else 0 end [Promedio US$ Cola],
		pl.totalQuantityLiquidationAdjust [Libras Liquidadas], 
		Isnull(TLQ.TotalLiquidacion,0) [Total Liquidación],
		Isnull(pl.sequentialLiquidation,'') [no.Liquidación],
		Isnull(pl.liquidationDate,'') [Fecha Liquidación], Isnull(pll.name,'') [Lista de Precios], pls.name [Estado],
		pl.wholeLeftover [LbsRechazo],	Isnull(pl.wholeGarbagePounds,0) + isnull(pl.poundsGarbageTail,0) [Basura],	
		Round(case when  isnull(pl.[wholeSubtotalAdjust], 0) + isnull(pl.[wholeLeftover], 0) > 0
			 then isnull(pl.[wholeSubtotalAdjust],0)/(isnull(pl.[wholeSubtotalAdjust], 0) + isnull(pl.[wholeLeftover], 0)) else 0 end * 100,2) [Rend.Cab],
		Round(Case when isnull(pl.[tailLeftover],0) = 0 Then		   
						case when (isnull(pl.[wholeLeftover], 0) - isnull(pl.[poundsGarbageTail], 0)) > 0
							 then pl.[subtotalTailAdjust]/(isnull(pl.[wholeLeftover], 0) - isnull(pl.[poundsGarbageTail], 0)) else 0 end 
						Else 
						case when (isnull(pl.[tailLeftover], 0) - isnull(pl.[poundsGarbageTail], 0)) > 0
							 then pl.[subtotalTailAdjust]/(isnull(pl.[tailLeftover], 0) - isnull(pl.[poundsGarbageTail], 0)) else 0 end End * 100,2) [Rend.Col],
		Isnull(Round(((Case when pl.id_processtype = 1 then pl.wholeSubtotal + (PL.wholeleftover - pl.poundsgarbagetail)
		else PL.tailLeftOver - PL.poundsGarbageTail end) / [PL].[totalQuantityRecived] ) * 100,2),0) [Rend.Gral],	
		isnull(Round((select SUM(grammagereference)/COUNT(grammagereference) from QualityControl qcc where qcc.id_lot = PL.id),2),0) [Gramaje],
		Det.KavetasBines [Gavetas/Bines],
		oc.nameBuyer [comprador],
		ISNULL(oc.buyerLabel,'') [etiquetaComprador]
	Into #ResumenLiquidacionSP
	from productionlot pl
	Inner Join #Lotetemp tp1 On Tp1.id = pl.id
	Inner Join Document dc On dc.id = pl.id
	left Join person ps On ps.id = pl.id_personProcessPlant
	left Join person ps1 On ps1.id = pl.id_provider
	left Join productionUnitProvider pu On pu.id = pl.id_productionUnitProvider
	left Join (select id_productionLot, sum(totalToPay) TotalLiquidacion from ProductionLotPayment group by id_productionLot) TLQ On TLQ.id_productionLot = pl.id
	left Join productionLotState PLS On PLS.id = Pl.id_productionLotState
	left Join (select id_productionLot, sum(totalPounds) LibrasEntero, sum(TotalToPay) ValorEntero
				from ProductionLotPayment Py1
				Inner Join Item It1 On It1.id = Py1.id_item
				Inner Join itemType Itt On Itt.id = It1.id_itemType
				where Itt.code = 'C' group by id_productionLot) TEnt On TEnt.id_productionLot = pl.id
	left Join (select id_productionLot, sum(totalPounds) LibrasCola, sum(TotalToPay) ValorCola
				from ProductionLotPayment Py1
				Inner Join Item It1 On It1.id = Py1.id_item
				Inner Join itemType Itt On Itt.id = It1.id_itemType
				where Itt.code = 'S' group by id_productionLot) TCol On TCol.id_productionLot = pl.id
	left Outer Join priceList pll On pll.id = pl.id_priceList
	left Join (select 
			b.[id_productionLot] as [id_productionLot]
			, Month(c.[emissionDate]) as [mesLiquidacion]
			, SUM(a.[quantityPoundsIL]) as [PoundsProcess]
		from [dbo].[LiquidationCartOnCartDetail] a
		join [dbo].[LiquidationCartOnCart] b on a.[id_LiquidationCartOnCart] = b.[id]
		join [dbo].[Document] c on b.[id] = c.[id] 
		join [dbo].[DocumentState] d on c.[id_documentState] = d.[id] and d.[code] not in('05')
		join [dbo].[ProcessType] e on b.[idProccesType] = e.[id]
		where e.Code = 'ENT'
		group by b.[id_productionLot], Month(c.[emissionDate])) TEnte On TEnte.[id_productionLot] = pl.id and Month(pl.receptionDate) = TEnte.[mesLiquidacion]
	Left Join (select 
			b.[id_productionLot] as [id_productionLot]
			, Month(c.[emissionDate]) as [mesLiquidacion]
			, SUM(a.[quantityPoundsIL]) as [PoundsProcess]
		from [dbo].[LiquidationCartOnCartDetail] a
		join [dbo].[LiquidationCartOnCart] b on a.[id_LiquidationCartOnCart] = b.[id]
		join [dbo].[Document] c on b.[id] = c.[id] 
		join [dbo].[DocumentState] d on c.[id_documentState] = d.[id] and d.[code] not in('05')
		join [dbo].[ProcessType] e on b.[idProccesType] = e.[id]
		where e.Code = 'COL'
		group by b.[id_productionLot], Month(c.[emissionDate])) TCola On TCola.[id_productionLot] = pl.id and Month(pl.receptionDate) = TCola.[mesLiquidacion]
	Left Join (select b.[id_productionLot] as [id_productionLot]
			,Min(C.[emissionDate]) as [FechaEmision]
			, Month(c.[emissionDate]) as [mesLiquidacion]
		from [dbo].[LiquidationCartOnCartDetail] a
		join [dbo].[LiquidationCartOnCart] b on a.[id_LiquidationCartOnCart] = b.[id]
		join [dbo].[Document] c on b.[id] = c.[id] 
		join [dbo].[DocumentState] d on c.[id_documentState] = d.[id] and d.[code] not in('05')
		join [dbo].[ProcessType] e on b.[idProccesType] = e.[id]
		group by b.[id_productionLot], Month(c.[emissionDate]))TFLq On TFLq.[id_productionLot] = pl.id and Month(pl.receptionDate) = TFLq.[mesLiquidacion]
	Left Join #LoteGuia gui On gui.id = pl.id
	Left Join #LoteOc oc On oc.id = pl.id
	Left Join (Select id_productionLot, sum(drawersNumber) KavetasBines 
				from productionLotDetail 
				group by id_productionLot) Det On Det.id_productionLot = pl.id
	where pls.code not in ('09')
	and pl.id_productionUnit = 1 and pl.id_productionProcess = 1
	order by pl.receptionDate


	Select	
		Count(*) as [numLiquidaciones],
		[comprador] as 'proveedor',
		SUM([Libras Programadas]) as 'LibrasDespachadas',
		SUM([Libras Remitidas]) as 'LibrasRemitidas',
		SUM([Libras Procesadas]) as 'LibrasProcesadas',

		SUM([Libras Procesadas Entero]) * 0.453592 as 'KilosCabeza',
		SUM([Libras Procesadas Cola]) as 'LibrasCola',
		Round(AVG([Rend.Cab]),2) as 'RendimientoCabeza',
		Round(AVG([Rend.Col]),2) as 'RendimientoCola',
		SUM([US$ Entero]) as 'ValorCabeza',
		SUM([US$ Cola]) as 'ValorCola',
		SUM([Total Liquidación]) as 'dolaresTotal',
		--99999 as 'PrecioCabeza',
		ISNULL((SUM([US$ Entero]) * 0.453592)/NULLIF(SUM([Libras Procesadas Entero]),0),0) as 'PrecioCabeza',
		ISNULL(SUM([US$ Cola])/NULLIF(SUM([Libras Procesadas Cola]),0),0) as 'PrecioCola',
		ISNULL(SUM([Total Liquidación])/NULLIF(SUM([Libras Procesadas]),0),0) as 'PrecioTotal'

	from	#ResumenLiquidacionSP
	GROUP BY [comprador]
	ORDER BY [comprador]
GO


