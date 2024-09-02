
GO
/****** Object:  StoredProcedure [dbo].[RptCierreMaquina]    Script Date: 4/1/2023 15:44:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[spRptCierreMaquina] --4451

	@id INT
AS
	Set NoCount On
	CREATE TABLE #DetalleNumerico(
		id_liquidationCartOnCart int,
		poundsProcessed NUMERIC(10,2),
		poundsCooling NUMERIC(10,2),
		noOfBox NUMERIC(10,2)
	)

	CREATE TABLE #Detalles(
		id INT,
		numeroLiquidacion VARCHAR(250),
		provider VARCHAR(250),
		nameProviderShrimp VARCHAR(250),
		productionUnitProviderPool VARCHAR(250),
		Detailweight NUMERIC(10,2),
		proccesType VARCHAR(250),
		numberLot VARCHAR(250),
		plantProcess VARCHAR(250),
		poundsRemitted NUMERIC(10,2),
		poundsProcessed NUMERIC(10,2),
		poundsCooling NUMERIC(10,2),
		noOfBox INT,
		cod_state VARCHAR(250),
		Detailstate VARCHAR(250),
		id_machineForProd INT,
		id_MachineProdOpening INT
	)

	CREATE TABLE #CabezaCola(
		id INT,
		Cola NUMERIC(10,2),
		Cabeza NUMERIC(10,2),
		ColaRefri NUMERIC(10,2),
		CabezaRefri NUMERIC(10,2)
	)

	declare @Gramaje table(
		id_lot int,
		gramage numeric(14,4)
	)
	insert into @Gramaje
	SELECT 
		qc.id_Lot
		,AVG(qc.grammageReference) 
	FROM qualityControl qc
	inner join productionLot pl
		on pl.id = qc.id_lot
	left join Document DQC
		on DQC.id = Qc.id
	left join DocumentState DQCDS
		on DQCDS.id = DQC.id_documentType
	   and DQCDS.code != '05'
	group by qc.id_lot

	INSERT INTO #CabezaCola
	SELECT 
		lcoc.id,
		case
			when pt.code = 'COL' then Sum(LCoCD.quantityPoundsIL) else 0 
		end Cola,
		case
			when pt.code = 'ENT' then Sum(LCoCD.quantityPoundsIL) else 0 
		end Cabeza,
		case
			when pt.code = 'COL' then Sum(LCoCD.quantityPoundsITW) else 0 
		end ColaRefri,
		case
			when pt.code = 'ENT' then Sum(LCoCD.quantityPoundsITW) else 0 
		end CabezaRefri
	from LiquidationCartOnCart LCoC
	Inner join LiquidationCartOnCartDetail LCoCD
	on LCoCD.id_LiquidationCartOnCart = LCoC.id
	Inner join ProcessType pt
	on pt.id = LCoC.idProccesType
	group by lcoc.id, pt.code

	INSERT INTO #DetalleNumerico
	SELECT 
		LCoC.id 'id_liquidationCartOnCart',
		SUM(LCoCD.quantityPoundsIL) 'poundsProcessed',
		SUM(LCoCD.quantityPoundsITW) 'poundsCooling',
		Convert(numeric(10,2),SUM(LCoCD.quatityBoxesIL)) 'noOfBox'
	from LiquidationCartOnCart LCoC
	Inner join LiquidationCartOnCartDetail LCoCD
	on LCoCD.id_LiquidationCartOnCart = LCoC.id
	group by LCoC.id


	Insert into #Detalles
	select distinct
		LCoC.id,
		DLCoC.number 'numeroLiquidacion',
		PPLLCoc.fullname_businessName 'provider',
		PLPUP.name 'nameProviderShrimp',
		PLPUPP.name 'productionUnitProviderPool',
		isnull(Gr.gramage,0) 'weight',
		Pt.name 'proccesType',
		PLLCoC.internalNumber 'numberLot',
		MFPP.fullname_businessName 'plantProcess',
		SUm(ISNULL(PLLCoCD.quantityRecived,0)) 'poundsRemitted',
		TmpDetalleNumerico.poundsProcessed 'poundsProcessed',
		TmpDetalleNumerico.poundsCooling 'poundsCooling',
		TmpDetalleNumerico.noOfBox 'noOfBox',
		DSLCoc.code 'cod_state',
		DSLCoc.name 'state',
		MFP.id,
		MPO.id
	from LiquidationCartOnCart LCoC
	Inner join MachineProdOpening MPO
	on MPO.id = LCoC.id_MachineProdOpening
	Inner join MachineProdOpeningDetail MPOD
	on MPOD.id_MachineProdOpening = MPO.id
	Inner Join ClosingMachinesTurn cmt
	On cmt.id_MachineProdOpeningDetail = MPOD.id
	Inner join Document DLCoC
	on DLCoC.id = LCoC.id
	inner join DocumentState DSLCoc
	on DSLCoc.id = DLCoC.id_documentState
	And DSLCoc.code <> '05'
		-- Join productionLot
		Inner join ProductionLot PLLCoC
		on PLLCoC.id = LCoC.id_ProductionLot
		Inner join ProductionLotDetail PLLCoCD
		on PLLCoCD.id_ProductionLot = PLLCoC.id
		Inner join Person PPLLCoc
		on PPLLCoc.id = PLLCoC.id_provider
		Inner join ProductionUnitProvider PLPUP
		on PLPUP.id = PLLCoC.id_productionUnitProvider
		Inner join ProductionUnitProviderPool PLPUPP
		on PLPUPP.id = PLLCoC.id_productionUnitProviderPool
		left join ProductionLotDetailQualityControl PLDQC
		on PLDQC.id_productionLotDetail = PLLCoCD.id
		left join @Gramaje gr
		on gr.id_lot = PLLCoC.id
			--left join QualityControl QC
			--ON QC.id = PLDQC.id_qualityControl
			--left join Document DQC
			--on DQC.id = Qc.id
			--left join DocumentState DQCDS
			--on DQCDS.id = DQC.id_documentType
			--and DQCDS.code != '05'

		Inner join ProcessType Pt
		on pt.id = LCoC.idProccesType
		-- join MachineForProd
		Inner Join MachineForProd MFP
		on MFP.id = LCoC.id_MachineForProd
		Inner join Person MFPP
		on MFPP.id = MFP.id_personProcessPlant
		-- fin join MachineForProd
	inner join #DetalleNumerico TmpDetalleNumerico
	on TmpDetalleNumerico.id_liquidationCartOnCart = LCoC.id
	group by LCoC.id,
		DLCoC.number,
		PPLLCoc.fullname_businessName,
		PLPUP.name,
		PLPUPP.name ,
		isnull(Gr.gramage,0),
		Pt.name ,
		PLLCoC.internalNumber,
		MFPP.fullname_businessName,
		TmpDetalleNumerico.poundsProcessed ,
		TmpDetalleNumerico.poundsCooling,
		TmpDetalleNumerico.noOfBox,
		DSLCoc.code,
		DSLCoc.name,
		MFP.id,
		MPO.id


	select 
		cmt.id id,
		d.number 'Number',
		d.emissiondate 'FechaEmission',
		MFP.name 'maquina',
		PMPOD.fullName_businessName 'Responsable',
		t.name 'Turno',
		ds.name 'estado',
		MPOD.numPerson 'NumeroPersona',
		PMFP.processPlant 'Proceso',
		tempDetalles.*,
		(SELECT TOP(1) C.logo2 From Document d
			inner join EmissionPoint ep on d.id_emissionPoint = ep.id
			inner join Company c on c.id = ep.id_company) Logo,
		tempCb.cabeza 'Cabeza',
		tempCb.cola 'Cola',
		tempCb.cabezaRefri 'CabezaRefri',
		tempCb.colaRefri 'ColaRefri'
	from ClosingMachinesTurn cmt
	INNER JOIN Document d
	ON D.id = cmt.id
	INNER JOIN DocumentState ds
	on ds.id = d.id_documentState
	INNER JOIN MachineProdOpeningDetail MPOD
	on MPOD.id = cmt.id_MachineProdOpeningDetail
	INNER JOIN MachineProdOpening MPO
	on MPO.id = MPOD.id_MachineProdOpening
	Inner join Turn t
	on t.id = MPO.id_turn
	INNER JOIN MachineForProd MFP
	ON MFP.id = MPOD.id_MachineForProd
	INNER JOIN Person PMFP
	on PMFP.id = MFP.id_personProcessPlant
	INNER JOIN person PMPOD
	on PMPOD.id = MPOD.id_Person
	left JOIN #Detalles tempDetalles
	on tempDetalles.id_machineForProd = MFP.id
	and tempDetalles.id_MachineProdOpening =  MPO.id 
	left JOIN #CabezaCola tempCb
	on tempCb.id = tempDetalles.id
	where cmt.id = @id

	
