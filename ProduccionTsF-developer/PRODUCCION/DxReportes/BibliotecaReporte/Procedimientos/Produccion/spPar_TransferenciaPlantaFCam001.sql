GO
/****** Object:  StoredProcedure [dbo].[ReportFCam001]    Script Date: 02/02/2023 14:04:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[spPar_TransferenciaPlantaFCam001]
	@id INT
AS
	SET NOCOUNT ON
	DECLARE @idLiquidacionCartXCart INT
	SET @idLiquidacionCartXCart = (SELECT TOP 1 lccd.id_LiquidationCartOnCart 
									FROM InventoryMovePlantTransferDetail impt
									INNER JOIN LiquidationCartOnCartDetail lccd  
									ON lccd.id = impt.id_liquidationCartOnCartDetail
									WHERE impt.id_inventoryMovePlantTransfer = @id)

	--SELECT @idLiquidacionCartXCart

	DECLARE @DetalleInventario TABLE(
		idMovInventario INT,
		idDetMovInventario INT,
		fechaEmision DATETIME,
		nombreItem VARCHAR(250),
		codigoItem VARCHAR(250),
		marca VARCHAR(150),
		peso VARCHAR(150),
		talla VARCHAR(50),
		cajas INT,
		orden INT
	)

	DECLARE @detalleDocumentoOrigen TABLE(
		id_documentOrigin INT,
		name VARCHAR(250)
	)

	DECLARE @detalleMaquina TABLE(
		idMachineProdOpeningDetail INT,
		maquinaCongelamiento VARCHAR(250),
		turno VARCHAR(150)
	)

	DECLARE @idsCarroXCarroDetalle TABLE(
		idCarroXCarroDetail INT
	)

	DECLARE @detaleLiquidacionCarroXCarro TABLE(
		idLiquidacionCarroxCarro INT,
		idLiquidacionCarroxCarroDetalle INT,
		maquina VARCHAR(250),
		numeroLiqCarroxCarro VARCHAR(150),
		tipoProceso VARCHAR(150),
		liquidador VARCHAR(250),
		lote VARCHAR(150),
		numeroLote VARCHAR(150),
		numeroCarro VARCHAR(100),
		numeroCajas NUMERIC(15,4)
	)

	DECLARE @fCam001 TABLE(
		id	INT,
		FechaEmision DATETIME,
		placaTurno VARCHAR(250),
		turno VARCHAR(250),
		HoraInicio	DATETIME,
		HoraCierre	VARCHAR(250),
		Lote VARCHAR(150),
		numeroDocumento VARCHAR(150),
		tipoDocumento VARCHAR(250),
		estadoDocumento VARCHAR(150),
		descripcion VARCHAR(250),
		codigoItem VARCHAR(150),
		NombreItem VARCHAR(150),
		Marc VARCHAR(250),
		Peso VARCHAR(150),
		Cliente VARCHAR(250),
		Talla VARCHAR(150),
		numeroCarro VARCHAR(250),
		NumeroCajas	INT,
		Orden INT
	)

	INSERT INTO @idsCarroXCarroDetalle
		SELECT DISTINCT
			id_liquidationCartOnCartDetail
		FROM InventoryMovePlantTransferDetail	
		WHERE id_inventoryMovePlantTransfer = @id

	INSERT INTO @DetalleInventario
		SELECT 
			im.id idMovInventario,
			imd.id idDetMovInventario,
			dim.emissionDate fechaEmision,
			i.name nombreItem,
			i.masterCode codigoItem,
			it.name Marca,
			p.name Peso,
			its.name Talla,
			isnull(imd.amountMove,0) - 	isnull(imdt.quantity,0) Cajas,
			its.orderSize Orden
		FROM InventoryMove im
		INNER JOIN InventoryMoveDetail imd
			ON imd.id_inventoryMove = im.id
		LEFT OUTER JOIN InventoryMoveDetailTransfer imdt
		ON imdt.id_inventoryMoveDetailEntry = imd.id
		INNER JOIN Document dim
			ON dim.id = im.id
		INNER JOIN DocumentState dsim
			ON dsim.id = dim.id_documentState
		INNER JOIN Item i
			ON i.id = imd.id_item
		INNER JOIN Presentation p
			ON p.id = i.id_presentation
		LEFT OUTER JOIN ItemGeneral ig
			ON ig.id_item = i.id
		LEFT JOIN ItemTrademark it
			ON it.id = ig.id_trademark
		LEFT JOIN ItemSize its
			ON its.id = ig.id_size
		WHERE im.id = @id
		ORDER BY its.orderSize

	INSERT INTO @detalleDocumentoOrigen
		SELECT 
			dsr.id_documentOrigin
			,dtdsr.name 
		FROM  DocumentSource dsr
		INNER JOIN Document ddsr
			ON dsr.id_document = ddsr.id
		INNER JOIN DocumentType dtdsr
			ON dtdsr.id = ddsr.id_documentType
		WHERE dtdsr.code = '136'
		and id_documentOrigin = @id


	INSERT INTO @detalleMaquina
		SELECT DISTINCT
			mpod.id idMachineProdOpeningDetail,
			mfp.name maquinaCongelamiento,
			t.name turno
		FROM MachineProdOpeningDetail mpod
		INNER JOIN MachineProdOpening mpd
			ON mpd.id = mpod.id_MachineProdOpening
		INNER JOIN MachineForProd mfp
			ON mfp.id = mpod.id_MachineForProd
		INNER JOIN Turn t
			ON t.id = mpd.id_Turn


	INSERT INTO @detaleLiquidacionCarroXCarro
	SELECT
		lcc.id idLiquidacionCarroxCarro,
		lccd.id idLiquidacionCarroxCarroDetalle,
		mfp.name maquina,
		dlcc.number numeroLiqCarroxCarro,
		pt.name tipoProceso,
		p.fullname_businessName Liquidador,
		pl.number lote,
		pl.internalnumber numeroLote,
		pc.code NumeroCarro,
		lccd.quatityBoxesIL NumeroCajas
	FROM LiquidationCartOnCartDetail lccd
	INNER JOIN @idsCarroXCarroDetalle ilccd
		ON ilccd.idCarroXCarroDetail = lccd.id
	INNER JOIN LiquidationCartOnCart lcc
		ON lcc.id = lccd.id_LiquidationCartOnCart
	INNER JOIN MachineForProd mfp
		ON mfp.id = lcc.id_MachineForProd
	INNER JOIN Document dlcc
		ON dlcc.id = lcc.id
	INNER JOIN ProductionCart pc
		ON pc.id = lccd.id_ProductionCart
	INNER JOIN ProcessType pt
		ON pt.id = lcc.idProccesType
	INNER JOIN Person p
		ON p.id = lcc.id_liquidator
	INNER JOIN ProductionLot pl
		ON pl.id = lcc.id_ProductionLot

	--SELECT * FROM @detalleMaquina
	--SELECT * FROM @detalleDocumentoOrigen
	--SELECT * FROM @DetalleInventario
	--SELECT * FROM @detaleLiquidacionCarroXCarro

	INSERT INTO @fCam001
	SELECT DISTINCT
		--===============================
		--			Cabecera
		--===============================
		impt.id id,
		imDat.fechaEmision FechaEmision,
		cccf.maquinaCongelamiento + ' - ' + cccf.turno placaTurno,
		cccf.turno turno,
		impt.dateTimeEntry HoraInicio,
		'' HoraCierre,
		lccdT.Lote + ' - ' + lccdT.numeroLote Lote,
		d.number numero,
		dt.name tipoDocumento,
		ds.name estadoDocumento,
		d.description descripcion,
		--===============================
		--			Detalle
		--===============================
		--imDat.idDetMovInventario,
		imDat.codigoItem,
		imDat.nombreItem NombreItem,
		imDat.Marca Marc,
		imDat.Peso Peso,
		'' Cliente,
		imDat.Talla Talla,
		lccdT.NumeroCarro numeroCarro,
		imDat.cajas NumeroCajas,
		imDat.orden Orden
	FROM InventoryMovePlantTransfer impt
	INNER JOIN Document d
		ON d.id = impt.id
	INNER JOIN DocumentType dt
		ON dt.id = d.id_documentType
	INNER JOIN DocumentState ds
		ON ds.id = d.id_documentState
	INNER JOIN InventoryMovePlantTransferDetail imptd
		ON impt.id = imptd.id_inventoryMovePlantTransfer
	INNER JOIN @detalleMaquina cccf
		ON cccf.idMachineProdOpeningDetail = impt.id_machineProdOpeningDetail
	INNER JOIN @detalleDocumentoOrigen ddsrc 
		ON ddsrc.id_documentOrigin = impt.id
	INNER JOIN @DetalleInventario imDat
		ON imDat.idMovInventario = ddsrc.id_documentOrigin
	INNER JOIN @detaleLiquidacionCarroXCarro lccdT
		ON lccdT.idLiquidacionCarroxCarroDetalle = imptd.id_liquidationCartOnCartDetail
	WHERE impt.id = @id
--	order by imDat.Marca,imDat.orden
	
		
	SELECT 
		* ,
		(SELECT TOP(1) C.logo 
		FROM Document d
		INNER JOIN EmissionPoint ep ON d.id_emissionPoint = ep.id
		INNER JOIN Company c ON c.id = ep.id_company
		WHERE d.id = @id) LOGO
	FROM @fCam001 
	ORDER BY Orden


	--SELECT * FROM InventoryMovePlantTransferDetail WHERE id_inventoryMovePlantTransfer = 300785
