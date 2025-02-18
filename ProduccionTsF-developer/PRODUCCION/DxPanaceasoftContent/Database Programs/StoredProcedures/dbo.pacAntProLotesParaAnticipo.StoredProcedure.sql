If Exists(
	Select	*
	From	sys.procedures
	Where	name = 'pacAntProLotesParaAnticipo'
	)
Begin
	Drop Procedure dbo.pacAntProLotesParaAnticipo
End
Go
Create Procedure dbo.pacAntProLotesParaAnticipo
As
SET NOCOUNT ON

DECLARE @minLibParaAnticipo NUMERIC(15,6)

SELECT @minLibParaAnticipo = valueDecimal
FROM [dbo].[AdvanceParameters] WHERE [code] = 'NMLP'

SELECT @minLibParaAnticipo = ISNULL(@minLibParaAnticipo, 0)

CREATE TABLE #TmpProductionLot
	(
		[id] int,
		[number] varchar(100),
		[internalNumber] varchar(100),
		[lotState] varchar(100),
		[ReceptionDate] datetime,
		[id_provider] int,
		[ProviderName] varchar(100),
		[id_buyer] int,
		[BuyerName] varchar(100),
		[QuantityPoundsReceived] decimal(14,6),
		[ZoneName] varchar(100),
		[SiteName] varchar(100),
		[PersonProcessPlant] varchar(100)
	)

INSERT INTO  #TmpProductionLot
SELECT DISTINCT  a.[id] AS [id]
	, a.[number] AS [number]
	, a.[internalNumber] AS [internalNumber]
	, pls.[name] AS [lotState]
	, a.[receptionDate] AS [ReceptionDate]
	, a.[id_provider] AS [id_provider]
	, pro.[fullname_businessName] AS [ProviderName]
	, a.[id_buyer] AS [id_buyer]
	, com.[fullname_businessName] AS [BuyerName]
	, CONVERT(decimal(12,4),pld.quantityRecived) AS [QuantityPoundsReceived]
	--, CONVERT(decimal(12,4),SUM(pld.quantityRecived)) AS [QuantityPoundsReceived]
	, fzo.[name] AS [ZoneName]
	, fsi.[name] AS [SiteName]
	, ppro.[processPlant] AS [PersonProcessPlant]
FROM [dbo].[ProductionLot] a
JOIN [dbo].[Lot] lo ON a.[id] = lo.[id]
JOIN [dbo].[Document] do ON a.[id] = do.[id]
--JOIN [dbo].[ProductionLotState] pls ON a.[id_ProductionLotState] = pls.[id]
-- AND pls.[code] not in ( '09')
JOIN [dbo].[ProductionLotState] pls 
on pls.[id] = a.[id_ProductionLotState] AND pls.[code] not in ('01','07','08','09')
INNER JOIN [dbo].[ProductionLotDetail] pld ON a.[id] = pld.[id_productionLot]
JOIN [dbo].[QualityControl] c ON a.[id] = c.[id_lot]
JOIN [dbo].[Person] pro ON pro.[id] = a.[id_provider]
JOIN [dbo].[Person] com ON com.[id] = a.[id_buyer]
JOIN [dbo].[Person] ppro ON ppro.[id] = a.[id_personProcessPlant]
LEFT OUTER JOIN [dbo].[ProductionUnitProvider] pup ON a.[id_productionUnitProvider] = pup.[id]
LEFT OUTER JOIN [dbo].[FishingZone] fzo ON pup.[id_FishingZone] = fzo.[id]
LEFT OUTER JOIN [dbo].[FishingSite] fsi ON pup.[id_FishingSite] = fsi.[id]
INNER JOIN [dbo].[ProductionLotDetailQualityControl] pdqc ON pdqc.[id_productionLotDetail] = pld.[id]
				AND pdqc.[id_qualityControl] = c.[id]
WHERE 1=1
	AND NOT EXISTS(SELECT [id_lot] FROM [AdvanceProvider] z
					join [dbo].[Document] doap on z.[id] = doap.[id]
					join [dbo].[DocumentState] dsap on doap.[id_documentState] = dsap.[id] and dsap.[code] not in ('05') 
					WHERE z.[id_Lot] = a.[id] )
	AND lo.[hasAdvanceProvider] <> 1
	--AND NOT EXISTS (SELECT * FROM [dbo].[AdvanceProvider] ap WHERE ap.[id_lot] = a.[id])
--GROUP BY a.[id],a.[number], a.[internalNumber], a.[receptionDate], a.[id_provider], pro.[fullname_businessName]
--	, a.[id_buyer], com.[fullname_businessName], fzo.[name], fsi.[name], ppro.[processPlant], pls.[name]

Select [id]
	, [number]
	, [internalNumber]
	, [lotState]
	, [ReceptionDate]
	, [id_provider]
	, [ProviderName]
	, [id_buyer]
	, [BuyerName]
	, CONVERT(decimal(12,4),SUM([QuantityPoundsReceived])) AS [QuantityPoundsReceived]
	, [ZoneName]
	, [SiteName]
	, [PersonProcessPlant] 
	from #TmpProductionLot
	GROUP BY [id], [number], [internalNumber], [lotState], [ReceptionDate], [id_provider], [ProviderName], [id_buyer], [BuyerName]
	, [ZoneName], [SiteName], [PersonProcessPlant]








Go
