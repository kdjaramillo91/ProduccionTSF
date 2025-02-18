/****** Object:  StoredProcedure [dbo].[par_ProductionLotPaymentSinDistributedDetail]    Script Date: 01/06/2023 10:07:30 a. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Query - SP Original: par_ProductionLotPaymentSinDistributedDetail
CREATE OR ALTER PROC [dbo].[spPar_DistribucionPagoSinDistribucion]
@id int
as 
select 
		[PLP].[id],
	    Item.name As NombreProducto,
		Itc.name AS Clase, 
		Iz.description As Talla,
		Itp.name As ProcesoDetalle,
		PLP.totalProcessMetricUnit as RendimientoTotal,
		Mu.code As UM,
		PLP.price as PrecioUnitario,
        PLP.totalToPay as ValorTotal
  from [dbo].[ProductionLotPayment]   PLP
  Inner Join Item Item On Item.[id] = PLP.[id_item]
  inner Join ItemType Itp On Item.[id_itemType] = Itp.[id]
  Inner Join Itemtypecategory Itc On Itc.[id] = Item.id_itemTypeCategory
  Inner Join ItemGeneral Ig On Ig.id_item = Item.id
  Inner Join MetricUnit Mu On Mu.id = PLP.id_metricUnitProcess
  Left Join ItemSize Iz On Iz.id = Ig.id_size
 
  Where [PLP].id_productionLot = @id 
  Order by Item.auxCode asc, Iz.[orderSize]

/*
	EXEC spPar_DistribucionPagoSinDistribucion @id=982718
*/


GO