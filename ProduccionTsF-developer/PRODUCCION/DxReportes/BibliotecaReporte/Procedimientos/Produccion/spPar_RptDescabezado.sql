/****** Object:  StoredProcedure [dbo].[Rpt_Descabezado]    Script Date: 02/02/2023 10:54:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE OR ALTER PROCEDURE [dbo].[spPar_RptDescabezado]
@fechaInicio datetime,
@fechafin datetime
As      

Set NoCount On   
DECLARE @cad    VARCHAR(8000)  
print 'ooooo'
   
SET @fechaInicio = CONVERT(VARCHAR,@fechaInicio,112) 
SET @fechafin = CONVERT(VARCHAR,@fechafin,112) 

print'PPPP'

CREATE TABLE #Document(
		id	INT
	)

Create Table #Agrupacion (
	SecuenciaAgrupacion int,
	FechaIngreso datetime,
	Turno int,
	SumaRechazo Decimal(14,6),
	SumaDirecto Decimal(14,6))


	SET @Cad = ""  
		SET @Cad = @Cad + "Insert Into #Document" + CHAR(13)    
		SET @Cad = @Cad + "Select dc.id FROM DOCUMENT DC" + CHAR(13) 
		SET @Cad = @Cad + "Inner Join Headless HA" + CHAR(13) 
		SET @Cad = @Cad + "   On HA.id = DC.ID" + CHAR(13) 
		If @fechaInicio <> '' and  @fechafin <> ''
			SET @Cad = @Cad + "Where CONVERT(VARCHAR,emissionDate,112) >= '" + CONVERT(VARCHAR,@fechaInicio,112) + "' And CONVERT(VARCHAR,emissionDate,112) <= '" + CONVERT(VARCHAR,@fechafin,112) + "'" + CHAR(13)
		If @fechaInicio <> '' and  @fechafin = ''
			SET @Cad = @Cad + "Where CONVERT(VARCHAR,emissionDate,112) >= '" + CONVERT(VARCHAR,@fechaInicio,112) + "'" + CHAR(13)	 
		If @fechaInicio = '' and  @fechafin <> ''
			SET @Cad = @Cad + "Where CONVERT(VARCHAR,emissionDate,112) <= '" + CONVERT(VARCHAR,@fechafin,112) + "'" + CHAR(13)					 
		EXEC (@Cad) 

		

	Insert Into #Agrupacion
	Select ltrim(ROW_NUMBER() OVER(order BY DC.emissionDate)) SecuenciaAgrupacion, 
	DC.emissionDate, HE.id_turn , 
	sum(Isnull(HE.lbsWholeSurplus,0))SumaRechazo,
	sum(Isnull(He.lbsDirect,0)) SumaDirecto
	from Headless he
	Inner Join Document dc
	   On dc.id = he.id
	Inner Join #Document Tp
	   On Tp.id = Dc.id
	Group by DC.emissionDate, HE.id_turn

	Select 
	DC.emissionDate as FechaIngreso,  
	TR.[name] as Turno,
	PS.fullname_businessName as Proveedor,
	PUP.[name] as Piscina,
	PL.internalNumber as Lote,
	HE.grammage as Gramage,
	CO.[name] as Color,
	Isnull(HE.lbsWholeSurplus,0) as Rechazo,
	Isnull(He.lbsDirect,0) as Directo,
	HE.noOfDrawers as Kavetas,
	HE.noOfPeople as NPersonas,
	He.dateStartTime as HoraInicio,
	He.endDateTime as HoraFin,
	He.manualPerformance as RendimientoManual,
	DC.Number + ' / ' + DS.[name] + ' / ' + DC.[description] as Observacion,
	AG.SecuenciaAgrupacion as SecuenciaAgrupacion,
	AG.SumaRechazo as SumaRechazo,
	AG.SumaDirecto as SumaDirecto,
	ltrim(ROW_NUMBER() OVER(PARTITION BY AG.SecuenciaAgrupacion ORDER BY He.dateStartTime)) as Contador,
	logo.logo2 as Logo
	from Headless he
	Inner Join Document dc
	   On dc.id = he.id
	Inner Join DocumentState DS
	   On DS.id = DC.id_documentState
	Inner Join turn tr
	   On tr.id = id_turn
	Inner Join ItemColor co
	   On co.id = he.id_color
	Inner Join ProductionLot pl
	   On pl.id = he.id_productionLot
	Inner join Person ps 
	   on PS.id = PL.id_provider  
	Inner join ProductionUnitProviderPool PUP 
	   On PUP.id = PL.id_productionUnitProviderPool 
	Inner Join #Agrupacion AG
	   On AG.FechaIngreso = DC.emissionDate
	  And AG.Turno = HE.id_turn 
	Left Outer Join (SELECT TOP(1) ep.id, C.logo2 From Document d
		inner join EmissionPoint ep on d.id_emissionPoint = ep.id
		inner join Company c on c.id = ep.id_company) logo
	   On Logo.id = DC.id_emissionPoint
	Order by He.dateStartTime      
