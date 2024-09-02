         GO
/****** Object:  StoredProcedure [dbo].[spRpt_AnalisisCalidad] 986526    Script Date: 16/02/2023 05:35:35 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create or alter PROCEDURE [dbo].[spRpt_AnalisisCalidad]
	@id int
as
	Set noCount on
	Declare @OlorCrudo Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @SaborCabeza Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @OlorCocido Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @SaborCola Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @Color Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @Flacidez Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @Mudado Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @DesidratadoLeve Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @DesidratadoMode Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @CabezaFloja Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @CabezaRoja Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @CabezaAnaranjada Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @HepatoRojo Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @BranquiasSucias Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @PicadoLeve Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @PicadoFuerte Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @Quebrado Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @Melanosis Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @Rosado Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @Semirosado Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @Corbata Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @Juvenil Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @Otros Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @especie Table (id_qualityControl	Int,resultValue	varchar(max))

	Declare @TotalDefectos Table (id_qualityControl	Int,TotalDefectos	numeric(13,2))
	Declare @TotalOtrasEspecies Table(id_qualityControl	Int,TotalOtrasEspecies	numeric(13,2))
	Declare @TotalPiezas Table (id_qualityControl	Int,TotalPiezas int)

	Declare @Californiense Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @Stylirrostris Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @Occidental Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @MaterialExtraño Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @AccionesCorrectivas Table (id_qualityControl	Int,resultValue	varchar(max))
	Declare @TotalRecibidas Table (id	Int,quantitydrained	numeric(13,2))

	Insert Into @OlorCrudo	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'OLOR CRUDO'

	Insert Into @SaborCabeza	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'SABOR CABEZA'

	Insert Into @OlorCocido	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'OLOR COCIDO'

	Insert Into @SaborCola	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'SABOR COLA'

	Insert Into @Color	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'COLOR'

	Insert Into @Flacidez	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'FLACIDEZ'

	Insert Into @Mudado	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'MUDADO'

	Insert Into @DesidratadoLeve	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'DESHIDRATADO LEVE'

	Insert Into @DesidratadoMode	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'DESHIDRATADO MODER.'

	Insert Into @CabezaFloja	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'CABEZA FLOJA'

	Insert Into @CabezaRoja	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'CABEZA ROJA'

	Insert Into @CabezaAnaranjada	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'CABEZA ANARANJADA'

	Insert Into @HepatoRojo	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'HEPATO. ROTO'

	Insert Into @BranquiasSucias	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'BRANQUIAS SUCIAS'

	Insert Into @PicadoLeve	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'PICADO LEVE'

	Insert Into @PicadoFuerte	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'PICADO FUERTE'

	Insert Into @Quebrado	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'QUEBRADO'

	Insert Into @Melanosis	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'MELANOSIS'

	Insert Into @Rosado	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'ROSADO'

	Insert Into @Semirosado	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'SEMIROSADO'

	Insert Into @Corbata	
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'CORBATA'

	Insert Into @Juvenil
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'JUVENIL'

	Insert Into @Otros
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'OTROS'

	Insert Into @TotalDefectos
	Select id_qualityControl, SUM(CONVERT(decimal(13,2), REPLACE(resultValue,',','.')))TotalDefectos from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Inner Join QualityControlAnalysisGroupAnalysis qan On qan.id_QualityAnalysis = qa.id
	where id_QualityControlAnalysisGroup=2 and resultValue <> ''
	Group by id_qualityControl

		Insert Into @TotalPiezas
	Select	d.id_qualityControl, 
			sum(otherResultValue) TotalPiezas
	from(select  id_qualityControl,
					CONVERT(int,case 
								when (CHARINDEX('.', otherResultValue) > 0) then SUBSTRING(otherResultValue,0, CHARINDEX('.', otherResultValue))
								when (CHARINDEX(',', otherResultValue) > 0) then SUBSTRING(otherResultValue,0, CHARINDEX(',', otherResultValue))
								else otherResultValue
							end) otherResultValue
			from	QualityControlDetail qcd
			Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
			Inner Join QualityControlAnalysisGroupAnalysis qan On qan.id_QualityAnalysis = qa.id
			where	id_QualityControlAnalysisGroup=2 
					and isnull(otherResultValue,'') <> '') d
	Group by d.id_qualityControl




	Insert Into @Californiense
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'P. CALIFORNIENSES'

	Insert Into @Stylirrostris
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'P. STYLIRROSTRIS'

	Insert Into @Occidental
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'P. OCCIDENTALES'	

	Insert Into @MaterialExtraño
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'MATERIAL EXTRAÑO'

		Insert Into @especie	
	Select id_qualityControl,resultValue = case when otherResultValue <> '' then resultValue + ' / ' + otherResultValue else resultValue end 
	from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'ESPECIE'

	Insert Into @TotalOtrasEspecies
	Select id_qualityControl, SUM(CONVERT(decimal(13,2), REPLACE(resultValue,',','.')))TotalOtrasEspecies from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Inner Join QualityControlAnalysisGroupAnalysis qan On qan.id_QualityAnalysis = qa.id
	where id_QualityControlAnalysisGroup=3 and resultValue <> ''
	Group by id_qualityControl

	Insert Into @AccionesCorrectivas
	Select id_qualityControl,resultValue from QualityControlDetail qcd
	Inner Join QualityAnalysis qa On qa.id = qcd.id_qualityAnalysis 
	Where qa.name = 'ACCIONES CORRECTIVAS'

	Insert Into @TotalRecibidas
	Select Plot.id,Sum(quantitydrained)quantitydrained from Productionlot Plot
	Inner Join ProductionlotDetail Dplo On Dplo.id_productionLot = Plot.id
	Group by Plot.id

	Select Distinct
		b.name as [TipodeAnalisis],
		a.number as [NAnalisis], 
		c.number as [SecTransaccional], 
		d.internalNumber as [NLote],
		d.receptionDate as [FechaHoradeLlegada],
		convert(varchar(250),e.fullname_businessName) as [Proveedor],
		convert(varchar(250),f.name) as [Camaronera],
		g.name as [NPiscina],
		Isnull(f.INPnumber,'') as [INP],
		Isnull(f.ministerialAgreement,'') as [NAcuerdoMinisterial],
		Isnull(f.tramitNumber,'') as [NTramite],
		k.number as [NGuiaRemision],
		l.name as [Producto],
		m.name as [TipoProceso],
		a.DrawersReceived as [NGavetas],
		d.totalQuantityRemitted as [LibrasProgramadas],
		d.totalQuantityRecived as [LibrasRemitidas],
		aw.quantitydrained as [LibrasRecibidas],
		a.qualityControlDate as [FechadeAnalisis],
		Convert(varchar,a.qualityControlTime ,108)as [HoradeAnalisis],
		iz.name as Talla,
		a.residual as [ResidualS02],
		Round(a.temperature,2,0) as [Temperatura],
		a.grammageReference as [Gramaje],
		o.resultValue as [OlorCrudo],
		p.resultValue as [SaborCabeza],
		q.resultValue as [OlorCocido],
		r.resultValue as [SaborCola],
		s.resultValue as [Color],
		t.resultValue as [Flacidez],
		u.resultValue as [Mudado],
		v.resultValue as [DeshidratadoLeve],
		w.resultValue as [DeshidratadoModer],
		x.resultValue as [CabezaFloja],
		y.resultValue as [CabezaRoja],
		z.resultValue as [CabezaAnaranjada],
		aa.resultValue as [HepatoRojo],
		ab.resultValue as [BranquiasSucias],
		ac.resultValue as [PicadoLeve],
		ad.resultValue as [PicadoFuerte],
		ae.resultValue as [Quebrado],
		af.resultValue as [Melanosis],
		ag.resultValue as [Rosado],
		ah.resultValue as [Semirosado],
		ai.resultValue as [Corbata],
		aj.resultValue as [Juvenil],
		ak.resultValue as [Otros],
		al.TotalDefectos as [TotalDefectos],
		a.wholePerformance as [RendimientoEntero],
		am.resultValue as [PCaliforniense],
		an.resultValue as [PStylirrostris],
		ao.resultValue as [POccidental],
		ap.resultValue as [MaterialExtrano],
		Isnull(at.TotalOtrasEspecies,0) as [TotalOtrasEspecies],
		aq.resultValue as [AccionesCorrectivas],
		Isnull(a.reference,'') as [Referencia],
		Isnull(av.fullname_businessName,'') as [Analista],
		Isnull(au.name,'') as [EsConforme],
		Isnull(a.description,'') as [Observacion],
		pd.processPlant as [Proceso],
		ds.name as [Estado],
		a.id_company as idcompany
		,isnull(a.totalWeightSample,0) as TotalMuestra
		,isnull(t.resultValue,0) as [Especie]
		,isnull(pz.TotalPiezas,0) as [TotalPiezas]
		,a.iceContamination as ContaminacionHielo
		,a.transportCondition as CondicionTransporte
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
	Inner Join @OlorCrudo o On o.id_qualityControl = a.id
	Inner Join @SaborCabeza p On p.id_qualityControl = a.id
	Inner Join @OlorCocido q On q.id_qualityControl = a.id
	Inner Join @SaborCola r On r.id_qualityControl = a.id
	Inner Join @Color s On s.id_qualityControl = a.id
	Inner Join @Flacidez t On t.id_qualityControl = a.id
	Inner Join @Mudado u On u.id_qualityControl = a.id
	Inner Join @DesidratadoLeve v On v.id_qualityControl = a.id
	Inner Join @DesidratadoMode w On w.id_qualityControl = a.id
	Inner Join @CabezaFloja x On x.id_qualityControl = a.id
	Inner Join @CabezaRoja y On y.id_qualityControl = a.id
	Inner Join @CabezaAnaranjada z On z.id_qualityControl = a.id
	Inner Join @HepatoRojo aa On aa.id_qualityControl = a.id
	Inner Join @BranquiasSucias ab On ab.id_qualityControl = a.id
	Inner Join @PicadoLeve ac On ac.id_qualityControl = a.id
	Inner Join @PicadoFuerte ad On ad.id_qualityControl = a.id
	Inner Join @Quebrado ae On ae.id_qualityControl = a.id
	Inner Join @Melanosis af On af.id_qualityControl = a.id
	Inner Join @Rosado ag On ag.id_qualityControl = a.id
	Inner Join @Semirosado ah On ah.id_qualityControl = a.id
	Inner Join @Corbata ai On ai.id_qualityControl = a.id
	Inner Join @Juvenil aj On aj.id_qualityControl = a.id
	Inner Join @Otros ak On ak.id_qualityControl = a.id
	Inner Join @TotalDefectos al On al.id_qualityControl = a.id 
	Inner Join @Californiense am On am.id_qualityControl = a.id 
	Inner Join @Stylirrostris an On an.id_qualityControl = a.id 
	Inner Join @Occidental ao On ao.id_qualityControl = a.id 
	Inner Join @MaterialExtraño ap On ap.id_qualityControl = a.id 
	Inner Join @AccionesCorrectivas aq On aq.id_qualityControl = a.id 
	Inner Join QualityControlResultConformityOnHeaderValue ar On ar.id_QualityControl = a.id 
	Inner Join QualityControlResultConformity au On au.id = ar.id_QualityControlResultConformity 
	Inner Join Person av On av.id = a.Id_analyst
	Inner Join @TotalRecibidas aw On aw.id = d.id
	Left outer Join @TotalOtrasEspecies at On at.id_qualityControl = a.id 
	Left outer Join @TotalPiezas pz On pz.id_qualityControl = a.id 
	Left outer Join ItemSize iz On iz.id = a.id_size
	where a.id = @id




