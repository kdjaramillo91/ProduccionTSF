-- dbo.pat_TransCtlEval 1511929, 59, '0',39

					 
Create procedure dbo.pat_TransCtlEval
( 
	@documentId		int			null,  
	@documentTypeId int			null,  
	@stage			varchar(35) null,
	@numdetails		int			null
)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @CONST_MIN_CONTROL INT = 5;
	DECLARE @ProcesoExecBLoquea INT = 0;
	DECLARE @BloqueTablaProceso INT = 0;
	DECLARE @RecursosDisponibles INT = 0;

	-- 1 No pasa | 0 Pasa
	-- Existe otro proceso en ejecucion y este genera interbloqueo 1|0 => 
	SET @ProcesoExecBLoquea  = 0;

	-- Existe bloqueo en el servidor y corresponde a una de las tablas de proceso, en la seccion de interbloqueo  1|0
	SET @BloqueTablaProceso = 0;
	
	-- La diferencia entre el maximo de recurso y los recursos usados NO son los suficientes para el consumo estadistico del proceso 1|0
	-- ANALIZAR EN EL TIPO CONFIGURAR MAX REC PROM DE USO DE RECURSOP PARA HACER LA VALIDACION
	SET  @RecursosDisponibles = 0;

	IF(@ProcesoExecBLoquea + @BloqueTablaProceso + @RecursosDisponibles) < @CONST_MIN_CONTROL
	BEGIN
		select	@documentId DocumentId,
				@documentTypeId DocumentTypeId, 
				@stage Stage, 
				@numdetails NumDetails
	END
	ELSE
	BEGIN
		SELECT
			TOP 1
			Identificador,
			DocumentTypeId,
			DocumentId,
			DocumentNumber,
			Stage,
			NumDetails,
			DataExecution,
			DataExecutionTypes,
			DataTempKeys,
			DataTempValues,
			DataTempTypes,
			DataSession
		FROM TransCtlQueue
		WHERE QueueEstate = 'PEN';	
	END

END