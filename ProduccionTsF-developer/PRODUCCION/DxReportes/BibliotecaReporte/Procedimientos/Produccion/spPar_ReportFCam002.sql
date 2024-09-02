/****** Object:  StoredProcedure [dbo].[ReportFCam003]    Script Date: 01/03/2023 04:50:21 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

-- Query-SP Original: ReportFCam002
CREATE OR ALTER PROCEDURE [dbo].[spPar_ReportFCam002]
	@id INT,
	@processType VARCHAR(250)
AS 

	SET NOCOUNT ON

	DECLARE @detalleItem TABLE(
		idItem INT,
		Marca VARCHAR(150),
		Talla VARCHAR(150),
		Empaque VARCHAR(150),
		Color VARCHAR(150),
		Peso VARCHAR(150),
		Orden INT
	)

	DECLARE @detalleMovimientos TABLE(
		idMachineProdOpeningDetail INT,
		FechaInicio DATETIME,
		Lote VARCHAR(100),
		Color VARCHAR(150),
		Empaque VARCHAR(150),
		Talla VARCHAR(150),
		Marca VARCHAR(150),
		Cliente VARCHAR(250),
		NoCoche VARCHAR(100),
		Cajas NUMERIC(13,4),
		Peso VARCHAR(150),
		Orden INT
	)

	INSERT INTO @detalleItem
		SELECT 
			i.id idItem,
			it.name Marca,
			its.name Talla,
			itm.name Empaque,
			ic.name Color,
			p.name Peso,
			its.orderSize
		FROM Item i
		INNER JOIN Presentation p
			ON p.id = i.id_presentation
		LEFT OUTER JOIN ItemGeneral ig
			ON ig.id_item = i.id
		LEFT JOIN ItemTrademark it
			ON it.id = ig.id_trademark
		LEFT JOIN ItemSize its
			ON its.id = ig.id_size
		LEFT JOIN ItemColor ic
			ON ig.id_color = ic.id
		LEFT JOIN ItemWeight iw
			ON iw.id = i.id_itemWeight
		LEFT JOIN ItemTrademarkModel itm
			ON itm.id = ig.id_trademarkModel

		INSERT INTO @detalleMovimientos
		select	DISTINCT	
			impt.id_machineProdOpeningDetail idMachineProdOpeningDetail,
			--imdc.emissionDate FechaInicio,
			impt.dateTimeEntry FechaInicio,
			pl.internalNumber Lote,
			dit.Color Color,
			dit.Empaque Empaque,
			dit.Talla Talla,
			dit.Marca Marca,
			ISNULL( pe.fullname_businessName,"SIN CLIENTE")  cliente,
			pc.code NoCoche,
			ISNULL(imd.amountMove,0) - 	ISNULL(imdt.quantity,0) Cajas,
			dit.Peso Peso,
			dit.Orden Orden
from	
		-- SECCION 1 FILTRO 
		-- ====================================
		InventoryMovePlantTransfer impt
		INNER JOIN MachineProdOpeningDetail mpod
		ON impt.id_machineProdOpeningDetail = mpod.id
		INNER JOIN ClosePlateTunnelCool cptp
		ON cptp.id_MachineProdOpeningDetail = impt.id_machineProdOpeningDetail
		-- ====================================
		INNER JOIN InventoryMovePlantTransferDetail imptd  -- CAR N CART
		ON imptd.id_inventoryMovePlantTransfer = impt.id
		INNER JOIN Document imptdc
		ON	imptdc.id = impt.id
		INNER JOIN DocumentState imptds
		ON imptds.id = imptdc.id_documentState

		inner join LiquidationCartOnCart lcc
		on lcc.id = imptd.id_liquidationCartOnCart
		inner join LiquidationCartOnCartDetail lccd
		on lccd.id_LiquidationCartOnCart = lcc.id
		
		INNER JOIN ProductionCart pc
		ON pc.id = lccd.id_ProductionCart
		INNER JOIN ProcessType pt
		ON pt.id = lcc.idProccesType
		INNER JOIN ProductionLot pl
		ON pl.id = lcc.id_ProductionLot
		-- INVENTARIO
		-- ====================================
			INNER JOIN InventoryMoveDetail imd
			ON imd.id_inventoryMove = impt.id
			and imd.id_productionCart = lccd.id_ProductionCart
			and imd.id_item = lccd.id_ItemToWarehouse
			INNER JOIN Document imdc
			ON imdc.id = imd.id_inventoryMove
			LEFT JOIN InventoryMoveDetailTransfer imdt
			ON imdt.id_inventoryMoveDetailEntry = imd.id
		---- LEFT JOIN ITEMS 
		-- ====================================
		LEFT JOIN @detalleItem dit
		ON dit.idItem = imd.id_item
		-- ====================================
		---- CLIENTE
		LEFT JOIN Person pe 
		ON pe.id	= lccd.id_Client
		-- ====================================
		WHERE pt.code = 'ent'
			  AND cptp.id = @id
			  AND imptds.code <> '05'
			  and lccd.id_ItemToWarehouse = imd.id_item
		-- ====================================

	SELECT 
		--====================================
		--				Cabecera
		--====================================
		d.emissionDate FechaEmision,
		ds.name TipoDocumento,
		CASE WHEN ISNULL(dm.FechaInicio, '') = '' THEN '' ELSE CONVERT(VARCHAR(50), dm.FechaInicio, 20) END HoraInicio,
		cptp.closeDate HFinal,
		mfp.name Tunel,
		--====================================
		--				Detalle
		--====================================
		dm.Lote Lote,
		dm.Marca Marca,
		dm.Cliente Cliente,
		dm.Talla Talla,
		dm.Peso Peso,
		dm.Color Color,
		dm.NoCoche NoCoche,
		dm.Empaque Empaque,
		dm.Cajas Cajas,
		t.name Turno,
		(SELECT TOP(1) C.logo 
		FROM Document d
		INNER JOIN EmissionPoint ep ON d.id_emissionPoint = ep.id
		INNER JOIN Company c ON c.id = ep.id_company
		WHERE d.id = @id) Logo
	FROM ClosePlateTunnelCool cptp
	INNER JOIN Document d
		ON d.id = cptp.id
	INNER JOIN DocumentState ds
		ON ds.id = d.id_documentState
	INNER JOIN MachineProdOpeningDetail mpod
		ON mpod.id = cptp.id_MachineProdOpeningDetail
	INNER JOIN MachineProdOpening mpo
		ON mpo.id = mpod.id_MachineProdOpening
	INNER JOIN Turn t
		ON t.id = mpo.id_Turn
	INNER JOIN MachineForProd mfp
		ON mfp.id = mpod.id_MachineForProd
	LEFT JOIN @detalleMovimientos dm
		ON dm.idMachineProdOpeningDetail = mpod.id
    
	WHERE cptp.id = @id
	--------------------------------
	ORDER BY dm.Lote, dm.Marca, dm.Orden

/*
	EXEC spPar_ReportFCam002 641761, 'ENT'
*/
GO