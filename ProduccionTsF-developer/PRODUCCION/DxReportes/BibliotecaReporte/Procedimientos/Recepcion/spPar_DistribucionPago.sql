/****** Object:  StoredProcedure [dbo].[spPar_DistribucionPago]    Script Date: 01/06/2023 09:52:51 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Query-SP Original: par_ProductionLotPaymentDistributed
CREATE OR ALTER   procedure [dbo].[spPar_DistribucionPago]
@id int
as 
select [PLP].[id],
       [PL].[internalNumber] as [Lote],
       [PL].[number] AS [SecTransaccional],
       Pup.name as Camaronera,
	   Pupp.name as [Piscina],
	   Person.fullname_businessName as [NombreProveedor],
	   
	    Pl.wholeSubtotalAdjustProcess As RendimientoEnteroSinDistribuir,
	    Pl.subtotalTailAdjust As RendimientoColaSinDistribuir,
		Isnull(Pl.subtotalTailAdjust,0) +  Isnull(Pl.wholeSubtotalAdjustProcess,0) As RendimientoTotalSinDistribuir,
		Pl.wholeTotalToPay As TotalEnteroSinDistribuir,
		Pl.tailTotalToPay As TotalColaSinDistribuir,
		Isnull(Pl.wholeTotalToPay,0) +  Isnull(Pl.tailTotalToPay,0) As TotalSinDistribuir,
		Round(Dst1.performance,2) As RendimientoEnteroConDistribucion,
		Round(Dst2.performance,2) As RendimientoColaConDistribucion,
		Round(Dst1.performance,2) + Round(Dst2.performance,2) As RendimientoTotalConDistribucion,
		Round(Dst1.totalPayLp,2) As TotalEnteroConDistribucion,
		Round(Dst2.totalPayLp,2) As TotalColaConDistribucion,
		Round(Dst1.totalPayLp,2) +  Round(Dst2.totalPayLp,2) As TotalConDistribucion,
	 --   Item.name As NombreProducto,
		--Itc.name AS Clase, 
		--Iz.description As Talla,
		--Itp.name As ProcesoDetalle,
		--PLP.totalProcessMetricUnit as RendimientoTotal,
		--Mu.code As UM,
		--PLP.price as PrecioUnitario,
  --      PLP.totalToPay as ValorTotal,
	    [Person].identification_number as identity_prov,
		Company.businessName as Nombre_Cia,
		Company.ruc as Ruc_Cia,
		Company.phoneNumber as Telephone_Cia,
		company.logo2 as Logo,
		PL.sequentialLiquidation AS SecuenciaLiquidacion,
		plst.[name] as [EstadoDocumento], 
		PPL.[processPlant] as [Proceso]
  from [dbo].[ProductionLotPayment]   PLP
  Inner join [dbo].[ProductionLot] [PL] On [PL].[id] = [PLP].[id_productionLot]
  Inner join [dbo].[ProductionLotState] plst On PL.id_ProductionLotState = plst.id
  Inner join [dbo].[Person] PPL On Pl.id_personProcessPlant = PPL.id
  Inner Join Document Dc On Dc.id = PL.id
  Inner Join Item Item On Item.[id] = [PLP].[id_item]
  inner join ItemType Itp On Item.[id_itemType] = Itp.[id]
  Inner Join Itemtypecategory Itc On Itc.[id] = Item.id_itemTypeCategory
  Inner Join Person Person On [Person].[id] = [PL].id_provider
  Inner Join ItemGeneral Ig On Ig.id_item = Item.id
  Inner Join MetricUnit Mu On Mu.id = PLP.id_metricUnitProcess
  Left Join ItemSize Iz On Iz.id = Ig.id_size
  Inner Join ProductionUnitProvider Pup On Pup.id = PL.id_ProductionUnitProvider
  Inner Join ProductionUnitProviderPool Pupp On Pupp.id = [PL].id_productionUnitProviderPool
  Inner Join Document On Document.id = PLP.[id_productionLot]
  Inner Join [dbo].[EmissionPoint] Ems On Ems.[id] = [Document].[id_emissionPoint]
  Inner join [dbo].[Company] Company On Company.[id] = Ems.[id_company]       
  Inner join [dbo].[ProcessType] ptp on PL.[id_processtype] = ptp.[id]
  Left join (Select [PLP].id_productionLot, Sum(performance) performance, Sum(totalPayLp) totalPayLp
				from [dbo].[ProductionLotPayment]   PLP
				  Inner join [dbo].[ProductionLotPaymentDistributed] [PLPD] On [PLPD].[id_ProductionLotPayment] = [PLP].[id]
				  Inner Join Item Item On Item.[id] = [PLPD].[id_item]
				  Inner Join Presentation Pt On Pt.[id] = Item.id_presentation
				  Inner Join MetricUnit Mu On Mu.id = Pt.id_metricUnit
				Where [PLP].id_productionLot = @id And Mu.code = 'Kg'
				Group By [PLP].id_productionLot)Dst1 On Dst1.id_productionLot = PL.id
  Left join (Select [PLP].id_productionLot, Sum(performance) performance, Sum(totalPayLp) totalPayLp
				from [dbo].[ProductionLotPayment]   PLP
				  Inner join [dbo].[ProductionLotPaymentDistributed] [PLPD] On [PLPD].[id_ProductionLotPayment] = [PLP].[id]
				  Inner Join Item Item On Item.[id] = [PLPD].[id_item]
				  Inner Join Presentation Pt On Pt.[id] = Item.id_presentation
				  Inner Join MetricUnit Mu On Mu.id = Pt.id_metricUnit
				Where [PLP].id_productionLot = @id And Mu.code = 'Lbs'
				Group By [PLP].id_productionLot)Dst2 On Dst2.id_productionLot = PL.id
  Left join (Select [PLP].id_productionLot, Sum(performance) performance, Sum(totalPayLp) totalPayLp
				from [dbo].[ProductionLotPayment]   PLP
				  Inner join [dbo].[ProductionLotPaymentDistributed] [PLPD] On [PLPD].[id_ProductionLotPayment] = [PLP].[id]
				  Inner Join Item Item On Item.[id] = [PLPD].[id_item]
				  Inner Join Presentation Pt On Pt.[id] = Item.id_presentation
				  Inner Join MetricUnit Mu On Mu.id = Pt.id_metricUnit
				Where [PLP].id_productionLot = @id 
				Group By [PLP].id_productionLot)Dst3 On Dst3.id_productionLot = PL.id
  Where [PLP].id_productionLot = @id 
	  And Dc.id_documentState <> '05'
  Order by Item.auxCode asc, Iz.[orderSize]

/*
	EXEC spPar_DistribucionPago @id=982718
*/

GO