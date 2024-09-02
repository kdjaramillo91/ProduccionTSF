
/****** Object:  StoredProcedure [dbo].[par_GuiasRemisionTerrestreMatriz]  '2023-09-01','2023-09-30'  Script Date: 14/10/2023 10:42:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO


alter PROCEDURE [dbo].[par_GuiasRemisionTerrestreMatriz]
	@fechaEmisionInicio DATETIME,
	@fechaEmisionFinal DATETIME
	
	AS 
	Declare @cad                  VARCHAR(8000)   

	SET @fechaEmisionInicio = CONVERT(VARCHAR,@fechaEmisionInicio,112) 
	SET @fechaEmisionFinal = CONVERT(VARCHAR,@fechaEmisionFinal,112) 

	CREATE TABLE #Documentos(
		id int
	)

	CREATE TABLE #MaterialesDespacho(
		id int,
		nombre VARCHAR(250),
		cantidad DECIMAL
	)

	CREATE TABLE #ValorLiquidacion(
		id int,
		invoiceNumber VARCHAR(250),
		totalLiquidado DECIMAL
	)

	CREATE TABLE #PersonalAsignado(
		id INT,
		personAsigCode VARCHAR(5),
		id_Person INT,
		viaticPrice DECIMAL,
		tipoViaje VARCHAR(250)
	)

	CREATE TABLE #Sellos(
		number int,
		id_guia int,
		numSello varchar(50),
		salida varchar(50),
		llegada varchar(50),
		viaje varchar(150)
	)

	CREATE TABLE #ViajesGuia(
		[id_guia] INT,
		[Sello 1] VARCHAR(250),
		[Viaje 1] VARCHAR(250),
		[Estado Salida 1] VARCHAR(250),
		[Estado Llegada 1]VARCHAR(250),
		[Sello 2] VARCHAR(250),
		[Viaje 2] VARCHAR(250),
		[Estado Salida 2] VARCHAR(250),
		[Estado Llegada 2]VARCHAR(250),
		[Sello 3] VARCHAR(250),
		[Viaje 3] VARCHAR(250),
		[Estado Salida 3] VARCHAR(250),
		[Estado Llegada 3]VARCHAR(250),
		[Sello 4] VARCHAR(250),
		[Viaje 4] VARCHAR(250),
		[Estado Salida 4] VARCHAR(250),
		[Estado Llegada 4]VARCHAR(250)
	)

	CREATE TABLE #dispatchMaterial(
		[id_guia] INT,
		[GAVETAS CERRADAS] NUMERIC(20,3),
		[GAVETAS CALADAS] NUMERIC(20,3),
		[SACO CON HIELO TERCEROS] NUMERIC(20,3),
		[METABISULFITO] NUMERIC(20,3),
		[SAL] NUMERIC(20,3),
		[SACOS VACIOS TERCEROS] NUMERIC(20,3),
		[SACO CON HIELO] NUMERIC(20,3),
		[SACOS VACIOS] NUMERIC(20,3),
		[BINES AZUL] NUMERIC(20,3),
		[BINES TERCEROS] NUMERIC(20,3),
		[SELLOS DE BINES] NUMERIC(20,3),
		
	) 

	Set @Cad = ""  
	Set @Cad = @Cad + "Insert Into #Documentos" + Char(13)    
	Set @Cad = @Cad + "SELECT [d].[id] FROM [Document][d]" + Char(13) 
	Set @Cad = @Cad + "INNER JOIN [RemissionGuide][ds]" + Char(13) 
	Set @Cad = @Cad + "ON [ds].[id] = [d].[id]" + Char(13) 
	If @fechaEmisionInicio <> '' and  @fechaEmisionFinal <> ''
		Set @Cad = @Cad + "WHERE Convert(Varchar,d.emissionDate,112) >= '" + Convert(Varchar,@fechaEmisionInicio,112) + "' And Convert(Varchar,d.emissionDate,112) <= '" + Convert(Varchar,@fechaEmisionFinal,112) + "'" + Char(13)
	If @fechaEmisionInicio <> '' and  @fechaEmisionFinal = ''
		Set @Cad = @Cad + "WHERE Convert(Varchar,d.emissionDate,112) >= '" + Convert(Varchar,@fechaEmisionInicio,112) + "'" + Char(13)	 
	If @fechaEmisionInicio = '' and  @fechaEmisionFinal <> ''
		Set @Cad = @Cad + "WHERE Convert(Varchar,d.emissionDate,112) <= '" + Convert(Varchar,@fechaEmisionFinal,112) + "'" + Char(13)	
	EXEC (@Cad) 

	INSERT INTO #MaterialesDespacho 
	select	 [remissionGuide].[id]
			,[item].[name]
			,[remissionGuideDispatchMaterial].[sourceExitQuantity]
	from [RemissionGuide] [remissionGuide]
		inner join [RemissionGuideDispatchMaterial] [remissionGuideDispatchMaterial]
	on [remissionGuide].[id] = [remissionGuideDispatchMaterial].[id_remisionGuide]
		inner join [Document][document]
	on [document].[id] = [remissionGuide].[id]
		inner join [Item] [item]
	on [item].[id] = [remissionGuideDispatchMaterial].[id_item]
	--	inner join [ItemWarehouse][itemWarehouse]
	--on [itemWarehouse].[id_item] = [item].[id]
	--	inner join [Warehouse][warehouse]
	--on [warehouse].[id] = [itemWarehouse].[id_warehouse]
		inner join [#Documentos][documentos]
	on [documentos].[id] = [document].[id]
		
	INSERT INTO #dispatchMaterial 
		SELECT id,0,0,0,0,0,0,0,0,0,0,0 FROM #Documentos

	UPDATE [DM]
	SET [DM].[GAVETAS CERRADAS]= [MD].[cantidad]
	FROM [#dispatchMaterial][DM]
	INNER JOIN [#MaterialesDespacho] [MD]
	ON [DM].[id_guia] = [MD].[id]
	AND [MD].[nombre] like 'GAVETAS CERRADAS'

	UPDATE [DM]
	SET [DM].[GAVETAS CALADAS] = [MD].[cantidad]
	FROM [#dispatchMaterial][DM]
	INNER JOIN [#MaterialesDespacho] [MD]
	ON [DM].[id_guia] = [MD].[id]
	AND [MD].[nombre] like 'GAVETAS CALADAS'

	UPDATE [DM]
	SET [DM].[SACO CON HIELO TERCEROS] = [MD].[cantidad]
	FROM [#dispatchMaterial][DM]
	INNER JOIN [#MaterialesDespacho] [MD]
	ON [DM].[id_guia] = [MD].[id]
	AND [MD].[nombre] like 'SACO CON HIELO TERCEROS'

	UPDATE [DM]
	SET [DM].[METABISULFITO] = [MD].[cantidad]
	FROM [#dispatchMaterial][DM]
	INNER JOIN [#MaterialesDespacho] [MD]
	ON [DM].[id_guia] = [MD].[id]
	AND [MD].[nombre] like 'METABISULFITO'

	UPDATE [DM]
	SET [DM].[SAL] = [MD].[cantidad]
	FROM [#dispatchMaterial][DM]
	INNER JOIN [#MaterialesDespacho] [MD]
	ON [DM].[id_guia] = [MD].[id]
	AND [MD].[nombre] like 'SAL'

	UPDATE [DM]
	SET [DM].[SACOS VACIOS TERCEROS] = [MD].[cantidad]
	FROM [#dispatchMaterial][DM]
	INNER JOIN [#MaterialesDespacho] [MD]
	ON [DM].[id_guia] = [MD].[id]
	AND [MD].[nombre] like 'SACOS VACIOS TERCEROS'

	UPDATE [DM]
	SET [DM].[SACO CON HIELO] = [MD].[cantidad]
	FROM [#dispatchMaterial][DM]
	INNER JOIN [#MaterialesDespacho] [MD]
	ON [DM].[id_guia] = [MD].[id]
	AND [MD].[nombre] like 'SACO CON HIELO'

	UPDATE [DM]
	SET [DM].[SACOS VACIOS] = [MD].[cantidad]
	FROM [#dispatchMaterial][DM]
	INNER JOIN [#MaterialesDespacho] [MD]
	ON [DM].[id_guia] = [MD].[id]
	AND [MD].[nombre] like 'SACOS VACIOS'

	UPDATE [DM]
	SET [DM].[BINES AZUL] = [MD].[cantidad]
	FROM [#dispatchMaterial][DM]
	INNER JOIN [#MaterialesDespacho] [MD]
	ON [DM].[id_guia] = [MD].[id]
	AND [MD].[nombre] like 'BINES AZUL'



	UPDATE [DM]
	SET [DM].[BINES TERCEROS] = [MD].[cantidad]
	FROM [#dispatchMaterial][DM]
	INNER JOIN [#MaterialesDespacho] [MD]
	ON [DM].[id_guia] = [MD].[id]
	AND [MD].[nombre] like 'BINES TERCEROS'

	UPDATE [DM]
	SET [DM].[SELLOS DE BINES] = [MD].[cantidad]
	FROM [#dispatchMaterial][DM]
	INNER JOIN [#MaterialesDespacho] [MD]
	ON [DM].[id_guia] = [MD].[id]
	AND [MD].[nombre] like 'SELLOS DE BINES'




	Insert into #ValorLiquidacion
	select   [remissionGuide].[id]
			,[liquidationFreight].[invoiceNumber]	
			,[liquidationFreightDetail].[price]
	from [RemissionGuide] [remissionGuide]
		inner join [Document][document]
	on [document].[id] = [remissionGuide].[id]
		inner join [LiquidationFreightDetail][liquidationFreightDetail]
	on [liquidationFreightDetail].[id_remisionGuide] = [remissionGuide].[id]
		inner join [LiquidationFreight][liquidationFreight]
	on [liquidationFreight].[id] = [liquidationFreightDetail].[id_liquidationFreight]
		inner join [#Documentos][documentos]
	on [documentos].[id] = [document].[id]

	INSERT INTO #PersonalAsignado
	select
			 [remissionGuide].[id]
			,[remissionGuideAssignedStaffRol].[code]
			,[remissionGuideAssignedStaff].[id_person]
			,[remissionGuideAssignedStaff].[viaticPrice]
			,[remissionGuideTravelType].[name]
	from [RemissionGuide] [remissionGuide]
		inner join [Document][document]
	on [document].[id] = [remissionGuide].[id]
		inner join [RemissionGuideAssignedStaff] [remissionGuideAssignedStaff]
	on [remissionGuideAssignedStaff].[id_remissionGuide] = [remissionGuide].[id]
		inner join [RemissionGuideAssignedStaffRol][remissionGuideAssignedStaffRol]
	on [remissionGuideAssignedStaffRol].[id] = [remissionGuideAssignedStaff].[id_assignedStaffRol]
		INNER JOIN [RemissionGuideTravelType][remissionGuideTravelType]
	on [remissionGuideAssignedStaff].[id_travelType] = [remissionGuideTravelType].[id]
		inner join [#Documentos][documentos]
	on [documentos].[id] = [document].[id]
	where [remissionGuideAssignedStaff].[isActive] = 1

	INSERT INTO #Sellos
	select
			 ROW_NUMBER() OVER(PARTITION BY [remissionGuide].[id] ORDER BY [remissionGuide].[id] DESC) AS Number
			,[remissionGuide].[id] 
			,[remissionGuideSecuritySeal].[number]
			,'Estado Salida' = ISNULL((select top(1) [sss].[name] from [SecuritySealState][sss] where [sss].[id] = [remissionGuideSecuritySeal].[id_exitState]),'')
			,'Estado Llegada' = ISNULL((select top(1) [sss].[name] from [SecuritySealState][sss] where [sss].[id] = [remissionGuideSecuritySeal].[id_arrivalState]),'')
			,[remissionGuideTravelType].[name] as 'Viaje'
	from [RemissionGuide] [remissionGuide]
		inner join [Document][document]
	on [document].[id] = [remissionGuide].[id]
		INNER JOIN [RemissionGuideSecuritySeal][remissionGuideSecuritySeal]
	on [remissionGuideSecuritySeal].[id_remissionGuide] = [remissionGuide].[id]
		INNER JOIN [RemissionGuideTravelType][remissionGuideTravelType]
	on [remissionGuideSecuritySeal].[id_travelType] = [remissionGuideTravelType].[id]
	--	inner join [#Documentos][documentos]
	--on [documentos].[id] = [document].[id]
	where [remissionGuideSecuritySeal].[isActive] = 1

	INSERT INTO #ViajesGuia
		SELECT distinct id,'','','','','','','','','','','','','','','','' FROM RemissionGuide

		UPDATE Viaje
		SET  [Viaje].[Sello 1] = [sellos].[numSello]
			,[Viaje].[Viaje 1] = [sellos].[viaje]
			,[Viaje].[Estado Salida 1] = [sellos].[salida]
			,[Viaje].[Estado Llegada 1] = [sellos].[llegada]
		FROM [#ViajesGuia] [Viaje]
		INNER JOIN [remissionGuide] [RG]
		ON [RG].[id] = [Viaje].[id_guia]
		LEFT JOIN [#Sellos][sellos]
		ON [sellos].[id_guia] = [Viaje].[id_guia]
		AND [RG].[id] = [sellos].[id_guia]
		WHERE [sellos].[number] = 1

		UPDATE Viaje
		SET  [Viaje].[Sello 2] = [sellos].[numSello]
			,[Viaje].[Viaje 2] = [sellos].[viaje]
			,[Viaje].[Estado Salida 2] = [sellos].[salida]
			,[Viaje].[Estado Llegada 2] = [sellos].[llegada]
		FROM [#ViajesGuia] [Viaje]
		INNER JOIN [remissionGuide] [RG]
		ON [RG].[id] = [Viaje].[id_guia]
		LEFT JOIN [#Sellos][sellos]
		ON [sellos].[id_guia] = [Viaje].[id_guia]
		AND [RG].[id] = [sellos].[id_guia]
		WHERE [sellos].[number] = 2

		UPDATE Viaje
		SET  [Viaje].[Sello 3] = [sellos].[numSello]
			,[Viaje].[Viaje 3] = [sellos].[viaje]
			,[Viaje].[Estado Salida 3] = [sellos].[salida]
			,[Viaje].[Estado Llegada 3] = [sellos].[llegada]
		FROM [#ViajesGuia] [Viaje]
		INNER JOIN [remissionGuide] [RG]
		ON [RG].[id] = [Viaje].[id_guia]
		LEFT JOIN [#Sellos][sellos]
		ON [sellos].[id_guia] = [Viaje].[id_guia]
		AND [RG].[id] = [sellos].[id_guia]
		WHERE [sellos].[number] = 3

		UPDATE Viaje
		SET  [Viaje].[Sello 4] = [sellos].[numSello]
			,[Viaje].[Viaje 4] = [sellos].[viaje]
			,[Viaje].[Estado Salida 4] = [sellos].[salida]
			,[Viaje].[Estado Llegada 4] = [sellos].[llegada]
		FROM [#ViajesGuia] [Viaje]
		INNER JOIN [remissionGuide] [RG]
		ON [RG].[id] = [Viaje].[id_guia]
		LEFT JOIN [#Sellos][sellos]
		ON [sellos].[id_guia] = [Viaje].[id_guia]
		AND [RG].[id] = [sellos].[id_guia]
		WHERE [sellos].[number] = 4



	select distinct
		 [documentType].[name] as 'TipoDocumento'
		,[document].[number] as 'No.Documento'
		,[documentState].[name] as 'Estado'
		,eds.name as 'EstadoElectronico'
		,CONVERT(VARCHAR(10),([document].[emissionDate]),20) as 'Fch.Emisión'
		,[document].[accesskey] as 'ClaveAcceso'
		,ISNULL(CONVERT(VARCHAR(19),([document].[authorizationDate]),20),'') as 'Fch.Autorización'
		,ISNULL([document].[authorizationNumber],'') as 'No.Autorización'
		,[document].[description] as 'Descripción'
		,(CONVERT(VARCHAR(10),([remissionGuide].[despachureDate]),103)) + ' ' + REPLACE(Convert(Varchar(5),[remissionGuide].[despachurehour]),':','h') as 'Fch.Despacho'
		,ISNULL((CONVERT(VARCHAR(10),([remissionGuideControlVehicle].[exitDateProductionBuilding]),120) 
			+ ' ' + REPLACE((CONVERT(VARCHAR(5),([remissionGuideControlVehicle].[exitTimeProductionBuilding]),8)),':','h')),'') as 'FechaSalidaPlanta'
		,ISNULL((CONVERT(VARCHAR(10),([remissionGuideControlVehicle].[entranceDateProductionUnitProviderBuilding]),120) 
			+ ' ' + CONVERT(VARCHAR,([remissionGuideControlVehicle].[entranceTimeProductionUnitProviderBuilding]),8)),'') as 'FechaLLegadaCamaronera'
		,ISNULL((CONVERT(VARCHAR(10),([remissionGuideControlVehicle].[exitDateProductionUnitProviderBuilding]),120) 
			+ ' ' + CONVERT(VARCHAR,([remissionGuideControlVehicle].[exitTimeProductionUnitProviderBuilding]),8)),'') as 'FechaSalidaCamaronera'
		,ISNULL((CONVERT(VARCHAR(10),([remissionGuideControlVehicle].[entranceDateProductionBuilding]),120) 
			+ ' ' + CONVERT(VARCHAR,([remissionGuideControlVehicle].[entranceTimeProductionBuilding]),8)),'') as 'FechaEntradaPlanta'
		,'Comprador' = (Select top(1) [person].[fullname_businessName] from [Person] [person] WHERE [person].[id] = [remissionGuide].[id_buyer])
		,'No.OrdenCompra' = ISNULL((SELECT top(1) [d]. [number] FROM [Document][d] Where [d].[id] = [purchaseOrder].[id]),'')
		,CONVERT(VARCHAR(10),([purchaseOrder].[deliveryDate]),20) as 'Fch.OrdenCompra'
		,'DescripciónCompra' = ISNULL((SELECT top(1) [d].[description] FROM [Document][d] Where [d].[id] = [purchaseOrder].[id]),'')
		,'Proveedor' = (Select top(1) [person].[fullname_businessName] from [Person] [person] WHERE [person].[id] = [remissionGuide].[id_providerRemisionGuide])
		,'Camaronera' = (Select top(1) [person].[fullname_businessName] from [Person] [person] WHERE [person].[id] = [remissionGuide].[id_protectiveProvider])
		,'Zona' = (Select top(1) [fishingZone].[name] from [FishingZone][fishingZone] 
					where [fishingZone].[id] = [productionUnitProvider].[id_fishingZone])
		,'Sitio' = (Select top(1) [fishingSite].[name] from [FishingSite][fishingSite] 
					where [fishingSite].[id] = [productionUnitProvider].[id_fishingSite])
		,ISNULL([productionUnitProvider].[INPnumber],'') as 'INP'
		,ISNULL([productionUnitProvider].[ministerialAgreement],'') AS 'No.Acuerdo'
		,ISNULL([productionUnitProvider].[tramitNumber],'') AS 'No.Trámite'
		,ISNULL([remissionGuide].[startAdress],'') AS 'Dir.Partida'
		,ISNULL([remissionGuide].[route],'') as 'Dir.Llegada'
		,[purchaseOrderShippingType].[name] as 'VíaTransporte'
		,'Producto' = (select top(1) [i].[name] from [Item][i] where [i].[id] = [purchaseOrderDetail].[id_item])
		,[remissionGuideDetail].[quantityOrdered] as 'Cant.Ordenada'
		,[remissionGuideDetail].[quantityProgrammed] as 'Cant.Programada'
		,[remissionGuideDetail].[quantityDispatchPending] as 'Cant.Pendiente'
		,[remissionGuideDetail].[quantityReceived] as 'Cant.Recibida'
		--,[DM].[Gavetas]
		--,[DM].[Metabisulfito]
		--,[DM].[Sal saco de 50 lbs]
		--,[DM].[Saco con Hielo 50lbs]
		--,[DM].[Sacos Vacíos]
		--,[DM].[Bines Procamaronex]

		,[DM].[GAVETAS CERRADAS],
		[DM].[GAVETAS CALADAS],
		[DM].[SACO CON HIELO TERCEROS],
		[DM].METABISULFITO ,
		[DM].SAL,
		[DM].[SACOS VACIOS TERCEROS],
		[DM].[SACO CON HIELO],
		[DM].[SACOS VACIOS],
		[DM].[BINES AZUL],
		[DM].[BINES TERCEROS],
		[DM].[SELLOS DE BINES]
			,case
			when [remissionGuideTransportation].isOwn = 0 then 'No'
			when [remissionGuideTransportation].isOwn = 1 then 'Si'
		 end as 'TransporteTercero'
		 ,'Tarifario' = ISNULL((Select top(1) [transportTariffType].[name] From [TransportTariffType][transportTariffType] where [remissionGuide].[id_TransportTariffType] = [transportTariffType].[id]),'')
		 ,'CiaTransporte' = ISNULL((Select top(1) [p].[fullName_businessName] from [Person][p] Where [p].[id] = [remissionGuideTransportation].[id_provider]),'')
		 ,[remissionGuideTransportation].[carRegistration] 'Placa'
		 ,'Marca' = ISNULL((Select top(1) [v].[mark] from [Vehicle] [v] Where [remissionGuideTransportation].[id_vehicle] = [v].[id]),'')
		 ,'Modelo' = ISNULL((Select top(1) [v].[model] from [Vehicle] [v] Where [remissionGuideTransportation].[id_vehicle] = [v].[id]),'')
		 ,'CandadoHunter' = ISNULL((Select top(1) [v].[hunterLockText] from [Vehicle] [v] Where [remissionGuideTransportation].[id_vehicle] = [v].[id]),'')
		 ,[remissionGuideTransportation].[driverName] as 'Chofer'
		 ,ISNULL([remissionGuideTransportation].[valuePrice],0) as 'ValorFlete'
		 ,ISNULL([remissionGuideTransportation].[advancePrice],0) as 'ValorAnticipo'
		 ,[remissionGuideTransportation].[carRegistration] 'PlacaTercero'
		 ,[remissionGuideTransportation].[driverName] as 'ChoferTercero'
		 ,ISNULL([remissionGuideTransportation].[descriptionTrans],'') as 'Observacion'
		 ,'ProveedorHielo' = ISNULL((select top(1) [ibc].[name_ProviderIceBags] From [RemissionGuideCustomizedIceBuyInformation][ibc] Where [ibc].[id_remissionGuide] = [remissionGuide].[id]),'')
		 ,'No.Factura' = ISNULL((select top(1) [valorLiquidacion].[invoiceNumber] from [#ValorLiquidacion][valorLiquidacion] where [valorLiquidacion].[id] = [remissionGuide].[id]),'')
		 ,ISNULL([liquidationFreightDetail].[pricetotal],0) 'valorLiquidado'
		 ,ISNULL([NumLiq].[sequential],'') as 'No.LiquidaciónFlete'
		 ,'PersonaSeguridad' = ISNULL((select top(1) [p].[fullname_businessName] FROM [Person][p] 
										where [p].[id] = (select top(1) [pa].[id_person] From [#PersonalAsignado][pa] where [pa].[id] = [remissionGuide].[id] 
											and [pa].[personAsigCode] = 'SEG')),'')
		 --,'Viaje' = ISNULL((select top(1) [pa].[tipoViaje] From [#PersonalAsignado][pa] where [pa].[id] = [remissionGuide].[id] 
			--								and [pa].[personAsigCode] = 'SEG'),'')
		 --,'Viatico' = ISNULL((select top(1) [pa].[viaticPrice] From [#PersonalAsignado][pa] where [pa].[id] = [remissionGuide].[id] 
			--								and [pa].[personAsigCode] = 'SEG'),0)
		 ,'PersonaBiologo' = ISNULL((select top(1) [p].[fullname_businessName] FROM [Person][p] 
										where [p].[id] = (select top(1) [pa].[id_person] From [#PersonalAsignado][pa] where [pa].[id] = [remissionGuide].[id] 
											and [pa].[personAsigCode] = 'BIO')),'')
		 --,'Viaje' = ISNULL((select top(1) [pa].[tipoViaje] From [#PersonalAsignado][pa] where [pa].[id] = [remissionGuide].[id] 
			--								and [pa].[personAsigCode] = 'BIO'),'')
		 --,'Viatico' = ISNULL((select top(1) [pa].[viaticPrice] From [#PersonalAsignado][pa] where [pa].[id] = [remissionGuide].[id] 
											--and [pa].[personAsigCode] = 'BIO'),0)
		 ,'PersonaChofer' = ISNULL((select top(1) [p].[fullname_businessName] FROM [Person][p] 
										where [p].[id] = (select top(1) [pa].[id_person] From [#PersonalAsignado][pa] where [pa].[id] = [remissionGuide].[id] 
											and [pa].[personAsigCode] = 'CHO')),'')
		 ,[vg].[Sello 1]
		 ,[vg].[Viaje 1]
		 ,[vg].[Estado Salida 1]
		 ,[vg].[Estado Llegada 1]
		 ,[vg].[Sello 2]
		 ,[vg].[Viaje 2]
		 ,[vg].[Estado Salida 2]
		 ,[vg].[Estado Llegada 2]
		 ,[vg].[Sello 3]
		 ,[vg].[Viaje 3]
		 ,[vg].[Estado Salida 3]
		 ,[vg].[Estado Llegada 3]
		 ,[vg].[Sello 4]
		 ,[vg].[Viaje 4]
		 ,[vg].[Estado Salida 4]
		 ,[vg].[Estado Llegada 4]
		 ,(select top(1) processPlant from person where id = [remissionGuide].[id_personProcessPlant]) 'Proceso Planta'
		 , isnull(cer.code,'') as Certificado
	from [RemissionGuide] [remissionGuide]
		inner join [RemissionGuideDetail][remissionGuideDetail]
	on [remissionGuideDetail].[id_remisionGuide] = [remissionGuide].[id]
		inner join [RemissionGuideDetailPurchaseOrderDetail][remissionGuideDetailPurchaseOrderDetail]
	on [remissionGuideDetailPurchaseOrderDetail].[id_remissionGuideDetail] = [remissionGuideDetail].[id]
		inner join [RemissionGuideTransportation][remissionGuideTransportation]
	on [remissionGuideTransportation].[id_remionGuide] = [remissionGuide].[id]
		inner Join [PurchaseOrderDetail][purchaseOrderDetail]
	on [purchaseOrderDetail].[id] = [remissionGuideDetailPurchaseOrderDetail].[id_purchaseOrderDetail]
		INNer join [PurchaseOrder] [purchaseOrder]
	on [purchaseOrder].[id] = [purchaseOrderDetail].[id_purchaseOrder]
		inner join [RemissionGuideType][remissionGuideType]
	on [remissionGuideType].[id] = [remissionGuide].[id_remissionGuideType]
		left outer Join [LiquidationFreightDetail][liquidationFreightDetail]
	on [liquidationFreightDetail].[id_RemisionGuide] = [remissionGuide].[id]
		left outer Join [LiquidationFreight][liquidationFreight]
	on [liquidationFreight].[id] = [liquidationFreightDetail].[id_LiquidationFreight]
		left join [RemissionGuideControlVehicle][remissionGuideControlVehicle]
	on [remissionGuideControlVehicle].[id_remissionGuide] = [remissionGuide].[id]
		inner join [ProductionUnitProvider][productionUnitProvider]
	on [productionUnitProvider].[id] = [remissionGuide].[id_productionUnitProvider]
		inner join [PurchaseOrderShippingType][purchaseOrderShippingType]
	on [purchaseOrderShippingType].[id] = [remissionGuide].[id_shippingType]
		left join [Document][NumLiq]
	on [NumLiq].[id] = [liquidationFreight].[id]
	--	inner join [documentState][NumLiqDs]
	--on [NumLiqDs].[id] = [NumLiq].[id_documentState] and [NumLiqDs].[code] <> '05'
		inner join [Document] [document]
	on [document].[id] = [remissionGuide].[id]
		inner join [DocumentState] [documentState]
	on [documentState].[id] = [document].[id_documentState]
		inner join [DocumentType] [documentType]
	on [documentType].[id] = [document].[id_documentType]
		inner join [#Documentos][documentos]
	on [documentos].[id] = [document].[id]
		inner join [#ViajesGuia] [vg]
	on [vg].[id_guia] = [remissionGuide].[id]
		and [document].[id] = [vg].[id_guia]
		INNER JOIN [#dispatchMaterial] [DM]
	on [DM].[id_guia] = [remissionGuide].[id]
		and [document].[id] = [vg].[id_guia]
		left JOIN [ElectronicDocument] Ed
	on ed.id = document.id
		left JOIN [ElectronicDocumentState] eds
	on eds.id = ed.id_electronicDocumentState
	    left join Certification cer on cer.id = [remissionGuide].id_certification
	--where document.sequential in (56763,56762)
	ORDER BY [document].[number],[documentState].[name]



	




