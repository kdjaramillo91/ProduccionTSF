GO
/****** Object:  StoredProcedure [dbo].[IngresoEgresoTumbadoReport]    Script Date: 02/02/2023 9:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[spPar_TmbadoPlacaIngresoTransferencia]--641185,1,"I"
	@id INT,
	@id_user INT,
	@naturaleza CHAR
AS
	SET NOCOUNT ON

	DECLARE @ids_Ingreso TABLE(
		id INT
	)

	DECLARE @reporteMovimiento TABLE(
		 id	INT
		,TituloReporte VARCHAR(250)
		,Bodega	VARCHAR(250)
		,Motivo	VARCHAR(250)
		,FechaEmision	DATETIME
		,CodigoProducto	VARCHAR(100)
		,DescripcionProducto	VARCHAR(250)
		,UnidadMedida	VARCHAR(100)
		,CodigoUnidadMedida	VARCHAR(25)
		,Cantidad	DECIMAL(20,6)
		,NumeroSecuencia	VARCHAR(100)
		,IdUbicacion	INT
		,NombreUbicacion	VARCHAR(250)
		,CentroCosto	VARCHAR(250)
		,SubCentroCosto	VARCHAR(250)
		,CodigoNaturaleza	VARCHAR(10)
		,NombreNaturaleza	VARCHAR(100)
		,SecuenciaGuiaRemision	VARCHAR(100)
		,SecuenciaRequisicion	VARCHAR(100)
		,SecuenciaLiquidacionMateriales	VARCHAR(100)
		,IdCompania	INT
		,Descripcion	VARCHAR(250)
		,medidas	VARCHAR(100)
		,numLote	VARCHAR(50)
		,SecTransac	VARCHAR(100)
		,EstadoDocumento	VARCHAR(100)
		,itemTalla	VARCHAR(100)
		,tipoItem	VARCHAR(100)
		,nombreUsuario	VARCHAR(100)
		,bodegaIngreso	VARCHAR(250)
		,bodegaEgreso	VARCHAR(250)
		,UbicacionEgreso	VARCHAR(250)
		,numeroEgreso VARCHAR(100)
		,libras		DECIMAL(20,6)
		,Kilos		DECIMAL(20,6)
	)

	DECLARE @id_Egreso INT
	SET @id_Egreso = (SELECT id_document FROM DocumentSource
						WHERE id_documentOrigin = @id)

	INSERT INTO @ids_Ingreso
		SELECT id_document FROM DocumentSource
		WHERE id_documentOrigin = @id_Egreso
	
	IF @Naturaleza = 'I'
	BEGIN
		DECLARE @registros INT
		DECLARE @id_TumbadoPlaca INT
		SET @registros = (SELECT COUNT(*) FROM @ids_Ingreso)
		WHILE @registros > 0
		BEGIN
			SET @id_TumbadoPlaca = (SELECT TOP 1 id FROM @ids_Ingreso)
			INSERT INTO @reporteMovimiento
			EXEC par_Movimiento_InventarioMotivo @id_TumbadoPlaca,@id_user

			DELETE TOP (1) FROM @ids_Ingreso
			SET @registros = (SELECT COUNT(*) FROM @ids_Ingreso)
		END

		UPDATE @reporteMovimiento
		SET TituloReporte = 'INGRESO POR TRANSFERENCIA'
	END

	IF @Naturaleza = 'E'
	BEGIN
		INSERT INTO @reporteMovimiento
			EXEC par_Movimiento_InventarioMotivo @id_Egreso,@id_user

		UPDATE @reporteMovimiento
		SET TituloReporte = 'EGRESO POR TRANSFERENCIA'
	END
	
	SELECT * FROM @reporteMovimiento



--exec [IngresoEgresoTumbadoReport] 369823,1,'E'
--exec [IngresoEgresoTumbadoReport] 369823, 1 , 'I'
