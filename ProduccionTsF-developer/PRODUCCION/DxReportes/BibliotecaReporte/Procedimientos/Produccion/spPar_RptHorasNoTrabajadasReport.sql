GO
/****** Object:  StoredProcedure [dbo].[Rpt_HorasNoTrabajadasReport]    Script Date: 17/02/2023 10:37:11 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER PROCEDURE [dbo].[spPar_RptHorasNoTrabajadasReport]
	@id int
as
	Set noCount on

	declare @motivos Table(
		motivoLote int,
		id_machineForProd int,
		idProccesType int
	)

	declare @MotivoLote Table(
		id_motiveLot int ,
		motivo varchar(250),
		lote varchar(250),
		librasProcesadas decimal(20,4),
		id_processType int
	)

	declare @loteLibras Table(
	id_motiveLot int ,
	librasProcesadas decimal(20,4)
	)

	declare @sumTiempo table(
		id_nonProductiveHour int,
		stop bit,
		totalHora varchar(20),
		minutos int
	)

	declare @General Table(
		idDetalleNph int,
		numeroDocumento varchar(250),
		fechaEmision DateTime,
		estadoDocumento varchar(250),
		descripcion varchar(250),
		maquina varchar(250),
		proceso  varchar(250),
		Turno  varchar(250),
		parada  bit,
		loteMotivo  varchar(250),
		fechaInicio Datetime,
		horaInicio  varchar(20),
		fechaFin Datetime,
		horaFin varchar(20),
		totalHours varchar(250),
		observacion varchar(250),
		totalParada varchar(20),
		totalProduccion varchar(20),
		total varchar(20),
		idMaquina int,
		librasProcesadas Decimal(20,4),
		id_motiveLot int,
		id_RemisionGuide int,
		id_NonProductiveHour int,
		numPerson int
	)
	
	declare @CantidadProcesada table(
		id int,
		quantityPoundsIL numeric(20,4),
		quantityKgsIL numeric(20,4),
		quantityKgsITW numeric(20,4),
		quantityPoundsITW numeric(20,4)
	)

	DECLARE @Final Table(
		numeroDocumento	varchar(250),
		fechaEmision	datetime,
		estadoDocumento	varchar(250),
		descripcion	varchar(250),
		id_Maquina  int,
		maquina 	varchar(250),
		proceso		varchar(250),
		Turno		varchar(250),
		totalHours	varchar(5),
		totalParada	varchar(5),
		totalProduccion	varchar(5),
		total	varchar(20),
		fechaInico datetime,
		dProveedor	varchar(250),
		dPiscina	varchar(250),
		dgrammage numeric(20,4),
		dLote 	varchar(250),	
		dcc bit,
		dsc	bit,
		dLibrasIngresadas numeric(20,4),
		dHoraInicio	varchar(5),
		dHoraFin varchar(5),
		dlibrasProcesadas numeric(20,4)	,
		dnumeroPersonas	numeric(10,2),
		dMotivo		varchar(250),
		dobservacion 	varchar(250),
		id_NonProductiveHour int
	)

	declare @totalesLibrasProcesadas table(
		librasProcesadas numeric(20,4),
		code varchar(4)
	)

		declare @libras table(
		d2proceso bit,
		totalHours int
	)

	declare @idDetail table(
		id int
	)
	
	declare @Gramaje table(
		id_lot int,
		gramage numeric(14,4)
	)

	declare @Liquidacion table(
		id int,
		number varchar(25))

	declare @idLiquidacion table(
		id_nonProductiveHourDetail int,
		id_Liquidacion int)

	Insert into @Liquidacion
	select id,SUBSTRING(observation,9,18) 
	from nonProductiveHourDetail dh
	where id_nonProductiveHour = @id

	Insert Into @idLiquidacion
	Select distinct lq.id, dc.id 
	from @Liquidacion lq
	Inner Join Document dc
	   On dc.number = lq.number
	Inner Join DocumentType dt
	   On dt.id = dc.id_documentType
	Where dt.code = '74'

	
	insert into @Gramaje
	SELECT 
		qc.id_Lot
		,AVG(qc.grammageReference) 
	FROM qualityControl qc
	inner join productionLot pl
		on pl.id = qc.id_lot
	group by qc.id_lot

	--print(000)
	--insert into @sumTiempo
	--select 
	--	id_nonProductiveHour
	--	,stop
	--	,convert(varchar(20),dateadd (minute, (sum(datediff(minute,'0:00:00',totalHours))), ''),114)
	--	,sum(datediff(minute,'0:00:00',totalHours))
	--from NonProductiveHourDetail
	--where id_nonProductiveHour = @id
	--group by stop,id_nonProductiveHour
	--print(111)

	--print 'uuuuuuu'
	insert into @idDetail
	select id from nonProductiveHourDetail
	where id_nonProductiveHour = @id
	declare @hora varchar(5) 
	declare @fi time
	declare @fe time
	DECLARE @id_detail int

	while(select count(*) from @idDetail) > 0 
	begin
		set @id_detail = (select top(1) id from @idDetail)
		set @fi = (select top(1) startTime from nonProductiveHourDetail where id = @id_detail)
		set @fe = (select top(1) endTime from nonProductiveHourDetail where id = @id_detail)

		if(@fi = (select convert(time,'0:00')) and @fe = (select convert(time,'0:00')))
		begin
			set @hora = (select top(1) convert (char (8), dateadd (minute, 720, ''), 114))
			insert into @sumTiempo
			select top(1) id_nonproductivehour,stop,@hora,720 from nonProductiveHourDetail where id = @id_detail
		end 
		else
		begin 
			set @hora  = (select top(1)
				convert(varchar(5),dateadd (minute, (sum(datediff(minute,'0:00:00',totalHours))), ''),114)
				from NonProductiveHourDetail
				where id = @id_detail
				group by stop,id_nonProductiveHour)

				insert into @sumTiempo
				select top(1) id_nonproductivehour,stop,@hora,sum(datediff(minute,'0:00:00',totalHours)) from nonProductiveHourDetail where id = @id_detail group by stop,id_nonProductiveHour
		end

		delete top(1) @idDetail
	end
	--print 'uuuuu'
	insert into @motivos
	select
		nphd.id_motiveLot,
		nph.id_machineForProd,
		nphd.id_processType
	from  NonProductiveHour nph
	inner join NonProductiveHourDetail nphd
	on nphd.id_nonProductiveHour = nph.id
	where nph.id = @id

	Insert Into @loteLibras
	select distinct lcoc.id_productionLot,sum(lcocd.quantityPoundsIL) quantityPoundsIL
									from liquidationcartoncart lcoc
									Inner join document d
									on d.id = lcoc.id
									Inner join documentState ds 
									on ds.id = d.id_documentState
									inner join liquidationcartoncartDetail lcocd
									on lcoc.id = lcocd.id_LiquidationCartOnCart
									inner join (select distinct * from @motivos) m
									on m.motivoLote = lcoc.id_productionLot
									and m.id_machineForProd = lcoc.id_MachineForProd 
									and m.idProccesType = lcoc.idProccesType
									where ds.code <> '05'
									group by lcoc.id_productionLot, lcoc.idProccesType

	declare @motivoId int
	declare @lote varchar(250)
	declare @Motivo varchar(250)
	declare @librasProcesadas decimal(20,4)
	declare @idProcessType int

	--select * from @motivos
	while((select count(*) from @motivos) > 0)
	begin
		set @motivoId = (select top 1  motivoLote from @motivos)
		set @lote = isnull((select internalNumber from ProductionLot where id = @motivoId),'')
		set @Motivo = isnull((select name from productiveHoursReason where id = @motivoId),'')
		set @librasProcesadas = 0
		--set @librasProcesadas = isnull((select top 1 sum(lcocd.quantityPoundsIL) quantityPoundsIL
		--							from liquidationcartoncart lcoc
		--							inner join document d
		--							on d.id = lcoc.id
		--							inner join documentState ds 
		--							on ds.id = d.id_documentState
		--							inner join liquidationcartoncartDetail lcocd
		--							on lcoc.id = lcocd.id_LiquidationCartOnCart
		--							inner join @motivos m
		--							on m.motivoLote = lcoc.id_productionLot
		--							and m.id_machineForProd = lcoc.id_MachineForProd 
		--							and m.idProccesType = lcoc.idProccesType
		--							where ds.code <> '05' 
		--							group by lcoc.id_productionLot, lcoc.idProccesType),0)

		set @idProcessType = isnull((select top 1 lcoc.idProccesType quantityPoundsITW
									from liquidationcartoncart lcoc
									inner join document d
									on d.id = lcoc.id
									inner join documentState ds 
									on ds.id = d.id_documentState
									inner join liquidationcartoncartDetail lcocd
									on lcoc.id = lcocd.id_LiquidationCartOnCart
									inner join @motivos m
									on m.motivoLote = lcoc.id_productionLot
									and m.id_machineForProd = lcoc.id_MachineForProd 
									and m.idProccesType = lcoc.idProccesType
									where ds.code <> '05'
									group by lcoc.id_productionLot, lcoc.idProccesType),0)
		insert into @MotivoLote
			select  @motivoId,@Motivo,@lote,@librasProcesadas,@idProcessType

		delete top(1) @motivos
	end
		

		
Update mlo
set librasProcesadas = lib.librasProcesadas
from @loteLibras lib
Inner Join @MotivoLote mlo
   On mlo.id_motiveLot = lib.id_motiveLot


--select * from @MotivoLote	
	declare @totalParada  varchar(250)
	declare @totalProduccion  varchar(250)
	declare @total  varchar(250)	
	set @totalParada = ( select 
	convert (char (5), dateadd (minute,sum(minutos), ''), 114)from @sumTiempo where stop = 1)
	set @totalProduccion =  (select convert (char (5), dateadd (minute,sum(minutos), ''), 114)from @sumTiempo where stop = 0)
	set @total = (select convert (char (5), dateadd (minute,sum(minutos), ''), 114) from @sumTiempo)
	
	--(select convert(varchar(5),dateadd (minute, (sum(datediff(minute,'0:00:00',sum(minutos)))), ''),114) 
	--						from NonProductiveHourDetail where id_nonProductiveHour = @id)

	--insert into @CantidadProcesada
	--select 
	--	 LCOC.id
	--	,sum(lcocd.quantityPoundsIL) 'quantityPoundsIL'
	--	,sum(lcocd.quantityKgsIL) 'quantityKgsIL'
	--	,sum(lcocd.quantityKgsITW	) 'quantityKgsITW'
	--	,sum(lcocd.quantityPoundsITW) 'quantityPoundsITW'
	--from LiquidationCartOnCart lcoc
	--inner join LiquidationCartOnCartDetail lcocd
	--on lcocd.id_LiquidationCartOnCart = LCOC.id
	--where LCOC.id = @id
	--group by LCOC.id
	--select  * from @MotivoLote
	insert into @General
	select distinct
		nphd.id 'idDetalleNonProductiveHour'
		,d.number 'numeroDocumento'
		,d.emissionDate 'fechaEmision'
		,ds.name 'estadoDocumento'
		,d.description 'descripcion'
		,mfp.name 'maquina'
		,pmfp.processPlant 'proceso'
		,t.name 'Turno'
		,nphd.stop 'parada'
		,case 
			when ml.motivo = '' then ml.lote else ml.motivo
		end 'loteMotivo'
		,nphd.startDate 'fechaInicio'
		,Convert(varchar(5),nphd.startTime,114) 'horaInicio'
		,nphd.endDate 'fechaFin'
		,Convert(varchar(5),nphd.endTime,114) 'horaFin'
		,nphd.totalHours 'totalHours'
		,nphd.observation 'observacion'
		,left(@totalParada,5) 'totalParada'
		,left(@totalProduccion,5) 'totalProduccion'
		,left(@total,5) 'total'
		,mfp.id 'idMaquina',0
		,nphd.id_motiveLot 
		,nphd.id_RemisionGuide
		,nphd.id_nonProductiveHour
		,isnull(nphd.numPerson,0)
		--,ml.librasProcesadas 
	from NonProductiveHour nph
	inner join NonProductiveHourDetail nphd
		on nph.id = nphd.id_nonProductiveHour
	inner join document d
		on d.id = nph.id
	inner join documentType dt
		on dt.id = d.id_documentType
	inner join documentState ds
		on ds.id = d.id_documentState
	inner join machineForProd mfp
		on mfp.id = nph.id_machineForProd
	inner join person pmfp
		on pmfp.id = mfp.id_personProcessPlant
	inner join turn t
		on t.id = nph.id_turn
	inner join @MotivoLote ml
		on ml.id_motiveLot = nphd.id_motiveLot
		--and ml.id_processType = nphd.id_processType 
	where nph.id = @id and nphd.stop = 1
	--order by nphd.startDate,nphd.startTime

	--select * from @General
	insert into @General
	select distinct
		nphd.id 'idDetalleNonProductiveHour'
		,d.number 'numeroDocumento'
		,d.emissionDate 'fechaEmision'
		,ds.name 'estadoDocumento'
		,d.description 'descripcion'
		,mfp.name 'maquina'
		,pmfp.processPlant 'proceso'
		,t.name 'Turno'
		,nphd.stop 'parada'
		,case 
			when ml.motivo = '' then ml.lote else ml.motivo
		end 'loteMotivo'
		,nphd.startDate 'fechaInicio'
		,Convert(varchar(5),nphd.startTime,114) 'horaInicio'
		,nphd.endDate 'fechaFin'
		,Convert(varchar(5),nphd.endTime,114) 'horaFin'
		,nphd.totalHours 'totalHours'
		,nphd.observation 'observacion'
		,left(@totalParada,5) 'totalParada'
		,left(@totalProduccion,5) 'totalProduccion'
		,left(@total,5) 'total'
		,mfp.id 'idMaquina'
		,ml.librasProcesadas 
		,nphd.id_motiveLot
		,nphd.id_RemisionGuide
		,nphd.id_nonProductiveHour
		,isnull(nphd.numPerson,0)
	from NonProductiveHour nph
	inner join NonProductiveHourDetail nphd
		on nph.id = nphd.id_nonProductiveHour
	inner join document d
		on d.id = nph.id
	inner join documentType dt
		on dt.id = d.id_documentType
	inner join documentState ds
		on ds.id = d.id_documentState
	inner join machineForProd mfp
		on mfp.id = nph.id_machineForProd
	inner join person pmfp
		on pmfp.id = mfp.id_personProcessPlant
	inner join turn t
		on t.id = nph.id_turn
	inner join @MotivoLote ml
		on ml.id_motiveLot = nphd.id_motiveLot
		and ml.id_processType = nphd.id_processType 
	where nph.id = @id and nphd.stop = 0



	update @General set librasProcesadas = 0 where parada = 0 and idDetalleNph not in (
	select max(idDetalleNph) from @General where parada = 0 group by loteMotivo)

	
	insert into @Final
	select distinct
		g.numeroDocumento 'numeroDocumento'
		,g.fechaEmision 'fechaEmision'
		,g.estadoDocumento 'estadoDocumento'
		,g.descripcion 'descripcion'
		,g.idMaquina 'idMaquina'
		,g.maquina 'maquina'
		,g.proceso 'proceso'
		,g.Turno 'Turno'
		,g.totalHours 'totalHours'
		,g.totalParada 'totalParada'
		,g.totalProduccion 'totalProduccion'
		,g.total 'total'
		,g.fechaInicio 'FechaInico'
		,pprov.fullname_businessName 'dProveedor'
		,pul.name 'dPiscina'
		--,gr.gramage 'dgrammage'
		,isnull(temp.grammageReference,0) 'dgrammage'
		----isnull(ap.grammageLot,0)
		---- qc.grammageReference 
		,pl.internalNumber 'dLote'
		,case when pt.code = 'ENT' then 1 else 0 end 'dcc'
		,case when pt.code = 'COL' then 1 else 0 end 'dsc'
		--,pl.totalQuantityRecived 'dLibrasIngresadas'
		,isnull(temp.quantityRecived,0) 'dLibrasIngresadas'
		,g.horaInicio 'dHoraInicio'
		,g.horaFin 'dHoraFin'
		,g.librasProcesadas 'dlibrasProcesadas'
		,g.numPerson 'dnumeroPersonas'
		,'ACTIVO' 'dMotivo'
		,left(g.observacion,25) + ' Guia No: ' + temp.number 'dobservacion'
		, g.id_NonProductiveHour
	from @General g
	inner join productionLot pl
		on pl.internalNumber = g.loteMotivo
	inner join MachineForProd mfp
		on mfp.id = g.idMaquina
	inner join LiquidationCartOnCart lcoc
		on pl.id = lcoc.id_productionLot
    inner Join @idLiquidacion idl
	    on idl.id_Liquidacion = lcoc.id
	inner join document d
		on d.id = lcoc.id
	inner join documentState ds 
		on ds.id = d.id_documentState
	and mfp.id = lcoc.id_MachineForProd
	inner join person pprov
		on pprov.id = pl.id_provider
	inner join ProductionUnitProvider pu
		on pu.id = pl.id_productionUnitProvider
	inner join ProductionUnitProviderPool pul
		on pul.id_productionUnitProvider = pu.id
		and pl.id_productionUnitProviderPool = pul.id
	inner join nonProductiveHourDetail nphd
		on nphd.id = g.idDetalleNph
	inner join processType pt
		on pt.id = nphd.id_processType
	inner join MachineProdOpeningDetail mfpo
		on mfpo.id_machineProdOpening = lcoc.id_machineProdOpening
	left join @Gramaje gr
		on gr.id_lot = pl.id
	left Join (select distinct drg.id_remisionGuide id_remissionGuide,QCon.grammageReference,dc.number,Pld.* 
	     from RemissionGuideDetail drg
        Inner Join Document dc
		   On dc.id = drg.id_remisionGuide
		Inner Join ProductionLotDetailPurchaseDetail pdt
		   On pdt.id_remissionGuideDetail = drg.id
		Inner Join ProductionLotDetail Pld
		   On Pld.id = pdt.id_productionLotDetail
		Left Outer Join ProductionLotDetailQualityControl pqd
		   On pqd.id_productionLotDetail = Pld.id
        Left Outer Join QualityControl QCon
		   On QCon.id = pqd.id_qualityControl) temp
       On temp.id_productionLot = pl.id
      And  temp.id_remissionGuide = g.id_RemisionGuide
	where parada = 0
	and ds.code <> '05'
	--select * from @Final
	

	insert into @Final
	select 
		g.numeroDocumento 'numeroDocumento'
		,g.fechaEmision 'fechaEmision'
		,g.estadoDocumento 'estadoDocumento'
		,g.descripcion 'descripcion'
		,g.idMaquina 'idMaquina'
		,g.maquina 'maquina'
		,g.proceso 'proceso'
		,g.Turno 'Turno'
		,g.totalHours 'totalHours'
		,g.totalParada 'totalParada'
		,g.totalProduccion 'totalProduccion'
		,g.total 'total'
		,g.fechaInicio
		,'' 'dProveedor'
		,'' 'dPiscina'
		,0
		,''
		,0
		,0
		,0
		,g.horaInicio 'dHoraInicio'
		,g.horaFin 'dHoraFin'
		,0.000 'dlibrasProcesadas'
		,0
		,g.loteMotivo 'dMotivo'
		,ISNULL(g.observacion,'') 'dobservacion'
		,g.id_NonProductiveHour
	from @general g
	where parada = 1


	--select * from @Final
	--Update fn
	--  Set dnumeroPersonas = mpd.numPerson
	--from @Final fn
	--Inner join MachineProdOpeningDetail mpd
	--   On mpd.id_MachineForProd = fn.id_Maquina
	--Inner Join Document dc
	--   On dc.id = mpd.id_MachineProdOpening
	--Inner Join NonProductiveHour nph
	--   On nph.id = fn.id_NonProductiveHour
	--Where fn.dLote = ''
	--  And convert(varchar(10),dc.emissionDate,112) = convert(varchar(10),nph.emissionDate,112)
	
	--update @Final set dLibrasIngresadas = dlibrasProcesadas where dlibrasProcesadas = 0 and dLote <> ''

	insert into @libras
	select distinct
		0
		,sum(datediff(minute,'0:00:00',convert(datetime,totalHours)))
	from @Final
	where dcc = 1

	insert into @libras
	select  distinct
		1
		,sum(datediff(minute,'0:00:00',convert(datetime,totalHours)))
	from @Final
	where dsc = 1

	declare @horasMaquina int
	set @horasMaquina = (select sum(totalHours) from @libras)

	insert into @totalesLibrasProcesadas
	select 
		ml.librasProcesadas,
		pt.code
	from @MotivoLote ml
	inner join productionLot pl
	on pl.internalNumber =  ml.motivo
	inner join nonProductiveHourDetail nphd
	on nphd.id_motiveLot = pl.id
	and ml.id_processType = nphd.id_processType
	inner join processType pt
	on pt.id = nphd.id_processType
	where ml.motivo <> '' and nphd.stop = 0

	--select * from @totalesLibrasProcesadas
	
	select 
	*
	,isNULL((select isNULL(sum(dlibrasProcesadas),0) from @Final where dcc = 1),0) 'Cabeza'
	,(select convert(varchar(5),dateadd (minute,totalHours,''),114) from @libras where d2proceso = 0) 'HoraCabeza'
	,isNULL((select isNULL(sum(dLibrasProcesadas),0) from @Final where dsc = 1),0) 'Cola'
	,isNULL((select isNULL(sum(dLibrasProcesadas),0) from @Final),0) 'totald2'
	,(select convert(varchar(5),dateadd (minute,totalHours,''),114) from @libras where d2proceso = 1) 'HoraCola'
	,(select convert(varchar(5),dateadd (minute,@horasMaquina,''),114)) 'totalHoras'
	,isnull((select CEILING(sum(dnumeroPersonas)/2) from @final),0) 'Personas'
	,isNULL((select sum(minutos) from @sumTiempo where stop = 1),0) MinutosNoProductivo 
	,isNULL((select sum(minutos) from @sumTiempo where stop = 0),0) minutosProductivo
	,isnull((isnull((select sum(minutos) from @sumTiempo where stop = 1),0)/(select sum(minutos) from @sumTiempo)),0) porminutosNoProductivo
	,isnull((isnull((select sum(minutos) from @sumTiempo where stop = 0),0)/(select sum(minutos) from @sumTiempo)),0) porminutosProductivo
	,isNULL((select isNULL(sum(totalHours),0) from @libras where d2proceso = 0),0) 'Cabezaminuto'
	,isNULL((select isNULL(sum(totalHours),0) from @libras where d2proceso = 1),0) 'Colaminuto'
	,isNULL((select isNULL(sum(totalHours),0) from @libras),0) 'Totalminuto'
	,(SELECT TOP(1) C.logo2 From Document d
		inner join EmissionPoint ep on d.id_emissionPoint = ep.id
		inner join Company c on c.id = ep.id_company
		where d.id = @id) logo
	from @Final
	order by fechaInico,dhoraInicio
