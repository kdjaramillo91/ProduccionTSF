
-- ==============================================================================================================
-- Creado por:		Wilber Leonard Benavides
-- Fecha Creación:	31 de Junio del 2019
-- Descripción:		Procedimiento almacenado para consultar los Controles de Calidad.
/*
-- Pruebas:			
					EXEC [dbo].[qa_Consultar_QualityControl_StoredProcedure] NULL, NULL
*/
--CREATE
CREATE PROCEDURE [dbo].[qa_Consultar_QualityControl_StoredProcedure]
	@ParametrosBusquedaQualityControl		NVARCHAR(MAX)
AS
BEGIN
	
	--==================================================================================================
	-- MAPEO DE PARÁMETROS
	--==================================================================================================
	DECLARE @id_documentState						INT 			= JSON_VALUE(@ParametrosBusquedaQualityControl, '$.id_documentState')
	DECLARE @qualityControlNumber					VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaQualityControl, '$.qualityControlNumber')
	DECLARE @documentReference						VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaQualityControl, '$.documentReference')
	DECLARE @startEmissionDate						DATETIME	    = JSON_VALUE(@ParametrosBusquedaQualityControl, '$.startEmissionDate')
	DECLARE @endEmissionDate						DATETIME	    = JSON_VALUE(@ParametrosBusquedaQualityControl, '$.endEmissionDate')
	DECLARE @id_qualityControlConfiguration			INT 			= JSON_VALUE(@ParametrosBusquedaQualityControl, '$.id_qualityControlConfiguration')
	DECLARE @id_analyst								INT 			= JSON_VALUE(@ParametrosBusquedaQualityControl, '$.id_analyst')
	DECLARE @conforms								VARCHAR(MAX)	= JSON_VALUE(@ParametrosBusquedaQualityControl, '$.conforms')

	--==================================================================================================
	-- CREACIÓN DE TABLA TEMPORAL
	--==================================================================================================
	CREATE TABLE #TmpIdentificadoresQualityControl
	(
		id	int
	)

	INSERT INTO #TmpIdentificadoresQualityControl
	SELECT A.id As id
	FROM dbo.QualityControl A WITH (NOLOCK)
	INNER JOIN dbo.Document B WITH (NOLOCK) ON (B.id = A.id)
	WHERE ((B.id_documentState = ISNULL(@id_documentState, B.id_documentState) OR @id_documentState = 0)
		  AND (CAST(B.number AS VARCHAR(20)) LIKE '%' + @qualityControlNumber + '%' OR @qualityControlNumber = 'Todos')
		  AND (CAST(B.number AS VARCHAR(20)) LIKE '%' + @documentReference + '%' OR @documentReference = 'Todos')
		  AND (((B.emissionDate >= @startEmissionDate OR @startEmissionDate IS NULL) AND (B.emissionDate < DATEADD(day, 1, @endEmissionDate) OR @endEmissionDate IS NULL)))
		  AND (A.id_qualityControlConfiguration = ISNULL(@id_qualityControlConfiguration, A.id_qualityControlConfiguration) OR @id_qualityControlConfiguration = 0)
		  AND (A.id_analyst = ISNULL(@id_analyst, A.id_analyst) OR @id_analyst = 0)
		  AND (A.isConforms = IIF((@conforms = 'Conforme'), 1, 0) OR @conforms = 'Todos'));
		  

		--select * from #TmpIdentificadoresQualityControl;

	SELECT A.id
		  ,C.number
		  ,E.[name] as analysisType
		  ,C.emissionDate as dateAnalysis
		  ,F.internalNumber
		  ,N.number as remissionGuideNumber
		  ,O.processPlant as remissionGuideProcess
		  ,G.fullname_businessName as proveedor
		  ,H.[name] as processType
		  ,P.quantitydrained as quantityPoundsReceived
		  ,I.fullname_businessName as analyst
		  ,A.isConforms
          ,IIF((A.isConforms = 1), 'Conforme', 'No Conforme') as isConformsStr
		  ,D.[name] as documentState
		FROM [dbo].[QualityControl] A WITH (NOLOCK)
		INNER JOIN #TmpIdentificadoresQualityControl B ON (B.id = A.id)
		INNER JOIN dbo.Document C WITH (NOLOCK) ON (C.id = B.id)
		INNER JOIN dbo.DocumentState D WITH (NOLOCK) ON (D.id = C.id_documentState)
		INNER JOIN dbo.QualityControlConfiguration E WITH (NOLOCK) ON (E.id = A.id_qualityControlConfiguration)
		LEFT JOIN dbo.ProductionLot F WITH (NOLOCK) ON (F.id = A.id_lot)
		LEFT JOIN dbo.Person G WITH (NOLOCK) ON (G.id = F.id_provider)
		INNER JOIN dbo.ProcessType H WITH (NOLOCK) ON (H.id = A.id_processType)
		INNER JOIN dbo.Person I WITH (NOLOCK) ON (I.id = A.id_analyst)
		INNER JOIN dbo.ProductionLotDetailQualityControl J WITH (NOLOCK) ON (J.id_qualityControl = A.id)
		LEFT JOIN dbo.ProductionLotDetailPurchaseDetail K WITH (NOLOCK) ON (K.id_productionLotDetail = J.id_productionLotDetail)
		LEFT JOIN dbo.RemissionGuideDetail L WITH (NOLOCK) ON (L.id = K.id_remissionGuideDetail)
		LEFT JOIN dbo.RemissionGuide M WITH (NOLOCK) ON (M.id = L.id_remisionGuide)
		LEFT JOIN dbo.Document N WITH (NOLOCK) ON (N.id = M.id)
		LEFT JOIN dbo.Person O WITH (NOLOCK) ON (O.id = M.id_personProcessPlant)
		LEFT JOIN dbo.ProductionLotDetail P WITH (NOLOCK) ON (P.id = J.id_productionLotDetail)
		ORDER BY C.emissionDate, A.id DESC

END