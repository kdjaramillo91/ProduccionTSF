/****** Object:  StoredProcedure [dbo].[spPar_RptHorasNoTrabajadasDetalleReport]    Script Date: 23/02/2023 03:37:11 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER procedure [spPar_RptHorasNoTrabajadasDetalleReport]
	@id int
AS
	Set noCount on

	declare @motivos Table(
		motivoLote int
	)

	declare @MotivoLote Table(
		id_motiveLot int ,
		motivo varchar(250),
		lote varchar(250),
		code varchar(250)
	)

	declare @sumTiempo table(
		id_nonProductiveHour int,
		stop bit,
		totalHora varchar(20),
		minutos int
	)

	declare @General Table(
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
		horaInicio  varchar(10),
		fechaFin Datetime,
		horaFin varchar(10),
		totalHours varchar(250),
		observacion varchar(250),
		totalParada varchar(10),
		totalProduccion varchar(10),
		total varchar(10),
		idMaquina int,
		id int
	)
	
	declare @idDetail table(
		id int
	)

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


	insert into @motivos
	select distinct 
		id_motiveLot
	from NonProductiveHourDetail

	declare @motivoId int
	declare @lote varchar(250)
	declare @Motivo varchar(250)
	--declare @code varchar(250)
	while((select count(*) from @motivos) > 0)
	begin
		set @motivoId = (select top 1  motivoLote from @motivos)
		set @lote = isnull((select internalNumber from ProductionLot where id = @motivoId),'');
		set @Motivo = isnull((select name from productiveHoursReason where id = @motivoId),'');
		--set @code = isnull((select code from productiveHoursReason where id = @motivoId),'');
		insert into @MotivoLote
			select @motivoId,@lote,@Motivo,@motivoId

		delete top(1) @motivos
	end

	declare @totalParada  varchar(10)
	declare @totalProduccion  varchar(10)
	declare @total  varchar(10)

	set @totalParada = ( select convert (char (5), dateadd (minute,sum(minutos), ''), 114)from @sumTiempo where stop = 1)
	set @totalProduccion =  (select convert (char (5), dateadd (minute,sum(minutos), ''), 114)from @sumTiempo where stop = 0)
	set @total = (select convert (char (5), dateadd (minute,sum(minutos), ''), 114) from @sumTiempo)

	insert into @General
	select 
		d.number 'numeroDocumento'
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
		,ml.code
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
	where nph.id = @id
	order by ml.code

	--select * from @general

		declare  @detalleNoProductivo Table(
			motivo varchar(250),
			paradas int,
			minutos int,
			totalMinutos int,
			idmot int,
			code varchar(10)
		)

		declare  @detalleNoProductivo2 Table(
			motivo varchar(250),
			paradas int,
			minutos int,
			totalMinutos int,
			idmot int,
			code varchar(10)
		)

		
		----select * from @sumTiempo

		insert into @detalleNoProductivo
		select 
			loteMotivo
			,count(*)
			,case when convert(varchar(5),totalHours,114) = '24:00'
				then 1440 
				else
					sum(datediff(minute,'0:00:00',convert(datetime,totalHours)))
				end
			,0
			,id
			,''
		from  @general
		where parada = 1
		group by loteMotivo,totalHours,id

		--select * from @detalleNoProductivo

		insert into @detalleNoProductivo2
		select 
			motivo,
			sum(paradas),
			sum(minutos),
			sum(totalMinutos),
			idmot code
			,''
		from @detalleNoProductivo
		group by motivo,idmot

		--select * from @detalleNoProductivo2

		declare @totalmin int
		set @totalmin = (select sum(minutos) from @detalleNoProductivo)

		update @detalleNoProductivo2
		set totalMinutos = @totalmin

		update @detalleNoProductivo2
		set code = (select code from productiveHoursReason where id = idmot)

		select 
		*,
		 convert (char (5), dateadd (minute,minutos, ''), 114) 'horaMinutos', 
		 convert (char (5), dateadd (minute,totalMinutos, ''), 114) 'horaTotal'
		from @detalleNoProductivo2
		order by code