IF OBJECT_ID('inv_Consultar_InventoryMovePlantTransferPendingNew_StoredProcedure') IS NULL
EXEC('CREATE PROCEDURE inv_Consultar_InventoryMovePlantTransferPendingNew_StoredProcedure AS')

GO
ALTER   Procedure [dbo].[inv_Consultar_InventoryMovePlantTransferPendingNew_StoredProcedure]
As
BEGIN
	
	select LCOCD.id as id_liquidationCartOnCartDetail, DOC.number as numberLiquidationCartOnCart, FORMAT (DOC.emissionDate, 'dd-MM-yyyy') as emissionDateStr,
	   DOC.emissionDate as emissionDate, LCOC.id_MachineForProd as id_machineForProd, MFP.[name] as machineForProd, LCOCD.id_productionCart as id_productionCart,
	   PC.[name] as productionCart, T.[name] as turn, LCOC.idProccesType as id_processType, IIF(PT.code='ENT', 'ENTERO', 'COLA') as processType,
	   PL.internalNumber as numberLot, P.processPlant as processPlant, P1.fullname_businessName as [provider], LCOCD.id_Client as id_customer,
	   IIF(LCOCD.id_Client IS NULL, 'SIN CLIENTE', P0.fullname_businessName) as customer, I.[name] as itemWarehouse, (LCOCD.quatityBoxesIL - IIF(LCOCD.boxesReceived IS NULL, 0.00, LCOCD.boxesReceived)) as box
FROM dbo.LiquidationCartOnCartDetail LCOCD WITH (NOLOCK)
INNER JOIN Item I WITH (NOLOCK) ON (I.id = LCOCD.id_ItemToWarehouse)
INNER JOIN ProductionCart PC WITH (NOLOCK) ON (PC.id = LCOCD.id_productionCart)
INNER JOIN LiquidationCartOnCart LCOC WITH (NOLOCK) ON (LCOC.id = LCOCD.id_LiquidationCartOnCart)
INNER JOIN ProductionLot PL WITH (NOLOCK) ON (PL.id = LCOC.id_ProductionLot)
INNER JOIN dbo.Person P1 WITH (NOLOCK) ON (P1.id = PL.id_provider)
INNER JOIN ProcessType PT WITH (NOLOCK) ON (PT.id = LCOC.idProccesType)
INNER JOIN MachineProdOpening MPO WITH (NOLOCK) ON (MPO.id = LCOC.id_MachineProdOpening)
INNER JOIN Turn T WITH (NOLOCK) ON (T.id = MPO.id_Turn)
INNER JOIN dbo.MachineForProd MFP WITH (NOLOCK) ON (MFP.id = LCOC.id_MachineForProd)
INNER JOIN dbo.Person P WITH (NOLOCK) ON (P.id = MFP.id_personProcessPlant)
INNER JOIN dbo.Document DOC WITH (NOLOCK) ON (DOC.id = LCOC.id)
INNER JOIN dbo.DocumentState DS WITH (NOLOCK) ON (DS.id = DOC.id_documentState AND DS.code = '01')--01: PENDIENTE
INNER JOIN dbo.SubProcessIOProductionProcess SPIOPP WITH (NOLOCK) ON (SPIOPP.id = LCOCD.id_subProcessIOProductionProcess)
INNER JOIN dbo.ProductionProcess PP WITH (NOLOCK) ON (PP.id = SPIOPP.id_productionProcess AND PP.code = 'CNG')--CNG: CONGELAMIENTO
LEFT JOIN Person P0 WITH (NOLOCK) ON (P0.id = LCOCD.id_Client)
WHERE (LCOCD.boxesReceived IS NULL OR (LCOCD.boxesReceived IS NOT NULL AND LCOCD.boxesReceived < LCOCD.quatityBoxesIL))
ORDER BY id_liquidationCartOnCartDetail
END
