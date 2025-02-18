IF OBJECT_ID('spc_AnalisisCalidad') IS NULL
EXEC('CREATE PROCEDURE spc_AnalisisCalidad AS')

GO

ALTER Procedure [dbo].[spc_AnalisisCalidad]
@FechaEmisionInicio			DATETIME,
@FechaEmisionFinal			DATETIME

AS
	
	BEGIN
	
	Declare @TotalDefectos Table (id_qualityControl	Int,TotalDefectos	numeric(13,2))
	Declare @TotalOtrasEspecies Table(id_qualityControl	Int,TotalOtrasEspecies	numeric(13,2))
	Declare @TotalPiezas Table (id_qualityControl	Int,TotalPiezas int)

	Declare @AccionesCorrectivas Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @TotalRecibidas Table (id	Int,quantitydrained	numeric(13,2))

	SET @FechaEmisionInicio = CONVERT(VARCHAR, @FechaEmisionInicio,112) 
	SET @FechaEmisionFinal = CONVERT(VARCHAR ,@FechaEmisionFinal,112) 

	Insert Into @TotalDefectos
	Select id_qualityControl, SUM(CONVERT(decimal(13,2), REPLACE(resultValue,',','.')))TotalDefectos from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Inner Join QualityControlAnalysisGroupAnalysis qan On qan.id_QualityAnalysis = qa.id
	where id_QualityControlAnalysisGroup=2 and resultValue <> '' and qa.name not in ('FRESCO Y SANO')
	Group by id_qualityControl


	Insert Into @TotalPiezas
	Select	d.id_qualityControl, 
			--SUM(CONVERT(int, REPLACE(d.otherResultValue,',','.'))) TotalPiezas 
			sum(otherResultValue) TotalPiezas
	from(
			select  id_qualityControl,
					CONVERT(int,
							case 
								when (CHARINDEX('.', otherResultValue) > 0) then SUBSTRING(otherResultValue,0, CHARINDEX('.', otherResultValue))
								when (CHARINDEX(',', otherResultValue) > 0) then SUBSTRING(otherResultValue,0, CHARINDEX(',', otherResultValue))
								else otherResultValue
							end) otherResultValue
			from	QualityControlDetail qcd
			Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
			Inner Join QualityControlAnalysisGroupAnalysis qan On qan.id_QualityAnalysis = qa.id
			where	id_QualityControlAnalysisGroup=2 
					and isnull(otherResultValue,'') <> ''
		) d
	Group by d.id_qualityControl

	
	Insert Into @TotalOtrasEspecies
	Select id_qualityControl, SUM(CONVERT(decimal(13,2), REPLACE(resultValue,',','.')))TotalOtrasEspecies from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Inner Join QualityControlAnalysisGroupAnalysis qan On qan.id_QualityAnalysis = qa.id
	where id_QualityControlAnalysisGroup=3 and resultValue <> ''
	Group by id_qualityControl


	Insert Into @TotalRecibidas
	Select Plot.id,Sum(quantitydrained)quantitydrained from Productionlot Plot
	Inner Join ProductionlotDetail Dplo On Dplo.id_productionLot = Plot.id
	Group by Plot.id


	-- declaracion de variables
	declare @tablePivot varchar(50) = '#TablaPivot'
	declare @cad Nvarchar(max)
	declare @nombreColumna varchar(50) 
	declare @identificador int 


	declare @NombreColumnasTable table(
		identificador int,
		columna varchar(250) collate database_default,
		orden int
	)

	-- aqui se hace el Join principal vinculado al join especificado
	Select distinct
		qcd.id_qualityControl [COD. ANALISIS]
	into #TablaPivot -- cambiar por tabla a editar
	from QualityControlDetail qcd
	Inner Join QualityAnalysis qa 
	On qa.id = qcd.id_qualityAnalysis 
	----------------------------------------------------------------------------------------------------------

	-- ingresamos los nombres e identificador de las columnas para agregar 
	insert into @NombreColumnasTable
	select distinct
		qa.id,
		case when gr.id_QualityControlAnalysisGroup = 5 then qa.name + '.' else qa.name end ,
		gr.id_QualityControlAnalysisGroup
	from QualityControlDetail qcd
	Inner Join QualityAnalysis qa 
	On qa.id = qcd.id_qualityAnalysis 
	Inner Join QualityControlAnalysisGroupAnalysis gr
	   On gr.id_QualityAnalysis = qa.id
	order by gr.id_QualityControlAnalysisGroup


	IF (SELECT COUNT(*) From @NombreColumnasTable) > 0
	BEGIN
		WHILE (SELECT COUNT(*) From @NombreColumnasTable) > 0
		BEGIN 
			DECLARE @Resultado  AS NVARCHAR(MAX)  
			set @nombreColumna = (SELECT top 1 columna From @NombreColumnasTable)
			set @identificador = (SELECT top 1 identificador From @NombreColumnasTable)
		
			set @cad = ''
			set @cad = @cad + 'SELECT @Resultado = CASE WHEN COUNT(*) > 0 THEN RIGHT(''' + '.''' + '+CAST(COUNT(*) AS NVARCHAR(5)), 5) ELSE '''+''' END '
			set @cad = @cad + 'FROM tempdb.INFORMATION_SCHEMA.COLUMNS '
			set @cad = @cad + 'WHERE TABLE_NAME LIKE ''' + @tablePivot + '%''' +'AND COLUMN_NAME = '''+ @nombreColumna + ''' '


			PRINT @cad

			EXEC sp_executesql @cad, N'@Resultado  AS NVARCHAR(MAX) OUTPUT', @Resultado OUTPUT;

			SET @nombreColumna = @nombreColumna + @Resultado
			
			set @cad = ''

			set @cad = @cad + 'ALTER TABLE [' + @tablePivot + ']' + char(13)
			set @cad = @cad + 'ADD [' + @nombreColumna + '] varchar(250) null'  + char(13)
			exec (@cad)
			PRINT @cad
			set @cad = ''
			set @cad = @cad + 'UPDATE pv' + char(13) 
			set @cad = @cad + 'SET pv.[' + @nombreColumna + '] = Isnull((select top 1 
								qcd.resultValue
									from QualityControlDetail qcd
									Inner Join QualityAnalysis qa 
									On qa.id = qcd.id_qualityAnalysis 
									where qcd.id_qualityControl = pv.[COD. ANALISIS]
									and qa.id = ''' + convert(varchar(15),@identificador) + '''),'''')'  + char(13)
			set @cad = @cad + 'From [' + @tablePivot + '] pv' + char(13) 
			print (@cad)
			exec (@cad)

			delete top (1) @NombreColumnasTable
		END
	END


	-- declaracion de variables
	declare @tablePivot2 varchar(50) = '#TablaPivot2'
	declare @cad2 varchar(max)
	declare @nombreColumna2 varchar(50) 
	declare @identificador2 int 


	declare @NombreColumnasTable2 table(
		identificador int,
		columna varchar(250) collate database_default,
		orden int
	)

	-- aqui se hace el Join principal vinculado al join especificado
	Select distinct
		qcd.id_qualityControl [COD. ANALISIS]
	into #TablaPivot2 -- cambiar por tabla a editar
	from QualityControlDetail qcd
	Inner Join QualityAnalysis qa 
	On qa.id = qcd.id_qualityAnalysis 


	-- ingresamos los nombres e identificador de las columnas para agregar 
	insert into @NombreColumnasTable2
	select distinct
		qa.id,
		qa.name,
		gr.id_QualityControlAnalysisGroup
	from QualityControlDetail qcd
	Inner Join QualityAnalysis qa 
	On qa.id = qcd.id_qualityAnalysis 
	Inner Join QualityControlAnalysisGroupAnalysis gr
	   On gr.id_QualityAnalysis = qa.id
	Where gr.id_QualityControlAnalysisGroup in (1,2)
	order by gr.id_QualityControlAnalysisGroup


	IF (SELECT COUNT(*) From @NombreColumnasTable2) > 0
	BEGIN
		WHILE (SELECT COUNT(*) From @NombreColumnasTable2) > 0
		BEGIN 
			set @nombreColumna2 = (SELECT top 1 columna + Case when orden = 1 Then '_DES' else '_UNI' end From @NombreColumnasTable2)
			set @identificador2 = (SELECT top 1 identificador From @NombreColumnasTable2)
		

			set @cad = ''

			set @cad = @cad + 'ALTER TABLE [' + @tablePivot2 + ']' + char(13)
			set @cad = @cad + 'ADD [' + @nombreColumna2 + '] varchar(250) null'  + char(13)
			exec (@cad)

			set @cad = ''
			set @cad = @cad + 'UPDATE pv' + char(13) 
			set @cad = @cad + 'SET pv.[' + @nombreColumna2 + '] = Isnull((select top 1 
								qcd.otherResultValue
									from QualityControlDetail qcd
									Inner Join QualityAnalysis qa 
									On qa.id = qcd.id_qualityAnalysis 
									where qcd.id_qualityControl = pv.[COD. ANALISIS]
									and qa.id = ' + convert(varchar(15),@identificador2) + '),'''')'  + char(13)
			set @cad = @cad + 'From [' + @tablePivot2 + '] pv' + char(13) 
			print (@cad)
			exec (@cad)

			delete top (1) @NombreColumnasTable2
		END
	END


	Select Distinct
		b.name as [Tipo de Análisis],
		a.number as [Nº Análisis], 
		c.number as [Sec. Transaccional], 
		d.internalNumber as [Nº Lote],
		isnull(cer.code,'') as Certificado,
		isnull(cer.name,'') as [Certificado Nombre],
		CONVERT(CHAR(10),d.receptionDate,103) + ' ' + LEFT(FORMAT(CAST(d.receptionDate AS DATETIME),'hh:mm tt'),5) as [Fecha/Hora de Llegada],
		convert(varchar(250),e.fullname_businessName) as [Proveedor],
		convert(varchar(250),f.name) as [Camaronera],
		g.name as [Nº Piscina],
		Isnull(f.INPnumber,'') as [INP],
		Isnull(f.ministerialAgreement,'') as [Nº Acuerdo Ministerial],
		Isnull(f.tramitNumber,'') as [Nº Trámite],
		k.number as [Nº Guía Remisión],
		l.name as [Producto],
		m.name as [Tipo Proceso],
		a.DrawersReceived as [Nº Gavetas],
		d.totalQuantityRemitted as [Libras Programadas],
		d.totalQuantityRecived as [Libras Remitidas],
		--aw.quantitydrained as [Libras Recibidas],
		CONVERT (char(10), a.qualityControlDate, 103) as [Fecha de Análisis],
		convert(char(5), a.qualityControlTime, 108) as [Hora de Análisis],
		a.residual as [Residual S02],
		Round(a.temperature,2,0) as [Temperatura],
		a.grammageReference as [Gramaje],
		pv.*,
		pv2.*,
		al.TotalDefectos as [Total Defectos],
		Isnull(pz.TotalPiezas,0) as [Total Piezas],
		a.wholePerformance as [Rendimiento Entero],
		Isnull(at.TotalOtrasEspecies,0) as [Total Otras Especies],
		--aq.resultValue as [Acciones Correctivas],
		Isnull(a.reference,'') as [Referencia],
		Isnull(av.fullname_businessName,'') as [Analista],
		Isnull(au.name,'') as [Es Conforme],
		Isnull(a.description,'') as [Observacion],
		pd.processPlant as [Proceso]
	from QualityControl a
	Inner Join QualityControlConfiguration b On b.id = a.id_qualityControlConfiguration
	Inner Join Lot c On c.id = a.id_lot
	Inner Join Productionlot d On d.id = a.id_lot
	INNER JOIN Person pd On pd.id = d.id_personProcessPlant
	Inner Join Person e On e.id  = d.id_provider
	Inner Join ProductionUnitProvider f On f.id = d.id_productionUnitProvider
	Inner Join ProductionUnitProviderPool g On g.id = d.id_productionUnitProviderPool
	Inner Join ProductionLotDetailQualityControl h On h.id_qualityControl = a.id
	Inner Join ProductionLotDetailPurchaseDetail i On i.id_productionLotDetail = h.id_productionLotDetail
	Inner Join RemissionGuideDetail j On j.id = i.id_remissionGuideDetail
	Inner Join Document k On k.id = j.id_remisionGuide
	Inner Join Document dc On dc.id = a.id
	Inner Join DocumentState ds On ds.id = dc.id_documentState
	Inner Join DocumentType dt On dt.id = dc.id_documentType And dt.code = '61'
	Inner Join Item l On l.id = a.id_Item
	Inner Join ProcessType m On m.id = a.id_ProcessType
	Left outer Join ItemSize iz On iz.id = a.id_size
	Left outer Join Certification cer on cer.id = d.id_certification
	Left outer Join QualityControlResultConformityOnHeaderValue ar On ar.id_QualityControl = a.id 
	Left outer Join QualityControlResultConformity au On au.id = ar.id_QualityControlResultConformity 
	Left outer Join Person av On av.id = a.Id_analyst
	Left outer Join @TotalDefectos al On al.id_qualityControl = a.id 
	Left outer Join @TotalPiezas pz On pz.id_qualityControl = a.id 
	Left outer Join @TotalRecibidas aw On aw.id = d.id
	Left outer Join @TotalOtrasEspecies at On at.id_qualityControl = a.id 
	--Left outer Join @AccionesCorrectivas aq On aq.id_qualityControl = a.id
	Left outer Join #TablaPivot pv On pv.[COD. ANALISIS] = a.id
	Left outer Join #TablaPivot2 pv2 On pv2.[COD. ANALISIS] = a.id
	WHERE Convert(Varchar,A.qualityControlDate,112) >= Convert(Varchar,@fechaEmisionInicio,112)
	AND  Convert(Varchar,A.qualityControlDate,112) <= Convert(Varchar,@FechaEmisionFinal,112)
	
END