-- ==============================================================================================================
-- Creado por:		Wilber Leonard Benavides
-- Fecha Creación:	12 de Septiembre del 2021
-- Descripción:		Procedimiento almacenado para consultar las Transferencia en plantas realizadas.
/*
-- Pruebas:			
					EXEC	@return_value = [dbo].[inv_Consultar_InventoryMovePlantTransfer_StoredProcedure]
		@ParametrosBusquedaInventoryMovePlantTransfer = '{"id_machineForProd":0,"number":null,"reference":null,"startEmissionDate":null,
														  "endEmissionDate":null, "id_state":0, "id_productionCart":0, "id_processType":0,
														  "id_provider":0, "id_warehouseEntry":0, "id_warehouseLocationEntry":0, "id_receiver":0, 
														  "numberLot":null, "secTransaction":null, "id_turn":0}'
*/

CREATE PROCEDURE [dbo].[inv_Consultar_InventoryMovePlantTransfer_StoredProcedure]
	@ParametrosBusquedaInventoryMovePlantTransfer		NVARCHAR(MAX)
	--@Resultado							NVARCHAR(MAX) OUTPUT
AS
BEGIN
	
	--==================================================================================================
	-- MAPEO DE PARÁMETROS
	--==================================================================================================
	DECLARE @id_machineForProd			INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.id_machineForProd')
	DECLARE @number						VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.number')
	DECLARE @reference					VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.reference')
	DECLARE @startEmissionDate			DATETIME	    = JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.startEmissionDate')
	DECLARE @endEmissionDate			DATETIME	    = JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.endEmissionDate')
	DECLARE @id_state					INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.id_state')
	DECLARE @id_productionCart			INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.id_productionCart')
	DECLARE @id_processType				INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.id_processType')
	DECLARE @id_provider				INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.id_provider')
	DECLARE @id_warehouseEntry			INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.id_warehouseEntry')
	DECLARE @id_warehouseLocationEntry	INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.id_warehouseLocationEntry')
	DECLARE @id_receiver				INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.id_receiver')
	DECLARE @numberLot					VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.numberLot')
	DECLARE @secTransaction				VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.secTransaction')
	DECLARE @id_turn					INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.id_turn')
	DECLARE @id_inventoryReason			INT 			= JSON_VALUE(@ParametrosBusquedaInventoryMovePlantTransfer, '$.id_inventoryReason')


--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL
	--==================================================================================================
	CREATE TABLE #TmpInventoryMoveDetail
	(
		id int
	)

INSERT INTO #TmpInventoryMoveDetail
SELECT B.id as id
FROM dbo.InventoryMovePlantTransfer B WITH (NOLOCK)
INNER JOIN dbo.InventoryMove C WITH (NOLOCK) ON (C.id = B.id)
LEFT JOIN dbo.InventoryMoveDetail IMD WITH (NOLOCK) ON (IMD.id_inventoryMove = C.id)
LEFT JOIN dbo.Lot LOT WITH (NOLOCK) ON (LOT.id = IMD.id_lot)
LEFT JOIN dbo.ProductionLot PL WITH (NOLOCK) ON (PL.id = LOT.id)
WHERE  
    ( ISNULL(IMD.id_warehouseEntry,0) = ISNULL(@id_warehouseEntry,  ISNULL(IMD.id_warehouseEntry,0)) OR @id_warehouseEntry = 0)
AND ( ISNULL(IMD.id_warehouseLocationEntry,0) = ISNULL(@id_warehouseLocationEntry,  ISNULL(IMD.id_warehouseLocationEntry,0)) OR @id_warehouseLocationEntry = 0)
AND ((CAST(LOT.internalNumber AS VARCHAR(20)) LIKE '%' + @numberLot + '%' OR ISNULL(@numberLot, '') = '') OR
		      (CAST(PL.internalNumber AS VARCHAR(20)) LIKE '%' + @numberLot + '%' OR ISNULL(@numberLot, '') = ''))
		  AND (CAST(LOT.number AS VARCHAR(20)) LIKE '%' + @secTransaction + '%' OR ISNULL(@secTransaction, '') = '')
GROUP BY B.id

--Select * from #TmpInventoryMoveDetail

--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL
	--==================================================================================================
	CREATE TABLE #TmpLiquidationCartOnCartDetail
	(
		id int
	)

INSERT INTO #TmpLiquidationCartOnCartDetail
SELECT B.id as id
FROM dbo.LiquidationCartOnCartDetail LCOCD WITH (NOLOCK)-- ON (LCOCD.id_liquidationCartOnCart = D.id)
--INNER JOIN dbo.LiquidationCartOnCart D WITH (NOLOCK) ON (D.id = LCOCD.id_liquidationCartOnCart)
INNER JOIN dbo.InventoryMovePlantTransferDetail A WITH (NOLOCK) ON (A.id_liquidationCartOnCart = LCOCD.id_liquidationCartOnCart)
INNER JOIN dbo.InventoryMovePlantTransfer B WITH (NOLOCK) ON (B.id = A.id_inventoryMovePlantTransfer)
--LEFT JOIN dbo.Lot LOT WITH (NOLOCK) ON (LOT.id = IMD.id_lot)
--LEFT JOIN dbo.ProductionLot PL WITH (NOLOCK) ON (PL.id = LOT.id)
WHERE  
  (LCOCD.id_ProductionCart = ISNULL(@id_productionCart, LCOCD.id_ProductionCart) OR @id_productionCart = 0)
GROUP BY B.id

--Select * from #TmpLiquidationCartOnCartDetail

DECLARE @id_inventoryReasonAux			INT;
DECLARE @inventoryReasonAux			varchar(MAX);

--INSERT INTO (@id_inventoryReason, @inventoryReason)
SELECT @id_inventoryReasonAux = A.id, @inventoryReasonAux = A.[name]
FROM InventoryReason A
WHERE A.code = 'IPTAPRP'

--SELECT @id_inventoryReason, @inventoryReason
--SELECT IIF(''='', 1, 0)  q

--H.[name] as nameMaterialthirdWarehouse
--I.[name] as [nameMaterialWarehouse]
--J.[name] as nameMaterialthirdWarehouseLocation
--K.[name] as [nameMaterialWarehouseLocation]

SELECT A.id_inventoryMovePlantTransfer as id,  ISNULL(C1.natureSequential, '') as number, IIF(ISNULL(E.isCopackingLot, 0)=1, G.id_materialthirdWarehouse, G.id_materialWarehouse) as id_warehouse,
	   IIF(ISNULL(E.isCopackingLot, 0)=1, H.[name], I.[name]) as warehouse, IIF(ISNULL(E.isCopackingLot, 0)=1, G.id_materialthirdWarehouseLocation,G.id_materialWarehouseLocation) as id_warehouseLocation,
	   IIF(ISNULL(E.isCopackingLot, 0)=1, J.[name], K.[name]) as warehouseLocation, DOC.emissionDate,
	   ISNULL(C1.id_inventoryReason, @id_inventoryReasonAux) as id_inventoryReason, ISNULL(IR.[name], @inventoryReasonAux) as inventoryReason,
	   IEM.id_receiver, ISNULL(PER.fullname_businessName, '') as receiver, DS.[name] as [state], F.id_MachineForProd as id_machineForProdEntry,
	   D.id_MachineForProd as id_machineForProdExit, Convert(BIT, IIF(DS.code='01', 1, 0)) as canEdit, Convert(BIT, IIF(DS.code='01', 1, 0)) as canAproved,
	   Convert(BIT, IIF(DS.code='01', 1, 0)) as canAnnul, Convert(BIT, IIF(DS.code='03', 1, 0)) as canReverse
	FROM dbo.InventoryMovePlantTransferDetail A WITH (NOLOCK)
	INNER JOIN #TmpInventoryMoveDetail TIMD WITH (NOLOCK) ON (TIMD.id = A.id_inventoryMovePlantTransfer)
	INNER JOIN #TmpLiquidationCartOnCartDetail TLCOCD WITH (NOLOCK) ON (TLCOCD.id = A.id_inventoryMovePlantTransfer)
	INNER JOIN dbo.InventoryMovePlantTransfer B WITH (NOLOCK) ON (B.id = A.id_inventoryMovePlantTransfer)
	INNER JOIN dbo.InventoryMove C WITH (NOLOCK) ON (C.id = B.id)
	INNER JOIN dbo.Document DOC WITH (NOLOCK) ON (DOC.id = C.id)
	INNER JOIN dbo.DocumentState DS WITH (NOLOCK) ON (DS.id = DOC.id_documentState)
	INNER JOIN dbo.LiquidationCartOnCart D WITH (NOLOCK) ON (D.id = A.id_liquidationCartOnCart)
	--INNER JOIN dbo.LiquidationCartOnCartDetail LCOCD WITH (NOLOCK) ON (LCOCD.id_liquidationCartOnCart = D.id)
	--INNER JOIN dbo.InventoryMoveDetail IMD WITH (NOLOCK) ON (IMD.id_inventoryMove = C.id)
	--LEFT JOIN dbo.Lot LOT WITH (NOLOCK) ON (LOT.id = IMD.id_lot)
	--LEFT JOIN dbo.ProductionLot PL WITH (NOLOCK) ON (PL.id = LOT.id)
	INNER JOIN dbo.ProductionLot E WITH (NOLOCK) ON (E.id = D.id_ProductionLot)
	INNER JOIN dbo.MachineProdOpeningDetail F WITH (NOLOCK) ON (F.id = B.id_machineProdOpeningDetail)
	INNER JOIN dbo.MachineForProd G WITH (NOLOCK) ON (G.id = F.id_MachineForProd)
	INNER JOIN dbo.MachineProdOpening MPO WITH (NOLOCK) ON (MPO.id = F.id_MachineProdOpening)
	LEFT JOIN dbo.Warehouse H WITH (NOLOCK) ON (H.id = G.id_materialthirdWarehouse)
	LEFT JOIN dbo.Warehouse I WITH (NOLOCK) ON (I.id = G.id_materialWarehouse)
	LEFT JOIN dbo.WarehouseLocation J WITH (NOLOCK) ON (J.id = G.id_materialthirdWarehouseLocation)
	LEFT JOIN dbo.WarehouseLocation K WITH (NOLOCK) ON (K.id = G.id_materialWarehouseLocation)
	LEFT JOIN dbo.InventoryMove C1 WITH (NOLOCK) ON (C1.id = B.id_inventoryMoveEntry)
	LEFT JOIN dbo.InventoryReason IR WITH (NOLOCK) ON (IR.id = C1.id_inventoryReason)
	LEFT JOIN dbo.InventoryEntryMove IEM WITH (NOLOCK) ON (IEM.id = C.id)
	LEFT JOIN dbo.Person PER WITH (NOLOCK) ON (PER.id = IEM.id_receiver)
	WHERE (((DOC.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (DOC.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))
		  AND (DS.id = ISNULL(@id_state, DS.id) OR @id_state = 0)
		  AND (CAST(DOC.reference AS VARCHAR(20)) LIKE '%' + @reference + '%' OR ISNULL(@reference, '') = '')
		  --AND (IMD.id_warehouseEntry = ISNULL(@id_warehouseEntry, IMD.id_warehouseEntry) OR @id_warehouseEntry = 0)
		  --AND (IMD.id_warehouseLocationEntry = ISNULL(@id_warehouseLocationEntry, IMD.id_warehouseLocationEntry) OR @id_warehouseLocationEntry = 0)
		  AND (IEM.id_receiver = ISNULL(@id_receiver, IEM.id_receiver) OR @id_receiver = 0)
		  --AND ((CAST(LOT.internalNumber AS VARCHAR(20)) LIKE '%' + @numberLot + '%' OR ISNULL(@numberLot, '') = '') OR
		  --    (CAST(PL.internalNumber AS VARCHAR(20)) LIKE '%' + @numberLot + '%' OR ISNULL(@numberLot, '') = ''))
		  --AND (CAST(LOT.number AS VARCHAR(20)) LIKE '%' + @secTransaction + '%' OR ISNULL(@secTransaction, '') = '')
		  AND (D.idProccesType = ISNULL(@id_processType, D.idProccesType) OR @id_processType = 0)
		  AND (E.id_provider = ISNULL(@id_provider, E.id_provider) OR @id_provider = 0)
		  AND (D.id_MachineForProd = ISNULL(@id_machineForProd, D.id_MachineForProd) OR @id_machineForProd = 0)
		  AND (MPO.id_Turn = ISNULL(@id_turn, MPO.id_Turn) OR @id_turn = 0)
		  --AND (LCOCD.id_ProductionCart = ISNULL(@id_productionCart, LCOCD.id_ProductionCart) OR @id_productionCart = 0)
		  AND (CAST(ISNULL(C1.natureSequential, '') AS VARCHAR(20)) LIKE '%' + @number + '%' OR ISNULL(@number, '') = '')
		  AND (ISNULL(C1.id_inventoryReason,0) = ISNULL(@id_inventoryReason, ISNULL(C1.id_inventoryReason,0)) OR @id_inventoryReason = 0)
		  GROUP BY  A.id_inventoryMovePlantTransfer, C1.natureSequential, E.isCopackingLot, G.id_materialthirdWarehouse, H.[name],
					G.id_materialWarehouse, I.[name], G.id_materialthirdWarehouseLocation, J.[name], G.id_materialWarehouseLocation, 
					K.[name], DOC.emissionDate, C1.id_inventoryReason, IR.[name], IEM.id_receiver, PER.fullname_businessName, DS.[name],
					F.id_MachineForProd, D.id_MachineForProd, DS.code
		  --having (sum(A.entryAmount) - sum(A.exitAmount)) > 0
END