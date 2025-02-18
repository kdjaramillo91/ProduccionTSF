-- -- insert into ObjectPermission values ('EXPFACTCOMMATRIZ','Factura Comercial Matriz','Reporte Matriz de Factura Comercial',1,1,GETDATE(),1,GETDATE())
/****** Object:  StoredProcedure [dbo].[spc_FacturasFiscalesMatrizReport]    Script Date: 12/03/2024 09:08:08 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO


	create or alter Procedure [dbo].[spc_FacturasComercialMatrizReport]
	@fechaEmisionInicio Datetime,
	@fechaEmisionFinal Datetime
	as 
	Declare @cad                  varchar(8000)   

	Set @fechaEmisionInicio = convert(Varchar,@fechaEmisionInicio,112) 
	Set @fechaEmisionFinal = convert(Varchar,@fechaEmisionFinal,112) 

	Create Table #tempSumas (
	TotalFactura Varchar(50),
	Secuencia Varchar(50),
	TotalCartones	Int,
	TotalPesoBrutoCRT Decimal(18,6),
	TotalLibras Decimal(18,6),
	TotalKilos Decimal(18,6),
	TotalPesoNetoLibras Decimal(18,6),
	TotalLibrasNetas Decimal(18,6),
	TotalValorNeto Decimal(18,6)
	)

	Create Table #temp(
	REFRENDO varchar(150)Collate DATABASE_DEFAULT NULL,
	DAU	Varchar(150)Collate DATABASE_DEFAULT NULL,
	FUE Varchar(150)Collate DATABASE_DEFAULT NULL,
	FCH_FUE Varchar(150)Collate DATABASE_DEFAULT NULL,
	RAZ_SOCIAL Varchar(500)Collate DATABASE_DEFAULT NULL,
	CONSIGNATARIO Varchar(500)Collate DATABASE_DEFAULT NULL,
	PAIS_ORIGE Varchar(500)Collate DATABASE_DEFAULT NULL,
	NRO_SERIE Varchar(150)Collate DATABASE_DEFAULT NULL,
	NRO_SECUEN Varchar(150)Collate DATABASE_DEFAULT NULL,
	Fecha_Emision Varchar(150)Collate DATABASE_DEFAULT NULL,
	NRO_AUTORI Varchar(180)Collate DATABASE_DEFAULT NULL,
	CARTONES int,
	PES_BRU_LB	decimal(13,6),
	PES_BRU_KL		decimal(13,6),
	PES_BRU_CRT	decimal(18,6),
	LIBRAS	decimal(18,6),
	KILOS	decimal(18,6),
	PES_NET_LB	decimal(18,6),
	PRECIO_LB	decimal(13,6),
	PRECIO_KL	decimal(13,6),
	PRE_CONV_LB	decimal(13,6),
	PRE_UNI	decimal(13,6),
	PRECIO_REF	varchar(150)Collate DATABASE_DEFAULT NULL,
	COD_PRODUCTO varchar(500)Collate DATABASE_DEFAULT NULL,
	NOMBRE_MAR	varchar(500)Collate DATABASE_DEFAULT NULL,
	MARCA	varchar(700)Collate DATABASE_DEFAULT NULL,
	FCH_EMBARQ	varchar(150)Collate DATABASE_DEFAULT NULL,
	PAIS	varchar(180)Collate DATABASE_DEFAULT NULL,
	PUERTO	varchar(180)Collate DATABASE_DEFAULT NULL,
	TIPO_SC varchar(150)Collate DATABASE_DEFAULT NULL,
	TIPO_ST	varchar(150)Collate DATABASE_DEFAULT NULL,
	TALLA	varchar(150)Collate DATABASE_DEFAULT NULL,
	LBRS_NETA decimal(18,6),
	VALOR_NETO decimal(18,6),
	SUBPARTIDA	varchar(250)Collate DATABASE_DEFAULT NULL,
	UNIDAD	varchar(250)Collate DATABASE_DEFAULT NULL,
	CodigoMaster	varchar(150)Collate DATABASE_DEFAULT NULL,
	OBSERVACIÓN	varchar(850)Collate DATABASE_DEFAULT NULL)

	select top 1 * into #tempVista from vieFacturasComercialMatrizReport
	delete #tempVista

	Set @Cad = ""  
	Set @Cad = @Cad + "Insert Into #tempVista" + Char(13)    
	Set @Cad = @Cad + "Select * from vieFacturasComercialMatrizReport" + Char(13) 
	If @fechaEmisionInicio <> '' and  @fechaEmisionFinal <> ''
		Set @Cad = @Cad + "Where Convert(Varchar,Fecha_Emision,112) >= '" + Convert(Varchar,@fechaEmisionInicio,112) + "' And Convert(Varchar,Fecha_Emision,112) <= '" + Convert(Varchar,@fechaEmisionFinal,112) + "'" + Char(13)
	If @fechaEmisionInicio <> '' and  @fechaEmisionFinal = ''
		Set @Cad = @Cad + "Where Convert(Varchar,Fecha_Emision,112) >= '" + Convert(Varchar,@fechaEmisionInicio,112) + "'" + Char(13)	 
	If @fechaEmisionInicio = '' and  @fechaEmisionFinal <> ''
		Set @Cad = @Cad + "Where Convert(Varchar,Fecha_Emision,112) <= '" + Convert(Varchar,@fechaEmisionFinal,112) + "'" + Char(13)
	Set @Cad = @Cad + "Order by NRO_SECUEN,Fecha_Emision" + Char(13)						 
	EXEC (@Cad) 

print 'error'

---------------- creo la sumatoria por cada diferenete secuencia
	Insert Into #tempSumas
	select	'Total Factura' as TotalFactura, vfmr.Nro_secuen as Secuencia
			,Isnull(sum(vfmr.[CARTONES]),0) as 'TotalCartones'
			--,Isnull(sum(vfmr.[PES_BRU_LB]),0) as 'TotalPesoBrutoLibras'
			--,Isnull(sum(vfmr.[PES_BRU_KL]),0) as 'TotalPesoBrutoKilos'
			,Isnull(sum(vfmr.[PES_BRU_CRT]),0) as 'TotalPesoBrutoCRT'
			,Isnull(sum(vfmr.[LIBRAS]),0) as 'TotalLibras'
			,Isnull(sum(vfmr.[KILOS]),0) as 'TotalKilos'
			,Isnull(sum(vfmr.[PES_NET_LB]),0) as 'TotalPesoNetoLibras'
			--,Isnull(sum(vfmr.[PRECIO_LB]),0) as 'TotalPrecioLibras'
			--,Isnull(sum(vfmr.[PRECIO_KL]),0) as 'TotalPrecioKilo'
			--,Isnull(sum(vfmr.[PRE_CONV_LB]),0) as 'TotalPrecioConvLibras'
			--,Isnull(sum(vfmr.[PRE_UNI]),0) as 'TotalPrecioUnitario'
			,Isnull(sum(vfmr.[LBRS_NETA]),0) as 'TotalLibrasNetas'
			,Isnull(sum(vfmr.[VALOR_NETO]),0) as 'TotalValorNeto'
	from #tempVista vfmr
	GROUP BY (Nro_secuen)

	----------------- suma total de totales ------------------------
	Insert Into #tempSumas
	select	'Total General' as TotalFactura, '999999999' as Secuencia
			,Isnull(sum(vfmr.[CARTONES]),0) as 'TotalCartones'
			,Isnull(sum(vfmr.[PES_BRU_CRT]),0) as 'TotalPesoBrutoCRT'
			,Isnull(sum(vfmr.[LIBRAS]),0) as 'TotalLibras'
			,Isnull(SUM(vfmr.[KILOS]),0) as 'TotalKilos'
			,Isnull(SUM(vfmr.[PES_NET_LB]),0) as 'TotalPesoNetoLibras'
			,Isnull(SUM(vfmr.[LBRS_NETA]),0) as 'TotalLibrasNetas'
			,Isnull(SUM(vfmr.[VALOR_NETO]),0) as 'TotalValorNeto'
	from #tempVista vfmr

	--------- inserto los datos para relacionarlos
	insert into #temp 
	select 'Total Factura','','','','','','','',Secuencia,'','',TotalCartones,0,0,TotalPesoBrutoCRT,TotalLibras,TotalKilos,
	TotalPesoNetoLibras,0,0,0,0,'','','','','','','','','','',TotalLibrasNetas,
	TotalValorNeto,'','','ZZZZZZZZ','' 
	from #tempSumas Suma

	Update #temp set REFRENDO = 'Total General' where NRO_SECUEN = '999999999'

	select top 1 '' orden,* into #MatrizFinal from #temp where 1=1
	delete #MatrizFinal

	--print 'error'

	Insert Into #MatrizFinal
	select 2 as orden, * from #temp where REFRENDO <> 'Total General'
	Union
	select 3 as orden, * from #temp where REFRENDO = 'Total General'
	union
	select 1 as orden,*  from #tempVista
	order by Nro_secuen, orden, CodigoMaster

	--spc_FacturasComercialMatrizReport '2023/03/01','2023/03/31'

	Select REFRENDO,DAU,FUE,FCH_FUE, RAZ_SOCIAL, CONSIGNATARIO, PAIS_ORIGE,NRO_SERIE,Case When NRO_SERIE = '' Then '' else NRO_SECUEN End NRO_SECUEN,
	Fecha_Emision,NRO_AUTORI,CARTONES,Case When NRO_SERIE = '' Then NULL else PES_BRU_LB End PES_BRU_LB,
	Case When NRO_SERIE = '' Then NULL else PES_BRU_KL End PES_BRU_KL,	PES_BRU_CRT,LIBRAS,	KILOS,PES_NET_LB,
	Case When NRO_SERIE = '' Then NULL else PRECIO_LB End PRECIO_LB,	
	Case When NRO_SERIE = '' Then NULL else PRECIO_KL End PRECIO_KL,	
	Case When NRO_SERIE = '' Then NULL else PRE_CONV_LB End PRE_CONV_LB,	
	Case When NRO_SERIE = '' Then NULL else PRE_UNI End PRE_UNI,	
	PRECIO_REF,COD_PRODUCTO ,NOMBRE_MAR,MARCA,FCH_EMBARQ,pais,PUERTO,
	TIPO_SC,TIPO_ST,TALLA,LBRS_NETA,VALOR_NETO,SUBPARTIDA,UNIDAD,[OBSERVACIÓN]
	from #MatrizFinal 
	--where NRO_SECUEN ='000005290'


